namespace GuessTheNumber.Domain;

/// <summary>
/// Статистика сыгранных партий. Сохраняется между запусками игры.
/// </summary>
public sealed class GameStatistics
{
    public int GamesPlayed { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int TotalAttempts { get; set; }

    public int? BestAttempts { get; set; }

    /// <summary>
    /// Учитывает завершённую партию. Вызывается из события GameFinished.
    /// </summary>
    public void RecordGame(GameFinishedEventArgs args)
    {
        GamesPlayed++;
        TotalAttempts += args.AttemptsUsed;

        if (args.IsWon)
        {
            Wins++;

            if (BestAttempts is null || args.AttemptsUsed < BestAttempts)
            {
                BestAttempts = args.AttemptsUsed;
            }
        }
        else
        {
            Losses++;
        }
    }

    public double GetWinRate()
    {
        return GamesPlayed == 0 ? 0 : (double)Wins / GamesPlayed * 100;
    }

    public double GetAverageAttempts()
    {
        return GamesPlayed == 0 ? 0 : (double)TotalAttempts / GamesPlayed;
    }
}
