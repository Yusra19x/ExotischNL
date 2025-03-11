namespace ExotischNLConsoleApp.Models
{
    internal class Observation
    {
        public string Description { get; private set; }
        public DateTime Date { get; private set; }
        public string Time { get; private set; }
        public int Amount { get; private set; }
        public string Sex { get; private set; }
        public string HowSure { get; private set; }
        public string Share { get; private set; }
        public string Explanation { get; private set; }




        public Observation(string description, string sex, int amount,string howSure,string explanation)
        {
            this.Description = description;
            this.Explanation = explanation;
            this.Share = "no"; 
            this.Sex = sex;
            this.Date = DateTime.Now.Date;
            this.Time = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
            this.Amount = amount;
            this.HowSure = howSure;
            
        }
    }
}
