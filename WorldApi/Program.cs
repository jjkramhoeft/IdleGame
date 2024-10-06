using Storage;
using Model;
using System.Text.Json;
using Generator;
using DebugMapGenerator;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  var filePath = Path.Combine(System.AppContext.BaseDirectory, "Model.xml");
  c.IncludeXmlComments(filePath);
});
builder.Services.AddSingleton<IWorldStore>(new LocalDb("test-world"));
builder.Services.AddHttpClient("comfy", c =>
{
  c.BaseAddress = new Uri("http://127.0.0.1:8188/prompt");
});

var MyAllowSpecificLocalOrigins = "_myAllowSpecificLocalOrigins";
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: MyAllowSpecificLocalOrigins,
                    policy =>
                    {
                      policy.WithOrigins("http://localhost:4200", "http://localhost", "https://localhost");
                    });
});



builder.WebHost.UseUrls("https://localhost:7210", "https://localhost:443");

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseCors(MyAllowSpecificLocalOrigins);
  app.UseHttpsRedirection();
  app.UseStaticFiles();
  app.UseSwagger();
  app.UseSwaggerUI();
}

//var options = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All) };
var options = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

app.UseHttpsRedirection();

// Base ------------------------------------------------------------------------------------------------------------------------------------------------

app.MapGet("/persons/location/current/{characterid}", (int characterid, IWorldStore store) =>
{
  var currentPlayerLocationLog = store.GetCurrentCharacterLocationLog(characterid);
  if (currentPlayerLocationLog is null)
    return null;
  List<Person> persons = store.GetPersonsAtLocation(currentPlayerLocationLog.LocationBox);
  return persons;
})
.WithName("GetPersonsAtCurrentLocation")
.WithDescription("Get all characters at a location given by characterId's current location")
.WithTags("Base")
.WithOpenApi();


app.MapGet("/region/longitude/{longitude}/latitude/{latitude}", (long longitude, long latitude, IWorldStore store) =>
{
  Point p = new(longitude, latitude);
  Box regionBox = Box.GetRegionBox(p);
  Region? region = store.GetRegion(regionBox);
  if (region is null)
  {
    store.RequestRegion(regionBox);
    return null;
  }
  else
    return region;
})
.WithName("GetRegion")
.WithDescription("Get region, if its not in db then it is requested.")
.WithTags("Base")
.WithOpenApi();

app.MapGet("/location/longitude/{longitude}/latitude/{latitude}", (long longitude, long latitude, IWorldStore store) =>
{
  Point p = new(longitude, latitude);
  Box locationBox = Box.GetLocationBox(p);
  Location? location = store.GetLocation(locationBox);
  if (location is null)
  {
    store.RequestLocation(locationBox);
    return null;
  }
  else
    return location;
})
.WithName("GetLocation")
.WithDescription("Get location, if its not in db then it is requested.")
.WithTags("Base")
.WithOpenApi();

app.MapGet("/location/current/{characterid}", (int characterid, IWorldStore store) =>
{
  Location? location = null;
  World.InitWorld();
  var currentPlayerLocationLog = store.GetCurrentCharacterLocationLog(characterid);
  if (currentPlayerLocationLog is not null && currentPlayerLocationLog.LocationBox is not null)
    location = store.GetLocation(currentPlayerLocationLog.LocationBox);
  if (location is null)
    return null;
  else
    return location;
})
.WithName("GetCurrentLocation")
.WithDescription("Get current location.")
.WithTags("Base")
.WithOpenApi();


app.MapGet("/person/{id}", (int id, IWorldStore store) =>
{

  Person? person = store.GetPerson(id);
  if (person is null)
  {
    return null;
  }
  else
    return person;
})
.WithName("GetPerson")
.WithDescription("Get person.")
.WithTags("Base")
.WithOpenApi();


app.MapGet("/characterlocation/current/{characterid}", (int characterid, IWorldStore store) =>
{
  CharacterLocationLog? cl = store.GetCurrentCharacterLocationLog(characterid);
  return cl;
})
.WithName("GetCurrentCharacterLocation")
.WithTags("Base")
.WithOpenApi();

app.MapGet("/characterlocation/destination/{characterid}", (int characterid, IWorldStore store) =>
{
  CharacterLocationLog? cl = store.GetCharacterDestinationLocationLog(characterid);
  return cl;
})
.WithName("GetCharacterDestinationLocation")
.WithTags("Base")
.WithOpenApi();

app.MapGet("/characterlocation/depart/{characterid}/for-destination/{longitude}/{latitude}", (long longitude, long latitude, IWorldStore store, int characterid = 1) =>
{
  Point destinationPoint = new(longitude, latitude);
  Box destinationBox = Box.GetLocationBox(destinationPoint);
  bool res = store.Depart(characterid, destinationBox, destinationPoint);
  return res;
})
.WithName("SetCharacterDepartureLocation")
.WithTags("Base")
.WithOpenApi();

app.MapGet("/traveloptions/{characterid}", (IWorldStore store, int characterid = 1) =>
{
  var storageTravelOptions = MapLogistics.TravelPlanner.GetCurrentTravelOptions(characterid);
  List<TravelOption> apiTravelOptions = [];
  foreach (var to in storageTravelOptions)
  {
    TravelOption toApi = to;
    toApi.DepartAction = new Uri($"https://localhost:7210/characterlocation/depart/{characterid}/for-destination/{to.PointB.Longitude}/{to.PointB.Latitude}");
    apiTravelOptions.Add(to);
  }
  return apiTravelOptions;
})
.WithName("CurrentTravelOptions")
.WithTags("Base")
.WithOpenApi();

