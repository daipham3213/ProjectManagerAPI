using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectManagerAPI.Core;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Repositories;
using ProjectManagerAPI.Core.ServiceResource;
using ProjectManagerAPI.Core.Services;
using ProjectManagerAPI.Persistence;
using ProjectManagerAPI.Persistence.ReposMocks;
using ProjectManagerAPI.Persistence.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectManagerAPI.Core.Policy;

namespace ProjectManagerAPI
{
    public class Startup
    {
        private readonly string _myAllowSpecificOrigins = "AllowSpecficOrigins";
        private readonly string _signalr = "signalr";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {

                options.AddPolicy(name: _myAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5000", "http://localhost:3000", "https://localhost:5001", "https://daipham3213-001-site1.htempurl.com",
                                "https://prj-manager.herokuapp.com")
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithExposedHeaders("Content-Range");
                    });
                options.AddPolicy(_signalr,
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetIsOriginAllowed(hostName => true));
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
                options.ValidationInterval = TimeSpan.FromSeconds(10));

            //Disable Automatic Model State Validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Use the default property (Pascal) casing
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddDbContext<ProjectManagerDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("LocalDB")));
            services.AddIdentity<User, IdentityRole<Guid>>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_@";
            })
                .AddEntityFrameworkStores<ProjectManagerDbContext>()
                .AddDefaultTokenProviders();


            //Mail Service
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            //AutoMapper
            services.AddAutoMapper(typeof(Startup));

            //Inject interfaces
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IPhotoService, PhotoService>();

            //Inject core model
            services.AddScoped<IGroupTypeRepository, GroupTypeRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IAvatarRepository, AvatarRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IPhaseRepository,  PhaseRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IRefreshToken, RefreshTokenRepository>();

            //Inject Authorizations
            services.AddTransient<IAuthorizationHandler, GroupAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, GroupTypeAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, UserAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, AvatarAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, ProjectAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, PhaseAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, ReportAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, TaskAuthorizationHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project Manager API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.<br>
                                   Enter 'Bearer' [space] and then your token in the text input below.<br>
                                   Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                            Scheme="oauth2",
                            Name="Bearer",
                            In=ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            //Authentication
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            string issuer = Configuration.GetValue<string>("Tokens:Issuer");
            string signingKey = Configuration.GetValue<string>("Tokens:Key");
            byte[] signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);

            var key = new SymmetricSecurityKey(signingKeyBytes);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie(config =>
                {
                    config.Cookie.Name = "refreshToken";
                    config.Cookie.HttpOnly = true;//The cookie cannot be obtained by the front-end or the browser, and can only be modified on the server side
                    config.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;//This cookie cannot be used as a third-party cookie under any circumstances, without exception. For example, suppose b.com sets the following cookies:
                })
               .AddJwtBearer(opt =>
               {
                   opt.RequireHttpsMetadata = false;
                   opt.SaveToken = true;
                   opt.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = key,
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                       ClockSkew = TimeSpan.Zero
                   };
               });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PM API v1"));
            }

            app.UseCors(_myAllowSpecificOrigins);
            app.UseCors(_signalr);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
