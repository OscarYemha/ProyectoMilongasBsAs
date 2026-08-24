namespace Milongas.Extractor.Models;

public class AgendaDiaWeb
{
    public DateOnly Fecha { get; set; }

    public bool EsFechaActiva { get; set; }

    public string Html { get; set; } = "";
}