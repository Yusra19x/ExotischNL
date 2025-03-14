using System.Data.SQLite;
namespace ExotischNLConsoleApp.Data
{
    internal class ConnectionDB
    {
        private string connectionString = "Data Source=C:\\Program Files\\SQLiteStudio\\Exotisch Nederland.db;";
        internal SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
