using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using FarmerzonAuthentication.Helper;
using FarmerzonAuthenticationDataAccess;
using FarmerzonAuthenticationErrorHandling;
using FarmerzonAuthenticationManager.Implementation;
using FarmerzonAuthenticationManager.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FarmerzonAuthentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddDapr();
            
            services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            // Disable default model validation like it is described under the following link
            // https://www.talkingdotnet.com/disable-automatic-model-state-validation-in-asp-net-core-2-1/
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            // Add a custom model validation like it is described under the following link 
            // https://www.talkingdotnet.com/validate-model-state-automatically-asp-net-core-2-0/
            services.AddMvc(options => { options.Filters.Add(typeof(ValidateModelStateAttribute)); });
            
            // Add authentication database
            services.AddDbContext<FarmerzonAuthenticationContext>(
                option => option.UseNpgsql(
                    Configuration.GetConnectionString("FarmerzonAuthentication"),
                    x => x.MigrationsAssembly(nameof(FarmerzonAuthentication))));
            
            // Add authentication service
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    // Lockout settings after 
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5; 
                })
                .AddEntityFrameworkStores<FarmerzonAuthenticationContext>()
                .AddDefaultTokenProviders();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Issuer"],
                RequireExpirationTime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]))
            };
            services.AddSingleton(tokenValidationParameters);
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Farmerzon Authentication API", Version = "v1"});

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            
            // manager DI container
            services.AddScoped<IAuthenticationManager, AuthenticationManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, FarmerzonAuthenticationContext context)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Farmerzon Authentication API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            }

            app.UseRouting();
            app.UseCloudEvents();

            // It is important to use app.UseAuthentication(); before app.UseAuthorization();
            // Otherwise authentication with json web tokens doesn't work.
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });
            
            context.Database.Migrate();
        }
    }
}