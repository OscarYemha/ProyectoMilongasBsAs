using Microsoft.Playwright;
using Milongas.Extractor.Models;

namespace Milongas.Extractor.Services;

public class BrowserService : IAsyncDisposable
{
    private IPlaywright? playwright;
    private IBrowser? browser;

    private IPage? paginaAgenda;
    private IPage? paginaDetalle;

    private async Task InicializarAsync()
    {
        if (paginaAgenda is not null &&
            paginaDetalle is not null)
        {
            return;
        }

        playwright =
            await Playwright.CreateAsync();

        browser =
            await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

        paginaAgenda =
            await browser.NewPageAsync();

        paginaDetalle =
            await browser.NewPageAsync();
    }

    public async Task<AgendaWebResultado>
        ObtenerHtmlDiasVisiblesAsync(
            string url,
            DateOnly fechaReferencia)
    {
        Dictionary<DateOnly, string> htmlPorFecha =
            new();

        DateOnly? fechaActiva = null;

        await foreach (
            AgendaDiaWeb dia
            in ObtenerDiasVisiblesProgresivoAsync(
                url,
                fechaReferencia))
        {
            htmlPorFecha[dia.Fecha] =
                dia.Html;

            if (dia.EsFechaActiva)
            {
                fechaActiva =
                    dia.Fecha;
            }
        }

        if (!fechaActiva.HasValue)
        {
            throw new InvalidOperationException(
                "No se pudo determinar la fecha activa de Hoy Milonga.");
        }

        return new AgendaWebResultado
        {
            FechaActiva =
                fechaActiva.Value,

            HtmlPorFecha =
                htmlPorFecha
        };
    }

    public async IAsyncEnumerable<AgendaDiaWeb>
        ObtenerDiasVisiblesProgresivoAsync(
            string url,
            DateOnly fechaReferencia)
    {
        await InicializarAsync();

        IPage paginaActual =
            ObtenerPaginaAgenda();

        await paginaActual.GotoAsync(
            url,
            new PageGotoOptions
            {
                WaitUntil =
                    WaitUntilState.DOMContentLoaded
            });

        await paginaActual.WaitForSelectorAsync(
            "button.day-button",
            new PageWaitForSelectorOptions
            {
                State =
                    WaitForSelectorState.Visible,

                Timeout =
                    30_000
            });

        ILocator botonesDias =
            paginaActual.Locator(
                "button.day-button");

        int cantidadBotones =
            await botonesDias.CountAsync();

        if (cantidadBotones == 0)
        {
            throw new InvalidOperationException(
                "No se encontraron días disponibles en Hoy Milonga.");
        }

        ILocator botonActivo =
            paginaActual.Locator(
                "button.day-button.active");

        await botonActivo.WaitForAsync(
            new LocatorWaitForOptions
            {
                State =
                    WaitForSelectorState.Visible,

                Timeout =
                    30_000
            });

        int indiceActivo =
            await botonActivo
                .EvaluateAsync<int>(
                    @"elemento => {
                        const botones =
                            Array.from(
                                document.querySelectorAll(
                                    'button.day-button'));

                        return botones.indexOf(
                            elemento);
                    }");

        if (indiceActivo < 0)
        {
            throw new InvalidOperationException(
                "No se pudo identificar el día activo.");
        }

        List<DateOnly> fechas =
            await ObtenerFechasBotonesAsync(
                botonesDias,
                cantidadBotones,
                fechaReferencia);

        List<int> indices =
            Enumerable
                .Range(
                    0,
                    cantidadBotones)
                .OrderBy(
                    indice =>
                        indice == indiceActivo
                            ? 0
                            : 1)
                .ThenBy(
                    indice =>
                        indice)
                .ToList();

        foreach (int indice in indices)
        {
            ILocator boton =
                botonesDias.Nth(
                    indice);

            DateOnly fecha =
                fechas[indice];

            await CerrarModalSiExisteAsync(
                paginaActual);

            bool yaEstaActivo =
                await boton
                    .EvaluateAsync<bool>(
                        @"elemento =>
                            elemento.classList.contains(
                                'active')");

            if (!yaEstaActivo)
            {
                ILocator listaAntes =
                    ObtenerListaVisible(
                        paginaActual);

                await listaAntes.WaitForAsync(
                    new LocatorWaitForOptions
                    {
                        State =
                            WaitForSelectorState.Visible,

                        Timeout =
                            30_000
                    });

                string htmlAnterior =
                    await listaAntes
                        .InnerHTMLAsync();

                await boton.EvaluateAsync(
                    @"elemento =>
                        elemento.click()");

                await EsperarBotonActivoAsync(
                    paginaActual,
                    indice);

                await EsperarCambioListaAsync(
                    paginaActual,
                    htmlAnterior);
            }

            await EsperarListaEstableAsync(
                paginaActual);

            ILocator listaVisible =
                ObtenerListaVisible(
                    paginaActual);

            await listaVisible.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State =
                        WaitForSelectorState.Visible,

                    Timeout =
                        30_000
                });

            string html =
                await listaVisible
                    .EvaluateAsync<string>(
                        @"elemento =>
                            elemento.outerHTML");

            yield return new AgendaDiaWeb
            {
                Fecha =
                    fecha,

                EsFechaActiva =
                    indice == indiceActivo,

                Html =
                    html
            };
        }
    }

    private static async Task<List<DateOnly>>
        ObtenerFechasBotonesAsync(
            ILocator botonesDias,
            int cantidadBotones,
            DateOnly fechaReferencia)
    {
        List<DateOnly> fechas =
            new();

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

        if (primerDia >
            fechaReferencia.Day)
        {
            mesActual--;

            if (mesActual < 1)
            {
                mesActual = 12;
                añoActual--;
            }
        }

        int? diaAnterior =
            null;

        for (int i = 0;
             i < cantidadBotones;
             i++)
        {
            string textoDia =
                await botonesDias
                    .Nth(i)
                    .Locator(".day-number")
                    .InnerTextAsync();

            int dia =
                int.Parse(
                    textoDia);

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

            fechas.Add(
                new DateOnly(
                    añoActual,
                    mesActual,
                    dia));

            diaAnterior =
                dia;
        }

        return fechas;
    }

    private static async Task
        EsperarBotonActivoAsync(
            IPage pagina,
            int indice)
    {
        await pagina.WaitForFunctionAsync(
            @"indice => {
                const botones =
                    Array.from(
                        document.querySelectorAll(
                            'button.day-button'));

                const boton =
                    botones[indice];

                return boton &&
                       boton.classList.contains(
                           'active');
            }",
            indice,
            new PageWaitForFunctionOptions
            {
                Timeout =
                    30_000
            });
    }

