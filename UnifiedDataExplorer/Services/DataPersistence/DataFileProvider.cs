using DotNetCommon.PersistenceHelpers;
using DotNetCommon.Security;
using DotNetCommon.SystemHelpers;
using UnifiedDataExplorer.Services.DataFiles;

namespace UnifiedDataExplorer.Services
{
    public class DataFileProvider
    {
        private readonly string _appDataDirectory;
        private readonly CredentialProvider _credentialProvider;

        public DataFileProvider(string appDataDirectory)
        {
            _appDataDirectory = appDataDirectory;
            SystemFunctions.CreateDirectory(_appDataDirectory);
        }

        public DataFileProvider(string appDataDirectory, CredentialProvider credentialProvider) : this(appDataDirectory)
        {
            _credentialProvider = credentialProvider;
        }

        public AppDataFile BuildDataViewFile()
        {
            return BuildAppDataFile("DataViews");
        }

        public CredentialsDataFile BuildCredentialsFile()
        {
            AppDataFile appDataFile = BuildAppDataFile(null, "Credentials");
            CredentialsDataFile credentialsDataFile = new CredentialsDataFile(appDataFile.FullFilePath, _credentialProvider);
            return credentialsDataFile;
        }

        public AppDataFile BuildKeyFile()
        {
            return BuildAppDataFile(null, "Key");
        }

        private AppDataFile BuildAppDataFile(string subdirectoryPath = null, string fileName = null)
        {
            string directory = _appDataDirectory;
            if(subdirectoryPath != null) directory = SystemFunctions.CombineDirectoryComponents(_appDataDirectory, subdirectoryPath);
            SystemFunctions.CreateDirectory(directory);
            return new AppDataFile(directory, fileName);
        }
    }
}