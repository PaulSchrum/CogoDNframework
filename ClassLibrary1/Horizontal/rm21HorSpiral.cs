using ptsCogo.Angle;
using ptsCogo.coordinates;
using ptsCogo.coordinates.CurvilinearCoordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo.Horizontal
{
    public class rm21HorSpiralc : HorizontalAlignmentBase
    {

    }

    internal struct sprialPhantomValues
    {
        Double Length { get; set; }
        Deflection Deflection { get; set; }
        ptsPoint ZeroDcPoint { get; set; }
        Double spiralX { get; set; }
        Double spiralY { get; set; }
    }
}
