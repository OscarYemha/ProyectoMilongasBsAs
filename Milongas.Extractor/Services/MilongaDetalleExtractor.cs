using HtmlAgilityPack;
using Milongas.Extractor.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Milongas.Extractor.Services;

public class MilongaDetalleExtractor
{
    public void CompletarDatos(
        Milonga milonga,
        string html)
    {
        HtmlDocument documento = new();
        documento.LoadHtml(html);

        milonga.Direccion =
            ObtenerDireccion(documento);

        (double? latitud, double? longitud) =
            ObtenerCoordenadas(documento);

        milonga.Latitud = latitud;
        milonga.Longitud = longitud;
    }

    private static string ObtenerDireccion(
        HtmlDocument documento)
    {
        HtmlNode? nodoDireccion =
            documento.DocumentNode.SelectSingleNode(
                "//span[contains(@class,'user-select-all')]");

        if (nodoDireccion is null)
        {
            return "";
        }

        return LimpiarTexto(
            nodoDireccion.InnerText);
    }

    private static (double? Latitud, double? Longitud)
        ObtenerCoordenadas(
            HtmlDocument documento)
    {
        HtmlNode? enlaceMapa =
            documento.DocumentNode.SelectSingleNode(
                "//a[contains(@href,'maps.google')]");

        if (enlaceMapa is null)
        {
            return (null, null);
        }

        string href =
            HtmlEntity.DeEntitize(
                enlaceMapa.GetAttributeValue(
                    "href",
                    ""));

        Match coincidencia = Regex.Match(
            href,
            @"daddr=(-?\d+(?:\.\d+)?)\s*,\s*(-?\d+(?:\.\d+)?)",
            RegexOptions.IgnoreCase);

        if (!coincidencia.Success)
        {
            return (null, null);
        }

        bool latitudValida =
            double.TryParse(
                coincidencia.Groups[1].Value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double latitud);

        bool longitudValida =
            double.TryParse(
                coincidencia.Groups[2].Value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double longitud);

        if (!latitudValida ||
            !longitudValida)
        {
            return (null, null);
        }

        return (latitud, longitud);
    }

    private static string LimpiarTexto(
        string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return "";
        }

        string decodificado =
            HtmlEntity.DeEntitize(texto);

        return string.Join(
            " ",
            decodificado.Split(
                [' ', '\r', '\n', '\t'],
                StringSplitOptions.RemoveEmptyEntries));
    }
}