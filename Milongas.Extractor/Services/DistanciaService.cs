using Milongas.Extractor.Models;

namespace Milongas.Extractor.Services;

public class DistanciaService
{
    private const double RadioTierraKm = 6371.0;

    public void CalcularDistancias(
        List<Milonga> milongas,
        double latitudOrigen,
        double longitudOrigen)
    {
        foreach (Milonga milonga in milongas)
        {
            if (!milonga.Latitud.HasValue ||
                !milonga.Longitud.HasValue)
            {
                milonga.DistanciaKm = null;
                continue;
            }

            milonga.DistanciaKm =
                CalcularDistanciaKm(
                    latitudOrigen,
                    longitudOrigen,
                    milonga.Latitud.Value,
                    milonga.Longitud.Value);
        }
    }

    public List<Milonga> OrdenarPorDistancia(
        List<Milonga> milongas)
    {
        return milongas
            .OrderBy(milonga =>
                milonga.DistanciaKm ?? double.MaxValue)
            .ToList();
    }

    private static double CalcularDistanciaKm(
        double latitudOrigen,
        double longitudOrigen,
        double latitudDestino,
        double longitudDestino)
    {
        double latitudOrigenRad =
            ConvertirARadianes(latitudOrigen);

        double longitudOrigenRad =
            ConvertirARadianes(longitudOrigen);

        double latitudDestinoRad =
            ConvertirARadianes(latitudDestino);

        double longitudDestinoRad =
            ConvertirARadianes(longitudDestino);

        double diferenciaLatitud =
            latitudDestinoRad - latitudOrigenRad;

        double diferenciaLongitud =
            longitudDestinoRad - longitudOrigenRad;

        double a =
            Math.Pow(
                Math.Sin(diferenciaLatitud / 2),
                2) +
            Math.Cos(latitudOrigenRad) *
            Math.Cos(latitudDestinoRad) *
            Math.Pow(
                Math.Sin(diferenciaLongitud / 2),
                2);

        double c =
            2 * Math.Atan2(
                Math.Sqrt(a),
                Math.Sqrt(1 - a));

        return RadioTierraKm * c;
    }

    private static double ConvertirARadianes(
        double grados)
    {
        return grados * Math.PI / 180.0;
    }
}