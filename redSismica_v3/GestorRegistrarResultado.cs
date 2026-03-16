using System;
using System.Collections.Generic;
using System.Linq;

namespace RedSismicaWinForms
{
    public class GestorRegistrarResultado
    {
        private List<EventoSismico> eventosSismicosAutoDetectados;
        private EventoSismico eventoSeleccionado;
        private Sesion sesion;
        private Usuario usuarioLogueado => obtenerUsuarioLogueado();

        public GestorRegistrarResultado(Sesion sesion)
        {
            this.sesion = sesion;
            eventosSismicosAutoDetectados = CrearEventosDePrueba();
        }
        public List<EventoSismico> tomarRegistroResultadoRevisionManual()
        {
            // Inicializa variables de control del caso de usoo
            eventoSeleccionado = null;

            eventosSismicosAutoDetectados = obtenerEventosSismicosAutoDetectados();

            return eventosSismicosAutoDetectados;
        }

        public List<EventoSismico> obtenerEventosSismicosAutoDetectados()
        {
            List<EventoSismico> listaAutoDetectados = new List<EventoSismico>();
            foreach (var ev in eventosSismicosAutoDetectados)
            {
                if (ev.esAutoDetectado())
                {
                    listaAutoDetectados.Add(ev);
                }
            }
            // Ordeno la lista por fecha/hora de ocurrencia
            ordenarPorFechaHora(listaAutoDetectados);

            return listaAutoDetectados;
        }

        public List<EventoSismico> ordenarPorFechaHora(List<EventoSismico> lista)
        {
            lista.Sort((a, b) => a.getFechaHoraOcurrencia().CompareTo(b.getFechaHoraOcurrencia()));
            return lista;
        }

        public void tomarSeleccionEvento(EventoSismico evento)
        {
            buscarEstadoBloqueadoEnRevision(evento);

        }

        public void buscarEstadoBloqueadoEnRevision(EventoSismico evento)
        {
            var estado = evento.obtenerEstadoActual();
            
                if (estado.esAmbitoEventoSismico() && estado.esBloqueadoEnRevision())
                {
                    var fechaHoraActual = obtenerFechaHoraActual();
                    bloquearEventoSismico(evento, usuarioLogueado);
                }
            
        }//IMPLEMENTAR

        public DateTime obtenerFechaHoraActual() => DateTime.Now;//IMPLEMENTAR

        public string buscarDatosEventoSismico(EventoSismico evento)
        {
            return evento.getDatosEventoSismico();
        }//IMPLEMENTAR

        //buscarMuestrasSeriesTemporales 

        public void bloquearEventoSismico (EventoSismico evento, Usuario usuario)
        {
            evento.bloquearEventoSismico(usuario);
        }

        public void llamarCuGenerarSismograma() { } 

        public void tomarOpcionVisualizarMapa() { } 

        public void tomarOpcionModificarEvSismico() { } //IMPLEMENTAR

        public void tomarAccionConEvento(string accion)
        {
            // if validarDatosEventoSismico 
            // if validarAccionEvento
            if (eventoSeleccionado == null) return;

            if (accion == "Confirmar")
            {
                eventoSeleccionado.setEstado("Confirmado", usuarioLogueado);
            }
            else if (accion == "Rechazar")
            {
                eventoSeleccionado.setEstado("Rechazado", usuarioLogueado);
            }
            else if (accion == "Solicitar revisión a experto")
            {
                eventoSeleccionado.setEstado("Derivado a Experto", usuarioLogueado);
            }
        }

        // En GestorRegistrarResultado.cs

        public bool validarDatosEventoSismico(int indice)
        {
            if (indice < 0 || indice >= eventosSismicosAutoDetectados.Count)
                return false;

            var evento = eventosSismicosAutoDetectados[indice];

            // Valida que todos existan y no sean vacíos
            bool tieneMagnitud = evento.getMagnitudRitcher() != null;
            bool tieneAlcance = evento.getAlcance() != null;
            bool tieneOrigen = evento.getOrigenDeGeneracion() != null;

            return tieneMagnitud && tieneAlcance && tieneOrigen;
        }

        public bool validarAccionEvento(string accion)
        {
            return !string.IsNullOrWhiteSpace(accion);
        }


        //obtenerFechaHoraActual otra vez

        public Usuario obtenerUsuarioLogueado() => sesion.getUsuarioLogueado();

        public void rechazarEventoSismico(EventoSismico evento)
        {
            if (evento == null) return;
            evento.rechazar(usuarioLogueado);
        }  //NO ESTA BIEN IMPLEMENTADO, REVISAR


