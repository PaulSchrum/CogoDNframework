using ptsCogo.Angle;
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
        public ptsVector spiralX { get; protected set; }
        public ptsVector spiralY { get; protected set; }
        public ptsDegree Dc1 { get; protected set; }
        public ptsDegree Dc2 { get; protected set; }
        public Double DcChangeRate { get; protected set; }

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

        public bool CurvatureIncreasesAhead { get { return DcChangeRate > 0.0; } }
        public bool CurvatureDecreasesAhead { get { return DcChangeRate < 0.0; } }
        public bool isConnecting { get { return Dc1 != 0.0 && Dc2 != 0.0; } }

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

            ptsVector chordVector = spiralX + spiralY;

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

            newSpi.Dc1 = ptsDegree.newFromDegrees(degreeIn);
            newSpi.Dc1 = ptsDegree.newFromDegrees(degreeOut);
            newSpi.Length = length;

            newSpi.DcChangeRate = (degreeOut - degreeIn) / length;
            if(newSpi.CurvatureIncreasesAhead)
            {
                if(newSpi.Dc1 == 0.0 ) // Type 1 Spiral
                {
                    newSpi.AnchorLength = length;
                    newSpi.AnchorRay = newSpi.BeginRay;
                }
                else   // Type 3 Spiral
                {
                    newSpi.AnchorLength = length;
                    newSpi.AnchorRay = newSpi.BeginRay;

                    // This flips the ray 180 degrees.
                    newSpi.AnchorRay.HorizontalDirection += Deflection.HALFCIRCLE;
                }
            }
            else
            {
                if(newSpi.Dc2 == 0.0) // Type 2 Spiral
                {

                }
                else   // Type 4 Spiral
                {

                }
            }

            return newSpi;
        }
    }

}
