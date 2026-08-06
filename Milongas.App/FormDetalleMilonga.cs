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

            // Temporal, para comprobar que el detalle
            // completo realmente fue obtenido.
            if (!string.IsNullOrWhiteSpace(
                detalle.Estado))
            {
                Text =
                    $"{milonga.Nombre} - " +
                    $"{detalle.Estado}";
            }
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
}