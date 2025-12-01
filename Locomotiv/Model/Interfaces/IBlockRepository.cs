using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Model.Interfaces
{
    public interface IBlockRepository
    {
        IEnumerable<Block> GetAll();
    }
}
