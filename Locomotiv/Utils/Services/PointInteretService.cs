using Locomotiv.Model.DAL;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Model.Interfaces;
using Locomotiv.Model;


namespace Locomotiv.Utils.Services
{
    public class PointInteretService : IPointInteretService
    {
        private readonly IPointInteretDAL _pointInteretRepository;
        public PointInteretService(IPointInteretDAL pointInteretRepository) 
        {
            _pointInteretRepository = pointInteretRepository;
        }

        public IEnumerable<PointInteret> GetAll()
        {
            return _pointInteretRepository.GetAll();
        }

        public PointInteret GetById(int id)
        { 
            var PointInteret = _pointInteretRepository.GetById(id);
            if (PointInteret == null)
            {
                throw new Exception("Point d'interêt n'existe pas");
            }
            return PointInteret;
        }
    }
}
