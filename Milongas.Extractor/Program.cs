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

    List<Milonga> agenda =
        await hoyMilongaService.ObtenerAgendaAsync(
            url,
            fechaReferencia);

    Console.WriteLine();
    Console.WriteLine(
        $"Total de milongas obtenidas: {agenda.Count}");


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

    Console.WriteLine();
    Console.WriteLine(
        $"Milongas disponibles para {fechaReferencia:dd/MM/yyyy}: " +
        $"{resultado.Count}");

    Console.WriteLine();

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