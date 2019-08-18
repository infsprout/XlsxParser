using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Text;
using System.Xml;

namespace XlsxParser.Internal
{

    using DateTime = System.DateTime;

    using HashAlgorithmCreators
        = Dictionary<string, System.Func<HashAlgorithm>>;

    using CipherAlgorithmCreators
        = Dictionary<string, System.Func<SymmetricAlgorithm>>;

    internal sealed class XlsxDecryption
    {
        public const long MaxControlTicks = 100 * 10000; // 1 sec = 1000 * 10000

        private XlsxDecryption() { }

        private static HashAlgorithmCreators
        _hashAlgorithmCreators = new HashAlgorithmCreators {
            {""          , SHA1.Create},
            {"SHA1"      , SHA1.Create},
            {"SHA-1"     , SHA1.Create},
            {"SHA256"    , SHA256.Create},
            {"SHA384"    , SHA384.Create},
            {"SHA512"    , SHA512.Create},
            {"MD5"       , MD5.Create},
            {"MD4"       , ExtraHashAlgorithm.CreateMD4},
            {"MD2"       , ExtraHashAlgorithm.CreateMD2},
            {"RIPEMD-128", ExtraHashAlgorithm.CreateRipeMD128},
            {"RIPEMD-160", ExtraHashAlgorithm.CreateRipeMD160},
            {"WHIRLPOOL" , ExtraHashAlgorithm.CreateWhirlpool},
        };

        private static CipherAlgorithmCreators
        _cipherAlgorithmCreators = new CipherAlgorithmCreators {
            // RC2, RC4, DES, and DESX are not recommended in standard.
            {""        , Rijndael.Create},
            {"AES"     , Rijndael.Create},
            {"3DES"    , TripleDES.Create},
            {"3DES_112", TripleDES.Create},
        };

        internal class Result
        {
            public string error;
            public byte[] bytes;
        }

        internal static bool IsEncryptedFile(Stream stream)
        {
            if (!_IsCompoundFileFormat(stream)) {
                return false;
            }
            var cfi = new _CompoundFileInfo(stream);
            var ei = _ReadEncryptionInfo(cfi);
            if (ei == null) {
                return false;
            }
            return true;
        }

        internal static IEnumerator Decrypt(
            Stream stream, string password, Result result)
        {
            var startTicks = DateTime.Now.Ticks;
            System.Func<bool> keepControl = () => {
                var ticks = DateTime.Now.Ticks;
                if (startTicks < 0) {
                    startTicks = ticks;
                }
                if (ticks - startTicks < MaxControlTicks) {
                    return true;
                }
                startTicks = -1;
                return false;
            };

            if (!_IsCompoundFileFormat(stream)) {
                result.error = "Invalid file format.";
                yield break;
            }
            var cfi = new _CompoundFileInfo(stream);
            var ei = _ReadEncryptionInfo(cfi);
            if (ei == null) {
                result.error = "EncryptionInfo not found.";
                yield break;
            }
            result.error = _VerifyEncryptionInfo(ei);
            if (!string.IsNullOrEmpty(result.error)) {
                yield break;
            }
            yield return null;
            var hAlg = _CreateHashAlgorithmForKey(ei);
            byte[] hFinalPrev = null;
            foreach (var h in _GenerateHFinalPrev(hAlg, ei, password)) {
                hFinalPrev = h;
                if (!keepControl()) {
                    yield return null;
                }
            }
            yield return null;
            var isSuccess = _VerifyEncryptionKey(hAlg, ei, hFinalPrev);
            if (!isSuccess) {
                result.error = "Password incorrect.";
                yield break;
            }
            var ep = _ReadEncryptedPackage(cfi);
            yield return null;
            var ek = _MakeDataEncryptionKey(hAlg, ei, hFinalPrev);
            var bytes = new byte[ep.Length - 8];
            var dec = _DecryptPackage(ei, ek, ep, bytes, 0).GetEnumerator();
            while (dec.MoveNext()) {
                if (!keepControl()) {
                    yield return null;
                }
            }
            var streamSize = _ToI64(ep, 0);
            System.Array.Resize(ref bytes, (int)streamSize);
            result.bytes = bytes;
        }

