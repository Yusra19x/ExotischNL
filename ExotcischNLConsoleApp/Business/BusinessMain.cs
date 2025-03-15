using ExotischNLConsoleApp.Data;
using ExotischNLConsoleApp.Models;


namespace ExotischNLConsoleApp.Business
{

    internal class BusinessMain
    {
        public Organisme organism;
        public Observation observation;
        public OrganismType organismType;
        public ScientificName scientificName;
        public Location location;
        private DataMain _dataMain;
        private int observationAmount;
        public BusinessMain()

        {
            _dataMain = new DataMain();
        }

        public void CheckInputRegistration(List<string> outWaarnemingRegistreren) // Checks if the given values match the ones we want in the database
        {
            bool checkOorsprong = true;
            bool checkGeslacht = true;
            bool checkZekerheid = true;
            do
            {
                if ((outWaarnemingRegistreren[2].ToLower() == "i" || outWaarnemingRegistreren[2].ToLower() == "inheems") || (outWaarnemingRegistreren[2].ToLower() == "e" || outWaarnemingRegistreren[2].ToLower() == "exoot"))
                {
                    checkOorsprong = false;
                    if (outWaarnemingRegistreren[2].ToLower() == "i" || outWaarnemingRegistreren[2].ToLower() == "inheems")
                    {
                        outWaarnemingRegistreren[2] = "inheems";
                    }
                    else
                    {
                        outWaarnemingRegistreren[2] = "exoot";
                    }
                }
                else
                {
                    Console.WriteLine("U heeft de oorsprong fout ingevoerd.");
                    Console.WriteLine("Voer inheems, exoot, i of e in.");
                    string organismOrigin = Console.ReadLine();
                    outWaarnemingRegistreren[2] = organismOrigin;
                }

                if (outWaarnemingRegistreren[4].ToLower() == "m" || outWaarnemingRegistreren[4].ToLower() == "man" || outWaarnemingRegistreren[4].ToLower() == "v" || outWaarnemingRegistreren[4].ToLower() == "vrouw")
                {
                    checkGeslacht = false;
                    if(outWaarnemingRegistreren[4].ToLower() == "m" || outWaarnemingRegistreren[4].ToLower() == "man")
                    {
                        outWaarnemingRegistreren[4] = "man";
                    }
                    else
                    {
                        outWaarnemingRegistreren[4] = "vrouw";
                    }
                }
                else
                {
                    Console.WriteLine("U heeft de geslacht fout ingevoerd.");
                    Console.WriteLine("Voer man, vrouw, m of v in.");
                    string organismSex = Console.ReadLine();
                    outWaarnemingRegistreren[4] = organismSex;

                }

                if ((outWaarnemingRegistreren[7].ToLower() == "nz" || outWaarnemingRegistreren[7].ToLower() == "niet zeker") || (outWaarnemingRegistreren[7].ToLower() == "hz" || outWaarnemingRegistreren[7].ToLower() == "heel zeker"))
                {
                    checkZekerheid = false;
                    if (outWaarnemingRegistreren[7].ToLower() == "nz" || outWaarnemingRegistreren[7].ToLower() == "niet zeker")
                    {
                        outWaarnemingRegistreren[7] = "niet zeker";
                    }
                    else
                    {
                        outWaarnemingRegistreren[7] = "heel zeker";
                    }
                }
                else
                {
                    Console.WriteLine("U heeft de zekerheid fout in gevuld.");
                    Console.WriteLine("Voer niet zeker, heel zeker, nz of hz in.");
                    string organismZekerheid = Console.ReadLine();
                    outWaarnemingRegistreren[7] = organismZekerheid;

                }
                
                if (outWaarnemingRegistreren[8].ToLower() == "nee")
                {
                    outWaarnemingRegistreren[8] = "";
                }
                observationAmount = int.Parse(outWaarnemingRegistreren[6]);


            } while (checkOorsprong || checkGeslacht|| checkZekerheid);
            // Puts the given data into classes for easy access
            InputOrganisms(outWaarnemingRegistreren);
            InputObservations(outWaarnemingRegistreren, observationAmount);
            InputScientificName();
            InputOrganismType();;
            InputLocation(outWaarnemingRegistreren);
            // INSERT the given data into the database
            long Lid = (long)_dataMain.InsertLocation(location);
            long Sid = (long)_dataMain.InsertType(organismType);
            long WNid = (long)_dataMain.InsertScientificName(scientificName);
            _dataMain.InsertObservation(WNid, Sid, Lid, observation);
        }

