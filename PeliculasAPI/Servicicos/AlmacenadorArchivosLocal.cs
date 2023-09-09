namespace PeliculasAPI.Servicicos
{
    public class AlmacenadorArchivosLocal : IAlmacenarArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            //env vamos a poder obtener la ruta donde se encuentra nuestro wwwroot para asi poder colocar archivos en dicha carpeta
            this.env = env;
            //httpContextAccessor  vamos a poder determinar el dominio donde tenemos publicado nuestra web api y asi poder construir la url y esa es la que se va a guardar en la BD
            this.httpContextAccessor = httpContextAccessor;
        }
        public Task BorrarArchivo(string ruta, string contenedor)
        {
            if(ruta != null)
            {
                var nombreArchivo = Path.GetFileName(ruta);
                string directorioArchivo = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);

                if (File.Exists(directorioArchivo))
                {
                    File.Delete(directorioArchivo);
                }
            }

            return Task.FromResult(0);
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, 
            string ruta, string contentType)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenido, extension, contenedor, contentType);
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
        {
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, contenedor);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);
            await File.WriteAllBytesAsync(ruta, contenido);

            var urlActual = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";

            var urlParaBD = Path.Combine(urlActual, contenedor, nombreArchivo).Replace("\\", "/");
            return urlParaBD;
        }
    }
}
