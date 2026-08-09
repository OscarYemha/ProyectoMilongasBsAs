namespace Milongas.Extractor.Models;

public class Milonga
{
    public int Id { get; set; }

    public string Nombre { get; set; } = "";

    public string Horario { get; set; } = "";

    public string Salon { get; set; } = "";

    public string Barrio { get; set; } = "";

    public string Link { get; set; } = "";

    public string Imagen { get; set; } = "";

    public bool Cancelada { get; set; }

    public DateOnly Fecha { get; set; }

    public bool TieneClase { get; set; }

    public string HorarioClase { get; set; } = "";

    public string Direccion { get; set; } = "";

    public double? Latitud { get; set; }

    public double? Longitud { get; set; }

    public double? DistanciaKm { get; set; }

    public string Tipo { get; set; } = "";

    public string ModalidadEntrada { get; set; } = "";

    public string EventoEspecial { get; set; } = "";
}