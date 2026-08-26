using HtmlAgilityPack;
using Milongas.Extractor.Models;
using System.Text.RegularExpressions;

namespace Milongas.Extractor.Services;

public class HtmlExtractor
{
    public List<Milonga> ObtenerMilongas(
        string html)
    {
        HtmlDocument documento =
            new();

        documento.LoadHtml(
            html);

        HtmlNodeCollection? tarjetas =
            documento.DocumentNode.SelectNodes(
                "//*[@id='event-list']" +
                "//a[contains(" +
                "concat(' ', normalize-space(@class), ' '), " +
                "' event-list-item ')]");

        List<Milonga> milongas =
            new();

        if (tarjetas is null)
        {
            return milongas;
        }

        foreach (HtmlNode tarjeta in tarjetas)
        {
            string link =
                ObtenerLink(
                    tarjeta);

            string horarioClase =
                ObtenerHorarioClase(
                    tarjeta);

            (string salon, string barrio) =
                ObtenerUbicacion(
                    tarjeta);

            Milonga milonga =
                new()
                {
                    Id =
                        ObtenerId(
                            link),

                    Tipo =
                        ObtenerTipo(
                            tarjeta),

                    Nombre =
                        ObtenerNombre(
                            tarjeta),

                    Horario =
                        ObtenerHorario(
                            tarjeta),

                    Salon =
                        salon,

                    Barrio =
                        barrio,

                    Imagen =
                        ObtenerImagen(
                            tarjeta),

                    Link =
                        link,

                    Cancelada =
                        EstaCancelada(
                            tarjeta),

                    Destacada =
                        EstaDestacada(
                            tarjeta),

                    Finalizada =
                        EstaFinalizada(
                            tarjeta),

                    Abierta =
                        EstaAbierta(
                            tarjeta),

                    HorarioClase =
                        horarioClase,

                    TieneClase =
                        !string.IsNullOrWhiteSpace(
                            horarioClase),

                    ModalidadEntrada =
                        ObtenerModalidadEntrada(
                            tarjeta),

                    EventoEspecial =
                        ObtenerEventoEspecial(
                            tarjeta)
                };

            milongas.Add(
                milonga);
        }

        return milongas;
    }

    private static string ObtenerTipo(
        HtmlNode tarjeta)
    {
        HtmlNode? nodoTipo =
            tarjeta.SelectSingleNode(
                ".//small[contains(" +
                "@class,'text-uppercase')]");

        return LimpiarTexto(
            nodoTipo?.InnerText);
    }

    private static string ObtenerNombre(
        HtmlNode tarjeta)
    {
        HtmlNode? nodoNombre =
            tarjeta.SelectSingleNode(
                ".//h4[contains(" +
                "@class,'font-weight-bold')]");

        return LimpiarTexto(
            nodoNombre?.InnerText);
    }

    private static string ObtenerHorario(
        HtmlNode tarjeta)
    {
        string textoTarjeta =
            LimpiarTexto(
                tarjeta.InnerText);

        Match coincidencia =
            Regex.Match(
                textoTarjeta,
                @"\b\d{1,2}:\d{2}\s*-\s*" +
                @"(?:\d{1,2}:\d{2}|Medianoche)\b",
                RegexOptions.IgnoreCase);

        return coincidencia.Success
            ? coincidencia.Value
            : "";
    }

