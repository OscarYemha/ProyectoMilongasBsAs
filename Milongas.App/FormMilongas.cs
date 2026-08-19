using Microsoft.Playwright;
using Milongas.Extractor.Models;
using Milongas.Extractor.Services;

namespace Milongas.App;

public partial class FormMilongas : Form
{
    private readonly HoyMilongaService hoyMilongaService;
    private readonly AgendaService agendaService;
    private readonly DistanciaService distanciaService;

    // Evita que dos operaciones usen Chromium al mismo tiempo.
    private readonly SemaphoreSlim navegadorSemaphore =
        new(1, 1);

    // Permite cancelar una actualización anterior.
    private CancellationTokenSource? filtroCancellationTokenSource;

    private List<Milonga> agenda = new();

    private bool actualizandoFechas;
    private bool actualizandoFiltros;
    private bool agendaCargada;

    private const string Url =
        "https://www.hoy-milonga.com/buenos-aires/es/milongas";

    public FormMilongas()
    {
        InitializeComponent();

        hoyMilongaService =
            new HoyMilongaService();

        agendaService =
            new AgendaService();

        distanciaService =
            new DistanciaService();

        
        ConfigurarOrden();
        ConfigurarClase();

        HabilitarControlesAgenda(false);


        FormClosed +=
            Form1_FormClosed;

        FlpMilongas.Resize +=
            (_, _) => AjustarAnchoTarjetas();
    }

