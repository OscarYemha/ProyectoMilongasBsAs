using Microsoft.Playwright;
using Milongas.Extractor.Models;
using Milongas.Extractor.Services;

namespace Milongas.App;

public partial class FormMilongas : Form
{
    private const string Url =
        "https://www.hoy-milonga.com/buenos-aires/es/milongas";

    // Coordenadas temporales del Obelisco.
    // Más adelante serán reemplazadas por
    // la ubicación real del usuario.
    private const double LatitudOrigenTemporal =
        -34.6037;

    private const double LongitudOrigenTemporal =
        -58.3816;

    private readonly HoyMilongaService hoyMilongaService;
    private readonly AgendaService agendaService;
    private readonly DistanciaService distanciaService;

    // Evita que dos operaciones usen Chromium
    // para detalles al mismo tiempo.
    private readonly SemaphoreSlim navegadorSemaphore =
        new(1, 1);

    // Permite cancelar una actualización anterior
    // de filtros o búsqueda.
    private CancellationTokenSource?
        filtroCancellationTokenSource;

    private List<Milonga> agenda =
        new();

    private readonly HashSet<Milonga>
        milongasConDetalleCargado =
            new();

    private DateOnly? fechaActivaWeb;

    private bool actualizandoFechas;
    private bool actualizandoFiltros;
    private bool actualizandoInterfaz;

    // Ya existe información suficiente
    // para mostrar el primer día.
    private bool agendaCargada;

    // Finalizó la carga y precarga
    // de todos los días disponibles.
    private bool agendaCompletaCargada;

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

        HabilitarControlesAgenda(
            false);

        FormClosed +=
            Form1_FormClosed;

