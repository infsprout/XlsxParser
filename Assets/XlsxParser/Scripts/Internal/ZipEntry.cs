using UnityEngine;
using System.IO;
using System.Text;

namespace XlsxParser.Internal
{
    using Compression;
    /**
     * This class has been implemented minimally for xlsx file format decompress. 
     * Not recommended for general use.
     */
    internal sealed class ZipEntry
    {
        public const uint Signature = 0x02014b50;

        private Stream _stream;

        private long _startOffset;

        private ushort _flags;

        private ushort _compressionMethod;

        private ushort _fileNameLength;

        private ushort _extraFieldLength;

        private ushort _fileCommentLength;

        private long _nextStartOffset;

        private long _zip64InfoOffset;

        private string _fileName = null;
        public string fileName {
            get {
                if (_fileName == null) {
                    _fileName = _GetFileName();
                }
                return _fileName;
            }
        }

        public long compressedSize { get; private set; }

        public long uncompressedSize { get; private set; }

        public long dataOffset { get; private set; }

        public long dataSize {
            get {
                return (_compressionMethod != 0)
                ? compressedSize : uncompressedSize;
            }
        }

        public bool isEncrypted {
            get {
                return (_flags & (1 << 0)) != 0;
            }
        }

        public bool needUnicodeEncoding {
            get {
                return (_flags & (1 << 11)) != 0;
            }
        }

        public bool hasNext {
            get {
                return _startOffset != _nextStartOffset;
            }
        }

        private ZipEntry() { return; }

        public static ZipEntry Create(Stream stream)
        {
            if (stream == null) {
                return null;
            }
            var offset = _FindCentralDirectoryOffset(stream);
            uint signature = 0;
            if (offset > 0) {
                stream.Seek(offset, SeekOrigin.Begin);
                signature = _ReadU32(stream);
            }
            if (signature != Signature) {
                return null;
            }
            return _Create(stream, offset);
        }

        public ZipEntry Next()
        {
            if (!hasNext) {
                return null;
            }
            return _Create(_stream, _nextStartOffset);
        }

        public Stream GetUncompressedStream()
        {
            if (uncompressedSize == 0) {
                return new MemoryStream(new byte[0]);
            }

            if (uncompressedSize > int.MaxValue) {
                throw new System.InvalidOperationException();
            }

            if (isEncrypted) {
                throw new System.NotSupportedException();
            }

            var count = (int)dataSize;
            _stream.Seek(dataOffset, SeekOrigin.Begin);

            if (_compressionMethod == 0) {
                return new MemoryStream(_ReadBytes(_stream, count));
            } else if (_compressionMethod != 8) {
                throw new System.NotSupportedException();
            }

            return new DeflateStream(
                new MemoryStream(_ReadBytes(_stream, count)),
                CompressionMode.Decompress
            );
        }

        public byte[] GetUncompressedBytes()
        {
            using (var stream = GetUncompressedStream()) {
                var reader = new BinaryReader(stream);
                return reader.ReadBytes((int)uncompressedSize);
            }
        }

        public override string ToString()
        {
            return "ZipEntry (" + fileName + ")"
            + "\n  _startOffset: " + _startOffset
            + "\n  _flags: "
            + System.Convert.ToString(_flags, 2)
                .PadLeft(16, '0').Insert(8, " ")
            + "\n  _compressionMethod: " + _compressionMethod
            + "\n  _fileNameLength: " + _fileNameLength
            + "\n  _extraFieldLength: " + _extraFieldLength
            + "\n  _fileCommentLength: " + _fileCommentLength
            + "\n  _nextStartOffset: " + _nextStartOffset
            + "\n  _zip64InfoOffset: " + _zip64InfoOffset
            + "\n  fileName: " + fileName
            + "\n  compressedSize: " + compressedSize
            + "\n  uncompressedSize: " + uncompressedSize
            + "\n  dataOffset: " + dataOffset
            + "\n  needUnicodeEncoding: " + needUnicodeEncoding
            + "\n  hasNext: " + hasNext
            + "\n";
        }

        #region private methods    

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

        private static ushort _ReadU16(Stream stream)
        {
            var bytes = _ReadBytes(stream, 2);
            var v
            = (bytes[0] << 0) | (bytes[1] << 8);
            return (ushort)v;
        }

