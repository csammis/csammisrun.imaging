using System.Drawing;
using NUnit.Framework;

using CSammisRun.Imaging;
using CSammisRun.Imaging.Morphology;

namespace CSammisRun.Imaging.Test
{
    [TestFixture()]
    public class MorphoOperationsTest
    {
        private const int TEST_IMAGE_DIMENSION = 16;
        
        private byte[,] testImageSource = new byte[TEST_IMAGE_DIMENSION, TEST_IMAGE_DIMENSION] {
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
        
        private byte[,] testImageClosingSource = new byte[TEST_IMAGE_DIMENSION, TEST_IMAGE_DIMENSION] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,0,0,0,0,0,0,1,1,0,0,0,0},
            {0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,1,1,0,0,0,0,1,0},
            {0,0,0,0,0,0,0,0,1,0,0,0,1,1,1,0},
            {0,0,0,1,1,1,1,0,1,0,0,1,1,1,0,0},
            {0,0,1,1,1,1,0,1,1,1,1,1,1,0,0,0},
            {0,1,1,1,1,0,0,0,1,0,0,0,0,0,0,0},
            {0,1,0,0,0,1,0,0,0,1,0,0,0,0,0,0},
            {0,1,0,0,0,0,1,0,1,1,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,1,1,1,0,0,0,0,0,0},
            {0,1,1,0,0,0,0,1,1,1,0,0,0,0,0,0},
            {0,1,1,1,0,0,0,1,1,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
        
        // Erosion test from http://homepages.inf.ed.ac.uk/rbf/HIPR2/erode.htm
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
        
        // Dilation test from http://homepages.inf.ed.ac.uk/rbf/HIPR2/dilate.htm
        private byte[,] testImageDilationResult = new byte[TEST_IMAGE_DIMENSION, TEST_IMAGE_DIMENSION] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0},
            {0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0},
            {0,1,1,1,1,1,1,1,0,0,1,1,1,1,1,0},
            {0,1,1,1,1,1,1,1,0,1,1,1,1,1,1,0},
            {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,0,0,0,0,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0}};
        
