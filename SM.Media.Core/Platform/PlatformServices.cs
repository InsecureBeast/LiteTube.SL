using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Platform
{
    public class PlatformServices : IPlatformServices
    {
        private readonly Stack<Random> _generators = new Stack<Random>();

        public double GetRandomNumber()
        {
            Random random = this.GetRandom();
            double num = random.NextDouble();
            this.FreeRandom(random);
            return num;
        }

        public Stream Aes128DecryptionFilter(Stream stream, byte[] key, byte[] iv)
        {
            AesManaged aesManaged1 = new AesManaged();
            aesManaged1.Key = key;
            aesManaged1.IV = iv;
            AesManaged aesManaged2 = aesManaged1;
            return (Stream)new CryptoStream(stream, aesManaged2.CreateDecryptor(), CryptoStreamMode.Read);
        }

        public void GetSecureRandom(byte[] bytes)
        {
            new RNGCryptoServiceProvider().GetBytes(bytes);
        }

        private Random GetRandom()
        {
            lock (this._generators)
            {
                if (this._generators.Count > 0)
                    return this._generators.Pop();
            }
            byte[] bytes = new byte[4];
            this.GetSecureRandom(bytes);
            return new Random(BitConverter.ToInt32(bytes, 0));
        }

        private void FreeRandom(Random random)
        {
            lock (this._generators)
                this._generators.Push(random);
        }
    }
}
