using Milongas.Extractor.Models;

namespace Milongas.App;

public partial class MilongaCard : UserControl
{
    private readonly Func<Milonga, Task<MilongaDetalle>>
        obtenerDetalleAsync;

    private Milonga? milonga;
    private Color colorBorde =
    Color.FromArgb(225, 225, 225);

    private int grosorBorde = 1;

    private const int AlturaNormal = 150;
    private const int AlturaCancelada = 85;

    private const int MargenIzquierdo = 20;
    private const int XContenidoConImagen = 100;
    private const int MargenDerecho = 20;

    public MilongaCard(
        Func<Milonga, Task<MilongaDetalle>> obtenerDetalleAsync)
    {
        InitializeComponent();

        this.obtenerDetalleAsync =
            obtenerDetalleAsync;

        DoubleBuffered = true;
        ResizeRedraw = true;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

        UpdateStyles();

        Click += MilongaCard_Click;
        PicImagen.Click += MilongaCard_Click;
        LblTipo.Click += MilongaCard_Click;
        LblNombre.Click += MilongaCard_Click;
        LblHorario.Click += MilongaCard_Click;
        LblUbicacion.Click += MilongaCard_Click;
        LblClaseDistancia.Click += MilongaCard_Click;
        LblEstado.Click += MilongaCard_Click;
        LblModalidadEntrada.Click += MilongaCard_Click;
        LblEventoEspecial.Click += MilongaCard_Click;
        LblCancelada.Click += MilongaCard_Click;

    }

    protected override void OnPaint(
    PaintEventArgs e)
    {
        base.OnPaint(e);

        using Pen pen = new(
            colorBorde,
            grosorBorde);

        Rectangle rectangulo =
            ClientRectangle;

        rectangulo.Width -= 1;
        rectangulo.Height -= 1;

        e.Graphics.DrawRectangle(
            pen,
            rectangulo);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(BackColor);
    }

    private void AplicarEstilo(
    Milonga milonga)
    {
        if (milonga.Destacada)
        {
            BackColor =
                Color.FromArgb(
                    255,
                    244,
                    204);

            colorBorde =
                Color.FromArgb(
                    210,
                    170,
                    70);

            grosorBorde = 2;
        }
        else if (milonga.Finalizada)
        {
            BackColor =
                Color.FromArgb(
                    238,
                    238,
                    238);

            colorBorde =
                Color.FromArgb(
                    205,
                    205,
                    205);

            grosorBorde = 1;
        }
        else
        {
            BackColor =
                Color.FromArgb(
                    250,
                    250,
                    250);

            colorBorde =
                Color.FromArgb(
                    225,
                    225,
                    225);

            grosorBorde = 1;
        }

        Invalidate();
    }

    private void ConfigurarEstado(
    Milonga milonga)
    {
        LblEstado.Visible = false;

        if (milonga.Finalizada)
        {
            LblEstado.Text = "Finalizó";

            LblEstado.ForeColor =
                Color.Firebrick;

            LblEstado.BackColor =
                Color.FromArgb(
                    255,
                    240,
                    240);

            LblEstado.Visible = true;

            return;
        }

        if (milonga.Abierta)
        {
            LblEstado.Text = "Abierto";

            LblEstado.ForeColor =
                Color.SeaGreen;

            LblEstado.BackColor =
                Color.FromArgb(
                    232,
                    250,
                    242);

            LblEstado.Visible = true;
        }
    }

    public void CargarMilonga(
        Milonga milonga)
    {
        this.milonga = milonga;

        AplicarEstilo(milonga);
        ConfigurarEstado(milonga);


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

            PicImagen.Visible = 
                false;

            LblModalidadEntrada.Visible = 
                false;

            LblEventoEspecial.Visible = 
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
            PicImagen.Visible = false;
            return;
        }

        PicImagen.Visible = true;

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
        PicImagen.Visible = false;

        const int margenIzquierdo = 20;
        const int margenDerecho = 15;
        const int separacionHorizontal = 15;
        const int separacionVertical = 3;

        // Primero calculamos dónde quedará CANCELADO.
        int xCancelada =
            ClientSize.Width -
            LblCancelada.Width -
            margenDerecho;

        int anchoNombre =
            xCancelada -
            separacionHorizontal -
            margenIzquierdo;

