



namespace Talos.Enumerations
{
    public enum CreatureType : byte
    {
        Normal = 0,
        WalkThrough = 1,
        Merchant = 2,
        WhiteSquare = 3,
        Aisling = 4
    }
    internal enum Direction : byte
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        Invalid = 255
    }
    internal enum Dugon
    {
        White = 0,
        Green = 1,
        Blue = 2,
        Yellow = 3,
        Purple = 4,
        Brown = 5,
        Red = 6,
        Black = 7
    }

    internal enum EffectsBar : uint
    {
        None = 0,
        Mesmerize = 2,
        Dall = 3,
        Blind = 3,
        BeagCradh = 5,
        EisdCreature = 7,
        BonusExperience = 8,
        Hide = 10,
        Aite = 11,
        CreagNeart = 13,
        KelberothStance = 13,
        Dachaidh = 14,
        Beannaich = 16,
        MorBeannaich = 16,
        PerfectDefense = 19,
        FasSpiorad = 26,
        Poison = 35,
        Pause = 40,
        Suain = 50,
        FasDeireas = 52,
        Dion = 53,
        MorDion = 53,
        MorDionComlha = 53,
        IronSkin = 53,
        DeireasFaileas = 54,
        AsgallFaileas = 54,
        Mist = 55,
        Cradh = 82,
        MorCradh = 83,
        ArdCradh = 84,
        Seun = 88,
        Skull = 89,
        Pramh = 90,
        Armachd = 94,
        AegisSphere = 94,
        BeagSuain = 97,
        WolfFangFist = 101,
        PreventAffliction = 113,
        BeagFasNadur = 119,
        FasNadur = 119,
        MorFasNadur = 119,
        CatsHearing = 124,
        InnerFire = 126,
        DarkSeal = 133,
        Demise = 133,
        Purify = 143,
        Regeneration = 146,
        CounterAttack = 150,
        IncreasedRegeneration = 180,
        FeralForm = 183,
        BirdForm = 184,
        LizardForm = 185,

        //find these out
        //doesnt hurt since they are different numbers
        //but would be nice to know the effect bar/spell icon #
        Halt = 251,
        Coma = 252,
        DragonsFire = 253,
        PotentMantidScent = 254,
        SpellSkillBonus1 = 255
    }

    internal enum Element : byte
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Wind = 3,
        Earth = 4,
        Holy = 5,
        Darkness = 6,
        Wood = 7,
        Metal = 8,
        Nature = 9,
        Any = 10
    }

    internal enum EncryptMethod
    {
        None = 0,
        Normal = 1,
        MD5Key = 2
    }

    internal enum Mail : byte
    {
        HasParcel = 1,
        HasLetter = 16
    }

    internal enum MapFlags : ulong
    {
        Hostile = 1,
        NonHostile = 2,
        NoSpells = 4,
        NoSkills = 8,
        NoChat = 16,
        Snowing = 32,
    }

    internal enum MedeniaClass : byte
    {
        NonMed = 0,
        Gladiator = 1,
        Druid = 2,
        Archer = 3,
        Bard = 4,
        Summoner = 5,
        Unknown = 6
    }
    internal enum TemuairClass : byte
    {
        Peasant = 0,
        Warrior = 1,
        Rogue = 2,
        Wizard = 3,
        Priest = 4,
        Monk = 5
    }

    internal enum PreviousClass : byte
    {
        Pure = 0,
        Warrior = 1,
        Rogue = 2,
        Wizard = 3,
        Priest = 4,
        Monk = 5
    }

    internal enum MessageColor : byte
    {
        Red = 98,
        Yellow = 99,
        DarkGreen = 100,
        Silver = 101,
        DarkBlue = 102,
        White = 103,
        LighterGray = 104,
        LightGray = 105,
        Gray = 106,
        DarkGray = 107,
        DarkerGray = 108,
        Black = 109,
        HotPink = 111,
        DarkPurple = 112,
        NeonGreen = 113,
        Orange = 115,
        Brown = 116
    }

    internal enum Nation : byte
    {
        None = 1,
        Suomi = 2,
        Loures = 3,
        Mileth = 4,
        Tagor = 5,
        Rucesion = 6,
        Noes = 7
    }

    internal enum ServerMessageType : byte
    {
        Whisper = 0,        //blue text
        OrangeBar1 = 1,     //orange bar only
        OrangeBar2 = 2,
        ActiveMessage = 3,  //orange bar + shiftF
        OrangeBar3 = 4,
        AdminMessage = 5,   //in USDA, admins use this orange-bar channel
        OrangeBar5 = 6,
        UserOptions = 7,    //user options get sent through this byte
        ScrollWindow = 8,   //window with a scroll bar (like sense)
        NonScrollWindow = 9,//window without a scroll bar (identify item)
        WoodenBoard = 10,
        GroupChat = 11,     //puke green text (group)
        GuildChat = 12,     //olive green text
        ClosePopup = 17,    //closes current popup
        TopRight = 18       //white text top right
    }

    internal enum SpellAnimation : ushort
    {

        Armachd = 20,
        Skull = 24,
        PinkPoison = 25,
        Pramh = 32,
        Miss = 33,
        Suain = 40,
        LyliacPlant = 84,
        IronSkin = 89,
        Net = 114,
        Mesmerize = 117,
        IncreasedRegeneration = 170,
        CounterAttack = 184,
        Regeneration = 187,
        FrostArrow = 235,
        GreenBubblePoison = 247,
        MedeniaPoison = 295,
        FrostStrike = 377,

    }
    internal enum StatUpdateFlags : byte
    {
        None = 0,
        UnreadMail = 1,
        Unknown = 2,
        Secondary = 4,
        Experience = 8,
        Current = 16,
        Primary = 32,
        GameMasterA = 64,
        GameMasterB = 128,
        Swimming = 192,
        Full = 60
    }
}
