using System;
using System.Drawing;
using NUnit.Framework;

using CSammisRun.Imaging.Morphology;

namespace CSammisRun.Imaging.Test
{
    /// <summary>
    /// Tests on the OneBppImage object.
    /// </summary>
    [TestFixture]
    public class OneBppImageTest
    {
        private const int TEST_IMAGE_DIMENSION = 16;

        // Erosion test from http://homepages.inf.ed.ac.uk/rbf/HIPR2/erode.htm
        private byte[,] testImageErosionSource = new byte[TEST_IMAGE_DIMENSION, TEST_IMAGE_DIMENSION] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,0,0,0,0,1,1,1,0,0},
            {0,0,1,1,1,1,0,0,0,0,1,1,1,1,0,0},
            {0,0,0,1,1,0,0,0,0,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0},
            {0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
            {0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0},
            {0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0},
            {0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0},
            {0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0},
            {0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};

        private byte[,] testImageErosionResult = new byte[TEST_IMAGE_DIMENSION, TEST_IMAGE_DIMENSION] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};

        [TestFixtureSetUp]
        public void SetUp()
        {
            // Fix up the test arrays with the right values.
            // 0s are a lot prettier to look at than 255s!
            for (int y = 0; y < TEST_IMAGE_DIMENSION; y++)
            {
                for (int x = 0; x < TEST_IMAGE_DIMENSION; x++)
                {
                    testImageErosionSource[x,y] = (testImageErosionSource[x,y] == 0) ? Constants.PIXEL_VALUE_WHITESPACE : Constants.PIXEL_VALUE_INK;
                    testImageErosionResult[x,y] = (testImageErosionResult[x,y] == 0) ? Constants.PIXEL_VALUE_WHITESPACE : Constants.PIXEL_VALUE_INK;
                }
            }
        }

        [Test]
        public void TestInitFromByteData()
        {
            // Original code had a bug in it which reduced the dimensions of the image
            // each time it was constructed (off-by-one with the multidimensional byte access)
            OneBppImage testImage = new OneBppImage(testImageErosionSource);

            Assert.AreEqual(TEST_IMAGE_DIMENSION, testImage.Width);
            Assert.AreEqual(TEST_IMAGE_DIMENSION, testImage.Height);
        }

        [Test]
        public void TestErosion()
        {
            StructuralElement element = new StructuralElement(
                new byte[3,3] { {0,0,0}, {0,0,0}, {0,0,0} }, new Point(1,1));

            OneBppImage testImage = new OneBppImage(testImageErosionSource).Erode(element);

            for (int y = 0; y < TEST_IMAGE_DIMENSION; y++)
            {
                for (int x = 0; x < TEST_IMAGE_DIMENSION; x++)
                {
                    byte expected = testImageErosionResult[x,y];
                    byte actual = testImage.ImageData[x,y];

                    Assert.AreEqual(expected, actual, "Pixel values at ({0},{1}) are unexpectedly different", x, y);
                }
            }
        }
    }
}

