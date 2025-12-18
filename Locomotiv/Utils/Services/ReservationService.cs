using System;
using System.Collections.Generic;
using System.Linq;
using Locomotiv.Model;
using Locomotiv.Utils.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Locomotiv.Utils.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IItineraireService _itineraireService;

        public ReservationService(ApplicationDbContext context, IItineraireService itineraireService)
        {
            _context = context;
            _itineraireService = itineraireService;
        }

        public Reservation CreerReservation(int itineraireId, int userId, int nombrePassagers)
        {
            if (!PeutReserver(itineraireId, nombrePassagers, out string erreur))
            {
                throw new Exception(erreur);
            }

            var itineraire = _context.Itineraires
                .Include(i => i.Train)
                .Include(i => i.StationDepart)
                .Include(i => i.StationArrivee)
                .FirstOrDefault(i => i.Id == itineraireId);

            if (itineraire == null)
            {
                throw new Exception("Itinéraire introuvable.");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new Exception("Utilisateur introuvable.");
            }

            decimal tarifUnitaire = _itineraireService.CalculerTarifItineraire(itineraireId);
            decimal montantTotal = tarifUnitaire * nombrePassagers;

            string numeroBillet = GenererNumeroBillet(itineraire);

            var reservation = new Reservation
            {
                ItineraireId = itineraireId,
                UserId = userId,
                NombrePassagers = nombrePassagers,
                MontantTotal = montantTotal,
                DateReservation = DateTime.Now,
                EstActif = true,
                Statut = StatutReservation.Confirmee,
                NumeroBillet = numeroBillet
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return reservation;
        }

        public void AnnulerReservation(int reservationId)
        {
            var reservation = _context.Reservations
                .Include(r => r.Itineraire)
                .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                throw new Exception("Réservation introuvable.");
            }

            if (reservation.Statut == StatutReservation.Annulee)
            {
                throw new Exception("Cette réservation est déjà annulée.");
            }

            if (reservation.Statut == StatutReservation.Completee)
            {
                throw new Exception("Impossible d'annuler une réservation terminée.");
            }

            if (reservation.Itineraire.DateCreation <= DateTime.Now)
            {
                throw new Exception("Impossible d'annuler une réservation pour un itinéraire déjà commencé.");
            }

            reservation.Statut = StatutReservation.Annulee;
            reservation.EstActif = false;
            reservation.DateAnnulation = DateTime.Now;

            _context.SaveChanges();
        }

        public void ConfirmerReservation(int reservationId)
        {
            var reservation = _context.Reservations.Find(reservationId);

            if (reservation == null)
            {
                throw new Exception("Réservation introuvable.");
            }

            if (reservation.Statut != StatutReservation.EnAttente)
            {
                throw new Exception("Seules les réservations en attente peuvent être confirmées.");
            }

            reservation.Statut = StatutReservation.Confirmee;
            _context.SaveChanges();
        }

        public Reservation GetById(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Itineraire)
                    .ThenInclude(i => i.Train)
                .Include(r => r.Itineraire.StationDepart)
                .Include(r => r.Itineraire.StationArrivee)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                throw new Exception($"Réservation avec l'ID {id} introuvable.");
            }

            return reservation;
        }

        public IEnumerable<Reservation> GetReservationsParUtilisateur(int userId)
        {
            return _context.Reservations
                .Include(r => r.Itineraire)
                    .ThenInclude(i => i.Train)
                .Include(r => r.Itineraire.StationDepart)
                .Include(r => r.Itineraire.StationArrivee)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateReservation)
                .ToList();
        }

        public IEnumerable<Reservation> GetReservationsParItineraire(int itineraireId)
        {
            return _context.Reservations
                .Include(r => r.User)
                .Where(r => r.ItineraireId == itineraireId)
                .OrderBy(r => r.DateReservation)
                .ToList();
        }

        public IEnumerable<Reservation> GetReservationsActives()
        {
            return _context.Reservations
                .Include(r => r.Itineraire)
                    .ThenInclude(i => i.Train)
                .Include(r => r.Itineraire.StationDepart)
                .Include(r => r.Itineraire.StationArrivee)
                .Include(r => r.User)
                .Where(r => r.EstActif && r.Statut == StatutReservation.Confirmee)
                .OrderBy(r => r.Itineraire.DateCreation)
                .ToList();
        }

        public bool PeutReserver(int itineraireId, int nombrePassagers, out string erreur)
        {
            erreur = string.Empty;

            var itineraire = _context.Itineraires
                .Include(i => i.Train)
                .FirstOrDefault(i => i.Id == itineraireId);

            if (itineraire == null)
            {
                erreur = "Itinéraire introuvable.";
                return false;
            }

            if (!itineraire.EstActif)
            {
                erreur = "Cet itinéraire n'est plus actif.";
                return false;
            }

            if (itineraire.Train.Type != TypeTrain.Passagers)
            {
                erreur = "Les réservations ne sont possibles que pour les trains de passagers.";
                return false;
            }

            if (nombrePassagers <= 0)
            {
                erreur = "Le nombre de passagers doit être au moins 1.";
                return false;
            }

            int placesDisponibles = _itineraireService.CalculerPlacesDisponibles(itineraireId);
            if (nombrePassagers > placesDisponibles)
            {
                erreur = $"Pas assez de places disponibles. Places restantes: {placesDisponibles}";
                return false;
            }

            if (itineraire.DateCreation <= DateTime.Now)
            {
                erreur = "Impossible de réserver pour un itinéraire déjà commencé.";
                return false;
            }

            return true;
        }

        public int CompterReservationsActives(int itineraireId)
        {
            return _context.Reservations
                .Where(r => r.ItineraireId == itineraireId &&
                           r.EstActif &&
                           r.Statut == StatutReservation.Confirmee)
                .Sum(r => r.NombrePassagers);
        }

        public bool ReservationExiste(string numeroBillet)
        {
            return _context.Reservations.Any(r => r.NumeroBillet == numeroBillet);
        }

        public Reservation GetReservationParNumero(string numeroBillet)
        {
            var reservation = _context.Reservations
                .Include(r => r.Itineraire)
                    .ThenInclude(i => i.Train)
                .Include(r => r.Itineraire.StationDepart)
                .Include(r => r.Itineraire.StationArrivee)
                .Include(r => r.User)
                .FirstOrDefault(r => r.NumeroBillet == numeroBillet);

            if (reservation == null)
            {
                throw new Exception($"Réservation avec le numéro {numeroBillet} introuvable.");
            }

            return reservation;
        }

        private string GenererNumeroBillet(Itineraire itineraire)
        {
            string trainNumero = itineraire.Train.Numero.Replace(" ", "");
            string date = DateTime.Now.ToString("yyyyMMdd");
            string random = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();

            string numeroBillet = $"T{trainNumero}-{date}-{random}";

            int compteur = 1;
            while (ReservationExiste(numeroBillet))
            {
                random = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
                numeroBillet = $"T{trainNumero}-{date}-{random}-{compteur}";
                compteur++;
            }

            return numeroBillet;
        }
    }
}
