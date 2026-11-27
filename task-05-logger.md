# Задание 5. Логгер

**Время:** 40-50 минут. Синтаксис гуглить можно, рассуждайте вслух.

## Что нужно сделать

1. **Разобраться.** Над классом и над каждым его методом напишите XML-комментарий (`/// <summary>`, при необходимости `<param>` и `<returns>`). Опишите, **что** делает код и зачем, а не **как** (не «проходит циклом по списку»).
2. **Доработать** код под требования заказчика (ниже). Названия приёмов и типов в требованиях не указаны, выбирайте сами.
3. **Показать в `Main`**, что всё работает (сценарий ниже). Тестовые данные создайте сами.
4. **Ответить письменно на 3 вопроса** в блоке `#region Ответы на вопросы` в конце класса `Logger`: под каждым вопросом, после слова «Ответ:». Блок уже есть в заготовке. Если вы переименовали или заменили класс, перенесите блок в итоговый класс.

## Требования заказчика

- Получатели логов (консоль, файл, память, сеть) добавляются и убираются во время работы, `Logger` при этом не меняется.
- Одно обращение к логгеру обслуживает сразу несколько получателей.
- Логировать можно объект любого типа, а как он превращается в строку, определяет вызывающий код.

## Что показать в `Main`

- одно сообщение, которое приходит и в консоль, и в список в памяти;
- что после отключения консоли она больше не получает сообщений, а память получает;
- запись заказа и пользователя в разном формате;
- что будет, если первый получатель падает с ошибкой (что вы увидите и что бы вы с этим сделали).

## Заготовка кода

```csharp
using System;
using System.Collections.Generic;
using System.IO;

public class Order { public int Id; public decimal Price; }
public class User { public string Name; }

public class Logger
{
    public void Log(string msg, string target)
    {
        if (target == "console") Console.WriteLine(DateTime.Now + " " + msg);
        else if (target == "file") File.AppendAllText("log.txt", DateTime.Now + " " + msg + "\n");
    }

    public void LogOrder(Order o, string target) => Log("Заказ " + o.Id + " на " + o.Price, target);
    public void LogUser(User u, string target) => Log("Пользователь " + u.Name, target);

    #region Ответы на вопросы
    // Ответьте своими словами под каждым вопросом.
    //
    // Вопрос 1. Что такое multicast-делегат и в каком порядке вызываются подписчики?
    // Ответ:
    //
    // Вопрос 2. Если первый подписчик выбросит исключение, будет ли вызван второй?
    // Ответ:
    //
    // Вопрос 3. Чем event Action<string> лучше открытого поля Action<string>?
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
| Получатели добавляются и убираются на ходу | событие `event Action<string> Message` (`+=` / `-=`) |
| Одно обращение обслуживает несколько получателей | multicast-делегат |
| Логируется любой тип, формат задаёт вызывающий | `Log<T>(T item, Func<T, string> format)` |
| Падение получателя | `LogSafe`: обход `GetInvocationList()` с `try/catch` |

Блок с ответами перенесён в `Logger`, как и требует задание. `Log` вызывает получателей напрямую, чтобы в `Main` было видно, что падение первого получателя обрывает цепочку. `LogSafe` это исправляет.

```csharp
using System;
using System.Collections.Generic;

/// <summary>Заказ.</summary>
public class Order { public int Id; public decimal Price; }

/// <summary>Пользователь.</summary>
public class User { public string Name; }

/// <summary>Рассылает сообщения всем подключённым получателям; о них ничего не знает.</summary>
public class Logger
{
    /// <summary>Получатели сообщений (консоль, файл, память, сеть).</summary>
    public event Action<string> Message;

    /// <summary>Логирует объект любого типа в формате, заданном вызывающим кодом.</summary>
    public void Log<T>(T item, Func<T, string> format)
    {
        Message?.Invoke(Stamp(format(item)));
    }

