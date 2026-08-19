using Milongas.Extractor.Models;
using System.Globalization;
using System.Text;

namespace Milongas.Extractor.Services;

public class AgendaService
{
    public List<Milonga> Filtrar(
        List<Milonga> milongas,
        FiltroAgenda filtro)
    {
        List<Milonga> resultado = new();

        string barrioBuscado =
            NormalizarBarrio(filtro.Barrio ?? "");

        string textoBuscado =
            NormalizarTexto(filtro.Texto ?? "");

        foreach (Milonga milonga in milongas)
        {
            // Fecha
            if (filtro.Fecha.HasValue &&
                milonga.Fecha != filtro.Fecha.Value)
            {
                continue;
            }

            // Barrio
            if (!string.IsNullOrWhiteSpace(filtro.Barrio))
            {
                string barrioMilonga =
                    NormalizarBarrio(milonga.Barrio);

                if (barrioMilonga != barrioBuscado)
                {
                    continue;
                }
            }

            // Búsqueda general
            if (!string.IsNullOrWhiteSpace(filtro.Texto))
            {
                string nombre =
                    NormalizarTexto(milonga.Nombre);

                string salon =
                    NormalizarTexto(milonga.Salon);

                string barrio =
                    NormalizarTexto(milonga.Barrio);

                bool coincide =
                    nombre.Contains(textoBuscado) ||
                    salon.Contains(textoBuscado) ||
                    barrio.Contains(textoBuscado);

                if (!coincide)
                {
                    continue;
                }
            }

            // Cancelada
            if (filtro.Cancelada.HasValue &&
                milonga.Cancelada != filtro.Cancelada.Value)
            {
                continue;
            }

            // Clase
            if (filtro.TieneClase.HasValue &&
                milonga.TieneClase != filtro.TieneClase.Value)
            {
                continue;
            }

            resultado.Add(milonga);
        }

        return OrdenarPorHorario(resultado);
    }

    public List<string> ObtenerBarrios(
        List<Milonga> milongas)
    {
        Dictionary<string, string> barrios = new();

        foreach (Milonga milonga in milongas)
        {
            if (string.IsNullOrWhiteSpace(milonga.Barrio))
            {
                continue;
            }

            string barrioNormalizado =
                NormalizarBarrio(milonga.Barrio);

            if (!barrios.ContainsKey(barrioNormalizado))
            {
                barrios.Add(
                    barrioNormalizado,
                    milonga.Barrio.Trim());
            }
        }

        List<string> resultado =
            barrios.Values.ToList();

        resultado.Sort();

        return resultado;
    }

    private static List<Milonga> OrdenarPorHorario(
    List<Milonga> milongas)
    {
        return milongas
            .OrderBy(
                milonga =>
                    ObtenerPrioridad(milonga))
            .ThenBy(
                milonga =>
                    ObtenerHoraInicio(milonga.Horario))
            .ToList();
    }

    private static int ObtenerPrioridad(
    Milonga milonga)
    {
        if (milonga.Destacada)
        {
            return 0;
        }

        if (milonga.Cancelada)
        {
            return 1;
        }

        if (milonga.Abierta)
        {
            return 2;
        }

        if (milonga.Finalizada)
        {
            return 3;
        }

        return 2;
    }

    private static TimeOnly ObtenerHoraInicio(
        string horario)
    {
        if (string.IsNullOrWhiteSpace(horario))
        {
            return TimeOnly.MaxValue;
        }

        string horaTexto =
            horario.Split('-')[0].Trim();

        if (TimeOnly.TryParse(
            horaTexto,
            out TimeOnly hora))
        {
            return hora;
        }

        return TimeOnly.MaxValue;
    }

    private static string NormalizarTexto(
        string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return "";
        }

        string textoNormalizado =
            texto.Normalize(
                NormalizationForm.FormD);

        StringBuilder resultado = new();

        foreach (char caracter in textoNormalizado)
        {
            UnicodeCategory categoria =
                CharUnicodeInfo.GetUnicodeCategory(
                    caracter);

            if (categoria !=
                UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(
                    char.ToLowerInvariant(caracter));
            }
        }

        return resultado
            .ToString()
            .Normalize(
                NormalizationForm.FormC)
            .Trim();
    }

    private static string NormalizarBarrio(
        string barrio)
    {
        string resultado =
            NormalizarTexto(barrio);

        if (resultado.EndsWith(", caba"))
        {
            resultado =
                resultado[..^6].Trim();
        }

        return resultado;
    }
}