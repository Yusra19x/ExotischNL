using System.Data.SqlClient;
using ExotischNLConsoleApp.Models;
using ConsoleTables;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExotischNLConsoleApp.Data
{
    internal class DataMain
    {
        private ConnectionDB _conn;
        public DataMain()
        {
            _conn = new ConnectionDB();

        }
        public void CheckIfTableEmpty(string columnName1, string filterOperator, string betweenValue1, string betweenValue2, string columnName2, string sortingType, string tableName) // Check if the table is empty. if it is show message
        {
            SqlConnection conn = _conn.GetConnection(); // get connection from ConnectionDB.cs
            string query = $"SELECT COUNT(*) FROM {tableName}"; // SELECT query to check if the amount of rows is 0
            SqlCommand command = new SqlCommand(query, conn); // Makes the command with the given query and database connection
            conn.Open(); // Open the connection with the database so the app can send messages or commands
            int rowCount = (int)command.ExecuteScalar(); // Execute command using scalar (Returns a single value. in this case its a COUNT)

            if (rowCount == 0) // if the table is empty then send the message below
            {
                Console.WriteLine("de tabel is leeg. Voeg een organisme toe.");
            }
            else
            {
                DisplayAllObservations(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType, tableName);
            }
        }

        public void DisplayAllObservations(string columnName1, string filterOperator, string betweenValue1, string betweenValue2, string columnName2, string sortingType, string tableName) // SELECT all data from LOCATIE, SOORT, WETENSCHAPPELIJKENAAM and WAARNEMING/GEVALIDEERDEWAARNEMING and displays it all in a table
        {
            SqlConnection conn = _conn.GetConnection();
            string displayOrganisms = null;
            if (string.IsNullOrWhiteSpace(betweenValue2)) // If the second value is empty then don't use the second value in the select statemnt (operator is not between)
            {
                displayOrganisms = $"SELECT {tableName}.*, dbo.WETENSCHAPPELIJKENAAM.*, dbo.SOORT.*, dbo.LOCATIE.* FROM {tableName} INNER JOIN dbo.WETENSCHAPPELIJKENAAM ON {tableName}.WNid = dbo.WETENSCHAPPELIJKENAAM.WNid INNER JOIN dbo.SOORT ON {tableName}.Sid = dbo.SOORT.Sid INNER JOIN dbo.LOCATIE ON {tableName}.Lid = dbo.LOCATIE.Lid WHERE {columnName1}{filterOperator}{betweenValue1} ORDER BY {columnName2} {sortingType}";
            }
            else if (string.IsNullOrWhiteSpace(filterOperator) && string.IsNullOrWhiteSpace(betweenValue2)) // If the operator does not exist then no filter has been chosen
            {
                displayOrganisms = $"SELECT {tableName}.*, dbo.WETENSCHAPPELIJKENAAM.*, dbo.SOORT.*, dbo.LOCATIE.* FROM {tableName} INNER JOIN dbo.WETENSCHAPPELIJKENAAM ON {tableName}.WNid = dbo.WETENSCHAPPELIJKENAAM.WNid INNER JOIN dbo.SOORT ON {tableName}.Sid = dbo.SOORT.Sid INNER JOIN dbo.LOCATIE ON {tableName}.Lid = dbo.LOCATIE.Lid  WHERE {columnName1} ORDER BY {columnName2} {sortingType}";
            }
            else // Else the chosen filter is between which requires 2 values
            {
                displayOrganisms = $"SELECT {tableName}.*, dbo.WETENSCHAPPELIJKENAAM.*, dbo.SOORT.*, dbo.LOCATIE.* FROM {tableName} INNER JOIN dbo.WETENSCHAPPELIJKENAAM ON {tableName}.WNid = dbo.WETENSCHAPPELIJKENAAM.WNid INNER JOIN dbo.SOORT ON {tableName}.Sid = dbo.SOORT.Sid INNER JOIN dbo.LOCATIE ON {tableName}.Lid = dbo.LOCATIE.Lid WHERE {columnName1} {filterOperator} {betweenValue1} {betweenValue2} ORDER BY {columnName2} {sortingType}";
            }


            try // Try to read the row data and put it in a table using an extention
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(displayOrganisms, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                var table = new ConsoleTable("Wid", "Naam", "Wetenschappelijke naam", "Omschrijving", "Aantal", "Soort", "Voorkomen", "Geslacht", "Zekerheid", "Datum", "Tijd", "Toelichting");
                while (reader.Read())
                {
                    if(tableName == "dbo.WAARNEMING")
                    {
                        table.AddRow(reader[$"Wid"], reader["Naam"], reader["WetenschappelijkeNaam"], reader["Omschrijving"], reader["aantal"], reader["Soort"], reader["Voorkomen"], reader["geslacht"], reader["zekerheid"], reader["Datum"], reader["Tijd"], reader["toelichting"]);
                    }
                    else
                    {
                        table.AddRow(reader[$"GWid"], reader["Naam"], reader["WetenschappelijkeNaam"], reader["Omschrijving"], reader["aantal"], reader["Soort"], reader["Voorkomen"], reader["geslacht"], reader["zekerheid"], reader["Datum"], reader["Tijd"], reader["toelichting"]);
                    }

                }
                table.Write();
            }
            catch (Exception ex) // If there is an exception send a message
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally // Always close the connection with the database if it's open
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public decimal InsertScientificName(ScientificName scientificName) // INSERT data into WETENSCHAPPELIJKENAAM
        {
            string insertIntoWN = "INSERT INTO dbo.WETENSCHAPPELIJKENAAM (Naam,WetenschappelijkeNaam) VALUES (@Name, @ScienceName); SELECT SCOPE_IDENTITY();";
            SqlConnection conn = _conn.GetConnection();
            decimal resultScalar = -1;
            try
            {
                SqlCommand cmd = new SqlCommand(insertIntoWN, conn);
                cmd.Parameters.AddWithValue("@Name", scientificName.Name);
                cmd.Parameters.AddWithValue("@ScienceName", scientificName.ScienceName);
                conn.Open();
                cmd.ExecuteNonQuery(); // Executes the INSERT query
                resultScalar = (decimal)cmd.ExecuteScalar(); // Gets the WNid from the INSERTED record
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }

            }
            if (resultScalar == -1) // Show a message if the WNid is -1 (default) cause that's not supposed to happen
            {
                Console.WriteLine($"WNid is {resultScalar}");
            }
            return resultScalar;

        }

        public decimal InsertType(OrganismType type) // INSERT data into SOORT
        {
            string insertIntoSoort = "INSERT INTO dbo.Soort (Soort,Voorkomen) VALUES (@Type, @Origin); SELECT SCOPE_IDENTITY();";
            SqlConnection conn = _conn.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(insertIntoSoort, conn);
                cmd.Parameters.AddWithValue("@Type", type.OrgType);
                cmd.Parameters.AddWithValue("@Origin", type.Origin);
                conn.Open();
                cmd.ExecuteNonQuery(); // Executes the INSERT query
                decimal resultScalar = (decimal)cmd.ExecuteScalar(); // Gets the WNid from the INSERTED record

                return resultScalar;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return -1;

        }

        public decimal InsertLocation(Location location) // INSERT data into LOCATIE
        {
            string insertIntoLocatie = "INSERT INTO dbo.LOCATIE (Locatienaam,provincie,lengtegraad,breedtegraad) VALUES (@LocationName, @Province,@Longitude,@Latitude); SELECT SCOPE_IDENTITY();";
            SqlConnection conn = _conn.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(insertIntoLocatie, conn);
                cmd.Parameters.AddWithValue("@LocationName", location.LocationName);
                cmd.Parameters.AddWithValue("@Province", location.Province);
                cmd.Parameters.AddWithValue("@Longitude", location.Longitude);
                cmd.Parameters.AddWithValue("@Latitude", location.Latitude);
                conn.Open();
                cmd.ExecuteNonQuery(); // Executes the INSERT query
                decimal resultScalar = (decimal)cmd.ExecuteScalar(); // Gets the WNid from the INSERTED record
                return resultScalar;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return -1;

        }

        public void InsertObservation(decimal WNid, decimal Sid, decimal Lid, Observation observation) // INSERT data into WAARNEMING
        {
            string insertIntoObservation = "INSERT INTO dbo.WAARNEMING (Omschrijving,Datum,Tijd,WNid,Sid,Lid,toelichting,aantal,geslacht,zekerheid,ManierDelen) VALUES (@Description, @Date,@Time,@WNid,@Sid,@Lid,@Explanation,@Amount,@Sex,@HowSure,@Share)";
            SqlConnection conn = _conn.GetConnection();

            try
            {

                SqlCommand cmd = new SqlCommand(insertIntoObservation, conn);
                cmd.Parameters.AddWithValue("@Description", observation.Description);
                cmd.Parameters.AddWithValue("@Date", observation.Date);
                cmd.Parameters.AddWithValue("@Time", observation.Time);
                cmd.Parameters.AddWithValue("@WNid", WNid);
                cmd.Parameters.AddWithValue("@Sid", Sid);
                cmd.Parameters.AddWithValue("@Lid", Lid);
                cmd.Parameters.AddWithValue("@Explanation", observation.Explanation);
                cmd.Parameters.AddWithValue("@HowSure", observation.HowSure);
                cmd.Parameters.AddWithValue("@Amount", observation.Amount);
                cmd.Parameters.AddWithValue("@Sex", observation.Sex);
                cmd.Parameters.AddWithValue("@Share", observation.Share);

                conn.Open();
                cmd.ExecuteNonQuery(); // Execute INSERT query with the given parameters



            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        public void DisplayForAction() // Display all observations in WAARNEMING so you can choose what observation to approve
        {
            SqlConnection conn = _conn.GetConnection();
            string displayOrganisms = $"SELECT dbo.WAARNEMING.*, dbo.WETENSCHAPPELIJKENAAM.*, dbo.SOORT.*, dbo.LOCATIE.* FROM dbo.WAARNEMING INNER JOIN dbo.WETENSCHAPPELIJKENAAM ON dbo.WAARNEMING.WNid = dbo.WETENSCHAPPELIJKENAAM.WNid INNER JOIN dbo.SOORT ON dbo.WAARNEMING.Sid = dbo.SOORT.Sid INNER JOIN dbo.LOCATIE ON dbo.WAARNEMING.Lid = dbo.LOCATIE.Lid";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(displayOrganisms, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                var table = new ConsoleTable("Wid", "Naam", "Wetenschappelijke naam", "Omschrijving", "Aantal", "Soort", "Voorkomen", "Geslacht", "Zekerheid", "Datum", "Tijd", "Toelichting");
                while (reader.Read())
                {
                    table.AddRow(reader["Wid"], reader["Naam"], reader["WetenschappelijkeNaam"], reader["Omschrijving"], reader["aantal"], reader["Soort"], reader["Voorkomen"], reader["geslacht"], reader["zekerheid"], reader["Datum"], reader["Tijd"], reader["toelichting"]);

                }
                table.Write();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        public void DisplayForConfirmation(int rowID) // Confirm your choice
        {
            SqlConnection conn = _conn.GetConnection();
            string displayOrganisms = $"SELECT dbo.WAARNEMING.*, dbo.WETENSCHAPPELIJKENAAM.*, dbo.SOORT.*, dbo.LOCATIE.* FROM dbo.WAARNEMING INNER JOIN dbo.WETENSCHAPPELIJKENAAM ON dbo.WAARNEMING.WNid = dbo.WETENSCHAPPELIJKENAAM.WNid INNER JOIN dbo.SOORT ON dbo.WAARNEMING.Sid = dbo.SOORT.Sid INNER JOIN dbo.LOCATIE ON dbo.WAARNEMING.Lid = dbo.LOCATIE.Lid WHERE dbo.WAARNEMING.Wid = {rowID}";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(displayOrganisms, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                var table = new ConsoleTable("Wid", "Naam", "Wetenschappelijke naam", "Omschrijving", "Aantal", "Soort", "Voorkomen", "Geslacht", "Zekerheid", "Datum", "Tijd", "Toelichting");
                if (reader.Read())
                {
                    table.AddRow(reader["Wid"], reader["Naam"], reader["WetenschappelijkeNaam"], reader["Omschrijving"], reader["aantal"], reader["Soort"], reader["Voorkomen"], reader["geslacht"], reader["zekerheid"], reader["Datum"], reader["Tijd"], reader["toelichting"]);

                }
                table.Write();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();

                }
            }
        }
        public void ApproveObservation(int rowID) // Put chosen observation into GEVALIDEERDEWAARNEMING
        {
            SqlConnection conn = _conn.GetConnection();
            string displayOrganisms = $"SELECT *FROM dbo.WAARNEMING WHERE Wid = {rowID}";
            string deleteOrganisms = $"DELETE FROM dbo.WAARNEMING WHERE Wid = {rowID}"; // DELETES the chosen row 
            string insertIntoValidatedObservation = "INSERT INTO dbo.GEVALIDEERDEWAARNEMING (Omschrijving,Datum,Tijd,WNid,Sid,Lid,toelichting,aantal,geslacht,zekerheid,ManierDelen) VALUES (@Description, @Date,@Time,@WNid,@Sid,@Lid,@Explanation,@Amount,@Sex,@HowSure,@Share)";
            try
            {
                
                conn.Open();
                
                SqlCommand cmd = new SqlCommand(displayOrganisms, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var description = reader["Omschrijving"];
                    var amount = reader["aantal"];
                    var date = reader["Datum"];
                    var time = reader["Tijd"];
                    var explanation = reader["toelichting"];
                    var howSure = reader["zekerheid"];
                    var type = reader["Sid"];
                    var scientific = reader["WNid"];
                    var location = reader["Lid"];
                    var sex = reader["geslacht"];
                    reader.Close();

                    SqlCommand cmd2 = new SqlCommand(insertIntoValidatedObservation, conn);

                    cmd2.Parameters.AddWithValue("@Description", description);
                    cmd2.Parameters.AddWithValue("@Date", date);
                    cmd2.Parameters.AddWithValue("@Time", time);
                    cmd2.Parameters.AddWithValue("@WNid", scientific);
                    cmd2.Parameters.AddWithValue("@Sid", type);
                    cmd2.Parameters.AddWithValue("@Lid", location);
                    cmd2.Parameters.AddWithValue("@Explanation", explanation);
                    cmd2.Parameters.AddWithValue("@HowSure", howSure);
                    cmd2.Parameters.AddWithValue("@Amount", amount);
                    cmd2.Parameters.AddWithValue("@Sex", sex);
                    cmd2.Parameters.AddWithValue("@Share", "no");
                    cmd2.ExecuteNonQuery();

                    SqlCommand cmd3 = new SqlCommand(deleteOrganisms, conn);
                    cmd3.ExecuteNonQuery();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}
