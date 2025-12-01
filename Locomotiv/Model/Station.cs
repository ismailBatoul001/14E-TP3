using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Locomotiv.Model
{
    public class Station
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CapaciteMaximale { get; set; }

        public ICollection<Voie> Voies { get; set; } = new List<Voie>();
        public ICollection<Signal> Signaux { get; set; } = new List<Signal>();
        public ICollection<Train> TrainsEnGare { get; set; } = new List<Train>();
        public ICollection<User> EmployesAssignes { get; set; } = new List<User>();
    }
}