        public string buscarDatosEventoSismico(int indice)
        {
            if (indice < 0 || indice >= eventosSismicosAutoDetectados.Count)
                return "";
            return eventosSismicosAutoDetectados[indice].getDatosEventoSismico();
        }
        public List<string> buscarSeriesTemporales(int indice)
        {
            if (indice < 0 || indice >= eventosSismicosAutoDetectados.Count)
                return new List<string>();
            var evento = eventosSismicosAutoDetectados[indice];
            return evento.getSerieTemporal()
                         .Select(st => $"Estación: {st.obtenerCodigoEstacion()} | Sismógrafo: {st.obtenerNombreSismografo()}")
                         .ToList();
        }



        public void modificarDatosEvento(int indice, string alcance, double magnitud, string origen)
        {
            if (indice < 0 || indice >= eventosSismicosAutoDetectados.Count)
                return;
            var evento = eventosSismicosAutoDetectados[indice];
            evento.modificarDatos(alcance, magnitud, origen);
        }




        // Simulación de eventos auto detectados con datos de estación y sismógrafo
        private List<EventoSismico> CrearEventosDePrueba()
        {
            var lista = new List<EventoSismico>();

            // Crear tipos de dato
            var tipo1 = new TipoDeDato("Aceleración", "cm/s²", 5.0);
            var tipo2 = new TipoDeDato("Velocidad", "cm/s", 3.0);

            // Estaciones y sismógrafos
            var fechaSol = "1999";

            var est1 = new EstacionSismologica("EST01", "Córdoba Capital", "cert", fechaSol, 123, 123, "123");
            var est2 = new EstacionSismologica("EST02", "Mendoza Sur", "cert", fechaSol, 123, 123, "123");
            var est3 = new EstacionSismologica("EST03", "Salta Norte", "cert", fechaSol, 123, 123, "123");
            var sis1 = new Sismografo(1, "Sismo-X1", "desc", "nro");
            var sis2 = new Sismografo(2, "Sismo-Alpha", "desc", "nro");
            var sis3 = new Sismografo(3, "Sismo-Beta", "desc", "nro");

            // Series temporales
            var serie1 = new SerieTemporal(false, DateTime.Now.AddSeconds(-60), DateTime.Now, 1.0, est1, sis1);
            var serie2 = new SerieTemporal(false, DateTime.Now.AddSeconds(-60), DateTime.Now, 1.0, est2, sis2);
            var serie3 = new SerieTemporal(false, DateTime.Now.AddSeconds(-60), DateTime.Now, 1.0, est3, sis3);

            // Muestras + detalles para serie1
            var muestra1 = new MuestraSismica(DateTime.Now.AddSeconds(-30));
            muestra1.agregarDetalle(new DetalleMuestraSismica(3.2, tipo1));
            muestra1.agregarDetalle(new DetalleMuestraSismica(2.8, tipo2));
            serie1.agregarMuestra(muestra1);

            // Muestras + detalles para serie2
            var muestra2 = new MuestraSismica(DateTime.Now.AddSeconds(-20));
            muestra2.agregarDetalle(new DetalleMuestraSismica(4.1, tipo1));
            muestra2.agregarDetalle(new DetalleMuestraSismica(1.5, tipo2));
            serie2.agregarMuestra(muestra2);

            // Muestras + detalles para serie3
            var muestra3 = new MuestraSismica(DateTime.Now.AddSeconds(-10));
            muestra3.agregarDetalle(new DetalleMuestraSismica(2.7, tipo1));
            muestra3.agregarDetalle(new DetalleMuestraSismica(2.0, tipo2));
            serie3.agregarMuestra(muestra3);

            // Eventos con series
            var evento1 = new EventoSismico(DateTime.Now.AddMinutes(-20), 31.4, -64.2, 10.1, 31.3, -64.1, 10.2, 4.5);
            evento1.setEstado("Auto Detectado", usuarioLogueado);
            evento1.agregarSerieTemporal(serie1);

            var evento2 = new EventoSismico(DateTime.Now.AddMinutes(-15), -32.9, -68.8, 12.0, -32.8, -68.7, 11.9, 4.2);
            evento2.setEstado("Auto Detectado", usuarioLogueado);
            evento2.agregarSerieTemporal(serie2);

            var evento3 = new EventoSismico(DateTime.Now.AddMinutes(-10), -24.7, -65.4, 8.7, -24.6, -65.5, 8.9, 4.0);
            evento3.setEstado("Auto Detectado", usuarioLogueado);
            evento3.agregarSerieTemporal(serie3);

            lista.Add(evento1);
            lista.Add(evento2);
            lista.Add(evento3);

            return lista;
        }
    }
}
