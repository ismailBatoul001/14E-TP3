using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Model.Interfaces
{
    public interface IInspectionDAL
    {
        Inspection? GetById(int id);
        IEnumerable<Inspection> GetAll();
        IEnumerable<Inspection> GetByTrainId(int trainId);

        void Add(Inspection inspection);
        void Update(Inspection inspection);
        void Delete(int id);
    }
}