app.MapGet(
  "/current-player-map/width/{pixel}/{mapwidth}/seed/{seed}",
  (IWorldStore store, int mapwidth = 20000, int pixel = 300, int seed = 1) =>
{
  World.InitWorld();
  var currentPlayerLocationLog = store.GetCurrentCharacterLocationLog(1);
  var playerLocationsLog = store.GetCharacterLocationLog(1);
  List<Point> history = [];
  foreach (var pll in playerLocationsLog)
    if ((int)pll.CharacterLocationStatus != (int)CharacterLocationLog.Status.OnRoute)
      history.Add(pll.LocationPoint);
  var travelOptions = MapLogistics.TravelPlanner.GetCurrentTravelOptions(1);
  List<Point> posibilities = [];
  foreach (var to in travelOptions)
    if (!to.HasPreviouslyVisitedB)
      posibilities.Add(to.PointB);
  return Results.Bytes(MapPainter.GetMap(
    currentPlayerLocationLog!.LocationPoint.Longitude,
    currentPlayerLocationLog!.LocationPoint.Latitude,
    mapwidth,
    pixel,
    seed,
    history,
    posibilities).ToArray(), "image/png");
})
.WithName("CurrentPlayerMap")
.WithDescription("Map with visited locations")
.WithTags("Base")
.WithOpenApi();

app.MapGet(
  "/current-region-map/width/{pixel}/seed/{seed}",
  (IWorldStore store, int pixel = 300, int seed = 1, int colorType = 1) =>
{
  World.InitWorld();
  long centerLongitude = 0;
  long centerLatitude = 0;
  int mapwidth = 20000;
  var currentPlayerLocationLog = store.GetCurrentCharacterLocationLog(1);
  Box currentLocationBox = new(new(0, 0), Box.pointBoxSize);
  Box regionBox = new(new(0, 0), Box.regionBoxSize);
  if (currentPlayerLocationLog is not null)
  {
    currentLocationBox = currentPlayerLocationLog.LocationBox;
    var l = store.GetLocation(currentLocationBox);
    if (l is not null)
    {
      regionBox = l.AssociateRegionBox!;
    }
  }
  var r = store.GetRegion(regionBox);
  Point? capital = null;
  Point? secondCity = null;
  Point? lifeTree = null;
  List<Point> strongholds = [];
  if (r is not null)
  {
    capital = r.CapitalPoint;
    secondCity = r.SecondaryCityPoint;
    lifeTree = r.LifeTreePoint;
    strongholds = [.. r.RacialStrongholdPoints.Values];
    centerLongitude = r.Center.Longitude;
    centerLatitude = r.Center.Latitude;
    mapwidth = 150000;
  }
  var playerLocationsLog = store.GetCharacterLocationLog(1);
  List<Point> history = [];
  foreach (var pll in playerLocationsLog)
    if ((int)pll.CharacterLocationStatus == (int)CharacterLocationLog.Status.Current || (int)pll.CharacterLocationStatus == (int)CharacterLocationLog.Status.Departed)
      history.Add(pll.LocationPoint);
  //foreach (var pll in playerLocationsLog)
  //  if ((int)pll.CharacterLocationStatus == (int)CharacterLocationLog.Status.Historic)
  //    history.Add(pll.LocationPoint);
  var travelOptions = MapLogistics.TravelPlanner.GetCurrentTravelOptions(1);
  List<Point> posibilities = [];
  //foreach (var to in travelOptions)
  //  if (!to.HasPreviouslyVisitedB)
  //    posibilities.Add(to.PointB);
  return Results.Bytes(MapPainter.GetRegionMap(
    centerLongitude,
    centerLatitude,
    mapwidth,
    pixel,
    seed,
    capital, secondCity, lifeTree, strongholds,
    history,
    posibilities,
    colorType).ToArray(), "image/png");
})
.WithName("RegionMap")
.WithDescription("Map of Region")
.WithTags("Base")
.WithOpenApi();


app.MapGet(
  "/pictures/{pictype}/{picid}",
  (string pictype, string picid, IWorldStore store) =>
{
  string folder = "Locations";
  if (pictype.Equals("p"))
    folder = "Portraits";
  string filePath = $"D:\\VisualStudioProjects\\IdleGame\\Storage\\Data\\Pictures\\{folder}\\{pictype}{picid}.png";
  byte[] pictureBytes = System.IO.File.ReadAllBytes(filePath);
  return Results.Bytes(pictureBytes, "image/png");
})
.WithName("GetPictures")
.WithTags("Base")
.WithOpenApi();

// Internal ------------------------------------------------------------------------------------------------------------------------------------------------


app.MapGet("/debug-activity-info", (IWorldStore store) =>
{
  var request = store.GetDebugActivityInfo();
  return request;
})
.WithName("getDebnugActivityInfo")
.WithDescription("Get a text string wioth some info about what the janitor are doing")
.WithTags("Internal")
.WithOpenApi();


app.MapGet("/active-regionrequest/{onlyTop}", (IWorldStore store, bool onlyTop = true) =>
{
  var request = store.GetRegionRequest(onlyTop);
  return request;
})
.WithName("getActiveRegionRequest")
.WithDescription("Get first in line region request (if there is any)")
.WithTags("Internal")
.WithOpenApi();

