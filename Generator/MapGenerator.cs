using Generator.Locations;
using Model;

namespace Generator
{
#pragma warning disable CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
    public static class MapGenerator
    {
        public const double startRadius2 = 50000000000;

        public static int RadToNearestDirectionId(double directionInRad)
        {
            double twoPi = Math.PI * 2.0;
            double normalRad = directionInRad;
            while (twoPi < normalRad)
                normalRad -= twoPi;
            while (normalRad < 0.0)
                normalRad += twoPi;
            double deltaRadToEachSide = Math.PI / World.Directions.Count;

            foreach (var d in World.Directions.Values)
            {
                double minRad = d.AngleInRadians - deltaRadToEachSide;
                double maxRad = d.AngleInRadians + deltaRadToEachSide;
                if ((minRad + twoPi <= directionInRad && directionInRad <= maxRad + twoPi) ||
                    (minRad <= directionInRad && directionInRad <= maxRad) ||
                    (minRad - twoPi <= directionInRad && directionInRad <= maxRad - twoPi)
                )
                    return d.Id;
            }
            return 2;
            throw new Exception("Could not calc direction!");
        }

        public static Location CalculateBase(Point p, int seed)
        {
            Location l = new(p);
            // Day One - Light
            // Temperature
            float baseTemp = GetBaseTemperature(p, seed);
            // Age
            l.Age = GetAge(p, seed);

            // Day Two - Not Water
            // Wind
            float windDirection = GetWind(p, seed);
            double wx = Math.Cos(windDirection);
            double wy = Math.Sin(windDirection);
            while (windDirection < 0) windDirection += (float)Math.PI * 2f;
            while (Math.PI * 2f < windDirection) windDirection -= (float)Math.PI * 2f;
            // Storm frequens
            float stormF = GetStormFrequency(p, seed);
            // Precipitation
            float basePrecipitation = GetBasePrecipitation(p, seed);

            // Day Three - Land and Green
            // Height
            float[] height = GetHeight(p, seed);
            l.SlopeValue = CalcSlopeValue(height);
            l.Height = height[0];
            float temperature = CalcTemperature(height[0], baseTemp);
            float percipation = CalcPrecipitation(basePrecipitation, temperature, windDirection, height[1], height[2], height[0]);
            l.Climate = new()
            {
                AverageTemperature = temperature,
                PrecipitationAmount = percipation,
                StormFrequency = stormF,
                PredominantWindDirection = World.Directions[RadToNearestDirectionId(windDirection)]
            };
            // Geology
            l.GeologyId = GetGeology(p, seed);

            return l;
        }

        /// <summary>
        /// Try all filters to find bioms for this location
        /// </summary>
        /// <returns>Did it find at least one biom for this location</returns>
        public static bool CalculateBioms(this Location l, int seed)
        {
            Random rand = new(seed);
            List<(BiomIds, float)> allFittingBioms = [];
            foreach (var biomId in Biom.GetAllUsedBioms())
            {
                Biom b = World.Bioms[biomId];
                bool matched = false;
                bool boost = false;
                float weight = 0.001f;
                foreach (var f in b.Filters)
                    if (l.Match(f))
                    {
                        matched = true;
                        weight = f.Weight + rand.NextSingle() * 0.1f;
                        if (f.BoostingGeologyId == l.GeologyId)
                            boost = true;
                    }
                if (matched)
                {
                    if (boost)
                        weight *= 2f;
                    allFittingBioms.Add((b.Id, weight));
                }
            }
            if (allFittingBioms.Count == 0)
                return false;
            else if (allFittingBioms.Count == 1)
                l.BiomId = allFittingBioms[0].Item1;
            else
            {
                var t = allFittingBioms.OrderByDescending(f => f.Item2);
                l.BiomId = t.First().Item1;
            }

            if (l.Age == -1 && 0 < World.Bioms[l.BiomId].AncientId)
            {
                l.BiomId = World.Bioms[l.BiomId].AncientId;
            }
            else if (l.Age == 1 &&
                0 < World.Bioms[l.BiomId].ModernId &&
                l.SlopeValue < Slope.NormalSteepSplit &&
                l.GeologyId != GeologyId.Forest) // Forest geology suppresses fields
            {
                l.BiomId = World.Bioms[l.BiomId].ModernId;
            }
            return true;
        }

