using ExotischNLConsoleApp.Business;

namespace ExotischNLConsoleApp.Presentation
{
    internal class PresentationMain
    {
        private BusinessMain _businessMain;
        public PresentationMain()
        {
            // Geeft presentationlayer toegang tot de INTERNAL klasse BusinessMain.cs
            _businessMain = new BusinessMain();
            string question;
            int intInput;
            bool check = true;
            Console.WriteLine("Welkom bij Exotisch Nederland.");
            while (check) // What is the action you want to perform
            {
                
                question = "Wat wilt u doen? (voer een getal in)\n 1. Waarneming registreren.\n 2. Waarneming weegeven.\n 3. Waarneming bewerken (werkt niet).\n 4. Waarneming verwijderen (werkt niet).\n 5. Waarneming goedkeuren.\n 6. Programma afsluiten"; // The question you want to ask
                do
                {
                    string checkResult = InputEmptyCheck(question); // Checks if the input from the user is empty
                    intInput = (int)InputTypeIntCheck(checkResult); // Checks if the input from the user is an int
                    if (intInput < 0 || intInput > 6)
                    {
                        Console.WriteLine("Voer een getal tussen in waaruit u kunt kiezen.");
                    }
                } while (intInput < 0 || intInput > 6);
                switch (intInput) // Switch case for the chosen action
                {
                    case 1:
                        List<string> outWaarnemingRegistreren = WaarnemingRegistreren();
                        _businessMain.CheckInputRegistration(outWaarnemingRegistreren);
                        break;
                    case 2:
                        var chooseTable = WhatTable(); // Choose if you want to use the WAARNEMING or GEVALIDEERDEWAARNEMING table.
                        string tableName = chooseTable.Item1;
                        string tableIDName = chooseTable.Item2;
                        ChoiceFilterObservations(tableName, tableIDName);
                        break;
                    case 3:
                        // Not implemented yet
                        break;
                    case 4:
                        // Not implemented yet
                        break;
                    case 5:
                        ApproveObservation();
                        break;
                    case 6:
                        Environment.Exit(0);
                        check = false;
                        break;
                }
            }


        }

        private string InputEmptyCheck(string prompt) // check if input is empty.
        {
            string input;
            do // keeps asking for an input until the user gives atleast something
            {
                Console.Write($"{prompt}\n");
                input = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(input));
            return input;
        }

        private object InputTypeIntCheck(string input) // check if input is an int.
        {
            bool isValid;
            int number;
            do // Keeps asking for an input until the user puts an int in.
            {
                isValid = int.TryParse(input, out number);
                if (!isValid)
                {
                    Console.WriteLine("Voer een heel getal in.");
                    input = Console.ReadLine();
                }
            } while (!isValid);
            return number;
        }

        private List<string> WaarnemingRegistreren() // Code for Registering an observation
        {
            string question;
            List<string> answer = new List<string>();
            question = "Wat is de naam van het organisme?";
            answer.Add(InputEmptyCheck(question));
            question = "Wat is de soort van het organisme?";
            answer.Add(InputEmptyCheck(question));
            question = "Wat is de oorsprong van het organisme? ([i]nheems of [e]xoot)";
            answer.Add(InputEmptyCheck(question));
            question = "Wat is de wetenschappelijke naam van het organisme?";
            answer.Add(InputEmptyCheck(question));
            question = "Wat is de geslacht van het organisme? ([m]an of [v]rouw";
            answer.Add(InputEmptyCheck(question));
            question = "Wat is de beschrijving van het organisme?";
            answer.Add(InputEmptyCheck(question));
            question = "Hoeveel organismen van hetzelfde soort zag je? (Voer een getal in)";
            string checkResult = InputEmptyCheck(question);
            answer.Add(InputTypeIntCheck(checkResult).ToString());
            question = "Wat is de zekerheid van je observatie? (voer: niet zeker (nz) of heel zeker (hz) in.)";
            answer.Add(InputEmptyCheck(question));
            question = "Zijn er nog dingen dat je kwijt wilt over de observatie? (vul 'nee' in als je niks toe wilt voegen.)";
            answer.Add(InputEmptyCheck(question));
            question = "Wat is de locatienaam van je waarneming? (mag leeg zijn)";
            answer.Add(question);
            question = "In welk provincie heb je de waarneming gedaan? (mag leeg zijn)";
            answer.Add(question);
            return answer;
        }