app.MapGet("/active-locationrequest/{onlyTop}", (IWorldStore store, bool onlyTop = true) =>
{
  var request = store.GetLocationRequest(onlyTop);
  return request;
})
.WithName("getActiveLocationRequest")
.WithDescription("Get first in line region request (if there is any)")
.WithTags("Internal")
.WithOpenApi();

app.MapGet("/active-personrequest/{onlyTop}", (IWorldStore store, bool onlyTop = true) =>
{
  var request = store.GetPersonRequest(onlyTop);
  return request;
})
.WithName("getActivePersonRequest")
.WithDescription("Get first in line person request (if there is any)")
.WithTags("Internal")
.WithOpenApi();


// Debug ------------------------------------------------------------------------------------------------------------------------------------------------

app.MapGet("/initdb", (IWorldStore store) =>
{
  store.InitStore();
  return true;
})
.WithName("InitDb")
.WithDescription("Create all tables in world store")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet("/truncatedb", (IWorldStore store) =>
{
  store.EmptyStore();
  return true;
})
.WithName("TruncateDb")
.WithDescription("Truncate all tables in world store. The store will be empty!")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet(
  "/testpersons/{count}/{seed}", async (IWorldStore store, int count = 4, int seed = 0) =>
{
  World.InitWorld();
  bool done = await NameGenerator.InitAsync();
  Random rand = new(seed);
  Box startLocationBox = new(new(0, 0), Box.pointBoxSize);
  List<FamilyRelationshipIds> possibleFamilyIds = [];
  ProfessionIds profession = ProfessionIds.None;
  for (int i = 0; i < count; i++)
  {
    List<RaceIds> possibleRaces = [RaceIds.Dwarf, RaceIds.Elve, RaceIds.Fae, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Human, RaceIds.Lizard, RaceIds.Mer, RaceIds.Nymph, RaceIds.Orc, RaceIds.Thiefling];
    RaceIds race = possibleRaces[rand.Next(possibleRaces.Count)];
    List<ProfessionIds> possibleProfessions = [ProfessionIds.Bard, ProfessionIds.Beggar, ProfessionIds.Cook, ProfessionIds.Farmer, ProfessionIds.Fisherman, ProfessionIds.Guard, ProfessionIds.Hunter, ProfessionIds.InnKeeper, ProfessionIds.Ruler, ProfessionIds.Tailor, ProfessionIds.Shepherd];
    profession = possibleProfessions[rand.Next(possibleProfessions.Count)];

    SexIds sex = (SexIds)rand.Next(2);
    if (race == RaceIds.Nymph)
      sex = SexIds.Female;
    NameSex nSex = NameSex.Male;

    PersonAgeIds age = (PersonAgeIds)(rand.Next(2) + 5);
    if (race == RaceIds.Fae || race == RaceIds.Nymph)
      age = PersonAgeIds.Young;
    if (race == RaceIds.Ent)
      age = PersonAgeIds.Adult;

    if (age == PersonAgeIds.Child || age == PersonAgeIds.Infant)
    {
      possibleFamilyIds.Add(FamilyRelationshipIds.Child);
      profession = ProfessionIds.None;
    }
    else if (age == PersonAgeIds.Young)
    {
      possibleFamilyIds.Add(FamilyRelationshipIds.Child);
    }
    else if (age == PersonAgeIds.Old)
    {
      possibleFamilyIds.Add(FamilyRelationshipIds.Granny);
    }
    else
    {
      if (sex == SexIds.Female)
      {
        nSex = NameSex.Female;
        if (age == PersonAgeIds.Adult || age == PersonAgeIds.Mature)
          possibleFamilyIds.Add(FamilyRelationshipIds.Wife);
      }
      else
      {
        nSex = NameSex.Male;
        if (age == PersonAgeIds.Adult || age == PersonAgeIds.Mature)
          possibleFamilyIds.Add(FamilyRelationshipIds.Husband);
      }
    }

    var personRequest = new PersonRequest(startLocationBox)
    {
      CurrentGenerativeState = GenerativeState.None,
      FamilyRelationshipId = possibleFamilyIds[rand.Next(possibleFamilyIds.Count)],
      GivenName = NameGenerator.GenerateFirstName(seed + i, World.Races[race].Language, nSex),
      LocationBox = startLocationBox,
      LocationWealthId = (LocationWealthIds)(rand.Next(5) + 1),
      PersonAgeId = age,
      ProfessionId = profession,
      RaceId = race,
      SexId = sex,
      SurName = await NameGenerator.GenerateSurName(seed + i, World.Races[race].Language),
      Seed = i + seed
    };
    store.RequestPerson(personRequest);
  }
  return JsonSerializer.Serialize("done", options);
})
.WithName("SpawnSomePersons")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet(
  "/testnames/rc/{regioncount}/nc/{namecount}/s/seed", async (IWorldStore store, int regioncount = 4, int namecount = 6, int seed = 1) =>
{
  World.InitWorld();
  List<string> names = [];
  List<string> names2 = [];

  bool done = await NameGenerator.InitAsync();
  for (int i = 0; i < namecount; i++)
  {
    if (i % 6 == 0)
      names.Add($"Commen {i + 1} Male:   {NameGenerator.GenerateName(i, Language.Common, Generator.NameSex.Male)}");
    if (i % 6 == 1)
      names.Add($"Commen {i + 1} Female: {NameGenerator.GenerateName(i, Language.Common, Generator.NameSex.Female)}");
    if (i % 6 == 2)
      names.Add($"Exotic {i + 1} Male:   {NameGenerator.GenerateName(i, Language.Exotic, Generator.NameSex.Male)}");
    if (i % 6 == 3)
      names.Add($"Exotic {i + 1} Female: {NameGenerator.GenerateName(i, Language.Exotic, Generator.NameSex.Female)}");
    if (i % 6 == 4)
      names.Add($"Dark   {i + 1} Male:   {NameGenerator.GenerateName(i, Language.Dark, Generator.NameSex.Male)}");
    if (i % 6 == 5)
      names.Add($"Dark   {i + 1} Female: {NameGenerator.GenerateName(i, Language.Dark, Generator.NameSex.Female)}");
  }
  names.Add("-----------");
  Random rand = new(seed);
  for (int i = 0; i < regioncount; i++)
  {
    long bigNum = 100000000;
    long longi = rand.NextInt64(bigNum) - (bigNum / 2);
    long lati = rand.NextInt64(bigNum) - (bigNum / 2);
    seed++;
    Point p = new Point(longi, lati);
    Box regionBox = Box.GetRegionBox(p);
    Region r = MapGenerator.InitRegion(regionBox);
    await r.Survey(seed);
    r.Id = i + 1;
    names.Add($"Region {r.Id.ToString().PadLeft(2)} Name: {r.Name.PadRight(32)} Pop:{r.CombinedPopulationPotential.ToString().PadLeft(5)}, size:{r.LocationCount.ToString().PadLeft(5)} ");
    string supIds = string.Join(", ", r.SupportingRaceIds);
    names2.Add($"Region {r.Id.ToString().PadLeft(2)} ({r.Center.Longitude.ToString().PadLeft(9)},{r.Center.Latitude.ToString().PadLeft(9)}) Race: {World.Races[r.MainRaceId].Name}   Sup.Race: {supIds} ");
  }
  names.AddRange(names2);
  return JsonSerializer.Serialize(names, options);
})
.WithName("ShowSomeNames")
.WithTags("Investigate")
.WithOpenApi();