        FlpMilongas.Resize +=
            (_, _) =>
                AjustarAnchoTarjetas();
    }

    private async void BtnCargar_Click(
        object sender,
        EventArgs e)
    {
        filtroCancellationTokenSource?
            .Cancel();

        LblCargando.Text =
            "Cargando milongas...";

        LblCargando.Visible =
            true;

        FlpMilongas.Visible =
            false;

        BtnCargar.Enabled =
            false;

        BtnCargar.Text =
            "Cargando...";

        HabilitarControlesAgenda(
            false);

        agenda =
            new List<Milonga>();

        milongasConDetalleCargado
            .Clear();

        agendaCargada =
            false;

        agendaCompletaCargada =
            false;

        fechaActivaWeb =
            null;

        bool primerDiaMostrado =
            false;

        try
        {
            DateOnly fechaReferencia =
                DateOnly.FromDateTime(
                    DateTime.Today);

            await foreach (
                AgendaResultado resultadoDia
                in hoyMilongaService
                    .ObtenerAgendaProgresivaAsync(
                        Url,
                        fechaReferencia))
            {
                if (resultadoDia.Milongas.Count == 0)
                {
                    continue;
                }

                agenda.AddRange(
                    resultadoDia.Milongas);

                if (!primerDiaMostrado)
                {
                    fechaActivaWeb =
                        resultadoDia.FechaActiva;

                    agendaCargada =
                        true;

                    DateOnly fechaSeleccionada =
                        resultadoDia.FechaActiva;

                    LblCargando.Visible =
                        false;

                    FlpMilongas.Visible =
                        true;

                    await ProgramarActualizacionAsync(
                        fechaSeleccionada,
                        TxtBuscar.Text,
                        aplicarDemora: false);

                    primerDiaMostrado =
                        true;
                }
                else
                {
                    await PrecargarDetallesAsync(
                        resultadoDia.Milongas);
                }
            }

            agendaCompletaCargada =
                true;

            ActualizarFiltrosFinales();
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
            LblCargando.Visible =
                false;

            FlpMilongas.Visible =
                true;

            BtnCargar.Enabled =
                true;

            BtnCargar.Text =
                "Cargar agenda";

            HabilitarControlesAgenda(
                agendaCompletaCargada);
        }
    }

    private void HabilitarControlesAgenda(
        bool habilitar)
    {
        CmbFecha.Enabled =
            habilitar;

        TxtBuscar.Enabled =
            habilitar;

        CmbOrden.Enabled =
            habilitar;

        CmbBarrio.Enabled =
            habilitar;

        CmbClase.Enabled =
            habilitar;
    }

    private DateOnly? CargarFechas()
    {
        actualizandoFechas =
            true;

        try
        {
            DateOnly? fechaSeleccionadaAnterior =
                CmbFecha.SelectedItem
                    is DateOnly fecha
                        ? fecha
                        : null;

            List<DateOnly> fechas =
                agenda
                    .Select(
                        milonga =>
                            milonga.Fecha)
                    .Distinct()
                    .OrderBy(
                        fecha =>
                            fecha)
                    .ToList();

            CmbFecha.Items.Clear();

            foreach (
                DateOnly fechaDisponible
                in fechas)
            {
                CmbFecha.Items.Add(
                    fechaDisponible);
            }

            if (CmbFecha.Items.Count == 0)
            {
                return null;
            }

            // Si el usuario ya estaba viendo
            // una fecha, mantenemos la selección.
            if (fechaSeleccionadaAnterior.HasValue &&
                CmbFecha.Items.Contains(
                    fechaSeleccionadaAnterior.Value))
            {
                CmbFecha.SelectedItem =
                    fechaSeleccionadaAnterior.Value;
            }
            // En la primera carga usamos
            // la fecha activa informada por
            // Hoy Milonga.
            else if (
                fechaActivaWeb.HasValue &&
                CmbFecha.Items.Contains(
                    fechaActivaWeb.Value))
            {
                CmbFecha.SelectedItem =
                    fechaActivaWeb.Value;
            }
            else
            {
                CmbFecha.SelectedIndex =
                    0;
            }

            return CmbFecha.SelectedItem
                is DateOnly fechaSeleccionada
                    ? fechaSeleccionada
                    : null;
        }
        finally
        {
            actualizandoFechas =
                false;
        }
    }

    private void CargarBarrios()
    {
        actualizandoFiltros =
            true;

        try
        {
            string? barrioSeleccionado =
                CmbBarrio.SelectedItem
                    as string;

            List<string> barrios =
                agendaService.ObtenerBarrios(
                    agenda);

            CmbBarrio.Items.Clear();

            CmbBarrio.Items.Add(
                "Todos");

            foreach (
                string barrio
                in barrios)
            {
                CmbBarrio.Items.Add(
                    barrio);
            }

            if (!string.IsNullOrWhiteSpace(
                    barrioSeleccionado) &&
                CmbBarrio.Items.Contains(
                    barrioSeleccionado))
            {
                CmbBarrio.SelectedItem =
                    barrioSeleccionado;
            }
            else
            {
                CmbBarrio.SelectedIndex =
                    0;
            }
        }
        finally
        {
            actualizandoFiltros =
                false;
        }
    }

    private void ConfigurarClase()
    {
        CmbClase.Items.Clear();

        CmbClase.Items.Add(
            "Todas");

        CmbClase.Items.Add(
            "Con clase");

        CmbClase.Items.Add(
            "Sin clase");

        CmbClase.SelectedIndex =
            0;
    }

    private void ConfigurarOrden()
    {
        CmbOrden.Items.Clear();

        CmbOrden.Items.Add(
            "Horario");

        CmbOrden.Items.Add(
            "Distancia");

        CmbOrden.SelectedIndex =
            0;
    }

    private async Task ProgramarActualizacionAsync(
        DateOnly fecha,
        string texto,
        bool aplicarDemora)
    {
        filtroCancellationTokenSource?
            .Cancel();

        filtroCancellationTokenSource?
            .Dispose();

        filtroCancellationTokenSource =
            new CancellationTokenSource();

        CancellationToken cancellationToken =
            filtroCancellationTokenSource.Token;

        try
        {
            // Evita ejecutar una búsqueda
            // por cada letra escrita.
            if (aplicarDemora)
            {
                await Task.Delay(
                    400,
                    cancellationToken);
            }

            await MostrarMilongasAsync(
                fecha,
                texto,
                cancellationToken,
                mostrarCarga:
                    !aplicarDemora);
        }
        catch (OperationCanceledException)
        {
            // Es normal cancelar una
            // actualización anterior.
        }
    }

    private async Task MostrarMilongasAsync(
        DateOnly fecha,
        string texto,
        CancellationToken cancellationToken,
        bool mostrarCarga)
    {
        if (mostrarCarga)
        {
            LblCargando.Visible =
                true;

            FlpMilongas.Visible =
                false;
        }

        try
        {
            string? barrioSeleccionado =
                ObtenerBarrioSeleccionado();

            bool? tieneClase =
                ObtenerFiltroClase();

            FiltroAgenda filtro =
                new()
                {
                    Fecha =
                        fecha,

                    Texto =
                        texto,

                    Barrio =
                        barrioSeleccionado,

                    Cancelada =
                        null,

                    TieneClase =
                        tieneClase
                };

            List<Milonga> resultado =
                agendaService.Filtrar(
                    agenda,
                    filtro);

            cancellationToken
                .ThrowIfCancellationRequested();

            if (resultado.Count == 0)
            {
                ActualizarTabla(
                    resultado);

                return;
            }

            List<Milonga> milongasPendientes =
                resultado
                    .Where(
                        milonga =>
                            !milongasConDetalleCargado
                                .Contains(
                                    milonga))
                    .ToList();

            if (milongasPendientes.Count > 0)
            {
                await navegadorSemaphore.WaitAsync(
                    cancellationToken);

                try
                {
                    await hoyMilongaService
                        .CompletarDetallesAsync(
                            milongasPendientes);

                    foreach (
                        Milonga milonga
                        in milongasPendientes)
                    {
                        milongasConDetalleCargado
                            .Add(
                                milonga);
                    }
                }
                catch (PlaywrightException ex)
                {
                    if (!cancellationToken
                        .IsCancellationRequested)
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
                    navegadorSemaphore
                        .Release();
                }
            }

            cancellationToken
                .ThrowIfCancellationRequested();

            distanciaService.CalcularDistancias(
                resultado,
                LatitudOrigenTemporal,
                LongitudOrigenTemporal);

            if (CmbOrden.SelectedItem?
                    .ToString()
                == "Distancia")
            {
                resultado =
                    distanciaService
                        .OrdenarPorDistancia(
                            resultado);
            }

            cancellationToken
                .ThrowIfCancellationRequested();

            ActualizarTabla(
                resultado);
        }
        finally
        {
            if (mostrarCarga)
            {
                LblCargando.Visible =
                    false;

                FlpMilongas.Visible =
                    true;
            }
        }
    }

    private string?
        ObtenerBarrioSeleccionado()
    {
        if (CmbBarrio.SelectedItem
                is not string barrio ||
            barrio == "Todos")
        {
            return null;
        }

        return barrio;
    }

    private bool?
        ObtenerFiltroClase()
    {
        if (CmbClase.SelectedItem
            is not string clase)
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

        foreach (
            Milonga milonga
            in milongas)
        {
            MilongaCard card =
                new MilongaCard(
                    ObtenerDetalleSeguroAsync,
                    () =>
                        agendaCompletaCargada);

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
            FlpMilongas.ClientSize.Width -
            10;

        foreach (
            Control control
            in FlpMilongas.Controls)
        {
            control.Width =
                anchoDisponible -
                control.Margin.Horizontal;
        }
    }

    private async void
        CmbFecha_SelectedIndexChanged(
            object sender,
            EventArgs e)
    {
        if (actualizandoInterfaz ||
            actualizandoFechas ||
            !agendaCargada ||
            CmbFecha.SelectedItem
                is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: false);
    }

    private async void
        TxtBuscar_TextChanged(
            object sender,
            EventArgs e)
    {
        if (actualizandoInterfaz ||
            !agendaCargada ||
            CmbFecha.SelectedItem
                is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: true);
    }

    private async void
        CmbOrden_SelectedIndexChanged(
            object sender,
            EventArgs e)
    {
        if (actualizandoInterfaz ||
            !agendaCargada ||
            CmbFecha.SelectedItem
                is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: false);
    }

    private async void
        CmbBarrio_SelectedIndexChanged(
            object sender,
            EventArgs e)
    {
        if (actualizandoInterfaz ||
            actualizandoFiltros ||
            !agendaCargada ||
            CmbFecha.SelectedItem
                is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: false);
    }

    private async void
        CmbClase_SelectedIndexChanged(
            object sender,
            EventArgs e)
    {
        if (actualizandoInterfaz ||
            !agendaCargada ||
            CmbFecha.SelectedItem
                is not DateOnly fecha)
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
        filtroCancellationTokenSource?
            .Cancel();

        filtroCancellationTokenSource?
            .Dispose();

        await hoyMilongaService
            .DisposeAsync();

        navegadorSemaphore
            .Dispose();
    }

    private async Task<MilongaDetalle>
        ObtenerDetalleSeguroAsync(
            Milonga milonga)
    {
        await navegadorSemaphore
            .WaitAsync();

        try
        {
            return await hoyMilongaService
                .ObtenerDetalleAsync(
                    milonga);
        }
        finally
        {
            navegadorSemaphore
                .Release();
        }
    }

    private void ActualizarFiltrosFinales()
    {
        actualizandoInterfaz =
            true;

        SuspendLayout();

        CmbFecha.BeginUpdate();
        CmbBarrio.BeginUpdate();

        try
        {
            CargarBarrios();
            CargarFechas();

            HabilitarControlesAgenda(
                true);
        }
        finally
        {
            CmbFecha.EndUpdate();
            CmbBarrio.EndUpdate();

            ResumeLayout(
                true);

            actualizandoInterfaz =
                false;
        }
    }

    private async Task PrecargarDetallesAsync(
        List<Milonga> milongas)
    {
        List<Milonga> pendientes =
            milongas
                .Where(
                    milonga =>
                        !milongasConDetalleCargado
                            .Contains(
                                milonga))
                .ToList();

        if (pendientes.Count == 0)
        {
            return;
        }

        await navegadorSemaphore
            .WaitAsync();

        try
        {
            await hoyMilongaService
                .CompletarDetallesAsync(
                    pendientes);

            foreach (
                Milonga milonga
                in pendientes)
            {
                milongasConDetalleCargado
                    .Add(
                        milonga);
            }

            distanciaService.CalcularDistancias(
                pendientes,
                LatitudOrigenTemporal,
                LongitudOrigenTemporal);
        }
        catch (PlaywrightException)
        {
            // La precarga es una optimización.
            // Si una ficha falla acá, podrá
            // cargarse posteriormente al necesitarla.
        }
        finally
        {
            navegadorSemaphore
                .Release();
        }
    }
}