        private (string,string) WhatTable() // Asks what table you want to use (WAANREMING or GEVALIDEERDEWAARNEMING)
        {
            int intInput;
            string question;
            string tableName;
            string tableIDName;
            do
            {
                question = "Welke tabel wil je inzien? (voer een getal in)\n 1. Gevalideerde waarnemingen.\n 2. Niet Gevalideerde waarnemingen.";
                string checkResult = InputEmptyCheck(question);
                intInput = (int)InputTypeIntCheck(checkResult);
            } while (intInput < 0 || intInput > 2);
            switch (intInput)
            {
                case 1:
                    tableName = "dbo.GEVALIDEERDEWAARNEMING";
                    tableIDName = "GWid";
                    return (tableName, tableIDName);
                case 2:
                    tableName = "dbo.WAARNEMING";
                    tableIDName = "Wid";
                    return(tableName,tableIDName);
                default:
                    Console.WriteLine("Kies 1 of 2");
                    break;
            }
            return (null, null);
        }

        private void ChoiceFilterObservations(string tableName, string tableIDName) // Gives choices for the filter
        {
            int intInput;
            string question;

            do
            {
                do
                {
                    question = "Waarop wil je de tabel filteren? (voer een getal in)\n 1. Geen.\n 2. Naam.\n 3. Wetenschappelijke naam.\n 4. Soort.\n 5. Aantal.\n 6. Voorkomen.\n 7. Zekerheid.\n 8. Geslacht.\n 9. Provincie.\n 10. Locatienaam.";
                    string checkResult = InputEmptyCheck(question);
                    intInput = (int)InputTypeIntCheck(checkResult);
                } while (intInput < 0 || intInput > 10);
                string columnName;
                string filterOperator = null;
                string value1 = null;
                string betweenValue2 = null;
                if (intInput >= 2 && intInput < 11)
                {
                    filterOperator = ChoiceFilterOperator(intInput);
                    if (filterOperator == "BETWEEN")
                    {
                        value1 = ChooseBetweenValues().Item1;
                        betweenValue2 = ChooseBetweenValues().Item2;
                    }
                    else
                    {
                        switch(intInput)
                        {
                            case 6:
                                Console.WriteLine("Voer [i]nheems of [e]xoot in");
                                question = "Wat is de waarde?";
                                value1 = InputEmptyCheck(question).ToLower();
                                value1 = _businessMain.CheckValueFilter(intInput, value1);
                                break;
                            case 7:
                                Console.WriteLine("Voer heel zeker (hz) of niet zeker (nz) in");
                                question = "Wat is de waarde?";
                                value1 = InputEmptyCheck(question).ToLower();
                                value1 = _businessMain.CheckValueFilter(intInput, value1);
                                break;
                            case 8:
                                Console.WriteLine("Voer [m]an of [v]rouw in");
                                question = "Wat is de waarde?";
                                value1 = InputEmptyCheck(question).ToLower();
                                value1 = _businessMain.CheckValueFilter(intInput, value1);
                                break;
                            default:
                                question = "Wat is de waarde?";
                                value1 = InputEmptyCheck(question);
                                break;
                        }
                        
                    }
                }
                
                
                if ((intInput != 5 && intInput != 1) && int.TryParse(value1,out int intValue1)==false) // If the value is not an int then put the value between ''
                {

                    value1 = $"'{value1.ToLower()}'";
                }
                switch (intInput)
                {
                    case 1:
                        columnName = "1=1";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 2:
                        columnName = "dbo.WETENSCHAPPELIJKENAAM.Naam";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 3:
                        columnName = "dbo.WETENSCHAPPELIJKENAAM.WetenschappelijkeNaam";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 4:
                        columnName = "dbo.SOORT.Soort";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 5:
                        columnName = $"{tableName}.aantal";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 6:
                        columnName = "dbo.SOORT.Voorkomen";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 7:
                        columnName = $"{tableName}.zekerheid";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 8:
                        columnName = $"{tableName}.geslacht";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 9:
                        columnName = "dbo.LOCATIE.provincie";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    case 10:
                        columnName = "dbo.LOCATIE.Locatienaam";
                        ChoiceSortObservations(columnName, filterOperator, value1, betweenValue2, tableName,tableIDName);
                        break;
                    default:
                        Console.WriteLine("Voer een getal tussen in waaruit u kunt kiezen.");
                        break;
                }
            } while (intInput < 0 || intInput > 10);
        }

