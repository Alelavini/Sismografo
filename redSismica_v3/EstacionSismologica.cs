using System;

namespace RedSismicaWinForms
{
    public class EstacionSismologica
    {
        private string codigoEstacion;
        private string nombreEstacion;
        private string documentoCertificacionAdq;
        private string fechaSolicitudCertificacion;
        private double latitud;
        private double longitud;
        private string nroCertificacionAdq;

        public EstacionSismologica(
            string codigo,
            string nombre,
            string documentoCert,
            string fechaSolicitud,
            double latitud,
            double longitud,
            string nroCert
      )
        {
            this.codigoEstacion = codigo;
            this.nombreEstacion = nombre;
            this.documentoCertificacionAdq = documentoCert;
            this.fechaSolicitudCertificacion = fechaSolicitud;
            this.latitud = latitud;
            this.longitud = longitud;
            this.nroCertificacionAdq = nroCert;
        }

        public string obtenerCodigoEstacion() => codigoEstacion;
        public string obtenerNombreEstacion() => nombreEstacion;

        // Getters opcionales
        public string getDocumentoCertificacionAdq() => documentoCertificacionAdq;
        public string getFechaSolicitudCertificacion() => fechaSolicitudCertificacion;
        public double getLatitud() => latitud;
        public double getLongitud() => longitud;
        public string getNroCertificacionAdq() => nroCertificacionAdq;
    }
}
