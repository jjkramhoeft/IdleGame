using Model;

namespace Generator
{
    public static class PersonGenerator
    {
        public static async Task<List<PersonRequest>> CreateNoHoushold()
        {
            await Task.CompletedTask;
            List<PersonRequest> emptyResult = [];
            return emptyResult;
        }

        public static async Task<List<PersonRequest>> CreateHoushold(Location location, Region region)
        {


            int seed = location.Box.GetSeed();
            Random rand = new(seed);

            RaceIds race = RaceIds.None;
            int population = 0;
            var sortedRacialPopulationPotention = location.RacialPopulationPotention
                .Where(r => 0 < r.Value)
                .OrderByDescending(r => r.Value);
            if (!sortedRacialPopulationPotention.Any())
                return [];
            if (sortedRacialPopulationPotention.Count() == 1)
            {
                race = sortedRacialPopulationPotention.First().Key;
                population = sortedRacialPopulationPotention.First().Value;
            }
            else if (1 < sortedRacialPopulationPotention.Count() && sortedRacialPopulationPotention.Take(2).Last().Value < 4)
            {
                race = sortedRacialPopulationPotention.First().Key;
                population = sortedRacialPopulationPotention.First().Value;
            }
            else
            {

                if (rand.Next() < 0.9)
                {
                    race = sortedRacialPopulationPotention.First().Key;
                    population = sortedRacialPopulationPotention.First().Value;
                }
                else
                {
                    int secondHighestCount = sortedRacialPopulationPotention.Take(2).Last().Value;
                    var secondOptions = location.RacialPopulationPotention.Where(r => r.Value == secondHighestCount).ToList();
                    race = secondOptions[rand.Next(secondOptions.Count)].Key;
                    population = secondOptions[rand.Next(secondOptions.Count)].Value;
                }
            }
            int numberOfPeople = population;
            double r = rand.NextDouble();
            if (r < 0.1)
                numberOfPeople++;
            else if (1 < numberOfPeople && 0.90 < r)
                numberOfPeople--;

            ProfessionIds mainProfession = ProfessionIds.None;
            if (location.LocationTypeKey == LocationTypes.FarmHouse)
            {
                mainProfession = ProfessionIds.Farmer;
            }
            else if (location.LocationTypeKey == LocationTypes.HuntingCabin)
            {
                mainProfession = ProfessionIds.Hunter;
            }
            else if (location.LocationTypeKey == LocationTypes.FishingHut)
            {
                mainProfession = ProfessionIds.Fisherman;
            }
            else if (location.LocationTypeKey == LocationTypes.ShepherdCamp)
            {
                mainProfession = ProfessionIds.Shepherd;
            }
            else if (location.LocationTypeKey == LocationTypes.HermitHouse)
            {
                List<BiomIds> possibleWizardBioms = [BiomIds.BareRock, BiomIds.Cliffs, BiomIds.CrystalForest, BiomIds.LavaPlain, BiomIds.BareRock];
                List<BiomIds> possibleHerbalistBioms = [BiomIds.Bog, BiomIds.AncientForest, BiomIds.Mangrove, BiomIds.Marsh, BiomIds.MushroomForest];
                List<BiomIds> possibleMonkBioms = [BiomIds.MountainTundra];
                if (possibleWizardBioms.Contains(location.BiomId))
                    mainProfession = ProfessionIds.Wizard;
                else if (possibleHerbalistBioms.Contains(location.BiomId))
                    mainProfession = ProfessionIds.Herbalist;
                else if (possibleHerbalistBioms.Contains(location.BiomId))
                    mainProfession = ProfessionIds.Monk;

                if (race == RaceIds.Nymph)
                    mainProfession = ProfessionIds.Courtesan;

                if (race == RaceIds.Fae)
                    mainProfession = ProfessionIds.Wizard;
            }
            else if (location.LocationTypeKey == LocationTypes.GuardTower)
            {
                mainProfession = ProfessionIds.Guard;

                if (race == RaceIds.Nymph)
                    mainProfession = ProfessionIds.Courtesan;

                if (race == RaceIds.Fae)
                    mainProfession = ProfessionIds.Wizard;
            }

            ProfessionIds? sideProfession = ProfessionIds.None;

            LocationWealthIds wealth = (LocationWealthIds)region.RegionWealth;
            if (location.GeologyId == GeologyId.Poor)
                wealth = LocationWealth.DownGrade(wealth);
            else if (location.GeologyId == GeologyId.Fertile)
                wealth = LocationWealth.UpGrade(wealth);

            if (wealth == LocationWealthIds.Average || wealth == LocationWealthIds.Prospering || wealth == LocationWealthIds.Opulent)
            {
                if (location.LocationTypeKey == LocationTypes.FarmHouse)
                    sideProfession = ProfessionIds.Tailor;
                else if (location.LocationTypeKey == LocationTypes.HuntingCabin)
                    sideProfession = ProfessionIds.Herbalist;
                else if (location.LocationTypeKey == LocationTypes.FishingHut)
                    sideProfession = ProfessionIds.Carpenter;
                else if (location.LocationTypeKey == LocationTypes.ShepherdCamp)
                    sideProfession = ProfessionIds.Hunter;
            }

            return await CreateHousholdAsync(seed, mainProfession, sideProfession, wealth, numberOfPeople, race, location.Box);
        }

