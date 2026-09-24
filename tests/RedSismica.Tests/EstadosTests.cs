using RedSismica;
using Xunit;

namespace RedSismica.Tests;

// Transiciones del patrón State sobre el ciclo de vida de un evento sísmico
public class EstadosTests
{
    private readonly Usuario analista = new("agomez");

    private static EventoSismico NuevoEvento() =>
        new(DateTime.Today, -31.5, -68.5, 10, -31.5, -68.5, 12, 4.2);

    private EventoSismico EventoEn(string nombreEstado)
    {
        var evento = NuevoEvento();
        if (nombreEstado == AutoDetectado.Nombre) return evento;

        evento.bloquearEventoSismico(analista);
        if (nombreEstado == Confirmado.Nombre) evento.confirmar(analista);
        if (nombreEstado == Rechazado.Nombre) evento.rechazar(analista);
        if (nombreEstado == DerivadoAExperto.Nombre) evento.derivarAExperto(analista);
        return evento;
    }

    [Fact]
    public void Un_evento_nuevo_nace_auto_detectado()
    {
        var evento = NuevoEvento();

        Assert.IsType<AutoDetectado>(evento.obtenerEstadoActual());
        Assert.Single(evento.getCambiosDeEstado());
        Assert.True(evento.buscarActualCE()!.sosActual());
    }

    [Fact]
    public void Cada_transicion_crea_una_nueva_instancia_del_estado_concreto()
    {
        var evento = NuevoEvento();

        evento.bloquearEventoSismico(analista);
        Assert.IsType<BloqueadoEnRevision>(evento.obtenerEstadoActual());

        evento.liberarEventoSismico(analista);
        Assert.IsType<AutoDetectado>(evento.obtenerEstadoActual());

        evento.bloquearEventoSismico(analista);
        evento.derivarAExperto(analista);
        Assert.IsType<DerivadoAExperto>(evento.obtenerEstadoActual());
    }

    [Fact]
    public void Cada_transicion_cierra_el_cambio_de_estado_anterior()
    {
        var evento = NuevoEvento();

        evento.bloquearEventoSismico(analista);
        evento.rechazar(analista);

        var historial = evento.getCambiosDeEstado();
        Assert.Equal(3, historial.Count);
        Assert.All(historial.Take(2), ce => Assert.NotNull(ce.getFechaHoraFin()));
        Assert.Same(historial[2], evento.buscarActualCE());
        Assert.Same(evento.obtenerEstadoActual(), historial[2].getEstado());
    }

    [Fact]
    public void Los_estados_finales_cierran_el_evento()
    {
        var bloqueado = EventoEn(BloqueadoEnRevision.Nombre);
        Assert.Null(bloqueado.getFechaHoraFin());

        var confirmado = EventoEn(Confirmado.Nombre);
        Assert.NotNull(confirmado.getFechaHoraFin());
        Assert.True(confirmado.obtenerEstadoActual().esFinal());
    }

    // Un evento auto detectado sólo puede bloquearse
    [Fact]
    public void Auto_detectado_no_admite_registrar_un_resultado()
    {
        var evento = EventoEn(AutoDetectado.Nombre);

        Assert.Throws<TransicionInvalidaException>(() => evento.confirmar(analista));
        Assert.Throws<TransicionInvalidaException>(() => evento.rechazar(analista));
        Assert.Throws<TransicionInvalidaException>(() => evento.derivarAExperto(analista));
        Assert.Throws<TransicionInvalidaException>(() => evento.liberarEventoSismico(analista));
        Assert.Single(evento.getCambiosDeEstado());
    }

    [Fact]
    public void Bloqueado_en_revision_no_puede_volver_a_bloquearse()
    {
        var evento = EventoEn(BloqueadoEnRevision.Nombre);

        Assert.Throws<TransicionInvalidaException>(() => evento.bloquearEventoSismico(analista));
    }

    [Theory]
    [InlineData("Confirmado")]
    [InlineData("Rechazado")]
    [InlineData("Derivado a experto")]
    public void Los_estados_finales_no_admiten_transiciones(string estadoFinal)
    {
        var evento = EventoEn(estadoFinal);
        int cambiosAntes = evento.getCambiosDeEstado().Count;

        Assert.Throws<TransicionInvalidaException>(() => evento.bloquearEventoSismico(analista));
        Assert.Throws<TransicionInvalidaException>(() => evento.liberarEventoSismico(analista));
        Assert.Throws<TransicionInvalidaException>(() => evento.confirmar(analista));
        Assert.Throws<TransicionInvalidaException>(() => evento.rechazar(analista));
        Assert.Throws<TransicionInvalidaException>(() => evento.derivarAExperto(analista));
        Assert.Equal(cambiosAntes, evento.getCambiosDeEstado().Count);
        Assert.True(evento.esEstadoActual(estadoFinal));
    }
}
