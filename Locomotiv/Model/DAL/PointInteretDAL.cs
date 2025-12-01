using Locomotiv.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class PointInteretDAL : IPointInteretDAL
    {
        private readonly ApplicationDbContext _context;

        public PointInteretDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<PointInteret> GetAll()
        {
            return _context.PointsInteret.ToList();
        }

        public PointInteret? GetById(int id)
        {
            return _context.PointsInteret.FirstOrDefault(pi => pi.Id == id);
        }
    }
}
