﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ptsDigitalTerrainModel;
using System.IO;

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
        public void TinFromLidar_isNotNull()
        {
            this.Initialize();
            Assert.IsNotNull(this.tinFromLidar);
        }

        [TestMethod]
        public void TinFromLidar_ElevationSlopeAspect_correctForTriangleZero()
        {
            this.Initialize();
            TinFromLidar_isNotNull();
            var ElSlopeAspect = this.tinFromLidar
                .getElevationSlopeAzimuth(new ptsDTMpoint(2133760.0, 775765.59, 0.0));
            ElSlopeAspect.AssertDerivedValuesAreEqual(202.63, 2.6, 24.775);
        }

        [TestMethod]
        public void TinFromLidar_ElevationSlopeAspect_OnSameTriangleOfAroadwayFillSlope()
        {
            this.Initialize();
            TinFromLidar_isNotNull();
            var ElSlopeAspect = this.tinFromLidar
                .getElevationSlopeAzimuth(new ptsDTMpoint(2133835.08, 775629.79));
            ElSlopeAspect.AssertDerivedValuesAreEqual(200.0, 44.0, 226.48);

            ElSlopeAspect = this.tinFromLidar
                .getElevationSlopeAzimuth(new ptsDTMpoint(2133836.27, 775629.41));
            ElSlopeAspect.AssertDerivedValuesAreEqual(200.24, 44.0, 226.48);
        }

        [TestMethod]
        public void TinFromLidar_ElevationSlopeAspect_OnSameTriangleOfAbridgeEndBentSlope()
        {
            this.Initialize();
            TinFromLidar_isNotNull();
            var ElSlopeAspect = this.tinFromLidar
                .getElevationSlopeAzimuth(new ptsDTMpoint(2133952.01, 775539.31));
            ElSlopeAspect.AssertDerivedValuesAreEqual(190.0, 41.3, 137.291);

            ElSlopeAspect = this.tinFromLidar
                .getElevationSlopeAzimuth(new ptsDTMpoint(2133987.65, 775577.91));
            ElSlopeAspect.AssertDerivedValuesAreEqual(190.0, 41.3, 137.291);
        }

        [Ignore]
        [TestMethod]
        public void TinFromLidar_SavePointsAsDxf()
        {
            this.Initialize();
            TinFromLidar_isNotNull();

            var outDirectory = new DirectoryManager();
            outDirectory.CdUp(2).CdDown("CogoTests").CdDown("outputs");
            outDirectory.EnsureExists();
            string outFile = outDirectory.GetPathAndAppendFilename("SmallLidar_Points.dxf");

            this.tinFromLidar.WritePointsToDxf(outFile);

            bool fileExists = File.Exists(outFile);
            Assert.IsTrue(fileExists);
        }

    }
}
