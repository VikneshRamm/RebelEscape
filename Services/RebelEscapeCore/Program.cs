using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RebelEscapeCore.DataStore;
using RebelEscapeCore.Hubs;
using RebelEscapeCore.Services;

namespace RebelEscapeCore
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

            builder.Services.AddSingleton<IDataStore, DataStore.DataStore>();
            builder.Services.AddSingleton<IGameEngineService, GameEngineService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                string? jwtTokenKey = Environment.GetEnvironmentVariable("JWT_TOKEN_KEY");
                if (string.IsNullOrEmpty(jwtTokenKey))
                {
                    throw new Exception("JWT Token Key Not Available");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                            .GetBytes(jwtTokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/connectionhub"))
                        {
                            // Read the token out of the query string
                            Console.WriteLine("Inside the context");
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                };
            });

            builder.Services.AddCors(p => p.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200");
            }));

            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ConnectionHub>("/connectionhub");

            app.Run();
        }
    }
}
