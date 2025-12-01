using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.Interfaces
{
    public interface ITrainRepository
    {
        IList<Train> GetAll();
        Train? GetById(int id);
        IList<Train> GetByStation(int stationId);
        void Add(Train train);
        void Update(Train train);
        void Delete(Train train);
        int CountTrainsStation(int stationId);
    }
}
