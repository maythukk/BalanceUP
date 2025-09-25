using SQLite;
using System.IO;
using BalanceUP.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using BalanceUP.Models;

namespace BalanceUP.Data
{
    public class DatabaseHelper
    {
        private readonly SQLiteConnection _connection;

        public DatabaseHelper()
        {
            // Create or open the database
            _connection = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, "balanceup.db"));
            _connection.CreateTable<UserInfo>(); // Create the UserInfo table if it doesn't exist
            _connection.CreateTable<Expense>(); // Create Expense table
            _connection.CreateTable<Budget>(); // Create Budget table
        }

        // Insert a user into the database (method)
        public void InsertUser(UserInfo user)
        {
            try
            {
                // Check if the username already exists
                var existingUser = GetUserByUsername(user.Username);
                if (existingUser != null)
                {
                    Console.WriteLine($"User  with username '{user.Username}' already exists.");
                    throw new Exception("Username already exists.");
                }

                _connection.Insert(user); // Insert the user into the database
                Console.WriteLine($"User  '{user.Username}' inserted successfully."); // Log the username
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting user: {ex.Message}");
                throw;
            }
        }

        // Update a user in the database (method)
        public void UpdateUser(UserInfo user)
        {
            try
            {
                _connection.Update(user); // Update the user in the database
                Console.WriteLine($"User  '{user.Username}' updated successfully."); // Log the update
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }
        }

        // Retrieve a user by username (method)
        public UserInfo GetUserByUsername(string username)
        {
            try
            {
                var user = _connection.Table<UserInfo>().FirstOrDefault(u => u.Username == username); // Retrieve user by username
                if (user != null)
                {
                    Console.WriteLine($"Retrieved User: {user.Username}, Email: {user.Email}"); // Log the retrieved user
                }
                else
                {
                    Console.WriteLine($"No user found with username: {username}"); // Log if no user is found
                }
                return user; // Return the user or null if not found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user by username: {ex.Message}");
                return null; // Return null if there is an error
            }
        }

        // Insert an expense into the database (method)
        public void InsertExpense(Expense expense)
        {
            try
            {
                // link the expense with the currently logged-in user
                if (UserInfo.Current != null)
                {
                    expense.UserId = UserInfo.Current.Id; // Set the UserId to the current ID
                }

                _connection.Insert(expense); // Insert the expense into the database
                Console.WriteLine("Expense inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting expense: {ex.Message}");
                throw;
            }
        }

        // Delete an expense from the database (method)
        public void DeleteExpense(int id)
        {
            try
            {
                var expense = _connection.Table<Expense>().FirstOrDefault(e => e.Id == id);
                if (expense != null)
                {
                    _connection.Delete(expense); // Delete the expense from the database
                    Console.WriteLine("Expense deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Expense not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting expense: {ex.Message}");
                throw;
            }
        }

        // Retrieve all expenses for the current user (method)
        public List<Expense> GetAllExpenses()
        {
            try
            {
                if (UserInfo.Current != null)
                {
                    // Retrieve expenses related with the current user
                    return _connection.Table<Expense>().Where(e => e.UserId == UserInfo.Current.Id).ToList();
                }
                return new List<Expense>(); // Return an empty list if no user is logged in
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving all expenses: {ex.Message}");
                return new List<Expense>(); // Return an empty list if an error occurs
            }
        }

        // Get total expenses for the current user (method)
        public decimal GetTotalExpenses()
        {
            try
            {
                if (UserInfo.Current != null)
                {
                    return _connection.Table<Expense>().Where(e => e.UserId == UserInfo.Current.Id).Sum(e => e.Amount); // Calculate total expenses for the current user
                }
                return 0; // Return 0 if no user is logged in
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating total expenses: {ex.Message}");
                return 0; // Return 0 if an error occurs
            }
        }

        // Save the budget (method)
        public void SaveBudget(decimal amount)
        {
            try
            {
                if (UserInfo.Current != null)
                {
                    var budget = new Budget
                    {
                        UserId = UserInfo.Current.Id, // link the budget with the current user
                        Amount = amount,
                        SetDate = DateTime.Now
                    };

                    // Insert the budget into the database
                    _connection.Insert(budget);
                    Console.WriteLine("Budget saved successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving budget: {ex.Message}");
                throw;
            }
        }

        // Get the budget for the current month (method)
        public Budget GetBudgetForCurrentMonth()
        {
            try
            {
                if (UserInfo.Current == null)
                {
                    Console.WriteLine("No current user logged in");
                    return null;
                }

                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                Console.WriteLine($"Looking for budget: User={UserInfo.Current.Id}, Month={currentMonth}, Year={currentYear}");

                // Try to get all budgets for the user and filter in memory
                var allBudgets = _connection.Table<Budget>().Where(b => b.UserId == UserInfo.Current.Id).ToList();
                Console.WriteLine($"Found {allBudgets.Count} budgets for this user");

                foreach (var b in allBudgets)
                {
                    Console.WriteLine($"Budget ID={b.Id}: Amount={b.Amount}, Date={b.SetDate}");
                }

                // Filter manually
                var matchingBudget = allBudgets.FirstOrDefault(b =>
                    b.SetDate.Month == currentMonth && b.SetDate.Year == currentYear);

                if (matchingBudget != null)
                {
                    Console.WriteLine($"Found matching budget: {matchingBudget.Amount}");
                    return matchingBudget;
                }
                else
                {
                    Console.WriteLine("No matching budget found");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving budget: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        // Get expenses for a specific month (method)
        public List<Expense> GetExpensesByMonth(DateTime month)
        {
            try
            {
                var startDate = new DateTime(month.Year, month.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                // Query the database for expenses within the specified month
                return _connection.Table<Expense>()
                    .Where(e => e.Date >= startDate && e.Date <= endDate && e.UserId == UserInfo.Current.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving expenses for month: {ex.Message}");
                return new List<Expense>(); // Return an empty list if an error occurs
            }
        }
    }
}