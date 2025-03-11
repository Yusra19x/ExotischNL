namespace ExotischNLConsoleApp.Models
{
    
    internal class OrganismType
    {
        public string OrgType { get; private set; }
        public string Origin { get; private set; }
        public OrganismType(string orgType, string origin)
        {
            this.OrgType = orgType;
            this.Origin = origin;
        }
    }
}
