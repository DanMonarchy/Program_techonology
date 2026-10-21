using System;
using System.Collections.Generic;
using System.Text;

namespace Bank;

/// <summary>
/// Подарочная карта: ежемесячно пополняется на фиксированную сумму.
/// </summary>
public class GiftCardAccount : BankAccount
{
    private readonly decimal _monthlyDeposit = 0m;

    // monthlyDeposit - параметр по умолчанию, по умолчанию принимает 0,
    // при создании new GiftCardAccount("Ulyana", 1000); - monthlyDeposit = 0
    // new GiftCardAccount("Ulyana", 1000, 5000) => monthlyDeposit = 5000
    /// <summary>
    /// Создаёт подарочную карту.
    /// </summary>
    /// <param name="name">Имя владельца карты.</param>
    /// <param name="initialBalance">Начальный остаток.</param>
    /// <param name="monthlyDeposit">Сумма ежемесячного пополнения.</param>
    public GiftCardAccount(string name, decimal initialBalance, decimal monthlyDeposit = 0)
        : base(name, initialBalance)
        => _monthlyDeposit = monthlyDeposit;

    /// <summary>
    /// Пополняет карту на фиксированную сумму.
    /// </summary>
    public override void PerformMonthAndTransactions()
    {
        if (_monthlyDeposit != 0)
        {
            MakeDeposit(_monthlyDeposit, DateTime.UtcNow, "Add monthly deposit");
        }
    }

    /// <summary>
    /// Возвращает описание счёта с указанием ежемесячного пополнения.
    /// </summary>
    /// <returns>Текстовое описание счёта.</returns>
    public override string ToString() => base.ToString() + $"monthly deposit: {_monthlyDeposit}";
}
