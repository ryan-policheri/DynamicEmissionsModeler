using DotNetCommon.PersistenceHelpers;
using DotNetCommon.Security;

namespace UnifiedDataExplorer.Services.DataFiles
{
    public class CredentialsDataFile : AppDataFile
    {
        private readonly CredentialProvider _crendtialProvider;

        public CredentialsDataFile(string filePath, CredentialProvider credentialProvider) : base(filePath)
        {
            _crendtialProvider = credentialProvider;
        }

        public void UpdateEiaApiKey(string apiKey)
        {
            CredentialConfig config = this.Read<CredentialConfig>();
            if (config == null) config = new CredentialConfig();
            string encrypted = _crendtialProvider.EncryptValue(apiKey);
            config.EncryptedEiaWebApiKey = encrypted;
            this.Save(config);
        }
    }
}
