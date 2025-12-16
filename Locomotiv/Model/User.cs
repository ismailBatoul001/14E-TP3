namespace Locomotiv.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Role Role { get; set; }

        public int StationAssigneeId { get; set; }
        public Station StationAssignee { get; set; }


        public ICollection<Inspection> InspectionsEffectuees { get; set; } = new List<Inspection>();
    }
}