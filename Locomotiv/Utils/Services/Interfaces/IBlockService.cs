using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Model;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IBlockService
    {
        IEnumerable<Block> GetAll();
    }
}