        private sealed class _CompoundFileInfo
        {
            public const uint MaxSectorNumber = 0xFFFFFFFA;
            public const uint DifatEntryCountInHeader = 109;
            public const uint DirectoryEntrySize = 128;

            public Stream stream { get; private set; }
            public uint sectorSize { get; private set; }
            public uint miniStreamSize { get; private set; }
            public uint miniStreamSizeCutoff { get; private set; }

            public uint firstDirectorySectorId { get; private set; }
            public uint firstMiniFatSectorId { get; private set; }
            public uint firstMiniStreamSectorId { get; private set; }

            public uint difatEntryCountPerSector { get; private set; }
            public uint fatEntryCountPerSector { get; private set; }
            public uint directoryEntryCountPerSector { get; private set; }
            public uint miniStreamCountPerSector { get; private set; }

            public IList<uint> difatOffsetChain { get; private set; }

            public _CompoundFileInfo(Stream stream)
            {
                this.stream = stream;
                sectorSize = 1U << _ToU16(_ReadBytes(stream, 0x001E, 2));
                miniStreamSize = 1U << _ToU16(_ReadBytes(stream, 0x0020, 2));
                miniStreamSizeCutoff = _ToU32(_ReadBytes(stream, 0x0038, 4));

                firstDirectorySectorId = _ToU32(_ReadBytes(stream, 0x0030, 4));
                firstMiniFatSectorId = _ToU32(_ReadBytes(stream, 0x003C, 4));
                firstMiniStreamSectorId = _GetFirstMiniStreamSectorId(this);

                difatEntryCountPerSector = (sectorSize / 4) - 1;
                fatEntryCountPerSector = (sectorSize / 4);
                directoryEntryCountPerSector = (sectorSize / DirectoryEntrySize);
                miniStreamCountPerSector = (sectorSize / miniStreamSize);

                difatOffsetChain = _MakeDifatOffsetChain(this).AsReadOnly();
            }
        }

        private sealed class _EncryptionInfo
        {
            public byte[] bytes { get; private set; }
            public XmlDocument descryptor { get; private set; }

            public ushort majorVersion { get; private set; }
            public ushort minorVersion { get; private set; }

            public bool isStandardEncryption { get; private set; }
            public bool isExtensibleEncryption { get; private set; }
            public bool isAgileEncryption { get; private set; }

            public uint verifierOffset { get; private set; }
            public uint keyBits { get; private set; }

            public XmlElement encryptedKeyNode { get; private set; }
            public XmlElement keyDataNode { get; private set; }

            public _EncryptionInfo(byte[] bytes)
            {
                var major = _ToU16(bytes, 0);
                var minor = _ToU16(bytes, 2);
                this.bytes = bytes;
                majorVersion = major;
                minorVersion = minor;
                isStandardEncryption
                    = (major >= 2 && major <= 4 && minor == 2);
                isExtensibleEncryption
                    = (major >= 3 && major <= 4 && minor == 3);
                isAgileEncryption
                    = (major == 4 && minor == 4);
                if (isStandardEncryption) {
                    var headerSize = _ToU32(bytes, 8);
                    verifierOffset = 12 + headerSize;
                    keyBits = _ToU32(bytes, 28);
                } else if (isAgileEncryption) {
                    var sz = bytes.Length - 8;
                    using (var ms = new MemoryStream(bytes, 8, sz)) {
                        descryptor = new XmlDocument();
                        descryptor.Load(ms);
                    }
                    XmlNodeList xnl = null;
                    xnl = descryptor.GetElementsByTagName("p:encryptedKey");
                    encryptedKeyNode = (XmlElement)xnl[0];
                    xnl = descryptor.GetElementsByTagName("keyData");
                    keyDataNode = (XmlElement)xnl[0];
                }
            }
        }

        #region Stream private methods

        private static byte[] _ReadBytes(
            Stream stream, long position, int count)
        {
            stream.Position = position;
            return _ReadBytes(stream, count);
        }

