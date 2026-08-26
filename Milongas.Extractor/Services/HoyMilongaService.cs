using Milongas.Extractor.Models;

namespace Milongas.Extractor.Services;

public class HoyMilongaService : IAsyncDisposable
{
    private const string UrlBase =
        "https://www.hoy-milonga.com";

    private readonly BrowserService browserService;
    private readonly HtmlExtractor htmlExtractor;
    private readonly JsonExporter jsonExporter;
    private readonly MilongaDetalleExtractor detalleExtractor;
    private readonly DetalleCacheService detalleCacheService;

    public HoyMilongaService()
    {
        browserService =
            new BrowserService();

        htmlExtractor =
            new HtmlExtractor();

        jsonExporter =
            new JsonExporter();

        detalleExtractor =
            new MilongaDetalleExtractor();

        detalleCacheService =
            new DetalleCacheService();
    }

    public async Task<string> ActualizarAgendaAsync(
        string url,
        DateOnly fechaReferencia)
    {
        AgendaWebResultado resultadoWeb =
            await browserService
                .ObtenerHtmlDiasVisiblesAsync(
                    url,
                    fechaReferencia);

        List<Milonga> milongas =
            ExtraerMilongas(
                resultadoWeb.HtmlPorFecha);

        return await jsonExporter.GuardarAsync(
            milongas);
    }

    public async Task<AgendaResultado> ObtenerAgendaAsync(
        string url,
        DateOnly fechaReferencia)
    {
        AgendaWebResultado resultadoWeb =
            await browserService
                .ObtenerHtmlDiasVisiblesAsync(
                    url,
                    fechaReferencia);

        List<Milonga> milongas =
            ExtraerMilongas(
                resultadoWeb.HtmlPorFecha);

        return new AgendaResultado
        {
            FechaActiva =
                resultadoWeb.FechaActiva,

            Milongas =
                milongas
        };
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

            AsignarFecha(
                milongasDelDia,
                diaWeb.Fecha);

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
            UrlBase +
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
            await detalleCacheService.CargarAsync();

        bool cacheModificada =
            false;

        foreach (Milonga milonga in milongas)
        {
            if (cache.TryGetValue(
                    milonga.Id,
                    out DetalleMilongaCache? detalleGuardado) &&
                CacheValida(detalleGuardado))
            {
                AplicarDetalleCache(
                    milonga,
                    detalleGuardado);

                continue;
            }

            await CompletarDetalleAsync(
                milonga);

            cache[milonga.Id] =
                CrearDetalleCache(
                    milonga);

            cacheModificada =
                true;
        }

        if (cacheModificada)
        {
            await detalleCacheService.GuardarAsync(
                cache);
        }
    }

    public async Task<MilongaDetalle> ObtenerDetalleAsync(
        Milonga milonga)
    {
        string urlDetalle =
            UrlBase +
            milonga.Link;

        string htmlDetalle =
            await browserService
                .ObtenerHtmlDetalleAsync(
                    urlDetalle);

        return detalleExtractor.ObtenerDetalle(
            htmlDetalle);
    }

    private List<Milonga> ExtraerMilongas(
        Dictionary<DateOnly, string> htmlPorFecha)
    {
        List<Milonga> resultado =
            new();

        foreach (
            KeyValuePair<DateOnly, string> item
            in htmlPorFecha)
        {
            List<Milonga> milongasDelDia =
                htmlExtractor.ObtenerMilongas(
                    item.Value);

            AsignarFecha(
                milongasDelDia,
                item.Key);

            resultado.AddRange(
                milongasDelDia);
        }

        return resultado;
    }

    private static void AsignarFecha(
        List<Milonga> milongas,
        DateOnly fecha)
    {
        foreach (Milonga milonga in milongas)
        {
            milonga.Fecha =
                fecha;
        }
    }

    private static bool CacheValida(
        DetalleMilongaCache detalle)
    {
        return
            detalle.Latitud.HasValue &&
            detalle.Longitud.HasValue;
    }

    private static void AplicarDetalleCache(
        Milonga milonga,
        DetalleMilongaCache detalle)
    {
        milonga.Direccion =
            detalle.Direccion;

        milonga.Latitud =
            detalle.Latitud;

        milonga.Longitud =
            detalle.Longitud;
    }

    private static DetalleMilongaCache CrearDetalleCache(
        Milonga milonga)
    {
        return new DetalleMilongaCache
        {
            Direccion =
                milonga.Direccion,

            Latitud =
                milonga.Latitud,

            Longitud =
                milonga.Longitud
        };
    }

    public async ValueTask DisposeAsync()
    {
        await browserService.DisposeAsync();
    }
}