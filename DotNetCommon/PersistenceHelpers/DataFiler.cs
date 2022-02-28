using System.Text.Json;

namespace DotNetCommon.PersistenceHelpers
{
    public class DataFiler
    {
        public static async Task SaveObjectAsFile(AppDataFile file)
        {
            string json = JsonSerializer.Serialize(file.SaveObject);

            using (StreamWriter writer = new StreamWriter(file.FullFilePath))
            {
                await writer.WriteAsync(json);
            }
        }

        public static IEnumerable<AppDataFile> RetrieveAllAppFilesInDirectory(string directory)
        {
            ICollection<AppDataFile> appFiles = new List<AppDataFile>();

            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                AppDataFile appFile = new AppDataFile(info);
                appFiles.Add(appFile);
            }

            return appFiles;
        }

        public static async Task<AppDataFile> RetreiveAppDataObject<T>(string fullPath)
        {
            FileInfo info = new FileInfo(fullPath);
            FileStream stream = new FileStream(fullPath, FileMode.Open);
            using (StreamReader reader = new StreamReader(stream))
            {
                string contents = await reader.ReadToEndAsync();
                AppDataFile appDataFile = new AppDataFile(info);
                object obj = JsonSerializer.Deserialize<T>(contents);
                appDataFile.SaveObject = obj;
                return appDataFile;
            }
        }
    }
}