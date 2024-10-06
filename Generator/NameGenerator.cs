using System.Text.Json;
using Model;

namespace Generator
{
    public static class NameGenerator
    {
        private static SortedDictionary<string, TokenSet> CommonMaleNameSet = [];
        private static SortedDictionary<string, TokenSet> CommonFemaleNameSet = [];
        private static SortedDictionary<string, TokenSet> CommonSurnameSet = [];
        private static SortedDictionary<string, TokenSet> ExoticMaleNameSet = [];
        private static SortedDictionary<string, TokenSet> ExoticFemaleNameSet = [];
        private static SortedDictionary<string, TokenSet> ExoticSurnameSet = [];
        private static SortedDictionary<string, TokenSet> DarkMaleNameSet = [];
        private static SortedDictionary<string, TokenSet> DarkFemaleNameSet = [];
        private static SortedDictionary<string, TokenSet> DarkSurnameSet = [];
        private static bool Loaded = false;
        private static bool Loading = false;

        public static async Task<bool> InitAsync()
        {
            if (Loading || Loaded) return false;
            Loading = true;
            CommonMaleNameSet = await LoadTokenSet("GB_male_names_sets.json");
            CommonFemaleNameSet = await LoadTokenSet("GB_female_names_sets.json");
            CommonSurnameSet = await LoadTokenSet("GB_sur_names_sets.json");
            ExoticMaleNameSet = await LoadTokenSet("Elven_male_names_sets.json");
            ExoticFemaleNameSet = await LoadTokenSet("Elven_female_names_sets.json");
            ExoticSurnameSet = await LoadTokenSet("Elven_sur_names_sets.json");
            DarkMaleNameSet = await LoadTokenSet("darkelve_male_names_sets.json");
            DarkFemaleNameSet = await LoadTokenSet("darkelve_female_names_sets.json");
            DarkSurnameSet = await LoadTokenSet("darkelve_sur_names_sets.json");
            Loaded = true;
            return true;
        }

