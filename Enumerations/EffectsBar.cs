using System;


namespace Talos.Enumerations
{
    [Flags]
    internal enum EffectsBar : uint
    {
        None = 0,               //0x00
        Dall = 3,               //0x03
        BeagCradh = 5,          //0x05
        EisdCreature = 7,       //0x07
        Hide = 10,              //0xA
        BeagNaomhAite = 11,     //0xB
        NaomhAite = 11,         //0xB
        ArdNaomhAite = 11,      //0xB
        Beannaich = 16,         //0x10
        MorBeannaich = 16,      //0x10
        FasSpiorad = 26,        //0x1A
        Poison = 35,            //0x23
        Suain = 50,             //0x32
        FasDeireas = 52,        //0x34
        Dion = 53,              //0x35
        MorDion = 53,           //0x35
        MorDionComlha = 53,     //0x35
        IronSkin = 53,          //0x35
        DeireasFaileas = 54,    //0x36
        AsgallFaileas = 54,     //0x36
        Mist = 55,              //0x37
        Cradh = 82,             //0x52
        MorCradh = 83,          //0x53
        ArdCradh = 84,          //0x54
        Pramh = 90,             //0x5A
        Armachd = 94,           //0x5E
        AegisSphere = 94,       //0x5E
        BeagSuain = 97,         //0x61
        WolfFangFist = 101,     //0x65
        PreventAffliction = 113,//0x71
        BeagFasNadur = 119,     //0x77
        FasNadur = 119,         //0x77
        MorFasNadur = 119,      //0x77
        CatsHearing = 124,      //0x7C
        InnerFire = 126,        //0x7E
        DarkSeal = 133,         //0x85
        Demise = 133,           //0x85
        Halt = 99999,
        Pause = 9999,
        Purify = 9999,
        Coma = 9999,
        DragonsFire = 9999,
        PotentMantidScent = 9999,
        PerfectDefense = 9999,
        SpellSkillBonus1 = 9999
    }
}
