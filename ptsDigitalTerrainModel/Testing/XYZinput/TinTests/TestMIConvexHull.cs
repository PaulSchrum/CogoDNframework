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
using MIConvexHull;
using System.Linq;
using System.Text;
using System.Windows;

namespace TinTests
{
    public class Vertex : IVertex
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        public Vertex(double x, double y)
        {
            Position = new double[] { x, y };
        }

        public double X { get { return Position[0]; } }
        public double Y { get { return Position[1]; } }

        /// <summary>
        /// Gets or sets the Z. Not used by MIConvexHull2D.
        /// </summary>
        /// <value>The Z position.</value>
        public double Z { get; set; } = 0d;

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        /// <value>The coordinates.</value>
        public double[] Position { get; set; }

        public override string ToString()
        {
            string str = $"xy={Position[0]:f2},{Position[1]:f2}"; //  Z: {Position[2]:f1}";
            return str;
        }
    }

    public class Cell : TriangulationCell<Vertex, Cell>
    {
        double Det(double[,] m)
        {
            return m[0, 0] * ((m[1, 1] * m[2, 2]) - (m[2, 1] * m[1, 2])) - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);
        }

        double LengthSquared(double[] v)
        {
            double norm = 0;
            for(int i = 0; i < v.Length; i++)
            {
                double t = v[i];
                norm += t * t;
            }
            return norm;
        }


        public Cell()
        {            
        }

        public override string ToString()
        {
            try
            {
                string str = $"{this.Vertices[0]};{this.Vertices[1]};{this.Vertices[2]};{this.Vertices[0]}";
                return str;
            }
            catch
            {
                string str = $"{this.Vertices[0]};{this.Vertices[1]};{this.Vertices[0]}";
                return str;
            }
        }
    }

    [TestClass]
    public class TestMIConvexHull
    {
        public TestMIConvexHull()
        { }

        private List<Vertex> Vertices { get; set; } = null;
        private VoronoiMesh<Vertex, Cell, VoronoiEdge<Vertex, Cell>>
            VoronoiMesh { get; set; } = null;
        private IEnumerable<VoronoiEdge<Vertex, Cell>> Lines { get; set; } = null;
        private IEnumerable<Cell> Triangles { get; set; } = null;

        private void Initialize()
        {
            if(this.Vertices == null)
            {
                this.Vertices = new List<Vertex>();

                // This section adapted from MIConvexHull/Examples/3DConvexHullWPF
                Random r = new Random(4);  // Keep 4 to be synched with the WPF app
                for(int i = 0; i < 6; i++)
                {
                    Vertex v = new Vertex(
                        538.0 * r.NextDouble(),
                        495.04 * r.NextDouble());
                    v.Z = 5.0 * r.NextDouble();
                    this.Vertices.Add(v);
                }
            }

            if(this.VoronoiMesh == null)
            {
                this.VoronoiMesh = 
                    MIConvexHull.VoronoiMesh.Create<Vertex, Cell>(this.Vertices);
                this.Lines = this.VoronoiMesh.Edges;
                this.Triangles = this.VoronoiMesh.Triangles;
            }
        }

        [TestMethod]
        public void Verify_ListsNotNull_AndHaveRightCount()
        {
            this.Initialize();
            Assert.IsNotNull(this.Vertices);
            Assert.IsNotNull(this.Triangles);
            Assert.AreEqual(this.Vertices.Count, 6);
            Assert.AreEqual(this.Triangles.Count(), 6);
        }
    }
}


