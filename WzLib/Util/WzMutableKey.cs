using System;
using System.IO;
using System.Security.Cryptography;
using MapleLib.MapleCryptoLib;

namespace MapleLib.WzLib.Util
{
    public class WzMutableKey
    {
        public WzMutableKey(byte[] WzIv, byte[] AesKey)
        {
            this.iv = WzIv;
            this.aesKey = AesKey;
        }

        private static readonly int BatchSize = 4096;
        private byte[] iv;
        private byte[] aesKey;

        private byte[] keys;

        public byte this[int index]
        {
            get
            {
                if (keys == null || keys.Length <= index)
                {
                    EnsureKeySize(index + 1);
                }
                return this.keys[index];
            }
        }

        public void EnsureKeySize(int size)
        {
            if (keys != null && keys.Length >= size)
            {
                return;
            }

            size = (int)Math.Ceiling(1.0 * size / BatchSize) * BatchSize;
            byte[] newKeys = new byte[size];

            if (BitConverter.ToInt32(this.iv, 0) == 0)
            {
                this.keys = newKeys;
                return;
            }

            int startIndex = 0;

            if (keys != null)
            {
                Buffer.BlockCopy(keys, 0, newKeys, 0, keys.Length);
                startIndex = keys.Length;
            }

            Rijndael aes = Rijndael.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = aesKey;
            aes.Mode = CipherMode.ECB;
            MemoryStream ms = new MemoryStream(newKeys, startIndex, newKeys.Length - startIndex, true);
            CryptoStream s = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

            for (int i = startIndex; i < size; i += 16)
            {
                if (i == 0)
                {
                    byte[] block = new byte[16];
                    for (int j = 0; j < block.Length; j++)
                    {
                        block[j] = iv[j % 4];
                    }
                    s.Write(block, 0, block.Length);
                }
                else
                {
                    s.Write(newKeys, i - 16, 16);
                }
            }

            s.Flush();
            ms.Close();
            this.keys = newKeys;
        }
    }
}
