using System.Data.Entity;

namespace EffinghamDataAccess
{
    // Basically this sort of class would get one property per table.

    public class DataAccessContext : DbContext
    {
        public DbSet<AccountData> Accounts { get; set; }

        // Could specify the connection string this way.
        //public DataAccessContext()
        //    : base("ConnectionStringConfigFileKey")
        //{
            
        //}
    }
}
