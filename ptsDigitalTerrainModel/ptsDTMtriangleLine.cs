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
    class ptsDTMtriangleLine
    {
        public ptsDTMpoint firstPoint { get; set; }
        public ptsDTMpoint secondPoint { get; set; }
        public ptsDTMtriangle oneTriangle { get; set; }
        public ptsDTMtriangle theOtherTriangle { get; set; }

        public ptsDTMtriangleLine(ptsDTMpoint pt1, ptsDTMpoint pt2, ptsDTMtriangle tngle)
        {
            firstPoint = pt1;
            secondPoint = pt2;
            oneTriangle = tngle;
            theOtherTriangle = null;
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
