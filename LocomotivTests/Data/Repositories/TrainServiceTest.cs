using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocomotivTests.Data.Repositories
{
    public class TrainServiceTest
    {
        private readonly ITrainService _trainService;
        private readonly Mock<IStationDAL> _stationRepositoryMock;
        private readonly Mock<ITrainRepository> _trainRepositoryMock;

        public TrainServiceTest()
        {
            _stationRepositoryMock = new Mock<IStationDAL>();
            _trainRepositoryMock = new Mock<ITrainRepository>();
            _trainService = new TrainService(_trainRepositoryMock.Object, _stationRepositoryMock.Object);
        }

        [Fact]
        public void AjouterTrain_ShouldAddTrain()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            var train = new Train
            {
                Numero = "TR-003",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 500
            };

            _stationRepositoryMock.Setup(service => service.GetById(station.Id)).Returns(station);
            _trainRepositoryMock.Setup(repository => repository.CountTrainsStation(station.Id)).Returns(0);
            _trainRepositoryMock.Setup(repository => repository.GetAll()).Returns(new List<Train>());

            _trainService.AjouterTrain(train.Numero, train.Type, train.Etat, train.Capacite, station.Id);
            _trainRepositoryMock.Verify(repository => repository.Add(It.Is<Train>(train => train.Numero == "TR-003" && train.Type == TypeTrain.Passagers && train.Etat == EtatTrain.EnGare && train.Capacite == 500)), Times.Once);
        }

        [Fact]
        public void AjouterTrain_ShouldThrowException_WhenStationIsFull()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 0
            };
            var train = new Train
            {
                Numero = "TR-003",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 500
            };

            _stationRepositoryMock.Setup(service => service.GetById(station.Id)).Returns(station);
            _trainRepositoryMock.Setup(repository => repository.CountTrainsStation(station.Id)).Returns(1);

            Assert.Throws<Exception>(() => _trainService.AjouterTrain(train.Numero, train.Type, train.Etat, train.Capacite, station.Id));
        }

        [Fact]
        public void AjouterTrain_ShouldThrowException_WhenStationDoesNotExist()
        {
            var train = new Train
            {
                Numero = "TR-003",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 500
            };

            _stationRepositoryMock.Setup(service => service.GetById(It.IsAny<int>())).Returns((Station)null);

            Assert.Throws<Exception>(() => _trainService.AjouterTrain(train.Numero, train.Type, train.Etat, train.Capacite, 1));
        }

        [Fact]
        public void AjouterTrain_ShouldThrowException_WhenCapacityIsInvalid()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationRepositoryMock.Setup(service => service.GetById(station.Id)).Returns(station);
            _trainRepositoryMock.Setup(repository => repository.CountTrainsStation(station.Id)).Returns(0);
            _trainRepositoryMock.Setup(repository => repository.GetAll()).Returns(new List<Train>());

            Assert.Throws<Exception>(() => _trainService.AjouterTrain("TR-003", TypeTrain.Passagers, EtatTrain.EnGare, -100, station.Id));
        }

        [Fact]
        public void AjouterTrain_ShouldThrowException_WhenTrainNumberExists()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            var train = new Train
            {
                Numero = "TR-003",
                Type = TypeTrain.Passagers,
                Etat = EtatTrain.EnGare,
                Capacite = 500
            };

            _stationRepositoryMock.Setup(service => service.GetById(station.Id)).Returns(station);
            _trainRepositoryMock.Setup(repository => repository.CountTrainsStation(station.Id)).Returns(0);
            _trainRepositoryMock.Setup(repository => repository.GetAll()).Returns(new List<Train> { train });

            Assert.Throws<Exception>(() => _trainService.AjouterTrain(train.Numero, train.Type, train.Etat, train.Capacite, station.Id));
        }

        [Fact]
        public void AjouterTrain_ShouldThrowException_WhenTrainNumberDoesNotExist()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationRepositoryMock.Setup(service => service.GetById(station.Id)).Returns(station);
            _trainRepositoryMock.Setup(repository => repository.CountTrainsStation(station.Id)).Returns(0);
            _trainRepositoryMock.Setup(repository => repository.GetAll()).Returns(new List<Train>());

            Assert.Throws<Exception>(() => _trainService.AjouterTrain("", TypeTrain.Passagers, EtatTrain.EnGare, 500, station.Id));
        }

        [Fact]
        public void GetAll_ShouldReturnAllTrains()
        {
            var trains = new List<Train>
            {
                new Train { Numero = "TR-001", Type = TypeTrain.Express, Etat = EtatTrain.EnGare, Capacite = 300 },
                new Train { Numero = "TR-002", Type = TypeTrain.Passagers, Etat = EtatTrain.EnTransit, Capacite = 200 }
            };

            _trainRepositoryMock.Setup(repository => repository.GetAll()).Returns(trains);
            var ToutLesTrains = _trainService.GetAll();

            Assert.Equal(2, ToutLesTrains.Count());
            Assert.Equal("TR-001", ToutLesTrains.First().Numero);
            Assert.Equal("TR-002", ToutLesTrains.ElementAt(1).Numero);
        }

        [Fact]
        public void GetById_ShouldReturnTrain()
        {
            var train = new Train
            {
                Id = 1,
                Numero = "TR-001",
                Type = TypeTrain.Express,
                Etat = EtatTrain.EnGare,
                Capacite = 300
            };

            _trainRepositoryMock.Setup(r => r.GetById(train.Id)).Returns(train);

            var trainRecu = _trainService.GetById(train.Id);

            Assert.Equal("TR-001", trainRecu.Numero);
        }

        [Fact]
        public void GetById_ShouldThrowException_WhenIdInvalid()
        {
            int invalidId = 100;

            _trainRepositoryMock.Setup(r => r.GetById(invalidId)).Returns((Train)null);

            Assert.Throws<Exception>(() => _trainService.GetById(invalidId));
        }

        [Fact]
        public void ModifierTrain_ShouldUpdateTrain()
        {
            var train = new Train
            {
                Id = 1,
                Numero = "TR-001",
                Type = TypeTrain.Express,
                Etat = EtatTrain.EnGare,
                Capacite = 300
            };
            _trainService.ModifierTrain(train);

            _trainRepositoryMock.Verify(repository => repository.Update(train), Times.Once);
        }

        [Fact]
        public void SupprimerTrain_ShouldDeleteTrain()
        {
            var train = new Train
            {
                Id = 1,
                Numero = "TR-001",
                Type = TypeTrain.Express,
                Etat = EtatTrain.EnGare,
                Capacite = 300
            };

            _trainService.SupprimerTrain(train);

            _trainRepositoryMock.Verify(repository => repository.Delete(train), Times.Once);
        }

        [Fact]
        public void SupprimerTrain_ShouldThrowException_WhenTrainIsNull()
        {
            Assert.Throws<Exception>(() => _trainService.SupprimerTrain(null));
        }

        [Fact]
        public void CompterTrainsStation_ShouldReturnNumberOfTrainsInStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _trainRepositoryMock.Setup(repository => repository.CountTrainsStation(station.Id)).Returns(5);

            var count = _trainService.CompterTrainsStation(station.Id);

            Assert.Equal(5, count);
        }

        [Fact]
        public void GetByStation_ShouldReturnTrainsInStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            var trains = new List<Train>
            {
                new Train { Numero = "TR-001", Type = TypeTrain.Express, Etat = EtatTrain.EnGare, Capacite = 300, StationActuelleId = station.Id },
                new Train { Numero = "TR-002", Type = TypeTrain.Passagers, Etat = EtatTrain.EnTransit, Capacite = 200, StationActuelleId = station.Id }
            };

            _trainRepositoryMock.Setup(repository => repository.GetByStation(station.Id)).Returns(trains);

            var trainsInStation = _trainService.GetByStation(station.Id);

            Assert.Equal(2, trainsInStation.Count());
            Assert.All(trainsInStation, t => Assert.Equal(station.Id, t.StationActuelleId));
        }
    }
}
