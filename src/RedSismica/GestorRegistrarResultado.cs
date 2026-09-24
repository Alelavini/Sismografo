using System;
using System.Collections.Generic;
using System.Linq;

namespace RedSismica
{
    // Controlador del caso de uso "Registrar resultado de revisión manual"
    public class GestorRegistrarResultado
    {
        public const string AccionConfirmar = "Confirmar evento";
        public const string AccionRechazar = "Rechazar evento";
        public const string AccionDerivar = "Solicitar revisión a experto";
        public static readonly string[] Acciones = { AccionConfirmar, AccionRechazar, AccionDerivar };

        private List<EventoSismico> eventosSismicos;
        private EventoSismico? eventoSeleccionado;
        private Sesion sesion;

        public GestorRegistrarResultado(Sesion sesion, List<EventoSismico>? eventos = null)
        {
            this.sesion = sesion;
            eventosSismicos = eventos ?? DatosDePrueba.crearEventosSismicos(sesion.getUsuarioLogueado());
        }

        public Usuario obtenerUsuarioLogueado() => sesion.getUsuarioLogueado();
        public EventoSismico? getEventoSeleccionado() => eventoSeleccionado;

        public List<EventoSismico> tomarRegistroResultadoRevisionManual()
        {
            liberarEventoSeleccionado();
            return buscarEventosSismicosAutoDetectados();
        }

        public List<EventoSismico> buscarEventosSismicosAutoDetectados()
        {
            var autoDetectados = eventosSismicos.Where(ev => ev.esAutoDetectado()).ToList();
            return ordenarPorFechaHora(autoDetectados);
        }

        public List<EventoSismico> ordenarPorFechaHora(List<EventoSismico> lista)
        {
            lista.Sort((a, b) => a.getFechaHoraOcurrencia().CompareTo(b.getFechaHoraOcurrencia()));
            return lista;
        }

        // Al seleccionar un evento se lo bloquea para que ningún otro analista lo revise en paralelo
        public void tomarSeleccionEvento(EventoSismico evento)
        {
            if (evento == eventoSeleccionado) return;

            liberarEventoSeleccionado();

            if (!evento.esAutoDetectado())
                throw new InvalidOperationException("Sólo se pueden revisar eventos auto detectados.");

            bloquearEventoSismico(evento);
            eventoSeleccionado = evento;
        }

        public void bloquearEventoSismico(EventoSismico evento) => evento.bloquearEventoSismico(obtenerUsuarioLogueado());

        // Si el analista abandona la revisión, el evento vuelve a quedar disponible
        public void liberarEventoSeleccionado()
        {
            if (eventoSeleccionado != null && eventoSeleccionado.esBloqueadoEnRevision())
                eventoSeleccionado.liberarEventoSismico(obtenerUsuarioLogueado());
            eventoSeleccionado = null;
        }

        public string buscarDatosEventoSismico() => eventoSeleccionado?.getDatosEventoSismico() ?? "";

        public List<SerieTemporal> buscarSeriesTemporales()
        {
            if (eventoSeleccionado == null) return new List<SerieTemporal>();
            return eventoSeleccionado.getSerieTemporal()
                                     .OrderBy(st => st.obtenerCodigoEstacion())
                                     .ToList();
        }

        public void modificarDatosEvento(string alcance, double magnitud, string origen)
        {
            if (eventoSeleccionado == null)
                throw new InvalidOperationException("No hay un evento seleccionado.");
            if (magnitud <= 0)
                throw new ArgumentOutOfRangeException(nameof(magnitud), "La magnitud debe ser mayor a cero.");

            eventoSeleccionado.modificarDatos(alcance, magnitud, origen);
        }

        public bool validarDatosEventoSismico() => eventoSeleccionado != null && eventoSeleccionado.tieneDatosCompletos();

        public bool validarAccionEvento(string? accion) => accion != null && Acciones.Contains(accion);

        public ResultadoRevision tomarAccionConEvento(string? accion)
        {
            if (eventoSeleccionado == null)
                return ResultadoRevision.Error("Seleccioná un evento para revisar.");
            if (!validarAccionEvento(accion))
                return ResultadoRevision.Error("Seleccioná una acción válida.");
            if (!validarDatosEventoSismico())
                return ResultadoRevision.Error("El evento no tiene alcance y origen cargados. Completalos con \"Modificar datos\".");

            var usuario = obtenerUsuarioLogueado();
            switch (accion)
            {
                case AccionConfirmar: eventoSeleccionado.confirmar(usuario); break;
                case AccionRechazar: eventoSeleccionado.rechazar(usuario); break;
                case AccionDerivar: eventoSeleccionado.derivarAExperto(usuario); break;
            }

            string estado = eventoSeleccionado.obtenerEstadoActual()!.getDescripcion();
            eventoSeleccionado = null;
            return ResultadoRevision.Ok($"Resultado registrado: el evento quedó en estado \"{estado}\".");
        }
    }

    public record ResultadoRevision(bool Exito, string Mensaje)
    {
        public static ResultadoRevision Ok(string mensaje) => new(true, mensaje);
        public static ResultadoRevision Error(string mensaje) => new(false, mensaje);
    }
}
