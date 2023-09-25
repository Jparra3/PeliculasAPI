using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicicos;
using System.Text;

namespace PeliculasAPI
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

            services.AddAutoMapper(typeof(Startup));

            //↓↓↓ configurar para almacenar archivos ↓↓↓
            services.AddTransient<IAlmacenarArchivos, AlmacenadorArchivosLocal>();
            services.AddHttpContextAccessor();
            // ↑↑↑↑

            //↓↓↓↓ Registrar una vlidacion para saber si existe el id de una pelicula
            services.AddScoped<PeliculaExisteAttribute>();

            //↓↓↓↓↓
            //Se configura el geometry factory para ser usado en el AutoMapper sin estar usando el hardCode
            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

            //se utiliza para usar inyección de dependencias
            services.AddSingleton(provider =>
                new MapperConfiguration(config =>{
                    var geometryFactpry = provider.GetRequiredService<GeometryFactory>();
                    config.AddProfile(new AutoMapperProfiles(geometryFactpry));
                }).CreateMapper()
            );

            //↑↑↑↑↑

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                //se pueden utilizar tipos de datos de la librería netTopolySuite
                sqlServerOptions => sqlServerOptions.UseNetTopologySuite()
            ));

            services.AddControllers()
                .AddNewtonsoftJson(); //es para agregar el tema del patch, pero no funciona


            //↓↓↓↓↓↓↓↓  Agregar IdentityUser Tema de autenticación ↓↓↓↓↓↓↓↓  
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                    ClockSkew = TimeSpan.Zero
                });
            //↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

            services.AddEndpointsApiExplorer();                   

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PeliculasAPI",
                    Version = "v1",
                    Description = "Este es un web api para las peliculas API", //se puede agregar una descripción 
                    Contact = new OpenApiContact
                    {
                        Email = "jhonatanjopa@gmail.com",
                        Name = "Jhonatan",
                        Url = new Uri("https://www.linkedin.com/in/jhonatan-parra-almario/")
                    },//se puede agregar información de contacto
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    }

                });
            });

            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(); //para poder ver los archivos
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
