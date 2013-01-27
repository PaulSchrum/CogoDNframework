﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo.coordinates.CurvilinearCoordinates;
using ptsCogo;
using ptsCogo.Angle;
using System.Windows.Media;


namespace rm21Core.Ribbons
{
   public class PGLoffset : ribbonBase
   {
      public PGLoffset(CogoStation beginStation, CogoStation endStation, double initialWidth, Slope initialSlope)
         : base(beginStation, endStation, initialWidth, initialSlope) 
      {
         LiederLineHeight = 7.0;
      }

      public override string getHashName() { return "PGL Offset"; }

      public override void DrawCrossSection(IRM21cad2dDrawingContext cadContext,
         ref StationOffsetElevation aSOE, int whichSide)
      {
         base.setupCrossSectionDrawing(cadContext);
         cadContext.setElementColor(Color.FromArgb(128, 128, 128, 128));
         cadContext.setElementWeight(0.75);
         cadContext.addToDashArray(5);

         SuppressSlopeText = true;
         base.DrawCrossSection(cadContext, ref aSOE, whichSide);
      }

      public override void DrawPlanViewSchematic(IRM21cad2dDrawingContext cadContext, int whichSide)
      {
         base.DrawPlanViewSchematic(cadContext, whichSide);
      }

      public override Profile getOffsetProfile()
      {
         if (this.getMyScaleFactor() < 0)
            return Profile.arithmaticAddProfile(null, myOffsets, -1.0);

         return myOffsets;
      }
   }
}
