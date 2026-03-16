using System;
using System.Collections.Generic;

namespace RedSismicaWinForms
{
    public class MuestraSismica
    {
        private DateTime fechaHoraMuestra;
        private List<DetalleMuestraSismica> detalles;

        public MuestraSismica(DateTime fechaHoraMuestra)
        {
            this.fechaHoraMuestra = fechaHoraMuestra;
            this.detalles = new List<DetalleMuestraSismica>();
        }

        public List<DetalleMuestraSismica> obtenerDetallesMuestra()
        {
            return detalles;
        }

        public DateTime getFechaHoraMuestra()
        {
            return fechaHoraMuestra;
        }

        public void setFechaHoraMuestra(DateTime nuevaFecha)
        {
            fechaHoraMuestra = nuevaFecha;
        }

        public void agregarDetalle(DetalleMuestraSismica detalle)
        {
            detalles.Add(detalle);
        }
    }
}
