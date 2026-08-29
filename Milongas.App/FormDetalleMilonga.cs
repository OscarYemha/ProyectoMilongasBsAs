using Milongas.Extractor.Models;

namespace Milongas.App;

public partial class FormDetalleMilonga : Form
{
    private readonly Milonga milonga;

    private MilongaDetalle? detalle;

    public FormDetalleMilonga(
    Milonga milonga,
    MilongaDetalle detalle)
    {
        InitializeComponent();

        this.milonga =
            milonga;

        this.detalle =
            detalle;

        Text =
            milonga.Nombre;

        MostrarDatosBasicos();
        MostrarDetalle();
        ConfigurarContactos();

        LblCargando.Visible =
            false;

        PnlCabecera.Visible =
            true;

        PnlInformacion.Visible =
            true;

        FlpDescripcion.Visible =
            true;

        FlpContactos.Visible =
            true;
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


        CargarImagenDetalle();
        CargarFoto();
        ConfigurarBotonMapa();

        FlpDescripcion.Visible =
     !string.IsNullOrWhiteSpace(detalle.Descripcion) ||
     !string.IsNullOrWhiteSpace(detalle.Foto);

        ReacomodarInformacion();

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
        Control[] controles =
        {
        LblOrganizadores,
        LblHorario,
        LblSalon,
        LblDireccion,
        BtnComoLlego,
        LblBarrio,
        LblClase,
        LblDistancia,
        LblReserva
    };

        int y = 10;
        int espacio = 8;

        foreach (Control control in controles)
        {
            if (!control.Visible)
            {
                continue;
            }

            control.Location =
                new Point(
                    10,
                    y);

            y +=
                control.Height +
                espacio;
        }

        PnlInformacion.Height =
            y + 10;
    }

    private void ConfigurarContactos()
    {
        if (detalle is null)
        {
            FlpContactos.Visible = false;
            return;
        }

        ConfigurarBotonContacto(
            BtnFacebook,
            detalle.Facebook);

        ConfigurarBotonContacto(
            BtnInstagram,
            detalle.Instagram);

        ConfigurarBotonContacto(
            BtnWhatsApp,
            detalle.WhatsApp);

        ConfigurarBotonContacto(
            BtnEmail,
            detalle.Email);

        ConfigurarBotonContacto(
            BtnTelefono,
            detalle.Telefono);

        ConfigurarBotonContacto(
            BtnYouTube,
            detalle.YouTube);

        ConfigurarBotonContacto(
            BtnSitioWeb,
            detalle.SitioWeb);

        FlpContactos.Visible =
            BtnFacebook.Visible ||
            BtnInstagram.Visible ||
            BtnWhatsApp.Visible ||
            BtnEmail.Visible ||
            BtnTelefono.Visible ||
            BtnYouTube.Visible ||
            BtnSitioWeb.Visible;
    }

    private static void ConfigurarBotonContacto(
    Button boton,
    string enlace)
    {
        bool tieneEnlace =
            !string.IsNullOrWhiteSpace(enlace);

        boton.Visible =
            tieneEnlace;

        boton.Tag =
            tieneEnlace
                ? enlace
                : null;
    }

    private void BtnContacto_Click(
    object? sender,
    EventArgs e)
    {
        if (sender is not Button boton ||
            boton.Tag is not string enlace ||
            string.IsNullOrWhiteSpace(enlace))
        {
            return;
        }

        try
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = enlace,
                    UseShellExecute = true
                });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "No se pudo abrir el enlace",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ConfigurarBotonMapa()
    {
        if (detalle is null)
        {
            BtnComoLlego.Visible = false;
            return;
        }

        bool tieneMapa =
            !string.IsNullOrWhiteSpace(
                detalle.LinkMapa);

        BtnComoLlego.Visible =
            tieneMapa;

        BtnComoLlego.Tag =
            tieneMapa
                ? detalle.LinkMapa
                : null;
    }

    private void BtnComoLlego_Click(
    object? sender,
    EventArgs e)
    {
        if (BtnComoLlego.Tag is not string enlace ||
            string.IsNullOrWhiteSpace(enlace))
        {
            return;
        }

        try
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = enlace,
                    UseShellExecute = true
                });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "No se pudo abrir el mapa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}