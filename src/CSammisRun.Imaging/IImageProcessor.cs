using System;
using System.Drawing;

namespace CSammisRun.Imaging
{
    /// <summary>
    /// Defines an interface for processing a <see cref="Bitmap"/>
    /// </summary>
    public interface IImageProcessor
    {
        Bitmap ProcessImage(Bitmap image);
    }
}

