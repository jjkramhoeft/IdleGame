namespace Model
{
    /// <summary>
    /// A set of values that limits to fitting race, sex, age, profession, locationEconomy, temperature, locationType and biom
    /// </summary>
    public class PersonFilter
    {
        /// 
        public PersonFilter()
        {
            Races = [];
            Professions = [];
            Sex = [];
            Age = [];
            Wealth = [];
        }

        /// <summary>
        /// Cold things
        /// </summary>
        public float? MaxTemperature { get; set; }

        /// <summary>
        /// Hot things
        /// </summary>
        public float? MinTemperature { get; set; }

        /// <summary>
        /// Starter content
        /// </summary>
        public double? MaxDistanceFromOrigin { get; set; }

        /// <summary>
        /// High Level content
        /// </summary>
        public double? MinDistanceFromOrigin { get; set; }

        ///
        public List<RaceIds> Races { get; set; }

        ///
        public List<ProfessionIds> Professions { get; set; }

        ///
        public List<SexIds> Sex { get; set; }

        ///
        public List<PersonAgeIds> Age { get; set; }

        ///
        public List<LocationWealthIds> Wealth { get; set; }

        /// <summary>
        /// Frequency/likelihood. 0 -> 1
        /// </summary>
        public float Weight { get; set; } = 1f;

        ///     
        public static PersonFilter GetFilter(
            Temperature.NamedInterval t)
        {
            float tMax = Temperature.GetIntervalHigh(t);
            float tMin = Temperature.GetIntervalLow(t);

            return new()
            {
                MaxTemperature = tMax,
                MinTemperature = tMin
            };
        }

        ///      
        public static PersonFilter GetFilter(
            Temperature.NamedInterval tempIntervalName,
            List<SexIds> sex,
            List<PersonAgeIds> age,
            List<RaceIds> races)
        {
            float tMax = Temperature.GetIntervalHigh(tempIntervalName);
            float tMin = Temperature.GetIntervalLow(tempIntervalName);

            return new()
            {
                MaxTemperature = tMax,
                MinTemperature = tMin,
                Sex = sex,
                Age = age,
                Races = races
            };
        }

        ///      
        public static PersonFilter GetFilter(List<RaceIds> races)
        {
            return new()
            {
                Races = races
            };
        }

        ///       
        public static PersonFilter GetFilter(List<RaceIds> races, List<SexIds> sex)
        {
            return new()
            {
                Races = races,
                Sex = sex
            };
        }

        ///    
        public static PersonFilter GetFilter(List<ProfessionIds> professions, List<SexIds> sex)
        {
            return new()
            {
                Professions = professions,
                Sex = sex
            };
        }

        ///    
        public static PersonFilter GetFilter(List<ProfessionIds> professions, List<SexIds> sex, List<PersonAgeIds> age)
        {
            return new()
            {
                Professions = professions,
                Sex = sex,
                Age = age
            };
        }

        ///    
        public static PersonFilter GetFilter(List<PersonAgeIds> age, List<ProfessionIds> professions, List<RaceIds> race, List<SexIds> sex)
        {
            return new()
            {
                Professions = professions,
                Sex = sex,
                Age = age,
                Races = race
            };
        }

        ///    
        public static PersonFilter GetFilter(List<PersonAgeIds> age, List<ProfessionIds> professions, List<RaceIds> race, List<SexIds> sex, List<LocationWealthIds> wealth)
        {
            return new()
            {
                Professions = professions,
                Sex = sex,
                Age = age,
                Races = race,
                Wealth = wealth
            };
        }

        ///      
        public static PersonFilter GetFilter(List<RaceIds> races, List<SexIds> sex, List<LocationWealthIds> wealth)
        {
            return new()
            {
                Races = races,
                Sex = sex,
                Wealth = wealth
            };
        }

        ///    
        public static PersonFilter GetFilter(List<RaceIds> races, List<SexIds> sex, List<LocationWealthIds> wealth, List<PersonAgeIds> age)
        {
            return new()
            {
                Races = races,
                Sex = sex,
                Wealth = wealth,
                Age = age
            };
        }

        ///        
        public static PersonFilter GetFilter(List<RaceIds> races, List<LocationWealthIds> wealth)
        {
            return new()
            {
                Races = races,
                Wealth = wealth
            };
        }

        ///
        public static PersonFilter GetFilter(List<PersonAgeIds> age)
        {
            return new()
            {
                Age = age
            };
        }
    }
}