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
    }
}