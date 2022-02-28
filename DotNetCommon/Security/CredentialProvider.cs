using System.Security.Cryptography;
using System.Text;

namespace DotNetCommon.Security
{
    public class CredentialProvider : ICredentialProvider
    {
        private readonly string _ivEnvironmentVariable;
        private readonly string _keyEnvironmentVariable;

        private byte[] Iv => Convert.FromBase64String(Environment.GetEnvironmentVariable(_ivEnvironmentVariable));

        private byte[] Key => Convert.FromBase64String(Environment.GetEnvironmentVariable(_keyEnvironmentVariable));

        public CredentialProvider(string ivEnvironmentVariable, string keyEnvironmentVariable)
        {
            _ivEnvironmentVariable = ivEnvironmentVariable;
            _keyEnvironmentVariable = keyEnvironmentVariable;
        }

        public static void GenerateIvAndKey()
        {
            Aes aes = Aes.Create();
            string ivAsString = Convert.ToBase64String(aes.IV);
            string keyAsString = Convert.ToBase64String(aes.Key);
            Console.WriteLine($"IV: {ivAsString}");
            Console.WriteLine($"Key: {keyAsString}");
        }

        public string DecryptValue(string value)
        {
            using (Aes aes = Aes.Create())
            {
                aes.IV = Iv;
                aes.Key = Key;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(value)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        public string EncryptValue(string value)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                aes.IV = Iv;
                aes.Key = Key;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (MemoryStream initial = new MemoryStream(buffer))
                        {
                            initial.CopyTo(cs);
                        }
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}