    private static async Task
        EsperarCambioListaAsync(
            IPage pagina,
            string htmlAnterior)
    {
        await pagina.WaitForFunctionAsync(
            @"htmlAnterior => {
                const listas =
                    Array.from(
                        document.querySelectorAll(
                            '#event-list'));

                const visibles =
                    listas.filter(
                        elemento => {
                            const estilo =
                                window.getComputedStyle(
                                    elemento);

                            const rect =
                                elemento
                                    .getBoundingClientRect();

                            return (
                                estilo.display !==
                                    'none' &&
                                estilo.visibility !==
                                    'hidden' &&
                                rect.width > 0 &&
                                rect.height > 0
                            );
                        });

                if (visibles.length === 0) {
                    return false;
                }

                const listaActual =
                    visibles[
                        visibles.length - 1
                    ];

                return (
                    listaActual.innerHTML !==
                    htmlAnterior
                );
            }",
            htmlAnterior,
            new PageWaitForFunctionOptions
            {
                Timeout =
                    30_000
            });
    }

    private static ILocator
        ObtenerListaVisible(
            IPage pagina)
    {
        return pagina
            .Locator(
                "#event-list:visible")
            .Last;
    }

    private static async Task
        EsperarListaEstableAsync(
            IPage pagina)
    {
        string? htmlAnterior =
            null;

        int lecturasIguales =
            0;

        const int maxIntentos =
            25;

        for (int intento = 0;
             intento < maxIntentos;
             intento++)
        {
            ILocator listaVisible =
                ObtenerListaVisible(
                    pagina);

            await listaVisible.WaitForAsync(
                new LocatorWaitForOptions
                {
                    State =
                        WaitForSelectorState.Visible,

                    Timeout =
                        5_000
                });

            string htmlActual =
                await listaVisible
                    .InnerHTMLAsync();

            if (htmlActual ==
                htmlAnterior)
            {
                lecturasIguales++;
            }
            else
            {
                htmlAnterior =
                    htmlActual;

                lecturasIguales =
                    0;
            }

            if (lecturasIguales >= 2)
            {
                return;
            }

            await pagina.WaitForTimeoutAsync(
                200);
        }

        throw new TimeoutException(
            "La lista de milongas no terminó de estabilizarse.");
    }

    public async Task<string>
        ObtenerHtmlDetalleAsync(
            string urlDetalle)
    {
        await InicializarAsync();

        IPage paginaActual =
            ObtenerPaginaDetalle();

        await paginaActual.GotoAsync(
            urlDetalle,
            new PageGotoOptions
            {
                WaitUntil =
                    WaitUntilState.DOMContentLoaded
            });

        await paginaActual.WaitForFunctionAsync(
            @"() => {
                const body =
                    document.body;

                return (
                    body !== null &&
                    body.innerText
                        .trim()
                        .length > 100
                );
            }",
            null,
            new PageWaitForFunctionOptions
            {
                Timeout =
                    15_000
            });

        return await paginaActual
            .ContentAsync();
    }

    private IPage
        ObtenerPaginaAgenda()
    {
        if (paginaAgenda is null)
        {
            throw new InvalidOperationException(
                "La página de agenda no fue inicializada correctamente.");
        }

        return paginaAgenda;
    }

    private IPage
        ObtenerPaginaDetalle()
    {
        if (paginaDetalle is null)
        {
            throw new InvalidOperationException(
                "La página de detalle no fue inicializada correctamente.");
        }

        return paginaDetalle;
    }

    private static async Task
        CerrarModalSiExisteAsync(
            IPage pagina)
    {
        ILocator modal =
            pagina.Locator(
                "#club-add-modal");

        if (await modal.CountAsync() == 0 ||
            !await modal.IsVisibleAsync())
        {
            return;
        }

        await pagina.EvaluateAsync(
            @"() => {
                const modal =
                    document.querySelector(
                        '#club-add-modal');

                if (modal) {
                    modal.remove();
                }

                document
                    .querySelectorAll(
                        '.modal-backdrop')
                    .forEach(
                        elemento =>
                            elemento.remove());

                document.body
                    .classList
                    .remove(
                        'modal-open');

                document.body.style
                    .removeProperty(
                        'overflow');

                document.body.style
                    .removeProperty(
                        'padding-right');
            }");

        await pagina.WaitForTimeoutAsync(
            200);
    }

    public async ValueTask DisposeAsync()
    {
        if (paginaAgenda is not null)
        {
            await paginaAgenda
                .CloseAsync();
        }

        if (paginaDetalle is not null)
        {
            await paginaDetalle
                .CloseAsync();
        }

        if (browser is not null)
        {
            await browser.DisposeAsync();
        }

        playwright?.Dispose();

        paginaAgenda =
            null;

        paginaDetalle =
            null;

        browser =
            null;

        playwright =
            null;
    }
}