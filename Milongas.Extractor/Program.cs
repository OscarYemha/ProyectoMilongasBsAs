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
        if (milonga.TieneClase)
        {
            Console.WriteLine(
                $"{milonga.Horario} | " +
                $"{milonga.Nombre} | " +
                $"{milonga.Salon} | " +
                $"{milonga.Barrio} | " +
                $"CLASE {milonga.HorarioClase}");
        }
        else
        {
            Console.WriteLine(
                $"{milonga.Horario} | " +
                $"{milonga.Nombre} | " +
                $"{milonga.Salon} | " +
                $"{milonga.Barrio} | " +
                $"Sin clase.");
        }
    }

    Milonga? milongaPrueba = resultado
    .FirstOrDefault(milonga =>
        milonga.Nombre.Contains(
            "Muy",
            StringComparison.OrdinalIgnoreCase));

    if (milongaPrueba is not null)
    {
        string urlDetalle =
            "https://www.hoy-milonga.com" +
            milongaPrueba.Link;

        await hoyMilongaService.CompletarDetalleAsync(
     milongaPrueba);

        Console.WriteLine();
        Console.WriteLine("DETALLE DE PRUEBA");

        Console.WriteLine(
            $"Nombre: {milongaPrueba.Nombre}");

        Console.WriteLine(
            $"Dirección: {milongaPrueba.Direccion}");

        Console.WriteLine(
            $"Latitud: {milongaPrueba.Latitud}");

        Console.WriteLine(
            $"Longitud: {milongaPrueba.Longitud}");

        List<Milonga> pruebaDetalle =
    resultado
        .Take(5)
        .ToList();

        await hoyMilongaService.CompletarDetallesAsync(
            pruebaDetalle);

        Console.WriteLine();
        Console.WriteLine("DETALLES DE PRUEBA");

        foreach (Milonga milonga in pruebaDetalle)
        {
            Console.WriteLine();
            Console.WriteLine($"Nombre: {milonga.Nombre}");
            Console.WriteLine($"Dirección: {milonga.Direccion}");
            Console.WriteLine($"Latitud: {milonga.Latitud}");
            Console.WriteLine($"Longitud: {milonga.Longitud}");
        }
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