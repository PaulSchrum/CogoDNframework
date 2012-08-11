﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo;
using ptsCogo.Angle;

namespace ptsDigitalTerrainModel
{
   [Serializable]
   internal class ptsDTMtriangle : IComparable
   {
      // temporary scratch pad members -- do not serialize
      [NonSerialized]
      private String[] indexStrings;
      [NonSerialized]
      private UInt64[] indices;

      // substantitve fields - Do Serialize
      public ptsDTMpoint point1 { get; set; }
      public ptsDTMpoint point2 { get; set; }
      public ptsDTMpoint point3 { get; set; }

      private ptsVector normalVec_;
      public ptsVector normalVec { get { return normalVec_; } }

      // non-substantive fields
      [NonSerialized]
      private ptsCogo.ptsBoundingBox2d myBoundingBox_;

      public ptsDTMtriangle(Dictionary<UInt64, ptsDTMpoint> pointList, string pointRefs)
      {
         indices = new UInt64[3];
         indexStrings = pointRefs.Split(' ');
         UInt64.TryParse(indexStrings[0], out indices[0]);
         UInt64.TryParse(indexStrings[1], out indices[1]);
         UInt64.TryParse(indexStrings[2], out indices[2]);

         point1 = pointList[indices[0]];
         point2 = pointList[indices[1]];
         point3 = pointList[indices[2]];

         computeBoundingBox();
         normalVec_ = null;
      }

      public void computeBoundingBox()
      {
         myBoundingBox_ = new ptsBoundingBox2d(point1);
         myBoundingBox_.expandByPoint(point2);
         myBoundingBox_.expandByPoint(point3);
      }

      public bool isPointInBoundingBox(ptsPoint aPoint)
      {
         return myBoundingBox_.isPointInsideBB2d(aPoint);
      }

      #region IComparable Members

      /// <summary>
      /// Makes ptsDTMtriangles automatically sort itself based on x-axis order.
      /// </summary>
      /// <param name="obj"></param>
      /// <returns>int</returns>
      int IComparable.CompareTo(object obj)
      {
         ptsDTMtriangle other = (ptsDTMtriangle)obj;
         return this.myBoundingBox_.lowerLeftPt.compareByXthenY(other.myBoundingBox_.lowerLeftPt);
      }

      #endregion
      
      // adapted from
      // http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
      internal bool contains(ptsPoint aPoint)
      {
         bool b1, b2, b3;

         b1 = sign(aPoint, point1, point2) < 0.0f;
         b2 = sign(aPoint, point2, point3) < 0.0f;
         b3 = sign(aPoint, point3, point1) < 0.0f;

         return ((b1 == b2) && (b2 == b3));
      }

      double sign(ptsPoint p1, ptsPoint p2, ptsPoint p3)
      {
         return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
      }
      // End: adapted from

      public double givenXYgetZ(ptsPoint aPoint)
      {
         setupNormalVec();

         // Use equation         ax + bx
         //                 z = ----------     taken from Wolfram Alpha
         //                        -c
         //
         //  where a is normalVec_.i, b is .j, and c is .k
         //    and X is aPoint.x - point1.x
         //    and Y is aPoint.y - point1.y
         //
         //  Ultimately add z to point1.z to get the elevation

         double X = aPoint.x - point1.x;
         double Y = aPoint.y - point1.y;

         double Z = ((normalVec.x * X) + (normalVec.y * Y)) / 
                     (-1.0 * normalVec.z);

         return Z + point1.z;
      }

      public double? givenXYgetSlopePercent(ptsPoint aPoint)
      {
         setupNormalVec();

         if (0.0 == normalVec_.z) return null;

         return Math.Abs (100.0 *
            Math.Sqrt(normalVec_.x * normalVec_.x + normalVec_.y * normalVec_.y) /
                        normalVec_.z);
      }

      public Azimuth givenXYgetSlopeAzimuth(ptsPoint aPoint)
      {
         setupNormalVec();

         Azimuth slopeAz = new Azimuth();
         slopeAz.setFromXY(normalVec_.y, normalVec.x);

         return slopeAz; 
         
      }

      private void setupNormalVec()
      {
         if (normalVec_ == null)
         {
            normalVec_ = (point2 - point1).crossProduct(point3 - point1);
         }
      }

   }
}
