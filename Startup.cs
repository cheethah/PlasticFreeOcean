using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlasticFreeOcean.Models;
using Microsoft.EntityFrameworkCore;
using NJsonSchema;
using NSwag.AspNetCore;
using System.Reflection;
using Pomelo.EntityFrameworkCore.MySql;
using System;
using Microsoft.AspNetCore.Http;
using PlasticFreeOcean.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using PlasticFreeOcean.OuthProvider;

namespace PlasticFreeOcean
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PlasticFreeOceanContext>(options =>
                 options.UseMySql(Configuration.GetConnectionString("DefaultConnection")))
                    .AddUnitOfWork<PlasticFreeOceanContext>();

            services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<PlasticFreeOceanContext>().AddDefaultTokenProviders();

            services.AddScoped<RoleManager<Role>>();
        

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // If the LoginPath isn't set, ASP.NET Core defaults 
                // the path to /Account/Login.
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "FPOC API",
                    Description = "FPOC API used for website, android and ios FPOC API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Bagas Cita",
                        Email = "bagascita@gmail.com",
                         Url = "https://www.linkedin.com/in/bagascita/"
                    },
                });
            });
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<IAccountHelper, AccountHelper>();
            services.AddTransient<IRoleHelper, RoleHelper>();

            services.AddAuthentication()
            .AddCookie(cfg =>
            {
                cfg.SlidingExpiration = true;
                cfg.LoginPath = "/Admin/Login";
                cfg.LogoutPath = "/Admin/Logout";
                cfg.AccessDeniedPath = "/Admin/AccessDenied";
                cfg.Cookie.HttpOnly = true;
                cfg.Cookie.Expiration = TimeSpan.FromDays(150);
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;

                cfg.TokenValidationParameters = new TokenValidationParameters()
                {

                    ValidIssuer = Configuration["TokenAuthentication:Issuer"],
                    ValidAudience = Configuration["TokenAuthentication:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenAuthentication:SecretKey"]))
                };
            });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = "ServerCookie";
            //})
                    
            //.AddOAuthValidation()

             //   .AddOpenIdConnectServer(options =>
             //{
             //    // Enable the token endpoint.
             //    options.Provider = new AuthorizationProvider();
             //    // Enable the authorization and token endpoints
             //   options.AuthorizationEndpointPath = "/connect/authorize";
             //    options.LogoutEndpointPath = "/connect/logout";
             //    options.TokenEndpointPath = "/connect/token";
             //    options.UserinfoEndpointPath = "/connect/userinfo";

             //    options.ApplicationCanDisplayErrors = true;
             //    options.AllowInsecureHttp = true;

             //    options.AccessTokenLifetime = TimeSpan.FromMinutes(5);
             //    // During development, you can set AllowInsecureHttp
             //    // to true to disable the HTTPS requirement.
             //    options.AllowInsecureHttp = true;

             //});
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FPOC API V1");
            });
            app.UseAuthentication();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();

            app.Run(async context =>
            {
                context.Response.Redirect("swagger");
            });
        }
    }
    public class AutoRestSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            var typeInfo = context.SystemType.GetTypeInfo();

            if (typeInfo.IsEnum)
            {
                schema.Extensions.Add(
                    "x-ms-enum",
                    new { name = typeInfo.Name, modelAsString = true }
                );
            };
        }
    }
}
