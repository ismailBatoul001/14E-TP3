using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services
{
    public class BlockService : IBlockService
    {
        private readonly IBlockRepository _blockRepository;

        public BlockService(IBlockRepository blockRepository) 
        { 
            _blockRepository = blockRepository;
        }

        public IEnumerable<Block> GetAll()
        {
            return _blockRepository.GetAll();
        }
    }
}
