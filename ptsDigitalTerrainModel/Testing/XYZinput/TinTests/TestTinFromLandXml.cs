using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ptsDigitalTerrainModel;
using ptsCogo.Angle;
using ptsCogo;

namespace TinTests
{
    /// <summary>
    /// Summary description for TestTinFromLandXml
    /// </summary>
    [TestClass]
    public class TestTinFromLandXml
    {
        public TestTinFromLandXml()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private bool runGardenParkway = true;
        private ptsDTM gardenParkway { get; set; } = null;

        private void testInit()
        {
            if(this.gardenParkway == null)
                gardenParkway = ptsDTM.CreateFromExistingFile(
                @"D:\SourceModules\CSharp\CogoDN\ptsDigitalTerrainModel\Testing\XYZinput\TinTests\data\GPEtin.xml");
        }

        [TestMethod]
        public void Test_VerifyKeyLocation_InGardenParkway_LandXMLfile()
        {
            if(!runGardenParkway) return;

            testInit();

            PointSlopeAspect psa = 
                gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(529399.6100, 1408669.1900, 0.0));
            psa.AssertDerivedValuesAreEqual(671.13, 14.1, 285.94);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(529868.0890, 1407914.1624, 0.0));
            psa.AssertDerivedValuesAreEqual(654.16, 25.6, 26.6446);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(529533.0000, 1415029.0000, 0.0));
            psa.AssertDerivedValuesAreEqual(730.02, 1.4, 204.0430);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(529890.1833, 1409331.1264, 0.0));
            psa.AssertDerivedValuesAreEqual(684.01, 17.3, 298.4007);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(530069.6770, 1410934.1035, 0.0));
            psa.AssertDerivedValuesAreEqual(707.94, 14.5, 137.6170);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(531816.0608, 1402087.7063, 0.0));
            psa.AssertDerivedValuesAreEqual(603.45, 12.3, 288.7335);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(529211.7956, 1413338.5912, 0.0));
            psa.AssertDerivedValuesAreEqual(653.57, 11.1, 194.3426);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(529231.2117, 1413280.0812, 0.0));
            psa.AssertDerivedValuesAreEqual(650.76, 16.45, 294.6933);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(529987.6366, 1406533.3478, 0.0));
            psa.AssertDerivedValuesAreEqual(687.08, 1.8, 33.6581);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(530437.7030, 1402565.4940, 0.0));
            psa.AssertDerivedValuesAreEqual(610.00, 70.8, 146.5787);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(530935.4300, 1401737.1885, 0.0));
            psa.AssertDerivedValuesAreEqual(573.53, 34.3, 246.1545);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(518797.8895, 1383909.6974, 0.0));
            psa.AssertDerivedValuesAreEqual(null, null, null);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(532158.0000, 1413853.0000, 0.0));
            psa.AssertDerivedValuesAreEqual(null, null, null);

            psa = gardenParkway.getElevationSlopeAzimuth(new ptsDTMpoint(527915.0000, 1406442.0000, 0.0));
            psa.AssertDerivedValuesAreEqual(null, null, null);

        }
    }
}
