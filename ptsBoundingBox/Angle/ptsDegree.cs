﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo.Angle
{
   public struct ptsDegree
   {
      private readonly Double degrees_;
      public ptsDegree(Double newVal)
      {
         this.degrees_ = newVal;
      }

      public Double getAsRadians()
      {
         return degrees_ * Math.PI / 180.0;
      }

      public Double getAsDouble()
      {
         return degrees_;
      }

      public static ptsDegree newFromRadians(Double rad)
      {
         return new ptsDegree(180.0 * rad / Math.PI);
      }

      public static ptsDegree newFromDegreesMinutesSeconds(int degrees, int minutes, double seconds)
      {
         return new ptsDegree(
               Math.Abs((double)degrees) +
               (double)minutes / 60.0 + seconds / 3600.0
                        );
      }

      public static ptsDegree Asin(Double val)
      {
         return newFromRadians(Math.Asin(val));
      }

      public static ptsDegree Acos(Double val)
      {
         return newFromRadians(Math.Acos(val));
      }

      public static ptsDegree Atan(Double val)
      {
         return newFromRadians(Math.Atan(val));
      }

      public static ptsDegree Atan2(Double y, Double x)
      {
         return newFromRadians(Math.Atan2(y, x));
      }

      public static Double Sin(ptsDegree deg)
      {
         return Math.Sin(deg.getAsRadians());
      }

      public static Double Cos(ptsDegree deg)
      {
         return Math.Cos(deg.getAsRadians());
      }

      public static Double Tan(ptsDegree deg)
      {
         return Math.Tan(deg.getAsRadians());
      }

      public static implicit operator ptsDegree(double doubleVal)
      {
         return new ptsDegree(doubleVal);
      }

      public static bool operator >=(ptsDegree left, ptsDegree right)
      {
         return left.degrees_ >= right.degrees_;
      }

      public static bool operator <=(ptsDegree left, ptsDegree right)
      {
         return left.degrees_ <= right.degrees_;
      }

      public static ptsDegree operator +(ptsDegree left, Double right)
      {
         return left.degrees_ + right;
      }

      public static ptsDegree operator -(ptsDegree left, Double right)
      {
         return left.degrees_ - right;
      }

      public override string ToString()
      {
         return degrees_.ToString() + "°";
      }
   }

   public static class extendDoubleForPtsDegree
   {
      public static ptsDegree AsPtsDegree(this Double val)
      {
         return new ptsDegree(val);
      }
   }

}
