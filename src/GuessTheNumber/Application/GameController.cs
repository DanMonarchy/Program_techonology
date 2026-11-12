using GuessTheNumber.Domain;

namespace GuessTheNumber.Application;

/// <summary>
/// Вывод игры. Контроллер не знает, как именно отображается информация.
/// </summary>
public interface IGameView
{
    void ShowMainMenu(
        string playerName,
        DifficultyLevel difficulty,
        GameStatistics statistics);

    void ShowSettings(GameSettings settings);

    void ShowStatistics(GameStatistics statistics);

    void ShowDifficultyMenu(IReadOnlyList<DifficultyLevel> levels, string currentDifficultyKey);

    void ShowGameStarted(DifficultyLevel difficulty, bool hintsEnabled);

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
/// Сохранение и загрузка пользовательских настроек.
/// </summary>
public interface ISettingsStore
{
    GameSettings Load();

    void Save(GameSettings settings);
}

/// <summary>
/// Сохранение и загрузка статистики игр.
/// </summary>
public interface IStatisticsStore
{
    GameStatistics Load();

    void Save(GameStatistics statistics);
}

/// <summary>
/// Сценарий игры: главное меню, настройки и игровые партии.
/// </summary>
public sealed class GameController
{
    private readonly INumberGenerator _numberGenerator;
    private readonly IGameView _gameView;
    private readonly IInputReader _input;
    private readonly ISettingsStore _settingsStore;
    private readonly IStatisticsStore _statisticsStore;
    private readonly GameSettings _settings;
    private readonly GameStatistics _statistics;

    public GameController(
        INumberGenerator numberGenerator,
        IGameView gameView,
        IInputReader input,
        ISettingsStore settingsStore,
        IStatisticsStore statisticsStore)
    {
        _numberGenerator = numberGenerator ?? throw new ArgumentNullException(nameof(numberGenerator));
        _gameView = gameView ?? throw new ArgumentNullException(nameof(gameView));
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _settingsStore = settingsStore ?? throw new ArgumentNullException(nameof(settingsStore));
        _statisticsStore = statisticsStore ?? throw new ArgumentNullException(nameof(statisticsStore));
        _settings = _settingsStore.Load();
        _statistics = _statisticsStore.Load();
    }

    public void Run()
    {
        while (true)
        {
            _gameView.ShowMainMenu(
                _settings.PlayerName,
                _settings.ResolveDifficulty(),
                _statistics);
            var input = _input.ReadLine("Выберите пункт: ").Trim();

            switch (input)
            {
                case "1":
                    Play(_settings.ResolveDifficulty());
                    break;
                case "2":
                    ShowSettingsMenu();
                    break;
                case "3":
                    _gameView.ShowStatistics(_statistics);
                    break;
                case "0":
                    _gameView.ShowMessage("До свидания!");
                    return;
                default:
                    _gameView.ShowError("Введите 1, 2, 3 или 0.");
                    break;
            }
        }
    }

    private void ShowSettingsMenu()
    {
        while (true)
        {
            _gameView.ShowSettings(_settings);
            var input = _input.ReadLine(
                "1 — имя, 2 — сложность, 3 — подсказки, 0 — назад: ").Trim();

            switch (input)
            {
                case "1":
                    ChangeName();
                    break;
                case "2":
                    ChooseDefaultDifficulty();
                    break;
                case "3":
                    _settings.HintsEnabled = !_settings.HintsEnabled;
                    SaveSettings();
                    _gameView.ShowMessage(
                        $"Подсказки {(_settings.HintsEnabled ? "включены" : "выключены")}.");
                    break;
                case "0":
                    return;
                default:
                    _gameView.ShowError("Введите 1–3 или 0.");
                    break;
            }
        }
    }

    private void ChangeName()
    {
        var name = _input.ReadLine(
            $"Новое имя (Enter — оставить «{_settings.PlayerName}»): ").Trim();

        if (name.Length == 0)
        {
            return;
        }

        _settings.PlayerName = name;
        SaveSettings();
        _gameView.ShowMessage($"Имя сохранено: {name}.");
    }

    private void ChooseDefaultDifficulty()
    {
        while (true)
        {
            _gameView.ShowDifficultyMenu(DifficultyLevel.All, _settings.DifficultyKey);
            var input = _input.ReadLine("Выберите сложность (0 — назад): ").Trim();

            if (input == "0")
            {
                return;
            }

            if (!int.TryParse(input, out var choice) ||
                choice < 1 ||
                choice > DifficultyLevel.All.Count)
            {
                _gameView.ShowError(
                    $"Введите число от 1 до {DifficultyLevel.All.Count} или 0.");
                continue;
            }

            var level = DifficultyLevel.All[choice - 1];
            _settings.DifficultyKey = level.Key;
            SaveSettings();
            _gameView.ShowMessage($"Сложность по умолчанию: {level.Title}.");
            return;
        }
    }

    private void SaveSettings() => _settingsStore.Save(_settings);

    private void Play(DifficultyLevel difficulty)
    {
        var session = new GameSession(_numberGenerator, difficulty);

        // События игры уведомляют представление о попытках и завершении партии.
        session.GuessMade += (_, args) => _gameView.ShowGuessResult(args);
        session.GameFinished += (_, args) =>
        {
            _gameView.ShowGameFinished(args);
            _statistics.RecordGame(args);
            _statisticsStore.Save(_statistics);
        };

        _gameView.ShowGameStarted(difficulty, _settings.HintsEnabled);

        while (!session.IsFinished)
        {
            var hintPart = _settings.HintsEnabled ? ", h — подсказка" : string.Empty;
            var input = _input.ReadLine(
                $"Ваш вариант ({difficulty.MinNumber}–{difficulty.MaxNumber}; " +
                $"осталось {session.AttemptsLeft}){hintPart}, q — выход: ").Trim();

            if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                _gameView.ShowMessage("Игра прервана.");
                return;
            }

            if (input.Equals("h", StringComparison.OrdinalIgnoreCase))
            {
                if (!_settings.HintsEnabled)
                {
                    _gameView.ShowError("Подсказки отключены в настройках.");
                    continue;
                }

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
