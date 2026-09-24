using System;

namespace RedSismica
{
    // Un analista está revisando el evento: desde acá se registra el resultado o se lo libera
    public class BloqueadoEnRevision : Estado
    {
        public const string Nombre = "Bloqueado en revisión";

        public BloqueadoEnRevision() : base(Nombre) { }

        public override bool esBloqueadoEnRevision() => true;

        public override void liberar(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            cambiarEstado(evento, new AutoDetectado(), fechaHora, usuario);

        public override void confirmar(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            finalizarRevision(evento, new Confirmado(), fechaHora, usuario);

        public override void rechazar(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            finalizarRevision(evento, new Rechazado(), fechaHora, usuario);

        public override void derivarAExperto(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            finalizarRevision(evento, new DerivadoAExperto(), fechaHora, usuario);

        private void finalizarRevision(EventoSismico evento, Estado estadoFinal, DateTime fechaHora, Usuario usuario)
        {
            cambiarEstado(evento, estadoFinal, fechaHora, usuario);
            evento.setFechaHoraFin(fechaHora);
        }
    }
}
