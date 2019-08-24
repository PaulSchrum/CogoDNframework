using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ptsDigitalTerrainModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace TinTests
{
   public class TimeableTest
   {
      private Stopwatch sw = new Stopwatch();
      public Stopwatch stopwatch { get { return sw; } private set { } }
      public TestContext TestContext { get; set; }

      public void timerStart()
      {
         this.stopwatch.Start();
      }

      public void timerStopAndPrint(
         [CallerLineNumber] int lineNumber = 0,
         [CallerMemberName] string mbrName = null
         )
      {
         stopwatch.Stop();
         TestContext.WriteLine("Timespan: " +
            stopwatch.Elapsed.ToString() + " Line " +
            lineNumber.ToString() + " of " +
            mbrName);
         stopwatch.Reset(); stopwatch.Start();
      }
   }

   //[TestClass]
   public class DTMfromXYZdata : TimeableTest
   {
      private String pathRM21SourceCode;
      [TestInitialize]
      public void setup()
      {
         pathRM21SourceCode = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\";
      }

      private ptsDTM asheboroDTM = null;
      private bool AsheboroDataLoaded = false;
      private void loadAsheboroData()
      {
         if (AsheboroDataLoaded == false)
         {
            AsheboroDataLoaded = true;

            String fileToOpen = pathRM21SourceCode +
//               @"\ptsDigitalTerrainModel\Testing\XYZinput\" +
               "RandolphSmallSample.xyz.txt";

            asheboroDTM = new ptsDTM();
            asheboroDTM.loadFromXYZtextFile(fileToOpen);
         }
      }

      [TestMethod]
      public void SmallDataset_createsCorrectDTM()
      {
         loadAsheboroData();
      }
   }

   //[TestClass]
   public class GardenParkwayDTMtests : TimeableTest
   {
      private String pathRM21SourceCode;
      private Stopwatch stopwatch;
      private ptsDTM GardenParkwayDTM;
      private TimeSpan timeToLoadGardenParkwayTinFromXML;

      //[TestInitialize]
      public void setup()
      {
         var a = AppDomain.CurrentDomain.BaseDirectory;
         var dr = Directory.GetParent(a);
         
         while (!(dr.Name.Equals("RM21SourceCore")))
            dr = Directory.GetParent(dr.FullName);

         pathRM21SourceCode = dr.FullName + @"\NUnitTestingLibrary\TestingData\";
         stopwatch = new Stopwatch();
         stopwatch.Start();
         GardenParkwayDTM = new ptsDTM();
         var test = pathRM21SourceCode + "GPEtin.xml";
         try { GardenParkwayDTM.LoadTextFile(pathRM21SourceCode + "GPEtin.xml"); }
         catch (FileNotFoundException fnf) { GardenParkwayDTM = null; }
         stopwatch.Stop();
         timeToLoadGardenParkwayTinFromXML = stopwatch.Elapsed;
         System.Console.WriteLine("time to create Garden Parkway: " + timeToLoadGardenParkwayTinFromXML.ToString());
      }

      //[TestMethod]
      public void TIN_GardenParkwayTests_GetElevationSpeedTests()
      {
         this.timerStart();
         Double x = 529790.0; Double y = 1406750.0;
         var EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x, y, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 10.0, y += 5.0, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 10.0, y += 5.0, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 10.0, y += 5.0, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 1.0, y += 25.0, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 1.0, y += 25.0, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 1.0, y += 25.0, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 1.0, y += 25.0, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 1.0, y += 25.0, 0.0));
         EL = GardenParkwayDTM.getElevation(new ptsDTMpoint(x += 1.0, y += 25.0, 0.0));

         this.timerStopAndPrint();
         Assert.IsTrue(true);
      }

   }
}
