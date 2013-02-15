using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace CSammisRun.Imaging.Test
{
    [TestFixture]
    public class BinarizerTest
    {
        [Test]
        public void TestBinarizeFromBWBitmap()
        {
            Stream bitmapStream = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream("CSammisRun.Imaging.Test.Resources.10x20checkerboard.bmp");
            Assert.IsNotNull(bitmapStream);
            
            Bitmap resourceBitmap = new Bitmap(bitmapStream); 
            Assert.IsNotNull(resourceBitmap, "Bitmap could not be constructed from resource.");
            Assert.AreEqual(PixelFormat.Format32bppRgb, resourceBitmap.PixelFormat, "Resource bitmap has unexpected pixel format.");

            Binarizer binarizer = new Binarizer();
            Bitmap testBitmap = binarizer.ProcessImage(resourceBitmap);
            Assert.AreEqual(PixelFormat.Format1bppIndexed, testBitmap.PixelFormat, "Binarizer output is an unexpected pixel format.");

            Assert.AreEqual(10, testBitmap.Width, "Unexpected image width from binarizer output.");
            Assert.AreEqual(20, testBitmap.Height, "Unexpected image height from binarizer output.");
            
            for (int x = 0; x < testBitmap.Width; x++)
            {
                for (int y = 0; y < testBitmap.Height; y++)
                {
                    byte expectedValue = Constants.PIXEL_VALUE_WHITESPACE;
                    if ((x % 2 == 0) && (y % 2 == 1))
                    {
                        expectedValue = Constants.PIXEL_VALUE_INK;
                    }
                    else if ((x % 2 == 1) && (y % 2 == 0))
                    {
                        expectedValue = Constants.PIXEL_VALUE_INK;
                    }

                    Color actualPixel = testBitmap.GetPixel(x, y);
                    Assert.IsTrue(actualPixel.R == actualPixel.G && actualPixel.G == actualPixel.B, "Binarized pixel values do not match");

                    byte actualValue = actualPixel.R;
                    
                    Assert.AreEqual(expectedValue, actualValue, "Pixel values are unexpectedly different at ({0},{1})", x, y);
                }
            }
        }

        [Test]
        public void TestBinarizeFromBWPng()
        {
            Stream bitmapStream = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream("CSammisRun.Imaging.Test.Resources.10x20checkerboard.png");
            Assert.IsNotNull(bitmapStream);
            
            Bitmap resourceBitmap = new Bitmap(bitmapStream); 
            Assert.IsNotNull(resourceBitmap, "Bitmap could not be constructed from resource.");
            Assert.AreEqual(PixelFormat.Format32bppArgb, resourceBitmap.PixelFormat, "Resource bitmap has unexpected pixel format.");
            
            Binarizer binarizer = new Binarizer();
            Bitmap testBitmap = binarizer.ProcessImage(resourceBitmap);
            Assert.AreEqual(PixelFormat.Format1bppIndexed, testBitmap.PixelFormat, "Binarizer output is an unexpected pixel format.");
            
            Assert.AreEqual(10, testBitmap.Width, "Unexpected image width from binarizer output.");
            Assert.AreEqual(20, testBitmap.Height, "Unexpected image height from binarizer output.");
            
            for (int x = 0; x < testBitmap.Width; x++)
            {
                for (int y = 0; y < testBitmap.Height; y++)
                {
                    byte expectedValue = Constants.PIXEL_VALUE_WHITESPACE;
                    if ((x % 2 == 0) && (y % 2 == 1))
                    {
                        expectedValue = Constants.PIXEL_VALUE_INK;
                    }
                    else if ((x % 2 == 1) && (y % 2 == 0))
                    {
                        expectedValue = Constants.PIXEL_VALUE_INK;
                    }
                    
                    Color actualPixel = testBitmap.GetPixel(x, y);
                    Assert.IsTrue(actualPixel.R == actualPixel.G && actualPixel.G == actualPixel.B, "Binarized pixel values do not match");
                    
                    byte actualValue = actualPixel.R;
                    
                    Assert.AreEqual(expectedValue, actualValue, "Pixel values are unexpectedly different at ({0},{1})", x, y);
                }
            }
        }
    }
}

