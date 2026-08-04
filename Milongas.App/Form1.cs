using Milongas.Extractor.Models;
using Milongas.Extractor.Services;
using Microsoft.Playwright;

namespace Milongas.App;

public partial class Form1 : Form
{
    private readonly HoyMilongaService hoyMilongaService;
    private readonly AgendaService agendaService;
    private readonly DistanciaService distanciaService;

    // Protege todas las operaciones que usan Chromium.
    private readonly SemaphoreSlim navegadorSemaphore =
        new(1, 1);

    // Permite cancelar búsquedas anteriores.
    private CancellationTokenSource? filtroCancellationTokenSource;

    private List<Milonga> agenda = new();

    private bool actualizandoFechas;
    private bool agendaCargada;

    private const string Url =
        "https://www.hoy-milonga.com/buenos-aires/es/milongas";

    public Form1()
    {
        InitializeComponent();

        hoyMilongaService =
            new HoyMilongaService();

        agendaService =
            new AgendaService();

        distanciaService =
            new DistanciaService();

        ConfigurarTabla();

        FormClosed += Form1_FormClosed;
    }

    private async void BtnCargar_Click(
        object sender,
        EventArgs e)
    {
        filtroCancellationTokenSource?.Cancel();

        BtnCargar.Enabled = false;
        CmbFecha.Enabled = false;
        TxtBuscar.Enabled = false;
        BtnCargar.Text = "Cargando...";

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
            CmbFecha.Enabled = agendaCargada;
            TxtBuscar.Enabled = agendaCargada;
            BtnCargar.Text = "Cargar agenda";
        }
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
                    .OrderBy(fechaItem => fechaItem)
                    .ToList();

            CmbFecha.Items.Clear();

            foreach (DateOnly fechaItem in fechas)
            {
                CmbFecha.Items.Add(fechaItem);
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

            if (CmbFecha.SelectedItem is DateOnly fechaSeleccionada)
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
            // Evita iniciar una navegación por cada letra escrita.
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
            // Es normal cuando el usuario cambia el texto o la fecha.
        }
    }

    private async Task MostrarMilongasAsync(
        DateOnly fecha,
        string texto,
        CancellationToken cancellationToken)
    {
        FiltroAgenda filtro = new()
        {
            Fecha = fecha,
            Texto = texto,
            Cancelada = false
        };

        List<Milonga> resultado =
            agendaService.Filtrar(
                agenda,
                filtro);

        cancellationToken.ThrowIfCancellationRequested();

        // Mostramos primero los datos básicos inmediatamente.
        ActualizarTabla(resultado);

        if (resultado.Count == 0)
        {
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

        resultado =
            distanciaService.OrdenarPorDistancia(
                resultado);

        cancellationToken.ThrowIfCancellationRequested();

        ActualizarTabla(resultado);
    }

    private void ActualizarTabla(
        List<Milonga> milongas)
    {
        DgvMilongas.DataSource = null;
        DgvMilongas.DataSource = milongas;
    }

    private void ConfigurarTabla()
    {
        DgvMilongas.AutoGenerateColumns = false;
        DgvMilongas.Columns.Clear();

        DgvMilongas.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                HeaderText = "Horario",
                DataPropertyName = "Horario",
                Width = 110
            });

        DgvMilongas.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                HeaderText = "Milonga",
                DataPropertyName = "Nombre",
                Width = 220
            });

        DgvMilongas.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                HeaderText = "Salón",
                DataPropertyName = "Salon",
                Width = 200
            });

        DgvMilongas.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                HeaderText = "Barrio",
                DataPropertyName = "Barrio",
                Width = 130
            });

        DgvMilongas.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                HeaderText = "Distancia",
                DataPropertyName = "DistanciaKm",
                Width = 90,
                DefaultCellStyle =
                    new DataGridViewCellStyle
                    {
                        Format = "0.0 'km'"
                    }
            });

        DgvMilongas.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                HeaderText = "Clase",
                DataPropertyName = "HorarioClase",
                Width = 120
            });
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
        if (actualizandoFechas ||
            !agendaCargada ||
            CmbFecha.SelectedItem is not DateOnly fecha)
        {
            return;
        }

        await ProgramarActualizacionAsync(
            fecha,
            TxtBuscar.Text,
            aplicarDemora: true);
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
}