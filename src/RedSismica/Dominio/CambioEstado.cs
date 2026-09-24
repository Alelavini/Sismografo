using System;

namespace RedSismica
{
    public class CambioEstado
    {
        private DateTime fechaHoraInicio;
        private DateTime? fechaHoraFin;
        private Estado estado;
        private Usuario? usuario;

        public CambioEstado(DateTime inicio, DateTime? fin, Estado estado, Usuario? usuario)
        {
            this.fechaHoraInicio = inicio;
            this.fechaHoraFin = fin;
            this.estado = estado;
            this.usuario = usuario;
        }

        public bool sosActual() => fechaHoraFin == null;

        public void setFechaHoraFin(DateTime fecha) => fechaHoraFin = fecha;

        public DateTime getFechaHoraInicio() => fechaHoraInicio;
        public DateTime? getFechaHoraFin() => fechaHoraFin;
        public Estado getEstado() => estado;
        public Usuario? getUsuario() => usuario;
    }
}
