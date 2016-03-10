using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EffinghamDataAccess
{
    public class AccountData
    {
        /*
         * Could use the Fluent API instead of attributes to map this to the DB.
         * Could define multiple keys, table relationships.
         */

        /// <summary>
        /// Represents the unique identifier for the account.
        /// </summary>
        [Key] // Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Tell the DB to not automatically generate the value.
        public int AccountNumber { get; set; }

        /// <summary>
        /// Represents the account holder's name.
        /// </summary>
        [StringLength(100)] // Will use VARCHAR(MAX) if this isn't specified
        [Required] // Not null
        public string CustomerName { get; set; }

        /// <summary>
        /// Represents the account's current balance.
        /// </summary>
        [Required]
        public decimal Balance { get; set; }

        /// <summary>
        /// Distinguishes accounts by type, such as Checking and Savings.
        /// </summary>
        [StringLength(1)]
        [Required]
        public string AccountType { get; set; }
    }
}
