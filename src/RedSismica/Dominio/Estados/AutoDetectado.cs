using System;

namespace RedSismica
{
    // Estado inicial: el evento fue detectado por la red y espera revisión
    public class AutoDetectado : Estado
    {
        public const string Nombre = "Auto Detectado";

        public AutoDetectado() : base(Nombre) { }

        public override bool esAutoDetectado() => true;

        public override void bloquear(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            cambiarEstado(evento, new BloqueadoEnRevision(), fechaHora, usuario);
    }
}
