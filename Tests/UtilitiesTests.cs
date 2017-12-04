using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo;

namespace Tests
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void TestInterpolation()
        {
            double x1 = 10.0, y1 = 5.0, x2 = 20.0, y2 = 30.0;
            double expectY = 17.5;
            double actualY = utilFunctions.interpolateGetY(x1, y1, x2, y2, X: 15.0);

            Assert.AreEqual(
                expected: expectY,
                actual: actualY,
                delta: 0.00001
                );

            double expectX = 14.4;
            double actualX = utilFunctions.interpolateGetX(x1, y1, x2, y2, Y: 16.0);

            Assert.AreEqual(
                expected: expectX,
                actual: actualX,
                delta: 0.00001
                );

        }

        [TestMethod]
        public void FindRoot_x2_equals0()
        {
            Func<double, double>
                testFunc = x => x * x;

            double yBeingSought = 25.0;
            double actualX = utilFunctions.findRoot(1.0, 5.22, testFunc, yBeingSought);

            Assert.AreEqual(
                expected: 5.0,
                actual: actualX,
                delta: 0.0001
                );

            //yBeingSought = 0.0;
            //actualX = utilFunctions.findRoot(-1.0, 5.22, testFunc, yBeingSought);

            //Assert.AreEqual(
            //    expected: 0.0,
            //    actual: actualX,
            //    delta: 0.01
            //    );

        }
    }
}
