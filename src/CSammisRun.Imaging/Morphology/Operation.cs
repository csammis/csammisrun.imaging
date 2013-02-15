using System.Drawing;

namespace CSammisRun.Imaging.Morphology
{
    public class Operation
    {
        /// <summary>
        /// Gets the <see cref="StructuralElement"/> used for the morphology operation
        /// </summary>
        public StructuralElement StructuralElement { get; private set; }

        public Operation(StructuralElement element)
        {
            this.StructuralElement = element;
        }

        #region Public methods
        /// <summary>
        /// Dilate a <see cref="OneBppImage"/>
        /// </summary>
        public OneBppImage Dilate(OneBppImage source)
        {
            byte[,] data = new byte[source.Width, source.Height];
            
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    byte pointValue = Constants.PIXEL_VALUE_WHITESPACE;
                    foreach (Point p in StructuralElement.ExaminePoints)
                    {
                        int testX = x + p.X;
                        int testY = y + p.Y;
                        if (testX < 0 || testY < 0 || testX >= source.Width || testY >= source.Height)
                        {
                            continue;
                        }
                        
                        if (source.ImageData[testX, testY] == Constants.PIXEL_VALUE_INK)
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
        /// Erode a <see cref="OneBppImage"/>
        /// </summary>
        public OneBppImage Erode(OneBppImage source)
        {
            byte[,] data = new byte[source.Width, source.Height];
            
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    if (source.ImageData[x, y] == Constants.PIXEL_VALUE_INK)
                    {
                        foreach (Point p in StructuralElement.ExaminePoints)
                        {
                            int testX = x + p.X;
                            int testY = y + p.Y;
                            if (testX < 0 || testY < 0 || testX >= source.Width || testY >= source.Height)
                            {
                                data[x, y] = Constants.PIXEL_VALUE_WHITESPACE;
                                goto NextPixel;
                            }
                            
                            if (source.ImageData[testX, testY] != Constants.PIXEL_VALUE_INK)
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
        /// Perform a morphological closing on a <see cref="OneBppImage"/>
        /// </summary>
        public OneBppImage Close(OneBppImage source)
        {
            return Erode(Dilate(source));
        }
        
        /// <summary>
        /// Perform a morphological opening on a <see cref="OneBppImage"/>
        /// </summary>
        public OneBppImage Open(OneBppImage source)
        {
            return Dilate(Erode(source));
        }
        #endregion
    }
}

