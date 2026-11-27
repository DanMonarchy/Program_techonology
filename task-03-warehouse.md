# Задание 3. Склад

Время: 40-50 минут.

## Что нужно сделать

1. Разобраться. Над классом и над каждым его методом напишите XML-комментарий (`/// <summary>`, при необходимости `<param>` и `<returns>`). Опишите, что делает код и зачем, а не как (не «проходит циклом по списку»).
2. Доработать код под требования заказчика (ниже). Названия приёмов и типов в требованиях не указаны, выбирайте сами.
3. Показать в `Main`, что всё работает (сценарий ниже). Тестовые данные создайте сами.
4. Ответить письменно на 3 вопроса в блоке `#region Ответы на вопросы` в конце класса `Warehouse`: под каждым вопросом, после слова «Ответ:». Блок уже есть в заготовке. Если вы переименовали или заменили класс, перенесите блок в итоговый класс.

## Требования заказчика

* Поиск должен работать по любому признаку товара, нужны и «первый подходящий», и «все подходящие».
* Такой же склад понадобится для поставщиков и клиентов.
* Порог «мало товара» у разных категорий разный и задаётся снаружи.
* Закупки и бухгалтерия узнают о нехватке товара, склад про них ничего не знает.

## Что показать в `Main`

* склад товаров и склад поставщиков;
* поиск товара по названию, а также всех товаров дешевле цены, которая лежит у вас в переменной;
* что после изменения этой переменной поиск ведёт себя ожидаемым образом (объясните, каким);
* сообщение о нехватке для категории «Молоко» (порог 20) и «Мебель» (порог 2).

## Заготовка кода

```csharp
using System;
using System.Collections.Generic;

public class Product { public string Name; public string Category; public decimal Price; public int Count; }

public class Warehouse
{
    public List<Product> Products = new List<Product>();

    public Product FindByName(string name)
    { foreach (var p in Products) if (p.Name == name) return p; return null; }

    public Product FindCheaperThan(decimal price)
    { foreach (var p in Products) if (p.Price < price) return p; return null; }

    public void Remove(Product p, int qty)
    {
        p.Count -= qty;
        if (p.Count < 5) Console.WriteLine("Мало товара: " + p.Name);
    }

    #region Ответы на вопросы
    // Ответьте своими словами под каждым вопросом.
    //
    // Вопрос 1. Чем Predicate<T> отличается от Func<T, bool>?
    // Ответ:
    //
    // Вопрос 2. Что такое замыкание? Что будет, если захваченная переменная
    //           изменится после создания лямбды?
    // Ответ:
    //
    // Вопрос 3. Зачем может понадобиться where T : class и что ещё бывает в where?
    // Ответ:
    //
    #endregion
}

public class Program
{
    public static void Main()
    {
        // Ваш сценарий
    }
}
```

## Что сдавать

Один файл `Program.cs` (или проект целиком): доработанные классы с комментариями, `Main` и заполненный блок с ответами.

---

# Решение

| Требование | Приём |
|---|---|
| Поиск по любому признаку, «первый» и «все» | `FindFirst` и `FindAll` с `Predicate<T>` |
| Склад для поставщиков и клиентов | обобщённый `Warehouse<T> where T : class` |
| Пороги по категориям снаружи | словарь «категория → порог» в конструкторе `ProductWarehouse` |
| Закупки и бухгалтерия, склад о них не знает | событие `LowStock` |

Старый `Remove(p, qty)` стал `WriteOff` в `ProductWarehouse`: количество есть только у товаров. Событие срабатывает при `Count < порог`, для категорий вне словаря порог по умолчанию 5.

