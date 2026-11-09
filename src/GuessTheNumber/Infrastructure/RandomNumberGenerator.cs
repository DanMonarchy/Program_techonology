using GuessTheNumber.Domain;

namespace GuessTheNumber.Infrastructure;

/// <summary>
/// Генератор случайных чисел на основе <see cref="Random"/>.
/// </summary>
public sealed class RandomNumberGenerator : INumberGenerator
{
    private readonly Random _random = new();

    public int Next(int minInclusive, int maxInclusive)
    {
        return _random.Next(minInclusive, maxInclusive + 1);
    }
}
