using ptsCogo.coordinates.CurvilinearCoordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo.Angle;
using ptsCogo.coordinates;
using ptsCogo.Utils;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("Tests")]

namespace ptsCogo.Horizontal
{
    public class rm21HorizontalAlignment : HorizontalAlignmentBase
    {
        private List<HorizontalAlignmentBase> allChildSegments { get; set; } = new List<HorizontalAlignmentBase>();
        private List<alignmentDataPacket> alignmentData { get; set; }

        // used in constructing, never kept long term
        // scratch pad member for re-use.  Not part of the data structure.
        private List<HorizontalAlignmentBase> allChildSegments_scratchPad = null;

        // scratch pad member for re-use.  Not part of the data structure.
        List<ptsPoint> ptList = null;

        public rm21HorizontalAlignment() { }

        public rm21HorizontalAlignment(List<IRM21fundamentalGeometry> fundamentalGeometryList,
           String Name, List<Double> stationEquationing)
           : base(stationEquationing)
        {
            if(null == fundamentalGeometryList)
            { throw new NullReferenceException("Fundamental Geometry List may not be null."); }

            this.Name = Name;

            createAllSegments(fundamentalGeometryList);
            orderAllSegments_startToEnd();

            restationAlignment();
        }

        public static rm21HorizontalAlignment createFromCsvFile(string csvFileName)
        {
            rm21HorizontalAlignment retAlign = new rm21HorizontalAlignment();
            List<string> allLines = File.ReadAllLines(csvFileName).ToList();
            var tableStartLines = new Dictionary<string, int>();
            int rowCount = 0;
            foreach(var row in allLines)
            {
                var splitByColon = row.Split(':');
                if(splitByColon.Count() > 1)
                    tableStartLines[splitByColon[0]] = rowCount;
                rowCount++;
            }

            // process start point, direction, and station
            var startRayRow = allLines[tableStartLines["StartHA"] + 2].Split(',');
            var startRay = new ptsRay(x: startRayRow[0],
                y: startRayRow[1], z: null, azimuth: startRayRow[4]);

            double startStation = Convert.ToDouble(startRayRow[2]);
            int startRegion = Convert.ToInt32(startRayRow[3]);
            retAlign.BeginStation = startStation;
            // end "process start point, direction, and station"

            int lineCount = 0;
            // Read in all elements
            var endingRay = startRay;
            for(int i=tableStartLines["Elements"]+2; i<tableStartLines["Regions"]; i++)
            {
                var elementRow = allLines[i].Split(',').ToList();
                if(String.IsNullOrEmpty(elementRow[0]))
                    break;

                double degreeIn = Convert.ToDouble(elementRow[0]);
                double length = Convert.ToDouble(elementRow[1]);
                double degreeOut = Convert.ToDouble(elementRow[2]);
                var aNewSegment = newSegment(endingRay, degreeIn, length, degreeOut);
                aNewSegment.Parent = retAlign;
                endingRay = aNewSegment.EndRay;
                retAlign.allChildSegments.Add(aNewSegment);
                lineCount++;
            }
            // end "Read in all elements"

            // set up stations and regions
            // ToDo: Get this working correctly for alignments with regions
            //string begSta_ = allLines[tableStartLines["Regions"] + 2].Split(',').FirstOrDefault();

            //double begSta = String.IsNullOrEmpty(begSta_) ? 0.0 : Convert.ToDouble(begSta_);
            retAlign.restationAlignment();
            // "set up stations and regions"

            // Fill in essential properties
            retAlign.Parent = null;
            retAlign.Radius = 0.0;

            var firstItem = retAlign.allChildSegments.First();
            retAlign.BeginAzimuth = firstItem.BeginAzimuth;
            retAlign.BeginDegreeOfCurve = firstItem.BeginDegreeOfCurve;
            retAlign.BeginPoint = firstItem.BeginPoint;
            
            var lastItem = retAlign.allChildSegments.Last();
            retAlign.EndAzimuth = lastItem.EndAzimuth;
            retAlign.EndDegreeOfCurve = lastItem.EndDegreeOfCurve;
            retAlign.EndPoint = lastItem.EndPoint;
            // end of "Fill in essential properties;

            // validate the alignment ends on the last point
            // end "validate the alignment ends on the last point"


            
            return retAlign;
        }

