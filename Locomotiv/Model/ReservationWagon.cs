using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Model
{
    public class ReservationWagon
    {
        public int Id { get; set; }
        public int ClientCommercialId { get; set; }
        public User ClientCommercial { get; set; }

        public int ItineraireId { get; set; }
        public Itineraire Itineraire { get; set; }

        public int NombreWagons { get; set; }
        public TypeMarchandise TypeMarchandise { get; set; }
        public double PoidsTotal { get; set; }
        public double TarifTotal { get; set; }
        public DateTime DateReservation { get; set; }
        public StatutReservation Statut { get; set; }
        public string? NotesSpeciales { get; set; }
    }
}
