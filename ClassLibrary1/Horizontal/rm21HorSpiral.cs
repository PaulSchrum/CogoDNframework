﻿using ptsCogo.Angle;
using ptsCogo.coordinates;
using ptsCogo.coordinates.CurvilinearCoordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo.Horizontal
{
    public class rm21HorSpiralc : HorizontalAlignmentBase
    {
        public int SpiralType { get; protected set; }
        public ptsVector spiralDX { get; protected set; }
        public ptsVector spiralDY { get; protected set; }
        public double AnchorPhantomStation { get; protected set; }


        public ptsAngle DcChangeRate { get; protected set; }

        /// <summary>
        /// Used internally to find by getStationOffsetElevation to
        /// iteratate to a solution.
        /// </summary>
        private SpiralIterator myIterator { get; set; }

        /// <summary>
        /// The point where Dc = 0. For connecting spirals,
        /// this point is off the alignment, but we still have to use it.
        /// </summary>
        protected ptsPoint AnchorPoint
        { get { return this.AnchorRay.StartPoint; } }

        /// <summary>
        /// The point and ahead-bearing where Dc = 0. For connecting spirals,
        /// this point is off the alignment, but we still have to use it.
        /// </summary>
        protected ptsRay AnchorRay { get; set; }

        /// <summary>
        /// Total length along the element from the CS/SC Point to the
        /// Anchor Point. 
        /// </summary>
        protected Double AnchorLength { get; set; }

        /// <summary>
        /// True for Type 1 and Type 3.
        /// </summary>
        public bool CurvatureIncreasesAhead
        {
            get { return (Math.Abs(EndDegreeOfCurve.getAsRadians() - BeginDegreeOfCurve.getAsRadians()) > 0.0); }
        }

        /// <summary>
        /// True for Type 2 and Type 4.
        /// </summary>
        public bool CurvatureDecreasesAhead
        {
            get { return (Math.Abs(EndDegreeOfCurve.getAsRadians() - BeginDegreeOfCurve.getAsRadians()) < 0.0); }
        }


        public bool isConnecting { get { return BeginDegreeOfCurve != 0.0 && EndDegreeOfCurve != 0.0; } }

        public Double LengthFraction(Double L)
        {
            return L / this.Length;
        }

        /// <summary>
        /// From Hickerson, p 169.
        /// </summary>
        /// <param name="L"></param>
        /// <returns></returns>
        public Double ThetaFraction(Double L)
        {
            return this.Deflection.getAsRadians() * LengthFraction(L) * LengthFraction(L);
        }

        /// <summary>
        /// The spiral X equation from Hickerson, Appendix C, p 374
        /// This is the equation based on Radians, not Degrees.
        /// </summary>
        /// <param name="distanceAlong"></param>
        /// <returns></returns>
        private Double computeXlength(Double distanceAlong)
        {
            var theta = ThetaFraction(distanceAlong);
            Double x =
                +1.0
                - Math.Pow(theta, 2.0) / 10.0
                + Math.Pow(theta, 4.0) / 216.0
                - Math.Pow(theta, 6.0) / 9360.0
                + Math.Pow(theta, 8.0) / 685440.0
                ;

            return distanceAlong * x;
        }

        /// <summary>
        /// The spiral Y equation from Hickerson, Appendix C, p 374
        /// This is the equation based on Radians, not Degrees.
        /// </summary>
        /// <param name="distanceAlong"></param>
        /// <returns></returns>
        private Double computeYlength(Double distanceAlong)
        {
            var theta = ThetaFraction(distanceAlong);
            Double y =
                +theta / 3.0
                - Math.Pow(theta, 3.0) / 42.0
                + Math.Pow(theta, 5.0) / 1320.0
                - Math.Pow(theta, 7.0) / 75600.0
                + Math.Pow(theta, 9.0) / 6894720.0
                ;

            return distanceAlong * y;
        }

        public override Azimuth getAzimuth(double station)
        {
            var anchorLengthAlong = station - this.AnchorPhantomStation;
            return this.AnchorRay.HorizontalDirection + ThetaFraction(anchorLengthAlong);
        }

        /// <summary>
        /// Based on clockwise being positive.
        /// </summary>
        /// <param name="anSOE"></param>
        /// <returns></returns>
        public override Azimuth getPerpandicularAzimuth(double station)
        {
            return this.getAzimuth(station) + Deflection.HALFCIRCLE / 2.0;
        }

        /// <summary>
        /// Based on clockwise being positive.
        /// </summary>
        /// <param name="anSOE"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override ptsVector getPerpandicularVector(double station, double length)
        {
            var az = getPerpandicularAzimuth(station);
            return new ptsVector(az, length);
        }

        public override List<StationOffsetElevation> getStationOffsetElevation(ptsPoint interestPoint)
        {
            throw new NotImplementedException();
            var returnList = new List<StationOffsetElevation>();

            var spiratIterator = SpiralIterator.createIterator(this, interestPoint);

            return returnList;
        }

        public override ptsPoint getXYZcoordinates(StationOffsetElevation anSOE)
        {
            if(anSOE.station < this.BeginStation || anSOE.station > this.EndStation)
                return null;

            Double localDistAlong = anSOE.station - this.BeginStation;
            Double anchorDistAlong = anSOE.station - this.AnchorPhantomStation;

            Double xDist = computeXlength(anchorDistAlong);
            var dx = new ptsVector(this.AnchorRay, xDist);

            Double yDist = computeYlength(anchorDistAlong);
            var dy =
                (new ptsVector(this.AnchorRay, yDist * this.Deflection.deflectionDirection))
                .left90degrees();

            ptsVector chordVector = dx + dy;
            ptsPoint targetPoint = this.AnchorPoint + chordVector;

            var offsetIsNotZero = utilFunctions.tolerantCompare(
                    anSOE.offset.OFST, 0.0,
                    0.00001) != 0;

            if(offsetIsNotZero)
            {
                var perpandicluarVector =
                    this.getPerpandicularVector(anSOE.station, anSOE.offset.OFST);
                targetPoint = targetPoint + perpandicluarVector;
            }

            return targetPoint;
        }

        protected static void setSpiralType(rm21HorSpiralc spi, double degreeIn, double degreeOut)
        {
            if(degreeIn == 0.0)
                spi.SpiralType = 1;
            else if(degreeOut == 0.0)
                spi.SpiralType = 2;
            else if(Math.Abs(degreeIn) < Math.Abs(degreeOut))
                spi.SpiralType = 3;
            else
                spi.SpiralType = 4;
        }

        public static rm21HorSpiralc Create(
            ptsRay inRay,
            double length,
            double degreeIn,
            double degreeOut,
            double beginStation = 0.0)
        {
            rm21HorSpiralc newSpi = new rm21HorSpiralc();
            setSpiralType(newSpi, degreeIn, degreeOut);

            newSpi.BeginPoint = inRay.StartPoint;
            newSpi.BeginAzimuth = inRay.HorizontalDirection;

            newSpi.BeginDegreeOfCurve = ptsAngle.radiansFromDegree(degreeIn);
            newSpi.EndDegreeOfCurve = ptsAngle.radiansFromDegree(degreeOut);
            newSpi.Length = length;

            newSpi.DcChangeRate = (newSpi.EndDegreeOfCurve - newSpi.BeginDegreeOfCurve)
                                    / length;
            var v = newSpi.DcChangeRate.getAsDegreesDouble();

            var highestDegree = Math.Max(Math.Abs(degreeIn), Math.Abs(degreeOut));
            var minRadius = ptsDegree.asRadiusFromDegDouble(highestDegree);

            if(newSpi.SpiralType == 1)
            {
                var deflDirection = Math.Sign(newSpi.DcChangeRate.getAsRadians());
                var thetaS = deflDirection * length
                    / (2 * minRadius);
                newSpi.Deflection = new Deflection(thetaS);
                newSpi.BeginStation = beginStation;
                newSpi.AnchorPhantomStation = beginStation;
                newSpi.EndStation = beginStation + length;
                newSpi.EndAzimuth = newSpi.BeginAzimuth + newSpi.Deflection;
                double spiralX = newSpi.computeXlength(length);
                newSpi.spiralDX = new ptsVector(newSpi.BeginAzimuth, spiralX);
                double spiralY = newSpi.computeYlength(length);
                spiralY *= newSpi.Deflection.deflectionDirection;
                newSpi.spiralDY = new ptsVector(newSpi.BeginAzimuth, spiralY);
                newSpi.EndPoint = newSpi.BeginPoint + newSpi.spiralDX + newSpi.spiralDY;
            }

            if(newSpi.CurvatureIncreasesAhead)
            {
                if(newSpi.BeginDegreeOfCurve.getAsRadians() == 0.0) // Type 1 Spiral
                {
                    newSpi.AnchorPhantomStation = newSpi.BeginStation;
                    newSpi.AnchorLength = length;
                    newSpi.AnchorRay = newSpi.BeginRay;

                    Azimuth startAz = newSpi.BeginRay.HorizontalDirection;
                    Double x = newSpi.computeXlength(length);
                    newSpi.spiralDX = new ptsVector(startAz, x);

                    Azimuth perp2StartAz =
                        startAz + Deflection.Perpandicular(newSpi.Deflection.deflectionDirection);
                    Double y = newSpi.computeYlength(length);
                    newSpi.spiralDY = new ptsVector(perp2StartAz, y);
                }
                else   // Type 3 Spiral
                {
                    throw new NotImplementedException("Type 3 spirals not yet implemented.");
                }
            }
            else
            {
                if(newSpi.EndDegreeOfCurve.getAsRadians() == 0.0) // Type 2 Spiral
                {
                    newSpi.AnchorPhantomStation = newSpi.EndStation;
                    newSpi.AnchorLength = length;
                    //newSpi.AnchorRay =  
                }
                else   // Type 4 Spiral
                {
                    throw new NotImplementedException("Type 4 spirals not yet implemented.");
                }
            }

            return newSpi;
        }

        /// <summary>
        /// 
        /// </summary>
        private class SpiralIterator
        {
            /// <summary>
            /// Factory method to create a spiral iterator and populate it with its
            /// first five points
            /// </summary>
            /// <param name="mySpiral"></param>
            /// <returns></returns>
            public static SpiralIterator createIterator(rm21HorSpiralc mySpiral, ptsPoint aTargetPoint)
            {
                if(mySpiral.myIterator != null)
                    return mySpiral.myIterator;

                var newSpiralIter = new SpiralIterator(mySpiral, aTargetPoint);
                double aLen = mySpiral.Length;
                double begSta = mySpiral.BeginStation;
                double segmentStations = 4.0;
                for(double i = 0; i <= segmentStations; i++)
                {
                    double intermediateStation = begSta + (i * aLen / segmentStations);
                    ptsPoint pos = mySpiral.getXYZcoordinates(intermediateStation);
                    newSpiralIter.iterationList.Add(
                        new SpiralIterationPoint(intermediateStation, pos));
                }

                foreach(var iterPoint in newSpiralIter.iterationList)
                {
                    iterPoint.computeParameters(newSpiralIter, aTargetPoint);
                }

                return mySpiral.myIterator;
            }

            public rm21HorSpiralc mySpiral { get; private set; }
            public ptsPoint targetPoint { get; private set; }
            public List<SpiralIterationPoint> iterationList { get; private set; }
            private SpiralIterator(rm21HorSpiralc mySpi, ptsPoint aTargetPoint)
            {
                this.mySpiral = mySpi;
                this.targetPoint = aTargetPoint;
                this.iterationList = new List<SpiralIterationPoint>();
                mySpi.myIterator = this;
            }

            public struct SpiralIterationPoint
            {
                double station { get; set; }
                ptsPoint pointOnStation { get; set; }
                Azimuth tangentDirection { get; set; }
                ptsVector vectorToTarget { get; set; }
                Deflection deflectionToTarget { get; set; }

                public SpiralIterationPoint(double sta, ptsPoint pointOnSta)
                {
                    station = sta;
                    pointOnStation = pointOnSta;
                    tangentDirection = null;
                    vectorToTarget = null;
                    deflectionToTarget = null;
                }

                internal void computeParameters(SpiralIterator newSpiralIter,
                    ptsPoint targetPoint)
                {
                    var spi = newSpiralIter.mySpiral;
                    double sta = this.station;
                    this.tangentDirection = spi.getAzimuth(sta);
                    this.vectorToTarget = targetPoint - pointOnStation;
                    this.deflectionToTarget = tangentDirection - vectorToTarget.Azimuth;
                }
            }
        }
    }

}