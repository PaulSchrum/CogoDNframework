using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo;
using ptsCogo.Horizontal;
using ptsCogo.coordinates.CurvilinearCoordinates;
using ptsCogo.Angle;
using System.IO;
using ptsCogo.coordinates;

namespace Tests
{
    [TestClass]
    public class HorizontalAlignmentTests
    {
        public List<Double> testList { get; set; }
        private double stdDelta { get; set; } = 0.0005;

        //[SetUp]
        public void HAtestSetup()
        {

        }

        [TestMethod]
        public void Deflection_positiveLessThan180_getAsDegrees()
        {
            Double expectedValue = 45.0;
            Deflection defl = new Deflection(0.785398164, 1);
            Double actualValue = defl.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: stdDelta);
        }

        [TestMethod]
        public void Deflection_positiveGreaterThan180_getAsDegrees()
        {
            Double expectedValue = 310.0;
            Deflection defl = new Deflection(5.41052068118, 1);
            Double actualValue = defl.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: stdDelta);
        }

        [TestMethod]
        public void Deflection_negativeLessThan180_getAsDegrees()
        {
            Double expectedValue = -45.0;
            Deflection defl = new Deflection(0.785398164, -1);
            Double actualValue = defl.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: stdDelta);
        }

        [TestMethod]
        public void Deflection_negativeGreaterThan180_getAsDegrees()
        {
            Double expectedValue = -310.0;
            Deflection defl = new Deflection(5.41052068118, -1);
            Double actualValue = defl.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: 0.00001);
        }

        [TestMethod]
        public void Deflection_positiveLessThan180_getAsRadians()
        {
            Double expectedValue = 0.785398164;
            Deflection defl = new Deflection(0.785398164, 1);
            Double actualValue = defl.getAsRadians();
            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: 0.00001);
        }

        [TestMethod]
        public void Deflection_positiveGreaterThan180_getAsRadians()
        {
            Double expectedValue = 5.41052068118;
            Deflection defl = new Deflection(5.41052068118, 1);
            Double actualValue = defl.getAsRadians();
            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: 0.00001);
        }

        [TestMethod]
        public void Deflection_negativeLessThan180_getAsRadians()
        {
            Double expectedValue = -0.39479111970;
            Azimuth begAz = new Azimuth(new ptsPoint(0.0, 0.0, 0.0), new ptsPoint(10.0, 50.0, 0.0));
            Azimuth endAz = new Azimuth(new ptsPoint(10.0, 50.0, 0.0), new ptsPoint(0.0, 100.0, 0.0));
            Deflection defl = new Deflection(begAz, endAz, true);
            Double actualValue = defl.getAsRadians();
            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: 0.0000001);
        }

        [TestMethod]
        public void Deflection_negativeGreaterThan180_getAsRadians()
        {
            Double expectedValue = -5.88839418748;
            Azimuth endAz = new Azimuth(new ptsPoint(0.0, 0.0, 0.0), new ptsPoint(10.0, 50.0, 0.0));
            Azimuth begAz = new Azimuth(new ptsPoint(10.0, 50.0, 0.0), new ptsPoint(0.0, 100.0, 0.0));
            Deflection defl = new Deflection(begAz, endAz, false);
            Double actualValue = defl.getAsRadians();
            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: 0.00001);
        }

        [TestMethod]
        public void GenericAlignment_instantiateWithBeginStationOnly()
        {
            testList = new List<Double>();
            testList.Add(1000.0);
            GenericAlignment align = new GenericAlignment(testList);

            double actualDbl = align.BeginStation;
            double expectedDbl = 1000.0;

            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0000001);
        }

        [TestMethod]
        public void GenericAlignment_instantiateWithBeginAndEndStation()
        {
            testList = new List<Double>();
            testList.Add(1000.0);
            testList.Add(2000.0);
            GenericAlignment align = new GenericAlignment(testList);

            bool actualBool = true;
            actualBool = actualBool && (0 == utilFunctions.tolerantCompare(align.BeginStation, 1000.0, 0.00001));
            actualBool = actualBool && (0 == utilFunctions.tolerantCompare(align.EndStation, 2000.0, 0.00001));

            Assert.AreEqual(expected: true, actual: actualBool);
        }

        //[TestMethod]
        public void HorizontalAlignment_instantiateSingleLine_fromNullFundamentalGeometry()
        {
          //  Assert.That(() => new rm21HorizontalAlignment(fundamentalGeometryList: (List<IRM21fundamentalGeometry>)null,
            //   Name: null, stationEquationing: null), Throws.Exception.TypeOf<NullReferenceException>());
        }

        private List<IRM21fundamentalGeometry> createFundmGeoms1()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = new List<IRM21fundamentalGeometry>();

            //rm21MockFundamentalGeometry mockFG = new rm21MockFundamentalGeometry();

            //List<ptsPoint> ptLst = new List<ptsPoint>();
            //ptLst.Add(new ptsPoint(10.0, 10.0, 0.0));
            //ptLst.Add(new ptsPoint(80.7106781188, 80.7106781188, 0.0));

            //mockFG.pointList = ptLst;
            //mockFG.expectedType = expectedType.LineSegment;

            //fundmtlGeoms.Add(mockFG);
            return fundmtlGeoms;
        }

        private List<IRM21fundamentalGeometry> createFundmGeoms_arc1()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = new List<IRM21fundamentalGeometry>();

            //rm21MockFundamentalGeometry mockFG = new rm21MockFundamentalGeometry();

            //List<ptsPoint> ptLst = new List<ptsPoint>();
            //ptLst.Add(new ptsPoint(443.176112, 569.321807, 0.0));
            //ptLst.Add(new ptsPoint(1211.097635, -23.605404, 0.0));
            //ptLst.Add(new ptsPoint(1186.397526, 946.268836, 0.0));

            //mockFG.pointList = ptLst;
            //mockFG.expectedType = expectedType.ArcSegmentInsideSolution;

            //fundmtlGeoms.Add(mockFG);
            return fundmtlGeoms;
        }

        private List<IRM21fundamentalGeometry> createFundmGeoms_arc1_butExternalSolution()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = new List<IRM21fundamentalGeometry>();

            //rm21MockFundamentalGeometry mockFG2 = new rm21MockFundamentalGeometry();

            //List<ptsPoint> ptLst = new List<ptsPoint>();
            //ptLst.Add(new ptsPoint(443.176112, 569.321807, 0.0));
            //ptLst.Add(new ptsPoint(1211.097635, -23.605404, 0.0));
            //ptLst.Add(new ptsPoint(1186.397526, 946.268836, 0.0));

            //mockFG2.pointList = ptLst;

            //mockFG2.expectedType = expectedType.ArcSegmentOutsideSoluion;
            //mockFG2.deflectionSign = -1;

            //fundmtlGeoms.Add(mockFG2);
            return fundmtlGeoms;
        }

        [TestMethod]
        public void HorizontalAlignment_ComputeDegreeOfCurveFromRadius()
        {
            Double Radius = 5729.58; Double LengthForDegreeOfCurve = 100.0;
            ptsAngle DegreeOfCurve = new ptsAngle(Radius, LengthForDegreeOfCurve);

            Double actualValue = DegreeOfCurve.getAsDegreesDouble();
            Double expectedValue = 1.00;

            Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: 0.000001);
        }

        //[TestMethod]
        public void HorizontalAlignment_instantiateSingleLine_fromFundamentalGeometry()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = createFundmGeoms1();

            rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
               fundamentalGeometryList: fundmtlGeoms,
               Name: null, stationEquationing: null);

            Assert.IsNotNull(HA);
        }

        //[TestMethod]
        public void HorizontalAlignment_instantiateSingleLine_fromFundamentalGeometry_HAlengthIs100()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = createFundmGeoms1();

            rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
               fundamentalGeometryList: fundmtlGeoms,
               Name: null, stationEquationing: null);

            double actualLength = HA.EndStation - HA.BeginStation;
            double expectedLength = 100.0;

            Assert.AreEqual(expected: expectedLength, actual: actualLength, delta: 0.00001);

        }

        //[TestMethod]
        public void HorizontalAlignment_instantiateSingleArcInterior_fromFundamentalGeometry_HAlengthIs861()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = createFundmGeoms_arc1();

            rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
               fundamentalGeometryList: fundmtlGeoms,
               Name: null, stationEquationing: null);

            double actualLength = HA.EndStation - HA.BeginStation;
            double expectedLength = 861.359280;

            Assert.AreEqual(expected: expectedLength, actual: actualLength, delta: 0.00001);

        }

        //[TestMethod]
        public void HorizontalAlignment_instantiateSingleArcExterior_fromFundamentalGeometry_HAlengthIs5235()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = createFundmGeoms_arc1_butExternalSolution();

            rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
               fundamentalGeometryList: fundmtlGeoms,
               Name: null, stationEquationing: null);

            double actualLength = HA.EndStation - HA.BeginStation;
            double expectedLength = 5234.5162;

            Assert.AreEqual(expected: expectedLength, actual: actualLength, delta: 0.00001);

        }

        //[TestMethod]
        public void HorizontalAlignment_instantiateFromFundamentalGeometry_shouldNotThrowException()
        {
            var fgList = new List<IRM21fundamentalGeometry>();
            //var funGeomItem = new rm21MockFundamentalGeometry();

            //// Line 1, Item [0], geometric sequence 1
            //funGeomItem.pointList.Add(new ptsPoint(2078236.6862, 746878.8969, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(2078312.7571, 746940.1485, 0.0));
            //funGeomItem.expectedType = expectedType.LineSegment;
            //fgList.Add(funGeomItem);

            //// Line 2, Item [1], geometric sequence 3
            //funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(2078814.5136, 747096.7721, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(2079329.4845, 747059.2340, 0.0));
            //funGeomItem.expectedType = expectedType.LineSegment;
            //fgList.Add(funGeomItem);

            //// Arc 1, Item [2], geometric sequence 2, oriented backward
            //funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(2078814.5136, 747096.7721, 0.0)); // beg matches [0].end
            //funGeomItem.pointList.Add(new ptsPoint(2078762.3916, 746381.7284, 0.0)); // center pt
            //funGeomItem.pointList.Add(new ptsPoint(2078312.7571, 746940.1485, 0.0)); // end matches [1].begin
            //funGeomItem.expectedType = expectedType.ArcSegmentInsideSolution;
            //funGeomItem.deflectionSign = 1;
            //fgList.Add(funGeomItem);

            //try
            //{
            //    var HA = new rm21HorizontalAlignment(
            //       fundamentalGeometryList: fgList,
            //       Name: null, stationEquationing: null);
            //}
            //catch(Exception e)
            //{ Assert.Fail("Exception not expected: \"" + e.Message + "\""); }
        }

        //[TestMethod]
        public void HorizontalAlignment_instantiate5ItemHA_fromFundamentalGeometry_HAlengthIs7155()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = createTestHA_fundGeom1();

            rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
               fundamentalGeometryList: fundmtlGeoms,
               Name: null, stationEquationing: null);

            double actualLength = HA.EndStation - HA.BeginStation;
            double expectedLength = 7154.9385;

            Assert.AreEqual(expected: expectedLength, actual: actualLength, delta: 0.00015);

        }

        private List<IRM21fundamentalGeometry> createTestHA_fundGeom1()
        {
            // Code editors Note:  Items 4 and 5 are deliberately swapped.
            // Please do not change this.  The point of swapping them is to
            //    test the process of putting the items in the correct order.
            var returnList = new List<IRM21fundamentalGeometry>();
            /* var funGeomItem = new rm21MockFundamentalGeometry(); */

            // Line 1, Item 1
            //funGeomItem.pointList.Add(new ptsPoint(3556.2226, 2526.6156, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(4932.6214, 4309.2396, 0.0));
            //funGeomItem.expectedType = expectedType.LineSegment;
            //returnList.Add(funGeomItem);

            //// Arc 1, Item 2
            //funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(4932.6214, 4309.2396, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(5700.5429, 3716.3124, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(5675.8428, 4686.1866, 0.0));
            //funGeomItem.expectedType = expectedType.ArcSegmentInsideSolution;
            //funGeomItem.deflectionSign = 1;
            //returnList.Add(funGeomItem);

            //// Line 2, Item 3
            //funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(5675.8428, 4686.1866, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(6624.6701, 4710.3507, 0.0));
            //funGeomItem.expectedType = expectedType.LineSegment;
            //returnList.Add(funGeomItem);

            //// Line 3, Item 5
            //funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(7738.3259, 5168.4199, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(9093.6332, 6443.7163, 0.0));
            //funGeomItem.expectedType = expectedType.LineSegment;
            //returnList.Add(funGeomItem);

            //// Arc 2, Item 4
            //funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(6624.6701, 4710.3507, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(6581.7001, 6397.6113, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(7738.3259, 5168.4199, 0.0));
            //funGeomItem.expectedType = expectedType.ArcSegmentInsideSolution;
            //funGeomItem.deflectionSign = -1;
            //returnList.Add(funGeomItem);

            return returnList;
        }

        //[TestMethod]
        public void HorizontalAlignment_singleArcHAinsideSolutionRight_fromFundamentalGeometry_HAlengthIs666()
        {
            //var funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(6903.1384, 3830.6151, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(7458.9796, 3830.6151, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(7257.2446, 4348.5557, 0.0));
            //funGeomItem.expectedType = expectedType.ArcSegmentInsideSolution;
            //funGeomItem.deflectionSign = 1;
            //var fGeomList = new List<IRM21fundamentalGeometry>();
            //fGeomList.Add(funGeomItem);


            //rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
            //   fundamentalGeometryList: fGeomList,
            //   Name: null, stationEquationing: null);

            //double actualLength = HA.EndStation - HA.BeginStation;
            //double expectedLength = 666.6644;

            //Assert.AreEqual(expected: expectedLength, actual: actualLength, delta: 0.00015);

        }

        //[TestMethod]
        public void HorizontalAlignment_singleArcHAOutsideSolutionRight_fromFundamentalGeometry_HAlengthIs2439()
        {
            //var funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(7415.7202, 4384.7704, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(7458.9796, 3830.6151, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(6947.3437, 3613.3867, 0.0));
            //funGeomItem.expectedType = expectedType.ArcSegmentOutsideSoluion;
            //funGeomItem.deflectionSign = 1;
            //var fGeomList = new List<IRM21fundamentalGeometry>();
            //fGeomList.Add(funGeomItem);


            //rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
            //   fundamentalGeometryList: fGeomList,
            //   Name: null, stationEquationing: null);

            //double actualLength = HA.EndStation - HA.BeginStation;
            //double expectedLength = 2439.4665;

            //Assert.AreEqual(expected: expectedLength, actual: actualLength, delta: 0.00015);

        }

        //[TestMethod]
        public void HorizontalAlignment_singleArcHAInsideSolutionLeft_fromFundamentalGeometry_HAlengthIs1051()
        {
            //var funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(4003.3849, 4491.7185, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(3995.0953, 5346.2102, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(4803.1466, 5068.2214, 0.0));
            //funGeomItem.expectedType = expectedType.ArcSegmentInsideSolution;
            //funGeomItem.deflectionSign = -1;
            //var fGeomList = new List<IRM21fundamentalGeometry>();
            //fGeomList.Add(funGeomItem);

            //rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
            //   fundamentalGeometryList: fGeomList,
            //   Name: null, stationEquationing: null);

            //double actualLength = HA.EndStation - HA.BeginStation;
            //double expectedLength = 1050.8644;

            //Assert.AreEqual(expected: expectedLength, actual: actualLength, delta: 0.00015);

        }

        //[TestMethod]
        public void HorizontalAlignment_singleArcHAOutsideSolutionLeft_fromFundamentalGeometry_HAlengthIs4038()
        {

        }

        //[TestMethod]
        public void HorizontalAlignment_singleArcHAOutsideSolutionLeft_fromFundamentalGeometry_HAlengthIs3396()
        {
            //var funGeomItem = new rm21MockFundamentalGeometry();
            //funGeomItem.pointList.Add(new ptsPoint(6925.6663, 6218.7689, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(6540.7903, 5615.8802, 0.0));
            //funGeomItem.pointList.Add(new ptsPoint(5952.2191, 6022.3138, 0.0));
            //funGeomItem.expectedType = expectedType.ArcSegmentOutsideSoluion;
            //funGeomItem.deflectionSign = 1;
            //var fGeomList = new List<IRM21fundamentalGeometry>();
            //fGeomList.Add(funGeomItem);


            //rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
            //   fundamentalGeometryList: fGeomList,
            //   Name: null, stationEquationing: null);

            //double actualLength = HA.EndStation - HA.BeginStation;
            //double expectedLength = 3396.4881;

            //Assert.AreEqual(expected: expectedLength, actual: actualLength, delta: 0.0045);

        }

        [TestMethod]
        public void angleNormalization_withinPlusOrMinus2Pi_OverPositive2PI()
        {
            Double angleNeedingToBeNormalized = 2 * Math.PI * 4.56;
            Double expectedAfterNormalized = 2 * Math.PI * 0.56;
            ptsAngle anAngle = new ptsAngle();
            Double actualAfterNormalization =
               ptsAngle.ComputeRemainderScaledByDenominator(angleNeedingToBeNormalized, 2 * Math.PI);

            Assert.AreEqual(expected: expectedAfterNormalized,
               actual: actualAfterNormalization, delta: 0.0000001);
        }

        [TestMethod]
        public void angleNormalization_withinPlusOrMinus2Pi_UnderNegative2PI()
        {
            Double angleNeedingToBeNormalized = -710.0;
            Double expectedAfterNormalized = -350.0;
            ptsAngle anAngle = new ptsAngle();
            Double actualAfterNormalization =
               ptsAngle.ComputeRemainderScaledByDenominator(angleNeedingToBeNormalized, 360.0);

            Assert.AreEqual(expected: expectedAfterNormalized,
               actual: actualAfterNormalization, delta: 0.0000001);
        }

        //[TestMethod]
        public void HorizontalAlignment_givenStationOffsetValues_getXYvalues()
        {
            bool allValuesAgree = true;
            List<IRM21fundamentalGeometry> fundmtlGeoms = createTestHA_fundGeom1();

            rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
               fundamentalGeometryList: fundmtlGeoms,
               Name: null, stationEquationing: null);

            StationOffsetElevation anSOE = new StationOffsetElevation();

            // test a point right of the second line segment (the third segment)
            anSOE.station = 3611.75; anSOE.offset.OFST = 238.949;
            ptsPoint anXYpoint = HA.getXYZcoordinates(anSOE);
            if(anXYpoint != null)
            {
                allValuesAgree &= anXYpoint.x.tolerantEquals(6180.0, 0.014);
                allValuesAgree &= anXYpoint.y.tolerantEquals(4460.0, 0.014);
                allValuesAgree &= anXYpoint.z.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            // test a point right of the second arc (the fourth segment)
            anSOE.station = 4469.2978; anSOE.offset.OFST = 138.1336;
            anXYpoint = HA.getXYZcoordinates(anSOE);
            if(anXYpoint != null)
            {
                allValuesAgree &= anXYpoint.x.tolerantEquals(7062.3839, 0.014);
                allValuesAgree &= anXYpoint.y.tolerantEquals(4636.0766, 0.014);
                allValuesAgree &= anXYpoint.z.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            Assert.IsTrue(allValuesAgree);
        }

        //[TestMethod]
        public void HorizontalAlignment_givenBeginStationAndLeftOffset_getXYcoordinates()
        {
            var HA = new rm21HorizontalAlignment(
               fundamentalGeometryList: createTestHA_fundGeom1(),
               Name: null, stationEquationing: null);

            var actual = HA.getXYZcoordinates(0.0, -20.0, 0.0);
            Assert.AreEqual(expected: 3540.3922, actual: actual.x, delta: 0.00015);
            Assert.AreEqual(expected: 2538.8385, actual: actual.y, delta: 0.00015);

        }

        [TestMethod]
        public void ArcSegment_ctor1_correct()
        {
            var begPt = new ptsPoint(2152973.8702, 735330.7239);
            var endPt = new ptsPoint(2153151.3148, 735350.0147);
            var inAz = Azimuth.fromDegreesDouble(77.5488);
            var radius = 820.2083;

            //var anArc = new rm21HorArc(begPt, endPt, inAz, radius);
            var anArc = rm21HorArc.Create(begPt, endPt, inAz, radius);

            var actualDeflection = anArc.Deflection.getAsDegreesDouble();
            Assert.AreEqual(expected: 12.4932, actual: actualDeflection, delta: 0.00015);

            var actualLength = anArc.Length;
            Assert.AreEqual(expected: 178.8443, actual: actualLength, delta: 0.00015);
        }

        //[TestMethod]
        public void HorizontalAlignment_givenXYvalues_getStationOffsetValues()
        {
            List<IRM21fundamentalGeometry> fundmtlGeoms = createTestHA_fundGeom1();

            rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
               fundamentalGeometryList: fundmtlGeoms,
               Name: null, stationEquationing: null);

            StationOffsetElevation anSOE = null;
            bool allValuesAgree = true;

            // test point on first tangent
            ptsPoint somePoint = new ptsPoint(4046.2915, 3161.3216, 0.0);
            var soePoints = HA.getStationOffsetElevation(somePoint);
            if(soePoints != null && soePoints.Count > 0)
            {
                anSOE = soePoints.FirstOrDefault();
                allValuesAgree &= anSOE.station.tolerantEquals(801.8849, 0.00014);
                allValuesAgree &= anSOE.offset.OFST.tolerantEquals(0.0, 0.00014);
                allValuesAgree &= anSOE.elevation.EL.tolerantEquals(0.0, 0.00000001);
            }
            else
            {
                allValuesAgree = false;
            }

            // test point which is before the beginning of the HA
            somePoint = new ptsPoint(2500.0, 1000.0, 0.0);
            soePoints = HA.getStationOffsetElevation(somePoint);
            allValuesAgree &= soePoints.Count == 0;

            // test point which is beyond the end of the HA
            somePoint = new ptsPoint(9554.0, 9000.0, 0.0);
            soePoints = HA.getStationOffsetElevation(somePoint);
            allValuesAgree &= soePoints.Count == 0;

            // test point offset from first tangent
            somePoint.x = 4516.0; somePoint.y = 3404.0;
            soePoints = HA.getStationOffsetElevation(somePoint);
            if(soePoints != null && soePoints.Count > 0)
            {
                anSOE = soePoints.FirstOrDefault();
                allValuesAgree &= anSOE.station.tolerantEquals(1281.0297, 0.00014);
                allValuesAgree &= anSOE.offset.OFST.tolerantEquals(223.4706, 0.00014);
                allValuesAgree &= anSOE.elevation.EL.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            // test point on first arc
            somePoint.x = 5494.3772; somePoint.y = 4664.3429;
            soePoints = HA.getStationOffsetElevation(somePoint);
            if(soePoints != null && soePoints.Count > 0)
            {
                anSOE = soePoints.FirstOrDefault();
                allValuesAgree &= anSOE.station.tolerantEquals(2930.4718, 0.00014);
                allValuesAgree &= anSOE.offset.OFST.tolerantEquals(0.0, 0.00014);
                allValuesAgree &= anSOE.elevation.EL.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            // test point offset right of second arc
            somePoint.x = 6918.0; somePoint.y = 4557.0;
            soePoints = HA.getStationOffsetElevation(somePoint);
            if(soePoints != null && soePoints.Count > 0)
            {
                anSOE = soePoints.FirstOrDefault();
                allValuesAgree &= anSOE.station.tolerantEquals(4324.6956, 0.00014);
                allValuesAgree &= anSOE.offset.OFST.tolerantEquals(183.2743, 0.00014);
                allValuesAgree &= anSOE.elevation.EL.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            // test point offset left of second arc
            somePoint.x = 7103.0; somePoint.y = 4979.0;
            soePoints = HA.getStationOffsetElevation(somePoint);
            if(soePoints != null && soePoints.Count > 0)
            {
                anSOE = soePoints.FirstOrDefault();
                allValuesAgree &= anSOE.station.tolerantEquals(4614.0481, 0.00014);
                allValuesAgree &= anSOE.offset.OFST.tolerantEquals(-176.4468, 0.00014);
                allValuesAgree &= anSOE.elevation.EL.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            // test point offset left of third line (fifth segment
            somePoint.x = 8071.7032; somePoint.y = 5913.8278;
            soePoints = HA.getStationOffsetElevation(somePoint);
            if(soePoints != null && soePoints.Count > 0)
            {
                anSOE = soePoints.FirstOrDefault();
                allValuesAgree &= anSOE.station.tolerantEquals(6047.5668, 0.00014);
                allValuesAgree &= anSOE.offset.OFST.tolerantEquals(-314.4057, 0.00014);
                allValuesAgree &= anSOE.elevation.EL.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            // test point offset at center of first arc
            somePoint.x = 5700.5429; somePoint.y = 3716.3124;
            soePoints = HA.getStationOffsetElevation(somePoint);
            if(soePoints != null && soePoints.Count > 0)
            {
                anSOE = soePoints.FirstOrDefault();
                allValuesAgree &= anSOE.station.tolerantEquals(2252.1594, 0.00014);
                allValuesAgree &= anSOE.offset.OFST.tolerantEquals(970.1887, 0.00014);
                allValuesAgree &= anSOE.elevation.EL.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            // test point offset right that returns 2 SOE instances for the same point
            somePoint.x = 5940.0; somePoint.y = 3310.0;
            soePoints = HA.getStationOffsetElevation(somePoint);
            if(soePoints != null && soePoints.Count > 0)
            {
                allValuesAgree &= (soePoints.Count == 2);
                anSOE = soePoints.FirstOrDefault();
                allValuesAgree &= anSOE.station.tolerantEquals(3342.5539, 0.00014);
                allValuesAgree &= anSOE.offset.OFST.tolerantEquals(1382.4657, 0.00014);
                allValuesAgree &= anSOE.elevation.EL.tolerantEquals(0.0, 0.000001);
            }
            else
            {
                allValuesAgree = false;
            }

            Assert.IsTrue(allValuesAgree);
        }

        private rm21HorizontalAlignment createSingleTangentHA()
        {
            var newHA = new rm21HorizontalAlignment();
            newHA.reset(new ptsPoint(2082268.0907, 740846.3249, 0.0),
               new ptsPoint(2082339.9608, 740834.3849, 0.0));
            return newHA;
        }

        [TestMethod]
        public void HorizontalAlignment_from2points_producesCorrectValues()
        {
            rm21HorizontalAlignment HA = createSingleTangentHA();

            double expectedDbl = 72.8552;
            double actualDbl = (Double)HA.EndStation;
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0001);

            expectedDbl = 99.43257281767;
            Azimuth az = HA.EndAzimuth;
            actualDbl = az.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0001);

            HA.reset(new ptsPoint(100.0, 100.0, 0.0), new ptsPoint(80.0, 150.0, 0.0));

            expectedDbl = 53.8516480713;
            actualDbl = (Double)HA.EndStation;
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0001);

            expectedDbl = 338.198590514;
            actualDbl = HA.EndAzimuth.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0001);

        }

        [TestMethod]
        public void HorizontalAlignment_appendSimpleArcToTangent_DPonAlignment_EndStationIs391p78()
        {
            Double radius = 1170.0;
            var HA = createSingleTangentHA();
            HA.appendArc(ArcEndPoint: new ptsPoint(2082657.7727, 740825.3769, 0.0),
               radius: radius);

            Double expectedDbl = 391.7812;
            Double actualDbl = (Double)HA.EndStation;
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0001);

            expectedDbl = -15.61806;
            actualDbl = HA.GetElementByStation(390.0).Deflection.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0001);
        }

        [TestMethod]
        public void HorizontalAlignment_appendSimpleArcToTangent_DPoffAlignment_EndStationIs391p78()
        {
            Double radius = 1170.0;
            var HA = createSingleTangentHA();
            HA.appendArc(ArcEndPoint: new ptsPoint(2082652.9833, 740869.5684, 0.0),
               radius: radius);

            Double expectedDbl = 391.7812;
            Double actualDbl = (Double)HA.EndStation;
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0001);

            expectedDbl = -15.61806;
            actualDbl = HA.GetElementByStation(390.0).Deflection.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0001);
        }

        [TestMethod]
        public void HorizontalAlignment_appendTangentToSimpleArc_EndStationIs481p78()
        {
            Double radius = 1170.0;
            var HA = createSingleTangentHA();
            HA.appendArc(ArcEndPoint: new ptsPoint(2082660.0, 740870.0, 0.0),
               radius: radius);

            Double expectedDbl = -15.618046;
            Double actualDbl = HA.GetElementByStation(390.0).Deflection.getAsDegreesDouble();
            Assert.AreNotEqual(notExpected: expectedDbl, actual: actualDbl);

            HA.appendTangent(TangentEndPoint: new ptsPoint(2082747.242780, 740835.073448, 0.0));

            expectedDbl = -15.61806;
            actualDbl = HA.GetElementByStation(390.0).Deflection.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.000045);
            // Note: The error is 1 inch over 20 miles -- acceptable accuracy.
        }

        [TestMethod]
        public void HorizontalAlignment_appendTangentToSimpleArc_ArcEndPtAtRightLocation()
        {
            Double radius = 1170.0;
            var HA = createSingleTangentHA();
            HA.appendArc(ArcEndPoint: new ptsPoint(2082660.0, 740870.0, 0.0),
               radius: radius);

            Double expectedDbl = -15.618046;
            Double actualDbl = HA.GetElementByStation(390.0).Deflection.getAsDegreesDouble();
            Assert.AreNotEqual(notExpected: expectedDbl, actual: actualDbl);

            HA.appendTangent(TangentEndPoint: new ptsPoint(2082747.242780, 740835.073448, 0.0));

            ptsPoint arcEndPt = HA.GetElementByStation(390.0).EndPoint;
            expectedDbl = 2082657.772726;
            actualDbl = arcEndPt.x;
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.001);

            expectedDbl = 740825.376857;
            actualDbl = arcEndPt.y;
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.001);
        }

        [TestMethod]
        public void HorizontalAlignment_appendTangentToSimpleArc_DeflectionIsToTheRight()
        {
            var HA = new rm21HorizontalAlignment();
            HA.reset(new ptsPoint(2083115.2982, 740931.8698, 0.0),
               new ptsPoint(X: 2083652.8068, Y: 741163.1806, Z: 0.0));

            HA.appendArc(new ptsPoint(X: 2084073.2987, Y: 741255.4852, Z: 0.0), 1138.0);
            HA.appendTangent(new ptsPoint(X: 2084518.1573, Y: 741266.9617, Z: 0.0));

            Double expectedDbl = 21.806287;
            Double actualDbl = HA.GetElementByStation(1010.0).Deflection.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.000045);
        }

        private rm21HorizontalAlignment buildFrehandHAforTesting()
        {
            var retHA = new rm21HorizontalAlignment();
            retHA.reset(new ptsPoint(X: 2082268.0907, Y: 740846.3249, Z: 0.0),
               new ptsPoint(X: 2082339.9608, Y: 740834.3849, Z: 0.0));

            retHA.appendArc(new ptsPoint(X: 2082657.7727, Y: 740825.3769, Z: 0.0), 1170.0);
            retHA.appendTangent(new ptsPoint(X: 2082747.2428, Y: 740835.0734, Z: 0.0));

            retHA.appendArc(new ptsPoint(X: 2083115.2982, Y: 740931.8698, Z: 0.0), 1280.0);
            retHA.appendTangent(new ptsPoint(X: 2083652.8068, Y: 741163.1806, Z: 0.0));

            retHA.appendArc(new ptsPoint(X: 2084073.2987, Y: 741255.4852, Z: 0.0), 1138.0);
            retHA.appendTangent(new ptsPoint(X: 2084518.1573, Y: 741266.9617, Z: 0.0));

            return retHA;
        }

        [TestMethod]
        public void HorizontalAlignment_buildFreehand_EndStationIs2327p0486()
        {
            var HA = buildFrehandHAforTesting();
            Assert.IsNotNull(HA);

            Double expected = 2327.0486;
            Double actual = HA.Length;
            Assert.AreEqual(expected: expected, actual: actual, delta: 0.0025);
        }

        [TestMethod]
        public void HorizontalAlignment_buildFreehand_PointIs8LeftOf2000()
        {
            var HA = buildFrehandHAforTesting();
            Assert.IsNotNull(HA);

            StationOffsetElevation soe = new StationOffsetElevation(2000.0, -8.0, 0.0);
            var pt = HA.getXYZcoordinates(soe);

            Double expectedX = 2084191.011134;
            Double actualX = pt.x;
            Assert.AreEqual(expected: expectedX, actual: actualX, delta: 0.0025);
            Double expectedY = 741266.52458;
            Double actualY = pt.y;
            Assert.AreEqual(expected: expectedY, actual: actualY, delta: 0.0025);
        }

        [TestMethod]
        public void HorizontalAlignment_buildFreehand_5LeftOf2200isPoint()
        {
            var HA = buildFrehandHAforTesting();
            Assert.IsNotNull(HA);

            var soe = HA.getStationOffsetElevation(new ptsPoint(2084391.021982, 741268.683499));
            Assert.IsNotNull(soe);
            Assert.AreEqual(expected: 1, actual: soe.Count);

            Double expectedStation = 2200.0;
            Double actualStation = soe[0].station;
            Assert.AreEqual(expected: expectedStation, actual: actualStation, delta: 0.0025);
            Double expectedOffset = -5.0;
            Double actualOffset = soe[0].offset;
            Assert.AreEqual(expected: expectedOffset, actual: actualOffset, delta: 0.0025);
        }

        [TestMethod]
        public void HorizontalAlignment_instantiateArcSegment_RayForm_isCorrect()
        {
            Azimuth startAz = 77.5488;
            var beginRay = new ptsRay(
                new ptsPoint(2152973.8702, 735330.7239),
                startAz
                );
            var expectedEndRay = new ptsRay(
                new ptsPoint(2153151.3148, 735350.0147),
                Azimuth.fromDegreesDouble(90.0421)
                );
            double arcLength = 178.8443;
            double radius = 820.2083;
            double Dc = radius.dblDegreeFromRadius();
            rm21HorArc newArcRight = (rm21HorArc) rm21HorizontalAlignment.newSegment(beginRay, Dc, arcLength, Dc);

            var expCenterPt = new ptsPoint(2153150.7135, 734529.8066);
            Assert.AreEqual(expected: expCenterPt, actual: newArcRight.ArcCenterPt);

            Azimuth expAheadAz = 90.0421;
            Assert.AreEqual(expected: expAheadAz, actual: newArcRight.EndAzimuth);

            double expLongChordLength = 178.4902;
            Assert.AreEqual(expected: expLongChordLength, actual: newArcRight.LongChordVector.Length, delta: 0.0005);

            Assert.AreEqual(expected: arcLength, actual: newArcRight.Length, delta: 0.0005);

            int expectedDeflSign = 1;
            int actualDeflSign = newArcRight.Deflection.deflectionDirection;
            Assert.AreEqual(expected: expectedDeflSign, actual: actualDeflSign);

            // now flip it to be curving left and recheck.
            Deflection arcDef = newArcRight.Deflection * -1.0;
            Dc *= -1.0;
            rm21HorArc newArcLeft = (rm21HorArc)rm21HorizontalAlignment.newSegment(beginRay, Dc, arcLength, Dc);

            expCenterPt = new ptsPoint(2152797.0270, 736131.6405);
            //Assert.AreEqual(expected: expCenterPt, actual: newArcLeft.ArcCenterPt);

            expAheadAz = 65.0556;
            Assert.AreEqual(expected: expAheadAz, actual: newArcLeft.EndAzimuth);

            expLongChordLength = 178.4902;
            Assert.AreEqual(expected: expLongChordLength, actual: newArcLeft.LongChordVector.Length, delta: 0.0005);

            Assert.AreEqual(expected: arcLength, actual: newArcLeft.Length, delta: 0.0005);

            expectedDeflSign = -1;
            actualDeflSign = newArcLeft.Deflection.deflectionDirection;
            Assert.AreEqual(expected: expectedDeflSign, actual: actualDeflSign);
        }

        [TestMethod]
        public void HorizontalAlignment_instantiateTangent_RayForm_isCorrect()
        {
            Azimuth startAz = 110.4920;
            var beginRay = new ptsRay(
                new ptsPoint(2139755.8215, 735223.4531),
                startAz
                );
            var expectedEndRay = new ptsRay(
                new ptsPoint(2139970.4859, 735143.2274),
                Azimuth.fromDegreesDouble(110.4920)
                );

            double tangentLength = 229.1658;
            var newTangent = rm21HorizontalAlignment.newSegment(beginRay, 0.0, tangentLength, 0.0);

            Assert.AreEqual(expected: beginRay, actual: newTangent.BeginRay);
            Assert.AreEqual(expected: expectedEndRay, actual: newTangent.EndRay);

        }

        [TestMethod]
        public void HorizontalAlignment_instantiates_fromCSV_noSpirals()
        {
            var directory = new DirectoryManager();
            directory.CdUp(2).CdDown("CogoTests").CdDown("R2547");
            string testFile = directory.GetPathAndAppendFilename("ACC_REV.csv");

            rm21HorizontalAlignment AccRev = rm21HorizontalAlignment.createFromCsvFile(testFile);
            Assert.IsNotNull(AccRev);

            var actualItemCount = AccRev.childCount();
            Assert.AreEqual(expected: 17, actual: actualItemCount);

            Assert.AreEqual(expected: new ptsPoint(2139755.8215, 735223.4531),
                actual: AccRev.BeginPoint);

            Assert.AreEqual(expected: new ptsPoint(2142363.2412, 734302.9399),
                actual: AccRev.EndPoint);

            var actualStation = AccRev.BeginStation;
            double expectedStation = 1000.0;
            Assert.AreEqual(expected: expectedStation, 
                actual: actualStation, delta: stdDelta);

            expectedStation = 3895.032;
            Assert.AreEqual(expected: expectedStation,
                actual: AccRev.EndStation, delta: 0.00011);
        }

        // Todo: Add test for instantiate from csv file in which the stationing
        // includes at least one equality

        [TestMethod]
        public void HorizontalAlignment_fromCSV_stationOffsets_mapCorrectly()
        {
            var directory = new DirectoryManager();
            directory.CdUp(2).CdDown("CogoTests").CdDown("R2547");
            string testFile = directory.GetPathAndAppendFilename("Y15A.csv");

            rm21HorizontalAlignment Y15A = rm21HorizontalAlignment.createFromCsvFile(testFile);
            Assert.IsNotNull(Y15A);

            var soeList = Y15A.getStationOffsetElevation(
                new ptsPoint(655966.5667, 224111.1887));

            Assert.IsNotNull(soeList);
            Assert.AreEqual(expected: 1, actual: soeList.Count);
            var actualSOE = soeList.FirstOrDefault();
            StationOffsetElevation expectedSOE = new StationOffsetElevation(1120.0, 0.0, 0.0);
            Assert.AreEqual(expected: expectedSOE.station, actual: actualSOE.station, delta: stdDelta);

            soeList = Y15A.getStationOffsetElevation(
                new ptsPoint(655896.9056, 224002.7434));
            Assert.AreEqual(expected: 2, actual: soeList.Count);

            ptsPoint actualPoint = Y15A.getXYZcoordinates(1500.0, -2.0, 0.0);
            ptsPoint expectedPoint = new ptsPoint(656344.166,224137.087);
            Assert.AreEqual(expected: expectedPoint, actual: actualPoint);

        }

        [TestMethod]
        public void HorizontalAlignment_GetCoordinatesByStation_multiple()
        {
            var directory = new DirectoryManager();
            directory.CdUp(2).CdDown("CogoTests").CdDown("R2547");
            string testFile = directory.GetPathAndAppendFilename("Y15A.csv");

            rm21HorizontalAlignment Y15A = rm21HorizontalAlignment.createFromCsvFile(testFile);

            var allPoints = Y15A.getXYZcoordinateList(10.0);
            int actualCountOfStations = allPoints.Count;
            int expectedCount = 75;
            Assert.AreEqual(expected: expectedCount,
                actual: actualCountOfStations);
        }

        [TestMethod]
        public void EulerSpiral_createType1__correct()
        {
            var inRay = new ptsRay(new ptsPoint(0.0, 0.0), Azimuth.fromDegreesDouble(90.0));
            var aSpiral = rm21HorSpiralc.Create(inRay, 
                length: 60.0, degreeIn: 0.0, degreeOut: -95.4929659);

            Assert.IsNotNull(aSpiral);

            Double expectedThetaS = -28.64789;
            Double actualThetaS = aSpiral.Deflection.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedThetaS,actual: actualThetaS, delta: 0.00001);

            Double expectedX = 58.517;
            Double expectedY = 9.823;
            Double actualX = aSpiral.spiralDX.Length;
            Double actualY = aSpiral.spiralDY.Length;
            Assert.AreEqual(expectedX, actualX, 0.0015);
            Assert.AreEqual(expectedY, actualY, 0.0015);

            var expectedPt = new ptsPoint(19.994, 0.370);
            var sta20 = new StationOffsetElevation(20.0, 0.0, 0.0);
            var actualPt = aSpiral.getXYZcoordinates(sta20);
            Assert.IsTrue(expectedPt.Equals(actualPt));
            Assert.AreEqual(expected: 176.8178,
                    actual: aSpiral.getPerpandicularAzimuth(sta20.station).getAsDegreesDouble(),
                    delta: 0.001);

            expectedPt = new ptsPoint(39.803, 2.953);
            var sta40 = new StationOffsetElevation(40.0, 0.0, 0.0);
            actualPt = aSpiral.getXYZcoordinates(sta40);
            Assert.IsTrue(expectedPt.Equals(actualPt));
            Assert.AreEqual(expected: 167.268,
                    actual: aSpiral.getPerpandicularAzimuth(sta40.station).getAsDegreesDouble(),
                    delta: 0.001);

            expectedPt = new ptsPoint(19.938, 1.369);
            actualPt = aSpiral.getXYZcoordinates(new StationOffsetElevation(20.0, -1.0, 0.0));
            Assert.IsTrue(expectedPt.Equals(actualPt));

            expectedPt = new ptsPoint(40.244, 1.002);
            actualPt = aSpiral.getXYZcoordinates(new StationOffsetElevation(40.0, 2.0, 0.0));
            Assert.IsTrue(expectedPt.Equals(actualPt));

            //var expecteStationOffset = new StationOffsetElevation(0020.0, 0.0);
            //var testPoint = new ptsPoint(X: 19.9938, Y: 0.3703);
            //var actualStationOffset = aSpiral.getStationOffsetElevation(testPoint);
        }
    }


    public class DirectoryManager
    {  // From GitHubGist: https://gist.github.com/PaulSchrum/4fb6015d46d79c06b08acb7f1bb00c53
        // If I add other things (like createDir or move, etc. The version here should be updated.
        public string GetPathAndAppendFilename(string filename = null)
        {
            if(filename == null || filename.Length == 0)
                return this.path;
            return this.path + "\\" + filename;
        }

        public string path { get; protected set; }
        public List<string> pathAsList
        {
            get
            {
                return this.path.Split('\\').ToList();
            }
        }

        protected void setPathFromList(List<string> aList)
        {
            this.path = string.Join("\\", aList);
        }

        public DirectoryManager()
        {
            this.path = System.IO.Directory.GetCurrentDirectory();
        }

        public int depth
        {
            get
            {
                return pathAsList.Count - 1;
            }
        }

        public DirectoryManager CdUp(int upSteps)
        {
            if(upSteps > this.depth) throw new IOException("Can't cd up that high.");
            var wd = this.pathAsList.Take(depth - upSteps).ToList();
            this.setPathFromList(wd);
            return this;
        }

        public DirectoryManager CdDown(string directoryName)
        {
            if(!this.SubDirectories.Contains(directoryName))
                throw new DirectoryNotFoundException();
            var tempList = this.pathAsList;
            tempList.Add(directoryName);
            this.setPathFromList(tempList);
            return this;
        }

        public List<string> SubDirectories
        { get {
                var v = Directory.GetDirectories(this.path)
                    .Select(s => s.Split('\\').Last())
                    .ToList();
                return v;
            } }

        public override string ToString()
        {
            return this.path;
        }
    }

    internal class pointRadiusPair
    {
        public ptsPoint point { get; set; }
        public Double radius { get; set; }
    }


}