using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Locomotiv.Utils.Services
{
    public class PlanificationItineraireService : IPlanificationItineraireService
    {
        private readonly ITrainRepository _trainRepository;
        private readonly IStationDAL _stationDAL;
        private readonly IItineraireService _itineraireService;
        private readonly ApplicationDbContext _context;
        private readonly IPointInteretDAL _pointInteretDAL;

        public PlanificationItineraireService(
            ITrainRepository trainRepository,
            IStationDAL stationDAL,
            IItineraireService itineraireService,
            IPointInteretDAL pointInteretDAL)
        {
            _trainRepository = trainRepository;
            _stationDAL = stationDAL;
            _itineraireService = itineraireService;
            _pointInteretDAL = pointInteretDAL;
        }

        public IEnumerable<PointInteret> ObtenirTousLesPointsInteret()
        {
            return _pointInteretDAL.GetAll();
        }


        public IEnumerable<Train> ObtenirTrainsDisponibles()
        {
            return _trainRepository.GetAll()
                .Where(t => t.Etat != EtatTrain.HorsService && t.Etat != EtatTrain.EnTransit)
                .ToList();
        }

        public IEnumerable<Station> ObtenirToutesLesStations()
        {
            return _stationDAL.GetAll();
        }

        public bool ValiderItineraire(Train train, Station stationDepart, Station stationArrivee, out string erreur)
        {
            erreur = string.Empty;

            if (train == null)
            {
                erreur = "Veuillez sélectionner un train.";
                return false;
            }

            if (stationDepart == null)
            {
                erreur = "Veuillez sélectionner une station de départ.";
                return false;
            }

            if (stationArrivee == null)
            {
                erreur = "Veuillez sélectionner une station d'arrivée.";
                return false;
            }

            if (stationDepart.Id == stationArrivee.Id)
            {
                erreur = "Les stations de départ et d'arrivée doivent être différentes.";
                return false;
            }

            if (train.Etat == EtatTrain.HorsService)
            {
                erreur = "Le train est hors service.";
                return false;
            }

            if (train.Etat == EtatTrain.EnTransit)
            {
                erreur = "Le train est déjà en transit.";
                return false;
            }

            return true;
        }

        public Itineraire CreerNouvelItineraire(Train train, int stationDepartId, int stationArriveeId,
            List<ItineraireArret> arrets)
        {
            if (train == null)
                throw new ArgumentNullException(nameof(train), "Le train ne peut pas être null.");

            if (stationDepartId <= 0)
                throw new ArgumentException("L'ID de la station de départ est invalide.", nameof(stationDepartId));

            if (stationArriveeId <= 0)
                throw new ArgumentException("L'ID de la station d'arrivée est invalide.", nameof(stationArriveeId));

            return _itineraireService.CreerItineraire(train, stationDepartId, stationArriveeId, arrets);
        }

        public void DemarrerItineraire(int itineraireId)
        {
            if (itineraireId <= 0)
                throw new ArgumentException("L'ID de l'itinéraire est invalide.", nameof(itineraireId));

            _itineraireService.DemarrerItineraire(itineraireId);
        }

        public List<ItineraireArret> ConstruireListeArrets(IEnumerable<object> elements)
        {
            var arrets = new List<ItineraireArret>();

            if (elements == null)
                return arrets;

            foreach (var element in elements)
            {
                if (element is Station station)
                {
                    arrets.Add(new ItineraireArret
                    {
                        EstStation = true,
                        StationId = station.Id,
                        PointInteretId = null
                    });
                }
                else if (element is PointInteret point)
                {
                    arrets.Add(new ItineraireArret
                    {
                        EstStation = false,
                        StationId = null,
                        PointInteretId = point.Id
                    });
                }
            }

            return arrets;
        }
    }
}