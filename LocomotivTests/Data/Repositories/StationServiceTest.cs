using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Model;
using Moq;

namespace LocomotivTests.Data.Repositories
{
    public class StationServiceTest
    {
        private readonly Mock<IStationDAL> _stationDALMock;
        private readonly IStationService _stationService;

        public StationServiceTest() 
        {
            _stationDALMock = new Mock<IStationDAL>();
            _stationService = new StationService(_stationDALMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllStations()
        {
            var station1 = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };
            var station2 = new Station
            {
                Id = 2,
                Nom = "Gare Ouest",
                Latitude = 45.5088,
                Longitude = -73.554,
                CapaciteMaximale = 15
            };

            _stationDALMock.Setup(dal => dal.GetAll()).Returns(new List<Station> { station1, station2 });

            var stations = _stationService.GetAll().ToList();

            Assert.Equal(2, stations.Count);
            Assert.Contains(stations, s => s.Nom == "Gare Centrale");
            Assert.Contains(stations, s => s.Nom == "Gare Ouest");
        }

        [Fact]
        public void GetById_ShouldReturnStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationDALMock.Setup(dal => dal.GetById(1)).Returns(station);

            Assert.Equal("Gare Centrale", _stationService.GetById(1).Nom);
        }

        [Fact]
        public void GetById_ShouldThrowException_WhenIdInvalid()
        {
            int invalidId = 100;

            _stationDALMock.Setup(dal => dal.GetById(invalidId)).Returns((Station)null);

            Assert.Throws<Exception>(() => _stationService.GetById(invalidId));
        }

        [Fact]
        public void StationEstPleine_ShouldReturnStationNotFull()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            bool estPleine = _stationService.StationEstPleine(station.Id);

            Assert.False(estPleine);
        }

        [Fact]
        public void StationEstPleine_ShouldReturnStationFull()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 0
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            bool estPleine = _stationService.StationEstPleine(station.Id);

            Assert.False(!estPleine);
        }

        [Fact]
        public void GetNomStation_ShouldReturnStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 0
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            Assert.Equal("Gare Centrale", _stationService.GetNomStation(station.Id));
        }

        [Fact]
        public void GetNomStation_ShouldThrowException_WhenNomIsInvalid()
        {
            var station = new Station
            {
                Id = 1,
                Nom = null,
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 0
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            Assert.Throws<Exception>(() => _stationService.GetNomStation(station.Id));
        }

        [Fact]
        public void GetPositionStation_ShouldReturnPositionOfStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            Assert.Equal("Latitude: " + station.Latitude + ", Longitude: " + station.Longitude, _stationService.GetPositionStation(station.Id));
        }

        [Fact]
        public void GetCapaciteStation_ShouldReturnCapaciteOfStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            Assert.Equal("Capacité: " + 0 + "/" + station.CapaciteMaximale, _stationService.GetCapaciteStation(station.Id));
        }

        [Fact]
        public void GetVoies_ShouldReturnVoiesInStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            Assert.Equal(station.Voies, _stationService.GetVoies(station.Id));
        }

        [Fact]
        public void GetSignaux_ShouldReturnVoiesInStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            Assert.Equal(station.Signaux, _stationService.GetSignaux(station.Id));
        }

        [Fact]
        public void GetTrainsEnGare_ShouldReturnVoiesInStation()
        {
            var station = new Station
            {
                Id = 1,
                Nom = "Gare Centrale",
                Latitude = 45.5017,
                Longitude = -73.5673,
                CapaciteMaximale = 10
            };

            _stationDALMock.Setup(dal => dal.GetById(station.Id)).Returns(station);

            Assert.Equal(station.TrainsEnGare, _stationService.GetTrainsEnGare(station.Id));
        }
    }
}
