namespace EffinghamLibrary.Accounts
{
    // Default access modifier is internal
    public interface IBankAccount
    {
        int AccountNumber { get; }

        string CustomerName { get; set; }

        decimal Balance { get; }

        void Deposit(decimal amount);

        void Withdraw(decimal amount);
    }

    public interface IBankAccountMultipleCurrency : IBankAccount
    {
        void Deposit(decimal amount, CurrencyType currency);
    }
}
