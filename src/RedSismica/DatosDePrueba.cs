using System;
using System.Collections.Generic;

namespace RedSismica
{
    // Simula los eventos que la red sismológica detecta automáticamente
    public static class DatosDePrueba
    {
        private static readonly TipoDeDato Velocidad = new("Velocidad de onda", "km/seg", 7.0);
        private static readonly TipoDeDato Frecuencia = new("Frecuencia de onda", "Hz", 10.0);
        private static readonly TipoDeDato Longitud = new("Longitud de onda", "km/ciclo", 1.0);

        public static List<EventoSismico> crearEventosSismicos(Usuario usuario)
        {
            var zonda = new EstacionSismologica("SJ-ZON", "Zonda, San Juan", "CERT-0112", "2019-03-11", -31.55, -68.73, "0112");
            var uspallata = new EstacionSismologica("MZ-USP", "Uspallata, Mendoza", "CERT-0087", "2018-07-02", -32.59, -69.35, "0087");
            var salta = new EstacionSismologica("SA-CAP", "Salta Capital", "CERT-0145", "2020-01-20", -24.79, -65.41, "0145");
            var choz = new EstacionSismologica("NQ-CHM", "Chos Malal, Neuquén", "CERT-0161", "2021-05-14", -37.38, -70.27, "0161");
            var catamarca = new EstacionSismologica("CT-CAP", "San F. del V. de Catamarca", "CERT-0093", "2018-10-08", -28.47, -65.78, "0093");

            var guralp = new Sismografo(1, "Güralp CMG-3T", "Banda ancha", "G3T-4471");
            var trillium = new Sismografo(2, "Nanometrics Trillium 120", "Banda ancha", "T120-0932");
            var episensor = new Sismografo(3, "Kinemetrics EpiSensor", "Acelerómetro", "EPI-2210");

            DateTime hoy = DateTime.Today;

            var eventos = new List<EventoSismico>
            {
                crear(hoy.AddHours(2).AddMinutes(14), -31.62, -68.58, 110, -31.60, -68.55, 112, 4.6, usuario,
                      "Sismo regional", "Interplaca", serie(zonda, guralp, 6.1, 9.2, 0.7), serie(uspallata, trillium, 5.4, 8.1, 0.6)),
                crear(hoy.AddHours(5).AddMinutes(41), -32.91, -68.85, 12, -32.90, -68.84, 11, 3.8, usuario,
                      "Sismo local", "Intraplaca", serie(uspallata, trillium, 4.2, 12.5, 0.3)),
                crear(hoy.AddHours(7).AddMinutes(3), -24.71, -65.38, 182, -24.70, -65.40, 185, 5.2, usuario,
                      "Sismo regional", null, serie(salta, episensor, 7.4, 6.3, 1.2), serie(catamarca, guralp, 6.8, 5.9, 1.1)),
                crear(hoy.AddHours(9).AddMinutes(27), -37.81, -71.10, 8, -37.80, -71.12, 7, 2.9, usuario,
                      null, "Volcánico", serie(choz, trillium, 3.1, 15.0, 0.2)),
                crear(hoy.AddHours(11).AddMinutes(52), -28.42, -66.81, 140, -28.40, -66.80, 141, 4.1, usuario,
                      "Sismo regional", "Interplaca", serie(catamarca, guralp, 5.6, 7.7, 0.7), serie(zonda, episensor, 4.9, 7.1, 0.7)),
                crear(hoy.AddHours(13).AddMinutes(8), -27.85, -63.25, 575, -27.83, -63.27, 580, 6.3, usuario,
                      "Telesismo", "Interplaca", serie(catamarca, trillium, 8.2, 4.4, 1.9), serie(salta, guralp, 7.9, 4.1, 1.9), serie(zonda, episensor, 7.1, 3.8, 1.9)),
            };

            // Un evento que ya fue revisado: no debe aparecer en la lista
            var revisado = crear(hoy.AddHours(1), -33.02, -68.90, 25, -33.00, -68.88, 24, 3.3, usuario,
                                 "Sismo local", "Intraplaca", serie(uspallata, trillium, 3.8, 11.2, 0.3));
            revisado.confirmar(usuario);
            eventos.Add(revisado);

            return eventos;
        }

        private static EventoSismico crear(DateTime fecha, double latE, double longE, double profE,
                                           double latH, double longH, double profH, double magnitud, Usuario usuario,
                                           string? alcance, string? origen, params SerieTemporal[] series)
        {
            var evento = new EventoSismico(fecha, latE, longE, profE, latH, longH, profH, magnitud);
            if (alcance != null) evento.setAlcance(new AlcanceSismo(alcance, alcance));
            if (origen != null) evento.setOrigenDeGeneracion(new OrigenDeGeneracion(origen, origen));
            foreach (var st in series) evento.agregarSerieTemporal(st);
            evento.setEstado(Estado.AutoDetectado, usuario);
            return evento;
        }

        // Serie temporal con cinco muestras alrededor de los valores dados
        private static SerieTemporal serie(EstacionSismologica estacion, Sismografo sismografo,
                                           double velocidad, double frecuencia, double longitud)
        {
            var inicio = DateTime.Today;
            var st = new SerieTemporal(velocidad > Velocidad.getValorUmbral(), inicio, inicio.AddMinutes(5), 50, estacion, sismografo);
            double[] factores = { 0.82, 0.95, 1.0, 0.91, 0.78 };
            for (int i = 0; i < factores.Length; i++)
            {
                var muestra = new MuestraSismica(inicio.AddMinutes(i));
                muestra.agregarDetalle(new DetalleMuestraSismica(Math.Round(velocidad * factores[i], 2), Velocidad));
                muestra.agregarDetalle(new DetalleMuestraSismica(Math.Round(frecuencia * factores[i], 2), Frecuencia));
                muestra.agregarDetalle(new DetalleMuestraSismica(Math.Round(longitud * factores[i], 2), Longitud));
                st.agregarMuestra(muestra);
            }
            return st;
        }
    }
}
