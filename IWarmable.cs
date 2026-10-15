namespace CoffeeShop;

/// <summary>
/// метод для разогрева напитка
/// </summary>
public interface IWarmable
{
    /// <summary>
    /// переводит напиток в горячее состояние
    /// </summary>
    void Warm();
}