using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CSammisRun.Imaging.Morphology
{
    /// <summary>
    /// A base class which represents a 1bpp TIFF image
    /// </summary>
    public class OneBppImage
    {
        /// <summary>
        /// Initializes a new 1bpp image from a TIFF file
        /// </summary>
        public OneBppImage(string fileName)
        {
            Read1BppImageData(fileName);
        }

        /// <summary>
        /// Copies the specified <see cref="OneBppImage"/> into the new instance
        /// </summary>
        public OneBppImage(OneBppImage image)
        {
            this.Height = image.Height;
            this.Width = image.Width;

            this.ImageData = new byte[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    this.ImageData[x, y] = image.ImageData[x, y];
        }

        /// <summary>
        /// Initializes a new 1bpp image from an array in memory
        /// </summary>
        public OneBppImage(byte[,] imageData)
        {
            this.Width = imageData.GetLength(0);
            this.Height = imageData.GetLength(1);
            this.ImageData = imageData;
        }

        /// <summary>
        /// Gets the image data as an array of bytes which are either <see cref="PIXEL_VALUE_INK"/>
        /// or <see cref="PIXEL_VALUE_WHITESPACE"/>
        /// </summary>
        public byte[,] ImageData
        {
            get; private set;
        }

        /// <summary>
        /// Gets the width of the image
        /// </summary>
        public int Width
        {
            get; private set;
        }

        /// <summary>
        /// Gets the height of the image
        /// </summary>
        public int Height
        {
            get; private set;
        }

        #region Public methods
        /// <summary>
        /// Dilate the image with the specified <see cref="StructuralElement"/>
        /// </summary>
        public OneBppImage Dilate(StructuralElement element)
        {
            byte[,] data = new byte[this.Width, this.Height];

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    byte pointValue = Constants.PIXEL_VALUE_WHITESPACE;
                    foreach (Point p in element.ExaminePoints)
                    {
                        int testX = x + p.X;
                        int testY = y + p.Y;
                        if (testX < 0 || testY < 0 || testX >= this.Width || testY >= this.Height)
                        {
                            continue;
                        }

                        if (this.ImageData[testX, testY] == Constants.PIXEL_VALUE_INK)
                        {
                            pointValue = Constants.PIXEL_VALUE_INK;
                            break;
                        }
                    }

                    data[x, y] = pointValue;
                }
            }

            return new OneBppImage(data);
        }

        /// <summary>
        /// Erode the image with the specified <see cref="StructuralElement"/>
        /// </summary>
        public OneBppImage Erode(StructuralElement element)
        {
            byte[,] data = new byte[this.Width, this.Height];

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (this.ImageData[x, y] == Constants.PIXEL_VALUE_INK)
                    {
                        foreach (Point p in element.ExaminePoints)
                        {
                            int testX = x + p.X;
                            int testY = y + p.Y;
                            if (testX < 0 || testY < 0 || testX >= this.Width || testY >= this.Height)
                            {
                                data[x, y] = Constants.PIXEL_VALUE_WHITESPACE;
                                goto NextPixel;
                            }

                            if (this.ImageData[testX, testY] != Constants.PIXEL_VALUE_INK)
                            {
                                data[x, y] = Constants.PIXEL_VALUE_WHITESPACE;
                                goto NextPixel;
                            }
                        }

                        data[x, y] = Constants.PIXEL_VALUE_INK;
                    }
                    else
                    {
                        data[x, y] = Constants.PIXEL_VALUE_WHITESPACE;
                    }

                NextPixel: ; // continue
                }
            }

            return new OneBppImage(data);
        }

        /// <summary>
        /// Perform a morphological closing given the specified <see cref="StructuralElement"/>
        /// </summary>
        public OneBppImage Close(StructuralElement element)
        {
            return Dilate(element).Erode(element);
        }

        /// <summary>
        /// Perform a morphological opening given the specified <see cref="StructuralElement"/>
        /// </summary>
        public OneBppImage Open(StructuralElement element)
        {
            return Erode(element).Dilate(element);
        }
        #endregion

        #region Image read/write methods
        /// <summary>
        /// Reads pixel data from a 1bpp image on disk
        /// </summary>
        private void Read1BppImageData(string fileName)
        {
            using (Bitmap pageImage = new Bitmap(fileName))
            {
                if (pageImage.PixelFormat != PixelFormat.Format1bppIndexed)
                {
                    throw new ArgumentException("fileName");
                }

                this.Height = pageImage.Height;
                this.Width = pageImage.Width;

                this.ImageData = new byte[this.Width, this.Height];

                // Extract the pixel data from the bitmap
                BitmapData data = pageImage.LockBits(new Rectangle(0, 0, pageImage.Width, pageImage.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
                int dataLength = data.Stride * data.Height;
                byte[] bmpData = new byte[dataLength];
                Marshal.Copy(data.Scan0, bmpData, 0, dataLength);
                pageImage.UnlockBits(data);

                for (int y = 0; y < data.Height; y++)
                {
                    int bmpDataRowBase = y * data.Stride;
                    for (int x = 0; x < this.Width; x++)
                    {
                        int bmpDataIndex = bmpDataRowBase + (x >> 3); // The index where the pixel is stored sub-byte
                        byte mask = (byte)(0x80 >> (x & 0x07));

                        int pixel = (bmpData[bmpDataIndex] & mask);
                        this.ImageData[x, y] = (byte)((pixel == 0) ? Constants.PIXEL_VALUE_INK : Constants.PIXEL_VALUE_WHITESPACE);
                    }
                }
            }
        }

        /// <summary>
        /// Create a 32bpp ARGB <see cref="Bitmap"/> from the 1bpp image
        /// </summary>
        public virtual Bitmap CreateAsBitmap()
        {
            Bitmap retval = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);

            BitmapData bmpData = retval.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int dataLength = bmpData.Stride * bmpData.Height;
            byte[] data = new byte[dataLength];

            for (int y = 0, dataIndex = 0; y < this.Height; y++)
            {
                int rowBase = y * bmpData.Stride;
                for (int x = 0; x < this.Width; x++)
                {
                    byte pixel = this.ImageData[x, y];
                    int finalIndex = rowBase + (x * 4);

                    byte r = pixel, g = pixel, b = pixel;
                    
                    // Write the byte values in BGRA order, full opacity for A
                    data[finalIndex] = b; data[finalIndex + 1] = g; data[finalIndex + 2] = r; data[finalIndex + 3] = 0xFF;
                    dataIndex++;
                }
            }
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
            retval.UnlockBits(bmpData);

            return retval;
        }
        #endregion Image read/write methods
    }
}