        // Opening test from http://homepages.inf.ed.ac.uk/rbf/HIPR2/open.htm
        private byte[,] testImageOpeningResult = new byte[TEST_IMAGE_DIMENSION, TEST_IMAGE_DIMENSION] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,0,0,0,0,0,1,1,1,0,0},
            {0,0,1,1,1,1,0,0,0,0,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0},
            {0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0},
            {0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
            {0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0},
            {0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0},
            {0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0},
            {0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};
        
        // Closing test from http://homepages.inf.ed.ac.uk/rbf/HIPR2/close.htm
        private byte[,] testImageClosingResult = new byte[TEST_IMAGE_DIMENSION, TEST_IMAGE_DIMENSION] {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,1,1,0,0,0,0,0,0,1,1,0,0,0,0},
            {0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,1,1,1,0,0,0,1,0},
            {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0},
            {0,0,0,1,1,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
            {0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
            {0,1,0,0,0,1,1,1,1,1,0,0,0,0,0,0},
            {0,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0},
            {0,1,0,0,0,0,0,1,1,1,0,0,0,0,0,0},
            {0,1,1,0,0,0,0,1,1,1,0,0,0,0,0,0},
            {0,1,1,1,0,0,0,1,1,0,0,0,0,0,0,0},
            {0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}};

        private Operation morphoOperation;
        
        [TestFixtureSetUp]
        public void SetUp()
        {
            StructuralElement element = new StructuralElement(
                new byte[3,3] { {0,0,0}, {0,0,0}, {0,0,0} }, new Point(1,1));
            morphoOperation = new Operation(element);

            // Fix up the test arrays with the right values.
            // 0s are a lot prettier to look at than 255s!
            for (int y = 0; y < TEST_IMAGE_DIMENSION; y++)
            {
                for (int x = 0; x < TEST_IMAGE_DIMENSION; x++)
                {
                    testImageSource[x,y] = (testImageSource[x,y] == 0) ? Constants.PIXEL_VALUE_WHITESPACE : Constants.PIXEL_VALUE_INK;
                    testImageErosionResult[x,y] = (testImageErosionResult[x,y] == 0) ? Constants.PIXEL_VALUE_WHITESPACE : Constants.PIXEL_VALUE_INK;
                    testImageDilationResult[x,y] = (testImageDilationResult[x,y] == 0) ? Constants.PIXEL_VALUE_WHITESPACE : Constants.PIXEL_VALUE_INK;
                    testImageOpeningResult[x,y] = (testImageOpeningResult[x,y] == 0) ? Constants.PIXEL_VALUE_WHITESPACE : Constants.PIXEL_VALUE_INK;
                    testImageClosingSource[x,y] = (testImageClosingSource[x,y] == 0) ? Constants.PIXEL_VALUE_WHITESPACE : Constants.PIXEL_VALUE_INK;
                    testImageClosingResult[x,y] = (testImageClosingResult[x,y] == 0) ? Constants.PIXEL_VALUE_WHITESPACE : Constants.PIXEL_VALUE_INK;
                }
            }
        }

        [Test]
        public void TestErosion()
        {
            OneBppImage testImage = morphoOperation.Erode(new OneBppImage(testImageSource));
            
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
        
        [Test]
        public void TestDilation()
        {
            OneBppImage testImage = morphoOperation.Dilate(new OneBppImage(testImageSource));
            
            for (int y = 0; y < TEST_IMAGE_DIMENSION; y++)
            {
                for (int x = 0; x < TEST_IMAGE_DIMENSION; x++)
                {
                    byte expected = testImageDilationResult[x,y];
                    byte actual = testImage.ImageData[x,y];
                    
                    Assert.AreEqual(expected, actual, "Pixel values at ({0},{1}) are unexpectedly different", x, y);
                }
            }
        }
        
        [Test]
        public void TestOpening()
        {
            OneBppImage testImage = morphoOperation.Open(new OneBppImage(testImageSource));
            
            for (int y = 0; y < TEST_IMAGE_DIMENSION; y++)
            {
                for (int x = 0; x < TEST_IMAGE_DIMENSION; x++)
                {
                    byte expected = testImageOpeningResult[x,y];
                    byte actual = testImage.ImageData[x,y];
                    
                    Assert.AreEqual(expected, actual, "Pixel values at ({0},{1}) are unexpectedly different", x, y);
                }
            }
        }
        
        [Test]
        public void TestSecondOpeningHasNoEffect()
        {
            OneBppImage testImage1 = morphoOperation.Open(new OneBppImage(testImageSource));
            OneBppImage testImage2 = morphoOperation.Open(testImage1);
            
            for (int y = 0; y < TEST_IMAGE_DIMENSION; y++)
            {
                for (int x = 0; x < TEST_IMAGE_DIMENSION; x++)
                {
                    byte expected = testImage1.ImageData[x,y];
                    byte actual = testImage2.ImageData[x,y];
                    
                    Assert.AreEqual(expected, actual, "Pixel values at ({0},{1}) are unexpectedly different", x, y);
                }
            }
        }
        
        [Test]
        public void TestClosing()
        {
            OneBppImage testImage = morphoOperation.Close(new OneBppImage(testImageClosingSource));
            
            for (int y = 0; y < TEST_IMAGE_DIMENSION; y++)
            {
                for (int x = 0; x < TEST_IMAGE_DIMENSION; x++)
                {
                    byte expected = testImageClosingResult[x,y];
                    byte actual = testImage.ImageData[x,y];
                    
                    Assert.AreEqual(expected, actual, "Pixel values at ({0},{1}) are unexpectedly different", x, y);
                }
            }
        }
        
        [Test]
        public void TestSecondClosingHasNoEffect()
        {
            OneBppImage testImage1 = morphoOperation.Close(new OneBppImage(testImageClosingSource));
            OneBppImage testImage2 = morphoOperation.Close(testImage1);
            
            for (int y = 0; y < TEST_IMAGE_DIMENSION; y++)
            {
                for (int x = 0; x < TEST_IMAGE_DIMENSION; x++)
                {
                    byte expected = testImage1.ImageData[x,y];
                    byte actual = testImage2.ImageData[x,y];
                    
                    Assert.AreEqual(expected, actual, "Pixel values at ({0},{1}) are unexpectedly different", x, y);
                }
         
            }
        }
    }
}

