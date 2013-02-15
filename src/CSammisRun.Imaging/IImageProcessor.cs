using System.Drawing;

namespace CSammisRun.Imaging
{
    /// <summary>
    /// Defines an interface for processing a <see cref="Bitmap"/>
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Processes a <see cref="Bitmap"/> with a single operation
        /// </summary>
        /// <returns>
        /// A new <see cref="Bitmap"/> corresponding to <paramref name="image"/> after having been processed
        /// </returns>
        /// <param name='image'>
        /// A <see cref="Bitmap"/> to be processed.
        /// </param>
        /// <remarks>The <paramref name="image"/> will not be Disposed or mutated during processing.
        Bitmap ProcessImage(Bitmap image);
    }
}

