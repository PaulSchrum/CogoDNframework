﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo;

namespace ptsCogo.Horizontal
{
   public interface IRM21fundamentalGeometry
   {
      List<ptsPoint> getPointList();
      Double getBeginningDegreeOfCurve();
      Double getEndingDegreeOfCurve();
      expectedType getExpectedType();

   }

   public enum expectedType 
   { 
      LineSegment=0, 
      ArcSegmentInsideSolution=1, 
      ArcSegmentOutsideSoluion=2,
      ArcHalfCircleDeflectingLeft=3,
      ArcHalfCircleDeflectingRight=4,
      EulerSpiral=5 
   };
}
