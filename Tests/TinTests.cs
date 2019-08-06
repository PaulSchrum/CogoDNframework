using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ptsDigitalTerrainModel;

namespace Tests
{
    /// <summary>
    /// Attempts to read a .las file to get point data from it.
    /// Developed for .las 1.4.   See
    /// https://www.asprs.org/wp-content/uploads/2010/12/LAS_1_4_r13.pdf
    /// 
    ///     From that, Table 4.9 indicates point classes:
    /// 0 Created, never classified
    /// 1 Unclassified1
    /// 2 Ground
    /// 3 Low Vegetation
    /// 4 Medium Vegetation
    /// 5 High Vegetation
    /// 6 Building
    /// 7 Low Point(“low noise”)
    /// 8 High Point(typically “high noise”). Note that this value was previously
    ///         used for Model Key Points.Bit 1 of the Classification Flag must now
    /// 
    /// be used to indicate Model Key Points.This allows the model key point 
    /// class to be preserved.
    /// 9 Water
    /// 10 Rail
    /// 11 Road Surface
    /// 12 Bridge Deck
    /// 13 Wire - Guard
    /// 14 Wire – Conductor (Phase)
    /// 15 Transmission Tower
    /// 16 Wire-structure Connector (e.g.Insulator)
    /// 17 Reserved
    /// 18-63 Reserved
    /// 64-255 User definable – The specific use of these classes 
    ///     should be encoded in the Classification lookup VLR
    /// </summary>
    [TestClass]
    public class TinTests
    {
        string lidarFileName = @"D:\SourceModules\CSharp\CogoDN\ptsDigital" +
            @"TerrainModel\Testing\NC Lidar\Raleigh WRAL Soccer.las";

        ptsDTM tinFromLidar = null;

        private void Initialize()
        {
            if(this.tinFromLidar == null)
                tinFromLidar = ptsDTM.CreateFromLAS(lidarFileName);
        }

        [TestMethod]
        public void TIN_isNotNull()
        {
            this.Initialize();
        }
    }
}
