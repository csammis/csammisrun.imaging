using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CSammisRun.Imaging.Morphology
{
    public class ConnectedImage : OneBppImage
    {
        private const int USEFUL_REGION_THRESHOLD = 70;

        private int[,] labelData;
        private readonly List<Similarity> labels = new List<Similarity>();

        /// <summary>
        /// Initializes a image from a TIFF file
        /// </summary>
        public ConnectedImage(string fileName)
            : base(fileName)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ConnectedImage"/> from an array in memory
        /// </summary>
        /// <param name="imageData"></param>
        public ConnectedImage(byte[,] imageData)
            : base(imageData)
        {
        }

        /// <summary>
        /// Copies the specified <see cref="ConnectedImage"/> into the new instance
        /// </summary>
        public ConnectedImage(ConnectedImage image)
            : base(image)
        {
            labelData = image.labelData;
            foreach (Similarity similarity in image.labels)
            {
                Similarity sim = new Similarity();
                sim.ID = similarity.ID;
                sim.SameAs = similarity.SameAs;
                sim.Tag = similarity.Tag;
                labels.Add(sim);
            }
        }

        /// <summary>
        /// Gets the discrete connected regions in this image
        /// </summary>
        public ReadOnlyCollection<ConnectedRegion> Regions { get; private set; }

        /// <summary>
        /// Run the label connection algorithm on the image
        /// </summary>
        public void LabelConnectedRegions()
        {
            labelData = new int[Width, Height];
            labels.Clear();

            ScanConnectedRegions();
            NormalizeLabelSequence();
        }

        /// <summary>
        /// Condense the <see cref="Regions"/> collection to outermost regions -
        /// that is, if a region A is entirely contained by another region B,
        /// B will be rolled into part of A.
        /// </summary>
        public void MinimizeRegions()
        {
            List<ConnectedRegion> allRegions = new List<ConnectedRegion>(Regions);
            for (int i = 0; i < allRegions.Count; i++)
            {
                for (int j = i + 1; j < allRegions.Count; j++)
                {
                    if (allRegions[i].GetBoundingRectangle().Contains(
                        allRegions[j].GetBoundingRectangle()))
                    {
                        // Update the contained region in labelData to point
                        // at the containing region ID.
                        foreach (Point p in allRegions[j].GetCoordinates())
                        {
                            labelData[p.X, p.Y] = allRegions[i].RegionID;
                        }
                        allRegions.RemoveAt(j);
                        j--;
                    }
                }
            }
            Regions = new ReadOnlyCollection<ConnectedRegion>(allRegions);
        }

        /// <summary>
        /// Writes the image as a PNG with the connected regions colored
        /// </summary>
        public override Bitmap CreateAsBitmap()
        {
            int whiteSpaceLabel = labelData[0, 0]; // Assume that the upper-left corner is whitespace
            Bitmap retval = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            BitmapData bmpData = retval.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] data = new byte[bmpData.Stride * bmpData.Height];

            PropertyInfo[] properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);

            for (int y = 0; y < Height; y++)
            {
                int rowBase = y * bmpData.Stride;
                for (int x = 0; x < Width; x++)
                {
                    byte labelValue = (byte)labelData[x, y];
                    int finalIndex = rowBase + (x * 4);

                    byte r, g, b;
                    if (labelData[x, y] == whiteSpaceLabel)
                    {
                        r = g = b = 0xFF;
                    }
                    else
                    {
                        int colorIndex = (labelValue + 1) % properties.Length;
                        Color color = (Color)properties[colorIndex].GetValue(null, null);
                        b = color.B; if (b > 0xD8) b -= 0x30;
                        g = color.G; if (g > 0xD8) g -= 0x30;
                        r = color.R; if (r > 0xD8) r -= 0x30;
                    }
                    // Write the byte values in BGRA order, full opacity for A
                    data[finalIndex] = b; data[finalIndex + 1] = g; data[finalIndex + 2] = r; data[finalIndex + 3] = 0xFF;
                }
            }
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
            retval.UnlockBits(bmpData);

            return retval;
        }

        #region Region labeling methods

        private void ScanConnectedRegions()
        {
            Label(0, 0, NewLabel());

            // Label the first row
            for (int x = 1; x < Width; x++)
            {
                if (ImageData[x, 0] == Constants.PIXEL_VALUE_WHITESPACE) continue;

                if (ImageData[x, 0] == ImageData[x - 1, 0])
                {
                    Label(x, 0, Label(x - 1, 0));
                }
                else
                {
                    Label(x, 0, NewLabel());
                }
            }

            for (int y = 1, last_y = 0; y < Height; y++, last_y++)
            {
                // Connect the current row with the left side of the row above it
                if (ImageData[0, y] != Constants.PIXEL_VALUE_WHITESPACE)
                {
                    if (ImageData[0, y] == ImageData[0, last_y])
                    {
                        Label(0, y, Label(0, last_y));
                    }
                    else
                    {
                        Label(0, y, NewLabel());
                    }
                }

                // Scan the rest of the row
                for (int x = 1; x < Width; x++)
                {
                    if (ImageData[x, y] == Constants.PIXEL_VALUE_WHITESPACE) continue;

                    int myLabel = -1;
                    if (ImageData[x, y] == ImageData[x - 1, y])
                    {
                        myLabel = Label(x - 1, y);
                    }

                    int endLastRowScan = (x == Width - 1) ? 1 : 2;
                    for (int d = -1; d < endLastRowScan; ++d)
                    {
                        if (ImageData[x, y] == ImageData[x + d, last_y])
                        {
                            if (myLabel != -1)
                            {
                                Merge(myLabel, Label(x + d, last_y));
                            }
                            else
                            {
                                myLabel = Label(x + d, last_y);
                            }
                        }
                    }

                    if (myLabel != -1)
                    {
                        Label(x, y, myLabel);
                    }
                    else
                    {
                        Label(x, y, NewLabel());
                    }

                    if (ImageData[x - 1, y] == ImageData[x, last_y])
                    {
                        Merge(Label(x - 1, y), Label(x, last_y));
                    }
                }
            }
        }

        /// <summary>
        /// Normalizes the labeled regions of the image so that only root regions exist
        /// </summary>
        private void NormalizeLabelSequence()
        {
            // Tag the root labels
            int rootNodeCount = 0;
            for (int i = 0; i < labels.Count; i++)
            {
                if (IsRootNode(i))
                {
                    labels[i].Tag = rootNodeCount++;
                }
            }

            // Initialize as many regions as there are root labels
            ConnectedRegion[] regions = new ConnectedRegion[rootNodeCount];
            for (int i = 0; i < rootNodeCount; i++)
            {
                regions[i] = new ConnectedRegion();
                regions[i].RegionID = i;
            }

            // Fix pathological cases where sequential region numbering is borked
            Dictionary<int, int> realRegions = new Dictionary<int, int>();
            int currentRegion = 0;

            // Run through the image data and set non-root labels to their root,
            // and add each coordinate to the corresponding region.
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int regionID = labels[RootOf(labelData[x, y])].Tag;
                    if (realRegions.ContainsKey(regionID))
                    {
                        regionID = realRegions[regionID];
                    }
                    else
                    {
                        regionID = realRegions[regionID] = currentRegion;
                        currentRegion++;
                    }
                    regions[regionID].AddCoordinate(new Point(x, y));
                    labelData[x, y] = regionID;
                }
            }


            // Cheat: The zeroth region is the image BG itself in this application.
            List<ConnectedRegion> regionList = new List<ConnectedRegion>(regions);
            regionList.RemoveAt(0);

            //// Optimization:  A tiny region isn't going to contain any useful information.
            //// Remove small regions and resequence the list.
            //currentRegion = 0;
            //List<ConnectedRegion> killList = new List<ConnectedRegion>();
            //foreach (ConnectedRegion region in regionList)
            //{
            //    if (region.GetCoordinates().Count < USEFUL_REGION_THRESHOLD)
            //    {
            //        killList.Add(region);
            //    }
            //    else
            //    {
            //        region.RegionID = currentRegion++;
            //        foreach (Point p in region.GetCoordinates())
            //        {
            //            labelData[p.X, p.Y] = region.RegionID;
            //        }
            //    }
            //}
            //foreach (ConnectedRegion region in killList)
            //{
            //    regionList.Remove(region);
            //}
            Regions = new ReadOnlyCollection<ConnectedRegion>(regionList);
        }

        /// <summary>
        /// Gets the label number at a specified location in the output array
        /// </summary>
        private int Label(int locX, int locY)
        {
            return labelData[locX, locY];
        }

        /// <summary>
        /// Sets the label number at a specified location in the output array
        /// </summary>
        private void Label(int locX, int locY, int value)
        {
            labelData[locX, locY] = value;
        }

        /// <summary>
        /// Creates a new label
        /// </summary>
        private int NewLabel()
        {
            labels.Add(new Similarity(labels.Count)); // Add an extra one
            return labels.Count - 1;
        }

        #region Disjoint set unionization methods
        /// <summary>
        /// Gets a value indicating whether a label region is a root node
        /// </summary>
        private bool IsRootNode(int id)
        {
            return (id >= labels.Count || labels[id].SameAs == id);
        }

        /// <summary>
        /// Gets the root of the specified label region
        /// </summary>
        private int RootOf(int id)
        {
            while (!IsRootNode(id))
            {
                id = labels[id].SameAs = labels[labels[id].SameAs].SameAs;
            }
            return id;
        }

        /// <summary>
        /// Gets a value indicating whether two label regions are in the same disjoint set (rooted at the same label)
        /// </summary>
        private bool AreEquivalent(int id1, int id2)
        {
            return (RootOf(id1) == RootOf(id2));
        }

        /// <summary>
        /// Merges two label regions
        /// </summary>
        private bool Merge(int id1, int id2)
        {
            //if (!AreEquivalent(id1, id2))
            //{
                labels[RootOf(id1)].SameAs = RootOf(id2);
                return false;
            //}
            //return true;
        }
        #endregion Disjoint set unionization methods

        /// <summary>
        /// Contains information relating to a image region
        /// </summary>
        class Similarity
        {
            public int ID, SameAs, Tag;

            public Similarity()
            {
                this.ID = this.SameAs = 0;
            }

            public Similarity(int id)
            {
                this.ID = this.SameAs = id;
            }

            public Similarity(int id, int sameas)
            {
                this.ID = id; this.SameAs = sameas;
            }
        }
        #endregion

    }
}