        public static async Task<List<PersonRequest>> CreateHousholdAsync(int seed, ProfessionIds mainProfession, ProfessionIds? sideProfession, LocationWealthIds wealth, int numberOfPeople, RaceIds race, Box locationBox)
        {

            if (race == RaceIds.None)
                return [];

            Random rand = new(seed);
            //Pick sex
            double femaleShare = 0.5;
            if (race == RaceIds.Nymph)
                femaleShare = 1.0;
            else if (race == RaceIds.Fae)
                femaleShare = 0.9;
            else if (race == RaceIds.Mer)
                femaleShare = 0.8;
            else if (race == RaceIds.Ent)
                femaleShare = 0.3;
            else if (race == RaceIds.Minotaur)
                femaleShare = 0.3;
            else if (race == RaceIds.Satyr)
                femaleShare = 0.2;
            SexIds sex = SexIds.Male;
            double pick = rand.NextDouble();
            if (pick < femaleShare)
                sex = SexIds.Female;

            // age split for single household
            double infantShare = 0;
            double childShare = 0;
            double youngShare = 0.1;
            double adultShare = 0.6;
            double matureShare = 0.2;
            double oldShare = 0.1;
            PersonAgeIds age = PersonAgeIds.None;
            List<PersonRequest> result = [];
            string familySurName = await NameGenerator.GenerateSurName(seed, race);
            string firstName = NameGenerator.GenerateFirstName(seed, World.Races[race].Language, (NameSex)sex);
            if (numberOfPeople == 1)  // Hemit
            {
                //Pick age
                pick = rand.NextDouble();
                if (pick < infantShare)
                    age = PersonAgeIds.Infant;
                else if (pick < childShare)
                    age = PersonAgeIds.Child;
                else if (pick < youngShare)
                    age = PersonAgeIds.Young;
                else if (pick < adultShare)
                    age = PersonAgeIds.Adult;
                else if (pick < matureShare)
                    age = PersonAgeIds.Mature;
                else if (pick < oldShare)
                    age = PersonAgeIds.Old;
                result.Add(new(locationBox)
                {
                    LocationWealthId = wealth,
                    PersonAgeId = age,
                    ProfessionId = mainProfession,
                    SexId = sex,
                    RaceId = race,
                    CurrentGenerativeState = GenerativeState.None,
                    RequestTime = DateTime.Now,
                    Seed = seed,
                    GivenName = firstName,
                    SurName = familySurName,
                    FamilyRelationshipId = FamilyRelationshipIds.None,
                });
                return result;
            }
            // Husband and wife
            age = PersonAgeIds.Adult;
            result.Add(new(locationBox)
            {
                LocationWealthId = wealth,
                PersonAgeId = age,
                ProfessionId = mainProfession,
                SexId = SexIds.Male,
                RaceId = race,
                CurrentGenerativeState = GenerativeState.None,
                RequestTime = DateTime.Now,
                Seed = seed,
                GivenName = firstName,
                SurName = familySurName,
                FamilyRelationshipId = FamilyRelationshipIds.Husband,
            });
            sideProfession ??= mainProfession;
            result.Add(new(locationBox)
            {
                LocationWealthId = wealth,
                PersonAgeId = age,
                ProfessionId = (ProfessionIds)sideProfession,
                SexId = SexIds.Female,
                RaceId = race,
                CurrentGenerativeState = GenerativeState.None,
                RequestTime = DateTime.Now,
                Seed = seed + 1,
                GivenName = NameGenerator.GenerateFirstName(seed + 1, World.Races[race].Language, (NameSex)sex),
                SurName = familySurName,
                FamilyRelationshipId = FamilyRelationshipIds.None,
            });
            // age split for +2 household extended family and friends
            infantShare = 0.1;
            childShare = 0.2;
            youngShare = 0.2;
            adultShare = 0.1;
            matureShare = 0.2;
            oldShare = 0.2;
            for (int i = 0; i < numberOfPeople - 2; i++)
            {
                sex = SexIds.Male;
                if (rand.NextDouble() < femaleShare)
                    sex = SexIds.Female;
                FamilyRelationshipIds relationship = FamilyRelationshipIds.None;
                ProfessionIds profession = ProfessionIds.None;
                string surName = familySurName;
                string givenName = NameGenerator.GenerateFirstName(seed + 2 + i, World.Races[race].Language, (NameSex)sex);
                //Pick age, relationship and profession
                pick = rand.NextDouble();
                if (pick < infantShare)
                {
                    age = PersonAgeIds.Infant;
                    relationship = FamilyRelationshipIds.Child;
                    profession = ProfessionIds.None;
                }
                else if (pick < childShare)
                {
                    age = PersonAgeIds.Child;
                    relationship = FamilyRelationshipIds.Child;
                    profession = ProfessionIds.None;
                }
                else if (pick < youngShare)
                {
                    age = PersonAgeIds.Young;
                    relationship = FamilyRelationshipIds.Child;
                    profession = mainProfession;
                }
                else if (pick < adultShare)
                {
                    age = PersonAgeIds.Adult;
                    relationship = FamilyRelationshipIds.Friend;
                    profession = mainProfession;
                    surName = "";
                }
                else if (pick < matureShare)
                {
                    age = PersonAgeIds.Mature;
                    relationship = FamilyRelationshipIds.Friend;
                    profession = mainProfession;
                    surName = await NameGenerator.GenerateSurName(seed + 2 + i, race);
                }
                else if (pick < oldShare)
                {
                    age = PersonAgeIds.Old;
                    relationship = FamilyRelationshipIds.Granny;
                    profession = mainProfession;
                }
                // Re-pick profession
                if (profession == mainProfession && rand.NextDouble() < 0.2)
                    profession = (ProfessionIds)sideProfession;

                result.Add(new(locationBox)
                {
                    LocationWealthId = wealth,
                    PersonAgeId = age,
                    ProfessionId = profession,
                    SexId = sex,
                    RaceId = race,
                    CurrentGenerativeState = GenerativeState.None,
                    RequestTime = DateTime.Now,
                    Seed = seed + 2 + i,
                    GivenName = givenName,
                    SurName = surName,
                    FamilyRelationshipId = relationship,
                });
            }
            return result;
        }
    }
}