        internal static HorizontalAlignmentBase newSegment(ptsRay inRay, double degreeIn, double length,
            double degreeOut)
        {
            Double tolerance = 0.00005;
            int areEqual = cogoUtils.tolerantCompare(degreeIn, degreeOut, tolerance);
            if(areEqual == 0)
            {
                int isZero = cogoUtils.tolerantCompare(degreeIn, 0.0, tolerance);
                if(isZero == 0)
                {
                    return new rm21HorLineSegment(inRay, length);
                }
                else
                {
                    return rm21HorArc.Create(inRay, length, degreeIn);
                }
            }
            //else it is an Euler Spiral
            return rm21HorSpiralc.Create(inRay, length, degreeIn, degreeOut);

        }

        private void createAllSegments(List<IRM21fundamentalGeometry> fundamentalGeometryList)
        {
            foreach(var fgElement in fundamentalGeometryList)
            {
                switch(fgElement.getExpectedType())
                {
                    case expectedType.LineSegment:
                        createAddLineSegment(fgElement);
                        break;
                    case expectedType.EulerSpiral:
                        createAddEulerSpiralSegment(fgElement);
                        break;
                    default:
                        createAddArcSegment(fgElement);
                        break;
                }
            }
        }

        private void createAddLineSegment(IRM21fundamentalGeometry fundGeomLineSeg)
        {
            ptList = fundGeomLineSeg.getPointList();
            if(2 != ptList.Count)
                throw new Exception("Line Segment fundamental geometry must have two and only two points.");

            rm21HorLineSegment aLineSeg = new rm21HorLineSegment(ptList[0], ptList[1]);

            if(null == allChildSegments_scratchPad) allChildSegments_scratchPad = new List<HorizontalAlignmentBase>();

            allChildSegments_scratchPad.Add(aLineSeg);
        }

        private void createAddArcSegment(IRM21fundamentalGeometry fundGeomArcSeg)
        {
            ptList = fundGeomArcSeg.getPointList();
            if(3 != ptList.Count)
                throw new Exception("Arc Segment fundamental geometry must have three and only three points.");

            rm21HorArc anArg = new rm21HorArc(ptList[0], ptList[1], ptList[2], fundGeomArcSeg.getExpectedType(),
               fundGeomArcSeg.getDeflectionSign());

            if(null == allChildSegments_scratchPad) allChildSegments_scratchPad = new List<HorizontalAlignmentBase>();

            allChildSegments_scratchPad.Add(anArg);
        }


        private void createAddEulerSpiralSegment(IRM21fundamentalGeometry fundGeomEulerSpiralSeg)
        {
            ptList = fundGeomEulerSpiralSeg.getPointList();
            if(ptList.Count < 4)
                throw new Exception("Euler Spiral Segment fundamental geometry must have at least four points.");

            throw new NotImplementedException("RM21 Horizontal Alignment Euler Sprial Segment");
        }

        private void orderAllSegments_startToEnd()
        {
            var candidateFirstItems = new List<HorizontalAlignmentBase>();

            // Figure out which element is first.
            // if more than one element has no prior connection, it is in error.
            foreach(var item in allChildSegments_scratchPad)
            {
                bool hasNoBeginConnection = true;
                foreach(var otherItem in allChildSegments_scratchPad)
                {
                    if(item != otherItem)
                    {
                        hasNoBeginConnection = !theseConnectAtItemBeginPt(item, otherItem);
                        if(false == hasNoBeginConnection)
                            break;
                    }
                }

                if(true == hasNoBeginConnection)
                    candidateFirstItems.Add(item);
            }

            if(candidateFirstItems.Count == 1)
            {
                HorizontalAlignmentBase geometricallyFirstElement = candidateFirstItems.FirstOrDefault<HorizontalAlignmentBase>();
                allChildSegments = new List<HorizontalAlignmentBase>();
                allChildSegments.Add(geometricallyFirstElement);
                allChildSegments_scratchPad.Remove(geometricallyFirstElement);
            }
            else if(candidateFirstItems.Count > 1)
            {
                throw new Exception("Disjointed element list not permitted");
            }
            else
            {
                throw new Exception("Can't determine which element is first.");
            }
            // end "Figure out which element is first."
            // if more than one element has no prior connection, it is in error.

            var currentLastItem = allChildSegments.Last<HorizontalAlignmentBase>();
            if(0 == allChildSegments_scratchPad.Count) return;

            while(allChildSegments_scratchPad.Count > 0)
            {
                var Iter = allChildSegments_scratchPad.GetEnumerator();
                while(Iter.MoveNext())
                {
                    if(true == theseConnectAtItemBeginPt(Iter.Current, currentLastItem))
                    {
                        allChildSegments.Add(Iter.Current);
                        currentLastItem = allChildSegments.Last<HorizontalAlignmentBase>();
                        allChildSegments_scratchPad.Remove(Iter.Current);
                        break;
                    }
                }
            }
            allChildSegments_scratchPad = null;

        }

