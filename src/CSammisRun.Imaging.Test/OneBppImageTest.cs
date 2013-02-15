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
        private const int TEST_IMAGE_DIMENSION = 10;

        byte[,] testImageSource = new byte[TEST_IMAGE_DIMENSION,TEST_IMAGE_DIMENSION] {
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

        [Test]
        public void TestInitFromByteData()
        {
            // Original code had a bug in it which reduced the dimensions of the image
            // each time it was constructed (off-by-one with the multidimensional byte access)
            OneBppImage testImage = new OneBppImage(testImageSource);

            Assert.AreEqual(TEST_IMAGE_DIMENSION, testImage.Width);
            Assert.AreEqual(TEST_IMAGE_DIMENSION, testImage.Height);

            for (int x = 0; x < testImage.Width; x++)
            {
                for (int y = 0; y < testImage.Height; y++)
                {
                    Assert.AreEqual(testImageSource[x,y], testImage.ImageData[x,y],
                                    "Pixels are not identical at ({0},{1})", x, y);
                }
            }
        }

        [Test]
        public void TestCopyConstructor()
        {
            OneBppImage testImage = new OneBppImage(testImageSource);
            OneBppImage copyImage = new OneBppImage(testImage);

            Assert.AreEqual(testImage.Width, copyImage.Width, "Widths are not identical.");
            Assert.AreEqual(testImage.Height, copyImage.Height, "Heights are not identical.");

            Assert.AreNotSame(testImage.ImageData, copyImage.ImageData, "ImageData for copied image should be different than the original.");
            for (int x = 0; x < testImage.Width; x++)
            {
                for (int y = 0; y < testImage.Height; y++)
                {
                    Assert.AreEqual(testImage.ImageData[x,y], copyImage.ImageData[x,y],
                                    "Pixels are not identical at ({0},{1})", x, y);
                }
            }
        }
    }
}

