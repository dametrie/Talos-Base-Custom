using System;
using System.Collections.Generic;

namespace Talos.Definitions
{
    internal class CONSTANTS
    {
        public static readonly IReadOnlyDictionary<ushort, IReadOnlyCollection<ushort>> WHITELIST_BY_MAP_ID = new Dictionary<ushort, IReadOnlyCollection<ushort>>
        {
            { 7071, new ushort[] { 559, 246, 250, 929 } }, //Battle of Mount Merry
            { 10240, new ushort[] { 542, 544 } }, // Andor 140 Boss Room
            { 2120, new ushort[] { 876, 878 } }, //	Mount Giragan 1 - Yule Logs
            { 2088, new ushort[] { 876, 878 } }, //	Mount Giragan 2 - Yule Logs
            { 8111, new ushort[] { 205 } }, // Blackstar Crypt 11 Boss
            { 8496, new ushort[] { 164, 165, 166, 167 } }, // Macabre Grave Yard Zombie Survival
            { 8497, new ushort[] { 164, 165, 166, 167 } }, // Macabre Grave Yard Zombie Survival
            { 8498, new ushort[] { 164, 165, 166, 167 } }, // Macabre Grave Yard Zombie Survival
            { 8499, new ushort[] { 164, 165, 166, 167 } }, // Macabre Grave Yard Zombie Survival
            { 8500, new ushort[] { 46, 62, 814, 815 } }, // Count Macabre Mansion Event
            { 6999, new ushort[] { 492 } }, // Water Dungeon 15 Boss (energy sphere)
            { 509, new ushort[] { 912, 69, 321, 363, 365, 366, 367 } }, // 	Loures Battle Ring Arena Event
            { 8400, new ushort[] { 262, 394 } }, // Mileth Floppy Field 1 Giant Floppy
            { 10263, new ushort[] { 454 } }, // Alsaids Fire Canyon 1 Molten Cube
            { 8994, new ushort[] { 422 } }, // Lost Ruins 2 Grimes
            { 8983, new ushort[] { 404 } }, // Lost Ruins 7 Law
            { 8980, new ushort[] { 404 } }, // Law Shortcut 1 Law
            { 10000, new ushort[] { 422 } }, // Asilon Town Lorai
            { 8989, new ushort[] { 779, 782, 788 } }, // Lost Ruins 4
            { 8984, new ushort[] { 784, 785 } }, // Lost Ruins 6
            { 9377, new ushort[] { 692, 695 } }, // Plamit's Cave 13-1 Boss Room
            { 9378, new ushort[] { 692, 695 } }, //	Plamit Village
            { 9379, new ushort[] { 692, 695 } }, //	Plamit Inn
            { 9380, new ushort[] { 692, 695 } }, // Plamit Bar
            { 9381, new ushort[] { 692, 695 } }, // Bank of Plamit
            { 9382, new ushort[] { 692, 695 } }, // Plamit Potion Shop
            { 9383, new ushort[] { 692, 695 } }, // Plamit Armory
            { 9384, new ushort[] { 692, 695 } }, // plamit boss 8
            { 6646, new ushort[] { 318 } }, // Lynith Beach Way Boss 1
            { 6647, new ushort[] { 318 } }, // Lynith Beach Way Boss 1a
            { 6648, new ushort[] { 318 } }, // Lynith Beach Way Boss 1b
            { 6649, new ushort[] { 318 } }, // Lynith Beach Way Boss 1c
            { 6651, new ushort[] { 316 } }, // Lynith Beach Way Boss 2
            { 6652, new ushort[] { 316 } }, // Lynith Beach Way Boss 2a
            { 6653, new ushort[] { 316 } }, // Lynith Beach Way Boss 2b
            { 6654, new ushort[] { 316 } }, // Lynith Beach Way Boss 2c
            { 6656, new ushort[] { 207 } }, // Lynith Beach Way Boss 3
            { 6657, new ushort[] { 207 } }, // Lynith Beach Way Boss 3a
            { 6658, new ushort[] { 207 } }, // Lynith Beach Way Boss 3b
            { 6659, new ushort[] { 207 } }  // Lynith Beach Way Boss 3c
        };