        private bool theseConnectAtItemBeginPt(HorizontalAlignmentBase itemInQuestion, HorizontalAlignmentBase secondItem)
        {
            Double equalityTolerance = 0.00015;

            var distanceToEndVector = itemInQuestion.BeginPoint - secondItem.EndPoint;
            var distanceToBeginVector = itemInQuestion.BeginPoint - secondItem.BeginPoint;
            return (Math.Abs(distanceToEndVector.Length) < equalityTolerance) ||
                   (Math.Abs(distanceToBeginVector.Length) < equalityTolerance);
        }

        private void restationAlignment()
        {
            if(null == allChildSegments) return;
            Double runningTrueStation = this.BeginStation;
            foreach(var segment in allChildSegments)
            {
                segment.BeginStation = runningTrueStation;
                runningTrueStation += segment.Length;
                segment.EndStation = runningTrueStation;
                this.EndStation = runningTrueStation;
                segment.restationFollowOnOperation();
            }
        }

        public override StringBuilder createTestSetupOfFundamentalGeometry()
        {
            throw new NotImplementedException();
            var sb = new StringBuilder();

            foreach(var item in allChildSegments)
            {
                sb.Append(item.createTestSetupOfFundamentalGeometry());
            }

            return sb;
        }

        public new Double Length
        {
            get { return this.EndStation - this.BeginStation; }
            private set { }
        }

        public void getCrossSectionEndPoints(
           CogoStation station,
           out ptsPoint leftPt,
           double leftOffset,
           out ptsPoint rightPt,
           double rightOffset)
        {
            leftPt = null;
            rightPt = null;
        }

        public override List<ptsPoint> getPoints(coordinates.CurvilinearCoordinates.StationOffsetElevation anSOE)
        {
            var returnList = new List<ptsPoint>();

            foreach(var segment in allChildSegments)
            {
                returnList.AddRange(segment.getPoints(anSOE));
            }

            return returnList;
        }

        public Dictionary<StationOffsetElevation, ptsPoint> getPoints(IEnumerable<StationOffsetElevation> SOEs)
        {
            Dictionary<StationOffsetElevation, ptsPoint> returnDict = 
                new Dictionary<StationOffsetElevation, ptsPoint>();

            foreach(var anSoe in SOEs.OrderBy(st => st.station))
                returnDict[anSoe] = getXYZcoordinates(anSoe);

            return returnDict;
        }

        public Dictionary<StationOffsetElevation, ptsPoint> getXYZcoordinateList(
            double increment,
            double offset = 0.0,
            StationOffsetElevation beginStation = null,
            StationOffsetElevation endStation = null,
            String csvOutFileName = null)
        {
            if(increment <= 0.0)
                throw new Exception("increment value must be greater than 0.");

            StationOffsetElevation begSta = beginStation == null ?
                this.BeginStation : beginStation;
            StationOffsetElevation endSta = endStation == null ?
                this.EndStation : endStation;

            var stationList = new List<StationOffsetElevation>();
            var aSta = begSta;
            while(aSta <= endSta)
            {
                stationList.Add(aSta);
                aSta = new StationOffsetElevation(
                    aSta.station + increment, 
                    aSta.offset, aSta.elevation);
            }

            var returnDict = new Dictionary<StationOffsetElevation, ptsPoint>();

            foreach(var anSOE in stationList)
            {
                returnDict[anSOE] = this.getXYZcoordinates(anSOE);
            }

            if(csvOutFileName != null)
            {
                using(StreamWriter outFile = new StreamWriter(csvOutFileName))
                {
                    outFile.WriteLine("X,Y");
                    foreach(var row in returnDict.Values)
                    {
                        outFile.Write(row.x);
                        outFile.Write(",");
                        outFile.WriteLine(row.y);
                    }

                }
            }
            return returnDict; 
        }

        public override List<StationOffsetElevation> getStationOffsetElevation(ptsPoint aPoint)
        {
            var returnList = new List<StationOffsetElevation>();

            foreach(var segment in allChildSegments)
            {
                var SOEforThisSegment = segment.getStationOffsetElevation(aPoint);
                if(null != SOEforThisSegment)
                    returnList.AddRange(SOEforThisSegment);
            }

            returnList = returnList.OrderBy(soe => Math.Abs(soe.offset)).ToList<StationOffsetElevation>();

            return returnList;
        }

