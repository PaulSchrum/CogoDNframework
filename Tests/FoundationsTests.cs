using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo;
using ptsCogo.Angle;

namespace Tests
{
    [TestClass]
    public class FoundationsTests
    {
        private Double delta = 0.0000001;

        [TestMethod]
        public void ptsVector_additonVpV_returnsCorrect()
        {
            ptsVector v1 = new ptsVector(0.9, 0.1, 0.0);
            ptsVector v2 = new ptsVector(0.1, 0.9, 0.0);
            var vResult = v1 + v2;
            Double actualLength = vResult.Length;
            Double expectedLength = 1.41421356237;
            Assert.AreEqual(expected: expectedLength,
                actual: actualLength, delta: delta);
        }

        [TestMethod]
        public void ptsAngle_division_isCorrect()
        {
            var anAngle = ptsAngle.DEGREE * 180.0;
            var angleOverTwo = anAngle / 2.0;

            var expectedDegrees = 90.0;
            var actualDegrees = angleOverTwo.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedDegrees, actual: actualDegrees, delta: 0.0001);
        }

        [TestMethod]
        public void ptsAngle_verify_customRangeNormalization()
        {
            // Test basis 0 -- 360
            double inputVal = 360.0 + 20.0;
            double actual = ptsAngle.normalizeToCustomRange(inputVal, 0.0, 360.0);
            Assert.AreEqual(expected: 20.0, actual: actual, delta: 0.001);

            inputVal = 180.0;
            actual = ptsAngle.normalizeToCustomRange(inputVal, 0.0, 360.0);
            Assert.AreEqual(expected: 180.0, actual: actual, delta: 0.001);

            inputVal = -20.0;
            actual = ptsAngle.normalizeToCustomRange(inputVal, 0.0, 360.0);
            Assert.AreEqual(expected: 340.0, actual: actual, delta: 0.001);

            inputVal = -360.0 - 20.0;
            actual = ptsAngle.normalizeToCustomRange(inputVal, 0.0, 360.0);
            Assert.AreEqual(expected: 340.0, actual: actual, delta: 0.001);

            // Test basis -180 -- +180
            inputVal = 180.0 + 20.0;
            actual = ptsAngle.normalizeToCustomRange(inputVal, -180.0, 180.0);
            Assert.AreEqual(expected: -160.0, actual: actual, delta: 0.001);

            inputVal = 90.0;
            actual = ptsAngle.normalizeToCustomRange(inputVal, -180.0, 180.0);
            Assert.AreEqual(expected: 90.0, actual: actual, delta: 0.001);

            inputVal = -20.0;
            actual = ptsAngle.normalizeToCustomRange(inputVal, -180.0, 180.0);
            Assert.AreEqual(expected: -20.0, actual: actual, delta: 0.001);

            inputVal = -180.0 - 20.0;
            actual = ptsAngle.normalizeToCustomRange(inputVal, -180.0, 180.0);
            Assert.AreEqual(expected: 160.0, actual: actual, delta: 0.001);

        }

