namespace Generator
{
    public static class PerlinNoise
    {
        /// <summary>
        /// My implementation of Perlin Noise
        /// https://iquilezles.org/articles/gradientnoise/
        /// </summary>
        /// <param name="longitude">x in meters</param>
        /// <param name="latitude">y in meters</param>
        /// <param name="gridSize">size off perlin gradient grit</param>
        /// <param name="seed">Extra seed</param>
        /// <returns>Noise value, noise derivative dx, noise derivative dy</returns>
        public static float[] GetNoise(long longitude, long latitude, int gridSize, int seed)
        {
            long gridI = longitude / gridSize;
            if (longitude < 0)
                gridI--;
            float fractI = (longitude - gridI * gridSize) / (float)gridSize;

            long gridJ = latitude / gridSize;
            if (latitude < 0)
                gridJ--;
            float fractJ = (latitude - gridJ * gridSize) / (float)gridSize;

            (float, float) f = (fractI, fractJ);
            //  c     d     Grid coner names
            //     .
            //  a     b
            (int, int) gca = (0, 0);
            (int, int) gcb = (1, 0);
            (int, int) gcc = (0, 1);
            (int, int) gcd = (1, 1);

            (float, float) a = GetGradient(gridI, gridJ, gca, seed);
            (float, float) b = GetGradient(gridI, gridJ, gcb, seed);
            (float, float) c = GetGradient(gridI, gridJ, gcc, seed);
            (float, float) d = GetGradient(gridI, gridJ, gcd, seed);

            float uX = fractI * fractI * fractI * (fractI * (fractI * 6.0f - 15.0f) + 10.0f);
            float uY = fractJ * fractJ * fractJ * (fractJ * (fractJ * 6.0f - 15.0f) + 10.0f);
            float duX = 30.0f * fractI * fractI * (fractI * (fractI - 2.0f) + 1.0f);
            float duY = 30.0f * fractJ * fractJ * (fractJ * (fractJ - 2.0f) + 1.0f);

            float na = Dot(a, f, gca);
            float nb = Dot(b, f, gcb);
            float nc = Dot(c, f, gcc);
            float nd = Dot(d, f, gcd);

            float n = na + uX * (nb - na) + uY * (nc - na) + uX * uY * (na - nb - nc + nd);

            //(float, float)nD =a + uX * (b - a)        + uY * (c - a)        + uX * uY * (a - b - c + d)              + du * (u.yx * (na - nb - nc + nd) + vec2(nb, nc) - na);
            //          =      ga + u.x * (gb - ga)     + u.y * (gc - ga)     + u.x * u.y * (ga - gb - gc + gd)        + du * (u.yx * (va - vb - vc + vd) + vec2(vb, vc) - va))

            float nDx = a.Item1 + uX * (b.Item1 - a.Item1) + uY * (c.Item1 - a.Item1) + uX * uY * (a.Item1 - b.Item1 - c.Item1 + d.Item1) + duX * (uY * (na - nb - nc + nd) + nb - na);
            float nDy = a.Item2 + uX * (b.Item2 - a.Item2) + uY * (c.Item2 - a.Item2) + uX * uY * (a.Item2 - b.Item2 - c.Item2 + d.Item2) + duY * (uX * (na - nb - nc + nd) + nc - na);

            return [n, nDx, nDy];
        }

        private static (float, float) GetGradient(long gridI, long gridJ, (int, int) gridCorner, int seed)
        {
            long i = gridI + gridCorner.Item1;
            long j = gridJ + gridCorner.Item2;
            if (i < 0) i = -i;
            if (j < 0) j = -j;
            int rI = (int)(i & 0xFFFFF);
            int rJ = (int)(j & 0xFFFFF);
            int i1 = rI % 10;
            int i10 = rI % 100 / 10;
            int i100 = rI % 1000 / 100;
            int i1000 = rI % 10000 / 1000;
            int i10000 = rI % 100000 / 10000;
            int j1 = rJ % 10;
            int j10 = rJ % 100 / 10;
            int j100 = rJ % 1000 / 100;
            int j1000 = rJ % 10000 / 1000;
            int j10000 = rJ % 100000 / 10000;
            int firstSeed = i1 +
                            j1 * 10 +
                            i10 * 100 +
                            j10 * 1000 +
                            i100 * 10000 +
                            j100 * 100000 +
                            i1000 * 1000000 +
                            j1000 * 10000000 +
                            i10000 * 100000000 +
                            j10000 * 1000000000 +
                            seed;
            Random ran = new(firstSeed);
            int secondSeed = ran.Next(1048576);
            ran = new Random(secondSeed);
            int gradIndex = ran.Next(gradiants.Count);
            return gradiants[gradIndex];
        }

        private static readonly List<(float, float)> gradiants =
        [
            (0f,1f),(3f/5f,4f/5f),(4f/5f,3f/5f),
            (1f,0f),(4f/5f,-3f/5f),(3f/5f,-4f/5f),
            (0f,-1f),(-3f/5f,-4f/5f),(-4f/5f,-3f/5f),
            (-1f,0f),(-4f/5f,3f/5f),(-3f/5f,4f/5f),
            (0.3f,0.95393920142f),(0.95393920142f,0.3f),
            (0.3f,-0.95393920142f),(0.95393920142f,-0.3f),
            (-0.3f,0.95393920142f),(-0.95393920142f,0.3f),
            (-0.3f,-0.95393920142f),(0.95393920142f,-0.3f)
        ];

        private static float Dot((float, float) g, (float, float) f, (int, int) c)
        {
            return g.Item1 * (f.Item1 - c.Item1) + g.Item2 * (f.Item2 - c.Item2);
        }
    }
}