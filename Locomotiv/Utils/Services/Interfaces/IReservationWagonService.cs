using Locomotiv.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IReservationWagonService
    {
        IEnumerable<Train> ObtenirTrainsCommerciaux(int stationId, DateTime? date = null);
        bool VerifierDisponibilite(int trainId, int nombreWagons);
        double CalculerTarif(TypeMarchandise typeMarchandise, double poids, int nombreWagons);
        ReservationWagon CreerReservation(int clientId, int itineraireId, int nombreWagons,
            TypeMarchandise typeMarchandise, double poids, string? notes);
        bool AnnulerReservation(int reservationId);
        IEnumerable<ReservationWagon> ObtenirReservationsClient(int clientId);
        ReservationWagon? ObtenirReservation(int reservationId);
    }
}
