// See https://aka.ms/new-console-template for more information
using Model;
using Storage;
using Generator;
using SQLitePCL;

Console.WriteLine("Janitor starting!");
World.InitWorld();
LocalDb store = new("test-world");
bool onlyOne = true;
int runCount = 0;
int initRegionCount = 0;
int initLocationCount = 0;
int initPersonCount = 0;
int llmLocationCount = 0;
int comfyLocationCount = 0;
int llmPersonCount = 0;
int comfyPersonCount = 0;
int worldSeed = 1;
int playerId = 1;
bool lmStudioIsOn = true;
bool comfyIsOn = true;
bool debugExtremeVariance = true;
bool skipUninhabitedLocations = false; // only true for debug - will force uninhabited locations to 'Done', to save LLM and Picture run time
Dictionary<Guid, Janitor.ComfyImage> comfyImageQueue = [];

store.InitStore();

int unpicked = store.UnpickAll();
Console.WriteLine($"Startup clean: unpicked: {unpicked}");

while (true)// This is the main Janitor loop
{
    runCount++;
    var initialRegionRequest = GetFirstRegionRequest();

    if (initialRegionRequest is not null &&
        initialRegionRequest.CurrentGenerativeState == GenerativeState.None)
    {
        initRegionCount++;
        store.SetDebugActivityInfo(new DebugActivityInfo($"Initializing Region {initRegionCount}"));
        await DoRegionInitialization(initialRegionRequest);
        Console.WriteLine($"-- region init count: {initRegionCount}");
        continue;
    }

    var initialLocationRequest = GetFirstLocationRequest();

    if (initialLocationRequest is not null &&
        initialLocationRequest.CurrentGenerativeState == GenerativeState.None)
    {
        initLocationCount++;
        store.SetDebugActivityInfo(new DebugActivityInfo($"Initializing Location {initLocationCount}"));
        DoLocationInitialization(initialLocationRequest);
        Console.WriteLine($"-- location init count: {initLocationCount}");
        continue;
    }
    if (skipUninhabitedLocations)
    {
        if (initialLocationRequest is not null &&
        initialLocationRequest.CurrentGenerativeState == GenerativeState.Base)
        {
            var location = store.GetLocation(initialLocationRequest.Box);
            if (location is not null && location.Population == 0)
            {
                initialLocationRequest.CurrentGenerativeState = GenerativeState.Done;
                store.UpdateLocationRequestState(initialLocationRequest);
                Console.WriteLine("Uninhabited location skipped.");
                continue;
            }
        }
    }

    var initialPersonRequest = GetFirstPersonRequest();

    if (initialPersonRequest is not null &&
        initialPersonRequest.CurrentGenerativeState == GenerativeState.None)
    {
        initPersonCount++;
        store.SetDebugActivityInfo(new DebugActivityInfo($"Initializing Person {initPersonCount}"));
        DoPersonInitialization(initialPersonRequest);
        Console.WriteLine($"-- person init count: {initPersonCount}");
        continue;
    }

    // test players current location
    var currentCharacterLocation = store.GetCurrentCharacterLocationLog(playerId);
    if (currentCharacterLocation is null)
    {
        var hist = store.GetCharacterLocationLog(playerId);
        if (hist is not null && 0 < hist.Count)
        {
            Console.WriteLine($"Possible error - No current player location, but got {hist.Count} history");
            var latest = hist.OrderByDescending(l => l.FirstAt).First();
            store.Arrive(playerId, latest!.LocationBox, latest!.LocationPoint);
        }
        else
        { // no history going to start pos
            Box firstPlayerBox = new(new(0, 0), Box.pointBoxSize);
            Point firstPlayerPoint = MapGenerator.MakePoint(firstPlayerBox);
            store.Arrive(playerId, firstPlayerBox, firstPlayerPoint);
        }
        continue;
    }
    if (currentCharacterLocation.CharacterLocationStatus == CharacterLocationLog.Status.Current)
    {
        var location = store.GetLocation(currentCharacterLocation.LocationBox);
        if (location is null)
        {
            Box regionBox = Box.GetRegionBox(currentCharacterLocation.LocationPoint);
            var region = store.GetRegion(regionBox);
            if (region is null)
                store.RequestRegion(regionBox);
            else
                store.RequestLocation(currentCharacterLocation.LocationBox);
            continue;
        }

        // Test NeighborLocation
        store.SetDebugActivityInfo(new DebugActivityInfo("Testing Neighbor Locations"));
        var neighborBoxes = MapGenerator.GetNeighborLocationBoxes(currentCharacterLocation.LocationBox);
        bool requestedALocation = false;
        foreach (var neighborBox in neighborBoxes)
            if (store.RequestLocation(neighborBox))
                requestedALocation = true;
        if (requestedALocation)
            continue;

        // GoTo next block
    }
    else if (currentCharacterLocation.CharacterLocationStatus == CharacterLocationLog.Status.Departed)
    {
        // Travelling
        var destinationLocationLog = store.GetCharacterDestinationLocationLog(playerId);
        if (destinationLocationLog is null)
            store.Arrive(playerId, currentCharacterLocation.LocationBox, currentCharacterLocation.LocationPoint);// Error no destination - Go back!
        else
        {
            bool requestedALocation = false;
            // Init destination location
            if (store.RequestLocation(destinationLocationLog.LocationBox))
                requestedALocation = true;
            // Init destination neihbor locations

            var destinationNeighborBoxes = MapGenerator.GetNeighborLocationBoxes(destinationLocationLog.LocationBox);
            foreach (var neighborBox in destinationNeighborBoxes)
                if (store.RequestLocation(neighborBox))
                    requestedALocation = true;

            if (requestedALocation)
                continue;

            var destLocation = store.GetLocation(destinationLocationLog.LocationBox);
            if (destLocation is not null &&
                (
                    destLocation.State == GenerativeState.PictureAdded ||
                    destLocation.State == GenerativeState.Done
                ))
            {
                // Ready to arrive! destination and it's neihbor locations are done
                Console.WriteLine($"Arriving at point:({destinationLocationLog.LocationPoint.Longitude},{destinationLocationLog.LocationPoint.Latitude})");
                store.Arrive(playerId, destinationLocationLog.LocationBox, destinationLocationLog.LocationPoint);
                continue;
            }
        }
    }
    else
        throw new Exception("Unvalid currentlocation state! Fix code.");

    // retrieve queued pictures
    if (comfyIsOn &&
        comfyImageQueue.Count > 0)
    {
        if(comfyImageQueue.Count==1)
            store.SetDebugActivityInfo(new DebugActivityInfo($"Trying to retrieve {comfyImageQueue.First().Value.PictureType} pictures {comfyImageQueue.First().Value.Id}"));
        else
            store.SetDebugActivityInfo(new DebugActivityInfo($"Trying to retrieve pictures {comfyImageQueue.Count}"));
        HashSet<Guid> receivedImages = [];
        foreach (var imageInQueue in comfyImageQueue.Values)
        {
            if (await TryToReceive(imageInQueue))
                receivedImages.Add(imageInQueue.ComfyId);
        }
        foreach (Guid comfyId in receivedImages)
            comfyImageQueue.Remove(comfyId);
        if(receivedImages.Count>0)
            continue;
    }

    // make prompt for new pictures
    if (lmStudioIsOn &&
        comfyImageQueue.Count == 0)
    {
        if (initialLocationRequest is not null &&
            initialLocationRequest.CurrentGenerativeState == GenerativeState.Base)
        {
            llmLocationCount++;
            store.SetDebugActivityInfo(new DebugActivityInfo($"Making location prompts {llmLocationCount}"));
            await MakePromptForLocation(initialLocationRequest);
            Console.WriteLine($"---- location LLM count: {llmLocationCount}");
            continue;
        }
        if (initialPersonRequest is not null &&
            initialPersonRequest.CurrentGenerativeState == GenerativeState.Base)
        {
            llmPersonCount++;
            store.SetDebugActivityInfo(new DebugActivityInfo($"Making person prompts {llmPersonCount}"));
            await MakePromptForPerson(initialPersonRequest);
            Console.WriteLine($"---- person LLM count: {llmPersonCount}");
            continue;
        }
    }

    // Queue pictures for Generation
    if (comfyIsOn)
    {
        if (initialLocationRequest is not null &&
            initialLocationRequest.CurrentGenerativeState == GenerativeState.LlmPromptAdded)
        {
            comfyLocationCount++;
            store.SetDebugActivityInfo(new DebugActivityInfo($"Queing location pictures {comfyLocationCount}"));
            await QueueLocationPicture(initialLocationRequest);
            Console.WriteLine($"---- Location Comfy count: {comfyLocationCount}");
            continue;
        }
        if (initialPersonRequest is not null &&
            initialPersonRequest.CurrentGenerativeState == GenerativeState.LlmPromptAdded)
        {
            comfyPersonCount++;
            store.SetDebugActivityInfo(new DebugActivityInfo($"Queing person pictures {comfyPersonCount}"));
            await QueuePersonPicture(initialPersonRequest);
            Console.WriteLine($"---- person Comfy count: {comfyPersonCount}");
            continue;
        }
    }

    if(comfyImageQueue.Count==1)
        store.SetDebugActivityInfo(new DebugActivityInfo($"Sleeping while Comfy is generating {comfyImageQueue.First().Value.Id} pictures "));
    else if(comfyImageQueue.Count>1)
        store.SetDebugActivityInfo(new DebugActivityInfo($"Sleeping while Comfy is generating {comfyImageQueue.Count} pictures "));
    else
        store.SetDebugActivityInfo(new DebugActivityInfo("Sleeping"));
    Thread.Sleep(1000);
}

