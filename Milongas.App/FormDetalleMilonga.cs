using Milongas.Extractor.Models;

namespace Milongas.App;

public partial class FormDetalleMilonga : Form
{
    private readonly Milonga milonga;

    private readonly Func<Milonga, Task<MilongaDetalle>>
        obtenerDetalleAsync;

    private MilongaDetalle? detalle;

    public FormDetalleMilonga(
        Milonga milonga,
        Func<Milonga, Task<MilongaDetalle>> obtenerDetalleAsync)
    {
        InitializeComponent();

        this.milonga =
            milonga;

        this.obtenerDetalleAsync =
            obtenerDetalleAsync;

        Text =
            milonga.Nombre;

        Shown +=
            FormDetalleMilonga_Shown;
    }

    private async void FormDetalleMilonga_Shown(
        object? sender,
        EventArgs e)
    {
        try
        {
            UseWaitCursor = true;

            detalle =
                await obtenerDetalleAsync(
                    milonga);

            MostrarDetalle();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Error al cargar el detalle",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private void MostrarDetalle()
    {
        if (detalle is null)
        {
            return;
        }

        LblTipo.Text =
            milonga.Tipo.ToUpper();

        LblNombre.Text =
            milonga.Nombre;

        MostrarLabel(
            LblEstado,
            detalle.Estado);

        MostrarLabel(
            LblOrganizadores,
            detalle.Organizadores,
            "Organizan: ");

        MostrarLabel(
            LblHorario,
            milonga.Horario);

        MostrarLabel(
            LblSalon,
            milonga.Salon);

        MostrarLabel(
            LblDireccion,
            detalle.Direccion);

        MostrarLabel(
            LblBarrio,
            milonga.Barrio);

        string clase =
            milonga.TieneClase
                ? $"Clase: {milonga.HorarioClase}"
                : "";

        MostrarLabel(
            LblClase,
            clase);

        string distancia =
            milonga.DistanciaKm.HasValue
                ? $"{milonga.DistanciaKm.Value:0.0} km"
                : "";

        MostrarLabel(
            LblDistancia,
            distancia);

        string reserva =
            detalle.RecomiendaReservar
                ? "Se aconseja reservar"
                : "";

        MostrarLabel(
            LblReserva,
            reserva);

        MostrarLabel(
            LblDescripcion,
            detalle.Descripcion);

        CargarImagenDetalle();
        CargarFoto();
    }

    private static void MostrarLabel(
    Label label,
    string texto,
    string prefijo = "")
    {
        bool tieneContenido =
            !string.IsNullOrWhiteSpace(texto);

        label.Visible =
            tieneContenido;

        if (tieneContenido)
        {
            label.Text =
                prefijo + texto;
        }
    }

    private void CargarImagenDetalle()
    {
        if (detalle is null)
        {
            return;
        }

        PicImagen.Visible = true;

        if (string.IsNullOrWhiteSpace(
            detalle.ImagenDetalle))
        {
            PicImagen.Image = null;
            return;
        }

        PicImagen.LoadAsync(
            detalle.ImagenDetalle);
    }

    private void CargarFoto()
    {
        if (detalle is null ||
            string.IsNullOrWhiteSpace(detalle.Foto))
        {
            PicFoto.Image = null;
            PicFoto.Visible = false;
            return;
        }

        PicFoto.Visible = true;

        PicFoto.LoadAsync(
            detalle.Foto);
    }
}