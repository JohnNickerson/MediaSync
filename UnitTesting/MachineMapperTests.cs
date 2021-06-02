using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssimilationSoftware.MediaSync.Core.Mappers.CSV;

namespace UnitTesting
{
    [TestClass]
    public class MachineMapperTests
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var mapper = new CsvMachineMapper();
            Assert.IsNotNull(mapper);
        }
    }
}
