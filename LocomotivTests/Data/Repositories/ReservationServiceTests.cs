using System;
using System.Collections.Generic;
using System.Linq;
using Locomotiv.Model;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class ReservationServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IItineraireService> _itineraireServiceMock;
    private readonly ReservationService _reservationService;

    public ReservationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _itineraireServiceMock = new Mock<IItineraireService>();
        _reservationService = new ReservationService(_context, _itineraireServiceMock.Object);
    }

    [Fact]
    public void CreerReservation_DevraitCreerReservation_WhenValid()
    {
        var stationDepart = new Station
        {
            Id = 1,
            Nom = "Gare Centrale",
            Latitude = 45.5017,
            Longitude = -73.5673,
            CapaciteMaximale = 10
        };
        var stationArrivee = new Station
        {
            Id = 1,
            Nom = "Gare Ste-foy",
            Latitude = 45.5017,
            Longitude = -73.5673,
            CapaciteMaximale = 10
        };
        var train = new Train { Id = 1, Numero = "TR-001", Type = TypeTrain.Passagers };
        var itineraire = new Itineraire{ Id = 1, StationDepart = stationDepart, StationArrivee = stationArrivee, Train = train, TrainId = train.Id, EstActif = true, DateCreation = DateTime.Now.AddHours(25)};
        var user = new User { Id = 1, Nom = "Ismail", Password = "123", Prenom = "Batoul", Username = "isma" };

        _context.Trains.Add(train);
        _context.Itineraires.Add(itineraire);
        _context.Users.Add(user);
        _context.SaveChanges();

        _itineraireServiceMock.Setup(s => s.CalculerTarifItineraire(itineraire.Id)).Returns(100);
        _itineraireServiceMock.Setup(s => s.CalculerPlacesDisponibles(itineraire.Id)).Returns(10);

        var reservation = _reservationService.CreerReservation(itineraire.Id, user.Id, 2);

        Assert.NotNull(reservation);
        Assert.Equal(200, reservation.MontantTotal);
        Assert.True(reservation.EstActif);
        Assert.Equal(StatutReservation.Confirmee, reservation.Statut);
    }

    [Fact]
    public void CreerReservation_ShouldThrowException_WhenItineraireNotFound()
    {
        var user = new User { Id = 1, Nom = "Ismail", Password = "123", Prenom = "Batoul", Username = "isma" };
        _context.Users.Add(user);
        _context.SaveChanges();

        Assert.Throws<Exception>(() => _reservationService.CreerReservation(999, user.Id, 1));
    }

    [Fact]
    public void CreerReservation_ShouldThrowException_WhenUserNotFound()
    {
        var train = new Train { Id = 1, Numero = "TR-001", Type = TypeTrain.Passagers };
        var itineraire = new Itineraire { Id = 1, Train = train, EstActif = true, DateCreation = DateTime.Now.AddHours(1) };
        _context.Itineraires.Add(itineraire);
        _context.SaveChanges();

        _itineraireServiceMock.Setup(s => s.CalculerTarifItineraire(itineraire.Id)).Returns(100);
        _itineraireServiceMock.Setup(s => s.CalculerPlacesDisponibles(itineraire.Id)).Returns(10);

        Assert.Throws<Exception>(() => _reservationService.CreerReservation(itineraire.Id, 999, 1));
    }

    [Fact]
    public void AnnulerReservation_DevraitAnnulerReservation_WhenValid()
    {
        var train = new Train { Id = 1, Numero = "TR-001", Type = TypeTrain.Passagers };
        var itineraire = new Itineraire { Id = 1, Train = train, EstActif = true, DateCreation = DateTime.Now.AddHours(2) };
        var user = new User { Id = 1, Nom = "Ismail", Password = "123", Prenom = "Batoul", Username = "isma" };
        var reservation = new Reservation { Id = 1, Itineraire = itineraire, User = user, Statut = StatutReservation.Confirmee, EstActif = true, NumeroBillet = "123456789" };

        _context.Itineraires.Add(itineraire);
        _context.Users.Add(user);
        _context.Reservations.Add(reservation);
        _context.SaveChanges();

        _reservationService.AnnulerReservation(reservation.Id);

        var updated = _context.Reservations.Find(reservation.Id);
        Assert.Equal(StatutReservation.Annulee, updated.Statut);
        Assert.False(updated.EstActif);
    }

    [Fact]
    public void AnnulerReservation_ShouldThrowException_WhenAlreadyCancelled()
    {
        var itineraire = new Itineraire { Id = 1, DateCreation = DateTime.Now.AddHours(2), Train = new Train { Type = TypeTrain.Passagers } };
        var reservation = new Reservation { Id = 1, NumeroBillet = "123456789", Itineraire = itineraire, Statut = StatutReservation.Annulee };

        _context.Itineraires.Add(itineraire);
        _context.Reservations.Add(reservation);
        _context.SaveChanges();

        Assert.Throws<Exception>(() => _reservationService.AnnulerReservation(reservation.Id));
    }

    [Fact]
    public void ConfirmerReservation_ShouldConfirm_WhenEnAttente()
    {
        var reservation = new Reservation { Id = 1, NumeroBillet = "123456789", Statut = StatutReservation.EnAttente };
        _context.Reservations.Add(reservation);
        _context.SaveChanges();

        _reservationService.ConfirmerReservation(reservation.Id);

        var updated = _context.Reservations.Find(reservation.Id);
        Assert.Equal(StatutReservation.Confirmee, updated.Statut);
    }

    [Fact]
    public void PeutReserver_ShouldReturnFalse_WhenTrainNotPassagers()
    {
        var train = new Train { Id = 1, Numero = "TR-001", Type = TypeTrain.Express };
        var itineraire = new Itineraire { Id = 1, Train = train, EstActif = true, DateCreation = DateTime.Now.AddHours(1) };
        _context.Itineraires.Add(itineraire);
        _context.SaveChanges();

        var result = _reservationService.PeutReserver(itineraire.Id, 1, out string erreur);

        Assert.False(result);
        Assert.Equal("Les réservations ne sont possibles que pour les trains de passagers.", erreur);
    }

    [Fact]
    public void CompterReservationsActives_ShouldReturnNombreCorrect()
    {
        var itineraire = new Itineraire { Id = 1, Train = new Train { Type = TypeTrain.Passagers }, EstActif = true, DateCreation = DateTime.Now.AddHours(1) };
        var reservations = new List<Reservation>
        { 
        new Reservation { Id = 1, NumeroBillet = "123456789", Itineraire = itineraire, ItineraireId = 1, Statut = StatutReservation.Confirmee, EstActif = true, NombrePassagers = 2 },
        new Reservation { Id = 2, NumeroBillet = "123456789", Itineraire = itineraire, ItineraireId = 1, Statut = StatutReservation.Confirmee, EstActif = true, NombrePassagers = 3 }
        };

        _context.Itineraires.Add(itineraire);
        _context.Reservations.AddRange(reservations);
        _context.SaveChanges();

        var count = _reservationService.CompterReservationsActives(itineraire.Id);

        Assert.Equal(5, count);
    }
}

