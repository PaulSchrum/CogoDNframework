﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsDigitalTerrainModel;
using ptsCogo;
using ptsCogo.Angle;

namespace ptsDtmTest1
{
   class Program
   {
      static void Main(string[] args)
      {
         ptsDTM aTinFile = null;
         try
         {
            //aTinFile = new ptsDTM(@"C:\Users\Paul\Documents\Visual Studio 2010\Projects\XML Files\Garden Parkway\BigTest.xml");
            //aTinFile = new ptsDTM(@"C:\Users\Paul\Documents\Visual Studio 2010\Projects\XML Files\Garden Parkway\SmallExample.xml");
            aTinFile = new ptsDTM(@"C:\Users\Paul\Documents\Visual Studio 2010\Projects\XML Files\Garden Parkway\GPEtin.xml");
         }
         catch(Exception e)
         {
            System.Console.WriteLine("Exception: ");
            System.Console.WriteLine(e.Message);
         }
         
         //if (aTinFile != null)
            //aTinFile.saveAsBinary(@"C:\Users\Paul\Documents\Visual Studio 2010\Projects\XML Files\Garden Parkway\GPEtin.ptsTin");

         //aTinFile = ptsDTM.loadFromBinary(@"C:\Users\Paul\Documents\Visual Studio 2010\Projects\XML Files\Garden Parkway\testSave.ptsTin");

         //System.Console.WriteLine("Press a key to release memory");
         //System.Console.ReadKey();
         //aTinFile.testGetTriangle
         double? aSlope; Azimuth anAzimuth;
         double? anElevation = aTinFile.getElevation(new ptsPoint(529399.6100, 1408669.1900, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(529399.6100, 1408669.1900, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(529399.6100, 1408669.1900, 0.0));
         printResult(2, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(529868.0890, 1407914.1624, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(529868.0890, 1407914.1624, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(529868.0890, 1407914.1624, 0.0));
         printResult(3, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(529533.0000, 1415029.0000, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(529533.0000, 1415029.0000, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(529533.0000, 1415029.0000, 0.0));
         printResult(4, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(529890.1833, 1409331.1264, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(529890.1833, 1409331.1264, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(529890.1833, 1409331.1264, 0.0));
         printResult(5, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(530069.6770, 1410934.1035, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(530069.6770, 1410934.1035, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(530069.6770, 1410934.1035, 0.0));
         printResult(6, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(531816.0608, 1402087.7063, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(531816.0608, 1402087.7063, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(531816.0608, 1402087.7063, 0.0));
         printResult(7, anElevation, aSlope, anAzimuth);
         anAzimuth = aTinFile.getElevation(new ptsPoint(529211.7956, 1413338.5912, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(529211.7956, 1413338.5912, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(529211.7956, 1413338.5912, 0.0));
         printResult(8, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(529231.2117, 1413280.0812, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(529231.2117, 1413280.0812, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(529231.2117, 1413280.0812, 0.0));
         printResult(9, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(529987.6366, 1406533.3478, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(529987.6366, 1406533.3478, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(529987.6366, 1406533.3478, 0.0));
         printResult(10, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(530437.7030, 1402565.4940, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(530437.7030, 1402565.4940, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(530437.7030, 1402565.4940, 0.0));
         printResult(11, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(530935.4300, 1401737.1885, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(530935.4300, 1401737.1885, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(530935.4300, 1401737.1885, 0.0));
         printResult(12, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(518797.8895, 1383909.6974, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(518797.8895, 1383909.6974, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(518797.8895, 1383909.6974, 0.0));
         printResult(13, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(532158.0000, 1413853.0000, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(532158.0000, 1413853.0000, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(532158.0000, 1413853.0000, 0.0));
         printResult(14, anElevation, aSlope, anAzimuth);
         anElevation = aTinFile.getElevation(new ptsPoint(527915.0000, 1406442.0000, 0.0));
         aSlope = aTinFile.getSlope(new ptsPoint(527915.0000, 1406442.0000, 0.0));
         anAzimuth = aTinFile.getSlopeAzimuth(new ptsPoint(527915.0000, 1406442.0000, 0.0));
         printResult(15, anElevation, aSlope, anAzimuth);

         aTinFile = null;
         GC.Collect();

         System.Console.WriteLine("Memory Released");
         System.Console.ReadKey();
      }

      private static void printResult(int count, double? EL, double? slope, Azimuth AZ)
      {
         if (null == EL)
         {
            System.Console.WriteLine("Row {0} has elevation = null", count);
         }
         else
         {
            System.Console.Write("Row {0} has elevation = ", count);
            System.Console.Write((double)EL);
            System.Console.Write(", Slope = ");
            System.Console.Write((double)slope);
            System.Console.Write(", Az = ");
            System.Console.WriteLine(AZ);
         }
      }
   }

}
