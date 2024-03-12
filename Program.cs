using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using UserManagerApp.DB;
using UserManagerApp.Helpers;
using UserManagerApp.Repository;
using UserManagerApp.Services;

namespace UserManagerApp
{

    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var configBuilder = new ConfigurationBuilder().AddJsonFile("config.json");
            var configuration = configBuilder.Build();
            var connection = configuration.GetSection("MongoDatabase");

            builder.Services.AddSingleton(
                new MongoClient(connection.GetSection("connection").Value)
                              .GetDatabase(connection.GetSection("base").Value));            


            builder.Services.AddAuthorization();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();
            builder.Services.AddControllers();           

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var mdatabase = services.GetRequiredService<IMongoDatabase>();

                await Initialize.InitMonogoDB(mdatabase);
            }          
            

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            

            app.MapControllerRoute(name: "default", pattern: "{controller=User}/{action=Index}/{id?}");           

            app.Run();
        }
    }
}