        [TestMethod]
        public void ptsDegree_sin90_returns1p0()
        {
            ptsDegree deg = 90.0;
            Double expectedDbl = 1.0;
            Double actualDbl = ptsDegree.Sin(deg);
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: delta);
        }

        [TestMethod]
        public void ptsDegree_fromRadius5730_isOneDegree()
        {
            var radius = 5729.5779513;
            double expect = 1.0;
            double actual = ptsDegree.AsDblFromRadius(radius);
            Assert.AreEqual(expected: expect, actual: actual, delta: 0.00001);
        }

        [TestMethod]
        public void ptsDegree_Atan2Of10And0_returns90degrees()
        {
            ptsDegree deg = ptsDegree.Atan2(10.0, 0.0);
            Double expectedDbl = 90.0;
            Double actualDbl = deg.getAsDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: delta);
        }

        [TestMethod]
        public void ptsDegree_AsinOf1overSqrt2_shouldEqual45degrees()
        {
            ptsDegree deg = ptsDegree.Asin(1.0 / Math.Sqrt(2.0));
            Double expectedDbl = 45.0;
            Double actualDbl = deg.getAsDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: delta);
        }

        [TestMethod]
        public void AzimuthAddition_Az189PlusDeflNeg15_shouldEqual174()
        {
            Double expectedDbl = 174.0;
            Azimuth az = new Azimuth(); az.setFromDegreesDouble(189.0);
            Deflection defl = new Deflection(); defl.setFromDegreesDouble(-15.0);
            Azimuth newAz = az + defl;
            Double actualDbl = newAz.getAsDegreesDouble();
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: delta);
        }

        [TestMethod]
        public void ptsAngle_settingTo1_shouldResultIn_equals57_2957795Degrees()
        {
            ptsAngle angle = 1.0;
            Double expected = 57.2957795;
            Double actual = angle.getAsDegreesDouble();
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }

        [TestMethod]
        public void Azimuth_setToDMS183__29__29_5_shouldResultIn_Angle()
        {
            Azimuth anAzimuth = new Azimuth();
            anAzimuth.setFromDegreesMinutesSeconds(183, 29, 29.5);
            Double expected = 183.4915277778;
            Double actual = anAzimuth.getAsDegreesDouble();
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }

        [TestMethod]
        public void Deflection_setTo_Pos1Rad_shouldBe_Pos1Rad()
        {
            Deflection aDefl = new Deflection();
            aDefl = (Deflection)1.0;
            Double expected = 1.0;
            Double actual = aDefl.getAsRadians();
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }

        [TestMethod]
        public void Deflection_setTo_Neg1Rad_shouldBe_Neg1Rad()
        {
            Deflection aDefl = new Deflection();
            aDefl = (Deflection)(-1.0);
            Double expected = -1.0;
            Double actual = aDefl.getAsRadians();
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }

        [TestMethod]
        public void Deflection_setTo_Pos6Rad_shouldBe_Pos6Rad()
        {
            Deflection aDefl = new Deflection();
            aDefl = (Deflection)6.0;
            Double expected = 6.0;
            Double actual = aDefl.getAsRadians();
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }

        [TestMethod]
        public void Deflection_setTo_Pos2_shouldBe_Pos2Degrees()
        {
            Deflection defl = new Deflection();
            defl.setFromDegreesDouble(2.0);

            Double expected = 2.0;
            Double actual = defl.getAsDegreesDouble();
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }

        [TestMethod]
        public void Deflection_setTo_neg5__18__29_5()
        {
            Deflection aDeflection = new Deflection();
            aDeflection.setFromDegreesMinutesSeconds(-5, 18, 29.5);
            Double expected = -5.308194444444;
            Double actual = aDeflection.getAsDegreesDouble();
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }

        [TestMethod]
        public void Azimuth1_30_addDeflection_Pos2_15_shouldYieldNewAzimuth_3_45()
        {
            Azimuth anAzimuth = new Azimuth();
            anAzimuth.setFromDegreesMinutesSeconds(1, 30, 0);
            Deflection aDefl = new Deflection();
            aDefl.setFromDegreesMinutesSeconds(2, 15, 0);

            Double expected = 3.75;
            Azimuth newAz = anAzimuth + aDefl;
            Double actual = newAz.getAsDegreesDouble();
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }

        [TestMethod]
        public void Azimuth_createFromDouble_byAssignmentEquals()
        {
            double expected = 123.456;
            Azimuth anAz = expected;
            double actual = anAz.getAsDegreesDouble();
            Assert.AreEqual(expected: expected, actual: actual, delta: 0.0001);

            expected = 123.456;
            anAz = expected;
            actual = anAz.getAsDegreesDouble();
            Assert.AreEqual(expected: expected, actual: actual, delta: 0.0001);

            expected = 123.456;
            anAz = expected;
            actual = anAz.getAsDegreesDouble();
            Assert.AreEqual(expected: expected, actual: actual, delta: 0.0001);
        }

        [DataTestMethod]
        [DataRow(10, 2, 78.690067526)]
        [DataRow(10, -2, 101.309932474)]
        [DataRow(-10, 2, 281.309932474)]
        [DataRow(-10, -2, 258.690067526)]
        public void Azimuth_setFromXY(Double x, Double y, Double expectedDegrees)
        {
            Azimuth anAzimuth = new Azimuth();
            anAzimuth.setFromXY(x, y);
            Double actualDegrees = anAzimuth.getAsDegreesDouble();

            Assert.AreEqual(expected: expectedDegrees, actual: actualDegrees, delta: delta);
        }

        [DataTestMethod]
        [DataRow(20.0, 10.0, -10.0)]
        [DataRow(340.0, 350.0, 10.0)]
        [DataRow(20.0, 340.0, -40.0)]
        [DataRow(340.0, 20.0, 40.0)]
        public void AzimuthArithmatic_subtraction(Double Az1Dbl, Double Az2Dbl, Double expectedDeflection)
        {
            Azimuth Az1 = new Azimuth(); Az1.setFromDegreesDouble(Az1Dbl);
            Azimuth Az2 = new Azimuth(); Az2.setFromDegreesDouble(Az2Dbl);

            Double actualDeflection = Az2.minus(Az1).getAsDegreesDouble();

            Assert.AreEqual(expected: expectedDeflection, actual: actualDeflection, delta: 0.00000001);
        }

        [DataTestMethod]
        [DataRow(20.0, 10.0, -10.0)]
        [DataRow(340.0, 350.0, 10.0)]
        [DataRow(20.0, 340.0, -40.0)]
        [DataRow(340.0, 20.0, 40.0)]
        [DataRow(189.4326, 173.8145, -15.6181)]
        public void AzimuthArithmatic_addition(Double Az1Dbl, Double ExpectedAz2Dbl, Double DeflectionDbl)
        {
            Azimuth Az1 = new Azimuth(); Az1.setFromDegreesDouble(Az1Dbl);
            Deflection defl = DeflectionDbl.AsPtsDegree();
            Azimuth Az2 = Az1 + defl;

            Double actualAzimuth = Az2.getAsDegreesDouble();

            Assert.AreEqual(expected: ExpectedAz2Dbl, actual: actualAzimuth, delta: 0.00000001);
        }

        [TestMethod]
        public void AzimuthArithmatic_GetNormal()
        {
            Azimuth Az1 = new Azimuth(); Az1.setFromDegreesDouble(350.0);
            Azimuth Az1Right = Az1.RightNormal();
            Assert.AreEqual(
                expected: 80.0,
                actual: Az1Right.getAsDegreesDouble(),
                delta: 0.00001
                );
            Assert.AreEqual(
                expected: 260.0,
                actual: Az1.RightNormal(-1.0).getAsDegreesDouble(),
                delta: 0.00001
                );
        }

        [DataTestMethod]
        [DataRow(5.0, 10.0, 5.0)]
        [DataRow(15.0, 10.0, 5.0)]
        [DataRow(-5.0, 10.0, -5.0)]
        [DataRow(-15.0, 10.0, -5.0)]
        public void ComputeRemainder_ScaledByDenominator(Double numerator, Double Denominator, Double expectedDbl)
        {
            Double actualDbl = ptsAngle.ComputeRemainderScaledByDenominator(numerator, Denominator);
            Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.00000001);
        }

        [TestMethod]
        public void AzimuthArithmatic_Subtraction_Correct()
        {
            Azimuth Az1 = new Azimuth(); Az1.setFromDegreesDouble(299);
            Azimuth Az2 = new Azimuth(); Az2.setFromDegreesDouble(247);
            var defl = Az1.minus(Az2);  //start here. This value is wrong.
            //Assert.AreEqual(expected: 52.0, actual: defl, delta: 0.001);
        }

    }
}
