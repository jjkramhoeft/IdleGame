using Model;

namespace Generator.Locations
{
    public class LocationTypeFactory
    {
        public LocationTypeFactory()
        {
            Types = [];
            RegisterLocationType(LocationTypes.AnimalTrail,()=>new AnimalTrail());
            RegisterLocationType(LocationTypes.CaveWithPaintings,()=>new CaveWithPaintings());
            RegisterLocationType(LocationTypes.DeadAnimal,()=>new DeadAnimal());
            RegisterLocationType(LocationTypes.DeadTree,()=>new DeadTree());
            RegisterLocationType(LocationTypes.EmptyCave,()=>new EmptyCave());
            RegisterLocationType(LocationTypes.FallenTree,()=>new FallenTree());
            RegisterLocationType(LocationTypes.FieldOfFlowers,()=>new FieldOfFlowers());
            RegisterLocationType(LocationTypes.Glade,()=>new Glade());
            RegisterLocationType(LocationTypes.FarmHouse,()=>new FarmHouse());
            RegisterLocationType(LocationTypes.HuntingCabin,()=>new HuntingCabin());
            RegisterLocationType(LocationTypes.ShepherdCamp,()=>new ShepherdCamp());
            RegisterLocationType(LocationTypes.HermitHouse,()=>new HermitHouse());
            RegisterLocationType(LocationTypes.FishingHut,()=>new FishingHut());
            RegisterLocationType(LocationTypes.PlainLocation,()=>new PlainLocation());
            RegisterLocationType(LocationTypes.PlainSettlement,()=>new PlainSettlement());
            RegisterLocationType(LocationTypes.GuardTower,()=>new GuardTower());
        }

        public LocationType GetLocationType(LocationTypes locationType)
        {
            return Types[locationType]();
        }

        public IEnumerable<LocationTypes> GetSupportedLocationTypes()
        {
            return [.. Types.Keys];
        }

        protected Dictionary<LocationTypes,Func<LocationType>> Types {get;}

        private void RegisterLocationType(LocationTypes loactionType,Func<LocationType> factory)
        {
            Types[loactionType]=factory;
        }
    }
}