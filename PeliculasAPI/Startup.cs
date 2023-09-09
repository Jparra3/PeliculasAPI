using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PeliculasAPI.Servicicos;

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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddControllers();

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