        private static byte[] _ReadBytes(Stream stream, int count)
        {
            var bytes = new byte[count];
            var remaining = bytes.Length;
            var offset = 0;
            do {
                var n = stream.Read(bytes, offset, remaining);
                if (n == 0) {
                    break;
                }
                remaining -= n;
                offset += n;
            } while (remaining > 0);
            return bytes;
        }

        private static byte[] _ToBytes(uint v)
        {
            var bytes = new byte[sizeof(uint)];
            bytes[0] = (byte)(v >>  0);
            bytes[1] = (byte)(v >>  8);
            bytes[2] = (byte)(v >> 16);
            bytes[3] = (byte)(v >> 24);
            return bytes;
        }

        private static ushort _ToU16(byte[] bytes, int startIndex = 0)
        {
            var n = startIndex;
            var v
            = (bytes[n + 0] << 0) | (bytes[n + 1] << 8);
            return (ushort)v;
        }

        private static uint _ToU32(byte[] bytes, int startIndex = 0)
        {
            var n = startIndex;
            var v
            = (bytes[n + 0] <<  0) | (bytes[n + 1] <<  8)
            | (bytes[n + 2] << 16) | (bytes[n + 3] << 24);
            return (uint)v;
        }

        private static long _ToI64(byte[] bytes, int startIndex = 0)
        {
            var n = startIndex;
            var v1
            = (bytes[n + 0] <<  0) | (bytes[n + 1] <<  8)
            | (bytes[n + 2] << 16) | (bytes[n + 3] << 24);
            var v2
            = (bytes[n + 4] <<  0) | (bytes[n + 5] <<  8)
            | (bytes[n + 6] << 16) | (bytes[n + 7] << 24);
            return (long)((uint)v1 | ((ulong)v2 << 32));
        }

        #endregion

        #region CompoundFile private methods

        private static bool _IsCompoundFileFormat(Stream stream)
        {
            var signature = new byte[] {
            0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1
        };
            var bytes = _ReadBytes(stream, 0, signature.Length);
            var isSuccess = true;
            for (var n = 0; n < signature.Length; ++n) {
                if (signature[n] != bytes[n]) {
                    isSuccess = false;
                    break;
                }
            }
            stream.Position = 0;
            return isSuccess;
        }

        private static bool _IsRegulerSectorId(uint sectorId)
        {
            return sectorId <= _CompoundFileInfo.MaxSectorNumber;
        }

        private static uint _GetFirstMiniStreamSectorId(_CompoundFileInfo cfi)
        {
            var rootEntryOffset
            = (cfi.firstDirectorySectorId + 1) * cfi.sectorSize;
            return _ToU32(_ReadBytes(cfi.stream, rootEntryOffset + 0x0074, 4));
        }

        private static List<uint> _MakeDifatOffsetChain(_CompoundFileInfo cfi)
        {
            var chain = new List<uint>();
            chain.Add(0x004C);
            var next = _ToU32(_ReadBytes(cfi.stream, 0x0044, 4));
            while (_IsRegulerSectorId(next)) {
                var offset = (next + 1) * cfi.sectorSize;
                chain.Add(offset);
                next = _ToU32(_ReadBytes(
                    cfi.stream, offset + (cfi.sectorSize - 4), 4
                ));
            }
            return chain;
        }

        private static uint _FindFatSectorOffset(
            _CompoundFileInfo cfi, uint sectorId)
        {
            var decih = _CompoundFileInfo.DifatEntryCountInHeader;
            var decps = cfi.difatEntryCountPerSector;
            var difatEntryIndex = sectorId / cfi.fatEntryCountPerSector;
            uint n = 0;
            uint m = 0;
            if (difatEntryIndex < decih) {
                m = 4 * difatEntryIndex;
            } else {
                n = 1 + ((difatEntryIndex - decih) / decps);
                m = 4 * ((difatEntryIndex - decih) % decps);
            }
            var pos = cfi.difatOffsetChain[(int)n] + m;
            var fatSectorId = _ToU32(_ReadBytes(cfi.stream, pos, 4));
            return (fatSectorId + 1) * cfi.sectorSize;
        }

