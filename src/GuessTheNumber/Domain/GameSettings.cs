namespace GuessTheNumber.Domain;

/// <summary>
/// Пользовательские настройки, которые сохраняются между запусками.
/// </summary>
public sealed class GameSettings
{
    public string PlayerName { get; set; } = "Игрок";

    public string DifficultyKey { get; set; } = DifficultyLevel.Medium.Key;

    public bool HintsEnabled { get; set; } = true;

    /// <summary>
    /// Возвращает сложность, выбранную пользователем по умолчанию.
    /// </summary>
    public DifficultyLevel ResolveDifficulty()
    {
        return DifficultyKey switch
        {
            "easy" => DifficultyLevel.Easy,
            "hard" => DifficultyLevel.Hard,
            _ => DifficultyLevel.Medium,
        };
    }
}
