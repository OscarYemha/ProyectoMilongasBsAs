using Milongas.Extractor.Models;
using System.Text.Json;

namespace Milongas.Extractor.Services;

public class DetalleCacheService
{
    private readonly string rutaArchivo;

    public DetalleCacheService()
    {
        rutaArchivo = Path.Combine(
            AppContext.BaseDirectory,
            "detalles-cache.json");
    }

    public async Task<Dictionary<int, DetalleMilongaCache>> CargarAsync()
    {
        if (!File.Exists(rutaArchivo))
        {
            return new Dictionary<int, DetalleMilongaCache>();
        }

        string json =
            await File.ReadAllTextAsync(rutaArchivo);

        JsonSerializerOptions opciones = new()
        {
            PropertyNameCaseInsensitive = true
        };

        Dictionary<int, DetalleMilongaCache>? cache =
            JsonSerializer.Deserialize<
                Dictionary<int, DetalleMilongaCache>>(
                json,
                opciones);

        return cache ??
            new Dictionary<int, DetalleMilongaCache>();
    }

    public async Task GuardarAsync(
        Dictionary<int, DetalleMilongaCache> cache)
    {
        JsonSerializerOptions opciones = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy =
                JsonNamingPolicy.CamelCase
        };

        string json =
            JsonSerializer.Serialize(
                cache,
                opciones);

        await File.WriteAllTextAsync(
            rutaArchivo,
            json);
    }
}