    /// <summary>То же, но сбой одного получателя не мешает остальным.</summary>
    public void LogSafe<T>(T item, Func<T, string> format)
    {
        var handlers = Message;
        if (handlers == null) return;

        string line = Stamp(format(item));
        foreach (Action<string> h in handlers.GetInvocationList())
        {
            try { h(line); }
            catch (Exception ex) { Console.WriteLine($"LogSafe: ошибка получателя «{ex.Message}»"); }
        }
    }

    /// <summary>Добавляет к тексту время.</summary>
    private static string Stamp(string text)
    {
        return DateTime.Now.ToString("HH:mm:ss") + " " + text;
    }

    #region Ответы на вопросы
    // Ответьте своими словами под каждым вопросом.
    //
    // Вопрос 1. Что такое multicast-делегат и в каком порядке вызываются подписчики?
    // Ответ: Делегат, который хранит список методов и вызывает их все одним
    //        вызовом. Порядок вызова - порядок подписки (+=). Если у делегата
    //        есть возвращаемое значение, остаётся результат последнего метода.
    //
    // Вопрос 2. Если первый подписчик выбросит исключение, будет ли вызван второй?
    // Ответ: Нет. Исключение обрывает цепочку и уходит вызывающему коду. Чтобы
    //        остальные получили сообщение, обходят GetInvocationList() и
    //        вызывают каждого в своём try/catch (метод LogSafe).
    //
    // Вопрос 3. Чем event Action<string> лучше открытого поля Action<string>?
    // Ответ: У event снаружи доступны только += и -=. Нельзя вызвать его извне
    //        и нельзя присвоить через =, затерев всех подписчиков. Вызывает
    //        событие только сам класс.
    //
    #endregion
}

public class Program
{
    public static void Main()
    {
        var logger = new Logger();
        var memory = new List<string>();

        Action<string> toConsole = Console.WriteLine;
        Action<string> toMemory = memory.Add;

        logger.Message += toConsole;
        logger.Message += toMemory;

        // 1. Одно сообщение приходит и в консоль, и в память.
        Console.WriteLine("--- Одно сообщение в консоль и в память ---");
        logger.Log("Запуск системы", s => s);
        Console.WriteLine($"В памяти: {memory.Count}");

        // 2. Консоль отключена: память продолжает получать.
        Console.WriteLine("--- Консоль отключена ---");
        logger.Message -= toConsole;
        logger.Log("Консоль отключена", s => s);
        Console.WriteLine($"В памяти: {memory.Count}");

        // 3. Заказ и пользователь в разном формате.
        Console.WriteLine("--- Заказ и пользователь в разном формате ---");
        logger.Log(new Order { Id = 7, Price = 1500m }, o => $"Заказ {o.Id} на {o.Price}");
        logger.Log(new User { Name = "Анна" }, u => $"Пользователь {u.Name}");
        Console.WriteLine("Содержимое памяти:");
        foreach (var s in memory) Console.WriteLine(s);

        // 4. Первый получатель падает.
        Console.WriteLine("--- Первый получатель падает ---");
        var broken = new Logger();
        broken.Message += s => throw new InvalidOperationException("получатель недоступен");
        broken.Message += toConsole;

        try { broken.Log("Проверка Log", s => s); }
        catch (Exception ex) { Console.WriteLine($"Log: исключение «{ex.Message}», до консоли сообщение не дошло"); }

        broken.LogSafe("Проверка LogSafe", s => s);
    }
}
```

### Ожидаемый вывод

`<время>` в выводе означает текущее время в формате `ЧЧ:мм:сс`.

```text
--- Одно сообщение в консоль и в память ---
<время> Запуск системы
В памяти: 1
--- Консоль отключена ---
В памяти: 2
--- Заказ и пользователь в разном формате ---
Содержимое памяти:
<время> Запуск системы
<время> Консоль отключена
<время> Заказ 7 на 1500
<время> Пользователь Анна
--- Первый получатель падает ---
Log: исключение «получатель недоступен», до консоли сообщение не дошло
LogSafe: ошибка получателя «получатель недоступен»
<время> Проверка LogSafe
```