        // Medimos cuánto ocuparía el nombre en una sola línea.
        Size tamañoUnaLinea =
            TextRenderer.MeasureText(
                LblNombre.Text,
                LblNombre.Font,
                new Size(
                    int.MaxValue,
                    int.MaxValue),
                TextFormatFlags.SingleLine);

        bool necesitaDosLineas =
            tamañoUnaLinea.Width >
            anchoNombre;

        Height =
            necesitaDosLineas
                ? 85
                : 65;

        // Ahora centramos CANCELADO usando
        // la altura definitiva de la tarjeta.
        LblCancelada.Location =
            new Point(
                xCancelada,
                (ClientSize.Height -
                 LblCancelada.Height) / 2);

        LblNombre.AutoSize = false;
        LblNombre.Width =
            Math.Max(
                80,
                anchoNombre);

        LblNombre.Height =
            necesitaDosLineas
                ? 44
                : 24;

        LblNombre.AutoEllipsis = true;
        LblNombre.TextAlign =
            ContentAlignment.MiddleLeft;

        int altoContenido =
            LblTipo.Height +
            separacionVertical +
            LblNombre.Height;

        int yInicial =
            (ClientSize.Height -
             altoContenido) / 2;

        LblTipo.Location =
            new Point(
                margenIzquierdo,
                yInicial);

        LblNombre.Location =
            new Point(
                margenIzquierdo,
                yInicial +
                LblTipo.Height +
                separacionVertical);
    }

    private void AjustarLayoutNormal()
    {
        int cantidadFilas =
    new Control[]
    {
        LblTipo,
        LblNombre,
        LblHorario,
        LblUbicacion,
        LblClaseDistancia,
        LblModalidadEntrada,
        LblEventoEspecial
    }
    .Count(control => control.Visible);

        Height =
            cantidadFilas switch
            {
                <= 5 => 115,
                6 => 135,
                _ => 150
            };

        int xContenido =
            PicImagen.Visible
                ? 100
                : 20;

        int anchoContenido =
            ClientSize.Width -
            xContenido -
            20;

        if (PicImagen.Visible)
        {
            PicImagen.Location =
                new Point(
                    10,
                    (ClientSize.Height -
                     PicImagen.Height) / 2);
        }

        LblNombre.AutoSize = false;
        LblNombre.Width = anchoContenido;
        LblNombre.Height = 24;
        LblNombre.AutoEllipsis = true;

        ConfigurarAnchoLabel(
            LblUbicacion,
            anchoContenido);

        ConfigurarAnchoLabel(
            LblClaseDistancia,
            anchoContenido);

        ConfigurarAnchoLabel(
            LblModalidadEntrada,
            anchoContenido);

        ConfigurarAnchoLabel(
            LblEventoEspecial,
            anchoContenido);

        const int separacion = 3;

        List<Control> controles = new()
    {
        LblTipo,
        LblNombre,
        LblHorario,
        LblUbicacion,
        LblClaseDistancia,
        LblModalidadEntrada,
        LblEventoEspecial
    };

        List<Control> visibles =
            controles
                .Where(control => control.Visible)
                .ToList();

        int altoContenido =
            visibles.Sum(
                control => control.Height) +
            separacion *
            Math.Max(
                0,
                visibles.Count - 1);

        int y =
            Math.Max(
                6,
                (ClientSize.Height -
                 altoContenido) / 2);

        foreach (Control control in visibles)
        {
            // Horario se acomoda aparte si hay estado.
            if (control == LblHorario &&
                LblEstado.Visible)
            {
                LblEstado.Location =
                    new Point(
                        xContenido,
                        y +
                        (LblHorario.Height -
                         LblEstado.Height) / 2);

                LblHorario.Location =
                    new Point(
                        LblEstado.Right + 6,
                        y);

                y +=
                    Math.Max(
                        LblEstado.Height,
                        LblHorario.Height) +
                    separacion;

                continue;
            }

            control.Location =
                new Point(
                    xContenido,
                    y);

            y +=
                control.Height +
                separacion;
        }
    }

    private static void ConfigurarAnchoLabel(
    Label label,
    int ancho)
    {
        label.AutoSize = false;
        label.Width = Math.Max(20, ancho);
        label.Height = 18;
        label.AutoEllipsis = true;
    }
}