using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.PeliculasAPI.Models;
using System.Security.Claims;

namespace PeliculasAPI
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculaActor>().HasKey(x => new { x.PeliculaId, x.ActorId });
            modelBuilder.Entity<PeliculaGenero>().HasKey(x => new { x.PeliculaId, x.GeneroId });
            modelBuilder.Entity<PeliculaSala>().HasKey(x => new { x.PeliculaId, x.SalaCineId });
            SeedData(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {

            var rolAdminId = "9aae0b6d-d50c-4d0a-9b90-2a6873e3845d";
            var usuarioAdminId = "0c80db90-27ea-4c02-9831-1b3f160548ab";

            var rolAdmin = new IdentityRole()
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };
            

            var passwordHasher = new PasswordHasher<IdentityUser>();

            var username = "felipe@hotmail.com";

            var usuarioAdmin = new IdentityUser()
            {
                Id = usuarioAdminId,
                UserName = username,
                NormalizedUserName = username,
                Email = username,
                NormalizedEmail = username,
                PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
            };

            modelBuilder.Entity<IdentityUser>()
                .HasData(usuarioAdmin);

            modelBuilder.Entity<IdentityRole>()
                .HasData(rolAdmin);

            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasData(new IdentityUserClaim<string>()
                {
                    Id = 1,
                    ClaimType = ClaimTypes.Role,
                    UserId = usuarioAdminId,
                    ClaimValue = "Admin"
                });

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

           modelBuilder.Entity<SalaCine>()
               .HasData(new List<SalaCine>
               {
                    //new SalaDeCine{Id = 1, Nombre = "Agora", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))},
                    new SalaCine{Id = 4, Nombre = "Sambil", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.9118804, 18.4826214))},
                    new SalaCine{Id = 5, Nombre = "Megacentro", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.856427, 18.506934))},
                    new SalaCine{Id = 6, Nombre = "Village East Cinema", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-73.986227, 40.730898))}
               });

            var aventura = new Genero() { Id = 4, Nombre = "Aventura" };
            var animation = new Genero() { Id = 5, Nombre = "Animación" };
            var suspenso = new Genero() { Id = 6, Nombre = "Suspenso" };
            var romance = new Genero() { Id = 7, Nombre = "Romance" };

            modelBuilder.Entity<Genero>()
                .HasData(new List<Genero>
                {
                    aventura, animation, suspenso, romance
                });

            var jimCarrey = new Actor() { Id = 5, Nombre = "Jim Carrey", FechaNacimiento = new DateTime(1962, 01, 17) };
            var robertDowney = new Actor() { Id = 6, Nombre = "Robert Downey Jr.", FechaNacimiento = new DateTime(1965, 4, 4) };
            var chrisEvans = new Actor() { Id = 7, Nombre = "Chris Evans", FechaNacimiento = new DateTime(1981, 06, 13) };

            modelBuilder.Entity<Actor>()
                .HasData(new List<Actor>
                {
                    jimCarrey, robertDowney, chrisEvans
                });

            var endgame = new Pelicula()
            {
                Id = 2,
                Titulo = "Avengers: Endgame",
                EnCines = true,
                FechaEstreno = new DateTime(2019, 04, 26)
            };

            var iw = new Pelicula()
            {
                Id = 3,
                Titulo = "Avengers: Infinity Wars",
                EnCines = false,
                FechaEstreno = new DateTime(2019, 04, 26)
            };

            var sonic = new Pelicula()
            {
                Id = 4,
                Titulo = "Sonic the Hedgehog",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 02, 28)
            };
            var emma = new Pelicula()
            {
                Id = 5,
                Titulo = "Emma",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 02, 21)
            };
            var wonderwoman = new Pelicula()
            {
                Id = 6,
                Titulo = "Wonder Woman 1984",
                EnCines = false,
                FechaEstreno = new DateTime(2020, 08, 14)
            };

            modelBuilder.Entity<Pelicula>()
                .HasData(new List<Pelicula>
                {
                    endgame, iw, sonic, emma, wonderwoman
                });

            modelBuilder.Entity<PeliculaGenero>().HasData(
                new List<PeliculaGenero>()
                {
                    new PeliculaGenero(){PeliculaId = endgame.Id, GeneroId = suspenso.Id},
                    new PeliculaGenero(){PeliculaId = endgame.Id, GeneroId = aventura.Id},
                    new PeliculaGenero(){PeliculaId = iw.Id, GeneroId = suspenso.Id},
                    new PeliculaGenero(){PeliculaId = iw.Id, GeneroId = aventura.Id},
                    new PeliculaGenero(){PeliculaId = sonic.Id, GeneroId = aventura.Id},
                    new PeliculaGenero(){PeliculaId = emma.Id, GeneroId = suspenso.Id},
                    new PeliculaGenero(){PeliculaId = emma.Id, GeneroId = romance.Id},
                    new PeliculaGenero(){PeliculaId = wonderwoman.Id, GeneroId = suspenso.Id},
                    new PeliculaGenero(){PeliculaId = wonderwoman.Id, GeneroId = aventura.Id},
                });

            modelBuilder.Entity<PeliculaSala>().HasData(
                new List<PeliculaSala>()
                {
                    new PeliculaSala(){PeliculaId = sonic.Id, SalaCineId = 4},
                    new PeliculaSala(){PeliculaId = wonderwoman.Id, SalaCineId = 5}
                });

            modelBuilder.Entity<PeliculaActor>().HasData(
                new List<PeliculaActor>()
                {
                    new PeliculaActor(){PeliculaId = endgame.Id, ActorId = robertDowney.Id, Personaje = "Tony Stark", Orden = 1},
                    new PeliculaActor(){PeliculaId = endgame.Id, ActorId = chrisEvans.Id, Personaje = "Steve Rogers", Orden = 2},
                    new PeliculaActor(){PeliculaId = iw.Id, ActorId = robertDowney.Id, Personaje = "Tony Stark", Orden = 1},
                    new PeliculaActor(){PeliculaId = iw.Id, ActorId = chrisEvans.Id, Personaje = "Steve Rogers", Orden = 2},
                    new PeliculaActor(){PeliculaId = sonic.Id, ActorId = jimCarrey.Id, Personaje = "Dr. Ivo Robotnik", Orden = 1}
                });
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<SalaCine> SalasCines { get; set; }
        public DbSet<PeliculaActor> PeliculasActores { get; set; }
        public DbSet<PeliculaGenero> PeliculasGeneros { get; set; }
        public DbSet<PeliculaSala> PeliculasSalas { get; set; }
    }
}
