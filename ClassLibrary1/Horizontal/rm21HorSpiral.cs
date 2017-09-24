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
        public ptsVector spiralDX { get; protected set; }
        public ptsVector spiralDY { get; protected set; }
        public ptsDegree BeginDc { get; protected set; }
        public ptsDegree EndDc { get; protected set; }


        public ptsDegree DcChangeRate { get; protected set; }

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
            get { return (Math.Abs(EndDc.getAsRadians() - BeginDc.getAsRadians()) > 0.0); }
        }

        /// <summary>
        /// True for Type 2 and Type 4.
        /// </summary>
        public bool CurvatureDecreasesAhead
        {
            get { return (Math.Abs(EndDc.getAsRadians() - BeginDc.getAsRadians()) < 0.0); }
        }


        public bool isConnecting { get { return BeginDc != 0.0 && EndDc != 0.0; } }

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
            return LengthFraction(L) * LengthFraction(L);
        }

        /// <summary>
        /// The spiral X equation from Hickerson, Appendix C, p 374
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
        /// The spiral X equation from Hickerson, Appendix C, p 374
        /// </summary>
        /// <param name="distanceAlong"></param>
        /// <returns></returns>
        private Double computeYlength(Double distanceAlong)
        {
            var theta = ThetaFraction(distanceAlong);
            Double y =
                +theta / 3.0
                -Math.Pow(theta,3.0)/42.0
                + Math.Pow(theta, 5.0) / 1320.0
                - Math.Pow(theta, 7.0) / 75600.0
                + Math.Pow(theta, 9.0) / 6894720.0
                ;

            return distanceAlong * y;
        }

        public override ptsPoint getXYZcoordinates(StationOffsetElevation anSOE)
        {
            if(anSOE.station < this.BeginStation || anSOE.station > this.EndStation)
                return null;

            Double localDistAlong = anSOE.station - this.BeginStation;

            Double xDist = computeXlength(localDistAlong);
            Double yDist = computeYlength(localDistAlong);

            ptsVector chordVector = spiralDX + spiralDY;

            return null;
        }

        public static rm21HorSpiralc Create(
            ptsRay inRay, 
            double length, 
            double degreeIn, 
            double degreeOut)
        {
            rm21HorSpiralc newSpi = new rm21HorSpiralc();

            newSpi.BeginPoint = inRay.StartPoint;
            newSpi.BeginAzimuth = inRay.HorizontalDirection;

            newSpi.BeginDc = ptsDegree.newFromDegrees(degreeIn);
            newSpi.EndDc = ptsDegree.newFromDegrees(degreeOut);
            newSpi.Length = length;

            newSpi.DcChangeRate = newSpi.EndDc - newSpi.BeginDc
                                    / length;
            var v = newSpi.DcChangeRate.getAsRadians();

            if(newSpi.isConnecting) // Type 3 or 4
            {
                throw new NotImplementedException();
            }
            else  // Type 1 or 2
            {
                var thetaS = newSpi.DcChangeRate.getAsRadians() * length 
                    / (2 * degreeOfCurveLength);
                //thetaS = newSpi.DcChangeRate.getAsDouble() * length / 200.0;
                newSpi.Deflection = new Deflection(thetaS);
            }

            if(newSpi.CurvatureIncreasesAhead)
            {
                if(newSpi.BeginDc == 0.0 ) // Type 1 Spiral
                {
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
                if(newSpi.EndDc == 0.0) // Type 2 Spiral
                {
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
    }

}
