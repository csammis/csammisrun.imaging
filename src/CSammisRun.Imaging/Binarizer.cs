using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CSammisRun.Imaging
{
    public class Binarizer
    {
        /// <summary>
        /// The threshold value at which RGB pixel values are set to white instead of black
        /// </summary>
        public int Threshold { get; set; }

        public Binarizer()
        {
            Threshold = 500;
        }

        public Bitmap ProcessImage(Bitmap image)
        {
            Bitmap source = null, destination = null;

            // Unfortunately a using(source) won't work here because of how it gets conditionally assigned
            try
            {
                if (image.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    source = image;
                }
                else
                {
                    source = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                    if (image.HorizontalResolution != 0 && image.VerticalResolution != 0) //cstodo is this a Mono thing?
                    {
                        source.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    }
                    using (Graphics g = Graphics.FromImage(source))
                    {
                        g.DrawImageUnscaled(image, 0, 0);
                    }
                }

                // Get the source bitmap pixel values into memory
                BitmapData sourceBitmapData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
                                                              ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int sourceSize = sourceBitmapData.Stride * sourceBitmapData.Height;
                byte[] sourceData = new byte[sourceSize];
                Marshal.Copy(sourceBitmapData.Scan0, sourceData, 0, sourceSize);
                source.UnlockBits(sourceBitmapData);
                
                // Create the 1bpp image and create a buffer for its byte data
                destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);
                BitmapData destinationBitmapData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height),
                                                                        ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
                int destinationSize = destinationBitmapData.Stride * destinationBitmapData.Height;
                byte[] destinationData = new byte[destinationSize];

                // Iterate through the original image top-down and left-right
                for (int y = 0; y < source.Height; y++)
                {
                    int sourceIndex = y * sourceBitmapData.Stride;
                    int destinationIndex = y * destinationBitmapData.Stride;
                    byte destinationValue = 0;
                    int pixelValue = 128;
                    
                    for (int x = 0; x < source.Width; x++, sourceIndex += 4)
                    {
                        // Compute total of RGB values, skipping A
                        int rgbTotals = sourceData[sourceIndex + 1] + sourceData[sourceIndex + 2] + sourceData[sourceIndex + 3];
                        if (rgbTotals > this.Threshold)
                        {
                            destinationValue += (byte)pixelValue;
                        }

                        if (pixelValue == 1)
                        {
                            destinationData[destinationIndex] = destinationValue;
                            destinationIndex++;
                            destinationValue = 0;
                            pixelValue = 128;
                        }
                        else
                        {
                            pixelValue >>= 1;
                        }
                    }
                    
                    if (pixelValue != 128)
                    {
                        destinationData[destinationIndex] = destinationValue;
                    }
                }
                
                Marshal.Copy(destinationData, 0, destinationBitmapData.Scan0, destinationData.Length);
                destination.UnlockBits(destinationBitmapData);
            }
            finally
            {
                if (source != image && source != null)
                {
                    source.Dispose();
                }
            }

            return destination;
        }
    }
}

