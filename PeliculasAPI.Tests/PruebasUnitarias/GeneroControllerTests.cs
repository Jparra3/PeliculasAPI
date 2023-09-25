using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class GeneroControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            //preparación
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);

            //prueba
            var controller = new GenerosController(contexto2, mapper);
            var respuesta = await controller.Get();

            //verificación
            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {
            //preparación
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            //prueba
            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Get(1);

            //verificación
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            //preparación
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Generos.Add(new Genero() { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync();

            

            //prueba
            var contexto2 = ConstruirContext(nombreBd);
            var controller = new GenerosController(contexto2, mapper);
            var id = 1;
            var respuesta = await controller.Get(id);

            //verificación
            var resultado = respuesta.Value;
            Assert.AreEqual(id, resultado.Id);
        }

        [TestMethod]
        public async Task CrearGenero()
        {
            //preparación
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            var nuevoGenero = new GeneroCreateDto() { Nombre = "new Genero" };

            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Post(nuevoGenero);

            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreBd);
            var cantidad = await contexto2.Generos.CountAsync();
            Assert.AreEqual(1, cantidad);
        }

        [TestMethod]
        public async Task ActualizarGenero()
        {
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);
            var controller = new GenerosController(contexto2, mapper);

            var updateName = "Nuevo Nombre";
            var generoUpdateDto = new GeneroUpdateDto() { Nombre = updateName };

            var id = 1;
            var respuesta = await controller.Put(id, generoUpdateDto);

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBd);
            var existe = await contexto3.Generos.AnyAsync(x => x.Nombre == updateName);
            Assert.IsTrue(existe);
        }

        [TestMethod]
        public async Task IntentaBorrarGeneroNoExistente()
        {
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(contexto, mapper);

            var respuesta =  await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task IntentaBorrarGeneroExistente()
        {
            var nombreBd = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBd);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Nombre = "Genero 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBd);

            var controller = new GenerosController(contexto2, mapper);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBd);
            var existe = await contexto3.Generos.AnyAsync();
            Assert.IsFalse(existe);

        }
    }
}