        public static readonly IReadOnlyDictionary<string, IReadOnlyCollection<ushort>> WHITELIST_BY_MAP_NAME = new Dictionary<string, IReadOnlyCollection<ushort>>(StringComparer.OrdinalIgnoreCase)
        {
            //{ "Crypt", new ushort[] { 53 } }, // allow spider only in crypt maps cuz spider spawn spell),
            { "Shinewood Forest 3", new ushort[] { 263, 266 } }, // allow beetle/mantis in sw2 30+),
            { "Shinewood Forest 4", new ushort[] { 263, 266 } }, // allow beetle/mantis in sw2 40+),
            { "Aman Jungle", new ushort[] { 856, 873, 874, 875 } }, // allow frogs in aman jungle),
            { "Yowien Territory", new ushort[] { 634, 664 } }, // allow specific yt mobs),
            { "Beal na Carraige", new ushort[] { 707, 780, 781 } }, // allow giant things in bnc),
            { "Cthonic Remains 4", new ushort[] { 190, 210 } }, // allow skele draco in cr40+),
            { "Cthonic Remains 5", new ushort[] { 190, 210 } }, // allow skele draco in cr50+),
            { "Cursed Home", new ushort[] { 609, 610, 611, 612 } }, // allow casting on cursed home mobs),
            { "Muisir", new ushort[] { 926, 933, 940, 953, 954, 955, 960 } }, // allow casting on muisir mobs),
            { "Chadul's Mileth", new ushort[] { 401 } }, // boss of all instances of chadul mileth),
            { "Chadul's Abel", new ushort[] { 400 } }, // boss of all instances of chadul abel),
            { "Chadul's Loures", new ushort[] { 650 } }, // boss of all instances of chadul loures),
            { "Chadul's Piet", new ushort[] { 397 } }, // boss of all instances of chadul piet),
            { "Blackstar Crypt 5", new ushort[] { 401 } }, // first boss of blackstar crypt),
            { "Blackstar Crypt 11", new ushort[] { 205 } }, // second boss of blackstar crypt),
            { "Blackstar Crypt Boss", new ushort[] { 397 } } // last boss of blackstar crypt)
        };

        public static readonly IReadOnlyDictionary<string, IReadOnlyCollection<ushort>> BLACKLIST_BY_MAP_NAME = new Dictionary<string, IReadOnlyCollection<ushort>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Blackstar", new ushort[] { 529 } } // dont allow casting on chickens in blackstar
        };

