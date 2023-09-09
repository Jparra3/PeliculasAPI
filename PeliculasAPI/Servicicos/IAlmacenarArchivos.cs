namespace PeliculasAPI.Servicicos
{
    public interface IAlmacenarArchivos
    {
        //un contenedor es una carpeta en la cual se puede guardar unos archivos relacionados
        Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor,
            string contentType);
        Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor,
            string ruta, string contentType);
        Task BorrarArchivo(string ruta, string contenedor);
    }
}
