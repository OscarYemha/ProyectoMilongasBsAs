namespace Milongas.Extractor.Models;

public class AgendaResultado
{
    public DateOnly FechaActiva { get; set; }

    public List<Milonga> Milongas { get; set; } = new();
}