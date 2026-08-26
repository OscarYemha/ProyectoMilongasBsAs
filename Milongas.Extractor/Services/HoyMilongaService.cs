using Milongas.Extractor.Models;
using System.Diagnostics;

namespace Milongas.Extractor.Services;

public class HoyMilongaService : IAsyncDisposable
{
    private readonly BrowserService browserService;
    private readonly HtmlExtractor htmlExtractor;
    private readonly MilongaDetalleExtractor detalleExtractor;
    private readonly DetalleCacheService detalleCacheService;

    public HoyMilongaService()
    {
        browserService =
            new BrowserService();

        htmlExtractor =
            new HtmlExtractor();

        detalleExtractor =
            new MilongaDetalleExtractor();

        detalleCacheService =
            new DetalleCacheService();
    }

    public async IAsyncEnumerable<AgendaResultado>
        ObtenerAgendaProgresivaAsync(
            string url,
            DateOnly fechaReferencia)
    {
        await foreach (
            AgendaDiaWeb diaWeb
            in browserService
                .ObtenerDiasVisiblesProgresivoAsync(
                    url,
                    fechaReferencia))
        {
            List<Milonga> milongasDelDia =
                htmlExtractor.ObtenerMilongas(
                    diaWeb.Html);

            Debug.WriteLine(
                $"DÍA {diaWeb.Fecha:dd/MM/yyyy} - " +
                $"{milongasDelDia.Count} milongas");

            foreach (
                Milonga milonga
                in milongasDelDia.Take(3))
            {
                Debug.WriteLine(
                    $"    {milonga.Nombre}");
            }

            foreach (
                Milonga milonga
                in milongasDelDia)
            {
                milonga.Fecha =
                    diaWeb.Fecha;
            }

            yield return new AgendaResultado
            {
                FechaActiva =
                    diaWeb.EsFechaActiva
                        ? diaWeb.Fecha
                        : default,

                Milongas =
                    milongasDelDia
            };
        }
    }

    public async Task CompletarDetalleAsync(
        Milonga milonga)
    {
        string urlDetalle =
            "https://www.hoy-milonga.com" +
            milonga.Link;

        string htmlDetalle =
            await browserService
                .ObtenerHtmlDetalleAsync(
                    urlDetalle);

        detalleExtractor.CompletarDatos(
            milonga,
            htmlDetalle);
    }

    public async Task CompletarDetallesAsync(
        List<Milonga> milongas)
    {
        Dictionary<int, DetalleMilongaCache> cache =
            await detalleCacheService
                .CargarAsync();

        bool cacheModificada =
            false;

        foreach (Milonga milonga in milongas)
        {
            // Si ya tenemos la ficha en caché
            // y contiene coordenadas válidas,
            // evitamos volver a navegar.
            if (cache.TryGetValue(
                    milonga.Id,
                    out DetalleMilongaCache? detalleGuardado))
            {
                bool cacheValida =
                    detalleGuardado.Latitud.HasValue &&
                    detalleGuardado.Longitud.HasValue;

                if (cacheValida)
                {
                    milonga.Direccion =
                        detalleGuardado.Direccion;

                    milonga.Latitud =
                        detalleGuardado.Latitud;

                    milonga.Longitud =
                        detalleGuardado.Longitud;

                    continue;
                }
            }

            await CompletarDetalleAsync(
                milonga);

            DetalleMilongaCache nuevoDetalle =
                new()
                {
                    Direccion =
                        milonga.Direccion,

                    Latitud =
                        milonga.Latitud,

                    Longitud =
                        milonga.Longitud
                };

            cache[milonga.Id] =
                nuevoDetalle;

            cacheModificada =
                true;
        }

        // Evitamos escribir el archivo
        // cuando la caché no cambió.
        if (cacheModificada)
        {
            await detalleCacheService
                .GuardarAsync(
                    cache);
        }
    }

    public async Task<MilongaDetalle>
    ObtenerDetalleAsync(
        Milonga milonga)
    {
        string urlDetalle =
            "https://www.hoy-milonga.com" +
            milonga.Link;

        const int maxIntentos = 3;

        MilongaDetalle ultimoDetalle =
            new();

        for (int intento = 1;
             intento <= maxIntentos;
             intento++)
        {
            string htmlDetalle =
                await browserService
                    .ObtenerHtmlDetalleAsync(
                        urlDetalle);

            ultimoDetalle =
                detalleExtractor.ObtenerDetalle(
                    htmlDetalle);

            if (DetalleTieneInformacion(
                    ultimoDetalle))
            {
                return ultimoDetalle;
            }

            if (intento < maxIntentos)
            {
                await Task.Delay(
                    700);
            }
        }

        return ultimoDetalle;
    }

    private static bool DetalleTieneInformacion(
    MilongaDetalle detalle)
    {
        return
            !string.IsNullOrWhiteSpace(
                detalle.Direccion) ||
            !string.IsNullOrWhiteSpace(
                detalle.Organizadores) ||
            !string.IsNullOrWhiteSpace(
                detalle.Estado) ||
            !string.IsNullOrWhiteSpace(
                detalle.Descripcion) ||
            !string.IsNullOrWhiteSpace(
                detalle.ImagenDetalle) ||
            !string.IsNullOrWhiteSpace(
                detalle.Foto) ||
            !string.IsNullOrWhiteSpace(
                detalle.LinkMapa) ||
            !string.IsNullOrWhiteSpace(
                detalle.Facebook) ||
            !string.IsNullOrWhiteSpace(
                detalle.Instagram) ||
            !string.IsNullOrWhiteSpace(
                detalle.WhatsApp) ||
            !string.IsNullOrWhiteSpace(
                detalle.Telefono) ||
            !string.IsNullOrWhiteSpace(
                detalle.Email) ||
            !string.IsNullOrWhiteSpace(
                detalle.SitioWeb);
    }

    public async ValueTask DisposeAsync()
    {
        await browserService
            .DisposeAsync();
    }
}