using Locomotiv.Model;
using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace LocomotivTests.Data.Repositories
{
    public class ReservationWagonServiceTest : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ReservationWagonService _service;
        private readonly Station _station1;
        private readonly Station _station2;
        private readonly Train _trainCommercial;
        private readonly User _client;

        public ReservationWagonServiceTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new ReservationWagonService(_context);

            _station1 = new Station
            {
                Nom = "Gare Test 1",
                Latitude = 45.5,
                Longitude = -73.5,
                CapaciteMaximale = 10
            };

            _station2 = new Station
            {
                Nom = "Gare Test 2",
                Latitude = 46.5,
                Longitude = -74.5,
                CapaciteMaximale = 10
            };

            _context.Stations.AddRange(_station1, _station2);
            _context.SaveChanges();

            _trainCommercial = new Train
            {
                Numero = "T-COM-100",
                Type = TypeTrain.Marchandises,
                Etat = EtatTrain.EnGare,
                Capacite = 0,
                NombreWagonsTotal = 10,
                NombreWagonsDisponibles = 10,
                CapaciteChargeTonnes = 300.0,
                StationActuelleId = _station1.Id
            };

            _context.Trains.Add(_trainCommercial);
            _context.SaveChanges();

            _client = new User
            {
                Prenom = "Client",
                Nom = "Commercial",
                Username = "client1",
                Password = "pass123",
                Role = Role.ClientCommercial,
                StationAssigneeId = _station1.Id
            };

            _context.Users.Add(_client);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void ObtenirTrainsCommerciaux_DevraitRetournerTrainsCommerciauxDisponibles()
        {
            var trains = _service.ObtenirTrainsCommerciaux(_station1.Id).ToList();

            Assert.Single(trains);
            Assert.Equal("T-COM-100", trains[0].Numero);
        }

        [Fact]
        public void ObtenirTrainsCommerciaux_DevraitFiltrerParStation()
        {
            var trainStation2 = new Train
            {
                Numero = "T-COM-300",
                Type = TypeTrain.Marchandises,
                Etat = EtatTrain.EnGare,
                NombreWagonsTotal = 5,
                NombreWagonsDisponibles = 5,
                StationActuelleId = _station2.Id
            };

            _context.Trains.Add(trainStation2);
            _context.SaveChanges();

            var trainsStation1 = _service.ObtenirTrainsCommerciaux(_station1.Id).ToList();

            Assert.Single(trainsStation1);
            Assert.Equal("T-COM-100", trainsStation1[0].Numero);
        }

        [Fact]
        public void VerifierDisponibilite_DevraitRetournerVrai_QuandWagonsDisponibles()
        {
            var disponible = _service.VerifierDisponibilite(_trainCommercial.Id, 5);

            Assert.True(disponible);
        }

        [Fact]
        public void VerifierDisponibilite_DevraitRetournerFaux_QuandPasAssezWagons()
        {
            var disponible = _service.VerifierDisponibilite(_trainCommercial.Id, 15);

            Assert.False(disponible);
        }

        [Fact]
        public void VerifierDisponibilite_DevraitRetournerFaux_QuandTrainInexistant()
        {
            var disponible = _service.VerifierDisponibilite(9999, 5);

            Assert.False(disponible);
        }

        [Fact]
        public void CalculerTarif_DevraitCalculerCorrectement_PourWagonStandard()
        {
            var tarif = _service.CalculerTarif(TypeWagon.Standard, 10.0, 2);

            Assert.Equal(1500.0, tarif);
        }

        [Fact]
        public void CalculerTarif_DevraitAppliquerMultiplicateur_PourRefrigere()
        {
            var tarif = _service.CalculerTarif(TypeWagon.Refrigere, 10.0, 2);

            Assert.Equal(3000.0, tarif);
        }

        [Fact]
        public void CalculerTarif_DevraitAppliquerMultiplicateur_PourCiterne()
        {
            var tarif = _service.CalculerTarif(TypeWagon.Citerne, 10.0, 2);

            Assert.Equal(2250.0, tarif);
        }

        [Fact]
        public void CreerReservation_DevraitCreerAvecSucces()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            var reservation = _service.CreerReservation(
                _client.Id,
                itineraire.Id,
                3,
                TypeWagon.Standard,
                50.0,
                "Test"
            );

            Assert.Equal(_client.Id, reservation.ClientCommercialId);
            Assert.Equal(3, reservation.NombreWagons);
            Assert.Equal(StatutReservation.EnAttente, reservation.Statut);
        }

        [Fact]
        public void CreerReservation_DevraitReduireWagonsDisponibles()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            _service.CreerReservation(_client.Id, itineraire.Id, 3, TypeWagon.Standard, 50.0, null);

            var train = _context.Trains.Find(_trainCommercial.Id);
            Assert.Equal(7, train.NombreWagonsDisponibles);
        }

        [Fact]
        public void CreerReservation_DevraitLancerException_QuandItineraireInexistant()
        {
            var exception = Assert.Throws<Exception>(() =>
                _service.CreerReservation(_client.Id, 9999, 3, TypeWagon.Standard, 50.0, null)
            );

            Assert.Equal("Itinéraire non trouvé.", exception.Message);
        }

        [Fact]
        public void CreerReservation_DevraitLancerException_QuandPasAssezWagons()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            Assert.Throws<Exception>(() =>
                _service.CreerReservation(_client.Id, itineraire.Id, 15, TypeWagon.Standard, 50.0, null)
            );
        }

        [Fact]
        public void CreerReservation_DevraitLancerException_QuandPoidsDepasse()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            Assert.Throws<Exception>(() =>
                _service.CreerReservation(_client.Id, itineraire.Id, 3, TypeWagon.Standard, 500.0, null)
            );
        }

        [Fact]
        public void AnnulerReservation_DevraitAnnulerAvecSucces()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            var reservation = _service.CreerReservation(_client.Id, itineraire.Id, 3, TypeWagon.Standard, 50.0, null);

            var resultat = _service.AnnulerReservation(reservation.Id);

            Assert.True(resultat);

            var reservationAnnulee = _context.ReservationsWagons.Find(reservation.Id);
            Assert.Equal(StatutReservation.Annulee, reservationAnnulee.Statut);
        }

        [Fact]
        public void AnnulerReservation_DevraitLibererWagons()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            var reservation = _service.CreerReservation(_client.Id, itineraire.Id, 4, TypeWagon.Standard, 50.0, null);

            _service.AnnulerReservation(reservation.Id);

            var train = _context.Trains.Find(_trainCommercial.Id);
            Assert.Equal(10, train.NombreWagonsDisponibles);
        }

        [Fact]
        public void AnnulerReservation_DevraitRetournerFaux_QuandReservationInexistante()
        {
            var resultat = _service.AnnulerReservation(9999);

            Assert.False(resultat);
        }

        [Fact]
        public void AnnulerReservation_DevraitLancerException_QuandReservationCompletee()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            var reservation = new ReservationWagon
            {
                ClientCommercialId = _client.Id,
                ItineraireId = itineraire.Id,
                NombreWagons = 2,
                TypeWagon = TypeWagon.Standard,
                PoidsTotal = 20.0,
                TarifTotal = 1500.0,
                DateReservation = DateTime.Now,
                Statut = StatutReservation.Completee
            };

            _context.ReservationsWagons.Add(reservation);
            _context.SaveChanges();

            Assert.Throws<Exception>(() => _service.AnnulerReservation(reservation.Id));
        }

        [Fact]
        public void ObtenirReservationsClient_DevraitRetournerReservationsDuClient()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            _service.CreerReservation(_client.Id, itineraire.Id, 2, TypeWagon.Standard, 20.0, null);
            _service.CreerReservation(_client.Id, itineraire.Id, 3, TypeWagon.Refrigere, 30.0, null);

            var reservations = _service.ObtenirReservationsClient(_client.Id).ToList();

            Assert.Equal(2, reservations.Count);
        }

        [Fact]
        public void ObtenirReservation_DevraitRetournerReservation()
        {
            var itineraire = new Itineraire
            {
                TrainId = _trainCommercial.Id,
                StationDepartId = _station1.Id,
                StationArriveeId = _station2.Id,
                DateCreation = DateTime.Now,
                EstActif = true
            };

            _context.Itineraires.Add(itineraire);
            _context.SaveChanges();

            var created = _service.CreerReservation(_client.Id, itineraire.Id, 2, TypeWagon.Standard, 20.0, "Test");

            var reservation = _service.ObtenirReservation(created.Id);

            Assert.Equal(created.Id, reservation.Id);
            Assert.Equal("Test", reservation.NotesSpeciales);
        }
    }
}