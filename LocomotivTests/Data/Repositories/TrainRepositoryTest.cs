using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Sdk;
using Moq;
using Microsoft.EntityFrameworkCore;
using Locomotiv;
using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Model.DAL;

namespace LocomotivTests.Data.Repositories
{
    public class TrainRepositoryTest : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ITrainRepository _trainRepository;

        public TrainRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _trainRepository = new TrainRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void AddTrain_ShouldAddTrainToDatabase()
        {
            var train = new Train
            {
                Numero = "TR-001",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 300
            };
            _trainRepository.Add(train);

            var trains = _trainRepository.GetAll();
            Assert.Single(trains);
            Assert.Equal("TR-001", trains[0].Numero);
            Assert.Equal(TypeTrain.Passagers, trains[0].Type);
            Assert.Equal(EtatTrain.EnGare, trains[0].Etat);
            Assert.Equal(300, trains[0].Capacite);
        }

        [Fact]
        public void GetAll_ShouldReturnAllTrains()
        {
            var train = new Train
            {
                Numero = "TR-002",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 200
            };
            var train1 = new Train
            {
                Numero = "TR-003",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 250
            };
            _context.Trains.AddRange(train, train1);
            _context.SaveChanges();

            var trains = _trainRepository.GetAll();

            var premiertrain = trains.First();
            Assert.Equal("TR-002", premiertrain.Numero);
            Assert.Equal(TypeTrain.Passagers, premiertrain.Type);
            Assert.Equal(EtatTrain.EnGare, premiertrain.Etat);
            Assert.Equal(200, premiertrain.Capacite);
            var deuxiemetrain = trains.Skip(1).First();
            Assert.Equal("TR-003", deuxiemetrain.Numero);
            Assert.Equal(TypeTrain.Passagers, deuxiemetrain.Type);
            Assert.Equal(EtatTrain.EnGare, deuxiemetrain.Etat);
            Assert.Equal(250, deuxiemetrain.Capacite);
        }

        [Fact]
        public void GetById_ShouldReturnCorrectTrain()
        {
            var train = new Train
            {
                Numero = "TR-004",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 400
            };
            _context.Trains.Add(train);
            _context.SaveChanges();

            var trainRetourne = _trainRepository.GetById(train.Id);
            Assert.NotNull(trainRetourne);
            Assert.Equal("TR-004", trainRetourne.Numero);
            Assert.Equal(TypeTrain.Passagers, trainRetourne.Type);
            Assert.Equal(EtatTrain.EnGare, trainRetourne.Etat);
            Assert.Equal(400, trainRetourne.Capacite);
        }

        [Fact]
        public void GetByStation_ShouldReturnTrainsAtSpecificStation()
        {
            var station = new Station
            {
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            _context.Stations.Add(station);
            _context.SaveChanges();
            var train = new Train
            {
                Numero = "TR-007",
                Type = TypeTrain.Marchandises,
                Etat = EtatTrain.EnGare,
                Capacite = 180,
                StationActuelleId = station.Id
            };
            var train1 = new Train
            {
                Numero = "TR-008",
                Type = TypeTrain.Marchandises,
                Etat = EtatTrain.HorsService,
                Capacite = 600
            };
            _context.Trains.AddRange(train, train1);
            _context.SaveChanges();

            var trainsAtStation = _trainRepository.GetByStation(station.Id);
            Assert.Single(trainsAtStation);
            Assert.Equal("TR-007", trainsAtStation[0].Numero);
            Assert.Equal(TypeTrain.Marchandises, trainsAtStation[0].Type);
            Assert.Equal(EtatTrain.EnGare, trainsAtStation[0].Etat);
            Assert.Equal(180, trainsAtStation[0].Capacite);
        }

        [Fact]
        public void Update_ShouldModifyExistingTrain()
        {
            var train = new Train
            {
                Numero = "TR-009",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 220
            };
            _context.Trains.Add(train);
            _context.SaveChanges();

            train.Etat = EtatTrain.EnTransit;
            train.Capacite = 230;
            _trainRepository.Update(train);

            var updatedTrain = _trainRepository.GetById(train.Id);
            Assert.NotNull(updatedTrain);
            Assert.Equal("TR-009", updatedTrain.Numero);
            Assert.Equal(TypeTrain.Passagers, updatedTrain.Type);
            Assert.Equal(EtatTrain.EnTransit, updatedTrain.Etat);
            Assert.Equal(230, updatedTrain.Capacite);
        }

        [Fact]
        public void CountTrainsStation_ShouldReturnNumberOfTrainsThatAreInStation()
        {
            var station = new Station
            {
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            _context.Stations.Add(station);
            _context.SaveChanges();

            var train = new Train
            {
                Numero = "TR-010",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 300,
                StationActuelleId = station.Id
            };
            var train1 = new Train
            {
                Numero = "TR-011",
                Type = TypeTrain.Marchandises,
                Etat = EtatTrain.EnGare,
                Capacite = 700,
                StationActuelleId = station.Id
            };
            var train2 = new Train
            {
                Numero = "TR-012",
                Type = TypeTrain.Marchandises,
                Etat = EtatTrain.EnTransit,
                Capacite = 200,
                StationActuelleId = station.Id
            };
            _context.Trains.AddRange(train, train1, train2);
            _context.SaveChanges();

            var count = _trainRepository.CountTrainsStation(station.Id);
            Assert.Equal(2, count);
        }
    }
}
