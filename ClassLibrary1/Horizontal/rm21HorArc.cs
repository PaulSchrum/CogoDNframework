using netDxf;
using ptsCogo.Angle;
using ptsCogo.coordinates;
using ptsCogo.coordinates.CurvilinearCoordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo.Horizontal
{
    public class rm21HorArc : HorizontalAlignmentBase
    {

        private int deflDirection { get; set; }
        public ptsPoint ArcCenterPt { get; protected set; }
        public override Double Radius { get; protected set; }
        public ptsVector BeginRadiusVector { get; protected set; }
        public ptsVector EndRadiusVector { get; protected set; }

        public static rm21HorArc Create(ptsPoint begPt, ptsPoint endPt,
            Azimuth incomingAzimuth, Double radius)
        {
            return new rm21HorArc(begPt, endPt, incomingAzimuth, radius);
        }

        public rm21HorArc(ptsPoint begPt, ptsPoint endPt, Azimuth incomingAzimuth, Double radius)
           : base(begPt, endPt)
        {
            if(0 == utilFunctions.tolerantCompare((endPt - begPt).Length, 0.0, 0.000001))
                throw new ArcExceptionZeroLengthNotDefined();

            Double tanOffsetToEndPt;
            ptsRay incomingRay = new ptsRay(); incomingRay.advanceDirection = 1;
            incomingRay.StartPoint = begPt; incomingRay.HorizontalDirection = incomingAzimuth;
            tanOffsetToEndPt = incomingRay.getOffset(endPt);

            if(0 == utilFunctions.tolerantCompare(tanOffsetToEndPt, 0.0, 0.000001))
                throw new ArcExceptionZeroDeflectionNotDefined();

            this.BeginStation = 0.0;
            this.Radius = Math.Abs(radius);
            this.BeginAzimuth = incomingAzimuth;

            deflDirection = Math.Sign(tanOffsetToEndPt);
            //Deflection defl = Deflection.ctorDeflectionFromAngle(90.0, deflDirection);
            Deflection defl = Deflection.ctorDeflectionFromAngle(90.0, deflDirection);
            Azimuth azToCenter = incomingAzimuth + defl;
            ptsVector traverseToCenterVec = new ptsVector(azToCenter, radius);
            ArcCenterPt = begPt + traverseToCenterVec;

            this.BeginRadiusVector = this.ArcCenterPt - this.BeginPoint;
            Azimuth endVecAz = (this.ArcCenterPt - endPt).Azimuth;
            this.EndRadiusVector = new ptsVector(endVecAz, radius);
            this.EndPoint = this.ArcCenterPt + this.EndRadiusVector;

            var deflectionDbl = endVecAz - this.BeginRadiusVector.Azimuth;
            this.Deflection = new Deflection(deflectionDbl, deflDirection);
            this.EndAzimuth = this.BeginAzimuth + this.Deflection;

            // applies to English projects only (for now)
            this.BeginDegreeOfCurve = HorizontalAlignmentBase.computeDegreeOfCurve(this.Radius);
            this.EndDegreeOfCurve = this.BeginDegreeOfCurve;

            this.Length = 100.0 * this.Deflection.getAsRadians() / this.BeginDegreeOfCurve.getAsRadians();
            this.Length = Math.Abs(this.Length);
            this.EndStation = this.BeginStation + this.Length;


        }

        public rm21HorArc(ptsPoint begPt, ptsPoint centerPt, ptsPoint endPt, expectedType ExpectedType,
           int deflectionDirection)
           : base(begPt, endPt)
        {
            this.deflDirection = deflectionDirection;
            this.ArcCenterPt = centerPt;
            this.BeginRadiusVector = this.ArcCenterPt - this.BeginPoint;
            this.EndRadiusVector = this.ArcCenterPt - this.EndPoint;

            this.Radius = this.BeginRadiusVector.Length;
            Double validationRadius = this.EndRadiusVector.Length;
            if(Math.Abs(this.Radius - validationRadius) > 0.00014)
                throw new Exception("Given points do not represent a circle.");

            Double degreesToAdd = 90 * deflectionDirection;
            if(ExpectedType == expectedType.ArcSegmentOutsideSoluion)
                deflectionDirection *= -1;

            this.BeginAzimuth = this.BeginRadiusVector.Azimuth + ptsAngle.radiansFromDegree(degreesToAdd);
            this.EndAzimuth = this.EndRadiusVector.Azimuth + ptsAngle.radiansFromDegree(degreesToAdd);

            // applies to English projects only (for now)
            this.BeginDegreeOfCurve = HorizontalAlignmentBase.computeDegreeOfCurve(this.Radius);
            this.EndDegreeOfCurve = this.BeginDegreeOfCurve;

            if(ExpectedType == expectedType.ArcSegmentOutsideSoluion)
            {
                computeDeflectionForOutsideSolutionCurve();
                this.Length = 100.0 * this.Deflection.getAsRadians() / this.BeginDegreeOfCurve.getAsRadians();
                this.Length = Math.Abs(this.Length);
            }
            else
            {
                Deflection = new Deflection(this.BeginAzimuth, this.EndAzimuth, true);
                Double deflAsRadians = this.Deflection.getAsRadians();
                Double DcAsRadians = this.BeginDegreeOfCurve.getAsRadians();
                this.Length = 100.0 * deflAsRadians / DcAsRadians;
                this.Length = 100.0 * this.Deflection.getAsRadians() / this.BeginDegreeOfCurve.getAsRadians();
                this.Length = Math.Abs(this.Length);
            }
        }

        public static rm21HorArc Create(ptsRay inRay, double length, double degree)
        {
            double radius = Math.Abs(degree.RadiusFromDegreesDbl());
            ptsDegree deg = ptsDegree.newFromDegrees(degree);

            // Equations from Hickerson, pp 64 - 66
            // Traverse from start to PI, turn by Defl, then traverse to endPt
            Deflection Defl = length * deg.getAsRadians() / degreeOfCurveLength;

            double Tlength = radius * Math.Tan(Math.Abs(Defl.getAsRadians()) / 2.0);
            ptsVector tan1 = new ptsVector(inRay.HorizontalDirection, Tlength);
            ptsPoint PointIntersection = inRay.StartPoint + tan1;
            ptsVector tan2 = new ptsVector(inRay.HorizontalDirection + Defl, Tlength);

            ptsPoint endPt = PointIntersection + tan2;

            return Create(inRay.StartPoint, endPt, inRay.HorizontalDirection, radius);
        }

        private void computeDeflectionForOutsideSolutionCurve()
        {
            Double radVector1Az = this.BeginRadiusVector.Azimuth.getAsDegreesDouble();
            Double radVector2Az = this.EndRadiusVector.Azimuth.getAsDegreesDouble();

            int quadrantAZ1 = Azimuth.getQuadrant(radVector1Az);
            int quadrantAZ2 = Azimuth.getQuadrant(radVector2Az);

            Double defl = radVector2Az - radVector1Az;

            if(1 == this.deflDirection)
            {
                if(defl < 0.0) defl += 360.0;
            }
            else
            {
                defl = defl - 360.0;
            }

            Deflection = Deflection.ctorDeflectionFromAngle(defl, this.deflDirection);

        }

        public override StringBuilder createTestSetupOfFundamentalGeometry()
        {
            throw new NotImplementedException();
        }

        public override List<StationOffsetElevation> getStationOffsetElevation(ptsPoint interestPoint)
        {
            ptsVector arcCenterToInterestPtVector = new ptsVector(this.ArcCenterPt, interestPoint);
            Deflection deflToInterestPt = new Deflection(this.BeginRadiusVector.Azimuth, arcCenterToInterestPtVector.Azimuth, true);
            int arcDeflDirection = Math.Sign(this.Deflection.getAsDegreesDouble());
            if(arcDeflDirection * deflToInterestPt.getAsDegreesDouble() < 0.0)
            {
                return null;
            }
            else if(Math.Abs(this.Deflection.getAsDegreesDouble()) - Math.Abs(deflToInterestPt.getAsDegreesDouble()) < 0.0)
            {
                return null;
            }

            Double interestLength = this.Length * deflToInterestPt.getAsRadians() / this.Deflection.getAsRadians();
            Offset offset = new Offset(arcDeflDirection * (this.Radius - arcCenterToInterestPtVector.Length));

            var soe = new StationOffsetElevation(this.BeginStation + interestLength, offset);
            var returnList = new List<StationOffsetElevation>();
            returnList.Add(soe);
            return returnList;
        }

        public override ptsPoint getXYZcoordinates(StationOffsetElevation anSOE)
        {
            if(anSOE.station < this.BeginStation || anSOE.station > this.EndStation)
                return null;

            Double lengthIntoCurve = anSOE.station - this.BeginStation;

            Deflection deflToSOEpoint = new Deflection(this.BeginDegreeOfCurve.getAsRadians() * lengthIntoCurve / 100.0, this.deflDirection);
            Azimuth ccToSOEpointAzimuth = this.BeginRadiusVector.Azimuth + deflToSOEpoint;

            Double ccToSOEpointDistance = this.Radius - this.deflDirection * anSOE.offset.OFST;

            ptsVector ccToSOEpoint = new ptsVector(ccToSOEpointAzimuth, ccToSOEpointDistance);

            return this.ArcCenterPt + ccToSOEpoint;
        }

        public override void drawHorizontalByOffset
           (IPersistantDrawer_Cogo drawer, StationOffsetElevation soe1, StationOffsetElevation soe2)
        {
            ptsPoint startPoint = this.getXYZcoordinates(soe1);
            ptsPoint endPoint = this.getXYZcoordinates(soe2);
            drawer.PlaceArc(this, startPoint, soe1, endPoint, soe2);
        }


        internal void setDeflection(Deflection newDeflection)
        {
            if(newDeflection.getAsRadians() == 0.0)
                throw new Exception("Can't create arc with zero degree deflection.");

            this.Deflection = newDeflection;
            this.deflDirection = Math.Sign(newDeflection.getAsRadians());

            //var anAngle = this.BeginRadiusVector + this.Deflection;
            //var newAz = Azimuth.newAzimuthFromAngle(anAngle);
            this.EndRadiusVector = this.BeginRadiusVector + this.Deflection;
            this.EndAzimuth = this.BeginAzimuth + this.Deflection;
            this.Length = 100.0 * this.Deflection.getAsRadians() / this.BeginDegreeOfCurve.getAsRadians();
            this.Length = Math.Abs(this.Length);
            this.EndStation = this.BeginStation + this.Length;
            this.EndPoint = this.ArcCenterPt + this.EndRadiusVector;
        }

        public override void draw(ILinearElementDrawer drawer)
        {
            drawer.drawArcSegment(this.BeginPoint, this.ArcCenterPt, this.EndPoint,
               this.Deflection.getAsRadians());
        }

        public override void AddToDxf(DxfDocument dxfDoc)
        {
            double getCadAngle(ptsVector vec)
            {
                return ptsAngle.degreesFromRadians(Math.Atan2(vec.y, vec.x));
            }

            var startAngle = getCadAngle(this.BeginRadiusVector);
            var endAngle = getCadAngle(this.EndRadiusVector);
            netDxf.Entities.Arc theArc = null;
            if(this.deflDirection < 0)
                theArc =  new netDxf.Entities.Arc(
                    new Vector2(this.ArcCenterPt.x, this.ArcCenterPt.y),
                    this.Radius, startAngle, endAngle);
            else
                theArc = new netDxf.Entities.Arc(
                    new Vector2(this.ArcCenterPt.x, this.ArcCenterPt.y),
                    this.Radius, endAngle, startAngle);

            dxfDoc.AddEntity(theArc);
        }

    }
    public class ArcException : Exception
    {
        public ArcException(String msg) : base(msg) { }
    }

    public class ArcExceptionZeroDeflectionNotDefined : ArcException
    {
        public ArcExceptionZeroDeflectionNotDefined() :
           base("Arc with zero-degree deflection not defined.")
        { }
    }

    public class ArcExceptionZeroLengthNotDefined : ArcException
    {
        public ArcExceptionZeroLengthNotDefined() :
           base("Arc with zero-length not defined.")
        { }
    }

}
