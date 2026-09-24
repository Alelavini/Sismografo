using System;
using System.Collections.Generic;
using System.Linq;

namespace RedSismica
{
    public class EventoSismico
    {
        private DateTime fechaHoraOcurrencia;
        private DateTime? fechaHoraFin;

        private Estado? estadoActual;
        private List<CambioEstado> cambiosDeEstado = new List<CambioEstado>();
        private List<SerieTemporal> seriesTemporales = new List<SerieTemporal>();

        private double latEpicentro, longEpicentro, profEpicentro;
        private double latHipocentro, longHipocentro, profHipocentro;

        // Relaciones con otras clases del dominio
        private MagnitudRichter magnitud;
        private ClasificacionSismo clasificacion;
        private AlcanceSismo? alcance;
        private OrigenDeGeneracion? origen;

        public EventoSismico(DateTime fechaHora, double latE, double longE, double profE,
                             double latH, double longH, double profH, double valorMagnitud)
        {
            this.fechaHoraOcurrencia = fechaHora;
            this.latEpicentro = latE;
            this.longEpicentro = longE;
            this.profEpicentro = profE;
            this.latHipocentro = latH;
            this.longHipocentro = longH;
            this.profHipocentro = profH;
            this.magnitud = MagnitudRichter.desdeValor(valorMagnitud);
            this.clasificacion = ClasificacionSismo.desdeProfundidad(profH);
        }

        // Datos del evento
        public DateTime getFechaHoraOcurrencia() => fechaHoraOcurrencia;
        public DateTime? getFechaHoraFin() => fechaHoraFin;

        public double getLatitudEpicentro() => latEpicentro;
        public double getLongitudEpicentro() => longEpicentro;
        public double getProfundidadEpicentro() => profEpicentro;
        public double getLatitudHipocentro() => latHipocentro;
        public double getLongitudHipocentro() => longHipocentro;
        public double getProfundidadHipocentro() => profHipocentro;
        public double getValorMagnitud() => magnitud.getNumero();

        public MagnitudRichter getMagnitudRichter() => magnitud;
        public ClasificacionSismo getClasificacion() => clasificacion;
        public AlcanceSismo? getAlcance() => alcance;
        public OrigenDeGeneracion? getOrigenDeGeneracion() => origen;

        public void setAlcance(AlcanceSismo alc) => alcance = alc;
        public void setOrigenDeGeneracion(OrigenDeGeneracion orig) => origen = orig;

        // Series temporales
        public List<SerieTemporal> getSerieTemporal() => seriesTemporales;
        public void agregarSerieTemporal(SerieTemporal st) => seriesTemporales.Add(st);

        // Estados
        public Estado? obtenerEstadoActual() => estadoActual;
        public IReadOnlyList<CambioEstado> getCambiosDeEstado() => cambiosDeEstado;

        public CambioEstado? buscarActualCE() => cambiosDeEstado.FirstOrDefault(ce => ce.sosActual());

        public void crearNuevoCambioEstado(Estado nuevoEstado, Usuario usuario, DateTime fechaHora)
        {
            cambiosDeEstado.Add(new CambioEstado(fechaHora, null, nuevoEstado, usuario));
            estadoActual = nuevoEstado;
        }

        public void setEstado(string descripcionNuevoEstado, Usuario usuario)
        {
            DateTime ahora = DateTime.Now;
            buscarActualCE()?.setFechaHoraFin(ahora);
            crearNuevoCambioEstado(new Estado(descripcionNuevoEstado), usuario, ahora);

            // Los estados de cierre de la revisión finalizan el evento
            if (descripcionNuevoEstado is Estado.Confirmado or Estado.Rechazado or Estado.DerivadoAExperto)
                fechaHoraFin = ahora;
        }

        public void bloquearEventoSismico(Usuario usuario) => setEstado(Estado.BloqueadoEnRevision, usuario);
        public void liberarEventoSismico(Usuario usuario) => setEstado(Estado.AutoDetectado, usuario);
        public void rechazar(Usuario usuario) => setEstado(Estado.Rechazado, usuario);
        public void confirmar(Usuario usuario) => setEstado(Estado.Confirmado, usuario);
        public void derivarAExperto(Usuario usuario) => setEstado(Estado.DerivadoAExperto, usuario);

        public bool esEstadoActual(string estado) => estadoActual?.getDescripcion() == estado;
        public bool esAutoDetectado() => estadoActual != null && estadoActual.esAutoDetectado();
        public bool esBloqueadoEnRevision() => estadoActual != null && estadoActual.esBloqueadoEnRevision();

        // Un evento sólo puede cerrarse si tiene magnitud, alcance y origen de generación
        public bool tieneDatosCompletos() => alcance != null && origen != null && magnitud.getNumero() > 0;

        public void modificarDatos(string nombreAlcance, double valorMagnitud, string nombreOrigen)
        {
            magnitud = MagnitudRichter.desdeValor(valorMagnitud);
            alcance = new AlcanceSismo(nombreAlcance, nombreAlcance);
            origen = new OrigenDeGeneracion(nombreOrigen, nombreOrigen);
        }

        public string getDatosEventoSismico()
        {
            return $"Fecha/Hora: {fechaHoraOcurrencia:dd/MM/yyyy HH:mm:ss}\r\n" +
                   $"Epicentro: Lat {latEpicentro}, Long {longEpicentro}, Prof {profEpicentro} km\r\n" +
                   $"Hipocentro: Lat {latHipocentro}, Long {longHipocentro}, Prof {profHipocentro} km\r\n" +
                   $"Magnitud: {magnitud.getNumero():0.0} ({magnitud.getDescripcion()})\r\n" +
                   $"Clasificación: {clasificacion.getNombre()}\r\n" +
                   $"Alcance: {alcance?.getNombre() ?? "-"}\r\n" +
                   $"Origen: {origen?.getNombre() ?? "-"}\r\n" +
                   $"Estado actual: {estadoActual?.getDescripcion()}";
        }
    }
}
