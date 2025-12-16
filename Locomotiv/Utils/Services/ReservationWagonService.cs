using Locomotiv.Model;
using Locomotiv.Utils.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Locomotiv.Utils.Services
{
    public class ReservationWagonService : IReservationWagonService
    {
        private readonly ApplicationDbContext _context;
        private const double TARIF_BASE_PAR_WAGON = 500.0;
        private const double TARIF_PAR_TONNE = 50.0;

        public ReservationWagonService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Train> ObtenirTrainsCommerciaux(int stationId, DateTime? date = null)
        {
            var trainMarchandiseDispo = _context.Trains
                .Include(t => t.StationActuelle)
                .Where(t => t.Type == TypeTrain.Marchandises &&
                           t.Etat == EtatTrain.EnGare &&
                           t.NombreWagonsDisponibles > 0);

            if (stationId > 0)
            {
                trainMarchandiseDispo = trainMarchandiseDispo.Where(t => t.StationActuelleId == stationId);
            }

            return trainMarchandiseDispo.ToList();
        }

        public bool VerifierDisponibilite(int trainId, int nombreWagons)
        {
            var train = _context.Trains.Find(trainId);
            if (train == null || train.Type != TypeTrain.Marchandises)
                return false;

            return train.NombreWagonsDisponibles >= nombreWagons;
        }

        public double CalculerTarif(TypeMarchandise typeMarchandise, double poids, int nombreWagons)
        {
            double tarifBase = TARIF_BASE_PAR_WAGON * nombreWagons;
            double tarifPoids = TARIF_PAR_TONNE * poids;

            double multiplicateur = ObtenirMultiplicateur(typeMarchandise);

            return (tarifBase + tarifPoids) * multiplicateur;
        }

        private double ObtenirMultiplicateur(TypeMarchandise typeMarchandise)
        {
            if (typeMarchandise == TypeMarchandise.Dangereux)
                return 2.0;

            if (typeMarchandise == TypeMarchandise.Perissable)
                return 1.5;

            if (typeMarchandise == TypeMarchandise.Fragile)
                return 1.3;

            if (typeMarchandise == TypeMarchandise.Surdimensionne)
                return 1.8;

            return 1.0;
        }

        public ReservationWagon CreerReservation(int clientId, int itineraireId, int nombreWagons,
            TypeMarchandise typeMarchandise, double poids, string? notes)
        {
            var itineraire = _context.Itineraires
                .Include(i => i.Train)
                .FirstOrDefault(i => i.Id == itineraireId);

            if (itineraire == null)
                throw new Exception("Itinéraire non trouvé.");

            if (!VerifierDisponibilite(itineraire.TrainId, nombreWagons))
                throw new Exception("Nombre de wagons insuffisant.");

            var train = itineraire.Train;
            if (train.CapaciteChargeTonnes.HasValue && poids > train.CapaciteChargeTonnes.Value)
                throw new Exception($"Le poids dépasse la capacité maximale de {train.CapaciteChargeTonnes.Value} tonnes.");

            double tarif = CalculerTarif(typeMarchandise, poids, nombreWagons);

            var reservation = new ReservationWagon
            {
                ClientCommercialId = clientId,
                ItineraireId = itineraireId,
                NombreWagons = nombreWagons,
                TypeMarchandise = typeMarchandise,
                PoidsTotal = poids,
                TarifTotal = tarif,
                DateReservation = DateTime.Now,
                Statut = StatutReservation.EnAttente,
                NotesSpeciales = notes
            };

            _context.ReservationsWagons.Add(reservation);

            train.NombreWagonsDisponibles -= nombreWagons;

            _context.SaveChanges();

            return reservation;
        }

        public bool AnnulerReservation(int reservationId)
        {
            var reservation = _context.ReservationsWagons
                .Include(r => r.Itineraire)
                    .ThenInclude(i => i.Train)
                .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
                return false;

            if (reservation.Statut == StatutReservation.Completee)
                throw new Exception("Impossible d'annuler une réservation complétée.");

            reservation.Statut = StatutReservation.Annulee;

            var train = reservation.Itineraire.Train;
            train.NombreWagonsDisponibles += reservation.NombreWagons;

            _context.SaveChanges();
            return true;
        }

        public IEnumerable<ReservationWagon> ObtenirReservationsClient(int clientId)
        {
            return _context.ReservationsWagons
                .Include(r => r.Itineraire)
                    .ThenInclude(i => i.Train)
                .Include(r => r.Itineraire.StationDepart)
                .Include(r => r.Itineraire.StationArrivee)
                .Where(r => r.ClientCommercialId == clientId)
                .OrderByDescending(r => r.DateReservation)
                .ToList();
        }

        public ReservationWagon? ObtenirReservation(int reservationId)
        {
            return _context.ReservationsWagons
                .Include(r => r.Itineraire)
                    .ThenInclude(i => i.Train)
                .Include(r => r.Itineraire.StationDepart)
                .Include(r => r.Itineraire.StationArrivee)
                .Include(r => r.ClientCommercial)
                .FirstOrDefault(r => r.Id == reservationId);
        }
    }
}
