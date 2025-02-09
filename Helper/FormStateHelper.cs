using System;
using System.Collections.Generic;

namespace Talos.Helper
{
    [Serializable]
    public class FormStateHelper
    {

        public AislingPageState AislingPage { get; set; }

        public BashingPageState BashingPage { get; set; }
        public ToolsPageState ToolsPage { get; set; }
        public ComboBoxPageState ComboBoxPage { get; set; }

        public List<AllyPageState> AllyPages { get; set; } = new List<AllyPageState>();
        public List<EnemyPageState> EnemyPages { get; set; } = new List<EnemyPageState>();
    }

    [Serializable]
    public class AislingPageState
    {
        // ComboBox and CheckBox values
        public string FollowText { get; set; }
        public string DoublesComboxText { get; set; }
        public bool AutoDoubleCboxChecked { get; set; }
        public string ExpGemsComboxText { get; set; }
        public bool AutoGemCboxChecked { get; set; }
        public bool AutoStaffCboxChecked { get; set; }
        public bool HideLinesCboxChecked { get; set; }

        public string DionComboxText { get; set; }
        public string DionWhenComboxText { get; set; }
        public string AiteComboxText { get; set; }
        public string HealComboxText { get; set; }
        public string FasComboxText { get; set; }
        public string VineComboxText { get; set; }

        public bool OptionsSkullCboxChecked { get; set; }
        public bool OptionsSkullSurrboxChecked { get; set; }
        public bool OneLineWalkCboxChecked { get; set; }
        public bool DionCboxChecked { get; set; }
        public bool HealCboxChecked { get; set; }
        public bool DeireasFaileasCboxChecked { get; set; }
        public bool AoSithCboxChecked { get; set; }

        // Alert-related CheckBoxes
        public bool AlertStrangerCboxChecked { get; set; }
        public bool AlertRangerCboxChecked { get; set; }
        public bool AlertDuraCboxChecked { get; set; }
        public bool AlertSkulledCboxChecked { get; set; }
        public bool AlertEXPCboxChecked { get; set; }
        public bool AlertItemCapCboxChecked { get; set; }

        // Other CheckBox states
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
        public bool ChkSpeedStrangersChecked { get; set; }
        public bool SafeFSCboxChecked { get; set; }
        public bool EquipmentRepairCboxChecked { get; set; }
        public bool RangerLogCboxChecked { get; set; }
        public bool ChkLastStepF5Checked { get; set; }
        public bool LockstepCboxChecked { get; set; }

        // Lists
        public List<string> TrashList { get; set; }
        public List<string> OverrideList { get; set; }

        // Numeric values
        public decimal DionPctNumValue { get; set; }
        public decimal HealPctNumValue { get; set; }
        public decimal FollowDistanceNumValue { get; set; }

        // TextBox values
        public string FasSpioradText { get; set; }
        public string VineText { get; set; }

        public int WalkSpeedSldrValue { get;  set; }
        public decimal NumLastStepTimeValue { get;  set; }
        public string WalkSpeedTlbl { get;  set; }
        public bool ChkIgnoreDionWaypointsChecked { get; set; }
        public bool ChkNoCastOffsenseChecked { get; set; }
    }

    [Serializable]
    public class BashingPageState
    {
        // Checkboxes
        public bool ChkWaitForFasChecked { get; set; }
        public bool ChkWaitForCradhChecked { get; set; }
        public bool ChkFrostStrikeChecked { get; set; }
        public bool ChkUseSkillsFromRangeChecked { get; set; }
        public bool ChkChargeToTargetCbxChecked { get; set; }
        public bool ChkAssistBasherChecked { get; set; }
        public bool ChkTavWallHacksChecked { get; set; }
        public bool ChkTavWallStrangerChecked { get; set; }

        public bool ChkAssailsChecked { get; set; }
        public bool ChkExkuraChecked { get; set; }
        public bool ChkProtect1CboxChecked { get; set; }
        public bool ChkProtect2CboxChecked { get; set; }
        public bool ChkBashAsgallChecked { get; set; }
        public bool ChkIgnoreWalledInChecked { get; set; }
        public bool ChkRiskySkillsChecked { get; set; }
        public bool ChkRiskySkillsDionChecked { get; set; }
        public bool ChkCrasherCboxChecked { get; set; }
        public bool ChkUseCrashersChecked { get; set; }
        public bool ChkCrasherOnlyAsgallChecked { get; set; }
        public bool ChkCrasherAboveHPChecked { get; set; }
        public bool ChkPrioritizeChecked { get; set; }
        public bool ChkPriorityOnlyChecked { get; set; }