        public ptsPoint getXYZcoordinates(double station, double offset, double elevation)
        {
            var anSOE = new StationOffsetElevation(station, new Offset(offset), new Elevation(elevation));
            return getXYZcoordinates(anSOE);
        }

        public override ptsPoint getXYZcoordinates(StationOffsetElevation anSOE)
        {
            if(anSOE.station < this.BeginStation || anSOE.station > this.EndStation)
                return null;

            ptsPoint returnPoint = new ptsPoint();
            foreach(var segment in this.allChildSegments)
            {
                if(anSOE.station < segment.EndStation)
                {
                    return segment.getXYZcoordinates(anSOE);
                }
            }
            return null;
        }

        public List<CogoStation> getChangePoints()
        {
            List<CogoStation> returnList =
               (from item in allChildSegments
                select (CogoStation)item.BeginStation).ToList();
            returnList.Add((CogoStation)this.EndStation);
            return returnList;
        }

        public HorizontalAlignmentBase GetElementByStation(double testStation)
        {
            HorizontalAlignmentBase returnItem = null;
            foreach(var item in allChildSegments)
            {
                if(testStation <= item.EndStation)
                {
                    returnItem = item;
                    break;
                }
            }
            return returnItem;
        }

        public void reset(ptsPoint pt1, ptsPoint pt2)
        {
            var lineSeg = new rm21HorLineSegment(pt1, pt2);
            lineSeg.Parent = this;
            allChildSegments = new List<HorizontalAlignmentBase>();
            allChildSegments.Add(lineSeg);
            this.BeginStation = 0.0;
            this.EndStation = lineSeg.EndStation;
            this.Length = this.EndStation - this.BeginStation;
            this.BeginAzimuth = lineSeg.BeginAzimuth;
            this.EndAzimuth = lineSeg.EndAzimuth;
            this.BeginDegreeOfCurve = this.EndDegreeOfCurve = 0.0;
            this.BeginPoint = lineSeg.BeginPoint;
            this.EndPoint = lineSeg.EndPoint;
            restationAlignment();

            alignmentData = new List<alignmentDataPacket>();
            alignmentData.Add(new alignmentDataPacket(0, lineSeg));
        }

        public void appendArc(ptsPoint ArcEndPoint, Double radius)
        {
            var newArc = new rm21HorArc(this.EndPoint, ArcEndPoint, this.EndAzimuth, radius);
            newArc.BeginStation = this.EndStation;
            newArc.EndStation = newArc.BeginStation + newArc.Length;

            newArc.Parent = this;
            this.allChildSegments.Add(newArc);

            this.EndAzimuth = this.BeginAzimuth + newArc.Deflection;
            this.EndPoint = newArc.EndPoint;
            restationAlignment();

            alignmentData.Add(new alignmentDataPacket(alignmentData.Count, newArc));
        }