        /// <summary>
        /// This goes before picking race
        /// </summary>
        public static void PickLocationType(this Location location, Region region, int seed)
        {
            var factory = new LocationTypeFactory();
            var allLocationTypes = factory.GetSupportedLocationTypes();
            List<LocationTypes> matchingCommon = [];
            double commomMin = 0;
            double commonMax = 0.65;
            List<LocationTypes> matchingRare = [];
            double rareMin = 0.65;
            double rareMax = 0.92;
            List<LocationTypes> matchingVeryRare = [];
            double veryRareMin = 0.92;
            double veryRareMax = 1;
            foreach (var lt in allLocationTypes)
            {
                var f = factory.GetLocationType(lt);
                if (f.IsValidOn(location, region))
                {
                    switch (f.RarityOn(location, region))
                    {
                        case LocationType.Rarity.Commen:
                            matchingCommon.Add(lt);
                            break;
                        case LocationType.Rarity.Rare:
                            matchingRare.Add(lt);
                            break;
                        case LocationType.Rarity.VeryRare:
                            matchingVeryRare.Add(lt);
                            break;
                    }
                }
            }
            if (matchingCommon.Count == 0 && matchingRare.Count == 0 && matchingVeryRare.Count == 0)
                throw new Exception($"No matching locationType found biom:{location.BiomId} population:{location.Population} avg.temp.:{location.Climate?.AverageTemperature}");
            else if (0 < matchingCommon.Count && matchingRare.Count == 0 && matchingVeryRare.Count == 0)
            {
                commomMin = 0;
                commonMax = 1;
                rareMin = 2;
                rareMax = 2;
                veryRareMin = 2;
                veryRareMax = 2;
            }
            else if (0 < matchingCommon.Count && 0 < matchingRare.Count && matchingVeryRare.Count == 0)
            {
                commomMin = 0;
                commonMax = 0.79;
                rareMin = 0.79;
                rareMax = 1;
                veryRareMin = 2;
                veryRareMax = 2;
            }
            else if (0 < matchingCommon.Count && matchingRare.Count == 0 && 0 < matchingVeryRare.Count)
            {
                commomMin = 0;
                commonMax = 0.94;
                rareMin = 2;
                rareMax = 2;
                veryRareMin = 0.94;
                veryRareMax = 1;
            }
            else if (0 == matchingCommon.Count && 0 < matchingRare.Count && 0 == matchingVeryRare.Count)
            {
                commomMin = -1;
                commonMax = -1;
                rareMin = 0;
                rareMax = 1;
                veryRareMin = 2;
                veryRareMax = 2;
            }
            else if (0 < matchingCommon.Count && 0 < matchingRare.Count && 0 < matchingVeryRare.Count)
            {

            }
            else
                throw new NotImplementedException($"Combination of matching - Not implemented! common:{matchingCommon.Count}, rare:{matchingRare.Count}, very rare{matchingVeryRare.Count}");

            Random rand = new(seed);
            var r = rand.NextDouble();
            if (commomMin <= r && r < commonMax)
            {
                //common
                int rr = rand.Next(matchingCommon.Count);
                location.LocationTypeKey = matchingCommon[rr];
                return;
            }
            else if (rareMin <= r && r < rareMax)
            {
                //rare
                int rr = rand.Next(matchingRare.Count);
                location.LocationTypeKey = matchingRare[rr];
                return;
            }
            else if (veryRareMin <= r && r < veryRareMax)
            {
                //veryrare                
                int rr = rand.Next(matchingVeryRare.Count);
                location.LocationTypeKey = matchingVeryRare[rr];
                return;
            }
            else
                throw new Exception("Random bounds error!");
        }

        ///
        public static int CalculatePopulationPotential(this Location l)
        {
            if (l.BiomId == 0)
                return 0;
            if (Slope.NormalSteepSplit < l.SlopeValue)
            {
                if (l.BiomId == BiomIds.BareRock)
                {
                    if (0 < l.GeologyId && l.GeologyId == Geology.Minerals)
                        return 40;
                    else
                        return 10;
                }
                else
                    return 0;
            }
            int basePop = 0;
            switch (World.Bioms[l.BiomId].PopulationPotential)
            {
                case Biom.PopulationPotentials.None:
                    basePop = 0;
                    break;
                case Biom.PopulationPotentials.VeryFew:
                    basePop = 0;
                    if (l.Height < Height.MainlandCoastSplit)
                        basePop++;
                    if (l.Height < Height.LowHillsMainlandSplit)
                        basePop++;
                    break;
                case Biom.PopulationPotentials.Few:
                    basePop = 1;
                    if (l.Height < Height.MainlandCoastSplit)
                        basePop++;
                    if (l.Height < Height.LowHillsMainlandSplit)
                        basePop++;
                    break;
                case Biom.PopulationPotentials.Some:
                    basePop = 3;
                    if (l.Height < Height.MainlandCoastSplit)
                        basePop++;
                    if (l.Height < Height.LowHillsMainlandSplit)
                        basePop++;
                    break;
                case Biom.PopulationPotentials.Good:
                    basePop = 10;
                    if (l.Height < Height.MainlandCoastSplit)
                        basePop += 5;
                    if (l.Height < Height.LowHillsMainlandSplit)
                        basePop += 10;
                    if (Slope.FlatNormalSplit < l.SlopeValue)
                        basePop += 5;
                    break;
                case Biom.PopulationPotentials.Great:
                    basePop = 50;
                    if (l.Height < Height.MainlandCoastSplit)
                        basePop += 10;
                    if (l.Height < Height.LowHillsMainlandSplit)
                        basePop += 20;
                    if (Slope.FlatNormalSplit < l.SlopeValue)
                        basePop += 20;
                    break;
            }
            if (0 < l.GeologyId)
            {
                switch (l.GeologyId)
                {
                    case GeologyId.Fertile:
                        basePop *= 2;
                        break;
                    case GeologyId.Poor:
                        basePop /= 2;
                        break;
                    case GeologyId.Minerals:
                        if (Height.HillLowHillSplit < l.Height)
                            basePop *= 2;
                        break;
                }
            }
            return basePop;
        }