        private static List<uint> _MakeSectorOffsetChain(
            _CompoundFileInfo cfi, uint startSectorId)
        {
            var chain = new List<uint>();
            if (!_IsRegulerSectorId(startSectorId)) {
                return chain;
            }
            var next = startSectorId;
            do {
                var offset = (next + 1) * cfi.sectorSize;
                chain.Add(offset);
                var fatOffset = _FindFatSectorOffset(cfi, next);
                var n = 4 * (next % cfi.fatEntryCountPerSector);
                next = _ToU32(_ReadBytes(cfi.stream, fatOffset + n, 4));
            } while (_IsRegulerSectorId(next));
            return chain;
        }

        private static List<uint> _MakeMiniSectorOffsetChain(
            _CompoundFileInfo cfi, uint startMiniSectorId)
        {
            var chain = new List<uint>();
            if (!_IsRegulerSectorId(startMiniSectorId)) {
                return chain;
            }

            var miniFatSoc = _MakeSectorOffsetChain(
                cfi, cfi.firstMiniFatSectorId
            );
            var miniStreamSoc = _MakeSectorOffsetChain(
                cfi, cfi.firstMiniStreamSectorId
            );

            var next = startMiniSectorId;
            do {
                var n = next / cfi.miniStreamCountPerSector;
                var m = next % cfi.miniStreamCountPerSector;
                var offset = miniStreamSoc[(int)n] + (m * cfi.miniStreamSize);
                chain.Add(offset);
                var a = next / cfi.fatEntryCountPerSector;
                var b = next % cfi.fatEntryCountPerSector;
                var miniFatOffset = miniFatSoc[(int)a] + (b * 4);
                next = _ToU32(_ReadBytes(cfi.stream, miniFatOffset, 4));
            } while (_IsRegulerSectorId(next));
            return chain;
        }

        private static string _ReadNullTerminatedUnicodeString(byte[] bytes)
        {
            var count = bytes.Length;
            for (var n = 0; n < bytes.Length; n += 2) {
                if (bytes[n] == 0 && bytes[n + 1] == 0) {
                    count = n;
                    break;
                }
            }
            return Encoding.Unicode.GetString(bytes, 0, count);
        }

        private static uint _FindDirectoryEntryOffset(
            _CompoundFileInfo cfi, string entryName)
        {
            var startSectorId = cfi.firstDirectorySectorId;
            var chain = _MakeSectorOffsetChain(cfi, startSectorId);
            var mCount = cfi.directoryEntryCountPerSector;
            var deSize = _CompoundFileInfo.DirectoryEntrySize;
            for (uint n = 0; n < chain.Count; ++n) {
                for (uint m = 0; m < mCount; ++m) {
                    var offset = chain[(int)n] + (m * deSize);
                    var bytes = _ReadBytes(cfi.stream, offset, 64);
                    var name = _ReadNullTerminatedUnicodeString(bytes);
                    if (name == entryName) {
                        return offset;
                    }
                }
            }
            return 0;
        }

        private static byte[] _ReadUserDefinedData(
            _CompoundFileInfo cfi, uint entryOffset)
        {
            var eo = entryOffset;
            var startSectorId = _ToU32(_ReadBytes(cfi.stream, eo + 0x0074, 4));
            var streamSize = _ToI64(_ReadBytes(cfi.stream, eo + 0x0078, 8));
            uint sectorSize;
            List<uint> chain;
            if (streamSize >= cfi.miniStreamSizeCutoff) {
                sectorSize = cfi.sectorSize;
                chain = _MakeSectorOffsetChain(cfi, startSectorId);
            } else {
                sectorSize = cfi.miniStreamSize;
                chain = _MakeMiniSectorOffsetChain(cfi, startSectorId);
            }
            var bytes = new byte[streamSize];
            for (var n = 0; n < chain.Count; ++n) {
                var srcOffset = chain[n];
                var dstOffset = n * sectorSize;
                var count = System.Math.Min(sectorSize, streamSize - dstOffset);
                cfi.stream.Position = srcOffset;
                cfi.stream.Read(bytes, (int)dstOffset, (int)count);
            }
            return bytes;
        }

