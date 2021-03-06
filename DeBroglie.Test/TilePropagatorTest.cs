﻿using DeBroglie.Models;
using DeBroglie.Topo;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeBroglie.Test
{
    [TestFixture]
    public class TilePropagatorTest
    {
        [Test]
        public void TestToTopArray()
        {
            var a = new int[,]{
                { 1, 0 },
                { 0, 1 },
            };
            var model = OverlappingModel.Create(a, 2, false, 8);

            var propagator = new TilePropagator(model, new Topology(4, 4, false));

            propagator.Select(0, 0, 0, new Tile(1));
            var status = propagator.Run();

            Assert.AreEqual(Resolution.Decided, status);

            var result = propagator.ToValueArray<int>().ToArray2d();

            Assert.AreEqual(4, result.GetLength(0));
            Assert.AreEqual(4, result.GetLength(1));

            Assert.AreEqual(1, result[0, 0]);
            Assert.AreEqual(1, result[3, 3]);
        }

        [Test]
        public void TestMask()
        {
            var a = new int[,]{
                { 1, 0 },
                { 0, 1 },
            };
            var model = new AdjacentModel();
            model.AddSample(TopoArray.Create(a, true).ToTiles());

            var mask = new bool[5 * 5];
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 5; y++)
                {
                    if (x == 2 || y == 2)
                    {
                        mask[x + y * 5] = false;
                    }
                    else
                    {
                        mask[x + y * 5] = true;
                    }
                }
            }
            var topology = new Topology(5, 5, true).WithMask(mask);

            var propagator = new TilePropagator(model, topology);

            propagator.Run();

            Assert.AreEqual(Resolution.Decided, propagator.Status);
        }

        [Test]
        public void TestMaskWithOverlapping()
        {

            var a = new int[,]{
                { 1, 0 },
                { 0, 1 },
            };
            var model = OverlappingModel.Create(a, 2, false, 8);

            var mask = new bool[4 * 5];
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (x == 2 || x == 3)
                    {
                        mask[x + y * 5] = false;
                    }
                    else
                    {
                        mask[x + y * 5] = true;
                    }
                }
            }
            var topology = new Topology(5, 4, false).WithMask(mask);

            var propagator = new TilePropagator(model, topology);

            propagator.Select(0, 0, 0, new Tile(1));
            propagator.Select(4, 0, 0, new Tile(0));

            propagator.Run();

            Assert.AreEqual(Resolution.Decided, propagator.Status);
        }

        // This test illustrates a problem with how masks interact with the overlapping model.
        // The two select calls are not possible to fulfill with one pattern across the entire
        // output.
        // But cut the output region in two using a mask, and the overlap rectangle is 2x2 and so
        // not wide enough to cause interactions across the divide. So this should give Resolution.Decided,
        // Filling each half of the output with a chess pattern.
        //
        // But at the moment, it gives contradiction. The implementation doesn't handle masks properly,
        // and errs on the side of caution, basically ignoring the mask entirely.
        //
        // I hope to resolve this with https://github.com/BorisTheBrave/DeBroglie/issues/7
        [Test]
        [Ignore("Overlapping masks don't work ideally at the moment")]
        public void TestMaskWithThinOverlapping()
        {

            var a = new int[,]{
                { 1, 0 },
                { 0, 1 },
            };
            var model = OverlappingModel.Create(a, 2, false, 8);

            var mask = new bool[4 * 5];
            for (var x = 0; x < 5; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (x == 2)
                    {
                        mask[x + y * 5] = false;
                    }
                    else
                    {
                        mask[x + y * 5] = true;
                    }
                }
            }
            var topology = new Topology(5, 4, false).WithMask(mask);

            var propagator = new TilePropagator(model, topology);

            propagator.Select(0, 0, 0, new Tile(1));
            propagator.Select(4, 0, 0, new Tile(0));

            propagator.Run();

            Assert.AreEqual(Resolution.Decided, propagator.Status);
        }

    }
}
