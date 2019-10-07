using ptsCogo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsDigitalTerrainModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Class only works with indices. Calling code must have the
    /// point list and the triangle list that are being indexed into.
    /// </remarks>
    public class ptsDTMtriangleLine
    {
        public ptsDTMpoint firstPoint { get; set; }
        public ptsDTMpoint secondPoint { get; set; }
        internal ptsDTMtriangle oneTriangle { get; set; }
        internal ptsDTMtriangle theOtherTriangle { get; set; }

        internal ptsDTMtriangleLine(ptsDTMpoint pt1, ptsDTMpoint pt2, ptsDTMtriangle tngle)
        {
            firstPoint = pt1;
            secondPoint = pt2;
            oneTriangle = tngle;
            theOtherTriangle = null;
        }

        public bool isSameAs(double x1, double y1, double x2)
        {
            if (Math.Truncate(this.firstPoint.x) != Math.Truncate(x1) &&
                Math.Truncate(this.secondPoint.x) != Math.Truncate(x1))
                return false;

            if (Math.Truncate(this.firstPoint.y) != Math.Truncate(y1) &&
                Math.Truncate(this.secondPoint.y) != Math.Truncate(y1))
                return false;

            if (Math.Truncate(this.firstPoint.x) != Math.Truncate(x2) &&
                Math.Truncate(this.secondPoint.x) != Math.Truncate(x2))
                return false;

            return true;
        }

        internal ptsDTMtriangle GetOtherTriangle(ptsDTMtriangle currentTriangle)
        {
            if (oneTriangle == currentTriangle)
                return theOtherTriangle;
            return oneTriangle;
        }

        public bool IsValid
        {
            get { return FirstAvailableTriangle.IsValid || theOtherTriangle.IsValid; }
        }

        private ptsBoundingBox2d boundingBox_ = null;
        public ptsBoundingBox2d BoundingBox
        {
            get
            {
                if(boundingBox_ is null)
                {
                    ptsPoint pt = new ptsPoint(this.firstPoint.x, this.firstPoint.y);
                    boundingBox_ = new ptsBoundingBox2d(pt);
                    pt = new ptsPoint(this.secondPoint.x, this.secondPoint.y);
                    boundingBox_.expandByPoint(pt);
                }
                return boundingBox_;
            }
        }

        internal ptsDTMtriangle FirstAvailableTriangle
        {
            get
            {
                if (this.oneTriangle.IsValid)
                    return oneTriangle;

                if (!(this.theOtherTriangle is null)
                        && this.theOtherTriangle.IsValid)
                    return this.theOtherTriangle;

                return null;
            }
        }

        public int TriangleCount
        {
            get
            {
                int retVal = 2;
                if (this.theOtherTriangle is null ||
                    !this.theOtherTriangle.IsValid)
                    retVal--;

                if (this.oneTriangle is null ||
                    !this.oneTriangle.IsValid)
                    retVal--;


                return retVal;
            }
        }

        internal static triangleLineComparer compr = new triangleLineComparer();
    }

    internal class triangleLineComparer : IEqualityComparer<ptsDTMtriangleLine>
    {
        public bool Equals(ptsDTMtriangleLine x, ptsDTMtriangleLine y)
        {
            return (x.firstPoint.myIndex == y.firstPoint.myIndex &&
                x.secondPoint.myIndex == y.secondPoint.myIndex);
        }

        public int GetHashCode(ptsDTMtriangleLine obj)
        {
            return obj.firstPoint.myIndex - obj.secondPoint.myIndex
                + obj.firstPoint.myIndex % 101;
        }
    }


}