        private static _EncryptionInfo _ReadEncryptionInfo(
            _CompoundFileInfo cfi)
        {
            var offset = _FindDirectoryEntryOffset(cfi, "EncryptionInfo");
            if (offset == 0) {
                return null;
            }
            var bytes = _ReadUserDefinedData(cfi, offset);
            return new _EncryptionInfo(bytes);
        }

        private static byte[] _ReadEncryptedPackage(_CompoundFileInfo cfi)
        {
            var offset = _FindDirectoryEntryOffset(cfi, "EncryptedPackage");
            if (offset == 0) {
                return null;
            }
            return _ReadUserDefinedData(cfi, offset);
        }

        #endregion

        #region Crypto private methods

        private static string _VerifyEncryptionInfo(_EncryptionInfo ei)
        {
            if (ei.isExtensibleEncryption) {
                return "Extensible encryption is not supported.";
            }
            if (!ei.isStandardEncryption && !ei.isAgileEncryption) {
                return "Unknown encryption version.";
            }
            if (ei.isStandardEncryption) {
                var cAlgId = _ToU32(ei.bytes, 20);
                var hAlgId = _ToU32(ei.bytes, 24);
                if (cAlgId != 0x660E && cAlgId != 0x660F && cAlgId != 0x6610) {
                    return string.Format(
                        "'{0:X8}' is invalid 'AlgID' value.", cAlgId
                    );
                }
                if (hAlgId != 0x8004) {
                    return string.Format(
                        "'{0:X8}' is invalid 'AlgIDHash' value.", hAlgId
                    );
                }
                return null;
            }
            XmlElement node = null;
            string algName = null;
            System.Func<string> algNameError = () => {
                return string.Format(
                    "<{0}/> '{1}' is not supported.", node.Name, algName
                );
            };
            node = ei.encryptedKeyNode;
            algName = node.GetAttribute("hashAlgorithm");
            if (!_hashAlgorithmCreators.ContainsKey(algName)) {
                return algNameError();
            }
            algName = node.GetAttribute("cipherAlgorithm");
            if (!_cipherAlgorithmCreators.ContainsKey(algName)) {
                return algNameError();
            }
            node = ei.keyDataNode;
            algName = node.GetAttribute("hashAlgorithm");
            if (!_hashAlgorithmCreators.ContainsKey(algName)) {
                return algNameError();
            }
            algName = node.GetAttribute("cipherAlgorithm");
            if (!_cipherAlgorithmCreators.ContainsKey(algName)) {
                return algNameError();
            }
            return null;
        }

        private static
        HashAlgorithm _CreateHashAlgorithm(string name = null)
        {
            name = name ?? "";
            System.Func<HashAlgorithm> creator;
            if (_hashAlgorithmCreators.TryGetValue(name, out creator)) {
                return creator();
            }
            return _hashAlgorithmCreators[""]();
        }

        private static
        HashAlgorithm _CreateHashAlgorithmForKey(_EncryptionInfo ei)
        {
            if (ei.isStandardEncryption) {
                return _CreateHashAlgorithm();
            }
            var node = ei.encryptedKeyNode;
            var name = node.GetAttribute("hashAlgorithm");
            return _CreateHashAlgorithm(name);
        }

        private static
        HashAlgorithm _CreateHashAlgorithmForData(_EncryptionInfo ei)
        {
            if (ei.isStandardEncryption) {
                return _CreateHashAlgorithm();
            }
            var node = ei.keyDataNode;
            var name = node.GetAttribute("hashAlgorithm");
            return _CreateHashAlgorithm(name);
        }

        private static
        SymmetricAlgorithm _CreateCipherAlgorithm(string name = null)
        {
            name = name ?? "";
            System.Func<SymmetricAlgorithm> creator;
            if (_cipherAlgorithmCreators.TryGetValue(name, out creator)) {
                return creator();
            }
            return _cipherAlgorithmCreators[""]();
        }

