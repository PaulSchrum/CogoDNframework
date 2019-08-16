/* 
 * This file is composed by Paul Schrum, but the struct testVertex 
 * originates from MIConvexHull, and that requires the following notice:
 * 
 *     This . . . class is part of the MIConvexHull Library Project. 
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     MIConvexHull is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     MIConvexHull is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT License for more details.
 *  
 *     You should have received a copy of the MIT License
 *     along with MIConvexHull.
 *     
 *     Please find further details and contact information on GraphSynth
 *     at https://designengrlab.github.io/MIConvexHull/
 *************************************************************************/


using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ptsDigitalTerrainModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace TinTests
{

    public struct TestVertex : IVertex
    {
        public double[] Position { get; set; }

        public TestVertex(double x, double y, double z)
        {
            Position = new double[] { x, y, z };
        }

        public override string ToString()
        {
            var str = $"X: {Position[0]:f2}  Y: {Position[1]:f2}  Z: {Position[2]:f2}";
            return str;
        }
    }

    [TestClass]
    public class TestMIConvexHull
    {
        public TestMIConvexHull()
        { }

        private List<TestVertex> Vertices { get; set; } = null;

        private void Initialize()
        {
            if(Vertices == null)
            {
                this.Vertices = new List<TestVertex>();

                // This section adapted from MIConvexHull/Examples/3DConvexHullWPF
                var r = new Random(1);
                for(int i = 0; i < 10; i++)
                {
                    var v = new TestVertex(
                        100.0 * r.NextDouble(),
                        100.0 * r.NextDouble(),
                        5.0 * r.NextDouble());
                    this.Vertices.Add(v);
                }
            }
        }

        [TestMethod]
        public void verify_ListIsNotNull()
        {
            this.Initialize();
            Assert.IsNotNull(this.Vertices);
        }
    }
}