```csharp
using System;
using System.Collections.Generic;

/// <summary>Товар.</summary>
public class Product
{
    public string Name; public string Category; public decimal Price; public int Count;

    /// <summary>Создаёт товар.</summary>
    public Product(string name, string category, decimal price, int count)
    { Name = name; Category = category; Price = price; Count = count; }
}

/// <summary>Поставщик.</summary>
public class Supplier
{
    public string Name; public string City; public int Rating;

    /// <summary>Создаёт поставщика.</summary>
    public Supplier(string name, string city, int rating)
    { Name = name; City = city; Rating = rating; }
}

/// <summary>Данные о нехватке товара для подписчиков.</summary>
public class LowStockEventArgs : EventArgs
{
    public Product Product { get; }
    public int Threshold { get; }

    /// <summary>Запоминает товар и нарушенный порог.</summary>
    public LowStockEventArgs(Product product, int threshold)
    { Product = product; Threshold = threshold; }
}

/// <summary>Универсальный склад с поиском по любому условию.</summary>
public class Warehouse<T> where T : class
{
    protected readonly List<T> items = new List<T>();

    /// <summary>Содержимое склада (только чтение).</summary>
    public IReadOnlyList<T> Items => items;

    /// <summary>Ставит объект на учёт.</summary>
    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        items.Add(item);
    }

    /// <summary>Снимает объект с учёта.</summary>
    public bool Remove(T item) { return items.Remove(item); }

    /// <summary>Первый объект, подходящий под условие, или null.</summary>
    public T FindFirst(Predicate<T> match) { return items.Find(match); }

    /// <summary>Все объекты, подходящие под условие.</summary>
    public List<T> FindAll(Predicate<T> match) { return items.FindAll(match); }

    #region Ответы на вопросы
    // Ответьте своими словами под каждым вопросом.
    //
    // Вопрос 1. Чем Predicate<T> отличается от Func<T, bool>?
    // Ответ: Это два разных типа делегатов с одинаковой сигнатурой (T -> bool),
    //        друг в друга без преобразования не подставляются. Predicate<T>
    //        используют методы коллекций (Find, FindAll, RemoveAll), Func<T, bool>
    //        - LINQ (Where, Any).
    //
    // Вопрос 2. Что такое замыкание? Что будет, если захваченная переменная
    //           изменится после создания лямбды?
    // Ответ: Замыкание - лямбда, которая захватывает переменные из окружающего
    //        кода. Захватывается сама переменная, а не её значение, поэтому
    //        при следующем вызове лямбда увидит новое значение.
    //
    // Вопрос 3. Зачем может понадобиться where T : class и что ещё бывает в where?
    // Ответ: class разрешает только ссылочные типы: можно сравнивать с null и
    //        возвращать null («не найдено»). Ещё бывают struct, new(), базовый
    //        класс или интерфейс, notnull, unmanaged и их комбинации.
    //
    #endregion
}

/// <summary>Склад товаров с контролем остатков по категориям.</summary>
public class ProductWarehouse : Warehouse<Product>
{
    private readonly IReadOnlyDictionary<string, int> thresholds;
    private readonly int defaultThreshold;

    /// <summary>Срабатывает, когда остаток стал ниже порога категории.</summary>
    public event EventHandler<LowStockEventArgs> LowStock;

    /// <summary>Создаёт склад с порогами, заданными снаружи.</summary>
    public ProductWarehouse(IReadOnlyDictionary<string, int> thresholds, int defaultThreshold = 5)
    {
        this.thresholds = thresholds ?? throw new ArgumentNullException(nameof(thresholds));
        this.defaultThreshold = defaultThreshold;
    }

    /// <summary>Списывает товар и сообщает, если его стало мало.</summary>
    public void WriteOff(Product product, int qty)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        if (qty > product.Count) throw new InvalidOperationException("Нельзя списать больше остатка.");

        product.Count -= qty;
        int threshold = GetThreshold(product.Category);
        if (product.Count < threshold)
            LowStock?.Invoke(this, new LowStockEventArgs(product, threshold));
    }

    /// <summary>Порог «мало товара» для категории.</summary>
    private int GetThreshold(string category)
    {
        return thresholds.TryGetValue(category, out int value) ? value : defaultThreshold;
    }
}

/// <summary>Закупки: заказывают недостающий товар.</summary>
public class PurchasingDepartment
{
    /// <summary>Реагирует на нехватку.</summary>
    public void OnLowStock(object sender, LowStockEventArgs e)
    {
        Console.WriteLine($"[Закупки] Заказать «{e.Product.Name}» ({e.Product.Category}): осталось {e.Product.Count}, порог {e.Threshold}");
    }
}

/// <summary>Бухгалтерия: фиксирует нехватку.</summary>
public class AccountingDepartment
{
    /// <summary>Реагирует на нехватку.</summary>
    public void OnLowStock(object sender, LowStockEventArgs e)
    {
        Console.WriteLine($"[Бухгалтерия] Нехватка «{e.Product.Name}»: осталось {e.Product.Count} шт. по {e.Product.Price} руб.");
    }
}

public class Program
{
    public static void Main()
    {
        var thresholds = new Dictionary<string, int> { ["Молоко"] = 20, ["Мебель"] = 2 };

        var goods = new ProductWarehouse(thresholds);
        var suppliers = new Warehouse<Supplier>();

        goods.LowStock += new PurchasingDepartment().OnLowStock;
        goods.LowStock += new AccountingDepartment().OnLowStock;

        var milk = new Product("Молоко 3,2%", "Молоко", 80m, 25);
        var kefir = new Product("Кефир 1%", "Молоко", 70m, 40);
        var table = new Product("Стол офисный", "Мебель", 12000m, 3);
        var chair = new Product("Стул", "Мебель", 3500m, 10);
        goods.Add(milk); goods.Add(kefir); goods.Add(table); goods.Add(chair);

        suppliers.Add(new Supplier("Молочный двор", "Алматы", 5));
        suppliers.Add(new Supplier("МебельОпт", "Астана", 3));
        suppliers.Add(new Supplier("Быстрые поставки", "Алматы", 4));

        // Поиск по названию.
        Console.WriteLine("--- Поиск по названию ---");
        Product found = goods.FindFirst(p => p.Name == "Стол офисный");
        Console.WriteLine(found == null ? "Не найдено" : $"{found.Name}, {found.Price} руб., {found.Count} шт.");

        // Дешевле цены из переменной; лямбда захватывает саму переменную.
        Console.WriteLine("--- Дешевле maxPrice (замыкание) ---");
        decimal maxPrice = 100m;
        Predicate<Product> cheaper = p => p.Price < maxPrice;
        var result = goods.FindAll(cheaper);
        Console.WriteLine($"maxPrice = {maxPrice}: " + string.Join(", ", result.ConvertAll(p => p.Name)));

        maxPrice = 5000m;
        result = goods.FindAll(cheaper);
        Console.WriteLine($"maxPrice = {maxPrice}: " + string.Join(", ", result.ConvertAll(p => p.Name)));
        Console.WriteLine("Лямбда захватила саму переменную maxPrice, а не её значение на момент создания, поэтому после изменения она работает с новым порогом.");

        // Тот же склад для поставщиков.
        Console.WriteLine("--- Поставщики ---");
        var almaty = suppliers.FindAll(s => s.City == "Алматы");
        Console.WriteLine("Из Алматы: " + string.Join(", ", almaty.ConvertAll(s => s.Name)));
        Console.WriteLine("Первый с рейтингом 5: " + suppliers.FindFirst(s => s.Rating == 5)?.Name);

        // Нехватка: «Молоко» (порог 20) и «Мебель» (порог 2).
        Console.WriteLine("--- Списание и нехватка ---");
        goods.WriteOff(milk, 10);   // 25 -> 15 < 20: событие
        goods.WriteOff(table, 2);   // 3 -> 1 < 2: событие
        goods.WriteOff(chair, 3);   // 10 -> 7, порог 2: тишина
        Console.WriteLine("Списание стульев (7 осталось, порог 2): сообщений нет.");
    }
}
```

### Ожидаемый вывод

```text
--- Поиск по названию ---
Стол офисный, 12000 руб., 3 шт.
--- Дешевле maxPrice (замыкание) ---
maxPrice = 100: Молоко 3,2%, Кефир 1%
maxPrice = 5000: Молоко 3,2%, Кефир 1%, Стул
Лямбда захватила саму переменную maxPrice, а не её значение на момент создания, поэтому после изменения она работает с новым порогом.
--- Поставщики ---
Из Алматы: Молочный двор, Быстрые поставки
Первый с рейтингом 5: Молочный двор
--- Списание и нехватка ---
[Закупки] Заказать «Молоко 3,2%» (Молоко): осталось 15, порог 20
[Бухгалтерия] Нехватка «Молоко 3,2%»: осталось 15 шт. по 80 руб.
[Закупки] Заказать «Стол офисный» (Мебель): осталось 1, порог 2
[Бухгалтерия] Нехватка «Стол офисный»: осталось 1 шт. по 12000 руб.
Списание стульев (7 осталось, порог 2): сообщений нет.
```
