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
        public ptsVector spiralX { get; }
        public ptsVector spiralY { get; }
        public Double DcChangeRate { get; set; }

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
    }

}
