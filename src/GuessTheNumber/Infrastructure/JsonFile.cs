using System.Text.Encodings.Web;
using System.Text.Json;

namespace GuessTheNumber.Infrastructure;

/// <summary>
/// Общее чтение и сохранение данных в JSON-файл.
/// </summary>
internal static class JsonFile
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static T Load<T>(string filePath) where T : new()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return new T();
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, Options) ?? new T();
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or JsonException)
        {
            // Повреждённый файл не должен мешать запуску игры.
            return new T();
        }
    }

    public static void Save<T>(string filePath, T value)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(value, Options);
        File.WriteAllText(filePath, json);
    }
}