        private static uint _ReadU32(Stream stream)
        {
            var bytes = _ReadBytes(stream, 4);
            var v
            = (bytes[0] <<  0) | (bytes[1] <<  8)
            | (bytes[2] << 16) | (bytes[3] << 24);
            return (uint)v;
        }

        private static long _ReadI64(Stream stream)
        {
            var bytes = _ReadBytes(stream, 8);
            var v1
            = (bytes[0] <<  0) | (bytes[1] <<  8)
            | (bytes[2] << 16) | (bytes[3] << 24);
            var v2
            = (bytes[4] <<  0) | (bytes[5] <<  8)
            | (bytes[6] << 16) | (bytes[7] << 24);
            return (long)((uint)v1 | ((ulong)v2 << 32));
        }

        private byte[] _ReadBytes(long offset, int count)
        {
            _stream.Seek(_startOffset + offset, SeekOrigin.Begin);
            return _ReadBytes(_stream, count);
        }

        private ushort _ReadU16(long offset)
        {
            _stream.Seek(_startOffset + offset, SeekOrigin.Begin);
            return _ReadU16(_stream);
        }

        private uint _ReadU32(long offset)
        {
            _stream.Seek(_startOffset + offset, SeekOrigin.Begin);
            return _ReadU32(_stream);
        }

        private long _ReadI64(long offset)
        {
            _stream.Seek(_startOffset + offset, SeekOrigin.Begin);
            return _ReadI64(_stream);
        }

        private long _GetExtraFieldDataOffset(ushort id)
        {
            var offset = 0x002e + _fileNameLength;
            var end = offset + _extraFieldLength;
            while (offset < end) {
                if (id == _ReadU16(offset)) {
                    return offset + 0x0004;
                }
                offset += _ReadU16(offset + 0x0002);
            }
            return -1;
        }

        private static long _FindCentralDirectoryOffset(Stream stream)
        {
            var n = sizeof(ushort);
            if (stream.Length <= 0x0016) {
                return -1;
            }

            var end = ushort.MaxValue + sizeof(ushort);
            end = (int)System.Math.Min(end, stream.Length - 1);
            end -= 0x0014;

            for (; n < end; ++n) {
                stream.Seek(-n, SeekOrigin.End);
                var commentLength = _ReadU16(stream);
                if (commentLength == n - sizeof(ushort)) {
                    stream.Seek(-(n + 0x0014), SeekOrigin.End);
                    var signature = _ReadU32(stream);
                    if (signature == 0x06054b50) {
                        stream.Seek(-(n + 0x0004), SeekOrigin.End);
                        return _ReadU32(stream);
                    }
                }
            }
            return -1;
        }

        private static ZipEntry _Create(Stream stream, long offset)
        {
            var inst = new ZipEntry();
            inst._stream = stream;
            inst._startOffset = offset;

            inst._flags             = inst._ReadU16(0x0008);
            inst._compressionMethod = inst._ReadU16(0x000a);
            inst._fileNameLength    = inst._ReadU16(0x001c);
            inst._extraFieldLength  = inst._ReadU16(0x001e);
            inst._fileCommentLength = inst._ReadU16(0x0020);

            inst._nextStartOffset = inst._GetNextStartOffset();
            inst._zip64InfoOffset = inst._GetZip64InfoOffset();
            inst.compressedSize   = inst._GetCompressedSize();
            inst.uncompressedSize = inst._GetUncompressedSize();
            inst.dataOffset       = inst._GetDataOffset();

            return inst;
        }

        private string _GetFileName()
        {
            var bytes = _ReadBytes(0x002e, _fileNameLength);
            if (needUnicodeEncoding) {
                return Encoding.UTF8.GetString(bytes);
            }
            // CP437 is not fully supported in unity3d.
            var sb = new StringBuilder();
            foreach (var b in bytes) {
                sb.Append((char)_cp437ToUnicode[b]);
            }
            return sb.ToString();
        }

        private long _GetNextStartOffset()
        {
            var offset = 0x002e + _fileNameLength
            + _extraFieldLength + _fileCommentLength;

            if (_ReadU32(offset) == Signature) {
                return _startOffset + offset;
            }
            return _startOffset;
        }

        private long _GetZip64InfoOffset()
        {
            return _GetExtraFieldDataOffset(0x0001);
        }

