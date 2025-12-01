using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Model;

namespace Locomotiv.Utils.Services
{
    public class StationService : IStationService
    {
        private readonly IStationDAL _stationDAL;

        public StationService(IStationDAL pStationDAL)
        {
            _stationDAL = pStationDAL;
        }

        public IEnumerable<Station> GetAll()
        {
            return _stationDAL.GetAll();
        }

        public Station GetById(int id)
        {
            var station = _stationDAL.GetById(id);
            if (station == null)
            {
                throw new Exception("Station non trouvée.");
            }
            return station;
        }

        public bool StationEstPleine(int stationId)
        {
            var station = GetById(stationId);
            if (station.TrainsEnGare.Count >= station.CapaciteMaximale)
            {
                return true;
            }
            return false;
        }

        public string GetNomStation(int stationId)
        {
            var station = GetById(stationId);
            if(station.Nom == null)
            {
                throw new Exception("Le nom de la station est nul.");
            }
            return station.Nom;
        }

        public string GetPositionStation(int stationId)
        {
            var station = GetById(stationId);
            return "Latitude: " + station.Latitude + ", Longitude: " + station.Longitude;
        }

        public string GetCapaciteStation(int stationId)
        {
            var station = GetById(stationId);
            int nombreTrains = 0;
            if (station.TrainsEnGare != null)
            {
                nombreTrains = station.TrainsEnGare.Count;
            }
            return "Capacité: " + nombreTrains + "/" + station.CapaciteMaximale;
        }

        public IEnumerable<Voie> GetVoies(int stationId)
        {
            var station = GetById(stationId);
            return station.Voies;
        }

        public IEnumerable<Signal> GetSignaux(int stationId)
        {
            var station = GetById(stationId);
            return station.Signaux;
        }

        public IEnumerable<Train> GetTrainsEnGare(int stationId)
        {
            var station = GetById(stationId);
            return station.TrainsEnGare;
        }
    }
}