app.MapGet(
  "/debugmap/mw/{mapwidth}/mapx/{mapxoffset}/mapy/{mapyoffset}/mapsize/{pixel}/r1/{race1}/r2/{race2}/r3/{race3}/b1/{biom1}/b2/{biom2}/b3/{biom3}/sl/{steppedLegend}/b/{baseTempAndPercip}",
  (int mapwidth = 128000, int mapxoffset = -64000, int mapyoffset = -64000, int pixel = 256, int race1 = 1, int race2 = 2, int race3 = 99, int biom1 = 16, int biom2 = 17, int biom3 = 18, bool steppedLegend = false, bool baseTempAndPercip = false) =>
{
  World.InitWorld();
  return Results.Bytes(MapPainter.GetDebugMap(
    mapxoffset,
    mapyoffset,
    mapwidth,
    pixel,
    (RaceIds)race1,
    (RaceIds)race2,
    (RaceIds)race3,
    (BiomIds)biom1,
    (BiomIds)biom2,
    (BiomIds)biom3,
    steppedLegend,
    baseTempAndPercip).ToArray(), "image/png");
})
.WithName("DebugMap")
.WithDescription("Race 1-16, 99 => all.  Biom 1-37")
.WithTags("Investigate")
.WithOpenApi();




app.MapGet(
  "/debugmaps/mw/{mapwidth}/cx/{mapxcenter}/cy/{mapycenter}/mapsize/{pixel}/rA/{raceA}/rB/{raceB}/rC/{raceC}/bA/{biomA}/bB/{biomB}/bC/{biomC}",
  (int mapwidth = 128000, int mapxcenter = 0, int mapycenter = 0, int pixel = 256, int raceA = 1, int raceB = 2, int raceC = 3, int biomA = 2, int biomB = 5, int biomC = 23) =>
{
  World.InitWorld();
  return Results.Bytes(MapPainter.GetDebugMaps(
    mapxcenter,
    mapycenter,
    mapwidth,
    pixel,
    (RaceIds)raceA, (RaceIds)raceB, (RaceIds)raceC,
    (BiomIds)biomA, (BiomIds)biomB, (BiomIds)biomC).ToArray(), "image/png");
})
.WithName("DebugMaps")
.WithDescription("new")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet(
  "/biom-distribution/count/{count}/maxoffset/{maxoffset}/seed/{seed}",
  (int count = 10000, long maxoffset = 64000, int seed = 0) =>
{
  float minT = float.MaxValue;
  float maxT = float.MinValue;
  float minH = float.MaxValue;
  float maxH = float.MinValue;
  float minP = float.MaxValue;
  float maxP = float.MinValue;
  float minSl = float.MaxValue;
  float maxSl = float.MinValue;
  float minSt = float.MaxValue;
  float maxSt = float.MinValue;
  World.InitWorld();
  Dictionary<BiomIds, int> buckets = [];
  BiomIds b = BiomIds.None;
  buckets.Add(b, 0);
  Dictionary<BiomIds, int> secBuckets = [];
  Dictionary<string, float> dist = [];
  int countSecondaries = 0;
  int countPrimaries = 0;
  int countWithoutPrimaries = 0;
  Random r = new(seed);

  // Sample count random locations, calculate there bioms
  for (int i = 0; i < count; i++)
  {
    long x = r.NextInt64(maxoffset) - maxoffset / 2;
    long y = r.NextInt64(maxoffset) - maxoffset / 2;
    Point p = new(x, y);
    Location l = Generator.MapGenerator.CalculateBase(p, seed);
    if (l.Height < minH) minH = (float)l.Height;
    if (l.Height > maxH) maxH = (float)l.Height;
    if (l.SlopeValue < minSl) minSl = (float)l.SlopeValue;
    if (l.SlopeValue > maxSl) maxSl = (float)l.SlopeValue;
    if (l.Climate is not null)
    {
      if (l.Climate.AverageTemperature < minT) minT = (float)l.Climate.AverageTemperature;
      if (l.Climate.AverageTemperature > maxT) maxT = (float)l.Climate.AverageTemperature;
      if (l.Climate.PrecipitationAmount < minP) minP = (float)l.Climate.PrecipitationAmount;
      if (l.Climate.PrecipitationAmount > maxP) maxP = (float)l.Climate.PrecipitationAmount;
      if (l.Climate.StormFrequency < minSt) minSt = (float)l.Climate.StormFrequency;
      if (l.Climate.StormFrequency > maxSt) maxSt = (float)l.Climate.StormFrequency;
    }
    l.CalculateBioms(seed);
    if (0 < l.BiomId)
    {
      b = l.BiomId;
      countPrimaries++;
    }
    else
    {
      countWithoutPrimaries++;
      Temperature.NamedInterval misT = 0;
      Precipitation.NamedInterval misP = 0;
      Height.NamedInterval misH = 0;
      // find intevals for the no biom location
      foreach (var heigtInterval in Height.GetAllMainIntervals())
      {
        if (Height.GetIntervalHigh(heigtInterval) >= l.Height && Height.GetIntervalLow(heigtInterval) <= l.Height)
          misH = heigtInterval;
      }
      foreach (var tempInterval in Temperature.GetAllMainIntervals())
      {
        if (Temperature.GetIntervalHigh(tempInterval) >= l.Climate?.AverageTemperature && Temperature.GetIntervalLow(tempInterval) <= l.Climate.AverageTemperature)
          misT = tempInterval;
      }
      foreach (var precipitationInterval in Precipitation.GetAllMainIntervals())
      {
        if (Precipitation.GetIntervalHigh(precipitationInterval) >= l.Climate?.PrecipitationAmount && Precipitation.GetIntervalLow(precipitationInterval) <= l.Climate.PrecipitationAmount)
          misP = precipitationInterval;
      }
      int jk = 0;
      jk++;
    }
    if (buckets.TryGetValue(b, out int value))
      buckets[b] = ++value;
    else
      buckets.Add(b, 1);
  }


  foreach (var biom in World.Bioms.Values)
  {
    string name = (biom.Name + "(" + biom.Id + ")").PadRight(40, ' ');
    string total = "";
    int tCount = 0;
    string primary = "";
    string secondary = "";
    int sort = 0;

    if (buckets.ContainsKey(biom.Id))
    {
      tCount = buckets[biom.Id];
      float f = 100f * buckets[biom.Id] / (float)count;
      primary = $"primary: {f}%".PadRight(28, ' ');
      sort = buckets[biom.Id];
    }
    else
    {
      primary = "primary: 0%".PadRight(28, ' ');
    }

    if (secBuckets.ContainsKey(biom.Id))
    {
      tCount += secBuckets[biom.Id];
      float f = 100f * secBuckets[biom.Id] / (float)countSecondaries;
      secondary = $"sec: {f}%".PadRight(23, ' ');
    }
    else
    {
      secondary = "sec: 0%".PadRight(23, ' ');
    }

    float tF = 100f * tCount / (float)count;
    total = $"total: {tF}%".PadRight(23, ' ');

    dist.Add(name + total + primary + secondary, tCount);
  }
  float f0 = 100f * buckets[BiomIds.None] / (float)count;
  dist.Add($"No Biom: {f0}%  primaryCount:{countPrimaries} secondaryCount:{countSecondaries}", -1f);
  dist.Add($"Height min:{minH}", -2.0f);
  dist.Add($"Height max:{maxH}", -2.1f);
  dist.Add($"Slope min:{minSl}", -2.2f);
  dist.Add($"Slope max:{maxSl}", -2.3f);
  dist.Add($" Temp min:{minT}", -2.4f);
  dist.Add($" Temp max:{maxT}", -2.5f);
  dist.Add($"Preci min:{minP}", -2.6f);
  dist.Add($"Preci max:{maxP}", -2.7f);
  dist.Add($"Storm min:{minSt}", -2.8f);
  dist.Add($"Storm max:{maxSt}", -2.9f);
  return JsonSerializer.Serialize(dist.OrderByDescending(x => x.Value), options);
})
.WithName("BiomDist")
.WithDescription("Calculate biom frequensies")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet("/random-location-request/number-of-locations/{number}/max-extend-meters/{width}/{seed}", (IWorldStore store, int number = 10, int width = 10000, int seed = 474) =>
{
  Random rand = new(seed);
  for (int i = 0; i < number; i++)
  {
    long x = -width / 2 + rand.Next(width);
    long y = -width / 2 + rand.Next(width);
    Point p = new(x, y);
    _ = store.RequestLocation(Box.GetLocationBox(p));
  }
  return number;
})
.WithName("RandomLocationRequest")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet("/persons-metadata", (IWorldStore store) =>
{
  var res = store.GetPersonMetadata();
  return res;
})
.WithName("Persons")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet("/locations-metadata", (IWorldStore store) =>
{
  var res = store.GetLocationMetadata();
  return res;
})
.WithName("Locations")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet("/regions-metadata", (IWorldStore store) =>
{
  var res = store.GetRegionMetadataList();
  return res;
})
.WithName("Regions")
.WithTags("Investigate")
.WithOpenApi();