async Task QueueLocationPicture(LocationRequest initialLocationRequest)
{
    var location = store.GetLocation(initialLocationRequest.Box) ?? throw new Exception("Location missing");
    Guid id = await PictureGenerator.QueuePicture(
        PictureGenerator.MotiveType.location,
        location.Id,
        location.Box.GetSeed(),
        location.DescriptionPromptByLLM);
    if (id == Guid.Empty)
    {
        //Failed!
        Thread.Sleep(1000);
    }
    else
    {
        Janitor.ComfyImage newlyQueued = new(id, location.Id, PictureGenerator.MotiveType.location);
        comfyImageQueue.Add(id, newlyQueued);

        location.State = GenerativeState.PictureQueued;
        store.UpdateLocation(location);
        initialLocationRequest.CurrentGenerativeState = GenerativeState.PictureQueued;
        store.UpdateLocationRequestState(initialLocationRequest);
        Console.WriteLine($"Queued location picture at Comfy id:{id}");
    }
}

async Task QueuePersonPicture(PersonRequest initialPersonRequest)
{
    var person = store.GetPerson(initialPersonRequest.Id) ?? throw new Exception("Person missing");
    Guid id = await PictureGenerator.QueuePicture(
        PictureGenerator.MotiveType.portrait,
        person.Id,
        person.Id,
        person.DescriptionPromptByLLM);
    if (id == Guid.Empty)
    {
        //Failed!
        Thread.Sleep(1000);
    }
    else
    {
        Janitor.ComfyImage newlyQueued = new(id, person.Id, PictureGenerator.MotiveType.portrait);
        comfyImageQueue.Add(id, newlyQueued);

        person.State = GenerativeState.PictureQueued;
        store.UpdatePerson(person);
        initialPersonRequest.CurrentGenerativeState = GenerativeState.PictureQueued;
        store.UpdatePersonRequestState(initialPersonRequest);
        Console.WriteLine($"Portrait Picture by Comfy");
    }
}

