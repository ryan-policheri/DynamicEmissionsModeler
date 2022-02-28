namespace DotNetCommon.PersistenceHelpers
{
    public static class AppDataFolderOptions
    {
        public static string Roaming => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string Local => Roaming.Replace("Roaming", "Local");
    }
}