app.MapGet("/characterlocationlog/all/{characterid}", (IWorldStore store, int characterid = 1) =>
{
  List<CharacterLocationLog> cl = store.GetCharacterLocationLog(characterid);
  return cl;
})
.WithName("GetCAllCharacterLocationLogs")
.WithTags("Investigate")
.WithOpenApi();

app.MapGet("/reset-player/{characterid}", (IWorldStore store, int characterid = 1) =>
{
  store.ResetStore();
  return "Done !";
})
.WithName("ResetPlayer")
.WithTags("Investigate")
.WithOpenApi();


// Old ---------------------------------------------------------------------------------------------

app.MapGet(
  "/dbworld", (IWorldStore store) =>
{
  World.InitWorld();
  return JsonSerializer.Serialize(World.GetDebugWorld(), options);
})
.WithName("ShowDebugWorld")
.WithTags("Old")
.WithOpenApi();

app.MapGet(
  "/testperlinnoise/pw/{pixelWidth}/ph/{pixelHeight}/mw/{mapwidth}/mapx/{mapxoffset}/mapy/{mapyoffset}/ct/{output}/l1a/{layer1amp}/l1gs/{layer1gridsize}/l2a/{layer2amp}/l2gs/{layer2gridsize}/l3a/{layer3amp}/l3gs/{layer3gridsize}",
  (int pixelWidth = 200, int pixelHeight = 200, int mapwidth = 80000, int output = 0, int mapxoffset = 0, int mapyoffset = 0, float layer1amp = 1, int layer1gridsize = 1000, float layer2amp = 0, int layer2gridsize = 10000, float layer3amp = 0, int layer3gridsize = 100000) =>
{
  return Results.Bytes(MapPainter.GetPerlinNoiseTest(
    mapxoffset,
    pixelWidth,
    mapxoffset + mapwidth,
    mapyoffset,
    pixelHeight,
    mapyoffset + mapwidth * (pixelHeight / pixelWidth),
    output,
    layer1amp, layer1gridsize,
    layer2amp, layer2gridsize,
    layer3amp, layer2gridsize).ToArray(), "image/png");
})
.WithName("TestPerlinNoiseMap")
.WithDescription("A simple test of my Perlin Noise Implementation. 1 to 3 layers")
.WithTags("Old")
.WithOpenApi();