    private static (
        string Salon,
        string Barrio)
        ObtenerUbicacion(
            HtmlNode tarjeta)
    {
        HtmlNodeCollection? bloques =
            tarjeta.SelectNodes(
                ".//div[contains(" +
                "@class,'lh-20')]");

        if (bloques is null)
        {
            return ("", "");
        }

        foreach (HtmlNode bloque in bloques)
        {
            if (!bloque.InnerHtml.Contains(
                    "location-icon",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string html =
                bloque.InnerHtml;

            html =
                Regex.Replace(
                    html,
                    @"<br\s*/?>",
                    "\n",
                    RegexOptions.IgnoreCase);

            html =
                Regex.Replace(
                    html,
                    @"<[^>]+>",
                    " ");

            string texto =
                HtmlEntity.DeEntitize(
                    html);

            string[] lineas =
                texto.Split(
                    '\n',
                    StringSplitOptions
                        .RemoveEmptyEntries);

            if (lineas.Length == 0)
            {
                return ("", "");
            }

            string salon =
                LimpiarTexto(
                    lineas[0]);

            string barrio =
                "";

            if (lineas.Length >= 2)
            {
                barrio =
                    LimpiarTexto(
                        lineas[1]);

                barrio =
                    Regex.Replace(
                        barrio,
                        @"^Buenos Aires\s*-\s*",
                        "",
                        RegexOptions.IgnoreCase);
            }

            return (
                salon,
                barrio);
        }

        return ("", "");
    }

    private static string ObtenerImagen(
        HtmlNode tarjeta)
    {
        HtmlNode? nodoImagen =
            tarjeta.SelectSingleNode(
                ".//img[contains(" +
                "@src,'/data_images/event/logo/')]");

        return HtmlEntity.DeEntitize(
            nodoImagen?.GetAttributeValue(
                "src",
                "") ?? "");
    }

    private static string ObtenerLink(
        HtmlNode tarjeta)
    {
        return HtmlEntity.DeEntitize(
            tarjeta.GetAttributeValue(
                "href",
                ""));
    }

    private static int ObtenerId(
        string link)
    {
        Match coincidencia =
            Regex.Match(
                link,
                @"/milonga/(\d+)/",
                RegexOptions.IgnoreCase);

        if (!coincidencia.Success)
        {
            return 0;
        }

        return int.TryParse(
            coincidencia.Groups[1].Value,
            out int id)
                ? id
                : 0;
    }

    private static string ObtenerHorarioClase(
        HtmlNode tarjeta)
    {
        HtmlNode? iconoClase =
            tarjeta.SelectSingleNode(
                ".//*[@name and " +
                "contains(@name,'classes')]");

        if (iconoClase is null)
        {
            return "";
        }

        HtmlNode? bloqueClase =
            iconoClase.SelectSingleNode(
                "ancestor::div[" +
                "contains(" +
                "concat(' ', normalize-space(@class), ' '), " +
                "' lh-20 ')" +
                "][1]");

        if (bloqueClase is null)
        {
            return "";
        }

        Match coincidencia =
            Regex.Match(
                bloqueClase.OuterHtml,
                @"\b\d{1,2}:\d{2}\s*-\s*" +
                @"\d{1,2}:\d{2}\b");

        return coincidencia.Success
            ? coincidencia.Value
            : "";
    }

    private static string ObtenerModalidadEntrada(
        HtmlNode tarjeta)
    {
        string texto =
            LimpiarTexto(
                tarjeta.InnerText);

        if (texto.Contains(
                "A la gorra",
                StringComparison.OrdinalIgnoreCase))
        {
            return "A la gorra";
        }

        if (texto.Contains(
                "Gratis",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Gratis";
        }

        return "";
    }

    private static string ObtenerEventoEspecial(
        HtmlNode tarjeta)
    {
        string texto =
            LimpiarTexto(
                tarjeta.InnerText);

        Match coincidencia =
            Regex.Match(
                texto,
                @"Artística\s*(.+?)" +
                @"(?=\s*(?:" +
                @"clases|" +
                @"\d{1,2}:\d{2}\s*-\s*" +
                @"\d{1,2}:\d{2}|$))",
                RegexOptions.IgnoreCase);

        if (!coincidencia.Success)
        {
            return "";
        }

        return coincidencia
            .Groups[1]
            .Value
            .Trim();
    }

    private static bool EstaDestacada(
        HtmlNode tarjeta)
    {
        return tarjeta
            .GetClasses()
            .Any(
                clase =>
                    clase.Equals(
                        "highlighted-wrapper",
                        StringComparison.OrdinalIgnoreCase));
    }

    private static bool EstaFinalizada(
        HtmlNode tarjeta)
    {
        string texto =
            LimpiarTexto(
                tarjeta.InnerText);

        return texto.Contains(
            "FINALIZÓ",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool EstaCancelada(
        HtmlNode tarjeta)
    {
        string texto =
            LimpiarTexto(
                tarjeta.InnerText);

        return texto.Contains(
            "CANCELADO",
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool EstaAbierta(
        HtmlNode tarjeta)
    {
        string texto =
            LimpiarTexto(
                tarjeta.InnerText);

        return texto.Contains(
            "Abierto",
            StringComparison.OrdinalIgnoreCase);
    }

    private static string LimpiarTexto(
        string? texto)
    {
        if (string.IsNullOrWhiteSpace(
                texto))
        {
            return "";
        }

        string decodificado =
            HtmlEntity.DeEntitize(
                texto);

        return string.Join(
            " ",
            decodificado.Split(
                [' ', '\r', '\n', '\t'],
                StringSplitOptions
                    .RemoveEmptyEntries));
    }
}