async Task MakePromptForPerson(PersonRequest personRequest)
{
    var person = store.GetPerson(personRequest.Id) ?? throw new Exception("Person missing but has base state! Fix code.");
    if (!store.UpdatePersonRequestPicked(personRequest, true))
        throw new Exception("Could not pick person request for promt generating!");
    string positive = await TextGenerator.CreatePortraitPrompt(person);
    person.DescriptionPromptByLLM = positive;
    person.State = GenerativeState.LlmPromptAdded;
    if (store.UpdatePerson(person))
    {
        Console.WriteLine($"Person {person.Id} input: {person.Description} description by LLM: {positive}");
        personRequest.CurrentGenerativeState = GenerativeState.LlmPromptAdded;
        if (store.UpdatePersonRequestState(personRequest))
        {
            if (!store.UpdatePersonRequestPicked(personRequest, false))
                throw new Exception("Could NOT unpick person request for after save of propt!");
            Console.WriteLine($"Person description done.");
        }
        else
        {
            if (!store.UpdatePersonRequestPicked(personRequest, false))
                throw new Exception("Could NOT unpick person request after failed update request after failed save of promt!");
            throw new Exception($"Could NOT update state of person request to GenerativeState.LlmPromptAdded {GenerativeState.LlmPromptAdded}");
        }
    }
    else
    {
        if (!store.UpdatePersonRequestPicked(personRequest, false))
            throw new Exception("Could not unpick person request for after failed save of promt!");
        Console.WriteLine($"Failed saving!!! person description by LLM: {positive}");
    }
}

