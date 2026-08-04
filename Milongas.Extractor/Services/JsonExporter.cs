using Milongas.Extractor.Models;
using System.Text.Json;

namespace Milongas.Extractor.Services;

public class JsonExporter
{
    public async Task<string> GuardarAsync(List<Milonga> milongas)
    {
        JsonSerializerOptions opciones = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string json = JsonSerializer.Serialize(
            milongas,
            opciones);

        string rutaArchivo = Path.Combine(
            AppContext.BaseDirectory,
            "milongas.json");

        await File.WriteAllTextAsync(
            rutaArchivo,
            json);

        return rutaArchivo;
    }
}