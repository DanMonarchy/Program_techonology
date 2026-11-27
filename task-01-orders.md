# Задание 1. Заказы

Время: 40-50 минут.

## Что нужно сделать

1. Разобраться. Над классом и над каждым его методом напишите XML-комментарий (`/// <summary>`, при необходимости `<param>` и `<returns>`). Опишите, что делает код и зачем, а не как (не «проходит циклом по списку»).
2. Доработать код под требования заказчика (ниже). Названия приёмов и типов в требованиях не указаны, выбирайте сами.
3. Показать в `Main`, что всё работает (сценарий ниже). Тестовые данные создайте сами.
4. Ответить письменно на 3 вопроса в блоке `#region Ответы на вопросы` в конце класса `OrderService`: под каждым вопросом, после слова «Ответ:». Блок уже есть в заготовке. Если вы переименовали или заменили класс, перенесите блок в итоговый класс.

## Требования заказчика

* Завтра попросят отбирать заказы по городу и статусу, клиентов по почте и т. д. Копировать метод под каждое условие нельзя.
* Способ уведомления выбирает вызывающий код. Позже добавятся push и Telegram, `OrderService` при этом менять нельзя.
* Статистика и аудит должны узнавать об обработанном заказе, но `OrderService` про них не должен знать.

## Что показать в `Main`

* заказы дороже 5000 из Берлина;
* клиенты с почтой на `@gmail.com`;
* обработку заказов с уведомлением в консоль и отдельно с уведомлением другим способом, который вы придумаете;
* подсчёт обработанных заказов подключённым отдельным модулем и ещё один независимый модуль, который записывает номера обработанных заказов в список.

## Заготовка кода

```csharp
using System;
using System.Collections.Generic;

public class Order { public int Id; public decimal Price; public string City; public string Status; }
public class Customer { public string Name; public int Age; public string Email; }

public class OrderService
{
    public List<Order> GetExpensive(List<Order> orders)
    {
        var r = new List<Order>();
        foreach (var o in orders) if (o.Price > 1000) r.Add(o);
        return r;
    }

    public List<Customer> GetAdults(List<Customer> customers)
    {
        var r = new List<Customer>();
        foreach (var c in customers) if (c.Age >= 18) r.Add(c);
        return r;
    }

    public void Process(List<Order> orders, string mode)
    {
        foreach (var o in orders)
        {
            if (mode == "email") Console.WriteLine("Email: " + o.Id);
            else if (mode == "sms") Console.WriteLine("SMS: " + o.Id);
            o.Status = "Processed";
        }
    }

    #region Ответы на вопросы
    // Ответьте своими словами под каждым вопросом.
    //
    // Вопрос 1. Чем Func, Action и Predicate<T> отличаются друг от друга?
    // Ответ:
    //
    // Вопрос 2. Чем event отличается от публичного поля-делегата?
    // Ответ:
    //
    // Вопрос 3. Зачем при вызове события пишут ?.Invoke(...), а не вызывают его напрямую?
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
| Отбор по любому условию | обобщённый `Filter<T>(List<T>, Predicate<T>)` |
| Способ уведомления выбирает вызывающий | параметр `Action<Order> notify` |
| Статистика и аудит без знания о них | событие `OrderProcessed` |

```csharp
using System;
using System.Collections.Generic;

/// <summary>Заказ.</summary>
public class Order { public int Id; public decimal Price; public string City; public string Status; }

/// <summary>Клиент.</summary>
public class Customer { public string Name; public int Age; public string Email; }

/// <summary>Отбор и обработка заказов; о подписчиках и способах уведомления не знает.</summary>
public class OrderService
{
    /// <summary>Срабатывает после обработки каждого заказа.</summary>
    public event Action<Order> OrderProcessed;

    /// <summary>Отбирает элементы по любому условию.</summary>
    public List<T> Filter<T>(List<T> items, Predicate<T> match)
    {
        return items.FindAll(match);
    }

    /// <summary>Обрабатывает заказы, уведомляя выбранным вызывающим способом.</summary>
    public void Process(List<Order> orders, Action<Order> notify)
    {
        foreach (var o in orders)
        {
            o.Status = "Processed";
            notify?.Invoke(o);
            OrderProcessed?.Invoke(o);
        }
    }

