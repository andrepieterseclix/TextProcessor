using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace TextProcessor.ClipboardStream
{
    class HashHelper
    {
        public static string CalculateHashString(string[] input)
        {
            using (HashAlgorithm algorithm = new MD5CryptoServiceProvider())
            {
                byte[] hash = algorithm.ComputeHash(getBytes(input));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("X2"));

                return sb.ToString();
            }
        }

        private static byte[] getBytes(string[] input)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, input);
                return stream.ToArray();
            }
        }
    }
}
