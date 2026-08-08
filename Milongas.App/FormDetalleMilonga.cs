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
        LblCargando.Visible = true;
        PnlCabecera.Visible = false;
        PnlInformacion.Visible = false;
        FlpDescripcion.Visible = false;

        try
        {
            UseWaitCursor = true;

            detalle =
                await obtenerDetalleAsync(
                    milonga);

            MostrarDatosBasicos();
            MostrarDetalle();

            LblCargando.Visible = false;
            PnlCabecera.Visible = true;
            PnlInformacion.Visible = true;
            FlpDescripcion.Visible = true;
        }
        catch (Exception ex)
        {
            LblCargando.Text =
                "No se pudo cargar la información.";

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

    private void MostrarDatosBasicos()
    {
        LblEstado.Visible = false;
        LblOrganizadores.Visible = false;
        LblDireccion.Visible = false;
        LblReserva.Visible = false;
        LblDescripcion.Visible = false;

        PicImagen.Visible = false;
        PicFoto.Visible = false;

        LblTipo.Text =
            milonga.Tipo.ToUpper();

        LblNombre.Text =
            milonga.Nombre;

        MostrarLabel(
            LblHorario,
            milonga.Horario);

        MostrarLabel(
            LblSalon,
            milonga.Salon);

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

    }

    private void MostrarDetalle()
    {
        if (detalle is null)
        {
            return;
        }

        MostrarLabel(
            LblEstado,
            detalle.Estado);

        MostrarLabel(
            LblOrganizadores,
            detalle.Organizadores,
            "Organizan: ");

        MostrarLabel(
            LblDireccion,
            detalle.Direccion);

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

        ReacomodarInformacion();

        CargarImagenDetalle();
        CargarFoto();

        FlpDescripcion.Visible =
            LblDescripcion.Visible ||
            PicFoto.Visible;


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
        if (detalle is null ||
            string.IsNullOrWhiteSpace(detalle.ImagenDetalle))
        {
            PicImagen.Image = null;
            PicImagen.Visible = false;
            return;
        }

        PicImagen.Visible = true;

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

    private void ReacomodarInformacion()
    {
        Label[] labels =
        {
        LblOrganizadores,
        LblHorario,
        LblSalon,
        LblDireccion,
        LblBarrio,
        LblClase,
        LblDistancia,
        LblReserva
    };

        int y = 10;
        int espacio = 8;

        foreach (Label label in labels)
        {
            if (!label.Visible)
            {
                continue;
            }

            label.AutoSize = true;

            Size tamaño =
                TextRenderer.MeasureText(
                    label.Text,
                    label.Font);

            label.Location =
                new Point(
                    10,
                    y);

            y +=
                tamaño.Height +
                espacio;
        }

        PnlInformacion.Height =
            y + 10;
    }
}