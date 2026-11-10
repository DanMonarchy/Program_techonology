namespace GuessTheNumber.Domain;

/// <summary>
/// Пользовательские настройки, которые сохраняются между запусками.
/// </summary>
public sealed class GameSettings
{
    public string PlayerName { get; set; } = "Игрок";

    public string DifficultyKey { get; set; } = DifficultyLevel.Medium.Key;

    public bool HintsEnabled { get; set; } = true;

    public int CustomMinNumber { get; set; } = 1;

    public int CustomMaxNumber { get; set; } = 1000;

    public int CustomMaxAttempts { get; set; } = 10;

    /// <summary>
    /// Возвращает сложность, выбранную пользователем по умолчанию.
    /// </summary>
    public DifficultyLevel ResolveDifficulty()
    {
        return DifficultyKey switch
        {
            "easy" => DifficultyLevel.Easy,
            "hard" => DifficultyLevel.Hard,
            "custom" => CreateCustomDifficulty(),
            _ => DifficultyLevel.Medium,
        };
    }

    /// <summary>
    /// Создаёт уровень «Своя сложность» из сохранённых параметров.
    /// </summary>
    public DifficultyLevel CreateCustomDifficulty()
    {
        return DifficultyLevel.Custom(CustomMinNumber, CustomMaxNumber, CustomMaxAttempts);
    }
}
