using Microsoft.Playwright;

namespace Milongas.Extractor.Services;

public class BrowserService : IAsyncDisposable
{
    private IPlaywright? playwright;
    private IBrowser? browser;
    private IPage? pagina;

    private async Task InicializarAsync()
    {
        if (pagina is not null)
        {
            return;
        }

        Console.WriteLine("Iniciando Playwright...");

        playwright =
            await Playwright.CreateAsync();

        browser =
            await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false
                });

        pagina =
            await browser.NewPageAsync();
    }

    public async Task<Dictionary<DateOnly, string>> ObtenerHtmlDiasVisiblesAsync(
        string url,
        DateOnly fechaReferencia)
    {
        await InicializarAsync();

        IPage paginaActual =
            ObtenerPagina();

        Console.WriteLine("Abriendo Hoy Milonga...");

        await paginaActual.GotoAsync(
            url,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        ILocator botonesDias =
            paginaActual.Locator("button.day-button");

        int cantidadBotones =
            await botonesDias.CountAsync();

        Dictionary<DateOnly, string> htmlPorFecha = new();

        int mesActual =
            fechaReferencia.Month;

        int añoActual =
            fechaReferencia.Year;

        int primerDia =
            int.Parse(
                await botonesDias
                    .Nth(0)
                    .Locator(".day-number")
                    .InnerTextAsync());

        if (primerDia > fechaReferencia.Day)
        {
            mesActual--;

            if (mesActual < 1)
            {
                mesActual = 12;
                añoActual--;
            }
        }

        int? diaAnterior = null;

        for (int i = 0; i < cantidadBotones; i++)
        {
            ILocator boton =
                botonesDias.Nth(i);

            string textoDia =
                await boton
                    .Locator(".day-number")
                    .InnerTextAsync();

            int dia =
                int.Parse(textoDia);

            if (diaAnterior.HasValue &&
                dia < diaAnterior.Value)
            {
                mesActual++;

                if (mesActual > 12)
                {
                    mesActual = 1;
                    añoActual++;
                }
            }

            DateOnly fecha =
                new(
                    añoActual,
                    mesActual,
                    dia);

            Console.WriteLine(
                $"Seleccionando fecha: {fecha:dd/MM/yyyy}");

            await CerrarModalSiExisteAsync(
                paginaActual);

            await boton.EvaluateAsync(
                "elemento => elemento.click()");

            await paginaActual.WaitForTimeoutAsync(
                800);

            await paginaActual.WaitForSelectorAsync(
                "a.event-list-item",
                new PageWaitForSelectorOptions
                {
                    Timeout = 30_000
                });

            string html =
                await paginaActual.ContentAsync();

            htmlPorFecha.Add(
                fecha,
                html);

            diaAnterior = dia;
        }

        return htmlPorFecha;
    }

    public async Task<string> ObtenerHtmlDetalleAsync(
        string urlDetalle)
    {
        await InicializarAsync();

        IPage paginaActual =
            ObtenerPagina();

        await paginaActual.GotoAsync(
            urlDetalle,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        await paginaActual.WaitForTimeoutAsync(
            1000);

        return await paginaActual.ContentAsync();
    }

    private IPage ObtenerPagina()
    {
        if (pagina is null)
        {
            throw new InvalidOperationException(
                "El navegador no fue inicializado correctamente.");
        }

        return pagina;
    }

    private static async Task CerrarModalSiExisteAsync(
        IPage pagina)
    {
        ILocator modal =
            pagina.Locator("#club-add-modal");

        if (await modal.CountAsync() == 0 ||
            !await modal.IsVisibleAsync())
        {
            return;
        }

        Console.WriteLine(
            "Eliminando ventana emergente...");

        await pagina.EvaluateAsync(
            @"() => {
                const modal =
                    document.querySelector('#club-add-modal');

                if (modal) {
                    modal.remove();
                }

                document
                    .querySelectorAll('.modal-backdrop')
                    .forEach(elemento => elemento.remove());

                document.body.classList.remove('modal-open');

                document.body.style.removeProperty(
                    'overflow');

                document.body.style.removeProperty(
                    'padding-right');
            }");

        await pagina.WaitForTimeoutAsync(
            200);
    }

    public async ValueTask DisposeAsync()
    {
        if (browser is not null)
        {
            await browser.DisposeAsync();
        }

        playwright?.Dispose();

        pagina = null;
        browser = null;
        playwright = null;
    }
}