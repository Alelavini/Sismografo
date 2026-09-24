using System;

namespace RedSismica
{
    // Patrón State: cada subclase representa un estado concreto del evento sísmico y decide
    // qué transiciones admite. Por defecto toda transición es inválida; cada estado
    // sobrescribe sólo las operaciones que tienen sentido para él.
    public abstract class Estado
    {
        public const string AmbitoEventoSismico = "Evento Sismico";

        private readonly string nombre;
        private readonly string ambito;

        protected Estado(string nombre)
        {
            this.nombre = nombre;
            ambito = AmbitoEventoSismico;
        }

        public string getNombre() => nombre;
        public string getAmbito() => ambito;
        public bool esAmbitoEventoSismico() => ambito == AmbitoEventoSismico;

        public virtual bool esAutoDetectado() => false;
        public virtual bool esBloqueadoEnRevision() => false;
        public virtual bool esRechazado() => false;
        public virtual bool esFinal() => false;

        // ---------- Transiciones ----------

        public virtual void bloquear(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            throw transicionInvalida("bloquear");

        public virtual void liberar(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            throw transicionInvalida("liberar");

        public virtual void confirmar(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            throw transicionInvalida("confirmar");

        public virtual void rechazar(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            throw transicionInvalida("rechazar");

        public virtual void derivarAExperto(EventoSismico evento, DateTime fechaHora, Usuario usuario) =>
            throw transicionInvalida("derivar a experto");

        // Cierra el cambio de estado vigente, registra uno nuevo y deja al evento en el próximo estado
        protected void cambiarEstado(EventoSismico evento, Estado proximoEstado, DateTime fechaHora, Usuario usuario)
        {
            evento.buscarActualCE()?.setFechaHoraFin(fechaHora);
            evento.agregarCambioEstado(new CambioEstado(fechaHora, null, proximoEstado, usuario));
            evento.setEstadoActual(proximoEstado);
        }

        private TransicionInvalidaException transicionInvalida(string operacion) =>
            new($"No se puede {operacion} un evento en estado \"{nombre}\".");
    }

    public class TransicionInvalidaException : InvalidOperationException
    {
        public TransicionInvalidaException(string mensaje) : base(mensaje) { }
    }
}
