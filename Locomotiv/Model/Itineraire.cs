using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class Itineraire
    {
        public int Id { get; set; }
        public DateTime DateCreation { get; set; }
        public bool EstActif { get; set; }

        public int TrainId { get; set; }
        public Train Train { get; set; }

        public int StationDepartId { get; set; }
        public Station StationDepart { get; set; }

        public int StationArriveeId { get; set; }
        public Station StationArrivee { get; set; }

        public ICollection<ItineraireArret> Arrets { get; set; } = new List<ItineraireArret>();


    }
}
