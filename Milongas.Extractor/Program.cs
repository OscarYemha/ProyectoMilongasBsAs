using Milongas.Extractor.Models;
using Milongas.Extractor.Services;

const string url =
    "https://www.hoy-milonga.com/buenos-aires/es/milongas";

try
{
    HoyMilongaService hoyMilongaService =
        new HoyMilongaService();

    AgendaService agendaService =
        new AgendaService();

    DateOnly fechaReferencia =
        DateOnly.FromDateTime(DateTime.Today);

    AgendaResultado resultadoAgenda =
    await hoyMilongaService.ObtenerAgendaAsync(
        url,
        fechaReferencia);

    List<Milonga> agenda =
        resultadoAgenda.Milongas;


    FiltroAgenda filtro = new()
    {
        Fecha = fechaReferencia,
        Cancelada = false,
        TieneClase = null
    };

    List<Milonga> resultado =
        agendaService.Filtrar(
            agenda,
            filtro);

    foreach (Milonga milonga in resultado)
    {
        Console.WriteLine(
            $"{milonga.Horario} | " +
            $"{milonga.Nombre} | " +
            $"{milonga.Salon} | " +
            $"{milonga.Barrio}"
            );
    }
    
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine("Ocurrió un error:");
    Console.WriteLine(ex.Message);
}

Console.WriteLine();
Console.WriteLine("Presioná una tecla para finalizar.");
Console.ReadKey();