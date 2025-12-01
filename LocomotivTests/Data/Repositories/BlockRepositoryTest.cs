using System;
using System.Collections.Generic;
using System.Text;
using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace LocomotivTests.Data.Repositories
{
    public class BlockRepositoryTest : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlockRepository _blockRepository;

        public BlockRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _blockRepository = new BlockDAL(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetAll_ShouldReturnAllBlocks()
        {
            var block1 = new Block
            {
                Nom = "Block A",
                EstOccupe = false,
                LatitudeDebut = 45.5017,
                LongitudeDebut = -73.5673,
                LatitudeFin = 45.5088,
                LongitudeFin = -73.554,
            };
            var block2 = new Block
            {
                Nom = "Block B",
                EstOccupe = true,
                LatitudeDebut = 46.5017,
                LongitudeDebut = -74.5673,
                LatitudeFin = 46.5088,
                LongitudeFin = -74.554,
            };
            _context.Blocks.AddRange(block1, block2);
            _context.SaveChanges();

            var blocks = _blockRepository.GetAll().ToList();

            Assert.Equal(2, blocks.Count);
        }
    }
}
