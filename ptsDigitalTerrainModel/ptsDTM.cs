﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Concurrent;
using ptsCogo;
using ptsCogo.Angle;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace ptsDigitalTerrainModel
{
    [Serializable]
    public class ptsDTM
    {
        // Substantive members - Do serialize
        public List<ptsDTMpoint> allPoints { get; private set; }
        private List<ptsDTMtriangle> allTriangles;
        private ptsBoundingBox2d myBoundingBox;
        private LasFile lasFile { get; set; } = null;

        public static ptsDTM CreateFromLAS(string lidarFileName)
        {
            LasFile lasFile = new LasFile(lidarFileName);
            ptsDTM returnObject = new ptsDTM();
            int indexCount = 0;
            var gridIndexer = new Dictionary<Tuple<int, int>, int>();
            foreach(var point in lasFile.AllPoints)
            {
                if(returnObject.allPoints == null)
                {
                    returnObject.createAllpointsCollection();
                    returnObject.myBoundingBox = 
                        new ptsBoundingBox2d(
                            point.x, point.y, 
                            point.x, point.y,
                            point.z, point.z);
                }
                else
                    returnObject
                        .myBoundingBox.expandByPoint(point.x, point.y, point.z);
                returnObject.allPoints.Add(point);
                // Note this approach will occasionally skip over points that 
                // are double-stamps. I am fine with that for now.
                gridIndexer[point.GridCoordinates] = indexCount;
                indexCount++;
            }
            lasFile.ClearAllPoints();  // Because I have them now.

            var VoronoiMesh = MIConvexHull.VoronoiMesh
                .Create<ptsDTMpoint, ConvexFaceTriangle>(returnObject.allPoints);

            returnObject.allTriangles = new List<ptsDTMtriangle>(2*returnObject.allPoints.Count);
            foreach(var vTriangle in VoronoiMesh.Triangles)
            {
                var point1 = gridIndexer[vTriangle.Vertices[0].GridCoordinates];
                var point2 = gridIndexer[vTriangle.Vertices[1].GridCoordinates];
                var point3 = gridIndexer[vTriangle.Vertices[2].GridCoordinates];
                returnObject.allTriangles.Add(new ptsDTMtriangle(
                    returnObject.allPoints, point1, point2, point3));
                returnObject.myBoundingBox.expandByPoint(point1, point2, point3);
            }
            
            return returnObject;
        }

        // temp scratch pad members -- do not serialize
        [NonSerialized]
        private ConcurrentBag<ptsDTMtriangle> allTrianglesBag;
        [NonSerialized]
        private ptsDTMpoint scratchPoint;
        [NonSerialized]
        private ptsDTMtriangle scratchTriangle;
        [NonSerialized]
        private IntPair scratchUIntPair;


        [NonSerialized]
        private ptsDTMtriangleLine scratchTriangleLine;
        [NonSerialized]
        private Dictionary<IntPair, ptsDTMtriangleLine> triangleLines;
        [NonSerialized]
        private long memoryUsed = 0;

        [NonSerialized]
        private Dictionary<string, Stopwatch> stpWatches;
        [NonSerialized]
        private Stopwatch aStopwatch;
        [NonSerialized]
        static Stopwatch LoadTimeStopwatch = new Stopwatch();
        [NonSerialized]
        public static readonly String StandardExtension = ".ptsTin";

        private void LoadTINfromVRML(string fileName)
        {
            string line;
            long lineCount = 0;
            if(!(String.Compare(Path.GetExtension(fileName), ".wrl", true) == 0))
            {
                throw new ArgumentException("Filename must have wrl extension.");
            }

            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            try
            {
                while((line = file.ReadLine()) != null)
                {
                    if(false == validateVRMLfileHeader(line))
                        throw new System.IO.InvalidDataException("File not in VRML2 format.");
                    break;
                }

                lineCount++;
                while((line = file.ReadLine()) != null)
                {
                    lineCount++;
                    if(line.Equals("IndexedFaceSet"))
                        break;
                }

                while((line = file.ReadLine()) != null)
                {
                    lineCount++;
                    if(line.Equals("point"))
                    {
                        line = file.ReadLine();  // eat the open brace,  [
                        break;
                    }
                }

                ulong ptIndex = 0;
                while((line = file.ReadLine()) != null)
                {
                    lineCount++;
                    // Read until the close brace,  [
                    if(line.Equals("]"))
                        break;
                    scratchPoint = convertLineOfDataToPoint(line);
                    if(allPoints == null)
                    {
                        createAllpointsCollection();
                        myBoundingBox = new ptsBoundingBox2d(scratchPoint.x, scratchPoint.y, scratchPoint.x, scratchPoint.y);
                    }
                    allPoints.Add(scratchPoint);
                    ptIndex++;
                    myBoundingBox.expandByPoint(scratchPoint.x, scratchPoint.y, scratchPoint.z);
                }


                while((line = file.ReadLine()) != null)
                {
                    lineCount++;
                    if(line.Equals("coordIndex"))
                    {
                        line = file.ReadLine();  // eat the open brace,  [
                        break;
                    }
                }

                allTriangles = new List<ptsDTMtriangle>();
                while((line = file.ReadLine()) != null)
                {
                    lineCount++;
                    // Read until the close brace,  [
                    if(line.Equals("]"))
                        break;
                    scratchTriangle = convertLineOfDataToTriangle(line);
                    allTriangles.Add(scratchTriangle);
                }

                allTriangles.Sort();
            }
            finally
            {
                file.Close();
            }
        }

        private ptsDTMtriangle convertLineOfDataToTriangle(string line)
        {
            int ptIndex1, ptIndex2, ptIndex3;
            string[] parsed = line.Split(',');
            int correction = parsed.Length - 4;
            ptIndex1 = Convert.ToInt32(parsed[0 + correction]);
            ptIndex2 = Convert.ToInt32(parsed[1 + correction]);
            ptIndex3 = Convert.ToInt32(parsed[2 + correction]);
            ptsDTMtriangle triangle = new ptsDTMtriangle(allPoints, ptIndex1, ptIndex2, ptIndex3);
            return triangle;
        }

        private ptsDTMpoint convertLineOfDataToPoint(string line)
        {
            ptsDTMpoint newPt;
            string[] preParsedLine = line.Split(',');
            string[] parsedLine = preParsedLine[preParsedLine.Length - 1].Split(' ');

            newPt = new ptsDTMpoint(
               Convert.ToDouble(parsedLine[0]),
               Convert.ToDouble(parsedLine[1]),
               Convert.ToDouble(parsedLine[2]));

            return newPt;
        }

        private bool validateVRMLfileHeader(string line)
        {
            string[] words = line.Split(' ');
            if(words.Length < 2) return false;
            if(!(words[0].Equals("#VRML", StringComparison.OrdinalIgnoreCase))) return false;
            if(!(words[1].Equals("V2.0", StringComparison.OrdinalIgnoreCase))) return false;

            return true;
        }

        /// <summary>
        /// Creates a tin file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ptsDTM CreateFromExistingFile(string fileName)
        {
            ptsDTM returnTin = new ptsDTM();

            if(!String.IsNullOrEmpty(fileName))
            {
                String ext = Path.GetExtension(fileName);
                if(ext.Equals(StandardExtension, StringComparison.OrdinalIgnoreCase))
                    returnTin = loadTinFromBinary(fileName);
                else
                    returnTin.LoadTextFile(fileName);
            }

            return returnTin;
        }

        /// <summary>
        /// Loads tin from either LandXML or VRML file, depending on the extension passed in.
        /// </summary>
        /// <param name="fileName">Use .xml extension for LandXML. Use .wrl extension for VRML.</param>
        public void LoadTextFile(string fileName)
        {
            string extension;
            if(false == File.Exists(fileName))
                throw new FileNotFoundException("File Not Found", fileName);

            extension = Path.GetExtension(fileName);
            if(extension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
                LoadTINfromLandXML(fileName);
            else if(extension.Equals(".wrl", StringComparison.OrdinalIgnoreCase))
                LoadTINfromVRML(fileName);
            else
                throw new Exception("Filename must have xml or wrl extension.");
        }

        private void LoadTINfromLandXML(string fileName)
        {
            if(!(String.Compare(Path.GetExtension(fileName), ".xml", true) == 0))
            {
                throw new ArgumentException("Filename must have xml extension.");
            }

            memoryUsed = GC.GetTotalMemory(true);
            Stopwatch stopwatch = new Stopwatch();
            List<string> trianglesAsStrings;
            setupStopWatches();

            scratchUIntPair = new IntPair();

            System.Console.WriteLine("Load XML document took:");
            stopwatch.Reset(); stopwatch.Start();
            LoadTimeStopwatch.Reset(); LoadTimeStopwatch.Start();
            using(XmlTextReader reader = new XmlTextReader(fileName))
            {
                stopwatch.Stop(); consoleOutStopwatch(stopwatch);
                System.Console.WriteLine("Seeking Pnts collection took:");
                stopwatch.Reset(); stopwatch.Start();
                reader.MoveToContent();
                reader.ReadToDescendant("Surface");
                string astr = reader.GetAttribute("name");

                // Read Points
                reader.ReadToDescendant("Pnts");
                stopwatch.Stop(); consoleOutStopwatch(stopwatch);

                System.Console.WriteLine("Loading All Points took:");
                stopwatch.Reset(); stopwatch.Start();
                reader.Read();
                while(!(reader.Name.Equals("Pnts") && reader.NodeType.Equals(XmlNodeType.EndElement)))
                {
                    UInt64 id;
                    if(reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        UInt64.TryParse(reader.GetAttribute("id"), out id);
                        reader.Read();
                        if(reader.NodeType.Equals(XmlNodeType.Text))
                        {
                            scratchPoint = new ptsDTMpoint(reader.Value, id);
                            if(allPoints == null)
                            {
                                createAllpointsCollection();
                                myBoundingBox = new ptsBoundingBox2d(scratchPoint.x, scratchPoint.y, scratchPoint.x, scratchPoint.y);
                            }
                            allPoints.Add(scratchPoint);
                            myBoundingBox.expandByPoint(scratchPoint.x, scratchPoint.y, scratchPoint.z);
                        }
                    }
                    reader.Read();
                }

                // Read Triangles, but only as strings
                stopwatch.Stop(); consoleOutStopwatch(stopwatch);
                System.Console.WriteLine(allPoints.Count.ToString() + " Points Total.");

                System.Console.WriteLine("Loading Triangle Reference Strings took:");
                stopwatch.Reset(); stopwatch.Start();
                trianglesAsStrings = new List<string>();
                if(!(reader.Name.Equals("Faces")))
                {
                    reader.ReadToFollowing("Faces");
                }
                reader.Read();
                while(!(reader.Name.Equals("Faces") && reader.NodeType.Equals(XmlNodeType.EndElement)))
                {
                    if(reader.NodeType.Equals(XmlNodeType.Text))
                    {
                        trianglesAsStrings.Add(reader.Value);
                    }
                    reader.Read();
                }
                reader.Close();
                stopwatch.Stop(); consoleOutStopwatch(stopwatch);

                System.Console.WriteLine("Generating Triangle Collection took:");
                stopwatch.Reset(); stopwatch.Start();
            }


            // assemble the allTriangles collection
            //allTriangles = new List<ptsDTMtriangle>(trianglesAsStrings.Count);
            allTrianglesBag = new ConcurrentBag<ptsDTMtriangle>();
            Parallel.ForEach(trianglesAsStrings, refString =>
               {
                   allTrianglesBag.Add(new ptsDTMtriangle(allPoints, refString));
               }
               );
            allTriangles = allTrianglesBag.OrderBy(triangle => triangle.point1.x).ToList();
            trianglesAsStrings = null; allTrianglesBag = null;
            GC.Collect(); GC.WaitForPendingFinalizers();
            memoryUsed = GC.GetTotalMemory(true) - memoryUsed;
            LoadTimeStopwatch.Stop();

            stopwatch.Stop();
            System.Console.WriteLine(allTriangles.Count.ToString() + " Total Triangles.");
            consoleOutStopwatch(stopwatch);

            //
            //System.Console.WriteLine("Indexing Triangles for adjacency took:");
            //stopwatch.Reset(); stopwatch.Start();
            //generateTriangleLineIndex();  start here
            //stopwatch.Stop(); consoleOutStopwatch(stopwatch);

        }

        public void saveJustThePointsThenReadThemAgain()
        {
            String filenameToSaveTo = @"C:\Users\Paul\Documents\Visual Studio 2010\Projects\XML Files\Garden Parkway\allPoints.binary";

            BinaryFormatter binFrmtr = new BinaryFormatter();
            using
            (Stream fstream =
               new FileStream(filenameToSaveTo, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFrmtr.Serialize(fstream, this.allPoints);
            }

            this.allPoints.Clear();
            this.allTriangles.Clear();
            GC.Collect();
            Console.WriteLine("Pausing . . .");
            Task.Delay(500);

            BinaryFormatter binFrmtr2 = new BinaryFormatter();
            using
            (Stream fstream = File.OpenRead(filenameToSaveTo))
            {
                Dictionary<UInt64, ptsDTMpoint> testPts = new Dictionary<ulong, ptsDTMpoint>();
                LoadTimeStopwatch = new Stopwatch();
                LoadTimeStopwatch.Start();
                try
                {
                    testPts = (Dictionary<UInt64, ptsDTMpoint>)binFrmtr.UnsafeDeserialize(fstream, null);
                }
#pragma warning disable 0168
                catch(InvalidCastException e)
#pragma warning restore 0168
                { return; }
                finally { LoadTimeStopwatch.Stop(); }

            }

        }

        public void saveAsBinary(string filenameToSaveTo)
        {
            if(!Path.GetExtension(filenameToSaveTo).
               Equals(StandardExtension, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                 String.Format("Filename does not have extension: {0}.", StandardExtension));
            }

            BinaryFormatter binFrmtr = new BinaryFormatter();
            using
            (Stream fstream =
               new FileStream(filenameToSaveTo, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFrmtr.Serialize(fstream, this);
            }
        }

        static public ptsDTM loadTinFromBinary(string filenameToLoad)
        {
            BinaryFormatter binFrmtr = new BinaryFormatter();
            using
            (Stream fstream = File.OpenRead(filenameToLoad))
            {
                ptsDTM aDTM = new ptsDTM();
                LoadTimeStopwatch = new Stopwatch();
                LoadTimeStopwatch.Start();
                try
                {
                    aDTM = (ptsDTM)binFrmtr.Deserialize(fstream);
                    LoadTimeStopwatch.Stop();
                    int i = 0 + 1;
                }
#pragma warning disable 0168
                catch(InvalidCastException e)
#pragma warning restore 0168
                {
                    LoadTimeStopwatch.Stop();
                    return null;
                }
                LoadTimeStopwatch.Stop();

                Parallel.ForEach(aDTM.allTriangles
                   , new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }
                   , tri => tri.computeBoundingBox());
                return aDTM;
            }
            //return null;
        }

        private void setupStopWatches()
        {
            stpWatches = new Dictionary<string, Stopwatch>();
            stpWatches.Add("Process Points", new Stopwatch());
            stpWatches.Add("Process Triangles", new Stopwatch());
        }

        private void consoleOutStopwatch(Stopwatch anSW)
        {
            TimeSpan ts = anSW.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
            Console.WriteLine();
        }

        private bool addTriangleLine(int ndx1, int ndx2, ptsDTMtriangle aTriangle)
        {
            if(ndx1 == 0 || ndx2 == 0 || aTriangle == null)
                return false;

            if(ndx1 < ndx2)
            {
                scratchUIntPair.num1 = ndx1;
                scratchUIntPair.num2 = ndx2;
            }
            else
            {
                scratchUIntPair.num1 = ndx2;
                scratchUIntPair.num2 = ndx1;
            }

            if(triangleLines == null)
            {
                triangleLines = new Dictionary<IntPair, ptsDTMtriangleLine>();
                scratchTriangleLine = new ptsDTMtriangleLine(allPoints[ndx1], allPoints[ndx2], aTriangle);
                triangleLines.Add(scratchUIntPair, scratchTriangleLine);

                return true;
            }

            bool tryGetSucces = triangleLines.TryGetValue(scratchUIntPair, out scratchTriangleLine);
            if(tryGetSucces == false)  // we must add this line to the collection
            {
                scratchTriangleLine = new ptsDTMtriangleLine(allPoints[ndx1], allPoints[ndx2], aTriangle);
                triangleLines.Add(scratchUIntPair, scratchTriangleLine);
                return true;
            }
            else
            {
                if(scratchTriangleLine.theOtherTriangle == null)
                {
                    scratchTriangleLine.theOtherTriangle = aTriangle;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void testGetTriangles(ptsDTMpoint aPoint)
        {

            aStopwatch = new Stopwatch();
            System.Console.WriteLine("given a point, return triangles by BB:");
            aStopwatch.Reset(); aStopwatch.Start();

            List<ptsDTMtriangle> triangleSubset = getTrianglesForPointInBB(aPoint) as List<ptsDTMtriangle>;

            aStopwatch.Stop(); consoleOutStopwatch(aStopwatch);
        }

        internal List<ptsDTMtriangle> getTrianglesForPointInBB(ptsDTMpoint aPoint)
        {
            return (from ptsDTMtriangle triangle in allTriangles.AsParallel()
                    where triangle.isPointInBoundingBox(aPoint)
                    select triangle).ToList<ptsDTMtriangle>();
        }

        public void testGetTriangle(ptsPoint aPoint)
        {
            aStopwatch = new Stopwatch();
            System.Console.WriteLine("given a point, return containing Triangle:");
            aStopwatch.Reset(); aStopwatch.Start();

            ptsDTMtriangle singleTriangle = getTriangleContaining((ptsDTMpoint)aPoint);

            aStopwatch.Stop(); consoleOutStopwatch(aStopwatch);
        }

        private List<ptsDTMtriangle> localGroupTriangles;
        internal ptsDTMtriangle getTriangleContaining(ptsDTMpoint aPoint)
        {
            if(null == localGroupTriangles)
                localGroupTriangles = getTrianglesForPointInBB(aPoint).AsParallel().ToList();

            ptsDTMtriangle theTriangle =
               localGroupTriangles.FirstOrDefault(aTrngl => aTrngl.contains(aPoint));

            if(null == theTriangle)
            {
                localGroupTriangles = getTrianglesForPointInBB(aPoint).AsParallel().ToList();
                theTriangle =
                   localGroupTriangles.FirstOrDefault(aTrngl => aTrngl.contains(aPoint));
            }

            return theTriangle;
        }

        public double? getElevation(ptsPoint aPoint)
        {
            return getElevation((ptsDTMpoint)aPoint);
        }
        public double? getElevation(ptsDTMpoint aPoint)
        {
            ptsDTMtriangle aTriangle = getTriangleContaining(aPoint);
            if(null == aTriangle)
                return null;

            return aTriangle.givenXYgetZ(aPoint);

        }

        public double? getSlope(ptsPoint aPoint)
        {
            return getSlope((ptsDTMpoint)aPoint);
        }

        public double? getSlope(ptsDTMpoint aPoint)
        {
            ptsDTMtriangle aTriangle = getTriangleContaining(aPoint);
            if(null == aTriangle)
                return null;

            return aTriangle.givenXYgetSlopePercent(aPoint);

        }

        public Azimuth getAspect(ptsPoint aPoint)
        {
            return getAspect((ptsDTMpoint)aPoint);
        }

        public Azimuth getAspect(ptsDTMpoint aPoint)
        {
            ptsDTMtriangle aTriangle = getTriangleContaining(aPoint);
            if(null == aTriangle)
                return null;

            return aTriangle.givenXYgetSlopeAzimuth(aPoint);

        }

        public PointSlopeAspect getElevationSlopeAzimuth(ptsDTMpoint aPoint)
        {
            return new PointSlopeAspect(
                aPoint,
                getElevation(aPoint),
                getSlope(aPoint),
                getAspect(aPoint)
                );
        }

        public void loadFromXYZtextFile(string fileToOpen)
        {
            //ptsBoundingBox2d fileBB = new ptsBoundingBox2d()
            using(var inputFile = new StreamReader(fileToOpen))
            {
                Double x, y, z;
                String line;
                String[] values;
                while((line = inputFile.ReadLine()) != null)
                {
                    values = line.Split(',');
                    if(values.Length != 3) continue;
                    var newPt = new ptsDTMpoint(values[0], values[1], values[2]);
                    GridDTMhelper.addPoint(newPt);
                }
                int i = 0;
            }
        }

        private void createAllpointsCollection()
        {
            allPoints = new List<ptsDTMpoint>();
        }

        public String GenerateSizeSummaryString()
        {
            StringBuilder returnString = new StringBuilder();
            returnString.AppendLine(String.Format(
               "Points: {0:n0} ", allPoints.Count));
            returnString.AppendLine(String.Format("Triangles: {0:n0}", this.allTriangles.Count));
            returnString.AppendLine(String.Format("Total Memory Used: Approx. {0:n0} MBytes",
               memoryUsed / (1028 * 1028)));
            returnString.AppendLine(String.Format(
               "{0:f4} Average Points per Triangle.",
               (Double)((Double)allPoints.Count / (Double)allTriangles.Count)));
            returnString.AppendLine(String.Format("Total Load Time: {0:f4} seconds",
               (Double)LoadTimeStopwatch.ElapsedMilliseconds / 1000.0));
            return returnString.ToString();
        }
    }

    internal static class GridDTMhelper
    {
        private const long GridSize = 500;
        public static Dictionary<XYtuple, List<ptsDTMpoint>> grid = new Dictionary<XYtuple, List<ptsDTMpoint>>();
        public static void addPoint(ptsDTMpoint pt)
        {
            long xGrid = (long)Math.Floor(pt.x / GridSize);
            long yGrid = (long)Math.Floor(pt.y / GridSize);
            addPoint_(new XYtuple(xGrid, yGrid), pt);
        }

        private static void addPoint_(XYtuple tupl, ptsDTMpoint pt)
        {
            if(false == grid.ContainsKey(tupl))
            {
                var ptList = new List<ptsDTMpoint>();
                ptList.Add(pt);
                grid.Add(tupl, ptList);
                long lng = (long)(int.MaxValue) + 1L;
            }
            else
            {
                grid[tupl].Add(pt);
            }
        }
    }

    internal class XYtuple
    {
        public XYtuple(long x, long y)
        {
            X = x; Y = y;
        }
        public long X { get; set; }
        public long Y { get; set; }
    }

    public class PointSlopeAspect
    {
        public ptsPoint Point { get; private set; }
        public double? Elevation { get; private set; }
        public double? Slope { get; private set; }
        public Azimuth Aspect { get; private set; }

        public PointSlopeAspect(ptsDTMpoint pt, double? el=null, double? sl=null, Azimuth aspect=null)
        {
            this.Point = new ptsPoint(pt.x, pt.y);
            if(el != null) this.Point.z = (double) el;
            this.Elevation = el;
            this.Slope = sl;
            this.Aspect = aspect;
        }

        public override string ToString()
        {
            return $"EL: {Elevation:f2}, SL: {Slope:f1}%, AS: {Aspect}";
        }

        private static double tolerance = 0.15;
        private static double ntolerance = -tolerance;
        /// <summary>
        /// Compares only the derived values, elevation, slope, and aspect.
        /// </summary>
        /// <param name="el"></param>
        /// <param name="sl"></param>
        /// <param name="aspect"></param>
        public void AssertDerivedValuesAreEqual(double? el, double? sl, Azimuth aspect)
        {
            bool verifyNullStatus_AndIsItNull(Object n1, Object n2, string msg)
            {
                if(n1 == null && n2 == null) return true;
                if(n1 != null && n2 != null) return false;
                throw new Exception(msg);
            }

            if(verifyNullStatus_AndIsItNull(el, this.Elevation, 
                "Elevation items not same null state."))
                return;

            double? diff = this.Elevation - el;
            if(diff > tolerance || diff < ntolerance)
                throw new Exception("Elevation values differ.");

            if(verifyNullStatus_AndIsItNull(sl, this.Slope, 
                "Slope items not same null state."))
                return;

            diff = this.Slope - sl;
            if(diff > tolerance || diff < ntolerance)
                throw new Exception("Slope values differ.");

            if(verifyNullStatus_AndIsItNull(aspect, this.Aspect, 
                "Aspect items not same null state."))
                return;

            diff = this.Aspect - aspect;
            if(diff > tolerance || diff < ntolerance)
                throw new Exception("Aspect values differ.");

        }
    }

}
