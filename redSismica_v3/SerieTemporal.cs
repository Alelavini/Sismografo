using System;
using System.Collections.Generic;

namespace RedSismicaWinForms
{
    public class SerieTemporal
    {
        private bool condicionAlarma;
        private DateTime fechaHoraInicioRegistroMuestras;
        private DateTime fechaHoraRegistro;
        private double frecuenciaMuestreo;

        private List<MuestraSismica> muestras;

        private EstacionSismologica estacion;
        private Sismografo sismografo;

        public SerieTemporal(
            bool condicionAlarma,
            DateTime inicioRegistro,
            DateTime registro,
            double frecuencia,
            EstacionSismologica estacion,
            Sismografo sismografo)
        {
            this.condicionAlarma = condicionAlarma;
            this.fechaHoraInicioRegistroMuestras = inicioRegistro;
            this.fechaHoraRegistro = registro;
            this.frecuenciaMuestreo = frecuencia;
            this.estacion = estacion;
            this.sismografo = sismografo;
            this.muestras = new List<MuestraSismica>();
        }

        public List<MuestraSismica> obtenerMuestras()
        {
            return muestras;
        }

        public void agregarMuestra(MuestraSismica muestra)
        {
            muestras.Add(muestra);
        }

        // Métodos para obtener información relacionada
        public string obtenerCodigoEstacion()
        {
            return estacion.obtenerCodigoEstacion();
        }

        public string obtenerNombreSismografo()
        {
            return sismografo.getNombreSismografo();
        }

        // Getters opcionales
        public bool getCondicionAlarma() => condicionAlarma;
        public DateTime getFechaHoraInicioRegistroMuestras() => fechaHoraInicioRegistroMuestras;
        public DateTime getFechaHoraRegistro() => fechaHoraRegistro;
        public double getFrecuenciaMuestreo() => frecuenciaMuestreo;
    }
}