async Task MakePromptForLocation(LocationRequest locationRequest)
{
    var location = store.GetLocation(locationRequest.Box) ?? throw new Exception("Location missing but has base state! Fix code.");
    if (!store.UpdateLocationRequestPicked(locationRequest, true))
        throw new Exception("Could not pick location request for promt generating!");
    string positive = await TextGenerator.CreateLocationPrompt(location);
    location.DescriptionPromptByLLM = positive;
    location.State = GenerativeState.LlmPromptAdded;
    if (store.UpdateLocation(location))
    {
        Console.WriteLine($"Location description by LLM: {positive}");
        locationRequest.CurrentGenerativeState = GenerativeState.LlmPromptAdded;
        if (store.UpdateLocationRequestState(locationRequest))
        {
            if (!store.UpdateLocationRequestPicked(locationRequest, false))
                throw new Exception("Could not unpick location request for after failed  save of promt!");
            Console.WriteLine($"Location description done.");
        }
        else
        {
            if (!store.UpdateLocationRequestPicked(locationRequest, false))
                throw new Exception("Could not unpick location request for after failed update request after failed save of promt!");
            throw new Exception($"Could NOT update state of location request to GenerativeState.LlmPromptAdded {GenerativeState.LlmPromptAdded}");
        }
    }
    else
    {
        if (!store.UpdateLocationRequestPicked(locationRequest, false))
            throw new Exception("Could not unpick location request for after failed save of promt!");
        Console.WriteLine($"Failed saving!!! Location description by LLM: {positive}");
    }
}


async Task<bool> TryToReceive(Janitor.ComfyImage imageInQueue)
{
    ComfyHistoryResponse? history = await PictureGenerator.RetrievePictureHistory(imageInQueue.ComfyId);
    if (history is not null && history.status is not null)
    {
        // ready to retreive
        if (history.status.completed)
        {
            var comfyImage = history.outputs?.First().Value.images.First();
            if (comfyImage is null)
            {
                // Error the image should not be null when the status i done
                return false;
            }
            else
            {
                if(PictureGenerator.RetrievePicture(comfyImage.filename, imageInQueue.PictureType, imageInQueue.Id))
                {
                    if (imageInQueue.PictureType == PictureGenerator.MotiveType.portrait)
                    {
                        var person = store.GetPerson(imageInQueue.Id) ?? throw new Exception($"Missing person with {imageInQueue.Id} in store!");
                        var personRequest = store.GetPersonRequest(imageInQueue.Id) ?? throw new Exception($"Missing personRequest with {imageInQueue.Id} in store!");
                        person.PicturePath = PictureGenerator.GetFullPath(person.Id, PictureGenerator.MotiveType.portrait);
                        person.PictureUri = PictureGenerator.GetUri(person.Id, PictureGenerator.MotiveType.portrait);
                        person.State = GenerativeState.PictureAdded;
                        store.UpdatePerson(person);
                        personRequest.CurrentGenerativeState = GenerativeState.PictureAdded;
                        store.UpdatePersonRequestState(personRequest);
                    }
                    else if(imageInQueue.PictureType == PictureGenerator.MotiveType.location)
                    {
                        var location = store.GetLocation(imageInQueue.Id) ?? throw new Exception($"Missing location with {imageInQueue.Id} in store!");
                        var locationRequest = store.GetLocationRequest(location.Box) ?? throw new Exception($"Missing locationRequest with {imageInQueue.Id} in store!");
                        location.PicturePath = PictureGenerator.GetFullPath(location.Id, PictureGenerator.MotiveType.location);
                        location.PictureUri = PictureGenerator.GetUri(location.Id, PictureGenerator.MotiveType.location);
                        location.State = GenerativeState.PictureAdded;
                        store.UpdateLocation(location);
                        locationRequest.CurrentGenerativeState = GenerativeState.PictureAdded;
                        store.UpdateLocationRequestState(locationRequest);
                    }
                    else
                        throw new NotImplementedException($"Picture type {imageInQueue.PictureType} not implemented!");
                    return true;
                }
                else
                    return false;
            }
        }
        else
            return false;
    }
    else
        return false;
}

