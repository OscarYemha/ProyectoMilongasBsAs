using Milongas.Extractor.Models;

namespace Milongas.Extractor.Services;

public class HoyMilongaService : IAsyncDisposable
{
    private readonly BrowserService browserService;
    private readonly HtmlExtractor htmlExtractor;
    private readonly JsonExporter jsonExporter;
    private readonly MilongaDetalleExtractor detalleExtractor;
    private readonly DetalleCacheService detalleCacheService;

    public HoyMilongaService()
    {
        browserService = new BrowserService();
        htmlExtractor = new HtmlExtractor();
        jsonExporter = new JsonExporter();
        detalleExtractor = new MilongaDetalleExtractor();
        detalleCacheService = new DetalleCacheService();
    }

    public async Task<string> ActualizarAgendaAsync(
        string url,
        DateOnly fechaReferencia)
    {
        Dictionary<DateOnly, string> htmlPorFecha =
            await browserService.ObtenerHtmlDiasVisiblesAsync(
                url,
                fechaReferencia);

        List<Milonga> todasLasMilongas = new();

        foreach (KeyValuePair<DateOnly, string> item in htmlPorFecha)
        {
            DateOnly fecha = item.Key;
            string html = item.Value;

            List<Milonga> milongasDelDia =
                htmlExtractor.ObtenerMilongas(html);

            foreach (Milonga milonga in milongasDelDia)
            {
                milonga.Fecha = fecha;
            }

            Console.WriteLine(
                $"Milongas encontradas para " +
                $"{fecha:dd/MM/yyyy}: " +
                $"{milongasDelDia.Count}");

            todasLasMilongas.AddRange(
                milongasDelDia);
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Total de milongas extraídas: " +
            $"{todasLasMilongas.Count}");

        string rutaArchivo =
            await jsonExporter.GuardarAsync(
                todasLasMilongas);

        return rutaArchivo;
    }

    public async Task<List<Milonga>> ObtenerAgendaAsync(
        string url,
        DateOnly fechaReferencia)
    {
        Dictionary<DateOnly, string> htmlPorFecha =
            await browserService.ObtenerHtmlDiasVisiblesAsync(
                url,
                fechaReferencia);

        List<Milonga> todasLasMilongas = new();

        foreach (KeyValuePair<DateOnly, string> item in htmlPorFecha)
        {
            DateOnly fecha = item.Key;
            string html = item.Value;

            List<Milonga> milongasDelDia =
                htmlExtractor.ObtenerMilongas(html);

            foreach (Milonga milonga in milongasDelDia)
            {
                milonga.Fecha = fecha;
            }

            Console.WriteLine(
                $"Milongas encontradas para " +
                $"{fecha:dd/MM/yyyy}: " +
                $"{milongasDelDia.Count}");

            todasLasMilongas.AddRange(
                milongasDelDia);
        }

        return todasLasMilongas;
    }

    public async Task CompletarDetalleAsync(
        Milonga milonga)
    {
        string urlDetalle =
            "https://www.hoy-milonga.com" +
            milonga.Link;

        string htmlDetalle =
            await browserService.ObtenerHtmlDetalleAsync(
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

        bool cacheModificada = false;

        foreach (Milonga milonga in milongas)
        {
            // Si ya tenemos esta milonga guardada,
            // usamos los datos de la caché.
            if (cache.TryGetValue(
    milonga.Id,
    out DetalleMilongaCache? detalleGuardado))
            {
                bool cacheValida =
                    !string.IsNullOrWhiteSpace(detalleGuardado.Direccion) &&
                    detalleGuardado.Latitud.HasValue &&
                    detalleGuardado.Longitud.HasValue;

                if (cacheValida)
                {
                    Console.WriteLine(
                        $"Cargando desde caché: {milonga.Nombre}");

                    milonga.Direccion =
                        detalleGuardado.Direccion;

                    milonga.Latitud =
                        detalleGuardado.Latitud;

                    milonga.Longitud =
                        detalleGuardado.Longitud;

                    continue;
                }

                Console.WriteLine(
                    $"Caché incompleta, descargando nuevamente: {milonga.Nombre}");
            }

            // Si no está en la caché,
            // obtenemos los datos desde Hoy Milonga.
            Console.WriteLine(
                $"Descargando detalle: {milonga.Nombre}");

            await CompletarDetalleAsync(
                milonga);

            DetalleMilongaCache nuevoDetalle = new()
            {
                Direccion = milonga.Direccion,
                Latitud = milonga.Latitud,
                Longitud = milonga.Longitud
            };

            cache[milonga.Id] =
                nuevoDetalle;

            cacheModificada = true;
        }

        // Guardamos solamente si agregamos
        // algún detalle nuevo.
        if (cacheModificada)
        {
            await detalleCacheService.GuardarAsync(
                cache);

            Console.WriteLine(
                "Caché de detalles actualizada.");
        }
    }
    public async ValueTask DisposeAsync()
    {
        await browserService.DisposeAsync();
    }

    public async Task<MilongaDetalle> ObtenerDetalleAsync(
    Milonga milonga)
    {
        string urlDetalle =
            "https://www.hoy-milonga.com" +
            milonga.Link;

        string htmlDetalle =
            await browserService.ObtenerHtmlDetalleAsync(
                urlDetalle);

        MilongaDetalle detalle =
            detalleExtractor.ObtenerDetalle(
                htmlDetalle);

        return detalle;
    }
}