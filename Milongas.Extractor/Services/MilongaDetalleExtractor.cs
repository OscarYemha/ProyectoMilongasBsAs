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
        HtmlDocument documento =
            CargarDocumento(
                html);

        milonga.Direccion =
            ObtenerDireccion(
                documento);

        (double? latitud, double? longitud) =
            ObtenerCoordenadas(
                documento);

        milonga.Latitud =
            latitud;

        milonga.Longitud =
            longitud;
    }

    public MilongaDetalle ObtenerDetalle(
        string html)
    {
        HtmlDocument documento =
            CargarDocumento(
                html);

        (double? latitud, double? longitud) =
            ObtenerCoordenadas(
                documento);

        return new MilongaDetalle
        {
            Direccion =
                ObtenerDireccion(
                    documento),

            Latitud =
                latitud,

            Longitud =
                longitud,

            Organizadores =
                ObtenerOrganizadores(
                    documento),

            Estado =
                ObtenerEstado(
                    documento),

            RecomiendaReservar =
                ObtenerRecomiendaReservar(
                    documento),

            Facebook =
                ObtenerContacto(
                    documento,
                    "contact-options-facebook"),

            Instagram =
                ObtenerContacto(
                    documento,
                    "contact-options-instagram"),

            YouTube =
                ObtenerContacto(
                    documento,
                    "contact-options-youtube"),

            Email =
                ObtenerContacto(
                    documento,
                    "contact-options-email"),

            WhatsApp =
                ObtenerContacto(
                    documento,
                    "contact-options-whatsapp"),

            Telefono =
                ObtenerContacto(
                    documento,
                    "contact-options-phone"),

            SitioWeb =
                ObtenerContacto(
                    documento,
                    "contact-options-website"),

            Descripcion =
                ObtenerDescripcion(
                    documento),

            ImagenDetalle =
                ObtenerImagenDetalle(
                    documento),

            Foto =
                ObtenerFoto(
                    documento),

            LinkMapa =
                ObtenerLinkMapa(
                    documento)
        };
    }

    private static HtmlDocument CargarDocumento(
        string html)
    {
        HtmlDocument documento =
            new();

        documento.LoadHtml(
            html);

        return documento;
    }

    private static string ObtenerDireccion(
    HtmlDocument documento)
    {
        HtmlNode? enlaceMapa =
            documento.DocumentNode
                .SelectSingleNode(
                    "//a[@id='entity-header-directions']");

        if (enlaceMapa is null)
        {
            return "";
        }

        HtmlNode? contenedor =
            enlaceMapa.ParentNode;

        if (contenedor is null)
        {
            return "";
        }

        string texto =
            LimpiarTexto(
                contenedor.InnerText);

        string textoBoton =
            LimpiarTexto(
                enlaceMapa.InnerText);

        if (!string.IsNullOrWhiteSpace(
                textoBoton))
        {
            texto =
                texto.Replace(
                    textoBoton,
                    "",
                    StringComparison.OrdinalIgnoreCase)
                .Trim();
        }

        int separador =
            texto.IndexOf(
                '|');

        if (separador >= 0 &&
            separador < texto.Length - 1)
        {
            texto =
                texto[(separador + 1)..]
                    .Trim();
        }

        return texto;
    }

    private static (
        double? Latitud,
        double? Longitud)
        ObtenerCoordenadas(
            HtmlDocument documento)
    {
        string linkMapa =
            ObtenerLinkMapa(
                documento);

        if (string.IsNullOrWhiteSpace(
                linkMapa))
        {
            return (null, null);
        }

        Match coincidencia =
            Regex.Match(
                linkMapa,
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

        return (
            latitud,
            longitud);
    }

    private static string ObtenerOrganizadores(
        HtmlDocument documento)
    {
        HtmlNode? nodoOrganizadores =
            documento.DocumentNode
                .SelectSingleNode(
                    "//span[contains(normalize-space(.),'Organizadores/as:')]");

        if (nodoOrganizadores is null)
        {
            return "";
        }

        string texto =
            LimpiarTexto(
                nodoOrganizadores.InnerText);

        texto =
            Regex.Replace(
                texto,
                @"^Organizadores/as:\s*",
                "",
                RegexOptions.IgnoreCase);

        return texto.Trim();
    }

    private static string ObtenerEstado(
        HtmlDocument documento)
    {
        HtmlNode? nodoEstado =
            documento.DocumentNode
                .SelectSingleNode(
                    "//div[contains(@class,'grid-title')]" +
                    "//span[contains(@class,'badge') and " +
                    "contains(@class,'badge-pill')]");

        if (nodoEstado is null)
        {
            return "";
        }

        return LimpiarTexto(
            nodoEstado.InnerText);
    }

    private static bool ObtenerRecomiendaReservar(
        HtmlDocument documento)
    {
        HtmlNode? nodoReserva =
            documento.DocumentNode
                .SelectSingleNode(
                    "//span[contains(@class,'text-danger') and " +
                    "contains(normalize-space(.),'Se aconseja reservar')]");

        return nodoReserva is not null;
    }

    private static string ObtenerContacto(
        HtmlDocument documento,
        string id)
    {
        HtmlNode? nodo =
            documento.DocumentNode
                .SelectSingleNode(
                    $"//a[@id='{id}']");

        if (nodo is null)
        {
            return "";
        }

        return HtmlEntity.DeEntitize(
            nodo.GetAttributeValue(
                "href",
                ""));
    }

    private static string ObtenerDescripcion(
        HtmlDocument documento)
    {
        HtmlNode? nodoDescripcion =
            documento.DocumentNode
                .SelectSingleNode(
                    "//p[contains(@class,'pre-line')]");

        if (nodoDescripcion is null)
        {
            return "";
        }

        return LimpiarTexto(
            nodoDescripcion.InnerText);
    }

    private static string ObtenerImagenDetalle(
        HtmlDocument documento)
    {
        HtmlNode? nodoImagen =
            documento.DocumentNode
                .SelectSingleNode(
                    "//div[contains(@class,'grid-logo')]" +
                    "//img[contains(@src,'/data_images/event/logo/')]");

        if (nodoImagen is null)
        {
            return "";
        }

        return HtmlEntity.DeEntitize(
            nodoImagen.GetAttributeValue(
                "src",
                ""));
    }

    private static string ObtenerFoto(
        HtmlDocument documento)
    {
        HtmlNode? nodoImagen =
            documento.DocumentNode
                .SelectSingleNode(
                    "//img[@alt='photo' and " +
                    "contains(@src,'/data_images/event/photo/')]");

        if (nodoImagen is null)
        {
            return "";
        }

        return HtmlEntity.DeEntitize(
            nodoImagen.GetAttributeValue(
                "src",
                ""));
    }

    private static string ObtenerLinkMapa(
        HtmlDocument documento)
    {
        HtmlNode? enlaceMapa =
            documento.DocumentNode
                .SelectSingleNode(
                    "//a[contains(@href,'maps.google')]");

        if (enlaceMapa is null)
        {
            return "";
        }

        return HtmlEntity.DeEntitize(
            enlaceMapa.GetAttributeValue(
                "href",
                ""));
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