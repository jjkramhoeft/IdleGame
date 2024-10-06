using Model;

namespace Generator
{
    public static class PreTextGenerator
    {
        public static string CreatePersonDescription(Person person, Location location, Region region)
        {
            _ = location;
            _ = region;

            string sexText = "";
            string pronoun = "it";
            if (person.SexId == SexIds.Male)
            {
                sexText = "male";
                pronoun = "he";
            }
            else if (person.SexId == SexIds.Female)
            {
                sexText = "female";
                pronoun = "she";
            }

            string ageText = "";
            if (person.PersonAgeId == PersonAgeIds.Infant)
                ageText = "infant";
            else if (person.PersonAgeId == PersonAgeIds.Child)
                ageText = "young child";
            else if (person.PersonAgeId == PersonAgeIds.Young)
                ageText = "young";
            else if (person.PersonAgeId == PersonAgeIds.Adult)
                ageText = "";
            else if (person.PersonAgeId == PersonAgeIds.Mature)
                ageText = "middle aged";
            else if (person.PersonAgeId == PersonAgeIds.Old)
                ageText = "old";

            string raceText = "";
            if (person.RaceId == RaceIds.Elve)
                raceText = "elven";
            else if (person.RaceId == RaceIds.Dwarf)
                raceText = "elven";
            else if (person.RaceId == RaceIds.Centaur)
                raceText = "centaur";
            else if (person.RaceId == RaceIds.Ent)
                raceText = "tree ent";
            else if (person.RaceId == RaceIds.Fae)
                raceText = "fairy";
            else if (person.RaceId == RaceIds.Goblin)
                raceText = "goblin";
            else if (person.RaceId == RaceIds.Haflings)
                raceText = "halfling";
            else if (person.RaceId == RaceIds.Hare)
                raceText = "harengon half human and half hare with long ears";
            else if (person.RaceId == RaceIds.Lizard)
                raceText = "humanoid lizard";
            else if (person.RaceId == RaceIds.Mer)
                raceText = "mermaid";
            else if (person.RaceId == RaceIds.Minotaur)
                raceText = "minotaur";
            else if (person.RaceId == RaceIds.Nymph)
                raceText = "nymph";
            else if (person.RaceId == RaceIds.Orc)
                raceText = "orc";
            else if (person.RaceId == RaceIds.Satyr)
                raceText = "satyr";
            else if (person.RaceId == RaceIds.Thiefling)
                raceText = "thiefling";

            string professionText;
            if (person.ProfessionId == ProfessionIds.None)
                professionText = "";
            else
                professionText = World.Professions[person.ProfessionId].Name;

            string jewelryText;
            if (person.JewelryId == JewelryIds.None)
                jewelryText = "";
            else
                jewelryText = $", {pronoun} is wearing " + World.Jewelry[person.JewelryId].Name;

            string hairText = "";
            if (person.HairStyleId != HairStyleIds.None)
            {
                if (person.HairStyleId == HairStyleIds.Bald)
                    hairText = $"{pronoun} is bald";
                else if (person.HairColorId == HairColorIds.None)
                    hairText = "with " + World.HairStyles[person.HairStyleId].Name;
                else
                    hairText = "with " + World.HairStyles[person.HairStyleId].NameWithColorKey.Replace("XXX", World.HairColors[person.HairColorId].Name);
            }

            string dressText;
            if (person.DressId == DressIds.None)
                dressText = "";
            else if (person.DressColorId == DressColorIds.None)
                dressText = $"{pronoun} is wearing " + World.Dresses[person.DressId].Name;
            else
                dressText = $"{pronoun} is wearing " + World.Dresses[person.DressId].NameWithColorKey.Replace("XXX", World.DressColors[person.DressColorId].Name);

            //string locationText = $"living in {location.Description}"; // todo implement background

            //todo use skin color

            string combined = $"A portrait photo of a {ageText.ToLower()} {sexText.ToLower()} {raceText.ToLower()} {professionText.ToLower()} {hairText.ToLower()} {dressText.ToLower()} {jewelryText.ToLower()}";// {locationText}";
            combined = combined.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            return combined;
        }
    }
}