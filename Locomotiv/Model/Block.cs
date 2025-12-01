using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class Block
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public double LatitudeDebut { get; set; }
        public double LongitudeDebut { get; set; }
        public double LatitudeFin { get; set; }
        public double LongitudeFin { get; set; }
        public Train? TrainActuel { get; set; }
        public bool EstOccupe { get; set; }
        public int? TrainActuelId { get; set; }
        public ICollection<Signal>? Signaux { get; set; } = new List<Signal>();
        public ICollection<Block>? BlocksAdjacents { get; set; } = new List<Block>();

    }
}