        private static
        SymmetricAlgorithm _CreateCipherAlgorithm(XmlElement node)
        {
            var name = node.GetAttribute("cipherAlgorithm");
            var cAlg = _CreateCipherAlgorithm(name);
            var blockSize = int.Parse(node.GetAttribute("blockSize"));
            cAlg.KeySize = int.Parse(node.GetAttribute("keyBits"));
            cAlg.BlockSize = blockSize * 8;
            cAlg.Padding = PaddingMode.None;
            switch (node.GetAttribute("cipherChaining")) {
            case "ChainingModeCBC": cAlg.Mode = CipherMode.CBC; break;
            case "ChainingModeCFB": cAlg.Mode = CipherMode.CFB; break;
            }
            return cAlg;
        }

        private static byte[] _MakeIv(XmlElement node, byte[] blockKey = null)
        {
            var blockSize = int.Parse(node.GetAttribute("blockSize"));
            var hasBlockKey = (blockKey != null && blockKey.Length > 0);
            var salt = System.Convert.FromBase64String(
                node.GetAttribute("saltValue")
            );
            var iv = salt;
            if (hasBlockKey) {
                var hAlg = _CreateHashAlgorithm(
                    node.GetAttribute("hashAlgorithm")
                );
                iv = _H(hAlg, salt, blockKey);
                hAlg.Clear();
            }
            var n = iv.Length;
            System.Array.Resize(ref iv, blockSize);
            for (; n < iv.Length; ++n) { iv[n] = 0x36; }
            return iv;
        }

        private static
        SymmetricAlgorithm _CreateCipherAlgorithmForKey(_EncryptionInfo ei)
        {
            if (ei.isStandardEncryption) {
                return _CreateCipherAlgorithmForData(ei);
            }
            return _CreateCipherAlgorithm(ei.encryptedKeyNode);
        }

        private static
        SymmetricAlgorithm _CreateCipherAlgorithmForData(_EncryptionInfo ei)
        {
            SymmetricAlgorithm cAlg = null;
            if (ei.isStandardEncryption) {
                cAlg = _CreateCipherAlgorithm();
                cAlg.KeySize = (int)ei.keyBits;
                cAlg.Mode = CipherMode.ECB;
                cAlg.Padding = PaddingMode.None;
                return cAlg;
            }
            return _CreateCipherAlgorithm(ei.keyDataNode);
        }

        private static byte[] _MakeDataEncryptionKey(
            HashAlgorithm hAlg, _EncryptionInfo ei, byte[] hFinalPrev)
        {
            byte[] bk;
            if (ei.isStandardEncryption) {
                bk = _ToBytes(0);
                return _MakeStandardEncryptionKey(hAlg, ei, hFinalPrev, bk);
            }
            bk = new byte[] { 0x14, 0x6E, 0x0B, 0xE7, 0xAB, 0xAC, 0xD0, 0xD6 };
            var ek = _MakeEncryptionKey(hAlg, ei, hFinalPrev, bk);
            var node = ei.encryptedKeyNode;
            var ekv = System.Convert.FromBase64String(
                node.GetAttribute("encryptedKeyValue")
            );
            var cAlg = _CreateCipherAlgorithmForKey(ei);
            cAlg.Key = ek;
            cAlg.IV = _MakeIv(node);
            var dkv = new byte[ekv.Length];
            using (var dec = cAlg.CreateDecryptor()) {
                dec.TransformBlock(ekv, 0, dkv.Length, dkv, 0);
            }
            cAlg.Clear();
            return dkv;
        }

        private static byte[] _MakeEncryptionKey(
            HashAlgorithm hAlg, _EncryptionInfo ei,
            byte[] hFinalPrev, byte[] blockKey)
        {
            if (ei.isAgileEncryption) {
                return _MakeAgileEncryptionKey(hAlg, ei, hFinalPrev, blockKey);
            }
            return _MakeStandardEncryptionKey(hAlg, ei, hFinalPrev, blockKey);
        }

