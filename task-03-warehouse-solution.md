# Задание 3. Склад — решение

Один файл `Program.cs`: доработанные классы с XML-комментариями, сценарий в `Main` и блок с ответами на вопросы.

## Что и почему выбрано

| Требование заказчика | Решение |
|---|---|
| Поиск по любому признаку, «первый» и «все» | `FindFirst(Predicate<T>)` и `FindAll(Predicate<T>)`: условие передаётся лямбдой |
| Такой же склад для поставщиков и клиентов | Обобщённый класс `Warehouse<T> where T : class`. Товары, поставщики и клиенты используют один код |
| Порог «мало товара» у категорий разный и задаётся снаружи | `ProductWarehouse` получает словарь «категория → порог» в конструкторе. В коде склада чисел нет |
| Закупки и бухгалтерия узнают о нехватке, склад про них не знает | Событие `LowStock` (`EventHandler<LowStockEventArgs>`). Подписчики подключаются снаружи |

Особенности решения:

- Прежний `Remove(p, qty)` совмещал списание и вывод в консоль. Теперь он называется `WriteOff` и живёт в `ProductWarehouse`: количество есть только у товаров, а у поставщиков и клиентов его нет. Он проверяет, что количество достаточно, и только сообщает о нехватке событием.
- Событие срабатывает при `Count < порог`, как и в исходном коде (`< 5`). Для категорий, не указанных в словаре, действует порог по умолчанию 5.
- Сообщение пишет подписчик, поэтому склад больше не обращается к `Console`.

## Код (`Program.cs`)

