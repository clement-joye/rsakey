using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System.Text;

namespace RsaKey
{
    class Program
    {
        static void Main(string[] args)
        {
            string plainText = "localhost";

            string cipherText = Encrypt(plainText);
            string decryptedCipherText = Decrypt(cipherText);

            Console.WriteLine("Encrypted text: {0}", cipherText);
            Console.WriteLine("Decrypted text {0}. Encryption/Decryption was correct {1}",
                decryptedCipherText, (plainText == decryptedCipherText).ToString());
        }

        static string Decrypt(string cipherText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PemReader pr = new(
                File.OpenText("./pem_public.pem")
            );
            RsaKeyParameters keys = (RsaKeyParameters)pr.ReadObject();

            OaepEncoding eng = new(new RsaEngine());
            eng.Init(false, keys);

            int length = cipherTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> plainTextBytes = [];
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                plainTextBytes.AddRange(eng.ProcessBlock(
                    cipherTextBytes, chunkPosition, chunkSize
                ));
            }
            return Encoding.UTF8.GetString(plainTextBytes.ToArray());
        }

        static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PemReader pr = new(
                File.OpenText("./pem_private.pem")
            );

            var key = (RsaPrivateCrtKeyParameters)pr.ReadObject();

            OaepEncoding eng = new(new RsaEngine());
            eng.Init(true, key);

            int length = plainTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> cypherTextBytes = [];
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                cypherTextBytes.AddRange(eng.ProcessBlock(
                    plainTextBytes, chunkPosition, chunkSize
                ));
            }
            return Convert.ToBase64String(cypherTextBytes.ToArray());
        }
    }
}
