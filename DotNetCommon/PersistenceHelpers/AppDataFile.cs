using System.Text.Json;
using DotNetCommon.Extensions;
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

        public string FileDisplayName => DefaultFileName.TrimEnd(".json");

        public string FullFilePath => SystemFunctions.CombineDirectoryComponents(RootSaveDirectory, DefaultFileName);

        public bool FileExists => File.Exists(FullFilePath);

        public object SaveObject { get; internal set; }

        public void Save<T>(T saveObject) where T : new()
        {
            InternalSave<T>(saveObject, FullFilePath);
        }

        public void Save<T>(T saveObject, string customFileName) where T : new()
        {
            if (!customFileName.EndsWith(".json")) customFileName += ".json";
            string filePath = SystemFunctions.CombineDirectoryComponents(RootSaveDirectory, customFileName);
            InternalSave<T>(saveObject, filePath);
        }

        private void InternalSave<T>(T saveObject, string filePath) where T : new()
        {
            this.SaveObject = saveObject;
            string json = this.SaveObject.ToBeautifulJson();
            SystemFunctions.WriteAllText(filePath, json);
        }

        public async Task SaveAsync<T>(T saveObject) where T : new()
        {
            await InternalSaveAsync<T>(saveObject, FullFilePath);
        }

        public async Task SaveAsync<T>(T saveObject, string customFileName) where T : new()
        {
            if (!customFileName.EndsWith(".json")) customFileName += ".json";
            string filePath = SystemFunctions.CombineDirectoryComponents(RootSaveDirectory, customFileName);
            await InternalSaveAsync(saveObject, filePath);
        }

        private async Task InternalSaveAsync<T>(T saveObject, string filePath) where T : new()
        {
            this.SaveObject = saveObject;
            string json = this.SaveObject.ToBeautifulJson();
            await SystemFunctions.WriteAllTextAsync(filePath, json);
        }

        public T Read<T>() where T : new()
        {
            if (!FileExists) return new T();
            string contents = SystemFunctions.ReadAllText(this.FullFilePath);
            if (contents == null) return default(T);
            T obj = JsonSerializer.Deserialize<T>(contents);
            this.SaveObject = obj;
            return obj;
        }

        public async Task<T> ReadAsync<T>() where T : new()
        {
            if (!FileExists) return new T();
            string contents = await SystemFunctions.ReadAllTextAsync(this.FullFilePath);
            if (contents == null) return default(T);
            T obj = JsonSerializer.Deserialize<T>(contents);
            this.SaveObject = obj;
            return obj;
        }

        public T Update<T>(Action<T> updateFunction) where T : new()
        {
            T obj = this.Read<T>();
            updateFunction.Invoke(obj);
            this.Save(obj);
            return obj;
        }

        public async Task<T> UpdateAsync<T>(Action<T> updateFunction) where T : new()
        {
            T obj = await this.ReadAsync<T>();
            updateFunction.Invoke(obj);
            await this.SaveAsync(obj);
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