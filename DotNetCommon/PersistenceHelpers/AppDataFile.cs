namespace DotNetCommon.PersistenceHelpers
{
    public class AppDataFile
    {
        public AppDataFile(FileInfo fileInfo)
        {
            SaveDirectory = fileInfo.DirectoryName;
            FileName = fileInfo.Name;
        }

        public AppDataFile(object saveObject)
        {
            string fileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            throw new NotImplementedException();
            fileDirectory += "\\EIADataViewer";
            SaveDirectory = fileDirectory; //TODO: We should get this info from a config
            FileName = Guid.NewGuid().ToString() + ".json";
            SaveObject = saveObject;
        }

        public string SaveDirectory { get; }

        public string FileName { get; }

        public string FullFilePath => SaveDirectory + "\\" + FileName;

        public object SaveObject { get; set; }
    }
}
