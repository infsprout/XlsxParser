using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace XlsxParser.Internal
{

    using Cryptography;

    internal class ExtraHashAlgorithm : HashAlgorithm
    {

        public string AlgorithmName { get; private set; }

        private System.Action _reset;

        private System.Action<byte[], int, int> _blockUpdate;

        private System.Func<byte[], int, int> _doFinal;

        private ExtraHashAlgorithm(
            string algorithmName,
            int digestSize,
            System.Action reset,
            System.Action<byte[], int, int> blockUpdate,
            System.Func<byte[], int, int> doFinal)
        {
            if (algorithmName == null) {
                throw new System.ArgumentNullException("algorithmName");
            }
            if (digestSize <= 0) {
                throw new System.ArgumentOutOfRangeException("digestSize");
            }
            if (reset == null) {
                throw new System.ArgumentNullException("reset");
            }
            if (blockUpdate == null) {
                throw new System.ArgumentNullException("blockUpdate");
            }
            if (doFinal == null) {
                throw new System.ArgumentNullException("doFinal");
            }
            AlgorithmName = algorithmName;
            HashSizeValue = digestSize * 8;
            _reset = reset;
            _blockUpdate = blockUpdate;
            _doFinal = doFinal;
        }


        public override void Initialize()
        {
            _reset();
        }

        protected override void HashCore(
            byte[] array, int ibStart, int cbSize)
        {
            _blockUpdate(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            var hash = new byte[HashSizeValue / 8];
            _doFinal(hash, 0);
            return hash;
        }

        public static ExtraHashAlgorithm CreateMD2()
        {
            var ha = new MD2Digest();
            return new ExtraHashAlgorithm(
                ha.AlgorithmName, ha.GetDigestSize(),
                ha.Reset, ha.BlockUpdate, ha.DoFinal
            );
        }

        public static ExtraHashAlgorithm CreateMD4()
        {
            var ha = new MD4Digest();
            return new ExtraHashAlgorithm(
                ha.AlgorithmName, ha.GetDigestSize(),
                ha.Reset, ha.BlockUpdate, ha.DoFinal
            );
        }

        public static ExtraHashAlgorithm CreateRipeMD128()
        {
            var ha = new RipeMD128Digest();
            return new ExtraHashAlgorithm(
                ha.AlgorithmName, ha.GetDigestSize(),
                ha.Reset, ha.BlockUpdate, ha.DoFinal
            );
        }

        public static ExtraHashAlgorithm CreateRipeMD160()
        {
            var ha = new RipeMD160Digest();
            return new ExtraHashAlgorithm(
                ha.AlgorithmName, ha.GetDigestSize(),
                ha.Reset, ha.BlockUpdate, ha.DoFinal
            );
        }

        public static ExtraHashAlgorithm CreateWhirlpool()
        {
            var ha = new WhirlpoolDigest();
            return new ExtraHashAlgorithm(
                ha.AlgorithmName, ha.GetDigestSize(),
                ha.Reset, ha.BlockUpdate, ha.DoFinal
            );
        }

    }

}