        private long _GetCompressedSize()
        {
            if (_zip64InfoOffset > 0) {
                return _ReadI64(_zip64InfoOffset + 0x0008);
            }
            return _ReadU32(0x0014);
        }

        private long _GetUncompressedSize()
        {
            if (_zip64InfoOffset > 0) {
                return _ReadI64(_zip64InfoOffset + 0x0000);
            }
            return _ReadU32(0x0018);
        }

        private long _GetDataOffset()
        {
            long offset = _ReadU32(0x002a) + 0x001a;
            _stream.Seek(offset, SeekOrigin.Begin);
            offset += _ReadU16(_stream);
            offset += _ReadU16(_stream);
            return offset + 0x0004;
        }

        #endregion
        #region cp437
        private static ushort[] _cp437ToUnicode = {
            0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007,
            0x0008, 0x0009, 0x000A, 0x000B, 0x000C, 0x000D, 0x000E, 0x000F,
            0x0010, 0x0011, 0x0012, 0x0013, 0x0014, 0x0015, 0x0016, 0x0017,
            0x0018, 0x0019, 0x001A, 0x001B, 0x001C, 0x001D, 0x001E, 0x001F,
            0x0020, 0x0021, 0x0022, 0x0023, 0x0024, 0x0025, 0x0026, 0x0027,
            0x0028, 0x0029, 0x002A, 0x002B, 0x002C, 0x002D, 0x002E, 0x002F,
            0x0030, 0x0031, 0x0032, 0x0033, 0x0034, 0x0035, 0x0036, 0x0037,
            0x0038, 0x0039, 0x003A, 0x003B, 0x003C, 0x003D, 0x003E, 0x003F,

            0x0040, 0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047,
            0x0048, 0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F,
            0x0050, 0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057,
            0x0058, 0x0059, 0x005A, 0x005B, 0x005C, 0x005D, 0x005E, 0x005F,
            0x0060, 0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067,
            0x0068, 0x0069, 0x006A, 0x006B, 0x006C, 0x006D, 0x006E, 0x006F,
            0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077,
            0x0078, 0x0079, 0x007A, 0x007B, 0x007C, 0x007D, 0x007E, 0x007F,

            0x00C7, 0x00FC, 0x00E9, 0x00E2, 0x00E4, 0x00E0, 0x00E5, 0x00E7,
            0x00EA, 0x00EB, 0x00E8, 0x00EF, 0x00EE, 0x00EC, 0x00C4, 0x00C5,
            0x00C9, 0x00E6, 0x00C6, 0x00F4, 0x00F6, 0x00F2, 0x00FB, 0x00F9,
            0x00FF, 0x00D6, 0x00DC, 0x00A2, 0x00A3, 0x00A5, 0x20A7, 0x0192,
            0x00E1, 0x00ED, 0x00F3, 0x00FA, 0x00F1, 0x00D1, 0x00AA, 0x00BA,
            0x00BF, 0x2310, 0x00AC, 0x00BD, 0x00BC, 0x00A1, 0x00AB, 0x00BB,
            0x2591, 0x2592, 0x2593, 0x2502, 0x2524, 0x2561, 0x2562, 0x2556,
            0x2555, 0x2563, 0x2551, 0x2557, 0x255D, 0x255C, 0x255B, 0x2510,

            0x2514, 0x2534, 0x252C, 0x251C, 0x2500, 0x253C, 0x255E, 0x255F,
            0x255A, 0x2554, 0x2569, 0x2566, 0x2560, 0x2550, 0x256C, 0x2567,
            0x2568, 0x2564, 0x2565, 0x2559, 0x2558, 0x2552, 0x2553, 0x256B,
            0x256A, 0x2518, 0x250C, 0x2588, 0x2584, 0x258C, 0x2590, 0x2580,
            0x03B1, 0x00DF, 0x0393, 0x03C0, 0x03A3, 0x03C3, 0x00B5, 0x03C4,
            0x03A6, 0x0398, 0x03A9, 0x03B4, 0x221E, 0x03C6, 0x03B5, 0x2229,
            0x2261, 0x00B1, 0x2265, 0x2264, 0x2320, 0x2321, 0x00F7, 0x2248,
            0x00B0, 0x2219, 0x00B7, 0x221A, 0x207F, 0x00B2, 0x25A0, 0x00A0,
        };
        #endregion

    }

}