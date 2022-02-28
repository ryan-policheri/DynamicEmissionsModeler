using DotNetCommon.SystemHelpers;
using System.Security.Cryptography;
using System.Text;

namespace DotNetCommon.Security
{
    public class CredentialProvider : ICredentialProvider
    {
        private readonly string _ivEnvironmentVariable;
        private readonly string _keyEnvironmentVariable;
        private readonly string _encryptionFilePath;

        private byte[] Iv 
        { 
            get
            { 
                if (!String.IsNullOrWhiteSpace(_ivEnvironmentVariable))
                {
                    return Convert.FromBase64String(Environment.GetEnvironmentVariable(_ivEnvironmentVariable));
                }
                else
                {
                    EncryptionKey key = SystemFunctions.ReadJsonObject<EncryptionKey>(_encryptionFilePath);
                    return Convert.FromBase64String(key.Iv);
                }
            }
        }

        private byte[] Key 
        { 
            get
            {
                if (!String.IsNullOrWhiteSpace(_keyEnvironmentVariable))
                {
                    return Convert.FromBase64String(Environment.GetEnvironmentVariable(_keyEnvironmentVariable));
                }
                else
                {
                    EncryptionKey key = SystemFunctions.ReadJsonObject<EncryptionKey>(_encryptionFilePath);
                    return Convert.FromBase64String(key.Key);
                }
            }
        }

        public CredentialProvider(string ivEnvironmentVariable, string keyEnvironmentVariable)
        {
            _ivEnvironmentVariable = ivEnvironmentVariable;
            _keyEnvironmentVariable = keyEnvironmentVariable;
        }

        public CredentialProvider(string encryptionFilePath)
        {
            _encryptionFilePath = encryptionFilePath;
        }

        public static EncryptionKey GenerateIvAndKey()
        {
            Aes aes = Aes.Create();
            string ivAsString = Convert.ToBase64String(aes.IV);
            string keyAsString = Convert.ToBase64String(aes.Key);
            EncryptionKey key = new EncryptionKey
            {
                Iv = ivAsString,
                Key = keyAsString
            };
            return key;
        }

        public string DecryptValue(string value)
        {
            if (value == null) return null;
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
            if (value == null) return null;
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