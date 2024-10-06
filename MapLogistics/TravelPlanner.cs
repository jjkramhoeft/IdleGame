using Storage;
using Model;
using Generator;

namespace MapLogistics;

public static class TravelPlanner
{
    private static IWorldStore Store = new LocalDb("test-world");

    public static List<TravelOption> GetCurrentTravelOptions(int characterId)
    {
        World.InitWorld();
        var currentCharacterLocation = Store.GetCurrentCharacterLocationLog(characterId);
        if (currentCharacterLocation is null)
        {
            return [];
        }
        else if (currentCharacterLocation.CharacterLocationStatus == CharacterLocationLog.Status.Departed)
        {
            var destination = Store.GetCharacterDestinationLocationLog(1);
            if(destination is null)
                return [];
            int distance = (int)destination.LocationPoint.Dist(currentCharacterLocation.LocationPoint);
            double deltaY = -destination.LocationPoint.Latitude + currentCharacterLocation.LocationPoint.Latitude;
            double deltaX = -destination.LocationPoint.Longitude + currentCharacterLocation.LocationPoint.Longitude;
            double directionInRad = Math.Atan2(deltaY, deltaX);
            Direction direction = World.Directions[MapGenerator.RadToNearestDirectionId(directionInRad)];
            var fallback = new TravelOption(
                currentCharacterLocation.LocationPoint, 
                currentCharacterLocation.LocationPoint, 
                [TravelMode.Returning], 
                direction, 
                distance, 
                true);
            return [fallback];
        }
        else if (currentCharacterLocation.CharacterLocationStatus == CharacterLocationLog.Status.Current)
        {
            Location? location = Store.GetLocation(currentCharacterLocation.LocationBox);
            if (location is null)
                return [];
            var neighborBoxes = MapGenerator.GetNeighborLocationBoxes(Box.GetLocationBox(currentCharacterLocation.LocationPoint));
            Dictionary<Location,bool> neighborLocations = [];
            List<TravelOption> travelOptions = [];
            foreach (var neighborBox in neighborBoxes)
            {
                Location? neighborLocation = Store.GetLocation(neighborBox);
                if (neighborLocation is null)
                    Store.RequestLocation(neighborBox);
                else
                {
                    var v = Store.GetCharacterLocationLog(1,neighborLocation.Box);
                    if(v is not null && v.VisitCount>0)
                        neighborLocations.Add(neighborLocation, true);
                    else
                        neighborLocations.Add(neighborLocation, false);
                }
            }
            foreach (var foundLocation in neighborLocations)
            {
                double deltaY = foundLocation.Key.Point.Latitude - currentCharacterLocation.LocationPoint.Latitude;
                double deltaX = foundLocation.Key.Point.Longitude - currentCharacterLocation.LocationPoint.Longitude;
                double directionInRad = Math.Atan2(deltaY, deltaX);
                Direction direction = World.Directions[MapGenerator.RadToNearestDirectionId(directionInRad)];
                int distance = (int)foundLocation.Key.Point.Dist(currentCharacterLocation.LocationPoint);
                bool hasPreviouslyVisite = foundLocation.Value;
                List<TravelMode> travelModesA = World.Bioms[location.BiomId].ValidTravelModes;
                List<TravelMode> travelModesB = World.Bioms[foundLocation.Key.BiomId].ValidTravelModes;
                HashSet<TravelMode> validModesFromAToB = [];
                foreach (var tm1 in travelModesA)
                    foreach (var tm2 in travelModesB)
                    {
                        switch (tm1, tm2)
                        {
                            case (TravelMode.Walking, TravelMode.Walking):
                                validModesFromAToB.Add(TravelMode.Walking);
                                break;
                            case (TravelMode.Riding, TravelMode.Riding):
                                validModesFromAToB.Add(TravelMode.Riding);
                                break;
                            case (TravelMode.Carriage, TravelMode.Carriage):
                                validModesFromAToB.Add(TravelMode.Riding);
                                break;
                            case (TravelMode.Sailing, TravelMode.Sailing):
                                validModesFromAToB.Add(TravelMode.Sailing);
                                break;
                            case (TravelMode.Sailing, TravelMode.Walking):
                                if (foundLocation.Key.HasHabor)
                                    validModesFromAToB.Add(TravelMode.Sailing);
                                break;
                            case (TravelMode.Walking, TravelMode.Sailing):
                                if (location.HasHabor)
                                    validModesFromAToB.Add(TravelMode.Sailing);
                                break;
                            case (TravelMode.Walking, TravelMode.Swiming):
                                validModesFromAToB.Add(TravelMode.Swiming);
                                break;
                            case (TravelMode.Swiming, TravelMode.Walking):
                                validModesFromAToB.Add(TravelMode.Swiming);
                                break;
                            case (TravelMode.Mountaineering, TravelMode.Mountaineering):
                                validModesFromAToB.Add(TravelMode.Mountaineering);
                                break;
                            case (TravelMode.Walking, TravelMode.Mountaineering):
                                validModesFromAToB.Add(TravelMode.Mountaineering);
                                break;
                            case (TravelMode.Mountaineering, TravelMode.Walking):
                                validModesFromAToB.Add(TravelMode.Mountaineering);
                                break;
                        }
                    }                
                TravelOption travelOption = new(
                    currentCharacterLocation.LocationPoint, 
                    foundLocation.Key.Point, 
                    [.. validModesFromAToB], 
                    direction, 
                    distance, 
                    hasPreviouslyVisite);
                travelOptions.Add(travelOption);                
            }
            return travelOptions;
        }
        else
            throw new Exception($"Unexpected current location status {currentCharacterLocation.CharacterLocationStatus}!");
    }
}