        // NumericUpDown values
        public decimal NumOverrideDistanceValue { get; set; }
        public decimal NumAssitantStrayValue { get; set; }
        public decimal NumPFCounterValue { get; set; }
        public decimal NumCrasherHealthValue { get; set; }
        public decimal NumExHealValue { get; set; }
        public decimal NumSkillIntValue { get; set; }
        public decimal NumPingCompensation1Value { get; set; }
        public decimal NumMonsterWalkInterval1Value { get; set; }
        public decimal NumAtkRangeValue { get; set; }
        public decimal NumEngageRangeValue { get; set; }

        // TextBox values
        public string TextLeadBasherValue { get; set; }
        public string TextProtect1Value { get; set; }
        public string TextProtect2Value { get; set; }

        // Radiobutton values
        public bool RbtnLeaderTargetChecked { get; set; }
        public bool RbtnAssistantStrayChecked { get; set; }

        // Listbox values
        public List<string> PriorityLboxItems { get; set; }

    }

    [Serializable]
    public class ToolsPageState
    {
        public bool FormCboxChecked { get; set; }
        public decimal FormNumValue { get; set; }

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
        public bool DeformCBoxChecked { get; set; }
    }

    [Serializable]
    public class ComboBoxPageState
    {
        // ComboBox List items (lines)
        public List<string> Combo1ListItems { get; set; }
        public List<string> Combo2ListItems { get; set; }
        public List<string> Combo3ListItems { get; set; }
        public List<string> Combo4ListItems { get; set; }

        // Button text
        public string Combo1BtnText { get; set; }
        public string Combo2BtnText { get; set; }
        public string Combo3BtnText { get; set; }
        public string Combo4BtnText { get; set; }

        // CheckBox states
        public bool DontCbox1Checked { get; set; }
        public bool DontCBox2Checked { get; set; }
        public bool DontCBox3Checked { get; set; }
        public bool DontCBox4Checked { get; set; }
    }


    [Serializable]
    public class AllyPageState
    {
        public string MiscLyliacTboxText { get; set; }
        public bool MiscLyliacCboxChecked { get; set; }
        public bool DispelCurseCboxChecked { get; set; }
        public bool DispelSuainCboxChecked { get; set; }
        public bool DispelPoisonCboxChecked { get; set; }
        public bool DbAiteCboxChecked { get; set; }
        public decimal DbIocNumPctValue { get; set; }
        public bool DbRegenCboxChecked { get; set; }
        public bool DbBCCboxChecked { get; set; }
        public bool DbArmachdCboxChecked { get; set; }
        public string DbIocComboxText { get; set; }
        public bool DbIocCboxChecked { get; set; }
        public string DbFasComboxText { get; set; }
        public bool DbFasCboxChecked { get; set; }
        public string DbAiteComboxText { get; set; }
        public bool AllyMICSpamRbtnChecked { get; set; }
        public bool AllyMDCSpamRbtnChecked { get; set; }
        public bool AllyMDCRbtnChecked { get; set; }
        public bool AllyNormalRbtnChecked { get; set; }
        public string AllyPageName { get; set; }
    }

    [Serializable]
    public class EnemyPageState
    {
        public string AttackComboxTwoText { get; set; }
        public bool PriorityOnlyCboxChecked { get; set; }
        public bool PriorityCboxChecked { get; set; }
        public List<string> PriorityLboxItems { get; set; }
        public bool NearestFirstCbxChecked { get; set; }
        public bool FarthestFirstCbxChecked { get; set; }
        public bool FasFirstRbtnChecked { get; set; }
        public bool CurseFirstRbtnChecked { get; set; }
        public bool SpellOneRbtnChecked { get; set; }
        public bool SpellAllRbtnChecked { get; set; }
        public bool PramhFirstRbtnChecked { get; set; }
        public bool SpellFirstRbtnChecked { get; set; }
        public bool IgnoreCboxChecked { get; set; }
        public List<string> IgnoreLboxItems { get; set; }
        public decimal ExpectedHitsNumValue { get; set; }
        public bool MpndDionedChecked { get; set; }
        public bool MpndSilencedChecked { get; set; }
        public bool MspgPctChecked { get; set; }
        public bool AttackCboxTwoChecked { get; set; }
        public bool MspgSilencedChecked { get; set; }
        public string AttackComboxOneText { get; set; }
        public string SpellsFasComboxText { get; set; }
        public string SpellsCurseComboxText { get; set; }
        public bool SpellsControlCboxChecked { get; set; }
        public string SpellsControlComboxText { get; set; }
        public bool SpellsFasCboxChecked { get; set; }
        public bool SpellsCurseCboxChecked { get; set; }
        public bool TargetCursedCboxChecked { get; set; }
        public bool TargetFassedCboxChecked { get; set; }
        public string TargetComboxText { get; set; }
        public bool TargetCboxChecked { get; set; }
        public bool AttackCboxOneChecked { get; set; }
        public List<string> PriorityList { get; set; }
        public string EnemyPageName { get; set; }
    }


}
