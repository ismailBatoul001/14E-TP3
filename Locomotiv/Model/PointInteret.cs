using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model
{
    public class PointInteret
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Type { get; set; } 
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
    }
}
