﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeBroglie.Test
{
    [TestFixture]
    class TopArrayUtilsTest
    {
        [Test]
        public void TestRotate()
        {
            var a = new int[2, 2];
            a[0, 0] = 1;
            a[1, 0] = 2;
            a[0, 1] = 3;
            a[1, 1] = 4;
            var ta = TopArray.Create(a, new Topology(Directions.Cartesian2d, 2, 2, false));

            var r1 = TopArrayUtils.Rotate(ta, 1);
            Assert.AreEqual(2, r1.Get(0, 0));
            Assert.AreEqual(1, r1.Get(0, 1));
            Assert.AreEqual(4, r1.Get(1, 0));
            Assert.AreEqual(3, r1.Get(1, 1));

            var r3 = TopArrayUtils.Rotate(ta, 3);
            Assert.AreEqual(3, r3.Get(0, 0));
            Assert.AreEqual(4, r3.Get(0, 1));
            Assert.AreEqual(1, r3.Get(1, 0));
            Assert.AreEqual(2, r3.Get(1, 1));

            var refl = TopArrayUtils.Rotate(ta, 0, true);
            Assert.AreEqual(2, refl.Get(0, 0));
            Assert.AreEqual(4, refl.Get(0, 1));
            Assert.AreEqual(1, refl.Get(1, 0));
            Assert.AreEqual(3, refl.Get(1, 1));
        }

        [Test]
        public void TestHexRotate()
        {
            var a = new int[2, 2];
            a[0, 0] = 1;
            a[1, 0] = 2;
            a[0, 1] = 3;
            a[1, 1] = 4;
            var ta = TopArray.Create(a, new Topology(Directions.Hexagonal2d, 2, 2, false));

            var r1 = TopArrayUtils.HexRotate(ta, 13, false);
            Assert.AreEqual(2, r1.Get(0, 0));
            Assert.AreEqual(1, r1.Get(0, 1));
            Assert.AreEqual(4, r1.Get(1, 1));
            Assert.AreEqual(3, r1.Get(1, 2));

            var r3 = TopArrayUtils.HexRotate(ta, 3, false);
            Assert.AreEqual(4, r3.Get(0, 0));
            Assert.AreEqual(3, r3.Get(1, 0));
            Assert.AreEqual(2, r3.Get(0, 1));
            Assert.AreEqual(1, r3.Get(1, 1));

            var refl = TopArrayUtils.HexRotate(ta, 0, true);
            Assert.AreEqual(2, refl.Get(0, 0));
            Assert.AreEqual(1, refl.Get(1, 0));
            Assert.AreEqual(4, refl.Get(1, 1));
            Assert.AreEqual(3, refl.Get(2, 1));
        }
    }
}
