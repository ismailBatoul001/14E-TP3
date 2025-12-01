using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using Moq;
using Locomotiv.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocomotivTests.Data.Repositories
{
    public class BlockServiceTest
    {
        private readonly IBlockService _blockService;
        private readonly Mock<IBlockRepository> _blockRepositoryMock;

        public BlockServiceTest()
        {
            _blockRepositoryMock = new Mock<IBlockRepository>();
            _blockService = new BlockService(_blockRepositoryMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllBlocks()
        {
            var blocks = new List<Block>
            {
                new Block
                {
                    Nom = "Block A",
                    EstOccupe = false,
                    LatitudeDebut = 45.5017,
                    LongitudeDebut = -73.5673,
                    LatitudeFin = 45.5088,
                    LongitudeFin = -73.554,
                },
                new Block
                {
                    Nom = "Block B",
                    EstOccupe = true,
                    LatitudeDebut = 46.5017,
                    LongitudeDebut = -74.5673,
                    LatitudeFin = 46.5088,
                    LongitudeFin = -74.554,
                }
            };

            _blockRepositoryMock.Setup(repo => repo.GetAll()).Returns(blocks);
            var result = _blockService.GetAll();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, block => block.Nom == "Block A");
            Assert.Contains(result, block => block.Nom == "Block B");
        }
    }
}