        /// <summary>
        /// First version of map height.
        /// Max ~ 1.21 km  
        /// Min ~ -0.8 km
        /// 
        /// max dx ~ 2.45
        /// min dx ~ -2.66
        /// </summary>
        /// <returns>Height above sealevel in km, and the derivatives (h,dx,dy)</returns>
        public static float[] GetHeight(Point p, int seed)
        {
            int boxSize0 = 2000; float amp0 = 0.062523f;
            int boxSize1 = 4777; float amp1 = 0.125021f;
            int boxSize2 = 8032; float amp2 = 0.250424f;
            int boxSize3 = 16100; float amp3 = 0.500193f;
            int boxSize4 = 36000; float amp4 = 1.000011f;
            int boxSize5 = 64300; float amp5 = 0.400081f;

            var n0 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize0, seed);
            var n1 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize1, seed);
            var n2 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize2, seed);
            var n3 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize3, seed);
            var n4 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize4, seed);
            var n5 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize5, seed);

            float h = n0[0] * amp0 + n1[0] * amp1 + n2[0] * amp2 + n3[0] * amp3 + n4[0] * amp4 + n5[0] * amp5 + 0.25f;
            float dx = n1[1] * amp1 + n2[1] * amp2 + n3[1] * amp3 + n4[1] * amp4 + n5[1] * amp5;  // omitted n0[1] * amp0 
            float dy = n1[2] * amp1 + n2[2] * amp2 + n3[2] * amp3 + n4[2] * amp4 + n5[2] * amp5;  // omitted n0[2] * amp0 
            float[] res = [h, dx, dy];
            return res;
        }

        /// <summary>
        /// 0     => west -> east
        /// Pi/2  => south -> north
        /// Pi    => east-> west
        /// 3/2Pi => north -> south
        /// 
        /// Max ~ 6.28
        /// Min ~ 0.0
        /// </summary>
        /// <returns>Wind direction in radians</returns>
        public static float GetWind(Point p, int seed)
        {
            int boxSize0 = 150023;
            float amp0 = 1.1f;

            var n0 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize0, seed);
            float w = (n0[0] * amp0 + 0.66f) * 5f;
            float pi = 3.14159265358979323846f;
            if (w < 0) w += 2f * pi;
            if (2f * pi < w) w -= 2f * pi;
            return w;
        }

        /// <summary>
        /// Base temperature in degree celsius.
        /// Not adjusted for height.
        /// Max ~  37.4
        /// Min ~ -10.0
        /// </summary>
        public static float GetBaseTemperature(Point p, int seed)
        {
            int boxSize0 = 170207;
            float amp0 = 40f;

            var n0 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize0, seed);
            float t = n0[0] * amp0 + 15.0f;
            return t;
        }

        public const float StormFrequencyStormy = 0.70f;
        public const float StormFrequencyQuite = 0.00f;
        public static float GetStormFrequency(Point p, int seed)
        {
            int boxSize0 = 430990;

            var n0 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize0, seed);
            float t = n0[0];
            if (t < 0) t = 0f;
            return t * t * 3f;
        }

        public const float OldAge = -0.724f;
        public const float NewAge = 0.715f;
        /// <summary>
        /// Age 0=normal -1=Old 1=Modern
        /// </summary>
        public static int GetAge(Point p, int seed)
        {
            int boxSize0 = 4309;

            var n0 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize0, seed);
            var n1 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize0 + 17, seed);
            float t = n0[0] + n1[0] * 0.98f;
            if (t < OldAge) return -1;
            if (NewAge < t) return 1;
            return 0;
        }


        /// <summary>
        /// Max ~ 34
        /// Min ~ 4
        /// </summary>
        public static float GetBasePrecipitation(Point p, int seed)
        {
            int boxSize0 = 110207;
            float amp0 = 25f;
            long warpLongitudeDelta = 272121;
            long warpLatitudeDelta = -43434;
            float warpFactor = 160500f;

            var nlongi = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize0, seed);
            var nlati = PerlinNoise.GetNoise(p.Longitude + warpLongitudeDelta, p.Latitude + warpLatitudeDelta, boxSize0, seed);
            var n0 = PerlinNoise.GetNoise(p.Longitude + (long)(warpFactor * nlongi[0]), p.Latitude + (long)(warpFactor * nlati[0]), boxSize0, seed);
            float pp = (n0[0] + 0.8f) * amp0;
            return pp;
        }

        public static GeologyId GetGeology(Point p, int seed)
        {
            int bit1 = 0;
            int bit2 = 0;
            int bit3 = 0;

            if (p.Dist2(new(0, 0)) < startRadius2)
                return GeologyId.Plain;//Plain around start

            int boxSize0 = 150023;
            var n0 = PerlinNoise.GetNoise(p.Longitude, p.Latitude, boxSize0, seed);
            if (n0[0] > 0f) bit1 = 1;
            if (Math.Abs(n0[0]) < 0.31f) bit2 = 1;

            int boxSize1 = 120432;
            var n1 = PerlinNoise.GetNoise(p.Longitude + 43216, p.Latitude - 2349, boxSize1, seed);
            if (n1[0] > 0f) bit3 = 1;

            return (GeologyId)bit1 + 2 * bit2 + 4 * bit3;
        }

        public static float CalcTemperature(float height, float baseTemperature)
        {
            if (height < 0)
                return baseTemperature;
            return baseTemperature - height * 15f;
        }

        public static float CalcPrecipitation(float basePrecipitation, float temperature, float windDirection, float hDx, float hDy, float h)
        {
            float wX = (float)Math.Cos(windDirection);
            float wY = (float)Math.Sin(windDirection);
            float det = wX * hDy - wY * hDx;
            float factSlope = 0.35f * Math.Abs(det);
            if (h < 0.1f)
                factSlope = 1;

            float factTemp = 1f;
            if (temperature > 1f)
                factTemp = 1f + temperature / 30f;

            return 1.3f * basePrecipitation * (1f + factSlope * factTemp * 1.5f) - 10f;
        }

        public static float CalcSlopeValue(float[] h)
        {
            float d = Math.Max(Math.Abs(h[1]), Math.Abs(h[2]));
            if (d < 0f)
                return 0f;
            if (1.8f < d)
                return 1.8f;
            return d;
        }

        /// <summary>
        /// Make Regions for societies, One in each 100.000 x 100.000 box (regionBoxSize x regionBoxSize)
        /// </summary>
        public static List<Region> MakeRegions(long minLongitude, long maxLongitude, long minLatitude, long maxLatitude)
        {
            List<Box> boxes = GetBoxes(minLongitude, maxLongitude, minLatitude, maxLatitude, Box.regionBoxSize, true);

            var regions = new List<Region>();
            foreach (var box in boxes)
                regions.Add(InitRegion(box));
            return regions;
        }

        ///
        public static Region InitRegion(Box box)
        {
            var r = new Region(box);
            Random random = new(box.GetSeed());
            random = new(random.Next(1000000000));
            r.Center = new Point(
                box.LowerLeftPoint.Longitude + random.Next(Box.regionBoxSize),
                box.LowerLeftPoint.Latitude + random.Next(Box.regionBoxSize));
            return r;
        }

        ///
        public static List<Region> GetNeighborRegions(Box box)
        {
            if (box.Size != Box.regionBoxSize)
                throw new Exception("Box is not region sized!");
            List<Region> neighbors = [];
            Region regionUp = InitRegion(new(new(box.LowerLeftPoint.Longitude, box.LowerLeftPoint.Latitude + Box.regionBoxSize), Box.regionBoxSize));
            Region regionUpRight = InitRegion(new(new(box.LowerLeftPoint.Longitude + Box.regionBoxSize, box.LowerLeftPoint.Latitude + Box.regionBoxSize), Box.regionBoxSize));
            Region regionRight = InitRegion(new(new(box.LowerLeftPoint.Longitude + Box.regionBoxSize, box.LowerLeftPoint.Latitude), Box.regionBoxSize));
            Region regionDownRight = InitRegion(new(new(box.LowerLeftPoint.Longitude + Box.regionBoxSize, box.LowerLeftPoint.Latitude - Box.regionBoxSize), Box.regionBoxSize));
            Region regionDown = InitRegion(new(new(box.LowerLeftPoint.Longitude, box.LowerLeftPoint.Latitude - Box.regionBoxSize), Box.regionBoxSize));
            Region regionDownLeft = InitRegion(new(new(box.LowerLeftPoint.Longitude - Box.regionBoxSize, box.LowerLeftPoint.Latitude - Box.regionBoxSize), Box.regionBoxSize));
            Region regionLeft = InitRegion(new(new(box.LowerLeftPoint.Longitude - Box.regionBoxSize, box.LowerLeftPoint.Latitude), Box.regionBoxSize));
            Region regionUpLeft = InitRegion(new(new(box.LowerLeftPoint.Longitude - Box.regionBoxSize, box.LowerLeftPoint.Latitude + Box.regionBoxSize), Box.regionBoxSize));
            neighbors.Add(regionUp);
            neighbors.Add(regionUpRight);
            neighbors.Add(regionRight);
            neighbors.Add(regionDownRight);
            neighbors.Add(regionDown);
            neighbors.Add(regionDownLeft);
            neighbors.Add(regionLeft);
            neighbors.Add(regionUpLeft);
            return neighbors;
        }

        ///
        public static List<Box> GetNeighborLocationBoxes(Box box)
        {
            if (box.Size != Box.pointBoxSize)
                throw new Exception("Box is not point box sized!");
            List<Box> neighbors = [];
            Box boxUp = new(new Point(box.LowerLeftPoint.Longitude, box.LowerLeftPoint.Latitude + Box.pointBoxSize), Box.pointBoxSize);
            Box boxUpRight = new(new Point(box.LowerLeftPoint.Longitude + Box.pointBoxSize, box.LowerLeftPoint.Latitude + Box.pointBoxSize), Box.pointBoxSize);
            Box boxRight = new(new Point(box.LowerLeftPoint.Longitude + Box.pointBoxSize, box.LowerLeftPoint.Latitude), Box.pointBoxSize);
            Box boxDownRight = new(new Point(box.LowerLeftPoint.Longitude + Box.pointBoxSize, box.LowerLeftPoint.Latitude - Box.pointBoxSize), Box.pointBoxSize);
            Box boxDown = new(new Point(box.LowerLeftPoint.Longitude, box.LowerLeftPoint.Latitude - Box.pointBoxSize), Box.pointBoxSize);
            Box boxDownLeft = new(new Point(box.LowerLeftPoint.Longitude - Box.pointBoxSize, box.LowerLeftPoint.Latitude - Box.pointBoxSize), Box.pointBoxSize);
            Box boxLeft = new(new Point(box.LowerLeftPoint.Longitude - Box.pointBoxSize, box.LowerLeftPoint.Latitude), Box.pointBoxSize);
            Box boxUpLeft = new(new Point(box.LowerLeftPoint.Longitude - Box.pointBoxSize, box.LowerLeftPoint.Latitude + Box.pointBoxSize), Box.pointBoxSize);
            neighbors.Add(boxUp);
            neighbors.Add(boxUpRight);
            neighbors.Add(boxRight);
            neighbors.Add(boxDownRight);
            neighbors.Add(boxDown);
            neighbors.Add(boxDownLeft);
            neighbors.Add(boxLeft);
            neighbors.Add(boxUpLeft);
            return neighbors;
        }

        public static async Task Survey(this Region r, int worldSeed)
        {
            int regionSeed = r.Box.GetSeed();
            int basePopulationPotential = 0;
            List<Location> regionLocations = [];
            Dictionary<RaceIds, int> racialPopulationPotentialInRegion = [];
            Dictionary<RaceIds, int> racialMaxPopulationAtAnyLocation = [];
            foreach (var raceId in Race.AllIds)
            {
                Race rr = World.Races[raceId];
                racialMaxPopulationAtAnyLocation.Add(rr.Id, 0);
                r.RacialLocationCount.Add(rr.Id, 0);
                r.RacialTopLocations.Add(rr.Id, []);
                racialPopulationPotentialInRegion.Add(rr.Id, 0);
            }
            foreach (var g in Geology.AllIds)
                r.GeologiesLocationCount.Add(g, 0);
            List<Region> neighborRegions = GetNeighborRegions(r.Box);
            foreach (Point p in MakePoints(
                r.Box.LowerLeftPoint.Longitude - Box.regionBoxSize + Box.pointBoxSize, r.Box.LowerLeftPoint.Longitude + (Box.regionBoxSize * 2) - Box.pointBoxSize,
                r.Box.LowerLeftPoint.Latitude - Box.regionBoxSize + Box.pointBoxSize, r.Box.LowerLeftPoint.Latitude + (Box.regionBoxSize * 2) - Box.pointBoxSize))
            {
                double regionDist2 = r.Center.Dist2(p);
                bool ok = true;
                foreach (Region neighborRegion in neighborRegions)
                    if (neighborRegion.Center.Dist2(p) < regionDist2)
                    {
                        ok = false;
                        break;
                    }
                if (ok)
                {
                    Location l = CalculateBase(p, worldSeed);
                    l.CalculateBioms(regionSeed);
                    var locationPopulationPotential = l.CalculatePopulationPotential();
                    if (l.Age == -1) r.AncientLocationCount++;
                    else if (l.Age == 1) r.ModernLocationCount++;
                    r.GeologiesLocationCount[l.GeologyId]++;
                    Dictionary<RaceIds, int> racialPopulationPotentionAtThisLocation = [];
                    foreach (var rr in Race.AllIds)
                        racialPopulationPotentionAtThisLocation.Add(rr, 0);
                    if (0 < locationPopulationPotential && (l.BiomId != BiomIds.None))
                    {
                        r.InhabitedLocationCount++;
                        foreach (var raceId in Race.AllIds)
                        {
                            Race race = World.Races[raceId];
                            if (race.Filter is null || l.Match(race.Filter))
                            {
                                var racePopPot = CalcRacialPopulationPotential(race, locationPopulationPotential, l.BiomId, l.GeologyId);
                                if (0 < racePopPot)
                                {
                                    racialPopulationPotentionAtThisLocation[race.Id] = racePopPot;
                                    racialPopulationPotentialInRegion[race.Id] += racePopPot;
                                    if (racialMaxPopulationAtAnyLocation[race.Id] < racePopPot)
                                        racialMaxPopulationAtAnyLocation[race.Id] = racePopPot;
                                    l.RacialPopulationPotention = racialPopulationPotentionAtThisLocation;// better safe than sorry - do it now, do not only wait
                                    r.RacialTopLocations[race.Id].Add(l);
                                }
                            }
                        }
                        RaceIds topRace = RaceIds.None;
                        int topRacePopulationPotential = 0;
                        foreach (var raceId in Race.AllIds)
                        {
                            if (topRacePopulationPotential < racialPopulationPotentionAtThisLocation[raceId])
                            {
                                topRacePopulationPotential = racialPopulationPotentionAtThisLocation[raceId];
                                topRace = raceId;
                            }
                        }
                        if (topRace != RaceIds.None)
                            r.RacialLocationCount[topRace]++;
                    }
                    l.RacialPopulationPotention = racialPopulationPotentionAtThisLocation;
                    regionLocations.Add(l);
                    basePopulationPotential += locationPopulationPotential;
                }
            }
            r.CombinedPopulationPotential = basePopulationPotential;
            r.RacialPopulationPotential = racialPopulationPotentialInRegion;
            foreach (var raceId in Race.AllIds)
            {
                int theMax = racialMaxPopulationAtAnyLocation[raceId];
                List<Location> locationsWithMaxPopPot = [];
                foreach (var l in r.RacialTopLocations[raceId])
                {
                    if (l.RacialPopulationPotention[raceId] == theMax)
                        locationsWithMaxPopPot.Add(l);
                }
                r.RacialTopLocations[raceId] = locationsWithMaxPopPot;
            }
            foreach (Location l in regionLocations)
            {
                r.LocationCount++;
                if (l.Height < Height.WaterLevel)
                    r.WaterLocationCount++;
                if (r.BiomCount.TryGetValue(l.BiomId, out int value))
                    r.BiomCount[l.BiomId] = value + 1;
                else
                    r.BiomCount.Add(l.BiomId, 1);
            }
            r.RegionWealth = CalculateWealth(regionSeed, regionLocations.Count, r.GeologiesLocationCount[GeologyId.Minerals], r.GeologiesLocationCount[GeologyId.Fertile], r.GeologiesLocationCount[GeologyId.Poor]);
            r.MainRaceId = PickMainRace(regionSeed, r);
            r.RulingFamilyName = await NameGenerator.GenerateSurName(regionSeed, r.MainRaceId);
            foreach (var supR in PickSupportingRaces(regionSeed, r))
                r.SupportingRaceIds.Add(supR.Id);
            foreach (var oppR in PickOpposingRaces(regionSeed, r))
                r.OpposingRaceIds.Add(oppR.Id);
            r.Name = NameGenerator.GenerateRegionName(regionSeed, r);
            r.PickMainLocationsInRegion(regionSeed);
            // Pick worldtree location
            int maxWeight = 0;
            BiomIds lifeTreeBiom  = BiomIds.None;
            foreach (var biomId in World.GetForestBiomIds())
            {
                if(r.BiomCount.Keys.Contains(biomId))
                {
                    int weight = r.BiomCount[biomId];
                    if (biomId == BiomIds.AncientForest)
                        weight *= 5;
                    if (biomId == BiomIds.MushroomForest)
                        weight *= 7;
                    if(maxWeight<=weight)
                    {
                        maxWeight=weight;
                        lifeTreeBiom = biomId;
                    }
                }
            }
            long logitudeSum =0;
            long latitudeSum =0;
            int count =0;
            foreach (Location l in regionLocations)
            {
                if(l.BiomId==lifeTreeBiom)
                {
                    logitudeSum += l.Box.LowerLeftPoint.Longitude;
                    latitudeSum += l.Box.LowerLeftPoint.Latitude;
                    count++;
                }
            }
            Point avgPoint = new(logitudeSum/count,latitudeSum/count);
            double minDist2 = double.MaxValue; 
            foreach (Location l in regionLocations)
            {
                if(l.BiomId==lifeTreeBiom)
                {
                    var dist2 = l.Box.LowerLeftPoint.Dist2(avgPoint);
                    if(dist2<minDist2)
                    {
                       minDist2=dist2; 
                       r.LifeTreePoint = l.Point;
                    }
                }
            }
        }

        public static int CalcRacialPopulationPotential(Race r, int locationPopulationPotential, BiomIds primaryBiomId, GeologyId geologyId)
        {
            int racePopPot = 0;
            if (r.PrimaryBioms.Contains(primaryBiomId))
                racePopPot = (int)(locationPopulationPotential * r.PopulationModifier * 1.1f);
            else if (r.SecondaryBioms.Contains(primaryBiomId))
                racePopPot += (int)(locationPopulationPotential * r.PopulationModifier * 0.5f);
            if (geologyId == GeologyId.Forest)
            {
                if (r.Id == RaceIds.Human)
                    racePopPot /= 2;
                else if (r.Id == RaceIds.Elve)
                    racePopPot *= 3;
                else if (r.Id == RaceIds.Ent)
                    racePopPot *= 2;
            }
            else if (geologyId == GeologyId.Minerals)
            {
                if (r.Id == RaceIds.Dwarf)
                    racePopPot *= 3;
            }
            else if (geologyId == GeologyId.Dark)
            {
                if (r.Id == RaceIds.Orc)
                    racePopPot *= 2;
                else if (r.Id == RaceIds.Goblin)
                    racePopPot *= 2;
                else if (r.Id == RaceIds.Lizard)
                    racePopPot *= 2;
                else if (r.Id == RaceIds.Minotaur)
                    racePopPot *= 2;
                else if (r.Id == RaceIds.Ent)
                    racePopPot /= 2;
                else if (r.Id == RaceIds.Fae)
                    racePopPot /= 2;
                else if (r.Id == RaceIds.Elve)
                    racePopPot /= 2;
                else if (r.Id == RaceIds.Human)
                    racePopPot /= 2;
                else if (r.Id == RaceIds.Hare)
                    racePopPot /= 2;
                else if (r.Id == RaceIds.Haflings)
                    racePopPot /= 2;
                else if (r.Id == RaceIds.Centaur)
                    racePopPot /= 2;
            }
            else if (geologyId == GeologyId.Magic)
            {
                if (r.Id == RaceIds.Fae)
                    racePopPot *= 3;
                else if (r.Id == RaceIds.Human)
                    racePopPot /= 3;
                else
                    racePopPot *= 2;
            }
            return racePopPot;
        }

        private static RegionWealth CalculateWealth(int seed, int totalCount, int mineralsGeologyLocationCount, int fertileGeologyLocationCount, int poorGeologyLocationCount)
        {
            float procentMineral = 100f * mineralsGeologyLocationCount / totalCount;
            float procentFertile = 100f * fertileGeologyLocationCount / totalCount;
            float procentPoor = 100f * poorGeologyLocationCount / totalCount;
            float procentSize = 100f * totalCount / (Box.regionBoxSize / Box.pointBoxSize * (Box.regionBoxSize / Box.pointBoxSize));
            int w = 0;
            if (200 < mineralsGeologyLocationCount) w++;
            if (5 < procentMineral) w++;
            if (35 < procentMineral) w++;
            if (500 < fertileGeologyLocationCount) w++;
            if (10 < procentFertile) w++;
            if (30 < procentFertile) w++;
            if (300 < poorGeologyLocationCount) w--;
            if (20 < procentPoor) w--;
            if (50 < procentPoor) w--;
            if (140 < procentSize) w++;
            if (procentSize < 60) w--;
            if (w <= -3) return RegionWealth.Destitude;
            if (w < -1) return RegionWealth.Poor;
            if (w <= 1) return RegionWealth.Average;
            if (w <= 4) return RegionWealth.Prospering;
            return RegionWealth.Opulent;
        }

        /// <summary>
        /// Make Points for locations, One in each 1000 x 1000 box (pointBoxSize x pointBoxSize)
        /// </summary>
        public static List<Point> MakePoints(long minLongitude, long maxLongitude, long minLatitude, long maxLatitude)
        {
            List<Box> boxes = GetBoxes(minLongitude, maxLongitude, minLatitude, maxLatitude, Box.pointBoxSize, false);

            var points = new List<Point>();
            foreach (var box in boxes)
                points.Add(MakePoint(box));
            return points;
        }

        /// <summary>
        /// Make a points for a locations, in the 1000 x 1000 box (pointBoxSize x pointBoxSize)
        /// </summary>
        public static Point MakePoint(Box box)
        {
            int boxSeed = box.GetSeed();
            Random random = new(boxSeed);
            random = new(random.Next(100000000) + boxSeed);
            long deltax = random.Next(Box.pointBoxSize);
            long deltay = random.Next(Box.pointBoxSize);
            return new Point(
                box.LowerLeftPoint.Longitude + deltax,
                box.LowerLeftPoint.Latitude + deltay);
        }

        private static List<Box> GetBoxes(long minLongitude, long maxLongitude, long minLatitude, long maxLatitude, long boxSize, bool AddExtra)
        {
            int step = 1;
            if (boxSize == Box.pointBoxSize)
            {
                long esti = (maxLongitude - minLongitude) / boxSize;
                int fact = (int)esti / 350;
                if (fact < 1) fact = 1;
                step = fact;
            }
            List<Box> boxes = [];
            long startI = minLongitude / boxSize;
            long startJ = minLatitude / boxSize;
            long endI = maxLongitude / boxSize;
            long endJ = maxLatitude / boxSize;
            if (AddExtra)
            {
                startI--;
                startJ--;
                endI++;
                endJ++;
            }
            for (var y = startJ; y <= endJ; y += step)
                for (var x = startI; x <= endI; x += step)
                    boxes.Add(new Box(new Point(x * boxSize, y * boxSize), boxSize));
            return boxes;
        }

        public static RaceIds PickMainRace(int seed, Region r)
        {
            if (r.LocationCount < r.GeologiesLocationCount[GeologyId.Dark] * 2)
            {
                if (r.RacialPopulationPotential[RaceIds.Orc] < r.RacialPopulationPotential[RaceIds.Lizard])
                    if (r.RacialPopulationPotential[RaceIds.Human] < r.RacialPopulationPotential[RaceIds.Lizard] * 2)
                        return RaceIds.Lizard;
                    else
                        return RaceIds.Human;
                else if (r.RacialPopulationPotential[RaceIds.Orc] * 2 < r.RacialPopulationPotential[RaceIds.Goblin])
                    if (r.RacialPopulationPotential[RaceIds.Human] < r.RacialPopulationPotential[RaceIds.Goblin] * 2)
                        return RaceIds.Goblin;
                    else
                        return RaceIds.Human;
                else
                if (r.RacialPopulationPotential[RaceIds.Human] < r.RacialPopulationPotential[RaceIds.Orc] * 2)
                    return RaceIds.Orc;
                else
                    return RaceIds.Human;
            }
            else
            {
                var SortetRacialPopulationPotential = r.RacialPopulationPotential.OrderByDescending(x => x.Value);
                return SortetRacialPopulationPotential.First().Key;
            }
        }

        public static void PickMainLocationsInRegion(this Region r, int seed)
        {
            Random rand = new(seed);
            Location? capital = null;
            Location? second = null;
            // Pick main race capital and second city
            if (0 < r.MainRaceId && r.RacialTopLocations.ContainsKey(r.MainRaceId))
            {
                double minDist2ToCenter = double.MaxValue;
                double maxDist2ToCenter = double.MinValue;
                foreach (var l in r.RacialTopLocations[r.MainRaceId])
                {
                    var dist2ToCenter = l.Point.Dist2(r.Center);
                    if (dist2ToCenter < minDist2ToCenter)
                    {
                        minDist2ToCenter = dist2ToCenter;
                        capital = l;
                        capital.Population *= 3;
                        Console.WriteLine($"Capital found ({r.MainRaceId}). poppulation:{capital.Population}");
                    }
                    else if (maxDist2ToCenter < dist2ToCenter)
                    {
                        maxDist2ToCenter = dist2ToCenter;
                        second = l;
                        second.Population *= 2;
                        Console.WriteLine($"Second city found ({r.MainRaceId}). poppulation:{second.Population}");
                    }
                }
            }
            // Pick other races strongholds
            foreach (var rr in r.RacialTopLocations.Keys)
            {
                if (0 < r.MainRaceId && rr == r.MainRaceId)
                {
                    // skip
                }
                else
                {
                    int count = r.RacialTopLocations[rr].Count;
                    if (0 < count)
                    {
                        int pick = rand.Next(count);
                        var stronghold = r.RacialTopLocations[rr][pick];
                        stronghold.Population = stronghold.RacialPopulationPotention[rr];
                        if (9 < stronghold.Population)
                        {
                            r.RacialStrongholdPoints.Add(rr, stronghold.Point);
                            stronghold.LocationTypeKey = LocationTypes.StrongholdCenter;
                            r.RacialStrongholdRequests.Add(rr, [stronghold]);
                            Console.WriteLine($"{rr} Stronghold found. poppulation:{stronghold.Population}");
                        }
                        else
                        {
                            // skip this race stronghold in this region - it is to small/week   
                            Console.WriteLine($"Skipping {rr} stronghold in this region. poppulation:{stronghold.Population}");
                        }
                    }
                }
            }
            r.CapitalPoint = capital?.Point;
            r.SecondaryCityPoint = second?.Point;
        }

        public static List<Race> PickSupportingRaces(int seed, Region r)
        {
            _ = seed;
            if (r.MainRaceId == RaceIds.None)
                return [];
            Dictionary<RaceIds, int> candidates = [];
            if (r.MainRaceId == RaceIds.Human)//Human
            {
                if (r.LocationCount < r.GeologiesLocationCount[GeologyId.Dark] * 2)
                {
                    if (r.RacialPopulationPotential.ContainsKey(RaceIds.Orc))//Orc
                        candidates.Add(RaceIds.Orc, r.RacialPopulationPotential[RaceIds.Orc]);
                    if (r.RacialPopulationPotential.ContainsKey(RaceIds.Goblin))//Goblin
                        candidates.Add(RaceIds.Goblin, r.RacialPopulationPotential[RaceIds.Goblin]);
                    if (r.RacialPopulationPotential.ContainsKey(RaceIds.Lizard))//Lizard
                        candidates.Add(RaceIds.Lizard, r.RacialPopulationPotential[RaceIds.Lizard]);
                    if (r.RacialPopulationPotential.ContainsKey(RaceIds.Minotaur))//Minotaur
                        candidates.Add(RaceIds.Minotaur, r.RacialPopulationPotential[RaceIds.Minotaur] * 2);
                }
                else
                {
                    if (r.RacialPopulationPotential.ContainsKey(RaceIds.Elve))//Elves
                        candidates.Add(RaceIds.Elve, r.RacialPopulationPotential[RaceIds.Elve]);
                    if (r.RacialPopulationPotential.ContainsKey(RaceIds.Dwarf))//Dwarf
                        candidates.Add(RaceIds.Dwarf, r.RacialPopulationPotential[RaceIds.Dwarf]);
                    if (r.RacialPopulationPotential.ContainsKey(RaceIds.Haflings))//Haflings
                        candidates.Add(RaceIds.Haflings, r.RacialPopulationPotential[RaceIds.Haflings]);
                    if (r.RacialPopulationPotential.ContainsKey(RaceIds.Hare))//Hare
                        candidates.Add(RaceIds.Hare, r.RacialPopulationPotential[RaceIds.Hare]);
                }
            }
            else if (r.MainRaceId == RaceIds.Elve)//Elves
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Human))//Human
                    candidates.Add(RaceIds.Human, r.RacialPopulationPotential[RaceIds.Human]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Haflings))//Haflings
                    candidates.Add(RaceIds.Haflings, r.RacialPopulationPotential[RaceIds.Haflings]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Hare))//Hare
                    candidates.Add(RaceIds.Hare, r.RacialPopulationPotential[RaceIds.Hare]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Ent))//Ents
                    candidates.Add(RaceIds.Ent, r.RacialPopulationPotential[RaceIds.Ent] * 3);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Fae))//Fae
                    candidates.Add(RaceIds.Fae, r.RacialPopulationPotential[RaceIds.Fae]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Centaur))//Centaur
                    candidates.Add(RaceIds.Centaur, r.RacialPopulationPotential[RaceIds.Centaur]);
            }
            else if (r.MainRaceId == RaceIds.Dwarf)//Dwarf
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Human))//Human
                    candidates.Add(RaceIds.Human, r.RacialPopulationPotential[RaceIds.Human]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Haflings))//Haflings
                    candidates.Add(RaceIds.Haflings, r.RacialPopulationPotential[RaceIds.Haflings]);
            }
            else if (r.MainRaceId == RaceIds.Orc)//Orc
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Goblin))//Goblin
                    candidates.Add(RaceIds.Goblin, r.RacialPopulationPotential[RaceIds.Goblin]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Lizard))//Lizard
                    candidates.Add(RaceIds.Lizard, r.RacialPopulationPotential[RaceIds.Lizard]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Minotaur))//Minotaur
                    candidates.Add(RaceIds.Minotaur, r.RacialPopulationPotential[RaceIds.Minotaur] * 2);
            }
            else if (r.MainRaceId == RaceIds.Goblin)//Goblin
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Orc))//Orc
                    candidates.Add(RaceIds.Orc, r.RacialPopulationPotential[RaceIds.Orc]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Lizard))//Lizard
                    candidates.Add(RaceIds.Lizard, r.RacialPopulationPotential[RaceIds.Lizard]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Minotaur))//Minotaur
                    candidates.Add(RaceIds.Minotaur, r.RacialPopulationPotential[RaceIds.Minotaur] * 2);
            }
            else if (r.MainRaceId == RaceIds.Haflings)//Hafling
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Human))//Human
                    candidates.Add(RaceIds.Human, r.RacialPopulationPotential[RaceIds.Human]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Elve))//Elves
                    candidates.Add(RaceIds.Elve, r.RacialPopulationPotential[RaceIds.Elve]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Hare))//Hare
                    candidates.Add(RaceIds.Hare, r.RacialPopulationPotential[RaceIds.Hare]);
            }
            else if (r.MainRaceId == RaceIds.Lizard)//Lizard
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Goblin))//Goblin
                    candidates.Add(RaceIds.Goblin, r.RacialPopulationPotential[RaceIds.Goblin]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Orc))//Orc
                    candidates.Add(RaceIds.Orc, r.RacialPopulationPotential[RaceIds.Orc]);
            }
            else if (r.MainRaceId == RaceIds.Hare)//Hare
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Elve))//Elves
                    candidates.Add(RaceIds.Elve, r.RacialPopulationPotential[RaceIds.Elve]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Haflings))//Haflings
                    candidates.Add(RaceIds.Haflings, r.RacialPopulationPotential[RaceIds.Haflings]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Ent))//Ents
                    candidates.Add(RaceIds.Ent, r.RacialPopulationPotential[RaceIds.Ent] * 3);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Fae))//Fae
                    candidates.Add(RaceIds.Fae, r.RacialPopulationPotential[RaceIds.Fae]);
            }
            else if (r.MainRaceId == RaceIds.Centaur)//Centaur
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Elve))//Elves
                    candidates.Add(RaceIds.Elve, r.RacialPopulationPotential[RaceIds.Elve]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Haflings))//Haflings
                    candidates.Add(RaceIds.Haflings, r.RacialPopulationPotential[RaceIds.Haflings]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Ent))//Ents
                    candidates.Add(RaceIds.Ent, r.RacialPopulationPotential[RaceIds.Ent] * 3);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Fae))//Fae
                    candidates.Add(RaceIds.Fae, r.RacialPopulationPotential[RaceIds.Fae]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Hare))//Hare
                    candidates.Add(RaceIds.Hare, r.RacialPopulationPotential[RaceIds.Hare]);
            }
            else if (r.MainRaceId == RaceIds.Ent)//Ent
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Elve))//Elves
                    candidates.Add(RaceIds.Elve, r.RacialPopulationPotential[RaceIds.Elve]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Fae))//Fae
                    candidates.Add(RaceIds.Fae, r.RacialPopulationPotential[RaceIds.Fae]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Hare))//Hare
                    candidates.Add(RaceIds.Hare, r.RacialPopulationPotential[RaceIds.Hare]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Centaur))//Centaur
                    candidates.Add(RaceIds.Centaur, r.RacialPopulationPotential[RaceIds.Centaur]);
            }
            else if (r.MainRaceId == RaceIds.Minotaur)//Minotaur
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Goblin))//Goblin
                    candidates.Add(RaceIds.Goblin, r.RacialPopulationPotential[RaceIds.Goblin]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Orc))//Orc
                    candidates.Add(RaceIds.Orc, r.RacialPopulationPotential[RaceIds.Orc]);
            }
            else if (r.MainRaceId == RaceIds.Satyr)//Satyr
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Fae))//Fae
                    candidates.Add(RaceIds.Fae, r.RacialPopulationPotential[RaceIds.Fae]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Nymph))//Nymph
                    candidates.Add(RaceIds.Nymph, r.RacialPopulationPotential[RaceIds.Nymph]);

            }
            else if (r.MainRaceId == RaceIds.Fae)//Fae
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Satyr))//Satyr
                    candidates.Add(RaceIds.Satyr, r.RacialPopulationPotential[RaceIds.Satyr]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Nymph))//Nymph
                    candidates.Add(RaceIds.Nymph, r.RacialPopulationPotential[RaceIds.Nymph]);
            }
            else if (r.MainRaceId == RaceIds.Nymph)//Nymph
            {
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Satyr))//Satyr
                    candidates.Add(RaceIds.Satyr, r.RacialPopulationPotential[RaceIds.Satyr]);
                if (r.RacialPopulationPotential.ContainsKey(RaceIds.Fae))//Fae
                    candidates.Add(RaceIds.Fae, r.RacialPopulationPotential[RaceIds.Fae]);
            }
            if (candidates.Count < 1)
                return [];
            else
            {
                var sorted = candidates.OrderByDescending(x => x.Value);
                int i = 0;
                List<Race> result = [];
                foreach (var can in sorted)
                {
                    result.Add(World.Races[can.Key]);
                    i++;
                    if (2 < i) break;
                }
                return result;
            }
        }

        public static List<Race> PickOpposingRaces(int seed, Region r)
        {
            _ = seed;
            _ = r;
            return []; // None as default
        }
    }
#pragma warning restore CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
}