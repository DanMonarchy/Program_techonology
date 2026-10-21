using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Bank;

/// <summary>
/// Накопительный счёт: ежемесячно начисляет проценты на остаток.
/// </summary>
public class InterestEarningAccount : BankAccount
{
    /// <summary>
    /// Создаёт накопительный счёт.
    /// </summary>
    /// <param name="name">Имя владельца счёта.</param>
    /// <param name="initialBalance">Начальный остаток.</param>
    public InterestEarningAccount(string name, decimal initialBalance)
        : base(name, initialBalance)
    { }

    // override позволяет в дочернем классе определить новую реализацию
    // метода PerformMonthAndTransactions
    /// <summary>
    /// Начисляет проценты на остаток счёта.
    /// </summary>
    public override void PerformMonthAndTransactions()
    {
        if (Balance > 500m)
        {
            decimal interest = Balance * 0.02m;
            MakeDeposit(interest, DateTime.UtcNow, "Apply month interest");
        }

    }
}
