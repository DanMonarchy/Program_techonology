using GuessTheNumber.Application;
using GuessTheNumber.Domain;

namespace GuessTheNumber.Infrastructure;

/// <summary>
/// Сохраняет статистику в JSON-файл и загружает её при запуске.
/// </summary>
public sealed class StatisticsStore : IStatisticsStore
{
    private readonly string _filePath;

    public StatisticsStore(string filePath)
    {
        _filePath = filePath;
    }

    public GameStatistics Load() => JsonFile.Load<GameStatistics>(_filePath);

    public void Save(GameStatistics statistics) => JsonFile.Save(_filePath, statistics);
}
