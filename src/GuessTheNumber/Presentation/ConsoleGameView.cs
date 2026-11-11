using GuessTheNumber.Application;
using GuessTheNumber.Domain;

namespace GuessTheNumber.Presentation;

/// <summary>
/// Консольная реализация вывода и ввода.
/// </summary>
public sealed class ConsoleGameView : IGameView, IInputReader
{
    public void ShowMainMenu(
        string playerName,
        DifficultyLevel difficulty,
        GameStatistics statistics)
    {
        Console.WriteLine();
        Console.WriteLine("- Главное меню -");
        Console.WriteLine($"Игрок: {playerName}");
        Console.WriteLine(
            $"Сложность: {difficulty.Title} " +
            $"({difficulty.MinNumber}–{difficulty.MaxNumber}, попыток: {difficulty.MaxAttempts})");
        Console.WriteLine(
            statistics.GamesPlayed == 0
                ? "Статистика: пока нет сыгранных партий"
                : $"Статистика: побед {statistics.Wins} из {statistics.GamesPlayed}");
        Console.WriteLine("  1. Играть");
        Console.WriteLine("  2. Настройки");
        Console.WriteLine("  3. Статистика");
        Console.WriteLine("  0. Выход");
    }

    public void ShowStatistics(GameStatistics statistics)
    {
        Console.WriteLine();
        Console.WriteLine("- Статистика -");

        if (statistics.GamesPlayed == 0)
        {
            Console.WriteLine("Пока нет сыгранных партий.");
            return;
        }

        Console.WriteLine($"Игр: {statistics.GamesPlayed}");
        Console.WriteLine($"Побед: {statistics.Wins}");
        Console.WriteLine($"Поражений: {statistics.Losses}");
        Console.WriteLine($"Процент побед: {statistics.GetWinRate():0.#}%");
        Console.WriteLine($"Среднее попыток за игру: {statistics.GetAverageAttempts():0.#}");

        if (statistics.BestAttempts is not null)
        {
            Console.WriteLine($"Лучший результат (попыток): {statistics.BestAttempts}");
        }
    }

    public void ShowSettings(GameSettings settings)
    {
        var difficulty = settings.ResolveDifficulty();
        var custom = settings.CreateCustomDifficulty();

        Console.WriteLine();
        Console.WriteLine("- Настройки -");
        Console.WriteLine($"Игрок: {settings.PlayerName}");
        Console.WriteLine($"Сложность по умолчанию: {difficulty.Title}");
        Console.WriteLine($"Подсказки: {(settings.HintsEnabled ? "включены" : "выключены")}");
        Console.WriteLine(
            $"Своя сложность: {custom.MinNumber}–{custom.MaxNumber}, " +
            $"попыток: {custom.MaxAttempts}");
        Console.WriteLine("  1. Изменить имя");
        Console.WriteLine("  2. Выбрать сложность");
        Console.WriteLine("  3. Включить/выключить подсказки");
        Console.WriteLine("  4. Настроить свою сложность");
        Console.WriteLine("  0. Назад");
    }

    public void ShowDifficultyMenu(
        IReadOnlyList<DifficultyLevel> levels,
        string currentDifficultyKey)
    {
        Console.WriteLine();
        Console.WriteLine("- Выбор сложности -");

        for (var index = 0; index < levels.Count; index++)
        {
            var level = levels[index];
            var marker = level.Key == currentDifficultyKey ? "  <- по умолчанию" : string.Empty;
            Console.WriteLine(
                $"  {index + 1}. {level.Title} — числа {level.MinNumber}–{level.MaxNumber}, " +
                $"попыток: {level.MaxAttempts}{marker}");
        }

        Console.WriteLine("  0. Назад");
    }

    public void ShowGameStarted(DifficultyLevel difficulty, bool hintsEnabled)
    {
        Console.WriteLine();
        Console.WriteLine("- Новая игра -");
        Console.WriteLine($"Сложность: {difficulty.Title}");
        Console.WriteLine($"Диапазон:  {difficulty.MinNumber}–{difficulty.MaxNumber}");
        Console.WriteLine($"Попыток:   {difficulty.MaxAttempts}");
        Console.WriteLine($"Подсказки: {(hintsEnabled ? "включены" : "выключены")}");
        Console.WriteLine("Введите q, чтобы прервать игру.");
    }

    public void ShowGuessResult(GuessMadeEventArgs args)
    {
        Console.WriteLine();

        switch (args.Outcome)
        {
            case GuessOutcome.TooLow:
                Console.WriteLine("Больше! Загаданное число больше вашего варианта.");
                break;
            case GuessOutcome.TooHigh:
                Console.WriteLine("Меньше! Загаданное число меньше вашего варианта.");
                break;
            case GuessOutcome.Correct:
                Console.WriteLine("Верно! Вы угадали число.");
                break;
        }

        Console.WriteLine($"Осталось попыток: {args.AttemptsLeft}.");
    }

    public void ShowGameFinished(GameFinishedEventArgs args)
    {
        Console.WriteLine();

        if (args.IsWon)
        {
            Console.WriteLine(
                $"Победа! Число {args.TargetNumber} угадано за {args.AttemptsUsed} " +
                $"{AttemptsWord(args.AttemptsUsed)}.");
        }
        else
        {
            Console.WriteLine($"Попытки закончились. Было загадано число {args.TargetNumber}.");
        }

        Console.WriteLine();
    }

    private static string AttemptsWord(int count)
    {
        var lastTwo = count % 100;
        var last = count % 10;

        if (lastTwo is >= 11 and <= 14)
        {
            return "попыток";
        }

        return last switch
        {
            1 => "попытку",
            2 or 3 or 4 => "попытки",
            _ => "попыток",
        };
    }

    public void ShowHint(int lowerBound, int upperBound)
    {
        Console.WriteLine($"Подсказка: загаданное число находится в диапазоне {lowerBound}–{upperBound}.");
    }

    public void ShowMessage(string message) => Console.WriteLine(message);

    public void ShowError(string message) => Console.WriteLine($"Ошибка: {message}");

    public string ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? "q";
    }
}