        private string ChoiceFilterOperator(int Choicefilter) // Choose the filter operator
        {
            int intInput;
            string question;

            if (Choicefilter == 6 || Choicefilter == 7|| Choicefilter == 8)
            {
                return "=";
            }
            else
            {
                do
                {
                    question = "Hoe wil je filteren? (voer een getal in)\n 1. =.\n 2. Groter dan.\n 3. Kleiner dan.\n 4. Groter of hetzelfde als.\n 5. Kleiner of hetzelfde als.\n 6. Tussen.";
                    string checkResult = InputEmptyCheck(question);
                    intInput = (int)InputTypeIntCheck(checkResult);
                    switch (intInput)
                    {
                        case 1:
                            return "=";
                        case 2:
                            return ">";
                        case 3:
                            return "<";
                        case 4:
                            return ">=";
                        case 5:
                            return "<=";
                        case 6:
                            return "BETWEEN";
                        default:
                            Console.WriteLine("Voer een getal tussen in waaruit u kunt kiezen.");
                            break;
                    }
                } while (intInput < 0 || intInput > 6);
            }
            
            return "error";
        }

        private (string,string) ChooseBetweenValues() // Choose 2 values you want to filter between
        {
            string question;
            question = "Wat is de eerste waarde?";
            string checkResult1 = InputEmptyCheck(question);
            question = "Wat is de tweede waarde?";
            string checkResult2 = InputEmptyCheck(question);
            return (checkResult1, checkResult2);
        }

        private void ChoiceSortObservations(string columnName1, string filterOperator, string betweenValue1, string betweenValue2, string tableName,string tableIDName) // Choose the way you want to SORT the chosen table
        {
            int intInput;
            string question;

            do
            {
                question = "Waarop wil je de tabel sorteren? (voer een getal in)\n 1. Niet sorteren (Is automatisch Wid).\n 2. Sorteren op naam.\n 3. Sorteren op wetenschappelijke naam.\n 4. Sorteren op datum.\n 5. Sorteren op soort.\n 6. Sorteren op aantal.";
                string checkResult = InputEmptyCheck(question);
                intInput = (int)InputTypeIntCheck(checkResult);
                string sortingType = ChoiceSortAscDesc();
                string columnName2;

                switch (intInput)
                {
                    case 1:
                        
                        columnName2 = $"{tableName}.{tableIDName}";
                        _businessMain.DisplayAllObservations(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType, tableName);
                        
                        break;
                    case 2:
                        columnName2 = "dbo.WETENSCHAPPELIJKENAAM.Naam";
                        _businessMain.DisplayAllObservations(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType,tableName);
                        break;
                    case 3:
                        columnName2 = "dbo.WETENSCHAPPELIJKENAAM.WetenschappelijkeNaam";
                        _businessMain.DisplayAllObservations(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType,tableName);
                        break;
                    case 4:
                        columnName2 = $"{tableName}.Datum";
                        _businessMain.DisplayAllObservations(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType,tableName);
                        break;
                    case 5:
                        columnName2 = "dbo.SOORT.Soort";
                        _businessMain.DisplayAllObservations(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType,tableName);
                        break;
                    case 6:
                        columnName2 = $"{tableName}.aantal";
                        _businessMain.DisplayAllObservations(columnName1, filterOperator, betweenValue1, betweenValue2, columnName2, sortingType,tableName);
                        break;
                    default:
                        Console.WriteLine("Voer een getal tussen in waaruit u kunt kiezen.");
                        break;
                }
            } while (intInput< 0 || intInput> 6);
        }

        private string ChoiceSortAscDesc() // Choose whether you want to SORT ASCENDING or DESCENDING
        {
            int intInput;
            string question;

            do
            {
                question = "Wil je oplopend of aflopend sorteren? (voer een getal in)\n 1. Oplopend.\n 2. Aflopend.";
                string checkResult = InputEmptyCheck(question);
                intInput = (int)InputTypeIntCheck(checkResult);
                Console.Clear();
                switch (intInput)
                {
                    case 1:
                        return "ASC";
                    case 2:
                        return "DESC";
                    default:
                        Console.WriteLine("Voer een getal tussen in waaruit u kunt kiezen.");
                        break;
                }
            } while (intInput < 0 || intInput > 2);
            return "error";
        }

        private void ApproveObservation() // Choose which observation you want to approve of
        {
            string question;
            // select query zodat ik de ids kan krijgen van alle records
            _businessMain.DisplayForAction();
            // bevestiging van keuze
            question = "Welke waarneming wil je goedkeuren? (Voer de ID (getal) in)";
            string strRowID = InputEmptyCheck(question);
            int rowID = (int)InputTypeIntCheck(strRowID);
            // input gebruiker om observatie goed te keuren
            _businessMain.ConfirmationAppove(rowID);
            question = $"Weet je zeker dat je waarneming {rowID} goed wilt keuren. (j/n)";
            string vraagJaNee = InputEmptyCheck(question);
            if(vraagJaNee == "j" || vraagJaNee == "ja")
            {
                _businessMain.ApproveObservation(rowID);
            }
            
        }
    }
}
