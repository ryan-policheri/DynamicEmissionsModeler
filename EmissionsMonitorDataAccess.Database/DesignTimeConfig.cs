namespace EmissionsMonitorDataAccess.Database
{
    public class DesignTimeConfig
    {
        public string DefaultConnection { get; set; } = "Server=.\\SQLEXPRESS;Database=IowaDev;Trusted_Connection=True;";

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool CredentialsExist => !String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(Password);

        public string BuildConnectionString()
        {
            if (CredentialsExist) return DefaultConnection + $";User Id={UserName};Password={Password}";
            else return DefaultConnection;
        } 
    }
}
