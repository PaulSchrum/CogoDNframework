﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ptsDigitalTerrainModel;
using System.IO;
using ptsCogo.Horizontal;

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
        public void TinFromLidar_Statistics_AreCorrect()
        {  // To do: complete this. It is currently not really checking anything.
            getPrunedTin();
            var stats = this.aTin.Statistics;
            Assert.IsNotNull(stats);

            var pointCount = stats.PointCount;
            var expected = this.aTin.allPoints.Count();
            Assert.AreEqual(expected: expected, actual: pointCount);

        }

        [TestMethod]
        public void TinFromLidar_Decimation_WorksCorrectly()
        {
            getPrunedTin();
            var undecimatedStats = this.aTin.Statistics;

            var decimatedTin = ptsDTM.CreateFromLAS(lidarFileName, skipPoints: 1);
            decimatedTin.pruneTinHull();
            var decimatedSt = decimatedTin.Statistics;

            var halfUndicimated = undecimatedStats.PointCount / 2;
            var nearness = Math.Abs(decimatedSt.PointCount - halfUndicimated);
            Assert.IsTrue(nearness < 5);

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
            string outFile = outDirectory.GetPathAndAppendFilename("SmallLidar_Points.obj");

            this.tinFromLidar.WriteToWaveFront(outFile);

            bool fileExists = File.Exists(outFile);
            Assert.IsTrue(fileExists);
        }

        [Ignore]
        [TestMethod]
        public void TinFromLidar_SaveAsWavefrontObj()
        {
            this.Initialize();
            TinFromLidar_isNotNull();

            var outDirectory = new DirectoryManager();
            outDirectory.CdUp(2).CdDown("CogoTests").CdDown("outputs");
            outDirectory.EnsureExists();
            string outFile = outDirectory.GetPathAndAppendFilename("SmallLidar_Points.obj");

            this.tinFromLidar.WriteToWaveFront(outFile);

            bool fileExists = File.Exists(outFile);
            Assert.IsTrue(fileExists);
        }

        [Ignore]  // We don't need this to run every time we run all tests.
        [TestMethod]
        public void TinFromLidar_SaveTinsAsDxfTriangleShapes()
        {
            this.Initialize();
            TinFromLidar_isNotNull();

            var outDirectory = new DirectoryManager();
            outDirectory.CdUp(2).CdDown("CogoTests").CdDown("outputs");
            outDirectory.EnsureExists();
            string outFile = outDirectory.GetPathAndAppendFilename("SmallLidar_Triangles.dxf");

            this.tinFromLidar.WriteTinToDxf(outFile);

            bool fileExists = File.Exists(outFile);
            Assert.IsTrue(fileExists);
        }

        [Ignore]
        [TestMethod]
        public void TinFromLidar_IntersectsAlignment_CreatesProfile()
        {
            var aTin = ptsDTM.CreateFromLAS(lidarFileName);
            aTin.pruneTinHull();

            var directory = new DirectoryManager();
            directory.CdUp(2).CdDown("CogoTests");
            string testFile = directory.GetPathAndAppendFilename("SmallLidar_StreamAlignment.dxf");

            IList<rm21HorizontalAlignment> PerryCreekAlignments =
                rm21HorizontalAlignment.createFromDXFfile(testFile);
            Assert.IsNotNull(PerryCreekAlignments);
            Assert.AreEqual(expected: 2, actual: PerryCreekAlignments.Count);
            var secondAlignment = PerryCreekAlignments[1];

            ptsCogo.Profile groundProfile = secondAlignment.ProfileFromSurface(aTin);
        }

        private ptsDTM aTin = null;
        private void getPrunedTin()
        {
            if(null == aTin)
            {
                this.aTin = ptsDTM.CreateFromLAS(lidarFileName);
                this.aTin.pruneTinHull();
            }
        }

        [Ignore]
        [TestMethod]
        public void TinFromLidar_CompareTriangleCount()
        {
            this.Initialize();
            getPrunedTin();
            var aTin = this.aTin;
            var triangleCount = this.tinFromLidar.TriangleCount;
            int expected = 150781;
            Assert.AreEqual(expected: expected, actual: triangleCount);
            //triangleCount = aTin.TriangleCount;
            var diff = triangleCount - aTin.TriangleCount;

            Assert.AreEqual(expected: 517, actual: diff);  // This now fails (12/4/10). To do: Figure this out.

            //var outDirectory = new DirectoryManager();
            //outDirectory.CdUp(2).CdDown("CogoTests").CdDown("outputs");
            //outDirectory.EnsureExists();
            //string outFile = outDirectory.GetPathAndAppendFilename("SmallLidar_Triangles.dxf");

            //aTin.WriteTinToDxf(outFile);

        }

        //[TestMethod]
        public void temp_createObjForTilleyCreek()
        {
            var LiFile = @"D:\Research\Datasets\LiDAR\Tilley Creek\TilleyCreek.las";
            var aTin = ptsDTM.CreateFromLAS(LiFile, 745000, 746800, 577800, 579300, skipPoints: 3, 
                new List<int> { 2, 6, 13 });
            aTin.pruneTinHull();
            aTin.WriteToWaveFront(@"D:\Research\Datasets\LiDAR\Tilley Creek\TilleyCreek.obj");
        }

    }
}
