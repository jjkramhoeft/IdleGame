using System.Drawing;
using Generator;
using Model;

namespace DebugMapGenerator;

#pragma warning disable CA1416 // Validate platform compatibility
public static class MapPainter
{
    /// can be seen in 3d at https://www.procgenesis.com/SimpleHMV/simplehmv.html
    public static MemoryStream GetPerlinNoiseTest(
        long minLongitude, int pixelWidth, long maxLongitude,
        long minLattitude, int pixelHeight, long maxLattitude,
        int outtype,
        float l0amp, int l0gridsize,
        float l1amp, int l1gridsize,
        float l2amp, int l2gridsize
        )
    {
        bool useWarp = true;
        long warpLongitudeDelta = (maxLongitude - minLongitude) / 50;
        long warpLatitudeDelta = -(maxLattitude - minLattitude) / 120;
        float warpFactor = 600f;

        using Bitmap bitmap = new(pixelWidth, pixelHeight);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            var start = DateTime.Now;
            for (int pixelX = 0; pixelX < pixelWidth; pixelX++)//west to east - left to right
            {
                for (int pixelY = 0; pixelY < pixelHeight; pixelY++)// north to south - top to buttom
                {
                    double longitude = minLongitude + 1.0 * (maxLongitude - minLongitude) * pixelX / (pixelWidth * 1.0);
                    double latitude = minLattitude + 1.0 * (maxLattitude - minLattitude) * pixelY / (pixelHeight * 1.0);
                    float n = 0;
                    float dx = 0;
                    float dy = 0;
                    if (l0amp != 0)
                    {
                        if (useWarp)
                        {
                            var nlongi = Generator.PerlinNoise.GetNoise((long)longitude, (long)latitude, l0gridsize, 0);
                            var nlati = Generator.PerlinNoise.GetNoise((long)longitude + warpLongitudeDelta, (long)latitude + warpLatitudeDelta, l0gridsize, 0);

                            var n0 = Generator.PerlinNoise.GetNoise(
                                (long)longitude + (long)(warpFactor * nlongi[0]),
                                (long)latitude + (long)(warpFactor * nlati[0]), l0gridsize, 0);
                            n = n0[0] * l0amp;
                        }
                        else
                        {
                            var n0 = Generator.PerlinNoise.GetNoise((long)longitude, (long)latitude, l0gridsize, 0);
                            n = n0[0] * l0amp;
                            dx = n0[1] * l0amp;
                            dy = n0[2] * l0amp;
                        }
                    }
                    if (l1amp != 0)
                    {
                        var n1 = Generator.PerlinNoise.GetNoise((long)longitude, (long)latitude, l1gridsize, 0);
                        n += n1[0] * l1amp;
                        dx += n1[1] * l1amp;
                        dy += n1[2] * l1amp;
                    }
                    if (l2amp != 0)
                    {
                        var n2 = Generator.PerlinNoise.GetNoise((long)longitude, (long)latitude, l2gridsize, 0);
                        n += n2[0] * l2amp;
                        dx += n2[1] * l2amp;
                        dy += n2[2] * l2amp;
                    }
                    Brush brush = new SolidBrush(GetColor([n, dx, dy], outtype));
                    g.FillRectangle(brush, pixelX, pixelHeight - pixelY - 1, 1, 1);
                }
            }
            var dur = DateTime.Now - start;
        }
        MemoryStream ms = new();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms;
    }

    public enum MapsType
    {
        Height = 1,
        HeightSteppedLegend,
        TemperatureSteppedLegend,
        GeologicType,
        Precipitation,
        Region,
        AllRaces,
        Bioms,
        NewAndOld,
        StormFreq,
        WindDirection,
        Slope,
        ThreeRaces,
        ThreeBioms,
    }

    private static Location[,]? locations;
    private static int[,]? populationPotential;
    private static float[,]? windDirection;
    private static (float, float)[,]? slope;
    private static Dictionary<RaceIds, int>[,]? racePopulationPotential;
    private static int[,]? regionId;
    private static RaceIds raceAId = RaceIds.Human;
    private static RaceIds raceBId = RaceIds.Elve;
    private static RaceIds raceCId = RaceIds.Dwarf;
    private static BiomIds biomAId = BiomIds.Desert;
    private static BiomIds biomBId = BiomIds.BareRock;
    private static BiomIds biomCId = BiomIds.Tundra;
    public static MemoryStream GetDebugMaps(
        long centerLongitude,
        long centerLattitude,
        long longitudeWidth,
        int pixelMapWidth,
        RaceIds raceA, RaceIds raceB, RaceIds raceC,
        BiomIds biomA, BiomIds biomB, BiomIds biomC)
    {
        raceAId = raceA;
        raceBId = raceB;
        raceCId = raceC;
        biomAId = biomA;
        biomBId = biomB;
        biomCId = biomC;
        string raceAText = "-";
        string raceBText = "-";
        string raceCText = "-";
        string biomAText = "-";
        string biomBText = "-";
        string biomCText = "-";
        if (0 < raceAId) raceAText = World.Races[raceAId].Name;
        if (0 < raceBId) raceBText = World.Races[raceBId].Name;
        if (0 < raceCId) raceCText = World.Races[raceCId].Name;
        if (0 < biomAId) biomAText = World.Bioms[biomAId].Name;
        if (0 < biomBId) biomBText = World.Bioms[biomBId].Name;
        if (0 < biomCId) biomCText = World.Bioms[biomCId].Name;
        int margin = 20;
        int legendWidth = 30;
        int pageWidth = (legendWidth + 2 * margin + pixelMapWidth) * 4;
        int pageHeight = (pixelMapWidth + 2 * margin) * 3;
        locations = new Location[pixelMapWidth, pixelMapWidth];
        populationPotential = new int[pixelMapWidth, pixelMapWidth];
        racePopulationPotential = new Dictionary<RaceIds, int>[pixelMapWidth, pixelMapWidth];
        windDirection = new float[pixelMapWidth, pixelMapWidth];
        slope = new (float, float)[pixelMapWidth, pixelMapWidth];
        List<Model.Region> regions = MapGenerator.MakeRegions(
            centerLongitude - longitudeWidth / 2 - Model.Box.regionBoxSize, centerLongitude + longitudeWidth / 2 + Model.Box.regionBoxSize,
            centerLattitude - longitudeWidth / 2 - Model.Box.regionBoxSize, centerLattitude + longitudeWidth / 2 + Model.Box.regionBoxSize);
        regionId = new int[pixelMapWidth, pixelMapWidth];
        int seed = 5;
        for (int x = 0; x < pixelMapWidth; x++)
            for (int y = 0; y < pixelMapWidth; y++)
            {
                Model.Point p = new(
                    centerLongitude + (x - pixelMapWidth / 2) * longitudeWidth / pixelMapWidth,
                    centerLattitude + (y - pixelMapWidth / 2) * longitudeWidth / pixelMapWidth);
                Location l = MapGenerator.CalculateBase(p, seed);
                _ = l.CalculateBioms(x + y * pixelMapWidth);
                locations[x, y] = l;
                windDirection[x, y] = MapGenerator.GetWind(p, seed);
                float[] h3 = MapGenerator.GetHeight(p, seed);
                slope[x, y] = (h3[1], h3[2]);
                populationPotential[x, y] = l.CalculatePopulationPotential();
                if (0 < populationPotential[x, y])
                {
                    int racePopPotSum = 0;
                    racePopulationPotential[x, y] = [];
                    foreach (var race in World.Races)
                    {
                        int racePopPot = 0;
                        if (race.Value.Filter is null || l.Match(race.Value.Filter))
                        {
                            if (0 < l.BiomId)
                            {
                                racePopPot = MapGenerator.CalcRacialPopulationPotential(
                                        race.Value, populationPotential[x, y], l.BiomId, l.GeologyId);
                                racePopPotSum += racePopPot;
                            }
                            racePopulationPotential[x, y].Add(race.Key, racePopPot);
                        }
                    }
                    racePopulationPotential[x, y].Add(0, racePopPotSum);
                }
            }
        using Bitmap bitmap = new(pageWidth, pageHeight);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            Font font = new(FontFamily.GenericSansSerif, 7f);
            List<PagePlace> ppMaps = [];
            List<PagePlace> ppLegends = [];
            List<PagePlace> ppText = [];
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 4; x++)
                {
                    ppMaps.Add(new(legendWidth + x * pageWidth / 4, margin + y * pageHeight / 3));
                    ppLegends.Add(new(-legendWidth + legendWidth + x * pageWidth / 4, +y * pageHeight / 3));
                    ppText.Add(new(x * pageWidth / 4, pageHeight - (margin * 2 + y * pageHeight / 3)));
                }
            WriteTitle(g, "Geology", ppText[0], font, pixelMapWidth);
            WriteTitle(g, "Wind Direction", ppText[1], font, pixelMapWidth);
            WriteTitle(g, "Temp", ppText[2], font, pixelMapWidth);
            WriteTitle(g, "Precipitation", ppText[3], font, pixelMapWidth);
            WriteTitle(g, "Storm Frequency", ppText[4], font, pixelMapWidth);
            WriteTitle(g, "New & Old", ppText[5], font, pixelMapWidth);
            WriteTitle(g, "Height", ppText[6], font, pixelMapWidth);
            WriteTitle(g, "Bioms", ppText[7], font, pixelMapWidth);
            WriteTitle(g, "Slope", ppText[8], font, pixelMapWidth);
            WriteTitle(g, "All Races", ppText[9], font, pixelMapWidth);
            WriteTitle(g, $"Races Red:{raceAText}, Green:{raceBText}, Blue:{raceCText}", ppText[10], font, pixelMapWidth);
            WriteTitle(g, $"Bioms {biomAText}, {biomBText}, {biomCText}", ppText[11], font, pixelMapWidth);

            g.MultiplyTransform(new(1, 0, 0, -1, 0, pageHeight - 1));

            DrawLegend(g, ppLegends[0], pixelMapWidth, MapType.GeologicType, true);
            DrawLegend(g, ppLegends[1], pixelMapWidth, MapType.WindDirection, true);
            DrawLegend(g, ppLegends[2], pixelMapWidth, MapType.BaseTemperature, true);
            DrawLegend(g, ppLegends[3], pixelMapWidth, MapType.BasePrecipitation, true);
            DrawLegend(g, ppLegends[4], pixelMapWidth, MapType.StormFreq, true);
            DrawLegend(g, ppLegends[6], pixelMapWidth, MapType.Height, true);
            DrawLegend(g, ppLegends[7], pixelMapWidth, MapType.Bioms, true);
            DrawLegend(g, ppLegends[8], pixelMapWidth, MapType.Slope, true);
            DrawRacesLegend(g, ppLegends[10], pixelMapWidth);

            DrawMaps(g, ppMaps[0], MapsType.GeologicType, pixelMapWidth);
            DrawMaps(g, ppMaps[1], MapsType.WindDirection, pixelMapWidth);
            DrawMaps(g, ppMaps[2], MapsType.TemperatureSteppedLegend, pixelMapWidth);
            DrawMaps(g, ppMaps[3], MapsType.Precipitation, pixelMapWidth);
            DrawMaps(g, ppMaps[4], MapsType.StormFreq, pixelMapWidth);
            DrawMaps(g, ppMaps[5], MapsType.NewAndOld, pixelMapWidth);
            DrawMaps(g, ppMaps[6], MapsType.HeightSteppedLegend, pixelMapWidth);
            DrawMaps(g, ppMaps[7], MapsType.Bioms, pixelMapWidth);
            DrawMaps(g, ppMaps[8], MapsType.Slope, pixelMapWidth);
            DrawMaps(g, ppMaps[9], MapsType.AllRaces, pixelMapWidth);
            DrawMaps(g, ppMaps[10], MapsType.ThreeRaces, pixelMapWidth);
            DrawMaps(g, ppMaps[11], MapsType.ThreeBioms, pixelMapWidth);
        }
        MemoryStream ms = new();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms;
    }

    public static MemoryStream GetMap(
        long centerLongitude,
        long centerLatitude,
        long longitudeWidth,
        int pixelMapWidth,
        int worldSeed,
        List<Model.Point> history,
        List<Model.Point> posibilities)
    {
        float[,] mapHeight = new float[pixelMapWidth, pixelMapWidth];
        for (int x = 0; x < pixelMapWidth; x++)
            for (int y = 0; y < pixelMapWidth; y++)
            {
                Model.Point p = new(
                    centerLongitude + (x - pixelMapWidth / 2) * longitudeWidth / pixelMapWidth,
                    centerLatitude + (y - pixelMapWidth / 2) * longitudeWidth / pixelMapWidth);
                float[] h3 = MapGenerator.GetHeight(p, worldSeed);
                mapHeight[x, y] = h3[0];
            }
        using Bitmap bitmap = new(pixelMapWidth, pixelMapWidth);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            g.MultiplyTransform(new(1, 0, 0, -1, 0, pixelMapWidth));

            // Height
            for (int x = 0; x < pixelMapWidth; x++)
                for (int y = 0; y < pixelMapWidth; y++)
                {
                    Color c = GetMapHeightColor(mapHeight[x, y], true);
                    Brush mapB = new SolidBrush(c);
                    g.FillRectangle(mapB, x, y, 1, 1);
                }

            // player travel posibilities
            Brush playerB = new SolidBrush(Color.Gray);
            foreach (var pp in posibilities)
            {
                int x = ToX(pp.Longitude, pixelMapWidth, longitudeWidth, centerLongitude);
                int y = ToX(pp.Latitude, pixelMapWidth, longitudeWidth, centerLatitude);
                g.FillRectangle(playerB, x - 3, y - 3, 6, 6);
            }
            // player history
            playerB = new SolidBrush(Color.Pink);
            foreach (var hp in history)
            {
                int x = ToX(hp.Longitude, pixelMapWidth, longitudeWidth, centerLongitude);
                int y = ToX(hp.Latitude, pixelMapWidth, longitudeWidth, centerLatitude);
                g.FillRectangle(playerB, x - 3, y - 3, 6, 6);
            }
            // current player location
            playerB = new SolidBrush(Color.HotPink);
            g.FillRectangle(playerB, pixelMapWidth / 2 - 3, pixelMapWidth / 2 - 3, 6, 6);
        }
        MemoryStream ms = new();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms;
    }

    public static MemoryStream GetRegionMap(
        long centerLongitude,
        long centerLatitude,
        long longitudeWidth,
        int pixelMapWidth,
        int worldSeed,
        Model.Point? capital,
        Model.Point? secondCity,
        Model.Point? lifeTree,
        List<Model.Point> strongholds,
        List<Model.Point> playerHistory,
        List<Model.Point> playerPosibilities,
        int colorType)
    {
        int x = 0;
        int y = 0;
        float[,] mapHeight = new float[pixelMapWidth, pixelMapWidth];
        BiomIds[,] mapBiom = new BiomIds[pixelMapWidth, pixelMapWidth];
        for (x = 0; x < pixelMapWidth; x++)
            for (y = 0; y < pixelMapWidth; y++)
            {
                Model.Point p = new(
                    centerLongitude + (x - pixelMapWidth / 2) * longitudeWidth / pixelMapWidth,
                    centerLatitude + (y - pixelMapWidth / 2) * longitudeWidth / pixelMapWidth);
                float[] h3 = MapGenerator.GetHeight(p, worldSeed);
                mapHeight[x, y] = h3[0];
                if (colorType != 1)
                {
                    var l = MapGenerator.CalculateBase(p, worldSeed);
                    Box b = new(p, Box.pointBoxSize);
                    if (l.CalculateBioms(b.GetSeed()))
                        mapBiom[x, y] = l.BiomId;
                }
            }
        using Bitmap bitmap = new(pixelMapWidth, pixelMapWidth);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            g.MultiplyTransform(new(1, 0, 0, -1, 0, pixelMapWidth));

            if (colorType == 1)
            {
                // Height
                for (x = 0; x < pixelMapWidth; x++)
                    for (y = 0; y < pixelMapWidth; y++)
                    {
                        Color c = GetMapHeightColor(mapHeight[x, y], true);
                        Brush mapB = new SolidBrush(c);
                        g.FillRectangle(mapB, x, y, 1, 1);
                    }
            }
            else
            {
                // biom
                for (x = 0; x < pixelMapWidth; x++)
                    for (y = 0; y < pixelMapWidth; y++)
                    {
                        g.FillRectangle(PickBiomColor(mapBiom[x, y]), x, y, 1, 1);
                    }
            }
            // Cities
            Brush cityB = new SolidBrush(Color.Black);
            Brush cityBinner = new SolidBrush(Color.Gold);
            if (capital is not null)
            {
                x = ToX(capital.Longitude, pixelMapWidth, longitudeWidth, centerLongitude);
                y = ToX(capital.Latitude, pixelMapWidth, longitudeWidth, centerLatitude);
                g.FillRectangle(cityB, x - 5, y - 5, 10, 10);
                g.FillRectangle(cityBinner, x - 4, y - 4, 8, 8);
            }
            if (secondCity is not null)
            {
                x = ToX(secondCity.Longitude, pixelMapWidth, longitudeWidth, centerLongitude);
                y = ToX(secondCity.Latitude, pixelMapWidth, longitudeWidth, centerLatitude);
                g.FillRectangle(cityB, x - 4, y - 4, 8, 8);
                g.FillRectangle(cityBinner, x - 3, y - 3, 6, 6);
            }
            //lifeTree
            if (lifeTree is not null)
            {
                x = ToX(lifeTree.Longitude, pixelMapWidth, longitudeWidth, centerLongitude);
                y = ToX(lifeTree.Latitude, pixelMapWidth, longitudeWidth, centerLatitude);
                g.FillRectangle(new SolidBrush(Color.Black), x - 5, y - 5, 10, 10);
                g.FillRectangle(new SolidBrush(Color.Green), x - 4, y - 4, 8, 8);
            }
            // strongholds
            cityB = new SolidBrush(Color.DarkGray);
            cityBinner = new SolidBrush(Color.LightGray);
            foreach (var pp in strongholds)
            {
                x = ToX(pp.Longitude, pixelMapWidth, longitudeWidth, centerLongitude);
                y = ToX(pp.Latitude, pixelMapWidth, longitudeWidth, centerLatitude);
                g.FillRectangle(cityB, x - 4, y - 4, 8, 8);
                g.FillRectangle(cityBinner, x - 3, y - 3, 6, 6);
            }

            // player travel posibilities
            Brush playerB = new SolidBrush(Color.Gray);
            foreach (var pp in playerPosibilities)
            {
                x = ToX(pp.Longitude, pixelMapWidth, longitudeWidth, centerLongitude);
                y = ToX(pp.Latitude, pixelMapWidth, longitudeWidth, centerLatitude);
                g.FillRectangle(playerB, x - 3, y - 3, 6, 6);
            }
            // player history - current player location is first
            playerB = new SolidBrush(Color.HotPink);
            foreach (var hp in playerHistory)
            {
                x = ToX(hp.Longitude, pixelMapWidth, longitudeWidth, centerLongitude);
                y = ToX(hp.Latitude, pixelMapWidth, longitudeWidth, centerLatitude);
                g.FillRectangle(playerB, x - 3, y - 3, 6, 6);
                playerB = new SolidBrush(Color.Pink);
            }
        }
        MemoryStream ms = new();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms;
    }

    private static int ToX(long worldLongi, int pixelMapWidth, long longitudeWidth, long centerLongitude)
    {
        var x = (worldLongi - (double)centerLongitude) / (longitudeWidth / 2.0) * (pixelMapWidth / 2.0);
        return (int)x + pixelMapWidth / 2;
    }

    private static void WriteTitle(Graphics g, string titleText, PagePlace pagePlace, Font font, int pixelWidth)
    {
        g.DrawString(titleText, font, new SolidBrush(Color.DarkGray), pagePlace.X, pagePlace.Y - pixelWidth);
    }

    private static void DrawMaps(Graphics g, PagePlace pp, MapsType mapsType, int pixelWidth)
    {
        if (locations is null)
            return;
        Color c = Color.Gray;
        for (int i = 0; i < pixelWidth; i++)
        {
            for (int j = 0; j < pixelWidth; j++)
            {
                if (mapsType == MapsType.Height)
                    c = GetMapHeightColor((float)locations[i, j].Height, false);
                else if (mapsType == MapsType.HeightSteppedLegend)
                    c = GetMapHeightColor((float)locations[i, j].Height, true);
                else if (mapsType == MapsType.TemperatureSteppedLegend)
                    c = GetTemperatureColor((float)locations[i, j].Climate!.AverageTemperature, true);
                else if (mapsType == MapsType.GeologicType)
                    c = GetMapXColor((int)locations[i, j].GeologyId);
                else if (mapsType == MapsType.Precipitation)
                    c = GetPrecipitationColor((float)locations[i, j].Climate!.PrecipitationAmount, true);
                else if (mapsType == MapsType.Region)
                {
                    if (regionId is null)
                        return;
                    c = PickColorOf20(regionId[i, j]);
                }
                else if (mapsType == MapsType.AllRaces)
                {
                    if (racePopulationPotential is null)
                        return;
                    if (racePopulationPotential[i, j] is null)
                        c = Color.Black;
                    else
                    {
                        int? allPop = racePopulationPotential[i, j][0];
                        if (allPop is null || allPop < 1)
                            c = Color.Black;
                        else if (allPop < 5)
                            c = Color.DimGray;
                        else if (allPop < 25)
                            c = Color.Silver;
                        else
                            c = Color.White;
                    }
                    if (c.Equals(Color.Black) && locations[i, j].Height <= 0)
                        c = Color.Blue;
                }
                else if (mapsType == MapsType.Bioms)
                {
                    if (locations[i, j].BiomId == 0)
                        c = Color.Red;
                    else
                        c = PickBiomsColor(locations[i, j].BiomId);
                }
                else if (mapsType == MapsType.NewAndOld)
                {
                    if (locations[i, j].Age == -1)
                        c = Color.Brown;
                    else if (locations[i, j].Age == 1)
                        c = Color.SteelBlue;
                    else
                        c = Color.White;
                }
                else if (mapsType == MapsType.StormFreq)
                    c = GetStormColor((float)locations[i, j].Climate!.StormFrequency, true);
                else if (mapsType == MapsType.WindDirection)
                {
                    if (windDirection is null) return;
                    var wd = locations[i, j].Climate!.PredominantWindDirection;
                    c = PickColorWheelOf8(wd!.Id);
                }
                else if (mapsType == MapsType.Slope)
                {
                    c = PickSlopeValueColor((float)locations[i, j].SlopeValue);
                    if (locations[i, j].Height <= 0)
                        c = Color.Blue;
                }
                else if (mapsType == MapsType.ThreeBioms)
                {
                    if (locations[i, j].BiomId == 0)
                        c = Color.Black;
                    else if (
                        locations[i, j].BiomId == biomAId ||
                        locations[i, j].BiomId == biomBId ||
                        locations[i, j].BiomId == biomCId)
                        c = PickBiomsColor(locations[i, j].BiomId);
                    else
                        c = Color.Black;
                }
                else if (mapsType == MapsType.ThreeRaces)
                {
                    var dict = racePopulationPotential![i, j];
                    if (dict is null)
                        c = Color.Black;
                    else
                    {
                        int raceAPopPot = 0;
                        int raceBPopPot = 0;
                        int raceCPopPot = 0;
                        if (dict.ContainsKey(raceAId))
                            raceAPopPot = dict[raceAId];
                        if (dict.ContainsKey(raceBId))
                            raceBPopPot = dict[raceBId];
                        if (dict.ContainsKey(raceCId))
                            raceCPopPot = dict[raceCId];
                        c = PickRacePopColor(raceAPopPot, raceBPopPot, raceCPopPot).Color;
                    }
                }

                Brush b = new SolidBrush(c);
                g.FillRectangle(b, pp.X + i, pp.Y + j, 1, 1);
            }
        }
        //Directions
        int infoSpacing = 10;
        Pen p = new(Color.Black);
        for (int i = 0; i < pixelWidth; i++)
        {
            for (int j = 0; j < pixelWidth; j++)
            {
                if (i % infoSpacing == 0 && j % infoSpacing == 0)
                {
                    if (mapsType == MapsType.WindDirection)
                    {
                        float w = windDirection![i, j];
                        int dx = (int)(0.8f * infoSpacing * Math.Cos(w));
                        int dy = (int)(0.8f * infoSpacing * Math.Sin(w));
                        g.DrawLine(p, pp.X + i, pp.Y + j,
                                    pp.X + i + dx, pp.Y + j + dy);
                    }
                    if (mapsType == MapsType.Slope)
                    {
                        float dx = slope![i, j].Item1 * 4f;
                        float dy = slope![i, j].Item2 * 4f;
                        if (dx > 0.8f) dx = 0.8f;
                        if (dy > 0.8f) dy = 0.8f;
                        g.DrawLine(p, pp.X + i, pp.Y + j,
                                    pp.X + i + dx, pp.Y + j + dy);
                    }
                }
            }
        }
    }

    public static MemoryStream GetDebugMap(
        long minLongitude,
        long minLattitude,
        long longitudeWidth,
        int pixelMapWidth,
        RaceIds raceA,
        RaceIds raceB,
        RaceIds raceC,
        BiomIds biomA,
        BiomIds biomB,
        BiomIds biomC,
        bool steppedLegend,
        bool useBaseValues)
    {
        int margin = 20;
        int legendWidth = 30;
        int pixelWidth = pixelMapWidth;
        int mapAndAllWidth = legendWidth + 2 * margin + pixelWidth;
        int mapAndAllHeight = pixelWidth + 2 * margin;
        int pageWidth = mapAndAllWidth * 4;
        int pageHeight = mapAndAllHeight * 3;
        int textOffset = 2;
        string rA = "-"; if (RaceIds.None != raceA) if (raceA == RaceIds.All) rA = "All"; else rA = World.Races[raceA].Name;
        string rB = "-"; if (RaceIds.None != raceB) if (raceB == RaceIds.All) rB = "All"; else rB = World.Races[raceB].Name;
        string rC = "-"; if (RaceIds.None != raceC) if (raceC == RaceIds.All) rC = "All"; else rC = World.Races[raceC].Name;
        string bA = "-"; if (0 < biomA) bA = World.Bioms[biomA].Name;
        string bB = "-"; if (0 < biomB) bB = World.Bioms[biomB].Name;
        string bC = "-"; if (0 < biomC) bC = World.Bioms[biomC].Name;
        using Bitmap bitmap = new(pageWidth, pageHeight);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);

            Font font = new(FontFamily.GenericSansSerif, 7f);
            g.DrawString("Bioms", font, new SolidBrush(Color.DarkGray), legendWidth, textOffset);
            g.DrawString("Race 1,2,3", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth, textOffset);
            g.DrawString(rA, font, new SolidBrush(Color.Red), (int)(legendWidth + mapAndAllWidth * 1.75), textOffset + 10);
            g.DrawString(rB, font, new SolidBrush(Color.Green), (int)(legendWidth + mapAndAllWidth * 1.75), textOffset + 20);
            g.DrawString(rC, font, new SolidBrush(Color.Blue), (int)(legendWidth + mapAndAllWidth * 1.75), textOffset + 30);
            g.DrawString("Race 4,5,6", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth * 2, textOffset);
            g.DrawString(bA, font, new SolidBrush(Color.Red), (int)(legendWidth + mapAndAllWidth * 1.9), 10 + mapAndAllHeight - 80);
            g.DrawString(bB, font, new SolidBrush(Color.Green), (int)(legendWidth + mapAndAllWidth * 1.9), 20 + mapAndAllHeight - 80);
            g.DrawString(bC, font, new SolidBrush(Color.Blue), (int)(legendWidth + mapAndAllWidth * 1.9), 30 + mapAndAllHeight - 80);
            g.DrawString("New vs. Old", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth * 3, textOffset);
            g.DrawString("Temperature", font, new SolidBrush(Color.DarkGray), legendWidth, textOffset + mapAndAllHeight);
            g.DrawString("Precipitation", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth, textOffset + mapAndAllHeight);
            g.DrawString("Slope", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth * 2, textOffset + mapAndAllHeight);
            g.DrawString("Storm Frequency", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth * 3, textOffset + mapAndAllHeight);

            g.DrawString("Height", font, new SolidBrush(Color.DarkGray), legendWidth, textOffset + mapAndAllHeight * 2);
            g.DrawString("Wind Direction", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth, textOffset + mapAndAllHeight * 2);
            g.DrawString("Geologic Type", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth * 2, textOffset + mapAndAllHeight * 2);
            g.DrawString("Kindoms & Points", font, new SolidBrush(Color.DarkGray), legendWidth + mapAndAllWidth * 3, textOffset + mapAndAllHeight * 2);

            g.DrawString("7Plain", font, new SolidBrush(Color.DarkGray), 20 + mapAndAllWidth * 2, 10 + textOffset + mapAndAllHeight * 2 + 0 * pixelWidth / 8 + pixelWidth / 16);
            g.DrawString("6Poor", font, new SolidBrush(Color.DarkGray), 20 + mapAndAllWidth * 2, 10 + textOffset + mapAndAllHeight * 2 + 1 * pixelWidth / 8 + pixelWidth / 16);
            g.DrawString("5Magic", font, new SolidBrush(Color.DarkGray), 20 + mapAndAllWidth * 2, 10 + textOffset + mapAndAllHeight * 2 + 2 * pixelWidth / 8 + pixelWidth / 16);
            g.DrawString("4Old", font, new SolidBrush(Color.DarkGray), 20 + mapAndAllWidth * 2, 10 + textOffset + mapAndAllHeight * 2 + 3 * pixelWidth / 8 + pixelWidth / 16);
            g.DrawString("3Forest", font, new SolidBrush(Color.DarkGray), 20 + mapAndAllWidth * 2, 10 + textOffset + mapAndAllHeight * 2 + 4 * pixelWidth / 8 + pixelWidth / 16);
            g.DrawString("2Fert", font, new SolidBrush(Color.DarkGray), 20 + mapAndAllWidth * 2, 10 + textOffset + mapAndAllHeight * 2 + 5 * pixelWidth / 8 + pixelWidth / 16);
            g.DrawString("1Dark", font, new SolidBrush(Color.DarkGray), 20 + mapAndAllWidth * 2, 10 + textOffset + mapAndAllHeight * 2 + 6 * pixelWidth / 8 + pixelWidth / 16);
            g.DrawString("0Mineral", font, new SolidBrush(Color.DarkGray), 20 + mapAndAllWidth * 2, 10 + textOffset + mapAndAllHeight * 2 + 7 * pixelWidth / 8 + pixelWidth / 16);

            g.MultiplyTransform(new(1, 0, 0, -1, 0, pageHeight - 1));

            DrawLegend(g, new(0, 0), pixelWidth, MapType.Height, steppedLegend);// 1KM below -> 0 -> 2KM top (ca. -1.0 -> 2.0)
            DrawLegend(g, new(mapAndAllWidth, 0), pixelWidth, MapType.WindDirection, steppedLegend);// 0 -> 2Pi
            DrawLegend(g, new(mapAndAllWidth * 2, 0), pixelWidth, MapType.GeologicType, steppedLegend);// 4 common Fertile,Forrest,Poor,Plains & 4 rare Magic, Geoloic Surplus, Dark
            DrawLegend(g, new(0, mapAndAllHeight), pixelWidth, MapType.BaseTemperature, steppedLegend);//
            DrawLegend(g, new(mapAndAllWidth, mapAndAllHeight), pixelWidth, MapType.BasePrecipitation, steppedLegend);//
            DrawLegend(g, new(0, mapAndAllHeight * 2), pixelWidth, MapType.Bioms, steppedLegend);//
            DrawRacesLegend(g, new((mapAndAllWidth - legendWidth) * 2, (int)(mapAndAllHeight * 2.3)), pixelWidth);//

            List<PagePlace> pps = [
                new (legendWidth, margin),
                new (legendWidth + margin + pixelWidth, margin),
                new (legendWidth + mapAndAllWidth, margin),
                new (legendWidth + margin + pixelWidth + mapAndAllWidth, margin),
                new (legendWidth + mapAndAllWidth*2, margin),
                new (legendWidth + margin + pixelWidth + mapAndAllWidth*2, margin),
                new (legendWidth + mapAndAllWidth*3, margin),
                new (legendWidth + margin + pixelWidth + mapAndAllWidth*3, margin),
                ];
            foreach (var pp in pps)
                DrawVerticalTicks(g, pp.X, pp.Y, pp.X + margin, pp.Y + pixelWidth,
                    pageWidth, pageHeight, minLongitude, minLattitude, longitudeWidth, pixelWidth);

            pps = [
                new (legendWidth + margin, 0),
                new (legendWidth + margin, pixelWidth + margin),
                new (legendWidth + margin + mapAndAllWidth, 0),
                new (legendWidth + margin + mapAndAllWidth, pixelWidth + margin),
                new (legendWidth + margin + mapAndAllWidth*2, 0),
                new (legendWidth + margin + mapAndAllWidth*2, pixelWidth + margin),
                new (legendWidth + margin + mapAndAllWidth*3, 0),
                new (legendWidth + margin + mapAndAllWidth*3, pixelWidth + margin),
                ];
            foreach (var pp in pps)
                DrawHorizontalTicks(g, pp.X, pp.Y, pp.X + pixelWidth, pp.Y + margin,
                    pageWidth, pageHeight, minLongitude, minLattitude, longitudeWidth, pixelWidth);


            DrawMap(g, new(legendWidth + margin + mapAndAllWidth * 3, margin + mapAndAllHeight * 2), MapType.NewAndOld, minLongitude, minLattitude, longitudeWidth, pixelWidth, steppedLegend, useBaseValues);
            DrawMap(g, new(legendWidth + margin + mapAndAllWidth, margin), MapType.WindDirection, minLongitude, minLattitude, longitudeWidth, pixelWidth, steppedLegend, useBaseValues);
            DrawMap(g, new(legendWidth + margin + mapAndAllWidth * 2, margin + mapAndAllHeight), MapType.Slope, minLongitude, minLattitude, longitudeWidth, pixelWidth, steppedLegend, useBaseValues);
            DrawMap(g, new(legendWidth + margin + mapAndAllWidth * 3, margin + mapAndAllHeight), MapType.StormFreq, minLongitude, minLattitude, longitudeWidth, pixelWidth, steppedLegend, useBaseValues);
            DrawMap(g, new(legendWidth + margin, margin), MapType.Height, minLongitude, minLattitude, longitudeWidth, pixelWidth, steppedLegend, useBaseValues);
            DrawMap(g, new(legendWidth + margin + mapAndAllWidth * 2, margin), MapType.GeologicType, minLongitude, minLattitude, longitudeWidth, pixelWidth, steppedLegend, useBaseValues);
            DrawPoints(g, new(legendWidth + margin + mapAndAllWidth * 3, margin), minLongitude, minLattitude, longitudeWidth, pixelWidth);
            DrawBioms(g, new(legendWidth + margin, margin + mapAndAllHeight * 2), minLongitude, minLattitude, longitudeWidth, pixelWidth, new((mapAndAllWidth + legendWidth) * 2, margin + mapAndAllHeight * 2), biomA, biomB, biomC);
            DrawRaces(g, new(mapAndAllWidth, margin + mapAndAllHeight * 2), minLongitude, minLattitude, longitudeWidth, pixelWidth, raceA, raceB, raceC);
            DrawMap(g, new(legendWidth + margin, margin + mapAndAllHeight), MapType.BaseTemperature, minLongitude, minLattitude, longitudeWidth, pixelWidth, steppedLegend, useBaseValues);
            DrawMap(g, new(legendWidth + margin + mapAndAllWidth, margin + mapAndAllHeight), MapType.BasePrecipitation, minLongitude, minLattitude, longitudeWidth, pixelWidth, steppedLegend, useBaseValues);


        }
        MemoryStream ms = new();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms;
    }

    private static void DrawLegend(Graphics g, PagePlace pp, int pixelWidth, MapType mapType, bool steppedLegend)
    {
        float delta = 0f;
        float legendHeight = 0f;
        float minLegendHeight;
        float maxLegendHeight;
        if (mapType == MapType.Height)
        {
            minLegendHeight = Height.Min;
            maxLegendHeight = Height.Max;
            delta = (maxLegendHeight - minLegendHeight) / pixelWidth;
            legendHeight = minLegendHeight;
        }
        else if (mapType == MapType.BaseTemperature)
        {
            minLegendHeight = Temperature.Min;
            maxLegendHeight = Temperature.Max;
            delta = (maxLegendHeight - minLegendHeight) / pixelWidth;
            legendHeight = minLegendHeight;
        }
        else if (mapType == MapType.BasePrecipitation)
        {
            minLegendHeight = Precipitation.Min;
            maxLegendHeight = Precipitation.Max;
            delta = (maxLegendHeight - minLegendHeight) / pixelWidth;
            legendHeight = minLegendHeight;
        }
        else if (mapType == MapType.WindDirection)
        {
            minLegendHeight = -0.5f;
            maxLegendHeight = 6.58f;
            delta = (maxLegendHeight - minLegendHeight) / pixelWidth;
            legendHeight = minLegendHeight;
        }
        else if (mapType == MapType.GeologicType)
        {
            minLegendHeight = 0f;
            maxLegendHeight = 8f;
            delta = (maxLegendHeight - minLegendHeight) / pixelWidth;
            legendHeight = minLegendHeight;
        }
        else if (mapType == MapType.Bioms)
        {
            minLegendHeight = 1.5f;
            maxLegendHeight = 37.5f;
            delta = (maxLegendHeight - minLegendHeight) / pixelWidth;
            legendHeight = minLegendHeight;
        }
        else if (mapType == MapType.Slope)
        {
            minLegendHeight = Slope.Min;
            maxLegendHeight = Slope.Max + 1;
            delta = (maxLegendHeight - minLegendHeight) / pixelWidth;
            legendHeight = minLegendHeight;
        }
        for (int i = 0; i < pixelWidth; i++)
        {
            Color c = Color.White;
            if (mapType == MapType.Height) c = GetMapHeightColor(legendHeight, steppedLegend);
            else if (mapType == MapType.WindDirection) c = PickColorWheelOf8((int)legendHeight);
            else if (mapType == MapType.GeologicType) c = GetMapXColor(legendHeight);
            else if (mapType == MapType.BaseTemperature) c = GetTemperatureColor(legendHeight, steppedLegend);
            else if (mapType == MapType.BasePrecipitation) c = GetPrecipitationColor(legendHeight, steppedLegend);
            else if (mapType == MapType.Bioms) c = PickBiomColor((BiomIds)(int)legendHeight).Color;
            else if (mapType == MapType.Slope) c = PickSlopeValueColor((int)legendHeight);
            Brush b = new SolidBrush(c);
            legendHeight += delta;
            g.FillRectangle(b, pp.X + 5, pp.Y + 20 + i, 15, 1);
        }
    }


    private static void DrawRacesLegend(Graphics gG, PagePlace pp, int pixelWidth)
    {
        int up = pixelWidth / 2;
        int left = pixelWidth / 8;
        float fact = 0.6f;
        Brush z = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
        Brush r = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
        Brush rg = new SolidBrush(Color.FromArgb(255, 255, 255, 0));
        Brush rb = new SolidBrush(Color.FromArgb(255, 255, 0, 255));
        Brush gb = new SolidBrush(Color.FromArgb(255, 0, 255, 255));
        Brush b = new SolidBrush(Color.FromArgb(255, 0, 0, 255));
        Brush g = new SolidBrush(Color.FromArgb(255, 0, 255, 0));

        int h0 = (int)(fact * 0f) + pp.Y + up;
        int h1 = (int)(fact * 22f) + pp.Y + up;
        int h2 = (int)(fact * 39f) + pp.Y + up;
        int h3 = (int)(fact * 56f) + pp.Y + up;
        int h4 = (int)(fact * 73f) + pp.Y + up;
        int h5 = (int)(fact * 90f) + pp.Y + up;
        int h6 = (int)(fact * 111f) + pp.Y + up;

        int w0 = (int)(fact * 0f) + pp.X - left;
        int w1 = (int)(fact * 22f) + pp.X - left;
        int w2 = (int)(fact * 35f) + pp.X - left;
        int w3 = (int)(fact * 48f) + pp.X - left;
        int w4 = (int)(fact * 63f) + pp.X - left;
        int w5 = (int)(fact * 88f) + pp.X - left;

        gG.FillPolygon(r, new System.Drawing.Point[] { new(w0, h6), new(w2, h5), new(w1, h4), new(w0, h4), new(w0, h6) });
        gG.FillPolygon(rg, new System.Drawing.Point[] { new(w1, h4), new(w2, h5), new(w4, h4), new(w3, h3), new(w1, h4) });
        gG.FillPolygon(rb, new System.Drawing.Point[] { new(w0, h4), new(w1, h4), new(w1, h2), new(w0, h2), new(w0, h4) });
        gG.FillPolygon(b, new System.Drawing.Point[] { new(w0, h0), new(w0, h2), new(w1, h2), new(w2, h1), new(w0, h0) });
        gG.FillPolygon(gb, new System.Drawing.Point[] { new(w1, h2), new(w3, h3), new(w4, h2), new(w2, h1), new(w1, h2) });
        gG.FillPolygon(g, new System.Drawing.Point[] { new(w3, h3), new(w4, h4), new(w5, h3), new(w4, h2), new(w3, h3) });
    }


    private static void DrawVerticalTicks(Graphics g, int lowerLeftX, int lowerLeftY, int upperRightX, int upperRightY, int pageWidth, int pageHeight,
        long minLongitude, long minLatitude, long longitudeWidth, int pixelWidth)
    {
        Brush smallTickBrush = new SolidBrush(Color.DarkBlue);
        Brush bigTickBrush = new SolidBrush(Color.Black);
        long maxLatitude = minLatitude + longitudeWidth;// square map!
        long smallBoxSize = Box.pointBoxSize;
        long bigBoxSize = Box.regionBoxSize;
        long countSmallTicks = longitudeWidth / smallBoxSize;// square map!
        long countBigTicks = longitudeWidth / bigBoxSize;// square map!
        if (countSmallTicks * 2.5 < pixelWidth)
        {// draw small
            long i = minLatitude / smallBoxSize * smallBoxSize;
            while (i <= maxLatitude)
            {
                float pixelY = pixelWidth * (i - minLatitude) / (float)(maxLatitude - minLatitude);
                g.FillRectangle(smallTickBrush, lowerLeftX + 10, lowerLeftY + pixelY, 10, 1);
                i += smallBoxSize;
            }
        }
        if (countBigTicks * 2.5 < pixelWidth)
        {// draw small
            long i = minLatitude / bigBoxSize * bigBoxSize;
            while (i <= maxLatitude)
            {
                float pixelY = pixelWidth * (i - minLatitude) / (float)(maxLatitude - minLatitude);
                g.FillRectangle(bigTickBrush, lowerLeftX, lowerLeftY + pixelY, 10, 1);
                i += bigBoxSize;
            }
        }
    }

    private static void DrawHorizontalTicks(Graphics g, int lowerLeftX, int lowerLeftY, int upperRightX, int upperRightY, int pageWidth, int pageHeight,
        long minLongitude, long minLatitude, long longitudeWidth, int pixelWidth)
    {
        Brush smallTickBrush = new SolidBrush(Color.DarkBlue);
        Brush bigTickBrush = new SolidBrush(Color.Black);
        long maxLongitude = minLongitude + longitudeWidth;
        long smallBoxSize = Box.pointBoxSize;
        long bigBoxSize = Box.regionBoxSize;
        long countSmallTicks = longitudeWidth / smallBoxSize;// square map!
        long countBigTicks = longitudeWidth / bigBoxSize;// square map!
        if (countSmallTicks * 2.5 < pixelWidth)
        {// draw small
            long i = minLongitude / smallBoxSize * smallBoxSize;
            while (i <= maxLongitude)
            {
                float pixelX = pixelWidth * (i - minLongitude) / (float)(maxLongitude - minLongitude);
                g.FillRectangle(smallTickBrush, lowerLeftX + pixelX, lowerLeftY + 10, 1, 10);
                i += smallBoxSize;
            }
        }
        if (countBigTicks * 2.5 < pixelWidth)
        {// draw small
            long i = minLongitude / bigBoxSize * bigBoxSize;
            while (i <= maxLongitude)
            {
                float pixelX = pixelWidth * (i - minLongitude) / (float)(maxLongitude - minLongitude);
                g.FillRectangle(bigTickBrush, lowerLeftX + pixelX, lowerLeftY, 1, 10);
                i += bigBoxSize;
            }
        }
    }

    private static void DrawMap(Graphics g, PagePlace pp, MapType mapType, long minLongitude, long minLatitude, long longitudeWidth, int pixelWidth, bool steppedLegend, bool useBaseValues)
    {
        Dictionary<int, int> countingBuckets = [];
        float h = 0f;
        float minH = float.MaxValue;
        float maxH = float.MinValue;
        Color c = Color.White;
        for (int i = 0; i < pixelWidth; i++)
        {
            for (int j = 0; j < pixelWidth; j++)
            {
                long longitude = (long)(minLongitude + longitudeWidth * (i / (double)pixelWidth));
                long latitude = (long)(minLatitude + longitudeWidth * (j / (double)pixelWidth));
                if (mapType == MapType.WindDirection)
                {
                    h = MapGenerator.GetWind(new Model.Point(longitude, latitude), 0);
                    c = GetMapWindColor(h, steppedLegend);
                }
                else if (mapType == MapType.Height || mapType == MapType.Slope)
                {
                    float[] h3 = MapGenerator.GetHeight(new Model.Point(longitude, latitude), 0);
                    h = h3[0];
                    if (mapType == MapType.Slope && h < 0)
                        c = Color.Blue;
                    else
                        c = GetMapHeightColor(h, steppedLegend);
                }
                else if (mapType == MapType.GeologicType)
                {
                    int gt = (int)MapGenerator.GetGeology(new Model.Point(longitude, latitude), 0);
                    if (countingBuckets.ContainsKey(gt))
                        countingBuckets[gt]++;
                    else
                        countingBuckets.Add(gt, 1);
                    c = GetMapXColor(gt);
                }
                else if (mapType == MapType.BaseTemperature)
                {
                    h = MapGenerator.GetBaseTemperature(new Model.Point(longitude, latitude), 0);
                    if (!useBaseValues)
                    {
                        float[] h3 = MapGenerator.GetHeight(new Model.Point(longitude, latitude), 0);
                        h = MapGenerator.CalcTemperature(h3[0], h);
                    }
                    c = GetTemperatureColor(h, steppedLegend);
                }
                else if (mapType == MapType.BasePrecipitation)
                {
                    h = MapGenerator.GetBasePrecipitation(new Model.Point(longitude, latitude), 0);
                    if (!useBaseValues)
                    {
                        var bt = MapGenerator.GetBaseTemperature(new Model.Point(longitude, latitude), 0);
                        float[] h3 = MapGenerator.GetHeight(new Model.Point(longitude, latitude), 0);
                        var t = MapGenerator.CalcTemperature(h3[0], bt);
                        var w = MapGenerator.GetWind(new Model.Point(longitude, latitude), 0);
                        h = MapGenerator.CalcPrecipitation(h, t, w, h3[1], h3[2], h3[0]);
                    }
                    c = GetPrecipitationColor(h, steppedLegend);
                }
                else if (mapType == MapType.StormFreq)
                {
                    h = MapGenerator.GetStormFrequency(new Model.Point(longitude, latitude), 0);
                    c = GetStormColor(h, steppedLegend);
                }
                else if (mapType == MapType.NewAndOld)
                {
                    h = MapGenerator.GetAge(new Model.Point(longitude, latitude), 0);
                    c = GetNewVsOldColor(h);
                }
                if (h < minH) minH = h;
                if (maxH < h) maxH = h;
                Brush b = new SolidBrush(c);
                g.FillRectangle(b, pp.X + i, pp.Y + j, 1, 1);
            }
        }

        int infoSpacing = 10;
        Pen p = new(Color.Black);
        for (int i = 0; i < pixelWidth; i++)
        {
            for (int j = 0; j < pixelWidth; j++)
            {
                long longitude = (long)(minLongitude + longitudeWidth * (i / (double)pixelWidth));
                long latitude = (long)(minLatitude + longitudeWidth * (j / (double)pixelWidth));
                if (mapType == MapType.WindDirection)
                {
                    if (i % infoSpacing == 0 && j % infoSpacing == 0)
                    {
                        h = MapGenerator.GetWind(new Model.Point(longitude, latitude), 0);
                        c = GetMapWindColor(h, steppedLegend);
                        int dx = (int)(0.8f * infoSpacing * Math.Cos(h));
                        int dy = (int)(0.8f * infoSpacing * Math.Sin(h));
                        g.DrawLine(p, pp.X + i, pp.Y + j,
                                    pp.X + i + dx, pp.Y + j + dy);
                    }
                }
                else if (mapType == MapType.Slope)
                {
                    if (i % infoSpacing == 0 && j % infoSpacing == 0)
                    {
                        float[] h3 = MapGenerator.GetHeight(new Model.Point(longitude, latitude), 0);
                        if (0f < h3[0])
                        {
                            float dx = h3[1] * 4f;
                            float dy = h3[2] * 4f;
                            if (dx > 0.8f) dx = 0.8f;
                            if (dy > 0.8f) dy = 0.8f;
                            g.DrawLine(p, pp.X + i, pp.Y + j,
                                        pp.X + i + dx, pp.Y + j + dy);
                        }
                    }
                }
            }
        }
    }

    private static void DrawPoints(Graphics g, PagePlace pp, long minLongitude, long minLatitude, long longitudeWidth, int pixelWidth)
    {
        int pixelHeight = pixelWidth;
        long maxLongitude = minLongitude + longitudeWidth;
        long maxLatitude = minLatitude + longitudeWidth;
        var points = MapGenerator.MakePoints(minLongitude, maxLongitude, minLatitude, maxLatitude);
        var regions = MapGenerator.MakeRegions(minLongitude, maxLongitude, minLatitude, maxLatitude);
        var bs = Box.pointBoxSize;
        int dotSize = 1;
        if (points.Count < 10000)
            dotSize = 2;
        if (points.Count < 1000)
            dotSize = 3;
        Pen box = new(Color.LightGray);
        if ((maxLongitude - minLongitude) < bs * 40)
        {
            foreach (var p in points)// point boxes
            {
                long bx0 = pixelWidth * (((p.Longitude / bs) - 1) * bs - minLongitude) / (maxLongitude - minLongitude);
                long by0 = pixelHeight * (p.Latitude / bs * bs - minLatitude) / (maxLatitude - minLatitude);
                long bxw = pixelWidth * bs / (maxLongitude - minLongitude);
                long byw = pixelWidth * bs / (maxLatitude - minLatitude);
                if (by0 < pixelHeight)
                    g.DrawRectangle(box, pp.X + bx0, pp.Y + by0, bxw, byw);
            }
        }
        foreach (var p in points)
        {
            long pixelX = pixelWidth * (p.Longitude - minLongitude) / (maxLongitude - minLongitude);
            long pixelY = pixelHeight * (p.Latitude - minLatitude) / (maxLatitude - minLatitude);
            if (pixelX < pixelWidth && pixelY < pixelHeight)
            {
                int pointRegionIndex = FindRelatedRegionIndex(p, regions);
                Brush brush = PickRegionColor(pointRegionIndex);
                g.FillRectangle(brush, pp.X + pixelX, pp.Y + pixelY, dotSize, dotSize);
            }
        }
    }

    private static void DrawRaces(Graphics g, PagePlace pp, long minLongitude, long minLatitude, long longitudeWidth, int pixelWidth, RaceIds raceA, RaceIds raceB, RaceIds raceC)
    {
        int pixelHeight = pixelWidth;
        long maxLongitude = minLongitude + longitudeWidth;
        long maxLatitude = minLatitude + longitudeWidth;
        var points = MapGenerator.MakePoints(minLongitude, maxLongitude, minLatitude, maxLatitude);
        var bs = Box.pointBoxSize;

        Brush box = new SolidBrush(Color.Gray);
        Brush missingBox = new SolidBrush(Color.Gray);
        g.FillRectangle(box, pp.X, pp.Y, pixelWidth, pixelWidth);
        foreach (var p in points)// point boxes
        {
            Box locationBox = Box.GetLocationBox(p);

            Location l = MapGenerator.CalculateBase(p, 0);
            long bx0 = pixelWidth * (((locationBox.LowerLeftPoint.Longitude / bs) - 1) * bs - minLongitude) / (maxLongitude - minLongitude);
            long by0 = pixelHeight * (locationBox.LowerLeftPoint.Latitude / bs * bs - minLatitude) / (maxLatitude - minLatitude);
            long bxw = pixelWidth * bs / (maxLongitude - minLongitude) + 1;
            long byw = pixelWidth * bs / (maxLatitude - minLatitude) + 1;
            int seed = 0;
            if (l.CalculateBioms(seed))
            {
                int racePopPotA = 0;
                int racePopPotB = 0;
                int racePopPotC = 0;
                int all = 0;
                int locationPopulationPotential = l.CalculatePopulationPotential();
                if (raceA == RaceIds.All || raceB == RaceIds.All || raceC == RaceIds.All)
                {
                    foreach (var race in World.Races)
                        if (race.Value.Filter is null || l.Match(race.Value.Filter))
                            all += MapGenerator.CalcRacialPopulationPotential(
                                race.Value, locationPopulationPotential, l.BiomId, l.GeologyId);
                }
                if (raceA == RaceIds.All)
                    racePopPotA = all;
                else if (0 < raceA && (World.Races[raceA].Filter is null || l.Match(World.Races[raceA].Filter)))
                    racePopPotA = MapGenerator.CalcRacialPopulationPotential(World.Races[raceA], locationPopulationPotential, l.BiomId, l.GeologyId);
                if (raceB == RaceIds.All)
                    racePopPotB = all;
                else if (0 < raceB && (World.Races[raceB].Filter is null || l.Match(World.Races[raceB].Filter)))
                    racePopPotB = MapGenerator.CalcRacialPopulationPotential(World.Races[raceB], locationPopulationPotential, l.BiomId, l.GeologyId);
                if (raceC == RaceIds.All)
                    racePopPotC = all;
                else if (0 < raceC && (World.Races[raceC].Filter is null || l.Match(World.Races[raceC].Filter)))
                    racePopPotC = MapGenerator.CalcRacialPopulationPotential(World.Races[raceC], locationPopulationPotential, l.BiomId, l.GeologyId);

                box = PickRacePopColor(racePopPotA, racePopPotB, racePopPotC);
            }
            else
                box = missingBox;
            g.FillRectangle(box, pp.X + bx0, pp.Y + by0, bxw, byw);
        }
    }

    private static void DrawBioms(Graphics g, PagePlace pp, long minLongitude, long minLatitude, long longitudeWidth, int pixelWidth, PagePlace pp2, BiomIds biomA, BiomIds biomB, BiomIds biomC)
    {
        int pixelHeight = pixelWidth;
        long maxLongitude = minLongitude + longitudeWidth;
        long maxLatitude = minLatitude + longitudeWidth;
        var points = MapGenerator.MakePoints(minLongitude, maxLongitude, minLatitude, maxLatitude);
        var bs = Box.pointBoxSize;

        Brush box = new SolidBrush(Color.White);
        Brush redBox = new SolidBrush(Color.Red);
        Brush greenBox = new SolidBrush(Color.Green);
        Brush blueBox = new SolidBrush(Color.Blue);
        g.FillRectangle(box, pp.X, pp.Y, pixelWidth, pixelWidth);
        g.FillRectangle(box, pp2.X, pp2.Y, pixelWidth, pixelWidth);
        foreach (var p in points)// point boxes
        {
            Box locationBox = Box.GetLocationBox(p);

            Location l = MapGenerator.CalculateBase(p, 0);
            long bx0 = pixelWidth * (((locationBox.LowerLeftPoint.Longitude / bs) - 1) * bs - minLongitude) / (maxLongitude - minLongitude);
            long by0 = pixelHeight * (locationBox.LowerLeftPoint.Latitude / bs * bs - minLatitude) / (maxLatitude - minLatitude);
            long bxw = pixelWidth * bs / (maxLongitude - minLongitude) + 1;
            long byw = pixelWidth * bs / (maxLatitude - minLatitude) + 1;
            int seed = (int)p.Longitude;
            if (l.CalculateBioms(seed))
                box = PickBiomColor(l.BiomId);
            else
                box = redBox;
            g.FillRectangle(box, pp.X + bx0, pp.Y + by0, bxw, byw);
            if (l.BiomId == biomA)
                g.FillRectangle(redBox, pp2.X + bx0, pp2.Y + by0, bxw, byw);
            if (l.BiomId == biomB)
                g.FillRectangle(greenBox, pp2.X + bx0, pp2.Y + by0, bxw, byw);
            if (l.BiomId == biomC)
                g.FillRectangle(blueBox, pp2.X + bx0, pp2.Y + by0, bxw, byw);
        }
    }


    private static Color PickSlopeValueColor(float slopeValue)
    {
        if (slopeValue < Slope.FlatNormalSplit) return Color.Green;
        if (slopeValue < Slope.NormalSteepSplit) return Color.Gray;
        return Color.Red;
    }

    private static SolidBrush PickBiomColor(BiomIds biomId)
    {
        if (biomId == BiomIds.Desert) return new SolidBrush(Color.SandyBrown);//desert
        if (biomId == BiomIds.BareRock) return new SolidBrush(Color.DarkGray);//BareRock
        if (biomId == BiomIds.Tundra) return new SolidBrush(Color.PaleGreen);//tundra
        if (biomId == BiomIds.Permafrost) return new SolidBrush(Color.LightGray);//permafrost
        if (biomId == BiomIds.MountainTundra) return new SolidBrush(Color.SlateGray);//Mountain Tundra
        if (biomId == BiomIds.Glaciers) return new SolidBrush(Color.LightCyan);//ice Glaciers
        if (biomId == BiomIds.BorealForests) return new SolidBrush(Color.DarkGreen);//Taiga
        if (biomId == BiomIds.TemperateForests) return new SolidBrush(Color.Green);//Temperate Forests
        if (biomId == BiomIds.TemperateConiferousForests) return new SolidBrush(Color.ForestGreen);//Temperate Coniferous Forests
        if (biomId == BiomIds.TemperateRainForests) return new SolidBrush(Color.LawnGreen);//Temperate Rain Forests
        if (biomId == BiomIds.TropicalRainForests) return new SolidBrush(Color.LimeGreen);//Tropical Rain Forests
        if (biomId == BiomIds.TropicalMoistForests) return new SolidBrush(Color.SeaGreen);//Tropical Moist Forests
        if (biomId == BiomIds.TropicalDryForests) return new SolidBrush(Color.YellowGreen);//Tropical Dry Forests
        if (biomId == BiomIds.TropicalCloudForests) return new SolidBrush(Color.Teal);//Tropical Cloud Forests
        if (biomId == BiomIds.Mangrove) return new SolidBrush(Color.DarkGreen);//Mangrove
        if (biomId == BiomIds.Savannah) return new SolidBrush(Color.LightGoldenrodYellow);//Savannah
        if (biomId == BiomIds.GrassSteppe) return new SolidBrush(Color.LawnGreen);//Grass Steppe
        if (biomId == BiomIds.Plains) return new SolidBrush(Color.RosyBrown);//Plains
        if (biomId == BiomIds.LavaPlain) return new SolidBrush(Color.Black);//Lava Plain
        if (biomId == BiomIds.Vulcano) return new SolidBrush(Color.Magenta);//Vulcano
        if (biomId == BiomIds.MushroomForest) return new SolidBrush(Color.BlueViolet);//Mushroom Forest        
        if (biomId == BiomIds.AncientForest) return new SolidBrush(Color.Olive);//Ancient Forest 
        if (biomId == BiomIds.Fields) return new SolidBrush(Color.Yellow);// Fields
        if (biomId == BiomIds.Swamp) return new SolidBrush(Color.Purple);//Swamp
        if (biomId == BiomIds.Bog) return new SolidBrush(Color.Plum);//Bog
        if (biomId == BiomIds.Marsh) return new SolidBrush(Color.PaleVioletRed);//Marsh
        if (biomId == BiomIds.RiverDelta) return new SolidBrush(Color.BlueViolet);//River Delta
        if (biomId == BiomIds.SandBeach) return new SolidBrush(Color.Khaki);//Sand Beach
        if (biomId == BiomIds.ReedsBeach) return new SolidBrush(Color.GreenYellow);//Reeds Beach
        if (biomId == BiomIds.Cliffs) return new SolidBrush(Color.LightSlateGray);//Cliffs
        if (biomId == BiomIds.CoralReef) return new SolidBrush(Color.SteelBlue);//Coral Reef
        if (biomId == BiomIds.Bank) return new SolidBrush(Color.MediumTurquoise);//Bank
        if (biomId == BiomIds.SeaweedForest) return new SolidBrush(Color.DarkSeaGreen);//Seaweed Forest
        if (biomId == BiomIds.CrystalForest) return new SolidBrush(Color.HotPink);//Crystal Forest
        if (biomId == BiomIds.Sea) return new SolidBrush(Color.Blue);//Sea
        if (biomId == BiomIds.SeaIce) return new SolidBrush(Color.White);//Sea Ice
        if (biomId == BiomIds.Ocean) return new SolidBrush(Color.DarkBlue);//Ocean
        return new SolidBrush(Color.Red);
    }

    private static Color PickBiomsColor(BiomIds biomId)
    {
        if (biomId == BiomIds.Desert) return Color.SandyBrown;//desert
        if (biomId == BiomIds.BareRock) return Color.DarkGray;//BareRock
        if (biomId == BiomIds.Tundra) return Color.PaleGreen;//tundra
        if (biomId == BiomIds.Permafrost) return Color.LightGray;//permafrost
        if (biomId == BiomIds.MountainTundra) return Color.SlateGray;//Mountain Tundra
        if (biomId == BiomIds.Glaciers) return Color.LightCyan;//ice Glaciers
        if (biomId == BiomIds.BorealForests) return Color.DarkGreen;//Taiga
        if (biomId == BiomIds.TemperateForests) return Color.Green;//Temperate Forests
        if (biomId == BiomIds.TemperateConiferousForests) return Color.ForestGreen;//Temperate Coniferous Forests
        if (biomId == BiomIds.TemperateRainForests) return Color.LawnGreen;//Temperate Rain Forests
        if (biomId == BiomIds.TropicalRainForests) return Color.LimeGreen;//Tropical Rain Forests
        if (biomId == BiomIds.TropicalMoistForests) return Color.SeaGreen;//Tropical Moist Forests
        if (biomId == BiomIds.TropicalDryForests) return Color.YellowGreen;//Tropical Dry Forests
        if (biomId == BiomIds.TropicalCloudForests) return Color.Teal;//Tropical Cloud Forests
        if (biomId == BiomIds.Mangrove) return Color.DarkGreen;//Mangrove
        if (biomId == BiomIds.Savannah) return Color.LightGoldenrodYellow;//Savannah
        if (biomId == BiomIds.GrassSteppe) return Color.LawnGreen;//Grass Steppe
        if (biomId == BiomIds.Plains) return Color.RosyBrown;//Plains
        if (biomId == BiomIds.LavaPlain) return Color.Black;//Lava Plain
        if (biomId == BiomIds.Vulcano) return Color.Magenta;//Vulcano
        if (biomId == BiomIds.MushroomForest) return Color.BlueViolet;//Mushroom Forest        
        if (biomId == BiomIds.AncientForest) return Color.Olive;//Ancient Forest 
        if (biomId == BiomIds.Fields) return Color.Yellow;// Fields
        if (biomId == BiomIds.Swamp) return Color.Purple;//Swamp
        if (biomId == BiomIds.Bog) return Color.Plum;//Bog
        if (biomId == BiomIds.Marsh) return Color.PaleVioletRed;//Marsh
        if (biomId == BiomIds.RiverDelta) return Color.BlueViolet;//River Delta
        if (biomId == BiomIds.SandBeach) return Color.Khaki;//Sand Beach
        if (biomId == BiomIds.ReedsBeach) return Color.GreenYellow;//Reeds Beach
        if (biomId == BiomIds.Cliffs) return Color.LightSlateGray;//Cliffs
        if (biomId == BiomIds.CoralReef) return Color.SteelBlue;//Coral Reef
        if (biomId == BiomIds.Bank) return Color.MediumTurquoise;//Bank
        if (biomId == BiomIds.SeaweedForest) return Color.DarkSeaGreen;//Seaweed Forest
        if (biomId == BiomIds.CrystalForest) return Color.HotPink;//Crystal Forest
        if (biomId == BiomIds.Sea) return Color.Blue;//Sea
        if (biomId == BiomIds.SeaIce) return Color.White;//Sea Ice
        if (biomId == BiomIds.Ocean) return Color.DarkBlue;//Ocean
        return Color.Red;
    }

    public static MemoryStream GetPointTest(long minLongitude, int pixelWidth, long maxLongitude, long minLatitude, int pixelHeight, long maxLatitude, int seed)
    {
        var bs = Box.pointBoxSize;
        var points = MapGenerator.MakePoints(minLongitude, maxLongitude, minLatitude, maxLatitude);
        var regions = MapGenerator.MakeRegions(minLongitude, maxLongitude, minLatitude, maxLatitude);
        int dotSize = 1;
        if (points.Count < 10000)
            dotSize = 2;
        if (points.Count < 1000)
            dotSize = 3;
        using Bitmap bitmap = new(pixelWidth, pixelHeight);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            g.MultiplyTransform(new(1, 0, 0, -1, 0, pixelHeight - 1));
            Pen box = new(Color.LightGray);
            if ((maxLongitude - minLongitude) < bs * 40)
            {
                foreach (var p in points)// point boxes
                {
                    long bx0 = pixelWidth * (p.Longitude / bs * bs - minLongitude) / (maxLongitude - minLongitude);
                    long by0 = pixelHeight * (p.Latitude / bs * bs - minLatitude) / (maxLatitude - minLatitude);
                    long bxw = pixelWidth * bs / (maxLongitude - minLongitude);
                    long byw = pixelWidth * bs / (maxLatitude - minLatitude);
                    g.DrawRectangle(box, bx0, by0, bxw, byw);
                }
            }
            foreach (var p in points)
            {
                long pixelX = pixelWidth * (p.Longitude - minLongitude) / (maxLongitude - minLongitude);
                long pixelY = pixelHeight * (p.Latitude - minLatitude) / (maxLatitude - minLatitude);
                int pointRegionIndex = FindRelatedRegionIndex(p, regions);
                Brush brush = PickRegionColor(pointRegionIndex);
                g.FillRectangle(brush, pixelX, pixelY, dotSize, dotSize);
            }
        }
        MemoryStream ms = new();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms;
    }

    private static SolidBrush PickRegionColor(int pointRegionIndex)
    {
        if (pointRegionIndex % 20 == 0) return new SolidBrush(Color.Red);
        if (pointRegionIndex % 20 == 1) return new SolidBrush(Color.Green);
        if (pointRegionIndex % 20 == 2) return new SolidBrush(Color.Blue);
        if (pointRegionIndex % 20 == 3) return new SolidBrush(Color.Yellow);
        if (pointRegionIndex % 20 == 4) return new SolidBrush(Color.Brown);
        if (pointRegionIndex % 20 == 5) return new SolidBrush(Color.Pink);
        if (pointRegionIndex % 20 == 6) return new SolidBrush(Color.LightGreen);
        if (pointRegionIndex % 20 == 7) return new SolidBrush(Color.DarkKhaki);
        if (pointRegionIndex % 20 == 8) return new SolidBrush(Color.DarkRed);
        if (pointRegionIndex % 20 == 9) return new SolidBrush(Color.LightBlue);
        if (pointRegionIndex % 20 == 10) return new SolidBrush(Color.Beige);
        if (pointRegionIndex % 20 == 11) return new SolidBrush(Color.Orange);
        if (pointRegionIndex % 20 == 12) return new SolidBrush(Color.BlueViolet);
        if (pointRegionIndex % 20 == 13) return new SolidBrush(Color.Purple);
        if (pointRegionIndex % 20 == 14) return new SolidBrush(Color.Lavender);
        if (pointRegionIndex % 20 == 15) return new SolidBrush(Color.Aquamarine);
        if (pointRegionIndex % 20 == 16) return new SolidBrush(Color.Azure);
        if (pointRegionIndex % 20 == 17) return new SolidBrush(Color.LightSalmon);
        if (pointRegionIndex % 20 == 18) return new SolidBrush(Color.SeaGreen);
        if (pointRegionIndex % 20 == 19) return new SolidBrush(Color.SaddleBrown);
        return new SolidBrush(Color.Black);
    }

    private static Color PickColorOf20(int pointRegionIndex)
    {
        if (pointRegionIndex % 20 == 0) return Color.Red;
        if (pointRegionIndex % 20 == 1) return Color.Green;
        if (pointRegionIndex % 20 == 2) return Color.Blue;
        if (pointRegionIndex % 20 == 3) return Color.Yellow;
        if (pointRegionIndex % 20 == 4) return Color.Brown;
        if (pointRegionIndex % 20 == 5) return Color.Pink;
        if (pointRegionIndex % 20 == 6) return Color.LightGreen;
        if (pointRegionIndex % 20 == 7) return Color.DarkKhaki;
        if (pointRegionIndex % 20 == 8) return Color.DarkRed;
        if (pointRegionIndex % 20 == 9) return Color.LightBlue;
        if (pointRegionIndex % 20 == 10) return Color.Beige;
        if (pointRegionIndex % 20 == 11) return Color.Orange;
        if (pointRegionIndex % 20 == 12) return Color.BlueViolet;
        if (pointRegionIndex % 20 == 13) return Color.Purple;
        if (pointRegionIndex % 20 == 14) return Color.Lavender;
        if (pointRegionIndex % 20 == 15) return Color.Aquamarine;
        if (pointRegionIndex % 20 == 16) return Color.Azure;
        if (pointRegionIndex % 20 == 17) return Color.LightSalmon;
        if (pointRegionIndex % 20 == 18) return Color.SeaGreen;
        if (pointRegionIndex % 20 == 19) return Color.SaddleBrown;
        return Color.Black;
    }


    private static Color PickColorWheelOf8(int direction)
    {
        if (direction % 8 == 1) return Color.FromArgb(0xFF, 0x00, 0x00);//north - #FF0000
        if (direction % 8 == 2) return Color.FromArgb(0xCC, 0xAA, 0x00);//northeast - #CCAA00
        if (direction % 8 == 3) return Color.FromArgb(0x96, 0xCC, 0x00);//east - #96CC00
        if (direction % 8 == 4) return Color.FromArgb(0x00, 0xB5, 0x51);//southeast - #00B551
        if (direction % 8 == 5) return Color.FromArgb(0x00, 0xB2, 0xB2);//south - #00B2B2
        if (direction % 8 == 6) return Color.FromArgb(0x00, 0x6E, 0xFF);//southwest - #006EFF
        if (direction % 8 == 7) return Color.FromArgb(0x8C, 0x00, 0xFF);//west - #8C00FF
        if (direction % 8 == 0) return Color.FromArgb(0xB5, 0x00, 0x98);//north west - #B50098
        return Color.Black;
    }

    private static SolidBrush PickRacePopColor(int r, int g, int b)
    {
        if (r > 1) r = 255;
        if (g > 1) g = 255;
        if (b > 1) b = 255;
        if (0 < r)
        {
            if (0 < g)
            {
                if (0 < b)
                    return new SolidBrush(Color.White);
                else
                    return new SolidBrush(Color.Yellow);
            }
            else
            {
                if (0 < b)
                    return new SolidBrush(Color.Magenta);
                else
                    return new SolidBrush(Color.Red);
            }
        }
        else
        {
            if (0 < g)
            {
                if (0 < b)
                    return new SolidBrush(Color.Cyan);
                else
                    return new SolidBrush(Color.Green);
            }
            else
            {
                if (0 < b)
                    return new SolidBrush(Color.Blue);
                else
                    return new SolidBrush(Color.Black);
            }
        }
    }


    private static int FindRelatedRegionIndex(Model.Point p, List<Model.Region> regions)
    {
        int res = -1;
        double minDist = double.MaxValue;
        for (int i = 0; i < regions.Count; i++)
        {
            var dist = regions[i].Center.Dist2(p);
            if (dist < minDist)
            {
                res = i;
                minDist = dist;
            }
        }
        return res;
    }

    private static Color GetMapHeightColor(float h, bool steppedLegend)
    {
        Dictionary<float, Color> mapColors = [];
        mapColors.Add(-100000f, Color.HotPink);
        mapColors.Add(Height.Min, Color.DarkBlue);
        mapColors.Add(Height.ShallowDeepWaterSplit, Color.Blue);
        mapColors.Add(Height.WaterLevel, Color.Yellow);
        mapColors.Add(Height.MainlandCoastSplit, Color.Lime);
        mapColors.Add(Height.LowHillsMainlandSplit, Color.Green);
        mapColors.Add(Height.HillLowHillSplit, Color.DarkGreen);
        mapColors.Add(Height.MountainHillSplit, Color.Maroon);
        mapColors.Add(Height.Max, Color.White);
        mapColors.Add(100000f, Color.HotPink);
        float first = 0f;
        float second = 0f;
        foreach (var mc in mapColors)
            if (mc.Key < h)
                first = mc.Key;
            else
            { // done
                second = mc.Key;
                break;
            }
        Color firstColor = mapColors[first];
        if (steppedLegend)
            return firstColor;
        Color secondColor = mapColors[second];
        float fact = (h - first) / (second - first);
        float r = firstColor.R + (secondColor.R - (float)firstColor.R) * fact;
        float g = firstColor.G + (secondColor.G - (float)firstColor.G) * fact;
        float b = firstColor.B + (secondColor.B - (float)firstColor.B) * fact;
        return Color.FromArgb(255, (int)r, (int)g, (int)b);
    }

    private static Color GetMapWindColor(float h, bool steppedLegend)
    {
        Dictionary<float, Color> mapColors = [];
        mapColors.Add(-1000f, Color.Black);
        mapColors.Add(-0.01f, Color.Black);
        mapColors.Add(0f, Color.Yellow);        // west -> east
        mapColors.Add(1.570796f, Color.Green);  // south -> north
        mapColors.Add(3.14159265f, Color.Blue); // east-> west
        mapColors.Add(4.71238898f, Color.Red);  // north -> south
        mapColors.Add(6.283185307f, Color.Yellow);
        mapColors.Add(6.29f, Color.Pink);
        mapColors.Add(1000f, Color.Pink);
        float first = 0f;
        float second = 0f;
        foreach (var mc in mapColors)
            if (mc.Key < h)
                first = mc.Key;
            else
            {
                // done
                second = mc.Key;
                break;
            }
        Color firstColor = mapColors[first];
        if (steppedLegend)
            return firstColor;
        Color secondColor = mapColors[second];
        float fact = (h - first) / (second - first);
        float r = firstColor.R + (secondColor.R - (float)firstColor.R) * fact;
        float g = firstColor.G + (secondColor.G - (float)firstColor.G) * fact;
        float b = firstColor.B + (secondColor.B - (float)firstColor.B) * fact;
        return Color.FromArgb(255, (int)r, (int)g, (int)b);
    }

    private static Color GetTemperatureColor(float t, bool steppedLegend)
    {
        Dictionary<float, Color> mapColors = [];
        mapColors.Add(-1000f, Color.HotPink);
        mapColors.Add(Temperature.Min, Color.Blue);
        mapColors.Add(Temperature.ColdVeryColdSplit, Color.LightBlue);
        mapColors.Add(Temperature.NormalColdSplit, Color.Green);
        mapColors.Add(Temperature.WarmNormalSplit, Color.Yellow);
        mapColors.Add(Temperature.WarmHotSplit, Color.Orange);
        mapColors.Add(Temperature.HotVeryHotSplit, Color.Red);
        mapColors.Add(Temperature.Max, Color.Black);
        mapColors.Add(1000f, Color.Pink);
        float first = 0f;
        float second = 0f;
        foreach (var mc in mapColors)
            if (mc.Key < t)
                first = mc.Key;
            else
            {
                // done
                second = mc.Key;
                break;
            }
        Color firstColor = mapColors[first];
        if (steppedLegend)
            return firstColor;
        Color secondColor = mapColors[second];
        float fact = (t - first) / (second - first);
        float r = firstColor.R + (secondColor.R - (float)firstColor.R) * fact;
        float g = firstColor.G + (secondColor.G - (float)firstColor.G) * fact;
        float b = firstColor.B + (secondColor.B - (float)firstColor.B) * fact;
        return Color.FromArgb(255, (int)r, (int)g, (int)b);
    }

    private static Color GetStormColor(float t, bool steppedLegend)
    {
        Dictionary<float, Color> mapColors = [];
        mapColors.Add(-100f, Color.White);
        mapColors.Add(MapGenerator.StormFrequencyQuite, Color.SkyBlue);
        mapColors.Add(MapGenerator.StormFrequencyStormy, Color.Red);
        mapColors.Add(1f, Color.Red);
        mapColors.Add(1000f, Color.Maroon);
        float first = 0f;
        float second = 0f;
        foreach (var mc in mapColors)
            if (mc.Key < t)
                first = mc.Key;
            else
            {
                // done
                second = mc.Key;
                break;
            }
        Color firstColor = mapColors[first];
        if (steppedLegend)
            return firstColor;
        Color secondColor = mapColors[second];
        float fact = (t - first) / (second - first);
        float r = firstColor.R + (secondColor.R - (float)firstColor.R) * fact;
        float g = firstColor.G + (secondColor.G - (float)firstColor.G) * fact;
        float b = firstColor.B + (secondColor.B - (float)firstColor.B) * fact;
        return Color.FromArgb(255, (int)r, (int)g, (int)b);
    }

    private static Color GetNewVsOldColor(float t)
    {
        if (t < 0) return Color.Brown;
        if (0 < t) return Color.SteelBlue;
        return Color.White;
    }

    private static Color GetPrecipitationColor(float t, bool steppedLegend)
    {
        Dictionary<float, Color> mapColors = [];
        mapColors.Add(-1000f, Color.HotPink);
        mapColors.Add(Precipitation.Min, Color.SandyBrown);
        mapColors.Add(Precipitation.DryVeryDrySplit, Color.White);
        mapColors.Add(Precipitation.NormalDrySplit, Color.LightGray);
        mapColors.Add(Precipitation.WetNormalSplit, Color.LightBlue);
        mapColors.Add(Precipitation.WetVeryWetSplit, Color.Blue);
        mapColors.Add(Precipitation.Max, Color.Black);
        mapColors.Add(1000f, Color.HotPink);
        float first = 0f;
        float second = 0f;
        foreach (var mc in mapColors)
            if (mc.Key < t)
                first = mc.Key;
            else
            {
                // done
                second = mc.Key;
                break;
            }
        Color firstColor = mapColors[first];
        if (steppedLegend)
            return firstColor;
        Color secondColor = mapColors[second];
        float fact = (t - first) / (second - first);
        float r = firstColor.R + (secondColor.R - (float)firstColor.R) * fact;
        float g = firstColor.G + (secondColor.G - (float)firstColor.G) * fact;
        float b = firstColor.B + (secondColor.B - (float)firstColor.B) * fact;
        return Color.FromArgb(255, (int)r, (int)g, (int)b);
    }

    private static Color GetMapXColor(float h)
    {
        // 4 common Fertile,Forrest,Poor,Plains & 4 rare Magic, Geoloic Surplus, Dark

        if (h < 0.99f) return Color.Black;     //  0 low  -  Geologic Surplus       2673
        if (h < 1.99f) return Color.Purple;    //  1 low  -  Dark                   2175
        if (h < 2.99f) return Color.Green;     //  2 high - Fertile                 12225
        if (h < 3.99f) return Color.DarkGreen; //  3 high - Forrest                 13440
        if (h < 4.99f) return Color.White;     //  4 low  -  ?                      3019
        if (h < 5.99f) return Color.HotPink;   //  5 low  -  Magic                  2808
        if (h < 6.99f) return Color.DimGray;   //  6 high - Poor                    13752
        if (h < 7.99f) return Color.Brown;     //  7 high - Plain                   12408
        if (h < 8.99f) return Color.Gray;
        if (h < 9.1f) return Color.Black;
        if (h < 10.1f) return Color.AliceBlue;
        if (h < 11.1f) return Color.RebeccaPurple;
        if (h < 12.1f) return Color.Beige;
        if (h < 13.1f) return Color.Gold;
        if (h < 14.1f) return Color.PaleGoldenrod;
        if (h < 15.1f) return Color.PaleGreen;
        if (h < 16.1f) return Color.BlueViolet;
        if (h < 17.1f) return Color.SeaGreen;
        return Color.White;
    }

    private static Color GetColor(float[]? n, int outType)
    {
        if (n is null) return Color.Black; ;
        double p = 0;
        double bytePart = 1 / 256.0;
        if (p < -2.0) return Color.DarkBlue;
        if (2.0 < p) return Color.DarkRed;
        if (outType == 0) p = n[0];
        else if (outType == 1) p = n[1];
        else if (outType == 2) p = n[2];
        else if (outType == 3)
        {
            p = (Math.Abs(n[1]) + Math.Abs(n[2])) / 2.0;
            if (2.0 < p) return Color.DarkRed;

            double normalPG = p / 2.0;
            byte rgbValG = (byte)(normalPG * 256);
            return Color.FromArgb(255, 50, 255 - rgbValG, 50);
        }
        else if (outType == 4)
        {
            p = n[0];
            if (Math.Abs(p) < 0.01)
                return Color.Green;
            double normalPg = (p + 1.0) / 2.0;
            if (1.0 - bytePart < normalPg)
                normalPg = 1.0 - bytePart;
            byte rgbValg = (byte)(normalPg * 256);
            return Color.FromArgb(255, rgbValg, rgbValg, rgbValg);
        }

        if (p < -1.0)
        {
            double normalPB = (p + 2.0) / 2.0;
            if (1.0 - bytePart < normalPB)
                normalPB = 1.0 - bytePart;
            byte rgbValB = (byte)(normalPB * 256);
            return Color.FromArgb(255, rgbValB, rgbValB, 255);
        }
        if (1.0 < p)
        {
            double normalPR = p / 2.0;
            if (1.0 - bytePart < normalPR)
                normalPR = 1.0 - bytePart;
            byte rgbValR = (byte)(normalPR * 256);
            return Color.FromArgb(255, 255, rgbValR, rgbValR);
        }
        double normalP = (p + 1.0) / 2.0;
        if (1.0 - bytePart < normalP)
            normalP = 1.0 - bytePart;
        byte rgbVal = (byte)(normalP * 256);
        return Color.FromArgb(255, rgbVal, rgbVal, rgbVal);

    }

}
#pragma warning restore CA1416 // Validate platform compatibility

public enum MapType
{
    Height = 1,
    WindDirection,
    BaseTemperature,
    GeologicType,
    BasePrecipitation,
    Slope,
    StormFreq,
    Bioms,
    NewAndOld,
    RacesA,
    RacesB,
}
