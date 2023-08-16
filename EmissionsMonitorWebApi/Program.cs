using DotNetCommon;
using DotNetCommon.Logging.File;
using DotNetCommon.Security;
using EIA.Domain.Model;
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
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string fileDirectory = builder.Configuration["LogDirectory"];
            if (fileDirectory == null) fileDirectory = builder.Environment.ContentRootPath;
            string fileName = $"EmissionsMonitorWebApi_{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}.log";
            FileLoggerConfig fileLoggerConfig = new FileLoggerConfig(fileDirectory, fileName);
            FileLoggerProvider fileLoggerProvider = new FileLoggerProvider(fileLoggerConfig);

            builder.Services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddProvider(fileLoggerProvider);
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

            // Encryption
            string iv = builder.Configuration["Encryption:Iv"];
            string key = builder.Configuration["Encryption:Key"];
            CredentialProvider credProvider = new CredentialProvider(iv, key);
            builder.Services.AddSingleton<ICredentialProvider>(credProvider);

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
    }

}