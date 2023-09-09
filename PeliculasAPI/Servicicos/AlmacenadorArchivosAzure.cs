namespace PeliculasAPI.Servicicos
{
    public class AlmacenadorArchivosAzure : IAlmacenarArchivos
    {
        public Task BorrarArchivo(string ruta, string contenedor)
        {
            throw new NotImplementedException();
        }

        public Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
        {
            throw new NotImplementedException();
        }
    }
}
