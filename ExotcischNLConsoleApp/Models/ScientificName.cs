
namespace ExotischNLConsoleApp.Models
{
    
    internal class ScientificName
    {
        public string ScienceName { get; private set; }
        public string Name { get; private set; }
        public ScientificName(string name, string scienceName)
        {
            this.ScienceName = scienceName;
            this.Name = name;
        }
    }
}
