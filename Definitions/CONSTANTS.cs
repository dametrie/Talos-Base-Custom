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
            { "Crypt", new ushort[] { 53 } }, // allow spider only in crypt maps cuz spider spawn spell),
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
            "and"
        };


    }
}