        public static async Task<SortedDictionary<string, TokenSet>> LoadTokenSet(string name)
        {
            using FileStream openStream = File.OpenRead(@"D:\VisualStudioProjects\IdleGame\Storage\PreGenData\" + name);
            SortedDictionary<string, TokenSet>? setsIn = await JsonSerializer.DeserializeAsync<SortedDictionary<string, TokenSet>>(openStream);
            return setsIn!;
        }

        public static string GenerateRegionName(int seed, Region reg)
        {
            Race r = World.Races[RaceIds.Human];
            if (0 < reg.MainRaceId)
                r = World.Races[reg.MainRaceId];
            bool big = false;
            bool small = false;
            if (reg.CombinedPopulationPotential < 10000)
                small = true;
            if (100000 < reg.CombinedPopulationPotential)
                small = true;

            List<(string, int)> pre = [("North ", 2), ("East ", 2), ("South ", 2), ("West ", 2), ("Northern ", 2), ("Eastern ", 2), ("Southen ", 2), ("Western ", 2)];
            List<(string, int)> preHuman = [("", 15), ("New ", 5), ("Neo-", 2), ("United ", 2), ("Free ", 2)];
            List<(string, int)> preNonhuman = [("", 15), ("Old ", 5)];
            List<(string, int)> preBig = [("", 15), ("Gran", 1), ("Grand ", 2), ("Greath ", 4)];
            List<(string, int)> preNumber = [("", 15), ("Second ", 1), ("Third ", 2), ("Fourth ", 3), ("Fifth ", 2), ("Sixth ", 1)];

            List<(string, int)> mid = [("", 10), ("Abbasid", 1), ("Aceh", 1), ("Achaemenid", 1), ("Afsharid", 1), ("Aizak", 1), ("Ajuran", 1), ("Akkadian", 1), ("Angevin", 1), ("Austro", 1), ("Avar", 1), ("Balhae", 1), ("Bamana", 1), ("Banjar", 1), ("Benin", 1), ("Bese", 1), ("Blanind", 1), ("Bogd", 1), ("Bornu", 1), ("Brazillian", 1), ("Bretoth", 1), ("Bruneian", 1), ("Bukhara", 1), ("Burgundian", 1), ("Buyid", 1), ("Calakmul", 1), ("Cao Wei", 1), ("Carolingian", 1), ("Carthaginian", 1), ("Central", 1), ("Cewer", 1), ("Chagatai", 1), ("Chalukya", 1), ("Chauhan", 1), ("Chenla", 1), ("Thera", 1), ("Chimor", 1), ("Chola", 1), ("Cihgecu", 1), ("Clapeth", 1), ("Clozqiet", 1), ("Comanche", 1), ("Congo", 1), ("Córdoba", 1), ("Crimean", 1), ("Cruunsilu", 1), ("Dacian", 1), ("Dagbon", 1), ("Dega", 1), ("Delhi", 1), ("Demak", 1), ("Dohoq", 1), ("Dumut", 1), ("Durrani", 1), ("Dzungar", 1), ("Egrium", 1), ("Ehni", 1), ("Eki", 1), ("Elamite", 1), ("Elba", 1),];
            mid.AddRange([("Gallic", 1), ("Ganat", 1), ("Ganga", 1), ("Gaza", 1), ("Genoese", 1), ("Ghana", 1), ("Ghurid", 1), ("Glepei", 1), ("Gnehrei", 1), ("Goguryeo", 1), ("Golden", 1), ("Gorkha", 1), ("Goryeo", 1), ("Gowa", 1), ("Grersah", 1), ("Gunbax", 1), ("Gupta", 1), ("Gurjara-Pratihara", 1), ("Göktürk", 1), ("Hafsid", 1), ("Han", 1), ("Harni", 1), ("Harsha", 1), ("Hephthalite", 1), ("Hittite", 1), ("Hocuiq", 1), ("Hofesh", 1), ("Hontea", 1), ("Hospitaller", 1), ("Hotak", 1), ("Hugax", 1), ("Huilstin", 1), ("Hunnic", 1), ("Husainid", 1), ("Iberian", 1), ("Icag", 1), ("Idisid", 1), ("Ifkush", 1), ("Ihreo", 1), ("Ikomau", 1), ("Ikush", 1), ("Ilkhanate", 1), ("Iqaq", 1), ("Irfi", 1), ("Isfa", 1), ("Iursar", 1), ("Jaftaag", 1), ("Jihu", 1), ("Jin", 1), ("Johor", 1), ("Jolof", 1), ("Joseon", 1), ("Judah", 1), ("Juncolu", 1), ("Kachari", 1), ("Kadamba", 1), ("Kadu", 1), ("Kagho", 1), ("Kalmar", 1), ("Frulshom", 1), ("Fulo", 1), ("Funan", 1),]);
            mid.AddRange([("Kamtel", 1), ("Kanem", 1), ("Kanva", 1), ("Kara-Khanid", 1), ("Kazakh", 1), ("Kelrus", 1), ("Khazar", 1), ("Khilji", 1), ("Khitai", 1), ("Khmer", 1), ("Kievan Rus'", 1), ("Konbaung", 1), ("Kong", 1), ("Kushan", 1), ("Kyrgyz", 1), ("Kaabu", 1), ("Later", 1), ("Lavo", 1), ("Lenig", 1), ("Liao", 1), ("Lidquax", 1), ("Lingus", 1), ("Lithuania", 1), ("Lodi", 1), ("Lodrya", 1), ("Mabe", 1), ("Madurai Nayak", 1), ("Magadha", 1), ("Majapahit", 1), ("Malacca", 1), ("Mali", 1), ("Mamluk", 1), ("Maratha", 1), ("Marhasi", 1), ("Marinid", 1), ("Massina", 1), ("Mataram", 1), ("Mauretania", 1), ("Maurya", 1), ("Mauryan", 1), ("Median", 1), ("Meqiak", 1), ("Mexican", 1), ("Ming", 1), ("Mitanni", 1), ("Moavian", 1), ("Mughal", 1), ("Nanda", 1), ("Nawhesh", 1), ("Nocqili", 1), ("Nuake", 1), ("Numidia", 1), ("Odrysian", 1), ("Omani", 1), ("Ostrogothic", 1), ("Oyo", 1), ("Pagan", 1), ("Pahlavi", 1), ("Pala", 1), ("Pallava", 1), ("Palmyrene", 1),]);
            mid.AddRange([("Pontus", 1), ("Potix", 1), ("Prussian", 1), ("Ptolemaic", 1), ("Qajar", 1), ("Qasend", 1), ("Qiluum", 1), ("Qimuc", 1), ("Qin", 1), ("Qing", 1), ("Qzoth", 1), ("Rapeq", 1), ("Rashidun", 1), ("Rashtrakuta", 1), ("Repa", 1), ("Rispinde", 1), ("Rodge", 1), ("Rouran", 1), ("Rozvi", 1), ("Rum", 1), ("Rustamid", 1), ("Safavid", 1), ("Saffarid", 1), ("Saga", 1), ("Samanid", 1), ("Samo's", 1), ("Sassanian", 1), ("Satavahana", 1), ("Scepeoc", 1), ("Scte", 1), ("Scuamthal", 1), ("Selecuid", 1), ("Seleucid", 1), ("Seljuk", 1), ("Seljuk", 1), ("Seorrul", 1), ("Shah", 1), ("Shang", 1), ("Shihmuh", 1), ("Shilo", 1), ("Shu Han", 1), ("Shunga", 1), ("Siafur", 1), ("Siam", 1), ("Sikh", 1), ("Singhasari", 1), ("Skåne", 1), ("Smefti", 1), ("Snubme", 1), ("Venetian", 1), ("Visigothic", 1), ("Wari", 1), ("Wassoulou", 1), ("Wattasid", 1), ("Weermosh", 1), ("Fatimid", 1), ("Feomaq", 1), ("Flewbio", 1), ("Pandyan", 1), ("Parthian", 1), ("Pifo", 1),]);
            mid.AddRange([("Sokoto", 1), ("Song", 1), ("Songhai", 1), ("Songhay", 1), ("Sphermo", 1), ("Srivijaya", 1), ("Stopmuk", 1), ("Strappam", 1), ("Sui", 1), ("Sukhothai", 1), ("Sultanate", 1), ("Sumerian", 1), ("Sur", 1), ("Sutat", 1), ("Swofdon", 1), ("Saadi", 1), ("Tahirid", 1), ("Tang", 1), ("Tawo", 1), ("Tây Son", 1), ("Tebo", 1), ("Thonburi", 1), ("Thresmousho", 1), ("Tibet", 1), ("Tibetan", 1), ("Tikal", 1), ("Timurid", 1), ("Tlemcen", 1), ("Tokugawa", 1), ("Toltec", 1), ("Toucouleur", 1), ("Toungoo", 1), ("Trebizond", 1), ("Triey Skijan", 1), ("Tsardom", 1), ("Tu'i Tonga", 1), ("Tui Manu’a", 1), ("Turgesh", 1), ("Turkic", 1), ("Twaaptik", 1), ("Uhhu", 1), ("Uhoureo", 1), ("Umayyad", 1), ("Unu", 1), ("Uyghur", 1), ("Uzbek", 1), ("Vandal", 1), ("Varman", 1),]);
            mid.AddRange([("Wezwir", 1), ("Wu Zhou", 1), ("Xia", 1), ("Xianbei", 1), ("Xin", 1), ("Xiognu", 1), ("Xiongnu", 1), ("Yaswun", 1), ("Yuan", 1), ("Zand", 1), ("Zhou", 1), ("Zikil", 1), ("Zimshath", 1), ("Zirid", 1), ("Zogni", 1), ("Zoize", 1), ("Aarloe", 1),]);
            List<(string, int)> midHuman = [("Assyria", 1), ("Assyrian", 1), ("Athens", 1), ("Austrian", 1), ("Aztec", 1), ("Babylonia", 1), ("Babylonian", 1), ("Belgian", 1), ("Bengal", 1), ("Bazil", 1), ("Britannic", 1), ("British", 1), ("Bulgarian", 1), ("Byzantine", 1), ("Danish", 1), ("Dutch", 1), ("Egyptian", 1), ("England", 1), ("Ethioian", 1), ("France", 1), ("Frankish", 1), ("French", 1), ("Georgia", 1), ("German", 1), ("Haiti", 1), ("Inca", 1), ("Indian", 1), ("Israel", 1), ("Italin", 1), ("Korean", 1), ("Latin", 1), ("Macedonian", 1), ("Mongol", 1), ("Napoleonic", 1), ("Norway", 1), ("Norwegian", 1), ("Ottoman", 1), ("Portugese", 1), ("Portuguese", 1), ("Roman", 1), ("Russian", 1), ("Scottish", 1), ("Serbian", 1), ("Spanish", 1), ("Spartan", 1), ("Sweden", 1), ("Swedish", 1), ("Armenian", 1), ("Baltikum", 1), ("Japanese", 1), ("Thessalonica", 1), ("Vietnam", 1), ("Zimbabwe", 1), ("Zulu", 1)];
            List<(string, int)> midNonhuman = [("Eternia", 1), ("Giania", 1), ("Florania", 1), ("Aghlabids", 1), ("Aiwuk", 1), ("Aksumite", 1), ("Akwamu", 1), ("Alaouite", 1), ("Almohad", 1), ("Almoravid", 1), ("Aq Qoyunlu", 1), ("Aragon", 1), ("Ashanti", 1), ("Aulikara", 1), ("Ayutthaya", 1), ("Ayyubid", 1), ("Ghaznavid", 1), ("Hoysala", 1), ("Khwarazmian", 1), ("Knaupzond", 1), ("Mlechchha", 1), ("Ograq", 1), ("Purépecha", 1), ("Qoyunlu", 1), ("Schihhish", 1), ("Sphephrasha", 1), ("Swomzuq", 1), ("Vijayanagara", 1), ("Xopfand", 1),];

            List<(string, int)> post = [(" Kingdom", 6), (" Dynasty", 2), (" Empire", 3), (" Region", 2), (" Nation", 3)];
            List<(string, int)> postHuman = [("", 15), (" Republic", 5), (" Alliance", 4), (" State", 3), (" Union", 2), (" Territory", 2)];
            List<(string, int)> postNonhuman = [("", 15), (" Ground", 2), (" Sect", 1), (" Land", 3), ("land", 1), (" Khaganate", 1), (" Sultanate", 1)];
            List<(string, int)> postSmall = [(" Duchy", 1), (" Province", 1), (" Len", 3), (" County", 3)];

            if (r.Id == RaceIds.Human)
            {// Human
                pre.AddRange(preHuman);
                mid.AddRange(midHuman);
                post.AddRange(postHuman);
                if (small)
                    post.AddRange(postSmall);
            }
            else
            {//  Nonhuman
                pre.AddRange(preNonhuman);
                mid.AddRange(midNonhuman);
                post.AddRange(postNonhuman);
            }
            string chosenPost = Choose(post, seed);
            string chosenMid = Choose(mid, seed);
            if (r.Id == RaceIds.Human)
            {// Human
                if (big && !string.IsNullOrWhiteSpace(chosenPost))
                    pre.AddRange(preBig);
            }
            else
            {//  Nonhuman
                if (!string.IsNullOrWhiteSpace(chosenPost))
                    pre.AddRange(preNumber);
            }
            string chosenPre = Choose(pre, seed);
            if (!string.IsNullOrWhiteSpace(chosenPre) && !string.IsNullOrWhiteSpace(chosenPost))
                if (chosenPre.Trim()[..2].Equals(chosenPost.Trim()[..2]))
                    chosenPre = ""; // pre and post can not start with the same 3 letters

            if (reg.RulingFamilyName is not null && !string.IsNullOrWhiteSpace(chosenPost))
            {
                Random ran = new(seed);
                int x = ran.Next(6);
                if (x <= 2)
                    chosenMid = reg.RulingFamilyName;
            }
            return (chosenPre + chosenMid + chosenPost).Replace("  ", " ");
        }

        private static string Choose(List<(string, int)> ps, int seed)
        {
            int sum = 0;
            foreach (var p in ps)
                sum += p.Item2;
            Random ran = new(seed);
            int x = ran.Next(sum);
            int runningSum = 0;
            foreach (var p in ps)
            {
                runningSum += p.Item2;
                if (x <= runningSum)
                    return p.Item1;
            }
            return "";
        }

        public static string GenerateCityName(int seed, Location loc, Region reg, Race race)
        {
            int minSettledPop = 10;
            List<(string, int)> pre = [];
            List<(string, int)> mid = [];
            List<(string, int)> post = [];
            var height = Height.GetIntervalName(loc.Height);
            var biomId = loc.BiomId;
            var geoId = loc.GeologyId;
            var temp = Temperature.GetIntervalName(loc.Climate?.AverageTemperature);
            var precip = Precipitation.GetIntervalName(loc.Climate?.PrecipitationAmount);
            var pop = loc.Population;
            //var historicNames = reg.HistoricNames;
            //var r race = reg.Race;
            if (minSettledPop < pop)
            {
                // Capital, City, Town, Village, Settlement,

                // Add fitting prefixes
                // Add fitting middles
                // Add fitting postfixes
                return "";
            }
            else if (0 < pop)
            {
                // House,Farm,Hut,Tents,Cave,Camp,Dvelling,Shelter,Hideout
                return "";
            }
            else
            {
                return "";
            }
        }

        public static string GenerateName(int seed, Language language, NameSex sex)
        {
            string name = "";
            switch (language, sex)
            {
                case (Language.Common, NameSex.Male):
                    name = Generate(seed, CommonMaleNameSet);
                    name += " " + Generate(seed, CommonSurnameSet);
                    break;
                case (Language.Common, NameSex.Female):
                    name = Generate(seed, CommonFemaleNameSet);
                    name += " " + Generate(seed, CommonSurnameSet);
                    break;
                case (Language.Exotic, NameSex.Male):
                    name = Generate(seed, ExoticMaleNameSet);
                    name += " " + Generate(seed, ExoticSurnameSet);
                    break;
                case (Language.Exotic, NameSex.Female):
                    name = Generate(seed, ExoticFemaleNameSet);
                    name += " " + Generate(seed, ExoticSurnameSet);
                    break;
                case (Language.Dark, NameSex.Male):
                    name = Generate(seed, DarkMaleNameSet);
                    name += " " + Generate(seed, DarkSurnameSet);
                    break;
                case (Language.Dark, NameSex.Female):
                    name = Generate(seed, DarkFemaleNameSet);
                    name += " " + Generate(seed, DarkSurnameSet);
                    break;
            }
            return name;
        }

        public static string GenerateFirstName(int seed, Language language, NameSex sex)
        {
            string name = "";
            switch (language, sex)
            {
                case (Language.Common, NameSex.Male):
                    name = Generate(seed, CommonMaleNameSet);
                    break;
                case (Language.Common, NameSex.Female):
                    name = Generate(seed, CommonFemaleNameSet);
                    break;
                case (Language.Exotic, NameSex.Male):
                    name = Generate(seed, ExoticMaleNameSet);
                    break;
                case (Language.Exotic, NameSex.Female):
                    name = Generate(seed, ExoticFemaleNameSet);
                    break;
                case (Language.Dark, NameSex.Male):
                    name = Generate(seed, DarkMaleNameSet);
                    break;
                case (Language.Dark, NameSex.Female):
                    name = Generate(seed, DarkFemaleNameSet);
                    break;
            }
            return name;
        }

        public static async Task<string> GenerateSurName(int seed, RaceIds raceId)
        {
            return await GenerateSurName(seed, World.Races[raceId].Language);
        }

        public static string Genitive(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return name;
            if(name.EndsWith("s"))
                return name+"'";
            else
                return name+"'s";
        }

        public static async Task<string> GenerateSurName(int seed, Language language)
        {
            var timer = DateTime.Now;
            while (!Loaded)
                if(!await InitAsync() && timer.AddMinutes(1)<DateTime.Now)
                    throw new Exception("Init time out!");
            string name = "";
            switch (language)
            {
                case Language.Common:
                    name = Generate(seed, CommonSurnameSet);
                    break;
                case Language.Exotic:
                    name = Generate(seed, ExoticSurnameSet);
                    break;
                case Language.Dark:
                    name = Generate(seed, DarkSurnameSet);
                    break;
            }
            return name;
        }

        public static string Generate(int seed, SortedDictionary<string, TokenSet> set)
        {
            if(set.Count == 0)
            {
                return "No set - no name!";
            }
            var rand = new Random(seed);
            string name = "£";
            while (name.Length < 30 && !name.EndsWith('$'))
            {
                int aLength = 3;
                if (name.Length < 3)
                    aLength = name.Length;
                string a = name.Substring(name.Length - aLength, aLength);
                char? next = GetNext(a, rand, set);

                if (next is null)
                    if (a.Length < 2)
                        name += "#";
                    else
                    {
                        string aS = a[2..];
                        char? nextS = GetNext(a, rand, set);
                        name += nextS;
                    }
                else
                    name += next;
            }
            var cleanName = name.Trim('£').Trim('$');
            cleanName = cleanName[..1].ToUpper() + cleanName[1..];
            if (cleanName.Contains('-'))
            {
                var parts = cleanName.Split('-');
                cleanName = parts[0];
                for (int i = 1; i < parts.Length; i++)
                    cleanName += "-" + parts[i][..1].ToUpper() + parts[i][1..];
            }
            return cleanName;
        }

        public static char? GetNext(string a, Random rand, SortedDictionary<string, TokenSet> set)
        {
            if (set.TryGetValue(a, out TokenSet? value))
            {
                float f = (float)rand.NextDouble();
                float runningSum = 0;
                foreach (var x in value.NextTokenFreq)
                {
                    runningSum += x.F;
                    if (f < runningSum)
                        return x.C;
                }
                return null;
            }
            else
            {
                if (a.Length < 2)
                    return null;
                else
                    return GetNext(a[..^1], rand, set);
            }
        }
    }

    public enum NameSex
    {
        None,
        Female,
        Male,
    }

    public class TokenFrequency
    {
        public char C { get; set; }
        public float F { get; set; }
    }

    public class TokenSet
    {
        public string PreTokens { get; set; } = "";
        public List<TokenFrequency> NextTokenFreq { get; set; } = [];
    }
}