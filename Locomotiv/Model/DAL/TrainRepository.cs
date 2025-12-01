using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Locomotiv.Model;
using Locomotiv.Model.Interfaces;

namespace Locomotiv.Model.DAL
{
    public class TrainRepository : ITrainRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Train> GetAll()
        {
            return _context.Trains
                .Include(t => t.StationActuelle)
                .Include(t => t.VoieActuelle)
                .Include(t => t.BlockActuel)
                .ToList();
        }

        public Train? GetById(int id)
        {
            return _context.Trains
                .Include(t => t.StationActuelle)
                .Include(t => t.VoieActuelle)
                .Include(t => t.BlockActuel)
                .FirstOrDefault(t => t.Id == id);
        }

        public IList<Train> GetByStation(int stationId)
        {
            return _context.Trains
                .Where(t => t.StationActuelleId == stationId)
                .Include(t => t.StationActuelle)
                .Include(t => t.VoieActuelle)
                .Include(t => t.BlockActuel)
                .ToList();
        }

        public void Add(Train train)
        {
            _context.Trains.Add(train);
            _context.SaveChanges();
        }

        public void Update(Train train)
        {
            _context.Trains.Update(train);
            _context.SaveChanges();
        }

        public void Delete(Train train)
        {
            _context.Trains.Remove(train);
            _context.SaveChanges();
        }

        public int CountTrainsStation(int stationId)
        {
            return _context.Trains.Count(t => t.StationActuelleId == stationId && t.Etat == EtatTrain.EnGare);
        }
    }
}
