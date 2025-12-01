using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class StationDAL : IStationDAL
    {
        private readonly ApplicationDbContext _context;

        public StationDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Station> GetAll()
        {
            return _context.Stations.ToList();
        }

        public Station? GetById(int id)
        {
            return _context.Stations
                .Include(s => s.Voies)
                .Include(s => s.Signaux)
                .Include(s => s.TrainsEnGare)
                .FirstOrDefault(s => s.Id == id);
        }
    }
}
