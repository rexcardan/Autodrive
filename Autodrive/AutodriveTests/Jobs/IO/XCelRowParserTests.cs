using Autodrive.Jobs.IO;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using Cardan.XCel;
using System.Collections.Generic;
using Autodrive.Linacs;

namespace Autodrive.Jobs.IO.Tests
{
    [TestClass()]
    public class XCelRowParserTests
    {
        [TestMethod()]
        public void GetEnergyTest()
        {
            var header = new XCelData("TEST", "Energy");
            var row = new XCelData("TEST", "6X");
            Assert.AreEqual(Energy._6X, XCelRowParser.GetEnergy(header, row));
        }
    }
}
