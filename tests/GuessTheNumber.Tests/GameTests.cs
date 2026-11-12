using GuessTheNumber.Domain;
using GuessTheNumber.Infrastructure;

namespace GuessTheNumber.Tests;

public sealed class GameTests
{
    private static DifficultyLevel TestDifficulty =>
        new("test", "Тест", 1, 100, 3);

    [Fact]
    public void CorrectGuessFinishesGameAsWin()
    {
        var session = new GameSession(new FixedNumberGenerator(42), TestDifficulty);
        var finished = new List<GameFinishedEventArgs>();
        session.GameFinished += (_, args) => finished.Add(args);

        session.MakeGuess(42);

        Assert.True(session.IsFinished);
        Assert.True(session.IsWon);
        Assert.Equal(1, session.AttemptsUsed);

        var result = Assert.Single(finished);
        Assert.True(result.IsWon);
        Assert.Equal(42, result.TargetNumber);
        Assert.Equal(1, result.AttemptsUsed);
    }

    [Fact]
    public void WrongGuessesUpdateBounds()
    {
        var session = new GameSession(new FixedNumberGenerator(50), TestDifficulty);

        session.MakeGuess(30);
        Assert.Equal(31, session.LowerBound);

        session.MakeGuess(70);
        Assert.Equal(69, session.UpperBound);
    }

    [Fact]
    public void GameFinishesAsLossWhenAttemptsRunOut()
    {
        var session = new GameSession(new FixedNumberGenerator(100), TestDifficulty);
        var finished = new List<GameFinishedEventArgs>();
        session.GameFinished += (_, args) => finished.Add(args);

        session.MakeGuess(1);
        session.MakeGuess(2);
        session.MakeGuess(3);

        Assert.True(session.IsFinished);
        Assert.False(session.IsWon);
        Assert.Equal(3, session.AttemptsUsed);

        var result = Assert.Single(finished);
        Assert.False(result.IsWon);
        Assert.Equal(3, result.AttemptsUsed);
    }

    [Fact]
    public void GameFinishedEventIsRaisedAfterGuessMade()
    {
        var session = new GameSession(new FixedNumberGenerator(5), TestDifficulty);
        var events = new List<string>();

        session.GuessMade += (_, _) => events.Add("guess");
        session.GameFinished += (_, _) => events.Add("finished");

        session.MakeGuess(5);

        Assert.Equal(new[] { "guess", "finished" }, events);
    }

    [Fact]
    public void StatisticsAggregateGames()
    {
        var statistics = new GameStatistics();

        statistics.RecordGame(new GameFinishedEventArgs(true, 10, 3));
        statistics.RecordGame(new GameFinishedEventArgs(true, 20, 1));
        statistics.RecordGame(new GameFinishedEventArgs(false, 30, 5));

        Assert.Equal(3, statistics.GamesPlayed);
        Assert.Equal(2, statistics.Wins);
        Assert.Equal(1, statistics.Losses);
        Assert.Equal(9, statistics.TotalAttempts);
        Assert.Equal(1, statistics.BestAttempts!.Value);
        Assert.Equal(66.667, statistics.GetWinRate(), 3);
        Assert.Equal(3.0, statistics.GetAverageAttempts(), 3);
    }

    [Theory]
    [InlineData("easy", "easy")]
    [InlineData("hard", "hard")]
    [InlineData("unknown", "medium")]
    public void SettingsResolveOnlyBuiltInDifficulties(string key, string expectedKey)
    {
        var settings = new GameSettings { DifficultyKey = key };

        Assert.Equal(expectedKey, settings.ResolveDifficulty().Key);
    }

    [Fact]
    public void DifficultyListContainsOnlyBuiltInLevels()
    {
        Assert.Equal(3, DifficultyLevel.All.Count);
        Assert.DoesNotContain(DifficultyLevel.All, level => level.Key == "custom");
    }

    [Fact]
    public void SettingsStoreSavesAndLoadsSettings()
    {
        var path = CreateTempPath();

        try
        {
            var store = new SettingsStore(path);
            store.Save(new GameSettings
            {
                PlayerName = "Аня",
                DifficultyKey = "hard",
                HintsEnabled = false,
            });

            var loaded = store.Load();

            Assert.Equal("Аня", loaded.PlayerName);
            Assert.Equal("hard", loaded.DifficultyKey);
            Assert.False(loaded.HintsEnabled);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void StatisticsStoreSavesAndLoadsStatistics()
    {
        var path = CreateTempPath();

        try
        {
            var store = new StatisticsStore(path);
            store.Save(new GameStatistics
            {
                GamesPlayed = 2,
                Wins = 1,
                Losses = 1,
                TotalAttempts = 5,
                BestAttempts = 2,
            });

            var loaded = store.Load();

            Assert.Equal(2, loaded.GamesPlayed);
            Assert.Equal(1, loaded.Wins);
            Assert.Equal(1, loaded.Losses);
            Assert.Equal(5, loaded.TotalAttempts);
            Assert.Equal(2, loaded.BestAttempts!.Value);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void StoreReturnsDefaultsForBrokenJson()
    {
        var path = CreateTempPath();

        try
        {
            File.WriteAllText(path, "{ broken");
            var loaded = new SettingsStore(path).Load();

            Assert.Equal("Игрок", loaded.PlayerName);
            Assert.Equal("medium", loaded.DifficultyKey);
            Assert.True(loaded.HintsEnabled);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static string CreateTempPath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            $"guess-the-number-{Guid.NewGuid():N}.json");
    }

    private sealed class FixedNumberGenerator : INumberGenerator
    {
        private readonly int _value;

        public FixedNumberGenerator(int value)
        {
            _value = value;
        }

        public int Next(int minInclusive, int maxInclusive) => _value;
    }
}
