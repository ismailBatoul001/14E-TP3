using Locomotiv.Model;
using System.Collections.Generic;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IItineraireService
    {
        bool PeutCreerItineraire(Train train, List<int> blocksIds, out string erreur);
        Itineraire CreerItineraire(Train train, int stationDepartId, int stationArriveeId, List<ItineraireArret> arrets);
        void DemarrerItineraire(int itineraireId);
        void ArreterItineraire(int itineraireId);
        bool VerifierSecuriteBlock(Block block, Train train);
        IEnumerable<Itineraire> GetItinerairesActifs();
        IEnumerable<Itineraire> RechercherItineraires(int? stationDepartId, int? stationArriveeId, DateTime? dateDepart, TimeSpan? heureDepartMin);
        int CalculerPlacesDisponibles(int itineraireId);
        decimal CalculerTarifItineraire(int itineraireId);
    }
}