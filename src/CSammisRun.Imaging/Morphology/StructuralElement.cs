using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CSammisRun.Imaging.Morphology
{
    /// <summary>
    /// A structural element used in erosion and dilation operations
    /// </summary>
    public class StructuralElement
    {
        private readonly List<Point> onPoints = new List<Point>();

        /// <summary>
        /// Initializes a new structural element
        /// </summary>
        /// <param name="element">A two-dimensional (width x height) byte array </param>
        public StructuralElement(byte[,] element, Point origin)
        {
            this.Width = element.GetLength(0);
            this.Height = element.GetLength(1);

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (element[x, y] == Constants.PIXEL_VALUE_INK)
                    {
                        this.onPoints.Add(new Point(x - origin.X, y - origin.Y));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the width of the structural element
        /// </summary>
        public int Width
        {
            get; private set;
        }

        /// <summary>
        /// Gets the height of the structural element
        /// </summary>
        public int Height
        {
            get; private set;
        }

        /// <summary>
        /// Gets a list of points relative to the element's origin that should be examined for intersection
        /// </summary>
        public List<Point> ExaminePoints
        {
            get { return onPoints; }
        }
    }
}