RegionRequest? GetFirstRegionRequest()
{
    var regionRequestResponse = store.GetRegionRequest(onlyOne);
    if (regionRequestResponse is not null && 0 < regionRequestResponse.Count)
        return regionRequestResponse.First();
    return null;
}

LocationRequest? GetFirstLocationRequest()
{
    var locationRequestResponse = store.GetLocationRequest(onlyOne);
    if (locationRequestResponse is not null && 0 < locationRequestResponse.Count)
        return locationRequestResponse.First();
    return null;
}

PersonRequest? GetFirstPersonRequest()
{
    var personRequestResponse = store.GetPersonRequest(onlyOne);
    if (personRequestResponse is not null && 0 < personRequestResponse.Count)
        return personRequestResponse.First();
    return null;
}

async Task DoRegionInitialization(RegionRequest regionRequest)
{
    Console.WriteLine($"Found a initial region request. RegionBoxLowerLeft: ({regionRequest.Box.LowerLeftPoint.Longitude},{regionRequest.Box.LowerLeftPoint.Latitude})");
    if (!store.UpdateRegionRequestPicked(regionRequest, true))
        throw new Exception("Did NOT pick region request");

    Region r = MapGenerator.InitRegion(regionRequest.Box);
    await r.Survey(worldSeed);
    r.State = GenerativeState.Base;
    if (store.InsertRegion(r))
    {
        regionRequest.CurrentGenerativeState = GenerativeState.Base;
        if (store.UpdateRegionRequestState(regionRequest))
        {
            if (!store.UpdateRegionRequestPicked(regionRequest, false))
                throw new Exception("Did NOT unpick region request!");
        }
        else
            throw new Exception("Did NOT update region request");
        Console.WriteLine($"Surveyed a new region named: {r.Name} the economi is {r.RegionWealth} and the ruling family name is {r.RulingFamilyName})");
    }
    else
        throw new Exception("Did NOT insert region");
}

