using System.Reflection;
using DotNetCommon.Security;

namespace DotNetCommon.PersistenceHelpers
{
    public class EncryptedAppDataFile : AppDataFile
    {
        private readonly ICredentialProvider _credentialProvider;

        public EncryptedAppDataFile(string filePath, ICredentialProvider credentialProvider) : this(new FileInfo(filePath), credentialProvider)
        {
        }

        public EncryptedAppDataFile(FileInfo fileInfo, ICredentialProvider credentialProvider) : base(fileInfo)
        {
            _credentialProvider = credentialProvider;
        }

        public EncryptedAppDataFile(string rootDirectory, string defaultFileName, ICredentialProvider credentialProvider) : base(rootDirectory, defaultFileName)
        {
            _credentialProvider = credentialProvider;
        }

        public new void Save<T>(T saveObject) where T : new()
        {
            EncryptValues<T>(saveObject);
            base.Save(saveObject);
        }

        public new void Save<T>(T saveObject, string customFileName) where T : new()
        {
            EncryptValues<T>(saveObject);
            base.Save(saveObject, customFileName);
        }

        public new async Task SaveAsync<T>(T saveObject) where T : new()
        {
            EncryptValues<T>(saveObject);
            await base.SaveAsync<T>(saveObject);
        }

        public new async Task SaveAsync<T>(T saveObject, string customFileName) where T : new()
        {
            EncryptValues<T>(saveObject);
            await base.SaveAsync<T>(saveObject, customFileName);
        }

        public new T Read<T>() where T : new()
        {
            T obj = base.Read<T>();
            DecryptValues<T>(obj);
            return obj;
        }

        public new async Task<T> ReadAsync<T>() where T : new()
        {
            T obj = await base.ReadAsync<T>();
            DecryptValues<T>(obj);
            return obj;
        }

        public new T Update<T>(Action<T> updateFunction) where T : new()
        {
            T obj = this.Read<T>();
            DecryptValues<T>(obj);
            updateFunction.Invoke(obj);
            EncryptValues<T>(obj);
            this.Save(obj);
            return obj;
        }

        public new async Task<T> UpdateAsync<T>(Action<T> updateFunction) where T : new()
        {
            T obj = await this.ReadAsync<T>();
            DecryptValues<T>(obj);
            updateFunction.Invoke(obj);
            EncryptValues<T>(obj);
            await this.SaveAsync(obj);
            return obj;
        }

        private void EncryptValues<T>(object saveObject) where T : new()
        {
            if (saveObject == null) throw new ArgumentNullException(nameof(saveObject));
            Type type = typeof(T);
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    string value = prop.GetValue(saveObject)?.ToString();
                    string encrpytedValue = _credentialProvider.EncryptValue(value);
                    prop.SetValue(saveObject, encrpytedValue);
                }
                else
                {
                    throw new NotImplementedException($"Need to implement encryption of {prop.PropertyType.Name}");
                }
            }
        }

        private void DecryptValues<T>(object saveObject) where T : new()
        {
            if (saveObject == null) throw new ArgumentNullException(nameof(saveObject));
            Type type = typeof(T);
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    string value = prop.GetValue(saveObject)?.ToString();
                    string decryptedValue = _credentialProvider.DecryptValue(value);
                    prop.SetValue(saveObject, decryptedValue);
                }
                else
                {
                    throw new NotImplementedException($"Need to implement decryption of {prop.PropertyType.Name}");
                }
            }
        }
    }
}
