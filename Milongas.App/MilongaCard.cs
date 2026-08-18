using Milongas.Extractor.Models;

namespace Milongas.App;

public partial class MilongaCard : UserControl
{
    private readonly Func<Milonga, Task<MilongaDetalle>>
        obtenerDetalleAsync;

    private Milonga? milonga;

    public MilongaCard(
        Func<Milonga, Task<MilongaDetalle>> obtenerDetalleAsync)
    {
        InitializeComponent();

        this.obtenerDetalleAsync =
            obtenerDetalleAsync;

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

        if (milonga.Cancelada)
        {
            LblTipo.Text =
                milonga.Tipo.ToUpper();

            LblNombre.Text =
                milonga.Nombre;

            LblCancelada.Visible =
                true;

            LblHorario.Visible =
                false;

            LblUbicacion.Visible =
                false;

            LblClaseDistancia.Visible =
                false;

            FlpDestacados.Visible =
                false;

            PicImagen.Visible = 
                false;

            CargarImagen(milonga);

            AjustarLayoutCancelada();

            return;
        }

        LblCancelada.Visible = 
            false;

        LblHorario.Visible =
            true;

        LblUbicacion.Visible =
            true;

        LblClaseDistancia.Visible =
            true;

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

        MostrarLabel(
    LblModalidadEntrada,
    milonga.ModalidadEntrada);

        MostrarLabel(
            LblEventoEspecial,
            milonga.EventoEspecial);

        FlpDestacados.Visible =
    LblModalidadEntrada.Visible ||
    LblEventoEspecial.Visible;

        AjustarLayoutNormal();
    }

    private static string ObtenerUbicacion(
        Milonga milonga)
    {
        if (!string.IsNullOrWhiteSpace(milonga.Salon) &&
            !string.IsNullOrWhiteSpace(milonga.Barrio))
        {
            return
                $"{milonga.Salon} · {milonga.Barrio}";
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
                $"{clase} · " +
                $"{milonga.DistanciaKm.Value:0.0} km";
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
            new FormDetalleMilonga(
                milonga,
                obtenerDetalleAsync);

        formulario.ShowDialog();
    }

    private void MilongaCard_Click(
        object? sender,
        EventArgs e)
    {
        AbrirDetalle();
    }

    private static void MostrarLabel(
    Label label,
    string texto)
    {
        bool tieneContenido =
            !string.IsNullOrWhiteSpace(texto);

        label.Visible =
            tieneContenido;

        if (tieneContenido)
        {
            label.Text =
                texto;
        }
    }

    private void AjustarLayoutCancelada()
    {
        Height = 85;

        PicImagen.Visible = false;

        int separacion = 3;

        int altoContenido =
            LblTipo.Height +
            separacion +
            LblNombre.Height;

        int yInicial =
            (ClientSize.Height - altoContenido) / 2;

        LblTipo.Location =
            new Point(20, yInicial);

        LblNombre.Location =
            new Point(
                20,
                yInicial +
                LblTipo.Height +
                separacion);

        LblCancelada.Location =
            new Point(
                ClientSize.Width -
                LblCancelada.Width -
                25,
                (ClientSize.Height -
                 LblCancelada.Height) / 2);
    }

    private void AjustarLayoutNormal()
    {
        Height = 130;

        PicImagen.Visible = true;

        PicImagen.Location =
            new Point(
                10,
                (ClientSize.Height -
                 PicImagen.Height) / 2);

        Control[] controles =
        {
        LblTipo,
        LblNombre,
        LblHorario,
        LblUbicacion,
        LblClaseDistancia,
        FlpDestacados
    };

        List<Control> visibles =
            controles
                .Where(control => control.Visible)
                .ToList();

        const int separacion = 3;

        int altoContenido =
            visibles.Sum(control => control.Height) +
            separacion * Math.Max(
                0,
                visibles.Count - 1);

        int y =
            (ClientSize.Height -
             altoContenido) / 2;

        foreach (Control control in visibles)
        {
            control.Location =
                new Point(
                    100,
                    y);

            y +=
                control.Height +
                separacion;
        }
    }
}