using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo
{
   [Serializable]
   public class ptsBoundingBox2d
   {
      public ptsPoint lowerLeftPt { get; set; }
      public ptsPoint upperRightPt { get; set; }

      private ptsBoundingBox2d() { }

      public ptsBoundingBox2d(
          Double LLx, Double LLy, 
          Double URx, Double URy,
          Double LLz=0.0, Double URz=0.0)
      {
         lowerLeftPt = new ptsPoint(LLx, LLy, LLz);
         upperRightPt = new ptsPoint(URx, URy, URz);
      }

      public ptsBoundingBox2d(ptsPoint aPoint)
      {
         lowerLeftPt = new ptsPoint(aPoint);
         upperRightPt = new ptsPoint(aPoint);
      }

      public void expandByPoint(ptsPoint aPoint)
      {
         expandByPoint(aPoint.x, aPoint.y, aPoint.z);
      }

      public void expandByPoint(Double x, Double y, Double z)
      {
         if (lowerLeftPt.isEmpty == true)
         {
            lowerLeftPt = new ptsPoint(x, y, z);
            upperRightPt = new ptsPoint(x, y, z);
         }
         else
         {
            if (x < lowerLeftPt.x)
               lowerLeftPt.x = x;
            if (y < lowerLeftPt.y)
               lowerLeftPt.y = y;
            if (z < lowerLeftPt.z)
               lowerLeftPt.z = z;

            if (x > upperRightPt.x)
               upperRightPt.x = x;
            if (y > upperRightPt.y)
               upperRightPt.y = y;
            if (z > upperRightPt.z)
               upperRightPt.z = z;
         }
      }

      public bool isPointInsideBB2d(Double x, Double y)
      {
         if (x < lowerLeftPt.x)
            return false;
         if (y < lowerLeftPt.y)
            return false;

         if (x > upperRightPt.x)
            return false;
         if (y > upperRightPt.y)
            return false;

         return true;
      }

      public bool isPointInsideBB2d(ptsPoint testPoint)
      {
         if (testPoint.x < lowerLeftPt.x)
            return false;
         if (testPoint.y < lowerLeftPt.y)
            return false;

         if (testPoint.x > upperRightPt.x)
            return false;
         if (testPoint.y > upperRightPt.y)
            return false;

         return true;
      }

      public bool isPointInsideBB3d(ptsPoint testPoint)
      {
         if (isPointInsideBB2d(testPoint) == false)
            return false;

         if (testPoint.z < lowerLeftPt.z)
            return false;

         if (testPoint.z > upperRightPt.z)
            return false;

         return true;
      }
   }
}
