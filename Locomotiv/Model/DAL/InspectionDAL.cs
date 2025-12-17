using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Model.DAL
{
    public class InspectionDAL : IInspectionDAL
    {
        private readonly ApplicationDbContext _context;

        public InspectionDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public Inspection? GetById(int id)
        {
            return _context.Inspections
                .Include(i => i.Train)
                .Include(i => i.Mecanicien)
                .FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<Inspection> GetAll()
        {
            return _context.Inspections
                .Include(i => i.Train)
                .Include(i => i.Mecanicien)
                .OrderByDescending(i => i.DateInspection)
                .ToList();
        }

        public IEnumerable<Inspection> GetByTrainId(int trainId)
        {
            return _context.Inspections
                .Where(i => i.TrainId == trainId)
                .Include(i => i.Mecanicien)
                .OrderByDescending(i => i.DateInspection)
                .ToList();
        }

        public void Add(Inspection inspection)
        {
            _context.Inspections.Add(inspection);
            _context.SaveChanges();
        }

        public void Update(Inspection inspection)
        {
            _context.Inspections.Update(inspection);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var insp = _context.Inspections.Find(id);
            if (insp == null) return;
            _context.Inspections.Remove(insp);
            _context.SaveChanges();
        }
    }
}
