using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Locomotiv.Model
{
    public class Signal
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public EtatSignal Etat { get; set; }

        public int StationId { get; set; }
        public Station Station { get; set; }

        public int BlockId { get; set; }
        public Block Block { get; set; }
    }
}
