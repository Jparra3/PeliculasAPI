using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class SalaDeCineCercanoFiltroDto
    {
        [Range(-90, 90)]
        public double Latitud { get; set; }
        [Range(-180, 180)]
        public double Longitud { get; set; }
        public int distanciaEnKms = 10;
        public int distanciaMaximaKMS = 50;
        public int DistanciaEnKms 
        {
            get
            {
                return distanciaEnKms;
            }
            set
            {
                distanciaEnKms = (value > distanciaMaximaKMS) ? distanciaMaximaKMS : value;
            }
        }
    }
}
