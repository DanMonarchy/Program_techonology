namespace CoffeeShop;

/// <summary>
/// Чайные напитки, которые могут быть зелёными или чёрными и могут быть разогреты(люблю чай)
/// </summary>
public class Tea : Beverage
{
    /// <summary>Тип чая.</summary>
    public string TeaType { get; set; }

    /// <summary>Если чай зеленый.</summary>
    public bool IsGreen => TeaType == "Green";

    /// <summary>
    /// Создаёт чай со всеми параметрами
    /// </summary>
    public Tea(int id, string name, int volume, string teaType)
        : base(id, name, volume)
    {
        TeaType = teaType;
    }

    /// <summary>
    /// Создаёт чай на основе названия и типа
    /// </summary>
    public Tea(string name, string teaType) : base(name)
    {
        TeaType = teaType;
    }

    /// <summary>
    /// Подаёт чай
    /// </summary>
    public override void Serve()
    {
        Console.WriteLine($"{Name} served.");
    }

    /// <summary>
    /// Возвращает текстовое описание чая
    /// </summary>
    public override string ToString()
    {
        return $"[Tea] {Name}, {Volume} ml, temperature: {Temperature}, type: {TeaType}";
    }
}