using GuessTheNumber.Domain;

namespace GuessTheNumber.Application;

/// <summary>
/// Вывод игры. Контроллер не знает, как именно отображается информация.
/// </summary>
public interface IGameView
{
    void ShowDifficultyMenu(IReadOnlyList<DifficultyLevel> levels);

    void ShowGameStarted(DifficultyLevel difficulty);

    void ShowGuessResult(GuessMadeEventArgs args);

    void ShowGameFinished(GameFinishedEventArgs args);

    void ShowHint(int lowerBound, int upperBound);

    void ShowMessage(string message);

    void ShowError(string message);
}

/// <summary>
/// Чтение пользовательского ввода.
/// </summary>
public interface IInputReader
{
    string ReadLine(string prompt);
}

/// <summary>
/// Сценарий игры: выбор сложности и одна партия.
/// </summary>
public sealed class GameController
{
    private readonly INumberGenerator _numberGenerator;
    private readonly IGameView _gameView;
    private readonly IInputReader _input;

    public GameController(
        INumberGenerator numberGenerator,
        IGameView gameView,
        IInputReader input)
    {
        _numberGenerator = numberGenerator ?? throw new ArgumentNullException(nameof(numberGenerator));
        _gameView = gameView ?? throw new ArgumentNullException(nameof(gameView));
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }

    public void Run()
    {
        var difficulty = ChooseDifficulty();
        if (difficulty is null)
        {
            return;
        }

        Play(difficulty);
    }

    private DifficultyLevel? ChooseDifficulty()
    {
        while (true)
        {
            _gameView.ShowDifficultyMenu(DifficultyLevel.All);
            var input = _input.ReadLine("Выберите сложность (0 — выход): ").Trim();

            if (input == "0")
            {
                return null;
            }

            if (int.TryParse(input, out var choice) &&
                choice >= 1 &&
                choice <= DifficultyLevel.All.Count)
            {
                return DifficultyLevel.All[choice - 1];
            }

            _gameView.ShowError($"Введите число от 1 до {DifficultyLevel.All.Count}.");
        }
    }

    private void Play(DifficultyLevel difficulty)
    {
        var session = new GameSession(_numberGenerator, difficulty);

        // События игры уведомляют представление о попытках и завершении партии.
        session.GuessMade += (_, args) => _gameView.ShowGuessResult(args);
        session.GameFinished += (_, args) => _gameView.ShowGameFinished(args);

        _gameView.ShowGameStarted(difficulty);

        while (!session.IsFinished)
        {
            var input = _input.ReadLine(
                $"Ваш вариант ({difficulty.MinNumber}–{difficulty.MaxNumber}; " +
                $"осталось {session.AttemptsLeft}), h — подсказка, q — выход: ").Trim();

            if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                _gameView.ShowMessage("Игра прервана.");
                return;
            }

            if (input.Equals("h", StringComparison.OrdinalIgnoreCase))
            {
                _gameView.ShowHint(session.LowerBound, session.UpperBound);
                continue;
            }

            if (!int.TryParse(input, out var guess))
            {
                _gameView.ShowError("Введите целое число.");
                continue;
            }

            if (guess < difficulty.MinNumber || guess > difficulty.MaxNumber)
            {
                _gameView.ShowError(
                    $"Число должно быть в диапазоне {difficulty.MinNumber}–{difficulty.MaxNumber}.");
                continue;
            }

            session.MakeGuess(guess);
        }
    }
}