        private static byte[] _MakeStandardEncryptionKey(
            HashAlgorithm hAlg, _EncryptionInfo ei,
            byte[] hFinalPrev, byte[] blockKey)
        {
            var hFinal = _H(hAlg, hFinalPrev, blockKey);
            var x1 = _MakeX1(hAlg, hFinal);
            var x2 = _MakeX2(hAlg, hFinal);
            var x3 = _MakeX3(x1, x2);
            var cbRequiredKeyLength = ei.keyBits / 8;
            System.Array.Resize(ref x3, (int)cbRequiredKeyLength);
            return x3;
        }

        private static byte[] _MakeAgileEncryptionKey(
            HashAlgorithm hAlg, _EncryptionInfo ei,
            byte[] hFinalPrev, byte[] blockKey)
        {
            var hFinal = _H(hAlg, hFinalPrev, blockKey);
            var node = ei.encryptedKeyNode;
            var keySize = int.Parse(node.GetAttribute("keyBits")) / 8;
            var n = hFinal.Length;
            System.Array.Resize(ref hFinal, keySize);
            for (; n < hFinal.Length; ++n) {
                hFinal[n] = 0x36;
            }
            return hFinal;
        }

        private static byte[] _H(HashAlgorithm hAlg, byte[] a, byte[] b)
        {
            var bytes = new byte[a.Length + b.Length];
            System.Array.Copy(a, 0, bytes, 0, a.Length);
            System.Array.Copy(b, 0, bytes, a.Length, b.Length);
            return hAlg.ComputeHash(bytes);
        }

        private static IEnumerable<byte[]> _GenerateHFinalPrev(
            HashAlgorithm hAlg, _EncryptionInfo ei, string password)
        {
            byte[] salt;
            uint spinCount;
            if (ei.isAgileEncryption) {
                var node = ei.encryptedKeyNode;
                salt = System.Convert.FromBase64String(
                    node.GetAttribute("saltValue")
                );
                spinCount = uint.Parse(node.GetAttribute("spinCount"));
            } else {
                salt = new byte[16];
                var offset = ei.verifierOffset + 4;
                System.Array.Copy(ei.bytes, offset, salt, 0, salt.Length);
                spinCount = 50000;
            }
            var hInit = _H(hAlg, salt, Encoding.Unicode.GetBytes(password));
            var hSize = hAlg.HashSize / 8;
            var hInput = new byte[4 + hSize];
            var hCurr = hInit;
            for (uint i = 0; i < spinCount; ++i) {
                hInput[0] = (byte)(i >> 0);
                hInput[1] = (byte)(i >> 8);
                hInput[2] = (byte)(i >> 16);
                hInput[3] = (byte)(i >> 24);
                System.Array.Copy(hCurr, 0, hInput, 4, hSize);
                hCurr = hAlg.ComputeHash(hInput);
                yield return null;
            }
            yield return hCurr;
        }

        private static byte[] _MakeX1(HashAlgorithm hAlg, byte[] hFinal)
        {
            var x1 = new byte[] {
                0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36,
                0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36,
                0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36,
                0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36,
                0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36,
                0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36,
                0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36,
                0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36, 0x36,
            };
            for (var n = 0; n < hFinal.Length; ++n) {
                x1[n] ^= hFinal[n];
            }
            return hAlg.ComputeHash(x1);
        }

        private static byte[] _MakeX2(HashAlgorithm hAlg, byte[] hFinal)
        {
            var x2 = new byte[] {
                0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
                0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
                0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
                0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
                0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
                0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
                0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
                0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C, 0x5C,
            };
            for (var n = 0; n < hFinal.Length; ++n) {
                x2[n] ^= hFinal[n];
            }
            return hAlg.ComputeHash(x2);
        }

        private static byte[] _MakeX3(byte[] x1, byte[] x2)
        {
            var x3 = new byte[x1.Length + x2.Length];
            System.Array.Copy(x1, 0, x3, 0, x1.Length);
            System.Array.Copy(x2, 0, x3, x1.Length, x2.Length);
            return x3;
        }

        private static bool _VerifyEncryptionKey(
            HashAlgorithm hAlg, _EncryptionInfo ei, byte[] key)
        {
            if (ei.isAgileEncryption) {
                return _VerifyAgileEncryptionKey(hAlg, ei, key);
            }
            return _VerifyStandardEncryptionKey(hAlg, ei, key);
        }

