using System.Data.Entity;

namespace EffinghamDataAccess
{
    public class DataAccessInitializer : DropCreateDatabaseAlways<DataAccessContext>
    {
        /// <summary>
        /// Seeds the DB with some arbitrary accounts.
        /// </summary>
        /// <param name="context">The context to seed. </param>
        protected override void Seed(DataAccessContext context)
        {
            context.Accounts.AddRange(new []
            {
                new AccountData
                {
                    AccountNumber = 1,
                    CustomerName = "Steve",
                    Balance = 100.0m,
                    AccountType = "C"
                },
                new AccountData
                {
                    AccountNumber = 2,
                    CustomerName = "Kate",
                    Balance = 206.0m,
                    AccountType = "S"
                }
            });

            context.SaveChanges();
        }
    }
}