    private async void BtnCargar_Click(
        object sender,
        EventArgs e)
    {
        filtroCancellationTokenSource?.Cancel();

        BtnCargar.Enabled = false;
        BtnCargar.Text = "Cargando...";

        HabilitarControlesAgenda(false);

        try
        {
            await navegadorSemaphore.WaitAsync();

            try
            {
                DateOnly fechaReferencia =
                    DateOnly.FromDateTime(
                        DateTime.Today);

                agenda =
                    await hoyMilongaService.ObtenerAgendaAsync(
                        Url,
                        fechaReferencia);
            }
            finally
            {
                navegadorSemaphore.Release();
            }

            agendaCargada = true;

            CargarBarrios();

            DateOnly? fechaSeleccionada =
                CargarFechas();

            if (fechaSeleccionada.HasValue)
            {
                await ProgramarActualizacionAsync(
                    fechaSeleccionada.Value,
                    TxtBuscar.Text,
                    aplicarDemora: false);
            }
        }
        catch (PlaywrightException ex)
        {
            MessageBox.Show(
                $"No se pudo cargar la agenda:\n{ex.Message}",
                "Error de navegación",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            BtnCargar.Enabled = true;
            BtnCargar.Text = "Cargar agenda";

            HabilitarControlesAgenda(
                agendaCargada);
        }
    }

    private void HabilitarControlesAgenda(
        bool habilitar)
    {
        CmbFecha.Enabled = habilitar;
        TxtBuscar.Enabled = habilitar;
        CmbOrden.Enabled = habilitar;
        CmbBarrio.Enabled = habilitar;
        CmbClase.Enabled = habilitar;
    }

    private DateOnly? CargarFechas()
    {
        actualizandoFechas = true;

        try
        {
            List<DateOnly> fechas =
                agenda
                    .Select(milonga => milonga.Fecha)
                    .Distinct()
                    .OrderBy(fecha => fecha)
                    .ToList();

            CmbFecha.Items.Clear();

            foreach (DateOnly fecha in fechas)
            {
                CmbFecha.Items.Add(fecha);
            }

            if (CmbFecha.Items.Count == 0)
            {
                return null;
            }

            DateOnly hoy =
                DateOnly.FromDateTime(
                    DateTime.Today);

            int indiceHoy =
                CmbFecha.Items.IndexOf(hoy);

            CmbFecha.SelectedIndex =
                indiceHoy >= 0
                    ? indiceHoy
                    : 0;

            if (CmbFecha.SelectedItem
                is DateOnly fechaSeleccionada)
            {
                return fechaSeleccionada;
            }

            return null;
        }
        finally
        {
            actualizandoFechas = false;
        }
    }

    private void CargarBarrios()
    {
        actualizandoFiltros = true;

        try
        {
            List<string> barrios =
                agendaService.ObtenerBarrios(
                    agenda);

            CmbBarrio.Items.Clear();

            CmbBarrio.Items.Add("Todos");

            foreach (string barrio in barrios)
            {
                CmbBarrio.Items.Add(barrio);
            }

            CmbBarrio.SelectedIndex = 0;
        }
        finally
        {
            actualizandoFiltros = false;
        }
    }

    private void ConfigurarClase()
    {
        CmbClase.Items.Clear();

        CmbClase.Items.Add("Todas");
        CmbClase.Items.Add("Con clase");
        CmbClase.Items.Add("Sin clase");

        CmbClase.SelectedIndex = 0;
    }

    private void ConfigurarOrden()
    {
        CmbOrden.Items.Clear();

        CmbOrden.Items.Add("Horario");
        CmbOrden.Items.Add("Distancia");

        CmbOrden.SelectedIndex = 0;
    }

    private async Task ProgramarActualizacionAsync(
        DateOnly fecha,
        string texto,
        bool aplicarDemora)
    {
        filtroCancellationTokenSource?.Cancel();
        filtroCancellationTokenSource?.Dispose();

        filtroCancellationTokenSource =
            new CancellationTokenSource();

        CancellationToken cancellationToken =
            filtroCancellationTokenSource.Token;

        try
        {
            // Evita ejecutar una búsqueda por cada letra escrita.
            if (aplicarDemora)
            {
                await Task.Delay(
                    400,
                    cancellationToken);
            }

            await MostrarMilongasAsync(
                fecha,
                texto,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Es normal cancelar una actualización anterior.
        }
    }

    private async Task MostrarMilongasAsync(
        DateOnly fecha,
        string texto,
        CancellationToken cancellationToken)
    {
        string? barrioSeleccionado =
            ObtenerBarrioSeleccionado();

        bool? tieneClase =
            ObtenerFiltroClase();

        FiltroAgenda filtro = new()
        {
            Fecha = fecha,
            Texto = texto,
            Barrio = barrioSeleccionado,
            Cancelada = null,
            TieneClase = tieneClase
        };

        List<Milonga> resultado =
            agendaService.Filtrar(
                agenda,
                filtro);

        cancellationToken.ThrowIfCancellationRequested();

        if (resultado.Count == 0)
        {
            ActualizarTabla(resultado);
            return;
        }

        await navegadorSemaphore.WaitAsync(
            cancellationToken);

        try
        {
            await hoyMilongaService.CompletarDetallesAsync(
                resultado);
        }
        catch (PlaywrightException ex)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                MessageBox.Show(
                    $"No se pudo cargar una de las fichas:\n{ex.Message}",
                    "Error de navegación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            return;
        }
        finally
        {
            navegadorSemaphore.Release();
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Coordenadas temporales del Obelisco.
        double latitudOrigen = -34.6037;
        double longitudOrigen = -58.3816;

        distanciaService.CalcularDistancias(
            resultado,
            latitudOrigen,
            longitudOrigen);

        if (CmbOrden.SelectedItem?.ToString()
            == "Distancia")
        {
            resultado =
                distanciaService.OrdenarPorDistancia(
                    resultado);
        }

        cancellationToken.ThrowIfCancellationRequested();

        ActualizarTabla(resultado);
    }

    private string? ObtenerBarrioSeleccionado()
    {
        if (CmbBarrio.SelectedItem is not string barrio ||
            barrio == "Todos")
        {
            return null;
        }

        return barrio;
    }

    private bool? ObtenerFiltroClase()
    {
        if (CmbClase.SelectedItem is not string clase)
        {
            return null;
        }

        if (clase == "Con clase")
        {
            return true;
        }

        if (clase == "Sin clase")
        {
            return false;
        }

        return null;
    }

    private void ActualizarTabla(
     List<Milonga> milongas)
    {

        FlpMilongas.Controls.Clear();

        foreach (Milonga milonga in milongas)
        {
            MilongaCard card =
                new MilongaCard(
                    ObtenerDetalleSeguroAsync);

            card.CargarMilonga(
                milonga);

            FlpMilongas.Controls.Add(
                card);
        }

        BeginInvoke(
            AjustarAnchoTarjetas);
    }

    private void AjustarAnchoTarjetas()
    {
        int anchoDisponible =
            FlpMilongas.ClientSize.Width - 10;

        foreach (Control control in
            FlpMilongas.Controls)
        {
            control.Width =
                anchoDisponible -
                control.Margin.Horizontal;
        }
    }

    private async void CmbFecha_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        if (actualizandoFechas ||
            !agendaCargada ||
            CmbFecha.SelectedItem is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: false);
    }

    private async void TxtBuscar_TextChanged(
        object sender,
        EventArgs e)
    {
        if (!agendaCargada ||
            CmbFecha.SelectedItem is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: true);
    }

    private async void CmbOrden_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        if (!agendaCargada ||
            CmbFecha.SelectedItem is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: false);
    }

    private async void CmbBarrio_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        if (actualizandoFiltros ||
            !agendaCargada ||
            CmbFecha.SelectedItem is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: false);
    }

    private async void CmbClase_SelectedIndexChanged(
        object sender,
        EventArgs e)
    {
        if (!agendaCargada ||
            CmbFecha.SelectedItem is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: false);
    }

    private async void Form1_FormClosed(
        object? sender,
        FormClosedEventArgs e)
    {
        filtroCancellationTokenSource?.Cancel();
        filtroCancellationTokenSource?.Dispose();

        await hoyMilongaService.DisposeAsync();

        navegadorSemaphore.Dispose();
    }

    private async Task<MilongaDetalle>
    ObtenerDetalleSeguroAsync(
        Milonga milonga)
    {
        await navegadorSemaphore.WaitAsync();

        try
        {
            return
                await hoyMilongaService.ObtenerDetalleAsync(
                    milonga);
        }
        finally
        {
            navegadorSemaphore.Release();
        }
    }
}