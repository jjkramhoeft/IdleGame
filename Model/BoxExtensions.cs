namespace Model
{
    ///
    public static class BoxExtensions
    {
        /// <summary>
        /// Is it same box
        /// </summary>
        /// <param name="boxA"></param>
        /// <param name="boxB"></param>
        /// <returns></returns>
        public static bool SameBox(this Box boxA, Box boxB)
        {
            if (boxA.Size != boxB.Size)
                return false;
            if (boxA.LowerLeftPoint.Longitude != boxB.LowerLeftPoint.Longitude)
                return false;
            if (boxA.LowerLeftPoint.Latitude != boxB.LowerLeftPoint.Latitude)
                return false;
            return true;
        }
    }
}