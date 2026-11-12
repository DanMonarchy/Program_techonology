using GuessTheNumber.Application;
using GuessTheNumber.Domain;

namespace GuessTheNumber.Infrastructure;

/// <summary>
/// Сохраняет настройки в JSON-файл, загружает их при запуске.
/// </summary>
public sealed class SettingsStore : ISettingsStore
{
    private readonly string _filePath;

    public SettingsStore(string filePath)
    {
        _filePath = filePath;
    }

    public GameSettings Load() => JsonFile.Load<GameSettings>(_filePath);

    public void Save(GameSettings settings) => JsonFile.Save(_filePath, settings);
}