        public void appendTangent(ptsPoint TangentEndPoint)
        {
            // See "Solving SSA triangles" 
            // http://www.mathsisfun.com/algebra/trig-solving-ssa-triangles.html

            var lastChainItem = allChildSegments.Last();
            if(!(lastChainItem is rm21HorArc))
                throw new Exception("Can't add tangent on a tangent segment.");

            var finalArc = lastChainItem as rm21HorArc;

            var incomingTanRay = new ptsRay(); incomingTanRay.StartPoint = finalArc.BeginPoint;
            incomingTanRay.HorizontalDirection = finalArc.BeginAzimuth;
            int offsetSide = Math.Sign(incomingTanRay.getOffset(TangentEndPoint));
            double rad = finalArc.Radius;
            Azimuth traverseToRevisedCenterPtAzimuth = finalArc.BeginAzimuth + offsetSide * Math.PI / 2.0;
            ptsVector traverseToRevisedCenterPt = new ptsVector(traverseToRevisedCenterPtAzimuth, rad);
            ptsPoint revCenterPt = finalArc.BeginPoint + traverseToRevisedCenterPt;

            ptsVector ccToTEPvec = finalArc.ArcCenterPt - TangentEndPoint;

            ptsAngle rho = Math.Asin(rad / ccToTEPvec.Length);
            ptsAngle ninetyDegrees = new ptsAngle();
            ninetyDegrees.setFromDegreesDouble(90.0);
            ptsAngle tau = ninetyDegrees - rho;

            Azimuth ccToPtVecAz = ccToTEPvec.Azimuth;
            Azimuth arcBegRadAz = finalArc.BeginRadiusVector.Azimuth;

            ptsCogo.Angle.Deflection outerDefl = ccToPtVecAz.minus(arcBegRadAz);
            ptsDegree defl = Math.Abs((tau - outerDefl).getAsDegreesDouble()) * offsetSide;
            Deflection newDeflection = new Deflection(defl.getAsRadians());
            finalArc.setDeflection(newDeflection: newDeflection);

            var appendedLineSegment = new rm21HorLineSegment(finalArc.EndPoint, TangentEndPoint);
            appendedLineSegment.BeginStation = finalArc.EndStation;
            appendedLineSegment.Parent = this;
            allChildSegments.Add(appendedLineSegment);

            this.EndAzimuth = appendedLineSegment.EndAzimuth;
            this.EndPoint = appendedLineSegment.EndPoint;
            restationAlignment();

            if(alignmentData.Count > 0)
                alignmentData.RemoveAt(alignmentData.Count - 1);
            alignmentData.Add(new alignmentDataPacket(alignmentData.Count, finalArc));
            alignmentData.Add(new alignmentDataPacket(alignmentData.Count, appendedLineSegment));
        }

        public override void draw(ILinearElementDrawer drawer)
        {
            foreach(var child in allChildSegments)
            {
                child.draw(drawer);
            }
            drawer.setAlignmentValues(this.alignmentData);  // technical debt: add tests for this
        }

        public void drawTemporary(ILinearElementDrawer drawer)
        {
            drawer.setDrawingStateTemporary();
            this.draw(drawer);
        }

        public void drawPermanent(ILinearElementDrawer drawer)
        {
            drawer.setDrawingStatePermanent();
            this.draw(drawer);
        }

        public long childCount()
        {
            return this.allChildSegments.Count;
        }

        public void removeFinalChildItem()
        {
            allChildSegments.RemoveAt(allChildSegments.Count - 1);
            var newLastElement = allChildSegments[allChildSegments.Count - 1];
            this.EndAzimuth = newLastElement.EndAzimuth;
            this.EndDegreeOfCurve = newLastElement.EndDegreeOfCurve;
            this.EndPoint = newLastElement.EndPoint;
            this.EndStation = newLastElement.EndStation;
            alignmentData.RemoveAt(alignmentData.Count - 1);
        }

        public String getDeflectionOfFinalArc()
        {
            int index;
            for(index = allChildSegments.Count - 1; index >= 0; index--)
            {
                var child = allChildSegments[index];
                if(child is rm21HorArc) return (child as rm21HorArc).Deflection.ToString();
            }
            return String.Empty;
        }


        public int ChildCount { get { return this.allChildSegments.Count; } }

        public HorizontalAlignmentBase getChildBySequenceNumber(int index)
        {
            if(index >= this.ChildCount) return null;
            return this.allChildSegments[index];
        }

        public IEnumerable<ptsPoint> GetAllEventPoints()
        {
            var returnList = new List<ptsPoint>();

            var kids = this.allChildSegments;
            if(kids is null) return null;

            returnList.Add(kids.First().BeginPoint);
            foreach(var item in kids)
                returnList.Add(item.EndPoint);

            return returnList;
        }

        public void WriteEventPointsToCSV(string outFile)
        { // From https://stackoverflow.com/a/18757247/1339950
            StringBuilder outString = new StringBuilder();

            outString.AppendLine("x,y,z");
            foreach(var pt in this.GetAllEventPoints())
                outString.AppendLine(pt.ToString());

            var v = outString.ToString();
            File.WriteAllText(outFile, outString.ToString());
        }
    }

    public sealed class alignmentDataPacket
    {

        public int myIndex { get; set; }
        public Double BeginStationDbl { get; set; }
        public Double Length { get; set; }
        public Double Radius { get; set; }
        public Deflection Deflection { get; set; }
        public Boolean HasChanged { get; set; }

        public alignmentDataPacket(int p, HorizontalAlignmentBase alignmentItem)
        {
            myIndex = p;
            BeginStationDbl = alignmentItem.BeginStation;
            Length = alignmentItem.Length;
            Radius = alignmentItem.Radius;
            Deflection = alignmentItem.Deflection;
            HasChanged = true;
        }
    }

}
