using System.Globalization;
using CoffeeShop;

/// <summary>
/// CultureInfo - нужно, чтобы при выводе чисел с плавающей точкой использовалась точка, а не запятая.
/// </summary>
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

/// <summary>
/// Меню кофейни, содержащее различные напитки
/// </summary>
List<Beverage> menu = new List<Beverage>();
menu.Add(new Coffee(1, "Latte", 300, true));
menu.Add(new Juice(2, "Orange", 500, true));
/// <summary>
/// Пример полиморфизма: мы можем хранить объекты разных типов (Coffee и Juice) в одном списке Beverage.
/// </summary>
foreach (Beverage drink in menu)
{
    Console.WriteLine(drink);                          
    Console.WriteLine($"  liters: {drink.VolumeInLiters}");
    drink.Serve();                                     

    if (drink is IWarmable warmable)
    {
        warmable.Warm();
    }
    else
    {
        Console.WriteLine($"{drink.Name} cannot be warmed.");
    }

    Console.WriteLine("----------------------------------------");
}