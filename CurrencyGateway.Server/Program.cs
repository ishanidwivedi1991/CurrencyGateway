using CurrencyGateway.Server.Controllers;
using NLog.Web;

namespace CurrencyGateway.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Application starting...");
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                builder.Services.AddControllers();
                builder.Services.AddHttpClient("currencyexchange", options => { options.BaseAddress = new Uri("https://api.fxratesapi.com/latest"); });
                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        policy =>
                        {
                            policy.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                        });
                });

                var app = builder.Build();

                app.UseDefaultFiles();
                app.UseStaticFiles();

                // Configure the HTTP request pipeline.
                app.UseCors();
                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.MapFallbackToFile("/index.html");

                app.Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Application stopped due to an exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
    }
}