app.MapGet(
  "/testpoints/width/{pixelWidth}/height/{pixelHeight}/mapwidth/{mapwidth}/seed/{seed}",
  (int pixelWidth, int pixelHeight, int mapwidth, int seed) =>
{
  return Results.Bytes(MapPainter.GetPointTest(
    mapwidth,
    pixelWidth,
    mapwidth * 2,
    mapwidth,
    pixelHeight,
    mapwidth + mapwidth * (pixelWidth / pixelHeight),
    seed).ToArray(), "image/png");
})
.WithName("TestPointMap")
.WithDescription("A simple test of random points")
.WithTags("Old")
.WithOpenApi();

app.MapGet("/testpicgen/{id}", async (int id, IHttpClientFactory _httpClientFactory, CancellationToken cancellationToken = default) =>
{
  string body = @"
{
  ""3"": {
    ""inputs"": {
      ""seed"": 65080844902,
      ""steps"": 30,
      ""cfg"": 5.45,
      ""sampler_name"": ""euler"",
      ""scheduler"": ""sgm_uniform"",
      ""denoise"": 1,
      ""model"": [
        ""4"",
        0
      ],
      ""positive"": [
        ""16"",
        0
      ],
      ""negative"": [
        ""40"",
        0
      ],
      ""latent_image"": [
        ""53"",
        0
      ]
    },
    ""class_type"": ""KSampler"",
    ""_meta"": {
      ""title"": ""KSampler""
    }
  },
  ""4"": {
    ""inputs"": {
      ""ckpt_name"": ""sd3_medium_incl_clips.safetensors""
    },
    ""class_type"": ""CheckpointLoaderSimple"",
    ""_meta"": {
      ""title"": ""Load Checkpoint""
    }
  },
  ""8"": {
    ""inputs"": {
      ""samples"": [
        ""3"",
        0
      ],
      ""vae"": [
        ""4"",
        2
      ]
    },
    ""class_type"": ""VAEDecode"",
    ""_meta"": {
      ""title"": ""VAE Decode""
    }
  },
  ""9"": {
    ""inputs"": {
      ""filename_prefix"": ""ComfyUI"",
      ""images"": [
        ""8"",
        0
      ]
    },
    ""class_type"": ""SaveImage"",
    ""_meta"": {
      ""title"": ""Save Image""
    }
  },
  ""16"": {
    ""inputs"": {
      ""text"": ""In this serene glade, sunlight filters through the canopy above, casting dappled shadows on the lush green grass below. The air is filled with the sweet scent of blooming wildflowers, including delicate purple lupines and vibrant yellow buttercups. Tall conifers stand sentinel around the edges, their branches swaying gently in the breeze. A soft carpet of ferns and moss covers the forest floor, creating a sense of tranquility and connection to nature.\n"",
      ""clip"": [
        ""42"",
        0
      ]
    },
    ""class_type"": ""CLIPTextEncode"",
    ""_meta"": {
      ""title"": ""Positive Prompt""
    }
  },
  ""40"": {
    ""inputs"": {
      ""text"": ""painting, drawing, text, watermark, border, frame"",
      ""clip"": [
        ""42"",
        0
      ]
    },
    ""class_type"": ""CLIPTextEncode"",
    ""_meta"": {
      ""title"": ""Negative Prompt""
    }
  },
  ""42"": {
    ""inputs"": {
      ""clip_name1"": ""clip_g.safetensors"",
      ""clip_name2"": ""clip_l.safetensors"",
      ""type"": ""sd3""
    },
    ""class_type"": ""DualCLIPLoader"",
    ""_meta"": {
      ""title"": ""DualCLIPLoader""
    }
  },
  ""53"": {
    ""inputs"": {
      ""width"": 1024,
      ""height"": 1024,
      ""batch_size"": 1
    },
    ""class_type"": ""EmptySD3LatentImage"",
    ""_meta"": {
      ""title"": ""EmptySD3LatentImage""
    }
  }
}";
  
  string body2 = @"
  {
  ""26"": {
    ""inputs"": {
      ""filename_prefix"": ""FLUX"",
      ""images"": [
        ""37"",
        0
      ]
    },
    ""class_type"": ""SaveImage"",
    ""_meta"": {
      ""title"": ""Save Flux Image""
    }
  },
  ""29"": {
    ""inputs"": {
      ""text"": ""a photorealistic portrait of a blonde female fairy warrior with translucent pastel wings. She holds is short sword in her hand"",
      ""clip"": [
        ""31"",
        1
      ]
    },
    ""class_type"": ""CLIPTextEncode"",
    ""_meta"": {
      ""title"": ""CLIP Text Encode (Prompt)""
    }
  },
  ""30"": {
    ""inputs"": {
      ""noise_seed"": 945226685939680
    },
    ""class_type"": ""RandomNoise"",
    ""_meta"": {
      ""title"": ""RandomNoise""
    }
  },
  ""31"": {
    ""inputs"": {
      ""ckpt_name"": ""nepotismFUXGGUFMeA_v2AIO.safetensors""
    },
    ""class_type"": ""CheckpointLoaderSimple"",
    ""_meta"": {
      ""title"": ""Load Checkpoint""
    }
  },
  ""33"": {
    ""inputs"": {
      ""sampler_name"": ""euler""
    },
    ""class_type"": ""KSamplerSelect"",
    ""_meta"": {
      ""title"": ""KSamplerSelect""
    }
  },
  ""34"": {
    ""inputs"": {
      ""scheduler"": ""simple"",
      ""steps"": 25,
      ""denoise"": 1,
      ""model"": [
        ""31"",
        0
      ]
    },
    ""class_type"": ""BasicScheduler"",
    ""_meta"": {
      ""title"": ""BasicScheduler""
    }
  },
  ""35"": {
    ""inputs"": {
      ""model"": [
        ""31"",
        0
      ],
      ""conditioning"": [
        ""29"",
        0
      ]
    },
    ""class_type"": ""BasicGuider"",
    ""_meta"": {
      ""title"": ""BasicGuider""
    }
  },
  ""36"": {
    ""inputs"": {
      ""noise"": [
        ""30"",
        0
      ],
      ""guider"": [
        ""35"",
        0
      ],
      ""sampler"": [
        ""33"",
        0
      ],
      ""sigmas"": [
        ""34"",
        0
      ],
      ""latent_image"": [
        ""38"",
        0
      ]
    },
    ""class_type"": ""SamplerCustomAdvanced"",
    ""_meta"": {
      ""title"": ""SamplerCustomAdvanced""
    }
  },
  ""37"": {
    ""inputs"": {
      ""samples"": [
        ""36"",
        0
      ],
      ""vae"": [
        ""31"",
        2
      ]
    },
    ""class_type"": ""VAEDecode"",
    ""_meta"": {
      ""title"": ""VAE Decode""
    }
  },
  ""38"": {
    ""inputs"": {
      ""width"": 896,
      ""height"": 1152,
      ""batch_size"": 1
    },
    ""class_type"": ""EmptyLatentImage"",
    ""_meta"": {
      ""title"": ""Empty Latent Image""
    }
  }
}
  ";


//SchnellFluxBasic_api2.json
  string body3 = @"
  {
  ""5"": {
    ""inputs"": {
      ""width"": 896,
      ""height"": 1152,
      ""batch_size"": 1
    },
    ""class_type"": ""EmptyLatentImage""
  },
  ""6"": {
    ""inputs"": {
      ""text"": ""Extremely detailed photo. A front view portrait photo of a 24 y.o scandinavian woman, she looking away. Red hair and freckles"",
      ""clip"": [ ""11"", 0 ]
    },
    ""class_type"": ""CLIPTextEncode""
  },
  ""8"": {
    ""inputs"": {
      ""samples"": [ ""13"", 0 ],
      ""vae"": [ ""10"", 0 ]
    },
    ""class_type"": ""VAEDecode""
  },
  ""9"": {
    ""inputs"": {
      ""filename_prefix"": ""ComfyUI"",
      ""images"": [ ""8"", 0 ]
    },
    ""class_type"": ""SaveImage""
  },
  ""10"": {
    ""inputs"": {
      ""vae_name"": ""ae.safetensors""
    },
    ""class_type"": ""VAELoader""
  },
  ""11"": {
    ""inputs"": {
      ""clip_name1"": ""t5xxl_fp8_e4m3fn.safetensors"",
      ""clip_name2"": ""clip_l.safetensors"",
      ""type"": ""flux""
    },
    ""class_type"": ""DualCLIPLoader""
  },
  ""12"": {
    ""inputs"": {
      ""unet_name"": ""realflux10b_10bTransformer.safetensors"",
      ""weight_dtype"": ""default""
    },
    ""class_type"": ""UNETLoader""
  },
  ""13"": {
    ""inputs"": {
      ""noise"": [ ""25"", 0 ],
      ""guider"": [ ""22"", 0 ],
      ""sampler"": [ ""16"", 0 ],
      ""sigmas"": [ ""17"", 0 ],
      ""latent_image"": [ ""5"", 0 ]
    },
    ""class_type"": ""SamplerCustomAdvanced""
  },
  ""16"": {
    ""inputs"": {
      ""sampler_name"": ""euler""
    },
    ""class_type"": ""KSamplerSelect""
  },
  ""17"": {
    ""inputs"": {
      ""scheduler"": ""beta"",
      ""steps"": 5,
      ""denoise"": 1,
      ""model"": [ ""12"", 0 ]
    },
    ""class_type"": ""BasicScheduler""
  },
  ""22"": {
    ""inputs"": {
      ""model"": [ ""12"", 0 ],
      ""conditioning"": [ ""28"", 0 ]
    },
    ""class_type"": ""BasicGuider""
  },
  ""25"": {
    ""inputs"": {
      ""noise_seed"": "+id+@"
    },
    ""class_type"": ""RandomNoise""
  },
  ""28"": {
    ""inputs"": {
      ""guidance"": 1,
      ""conditioning"": [ ""6"", 0 ]
    },
    ""class_type"": ""FluxGuidance""
  }
}
  ";

  string p = "{\"prompt\": " + body3 + "}";
  _=body;
  _=body2;
  var httpClient = _httpClientFactory.CreateClient("comfy");
  var httpRequestContent = new StringContent(p, Encoding.UTF8, "application/json");
  var httpResponse = await httpClient.PostAsync("", httpRequestContent, cancellationToken);
  var jsonResponse = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
  var resO = JsonSerializer.Deserialize<Model.ComfyPromptResponse>(jsonResponse);
  string queuid = (resO?.prompt_id ?? Guid.Empty).ToString().Replace("{", "").Replace("}", "");
  try
  {
    var httpResponseH = await httpClient.GetStringAsync($"/history/{queuid}", cancellationToken);
    var h = JsonSerializer.Deserialize<Dictionary<Guid, Model.ComfyHistoryResponse>>(httpResponseH);
  }
  catch (Exception ex)
  {
    _ = ex;

  }
  return "1";
})
.WithName("TestPictureGeneration")
.WithTags("External Api Test")
.WithOpenApi();