```csharp
using System;
using System.Collections.Generic;

#region Модели

/// <summary>
/// Товар на складе: что это, к какой категории относится, сколько стоит и сколько его осталось.
/// </summary>
public class Product
{
    public string Name;
    public string Category;
    public decimal Price;
    public int Count;

    /// <summary>
    /// Создаёт товар со всеми его характеристиками, чтобы склад мог искать и учитывать его.
    /// </summary>
    /// <param name="name">Название товара.</param>
    /// <param name="category">Категория, от которой зависит порог «мало товара».</param>
    /// <param name="price">Цена за единицу.</param>
    /// <param name="count">Текущий остаток.</param>
    public Product(string name, string category, decimal price, int count)
    {
        Name = name;
        Category = category;
        Price = price;
        Count = count;
    }
}

/// <summary>
/// Поставщик: контрагент, у которого закупается товар. Хранится на таком же складе, как и товары.
/// </summary>
public class Supplier
{
    public string Name;
    public string City;
    public int Rating;

    /// <summary>
    /// Создаёт карточку поставщика для учёта и последующего поиска.
    /// </summary>
    /// <param name="name">Название организации.</param>
    /// <param name="city">Город, откуда идёт поставка.</param>
    /// <param name="rating">Оценка надёжности (1-5).</param>
    public Supplier(string name, string city, int rating)
    {
        Name = name;
        City = city;
        Rating = rating;
    }
}

#endregion

#region Данные события

/// <summary>
/// Сведения о нехватке товара, которые склад передаёт всем заинтересованным подписчикам.
/// </summary>
public class LowStockEventArgs : EventArgs
{
    public Product Product { get; }
    public int Threshold { get; }

    /// <summary>
    /// Упаковывает товар и порог, чтобы подписчик понял, что именно и насколько просело.
    /// </summary>
    /// <param name="product">Товар, которого стало мало.</param>
    /// <param name="threshold">Порог его категории, ниже которого остаток считается малым.</param>
    public LowStockEventArgs(Product product, int threshold)
    {
        Product = product;
        Threshold = threshold;
    }
}

#endregion

#region Склады

/// <summary>
/// Универсальный склад: хранит любые объекты (товары, поставщиков, клиентов) и
/// умеет находить их по произвольному условию. Ничего не знает о том, что внутри.
/// </summary>
/// <typeparam name="T">Тип хранимых объектов (ссылочный, чтобы «не найдено» можно было выразить через null).</typeparam>
public class Warehouse<T> where T : class
{
    protected readonly List<T> items = new List<T>();

    /// <summary>
    /// Всё, что сейчас лежит на складе; только для чтения, чтобы менять состав можно было лишь через методы склада.
    /// </summary>
    public IReadOnlyList<T> Items => items;

    /// <summary>
    /// Принимает объект на склад, чтобы его можно было искать и учитывать.
    /// </summary>
    /// <param name="item">Объект, который ставится на учёт.</param>
    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        items.Add(item);
    }

    /// <summary>
    /// Снимает объект с учёта полностью (например, поставщик больше не работает с нами).
    /// </summary>
    /// <param name="item">Объект, который убирается со склада.</param>
    /// <returns>true, если объект был на складе и удалён; иначе false.</returns>
    public bool Remove(T item)
    {
        return items.Remove(item);
    }

    /// <summary>
    /// Находит первый объект, подходящий под условие, чтобы искать по любому признаку, а не по одному заранее выбранному.
    /// </summary>
    /// <param name="match">Условие поиска; его задаёт вызывающий код.</param>
    /// <returns>Первый подходящий объект или null, если таких нет.</returns>
    public T FindFirst(Predicate<T> match)
    {
        if (match == null) throw new ArgumentNullException(nameof(match));
        return items.Find(match);
    }

    /// <summary>
    /// Находит все объекты, подходящие под условие, когда нужен полный список, а не один результат.
    /// </summary>
    /// <param name="match">Условие поиска; его задаёт вызывающий код.</param>
    /// <returns>Список подходящих объектов; пустой, если ничего не найдено.</returns>
    public List<T> FindAll(Predicate<T> match)
    {
        if (match == null) throw new ArgumentNullException(nameof(match));
        return items.FindAll(match);
    }

    #region Ответы на вопросы
    // Ответьте своими словами под каждым вопросом.
    //
    // Вопрос 1. Чем Predicate<T> отличается от Func<T, bool>?
    // Ответ: По смыслу почти ничем: оба описывают «функцию, которая принимает T и
    //        отвечает да/нет». Но это два разных типа делегатов, и один в другой
    //        без преобразования не подставить, хотя одну и ту же лямбду можно
    //        присвоить любому из них. Predicate<T> несёт смысл «условие/проверка»
    //        и используется в классических методах коллекций (List<T>.Find,
    //        FindAll, Exists, RemoveAll), поэтому для наших FindFirst/FindAll он
    //        подходит естественно. Func<T, bool> - частный случай общего Func<...,
    //        TResult> (у него может быть до 16 параметров), его ждут LINQ-методы
    //        вроде Where и Any.
    //
    // Вопрос 2. Что такое замыкание? Что будет, если захваченная переменная
    //           изменится после создания лямбды?
    // Ответ: Замыкание - это когда лямбда «захватывает» переменные из окружающего
    //        кода и может пользоваться ими, даже когда тот код уже отработал.
    //        Захватывается сама переменная, а не её значение на момент создания.
    //        Поэтому, если переменную изменить после создания лямбды, при
    //        следующем вызове лямбда увидит новое значение. В Main это видно на
    //        maxPrice: тот же самый Predicate после смены цены находит другие
    //        товары. Побочный эффект - неожиданности, если переменную меняют
    //        случайно; чтобы зафиксировать значение, его копируют в локальную
    //        переменную перед созданием лямбды.
    //
    // Вопрос 3. Зачем может понадобиться where T : class и что ещё бывает в where?
    // Ответ: where T : class разрешает в качестве T только ссылочные типы. Это
    //        нужно, когда внутри обобщённого кода мы сравниваем с null или
    //        возвращаем null как «ничего не найдено», как в FindFirst. Для
    //        значимого типа (int, struct) null невозможен. Ещё в where бывают:
    //        struct (только значимые типы), new() (есть публичный конструктор без
    //        параметров, можно писать new T()), имя базового класса или
    //        интерфейса (T обязан от него наследоваться, и можно вызывать его
    //        члены), notnull (запрет null), unmanaged, а также комбинации через
    //        запятую, например where T : class, IComparable<T>, new().
    //
    #endregion
}

/// <summary>
/// Склад товаров: к обычному поиску добавляет списание и контроль остатков по категориям.
/// О том, кто и как реагирует на нехватку, не знает: только сообщает событием.
/// </summary>
public class ProductWarehouse : Warehouse<Product>
{
    private readonly IReadOnlyDictionary<string, int> thresholds;
    private readonly int defaultThreshold;

    /// <summary>
    /// Срабатывает, когда после списания остаток товара опустился ниже порога его категории.
    /// </summary>
    public event EventHandler<LowStockEventArgs> LowStock;

    /// <summary>
    /// Создаёт склад с порогами, заданными снаружи, чтобы правила «мало товара» менялись без правки склада.
    /// </summary>
    /// <param name="thresholds">Порог «мало товара» для каждой категории.</param>
    /// <param name="defaultThreshold">Порог для категорий, которых нет в словаре.</param>
    public ProductWarehouse(IReadOnlyDictionary<string, int> thresholds, int defaultThreshold = 5)
    {
        this.thresholds = thresholds ?? throw new ArgumentNullException(nameof(thresholds));
        this.defaultThreshold = defaultThreshold;
    }

    /// <summary>
    /// Списывает товар со склада (продажа, брак) и уведомляет подписчиков, если остаток стал малым.
    /// </summary>
    /// <param name="product">Товар, который списывается.</param>
    /// <param name="qty">Сколько единиц списать; должно быть положительным и не больше остатка.</param>
    public void WriteOff(Product product, int qty)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty), "Количество должно быть положительным.");
        if (qty > product.Count) throw new InvalidOperationException("Нельзя списать больше, чем есть на складе.");

        product.Count -= qty;

        int threshold = GetThreshold(product.Category);
        if (product.Count < threshold)
            LowStock?.Invoke(this, new LowStockEventArgs(product, threshold));
    }

    /// <summary>
    /// Определяет порог «мало товара» для категории, чтобы у разных категорий были свои правила.
    /// </summary>
    /// <param name="category">Категория товара.</param>
    /// <returns>Порог из словаря либо значение по умолчанию.</returns>
    private int GetThreshold(string category)
    {
        return thresholds.TryGetValue(category, out int value) ? value : defaultThreshold;
    }
}

#endregion

#region Подписчики

/// <summary>
/// Отдел закупок: реагирует на нехватку товара заказом у поставщика.
/// </summary>
public class PurchasingDepartment
{
    /// <summary>
    /// Принимает сообщение о нехватке и решает, что нужно докупить.
    /// </summary>
    /// <param name="sender">Склад, приславший сообщение.</param>
    /// <param name="e">Товар и порог, который он нарушил.</param>
    public void OnLowStock(object sender, LowStockEventArgs e)
    {
        Console.WriteLine($"[Закупки] Заказать «{e.Product.Name}» ({e.Product.Category}): осталось {e.Product.Count}, порог {e.Threshold}");
    }
}

/// <summary>
/// Бухгалтерия: фиксирует нехватку для учёта и планирования расходов.
/// </summary>
public class AccountingDepartment
{
    /// <summary>
    /// Принимает сообщение о нехватке и отмечает, на какую сумму нужно пополнение.
    /// </summary>
    /// <param name="sender">Склад, приславший сообщение.</param>
    /// <param name="e">Товар и порог, который он нарушил.</param>
    public void OnLowStock(object sender, LowStockEventArgs e)
    {
        Console.WriteLine($"[Бухгалтерия] Нехватка «{e.Product.Name}»: осталось {e.Product.Count} шт. по {e.Product.Price} руб.");
    }
}

#endregion

public class Program
{
    public static void Main()
    {
        // Пороги задаём снаружи, склад их не знает.
        var thresholds = new Dictionary<string, int>
        {
            ["Молоко"] = 20,
            ["Мебель"] = 2
        };

        // Склад товаров и склад поставщиков - один и тот же класс Warehouse<T>.
        var goods = new ProductWarehouse(thresholds);
        var suppliers = new Warehouse<Supplier>();

        // Подписываем закупки и бухгалтерию; склад о них ничего не знает.
        var purchasing = new PurchasingDepartment();
        var accounting = new AccountingDepartment();
        goods.LowStock += purchasing.OnLowStock;
        goods.LowStock += accounting.OnLowStock;

        // Тестовые данные.
        var milk = new Product("Молоко 3,2%", "Молоко", 80m, 25);
        var kefir = new Product("Кефир 1%", "Молоко", 70m, 40);
        var table = new Product("Стол офисный", "Мебель", 12000m, 3);
        var chair = new Product("Стул", "Мебель", 3500m, 10);
        goods.Add(milk);
        goods.Add(kefir);
        goods.Add(table);
        goods.Add(chair);

        suppliers.Add(new Supplier("Молочный двор", "Алматы", 5));
        suppliers.Add(new Supplier("МебельОпт", "Астана", 3));
        suppliers.Add(new Supplier("Быстрые поставки", "Алматы", 4));

        // 1. Поиск товара по названию.
        Console.WriteLine("--- Поиск по названию ---");
        Product found = goods.FindFirst(p => p.Name == "Стол офисный");
        Console.WriteLine(found == null
            ? "Не найдено"
            : $"{found.Name}, {found.Price} руб., {found.Count} шт.");

        // 2. Все товары дешевле цены из переменной.
        Console.WriteLine("--- Дешевле maxPrice (замыкание) ---");
        decimal maxPrice = 100m;
        Predicate<Product> cheaper = p => p.Price < maxPrice;

        List<Product> result = goods.FindAll(cheaper);
        Console.WriteLine($"maxPrice = {maxPrice}: " + string.Join(", ", result.ConvertAll(p => p.Name)));

        // 3. Меняем переменную: та же лямбда видит новое значение.
        maxPrice = 5000m;
        result = goods.FindAll(cheaper);
        Console.WriteLine($"maxPrice = {maxPrice}: " + string.Join(", ", result.ConvertAll(p => p.Name)));
        Console.WriteLine("Лямбда захватила саму переменную maxPrice, а не её значение на момент создания, поэтому после изменения она работает с новым порогом.");

        // 4. Поиск на складе поставщиков тем же классом.
        Console.WriteLine("--- Поставщики ---");
        var almatySuppliers = suppliers.FindAll(s => s.City == "Алматы");
        Console.WriteLine("Из Алматы: " + string.Join(", ", almatySuppliers.ConvertAll(s => s.Name)));
        Supplier reliable = suppliers.FindFirst(s => s.Rating == 5);
        Console.WriteLine("Первый с рейтингом 5: " + reliable?.Name);

        // 5. Нехватка: «Молоко» (порог 20) и «Мебель» (порог 2).
        Console.WriteLine("--- Списание и нехватка ---");
        goods.WriteOff(milk, 10);   // 25 -> 15, меньше 20: событие
        goods.WriteOff(table, 2);   // 3 -> 1, меньше 2: событие
        goods.WriteOff(chair, 3);   // 10 -> 7, порог Мебели 2: сообщений нет
        Console.WriteLine("Списание стульев (7 осталось, порог 2): сообщений нет.");
    }
}
```

## Ожидаемый вывод

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

## Как объяснить поведение при изменении переменной

Лямбда `p => p.Price < maxPrice` не копирует значение `maxPrice`, а хранит ссылку на саму переменную (замыкание). Поэтому тот же `Predicate`, вызванный после `maxPrice = 5000m`, использует уже 5000 и находит дополнительно «Стул» за 3500. Если нужно зафиксировать значение, перед созданием лямбды копируют его в отдельную локальную переменную.

## Что сдавать

Файл `Program.cs` с кодом из раздела выше: классы с XML-комментариями, `Main` со сценарием и блок `#region Ответы на вопросы` внутри `Warehouse<T>`.
