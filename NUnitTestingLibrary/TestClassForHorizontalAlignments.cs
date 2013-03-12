﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ptsCogo;
using ptsCogo.Horizontal;
using NUnitTestingLibrary.Mocks;

namespace NUnitTestingLibrary
{
   [TestFixture]
   public class TestClassForHorizontalAlignments
   {
      public List<Double> testList {get;set;}

      [SetUp]
      public void HAtestSetup()
      {

      }

      [Test]
      public void GenericAlignment_instantiateWithBeginStationOnly()
      {
         testList = new List<Double>();
         testList.Add(1000.0);
         GenericAlignment align = new GenericAlignment(testList);

         double actualDbl = align.BeginStation;
         double expectedDbl = 1000.0;

         Assert.AreEqual(expected: expectedDbl, actual: actualDbl, delta: 0.0000001);
      }

      [Test]
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

      [Test]
      public void HorizontalAlignment_instantiateSingleLine_fromNullFundamentalGeometry()
      {
         Assert.That(() => new rm21HorizontalAlignment(fundamentalGeometryList: (List<IRM21fundamentalGeometry>)null,
            Name: null, stationEquationing: null), Throws.Exception.TypeOf<NullReferenceException>());
      }

      private List<IRM21fundamentalGeometry> createFundmGeoms1()
      {
         List<IRM21fundamentalGeometry> fundmtlGeoms = new List<IRM21fundamentalGeometry>();

         rm21MockFundamentalGeometry mockFG = new rm21MockFundamentalGeometry();

         List<ptsPoint> ptLst = new List<ptsPoint>();
         ptLst.Add(new ptsPoint(10.0, 10.0, 0.0));
         ptLst.Add(new ptsPoint(80.7106781188, 80.7106781188, 0.0));

         mockFG.pointList = ptLst;
         mockFG.expectedType = expectedType.LineSegment;

         fundmtlGeoms.Add(mockFG);
         return fundmtlGeoms;
      }

      private List<IRM21fundamentalGeometry> createFundmGeoms_arc1()
      {
         List<IRM21fundamentalGeometry> fundmtlGeoms = new List<IRM21fundamentalGeometry>();

         rm21MockFundamentalGeometry mockFG = new rm21MockFundamentalGeometry();

         List<ptsPoint> ptLst = new List<ptsPoint>();
         ptLst.Add(new ptsPoint(443.176112, 569.321807, 0.0));
         ptLst.Add(new ptsPoint(1211.097635, -23.605404, 0.0));
         ptLst.Add(new ptsPoint(1186.397526, 946.268836, 0.0));

         mockFG.pointList = ptLst;
         mockFG.expectedType = expectedType.ArcSegmentInsideSolution;

         fundmtlGeoms.Add(mockFG);
         return fundmtlGeoms;
      }

      private List<IRM21fundamentalGeometry> createFundmGeoms_arc1_butExternalSolution()
      {
         List<IRM21fundamentalGeometry> fundmtlGeoms = new List<IRM21fundamentalGeometry>();

         rm21MockFundamentalGeometry mockFG2 = new rm21MockFundamentalGeometry();

         List<ptsPoint> ptLst = new List<ptsPoint>();
         ptLst.Add(new ptsPoint(443.176112, 569.321807, 0.0));
         ptLst.Add(new ptsPoint(1211.097635, -23.605404, 0.0));
         ptLst.Add(new ptsPoint(1186.397526, 946.268836, 0.0));

         mockFG2.pointList = ptLst;
         
         mockFG2.expectedType = expectedType.ArcSegmentOutsideSoluion;

         fundmtlGeoms.Add(mockFG2);
         return fundmtlGeoms;
      }

      [Test]
      public void HorizontalAlignment_instantiateSingleLine_fromFundamentalGeometry()
      {
         List<IRM21fundamentalGeometry> fundmtlGeoms = createFundmGeoms1();

         rm21HorizontalAlignment HA = new rm21HorizontalAlignment(
            fundamentalGeometryList: fundmtlGeoms,
            Name: null, stationEquationing: null);

         Assert.IsNotNull(HA);
      }

      [Test]
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

      [Test]
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

      [Test]
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

      [Test]
      public void HorizontalAlignment_ComputeDegreeOfCurveFromRadius()
      {
         Double Radius = 5729.58; Double LengthForDegreeOfCurve = 100.0;
         ptsAngle DegreeOfCurve = new ptsAngle(Radius, LengthForDegreeOfCurve);

         Double actualValue = DegreeOfCurve.getAsDegrees();
         Double expectedValue = 1.00;

         Assert.AreEqual(expected: expectedValue, actual: actualValue, delta: 0.000001);
      }

   }
}
