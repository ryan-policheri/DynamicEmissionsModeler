namespace EmissionsMonitorServices.Database
{
    public static class DbInitializer
    {
        public static void Initialize(EmissionsMonitorContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
