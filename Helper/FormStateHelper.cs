using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Helper
{
    public class FormStateHelper
    {
        // CheckBox states
        public bool DontCbox1Checked { get; set; }
        public bool DontCBox2Checked { get; set; }
        public bool DontCBox3Checked { get; set; }
        public bool DontCBox4Checked { get; set; }
        public bool AutoDoubleCboxChecked { get; set; }
        public bool AutoGemCboxChecked { get; set; }

        public bool AutoStaffCboxChecked { get; set; }  
        public bool HideLinesCboxChecked { get; set; }
        public bool FormCboxChecked { get; set; }
        public bool OptionsSkullCboxChecked { get; set; }
        public bool OptionsSkullSurrboxChecked { get; set; }
        public bool OneLineWalkCboxChecked { get; set; }
        public bool DionCboxChecked { get; set; }
        public bool HealCboxChecked { get; set; }
        public bool DeireasFaileasCboxChecked { get; set; }
        public bool AoSithCboxChecked { get; set; }
        public bool AlertStrangerCboxChecked { get; set; }
        public bool AlertRangerCboxChecked { get; set; }
        public bool AlertDuraCboxChecked { get; set; }
        public bool AlertSkulledCboxChecked { get; set; }
        public bool AlertEXPCboxChecked { get; set; }
        public bool AlertItemCapCboxChecked { get; set; }
        public bool AiteCboxChecked { get; set; }
        public bool FasCboxChecked { get; set; }
        public bool DisenchanterCboxChecked { get; set; }
        public bool WakeScrollCboxChecked { get; set; }
        public bool HideCboxChecked { get; set; }
        public bool DruidFormCboxChecked { get; set; }
        public bool MistCboxChecked { get; set; }
        public bool ArmachdCboxChecked { get; set; }
        public bool FasSpioradCboxChecked { get; set; }
        public bool BeagCradhCboxChecked { get; set; }
        public bool AegisSphereCboxChecked { get; set; }
        public bool DragonScaleCboxChecked { get; set; }
        public bool ManaWardCboxChecked { get; set; }
        public bool RegenerationCboxChecked { get; set; }
        public bool PerfectDefenseCboxChecked { get; set; }
        public bool DragonsFireCboxChecked { get; set; }
        public bool AsgallCboxChecked { get; set; }
        public bool MuscleStimulantCboxChecked { get; set; }
        public bool NerveStimulantCboxChecked { get; set; }
        public bool MonsterCallCboxChecked { get; set; }
        public bool VanishingElixirCboxChecked { get; set; }
        public bool VineyardCboxChecked { get; set; }
        public bool AutoRedCboxChecked { get; set; }
        public bool FungusExtractCboxChecked { get; set; }
        public bool MantidScentCboxChecked { get; set; }
        public bool AoSuainCboxChecked { get; set; }
        public bool AoCurseCboxChecked { get; set; }
        public bool AoPoisonCboxChecked { get; set; }
        public bool FollowCboxChecked { get; set; }
        public bool BubbleBlockCboxChecked { get; set; }
        public bool SpamBubbleCboxChecked { get; set; }
        public bool RangerStopCboxChecked { get; set; }
        public bool PickupGoldCboxChecked { get; set; }
        public bool PickupItemsCboxChecked { get; set; }
        public bool DropTrashCboxChecked { get; set; }
        public bool NoBlindCboxChecked { get; set; }
        public bool MapZoomCboxChecked { get; set; }
        public bool SeeHiddenCboxChecked { get; set; }
        public bool GhostHackCboxChecked { get; set; }
        public bool IgnoreCollisionCboxChecked { get; set; }
        public bool HideForegroundCboxChecked { get; set; }
        public bool MapFlagsEnableCboxChecked { get; set; }
        public bool MapSnowCboxChecked { get; set; }
        public bool MapTabsCboxChecked { get; set; }
        public bool MapSnowTileCboxChecked { get; set; }
        public bool UnifiedGuildChatCboxChecked { get; set; }
        public bool ToggleOverrideCboxChecked { get; set; }
        public bool SafeFSCboxChecked { get; set; }
        public bool EquipmentRepairCboxChecked { get; set; }
        public bool RangerLogCboxChecked { get; set; }
        public bool GMLogCBoxChecked { get; set; }
        public bool ChkGMSoundsChecked { get; set; }
        public bool DeformCBoxChecked { get; set; }
        public bool ChkTavWallHacksChecked { get; set; }
        public bool ChkTavWallStrangerChecked { get; set; }
        public bool ChkLastStepF5Checked { get; set; }
        public bool ChkAltLoginChecked { get; set; }
        public bool ChkSpeedStrangersChecked { get; set; }
        public bool LockstepCboxChecked { get; set; }
        public bool ChkWaitForFasChecked { get; set; }
        public bool ChkWaitForCradhChecked { get; set; }
        public bool ChkFrostStrikeChecked { get; set; }
        public bool ChkUseSkillsFromRangeChecked { get; set; }
        public bool ChargeToTargetCbxChecked { get; set; }
        public bool AssistBasherChkChecked { get; set; }
        public bool RadioLeaderTargetChecked { get; set; }
        public bool RadioAssitantStrayChecked { get; set; }
        public bool ChkCrasherChecked { get; set; }
        public bool ChkCrasherAboveHPChecked { get; set; }
        public bool ChkCrasherOnlyAsgallChecked { get; set; }
        public bool Protect1CbxChecked { get; set; }
        public bool Protect2CbxChecked { get; set; }
        public bool ChkAutoRepairBashChecked { get; set; }
        public bool ChkBashAssailsChecked { get; set; }
        public bool ChkRandomWaypointsChecked { get; set; }
        public bool ChkBashDionChecked { get; set; }
        public bool ChkExkuranumChecked { get; set; }
        public bool ChkNoCastOffenseChecked { get; set; }
        public bool ChkBashAsgallChecked { get; set; }

        // TextBox values
        public string LeadBasherTxt { get; set; }
        public string FasSpioradText { get; set; }
        public string SafeFSTbox { get; set; }
        public string MushroomComboxText { get; set; }
        public string ExpGemsComboxText { get; set; }
        public string DionComboxText { get; set; }
        public string DionWhenComboxText { get; set; }
        public string AiteComboxText { get; set; }
        public string HealComboxText { get; set; }
        public string AlertItemCapText { get; set; }

        public string AutoGroupList { get; set; }
        public string VineComboxText { get; set; }
        public string VineText { get; set; }
        public string FasComboxText { get; set; }

        public string PriorityList { get; set; }
        public string Protected1Tbx { get; set; }
        public string Protected2Tbx { get; set; }

        // NumericUpDown values
        public decimal FormNumValue { get; set; }
        public decimal DionPctNumValue { get; set; }
        public decimal HealPctNumValue { get; set; }
        public decimal OverrideDistanceNumValue { get; set; }
        public decimal NumAssitantStrayValue { get; set; }
        public decimal NumCrasherHealthValue { get; set; }
        public decimal NumExHealValue { get; set; }
        public decimal NumBashSkillDelayValue { get; set; }
        public decimal NumSkillIntValue { get; set; }
        public decimal PingCompensationNum1Value { get; set; }
        public decimal MonsterWalkIntervalNum1Value { get; set; }
        public decimal AtkRangeNumValue { get; set; }
        public decimal EngageRangeNumValue { get; set; }
        public decimal FollowDistanceNumValue { get; set; }
        public decimal WalkSpeedSldrValue { get; set; }
        public decimal NumLastStepTimeValue { get; set; }
        public decimal NumPFCounterValue { get; set; }

        // Button text
        public string Combo1BtnText { get; set; }
        public string Combo2BtnText { get; set; }
        public string Combo3BtnText { get; set; }
        public string Combo4BtnText { get; set; }
        public string BtnBashingText { get; set; }
        public string BtnBashingNewText { get; set; }

        // ComboBox values

        public string DoublesComboxText { get; set; }
        public string AutoDoubleCboxText { get; set; }
        public string AutoMushroomCBoxText { get; set; }
        public string AutoGemCboxText { get; set; }
        public string PriorityCboxText { get; set; }
        public string PriorityOnlyCboxText { get; set; }
        public string MapSnowCboxText { get; set; }
        public string MapTabsCboxText { get; set; }
        public string MapFlagsEnableCboxText { get; set; }
        public string AutoRedCboxText { get; set; }

        // Listbox items
        public List<string> Combo1ListItems { get; set; }
        public List<string> Combo2ListItems { get; set; }
        public List<string> Combo3ListItems { get; set; }
        public List<string> Combo4ListItems { get; set; }
        public List<string> TrashList { get; set; }
        public List<string> OverrideList { get; set; }
    }

}
