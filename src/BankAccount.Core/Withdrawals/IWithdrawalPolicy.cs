using BankAccounts.Core.Withdrawals;

namespace BankAccounts.Core.Withdrawals;

public interface IWithdrawalPolicy
{
    WithdrawalDecision Evaluate(
        decimal currentBalance,
        decimal amount,
        decimal minimumBalance);
}