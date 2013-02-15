using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace CSammisRun.Imaging.Morphology
{
    public class ConnectedRegion
    {
        /// <summary>
        /// Gets or sets the region's ID
        /// </summary>
        public int RegionID { get; set; }

        /// <summary>
        /// Gets the list of coordinates occupied by the region
        /// </summary>
        private readonly List<Point> coordinates = new List<Point>();
        private bool validBoundingRect;
        private Rectangle boundingRect;

        /// <summary>
        /// Initializes a new ConnectedRegion
        /// </summary>
        public ConnectedRegion()
        {
        }

        /// <summary>
        /// Merge a region into the current instance
        /// </summary>
        public void Merge(ConnectedRegion region)
        {
            coordinates.AddRange(region.coordinates);
            validBoundingRect = false;
        }

        /// <summary>
        /// Add a coordinate to the region
        /// </summary>
        /// <param name="p"></param>
        public void AddCoordinate(Point p)
        {
            coordinates.Add(p);
            validBoundingRect = false;
        }

        /// <summary>
        /// Gets a read-only view of the region's coordinate set
        /// </summary>
        public ReadOnlyCollection<Point> GetCoordinates()
        {
            return new ReadOnlyCollection<Point>(coordinates);
        }

        /// <summary>
        /// Get the pixel density in the current region
        /// </summary>
        /// <remarks>Pixel density is defined as the percentage of non-whitespace pixels in the region</remarks>
        public double GetPixelDensity()
        {
            Rectangle boundary = GetBoundingRectangle();
            int pixelCount = boundary.Height * boundary.Width;
            return ((double)coordinates.Count / pixelCount);
        }

        /// <summary>
        /// Determine a bounding rectangle for the region based on the current coordinate set
        /// </summary>
        public Rectangle GetBoundingRectangle()
        {
            if (validBoundingRect)
            {
                return boundingRect;
            }

            coordinates.Sort(new Comparison<Point>(delegate (Point a, Point b)
                {
                    if (a.X == b.X && a.Y == b.Y)
                    {
                        return 0;
                    }

                    if (a.X < b.X || a.Y < b.Y)
                    {
                        return -1;
                    }

                    return 1;
                }));

            int top = Int32.MaxValue, left = Int32.MaxValue, right = -1, bottom = -1;
            foreach (Point p in coordinates)
            {
                if (p.Y < top)
                {
                    top = p.Y;
                }
                if (p.Y > bottom)
                {
                    bottom = p.Y;
                }
                if (p.X < left)
                {
                    left = p.X;
                }
                if (p.X > right)
                {
                    right = p.X;
                }
            }

            boundingRect = new Rectangle(left, top, right - left, bottom - top);
            validBoundingRect = true;
            return boundingRect;
        }
    }
}
