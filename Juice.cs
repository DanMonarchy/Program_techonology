namespace CoffeeShop;

/// <summary>
/// Сок
/// </summary>
public class Juice : Beverage
{
    /// <summary>
    /// свежесть сока
    /// </summary>
    public bool IsFresh { get; set; }

    /// <summary>
    /// Свежий сок с объемом 500 мл или больше считается дорогим, ну если он натуральный конечно)))
    /// </summary>
    public bool IsExpensive => IsFresh && Volume >= 500;

    /// <summary>
    /// Создаёт сок со всеми параметрами
    /// </summary>
    public Juice(int id, string name, int volume, bool isFresh)
        : base(id, name, volume)
    {
        IsFresh = isFresh;
    }

    /// <summary>
    /// Создаёт сок на основе названия и свежести
    /// </summary>
    public Juice(string name, bool isFresh) : base(name)
    {
        IsFresh = isFresh;
    }

    /// <summary>
    /// Подаёт сок
    /// </summary>
    public override void Serve()
    {
        Console.WriteLine($"{Name} served.");
    }

    /// <summary>
    /// Возвращает текстовое описание сока
    /// </summary>
    public override string ToString()
    {
        return $"[Juice] {Name}, {Volume} ml, temperature: {Temperature}, fresh: {IsFresh}";
    }
}