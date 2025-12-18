using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Model
{
    public class Inspection
    {
        public int Id { get; set; }

        public int TrainId { get; set; }
        public Train? Train { get; set; }

        public DateTime DateInspection { get; set; } 

        public TypeInspection TypeInspection { get; set; }
        public ResultatInspection Resultat { get; set; }

        public string Observations { get; set; } = string.Empty;

        public int? MecanicienId { get; set; }
        public User? Mecanicien { get; set; }
    }
}