        public void InputOrganisms(List<string> outWaarnemingRegistreren) // Make object Organisme
        {
            organism = new Organisme(outWaarnemingRegistreren[0], outWaarnemingRegistreren[1], outWaarnemingRegistreren[2], outWaarnemingRegistreren[3], outWaarnemingRegistreren[4], outWaarnemingRegistreren[5]);
        }

        public void InputObservations(List<string> outWaarnemingRegistreren, int observationAmount) // Make object Observation
        {
            observation = new Observation(organism.Description, organism.Sex, observationAmount, outWaarnemingRegistreren[7], outWaarnemingRegistreren[8]);
            
        }

        public void InputScientificName() // Make object ScientificName
        {
            scientificName = new ScientificName(organism.Name, organism.ScienceName);
            
        }

        public void InputOrganismType() // Make object OrganismType
        {
            organismType = new OrganismType(organism.Type, organism.Origin);
            
        }

        public void InputLocation(List<string> outWaarnemingRegistreren)// Make object Location
        {
            location = new Location(outWaarnemingRegistreren[9], outWaarnemingRegistreren[10]);

        }

        public void DisplayAllObservations(string columnName1, string filterOperator, string betweenValue1, string betweenValue2, string columnName2, string sortingType, string tableName) // Display all observations with or without filters
        {
            _dataMain.CheckIfTableEmpty(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType,tableName);
        }
        
        public void DisplayForAction() // Display all observations in WAARNEMING so you can choose what observation to approve
        {
            _dataMain.DisplayForAction();
        }

        public void ConfirmationAppove(int rowID) // Confirm your choice
        {
            _dataMain.DisplayForConfirmation(rowID);
        }

        public void ApproveObservation(int rowID) // Go to data layer to put the chosen observation in GEVALIDEERDEWAARNEMING
        {
            _dataMain.ApproveObservation(rowID);
        }

        public void UpdateObservation()
        {
            Console.WriteLine("Welke waarneming wil je bewerken? (Kies een tabel)");
            Console.WriteLine("1. Gevalideerde waarnemingen\n2. Niet-gevalideerde waarnemingen");

            string tableChoice = Console.ReadLine();
            string tableName;
            string primaryKey;

            if (tableChoice == "1")
            {
                tableName = "GEVALIDEERDEWAARNEMING";
                primaryKey = "GWid";
            }
            else if (tableChoice == "2")
            {
                tableName = "WAARNEMING";
                primaryKey = "Wid";
            }
            else
            {
                Console.WriteLine("Ongeldige keuze.");
                return;
            }

            // Toon de waarnemingen uit de juiste tabel
            _dataMain.DisplayAllObservations("1=1", "", "", "", $"{tableName}.{primaryKey}", "ASC", tableName);

            Console.Write("Voer de ID in van de waarneming die je wilt bewerken: ");
            string inputID = Console.ReadLine();

            if (!int.TryParse(inputID, out int rowID))
            {
                Console.WriteLine("Ongeldige invoer. Voer een geldig nummer in.");
                return;
            }

            Console.WriteLine("Welke eigenschap wil je bewerken? (Kies een nummer)");
            Console.WriteLine("1. Omschrijving\n2. Aantal\n3. Datum\n4. Tijd\n5. Toelichting");

            string columnName = "";
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    columnName = "Omschrijving";
                    break;
                case "2":
                    columnName = "Aantal";
                    break;
                case "3":
                    columnName = "Datum";
                    break;
                case "4":
                    columnName = "Tijd";
                    break;
                case "5":
                    columnName = "Toelichting";
                    break;
                default:
                    Console.WriteLine("Ongeldige keuze.");
                    return;
            }

