using Milongas.Extractor.Models;

namespace Milongas.App;

public partial class MilongaCard : UserControl
{
    private Milonga? milonga;

    public MilongaCard()
    {
        InitializeComponent();

        Click += MilongaCard_Click;
        PicImagen.Click += MilongaCard_Click;
        LblTipo.Click += MilongaCard_Click;
        LblNombre.Click += MilongaCard_Click;
        LblHorario.Click += MilongaCard_Click;
        LblUbicacion.Click += MilongaCard_Click;
        LblClaseDistancia.Click += MilongaCard_Click;
    }

    public void CargarMilonga(
        Milonga milonga)
    {
        this.milonga = milonga;

        LblTipo.Text =
            milonga.Tipo.ToUpper();

        LblNombre.Text =
            milonga.Nombre;

        LblHorario.Text =
            milonga.Horario;

        LblUbicacion.Text =
            ObtenerUbicacion(milonga);

        LblClaseDistancia.Text =
            ObtenerClaseDistancia(milonga);

        CargarImagen(milonga);
    }

    private static string ObtenerUbicacion(
        Milonga milonga)
    {
        if (!string.IsNullOrWhiteSpace(milonga.Salon) &&
            !string.IsNullOrWhiteSpace(milonga.Barrio))
        {
            return $"{milonga.Salon} · {milonga.Barrio}";
        }

        if (!string.IsNullOrWhiteSpace(milonga.Salon))
        {
            return milonga.Salon;
        }

        return milonga.Barrio;
    }

    private static string ObtenerClaseDistancia(
        Milonga milonga)
    {
        string clase =
            milonga.TieneClase
                ? $"Clase {milonga.HorarioClase}"
                : "Sin clase";

        if (milonga.DistanciaKm.HasValue)
        {
            return
                $"{clase} · {milonga.DistanciaKm.Value:0.0} km";
        }

        return clase;
    }

    private void CargarImagen(
    Milonga milonga)
    {
        if (string.IsNullOrWhiteSpace(milonga.Imagen))
        {
            PicImagen.Image = null;
            return;
        }

        string urlImagen =
            milonga.Imagen;

        if (urlImagen.StartsWith("/"))
        {
            urlImagen =
                "https://www.hoy-milonga.com" +
                urlImagen;
        }

        PicImagen.LoadAsync(
            urlImagen);
    }

    private void AbrirDetalle()
    {
        if (milonga is null)
        {
            return;
        }

        FormDetalleMilonga formulario =
            new FormDetalleMilonga(milonga);

        formulario.ShowDialog();
    }

    private void MilongaCard_Click(
    object? sender,
    EventArgs e)
    {
        AbrirDetalle();
    }
}