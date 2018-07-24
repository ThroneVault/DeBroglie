﻿using DeBroglie.Constraints;
using NUnit.Framework;

namespace DeBroglie.Test
{
    [TestFixture]
    public class BorderConstraintTest
    {
        [Test]
        public void TestBorderConstraint()
        {
            var a = new int[,]{
                 {1, 0, 0},
                 {0, 1, 1},
                 {0, 1, 1},
            };

            var model = AdjacentModel.Create(a, true);
            var propagator = new TilePropagator(model, new Topology(10, 10, false), true, constraints: new[] {
                new BorderConstraint{
                    Tile = new Tile(0),
                }
            });
            var status = propagator.Run();
            Assert.AreEqual(CellStatus.Decided, status);
            var result = propagator.ToValueArray<int>().ToArray2d();
            Assert.AreEqual(0, result[0, 0]);
            Assert.AreEqual(0, result[9, 0]);
            Assert.AreEqual(0, result[0, 9]);
            Assert.AreEqual(0, result[9, 9]);

        }
    }
}
