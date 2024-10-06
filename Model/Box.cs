namespace Model
{
    /// <summary>
    /// Square map box
    /// </summary>
    public class Box(Point lowerLeftPoint, long size)
    {
        /// <summary>
        /// Constructor for a new Region
        /// </summary>
        public static Box GetRegionBox(Point p)
        {
            long lowerLeftLongitude = p.Longitude;
            if (0 <= lowerLeftLongitude)
            {
                lowerLeftLongitude /= regionBoxSize;
                lowerLeftLongitude *= regionBoxSize;
            }
            else
            {                
                lowerLeftLongitude ++;
                lowerLeftLongitude /= regionBoxSize;
                lowerLeftLongitude --;
                lowerLeftLongitude *= regionBoxSize;
            }

            long lowerLeftLatitude = p.Latitude;
            if (0 <= lowerLeftLatitude)
            {
                lowerLeftLatitude /= regionBoxSize;
                lowerLeftLatitude *= regionBoxSize;
            }
            else
            {                
                lowerLeftLatitude ++;
                lowerLeftLatitude /= regionBoxSize;
                lowerLeftLatitude --;
                lowerLeftLatitude *= regionBoxSize;
            }

            Point calculatedLowerLeft = new(lowerLeftLongitude, lowerLeftLatitude);
            Box rB = new(calculatedLowerLeft, regionBoxSize);
            return rB;
        }

        /// <summary>
        /// Constructor for a new Location
        /// </summary>
        public static Box GetLocationBox(Point p)
        {
            long lowerLeftLongitude = p.Longitude;
            if (0 <= lowerLeftLongitude)
            {
                lowerLeftLongitude /= pointBoxSize;
                lowerLeftLongitude *= pointBoxSize;
            }
            else
            {                
                lowerLeftLongitude ++;
                lowerLeftLongitude /= pointBoxSize;
                lowerLeftLongitude --;
                lowerLeftLongitude *= pointBoxSize;
            }

            long lowerLeftLatitude = p.Latitude;
            if (0 <= lowerLeftLatitude)
            {
                lowerLeftLatitude /= pointBoxSize;
                lowerLeftLatitude *= pointBoxSize;
            }
            else
            {                
                lowerLeftLatitude ++;
                lowerLeftLatitude /= pointBoxSize;
                lowerLeftLatitude --;
                lowerLeftLatitude *= pointBoxSize;
            }

            Point calculatedLowerLeft = new(lowerLeftLongitude, lowerLeftLatitude);
            Box lB = new(calculatedLowerLeft, pointBoxSize);
            return lB;
        }

        /// <summary>
        /// Lower left corner
        /// </summary>
        public Point LowerLeftPoint { get; set; } = lowerLeftPoint;

        /// <summary>
        /// Length of box sides (meters)
        /// </summary>
        public long Size { get; set; } = size;

        /// <summary>
        /// Length of box sides (meters) for Location point boses
        /// </summary>
        public const int pointBoxSize = 1000;

        /// <summary>
        /// Length of box sides (meters) for region boxes
        /// </summary>
        public const int regionBoxSize = 100000;

        /// <summary>
        /// Calculate pseudo random number for this box. Use as seed for all things relatede to this box
        /// </summary>
        public int GetSeed()
        {
            long i = LowerLeftPoint.Longitude / 1000L;
            long j = LowerLeftPoint.Latitude / 1000L;
            if (i < 0) i = -i + 7;
            if (j < 0) j = -j + 19; // the 7 & 19 is to avoid same seeds around the origon (0,0)
            int rI = (int)(i & 0xFFFFF);
            int rJ = (int)(j & 0xFFFFF);
            int i1 = rI % 10;
            int i10 = (rI % 100) / 10;
            int i100 = (rI % 1000) / 100;
            int i1000 = (rI % 10000) / 1000;
            int i10000 = (rI % 100000) / 10000;
            int j1 = rJ % 10;
            int j10 = (rJ % 100) / 10;
            int j100 = (rJ % 1000) / 100;
            int j1000 = (rJ % 10000) / 1000;
            int j10000 = (rJ % 100000) / 10000;
            int firstSeed = i1 +
                            j1 * 10 +
                            i10 * 100 +
                            j10 * 1000 +
                            i100 * 10000 +
                            j100 * 100000 +
                            i1000 * 1000000 +
                            j1000 * 10000000 +
                            i10000 * 100000000 +
                            j10000 * 1000000000;
            Random ran = new(firstSeed);
            return ran.Next(1048576);
        }        
    }
}