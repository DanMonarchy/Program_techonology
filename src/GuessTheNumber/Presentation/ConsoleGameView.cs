using GuessTheNumber.Application;
using GuessTheNumber.Domain;

namespace GuessTheNumber.Presentation;

/// <summary>
/// Консольная реализация вывода и ввода.
/// </summary>
public sealed class ConsoleGameView : IGameView, IInputReader
{
    public void ShowDifficultyMenu(IReadOnlyList<DifficultyLevel> levels)
    {
        Console.WriteLine();
        Console.WriteLine("- Выбор сложности -");

        for (var index = 0; index < levels.Count; index++)
        {
            var level = levels[index];
            Console.WriteLine(
                $"  {index + 1}. {level.Title} — числа {level.MinNumber}–{level.MaxNumber}, " +
                $"попыток: {level.MaxAttempts}");
        }

        Console.WriteLine("  0. Выход");
    }

    public void ShowGameStarted(DifficultyLevel difficulty)
    {
        Console.WriteLine();
        Console.WriteLine("- Новая игра -");
        Console.WriteLine($"Сложность: {difficulty.Title}");
        Console.WriteLine($"Диапазон:  {difficulty.MinNumber}–{difficulty.MaxNumber}");
        Console.WriteLine($"Попыток:   {difficulty.MaxAttempts}");
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
            Console.WriteLine($"Победа! Число {args.TargetNumber} угадано за {args.AttemptsUsed} попыток.");
        }
        else
        {
            Console.WriteLine($"Попытки закончились. Было загадано число {args.TargetNumber}.");
        }

        Console.WriteLine();
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
