namespace CoffeeShop;

/// <summary>
/// Абстрактный базовый класс для всех напитков в кофейне. Жаль мне кофе нельзя :'(
/// </summary>
public abstract class Beverage
{
    private readonly int _id;

    /// <summary>
    /// имя напитка
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Объем в миллилитрах.
    /// </summary>
    public int Volume { get; set; }

    /// <summary>
    /// Текущая температура напитка.
    /// </summary>
    public DrinkTemperature Temperature { get; set; } = DrinkTemperature.Hot;

    /// <summary>
    /// Объем в литрах, вычисляемый на основе объема в миллилитрах.
    /// </summary>
    public double VolumeInLiters => Volume / 1000.0;

    /// <summary>
    /// Индентификатор напитка, уникальный для каждого экземпляра.
    /// </summary>
    public int Id => _id;

    /// <summary>
    /// Создаёт напиток с заданным идентификатором, именем и объёмом.
    /// </summary>
    public Beverage(int id, string name, int volume)
    {
        _id = id;
        Name = name;
        Volume = volume;
    }

    /// <summary>
    /// Создаёт напиток с заданным именем и объёмом по умолчанию 0 мл.
    /// </summary>
    public Beverage(string name) : this(0, name, 0)
    {
    }

    /// <summary>
    /// Подаёт напиток клиенту
    /// </summary>
    public abstract void Serve();
}