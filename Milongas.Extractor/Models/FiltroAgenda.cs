namespace Milongas.Extractor.Models;

public class FiltroAgenda
{
    public DateOnly? Fecha { get; set; }

    public string? Barrio { get; set; }

    public string? Texto { get; set; }

    public bool? Cancelada { get; set; }

    public bool? TieneClase { get; set; }
}