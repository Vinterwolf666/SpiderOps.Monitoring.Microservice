


using Customer.Monitoring.Microservice.App;
using Customer.Monitoring.Microservice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Customer.Identity.Microservice.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

          

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var configuration = builder.Configuration;

            
            string projectId = "spiderops"; 

           

            builder.Services.AddDbContext<MonitoringDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Value"), b => b.MigrationsAssembly("Customer.Monitoring.Microservice.API")));



            builder.Services.AddScoped<IMonitoringRepository, MonitoringRepository>();
            
            builder.Services.AddScoped<IMonitoringServices, MonitoringService>();
            
            



            builder.Services.AddCors(options =>
            {

                options.AddPolicy("nuevaPolitica", app =>
                {

                    app.AllowAnyOrigin();
                    app.AllowAnyHeader();
                    app.AllowAnyMethod();
                });



            });


            var app = builder.Build();

           

            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("nuevaPolitica");

           app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}