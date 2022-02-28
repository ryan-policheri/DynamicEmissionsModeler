using System.Text.Json;
using DotNetCommon.SystemHelpers;

namespace DotNetCommon.PersistenceHelpers
{
    public class AppDataFile
    {
        public AppDataFile(string filePath) : this(new FileInfo(filePath))
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
        }

        public AppDataFile(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            RootSaveDirectory = fileInfo.DirectoryName;
            DefaultFileName = fileInfo.Name;
        }

        public AppDataFile(string rootDirectory, string defaultFileName)
        {
            if (rootDirectory == null) throw new ArgumentNullException(nameof(rootDirectory));
            RootSaveDirectory = rootDirectory;
            DefaultFileName = defaultFileName;
            if (DefaultFileName == null) DefaultFileName = Guid.NewGuid().ToString();
        }

        public string RootSaveDirectory { get; }

        private string _defaultFileName;
        public string DefaultFileName
        {
            get { return _defaultFileName; }
            private set
            {
                if (!String.IsNullOrWhiteSpace(value) && !value.EndsWith(".json"))
                {
                    value += ".json";
                }
                _defaultFileName = value;
            }
        }

        public string FullFilePath => SystemFunctions.CombineDirectoryComponents(RootSaveDirectory, DefaultFileName);

        public bool FileExists => File.Exists(FullFilePath);

        public object SaveObject { get; internal set; }

        public void Save(object saveObject)
        {
            InternalSave(saveObject, FullFilePath);
        }

        public void Save(object saveObject, string customFileName)
        {
            string filePath = SystemFunctions.CombineDirectoryComponents(RootSaveDirectory, customFileName);
            InternalSave(saveObject, filePath);
        }

        private void InternalSave(object saveObject, string filePath)
        {
            this.SaveObject = saveObject;
            string json = JsonSerializer.Serialize(this.SaveObject);
            SystemFunctions.WriteAllText(filePath, json);
        }

        public async Task SaveAsync(object saveObject)
        {
            await InternalSaveAsync(saveObject, FullFilePath);
        }

        public async Task SaveAsync(object saveObject, string customFileName)
        {
            string filePath = SystemFunctions.CombineDirectoryComponents(RootSaveDirectory, customFileName);
            await InternalSaveAsync(saveObject, filePath);
        }

        private async Task InternalSaveAsync(object saveObject, string filePath)
        {
            this.SaveObject = saveObject;
            string json = JsonSerializer.Serialize(this.SaveObject);
            await SystemFunctions.WriteAllTextAsync(filePath, json);
        }

        public T Read<T>()
        {
            string contents = SystemFunctions.ReadAllText(this.FullFilePath);
            if (contents == null) return default(T);
            T obj = JsonSerializer.Deserialize<T>(contents);
            this.SaveObject = obj;
            return obj;
        }

        public async Task<T> ReadAsync<T>()
        {
            string contents = await SystemFunctions.ReadAllTextAsync(this.FullFilePath);
            if (contents == null) return default(T);
            T obj = JsonSerializer.Deserialize<T>(contents);
            this.SaveObject = obj;
            return obj;
        }

        public static IEnumerable<AppDataFile> RetrieveAllAppFilesInDirectory(string directory)
        {
            ICollection<AppDataFile> appFiles = new List<AppDataFile>();

            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                AppDataFile appFile = new AppDataFile(file);
                appFiles.Add(appFile);
            }

            return appFiles;
        }
    }
}