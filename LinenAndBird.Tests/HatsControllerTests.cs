using System;
using System.Collections.Generic;
using LinenAndBird.Controllers;
using LinenAndBird.DataAccess;
using LinenAndBird.Models;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LinenAndBird.Tests
{
    public class HatsControllerTests
    {
        [Fact]
        public void requesting_all_hats_returns_all_hats()
        {
            //Arrange
            var controller = new HatsController(new FakeHatRepository());

            //Act
            var result = controller.GetAllHats();

            //Assert
            Assert.Equal(3, result.Count);
        }
    }

    public class FakeHatRepository : IHatRepository
    {
        public Hat GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Hat> GetAll()
        {
            return new List<Hat> {new Hat(), new Hat(), new Hat()};
        }

        public IEnumerable<Hat> GetByStyle(HatStyle style)
        {
            throw new NotImplementedException();
        }

        public void Add(Hat newHat)
        {
            throw new NotImplementedException();
        }
    }
}