async void DoLocationInitialization(LocationRequest locationRequest)
{
    Console.WriteLine($"Found a initial loaction request. LocationBoxLowerLeft: ({locationRequest.Box.LowerLeftPoint.Longitude},{locationRequest.Box.LowerLeftPoint.Latitude})");
    if (!store.UpdateLocationRequestPicked(locationRequest, true))
        throw new Exception("Did NOT pick location request");

    locationRequest.RequestTime = DateTime.Now;

    var p = MapGenerator.MakePoint(locationRequest.Box);
    // get associated region
    var regionBox = Box.GetRegionBox(p);
    var associateRegionBox = regionBox;
    var region = MapGenerator.InitRegion(regionBox);
    var dist2 = region.Center.Dist2(p);
    var neighborRegions = MapGenerator.GetNeighborRegions(regionBox);
    foreach (var neighborRegion in neighborRegions)
    {
        var nDist2 = neighborRegion.Center.Dist2(p);
        if (nDist2 < dist2)
        {
            associateRegionBox = neighborRegion.Box;
            dist2 = nDist2;
        }
    }
    // test if it is generated
    var associateRegion = store.GetRegion(associateRegionBox);
    if (associateRegion is null)
    {
        store.RequestRegion(associateRegionBox);
        locationRequest.CurrentGenerativeState = GenerativeState.None;
        if (store.UpdateLocationRequestState(locationRequest))
        {
            if (!store.UpdateLocationRequestPicked(locationRequest, false))
                throw new Exception("Did NOT unpick location request");
        }
        else
            throw new Exception("Did NOT update location request state, when bailing to region request!");
    }
    else
    {
        Location l = MapGenerator.CalculateBase(p, worldSeed);
        int locationSeed = l.Box.GetSeed();
        if (!l.CalculateBioms(locationSeed))
        {
            Console.WriteLine("Bad! No biom in new location!");
        }
        var basePopulationPotential = l.CalculatePopulationPotential();
#region debug many people        
        if(debugExtremeVariance && basePopulationPotential==0)
        {
            int debugLargePopulationPotential = locationSeed%5;
            basePopulationPotential = debugLargePopulationPotential;
        }
#endregion
        l.Population = basePopulationPotential;
        l.AssociateRegionBox = associateRegionBox;
        l.PickLocationType(associateRegion, locationSeed);
        var factory = new Generator.Locations.LocationTypeFactory();
        var locationFactory = factory.GetLocationType(l.LocationTypeKey ?? LocationTypes.None);
        foreach (var race in World.Races.Values)
            l.RacialPopulationPotention.Add(race.Id, MapGenerator.CalcRacialPopulationPotential(race, l.Population, l.BiomId, l.GeologyId));
#region debug many races            
        if(debugExtremeVariance && 0<basePopulationPotential)
        {
            int total = 0;
            foreach(var count in l.RacialPopulationPotention.Values)
                total+=count;
            if(total==0)
            {
                int debugRaceInt = locationSeed%9;
                RaceIds debugRace = debugRaceInt switch
                {
                    0 => RaceIds.Elve,
                    1 => RaceIds.Fae,
                    2 => RaceIds.Orc,
                    3 => RaceIds.Dwarf,
                    4 => RaceIds.Haflings,
                    5 => RaceIds.Mer,
                    6 => RaceIds.Lizard,
                    _ => RaceIds.Human
                };
                l.RacialPopulationPotention[debugRace]=1;
            }
        }
#endregion
        l.Description = locationFactory.GetTypeDescription(l, associateRegion);

        var personRequests = await locationFactory.GetPersonRequests(l, associateRegion);
        string familySurname = "";
        foreach (var personRequest in personRequests)
        {
            l.ActivePersonRequestIds.Add(store.RequestPerson(personRequest));// Request and retrieve id from db
            if (personRequest.FamilyRelationshipId == FamilyRelationshipIds.Husband || personRequest.FamilyRelationshipId == FamilyRelationshipIds.Wife)
                familySurname = personRequest.SurName;
        }
        if (string.IsNullOrWhiteSpace(familySurname))
            l.Name = locationFactory.GetTypeName(); // todo for now only a generic name, make it unique
        else
            l.Name = NameGenerator.Genitive(familySurname) + " " + locationFactory.GetTypeName(); // todo more variation
        l.State = GenerativeState.Base;
        // Done with location init, now save and unpick and update request
        if (store.InsertLocation(l))
        {
            locationRequest.CurrentGenerativeState = GenerativeState.Base;
            if (store.UpdateLocationRequestState(locationRequest))
            {
                if (!store.UpdateLocationRequestPicked(locationRequest, false))
                    throw new Exception("Did NOT unpick location request");
            }
            else
                throw new Exception("Did NOT update state location request");
            Console.WriteLine($"Created a location! {l.Name} with biom: {World.Bioms[l.BiomId].Name} ,basePopulationPotential:{basePopulationPotential}, desc:{l.Description}");
        }
        else
            throw new Exception("Did NOT insert location!");
    }
}