    #region Ответы на вопросы
    // Ответьте своими словами под каждым вопросом.
    //
    // Вопрос 1. Чем Func, Action и Predicate<T> отличаются друг от друга?
    // Ответ: Func возвращает значение (последний параметр типа - тип результата),
    //        Action ничего не возвращает (void), Predicate<T> принимает один
    //        параметр и возвращает bool.
    //
    // Вопрос 2. Чем event отличается от публичного поля-делегата?
    // Ответ: У event снаружи доступны только += и -=; вызвать его или присвоить
    //        через = нельзя, вызывает только сам класс. Публичное поле может
    //        вызвать кто угодно или затереть всех подписчиков.
    //
    // Вопрос 3. Зачем при вызове события пишут ?.Invoke(...), а не вызывают его напрямую?
    // Ответ: Без подписчиков событие равно null, и прямой вызов даст
    //        NullReferenceException. ?.Invoke вызывает только при наличии
    //        подписчиков и работает с локальной копией (безопасно при потоках).
    //
    #endregion
}

/// <summary>Статистика: считает обработанные заказы.</summary>
public class Statistics
{
    public int Count;
    /// <summary>Учитывает обработанный заказ.</summary>
    public void OnProcessed(Order o) { Count++; }
}

/// <summary>Аудит: записывает номера обработанных заказов.</summary>
public class Audit
{
    public List<int> Ids = new List<int>();
    /// <summary>Записывает номер заказа.</summary>
    public void OnProcessed(Order o) { Ids.Add(o.Id); }
}

public class Program
{
    public static void Main()
    {
        var orders = new List<Order>
        {
            new Order { Id = 1, Price = 7200m, City = "Берлин", Status = "New" },
            new Order { Id = 2, Price = 4500m, City = "Берлин", Status = "New" },
            new Order { Id = 3, Price = 6000m, City = "Париж",  Status = "New" },
            new Order { Id = 4, Price = 5200m, City = "Берлин", Status = "New" }
        };
        var customers = new List<Customer>
        {
            new Customer { Name = "Анна",  Age = 28, Email = "anna@gmail.com" },
            new Customer { Name = "Борис", Age = 35, Email = "boris@mail.ru" },
            new Customer { Name = "Вера",  Age = 22, Email = "vera@gmail.com" }
        };

        var service = new OrderService();
        var stats = new Statistics();
        var audit = new Audit();
        service.OrderProcessed += stats.OnProcessed;
        service.OrderProcessed += audit.OnProcessed;

        // 1. Заказы дороже 5000 из Берлина.
        decimal minPrice = 5000m;
        string city = "Берлин";
        var berlin = service.Filter(orders, o => o.Price > minPrice && o.City == city);
        Console.WriteLine("--- Заказы дороже 5000 из Берлина ---");
        Console.WriteLine(string.Join(", ", berlin.ConvertAll(o => o.Id.ToString())));

        // 2. Клиенты с почтой на @gmail.com.
        var gmail = service.Filter(customers, c => c.Email.EndsWith("@gmail.com"));
        Console.WriteLine("--- Клиенты с почтой @gmail.com ---");
        Console.WriteLine(string.Join(", ", gmail.ConvertAll(c => c.Name)));

        // 3. Обработка: уведомление в консоль.
        Console.WriteLine("--- Обработка с уведомлением в консоль ---");
        service.Process(berlin, o => Console.WriteLine($"Консоль: заказ {o.Id} обработан"));

        // 4. Обработка: другой способ уведомления (push), остальные новые заказы.
        Console.WriteLine("--- Обработка с уведомлением push ---");
        var rest = service.Filter(orders, o => o.Status == "New");
        service.Process(rest, o => Console.WriteLine($"Push: заказ {o.Id} обработан"));

        // 5. Результаты независимых модулей.
        Console.WriteLine("--- Модули ---");
        Console.WriteLine($"Статистика: обработано {stats.Count}");
        Console.WriteLine("Аудит: " + string.Join(", ", audit.Ids));
    }
}
```

### Ожидаемый вывод

```text
--- Заказы дороже 5000 из Берлина ---
1, 4
--- Клиенты с почтой @gmail.com ---
Анна, Вера
--- Обработка с уведомлением в консоль ---
Консоль: заказ 1 обработан
Консоль: заказ 4 обработан
--- Обработка с уведомлением push ---
Push: заказ 2 обработан
Push: заказ 3 обработан
--- Модули ---
Статистика: обработано 4
Аудит: 1, 4, 2, 3
```
