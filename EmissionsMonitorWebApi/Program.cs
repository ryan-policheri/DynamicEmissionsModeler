using DotNetCommon;
using DotNetCommon.Bases;
using DotNetCommon.Logging;
using DotNetCommon.Logging.Email;
using DotNetCommon.Logging.File;
using DotNetCommon.SystemHelpers;
using EIA.Services.Clients;
using EmissionsMonitorDataAccess;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorDataAccess.Database;
using EmissionsMonitorDataAccess.Database.Repositories;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorServices.DataSourceWrappers;
using Microsoft.EntityFrameworkCore;
using PiServices;

namespace EmissionsMonitorWebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //LOGGING
            ClientInfo clientInfo = new ClientInfo("EmissionsMonitorWebApi", "N/A", "N/A", Environment.MachineName, builder.Environment.EnvironmentName);

            IConfigurationSection fileLogSection = builder.Configuration.GetSection("Logging:File");
            FileLoggerConfig fileLogConfig = new FileLoggerConfig();
            FileLoggerProvider fileLoggerProvider = null;
            if (fileLogSection.Exists())
            {
                fileLogSection.Bind(fileLogConfig);
                if (String.IsNullOrWhiteSpace(fileLogConfig.LogDirectory)) fileLogConfig.LogDirectory = SystemFunctions.CombineDirectoryComponents(Environment.CurrentDirectory, "Logs");
                fileLogConfig.LogFileName = $"EmissionsMonitorWebApi_{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}.log";
                fileLogConfig.AssertValid();
                fileLoggerProvider = new FileLoggerProvider(fileLogConfig);
            }


            IConfigurationSection emailLogSection = builder.Configuration.GetSection("Logging:Email");
            EmailLoggerConfig emailLoggerConfig = new EmailLoggerConfig();
            EmailLoggerProvider emailLoggerProvider = null;
            if (emailLogSection.Exists())
            {
                emailLogSection.Bind(emailLoggerConfig);
                emailLoggerConfig.Subject = "Emissions Monitor Web Api Error Log";
                emailLoggerConfig.AssertValid();
                emailLoggerProvider = new EmailLoggerProvider(clientInfo, emailLoggerConfig);
            }

            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                if (fileLoggerProvider != null) logging.AddProvider(fileLoggerProvider);
                if (emailLoggerProvider != null) logging.AddProvider(emailLoggerProvider);
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string connString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (!builder.Environment.IsDevelopment()) connString += $";User Id={builder.Configuration["EmissionsMonitorDatabaseUser"]};Password={builder.Configuration["EmissionsMonitorDatabasePassword"]}";
            builder.Services.AddDbContext<EmissionsMonitorContext>(options => options.UseSqlServer(connString));
            builder.Services.AddTransient<IDataSourceRepository, DataSourceRepository>();
            builder.Services.AddTransient<IVirtualFileSystemRepository, VirtualFileSystemRepository>();

            builder.Services.AddTransient<EiaClient>();
            builder.Services.AddTransient<PiHttpClient>();
            builder.Services.AddTransient<DataSourceServiceFactory>(x =>
            {
                var factory = new DataSourceServiceFactory(() => x.GetService<EiaClient>(), () => x.GetService<PiHttpClient>(),
                    x.GetService<IDataSourceRepository>(), true);
                var task = Task.Run(async () => await factory.LoadAllDataSourceServices());
                task.Wait();
                return factory;
            });

            builder.Services.AddTransient<ITimeSeriesDataSource, MainDataSourceRepo>();
            builder.Services.AddTransient<ModelInitializationService>();
            builder.Services.AddTransient<DynamicCompilerService>();
            builder.Services.AddTransient<ModelExecutionService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }

        private static IConfiguration GetConfiguration(this HostBuilderContext builderContext, string sectionName = null)
        {
            return String.IsNullOrWhiteSpace(sectionName) ? builderContext.Configuration : builderContext.Configuration.GetSection(sectionName);
        }

        private static T BindConfigToValidatingObject<T>(this IConfiguration configuration) where T : ValidatingObject, new()
        {
            T obj = BindConfigToObject<T>(configuration);
            obj.AssertValid();
            return obj;
        }

        private static T BindConfigToObject<T>(this IConfiguration configuration) where T : new()
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            configuration.Bind(obj);
            return obj;
        }
    }

}