using System.Text.Encodings.Web;
using System.Text.Json;
using GuessTheNumber.Application;
using GuessTheNumber.Domain;

namespace GuessTheNumber.Infrastructure;

/// <summary>
/// Сохраняет настройки в JSON-файл и загружает их при запуске.
/// </summary>
public sealed class SettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly string _filePath;

    public SettingsStore(string filePath)
    {
        _filePath = filePath;
    }

    public GameSettings Load()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return new GameSettings();
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<GameSettings>(json, Options) ?? new GameSettings();
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or JsonException)
        {
            // Повреждённый файл не должен мешать запуску игры.
            return new GameSettings();
        }
    }

    public void Save(GameSettings settings)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(settings, Options);
        File.WriteAllText(_filePath, json);
    }
}
