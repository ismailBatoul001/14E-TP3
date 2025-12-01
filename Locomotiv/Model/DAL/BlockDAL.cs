using Locomotiv.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class BlockDAL : IBlockRepository
    {
        private readonly ApplicationDbContext _context;

        public BlockDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Block> GetAll()
        {
            return _context.Blocks.ToList();
        }
    }
}
