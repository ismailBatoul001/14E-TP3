using System;
using System.Collections.Generic;
using Locomotiv.Model;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IReservationService
    {
        Reservation CreerReservation(int itineraireId, int userId, int nombrePassagers);
        void AnnulerReservation(int reservationId);
        void ConfirmerReservation(int reservationId);
        Reservation GetById(int id);
        IEnumerable<Reservation> GetReservationsParUtilisateur(int userId);
        IEnumerable<Reservation> GetReservationsParItineraire(int itineraireId);
        IEnumerable<Reservation> GetReservationsActives();
        bool PeutReserver(int itineraireId, int nombrePassagers, out string erreur);
        int CompterReservationsActives(int itineraireId);
        bool ReservationExiste(string numeroBillet);

        Reservation GetReservationParNumero(string numeroBillet);
    }
}
