namespace Milongas.Extractor.Models;

public class AgendaWebResultado
{
    public DateOnly FechaActiva { get; set; }

    public Dictionary<DateOnly, string> HtmlPorFecha
    {
        get;
        set;
    } = new();
}