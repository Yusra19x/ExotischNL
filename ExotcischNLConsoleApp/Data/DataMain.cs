using System.Data.SQLite;
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

        public void CheckIfTableEmpty(string columnName1, string filterOperator, string betweenValue1, string betweenValue2, string columnName2, string sortingType, string tableName)
        {
            SQLiteConnection conn = _conn.GetConnection();
            string query = $"SELECT COUNT(*) FROM {tableName}";
            SQLiteCommand command = new SQLiteCommand(query, conn);
            conn.Open();
            int rowCount = Convert.ToInt32(command.ExecuteScalar());

            if (rowCount == 0)
            {
                Console.WriteLine("De tabel is leeg. Voeg een organisme toe.");
            }
            else
            {
                DisplayAllObservations(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType, tableName);
            }
            conn.Close();
        }

        public void DisplayAllObservations(string columnName1, string filterOperator, string betweenValue1, string betweenValue2, string columnName2, string sortingType, string tableName)
        {
            SQLiteConnection conn = _conn.GetConnection();
            string primaryKey = (tableName == "GEVALIDEERDEWAARNEMING") ? "GWid" : "Wid"; // Choose 1 or 2

            string displayOrganisms = $@"
            SELECT 
            {tableName}.{primaryKey} AS ID, 
            WETENSCHAPPELIJKENAAM.Naam, 
            {tableName}.Omschrijving, 
            {tableName}.Aantal, 
            {tableName}.Datum, 
            {tableName}.Tijd, 
            {tableName}.Toelichting 
            FROM {tableName} 
            INNER JOIN WETENSCHAPPELIJKENAAM ON {tableName}.WNid = WETENSCHAPPELIJKENAAM.WNid
            WHERE {columnName1} {filterOperator} {betweenValue1}
            ORDER BY {columnName2} {sortingType};";

            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(displayOrganisms, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Console.WriteLine("Geen resultaten gevonden.");
                    return;
                }

                var table = new ConsoleTable("ID", "Naam", "Omschrijving", "Aantal", "Datum", "Tijd", "Toelichting");

                while (reader.Read())
                {
                    table.AddRow(reader["ID"], reader["Naam"], reader["Omschrijving"], reader["Aantal"], reader["Datum"], reader["Tijd"], reader["Toelichting"]);
                }

                table.Write();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQL-fout: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Algemene fout: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        public long InsertScientificName(ScientificName scientificName)
        {
            string query = "INSERT INTO WETENSCHAPPELIJKENAAM (Naam, WetenschappelijkeNaam) VALUES (@Name, @ScienceName); SELECT last_insert_rowid();";
            SQLiteConnection conn = _conn.GetConnection();
            long result = -1;

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", scientificName.Name);
                cmd.Parameters.AddWithValue("@ScienceName", scientificName.ScienceName);
                conn.Open();
                cmd.ExecuteNonQuery();
                result = (long)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public long InsertType(OrganismType type)
        {
            string query = "INSERT INTO SOORT (Soort, Voorkomen) VALUES (@Type, @Origin); SELECT last_insert_rowid();";
            SQLiteConnection conn = _conn.GetConnection();
            long result = -1;

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Type", type.OrgType);
                cmd.Parameters.AddWithValue("@Origin", type.Origin);
                conn.Open();
                cmd.ExecuteNonQuery();
                result = (long)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public long InsertLocation(Location location)
        {
            string query = "INSERT INTO LOCATIE (Locatienaam, Provincie, Lengtegraad, Breedtegraad) VALUES (@LocationName, @Province, @Longitude, @Latitude); SELECT last_insert_rowid();";
            SQLiteConnection conn = _conn.GetConnection();
            long result = -1;

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@LocationName", location.LocationName);
                cmd.Parameters.AddWithValue("@Province", location.Province);
                cmd.Parameters.AddWithValue("@Longitude", location.Longitude);
                cmd.Parameters.AddWithValue("@Latitude", location.Latitude);
                conn.Open();
                cmd.ExecuteNonQuery();
                result = (long)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public void InsertObservation(long WNid, long Sid, long Lid, Observation observation)
        {
            string query = "INSERT INTO WAARNEMING (Omschrijving, Datum, Tijd, WNid, Sid, Lid, Toelichting, Aantal, Geslacht, Zekerheid, ManierDelen) VALUES (@Description, @Date, @Time, @WNid, @Sid, @Lid, @Explanation, @Amount, @Sex, @HowSure, @Share)";
            SQLiteConnection conn = _conn.GetConnection();

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
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
                cmd.Parameters.AddWithValue("@Share", "no");

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }
            public void DisplayForAction()
        {
            string query = "SELECT WAARNEMING.*, WETENSCHAPPELIJKENAAM.*, SOORT.*, LOCATIE.* " +
                           "FROM WAARNEMING " +
                           "INNER JOIN WETENSCHAPPELIJKENAAM ON WAARNEMING.WNid = WETENSCHAPPELIJKENAAM.WNid " +
                           "INNER JOIN SOORT ON WAARNEMING.Sid = SOORT.Sid " +
                           "INNER JOIN LOCATIE ON WAARNEMING.Lid = LOCATIE.Lid";

            SQLiteConnection conn = _conn.GetConnection();

            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                var table = new ConsoleTable("Wid", "Naam", "Wetenschappelijke naam", "Omschrijving", "Aantal", "Soort", "Voorkomen", "Geslacht", "Zekerheid", "Datum", "Tijd", "Toelichting");

                while (reader.Read())
                {
                    table.AddRow(reader["Wid"], reader["Naam"], reader["WetenschappelijkeNaam"], reader["Omschrijving"],
                                 reader["aantal"], reader["Soort"], reader["Voorkomen"], reader["geslacht"],
                                 reader["zekerheid"], reader["Datum"], reader["Tijd"], reader["toelichting"]);
                }

                table.Write();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij ophalen van gegevens: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        public void DisplayForConfirmation(int rowID)
        {
            string query = "SELECT WAARNEMING.*, WETENSCHAPPELIJKENAAM.*, SOORT.*, LOCATIE.* " +
                           "FROM WAARNEMING " +
                           "INNER JOIN WETENSCHAPPELIJKENAAM ON WAARNEMING.WNid = WETENSCHAPPELIJKENAAM.WNid " +
                           "INNER JOIN SOORT ON WAARNEMING.Sid = SOORT.Sid " +
                           "INNER JOIN LOCATIE ON WAARNEMING.Lid = LOCATIE.Lid " +
                           "WHERE WAARNEMING.Wid = @RowID";

            SQLiteConnection conn = _conn.GetConnection();

            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@RowID", rowID);
                SQLiteDataReader reader = cmd.ExecuteReader();
                var table = new ConsoleTable("Wid", "Naam", "Wetenschappelijke naam", "Omschrijving", "Aantal", "Soort", "Voorkomen", "Geslacht", "Zekerheid", "Datum", "Tijd", "Toelichting");

                if (reader.Read())
                {
                    table.AddRow(reader["Wid"], reader["Naam"], reader["WetenschappelijkeNaam"], reader["Omschrijving"],
                                 reader["aantal"], reader["Soort"], reader["Voorkomen"], reader["geslacht"],
                                 reader["zekerheid"], reader["Datum"], reader["Tijd"], reader["toelichting"]);
                }

                table.Write();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij ophalen van gegevens: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }


        public void ApproveObservation(int rowID)
        {
            string selectQuery = "SELECT * FROM WAARNEMING WHERE Wid = @RowID";
            string insertQuery = "INSERT INTO GEVALIDEERDEWAARNEMING (Omschrijving, Datum, Tijd, WNid, Sid, Lid, toelichting, aantal, geslacht, zekerheid, ManierDelen) " +
                                 "VALUES (@Description, @Date, @Time, @WNid, @Sid, @Lid, @Explanation, @Amount, @Sex, @HowSure, @Share)";
            string deleteQuery = "DELETE FROM WAARNEMING WHERE Wid = @RowID";


            try
            {
                SQLiteConnection conn_read = _conn.GetConnection();
                conn_read.Open();
                SQLiteCommand selectCmd = new SQLiteCommand(selectQuery, conn_read);
                selectCmd.Parameters.AddWithValue("@RowID", rowID);
                SQLiteDataReader reader = selectCmd.ExecuteReader();


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
                    conn_read.Close();

                    SQLiteConnection conn_write = _conn.GetConnection();
                    conn_write.Open();
                    SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, conn_write);
                    insertCmd.Parameters.AddWithValue("@Description", description);
                    insertCmd.Parameters.AddWithValue("@Date", date);
                    insertCmd.Parameters.AddWithValue("@Time", time);
                    insertCmd.Parameters.AddWithValue("@WNid", scientific);
                    insertCmd.Parameters.AddWithValue("@Sid", type);
                    insertCmd.Parameters.AddWithValue("@Lid", location);
                    insertCmd.Parameters.AddWithValue("@Explanation", explanation);
                    insertCmd.Parameters.AddWithValue("@HowSure", howSure);
                    insertCmd.Parameters.AddWithValue("@Amount", amount);
                    insertCmd.Parameters.AddWithValue("@Sex", sex);
                    insertCmd.Parameters.AddWithValue("@Share", "no");

                    insertCmd.ExecuteNonQuery();
                    conn_write.Close();

                    SQLiteConnection conn_delete = _conn.GetConnection();
                    conn_delete.Open();
                    SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, conn_delete);
                    deleteCmd.Parameters.AddWithValue("@RowID", rowID);
                    deleteCmd.ExecuteNonQuery();
                    conn_delete.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij goedkeuren waarneming: {ex.Message}");
            }
            finally
            {

            }
        }

        public void UpdateObservation(int rowID, string columnName, string newValue, string tableName)
        {
            string primaryKey = (tableName == "GEVALIDEERDEWAARNEMING") ? "GWid" : "Wid"; // Kies juiste ID-kolom

            string updateQuery = $"UPDATE {tableName} SET {columnName} = @NewValue WHERE {primaryKey} = @RowID";

            SQLiteConnection conn = _conn.GetConnection();

            try
            {
                conn.Open();

                // Controleer of de waarneming bestaat in de juiste tabel
                string checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {primaryKey} = @RowID";
                SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@RowID", rowID);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    Console.WriteLine($"Waarneming met ID {rowID} bestaat niet in {tableName}.");
                    return;
                }

                // Voer de update uit
                SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@NewValue", newValue);
                updateCmd.Parameters.AddWithValue("@RowID", rowID);
                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Waarneming met ID {rowID} in {tableName} is succesvol bijgewerkt.");
                }
                else
                {
                    Console.WriteLine("Er is iets misgegaan, waarneming is niet bijgewerkt.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij bijwerken van waarneming: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        public void DeleteObservation(int rowID, string tableName)
        {
            string primaryKey = (tableName == "GEVALIDEERDEWAARNEMING") ? "GWid" : "Wid"; // Choose 1 or 2

            string deleteQuery = $"DELETE FROM {tableName} WHERE {primaryKey} = @RowID";

            SQLiteConnection conn = _conn.GetConnection();

            try
            {
                conn.Open();

                // Looking if waarneming is in it
                string checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {primaryKey} = @RowID";
                SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@RowID", rowID);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    Console.WriteLine($"Waarneming met ID {rowID} bestaat niet in {tableName}.");
                    return;
                }

                // Removing
                SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@RowID", rowID);
                int rowsAffected = deleteCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Waarneming met ID {rowID} is succesvol verwijderd uit {tableName}.");
                }
                else
                {
                    Console.WriteLine("Er is iets misgegaan, waarneming is niet verwijderd.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij verwijderen van waarneming: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}

