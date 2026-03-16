using System;
using System.Collections.Generic;
using System.Linq;

namespace RedSismicaWinForms
{
    public class EventoSismico
    {
        private DateTime fechaHoraOcurrencia;
        private DateTime fechaHoraFin;

        private Estado estadoActual;
        private List<CambioEstado> cambiosDeEstado = new List<CambioEstado>();
        private List<SerieTemporal> seriesTemporales = new List<SerieTemporal>();

        // Coordenadas y magnitud simple (para mock o visualización rápida)
        private double latEpicentro, longEpicentro, profEpicentro;
        private double latHipocentro, longHipocentro, profHipocentro;
        private double magnitud;

        // Relaciones con otras clases del dominio
        private MagnitudRitcher magnitudRitcher;
        private ClasificacionSismo clasificacion;
        private AlcanceSismo alcance;
        private OrigenDeGeneracion origen;

        // Constructor principal
        public EventoSismico(DateTime fechaHora, double latE, double longE, double profE,
                             double latH, double longH, double profH, double magnitud)
        {
            this.fechaHoraOcurrencia = fechaHora;
            this.latEpicentro = latE;
            this.longEpicentro = longE;
            this.profEpicentro = profE;
            this.latHipocentro = latH;
            this.longHipocentro = longH;
            this.profHipocentro = profH;
            this.magnitud = magnitud;
        }

        // Getters de datos simples
        public DateTime getFechaHoraOcurrencia() => fechaHoraOcurrencia;
        public DateTime getFechaHoraFin() => fechaHoraFin;
        public void setFechaHoraFin(DateTime fecha) => fechaHoraFin = fecha;

        public double getLatitudEpicentro() => latEpicentro;
        public double getLongitudEpicentro() => longEpicentro;
        public double getLongitudHipocentro() => longHipocentro;
        public double getValorMagnitud() => magnitud;

        // Relación con series temporales
        public List<SerieTemporal> getSerieTemporal() => seriesTemporales;
        public void agregarSerieTemporal(SerieTemporal st) => seriesTemporales.Add(st);

        // Cambio de estado
        public CambioEstado buscarActualCE(List<CambioEstado> cambiosDeEstado)
        {
            CambioEstado cambioActual = cambiosDeEstado.LastOrDefault();

            return cambioActual;
        }
        public void crearNuevoCambioEstado(Estado nuevoEstado, Usuario usuario)
        {
            if (cambiosDeEstado.Count > 0)
                cambiosDeEstado.Last().setFechaHoraFin(DateTime.Now);

            CambioEstado cambio = new CambioEstado(DateTime.Now, null, nuevoEstado, usuario);
            cambiosDeEstado.Add(cambio);
            estadoActual = nuevoEstado;
        }

        public void setEstado(string descripcionNuevoEstado, Usuario usuario)
        {
            CambioEstado actualCE = buscarActualCE(cambiosDeEstado);
            Estado nuevoEstado = new Estado(descripcionNuevoEstado);
            crearNuevoCambioEstado(nuevoEstado, usuario);
        }

        public Estado obtenerEstadoActual() => estadoActual;

        public void bloquearEventoSismico(Usuario usuario)
        {
            setEstado("Bloqueado en revisión", usuario);

        }

        public void rechazar(Usuario usuario)
        {
            setEstado("Rechazado", usuario);
        }

        public bool esEstadoActual(string estado)
        {
            return estadoActual != null && estadoActual.getDescripcion() == estado;
        }

        public bool esAutoDetectado()
        {
            return estadoActual != null && estadoActual.esAutoDetectado();
        }

        // Setters y getters para relaciones agregadas
        public void setMagnitudRitcher(MagnitudRitcher magnitud) => magnitudRitcher = magnitud;
        public void setClasificacion(ClasificacionSismo clasif) => clasificacion = clasif;
        public void setAlcance(AlcanceSismo alc) => alcance = alc;
        public void setOrigenDeGeneracion(OrigenDeGeneracion orig) => origen = orig;

        public MagnitudRitcher getMagnitudRitcher() => magnitudRitcher;
        public ClasificacionSismo getClasificacion() => clasificacion;
        public AlcanceSismo getAlcance() => alcance;
        public OrigenDeGeneracion getOrigenDeGeneracion() => origen;

        // Resumen de datos
        public string getResumenEvento()
        {
            return $"Fecha/Hora: {fechaHoraOcurrencia:dd/MM HH:mm} - Magnitud: {magnitud} - Estado: {estadoActual?.getDescripcion()}";
        }

        public string getDatosEventoSismico()
        {
            return $"Fecha/Hora: {getFechaHoraOcurrencia()}\r\n" +
                   $" Epicentro: Lat {getLatitudEpicentro()}, Long {getLongitudEpicentro()}, Prof {profEpicentro} km\r\n" +
                   $" Hipocentro: Lat {latHipocentro}, Long {getLongitudHipocentro()}, Prof {profHipocentro} km\r\n" +
                   $" Magnitud: {getValorMagnitud()}\r\n" +
                   $" Estado actual: {estadoActual?.getDescripcion()}";
        }

        public string buscarDatosEventosSismicos()
        {
            return getDatosEventoSismico();
        }

        public void modificarDatos(string alcance, double magnitud, string origen)
        {
            
        }

    }
}
