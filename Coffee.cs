namespace CoffeeShop;

/// <summary>
/// Кофейные напитки, которые могут быть с молоком или без и могут быть разогреты
/// </summary>
public class Coffee : Beverage, IWarmable
{
    /// <summary>
    /// Было ли добавлено молоко(мммм лактоза...)
    /// </summary>
    public bool HasMilk { get; set; }

    /// <summary>
    /// Истина, если кофе с молоком, иначе ложь
    /// </summary>
    public bool IsLatte => HasMilk;

    /// <summary>
    /// Создаёт кофе со всеми параметрами
    /// </summary>
    public Coffee(int id, string name, int volume, bool hasMilk)
        : base(id, name, volume)
    {
        HasMilk = hasMilk;
    }

    /// <summary>
    /// Создаёт кофе на основе названия и флага молока выше.
    /// </summary>
    public Coffee(string name, bool hasMilk) : base(name)
    {
        HasMilk = hasMilk;
    }

    /// <summary>
    /// Подаёт кофе
    /// </summary>
    public override void Serve()
    {
        Console.WriteLine($"{Name} served.");
    }

    /// <summary>
    /// Разогревает кофе
    /// </summary>
    public void Warm()
    {
        Temperature = DrinkTemperature.Hot;
        Console.WriteLine($"{Name} warmed.");
    }

    /// <summary>
    /// Возвращает текстовое описание кофе
    /// </summary>
    public override string ToString()
    {
        return $"[Coffee] {Name}, {Volume} ml, temperature: {Temperature}";
    }
}