void DoPersonInitialization(PersonRequest personRequest)
{
    Console.WriteLine($"Found a initial person request. id:{personRequest.Id}");
    if (!store.UpdatePersonRequestPicked(personRequest, true))
        throw new Exception("Did NOT pick person request");

    Random rand = new(personRequest.Seed);
    personRequest.RequestTime = DateTime.Now;

    // get associated location
    var associateLocation = store.GetLocation(personRequest.LocationBox);
    if (associateLocation is null)
        throw new Exception("Missing location for person!");

    // get associated region
    var regionBox = associateLocation.AssociateRegionBox;
    if (regionBox is null)
        throw new Exception("Missing region box for person!");

    var associateRegion = store.GetRegion(regionBox);
    if (associateRegion is null)
        throw new Exception("Missing region for person!");

    Person person = new(personRequest.Id)
    {
        SexId = personRequest.SexId,
        PersonAgeId = personRequest.PersonAgeId,
        ProfessionId = personRequest.ProfessionId,
        RaceId = personRequest.RaceId,
        Name = personRequest.GivenName + " " + personRequest.SurName,
        WealthId = personRequest.LocationWealthId,
        State = GenerativeState.Base
    };

    // Pick matching color and styles

    List<SkinColorIds> possibleSkinColors = [];
    foreach (var sc in World.SkinColors)
        if (person.Match(sc.Value.Filter, associateLocation))
            possibleSkinColors.Add(sc.Key);
    if (0 < possibleSkinColors.Count)
        person.SkinColorId = possibleSkinColors[rand.Next(possibleSkinColors.Count)];

    List<HairStyleIds> possibleHairStyles = [];
    foreach (var hs in World.HairStyles)
        if (person.Match(hs.Value.Filter, associateLocation))
            possibleHairStyles.Add(hs.Key);
    if (0 < possibleHairStyles.Count)
        person.HairStyleId = possibleHairStyles[rand.Next(possibleHairStyles.Count)];

    List<HairColorIds> possibleHairColors = [];
    foreach (var hc in World.HairColors)
        if (person.Match(hc.Value.Filter, associateLocation))
            possibleHairColors.Add(hc.Key);
    if (0 < possibleHairColors.Count)
        person.HairColorId = possibleHairColors[rand.Next(possibleHairColors.Count)];

    List<DressIds> possibleDresses = [];
    foreach (var d in World.Dresses)
        if (person.Match(d.Value.Filter, associateLocation))
            possibleDresses.Add(d.Key);
    if (0 < possibleDresses.Count)
        person.DressId = possibleDresses[rand.Next(possibleDresses.Count)];

    List<DressColorIds> possibleDressColors = [];
    foreach (var dc in World.DressColors)
        if (person.Match(dc.Value.Filter, associateLocation))
            possibleDressColors.Add(dc.Key);
    if (0 < possibleDressColors.Count)
        person.DressColorId = possibleDressColors[rand.Next(possibleDressColors.Count)];

    List<JewelryIds> possibleJewelry = [];
    foreach (var j in World.Jewelry)
        if (person.Match(j.Value.Filter, associateLocation))
            possibleJewelry.Add(j.Key);
    if (0 < possibleJewelry.Count)
        person.JewelryId = possibleJewelry[rand.Next(possibleJewelry.Count)];

    string description = PreTextGenerator.CreatePersonDescription(person, associateLocation, associateRegion);
    person.Description = description;

    if (store.SavePerson(person))
    {
        personRequest.CurrentGenerativeState = GenerativeState.Base;
        if (store.UpdatePersonRequestState(personRequest))
        {
            if (!store.UpdatePersonRequestPicked(personRequest, false))
                throw new Exception("Did NOT unpick person request");
        }
        else
            throw new Exception("Did not update state of person request!");

        store.Arrive(person.Id, associateLocation.Box, associateLocation.Point);

        Console.WriteLine($"Created a person!");
    }
    else
        throw new Exception("Did NOT save person!");
}
