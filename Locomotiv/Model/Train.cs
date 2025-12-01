using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Locomotiv.Model
{
    public class Train
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public TypeTrain Type { get; set; }
        public EtatTrain Etat { get; set; }
        public int Capacite { get; set; }

        public int? StationActuelleId { get; set; }
        public Station? StationActuelle { get; set; }

        public int? VoieActuelleId { get; set; }
        public Voie? VoieActuelle { get; set; }

        public int? BlockActuelId { get; set; }
        public Block? BlockActuel { get; set; }
    }

}