            Console.Write($"Voer de nieuwe waarde in voor {columnName}: ");
            string newValue = Console.ReadLine();

            _dataMain.UpdateObservation(rowID, columnName, newValue, tableName);
        }

        public void DeleteObservation()
        {
            Console.WriteLine("Uit welke tabel wil je een waarneming verwijderen?");
            Console.WriteLine("1. Gevalideerde waarnemingen\n2. Niet-gevalideerde waarnemingen");

            string tableChoice = Console.ReadLine();
            string tableName;
            string primaryKey;

            if (tableChoice == "1")
            {
                tableName = "GEVALIDEERDEWAARNEMING";
                primaryKey = "GWid";
            }
            else if (tableChoice == "2")
            {
                tableName = "WAARNEMING";
                primaryKey = "Wid";
            }
            else
            {
                Console.WriteLine("Ongeldige keuze.");
                return;
            }

            _dataMain.DisplayAllObservations("1=1", "", "", "", $"{tableName}.{primaryKey}", "ASC", tableName);

            Console.Write("Voer de ID in van de waarneming die je wilt verwijderen: ");
            string inputID = Console.ReadLine();

            if (!int.TryParse(inputID, out int rowID))
            {
                Console.WriteLine("Ongeldige invoer. Voer een geldig nummer in.");
                return;
            }

            Console.Write($"Weet je zeker dat je waarneming met ID {rowID} uit {tableName} wilt verwijderen? (j/n): ");
            string confirm = Console.ReadLine().ToLower();

            if (confirm == "j" || confirm == "ja")
            {
                _dataMain.DeleteObservation(rowID, tableName);
            }
            else
            {
                Console.WriteLine("Verwijderen geannuleerd.");
            }
        }

        public string CheckValueFilter(int intInput, string filterValue)
        {
            switch(intInput)
            {
                case 6:
                    if (filterValue != "e" || filterValue != "i" || filterValue != "exoot" || filterValue != "inheems")
                    {
                        while (filterValue != "e" || filterValue != "i" || filterValue != "exoot" || filterValue != "inheems")
                        {
                            Console.WriteLine("Voer e, i, exoot of inheems in.");
                            filterValue = Console.ReadLine();
                        }
                    }
                    else if (filterValue == "e")
                    {
                        filterValue = "exoot";
                    }
                    else if (filterValue == "i")
                    {
                        filterValue = "inheems";
                    }
                    return filterValue;
                case 7:
                    
                    if (filterValue == "hz" || filterValue == "nz" || filterValue == "heel zeker" || filterValue == "niet zeker")
                    {
                        if (filterValue == "hz")
                        {
                            filterValue = "heel zeker";
                        }
                        else if (filterValue == "nz")
                        {
                            filterValue = "niet zeker";
                        }
                    }
                    else
                    {
                        while (filterValue != "hz" || filterValue != "nz" || filterValue != "heel zeker" || filterValue != "niet zeker")
                        {
                            Console.WriteLine("Voer hz, nz, heel zeker of niet zeker in.");
                            filterValue = Console.ReadLine();
                        }
                    }
                    return filterValue;
                case 8:
                    if (filterValue != "m" || filterValue != "v" || filterValue != "man" || filterValue != "vrouw")
                    {
                        while (filterValue != "m" || filterValue != "v" || filterValue != "man" || filterValue != "vrouw")
                        {
                            Console.WriteLine("Voer m, v, man of vrouw in.");
                            filterValue = Console.ReadLine();
                        }
                    }
                    else if (filterValue == "m")
                    {
                        filterValue = "man";
                    }
                    else if (filterValue == "v")
                    {
                        filterValue = "vrouw";
                    }
                    return filterValue;
            }
            return null;
        }
    }
}
