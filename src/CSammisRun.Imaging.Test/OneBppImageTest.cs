using System;
using System.Drawing;
using NUnit.Framework;

using CSammisRun.Imaging;
using CSammisRun.Imaging.Morphology;

namespace CSammisRun.Imaging.Test
{
    /// <summary>
    /// Tests on the OneBppImage object.
    /// </summary>
    [TestFixture]
    public class OneBppImageTest
    {
        [Test]
        public void TestInitFromByteData()
        {
            byte[,] testImageSource = new byte[10,10] {
                {0,0,1,1,1,1,1,1,0,0},
                {0,1,0,0,0,0,0,0,0,1},
                {1,0,0,1,0,0,1,0,0,1},
                {1,0,0,1,0,0,1,0,0,1},
                {1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,0,1,0,1},
                {1,0,0,1,1,1,1,0,0,1},
                {0,1,0,0,0,0,0,0,1,0},
                {0,0,1,1,1,1,1,1,0,0}};

            // Original code had a bug in it which reduced the dimensions of the image
            // each time it was constructed (off-by-one with the multidimensional byte access)
            OneBppImage testImage = new OneBppImage(testImageSource);

            Assert.AreEqual(10, testImage.Width);
            Assert.AreEqual(10, testImage.Height);
        }
    }
}

