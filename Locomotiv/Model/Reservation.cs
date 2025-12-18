using System;

namespace Locomotiv.Model
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime DateReservation { get; set; }
        public bool EstActif { get; set; }
        public StatutReservation Statut { get; set; }
        public int NombrePassagers { get; set; }
        public decimal MontantTotal { get; set; }

        public int ItineraireId { get; set; }
        public Itineraire Itineraire { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string NumeroBillet { get; set; }
        public DateTime? DateAnnulation { get; set; }
    }
}
