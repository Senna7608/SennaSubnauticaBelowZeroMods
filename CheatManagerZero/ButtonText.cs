namespace CheatManagerZero
{
    internal enum Commands
    {
        day,
        night,
        unlockall,
        clearinventory,
        unlockdoors,
        encyall,
        BackWarp
    }

    internal enum ToggleCommands
    {
        survival,
        freedom,
        hardcore,
        creative,       
        fastbuild,
        fastscan,
        fastgrow,
        fasthatch,
        filterfast,
        nocost,
        noenergy,
        nosurvival,
        oxygen,
        invisible,
        shotgun,
        nodamage,            
        alwaysday,       
        overpower,        
        resistcold,
        noweather,
        noiceworm
    }

    internal enum Categories
    {
        Vehicles,
        Tools,
        Equipment,
        Materials,
        Electronics,
        Upgrades,
        FoodAndWater,
        LootAndDrill,
        Herbivores,
        Carnivores,
        Parasites,
        Leviathan,
        Eggs,
        SeaSeed,
        LandSeed,
        FloraItem,
        SeaSpawn,
        LandSpawn,
        Blueprints,
        Warp
    }

    internal enum Weather
    {
        weather,
        lightning,
        precipitation,
        wind,
        cold
    }

    internal class ButtonText
    {
        internal readonly string[] Buttons = new string[7]
        {
            "day",
            "night",
            "unlockall",
            "clearinventory",
            "unlockdoors",
            "ency all",            
            "BackWarp"
        };

        internal readonly string[] DayNightTab = new string[6]
        {
            "0.1",
            "0.25",
            "0.5",
            "0.75",
            "1",
            "2"
        };

        internal readonly string[] ToggleButtons = new string[21]
        {
            "survival",
            "freedom",
            "hardcore",
            "creative",            
            "fastbuild",
            "fastscan",
            "fastgrow",
            "fasthatch",
            "filterfast",
            "nocost",
            "noenergy",
            "nosurvival",
            "nooxygen",
            "invisible",
            "shotgun",
            "nodamage",            
            "alwaysday",            
            "overpower",
            "resistcold",
            "noweather",
            "noiceWorm"
        };

        internal readonly string[] CategoriesTab = new string[20]
        {
            "Vehicles",
            "Tools",
            "Equipment",
            "Materials",
            "Electronics",
            "Upgrades",
            "Food & Water",
            "Loot & Drill",
            "Herbivores",
            "Carnivores",
            "Parasites",
            "Leviathan",
            "Eggs",
            "Sea: Seed",
            "Land: Seed",
            "Flora: Item",
            "Sea: Spawn",
            "Land: Spawn",
            "Blueprints",
            "Warp"
        };
    }
}
