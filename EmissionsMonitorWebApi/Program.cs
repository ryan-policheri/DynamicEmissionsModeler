using DotNetCommon;
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string connString = builder.Configuration.GetConnectionString("DefaultConnection");
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

            var hostUrl = builder.Configuration["HostUrl"];
            if (string.IsNullOrEmpty(hostUrl)) hostUrl = "http://0.0.0.0:5000";

            builder.WebHost
                .UseKestrel()
                .UseUrls(hostUrl)
                .Build();

            WebApplication app = builder.Build();

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