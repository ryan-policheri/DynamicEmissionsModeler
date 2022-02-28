using DotNetCommon.PersistenceHelpers;
using DotNetCommon.SystemHelpers;

namespace UIowaBuildingsModel
{
    public class DataFileProvider
    {
        private string _appDataDirectory;

        public DataFileProvider(string appDataDirectory)
        {
            _appDataDirectory = appDataDirectory;
            SystemFunctions.CreateDirectory(_appDataDirectory);
        }

        public AppDataFile BuildDataViewFile()
        {
            return BuildAppDataFile("DataViews");
        }

        public AppDataFile BuildCredentialsFile()
        {
            return BuildAppDataFile(null, "Credentials");
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