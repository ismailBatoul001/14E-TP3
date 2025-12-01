using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class ItineraireArret
    {
        public int Id { get; set; }
        public int Ordre { get; set; }
        public DateTime HeureArrivee { get; set; }
        public DateTime HeureDepart { get; set; }
        public bool EstStation { get; set; }

        public int ItineraireId { get; set; }
        public Itineraire Itineraire { get; set; }

        public int? StationId { get; set; }
        public Station Station { get; set; }

        public int? PointInteretId { get; set; }
        public PointInteret PointInteret { get; set; }
    }
}
