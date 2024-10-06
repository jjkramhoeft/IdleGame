using Model;

namespace Storage
{
    public interface IWorldStore
    {
        public void InitStore();
        public void EmptyStore();
        public void ResetStore();

        public void SetDebugActivityInfo(DebugActivityInfo debugActivityInfo);
        public DebugActivityInfo GetDebugActivityInfo();


        public bool InsertRegion(Region region);
        public bool UpdateRegion(Region region);
        public Region? GetRegion(Box regionBox);

        public bool RequestRegion(Box regionBox);

        public List<Metadata> GetRegionMetadataList();
        public bool UpdateRegionRequestState(RegionRequest regionRequest);
        public bool UpdateRegionRequestPicked(RegionRequest regionRequest, bool picked);
        public List<RegionRequest> GetRegionRequest(bool onlyTop = true);
        public List<RegionRequest> GetPickedRegionRequest();


        public bool InsertLocation(Location location);
        public bool UpdateLocation(Location location);
        public Location? GetLocation(Box locationBox);
        public Location? GetLocation(int id);

        public bool RequestLocation(Box locationBox);

        public List<Metadata> GetLocationMetadata();
        public bool UpdateLocationRequestState(LocationRequest locationRequest);
        public bool UpdateLocationRequestPicked(LocationRequest locationRequest, bool picked);
        public bool DeleteLocationRequest(Box box);
        public List<LocationRequest> GetLocationRequest(bool onlyTop = true);          
        public LocationRequest? GetLocationRequest(Box locationBox);        
        public List<LocationRequest> GetPickedLocationRequest();


        public Person? GetPerson(int id);
        public bool SavePerson(Person person);
        public bool UpdatePerson(Person person);
        public List<Metadata> GetPersonMetadata();
        public int RequestPerson(PersonRequest personRequest);
        public List<PersonRequest> GetPersonRequest(bool onlyTop = true);
        public PersonRequest? GetPersonRequest(int id);
        public List<PersonRequest> GetPickedPersonRequest();
        public bool UpdatePersonRequestState(PersonRequest personRequest);
        public bool UpdatePersonRequestPicked(PersonRequest personRequest, bool picked);


        public CharacterLocationLog? GetCurrentCharacterLocationLog(int characterId);
        public CharacterLocationLog? GetCharacterDestinationLocationLog(int characterId);
        public CharacterLocationLog? GetCharacterLocationLog(int characterId, Box locationBox);
        public List<CharacterLocationLog> GetCharacterLocationLog(int characterId);
        public List<Person> GetPersonsAtLocation(Box locationBox);
        public bool Arrive(int characterId, Box arrivalBox, Point arrivalPoint);
        public bool Depart(int characterId, Box destinationBox, Point destinationPoint);
    }
}