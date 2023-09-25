using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;
using System.Security.Claims;

namespace PeliculasAPI
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options): base (options)
        {

        }

        //implementar el FluentApi
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //crea la llave primaria compuesta para la relación de muchos a muchos
            modelBuilder.Entity<PeliculasActores>()
                .HasKey(x => new { x.ActorId, x.PeliculaId });

            modelBuilder.Entity<PeliculasGeneros>()
                .HasKey(x => new {x.GeneroId, x.PeliculaId });

            modelBuilder.Entity<PeliculasSalasDeCine>()
                .HasKey(x => new { x.PeliculaId, x.SalaDeCineId });

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var rolAdminId = "4218f49a-896e-4360-aea4-f0cc4a4c7861";
            var usuarioAdminId = "e701deef-1235-4fae-99b5-c2c3e11104a3";

            var rolAdmin = new IdentityRole()
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();
            var userName = "jhonatanjopa@gmail.com";

            var usuarioAdmin = new IdentityUser()
            {
                Id = usuarioAdminId,
                UserName = userName,
                NormalizedUserName = userName,
                Email = userName,
                NormalizedEmail = userName,
                PasswordHash = passwordHasher.HashPassword(null, "A123456")
            };

            //modelBuilder.Entity<IdentityUser>()
            //    .HasData(usuarioAdmin);

            //modelBuilder.Entity<IdentityRole>()
            //    .HasData(rolAdmin);

            //modelBuilder.Entity<IdentityUserClaim<string>>()
            //    .HasData(new IdentityUserClaim<string>()
            //    {
            //        Id = 1,
            //        ClaimType = ClaimTypes.Role,
            //        UserId = usuarioAdminId,
            //        ClaimValue = "Admin"
            //    });

        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<PeliculasActores> PeliculasActores { get; set; }
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
        public DbSet<SalaDeCine> SalaDeCines { get; set; }
        public DbSet<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }

        public DbSet<Review> Reviews { get; set; }

    }
}
