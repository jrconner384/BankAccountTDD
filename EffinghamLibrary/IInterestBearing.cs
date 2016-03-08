namespace EffinghamLibrary
{
    public interface IInterestBearing : IBankAccountMultipleCurrency
    {
        void AddMonthlyInterest();
    }
}