app.MapGet("/testpromtgen/location", async (IHttpClientFactory _httpClientFactory, CancellationToken cancellationToken = default) =>
{
  var m = new
  {
    messages = new[]{
      new{role="assistant", content="Your task is to provide a very brief description of landscapes."},
      new{role="user", content="Describe a swamp. It is cold"}},
    temperature = 0.8,
    max_tokens = -1,
    stream = false
  };
  using StringContent jsonContent = new(
      JsonSerializer.Serialize(m),
      Encoding.UTF8,
      "application/json");
  var client = new HttpClient
  {
    BaseAddress = new Uri("http://localhost:1234")
  };
  using HttpResponseMessage response = await client.PostAsync(
      "/v1/chat/completions",
      jsonContent,
      cancellationToken);
  response.EnsureSuccessStatusCode();
  var jsonResponse = await response.Content.ReadAsStringAsync();
  var r = JsonSerializer.Deserialize<LMStudioResponse>(jsonResponse);
  return r?.choices[0].message.content;
})
.WithName("TestLLMPomptGenerationForLocation")
.WithTags("External Api Test")
.WithOpenApi();

app.MapGet("/testpromtgen/person", async (IHttpClientFactory _httpClientFactory, CancellationToken cancellationToken = default) =>
{
  var m = new
  {
    messages = new[]{
      new{role="assistant", content="Your assignment is to describe characters photos from a fantasy movie. All your descriptions must match the theme which is fantasy and adventure, it takes place in a middle aged fantasy world. You must be concise and to the point, focus on face and clothing. Describe the person in a geric way, do not use names"},
      new{role="user", content="Describe a rich young female elven, she is wearing a golden dress and stands in a library."}},
    temperature = 0.8,
    max_tokens = -1,
    stream = false
  };
  using StringContent jsonContent = new(
      JsonSerializer.Serialize(m),
      Encoding.UTF8,
      "application/json");
  var client = new HttpClient
  {
    BaseAddress = new Uri("http://localhost:1234")
  };
  using HttpResponseMessage response = await client.PostAsync(
      "/v1/chat/completions",
      jsonContent,
      cancellationToken);
  response.EnsureSuccessStatusCode();
  var jsonResponse = await response.Content.ReadAsStringAsync();
  var r = JsonSerializer.Deserialize<LMStudioResponse>(jsonResponse);
  return r?.choices[0].message.content;
})
.WithName("TestLLMPomptGenerationForPortrait")
.WithTags("External Api Test")
.WithOpenApi();

app.MapPut("/characterlocation/arrive/{characterid}", (int characterid, Point arrivalPoint, IWorldStore store) =>
{
  Box arrivalBox = Box.GetLocationBox(arrivalPoint);
  bool res = store.Arrive(characterid, arrivalBox, arrivalPoint);
  return res;
})
.WithName("SetCharacterArrivalLocation")
.WithTags("Old")
.WithOpenApi();



app.Run();
