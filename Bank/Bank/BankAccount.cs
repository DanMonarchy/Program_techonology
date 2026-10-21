using System.Text;

namespace Bank;


// потомок класса object => можно переопределить
// виртуальные методы, находящиеся в object

/// <summary>
/// Базовый счёт: хранит историю операций и умеет пополняться и списывать деньги.
/// </summary>
public class BankAccount
{
    private readonly decimal _minimumBalance;
    static private int s_accountNuberSeed = 1000000000;

    /// <summary>
    /// Номер счёта, который банк выдаёт автоматически при создании счёта.
    /// </summary>
    public string Number { get; }

    /// <summary>
    /// Имя владельца счёта.
    /// </summary>
    public string Owner { get; private set; }

    /// <summary>
    /// Текущий остаток на счёте.
    /// </summary>
    public decimal Balance
    {
        get
        {
            decimal balance = 0;
            foreach (var item in _allTransactions)
            {
                balance += item.Amount;
            }

            return balance;
        }
    }

    private List<Transaction> _allTransactions = new List<Transaction>();

    /// <summary>
    /// Создаёт новый счёт и зачисляет на него начальный остаток.
    /// </summary>
    /// <param name="name">Имя владельца счёта.</param>
    /// <param name="initialBalance">Начальный остаток. Ноль и отрицательные значения игнорируются.</param>
    public BankAccount(string name, decimal initialBalance) : this(name, initialBalance, 0)
    {

    }

    /// <summary>
    /// Создаёт новый счёт с минимальным остатком.
    /// </summary>
    /// <param name="name">Имя владельца счёта.</param>
    /// <param name="initialBalance">Начальный остаток. Ноль и отрицательные значения игнорируются.</param>
    /// <param name="minimumBalance">Минимальный остаток, ниже которого списание запрещено.</param>
    public BankAccount(string name, decimal initialBalance, decimal minimumBalance)
    {

        Owner = name; // this.Owner = name
        Number = s_accountNuberSeed.ToString();
        s_accountNuberSeed++;

        _minimumBalance = minimumBalance;

        if (initialBalance > 0)
            MakeDeposit(initialBalance, DateTime.UtcNow, "Inital balance");
    }

    /// <summary>
    /// Пополняет счёт и записывает операцию в историю.
    /// </summary>
    /// <param name="amount">Сумма пополнения. Должна быть положительной.</param>
    /// <param name="date">Дата операции.</param>
    /// <param name="note">Комментарий к операции.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Бросается, если <paramref name="amount"/> меньше или равен нулю.
    /// </exception>
    public void MakeDeposit(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount of deposit must be positive");
        }

        var deposit = new Transaction(amount, date, note);
        _allTransactions.Add(deposit);
    }

    /// <summary>
    /// Списывает деньги со счёта и записывает операцию в историю.
    /// </summary>
    /// <param name="amount">Сумма списания. Должна быть положительной.</param>
    /// <param name="date">Дата операции.</param>
    /// <param name="note">Комментарий к операции.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Бросается, если <paramref name="amount"/> меньше или равен нулю.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Бросается, если после списания остаток уходит ниже минимального.
    /// </exception>
    public void MakeWithdrawal(decimal amount, DateTime date, string note)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        Transaction? overdraftTransaction = CheckWithdrawalLimit(Balance - amount < _minimumBalance);
        Transaction? withdrawal = new(-amount, date, note);

        _allTransactions.Add(withdrawal);

        if (overdraftTransaction is not null)
            _allTransactions.Add(overdraftTransaction);
    }

    /// <summary>
    /// Проверяет, допустимо ли списание за пределы минимального остатка.
    /// </summary>
    /// <param name="isOvesdrawn">Признак того, что остаток уходит ниже минимального.</param>
    /// <returns>
    /// Транзакция для оформления овердрафта либо <see langword="null"/>, если овердрафт не требуется.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Бросается, если списание недопустимо.
    /// </exception>
    protected virtual Transaction? CheckWithdrawalLimit(bool isOvesdrawn)
    {
        if (isOvesdrawn)
        {
            throw new InvalidOperationException("Not sufficient rubls for this withdrawal");
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Возвращает историю операций по счёту в виде таблицы.
    /// </summary>
    /// <returns>Текст с историей операций.</returns>
    public string GetAccountHistory()
    {
        var report = new StringBuilder();
        decimal balance = 0;
        report.Append("Data\t\tAmount\tBalance\tNote");
        foreach (var item in _allTransactions)
        {
            balance += item.Amount;
            report.AppendLine($"" +
                $"{item.Date.ToShortDateString()}\t" +
                $"{item.Amount}\t{balance}\t{item.Note}");
        }
        return report.ToString();
    }

    // Ключевое слово virtual позволяет в дочернем классе
    // предоставить другую реализацию
    // Метода PerformMonthAndTransactions
    /// <summary>
    /// Выполняет ежемесячные операции по счёту.
    /// </summary>
    public virtual void PerformMonthAndTransactions()
    {

    }

    // переопределяем метод базового класса - класса object
    // toString возвращает строку с информацией об объекте
    /// <summary>
    /// Возвращает описание счёта: тип, владелец, номер и остаток.
    /// </summary>
    /// <returns>Текстовое описание счёта.</returns>
    public override string ToString()
    => $"Type:{GetType().Name}\tOwner:{Owner}\tNumber of account:{Number}\tBalance:{Balance}";
}
