using System.Drawing;
using NUnit.Framework;

using CSammisRun.Imaging.Morphology;

namespace CSammisRun.Imaging.Test
{
    [TestFixture]
    public class StructuralElementTest
    {
        [Test]
        public void TestDimensions()
        {
            StructuralElement test = new StructuralElement(new byte[3,3]
                                                           { {0,0,0}, {0,0,0}, {0,0,0} }, new Point(1,1));
            Assert.AreEqual(3, test.Height, "Height is not correct.");
            Assert.AreEqual(3, test.Width, "Width is not correct.");
        }

        [Test]
        public void TestAllPointsOn()
        {
            StructuralElement test = new StructuralElement(new byte[3,3]
                                                           { {0,0,0}, {0,0,0}, {0,0,0} }, new Point(1,1));
            Assert.AreEqual(9, test.ExaminePoints.Count, "Unexpected number of points in the examination list.");
        }

        [Test]
        public void TestAllPointsOff()
        {
            StructuralElement test = new StructuralElement(new byte[3,3]
                                                           { {255,255,255}, {255,255,255}, {255,255,255} }, new Point(1,1));
            Assert.AreEqual(0, test.ExaminePoints.Count, "Unexpected number of points in the examination list.");
        }

        [Test]
        public void TestSomePointsOn()
        {
            StructuralElement test = new StructuralElement(new byte[3,3]
                                                           { {255,0,255}, {0,255,0}, {255,0,255} }, new Point(1,1));
            Assert.AreEqual(4, test.ExaminePoints.Count, "Unexpected number of points in the examination list.");
        }

        [Test]
        public void TestCenterOrigin()
        {
            StructuralElement test = new StructuralElement(new byte[3,3]
                                                          { {0,0,0}, {0,0,0}, {0,0,0} }, new Point(1,1));

            Assert.Contains(new Point(-1,-1), test.ExaminePoints);
            Assert.Contains(new Point(-1, 0), test.ExaminePoints);
            Assert.Contains(new Point(-1, 1), test.ExaminePoints);
            Assert.Contains(new Point( 0,-1), test.ExaminePoints);
            Assert.Contains(new Point( 0, 0), test.ExaminePoints);
            Assert.Contains(new Point( 0, 1), test.ExaminePoints);
            Assert.Contains(new Point( 1,-1), test.ExaminePoints);
            Assert.Contains(new Point( 1,0), test.ExaminePoints);
            Assert.Contains(new Point( 1, 1), test.ExaminePoints);
        }

        [Test]
        public void TestTopLeftOrigin()
        {
            StructuralElement test = new StructuralElement(new byte[3,3]
                                                           { {0,0,0}, {0,0,0}, {0,0,0} }, new Point(0,0));
            
            Assert.Contains(new Point(0,0), test.ExaminePoints);
            Assert.Contains(new Point(0,1), test.ExaminePoints);
            Assert.Contains(new Point(0,2), test.ExaminePoints);
            Assert.Contains(new Point(1,0), test.ExaminePoints);
            Assert.Contains(new Point(1,1), test.ExaminePoints);
            Assert.Contains(new Point(1,2), test.ExaminePoints);
            Assert.Contains(new Point(2,0), test.ExaminePoints);
            Assert.Contains(new Point(2,1), test.ExaminePoints);
            Assert.Contains(new Point(2,2), test.ExaminePoints);
        }

        [Test]
        public void TestOriginOutOfBounds()
        {
            Assert.DoesNotThrow(delegate () {
                new StructuralElement(new byte[3,3] { {0,0,0}, {0,0,0}, {0,0,0} }, new Point(5,5));
            }, "Unexpected throw from StructuralElement constructor.");
        }
    }
}

