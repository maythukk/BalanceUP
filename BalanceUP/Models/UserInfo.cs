using SQLite;

namespace BalanceUP.Models
{
    public class UserInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Unique identifier
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfileImagePath { get; set; } // Path to the user's profile image

        // property to hold the current user
        public static UserInfo Current { get; set; }
    }
}