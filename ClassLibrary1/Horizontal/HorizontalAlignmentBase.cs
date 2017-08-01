﻿using ptsCogo.Angle;
using ptsCogo.coordinates.CurvilinearCoordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo.Horizontal
{
    public class HorizontalAlignmentBase : GenericAlignment
    {
        public HorizontalAlignmentBase() : base() { }

        public HorizontalAlignmentBase(ptsPoint begPt, ptsPoint endPt)
           : base()
        {
            BeginPoint = begPt;
            EndPoint = endPt;

        }

        public HorizontalAlignmentBase(List<Double> stationEquationingList) : base(stationEquationingList)
        {

        }

        public virtual ptsPoint BeginPoint { get; protected set; }
        public virtual ptsPoint EndPoint { get; protected set; }

        public virtual Azimuth BeginAzimuth { get; protected set; }
        public virtual Azimuth EndAzimuth { get; protected set; }

        public virtual ptsAngle BeginDegreeOfCurve { get; protected set; }
        public virtual ptsAngle EndDegreeOfCurve { get; protected set; }

        public virtual Deflection Deflection { get; protected set; }
        public virtual Double Length { get; protected set; }
        public virtual Double Radius { get; protected set; }

        protected List<HorizontalAlignmentBase> incomingElements { get; set; }
        protected List<HorizontalAlignmentBase> outgoingElements { get; set; }

        public virtual ptsVector LongChordVector
        { get { return (new ptsVector(this.BeginPoint, this.EndPoint)).flattenZnew(); } }

        static HorizontalAlignmentBase() { degreeOfCurveLength = 100; }
        static public Double degreeOfCurveLength { get; set; }
        static public ptsAngle computeDegreeOfCurve(Double radius)
        {
            return new ptsAngle(radius, degreeOfCurveLength);
        }

        public virtual StringBuilder createTestSetupOfFundamentalGeometry() { return null; }

        public virtual List<ptsPoint> getPoints(coordinates.CurvilinearCoordinates.StationOffsetElevation anSOE)
        {
            throw new NotImplementedException();
        }

        public virtual ptsPoint getXYZcoordinates(StationOffsetElevation anSOE)
        {
            return null;
        }

        public virtual List<StationOffsetElevation> getStationOffsetElevation(ptsPoint aPoint)
        {
            throw new NotImplementedException();
        }

        /* * /
        public bool isUnitUSsurveyFoot { get { return thisUnit == Unit.SurveyFoot; } set { thisUnit = Unit.SurveyFoot; } }
        public bool isUnitFoot { get; set; }
        public bool isUnitMeter { get; set; }

        private enum Unit{Meter, Foot, SurveyFoot}
        private HorizontalAlignmentBase.Unit thisUnit { get; set; }  /*  */

        public virtual void drawHorizontalByOffset
           (IPersistantDrawer_Cogo drawer, StationOffsetElevation soe1, StationOffsetElevation soe2)
        {

        }

        public virtual void draw(ILinearElementDrawer drawer)
        {

        }

        public virtual ptsVector MoveStartPtTo(ptsPoint newBeginPoint)
        {
            var moveDistance = newBeginPoint - this.BeginPoint;
            this.MoveBy(moveDistance);
            return moveDistance;
        }

        public virtual void MoveBy(ptsVector moveDistance)
        {
            this.BeginPoint = this.BeginPoint + moveDistance;
            this.EndPoint = this.EndPoint + moveDistance;
        }
    }
}
