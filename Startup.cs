using BaseApi.WebApi.Features.Auth;
using BaseApi.WebApi.Features.Common;
using BaseApi.WebApi.Features.Users;
using BaseApi.WebApi.Features.Users.Services;
using BaseApi.WebApi.Helpers;
using BaseApi.WebApi.Infraestructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace BaseApi.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuración de Swagger para documentación de la API
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BaseApi.WebApi", Version = "v1" });
            });

            // Configuración del contexto de base de datos
            services.AddDbContext<BaseApiDbContext>(
                dbContextOptions => dbContextOptions
                    .UseSqlServer(Configuration.GetConnectionString("dbBaseApi"))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            // Registro de servicios mediante inyección de dependencias
            services.AddScoped<HanaDbContext>();
            services.AddTransient<AuthService, AuthService>();
            services.AddTransient<UserService, UserService>();
            services.AddTransient<CommonService, CommonService>();
            services.AddTransient<RoleService, RoleService>();
            services.AddTransient<PermissionService, PermissionService>();

            // Configuración de autenticación mediante token
            services.AddTokenAuthentication(Configuration);

            // Agrega controladores MVC a los servicios
            services.AddControllers();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgromoneyAppApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Habilitamos los cords para usar en la web OJO Solo en pruebas en produccion hay que especificiar el origen
            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BaseApi.WebApi v1"));
            app.UseAuthentication();
            app.UseAuthorization();
       

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
