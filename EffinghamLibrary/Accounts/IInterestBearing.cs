namespace EffinghamLibrary.Accounts
{
    public interface IInterestBearing : IBankAccountMultipleCurrency
    {
        void AddMonthlyInterest();
    }
}