        public static readonly IReadOnlyDictionary<string, string> WHITE_DUGON_RESPONSES = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"A bat flutters", "*patience*"},
            {"A candle flame dances", "*full breath*"},
            {"A cold, gray, empty room", "*rising breath*"},
            {"A die yields success", "*patience*"},
            {"A flower wilts", "*rising breath*"},
            {"A hand softly touches", "*falling breath*"},
            {"A leafless, winter tree", "*rising breath*"},
            {"A lock deftly undone", "*patience*"},
            {"A pyre consumes you", "*full breath*"},
            {"A spark dances in your chest", "*full breath*"},
            {"Aha!", "*empty breath*"},
            {"All fades away", "*rising breath*"},
            {"Blushing countenance of sunset", "*falling breath*"},
            {"Discovery", "*empty breath*"},
            {"Dissipation", "*rising breath*"},
            {"Flames envelop you", "*full breath*"},
            {"Flash of light", "*empty breath*"},
            {"Gentle violet light surrounds you", "*falling breath*"},
            {"Glint of gold", "*patience*"},
            {"Heat swells from below", "*full breath*"},
            {"Let go", "*rising breath*"},
            {"Light beneath the waves", "*empty breath*"},
            {"Light twinkles", "*full breath*"},
            {"Mother's caress", "*falling breath*"},
            {"Myriad, distant stars", "*empty breath*"},
            {"Ocean waves reflect light", "*falling breath*"},
            {"Passion", "*full breath*"},
            {"Profound understanding", "*empty breath*"},
            {"Rays reflected in crystals at sharp angles", "*patience*"},
            {"Satyrs dance about you", "*full breath*"},
            {"Swiftly dashing deer", "*patience*"},
            {"The hand turns over", "*patience*"},
            {"The moon shines as if in smiling", "*falling breath*"},
            {"The other cheek is offered", "*falling breath*"},
            {"The way is revealed", "*empty breath*"},
            {"The wolf whizzes by", "*patience*"},
            {"Trickle of thoughts", "*empty breath*"},
            {"You feel cold and numb", "*rising breath*"},
            {"Your body lies on an unlit pyre", "*rising breath*"},
            {"Your heart gently warms", "*falling breath*"}
        };
        public static readonly IReadOnlyDictionary<string, string> GREEN_DUGON_RESPONSES = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"A candle flame dances", "*rising breath*"},
            {"A crab sits still", "*empty breath*"},
            {"A cross", "*empty breath*"},
            {"A hand softly touches", "*full breath*"},
            {"A pyre consumes you", "*rising breath*"},
            {"A spark dances in your chest", "*rising breath*"},
            {"A well-cobbled path", "*empty breath*"},
            {"A wolf lunges at your throat", "*patience*"},
            {"A word", "*empty breath*"},
            {"An invigorating cry", "*patience*"},
            {"As without, within; as outside, inside", "*falling breath*"},
            {"Blushing countenance of sunset", "*full breath*"},
            {"Clash", "*patience*"},
            {"Cool, running river", "*falling breath*"},
            {"Flames envelop you", "*rising breath*"},
            {"Gentle violet light surrounds you", "*full breath*"},
            {"Heat swells from below", "*rising breath*"},
            {"Light twinkles", "*rising breath*"},
            {"Lush green leaves", "*falling breath*"},
            {"Mantis in the garden", "*falling breath*"},
            {"Mother's caress", "*full breath*"},
            {"Ocean waves reflect light", "*full breath*"},
            {"Passion", "*rising breath*"},
            {"Peaceful mountain", "*falling breath*"},
            {"Sages concur", "*empty breath*"},
            {"Satyrs dance about you", "*rising breath*"},
            {"Serene stream", "*falling breath*"},
            {"Stillness", "*falling breath*"},
            {"Stone tablet", "*empty breath*"},
            {"Sweat trickles down your chest", "*patience*"},
            {"Sword stabs into you", "*patience*"},
            {"The median appears obvious", "*empty breath*"},
            {"The moon shines as if in smiling", "*full breath*"},
            {"The other cheek is offered", "*full breath*"},
            {"Unbroken stare", "*patience*"},
            {"You taste blood", "*patience*"},
            {"Your empty hands cusp the wind", "*falling breath*"},
            {"Your heart gently warms", "*full breath*"},
            {"Your left shoulder and right balance", "*empty breath*"},
            {"Your muscles breathe easily", "*patience*"}
        };


        public static readonly IReadOnlyCollection<ushort> INVISIBLE_SPRITES = new ushort[]
        {
            676,
            690,
            691,
            699,
            700,
            701,
            532,
            530,
            528,
            526,
            492,
            387,
            289,
            290,
            292,
            293,
            294,
            295,
            296,
            297,
            298,
            299,
            253,
            252,
            195
        };

        public static readonly IReadOnlyCollection<ushort> GREEN_BOROS = new ushort[]
         {
            553,
            561,
            62
        };

        public static readonly IReadOnlyCollection<ushort> RED_BOROS = new ushort[]
        {
            552,
            561,
            62
        };

        public static readonly IReadOnlyCollection<ushort> UNDESIRABLE_SPRITES = new ushort[]
        {
            563,
            564,
            565,
            566,
            439,
            456,
            451,
            53
        };

        public static readonly IReadOnlyCollection<string> KNOWN_RANGERS = new string[]
        {
            "justyce",
            "justice",
            "gargoyle",
            "justify",
            "herb",
            "hulk",
            "tormund",
            "evanora",
            "reyakeely",
            "ishikawa",
            "listen",
            "error",
            "firewind",
            "xerge",
            "duenanknute",
            "joeker",
            "trial",
            "angelique",
            "evokation",
            "malache",
            "martrim",
            "ukkyo",
            "etienne",
            "algiza",
            "topic",
            "kremline",
            "venezia",
            "dionia",
            "etna",
            "viveena",
            "ladieerror",
            "errorjr",
            "and",
            "yukii"
        };

        public static readonly IReadOnlyCollection<string> TRASH_ITEMS = new string[]
        {
            "fior sal",
            "fior creag",
            "fior srad",
            "fior athar",
            "Purple Potion",
            "Blue Potion",
            "Light Belt",
            "Passion Flower",
            "Gold Jade Necklace",
            "Bone Necklace",
            "Amber Necklace",
            "Half Talisman",
            "Iron Greaves",
            "Goblin Helmet",
            "Cordovan Boots",
            "Shagreen Boots",
            "Magma Boots",
            "Hy-brasyl Bracer",
            "Hy-brasyl Gauntlet",
            "Hy-brasyl Belt",
            "Magus Apollo",
            "Holy Apollo",
            "Magus Diana",
            "Holy Diana",
            "Magus Gaea",
            "Holy Gaea"
        };

        public const int CREATURE_SPRITE_OFFSET = 16384;
        public const int ITEM_SPRITE_OFFSET = 32768;



    }
}
