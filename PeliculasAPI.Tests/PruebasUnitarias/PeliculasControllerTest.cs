using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class PeliculasControllerTest: BasePruebas
    {
        private string CrearDataPrueba()
        {
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var genero = new Genero() { Nombre = "Genero 1" };


            var peliculas = new List<Pelicula>()
            {
                new Pelicula() { Titulo = "Pelicula 1", FechaEstreno = new DateTime(2010, 1,1), EnCines = false},
                new Pelicula() { Titulo = "No estrenada", FechaEstreno = DateTime.Today.AddDays(1), EnCines = false},
                new Pelicula() { Titulo = "Pelicula en Cines", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true}
            };

            var peliculaConGenero = new Pelicula()
            {
                Titulo = "Pelicula con género",
                FechaEstreno = new DateTime(2010, 1, 1),
                EnCines = false
            };
            peliculas.Add(peliculaConGenero);

            contexto.Add(genero);
            contexto.AddRange(peliculas);
            contexto.SaveChanges();

            var peliculaGenero = new PeliculasGeneros() { GeneroId = genero.Id, PeliculaId = peliculaConGenero.Id };
            contexto.Add(peliculaGenero);
            contexto.SaveChanges();

            return nombreBd;
        }

        [TestMethod]
        public async Task FiltrarPorTitulo()
        {
            var nombreDb = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var tituloPelicula = "Pelicula 1";

            var filtroDto = new FiltroPeliculaDto()
            {
                Titulo = tituloPelicula,
                CantidadRegistrosPorPagina = 10,
            };

            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;

            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual(tituloPelicula, peliculas[0].Titulo);
        }


        [TestMethod]
        public async Task FiltrarEnCines()
        {
            var nombreDb = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto()
            {
                EnCines = true
            };

            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;

            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual(true, peliculas[0].EnCines);
            Assert.AreEqual("Pelicula en Cines", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarProximosEstrenos()
        {
            var nombreDb = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto()
            {
                ProximosEstrenos = true
            };

            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;

            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual(false, peliculas[0].EnCines);
            Assert.AreEqual("No estrenada", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarPorGenero()
        {
            var nombreDb = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();


            var generoId = contexto.Generos.Select(x => x.Id).First();

            var filtroDto = new FiltroPeliculaDto()
            {
                GeneroId = generoId
            };

            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;

            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Pelicula con género", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarOrdenaTituloAsc()
        {
            var nombreDb = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto()
            {
                CampoOrdenar = "titulo",
                OrdenAscendente = true
            };

            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDb);
            var peliculasDb = contexto2.Peliculas.OrderBy(x => x.Titulo).ToList();


            Assert.AreEqual(peliculasDb.Count, peliculas.Count);

            for(int i = 0; i < peliculasDb.Count; i++)
            {
                var peliculaController = peliculas[i];
                var peliculaDb = peliculasDb[i];
                
                Assert.AreEqual(peliculaDb.Id, peliculaController.Id);

            }
        }

        [TestMethod]
        public async Task FiltrarOrdenaTituloDesc()
        {
            var nombreDb = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDto = new FiltroPeliculaDto()
            {
                CampoOrdenar = "titulo",
                OrdenAscendente = false
            };

            var respuesta = await controller.Filtrar(filtroDto);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDb);
            var peliculasDb = contexto2.Peliculas.OrderByDescending(x => x.Titulo).ToList();


            Assert.AreEqual(peliculasDb.Count, peliculas.Count);

            for (int i = 0; i < peliculasDb.Count; i++)
            {
                var peliculaController = peliculas[i];
                var peliculaDb = peliculasDb[i];

                Assert.AreEqual(peliculaDb.Id, peliculaController.Id);

            }
        }

        [TestMethod]
        public async Task FiltrarPorCampoIncorrectoDevuelvePelicula()
        {
            var nombreDb = CrearDataPrueba();
            var mapper = ConfigurarAutoMapper();
            var contexto = ConstruirContext(nombreDb);

            var mock = new Mock<ILogger<PeliculasController>>();

            var controller = new PeliculasController(contexto, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtro = new FiltroPeliculaDto()
            {
                CampoOrdenar = "abcd",
                OrdenAscendente = true
            };

            var respuesta = await controller.Filtrar(filtro);
            var peliculas = respuesta.Value;

            var contexto2 = ConstruirContext(nombreDb);
            var peliculasDb = contexto2.Peliculas.ToList();
            Assert.AreEqual(peliculasDb.Count, peliculas.Count);
            Assert.AreEqual(1, mock.Invocations.Count);            
        }

    }
}
