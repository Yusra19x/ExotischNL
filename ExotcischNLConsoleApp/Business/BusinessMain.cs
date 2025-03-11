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
            decimal Lid = _dataMain.InsertLocation(location);
            decimal Sid = _dataMain.InsertType(organismType);
            decimal WNid = _dataMain.InsertScientificName(scientificName);
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