        private static bool _VerifyStandardEncryptionKey(
            HashAlgorithm hAlg, _EncryptionInfo ei, byte[] hFinalPrev)
        {
            var pos = ei.verifierOffset;
            var evi = new byte[16];
            var evh = new byte[32];
            System.Array.Copy(ei.bytes, pos + 20, evi, 0, evi.Length);
            System.Array.Copy(ei.bytes, pos + 40, evh, 0, evh.Length);
            var dvi = new byte[evi.Length];
            var dvh = new byte[evh.Length];
            var cAlg = _CreateCipherAlgorithmForKey(ei);
            cAlg.Key = _MakeEncryptionKey(hAlg, ei, hFinalPrev, _ToBytes(0));
            using (var dec = cAlg.CreateDecryptor()) {
                dec.TransformBlock(evi, 0, dvi.Length, dvi, 0);
                dec.TransformBlock(evh, 0, dvh.Length, dvh, 0);
            }
            cAlg.Clear();
            var hv = hAlg.ComputeHash(dvi);
            for (var n = 0; n < hv.Length; ++n) {
                if (hv[n] != dvh[n]) {
                    return false;
                }
            }
            return true;
        }

        private static bool _VerifyAgileEncryptionKey(
            HashAlgorithm hAlg, _EncryptionInfo ei, byte[] hFinalPrev)
        {
            var node = ei.encryptedKeyNode;
            var evi = System.Convert.FromBase64String(
                node.GetAttribute("encryptedVerifierHashInput")
            );
            var evh = System.Convert.FromBase64String(
                node.GetAttribute("encryptedVerifierHashValue")
            );
            var cAlg = _CreateCipherAlgorithmForKey(ei);
            cAlg.IV = _MakeIv(node);
            var dvi = new byte[evi.Length];
            var dvh = new byte[evh.Length];
            cAlg.Key = _MakeEncryptionKey(hAlg, ei, hFinalPrev,
                new byte[] { 0xFE, 0xA7, 0xD2, 0x76, 0x3B, 0x4B, 0x9E, 0x79 }
            );
            using (var dec = cAlg.CreateDecryptor()) {
                dec.TransformBlock(evi, 0, dvi.Length, dvi, 0);
            }
            cAlg.Key = _MakeEncryptionKey(hAlg, ei, hFinalPrev,
                new byte[] { 0xD7, 0xAA, 0x0F, 0x6D, 0x30, 0x61, 0x34, 0x4E }
            );
            using (var dec = cAlg.CreateDecryptor()) {
                dec.TransformBlock(evh, 0, dvh.Length, dvh, 0);
            }
            cAlg.Clear();
            var hv = hAlg.ComputeHash(dvi);
            for (var n = 0; n < hv.Length; ++n) {
                if (hv[n] != dvh[n]) {
                    return false;
                }
            }
            return true;
        }

        private static IEnumerable<int> _DecryptPackage(
            _EncryptionInfo ei, byte[] encryptionKey, byte[] encryptedPackage,
            byte[] output, int startSegment, int segmentCount = -1)
        {
            const int SegSz = 4096;
            var src = encryptedPackage;
            var dst = output;
            var cAlg = _CreateCipherAlgorithmForData(ei);
            cAlg.Key = encryptionKey;
            var end = segmentCount * SegSz;
            if (end < 0 || end > dst.Length) {
                end = dst.Length;
            }
            var seg = startSegment;
            for (var n = seg * SegSz; n < end; n += SegSz, ++seg) {
                if (ei.isAgileEncryption) {
                    cAlg.IV = _MakeIv(ei.keyDataNode, _ToBytes((uint)seg));
                }
                var count = (SegSz < end - n) ? SegSz : (end - n);
                var dec = cAlg.CreateDecryptor();
                dec.TransformBlock(src, 8 + n, count, dst, n);
                dec.Dispose();
                yield return count;
            }
            cAlg.Clear();
        }

        #endregion

    }

}
