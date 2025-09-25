using SQLite;

namespace BalanceUP.Models
{
    public class Expense
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Unique identifier
        public decimal Amount { get; set; } // Amount 
        public string Category { get; set; } // Category
        public DateTime Date { get; set; } // Date when the expense was added
        public int UserId { get; set; } // Foreign key
    }
}