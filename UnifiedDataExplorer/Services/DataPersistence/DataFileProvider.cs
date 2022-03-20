using DotNetCommon.PersistenceHelpers;
using DotNetCommon.Security;
using DotNetCommon.SystemHelpers;

namespace UnifiedDataExplorer.Services.DataPersistence
{
    public class DataFileProvider
    {
        private readonly string _appDataDirectory;
        private readonly ICredentialProvider _credentialProvider;

        public DataFileProvider(string appDataDirectory)
        {
            _appDataDirectory = appDataDirectory;
            SystemFunctions.CreateDirectory(_appDataDirectory);
        }

        public DataFileProvider(string appDataDirectory, ICredentialProvider credentialProvider) : this(appDataDirectory)
        {
            _credentialProvider = credentialProvider;
        }

        public string GetExportsDirectory()
        {
            return BuildAppDataFile("Exports").RootSaveDirectory;
        }

        public string GetReportsDirectory()
        {
            return BuildAppDataFile("Reports").RootSaveDirectory;
        }

        public AppDataFile BuildHourlyEmissionsReportDataFile()
        {
            return BuildAppDataFile("Reports", "HourlyEmissionsReportParameters");
        }

        public AppDataFile BuildDataViewFile()
        {
            return BuildAppDataFile("DataViews");
        }

        public EncryptedAppDataFile BuildCredentialsFile()
        {
            return BuildEncryptedAppDataFile(null, "Credentials");
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

        private EncryptedAppDataFile BuildEncryptedAppDataFile(string subdirectoryPath = null, string fileName = null)
        {
            string directory = _appDataDirectory;
            if (subdirectoryPath != null) directory = SystemFunctions.CombineDirectoryComponents(_appDataDirectory, subdirectoryPath);
            SystemFunctions.CreateDirectory(directory);
            return new EncryptedAppDataFile(directory, fileName, _credentialProvider);
        }
    }
}