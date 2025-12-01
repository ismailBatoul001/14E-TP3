using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class Voie
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public bool EstDisponible { get; set; }

        public int StationId { get; set; }
        public Station Station { get; set; }

        public int TrainActuelId { get; set; }
        public Train  TrainActuel { get; set; }
    }
}
