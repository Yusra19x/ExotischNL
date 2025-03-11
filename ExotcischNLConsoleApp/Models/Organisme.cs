namespace ExotischNLConsoleApp.Models
{
    internal class Organisme
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Origin { get; private set; }
        public string ScienceName { get; private set; }
        public string Sex { get; private set; }
        public string Description { get; private set; }
        public Organisme(string name, string type, string origin, string scienceName, string sex, string description)
        {
            this.Name = name;
            this.Type = type;
            this.Origin = origin;
            this.ScienceName = scienceName;
            this.Sex = sex;
            this.Description = description;
        }

        
    }
}
