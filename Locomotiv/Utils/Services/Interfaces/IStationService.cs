using Locomotiv.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IStationService
    {
        IEnumerable<Station> GetAll();
        Station? GetById(int id);
        bool StationEstPleine(int stationId);
        string GetNomStation(int stationId);
        string GetPositionStation(int stationId);
        string GetCapaciteStation(int stationId);
        IEnumerable<Voie> GetVoies(int stationId);
        IEnumerable<Signal> GetSignaux(int stationId);
        IEnumerable<Train> GetTrainsEnGare(int stationId);
    }
}
