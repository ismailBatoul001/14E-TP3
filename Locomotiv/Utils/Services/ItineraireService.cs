using Locomotiv.Model;
using Locomotiv.Utils.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Locomotiv.Utils.Services
{
    public class ItineraireService : IItineraireService
    {
        private readonly ApplicationDbContext _context;
        private const int TEMPS_ARRET_MINIMUM_MINUTES = 2;

        public ItineraireService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool PeutCreerItineraire(Train train, List<int> blocksIds, out string erreur)
        {
            erreur = string.Empty;

            if (train == null)
            {
                erreur = "Le train est invalide.";
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

        public bool VerifierSecuriteBlock(Block block, Train train)
        {
            return true;
        }

        public Itineraire CreerItineraire(Train train, int stationDepartId, int stationArriveeId, List<ItineraireArret> arrets)
        {
            var itineraire = new Itineraire
            {
                TrainId = train.Id,
                StationDepartId = stationDepartId,
                StationArriveeId = stationArriveeId,
                DateCreation = DateTime.Now,
                EstActif = false,
                Arrets = new List<ItineraireArret>()
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            for (int i = 0; i < arrets.Count; i++)
            {
                var arret = arrets[i];
                arret.Ordre = i + 1;
                arret.ItineraireId = itineraire.Id;

                if (i == 0)
                {
                    arret.HeureArrivee = DateTime.Now;
                    arret.HeureDepart = DateTime.Now.AddMinutes(TEMPS_ARRET_MINIMUM_MINUTES);
                }
                else
                {
                    arret.HeureArrivee = arrets[i - 1].HeureDepart.AddMinutes(10);
                    arret.HeureDepart = arret.HeureArrivee.AddMinutes(TEMPS_ARRET_MINIMUM_MINUTES);
                }

                _context.ItineraireArrets.Add(arret);
            }

            _context.SaveChanges();
            return itineraire;
        }

        public void DemarrerItineraire(int itineraireId)
        {
            var itineraire = _context.Itineraires
                .Include(i => i.Train)
                .Include(i => i.Arrets)
                .Include(i => i.StationDepart)
                .Include(i => i.StationArrivee)
                .FirstOrDefault(i => i.Id == itineraireId);

            if (itineraire == null)
            {
                return;
            }

            itineraire.Train.Etat = EtatTrain.EnTransit;
            itineraire.Train.StationActuelleId = itineraire.StationDepartId;
            itineraire.EstActif = true;

            if (itineraire.Train.VoieActuelleId.HasValue)
            {
                var voie = _context.Voies.Find(itineraire.Train.VoieActuelleId.Value);
                if (voie != null)
                {
                    voie.EstDisponible = true;
                    voie.TrainActuelId = 0;
                }
                itineraire.Train.VoieActuelleId = null;
            }

            _context.SaveChanges();
        }

        public void ArreterItineraire(int itineraireId)
        {
            var itineraire = _context.Itineraires
                .Include(i => i.Train)
                .Include(i => i.StationArrivee)
                .FirstOrDefault(i => i.Id == itineraireId);

            if (itineraire == null) return;

            itineraire.EstActif = false;
            itineraire.Train.Etat = EtatTrain.EnGare;
            itineraire.Train.StationActuelleId = itineraire.StationArriveeId;

            _context.SaveChanges();
        }

        public IEnumerable<Itineraire> GetItinerairesActifs()
        {
            return _context.Itineraires
                .Include(i => i.Train)
                .Include(i => i.StationDepart)
                .Include(i => i.StationArrivee)
                .Include(i => i.Arrets)
                    .ThenInclude(a => a.Station)
                .Where(i => i.EstActif)
                .ToList();
        }

        public IEnumerable<Itineraire> RechercherItineraires(int? stationDepartId,int? stationArriveeId,DateTime? dateDepart,TimeSpan? heureDepartMin)
        {
            var context = _context.Itineraires
                .Include(i => i.Train)
                .Include(i => i.StationDepart)
                .Include(i => i.StationArrivee)
                .Include(i => i.Arrets)
                    .ThenInclude(a => a.Station)
                .Where(i => i.EstActif && i.Train.Type == TypeTrain.Passagers)
                .ToList();

            var resultats = context.AsEnumerable();

            if (stationDepartId.HasValue)
            {
                resultats = resultats.Where(i => i.StationDepartId == stationDepartId.Value);
            }
            if (stationArriveeId.HasValue)
            {
                resultats = resultats.Where(i => i.StationArriveeId == stationArriveeId.Value);
            }
            if (dateDepart.HasValue)
            {
                resultats = resultats.Where(i => i.DateCreation.Date == dateDepart.Value.Date);
            }
            if (heureDepartMin.HasValue)
            {
                resultats = resultats.Where(i => i.DateCreation.TimeOfDay >= heureDepartMin.Value);
            }

            return resultats.OrderBy(i => i.DateCreation).ToList();
        }

        public int CalculerPlacesDisponibles(int itineraireId)
        {
            var itineraire = _context.Itineraires
                .Include(i => i.Train)
                .FirstOrDefault(i => i.Id == itineraireId);

            if (itineraire == null)
            {
                throw new Exception($"Itinéraire {itineraireId} introuvable.");
            }

            int reservations = _context.Reservations.Count(r => r.ItineraireId == itineraireId && r.EstActif);

            int placesDisponibles = itineraire.Train.Capacite - reservations;
            return Math.Max(0, placesDisponibles);
        }

        public decimal CalculerTarifItineraire(int itineraireId)
        {
            var itineraire = _context.Itineraires
                .Include(i => i.StationDepart)
                .Include(i => i.StationArrivee)
                .Include(i => i.Arrets)
                .FirstOrDefault(i => i.Id == itineraireId);

            if (itineraire == null)
            {
                throw new Exception($"Itinéraire {itineraireId} introuvable.");
            }

            double distanceTotale = CalculerDistance(
                itineraire.StationDepart.Latitude,
                itineraire.StationDepart.Longitude,
                itineraire.StationArrivee.Latitude,
                itineraire.StationArrivee.Longitude);

            decimal tarifBase = (decimal)(distanceTotale * 0.50);

            decimal coutArrets = itineraire.Arrets.Count * 0.50m;

            decimal tarifTotal = tarifBase + coutArrets;

            return Math.Max(tarifTotal, 5.0m);
        }
        private double CalculerDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = DegreesARadians(lat2 - lat1);
            double dLon = DegreesARadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(DegreesARadians(lat1)) * Math.Cos(DegreesARadians(lat2)) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double DegreesARadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}