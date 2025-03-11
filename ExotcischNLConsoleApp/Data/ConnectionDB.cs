using System.Data.SqlClient;
namespace ExotischNLConsoleApp.Data
{
    internal class ConnectionDB
    {
        private string connectionString = "Server=tcp:exotischnl-serv.database.windows.net,1433;Initial Catalog=ExotischNL;Persist Security Info=False;User ID=TeamTN;Password=Team1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        internal SqlConnection GetConnection() // Used internal because it protects the method from usage outside this project
        {
            return new SqlConnection(connectionString);
        }
    }
}
