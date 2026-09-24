using RedSismica;
using Xunit;

namespace RedSismica.Tests;

public class GestorRegistrarResultadoTests
{
    private readonly Usuario analista = new("agomez", new AnalistaEnSismos("Ana Gómez", "agomez@redsismica.gob.ar"));

    private GestorRegistrarResultado CrearGestor(List<EventoSismico>? eventos = null) =>
        new(new Sesion(DateTime.Now, analista), eventos);

    private EventoSismico CrearEvento(DateTime fecha, string estado = Estado.AutoDetectado, bool datosCompletos = true)
    {
        var evento = new EventoSismico(fecha, -31.5, -68.5, 10, -31.5, -68.5, 12, 4.2);
        if (datosCompletos)
        {
            evento.setAlcance(new AlcanceSismo("Sismo local", "Sismo local"));
            evento.setOrigenDeGeneracion(new OrigenDeGeneracion("Interplaca", "Interplaca"));
        }
        evento.setEstado(estado, analista);
        return evento;
    }

    [Fact]
    public void Lista_solo_eventos_auto_detectados_ordenados_por_fecha()
    {
        var hoy = DateTime.Today;
        var tarde = CrearEvento(hoy.AddHours(10));
        var temprano = CrearEvento(hoy.AddHours(2));
        var confirmado = CrearEvento(hoy.AddHours(5), Estado.Confirmado);
        var gestor = CrearGestor(new() { tarde, confirmado, temprano });

        var lista = gestor.tomarRegistroResultadoRevisionManual();

        Assert.Equal(new[] { temprano, tarde }, lista);
    }

    [Fact]
    public void Seleccionar_un_evento_lo_bloquea_en_revision()
    {
        var evento = CrearEvento(DateTime.Today);
        var gestor = CrearGestor(new() { evento });

        gestor.tomarSeleccionEvento(evento);

        Assert.True(evento.esBloqueadoEnRevision());
        Assert.Same(evento, gestor.getEventoSeleccionado());
    }

    [Fact]
    public void Cambiar_de_seleccion_libera_el_evento_anterior()
    {
        var primero = CrearEvento(DateTime.Today.AddHours(1));
        var segundo = CrearEvento(DateTime.Today.AddHours(2));
        var gestor = CrearGestor(new() { primero, segundo });

        gestor.tomarSeleccionEvento(primero);
        gestor.tomarSeleccionEvento(segundo);

        Assert.True(primero.esAutoDetectado());
        Assert.True(segundo.esBloqueadoEnRevision());
    }

    [Theory]
    [InlineData(GestorRegistrarResultado.AccionConfirmar, Estado.Confirmado)]
    [InlineData(GestorRegistrarResultado.AccionRechazar, Estado.Rechazado)]
    [InlineData(GestorRegistrarResultado.AccionDerivar, Estado.DerivadoAExperto)]
    public void Registrar_resultado_cambia_el_estado_y_cierra_el_evento(string accion, string estadoEsperado)
    {
        var evento = CrearEvento(DateTime.Today);
        var gestor = CrearGestor(new() { evento });
        gestor.tomarSeleccionEvento(evento);

        var resultado = gestor.tomarAccionConEvento(accion);

        Assert.True(resultado.Exito);
        Assert.True(evento.esEstadoActual(estadoEsperado));
        Assert.NotNull(evento.getFechaHoraFin());
        Assert.Null(gestor.getEventoSeleccionado());
        Assert.Empty(gestor.tomarRegistroResultadoRevisionManual());
    }

    [Fact]
    public void Registrar_resultado_guarda_el_historial_con_un_unico_estado_actual()
    {
        var evento = CrearEvento(DateTime.Today);
        var gestor = CrearGestor(new() { evento });
        gestor.tomarSeleccionEvento(evento);

        gestor.tomarAccionConEvento(GestorRegistrarResultado.AccionRechazar);

        var historial = evento.getCambiosDeEstado();
        Assert.Equal(new[] { Estado.AutoDetectado, Estado.BloqueadoEnRevision, Estado.Rechazado },
                     historial.Select(ce => ce.getEstado().getDescripcion()));
        Assert.Single(historial, ce => ce.sosActual());
        Assert.All(historial, ce => Assert.Same(analista, ce.getUsuario()));
    }

    [Fact]
    public void No_permite_registrar_si_faltan_alcance_u_origen()
    {
        var evento = CrearEvento(DateTime.Today, datosCompletos: false);
        var gestor = CrearGestor(new() { evento });
        gestor.tomarSeleccionEvento(evento);

        var resultado = gestor.tomarAccionConEvento(GestorRegistrarResultado.AccionConfirmar);

        Assert.False(resultado.Exito);
        Assert.True(evento.esBloqueadoEnRevision());
    }

    [Fact]
    public void Modificar_datos_completa_el_evento_y_permite_registrarlo()
    {
        var evento = CrearEvento(DateTime.Today, datosCompletos: false);
        var gestor = CrearGestor(new() { evento });
        gestor.tomarSeleccionEvento(evento);

        gestor.modificarDatosEvento("Sismo regional", 5.4, "Volcánico");
        var resultado = gestor.tomarAccionConEvento(GestorRegistrarResultado.AccionConfirmar);

        Assert.True(resultado.Exito);
        Assert.Equal(5.4, evento.getValorMagnitud());
        Assert.Equal("Moderado", evento.getMagnitudRichter().getDescripcion());
        Assert.Equal("Sismo regional", evento.getAlcance()!.getNombre());
        Assert.Equal("Volcánico", evento.getOrigenDeGeneracion()!.getNombre());
    }

    [Fact]
    public void Rechaza_acciones_invalidas_o_sin_evento_seleccionado()
    {
        var evento = CrearEvento(DateTime.Today);
        var gestor = CrearGestor(new() { evento });

        Assert.False(gestor.tomarAccionConEvento(GestorRegistrarResultado.AccionConfirmar).Exito);

        gestor.tomarSeleccionEvento(evento);
        Assert.False(gestor.tomarAccionConEvento("Borrar evento").Exito);
        Assert.False(gestor.tomarAccionConEvento(null).Exito);
    }

    [Fact]
    public void No_permite_seleccionar_eventos_ya_revisados()
    {
        var evento = CrearEvento(DateTime.Today, Estado.Confirmado);
        var gestor = CrearGestor(new() { evento });

        Assert.Throws<InvalidOperationException>(() => gestor.tomarSeleccionEvento(evento));
    }

    [Theory]
    [InlineData(10, "Superficial")]
    [InlineData(150, "Intermedio")]
    [InlineData(580, "Profundo")]
    public void Clasifica_el_sismo_segun_la_profundidad_del_hipocentro(double km, string esperado)
    {
        var evento = new EventoSismico(DateTime.Today, 0, 0, km, 0, 0, km, 4.0);

        Assert.Equal(esperado, evento.getClasificacion().getNombre());
    }

    [Fact]
    public void Los_datos_de_prueba_tienen_eventos_pendientes_con_series_temporales()
    {
        var gestor = CrearGestor();

        var pendientes = gestor.tomarRegistroResultadoRevisionManual();

        Assert.NotEmpty(pendientes);
        Assert.All(pendientes, ev => Assert.NotEmpty(ev.getSerieTemporal()));
    }
}
