namespace Model
{
    /// <summary>
    /// The Game World
    /// </summary>
    public static class World
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool IsInitialized { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public static bool IsInitializing { get; set; } = false;

        /// <summary>
        /// The random seed of the world. Used for all. Defaults to 0
        /// </summary>
        public static int Seed { get; set; }

        /// <summary>
        /// Bioms
        /// </summary>
        public static Dictionary<BiomIds, Biom> Bioms { get; set; } = [];

        /// <summary>
        /// Plants, flower, scrubs ect.
        /// </summary>
        public static Dictionary<PlantIds, Plant> Plants { get; set; } = [];

        /// <summary>
        /// Trees
        /// </summary>
        public static Dictionary<TreeIds, Tree> Trees { get; set; } = [];

        /// <summary>
        /// Races
        /// </summary>
        public static Dictionary<RaceIds, Race> Races { get; set; } = [];

        /// <summary>
        /// Named wind directions
        /// </summary>
        public static Dictionary<int, Direction> Directions { get; set; } = [];

        /// <summary>
        /// Named weather events
        /// </summary>
        public static Dictionary<int, WeatherEvent> WeatherEvents { get; set; } = [];

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<NeedIds, Need> Needs { get; set; } = [];

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<CraftIds, Craft> Crafts { get; set; } = [];

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<ProfessionIds, Profession> Professions { get; set; } = [];

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<GeologyId, Geology> Geologies { get; set; } = [];

        /// <summary>
        /// HairColors
        /// </summary>
        public static Dictionary<HairColorIds, HairColor> HairColors { get; set; } = [];

        /// <summary>
        /// Outfit and dresses
        /// </summary>
        public static Dictionary<DressIds, Dress> Dresses { get; set; } = [];

        /// <summary>
        /// Outfit and dresses
        /// </summary>
        public static Dictionary<JewelryIds, Jewelry> Jewelry { get; set; } = [];

        /// <summary>
        /// Hair Styles
        /// </summary>
        public static Dictionary<HairStyleIds, HairStyle> HairStyles { get; set; } = [];

        /// <summary>
        /// Hair Styles
        /// </summary>
        public static Dictionary<DressColorIds, DressColor> DressColors { get; set; } = [];

        /// <summary>
        /// Skin Colors
        /// </summary>
        public static Dictionary<SkinColorIds, SkinColor> SkinColors { get; set; } = [];

        /// <summary>
        /// A dictionary of needs - each with a list of the crafts that help fullfill that need.
        /// </summary>
        public static Dictionary<NeedIds, List<CraftIds>> CaftsHelpingNeeds { get; set; } = []; // key = NeedId, value = List<CraftIds>

        /// <summary>
        /// A dictionary of crafts - each with a list of the neds that help it helps to fullfill.
        /// </summary>
        public static Dictionary<CraftIds, List<NeedIds>> NeedsBeingHelpedByCrafts { get; set; } = []; // key = CraftId, value = List<NeedIds>

        /// <summary>
        /// Get a debug object like the static class world
        /// </summary>
        public static object GetDebugWorld()
        {
            return new
            {
                seed = Seed,
                bioms = Bioms,
                directions = Directions,
                weatherEvents = WeatherEvents,
                needs = Needs,
                crafts = Crafts,
                professions = GetDebugProfessions().ToArray(),
                caftsHelpingNeeds = GetDebugCraftsHelpingNeeds().ToArray(),
                needsBeingHelpedByCrafts = GetDebugNeedsBeingHelpedByCrafts().ToArray(),
                geologies = Geologies,
                races = Races
            };
        }

        private static Dictionary<string, List<string>> GetDebugNeedsBeingHelpedByCrafts()
        {
            Dictionary<string, List<string>> result = []; // key = CraftId, value = List<NeedIds>
            foreach (var craftIdWithNeedIdList in NeedsBeingHelpedByCrafts)
            {
                List<string> listOfNeedNames = [];
                foreach (var needId in craftIdWithNeedIdList.Value)
                    listOfNeedNames.Add(Needs[(NeedIds)needId].Name);
                result.Add(Crafts[(CraftIds)craftIdWithNeedIdList.Key].Name, listOfNeedNames);
            }
            return result;
        }

        private static Dictionary<string, List<string>> GetDebugCraftsHelpingNeeds()
        {
            Dictionary<string, List<string>> result = []; // key = NeedId, value = List<CraftIds>
            foreach (var needIdWithCraftIdList in CaftsHelpingNeeds)
            {
                List<string> listOfCraftNames = [];
                foreach (var craftId in needIdWithCraftIdList.Value)
                    listOfCraftNames.Add(Crafts[(CraftIds)craftId].Name);
                result.Add(Needs[(NeedIds)needIdWithCraftIdList.Key].Name, listOfCraftNames);
            }
            return result;
        }

        private static List<object> GetDebugProfessions()
        {
            List<object> dps = [];
            foreach (var p in Professions.Values)
                dps.Add(new
                {
                    prof = p,
                    mainCraftNames = p.MainCrafts.Select(x => Crafts[(CraftIds)x].Name).ToList(),
                    secondaryCraftNames = p.SecondaryCrafts.Select(x => Crafts[(CraftIds)x].Name).ToList()
                });
            return dps;
        }

        /// <summary>
        /// Fill the static worlds properties with content
        /// </summary>
        public static void InitWorld()
        {
            if (IsInitialized || IsInitializing)
                return;
            IsInitializing = true;
            InitWind();
            InitWeatherEvents();
            InitNeeds();
            InitCrafts();
            InitCaftsHelpingNeeds();
            InitNeedsBeingHelpedByCrafts();
            InitProfessions();
            InitPlants();
            InitTrees();
            InitBioms();
            InitGeologies();
            InitRaces();
            InitHairColors();
            InitHairStyles();
            InitDressColors();
            InitDresses();
            InitJewelry();
            InitSkinColors();
            IsInitialized = true;
            IsInitializing = false;
        }

        private static void InitPlants()
        {
            Plants.TryAdd(PlantIds.Anemone, new(PlantIds.Anemone)
            {
                Name = "Anemone",
                AltName = "",
                Descriptions = [],
            });
            Plants.TryAdd(PlantIds.Cacti, new(PlantIds.Cacti)
            {
                Name = "Cacti",
                AltName = "",
                Descriptions = [],
            });
            Plants.TryAdd(PlantIds.Cottongrass, new(PlantIds.Cottongrass)
            {
                Name = "Cottongrass",
                AltName = "",
                Descriptions = [],
            });
            Plants.TryAdd(PlantIds.Fern, new(PlantIds.Fern)
            {
                Name = "Fern",
                AltName = "",
                Descriptions = [],
                Rarity = 0.8,
            });
            Plants.TryAdd(PlantIds.Grass, new(PlantIds.Grass)
            {
                Name = "Grass",
                AltName = "",
                Descriptions = [],
                Rarity = 1,
            });
            Plants.TryAdd(PlantIds.Ivy, new(PlantIds.Ivy)
            {
                Name = "Ivy",
                AltName = "",
                Descriptions = [],
            });
            Plants.TryAdd(PlantIds.Lily, new(PlantIds.Lily)
            {
                Name = "Cacti",
                AltName = "",
                Descriptions = [],
            });
            Plants.TryAdd(PlantIds.Lichen, new(PlantIds.Lichen)
            {
                Name = "Licen",
                AltName = "",
                Descriptions = [],
            });
            Plants.TryAdd(PlantIds.Moss, new(PlantIds.Moss)
            {
                Name = "Moss",
                AltName = "",
                Descriptions = [],
                Rarity = 0.8,
            });
            Plants.TryAdd(PlantIds.Orchid, new(PlantIds.Orchid)
            {
                Name = "Orchid",
                AltName = "",
                Descriptions = ["Rainforest flower"],
            });
            Plants.TryAdd(PlantIds.PassionFlower, new(PlantIds.PassionFlower)
            {
                Name = "Passion Flower",
                AltName = "",
                Descriptions = ["Rainforest flower"],
                Rarity = 0.4,
            });
            Plants.TryAdd(PlantIds.Rose, new(PlantIds.Rose)
            {
                Name = "Rose",
                AltName = "",
                Descriptions = [],
                Rarity = 0.25,
            });
            Plants.TryAdd(PlantIds.Scrub, new(PlantIds.Scrub)
            {
                Name = "Scrub",
                AltName = "",
                Descriptions = [],
            });
            Plants.TryAdd(PlantIds.WaterLilies, new(PlantIds.WaterLilies)
            {
                Name = "Water Lilies",
                AltName = "",
                Descriptions = [],
                Rarity = 0.4,
            });
        }

        private static void InitTrees()
        {
            Trees.TryAdd(TreeIds.Acai, new(TreeIds.Acai)
            {
                Name = "Acai Palm",
                AltName = "Acai",
                Descriptions = ["rain forest"],
                Fruit = "acai berries",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Apple, new(TreeIds.Apple)
            {
                Name = "Apple Tree",
                AltName = "Malus pumila",
                Descriptions = ["The tree originated in Central Asia", "The apple is a deciduous tree, generally standing 2 to 10 metres tall", "Apple trees may naturally have a rounded to erect crown with a dense canopy of leaves. The bark of the trunk is dark gray or gray-brown, but young branches are reddish or dark-brown with a smooth texture."],
                Fruit = "Apple",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Apricot, new(TreeIds.Apricot)
            {
                Name = "Apricot Tree",
                AltName = "Apricot",
                Descriptions = ["The apricot is a small tree, 8 to 12 metres tall, with a trunk up to 40 centimetres in diameter and a dense, spreading canopy", "Grown in  plains"],
                Fruit = "Apricot",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Ash, new(TreeIds.Ash)
            {
                Name = "Ash Tree",
                AltName = "Ash",
                Descriptions = ["deciduous trees", "widespread throughout much of Europe, Asia, and North America."],
                Fruit = "",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Aspen, new(TreeIds.Aspen)
            {
                Name = "Aspen Tree",
                AltName = "Aspen",
                Descriptions = ["Aspen trees are all native to cold regions with cool summers, in the north of the northern hemisphere, extending south at high-altitude areas such as mountains or high plains. They are all medium-sized deciduous trees reaching 15 to 30 m tall"],
                Fruit = "",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Avocado, new(TreeIds.Avocado)
            {
                Name = "Avocado Tree",
                AltName = "Avocado",
                Descriptions = ["a medium-sized, evergreen tree", "It is native to the Americas. The tree likely originated in the highlands bridging south-central Mexico and Guatemala.", "cultivated in the tropical and Mediterranean climates"],
                Fruit = "Avocado",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Baobab, new(TreeIds.Baobab)
            {
                Name = "Baobab Tree",
                AltName = "",
                Descriptions = ["africa"],
                Fruit = "",
                Rarity = 0.6,
            });
            Trees.TryAdd(TreeIds.Basswood, new(TreeIds.Basswood)
            {
                Name = "Basswood Tree",
                AltName = "Tilia americana",
                Descriptions = ["North east US"],
                Fruit = "",
                Rarity = 0.3,
            });
            Trees.TryAdd(TreeIds.Beech, new(TreeIds.Beech)
            {
                Name = "Beech Tree",
                AltName = "Fagus",//Bøg
                Descriptions = [" native to temperate Eurasia and North America"],
                Fruit = "mast",
                Rarity = 0.6,
            });
            Trees.TryAdd(TreeIds.Birch, new(TreeIds.Birch)
            {
                Name = "Birch Tree",
                AltName = "Fagus",
                Descriptions = ["A birch is a thin-leaved deciduous hardwood tree", "are widespread in the Northern Hemisphere, particularly in northern areas of temperate climates and in boreal climates"],
                Fruit = "",
                Rarity = 0.7,
            });
            Trees.TryAdd(TreeIds.BlackMangrove, new(TreeIds.BlackMangrove)
            {
                Name = "Black Mangrove Tree",
                AltName = "Black Mangrove",
                Descriptions = [""],
                Fruit = "",
                Rarity = 0.75,
            });
            Trees.TryAdd(TreeIds.ButternutTree, new(TreeIds.ButternutTree)
            {
                Name = "Butternut Tree",
                AltName = "white walnut",
                Descriptions = ["native to the eastern United States and southeast Canada"],
                Fruit = "Butternut",
                Rarity = 0.3,
            });
            Trees.TryAdd(TreeIds.CacaoTree, new(TreeIds.CacaoTree)
            {
                Name = "Cacao Tree",
                AltName = "Cacao",
                Descriptions = ["Rainforest tree"],
                Fruit = "Cacao",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Curatella, new(TreeIds.Curatella)
            {
                Name = "Curatella Tree",
                AltName = "Curatella",
                Descriptions = ["savanah"],
                Fruit = "cashew nut",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Cedar, new(TreeIds.Cedar)
            {
                Name = "Cedar Tree",
                AltName = "Cedrus",
                Descriptions = ["coniferous trees in the plant family Pinaceae. They are native to the mountains of the western Himalayas and the Mediterranean region, occurring at altitudes of 1,500 to 3,200 m"],
                Fruit = "",
            });
            Trees.TryAdd(TreeIds.Cypress, new(TreeIds.Cypress)
            {
                Name = "Cypress Tree",
                AltName = "Cypress",
                Descriptions = ["typically found in warm-temperate and subtropical regions of Asia, Europe, and North America.", "Cypress trees typically reach heights of up to 25 metres and exhibit a pyramidal form, particularly in their youth. Many are characterised by their needle-like, evergreen foliage and acorn-like seed cones"],
                Fruit = "",
            });
            Trees.TryAdd(TreeIds.Elm, new(TreeIds.Elm)
            {
                Name = "Elm Tree",
                AltName = "Elm",
                Descriptions = ["deciduous and semi-deciduous trees ", "inhabiting the temperate and tropical-montane "],
                Fruit = "",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Fig, new(TreeIds.Fig)
            {
                Name = "Fig Tree",
                AltName = "Fig",
                Descriptions = ["a small deciduous tree or large shrub growing up to 7 to 10 m", "tropical and subtropical They tolerate moderate seasonal frost and can be grown even in hot-summer continental climates"],
                Fruit = "Fig",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Fir, new(TreeIds.Fir)
            {
                Name = "Fir Tree",
                AltName = "Fir",
                Descriptions = ["Firs are evergreen coniferous trees belonging to the genus Abies"],
                Fruit = "",
                Rarity = 0.6,
            });
            Trees.TryAdd(TreeIds.Ginkgo, new(TreeIds.Ginkgo)
            {
                Name = "Ginkgo Tree",
                AltName = "Ginkgo",
                Descriptions = ["East Asia."],
                Fruit = "",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Kapok, new(TreeIds.Kapok)
            {
                Name = "Kapok Tree",
                AltName = "Kapok",
                Descriptions = ["rainforest."],
                Fruit = "",
                Rarity = 0.8,
            });
            Trees.TryAdd(TreeIds.Lemmon, new(TreeIds.Lemmon)
            {
                Name = "Lemmon Tree",
                AltName = "Lemmon",
                Descriptions = ["small evergreen tree ", "native to Asia, primarily Northeast India"],
                Fruit = "Lemmon",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Magnolia, new(TreeIds.Magnolia)
            {
                Name = "Magnolia Tree",
                AltName = "Magnolia",
                Descriptions = ["Magnolias are spreading evergreen or deciduous trees or shrubs characterised by large fragrant flowers, which may be bowl-shaped or star-shaped, in shades of white, pink, purple, green, or yellow"],
                Fruit = "",
                Rarity = 0.3,
            });
            Trees.TryAdd(TreeIds.Mango, new(TreeIds.Mango)
            {
                Name = "Mango Tree",
                AltName = "Mangifera indica",
                Descriptions = ["It is a large fruit tree, capable of growing to a height of 30 metres. There are two distinct genetic populations in modern mangoes the Indian type and the Southeast Asian type"],
                Fruit = "Mango",
            });
            Trees.TryAdd(TreeIds.Maple, new(TreeIds.Maple)
            {
                Name = "Maple Tree",
                AltName = "Maple",
                Descriptions = ["Temperate"],
                Fruit = "Maple syrup",
            });
            Trees.TryAdd(TreeIds.Oak, new(TreeIds.Oak)
            {
                Name = "Oak Tree",
                AltName = "Oak",
                Descriptions = ["Temperate"],
                Fruit = "Acorn",
                Rarity = 0.6,
            });
            Trees.TryAdd(TreeIds.Olive, new(TreeIds.Olive)
            {
                Name = "Olive Tree",
                AltName = "Olive",
                Descriptions = [" Mediterranean "],
                Fruit = "Olive",
                Rarity = 0.5,
            });
            Trees.TryAdd(TreeIds.Palm, new(TreeIds.Palm)
            {
                Name = "Palm Tree",
                AltName = "Palm",
                Descriptions = [" Tropical "],
                Fruit = "",
                Rarity = 0.6,
            });
            Trees.TryAdd(TreeIds.Pecan, new(TreeIds.Pecan)
            {
                Name = "Pecan Tree",
                AltName = "Pecan",
                Descriptions = [" southern United States and northern Mexico "],
                Fruit = "Pecan nut",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Pine, new(TreeIds.Pine)
            {
                Name = "Pine Tree",
                AltName = "Pine",
                Descriptions = ["a conifer tree"],
                Fruit = "",
                Rarity = 0.7,
            });
            Trees.TryAdd(TreeIds.Plum, new(TreeIds.Plum)
            {
                Name = "Plum Tree",
                AltName = "Plum",
                Descriptions = ["a conifer tree"],
                Fruit = "Plum",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Poplar, new(TreeIds.Poplar)
            {
                Name = "Poplar Tree",
                AltName = "Poplar",
                Descriptions = ["a tall deciduous tree"],
                Fruit = "",
            });
            Trees.TryAdd(TreeIds.Rambutan, new(TreeIds.Rambutan)
            {
                Name = "Rambutan Tree",
                AltName = "Rambutan",
                Descriptions = ["a medium-sized tropical tree ", "It is an evergreen tree growing to a height of 15 to 24 metres"],
                Fruit = "Rambutan",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.RedAlder, new(TreeIds.RedAlder)
            {
                Name = "Red Alder Tree",
                AltName = "Alnus rubra",
                Descriptions = ["a deciduous north west coastn US and canada"],
                Fruit = "",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.RedMangrove, new(TreeIds.RedMangrove)
            {
                Name = "Red Mangrove Tree",
                AltName = "Rhizophora stylosa",
                Descriptions = [""],
                Fruit = "",
                Rarity = 0.6,
            });
            Trees.TryAdd(TreeIds.RubberTree, new(TreeIds.RubberTree)
            {
                Name = "Rubber Tree",
                AltName = "",
                Descriptions = ["rainforrest"],
                Fruit = "Rubber",
                Rarity = 0.3,
            });
            Trees.TryAdd(TreeIds.Teak, new(TreeIds.Teak)
            {
                Name = "Teak Tree",
                AltName = "Teak",
                Descriptions = ["a tropical hardwood tree "],
                Fruit = "",
            });
            Trees.TryAdd(TreeIds.TulipTree, new(TreeIds.TulipTree)
            {
                Name = "Tulip Tree",
                AltName = "saddle-leaf tree",
                Descriptions = ["grow to great size", "large yellow flowers superficially resembling tulips", "prefer a temperate climate"],
                Fruit = "",
                Rarity = 0.25,
            });
            Trees.TryAdd(TreeIds.Walnut, new(TreeIds.Walnut)
            {
                Name = "Walnut Tree",
                AltName = "Walnut",
                Descriptions = ["grow to great size", "large yellow flowers superficially resembling tulips", "prefer a temperate climate"],
                Fruit = "Walnut",
                Rarity = 0.4,
            });
            Trees.TryAdd(TreeIds.Willow, new(TreeIds.Willow)
            {
                Name = "Willow Tree",
                AltName = "Willow",
                Descriptions = [""],
                Fruit = "",
            });
        }

        private static void InitJewelry()
        {
            Jewelry.Add(JewelryIds.Crown, new(JewelryIds.Crown)
            {
                Name = "a Crown",
                Filter = PersonFilter.GetFilter([ProfessionIds.Ruler], [SexIds.Male]),
            });
            Jewelry.Add(JewelryIds.Diadem, new(JewelryIds.Diadem)
            {
                Name = "a Diadem",
                Filter = PersonFilter.GetFilter([ProfessionIds.Ruler], [SexIds.Female]),
            });
            Jewelry.Add(JewelryIds.GoldPendant, new(JewelryIds.GoldPendant)
            {
                Name = "Gold pendant",
                Filter = PersonFilter.GetFilter([],
                    [SexIds.Female],
                    [LocationWealthIds.Opulent, LocationWealthIds.Prospering],
                    [PersonAgeIds.Adult, PersonAgeIds.Young, PersonAgeIds.Mature, PersonAgeIds.Old]),
            });
            Jewelry.Add(JewelryIds.SilverPendant, new(JewelryIds.SilverPendant)
            {
                Name = "Silver pendant",
                Filter = PersonFilter.GetFilter([],
                    [SexIds.Female],
                    [LocationWealthIds.Prospering, LocationWealthIds.Average],
                    [PersonAgeIds.Adult, PersonAgeIds.Young, PersonAgeIds.Mature, PersonAgeIds.Old]),
            });
            Jewelry.Add(JewelryIds.GoldBracelets, new(JewelryIds.GoldBracelets)
            {
                Name = "Gold bracelets",
                Filter = PersonFilter.GetFilter([],
                    [SexIds.Female],
                    [LocationWealthIds.Opulent, LocationWealthIds.Prospering],
                    [PersonAgeIds.Adult, PersonAgeIds.Young, PersonAgeIds.Mature, PersonAgeIds.Old]),
            });
            Jewelry.Add(JewelryIds.BearToothNecklace, new(JewelryIds.BearToothNecklace)
            {
                Name = "a animal tooth necklace",
                Filter = PersonFilter.GetFilter(
                    [RaceIds.Goblin, RaceIds.Human, RaceIds.Lizard, RaceIds.Minotaur, RaceIds.Orc, RaceIds.Thiefling],
                    [SexIds.Male, SexIds.Female],
                    [LocationWealthIds.Poor, LocationWealthIds.Average],
                    [PersonAgeIds.Adult, PersonAgeIds.Young, PersonAgeIds.Mature, PersonAgeIds.Old]),
            });
        }

        private static void InitDresses()
        {
            Dresses.TryAdd(DressIds.BlouseAndSkirt, new(DressIds.BlouseAndSkirt)
            {
                Name = "Blouse and skirt",
                NameWithColorKey = " a blouse and XXX skirt",
                Filter = PersonFilter.GetFilter([RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Fae, RaceIds.Nymph], [SexIds.Female]),
            });
            Dresses.TryAdd(DressIds.BastSkirtAndShortTop, new(DressIds.BastSkirtAndShortTop)
            {
                Name = "Bast skirt and a short top",
                NameWithColorKey = "a bast skirt and a short XXX top",
                Filter = PersonFilter.GetFilter(Temperature.NamedInterval.HotAndVeryHot, [SexIds.Female], [PersonAgeIds.Adult, PersonAgeIds.Young],
                    [RaceIds.Hare, RaceIds.Human, RaceIds.Fae, RaceIds.Nymph]),
            });
            Dresses.TryAdd(DressIds.CookApron, new(DressIds.CookApron)
            {
                Name = "A cooks apron",
                NameWithColorKey = "XXX clothes and a cooks apron",
                Filter = PersonFilter.GetFilter([ProfessionIds.Cook], [SexIds.Female, SexIds.Male]),
            });
            Dresses.TryAdd(DressIds.WorkApron, new(DressIds.WorkApron)
            {
                Name = "A work apron",
                NameWithColorKey = "XXX clothes and a leather apron",
                Filter = PersonFilter.GetFilter([ProfessionIds.Blacksmith, ProfessionIds.Carpenter, ProfessionIds.Tailor, ProfessionIds.Servant], [SexIds.Female, SexIds.Male]),
            });
            Dresses.TryAdd(DressIds.LeatherClothing, new(DressIds.LeatherClothing)
            {
                Name = "Leather Clothing",
                NameWithColorKey = "XXX Leather Clothing",
                Filter = PersonFilter.GetFilter([ProfessionIds.Farmer,ProfessionIds.Fisherman,ProfessionIds.Carpenter,ProfessionIds.Tailor,ProfessionIds.InnKeeper,ProfessionIds.Guard,
                ProfessionIds.Trader,ProfessionIds.Herbalist,ProfessionIds.Miner,ProfessionIds.Blacksmith,ProfessionIds.Shepherd,ProfessionIds.Servant],
                [SexIds.Female, SexIds.Male]),
            });
            Dresses.TryAdd(DressIds.HuntingClothes, new(DressIds.HuntingClothes)
            {
                Name = "Hunting clothes",
                NameWithColorKey = "XXX hunting clothes",
                Filter = PersonFilter.GetFilter([ProfessionIds.Hunter], [SexIds.Female, SexIds.Male]),
            });
            Dresses.TryAdd(DressIds.ShortDress, new(DressIds.ShortDress)
            {
                Name = "Short dress",
                NameWithColorKey = "a short XXX dress",
                Filter = PersonFilter.GetFilter([ProfessionIds.Bard, ProfessionIds.InnKeeper, ProfessionIds.Tailor, ProfessionIds.Trader, ProfessionIds.Hunter, ProfessionIds.Courtesan], [SexIds.Female],
                    [PersonAgeIds.Young, PersonAgeIds.Adult]),
            });
            Dresses.TryAdd(DressIds.LongDress, new(DressIds.LongDress)
            {
                Name = "Long dress",
                NameWithColorKey = "a long XXX dress",
                Filter = PersonFilter.GetFilter([ProfessionIds.Bard, ProfessionIds.InnKeeper, ProfessionIds.Tailor, ProfessionIds.Trader, ProfessionIds.Hunter, ProfessionIds.Ruler,
                ProfessionIds.Cook, ProfessionIds.Farmer, ProfessionIds.Fisherman, ProfessionIds.Herbalist, ProfessionIds.Shepherd, ProfessionIds.Courtesan], [SexIds.Female]),
            });
            Dresses.TryAdd(DressIds.LongPlainRobe, new(DressIds.LongPlainRobe)
            {
                Name = "Long plain robe",
                NameWithColorKey = "a long XXX plain robe",
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Elve, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Lizard, RaceIds.Nymph, RaceIds.Satyr,
                RaceIds.Thiefling],
                [LocationWealthIds.Destitude, LocationWealthIds.Poor, LocationWealthIds.Average]),
            });
            Dresses.TryAdd(DressIds.ThickFurCoat, new(DressIds.ThickFurCoat)
            {
                Name = "Thick Fur Coat",
                NameWithColorKey = "a thick XXX fur coat",
                Filter = PersonFilter.GetFilter(Temperature.NamedInterval.ColdAndVeryCold, [SexIds.Female, SexIds.Male], [PersonAgeIds.Young, PersonAgeIds.Adult, PersonAgeIds.Mature, PersonAgeIds.Old],
                [RaceIds.Dwarf, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Human, RaceIds.Nymph, RaceIds.Thiefling]),
            });
            Dresses.TryAdd(DressIds.Chainmail, new(DressIds.Chainmail)
            {
                Name = "Chain mail",
                NameWithColorKey = "a Chainmail with XXX pants",
                Filter = PersonFilter.GetFilter([PersonAgeIds.Young, PersonAgeIds.Adult, PersonAgeIds.Mature],
                [ProfessionIds.Guard, ProfessionIds.Officer, ProfessionIds.Ruler],
                [RaceIds.Dwarf, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Human, RaceIds.Centaur, RaceIds.Thiefling, RaceIds.Elve, RaceIds.Lizard, RaceIds.Minotaur, RaceIds.Orc], [SexIds.Female, SexIds.Male]),
            });
            Dresses.TryAdd(DressIds.JacketAndPants, new(DressIds.JacketAndPants)
            {
                Name = "Jacket And Pants",
                NameWithColorKey = "a XXX jacket and XXX pants",
                Filter = PersonFilter.GetFilter([PersonAgeIds.Young, PersonAgeIds.Adult, PersonAgeIds.Mature, PersonAgeIds.Old],
                [ProfessionIds.Bard, ProfessionIds.Officer, ProfessionIds.Ruler, ProfessionIds.InnKeeper, ProfessionIds.Monk, ProfessionIds.Tailor, ProfessionIds.Wizard, ProfessionIds.Trader],
                [RaceIds.Haflings, RaceIds.Human, RaceIds.Fae, RaceIds.Thiefling, RaceIds.Elve], [SexIds.Male]),
            });
            Dresses.TryAdd(DressIds.Rags, new(DressIds.Rags)
            {
                Name = "Rags",
                NameWithColorKey = "torn XXX rags",
                Filter = PersonFilter.GetFilter([PersonAgeIds.Young, PersonAgeIds.Adult, PersonAgeIds.Mature, PersonAgeIds.Old],
                [ProfessionIds.Beggar, ProfessionIds.Farmer, ProfessionIds.Miner, ProfessionIds.Servant],
                [RaceIds.Haflings, RaceIds.Human, RaceIds.Fae, RaceIds.Thiefling, RaceIds.Elve], [SexIds.Male, SexIds.Female],
                [LocationWealthIds.Destitude, LocationWealthIds.Poor]),
            });
            Dresses.TryAdd(DressIds.PlateArmor, new(DressIds.PlateArmor)
            {
                Name = "Plate Amor",
                NameWithColorKey = "a full plate amor with XXX decorations",
                Filter = PersonFilter.GetFilter([PersonAgeIds.Adult, PersonAgeIds.Mature],
                [ProfessionIds.Ruler, ProfessionIds.Officer,],
                [RaceIds.Human, RaceIds.Elve, RaceIds.Orc, RaceIds.Dwarf], [SexIds.Male],
                [LocationWealthIds.Opulent, LocationWealthIds.Prospering]),
            });
            Dresses.TryAdd(DressIds.ScaleAmor, new(DressIds.ScaleAmor)
            {
                Name = "Scale amor",
                NameWithColorKey = "a scale amor in XXX color",
                Filter = PersonFilter.GetFilter([PersonAgeIds.Young, PersonAgeIds.Adult, PersonAgeIds.Mature],
                [ProfessionIds.Officer, ProfessionIds.Guard, ProfessionIds.Hunter],
                [RaceIds.Human, RaceIds.Elve, RaceIds.Orc, RaceIds.Lizard, RaceIds.Mer, RaceIds.Goblin], [SexIds.Male, SexIds.Female],
                [LocationWealthIds.Average, LocationWealthIds.Prospering]),
            });
        }

        private static void InitDressColors()
        {
            DressColors.TryAdd(DressColorIds.Beige, new(DressColorIds.Beige)
            {
                Name = "Beige",
                Filter = PersonFilter.GetFilter([RaceIds.Centaur, RaceIds.Dwarf, RaceIds.Dwarf, RaceIds.Elve, RaceIds.Ent, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human,
                RaceIds.Lizard, RaceIds.Minotaur, RaceIds.Orc, RaceIds.Satyr]),
            });
            DressColors.TryAdd(DressColorIds.Black, new(DressColorIds.Black)
            {
                Name = "Black",
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Human, RaceIds.Lizard, RaceIds.Minotaur, RaceIds.Orc, RaceIds.Satyr, RaceIds.Thiefling]),
            });
            DressColors.TryAdd(DressColorIds.Blue, new(DressColorIds.Blue)
            {
                Name = "Blue",
            });
            DressColors.TryAdd(DressColorIds.Brown, new(DressColorIds.Brown)
            {
                Name = "Brown",
                Filter = PersonFilter.GetFilter([RaceIds.Centaur, RaceIds.Dwarf, RaceIds.Dwarf, RaceIds.Ent, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Lizard, RaceIds.Minotaur,
                RaceIds.Orc, RaceIds.Satyr, RaceIds.Goblin]),
            });
            DressColors.TryAdd(DressColorIds.Gold, new(DressColorIds.Gold)
            {
                Name = "Gold",
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Centaur, RaceIds.Elve, RaceIds.Fae, RaceIds.Human, RaceIds.Lizard, RaceIds.Minotaur, RaceIds.Hare, RaceIds.Satyr,
                RaceIds.Thiefling, RaceIds.Mer], [SexIds.Female, SexIds.Male],
                [LocationWealthIds.Opulent]),
            });
            DressColors.TryAdd(DressColorIds.Gray, new(DressColorIds.Gray)
            {
                Name = "Gray",
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Lizard, RaceIds.Mer, RaceIds.Minotaur, RaceIds.Orc]),
            });
            DressColors.TryAdd(DressColorIds.Green, new(DressColorIds.Green)
            {
                Name = "Green",
                Filter = PersonFilter.GetFilter([RaceIds.Centaur, RaceIds.Elve, RaceIds.Ent, RaceIds.Fae, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Lizard,
                RaceIds.Mer, RaceIds.Nymph, RaceIds.Orc, RaceIds.Satyr, RaceIds.Thiefling]),
            });
            DressColors.TryAdd(DressColorIds.Pink, new(DressColorIds.Pink)
            {
                Name = "Pink",
                Filter = PersonFilter.GetFilter([RaceIds.Elve, RaceIds.Fae, RaceIds.Human, RaceIds.Mer, RaceIds.Nymph, RaceIds.Satyr], [SexIds.Female]),
            });
            DressColors.TryAdd(DressColorIds.Purple, new(DressColorIds.Purple)
            {
                Name = "Purple",
            });
            DressColors.TryAdd(DressColorIds.Red, new(DressColorIds.Red)
            {
                Name = "Red",
                Filter = PersonFilter.GetFilter([RaceIds.Centaur, RaceIds.Dwarf, RaceIds.Fae, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Lizard, RaceIds.Mer, RaceIds.Minotaur,
                RaceIds.Nymph, RaceIds.Orc, RaceIds.Satyr, RaceIds.Thiefling]),
            });
            DressColors.TryAdd(DressColorIds.Silver, new(DressColorIds.Silver)
            {
                Name = "Silver",
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Centaur, RaceIds.Elve, RaceIds.Fae, RaceIds.Human, RaceIds.Lizard, RaceIds.Minotaur, RaceIds.Hare, RaceIds.Satyr,
                RaceIds.Thiefling, RaceIds.Mer, RaceIds.Minotaur], [SexIds.Female, SexIds.Male],
                [LocationWealthIds.Opulent, LocationWealthIds.Prospering]),
            });
            DressColors.TryAdd(DressColorIds.Yellow, new(DressColorIds.Yellow)
            {
                Name = "Yellow",
            });
            DressColors.TryAdd(DressColorIds.White, new(DressColorIds.White)
            {
                Name = "White",
                Filter = PersonFilter.GetFilter([RaceIds.Elve, RaceIds.Fae, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Mer, RaceIds.Nymph, RaceIds.Thiefling]),
            });
        }

        private static void InitHairStyles()
        {
            HairStyles.TryAdd(HairStyleIds.Afro, new(HairStyleIds.Afro)
            {
                Name = "Afro",
                NameWithColorKey = "XXX afro hair",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Thiefling, RaceIds.Satyr, RaceIds.Minotaur]),
            });
            HairStyles.TryAdd(HairStyleIds.Bald, new(HairStyleIds.Bald)
            {
                Name = "Bald",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Orc, RaceIds.Goblin, RaceIds.Lizard], [SexIds.Male]),
            });
            HairStyles.TryAdd(HairStyleIds.BraidedLongHair, new(HairStyleIds.BraidedLongHair)
            {
                Name = "Long braided Hair",
                NameWithColorKey = "Long XXX braided Hair",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Elve, RaceIds.Fae, RaceIds.Nymph], [SexIds.Female]),
            });
            HairStyles.TryAdd(HairStyleIds.Dreadlocks, new(HairStyleIds.Dreadlocks)
            {
                Name = "Dreadlocks",
                NameWithColorKey = "XXX Dreadlocks",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Orc, RaceIds.Minotaur]),
            });
            HairStyles.TryAdd(HairStyleIds.FrenchBraid, new(HairStyleIds.FrenchBraid)
            {
                Name = "Hair in a french braid",
                NameWithColorKey = "XXX hair in a french braid",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Elve, RaceIds.Fae, RaceIds.Nymph], [SexIds.Female]),
            });
            HairStyles.TryAdd(HairStyleIds.LongHair, new(HairStyleIds.LongHair)
            {
                Name = "Long hair",
                NameWithColorKey = "Long XXX hair",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Elve, RaceIds.Fae, RaceIds.Nymph, RaceIds.Haflings, RaceIds.Mer, RaceIds.Thiefling], [SexIds.Female]),
            });
            HairStyles.TryAdd(HairStyleIds.LongUpdo, new(HairStyleIds.LongUpdo)
            {
                Name = "Long updo hair",
                NameWithColorKey = "Long XXX updo hair",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Elve, RaceIds.Fae, RaceIds.Nymph, RaceIds.Haflings, RaceIds.Thiefling], [SexIds.Female]),
            });
            HairStyles.TryAdd(HairStyleIds.Messy, new(HairStyleIds.Messy)
            {
                Name = "Messy hair",
                NameWithColorKey = "Messy XXX hair",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Haflings, RaceIds.Thiefling, RaceIds.Dwarf, RaceIds.Ent, RaceIds.Goblin, RaceIds.Hare, RaceIds.Minotaur], [SexIds.Female, SexIds.Male],
                [LocationWealthIds.Destitude, LocationWealthIds.Poor]),
            });
            HairStyles.TryAdd(HairStyleIds.ShortCurly, new(HairStyleIds.ShortCurly)
            {
                Name = "Short curly hair",
                NameWithColorKey = "Short XXX curly hair",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Haflings, RaceIds.Thiefling, RaceIds.Dwarf, RaceIds.Fae, RaceIds.Goblin, RaceIds.Hare, RaceIds.Minotaur, RaceIds.Satyr], [SexIds.Female, SexIds.Male]),
            });
            HairStyles.TryAdd(HairStyleIds.ShortHair, new(HairStyleIds.ShortHair)
            {
                Name = "Short hair",
                NameWithColorKey = "Short XXX hair",
                Filter = PersonFilter.GetFilter([RaceIds.Human, RaceIds.Haflings, RaceIds.Thiefling, RaceIds.Dwarf, RaceIds.Fae, RaceIds.Goblin, RaceIds.Hare, RaceIds.Minotaur], [SexIds.Male]),
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InitSkinColors()
        {
            SkinColors.TryAdd(SkinColorIds.Black, new(SkinColorIds.Black)
            {
                Name = "Black",
                Filter = PersonFilter.GetFilter([RaceIds.Centaur, RaceIds.Human, RaceIds.Minotaur, RaceIds.Satyr, RaceIds.Thiefling]),
            });
            SkinColors.TryAdd(SkinColorIds.Brown, new(SkinColorIds.Brown)
            {
                Name = "Brown",
                Filter = PersonFilter.GetFilter([RaceIds.Centaur, RaceIds.Human, RaceIds.Minotaur, RaceIds.Satyr, RaceIds.Ent, RaceIds.Hare]),
            });
            SkinColors.TryAdd(SkinColorIds.Green, new(SkinColorIds.Green)
            {
                Name = "Green",
                Filter = PersonFilter.GetFilter([RaceIds.Goblin, RaceIds.Orc, RaceIds.Lizard]),
            });
            SkinColors.TryAdd(SkinColorIds.White, new(SkinColorIds.White)
            {
                Name = "White",
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Fae, RaceIds.Haflings, RaceIds.Mer, RaceIds.Nymph, RaceIds.Thiefling]),
            });
        }

        /// <summary>
        /// Defines the hair colorrs
        /// </summary>
        private static void InitHairColors()
        {
            HairColors.TryAdd(HairColorIds.Blond, new(HairColorIds.Blond)
            {
                Name = "Blond",
                Filter = PersonFilter.GetFilter([RaceIds.Elve, RaceIds.Human, RaceIds.Haflings, RaceIds.Mer, RaceIds.Fae, RaceIds.Nymph]),
            });
            HairColors.TryAdd(HairColorIds.Black, new(HairColorIds.Black)
            {
                Name = "Black",
                Filter = PersonFilter.GetFilter([RaceIds.Orc, RaceIds.Human, RaceIds.Centaur, RaceIds.Mer, RaceIds.Goblin, RaceIds.Lizard, RaceIds.Minotaur]),
            });
            HairColors.TryAdd(HairColorIds.Blue, new(HairColorIds.Blue)
            {
                Name = "Blue",
                Filter = PersonFilter.GetFilter([RaceIds.Mer, RaceIds.Fae]),
            });
            HairColors.TryAdd(HairColorIds.Brown, new(HairColorIds.Brown)
            {
                Name = "Brown",
                Filter = PersonFilter.GetFilter([RaceIds.Centaur, RaceIds.Dwarf, RaceIds.Elve, RaceIds.Ent, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Minotaur, RaceIds.Satyr]),
            });
            HairColors.TryAdd(HairColorIds.Golden, new(HairColorIds.Golden)
            {
                Name = "Golden",
                Filter = PersonFilter.GetFilter([RaceIds.Elve, RaceIds.Fae, RaceIds.Elve, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Mer, RaceIds.Nymph]),
            });
            HairColors.TryAdd(HairColorIds.Gray, new(HairColorIds.Gray)
            {
                Name = "Gray",
                Filter = PersonFilter.GetFilter([PersonAgeIds.Old]),
            });
            HairColors.TryAdd(HairColorIds.Green, new(HairColorIds.Green)
            {
                Name = "Green",
                Filter = PersonFilter.GetFilter([RaceIds.Ent, RaceIds.Fae, RaceIds.Goblin, RaceIds.Lizard, RaceIds.Mer, RaceIds.Orc, RaceIds.Thiefling]),
            });
            HairColors.TryAdd(HairColorIds.Pink, new(HairColorIds.Pink)
            {
                Name = "Pink",
                Filter = PersonFilter.GetFilter([RaceIds.Fae, RaceIds.Mer, RaceIds.Thiefling]),
            });
            HairColors.TryAdd(HairColorIds.Purple, new(HairColorIds.Purple)
            {
                Name = "Purple",
                Filter = PersonFilter.GetFilter([RaceIds.Mer, RaceIds.Thiefling]),
            });
            HairColors.TryAdd(HairColorIds.Red, new(HairColorIds.Red)
            {
                Name = "Red",
                Filter = PersonFilter.GetFilter([RaceIds.Centaur, RaceIds.Dwarf, RaceIds.Fae, RaceIds.Goblin, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Lizard, RaceIds.Mer, RaceIds.Minotaur, RaceIds.Nymph, RaceIds.Orc, RaceIds.Satyr, RaceIds.Thiefling]),
            });
            HairColors.TryAdd(HairColorIds.White, new(HairColorIds.White)
            {
                Name = "White",
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Elve, RaceIds.Fae, RaceIds.Fae, RaceIds.Haflings, RaceIds.Hare, RaceIds.Human, RaceIds.Nymph, RaceIds.Mer, RaceIds.Thiefling]),
            });
        }

        private const double cp = 1.0 / 1.414213562373095;
        private const double cs = 0.3826834323650897717284599840304;
        private const double cx = 0.92387953251128675612818318939679;
        /// <summary>
        /// Defines the named directions
        /// </summary>
        private static void InitWind()
        {
            Directions.TryAdd(1, new(1) { Name = "East", LongitudePart = 1, LatitudePart = 0, AngleInRadians = Degree2Rad(0) });
            Directions.TryAdd(2, new(2) { Name = "East-northeast", LongitudePart = cx, LatitudePart = cs, AngleInRadians = Degree2Rad(22.5) });
            Directions.TryAdd(3, new(3) { Name = "Northeast", LongitudePart = cp, LatitudePart = cp, AngleInRadians = Degree2Rad(45) });
            Directions.TryAdd(4, new(4) { Name = "North-northeast", LongitudePart = cs, LatitudePart = cx, AngleInRadians = Degree2Rad(67.5) });
            Directions.TryAdd(5, new(5) { Name = "North", LongitudePart = 0, LatitudePart = 1, AngleInRadians = Degree2Rad(90) });
            Directions.TryAdd(6, new(6) { Name = "North-northwest", LongitudePart = -cs, LatitudePart = cx, AngleInRadians = Degree2Rad(112.5) });
            Directions.TryAdd(7, new(7) { Name = "Northwest", LongitudePart = -cp, LatitudePart = cp, AngleInRadians = Degree2Rad(135) });
            Directions.TryAdd(8, new(8) { Name = "West-northwest", LongitudePart = -cx, LatitudePart = cs, AngleInRadians = Degree2Rad(157.5) });
            Directions.TryAdd(9, new(9) { Name = "West", LongitudePart = -1, LatitudePart = 0, AngleInRadians = Degree2Rad(180) });
            Directions.TryAdd(10, new(10) { Name = "West-southwest", LongitudePart = -cx, LatitudePart = -cs, AngleInRadians = Degree2Rad(202.5) });
            Directions.TryAdd(11, new(11) { Name = "Southwest", LongitudePart = -cp, LatitudePart = -cp, AngleInRadians = Degree2Rad(225) });
            Directions.TryAdd(12, new(12) { Name = "South-southwest", LongitudePart = -cs, LatitudePart = -cx, AngleInRadians = Degree2Rad(247.5) });
            Directions.TryAdd(13, new(13) { Name = "South", LongitudePart = 0, LatitudePart = -1, AngleInRadians = Degree2Rad(270) });
            Directions.TryAdd(14, new(14) { Name = "South-southeast", LongitudePart = cs, LatitudePart = -cx, AngleInRadians = Degree2Rad(292.5) });
            Directions.TryAdd(15, new(15) { Name = "Southeast", LongitudePart = cp, LatitudePart = -cp, AngleInRadians = Degree2Rad(315) });
            Directions.TryAdd(16, new(16) { Name = "East-southeast", LongitudePart = cx, LatitudePart = -cs, AngleInRadians = Degree2Rad(337.5) });
        }
        private static double Degree2Rad(double degree)
        {
            while (degree < 0.0)
                degree += 360.0;
            return Math.PI * degree / 180.0;
        }

        /// <summary>
        /// Defines the named weather events
        /// </summary>
        private static void InitWeatherEvents()
        {
            WeatherEvents.TryAdd(1, new(1) { Name = "Rainbow" });
            WeatherEvents.TryAdd(2, new(2) { Name = "Hailstorm" });
            WeatherEvents.TryAdd(3, new(3) { Name = "Auroa" });
            WeatherEvents.TryAdd(4, new(4) { Name = "Tornado" });
            WeatherEvents.TryAdd(5, new(5) { Name = "Blizzard" });
            WeatherEvents.TryAdd(6, new(6) { Name = "Dense fog" });
            WeatherEvents.TryAdd(7, new(7) { Name = "Hurricane" });
        }

        /// <summary>
        /// Defines the named needs
        /// </summary>
        private static void InitNeeds()
        {
            Needs.TryAdd(NeedIds.Air, new(NeedIds.Air) { Name = "Air" });
            Needs.TryAdd(NeedIds.Temperature, new(NeedIds.Temperature) { Name = "Temperature" });
            Needs.TryAdd(NeedIds.Drink, new(NeedIds.Drink) { Name = "Drink" });
            Needs.TryAdd(NeedIds.Food, new(NeedIds.Food) { Name = "Food" });
            Needs.TryAdd(NeedIds.Rest, new(NeedIds.Rest) { Name = "Rest" });
            Needs.TryAdd(NeedIds.Clothes, new(NeedIds.Clothes) { Name = "Clothes" });
            Needs.TryAdd(NeedIds.Shelter, new(NeedIds.Shelter) { Name = "Shelter" });
            Needs.TryAdd(NeedIds.SecurityProtection, new(NeedIds.SecurityProtection) { Name = "SecurityProtection" });
            Needs.TryAdd(NeedIds.SecurityWealth, new(NeedIds.SecurityWealth) { Name = "SecurityWealth" });
            Needs.TryAdd(NeedIds.Health, new(NeedIds.Health) { Name = "Health" });
            Needs.TryAdd(NeedIds.Fellowship, new(NeedIds.Fellowship) { Name = "Fellowship" });
            Needs.TryAdd(NeedIds.Reproduction, new(NeedIds.Reproduction) { Name = "Reproduction" });
            Needs.TryAdd(NeedIds.Expression, new(NeedIds.Expression) { Name = "Expression" });
            Needs.TryAdd(NeedIds.Community, new(NeedIds.Community) { Name = "Community" });
            Needs.TryAdd(NeedIds.Entertainment, new(NeedIds.Entertainment) { Name = "Entertainment" });
        }

        /// <summary>
        /// Defines the named geology types
        /// </summary>
        private static void InitGeologies()
        {
            Geologies.TryAdd(GeologyId.Minerals, new(GeologyId.Minerals) { Name = "Minerals" }); //  0 low  -  Minerals       2673
            Geologies.TryAdd(GeologyId.Dark, new(GeologyId.Dark) { Name = "Dark" });             //  1 low  -  Dark           2175
            Geologies.TryAdd(GeologyId.Fertile, new(GeologyId.Fertile) { Name = "Fertile" });    //  2 high - Fertile        12225
            Geologies.TryAdd(GeologyId.Forest, new(GeologyId.Forest) { Name = "Forrest" });      //  3 high - Forrest        13440
            Geologies.TryAdd(GeologyId.Old, new(GeologyId.Old) { Name = "Old" });                //  4 low  -  ?              3019
            Geologies.TryAdd(GeologyId.Magic, new(GeologyId.Magic) { Name = "Magic" });          //  5 low  -  Magic          2808
            Geologies.TryAdd(GeologyId.Poor, new(GeologyId.Poor) { Name = "Poor" });             //  6 high - Poor           13752
            Geologies.TryAdd(GeologyId.Plain, new(GeologyId.Plain) { Name = "Plain" });          //  7 high - Plain          12408
        }

        /// <summary>
        /// Defines the relation ship from needs to crafts
        /// </summary>
        private static void InitCaftsHelpingNeeds()
        {
            CaftsHelpingNeeds.TryAdd(NeedIds.Air, []);
            CaftsHelpingNeeds.TryAdd(NeedIds.Temperature, [
                CraftIds.WoodCutting, CraftIds.Sewing, CraftIds.Tanning, CraftIds.Weave
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Drink, [
                CraftIds.Brewing, CraftIds.Pottery, CraftIds.Construction
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Food, [
                CraftIds.Farming, CraftIds.Hunting, CraftIds.Gathering, CraftIds.Trapping, CraftIds.Cooking, CraftIds.Pottery,
                CraftIds.Zoology, CraftIds.Botany, CraftIds.Fishing
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Rest, [
                CraftIds.Carpentry, CraftIds.Weave, CraftIds.Construction, CraftIds.Defense
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Clothes, [
                CraftIds.Farming, CraftIds.Hunting, CraftIds.Gathering, CraftIds.Sewing, CraftIds.Tanning, CraftIds.Weave
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Shelter, [
                CraftIds.WoodCutting, CraftIds.Blacksmithing, CraftIds.Carpentry, CraftIds.Architecture
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.SecurityProtection, [
                CraftIds.Hunting, CraftIds.Trapping, CraftIds.Blacksmithing, CraftIds.Architecture, CraftIds.Construction,
                CraftIds.Warfare, CraftIds.Combat, CraftIds.Archery, CraftIds.Defense, CraftIds.Magic
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.SecurityWealth, [
                CraftIds.Farming, CraftIds.Trade, CraftIds.Jeweler, CraftIds.Gemcutting
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Health, [
                CraftIds.Medicine, CraftIds.Herbalism, CraftIds.Botany,
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Fellowship, [
                CraftIds.Farming, CraftIds.Mining, CraftIds.Brewing, CraftIds.Writing, CraftIds.Reading,
                CraftIds.Shipbuilding, CraftIds.Architecture, CraftIds.Defense, CraftIds.Trade,
                CraftIds.Singing, CraftIds.Storytelling, CraftIds.Playing, CraftIds.Acting, CraftIds.Dancing, CraftIds.Entertaining
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Reproduction, [
                CraftIds.Singing, CraftIds.Dancing,CraftIds.Courtesanship
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Expression, [
                CraftIds.Jeweler, CraftIds.Writing, CraftIds.Architecture, CraftIds.Storytelling, CraftIds.Playing, CraftIds.Acting,
                CraftIds.Theology
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Community, [
                CraftIds.Cartography, CraftIds.Brewing, CraftIds.Reading, CraftIds.History, CraftIds.Architecture, CraftIds.Construction,
                CraftIds.Management, CraftIds.Defense, CraftIds.Singing, CraftIds.Storytelling, CraftIds.Playing, CraftIds.Acting,
                CraftIds.Theology
                ]);
            CaftsHelpingNeeds.TryAdd(NeedIds.Entertainment, [
                CraftIds.Brewing, CraftIds.Singing, CraftIds.Storytelling, CraftIds.Playing, CraftIds.Acting, CraftIds.Dancing,
                CraftIds.Entertaining, CraftIds.Courtesanship
                ]);
        }

        /// <summary>
        /// Defines the relation ship from crafts to needs
        /// </summary>
        private static void InitNeedsBeingHelpedByCrafts()// key = CraftId, value = List<NeedIds>
        {
            foreach (int i in Crafts.Keys)
            {
                List<NeedIds> l = [];
                foreach (var j in CaftsHelpingNeeds)
                    if (j.Value.Contains((CraftIds)i))
                        l.Add(j.Key);
                NeedsBeingHelpedByCrafts.TryAdd((CraftIds)i, l);
            }
        }

        /// <summary>
        /// Defines the named crafts
        /// </summary>
        private static void InitCrafts()
        {
            Crafts.TryAdd(CraftIds.WoodCutting, new(CraftIds.WoodCutting) { Name = "Wood cutting" });
            Crafts.TryAdd(CraftIds.Farming, new(CraftIds.Farming) { Name = "Farming" });
            Crafts.TryAdd(CraftIds.Hunting, new(CraftIds.Hunting) { Name = "Hunting" });
            Crafts.TryAdd(CraftIds.Mining, new(CraftIds.Mining) { Name = "Mining" });
            Crafts.TryAdd(CraftIds.Gathering, new(CraftIds.Gathering) { Name = "Gathering" });
            Crafts.TryAdd(CraftIds.Trapping, new(CraftIds.Trapping) { Name = "Trapping" });
            Crafts.TryAdd(CraftIds.Medicine, new(CraftIds.Medicine) { Name = "Medicine" });
            Crafts.TryAdd(CraftIds.Blacksmithing, new(CraftIds.Blacksmithing) { Name = "Blacksmithing" });
            Crafts.TryAdd(CraftIds.Cartography, new(CraftIds.Cartography) { Name = "Cartography" });
            Crafts.TryAdd(CraftIds.Carpentry, new(CraftIds.Carpentry) { Name = "Carpentry" });
            Crafts.TryAdd(CraftIds.Brewing, new(CraftIds.Brewing) { Name = "Brewing" });
            Crafts.TryAdd(CraftIds.Sewing, new(CraftIds.Sewing) { Name = "Sewing" });
            Crafts.TryAdd(CraftIds.Cooking, new(CraftIds.Cooking) { Name = "Cooking" });
            Crafts.TryAdd(CraftIds.Jeweler, new(CraftIds.Jeweler) { Name = "Jeweler" });
            Crafts.TryAdd(CraftIds.Gemcutting, new(CraftIds.Gemcutting) { Name = "Gemcutting" });
            Crafts.TryAdd(CraftIds.Writing, new(CraftIds.Writing) { Name = "Writing" });
            Crafts.TryAdd(CraftIds.Reading, new(CraftIds.Reading) { Name = "Reading" });
            Crafts.TryAdd(CraftIds.Herbalism, new(CraftIds.Herbalism) { Name = "Herbalism" });
            Crafts.TryAdd(CraftIds.History, new(CraftIds.History) { Name = "History" });
            Crafts.TryAdd(CraftIds.Pottery, new(CraftIds.Pottery) { Name = "Pottery" });
            Crafts.TryAdd(CraftIds.Shipbuilding, new(CraftIds.Shipbuilding) { Name = "Shipbuilding" });
            Crafts.TryAdd(CraftIds.Tracking, new(CraftIds.Tracking) { Name = "Tracking" });
            Crafts.TryAdd(CraftIds.Zoology, new(CraftIds.Zoology) { Name = "Zoology" });
            Crafts.TryAdd(CraftIds.Botany, new(CraftIds.Botany) { Name = "Botany" });
            Crafts.TryAdd(CraftIds.Architecture, new(CraftIds.Architecture) { Name = "Architecture" });
            Crafts.TryAdd(CraftIds.Construction, new(CraftIds.Construction) { Name = "Construction" });
            Crafts.TryAdd(CraftIds.Tanning, new(CraftIds.Tanning) { Name = "Tanning" });
            Crafts.TryAdd(CraftIds.Management, new(CraftIds.Management) { Name = "Management" });
            Crafts.TryAdd(CraftIds.Weave, new(CraftIds.Weave) { Name = "Weave" });
            Crafts.TryAdd(CraftIds.Warfare, new(CraftIds.Warfare) { Name = "Warfare" });
            Crafts.TryAdd(CraftIds.Combat, new(CraftIds.Combat) { Name = "Combat" });
            Crafts.TryAdd(CraftIds.Archery, new(CraftIds.Archery) { Name = "Archery" });
            Crafts.TryAdd(CraftIds.Defense, new(CraftIds.Defense) { Name = "Defense" });
            Crafts.TryAdd(CraftIds.Trade, new(CraftIds.Trade) { Name = "Trade" });
            Crafts.TryAdd(CraftIds.Singing, new(CraftIds.Singing) { Name = "Singing" });
            Crafts.TryAdd(CraftIds.Storytelling, new(CraftIds.Storytelling) { Name = "Storytelling" });
            Crafts.TryAdd(CraftIds.Playing, new(CraftIds.Playing) { Name = "Playing" });
            Crafts.TryAdd(CraftIds.Acting, new(CraftIds.Acting) { Name = "Acting" });
            Crafts.TryAdd(CraftIds.Dancing, new(CraftIds.Dancing) { Name = "Dancing" });
            Crafts.TryAdd(CraftIds.Fishing, new(CraftIds.Fishing) { Name = "Fishing" });
            Crafts.TryAdd(CraftIds.Theology, new(CraftIds.Theology) { Name = "Theology" });
            Crafts.TryAdd(CraftIds.Entertaining, new(CraftIds.Entertaining) { Name = "Entertaining" });
            Crafts.TryAdd(CraftIds.Magic, new(CraftIds.Magic) { Name = "Magic" });
            Crafts.TryAdd(CraftIds.Courtesanship, new(CraftIds.Courtesanship) { Name = "Courtesanship" });
        }

        /// <summary>
        /// Defines the named professions
        /// </summary>
        private static void InitProfessions()
        {
            Professions.TryAdd(ProfessionIds.Farmer, new(ProfessionIds.Farmer)
            {
                Name = "Farmer",
                MainCrafts = [CraftIds.Farming],
                SecondaryCrafts = [CraftIds.Botany],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Goblin, RaceIds.Satyr, RaceIds.Ent, RaceIds.Hare, RaceIds.Haflings, RaceIds.Orc]),
            });
            Professions.TryAdd(ProfessionIds.Hunter, new(ProfessionIds.Hunter)
            {
                Name = "Hunter",
                MainCrafts = [CraftIds.Hunting],
                SecondaryCrafts = [CraftIds.Tracking, CraftIds.Trapping, CraftIds.Zoology, CraftIds.Archery],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Goblin, RaceIds.Satyr, RaceIds.Lizard, RaceIds.Hare, RaceIds.Haflings, RaceIds.Orc, RaceIds.Mer, RaceIds.Minotaur, RaceIds.Thiefling]),
            });
            Professions.TryAdd(ProfessionIds.Carpenter, new(ProfessionIds.Carpenter)
            {
                Name = "Carpenter",
                MainCrafts = [CraftIds.Carpentry],
                SecondaryCrafts = [CraftIds.WoodCutting, CraftIds.Construction],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Goblin, RaceIds.Satyr, RaceIds.Lizard, RaceIds.Hare, RaceIds.Haflings, RaceIds.Orc]),
            });
            Professions.TryAdd(ProfessionIds.Cook, new(ProfessionIds.Cook)
            {
                Name = "Cook",
                MainCrafts = [CraftIds.Cooking],
                SecondaryCrafts = [CraftIds.Herbalism, CraftIds.Zoology, CraftIds.Gathering],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Goblin, RaceIds.Satyr, RaceIds.Lizard, RaceIds.Hare, RaceIds.Haflings, RaceIds.Orc, RaceIds.Centaur, RaceIds.Elve, RaceIds.Ent, RaceIds.Fae, RaceIds.Minotaur, RaceIds.Satyr]),
            });
            Professions.TryAdd(ProfessionIds.Tailor, new(ProfessionIds.Tailor)
            {
                Name = "Tailor",
                MainCrafts = [CraftIds.Sewing],
                SecondaryCrafts = [CraftIds.Weave],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Goblin, RaceIds.Satyr, RaceIds.Lizard, RaceIds.Hare, RaceIds.Haflings, RaceIds.Orc, RaceIds.Centaur, RaceIds.Elve, RaceIds.Ent, RaceIds.Fae, RaceIds.Satyr, RaceIds.Nymph]),
            });
            Professions.TryAdd(ProfessionIds.Ruler, new(ProfessionIds.Ruler)
            {
                Name = "Ruler",
                MainCrafts = [CraftIds.Management],
                SecondaryCrafts = [CraftIds.Writing, CraftIds.Reading, CraftIds.History, CraftIds.Warfare, CraftIds.Dancing],
            });
            Professions.TryAdd(ProfessionIds.InnKeeper, new(ProfessionIds.InnKeeper)
            {
                Name = "Inn Keeper",
                MainCrafts = [CraftIds.Trade],
                SecondaryCrafts = [CraftIds.Storytelling, CraftIds.Management, CraftIds.Cooking, CraftIds.Brewing],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Goblin, RaceIds.Satyr, RaceIds.Lizard, RaceIds.Hare, RaceIds.Haflings, RaceIds.Orc, RaceIds.Centaur, RaceIds.Elve, RaceIds.Ent, RaceIds.Fae, RaceIds.Minotaur, RaceIds.Satyr, RaceIds.Nymph]),
            });
            Professions.TryAdd(ProfessionIds.Guard, new(ProfessionIds.Guard)
            {
                Name = "Guard",
                MainCrafts = [CraftIds.Defense, CraftIds.Combat],
                SecondaryCrafts = [CraftIds.Tracking, CraftIds.Warfare, CraftIds.Archery],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Goblin, RaceIds.Satyr, RaceIds.Lizard, RaceIds.Hare, RaceIds.Haflings, RaceIds.Orc, RaceIds.Centaur, RaceIds.Elve, RaceIds.Ent, RaceIds.Fae, RaceIds.Minotaur, RaceIds.Satyr, RaceIds.Mer]),
            });
            Professions.TryAdd(ProfessionIds.Officer, new(ProfessionIds.Officer)
            {
                Name = "Officer",
                MainCrafts = [CraftIds.Warfare, CraftIds.Combat],
                SecondaryCrafts = [CraftIds.Archery, CraftIds.Defense]
            });
            Professions.TryAdd(ProfessionIds.Bard, new(ProfessionIds.Bard)
            {
                Name = "Bard",
                MainCrafts = [CraftIds.Singing, CraftIds.Playing],
                SecondaryCrafts = [CraftIds.Storytelling, CraftIds.Acting, CraftIds.Dancing],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Satyr, RaceIds.Haflings, RaceIds.Elve, RaceIds.Fae, RaceIds.Minotaur, RaceIds.Satyr, RaceIds.Mer, RaceIds.Nymph]),
            });
            Professions.TryAdd(ProfessionIds.Trader, new(ProfessionIds.Trader)
            {
                Name = "Trader",
                MainCrafts = [CraftIds.Trade],
                SecondaryCrafts = []
            });
            Professions.TryAdd(ProfessionIds.Herbalist, new(ProfessionIds.Herbalist)
            {
                Name = "Herbalist",
                MainCrafts = [CraftIds.Herbalism],
                SecondaryCrafts = [CraftIds.Botany, CraftIds.Medicine]
            });
            Professions.TryAdd(ProfessionIds.Miner, new(ProfessionIds.Miner)
            {
                Name = "Miner",
                MainCrafts = [CraftIds.Mining],
                SecondaryCrafts = [CraftIds.Gemcutting],
                Filter = PersonFilter.GetFilter([RaceIds.Dwarf, RaceIds.Human, RaceIds.Goblin, RaceIds.Hare, RaceIds.Haflings, RaceIds.Orc, RaceIds.Minotaur, RaceIds.Satyr]),
            });
            Professions.TryAdd(ProfessionIds.Beggar, new(ProfessionIds.Beggar)
            {
                Name = "Beggar",
                MainCrafts = [],
                SecondaryCrafts = []
            });
            Professions.TryAdd(ProfessionIds.Blacksmith, new(ProfessionIds.Blacksmith)
            {
                Name = "Blacksmith",
                MainCrafts = [CraftIds.Blacksmithing],
                SecondaryCrafts = [CraftIds.Defense]
            });
            Professions.TryAdd(ProfessionIds.Fisherman, new(ProfessionIds.Fisherman)
            {
                Name = "Fisherman",
                MainCrafts = [CraftIds.Fishing],
                SecondaryCrafts = [CraftIds.Shipbuilding]
            });
            Professions.TryAdd(ProfessionIds.Shepherd, new(ProfessionIds.Shepherd)
            {
                Name = "Shepherd",
                MainCrafts = [CraftIds.Zoology],
                SecondaryCrafts = [CraftIds.Gathering]
            });
            Professions.TryAdd(ProfessionIds.Wizard, new(ProfessionIds.Wizard)
            {
                Name = "Wizard",
                MainCrafts = [CraftIds.Magic],
                SecondaryCrafts = [CraftIds.Reading, CraftIds.Writing]
            });
            Professions.TryAdd(ProfessionIds.Monk, new(ProfessionIds.Monk)
            {
                Name = "Monk",
                MainCrafts = [CraftIds.Theology],
                SecondaryCrafts = [CraftIds.Reading, CraftIds.Writing, CraftIds.History]
            });
            Professions.TryAdd(ProfessionIds.Servant, new(ProfessionIds.Servant)
            {
                Name = "Servant",
                MainCrafts = [CraftIds.Farming, CraftIds.Gathering,],
                SecondaryCrafts = [CraftIds.Cooking, CraftIds.Entertaining,]
            });
            Professions.TryAdd(ProfessionIds.Courtesan, new(ProfessionIds.Courtesan)
            {
                Name = "Courtesan",
                MainCrafts = [CraftIds.Entertaining, CraftIds.Courtesanship],
                SecondaryCrafts = [CraftIds.Singing, CraftIds.Dancing,]
            });
        }

        /// <summary>
        /// Defines the named races
        /// </summary>
        private static void InitRaces()
        {
            Races.TryAdd(RaceIds.Human, new(RaceIds.Human)
            {
                Name = "Human",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.BorealForests, BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.TemperateRainForests,
                    BiomIds.GrassSteppe, BiomIds.Plains, BiomIds.Fields, BiomIds.RiverDelta, BiomIds.SandBeach, BiomIds.ReedsBeach],
                SecondaryBioms = [BiomIds.Tundra, BiomIds.MountainTundra, BiomIds.TropicalRainForests, BiomIds.TropicalMoistForests,
                    BiomIds.TropicalDryForests, BiomIds.Mangrove, BiomIds.Savannah, BiomIds.Marsh],
                Filter = null,
                PopulationModifier = 1f
            });
            Races.TryAdd(RaceIds.Elve, new(RaceIds.Elve)
            {
                Name = "Elve",
                Language = Language.Exotic,
                PrimaryBioms = [BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.TemperateRainForests, BiomIds.TropicalRainForests,
                    BiomIds.TropicalMoistForests, BiomIds.TropicalDryForests, BiomIds.TropicalCloudForests, BiomIds.AncientForest],
                SecondaryBioms = [BiomIds.BorealForests, BiomIds.Mangrove, BiomIds.MushroomForest,],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills, Temperature.NamedInterval.NormalToHot),
                PopulationModifier = 0.55f
            });
            Races.TryAdd(RaceIds.Dwarf, new(RaceIds.Dwarf)
            {
                Name = "Dwarf",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.BareRock, BiomIds.Tundra, BiomIds.MountainTundra, BiomIds.BorealForests, BiomIds.TemperateConiferousForests,
                    BiomIds.Vulcano],
                SecondaryBioms = [(BiomIds)1, BiomIds.Permafrost, BiomIds.TemperateForests, BiomIds.TemperateRainForests, BiomIds.TropicalRainForests,
                    BiomIds.TropicalMoistForests, BiomIds.TropicalDryForests, BiomIds.TropicalCloudForests, BiomIds.LavaPlain, BiomIds.Cliffs],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.HillsAndUp, Temperature.NamedInterval.All),
                PopulationModifier = 0.55f
            });
            Races.TryAdd(RaceIds.Orc, new(RaceIds.Orc)
            {
                Name = "Orc",
                Language = Language.Dark,
                PrimaryBioms = [BiomIds.Savannah, BiomIds.GrassSteppe, BiomIds.Plains],
                SecondaryBioms = [BiomIds.Tundra, BiomIds.BorealForests, BiomIds.TemperateForests, BiomIds.TemperateConiferousForests],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills, Temperature.NamedInterval.NormalToHot),
                PopulationModifier = 0.75f
            });
            Races.TryAdd(RaceIds.Goblin, new(RaceIds.Goblin)
            {
                Name = "Goblin",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.GrassSteppe, BiomIds.Plains, BiomIds.RiverDelta, BiomIds.SandBeach, BiomIds.ReedsBeach],
                SecondaryBioms = [BiomIds.TemperateForests, BiomIds.Mangrove, BiomIds.Savannah, (BiomIds)19, BiomIds.Fields, BiomIds.Swamp, BiomIds.Bog,
                    BiomIds.Marsh],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.CoastToLowHills, Temperature.NamedInterval.NormalToHot),
                PopulationModifier = 0.8f
            });
            Races.TryAdd(RaceIds.Haflings, new(RaceIds.Haflings)
            {
                Name = "Haflings",
                AltName = "Hobbit",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.GrassSteppe, BiomIds.Plains, BiomIds.Fields],
                SecondaryBioms = [BiomIds.BorealForests, BiomIds.TemperateRainForests, BiomIds.MushroomForest],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills, Temperature.NamedInterval.NormalToHot),
                PopulationModifier = 0.27f
            });
            Races.TryAdd(RaceIds.Lizard, new(RaceIds.Lizard)
            {
                Name = "Lizard Folk",
                AltName = "Lizardmen",
                Language = Language.Dark,
                PrimaryBioms = [BiomIds.TropicalRainForests, BiomIds.TropicalMoistForests, BiomIds.Mangrove, BiomIds.Swamp, BiomIds.Bog, BiomIds.Marsh,
                    BiomIds.RiverDelta, BiomIds.ReedsBeach],
                SecondaryBioms = [BiomIds.Desert, BiomIds.TemperateConiferousForests, BiomIds.TemperateRainForests, BiomIds.TropicalMoistForests,
                    BiomIds.Savannah, BiomIds.GrassSteppe, BiomIds.Plains, BiomIds.SandBeach],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.CoastToLowHills, Temperature.NamedInterval.WarmAndUp),
                PopulationModifier = 0.5f
            });
            Races.TryAdd(RaceIds.Hare, new(RaceIds.Hare)
            {
                Name = "Harengon",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.BorealForests, BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.GrassSteppe, BiomIds.Plains],
                SecondaryBioms = [BiomIds.Tundra, BiomIds.MountainTundra, BiomIds.TemperateRainForests, BiomIds.Savannah],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills, Temperature.NamedInterval.NormalToHot),
                PopulationModifier = 0.5f
            });
            Races.TryAdd(RaceIds.Centaur, new(RaceIds.Centaur)
            {
                Name = "Centaur",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.BorealForests, BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.TemperateRainForests,
                    BiomIds.TropicalMoistForests, BiomIds.TropicalDryForests, BiomIds.TropicalCloudForests],
                SecondaryBioms = [BiomIds.TropicalRainForests],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills, Temperature.NamedInterval.NormalToHot),
                PopulationModifier = 0.4f
            });
            Races.TryAdd(RaceIds.Ent, new(RaceIds.Ent)
            {
                Name = "Treant",
                AltName = "Ent",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.BorealForests, BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.TemperateRainForests,
                    BiomIds.TropicalRainForests, BiomIds.TropicalMoistForests, BiomIds.TropicalDryForests, BiomIds.Mangrove, BiomIds.AncientForest],
                SecondaryBioms = [BiomIds.Swamp],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills, Temperature.NamedInterval.NotVeryCold),
                PopulationModifier = 0.13f
            });
            Races.TryAdd(RaceIds.Mer, new(RaceIds.Mer)
            {
                Name = "Mer Folk",
                AltName = "Mermen",
                Language = Language.Exotic,
                PrimaryBioms = [BiomIds.CoralReef, BiomIds.SeaweedForest],
                SecondaryBioms = [BiomIds.Bank, BiomIds.Sea, BiomIds.Ocean],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.ShallowWater, Temperature.NamedInterval.NotVeryCold),
                PopulationModifier = 13f
            });
            Races.TryAdd(RaceIds.Minotaur, new(RaceIds.Minotaur)
            {
                Name = "Minotaur",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.TemperateForests, BiomIds.TemperateConiferousForests],
                SecondaryBioms = [BiomIds.BorealForests, BiomIds.TemperateRainForests, BiomIds.LavaPlain],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills, Temperature.NamedInterval.NotVeryCold),
                PopulationModifier = 0.13f
            });
            Races.TryAdd(RaceIds.Satyr, new(RaceIds.Satyr)
            {
                Name = "Satyr",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.TemperateForests, BiomIds.TemperateConiferousForests],
                SecondaryBioms = [BiomIds.BorealForests, BiomIds.TemperateRainForests],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills, Temperature.NamedInterval.NotVeryCold),
                PopulationModifier = 0.26f
            });
            Races.TryAdd(RaceIds.Fae, new(RaceIds.Fae)
            {
                Name = "Fae",
                AltName = "Faery",
                Language = Language.Exotic,
                PrimaryBioms = [BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.TropicalCloudForests, BiomIds.TropicalMoistForests],
                SecondaryBioms = [BiomIds.BorealForests, BiomIds.TemperateRainForests, BiomIds.TropicalRainForests, BiomIds.TropicalDryForests],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.ShallowWater, Temperature.NamedInterval.NormalToHot),
                PopulationModifier = 0.34f
            });
            Races.TryAdd(RaceIds.Nymph, new(RaceIds.Nymph)
            {
                Name = "Nymph",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.TropicalMoistForests],
                SecondaryBioms = [BiomIds.BorealForests, BiomIds.TemperateRainForests, BiomIds.TropicalRainForests, BiomIds.TropicalDryForests],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.ShallowWater, Temperature.NamedInterval.NormalToHot),
                PopulationModifier = 0.18f
            });
            Races.TryAdd(RaceIds.Thiefling, new(RaceIds.Thiefling)
            {
                Name = "Tiefling",
                Language = Language.Common,
                PrimaryBioms = [BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.TropicalMoistForests, BiomIds.Mangrove,
                    BiomIds.Plains, BiomIds.LavaPlain],
                SecondaryBioms = [BiomIds.BorealForests, BiomIds.TemperateRainForests, BiomIds.TropicalRainForests, BiomIds.TropicalDryForests,
                    BiomIds.Savannah, BiomIds.GrassSteppe],
                Filter = LocationFilter.GetFilter(Height.NamedInterval.ShallowWater, Temperature.NamedInterval.NotVeryCold),
                PopulationModifier = 0.13f
            });
        }


        /// <summary>
        /// Defines the named bioms
        /// </summary>
        private static void InitBioms()
        {
            Bioms.TryAdd(BiomIds.Desert, new(BiomIds.Desert)
            {
                Name = "Desert",
                AltName = "Wilderness",
                PlacementName = "out in the desert",
                Descriptions = ["a hars deserted wildernes", "a dry, barren area of land", "a dry, barren area of land covered with sand and without vegetation"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Hills, Temperature.NamedInterval.VeryHot, Precipitation.NamedInterval.Dry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills, Temperature.NamedInterval.VeryHot, Precipitation.NamedInterval.VeryDry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills, Temperature.NamedInterval.Hot, Precipitation.NamedInterval.VeryDry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills, Temperature.NamedInterval.VeryHot, Precipitation.NamedInterval.Dry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills, Temperature.NamedInterval.VeryHot, Precipitation.NamedInterval.VeryDry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills, Temperature.NamedInterval.Hot, Precipitation.NamedInterval.VeryDry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland, Temperature.NamedInterval.VeryHot, Precipitation.NamedInterval.Dry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland, Temperature.NamedInterval.VeryHot, Precipitation.NamedInterval.VeryDry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland, Temperature.NamedInterval.Hot, Precipitation.NamedInterval.VeryDry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast, Temperature.NamedInterval.VeryHot, Precipitation.NamedInterval.Dry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast, Temperature.NamedInterval.VeryHot, Precipitation.NamedInterval.VeryDry,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast, Temperature.NamedInterval.Hot, Precipitation.NamedInterval.VeryDry,0.9f),
                ],
                Plants = [PlantIds.Cacti],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Carriage],
            });

            Bioms.TryAdd(BiomIds.BareRock, new(BiomIds.BareRock)
            {
                Name = "Bare Rock",
                AltName = "",
                PlacementName = "on the rock",
                Descriptions = ["rocky wasteland", "stony bedrock"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains, Temperature.NamedInterval.All, Precipitation.NamedInterval.All,0.75f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills, Temperature.NamedInterval.VeryCold, Precipitation.NamedInterval.Dry,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills, Temperature.NamedInterval.VeryCold, Precipitation.NamedInterval.VeryDry,0.6f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills, Temperature.NamedInterval.Cold, Precipitation.NamedInterval.VeryDry,0.6f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills, Temperature.NamedInterval.Normal, Precipitation.NamedInterval.VeryDry,0.6f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills, Temperature.NamedInterval.Warm, Precipitation.NamedInterval.VeryDry,0.6f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland, Temperature.NamedInterval.Warm, Precipitation.NamedInterval.VeryDry,Slope.NamedInterval.Steep,0.6f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast, Temperature.NamedInterval.All, Precipitation.NamedInterval.VeryDry,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast, Temperature.NamedInterval.VeryCold, Precipitation.NamedInterval.NotVery,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast, Temperature.NamedInterval.VeryCold, Precipitation.NamedInterval.VeryWet,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast, Temperature.NamedInterval.Cold, Precipitation.NamedInterval.Dry,0.7f),
                ],
                Plants = [PlantIds.Lichen],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Carriage, TravelMode.Mountaineering],
            });

            Bioms.TryAdd(BiomIds.Tundra, new(BiomIds.Tundra)
            {
                Name = "Tundra",
                AltName = "",
                PlacementName = "on the tundra",
                Descriptions = ["a vast, flat, treeless tundra"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Dry,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Dry,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryDry,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.VeryDry,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.All,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryDry,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.All,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryDry,0.71f),
                ],
                Plants = [PlantIds.Cottongrass, PlantIds.Scrub, PlantIds.Grass],
                PopulationPotential = Biom.PopulationPotentials.VeryFew,
                Trees = [],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.Permafrost, new(BiomIds.Permafrost)
            {
                Name = "Permafrost",
                AltName = "",
                PlacementName = "at the permafrost",
                Descriptions = ["low grass and lots of ice, Permafrost"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Wet,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Normal,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.VeryWet,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Wet,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Normal,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Dry,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.VeryDry,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Dry,0.7f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Normal,0.7f),
                ],
                Plants = [PlantIds.Lichen, PlantIds.Grass, PlantIds.Moss],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Mountaineering],
            });

            Bioms.TryAdd(BiomIds.MountainTundra, new(BiomIds.MountainTundra)
            {
                Name = "Mountain Tundra",
                AltName = "High Lands",
                PlacementName = "in the mountain highlands",
                Descriptions = ["a vast, high Lands treeless mountain tundra"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.ColdToWarm,Precipitation.NamedInterval.All,0.72f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Normal,0.62f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,0.72f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.VeryWet,0.72f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Dry,0.72f),
                ],
                Plants = [PlantIds.Cottongrass, PlantIds.Lichen, PlantIds.Grass, PlantIds.Moss],
                PopulationPotential = Biom.PopulationPotentials.VeryFew,
                Trees = [],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Mountaineering],
            });

            Bioms.TryAdd(BiomIds.Glaciers, new(BiomIds.Glaciers)
            {
                Name = "Glaciers",
                AltName = "Ice",
                PlacementName = "at the glaciers",
                Descriptions = ["a large glacier, ice and crevasses"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.All,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryWet,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Wet,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Normal,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.VeryWet,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Wet,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Normal,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Normal,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Wet,0.70f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.VeryWet,0.70f),
                ],
                Plants = [],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Mountaineering],
            });

            Bioms.TryAdd(BiomIds.BorealForests, new(BiomIds.BorealForests)
            {
                Name = "Boreal Forests",
                AltName = "Taiga",
                PlacementName = "in the pine forest",
                Descriptions = ["Pine forest", "Boreal forests", "Conifer forest", "Pine woods"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryWet,0.52f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.VeryWet,0.52f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Wet,0.52f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Wet,0.52f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Normal,0.52f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.Normal,0.52f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Dry,0.52f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Dry,0.48f),
                ],
                ModernId = BiomIds.Fields,
                Plants = [PlantIds.Fern, PlantIds.Moss, PlantIds.Ivy],
                PopulationPotential = Biom.PopulationPotentials.Some,
                Trees = [TreeIds.Pine, TreeIds.Fir],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.TemperateForests, new(BiomIds.TemperateForests)
            {
                Name = "Temperate Forests",
                AltName = "Deciduous Forest",
                PlacementName = "in the deciduous forests",
                Descriptions = ["temperate forests", "deciduous forest", "temperate deciduous forest"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.NotVery,0.48f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,0.51f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.NotVery,0.51f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,0.51f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,0.51f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Wet,0.51f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Normal,0.51f),
                ],
                ModernId = BiomIds.Fields,
                Plants = [PlantIds.Fern, PlantIds.Anemone, PlantIds.Ivy, PlantIds.Rose],
                PopulationPotential = Biom.PopulationPotentials.Good,
                Trees = [TreeIds.Apple, TreeIds.Aspen, TreeIds.Ash, TreeIds.Basswood, TreeIds.Beech, TreeIds.Birch, TreeIds.ButternutTree,
                    TreeIds.Elm, TreeIds.Maple, TreeIds.Oak,TreeIds.Pecan, TreeIds.Plum, TreeIds.Poplar,TreeIds.Walnut, TreeIds.RedAlder,
                    TreeIds.Willow],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.TemperateConiferousForests, new(BiomIds.TemperateConiferousForests)
            {
                Name = "Temperate Coniferous Forests",
                AltName = "Evergreen Forest",
                PlacementName = "in the temperate coniferous forests",
                Descriptions = ["temperate coniferous forests", "evergreen forest", "evergreen coniferous forest"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.NotVery,0.50f),
                ],
                AncientId = BiomIds.AncientForest,
                ModernId = BiomIds.Fields,
                Plants = [PlantIds.Fern, PlantIds.Moss, PlantIds.Ivy],
                PopulationPotential = Biom.PopulationPotentials.Good,
                Trees = [TreeIds.Apricot, TreeIds.Pine, TreeIds.Cedar, TreeIds.Fir, TreeIds.Olive],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.TemperateRainForests, new(BiomIds.TemperateRainForests)
            {
                Name = "Temperate Rainforests",
                AltName = "Temperate Rainforest",
                PlacementName = "in the temperate rainforest",
                Descriptions = ["temperate broadleaf rainforests"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,0.9f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Wet,0.8f),
                ],
                AncientId = BiomIds.AncientForest,
                ModernId = BiomIds.Fields,
                Plants = [PlantIds.Fern, PlantIds.Moss, PlantIds.Orchid, PlantIds.PassionFlower],
                PopulationPotential = Biom.PopulationPotentials.Some,
                Trees = [TreeIds.Acai, TreeIds.Apricot, TreeIds.Cypress, TreeIds.Fig, TreeIds.Ginkgo, TreeIds.Lemmon, TreeIds.Magnolia, TreeIds.Palm, TreeIds.Teak, TreeIds.TulipTree],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.TropicalRainForests, new(BiomIds.TropicalRainForests)
            {
                Name = "Tropical Rainforests",
                AltName = "Rainforests, Jungle",
                PlacementName = "in the tropical rainforest",
                Descriptions = ["a rainforest with a closed and continuous tree canopy, moisture-dependent vegetation, epiphytes and lianas.", "a rainforests jungle"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.WarmAndUp,Precipitation.NamedInterval.VeryWet,0.91f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.WarmAndUp,Precipitation.NamedInterval.Wet,0.88f),
                ],
                Plants = [PlantIds.Fern, PlantIds.Moss, PlantIds.Orchid, PlantIds.PassionFlower],
                PopulationPotential = Biom.PopulationPotentials.Few,
                Trees = [TreeIds.Cypress, TreeIds.Apricot, TreeIds.CacaoTree, TreeIds.Fig, TreeIds.Kapok, TreeIds.Mango, TreeIds.Lemmon, TreeIds.Mango, TreeIds.Palm, TreeIds.Rambutan, TreeIds.RubberTree, TreeIds.Teak],
                ValidTravelModes = [TravelMode.Walking],
            });

            Bioms.TryAdd(BiomIds.TropicalMoistForests, new(BiomIds.TropicalMoistForests)
            {
                Name = "Tropical Moist Forests",
                AltName = "Tropical Forests",
                PlacementName = "in the moist tropical forest",
                Descriptions = ["a tropical moist forest with continuous tree canopy, broadleaf trees and lianas."],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.WarmAndUp,Precipitation.NamedInterval.Normal,0.89f)
                ],
                AncientId = BiomIds.AncientForest,
                Plants = [PlantIds.Fern, PlantIds.Moss, PlantIds.Orchid],
                PopulationPotential = Biom.PopulationPotentials.Few,
                Trees = [TreeIds.Teak, TreeIds.Cypress, TreeIds.Apricot, TreeIds.Fig, TreeIds.Lemmon, TreeIds.Magnolia, TreeIds.Palm, TreeIds.TulipTree, TreeIds.Teak,],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.TropicalDryForests, new(BiomIds.TropicalDryForests)
            {
                Name = "Tropical Dry Forests",
                AltName = "Tropical Forests",
                PlacementName = "in the dry tropical forest",
                Descriptions = [" a open woodland in tropical areas. forest with lianas and orchids."],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.Dry,0.89f),
                    LocationFilter.GetFilter(Height.NamedInterval.MainlandPlusAllHills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Dry,0.89f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Dry,0.80f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Dry,0.80f),
                ],
                Plants = [PlantIds.Fern, PlantIds.Orchid],
                PopulationPotential = Biom.PopulationPotentials.VeryFew,
                Trees = [TreeIds.Teak, TreeIds.Cypress, TreeIds.Magnolia, TreeIds.TulipTree, TreeIds.Pecan, TreeIds.Poplar],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.TropicalCloudForests, new(BiomIds.TropicalCloudForests)
            {
                Name = "Tropical Cloud Forests",
                AltName = "Cloud Forests",
                PlacementName = "up in the cloud forest of the mountains",
                Descriptions = ["Tropical Cloud Forests with low-level cloud cover at the canopy level, mountain forest"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,0.99f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Wet,0.98f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryWet,0.98f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mountains,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Wet,0.97f),
                ],
                Plants = [PlantIds.Fern, PlantIds.Moss, PlantIds.Orchid],
                PopulationPotential = Biom.PopulationPotentials.VeryFew,
                Trees = [TreeIds.Fir, TreeIds.Cypress, TreeIds.Pine, TreeIds.Ginkgo],
                ValidTravelModes = [TravelMode.Mountaineering],
            });

            Bioms.TryAdd(BiomIds.Mangrove, new(BiomIds.Mangrove)
            {
                Name = "Mangrove",
                AltName = "Coastal Tropical Forests",
                PlacementName = "in the mangrove forest",
                Descriptions = ["a mangrove forest, water and roots"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.WarmAndUp,Precipitation.NamedInterval.NotVery,Slope.NamedInterval.FlatOrNormal,0.95f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.WarmAndUp,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.95f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.93f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Normal,Slope.NamedInterval.FlatOrNormal,0.92f),
                ],
                Plants = [PlantIds.Ivy, PlantIds.WaterLilies],
                PopulationPotential = Biom.PopulationPotentials.Some,
                Trees = [TreeIds.Cypress, TreeIds.Willow, TreeIds.RedMangrove, TreeIds.BlackMangrove],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Sailing],
            });

            Bioms.TryAdd(BiomIds.Savannah, new(BiomIds.Savannah)
            {
                Name = "Savannah",
                AltName = "Savanna",
                PlacementName = "out on the savannah",
                Descriptions = ["A savanna with mixed woodland and grassland. trees being widely spaced so that the canopy does not close. an unbroken herbaceous layer consisting primarily of grasses"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.Normal,Slope.NamedInterval.FlatOrNormal,0.95f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Dry,Slope.NamedInterval.FlatOrNormal,0.95f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryDry,Slope.NamedInterval.FlatOrNormal,0.95f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryDry,Slope.NamedInterval.FlatOrNormal,0.95f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryDry,Slope.NamedInterval.FlatOrNormal,0.65f),
                ],
                Plants = [PlantIds.Scrub, PlantIds.Grass, PlantIds.Cacti],
                PopulationPotential = Biom.PopulationPotentials.Few,
                Trees = [TreeIds.RedAlder, TreeIds.Curatella, TreeIds.Baobab],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Carriage],
            });

            Bioms.TryAdd(BiomIds.GrassSteppe, new(BiomIds.GrassSteppe)
            {
                Name = "Grass Steppe",
                AltName = "Grassland",
                PlacementName = "out on the grassy steppes",
                Descriptions = ["a grassland plains", "wide grass steppe"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Normal,0.78f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryDry,0.8f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Wet,0.8f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Wet,0.8f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Normal,0.8f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Normal,0.79f),
                ],
                ModernId = BiomIds.Fields,
                Plants = [PlantIds.Grass, PlantIds.Lily],
                PopulationPotential = Biom.PopulationPotentials.Some,
                Trees = [TreeIds.Plum, TreeIds.Apple],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Carriage],
            });

            Bioms.TryAdd(BiomIds.Plains, new(BiomIds.Plains)
            {
                Name = "Plains",
                AltName = "",
                PlacementName = "out in the plains",
                Descriptions = ["a flatland with grasses, plains"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Dry,0.75f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.VeryWet,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.NotVery,0.71f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.ColdToWarm,Precipitation.NamedInterval.VeryDry,0.75f),
                ],
                ModernId = BiomIds.Fields,
                Plants = [PlantIds.Grass, PlantIds.Scrub],
                PopulationPotential = Biom.PopulationPotentials.Some,
                Trees = [TreeIds.RedAlder, TreeIds.Apricot],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Carriage],
            });

            Bioms.TryAdd(BiomIds.LavaPlain, new(BiomIds.LavaPlain)
            {
                Name = "Lava Plain",
                AltName = "",
                PlacementName = "out the lava plain",
                Descriptions = ["A lava field, a lava bed, a large mostly flat area of lava flows"],
                Plants = [PlantIds.Moss, PlantIds.Lichen, PlantIds.Scrub],
                Trees = [],
                ValidTravelModes = [TravelMode.Walking],
            });

            Bioms.TryAdd(BiomIds.Vulcano, new(BiomIds.Vulcano)
            {
                Name = "Vulcano",
                AltName = "",
                PlacementName = "by the vulcano",
                Descriptions = ["a vulcano"],
                Plants = [PlantIds.Lichen],
                Trees = [],
                ValidTravelModes = [TravelMode.Mountaineering],
            });

            Bioms.TryAdd(BiomIds.AncientForest, new(BiomIds.AncientForest)
            {
                Name = "Ancient Forest",
                AltName = "",
                PlacementName = "in the ancient forest",
                Descriptions = ["a ancient dense forest"],
                PopulationPotential = Biom.PopulationPotentials.Some,
                Plants = [PlantIds.Moss, PlantIds.Orchid, PlantIds.Rose, PlantIds.Fern],
                Trees = [TreeIds.Ginkgo, TreeIds.Oak, TreeIds.Fig, TreeIds.Cedar, TreeIds.Kapok],
                ValidTravelModes = [TravelMode.Walking],
            });

            Bioms.TryAdd(BiomIds.MushroomForest, new(BiomIds.MushroomForest)
            {
                Name = "Mushroom Forest",
                AltName = "",
                PlacementName = "in the mushroom forest",
                Descriptions = ["a forest of giant mushrooms"],
                Plants = [PlantIds.Moss, PlantIds.Fern],
                Trees = [],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.CrystalForest, new(BiomIds.CrystalForest)
            {
                Name = "Crystal Forest",
                AltName = "",
                PlacementName = "in the crystal forest",
                Plants = [],
                Trees = [],
                Descriptions = ["a forest of crystals"],
            });

            Bioms.TryAdd(BiomIds.Fields, new(BiomIds.Fields)
            {
                Name = "Fields",
                AltName = "",
                PlacementName = "out in the fields",
                Descriptions = ["an area of open land, planted with crops or pasture, typically bounded by hedges or fences.", "Pastures", "a wheat field and pastures"],
                PopulationPotential = Biom.PopulationPotentials.Great,
                Plants = [PlantIds.Grass, PlantIds.Lily, PlantIds.Rose],
                Trees = [TreeIds.Apple, TreeIds.Apricot, TreeIds.Plum],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Carriage],
            });

            Bioms.TryAdd(BiomIds.Swamp, new(BiomIds.Swamp)
            {
                Name = "Swamp",
                AltName = "Wetland",
                PlacementName = "in the swamp",
                Descriptions = ["Dense tree covered swamp, hanging mosses"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.Flat,0.88f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.Flat,0.88f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.88f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.VeryHot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.88f),
                ],
                Plants = [PlantIds.WaterLilies, PlantIds.Fern, PlantIds.Moss],
                PopulationPotential = Biom.PopulationPotentials.VeryFew,
                Trees = [TreeIds.Willow, TreeIds.RedMangrove, TreeIds.BlackMangrove],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Swiming],
            });

            Bioms.TryAdd(BiomIds.Bog, new(BiomIds.Bog)
            {
                Name = "Bog",
                AltName = "Wetland",
                PlacementName = "in the bog",
                Descriptions = ["a bog, wetland with peat and dead plants", "a mire, wetland with peat and dead plants"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.88f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.88f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                ],
                Plants = [PlantIds.WaterLilies, PlantIds.Fern, PlantIds.Moss, PlantIds.Scrub],
                PopulationPotential = Biom.PopulationPotentials.VeryFew,
                Trees = [TreeIds.Willow, TreeIds.BlackMangrove],
                ValidTravelModes = [TravelMode.Walking],
            });

            Bioms.TryAdd(BiomIds.Marsh, new(BiomIds.Marsh)
            {
                Name = "Marsh",
                AltName = "Wetland",
                PlacementName = "in the marsh",
                Descriptions = ["a marsh. a wetland with no woody plants. reeds and some water lilies"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.88f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Wet,Slope.NamedInterval.Flat,0.88f),
                    LocationFilter.GetFilter(Height.NamedInterval.Hills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Wet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Wet,Slope.NamedInterval.Flat,0.88f),
                    LocationFilter.GetFilter(Height.NamedInterval.LowHills,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Wet,Slope.NamedInterval.Flat,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Mainland,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.Wet,Slope.NamedInterval.FlatOrNormal,0.90f),
                ],
                Plants = [PlantIds.WaterLilies, PlantIds.Fern, PlantIds.Moss, PlantIds.Scrub],
                PopulationPotential = Biom.PopulationPotentials.VeryFew,
                Trees = [TreeIds.Willow, TreeIds.Birch],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.RiverDelta, new(BiomIds.RiverDelta)
            {
                Name = "River Delta",
                AltName = "",
                PlacementName = "by the river delta",
                Descriptions = ["a river delta"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.All,Precipitation.NamedInterval.NotVery,Slope.NamedInterval.FlatOrNormal,0.61f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.All,Precipitation.NamedInterval.VeryWet,Slope.NamedInterval.FlatOrNormal,0.61f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.NotVeryCold,Precipitation.NamedInterval.VeryDry,Slope.NamedInterval.Flat,0.61f),
                ],
                Plants = [PlantIds.WaterLilies, PlantIds.Fern, PlantIds.Moss],
                PopulationPotential = Biom.PopulationPotentials.Great,
                Trees = [TreeIds.Willow, TreeIds.RedMangrove, TreeIds.BlackMangrove],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Carriage],
            });

            Bioms.TryAdd(BiomIds.SandBeach, new(BiomIds.SandBeach)
            {
                Name = "Sand Beach",
                AltName = "",
                PlacementName = "by the beach",
                Descriptions = ["a wide sand beach"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.All,Precipitation.NamedInterval.All,Slope.NamedInterval.Normal,0.66f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.All,Slope.NamedInterval.Flat,0.66f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.NotVery,Slope.NamedInterval.Flat,0.66f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Cold,Precipitation.NamedInterval.VeryDry,Slope.NamedInterval.Flat,0.66f),
                ],
                Plants = [PlantIds.Scrub, PlantIds.Rose],
                PopulationPotential = Biom.PopulationPotentials.Good,
                Trees = [],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding, TravelMode.Carriage],
            });

            Bioms.TryAdd(BiomIds.ReedsBeach, new(BiomIds.ReedsBeach)
            {
                Name = "Reeds Beach",
                AltName = "",
                PlacementName = "by the reed-filled beach",
                Descriptions = ["a wetland close to the beash full of reeds"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.WarmAndUp,Precipitation.NamedInterval.All,Slope.NamedInterval.Flat,0.80f),
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.Normal,Precipitation.NamedInterval.All,Slope.NamedInterval.Flat,0.81f),
                ],
                Plants = [PlantIds.WaterLilies, PlantIds.Scrub],
                PopulationPotential = Biom.PopulationPotentials.Some,
                Trees = [TreeIds.Willow],
                ValidTravelModes = [TravelMode.Walking, TravelMode.Riding],
            });

            Bioms.TryAdd(BiomIds.Cliffs, new(BiomIds.Cliffs)
            {
                Name = "Cliffs",
                AltName = "",
                Descriptions = ["an area with steep cliffs and rocky slopes"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.Coast,Temperature.NamedInterval.All,Precipitation.NamedInterval.All,Slope.NamedInterval.Steep,0.91f),
                ],
                Plants = [PlantIds.Lichen, PlantIds.Scrub],
                PopulationPotential = Biom.PopulationPotentials.Few,
                Trees = [],
                ValidTravelModes = [TravelMode.Mountaineering],
            });

            Bioms.TryAdd(BiomIds.CoralReef, new(BiomIds.CoralReef)
            {
                Name = "Coral Reef",
                AltName = "",
                Descriptions = ["a coral reef. warm, shallow, clear and sunny water. many colorful fish"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.ShallowWater,Temperature.NamedInterval.Warm,Precipitation.NamedInterval.All,0.73f),
                    LocationFilter.GetFilter(Height.NamedInterval.ShallowWater,Temperature.NamedInterval.Hot,Precipitation.NamedInterval.All,0.75f),
                ],
                Plants = [],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Swiming, TravelMode.Sailing],
            });

            Bioms.TryAdd(BiomIds.Bank, new(BiomIds.Bank)
            {
                Name = "Bank",
                AltName = "",
                Descriptions = ["a large under water sandbank in shallow water at the sea, many fish"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.ShallowWater,Temperature.NamedInterval.NotVeryCold,Precipitation.NamedInterval.All,0.74f)
                ],
                Plants = [],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Swiming, TravelMode.Sailing],
            });

            Bioms.TryAdd(BiomIds.SeaweedForest, new(BiomIds.SeaweedForest)
            {
                Name = "Seaweed Forest",
                AltName = "",
                Descriptions = ["the sea, seaweed forest, kelp"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.ShallowWater,Temperature.NamedInterval.NotVeryCold,Precipitation.NamedInterval.All,0.75f)
                ],
                Plants = [],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Sailing, TravelMode.Swiming],
            });

            Bioms.TryAdd(BiomIds.Sea, new(BiomIds.Sea)
            {
                Name = "Sea",
                AltName = "",
                Descriptions = ["the sea, calm waves, open water"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.ShallowWater,Temperature.NamedInterval.All,Precipitation.NamedInterval.All,0.76f)
                ],
                Plants = [],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Sailing],
            });

            Bioms.TryAdd(BiomIds.SeaIce, new(BiomIds.SeaIce)
            {
                Name = "Sea Ice",
                AltName = "",
                Descriptions = ["sea ​​ice, drifting icebergs"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.ShallowWater,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.All,0.85f),
                    LocationFilter.GetFilter(Height.NamedInterval.DeepWater,Temperature.NamedInterval.VeryCold,Precipitation.NamedInterval.All,0.84f),
                ],
                Plants = [],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Sailing],
            });

            Bioms.TryAdd(BiomIds.Ocean, new(BiomIds.Ocean)
            {
                Name = "Ocean",
                AltName = "Sea",
                Descriptions = ["a wast deep ocean, big waves"],
                Filters =
                [
                    LocationFilter.GetFilter(Height.NamedInterval.DeepWater,Temperature.NamedInterval.All,Precipitation.NamedInterval.All,0.85f)
                ],
                Plants = [],
                PopulationPotential = Biom.PopulationPotentials.None,
                Trees = [],
                ValidTravelModes = [TravelMode.Sailing],
            });

            //Slime Swamp,Perma Fog,Oasis
        }

        /// 
        private static readonly List<BiomIds> ForestBiomIds = [BiomIds.BorealForests, BiomIds.TemperateForests, BiomIds.TemperateConiferousForests, BiomIds.TemperateRainForests,
            BiomIds.TropicalRainForests, BiomIds.TropicalMoistForests, BiomIds.TropicalDryForests, BiomIds.TropicalCloudForests, BiomIds.Mangrove, BiomIds.MushroomForest, BiomIds.AncientForest];

        /// 
        public static List<BiomIds> GetForestBiomIds() => ForestBiomIds;

    }
}