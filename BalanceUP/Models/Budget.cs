using SQLite;
namespace BalanceUP.Models
{
    public class Budget
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Primary key

        [Indexed]
        public int UserId { get; set; } // Foreign key

        public decimal Amount { get; set; } // Budget amount

        // Store as string
        public string SetDateStr { get; set; }

        [Ignore]
        public DateTime SetDate
        {
            get => DateTime.Parse(SetDateStr ?? DateTime.Now.ToString());
            set => SetDateStr = value.ToString("o");
        }
    }
}