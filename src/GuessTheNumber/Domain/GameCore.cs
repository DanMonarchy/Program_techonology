namespace GuessTheNumber.Domain;

/// <summary>
/// Уровень сложности: диапазон чисел и количество попыток.
/// </summary>
public sealed record DifficultyLevel(string Title, int MinNumber, int MaxNumber, int MaxAttempts)
{
    public static DifficultyLevel Easy { get; } = new("Лёгкий", 1, 50, 10);

    public static DifficultyLevel Medium { get; } = new("Средний", 1, 100, 8);

    public static DifficultyLevel Hard { get; } = new("Сложный", 1, 500, 7);

    public static IReadOnlyList<DifficultyLevel> All { get; } = new[] { Easy, Medium, Hard };
}

/// <summary>
/// Результат сравнения попытки с загаданным числом.
/// </summary>
public enum GuessOutcome
{
    TooLow,
    TooHigh,
    Correct,
}

/// <summary>
/// Источник случайных чисел. Интерфейс позволяет заменить генератор в тестах.
/// </summary>
public interface INumberGenerator
{
    int Next(int minInclusive, int maxInclusive);
}

/// <summary>
/// Данные события о попытке игрока.
/// </summary>
public sealed record GuessMadeEventArgs(
    int Guess,
    GuessOutcome Outcome,
    int AttemptsLeft,
    int LowerBound,
    int UpperBound);

/// <summary>
/// Данные события о завершении игры.
/// </summary>
public sealed record GameFinishedEventArgs(
    bool IsWon,
    int TargetNumber,
    int AttemptsUsed);

/// <summary>
/// Одна игровая партия: загаданное число, попытки и подсказки.
/// Класс не знает о консоли и файлах.
/// </summary>
public sealed class GameSession
{
    private readonly INumberGenerator _numberGenerator;
    private int _attemptsUsed;

    public GameSession(INumberGenerator numberGenerator, DifficultyLevel difficulty)
    {
        _numberGenerator = numberGenerator ?? throw new ArgumentNullException(nameof(numberGenerator));
        Difficulty = difficulty ?? throw new ArgumentNullException(nameof(difficulty));

        TargetNumber = _numberGenerator.Next(difficulty.MinNumber, difficulty.MaxNumber);
        LowerBound = difficulty.MinNumber;
        UpperBound = difficulty.MaxNumber;
    }

    public DifficultyLevel Difficulty { get; }

    public int TargetNumber { get; }

    public int AttemptsUsed => _attemptsUsed;

    public int AttemptsLeft => Difficulty.MaxAttempts - _attemptsUsed;

    public bool IsFinished { get; private set; }

    public bool IsWon { get; private set; }

    public int LowerBound { get; private set; }

    public int UpperBound { get; private set; }

    /// <summary>Событие вызывается после каждой попытки.</summary>
    public event EventHandler<GuessMadeEventArgs>? GuessMade;

    /// <summary>Событие вызывается один раз — когда игра выиграна или попытки закончились.</summary>
    public event EventHandler<GameFinishedEventArgs>? GameFinished;

    public void MakeGuess(int value)
    {
        if (IsFinished)
        {
            throw new InvalidOperationException("Игра уже завершена.");
        }

        if (value < Difficulty.MinNumber || value > Difficulty.MaxNumber)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Число должно быть в диапазоне {Difficulty.MinNumber}–{Difficulty.MaxNumber}.");
        }

        _attemptsUsed++;

        var outcome = value < TargetNumber
            ? GuessOutcome.TooLow
            : value > TargetNumber
                ? GuessOutcome.TooHigh
                : GuessOutcome.Correct;

        if (outcome == GuessOutcome.TooLow)
        {
            LowerBound = Math.Max(LowerBound, value + 1);
        }
        else if (outcome == GuessOutcome.TooHigh)
        {
            UpperBound = Math.Min(UpperBound, value - 1);
        }

        GuessMade?.Invoke(
            this,
            new GuessMadeEventArgs(value, outcome, AttemptsLeft, LowerBound, UpperBound));

        if (outcome == GuessOutcome.Correct)
        {
            Finish(isWon: true);
        }
        else if (AttemptsLeft == 0)
        {
            Finish(isWon: false);
        }
    }

    private void Finish(bool isWon)
    {
        IsFinished = true;
        IsWon = isWon;
        GameFinished?.Invoke(this, new GameFinishedEventArgs(isWon, TargetNumber, _attemptsUsed));
    }
}
