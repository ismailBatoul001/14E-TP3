using Locomotiv.Model;
using System.Collections.Generic;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IPlanificationItineraireService
    {
        IEnumerable<Train> ObtenirTrainsDisponibles();


        IEnumerable<Station> ObtenirToutesLesStations();


        IEnumerable<PointInteret> ObtenirTousLesPointsInteret();



        bool ValiderItineraire(Train train, Station stationDepart, Station stationArrivee, out string erreur);

        Itineraire CreerNouvelItineraire(Train train, int stationDepartId, int stationArriveeId,
            List<ItineraireArret> arrets);


        void DemarrerItineraire(int itineraireId);


        List<ItineraireArret> ConstruireListeArrets(IEnumerable<object> elements);
    }
}