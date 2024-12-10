using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Talos.Base;
using Talos.Forms.UI;

namespace Talos.Forms
{
    partial class ClientTab
    {
      
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.clientTabControl = new System.Windows.Forms.TabControl();
            this.mainCoverTab = new System.Windows.Forms.TabPage();
            this.coverMapInfoGrp = new System.Windows.Forms.GroupBox();
            this.mapInfoInfoLbl = new System.Windows.Forms.Label();
            this.lastClickedSpriteLbl = new System.Windows.Forms.Label();
            this.mpLbl = new System.Windows.Forms.Label();
            this.hpLbl = new System.Windows.Forms.Label();
            this.friendsGroup = new System.Windows.Forms.GroupBox();
            this.removeFriendBtn = new System.Windows.Forms.Button();
            this.targetFriendBtn = new System.Windows.Forms.Button();
            this.friendAltsBtn = new System.Windows.Forms.Button();
            this.groupFriendBtn = new System.Windows.Forms.Button();
            this.friendList = new System.Windows.Forms.ListBox();
            this.strangerGroup = new System.Windows.Forms.GroupBox();
            this.lastSeenBtn = new System.Windows.Forms.Button();
            this.friendStrangerBtn = new System.Windows.Forms.Button();
            this.targetStrangerBtn = new System.Windows.Forms.Button();
            this.groupStrangerBtn = new System.Windows.Forms.Button();
            this.strangerList = new System.Windows.Forms.ListBox();
            this.groupGroup = new System.Windows.Forms.GroupBox();
            this.friendGroupBtn = new System.Windows.Forms.Button();
            this.groupAltsBtn = new System.Windows.Forms.Button();
            this.kickGroupedBtn = new System.Windows.Forms.Button();
            this.groupList = new System.Windows.Forms.ListBox();
            this.addEnemyGroup = new System.Windows.Forms.GroupBox();
            this.addBossBtn = new System.Windows.Forms.Button();
            this.addMonsterLbl = new System.Windows.Forms.Label();
            this.lastClickedNameLbl = new System.Windows.Forms.Label();
            this.addAislingLbl = new System.Windows.Forms.Label();
            this.lastClickedIDLbl = new System.Windows.Forms.Label();
            this.addAltsBtn = new System.Windows.Forms.Button();
            this.addGroupBtn = new System.Windows.Forms.Button();
            this.addAislingText = new System.Windows.Forms.TextBox();
            this.addAislingBtn = new System.Windows.Forms.Button();
            this.addMonsterText = new System.Windows.Forms.TextBox();
            this.addMonsterBtn = new System.Windows.Forms.Button();
            this.addAllMonstersBtn = new System.Windows.Forms.Button();
            this.coverEXPGrp = new System.Windows.Forms.GroupBox();
            this.autoMushroomCbox = new System.Windows.Forms.CheckBox();
            this.mushroomCombox = new System.Windows.Forms.ComboBox();
            this.expBoxedLbl = new System.Windows.Forms.Label();
            this.goldHourLbl = new System.Windows.Forms.Label();
            this.apHourLbl = new System.Windows.Forms.Label();
            this.expGemsLbl = new System.Windows.Forms.Label();
            this.doublesLbl = new System.Windows.Forms.Label();
            this.useGemBtn = new System.Windows.Forms.Button();
            this.useDoubleBtn = new System.Windows.Forms.Button();
            this.autoGemCbox = new System.Windows.Forms.CheckBox();
            this.autoDoubleCbox = new System.Windows.Forms.CheckBox();
            this.calcResetBtn = new System.Windows.Forms.Button();
            this.expGemsCombox = new System.Windows.Forms.ComboBox();
            this.doublesCombox = new System.Windows.Forms.ComboBox();
            this.expHourLbl = new System.Windows.Forms.Label();
            this.expSessionLbl = new System.Windows.Forms.Label();
            this.mainAislingsTab = new System.Windows.Forms.TabPage();
            this.aislingTabControl = new System.Windows.Forms.TabControl();
            this.selfTab = new System.Windows.Forms.TabPage();
            this.stuffGroup = new System.Windows.Forms.GroupBox();
            this.lblSecs = new System.Windows.Forms.Label();
            this.numLastStepTime = new System.Windows.Forms.NumericUpDown();
            this.chkLastStepF5 = new System.Windows.Forms.CheckBox();
            this.chkIgnoreDionWaypoints = new System.Windows.Forms.CheckBox();
            this.chkNoCastOffense = new System.Windows.Forms.CheckBox();
            this.optionsSkullSurrbox = new System.Windows.Forms.CheckBox();
            this.safeFSTbox = new System.Windows.Forms.TextBox();
            this.safeFSCbox = new System.Windows.Forms.CheckBox();
            this.optionsSkullCbox = new System.Windows.Forms.CheckBox();
            this.overrideGroup = new System.Windows.Forms.GroupBox();
            this.overrideDistanceLbl = new System.Windows.Forms.Label();
            this.overrideDistanceNum = new System.Windows.Forms.NumericUpDown();
            this.getSpriteBtn = new System.Windows.Forms.Button();
            this.toggleOverrideCbox = new System.Windows.Forms.CheckBox();
            this.removeOverrideBtn = new System.Windows.Forms.Button();
            this.addOverrideBtn = new System.Windows.Forms.Button();
            this.overrideText = new System.Windows.Forms.TextBox();
            this.overrideList = new System.Windows.Forms.ListBox();
            this.alertsGroup = new System.Windows.Forms.GroupBox();
            this.alertItemCapCbox = new System.Windows.Forms.CheckBox();
            this.alertRangerCbox = new System.Windows.Forms.CheckBox();
            this.alertEXPCbox = new System.Windows.Forms.CheckBox();
            this.alertSkulledCbox = new System.Windows.Forms.CheckBox();
            this.alertDuraCbox = new System.Windows.Forms.CheckBox();
            this.alertStrangerCbox = new System.Windows.Forms.CheckBox();
            this.usableItemsGroup = new System.Windows.Forms.GroupBox();
            this.equipmentrepairCbox = new System.Windows.Forms.CheckBox();
            this.autoRedCbox = new System.Windows.Forms.CheckBox();
            this.mantidScentCbox = new System.Windows.Forms.CheckBox();
            this.fungusExtractCbox = new System.Windows.Forms.CheckBox();
            this.classSpecificGroup = new System.Windows.Forms.GroupBox();
            this.vineText = new System.Windows.Forms.TextBox();
            this.vineCombox = new System.Windows.Forms.ComboBox();
            this.vineyardCbox = new System.Windows.Forms.CheckBox();
            this.monsterCallCbox = new System.Windows.Forms.CheckBox();
            this.wakeScrollCbox = new System.Windows.Forms.CheckBox();
            this.muscleStimulantCbox = new System.Windows.Forms.CheckBox();
            this.nerveStimulantCbox = new System.Windows.Forms.CheckBox();
            this.dragonScaleCbox = new System.Windows.Forms.CheckBox();
            this.dragonsFireCbox = new System.Windows.Forms.CheckBox();
            this.vanishingElixirCbox = new System.Windows.Forms.CheckBox();
            this.fasSpioradText = new System.Windows.Forms.TextBox();
            this.manaWardCbox = new System.Windows.Forms.CheckBox();
            this.fasSpioradCbox = new System.Windows.Forms.CheckBox();
            this.hideCbox = new System.Windows.Forms.CheckBox();
            this.asgallCbox = new System.Windows.Forms.CheckBox();
            this.armachdCbox = new System.Windows.Forms.CheckBox();
            this.disenchanterCbox = new System.Windows.Forms.CheckBox();
            this.beagCradhCbox = new System.Windows.Forms.CheckBox();
            this.druidFormCbox = new System.Windows.Forms.CheckBox();
            this.regenerationCbox = new System.Windows.Forms.CheckBox();
            this.mistCbox = new System.Windows.Forms.CheckBox();
            this.perfectDefenseCbox = new System.Windows.Forms.CheckBox();
            this.aegisSphereCbox = new System.Windows.Forms.CheckBox();
            this.walkGroup = new System.Windows.Forms.GroupBox();
            this.chkSpeedStrangers = new System.Windows.Forms.CheckBox();
            this.lockstepCbox = new System.Windows.Forms.CheckBox();
            this.spamBubbleCbox = new System.Windows.Forms.CheckBox();
            this.bubbleBlockCbox = new System.Windows.Forms.CheckBox();
            this.rangerStopCbox = new System.Windows.Forms.CheckBox();
            this.walkSpeedLbl = new System.Windows.Forms.Label();
            this.walkFastLbl = new System.Windows.Forms.Label();
            this.walkSlowLbl = new System.Windows.Forms.Label();
            this.walkSpeedSldr = new System.Windows.Forms.TrackBar();
            this.walkAllClientsBtn = new System.Windows.Forms.Button();
            this.walkBtn = new System.Windows.Forms.Button();
            this.walkMapCombox = new System.Windows.Forms.ComboBox();
            this.tilesLbl = new System.Windows.Forms.Label();
            this.followDistanceNum = new System.Windows.Forms.NumericUpDown();
            this.distanceLbl = new System.Windows.Forms.Label();
            this.followText = new System.Windows.Forms.TextBox();
            this.followCbox = new System.Windows.Forms.CheckBox();
            this.botOptionsGroup = new System.Windows.Forms.GroupBox();
            this.rangerLogCbox = new System.Windows.Forms.CheckBox();
            this.oneLineWalkCbox = new System.Windows.Forms.CheckBox();
            this.hideLinesCbox = new System.Windows.Forms.CheckBox();
            this.autoStaffCbox = new System.Windows.Forms.CheckBox();
            this.dispelsGroup = new System.Windows.Forms.GroupBox();
            this.aoPoisonCbox = new System.Windows.Forms.CheckBox();
            this.aoCurseCbox = new System.Windows.Forms.CheckBox();
            this.aoSuainCbox = new System.Windows.Forms.CheckBox();
            this.lootGroup = new System.Windows.Forms.GroupBox();
            this.dropTrashCbox = new System.Windows.Forms.CheckBox();
            this.removeTrashBtn = new System.Windows.Forms.Button();
            this.addTrashBtn = new System.Windows.Forms.Button();
            this.pickupItemsCbox = new System.Windows.Forms.CheckBox();
            this.pickupGoldCbox = new System.Windows.Forms.CheckBox();
            this.lootPickupLbl = new System.Windows.Forms.Label();
            this.lootDropLbl = new System.Windows.Forms.Label();
            this.addTrashText = new System.Windows.Forms.TextBox();
            this.trashList = new System.Windows.Forms.ListBox();
            this.dionHealsGroup = new System.Windows.Forms.GroupBox();
            this.aoSithCbox = new System.Windows.Forms.CheckBox();
            this.deireasFaileasCbox = new System.Windows.Forms.CheckBox();
            this.healCbox = new System.Windows.Forms.CheckBox();
            this.dionCbox = new System.Windows.Forms.CheckBox();
            this.healPctNum = new System.Windows.Forms.NumericUpDown();
            this.healCombox = new System.Windows.Forms.ComboBox();
            this.dionPctNum = new System.Windows.Forms.NumericUpDown();
            this.dionWhenCombox = new System.Windows.Forms.ComboBox();
            this.dionWhenLbl = new System.Windows.Forms.Label();
            this.dionCombox = new System.Windows.Forms.ComboBox();
            this.fasAiteGroup = new System.Windows.Forms.GroupBox();
            this.fasCbox = new System.Windows.Forms.CheckBox();
            this.aiteCbox = new System.Windows.Forms.CheckBox();
            this.fasCombox = new System.Windows.Forms.ComboBox();
            this.aiteCombox = new System.Windows.Forms.ComboBox();
            this.nearbyAllyTab = new System.Windows.Forms.TabPage();
            this.nearbyAllyTable = new System.Windows.Forms.TableLayoutPanel();
            this.mainMonstersTab = new System.Windows.Forms.TabPage();
            this.monsterTabControl = new System.Windows.Forms.TabControl();
            this.nearbyEnemyTab = new System.Windows.Forms.TabPage();
            this.nearbyEnemyTable = new System.Windows.Forms.TableLayoutPanel();
            this.bashingTab = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkRandomWaypoints = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ProtectGbx = new System.Windows.Forms.GroupBox();
            this.Protected2Tbx = new System.Windows.Forms.TextBox();
            this.Protect2Cbx = new System.Windows.Forms.CheckBox();
            this.Protected1Tbx = new System.Windows.Forms.TextBox();
            this.Protect1Cbx = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkBashAssails = new System.Windows.Forms.CheckBox();
            this.btnRepairAuto = new System.Windows.Forms.Button();
            this.chkAutoRepairBash = new System.Windows.Forms.CheckBox();
            this.chkCrasher = new System.Windows.Forms.CheckBox();
            this.chkUseSkillsFromRange = new System.Windows.Forms.CheckBox();
            this.ChargeToTargetCbx = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numAssitantStray = new System.Windows.Forms.NumericUpDown();
            this.radioAssitantStray = new System.Windows.Forms.RadioButton();
            this.assistBasherChk = new System.Windows.Forms.CheckBox();
            this.radioLeaderTarget = new System.Windows.Forms.RadioButton();
            this.leadBasherTxt = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkFrostStrike = new System.Windows.Forms.CheckBox();
            this.numPFCounter = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkWaitForCradh = new System.Windows.Forms.CheckBox();
            this.chkWaitForFas = new System.Windows.Forms.CheckBox();
            this.bashingSkillsToUseGrp = new System.Windows.Forms.GroupBox();
            this.btnBashing = new System.Windows.Forms.Button();
            this.toolsTab = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.comboSelector = new System.Windows.Forms.ComboBox();
            this.numComboImgSelect = new System.Windows.Forms.NumericUpDown();
            this.picComboSkill = new System.Windows.Forms.PictureBox();
            this.btnAddSkillCombo = new System.Windows.Forms.Button();
            this.resetGroup = new System.Windows.Forms.GroupBox();
            this.resetLbl = new System.Windows.Forms.Label();
            this.btnResetAllStatus = new System.Windows.Forms.Button();
            this.customLinesGroup = new System.Windows.Forms.GroupBox();
            this.customLinesBox = new System.Windows.Forms.TextBox();
            this.unifiedGuildChatCbox = new System.Windows.Forms.CheckBox();
            this.dmuGroup = new System.Windows.Forms.GroupBox();
            this.viewDMUCbox = new System.Windows.Forms.CheckBox();
            this.toggleShareCbox = new System.Windows.Forms.CheckBox();
            this.saveDmuBtn = new System.Windows.Forms.Button();
            this.toggleGenderCbox = new System.Windows.Forms.CheckBox();
            this.toggleDmuCbox = new System.Windows.Forms.CheckBox();
            this.acc3ColorNum = new System.Windows.Forms.NumericUpDown();
            this.acc3Num = new System.Windows.Forms.NumericUpDown();
            this.acc2ColorNum = new System.Windows.Forms.NumericUpDown();
            this.acc2Num = new System.Windows.Forms.NumericUpDown();
            this.acc1ColorNum = new System.Windows.Forms.NumericUpDown();
            this.acc1Num = new System.Windows.Forms.NumericUpDown();
            this.hairColorLbl = new System.Windows.Forms.Label();
            this.shieldNum = new System.Windows.Forms.NumericUpDown();
            this.hairColorNum = new System.Windows.Forms.NumericUpDown();
            this.weaponNum = new System.Windows.Forms.NumericUpDown();
            this.bootsNum = new System.Windows.Forms.NumericUpDown();
            this.overcoatNum = new System.Windows.Forms.NumericUpDown();
            this.bodyNum = new System.Windows.Forms.NumericUpDown();
            this.headNum = new System.Windows.Forms.NumericUpDown();
            this.headLbl = new System.Windows.Forms.Label();
            this.shieldLbl = new System.Windows.Forms.Label();
            this.weaponLbl = new System.Windows.Forms.Label();
            this.bootsLbl = new System.Windows.Forms.Label();
            this.armorLbl = new System.Windows.Forms.Label();
            this.acc3ColorLbl = new System.Windows.Forms.Label();
            this.acc2ColorLbl = new System.Windows.Forms.Label();
            this.acc1ColorLbl = new System.Windows.Forms.Label();
            this.acc3Lbl = new System.Windows.Forms.Label();
            this.acc2Lbl = new System.Windows.Forms.Label();
            this.acc1Lbl = new System.Windows.Forms.Label();
            this.bodyLbl = new System.Windows.Forms.Label();
            this.faceLbl = new System.Windows.Forms.Label();
            this.faceNum = new System.Windows.Forms.NumericUpDown();
            this.renamedStaffsGrp = new System.Windows.Forms.GroupBox();
            this.removeRenameBtn = new System.Windows.Forms.Button();
            this.renameLbl = new System.Windows.Forms.Label();
            this.renameList = new System.Windows.Forms.ListBox();
            this.addRenameBtn = new System.Windows.Forms.Button();
            this.renameText = new System.Windows.Forms.TextBox();
            this.renameCombox = new System.Windows.Forms.ComboBox();
            this.stuffGrp = new System.Windows.Forms.GroupBox();
            this.deformCbox = new System.Windows.Forms.CheckBox();
            this.safeScreenCbox = new System.Windows.Forms.CheckBox();
            this.formNum = new System.Windows.Forms.NumericUpDown();
            this.formCbox = new System.Windows.Forms.CheckBox();
            this.mapFlagsGroup = new System.Windows.Forms.GroupBox();
            this.darknessCbox = new System.Windows.Forms.CheckBox();
            this.mapSnowCbox = new System.Windows.Forms.CheckBox();
            this.mapSnowTileCbox = new System.Windows.Forms.CheckBox();
            this.mapTabsCbox = new System.Windows.Forms.CheckBox();
            this.mapFlagsEnableCbox = new System.Windows.Forms.CheckBox();
            this.clientHacksGroup = new System.Windows.Forms.GroupBox();
            this.ghostHackCbox = new System.Windows.Forms.CheckBox();
            this.ignoreCollisionCbox = new System.Windows.Forms.CheckBox();
            this.hideForegroundCbox = new System.Windows.Forms.CheckBox();
            this.mapZoomCbox = new System.Windows.Forms.CheckBox();
            this.seeHiddenCbox = new System.Windows.Forms.CheckBox();
            this.noBlindCbox = new System.Windows.Forms.CheckBox();
            this.mainTasksTab = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.btnCheckLoginTime = new System.Windows.Forms.Button();
            this.btnConLogin = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFood = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.TailorGroup = new System.Windows.Forms.GroupBox();
            this.TailorBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ascendGroup = new System.Windows.Forms.GroupBox();
            this.ascendAllBtn = new System.Windows.Forms.Button();
            this.deathOptionCbx = new System.Windows.Forms.CheckBox();
            this.ascendOptionCbx = new System.Windows.Forms.ComboBox();
            this.ascendBtn = new System.Windows.Forms.Button();
            this.prayerGroup = new System.Windows.Forms.GroupBox();
            this.prayerAcceptCbox = new System.Windows.Forms.CheckBox();
            this.prayerAssistCbox = new System.Windows.Forms.CheckBox();
            this.prayerAssistTbox = new System.Windows.Forms.TextBox();
            this.togglePrayingBtn = new System.Windows.Forms.Button();
            this.itemFindingGB = new System.Windows.Forms.GroupBox();
            this.btnItemRemove = new System.Windows.Forms.Button();
            this.btnItemAdd = new System.Windows.Forms.Button();
            this.listOfItems = new System.Windows.Forms.ListBox();
            this.btnLoadItemList = new System.Windows.Forms.Button();
            this.listToFind = new System.Windows.Forms.ListBox();
            this.btnItemFinder = new System.Windows.Forms.Button();
            this.votingGroup = new System.Windows.Forms.GroupBox();
            this.voteForAllBtn = new System.Windows.Forms.Button();
            this.voteText = new System.Windows.Forms.TextBox();
            this.toggleVotingBtn = new System.Windows.Forms.Button();
            this.fishingGroup = new System.Windows.Forms.GroupBox();
            this.fishingCombox = new System.Windows.Forms.ComboBox();
            this.keepFishRdio = new System.Windows.Forms.RadioButton();
            this.sellFishRdio = new System.Windows.Forms.RadioButton();
            this.toggleFishingBtn = new System.Windows.Forms.Button();
            this.farmGroup = new System.Windows.Forms.GroupBox();
            this.setNearestBankBtn = new System.Windows.Forms.Button();
            this.returnFarmBtn = new System.Windows.Forms.Button();
            this.setCustomBankBtn = new System.Windows.Forms.Button();
            this.toggleFarmBtn = new System.Windows.Forms.Button();
            this.hubaeGroup = new System.Windows.Forms.GroupBox();
            this.finishedWhiteBtn = new System.Windows.Forms.Button();
            this.teacherLbl = new System.Windows.Forms.Label();
            this.teacherText = new System.Windows.Forms.TextBox();
            this.killedCrabBtn = new System.Windows.Forms.Button();
            this.killedBatBtn = new System.Windows.Forms.Button();
            this.toggleHubaeBtn = new System.Windows.Forms.Button();
            this.laborGroup = new System.Windows.Forms.GroupBox();
            this.fastLaborCbox = new System.Windows.Forms.CheckBox();
            this.laborNameText = new System.Windows.Forms.TextBox();
            this.laborBtn = new System.Windows.Forms.Button();
            this.polishGroup = new System.Windows.Forms.GroupBox();
            this.polishUncutCbox = new System.Windows.Forms.CheckBox();
            this.polishBtn = new System.Windows.Forms.Button();
            this.polishAssistCbox = new System.Windows.Forms.CheckBox();
            this.assistText = new System.Windows.Forms.TextBox();
            this.polishLbl = new System.Windows.Forms.Label();
            this.ascensionGroup = new System.Windows.Forms.GroupBox();
            this.tocYNum = new System.Windows.Forms.NumericUpDown();
            this.tocYLbl = new System.Windows.Forms.Label();
            this.tocXNum = new System.Windows.Forms.NumericUpDown();
            this.tocXLbl = new System.Windows.Forms.Label();
            this.tocLbl = new System.Windows.Forms.Label();
            this.statLbl = new System.Windows.Forms.Label();
            this.statCombox = new System.Windows.Forms.ComboBox();
            this.statsNum = new System.Windows.Forms.NumericUpDown();
            this.statsPerLbl = new System.Windows.Forms.Label();
            this.toggleAscensionBtn = new System.Windows.Forms.Button();
            this.dojoTab = new System.Windows.Forms.TabPage();
            this.flowerCbox = new System.Windows.Forms.CheckBox();
            this.dojo2SpaceCbox = new System.Windows.Forms.CheckBox();
            this.rescueCbox = new System.Windows.Forms.CheckBox();
            this.flowerText = new System.Windows.Forms.TextBox();
            this.dojoRefreshBtn = new System.Windows.Forms.Button();
            this.dojoCounterAttackCbox = new System.Windows.Forms.CheckBox();
            this.dojoRegenerationCbox = new System.Windows.Forms.CheckBox();
            this.unmaxedSpellsGroup = new System.Windows.Forms.GroupBox();
            this.dojoOptionsGroup = new System.Windows.Forms.GroupBox();
            this.autoLbl = new System.Windows.Forms.Label();
            this.dojoAutoStaffCbox = new System.Windows.Forms.CheckBox();
            this.toggleDojoBtn = new System.Windows.Forms.Button();
            this.dojoBonusCbox = new System.Windows.Forms.CheckBox();
            this.unmaxedSkillsGroup = new System.Windows.Forms.GroupBox();
            this.mainEventsTab = new System.Windows.Forms.TabPage();
            this.candyTreatsGroup = new System.Windows.Forms.GroupBox();
            this.candyTreatsLbl = new System.Windows.Forms.Label();
            this.toggleCandyTreatsBtn = new System.Windows.Forms.Button();
            this.parchmentMaskGroup = new System.Windows.Forms.GroupBox();
            this.parchmentMaskLbl = new System.Windows.Forms.Label();
            this.toggleParchmentMaskBtn = new System.Windows.Forms.Button();
            this.scavengerHuntGroup = new System.Windows.Forms.GroupBox();
            this.scavengerHuntLbl = new System.Windows.Forms.Label();
            this.toggleScavengerHuntBtn = new System.Windows.Forms.Button();
            this.seasonalDblGroup = new System.Windows.Forms.GroupBox();
            this.seasonalDblLbl = new System.Windows.Forms.Label();
            this.toggleSeaonalDblBtn = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.toggleCatchLeprechaunBtn = new System.Windows.Forms.Button();
            this.MAWGroup = new System.Windows.Forms.GroupBox();
            this.mawLbl = new System.Windows.Forms.Label();
            this.toggleMAWBtn = new System.Windows.Forms.Button();
            this.fowlGroup = new System.Windows.Forms.GroupBox();
            this.fowlLbl = new System.Windows.Forms.Label();
            this.toggleFowlBtn = new System.Windows.Forms.Button();
            this.pigChaseGroup = new System.Windows.Forms.GroupBox();
            this.pigChaseLbl = new System.Windows.Forms.Label();
            this.togglePigChaseBtn = new System.Windows.Forms.Button();
            this.yuleGroup = new System.Windows.Forms.GroupBox();
            this.yuleLbl = new System.Windows.Forms.Label();
            this.toggleYuleBtn = new System.Windows.Forms.Button();
            this.bugGroup = new System.Windows.Forms.GroupBox();
            this.bugLbl = new System.Windows.Forms.Label();
            this.toggleBugBtn = new System.Windows.Forms.Button();
            this.comboTab = new System.Windows.Forms.TabPage();
            this.legendGroup = new System.Windows.Forms.GroupBox();
            this.legend3Lbl = new System.Windows.Forms.Label();
            this.legend2Lbl = new System.Windows.Forms.Label();
            this.exampleComboBtn = new System.Windows.Forms.Button();
            this.legend1Lbl = new System.Windows.Forms.Label();
            this.comboGroup = new System.Windows.Forms.GroupBox();
            this.comboLbl = new System.Windows.Forms.Label();
            this.dontCBox4 = new System.Windows.Forms.CheckBox();
            this.combo4Btn = new System.Windows.Forms.Button();
            this.combo4List = new System.Windows.Forms.TextBox();
            this.dontCBox3 = new System.Windows.Forms.CheckBox();
            this.combo3Btn = new System.Windows.Forms.Button();
            this.combo3List = new System.Windows.Forms.TextBox();
            this.dontCBox2 = new System.Windows.Forms.CheckBox();
            this.combo2Btn = new System.Windows.Forms.Button();
            this.combo2List = new System.Windows.Forms.TextBox();
            this.dontCbox1 = new System.Windows.Forms.CheckBox();
            this.combo1Btn = new System.Windows.Forms.Button();
            this.combo1List = new System.Windows.Forms.TextBox();
            this.infoTab = new System.Windows.Forms.TabPage();
            this.mapImageGroup = new System.Windows.Forms.GroupBox();
            this.mapImageResizeLbl = new System.Windows.Forms.Label();
            this.resizeMapTBox = new System.Windows.Forms.TextBox();
            this.saveMapImageCbox = new System.Windows.Forms.CheckBox();
            this.mapImageNameLvl = new System.Windows.Forms.Label();
            this.mapNameRdio = new System.Windows.Forms.RadioButton();
            this.mapNumberRdio = new System.Windows.Forms.RadioButton();
            this.mapImageCbox = new System.Windows.Forms.CheckBox();
            this.mappingGroup = new System.Windows.Forms.GroupBox();
            this.removeWarpLbl = new System.Windows.Forms.Label();
            this.changeWarpText = new System.Windows.Forms.TextBox();
            this.addWarpBtn = new System.Windows.Forms.Button();
            this.removeWarpBtn = new System.Windows.Forms.Button();
            this.chkMappingToggle = new System.Windows.Forms.CheckBox();
            this.dialogGroup = new System.Windows.Forms.GroupBox();
            this.dialogIdLbl = new System.Windows.Forms.Label();
            this.pursuitLbl = new System.Windows.Forms.Label();
            this.npcText = new System.Windows.Forms.TextBox();
            this.effectGroup = new System.Windows.Forms.GroupBox();
            this.effectBtn = new System.Windows.Forms.Button();
            this.effectNum = new System.Windows.Forms.NumericUpDown();
            this.mapToolsGroup = new System.Windows.Forms.GroupBox();
            this.spellBarIdsBtn = new System.Windows.Forms.Button();
            this.mapNodeIdsBtn = new System.Windows.Forms.Button();
            this.classDetectorBtn = new System.Windows.Forms.Button();
            this.pursuitIdsBtn = new System.Windows.Forms.Button();
            this.packetTab = new System.Windows.Forms.TabPage();
            this.packetPanel = new System.Windows.Forms.Panel();
            this.packetList = new System.Windows.Forms.ListBox();
            this.packetSidePanel = new System.Windows.Forms.Panel();
            this.packetHexText = new System.Windows.Forms.RichTextBox();
            this.packetCharText = new System.Windows.Forms.RichTextBox();
            this.packetBottomPanel = new System.Windows.Forms.Panel();
            this.packetInputText = new System.Windows.Forms.RichTextBox();
            this.packetBottomRightPanel = new System.Windows.Forms.Panel();
            this.sendPacketToClientBtn = new System.Windows.Forms.Button();
            this.sendPacketToServerBtn = new System.Windows.Forms.Button();
            this.packetStrip = new System.Windows.Forms.ToolStrip();
            this.packetStripLbl = new System.Windows.Forms.ToolStripLabel();
            this.toggleLogSendBtn = new System.Windows.Forms.ToolStripButton();
            this.toggleLogRecvBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearPacketLogBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleDialogBtn = new System.Windows.Forms.ToolStripButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.creatureHashListBox = new System.Windows.Forms.ListBox();
            this.worldObjectListBox = new System.Windows.Forms.ListBox();
            this.targetName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.spellName = new System.Windows.Forms.TextBox();
            this.button13 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.startStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.clearStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.loadStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.saveStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.waypointsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.walkToMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.windowOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullscreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expBonusCooldownTimer = new System.Windows.Forms.Timer(this.components);
            this.mushroomBonusCooldownTimer = new System.Windows.Forms.Timer(this.components);
            this.currentAction = new System.Windows.Forms.Label();
            this.chatPanel = new Talos.Forms.UI.ChatPanel();
            this.clientTabControl.SuspendLayout();
            this.mainCoverTab.SuspendLayout();
            this.coverMapInfoGrp.SuspendLayout();
            this.friendsGroup.SuspendLayout();
            this.strangerGroup.SuspendLayout();
            this.groupGroup.SuspendLayout();
            this.addEnemyGroup.SuspendLayout();
            this.coverEXPGrp.SuspendLayout();
            this.mainAislingsTab.SuspendLayout();
            this.aislingTabControl.SuspendLayout();
            this.selfTab.SuspendLayout();
            this.stuffGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLastStepTime)).BeginInit();
            this.overrideGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overrideDistanceNum)).BeginInit();
            this.alertsGroup.SuspendLayout();
            this.usableItemsGroup.SuspendLayout();
            this.classSpecificGroup.SuspendLayout();
            this.walkGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.walkSpeedSldr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.followDistanceNum)).BeginInit();
            this.botOptionsGroup.SuspendLayout();
            this.dispelsGroup.SuspendLayout();
            this.lootGroup.SuspendLayout();
            this.dionHealsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.healPctNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dionPctNum)).BeginInit();
            this.fasAiteGroup.SuspendLayout();
            this.nearbyAllyTab.SuspendLayout();
            this.mainMonstersTab.SuspendLayout();
            this.monsterTabControl.SuspendLayout();
            this.nearbyEnemyTab.SuspendLayout();
            this.bashingTab.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.ProtectGbx.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAssitantStray)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPFCounter)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.toolsTab.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numComboImgSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picComboSkill)).BeginInit();
            this.resetGroup.SuspendLayout();
            this.customLinesGroup.SuspendLayout();
            this.dmuGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.acc3ColorNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc3Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc2ColorNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc2Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc1ColorNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc1Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shieldNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hairColorNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.weaponNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bootsNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overcoatNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bodyNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.headNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.faceNum)).BeginInit();
            this.renamedStaffsGrp.SuspendLayout();
            this.stuffGrp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formNum)).BeginInit();
            this.mapFlagsGroup.SuspendLayout();
            this.clientHacksGroup.SuspendLayout();
            this.mainTasksTab.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.TailorGroup.SuspendLayout();
            this.ascendGroup.SuspendLayout();
            this.prayerGroup.SuspendLayout();
            this.itemFindingGB.SuspendLayout();
            this.votingGroup.SuspendLayout();
            this.fishingGroup.SuspendLayout();
            this.farmGroup.SuspendLayout();
            this.hubaeGroup.SuspendLayout();
            this.laborGroup.SuspendLayout();
            this.polishGroup.SuspendLayout();
            this.ascensionGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tocYNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tocXNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statsNum)).BeginInit();
            this.dojoTab.SuspendLayout();
            this.dojoOptionsGroup.SuspendLayout();
            this.mainEventsTab.SuspendLayout();
            this.candyTreatsGroup.SuspendLayout();
            this.parchmentMaskGroup.SuspendLayout();
            this.scavengerHuntGroup.SuspendLayout();
            this.seasonalDblGroup.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.MAWGroup.SuspendLayout();
            this.fowlGroup.SuspendLayout();
            this.pigChaseGroup.SuspendLayout();
            this.yuleGroup.SuspendLayout();
            this.bugGroup.SuspendLayout();
            this.comboTab.SuspendLayout();
            this.legendGroup.SuspendLayout();
            this.comboGroup.SuspendLayout();
            this.infoTab.SuspendLayout();
            this.mapImageGroup.SuspendLayout();
            this.mappingGroup.SuspendLayout();
            this.dialogGroup.SuspendLayout();
            this.effectGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.effectNum)).BeginInit();
            this.mapToolsGroup.SuspendLayout();
            this.packetTab.SuspendLayout();
            this.packetPanel.SuspendLayout();
            this.packetSidePanel.SuspendLayout();
            this.packetBottomPanel.SuspendLayout();
            this.packetBottomRightPanel.SuspendLayout();
            this.packetStrip.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // clientTabControl
            // 
            this.clientTabControl.Controls.Add(this.mainCoverTab);
            this.clientTabControl.Controls.Add(this.mainAislingsTab);
            this.clientTabControl.Controls.Add(this.mainMonstersTab);
            this.clientTabControl.Controls.Add(this.bashingTab);
            this.clientTabControl.Controls.Add(this.toolsTab);
            this.clientTabControl.Controls.Add(this.mainTasksTab);
            this.clientTabControl.Controls.Add(this.dojoTab);
            this.clientTabControl.Controls.Add(this.mainEventsTab);
            this.clientTabControl.Controls.Add(this.comboTab);
            this.clientTabControl.Controls.Add(this.infoTab);
            this.clientTabControl.Controls.Add(this.packetTab);
            this.clientTabControl.Controls.Add(this.tabPage1);
            this.clientTabControl.ForeColor = System.Drawing.Color.Black;
            this.clientTabControl.HotTrack = true;
            this.clientTabControl.Location = new System.Drawing.Point(0, 24);
            this.clientTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.clientTabControl.Name = "clientTabControl";
            this.clientTabControl.Padding = new System.Drawing.Point(0, 0);
            this.clientTabControl.SelectedIndex = 0;
            this.clientTabControl.Size = new System.Drawing.Size(850, 544);
            this.clientTabControl.TabIndex = 3;
            // 
            // mainCoverTab
            // 
            this.mainCoverTab.BackColor = System.Drawing.Color.White;
            this.mainCoverTab.Controls.Add(this.coverMapInfoGrp);
            this.mainCoverTab.Controls.Add(this.lastClickedSpriteLbl);
            this.mainCoverTab.Controls.Add(this.mpLbl);
            this.mainCoverTab.Controls.Add(this.hpLbl);
            this.mainCoverTab.Controls.Add(this.friendsGroup);
            this.mainCoverTab.Controls.Add(this.strangerGroup);
            this.mainCoverTab.Controls.Add(this.groupGroup);
            this.mainCoverTab.Controls.Add(this.addEnemyGroup);
            this.mainCoverTab.Controls.Add(this.coverEXPGrp);
            this.mainCoverTab.Controls.Add(this.chatPanel);
            this.mainCoverTab.ForeColor = System.Drawing.Color.Black;
            this.mainCoverTab.Location = new System.Drawing.Point(4, 24);
            this.mainCoverTab.Name = "mainCoverTab";
            this.mainCoverTab.Size = new System.Drawing.Size(842, 516);
            this.mainCoverTab.TabIndex = 0;
            this.mainCoverTab.Text = "Cover";
            // 
            // coverMapInfoGrp
            // 
            this.coverMapInfoGrp.Controls.Add(this.mapInfoInfoLbl);
            this.coverMapInfoGrp.Location = new System.Drawing.Point(638, 468);
            this.coverMapInfoGrp.Name = "coverMapInfoGrp";
            this.coverMapInfoGrp.Size = new System.Drawing.Size(201, 39);
            this.coverMapInfoGrp.TabIndex = 134;
            this.coverMapInfoGrp.TabStop = false;
            this.coverMapInfoGrp.Text = "Map";
            // 
            // mapInfoInfoLbl
            // 
            this.mapInfoInfoLbl.ForeColor = System.Drawing.Color.Black;
            this.mapInfoInfoLbl.Location = new System.Drawing.Point(11, 16);
            this.mapInfoInfoLbl.Name = "mapInfoInfoLbl";
            this.mapInfoInfoLbl.Size = new System.Drawing.Size(179, 15);
            this.mapInfoInfoLbl.TabIndex = 2;
            this.mapInfoInfoLbl.Text = "MapInfo";
            this.mapInfoInfoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lastClickedSpriteLbl
            // 
            this.lastClickedSpriteLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lastClickedSpriteLbl.AutoSize = true;
            this.lastClickedSpriteLbl.Location = new System.Drawing.Point(113, 133);
            this.lastClickedSpriteLbl.Name = "lastClickedSpriteLbl";
            this.lastClickedSpriteLbl.Size = new System.Drawing.Size(40, 15);
            this.lastClickedSpriteLbl.TabIndex = 10;
            this.lastClickedSpriteLbl.Text = "Sprite:";
            // 
            // mpLbl
            // 
            this.mpLbl.BackColor = System.Drawing.Color.MidnightBlue;
            this.mpLbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mpLbl.ForeColor = System.Drawing.Color.White;
            this.mpLbl.Location = new System.Drawing.Point(6, 484);
            this.mpLbl.Margin = new System.Windows.Forms.Padding(3);
            this.mpLbl.Name = "mpLbl";
            this.mpLbl.Size = new System.Drawing.Size(109, 23);
            this.mpLbl.TabIndex = 20;
            this.mpLbl.Text = "MP";
            this.mpLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hpLbl
            // 
            this.hpLbl.BackColor = System.Drawing.Color.Crimson;
            this.hpLbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hpLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hpLbl.ForeColor = System.Drawing.Color.White;
            this.hpLbl.Location = new System.Drawing.Point(6, 253);
            this.hpLbl.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.hpLbl.Name = "hpLbl";
            this.hpLbl.Size = new System.Drawing.Size(109, 23);
            this.hpLbl.TabIndex = 19;
            this.hpLbl.Text = "HP";
            this.hpLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // friendsGroup
            // 
            this.friendsGroup.Controls.Add(this.removeFriendBtn);
            this.friendsGroup.Controls.Add(this.targetFriendBtn);
            this.friendsGroup.Controls.Add(this.friendAltsBtn);
            this.friendsGroup.Controls.Add(this.groupFriendBtn);
            this.friendsGroup.Controls.Add(this.friendList);
            this.friendsGroup.Location = new System.Drawing.Point(635, 9);
            this.friendsGroup.Name = "friendsGroup";
            this.friendsGroup.Size = new System.Drawing.Size(172, 238);
            this.friendsGroup.TabIndex = 18;
            this.friendsGroup.TabStop = false;
            this.friendsGroup.Text = "Friends";
            // 
            // removeFriendBtn
            // 
            this.removeFriendBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeFriendBtn.ForeColor = System.Drawing.Color.Black;
            this.removeFriendBtn.Location = new System.Drawing.Point(102, 128);
            this.removeFriendBtn.Name = "removeFriendBtn";
            this.removeFriendBtn.Size = new System.Drawing.Size(64, 49);
            this.removeFriendBtn.TabIndex = 4;
            this.removeFriendBtn.Text = "Remove Selected";
            this.removeFriendBtn.UseVisualStyleBackColor = true;
            this.removeFriendBtn.Click += new System.EventHandler(this.removeFriendBtn_Click);
            // 
            // targetFriendBtn
            // 
            this.targetFriendBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.targetFriendBtn.ForeColor = System.Drawing.Color.Black;
            this.targetFriendBtn.Location = new System.Drawing.Point(102, 183);
            this.targetFriendBtn.Name = "targetFriendBtn";
            this.targetFriendBtn.Size = new System.Drawing.Size(64, 49);
            this.targetFriendBtn.TabIndex = 3;
            this.targetFriendBtn.Text = "Target Selected";
            this.targetFriendBtn.UseVisualStyleBackColor = true;
            // 
            // friendAltsBtn
            // 
            this.friendAltsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.friendAltsBtn.ForeColor = System.Drawing.Color.Black;
            this.friendAltsBtn.Location = new System.Drawing.Point(102, 73);
            this.friendAltsBtn.Name = "friendAltsBtn";
            this.friendAltsBtn.Size = new System.Drawing.Size(64, 49);
            this.friendAltsBtn.TabIndex = 2;
            this.friendAltsBtn.Text = "Add Alts";
            this.friendAltsBtn.UseVisualStyleBackColor = true;
            this.friendAltsBtn.Click += new System.EventHandler(this.friendAltsBtn_Click);
            // 
            // groupFriendBtn
            // 
            this.groupFriendBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupFriendBtn.ForeColor = System.Drawing.Color.Black;
            this.groupFriendBtn.Location = new System.Drawing.Point(102, 18);
            this.groupFriendBtn.Name = "groupFriendBtn";
            this.groupFriendBtn.Size = new System.Drawing.Size(64, 49);
            this.groupFriendBtn.TabIndex = 1;
            this.groupFriendBtn.Text = "Group Selected";
            this.groupFriendBtn.UseVisualStyleBackColor = true;
            this.groupFriendBtn.Click += new System.EventHandler(this.groupFriendBtn_Click);
            // 
            // friendList
            // 
            this.friendList.ForeColor = System.Drawing.Color.Black;
            this.friendList.FormattingEnabled = true;
            this.friendList.ItemHeight = 15;
            this.friendList.Location = new System.Drawing.Point(6, 18);
            this.friendList.Name = "friendList";
            this.friendList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.friendList.Size = new System.Drawing.Size(90, 199);
            this.friendList.Sorted = true;
            this.friendList.TabIndex = 0;
            // 
            // strangerGroup
            // 
            this.strangerGroup.Controls.Add(this.lastSeenBtn);
            this.strangerGroup.Controls.Add(this.friendStrangerBtn);
            this.strangerGroup.Controls.Add(this.targetStrangerBtn);
            this.strangerGroup.Controls.Add(this.groupStrangerBtn);
            this.strangerGroup.Controls.Add(this.strangerList);
            this.strangerGroup.Location = new System.Drawing.Point(220, 9);
            this.strangerGroup.Name = "strangerGroup";
            this.strangerGroup.Size = new System.Drawing.Size(172, 238);
            this.strangerGroup.TabIndex = 11;
            this.strangerGroup.TabStop = false;
            this.strangerGroup.Text = "Strangers";
            // 
            // lastSeenBtn
            // 
            this.lastSeenBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lastSeenBtn.ForeColor = System.Drawing.Color.Black;
            this.lastSeenBtn.Location = new System.Drawing.Point(102, 128);
            this.lastSeenBtn.Name = "lastSeenBtn";
            this.lastSeenBtn.Size = new System.Drawing.Size(64, 49);
            this.lastSeenBtn.TabIndex = 4;
            this.lastSeenBtn.Text = "Last Seen";
            this.lastSeenBtn.UseVisualStyleBackColor = true;
            this.lastSeenBtn.Click += new System.EventHandler(this.lastSeenBtn_Click);
            // 
            // friendStrangerBtn
            // 
            this.friendStrangerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.friendStrangerBtn.ForeColor = System.Drawing.Color.Black;
            this.friendStrangerBtn.Location = new System.Drawing.Point(102, 73);
            this.friendStrangerBtn.Name = "friendStrangerBtn";
            this.friendStrangerBtn.Size = new System.Drawing.Size(64, 49);
            this.friendStrangerBtn.TabIndex = 3;
            this.friendStrangerBtn.Text = "Friend Selected";
            this.friendStrangerBtn.UseVisualStyleBackColor = true;
            this.friendStrangerBtn.Click += new System.EventHandler(this.friendStrangerBtn_Click);
            // 
            // targetStrangerBtn
            // 
            this.targetStrangerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.targetStrangerBtn.ForeColor = System.Drawing.Color.Black;
            this.targetStrangerBtn.Location = new System.Drawing.Point(102, 183);
            this.targetStrangerBtn.Name = "targetStrangerBtn";
            this.targetStrangerBtn.Size = new System.Drawing.Size(64, 49);
            this.targetStrangerBtn.TabIndex = 2;
            this.targetStrangerBtn.Text = "Target Selected";
            this.targetStrangerBtn.UseVisualStyleBackColor = true;
            // 
            // groupStrangerBtn
            // 
            this.groupStrangerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupStrangerBtn.ForeColor = System.Drawing.Color.Black;
            this.groupStrangerBtn.Location = new System.Drawing.Point(102, 18);
            this.groupStrangerBtn.Name = "groupStrangerBtn";
            this.groupStrangerBtn.Size = new System.Drawing.Size(64, 49);
            this.groupStrangerBtn.TabIndex = 1;
            this.groupStrangerBtn.Text = "Group Selected";
            this.groupStrangerBtn.UseVisualStyleBackColor = true;
            this.groupStrangerBtn.Click += new System.EventHandler(this.groupStrangerBtn_Click);
            // 
            // strangerList
            // 
            this.strangerList.ForeColor = System.Drawing.Color.Black;
            this.strangerList.FormattingEnabled = true;
            this.strangerList.ItemHeight = 15;
            this.strangerList.Location = new System.Drawing.Point(6, 18);
            this.strangerList.Name = "strangerList";
            this.strangerList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.strangerList.Size = new System.Drawing.Size(90, 199);
            this.strangerList.TabIndex = 0;
            // 
            // groupGroup
            // 
            this.groupGroup.Controls.Add(this.friendGroupBtn);
            this.groupGroup.Controls.Add(this.groupAltsBtn);
            this.groupGroup.Controls.Add(this.kickGroupedBtn);
            this.groupGroup.Controls.Add(this.groupList);
            this.groupGroup.Location = new System.Drawing.Point(425, 9);
            this.groupGroup.Name = "groupGroup";
            this.groupGroup.Size = new System.Drawing.Size(172, 238);
            this.groupGroup.TabIndex = 10;
            this.groupGroup.TabStop = false;
            this.groupGroup.Text = "Grouped";
            // 
            // friendGroupBtn
            // 
            this.friendGroupBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.friendGroupBtn.ForeColor = System.Drawing.Color.Black;
            this.friendGroupBtn.Location = new System.Drawing.Point(102, 73);
            this.friendGroupBtn.Name = "friendGroupBtn";
            this.friendGroupBtn.Size = new System.Drawing.Size(64, 49);
            this.friendGroupBtn.TabIndex = 4;
            this.friendGroupBtn.Text = "Friend Selected";
            this.friendGroupBtn.UseVisualStyleBackColor = true;
            this.friendGroupBtn.Click += new System.EventHandler(this.friendGroupBtn_Click);
            // 
            // groupAltsBtn
            // 
            this.groupAltsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupAltsBtn.ForeColor = System.Drawing.Color.Black;
            this.groupAltsBtn.Location = new System.Drawing.Point(102, 18);
            this.groupAltsBtn.Name = "groupAltsBtn";
            this.groupAltsBtn.Size = new System.Drawing.Size(64, 49);
            this.groupAltsBtn.TabIndex = 2;
            this.groupAltsBtn.Text = "Group Alts";
            this.groupAltsBtn.UseVisualStyleBackColor = true;
            this.groupAltsBtn.Click += new System.EventHandler(this.groupAltsBtn_Click);
            // 
            // kickGroupedBtn
            // 
            this.kickGroupedBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.kickGroupedBtn.ForeColor = System.Drawing.Color.Black;
            this.kickGroupedBtn.Location = new System.Drawing.Point(102, 128);
            this.kickGroupedBtn.Name = "kickGroupedBtn";
            this.kickGroupedBtn.Size = new System.Drawing.Size(64, 49);
            this.kickGroupedBtn.TabIndex = 1;
            this.kickGroupedBtn.Text = "Kick Selected";
            this.kickGroupedBtn.UseVisualStyleBackColor = true;
            this.kickGroupedBtn.Click += new System.EventHandler(this.kickGroupedBtn_Click);
            // 
            // groupList
            // 
            this.groupList.ForeColor = System.Drawing.Color.Black;
            this.groupList.FormattingEnabled = true;
            this.groupList.ItemHeight = 15;
            this.groupList.Location = new System.Drawing.Point(6, 18);
            this.groupList.Name = "groupList";
            this.groupList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.groupList.Size = new System.Drawing.Size(90, 199);
            this.groupList.Sorted = true;
            this.groupList.TabIndex = 0;
            // 
            // addEnemyGroup
            // 
            this.addEnemyGroup.Controls.Add(this.addBossBtn);
            this.addEnemyGroup.Controls.Add(this.addMonsterLbl);
            this.addEnemyGroup.Controls.Add(this.lastClickedNameLbl);
            this.addEnemyGroup.Controls.Add(this.addAislingLbl);
            this.addEnemyGroup.Controls.Add(this.lastClickedIDLbl);
            this.addEnemyGroup.Controls.Add(this.addAltsBtn);
            this.addEnemyGroup.Controls.Add(this.addGroupBtn);
            this.addEnemyGroup.Controls.Add(this.addAislingText);
            this.addEnemyGroup.Controls.Add(this.addAislingBtn);
            this.addEnemyGroup.Controls.Add(this.addMonsterText);
            this.addEnemyGroup.Controls.Add(this.addMonsterBtn);
            this.addEnemyGroup.Controls.Add(this.addAllMonstersBtn);
            this.addEnemyGroup.Location = new System.Drawing.Point(6, 6);
            this.addEnemyGroup.Name = "addEnemyGroup";
            this.addEnemyGroup.Size = new System.Drawing.Size(177, 241);
            this.addEnemyGroup.TabIndex = 9;
            this.addEnemyGroup.TabStop = false;
            this.addEnemyGroup.Text = "Add Targets";
            // 
            // addBossBtn
            // 
            this.addBossBtn.Enabled = false;
            this.addBossBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addBossBtn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addBossBtn.ForeColor = System.Drawing.Color.Black;
            this.addBossBtn.Location = new System.Drawing.Point(9, 203);
            this.addBossBtn.Margin = new System.Windows.Forms.Padding(5);
            this.addBossBtn.Name = "addBossBtn";
            this.addBossBtn.Size = new System.Drawing.Size(50, 30);
            this.addBossBtn.TabIndex = 9;
            this.addBossBtn.Text = "Boss";
            this.addBossBtn.UseVisualStyleBackColor = true;
            // 
            // addMonsterLbl
            // 
            this.addMonsterLbl.AutoSize = true;
            this.addMonsterLbl.ForeColor = System.Drawing.Color.Black;
            this.addMonsterLbl.Location = new System.Drawing.Point(65, 156);
            this.addMonsterLbl.Margin = new System.Windows.Forms.Padding(3);
            this.addMonsterLbl.Name = "addMonsterLbl";
            this.addMonsterLbl.Size = new System.Drawing.Size(51, 15);
            this.addMonsterLbl.TabIndex = 8;
            this.addMonsterLbl.Text = "Enemies";
            // 
            // lastClickedNameLbl
            // 
            this.lastClickedNameLbl.ForeColor = System.Drawing.Color.Black;
            this.lastClickedNameLbl.Location = new System.Drawing.Point(9, 110);
            this.lastClickedNameLbl.Name = "lastClickedNameLbl";
            this.lastClickedNameLbl.Size = new System.Drawing.Size(158, 15);
            this.lastClickedNameLbl.TabIndex = 1;
            this.lastClickedNameLbl.Text = "Name:";
            this.lastClickedNameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // addAislingLbl
            // 
            this.addAislingLbl.AutoSize = true;
            this.addAislingLbl.ForeColor = System.Drawing.Color.Black;
            this.addAislingLbl.Location = new System.Drawing.Point(65, 19);
            this.addAislingLbl.Margin = new System.Windows.Forms.Padding(3);
            this.addAislingLbl.Name = "addAislingLbl";
            this.addAislingLbl.Size = new System.Drawing.Size(35, 15);
            this.addAislingLbl.TabIndex = 7;
            this.addAislingLbl.Text = "Allies";
            // 
            // lastClickedIDLbl
            // 
            this.lastClickedIDLbl.AutoSize = true;
            this.lastClickedIDLbl.ForeColor = System.Drawing.Color.Black;
            this.lastClickedIDLbl.Location = new System.Drawing.Point(6, 127);
            this.lastClickedIDLbl.Name = "lastClickedIDLbl";
            this.lastClickedIDLbl.Size = new System.Drawing.Size(21, 15);
            this.lastClickedIDLbl.TabIndex = 0;
            this.lastClickedIDLbl.Text = "ID:";
            this.lastClickedIDLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // addAltsBtn
            // 
            this.addAltsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addAltsBtn.ForeColor = System.Drawing.Color.Black;
            this.addAltsBtn.Location = new System.Drawing.Point(68, 66);
            this.addAltsBtn.Margin = new System.Windows.Forms.Padding(5);
            this.addAltsBtn.Name = "addAltsBtn";
            this.addAltsBtn.Size = new System.Drawing.Size(100, 30);
            this.addAltsBtn.TabIndex = 5;
            this.addAltsBtn.Text = "All Alts";
            this.addAltsBtn.UseVisualStyleBackColor = true;
            this.addAltsBtn.Click += new System.EventHandler(this.addAltsBtn_Click);
            // 
            // addGroupBtn
            // 
            this.addGroupBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addGroupBtn.ForeColor = System.Drawing.Color.Black;
            this.addGroupBtn.Location = new System.Drawing.Point(9, 66);
            this.addGroupBtn.Margin = new System.Windows.Forms.Padding(5);
            this.addGroupBtn.Name = "addGroupBtn";
            this.addGroupBtn.Size = new System.Drawing.Size(50, 30);
            this.addGroupBtn.TabIndex = 4;
            this.addGroupBtn.Text = "Group";
            this.addGroupBtn.UseVisualStyleBackColor = true;
            this.addGroupBtn.Click += new System.EventHandler(this.addGroupBtn_Click);
            // 
            // addAislingText
            // 
            this.addAislingText.BackColor = System.Drawing.Color.White;
            this.addAislingText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addAislingText.ForeColor = System.Drawing.Color.Black;
            this.addAislingText.Location = new System.Drawing.Point(68, 37);
            this.addAislingText.Margin = new System.Windows.Forms.Padding(0);
            this.addAislingText.MaxLength = 12;
            this.addAislingText.Name = "addAislingText";
            this.addAislingText.Size = new System.Drawing.Size(100, 23);
            this.addAislingText.TabIndex = 1;
            this.addAislingText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // addAislingBtn
            // 
            this.addAislingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addAislingBtn.ForeColor = System.Drawing.Color.Black;
            this.addAislingBtn.Location = new System.Drawing.Point(9, 31);
            this.addAislingBtn.Margin = new System.Windows.Forms.Padding(0);
            this.addAislingBtn.Name = "addAislingBtn";
            this.addAislingBtn.Size = new System.Drawing.Size(50, 30);
            this.addAislingBtn.TabIndex = 0;
            this.addAislingBtn.Text = "Ally";
            this.addAislingBtn.UseVisualStyleBackColor = true;
            this.addAislingBtn.Click += new System.EventHandler(this.addAislingBtn_Click);
            // 
            // addMonsterText
            // 
            this.addMonsterText.BackColor = System.Drawing.Color.White;
            this.addMonsterText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addMonsterText.ForeColor = System.Drawing.Color.Black;
            this.addMonsterText.Location = new System.Drawing.Point(67, 174);
            this.addMonsterText.Margin = new System.Windows.Forms.Padding(0);
            this.addMonsterText.MaxLength = 12;
            this.addMonsterText.Name = "addMonsterText";
            this.addMonsterText.Size = new System.Drawing.Size(100, 23);
            this.addMonsterText.TabIndex = 3;
            this.addMonsterText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // addMonsterBtn
            // 
            this.addMonsterBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addMonsterBtn.ForeColor = System.Drawing.Color.Black;
            this.addMonsterBtn.Location = new System.Drawing.Point(9, 168);
            this.addMonsterBtn.Margin = new System.Windows.Forms.Padding(0);
            this.addMonsterBtn.Name = "addMonsterBtn";
            this.addMonsterBtn.Size = new System.Drawing.Size(50, 30);
            this.addMonsterBtn.TabIndex = 2;
            this.addMonsterBtn.Text = "Sprite";
            this.addMonsterBtn.UseVisualStyleBackColor = true;
            this.addMonsterBtn.Click += new System.EventHandler(this.addMonsterBtn_Click);
            // 
            // addAllMonstersBtn
            // 
            this.addAllMonstersBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addAllMonstersBtn.ForeColor = System.Drawing.Color.Black;
            this.addAllMonstersBtn.Location = new System.Drawing.Point(67, 203);
            this.addAllMonstersBtn.Margin = new System.Windows.Forms.Padding(5);
            this.addAllMonstersBtn.Name = "addAllMonstersBtn";
            this.addAllMonstersBtn.Size = new System.Drawing.Size(100, 30);
            this.addAllMonstersBtn.TabIndex = 6;
            this.addAllMonstersBtn.Text = "All Monsters";
            this.addAllMonstersBtn.UseVisualStyleBackColor = true;
            this.addAllMonstersBtn.Click += new System.EventHandler(this.addAllMonstersBtn_Click);
            // 
            // coverEXPGrp
            // 
            this.coverEXPGrp.Controls.Add(this.autoMushroomCbox);
            this.coverEXPGrp.Controls.Add(this.mushroomCombox);
            this.coverEXPGrp.Controls.Add(this.expBoxedLbl);
            this.coverEXPGrp.Controls.Add(this.goldHourLbl);
            this.coverEXPGrp.Controls.Add(this.apHourLbl);
            this.coverEXPGrp.Controls.Add(this.expGemsLbl);
            this.coverEXPGrp.Controls.Add(this.doublesLbl);
            this.coverEXPGrp.Controls.Add(this.useGemBtn);
            this.coverEXPGrp.Controls.Add(this.useDoubleBtn);
            this.coverEXPGrp.Controls.Add(this.autoGemCbox);
            this.coverEXPGrp.Controls.Add(this.autoDoubleCbox);
            this.coverEXPGrp.Controls.Add(this.calcResetBtn);
            this.coverEXPGrp.Controls.Add(this.expGemsCombox);
            this.coverEXPGrp.Controls.Add(this.doublesCombox);
            this.coverEXPGrp.Controls.Add(this.expHourLbl);
            this.coverEXPGrp.Controls.Add(this.expSessionLbl);
            this.coverEXPGrp.Location = new System.Drawing.Point(635, 253);
            this.coverEXPGrp.Name = "coverEXPGrp";
            this.coverEXPGrp.Size = new System.Drawing.Size(201, 225);
            this.coverEXPGrp.TabIndex = 6;
            this.coverEXPGrp.TabStop = false;
            this.coverEXPGrp.Text = "EXP";
            // 
            // autoMushroomCbox
            // 
            this.autoMushroomCbox.AutoSize = true;
            this.autoMushroomCbox.ForeColor = System.Drawing.Color.Black;
            this.autoMushroomCbox.Location = new System.Drawing.Point(100, 155);
            this.autoMushroomCbox.Name = "autoMushroomCbox";
            this.autoMushroomCbox.Size = new System.Drawing.Size(52, 19);
            this.autoMushroomCbox.TabIndex = 30;
            this.autoMushroomCbox.Text = "Auto";
            this.autoMushroomCbox.UseVisualStyleBackColor = true;
            // 
            // mushroomCombox
            // 
            this.mushroomCombox.ForeColor = System.Drawing.Color.Black;
            this.mushroomCombox.FormattingEnabled = true;
            this.mushroomCombox.Items.AddRange(new object[] {
            "Best Available",
            "Double",
            "50 Percent",
            "Greatest",
            "Greater",
            "Great",
            "Experience Mushroom",
            "",
            ""});
            this.mushroomCombox.Location = new System.Drawing.Point(6, 152);
            this.mushroomCombox.Name = "mushroomCombox";
            this.mushroomCombox.Size = new System.Drawing.Size(88, 23);
            this.mushroomCombox.TabIndex = 29;
            this.mushroomCombox.Text = "Best Available";
            // 
            // expBoxedLbl
            // 
            this.expBoxedLbl.ForeColor = System.Drawing.Color.Black;
            this.expBoxedLbl.Location = new System.Drawing.Point(6, 19);
            this.expBoxedLbl.Name = "expBoxedLbl";
            this.expBoxedLbl.Size = new System.Drawing.Size(139, 15);
            this.expBoxedLbl.TabIndex = 28;
            this.expBoxedLbl.Text = "Boxed";
            this.expBoxedLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // goldHourLbl
            // 
            this.goldHourLbl.ForeColor = System.Drawing.Color.Black;
            this.goldHourLbl.Location = new System.Drawing.Point(6, 79);
            this.goldHourLbl.Name = "goldHourLbl";
            this.goldHourLbl.Size = new System.Drawing.Size(125, 15);
            this.goldHourLbl.TabIndex = 27;
            this.goldHourLbl.Text = "Gold/hr";
            this.goldHourLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // apHourLbl
            // 
            this.apHourLbl.ForeColor = System.Drawing.Color.Black;
            this.apHourLbl.Location = new System.Drawing.Point(6, 64);
            this.apHourLbl.Name = "apHourLbl";
            this.apHourLbl.Size = new System.Drawing.Size(125, 15);
            this.apHourLbl.TabIndex = 26;
            this.apHourLbl.Text = "AP/hr";
            this.apHourLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // expGemsLbl
            // 
            this.expGemsLbl.AutoSize = true;
            this.expGemsLbl.ForeColor = System.Drawing.Color.Black;
            this.expGemsLbl.Location = new System.Drawing.Point(6, 161);
            this.expGemsLbl.Name = "expGemsLbl";
            this.expGemsLbl.Size = new System.Drawing.Size(0, 15);
            this.expGemsLbl.TabIndex = 25;
            // 
            // doublesLbl
            // 
            this.doublesLbl.AutoSize = true;
            this.doublesLbl.ForeColor = System.Drawing.Color.Black;
            this.doublesLbl.Location = new System.Drawing.Point(5, 103);
            this.doublesLbl.Name = "doublesLbl";
            this.doublesLbl.Size = new System.Drawing.Size(140, 15);
            this.doublesLbl.TabIndex = 24;
            this.doublesLbl.Text = "Doubles and Mushrooms";
            // 
            // useGemBtn
            // 
            this.useGemBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.useGemBtn.ForeColor = System.Drawing.Color.Black;
            this.useGemBtn.Location = new System.Drawing.Point(158, 178);
            this.useGemBtn.Name = "useGemBtn";
            this.useGemBtn.Size = new System.Drawing.Size(43, 23);
            this.useGemBtn.TabIndex = 23;
            this.useGemBtn.Text = "Use";
            this.useGemBtn.UseVisualStyleBackColor = true;
            this.useGemBtn.Click += new System.EventHandler(this.useGemBtn_Click);
            // 
            // useDoubleBtn
            // 
            this.useDoubleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.useDoubleBtn.ForeColor = System.Drawing.Color.Black;
            this.useDoubleBtn.Location = new System.Drawing.Point(158, 122);
            this.useDoubleBtn.Name = "useDoubleBtn";
            this.useDoubleBtn.Size = new System.Drawing.Size(43, 23);
            this.useDoubleBtn.TabIndex = 22;
            this.useDoubleBtn.Text = "Use";
            this.useDoubleBtn.UseVisualStyleBackColor = true;
            this.useDoubleBtn.Click += new System.EventHandler(this.useDoubleBtn_Click);
            // 
            // autoGemCbox
            // 
            this.autoGemCbox.AutoSize = true;
            this.autoGemCbox.ForeColor = System.Drawing.Color.Black;
            this.autoGemCbox.Location = new System.Drawing.Point(100, 182);
            this.autoGemCbox.Name = "autoGemCbox";
            this.autoGemCbox.Size = new System.Drawing.Size(52, 19);
            this.autoGemCbox.TabIndex = 21;
            this.autoGemCbox.Text = "Auto";
            this.autoGemCbox.UseVisualStyleBackColor = true;
            // 
            // autoDoubleCbox
            // 
            this.autoDoubleCbox.AutoSize = true;
            this.autoDoubleCbox.ForeColor = System.Drawing.Color.Black;
            this.autoDoubleCbox.Location = new System.Drawing.Point(100, 126);
            this.autoDoubleCbox.Name = "autoDoubleCbox";
            this.autoDoubleCbox.Size = new System.Drawing.Size(52, 19);
            this.autoDoubleCbox.TabIndex = 18;
            this.autoDoubleCbox.Text = "Auto";
            this.autoDoubleCbox.UseVisualStyleBackColor = true;
            // 
            // calcResetBtn
            // 
            this.calcResetBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.calcResetBtn.ForeColor = System.Drawing.Color.Black;
            this.calcResetBtn.Location = new System.Drawing.Point(145, 8);
            this.calcResetBtn.Name = "calcResetBtn";
            this.calcResetBtn.Size = new System.Drawing.Size(56, 38);
            this.calcResetBtn.TabIndex = 4;
            this.calcResetBtn.Text = "Reset";
            this.calcResetBtn.UseVisualStyleBackColor = true;
            this.calcResetBtn.Click += new System.EventHandler(this.calcResetBtn_Click);
            // 
            // expGemsCombox
            // 
            this.expGemsCombox.ForeColor = System.Drawing.Color.Black;
            this.expGemsCombox.FormattingEnabled = true;
            this.expGemsCombox.Items.AddRange(new object[] {
            "Ascend HP",
            "Ascend MP"});
            this.expGemsCombox.Location = new System.Drawing.Point(6, 179);
            this.expGemsCombox.Name = "expGemsCombox";
            this.expGemsCombox.Size = new System.Drawing.Size(88, 23);
            this.expGemsCombox.TabIndex = 3;
            this.expGemsCombox.Text = "Ascend HP";
            // 
            // doublesCombox
            // 
            this.doublesCombox.ForeColor = System.Drawing.Color.Black;
            this.doublesCombox.FormattingEnabled = true;
            this.doublesCombox.Items.AddRange(new object[] {
            "Vday 100%",
            "Xmas 100%",
            "Xmas 50%",
            "Star 100%",
            "Kruna 100%",
            "Kruna 50%"});
            this.doublesCombox.Location = new System.Drawing.Point(6, 123);
            this.doublesCombox.Name = "doublesCombox";
            this.doublesCombox.Size = new System.Drawing.Size(88, 23);
            this.doublesCombox.TabIndex = 2;
            this.doublesCombox.Text = "Vday 100%";
            // 
            // expHourLbl
            // 
            this.expHourLbl.ForeColor = System.Drawing.Color.Black;
            this.expHourLbl.Location = new System.Drawing.Point(6, 49);
            this.expHourLbl.Name = "expHourLbl";
            this.expHourLbl.Size = new System.Drawing.Size(125, 15);
            this.expHourLbl.TabIndex = 1;
            this.expHourLbl.Text = "EXP/hr";
            this.expHourLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // expSessionLbl
            // 
            this.expSessionLbl.ForeColor = System.Drawing.Color.Black;
            this.expSessionLbl.Location = new System.Drawing.Point(6, 34);
            this.expSessionLbl.Name = "expSessionLbl";
            this.expSessionLbl.Size = new System.Drawing.Size(139, 15);
            this.expSessionLbl.TabIndex = 0;
            this.expSessionLbl.Text = "Session";
            this.expSessionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainAislingsTab
            // 
            this.mainAislingsTab.Controls.Add(this.aislingTabControl);
            this.mainAislingsTab.ForeColor = System.Drawing.Color.Black;
            this.mainAislingsTab.Location = new System.Drawing.Point(4, 24);
            this.mainAislingsTab.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.mainAislingsTab.Name = "mainAislingsTab";
            this.mainAislingsTab.Size = new System.Drawing.Size(842, 516);
            this.mainAislingsTab.TabIndex = 1;
            this.mainAislingsTab.Text = "Aislings";
            this.mainAislingsTab.UseVisualStyleBackColor = true;
            // 
            // aislingTabControl
            // 
            this.aislingTabControl.Controls.Add(this.selfTab);
            this.aislingTabControl.Controls.Add(this.nearbyAllyTab);
            this.aislingTabControl.Location = new System.Drawing.Point(0, 0);
            this.aislingTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.aislingTabControl.Name = "aislingTabControl";
            this.aislingTabControl.Padding = new System.Drawing.Point(0, 0);
            this.aislingTabControl.SelectedIndex = 0;
            this.aislingTabControl.Size = new System.Drawing.Size(846, 520);
            this.aislingTabControl.TabIndex = 0;
            // 
            // selfTab
            // 
            this.selfTab.BackColor = System.Drawing.Color.White;
            this.selfTab.Controls.Add(this.stuffGroup);
            this.selfTab.Controls.Add(this.overrideGroup);
            this.selfTab.Controls.Add(this.alertsGroup);
            this.selfTab.Controls.Add(this.usableItemsGroup);
            this.selfTab.Controls.Add(this.classSpecificGroup);
            this.selfTab.Controls.Add(this.walkGroup);
            this.selfTab.Controls.Add(this.botOptionsGroup);
            this.selfTab.Controls.Add(this.dispelsGroup);
            this.selfTab.Controls.Add(this.lootGroup);
            this.selfTab.Controls.Add(this.dionHealsGroup);
            this.selfTab.Controls.Add(this.fasAiteGroup);
            this.selfTab.ForeColor = System.Drawing.Color.Black;
            this.selfTab.Location = new System.Drawing.Point(4, 24);
            this.selfTab.Name = "selfTab";
            this.selfTab.Size = new System.Drawing.Size(838, 492);
            this.selfTab.TabIndex = 0;
            this.selfTab.Text = "Self";
            // 
            // stuffGroup
            // 
            this.stuffGroup.Controls.Add(this.lblSecs);
            this.stuffGroup.Controls.Add(this.numLastStepTime);
            this.stuffGroup.Controls.Add(this.chkLastStepF5);
            this.stuffGroup.Controls.Add(this.chkIgnoreDionWaypoints);
            this.stuffGroup.Controls.Add(this.chkNoCastOffense);
            this.stuffGroup.Controls.Add(this.optionsSkullSurrbox);
            this.stuffGroup.Controls.Add(this.safeFSTbox);
            this.stuffGroup.Controls.Add(this.safeFSCbox);
            this.stuffGroup.Controls.Add(this.optionsSkullCbox);
            this.stuffGroup.Location = new System.Drawing.Point(388, 294);
            this.stuffGroup.Name = "stuffGroup";
            this.stuffGroup.Size = new System.Drawing.Size(219, 189);
            this.stuffGroup.TabIndex = 12;
            this.stuffGroup.TabStop = false;
            this.stuffGroup.Text = "Stuff";
            // 
            // lblSecs
            // 
            this.lblSecs.AutoSize = true;
            this.lblSecs.Location = new System.Drawing.Point(180, 152);
            this.lblSecs.Name = "lblSecs";
            this.lblSecs.Size = new System.Drawing.Size(29, 15);
            this.lblSecs.TabIndex = 25;
            this.lblSecs.Text = "secs";
            // 
            // numLastStepTime
            // 
            this.numLastStepTime.Location = new System.Drawing.Point(145, 150);
            this.numLastStepTime.Name = "numLastStepTime";
            this.numLastStepTime.Size = new System.Drawing.Size(30, 23);
            this.numLastStepTime.TabIndex = 24;
            this.numLastStepTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numLastStepTime.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // chkLastStepF5
            // 
            this.chkLastStepF5.AutoSize = true;
            this.chkLastStepF5.ForeColor = System.Drawing.Color.Black;
            this.chkLastStepF5.Location = new System.Drawing.Point(7, 151);
            this.chkLastStepF5.Name = "chkLastStepF5";
            this.chkLastStepF5.Size = new System.Drawing.Size(140, 19);
            this.chkLastStepF5.TabIndex = 23;
            this.chkLastStepF5.Text = "Refresh if last move >";
            this.chkLastStepF5.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreDionWaypoints
            // 
            this.chkIgnoreDionWaypoints.AutoSize = true;
            this.chkIgnoreDionWaypoints.ForeColor = System.Drawing.Color.Black;
            this.chkIgnoreDionWaypoints.Location = new System.Drawing.Point(7, 129);
            this.chkIgnoreDionWaypoints.Name = "chkIgnoreDionWaypoints";
            this.chkIgnoreDionWaypoints.Size = new System.Drawing.Size(134, 19);
            this.chkIgnoreDionWaypoints.TabIndex = 22;
            this.chkIgnoreDionWaypoints.Text = "Ignore Dioned Mobs";
            this.chkIgnoreDionWaypoints.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkIgnoreDionWaypoints.UseVisualStyleBackColor = true;
            // 
            // chkNoCastOffense
            // 
            this.chkNoCastOffense.AutoSize = true;
            this.chkNoCastOffense.ForeColor = System.Drawing.Color.Black;
            this.chkNoCastOffense.Location = new System.Drawing.Point(7, 104);
            this.chkNoCastOffense.Name = "chkNoCastOffense";
            this.chkNoCastOffense.Size = new System.Drawing.Size(205, 19);
            this.chkNoCastOffense.TabIndex = 21;
            this.chkNoCastOffense.Text = "No Offense If Basher Waypointing";
            this.chkNoCastOffense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkNoCastOffense.UseVisualStyleBackColor = true;
            // 
            // optionsSkullSurrbox
            // 
            this.optionsSkullSurrbox.AutoSize = true;
            this.optionsSkullSurrbox.ForeColor = System.Drawing.Color.Black;
            this.optionsSkullSurrbox.Location = new System.Drawing.Point(7, 79);
            this.optionsSkullSurrbox.Name = "optionsSkullSurrbox";
            this.optionsSkullSurrbox.Size = new System.Drawing.Size(183, 19);
            this.optionsSkullSurrbox.TabIndex = 20;
            this.optionsSkullSurrbox.Text = "Log if skulled and surrounded";
            this.optionsSkullSurrbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optionsSkullSurrbox.UseVisualStyleBackColor = true;
            // 
            // safeFSTbox
            // 
            this.safeFSTbox.BackColor = System.Drawing.Color.White;
            this.safeFSTbox.ForeColor = System.Drawing.Color.Black;
            this.safeFSTbox.Location = new System.Drawing.Point(125, 20);
            this.safeFSTbox.Name = "safeFSTbox";
            this.safeFSTbox.Size = new System.Drawing.Size(76, 23);
            this.safeFSTbox.TabIndex = 19;
            this.safeFSTbox.Text = "50000";
            this.safeFSTbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // safeFSCbox
            // 
            this.safeFSCbox.AutoSize = true;
            this.safeFSCbox.ForeColor = System.Drawing.Color.Black;
            this.safeFSCbox.Location = new System.Drawing.Point(7, 22);
            this.safeFSCbox.Name = "safeFSCbox";
            this.safeFSCbox.Size = new System.Drawing.Size(104, 19);
            this.safeFSCbox.TabIndex = 19;
            this.safeFSCbox.Text = "Don\'t FS under";
            this.safeFSCbox.UseVisualStyleBackColor = true;
            // 
            // optionsSkullCbox
            // 
            this.optionsSkullCbox.AutoSize = true;
            this.optionsSkullCbox.ForeColor = System.Drawing.Color.Black;
            this.optionsSkullCbox.Location = new System.Drawing.Point(7, 54);
            this.optionsSkullCbox.Name = "optionsSkullCbox";
            this.optionsSkullCbox.Size = new System.Drawing.Size(148, 19);
            this.optionsSkullCbox.TabIndex = 19;
            this.optionsSkullCbox.Text = "Log if skulled for 5 secs";
            this.optionsSkullCbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.optionsSkullCbox.UseVisualStyleBackColor = true;
            // 
            // overrideGroup
            // 
            this.overrideGroup.Controls.Add(this.overrideDistanceLbl);
            this.overrideGroup.Controls.Add(this.overrideDistanceNum);
            this.overrideGroup.Controls.Add(this.getSpriteBtn);
            this.overrideGroup.Controls.Add(this.toggleOverrideCbox);
            this.overrideGroup.Controls.Add(this.removeOverrideBtn);
            this.overrideGroup.Controls.Add(this.addOverrideBtn);
            this.overrideGroup.Controls.Add(this.overrideText);
            this.overrideGroup.Controls.Add(this.overrideList);
            this.overrideGroup.Location = new System.Drawing.Point(613, 272);
            this.overrideGroup.Name = "overrideGroup";
            this.overrideGroup.Size = new System.Drawing.Size(219, 211);
            this.overrideGroup.TabIndex = 11;
            this.overrideGroup.TabStop = false;
            this.overrideGroup.Text = "Waypoint Item Override";
            // 
            // overrideDistanceLbl
            // 
            this.overrideDistanceLbl.AutoSize = true;
            this.overrideDistanceLbl.ForeColor = System.Drawing.Color.Black;
            this.overrideDistanceLbl.Location = new System.Drawing.Point(143, 35);
            this.overrideDistanceLbl.Name = "overrideDistanceLbl";
            this.overrideDistanceLbl.Size = new System.Drawing.Size(55, 15);
            this.overrideDistanceLbl.TabIndex = 21;
            this.overrideDistanceLbl.Text = "Distance:";
            // 
            // overrideDistanceNum
            // 
            this.overrideDistanceNum.ForeColor = System.Drawing.Color.Black;
            this.overrideDistanceNum.Location = new System.Drawing.Point(152, 53);
            this.overrideDistanceNum.Name = "overrideDistanceNum";
            this.overrideDistanceNum.Size = new System.Drawing.Size(43, 23);
            this.overrideDistanceNum.TabIndex = 20;
            this.overrideDistanceNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.overrideDistanceNum.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // getSpriteBtn
            // 
            this.getSpriteBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.getSpriteBtn.ForeColor = System.Drawing.Color.Black;
            this.getSpriteBtn.Location = new System.Drawing.Point(132, 81);
            this.getSpriteBtn.Name = "getSpriteBtn";
            this.getSpriteBtn.Size = new System.Drawing.Size(81, 44);
            this.getSpriteBtn.TabIndex = 19;
            this.getSpriteBtn.Text = "Grab Sprite (1st Slot)";
            this.getSpriteBtn.UseVisualStyleBackColor = true;
            this.getSpriteBtn.Click += new System.EventHandler(this.getSpriteBtn_Click);
            // 
            // toggleOverrideCbox
            // 
            this.toggleOverrideCbox.AutoSize = true;
            this.toggleOverrideCbox.ForeColor = System.Drawing.Color.Black;
            this.toggleOverrideCbox.Location = new System.Drawing.Point(153, 14);
            this.toggleOverrideCbox.Name = "toggleOverrideCbox";
            this.toggleOverrideCbox.Size = new System.Drawing.Size(61, 19);
            this.toggleOverrideCbox.TabIndex = 18;
            this.toggleOverrideCbox.Text = "Enable";
            this.toggleOverrideCbox.UseVisualStyleBackColor = true;
            // 
            // removeOverrideBtn
            // 
            this.removeOverrideBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeOverrideBtn.ForeColor = System.Drawing.Color.Black;
            this.removeOverrideBtn.Location = new System.Drawing.Point(132, 132);
            this.removeOverrideBtn.Name = "removeOverrideBtn";
            this.removeOverrideBtn.Size = new System.Drawing.Size(81, 44);
            this.removeOverrideBtn.TabIndex = 17;
            this.removeOverrideBtn.Text = "Remove Selected";
            this.removeOverrideBtn.UseVisualStyleBackColor = true;
            this.removeOverrideBtn.Click += new System.EventHandler(this.removeOverrideBtn_Click);
            // 
            // addOverrideBtn
            // 
            this.addOverrideBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addOverrideBtn.ForeColor = System.Drawing.Color.Black;
            this.addOverrideBtn.Location = new System.Drawing.Point(132, 181);
            this.addOverrideBtn.Name = "addOverrideBtn";
            this.addOverrideBtn.Size = new System.Drawing.Size(81, 23);
            this.addOverrideBtn.TabIndex = 16;
            this.addOverrideBtn.Text = "Add Sprite";
            this.addOverrideBtn.UseVisualStyleBackColor = true;
            this.addOverrideBtn.Click += new System.EventHandler(this.addOverrideBtn_Click);
            // 
            // overrideText
            // 
            this.overrideText.AcceptsReturn = true;
            this.overrideText.ForeColor = System.Drawing.Color.Black;
            this.overrideText.Location = new System.Drawing.Point(6, 181);
            this.overrideText.MaxLength = 25;
            this.overrideText.Name = "overrideText";
            this.overrideText.Size = new System.Drawing.Size(120, 23);
            this.overrideText.TabIndex = 10;
            this.overrideText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // overrideList
            // 
            this.overrideList.BackColor = System.Drawing.Color.White;
            this.overrideList.ForeColor = System.Drawing.Color.Black;
            this.overrideList.FormattingEnabled = true;
            this.overrideList.ItemHeight = 15;
            this.overrideList.Location = new System.Drawing.Point(6, 22);
            this.overrideList.Name = "overrideList";
            this.overrideList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.overrideList.Size = new System.Drawing.Size(120, 154);
            this.overrideList.TabIndex = 1;
            // 
            // alertsGroup
            // 
            this.alertsGroup.Controls.Add(this.alertItemCapCbox);
            this.alertsGroup.Controls.Add(this.alertRangerCbox);
            this.alertsGroup.Controls.Add(this.alertEXPCbox);
            this.alertsGroup.Controls.Add(this.alertSkulledCbox);
            this.alertsGroup.Controls.Add(this.alertDuraCbox);
            this.alertsGroup.Controls.Add(this.alertStrangerCbox);
            this.alertsGroup.Location = new System.Drawing.Point(388, 60);
            this.alertsGroup.Name = "alertsGroup";
            this.alertsGroup.Size = new System.Drawing.Size(219, 100);
            this.alertsGroup.TabIndex = 9;
            this.alertsGroup.TabStop = false;
            this.alertsGroup.Text = "Alerts";
            // 
            // alertItemCapCbox
            // 
            this.alertItemCapCbox.AutoSize = true;
            this.alertItemCapCbox.ForeColor = System.Drawing.Color.Black;
            this.alertItemCapCbox.Location = new System.Drawing.Point(11, 72);
            this.alertItemCapCbox.Name = "alertItemCapCbox";
            this.alertItemCapCbox.Size = new System.Drawing.Size(74, 19);
            this.alertItemCapCbox.TabIndex = 21;
            this.alertItemCapCbox.Text = "Item Cap";
            this.alertItemCapCbox.UseVisualStyleBackColor = true;
            // 
            // alertRangerCbox
            // 
            this.alertRangerCbox.AutoSize = true;
            this.alertRangerCbox.Checked = true;
            this.alertRangerCbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alertRangerCbox.ForeColor = System.Drawing.Color.Black;
            this.alertRangerCbox.Location = new System.Drawing.Point(11, 45);
            this.alertRangerCbox.Name = "alertRangerCbox";
            this.alertRangerCbox.Size = new System.Drawing.Size(100, 19);
            this.alertRangerCbox.TabIndex = 20;
            this.alertRangerCbox.Text = "Ranger Visible";
            this.alertRangerCbox.UseVisualStyleBackColor = true;
            // 
            // alertEXPCbox
            // 
            this.alertEXPCbox.AutoSize = true;
            this.alertEXPCbox.ForeColor = System.Drawing.Color.Black;
            this.alertEXPCbox.Location = new System.Drawing.Point(116, 72);
            this.alertEXPCbox.Name = "alertEXPCbox";
            this.alertEXPCbox.Size = new System.Drawing.Size(85, 19);
            this.alertEXPCbox.TabIndex = 19;
            this.alertEXPCbox.Text = "EXP Maxed";
            this.alertEXPCbox.UseVisualStyleBackColor = true;
            // 
            // alertSkulledCbox
            // 
            this.alertSkulledCbox.AutoSize = true;
            this.alertSkulledCbox.ForeColor = System.Drawing.Color.Black;
            this.alertSkulledCbox.Location = new System.Drawing.Point(116, 20);
            this.alertSkulledCbox.Name = "alertSkulledCbox";
            this.alertSkulledCbox.Size = new System.Drawing.Size(64, 19);
            this.alertSkulledCbox.TabIndex = 18;
            this.alertSkulledCbox.Text = "Skulled";
            this.alertSkulledCbox.UseVisualStyleBackColor = true;
            // 
            // alertDuraCbox
            // 
            this.alertDuraCbox.AutoSize = true;
            this.alertDuraCbox.ForeColor = System.Drawing.Color.Black;
            this.alertDuraCbox.Location = new System.Drawing.Point(116, 45);
            this.alertDuraCbox.Name = "alertDuraCbox";
            this.alertDuraCbox.Size = new System.Drawing.Size(102, 19);
            this.alertDuraCbox.TabIndex = 16;
            this.alertDuraCbox.Text = "30% Durability";
            this.alertDuraCbox.UseVisualStyleBackColor = true;
            // 
            // alertStrangerCbox
            // 
            this.alertStrangerCbox.AutoSize = true;
            this.alertStrangerCbox.ForeColor = System.Drawing.Color.Black;
            this.alertStrangerCbox.Location = new System.Drawing.Point(11, 20);
            this.alertStrangerCbox.Name = "alertStrangerCbox";
            this.alertStrangerCbox.Size = new System.Drawing.Size(99, 19);
            this.alertStrangerCbox.TabIndex = 15;
            this.alertStrangerCbox.Text = "Aisling Visible";
            this.alertStrangerCbox.UseVisualStyleBackColor = true;
            // 
            // usableItemsGroup
            // 
            this.usableItemsGroup.Controls.Add(this.equipmentrepairCbox);
            this.usableItemsGroup.Controls.Add(this.autoRedCbox);
            this.usableItemsGroup.Controls.Add(this.mantidScentCbox);
            this.usableItemsGroup.Controls.Add(this.fungusExtractCbox);
            this.usableItemsGroup.ForeColor = System.Drawing.Color.Black;
            this.usableItemsGroup.Location = new System.Drawing.Point(507, 166);
            this.usableItemsGroup.Name = "usableItemsGroup";
            this.usableItemsGroup.Size = new System.Drawing.Size(100, 122);
            this.usableItemsGroup.TabIndex = 7;
            this.usableItemsGroup.TabStop = false;
            this.usableItemsGroup.Text = "Items";
            // 
            // equipmentrepairCbox
            // 
            this.equipmentrepairCbox.AutoSize = true;
            this.equipmentrepairCbox.ForeColor = System.Drawing.Color.Black;
            this.equipmentrepairCbox.Location = new System.Drawing.Point(6, 97);
            this.equipmentrepairCbox.Name = "equipmentrepairCbox";
            this.equipmentrepairCbox.Size = new System.Drawing.Size(92, 19);
            this.equipmentrepairCbox.TabIndex = 15;
            this.equipmentrepairCbox.Text = "Equip Repair";
            this.equipmentrepairCbox.UseVisualStyleBackColor = true;
            // 
            // autoRedCbox
            // 
            this.autoRedCbox.AutoSize = true;
            this.autoRedCbox.ForeColor = System.Drawing.Color.Black;
            this.autoRedCbox.Location = new System.Drawing.Point(6, 22);
            this.autoRedCbox.Name = "autoRedCbox";
            this.autoRedCbox.Size = new System.Drawing.Size(75, 19);
            this.autoRedCbox.TabIndex = 14;
            this.autoRedCbox.Text = "Auto Red";
            this.autoRedCbox.UseVisualStyleBackColor = true;
            // 
            // mantidScentCbox
            // 
            this.mantidScentCbox.AutoSize = true;
            this.mantidScentCbox.ForeColor = System.Drawing.Color.Black;
            this.mantidScentCbox.Location = new System.Drawing.Point(6, 72);
            this.mantidScentCbox.Name = "mantidScentCbox";
            this.mantidScentCbox.Size = new System.Drawing.Size(93, 19);
            this.mantidScentCbox.TabIndex = 11;
            this.mantidScentCbox.Text = "MantidScent";
            this.mantidScentCbox.UseVisualStyleBackColor = true;
            // 
            // fungusExtractCbox
            // 
            this.fungusExtractCbox.AutoSize = true;
            this.fungusExtractCbox.ForeColor = System.Drawing.Color.Black;
            this.fungusExtractCbox.Location = new System.Drawing.Point(6, 47);
            this.fungusExtractCbox.Name = "fungusExtractCbox";
            this.fungusExtractCbox.Size = new System.Drawing.Size(84, 19);
            this.fungusExtractCbox.TabIndex = 9;
            this.fungusExtractCbox.Text = "F.B. Extract";
            this.fungusExtractCbox.UseVisualStyleBackColor = true;
            // 
            // classSpecificGroup
            // 
            this.classSpecificGroup.Controls.Add(this.vineText);
            this.classSpecificGroup.Controls.Add(this.vineCombox);
            this.classSpecificGroup.Controls.Add(this.vineyardCbox);
            this.classSpecificGroup.Controls.Add(this.monsterCallCbox);
            this.classSpecificGroup.Controls.Add(this.wakeScrollCbox);
            this.classSpecificGroup.Controls.Add(this.muscleStimulantCbox);
            this.classSpecificGroup.Controls.Add(this.nerveStimulantCbox);
            this.classSpecificGroup.Controls.Add(this.dragonScaleCbox);
            this.classSpecificGroup.Controls.Add(this.dragonsFireCbox);
            this.classSpecificGroup.Controls.Add(this.vanishingElixirCbox);
            this.classSpecificGroup.Controls.Add(this.fasSpioradText);
            this.classSpecificGroup.Controls.Add(this.manaWardCbox);
            this.classSpecificGroup.Controls.Add(this.fasSpioradCbox);
            this.classSpecificGroup.Controls.Add(this.hideCbox);
            this.classSpecificGroup.Controls.Add(this.asgallCbox);
            this.classSpecificGroup.Controls.Add(this.armachdCbox);
            this.classSpecificGroup.Controls.Add(this.disenchanterCbox);
            this.classSpecificGroup.Controls.Add(this.beagCradhCbox);
            this.classSpecificGroup.Controls.Add(this.druidFormCbox);
            this.classSpecificGroup.Controls.Add(this.regenerationCbox);
            this.classSpecificGroup.Controls.Add(this.mistCbox);
            this.classSpecificGroup.Controls.Add(this.perfectDefenseCbox);
            this.classSpecificGroup.Controls.Add(this.aegisSphereCbox);
            this.classSpecificGroup.ForeColor = System.Drawing.Color.Black;
            this.classSpecificGroup.Location = new System.Drawing.Point(174, 166);
            this.classSpecificGroup.Name = "classSpecificGroup";
            this.classSpecificGroup.Size = new System.Drawing.Size(327, 100);
            this.classSpecificGroup.TabIndex = 3;
            this.classSpecificGroup.TabStop = false;
            this.classSpecificGroup.Text = "Class Specific";
            // 
            // vineText
            // 
            this.vineText.BackColor = System.Drawing.Color.White;
            this.vineText.ForeColor = System.Drawing.Color.Black;
            this.vineText.Location = new System.Drawing.Point(221, 70);
            this.vineText.Name = "vineText";
            this.vineText.Size = new System.Drawing.Size(76, 23);
            this.vineText.TabIndex = 18;
            this.vineText.Text = "0000";
            this.vineText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.vineText.Visible = false;
            // 
            // vineCombox
            // 
            this.vineCombox.BackColor = System.Drawing.Color.White;
            this.vineCombox.ForeColor = System.Drawing.Color.Black;
            this.vineCombox.FormattingEnabled = true;
            this.vineCombox.Items.AddRange(new object[] {
            "Delay",
            "Alt MP <"});
            this.vineCombox.Location = new System.Drawing.Point(121, 70);
            this.vineCombox.Name = "vineCombox";
            this.vineCombox.Size = new System.Drawing.Size(86, 23);
            this.vineCombox.TabIndex = 17;
            this.vineCombox.Text = "Delay";
            this.vineCombox.Visible = false;
            // 
            // vineyardCbox
            // 
            this.vineyardCbox.AutoSize = true;
            this.vineyardCbox.Location = new System.Drawing.Point(6, 72);
            this.vineyardCbox.Name = "vineyardCbox";
            this.vineyardCbox.Size = new System.Drawing.Size(104, 19);
            this.vineyardCbox.TabIndex = 12;
            this.vineyardCbox.Text = "Lyliac Vineyard";
            this.vineyardCbox.UseVisualStyleBackColor = true;
            this.vineyardCbox.Visible = false;
            // 
            // monsterCallCbox
            // 
            this.monsterCallCbox.AutoSize = true;
            this.monsterCallCbox.ForeColor = System.Drawing.Color.Black;
            this.monsterCallCbox.Location = new System.Drawing.Point(121, 47);
            this.monsterCallCbox.Name = "monsterCallCbox";
            this.monsterCallCbox.Size = new System.Drawing.Size(93, 19);
            this.monsterCallCbox.TabIndex = 15;
            this.monsterCallCbox.Text = "Monster Call";
            this.monsterCallCbox.UseVisualStyleBackColor = true;
            this.monsterCallCbox.Visible = false;
            // 
            // wakeScrollCbox
            // 
            this.wakeScrollCbox.AutoSize = true;
            this.wakeScrollCbox.ForeColor = System.Drawing.Color.Black;
            this.wakeScrollCbox.Location = new System.Drawing.Point(120, 22);
            this.wakeScrollCbox.Name = "wakeScrollCbox";
            this.wakeScrollCbox.Size = new System.Drawing.Size(87, 19);
            this.wakeScrollCbox.TabIndex = 4;
            this.wakeScrollCbox.Text = "Wake Scroll";
            this.wakeScrollCbox.UseVisualStyleBackColor = true;
            this.wakeScrollCbox.Visible = false;
            // 
            // muscleStimulantCbox
            // 
            this.muscleStimulantCbox.AutoSize = true;
            this.muscleStimulantCbox.ForeColor = System.Drawing.Color.Black;
            this.muscleStimulantCbox.Location = new System.Drawing.Point(121, 22);
            this.muscleStimulantCbox.Name = "muscleStimulantCbox";
            this.muscleStimulantCbox.Size = new System.Drawing.Size(94, 19);
            this.muscleStimulantCbox.TabIndex = 12;
            this.muscleStimulantCbox.Text = "M. Stimulant";
            this.muscleStimulantCbox.UseVisualStyleBackColor = true;
            this.muscleStimulantCbox.Visible = false;
            // 
            // nerveStimulantCbox
            // 
            this.nerveStimulantCbox.AutoSize = true;
            this.nerveStimulantCbox.ForeColor = System.Drawing.Color.Black;
            this.nerveStimulantCbox.Location = new System.Drawing.Point(121, 22);
            this.nerveStimulantCbox.Name = "nerveStimulantCbox";
            this.nerveStimulantCbox.Size = new System.Drawing.Size(92, 19);
            this.nerveStimulantCbox.TabIndex = 13;
            this.nerveStimulantCbox.Text = "N. Stimulant";
            this.nerveStimulantCbox.UseVisualStyleBackColor = true;
            this.nerveStimulantCbox.Visible = false;
            // 
            // dragonScaleCbox
            // 
            this.dragonScaleCbox.AutoSize = true;
            this.dragonScaleCbox.ForeColor = System.Drawing.Color.Black;
            this.dragonScaleCbox.Location = new System.Drawing.Point(221, 47);
            this.dragonScaleCbox.Name = "dragonScaleCbox";
            this.dragonScaleCbox.Size = new System.Drawing.Size(103, 19);
            this.dragonScaleCbox.TabIndex = 7;
            this.dragonScaleCbox.Text = "Dragon\'s Scale";
            this.dragonScaleCbox.UseVisualStyleBackColor = true;
            this.dragonScaleCbox.Visible = false;
            // 
            // dragonsFireCbox
            // 
            this.dragonsFireCbox.AutoSize = true;
            this.dragonsFireCbox.ForeColor = System.Drawing.Color.Black;
            this.dragonsFireCbox.Location = new System.Drawing.Point(121, 47);
            this.dragonsFireCbox.Name = "dragonsFireCbox";
            this.dragonsFireCbox.Size = new System.Drawing.Size(95, 19);
            this.dragonsFireCbox.TabIndex = 10;
            this.dragonsFireCbox.Text = "Dragon\'s Fire";
            this.dragonsFireCbox.UseVisualStyleBackColor = true;
            this.dragonsFireCbox.Visible = false;
            // 
            // vanishingElixirCbox
            // 
            this.vanishingElixirCbox.AutoSize = true;
            this.vanishingElixirCbox.ForeColor = System.Drawing.Color.Black;
            this.vanishingElixirCbox.Location = new System.Drawing.Point(121, 47);
            this.vanishingElixirCbox.Name = "vanishingElixirCbox";
            this.vanishingElixirCbox.Size = new System.Drawing.Size(105, 19);
            this.vanishingElixirCbox.TabIndex = 16;
            this.vanishingElixirCbox.Text = "Vanishing Elixir";
            this.vanishingElixirCbox.UseVisualStyleBackColor = true;
            this.vanishingElixirCbox.Visible = false;
            // 
            // fasSpioradText
            // 
            this.fasSpioradText.BackColor = System.Drawing.Color.White;
            this.fasSpioradText.ForeColor = System.Drawing.Color.Black;
            this.fasSpioradText.Location = new System.Drawing.Point(221, 20);
            this.fasSpioradText.Name = "fasSpioradText";
            this.fasSpioradText.Size = new System.Drawing.Size(76, 23);
            this.fasSpioradText.TabIndex = 8;
            this.fasSpioradText.Text = "5000";
            this.fasSpioradText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.fasSpioradText.Visible = false;
            // 
            // manaWardCbox
            // 
            this.manaWardCbox.AutoSize = true;
            this.manaWardCbox.ForeColor = System.Drawing.Color.Black;
            this.manaWardCbox.Location = new System.Drawing.Point(6, 47);
            this.manaWardCbox.Name = "manaWardCbox";
            this.manaWardCbox.Size = new System.Drawing.Size(87, 19);
            this.manaWardCbox.TabIndex = 7;
            this.manaWardCbox.Text = "Mana Ward";
            this.manaWardCbox.UseVisualStyleBackColor = true;
            this.manaWardCbox.Visible = false;
            // 
            // fasSpioradCbox
            // 
            this.fasSpioradCbox.AutoSize = true;
            this.fasSpioradCbox.ForeColor = System.Drawing.Color.Black;
            this.fasSpioradCbox.Location = new System.Drawing.Point(121, 22);
            this.fasSpioradCbox.Name = "fasSpioradCbox";
            this.fasSpioradCbox.Size = new System.Drawing.Size(86, 19);
            this.fasSpioradCbox.TabIndex = 5;
            this.fasSpioradCbox.Text = "Fas Spiorad";
            this.fasSpioradCbox.UseVisualStyleBackColor = true;
            this.fasSpioradCbox.Visible = false;
            // 
            // hideCbox
            // 
            this.hideCbox.AutoSize = true;
            this.hideCbox.ForeColor = System.Drawing.Color.Black;
            this.hideCbox.Location = new System.Drawing.Point(121, 72);
            this.hideCbox.Name = "hideCbox";
            this.hideCbox.Size = new System.Drawing.Size(51, 19);
            this.hideCbox.TabIndex = 1;
            this.hideCbox.Text = "Hide";
            this.hideCbox.UseVisualStyleBackColor = true;
            this.hideCbox.Visible = false;
            // 
            // asgallCbox
            // 
            this.asgallCbox.AutoSize = true;
            this.asgallCbox.ForeColor = System.Drawing.Color.Black;
            this.asgallCbox.Location = new System.Drawing.Point(6, 72);
            this.asgallCbox.Name = "asgallCbox";
            this.asgallCbox.Size = new System.Drawing.Size(96, 19);
            this.asgallCbox.TabIndex = 10;
            this.asgallCbox.Text = "Asgall Faileas";
            this.asgallCbox.UseVisualStyleBackColor = true;
            this.asgallCbox.Visible = false;
            // 
            // armachdCbox
            // 
            this.armachdCbox.AutoSize = true;
            this.armachdCbox.ForeColor = System.Drawing.Color.Black;
            this.armachdCbox.Location = new System.Drawing.Point(6, 47);
            this.armachdCbox.Name = "armachdCbox";
            this.armachdCbox.Size = new System.Drawing.Size(75, 19);
            this.armachdCbox.TabIndex = 4;
            this.armachdCbox.Text = "Armachd";
            this.armachdCbox.UseVisualStyleBackColor = true;
            this.armachdCbox.Visible = false;
            // 
            // disenchanterCbox
            // 
            this.disenchanterCbox.AutoSize = true;
            this.disenchanterCbox.ForeColor = System.Drawing.Color.Black;
            this.disenchanterCbox.Location = new System.Drawing.Point(6, 22);
            this.disenchanterCbox.Name = "disenchanterCbox";
            this.disenchanterCbox.Size = new System.Drawing.Size(95, 19);
            this.disenchanterCbox.TabIndex = 4;
            this.disenchanterCbox.Text = "Disenchanter";
            this.disenchanterCbox.UseVisualStyleBackColor = true;
            this.disenchanterCbox.Visible = false;
            // 
            // beagCradhCbox
            // 
            this.beagCradhCbox.AutoSize = true;
            this.beagCradhCbox.ForeColor = System.Drawing.Color.Black;
            this.beagCradhCbox.Location = new System.Drawing.Point(6, 72);
            this.beagCradhCbox.Name = "beagCradhCbox";
            this.beagCradhCbox.Size = new System.Drawing.Size(87, 19);
            this.beagCradhCbox.TabIndex = 5;
            this.beagCradhCbox.Text = "Beag Cradh";
            this.beagCradhCbox.UseVisualStyleBackColor = true;
            this.beagCradhCbox.Visible = false;
            // 
            // druidFormCbox
            // 
            this.druidFormCbox.AutoSize = true;
            this.druidFormCbox.ForeColor = System.Drawing.Color.Black;
            this.druidFormCbox.Location = new System.Drawing.Point(6, 22);
            this.druidFormCbox.Name = "druidFormCbox";
            this.druidFormCbox.Size = new System.Drawing.Size(86, 19);
            this.druidFormCbox.TabIndex = 2;
            this.druidFormCbox.Text = "Druid Form";
            this.druidFormCbox.UseVisualStyleBackColor = true;
            this.druidFormCbox.Visible = false;
            // 
            // regenerationCbox
            // 
            this.regenerationCbox.AutoSize = true;
            this.regenerationCbox.ForeColor = System.Drawing.Color.Black;
            this.regenerationCbox.Location = new System.Drawing.Point(6, 22);
            this.regenerationCbox.Name = "regenerationCbox";
            this.regenerationCbox.Size = new System.Drawing.Size(96, 19);
            this.regenerationCbox.TabIndex = 8;
            this.regenerationCbox.Text = "Regeneration";
            this.regenerationCbox.UseVisualStyleBackColor = true;
            this.regenerationCbox.Visible = false;
            // 
            // mistCbox
            // 
            this.mistCbox.AutoSize = true;
            this.mistCbox.ForeColor = System.Drawing.Color.Black;
            this.mistCbox.Location = new System.Drawing.Point(121, 47);
            this.mistCbox.Name = "mistCbox";
            this.mistCbox.Size = new System.Drawing.Size(49, 19);
            this.mistCbox.TabIndex = 3;
            this.mistCbox.Text = "Mist";
            this.mistCbox.UseVisualStyleBackColor = true;
            this.mistCbox.Visible = false;
            // 
            // perfectDefenseCbox
            // 
            this.perfectDefenseCbox.AutoSize = true;
            this.perfectDefenseCbox.ForeColor = System.Drawing.Color.Black;
            this.perfectDefenseCbox.Location = new System.Drawing.Point(6, 47);
            this.perfectDefenseCbox.Name = "perfectDefenseCbox";
            this.perfectDefenseCbox.Size = new System.Drawing.Size(108, 19);
            this.perfectDefenseCbox.TabIndex = 9;
            this.perfectDefenseCbox.Text = "Perfect Defense";
            this.perfectDefenseCbox.UseVisualStyleBackColor = true;
            this.perfectDefenseCbox.Visible = false;
            // 
            // aegisSphereCbox
            // 
            this.aegisSphereCbox.AutoSize = true;
            this.aegisSphereCbox.ForeColor = System.Drawing.Color.Black;
            this.aegisSphereCbox.Location = new System.Drawing.Point(6, 22);
            this.aegisSphereCbox.Name = "aegisSphereCbox";
            this.aegisSphereCbox.Size = new System.Drawing.Size(94, 19);
            this.aegisSphereCbox.TabIndex = 6;
            this.aegisSphereCbox.Text = "Aegis Sphere";
            this.aegisSphereCbox.UseVisualStyleBackColor = true;
            this.aegisSphereCbox.Visible = false;
            // 
            // walkGroup
            // 
            this.walkGroup.Controls.Add(this.chkSpeedStrangers);
            this.walkGroup.Controls.Add(this.lockstepCbox);
            this.walkGroup.Controls.Add(this.spamBubbleCbox);
            this.walkGroup.Controls.Add(this.bubbleBlockCbox);
            this.walkGroup.Controls.Add(this.rangerStopCbox);
            this.walkGroup.Controls.Add(this.walkSpeedLbl);
            this.walkGroup.Controls.Add(this.walkFastLbl);
            this.walkGroup.Controls.Add(this.walkSlowLbl);
            this.walkGroup.Controls.Add(this.walkSpeedSldr);
            this.walkGroup.Controls.Add(this.walkAllClientsBtn);
            this.walkGroup.Controls.Add(this.walkBtn);
            this.walkGroup.Controls.Add(this.walkMapCombox);
            this.walkGroup.Controls.Add(this.tilesLbl);
            this.walkGroup.Controls.Add(this.followDistanceNum);
            this.walkGroup.Controls.Add(this.distanceLbl);
            this.walkGroup.Controls.Add(this.followText);
            this.walkGroup.Controls.Add(this.followCbox);
            this.walkGroup.ForeColor = System.Drawing.Color.Black;
            this.walkGroup.Location = new System.Drawing.Point(6, 323);
            this.walkGroup.Name = "walkGroup";
            this.walkGroup.Size = new System.Drawing.Size(376, 160);
            this.walkGroup.TabIndex = 6;
            this.walkGroup.TabStop = false;
            this.walkGroup.Text = "Walk";
            // 
            // chkSpeedStrangers
            // 
            this.chkSpeedStrangers.AutoSize = true;
            this.chkSpeedStrangers.ForeColor = System.Drawing.Color.Black;
            this.chkSpeedStrangers.Location = new System.Drawing.Point(255, 52);
            this.chkSpeedStrangers.Name = "chkSpeedStrangers";
            this.chkSpeedStrangers.Size = new System.Drawing.Size(124, 19);
            this.chkSpeedStrangers.TabIndex = 24;
            this.chkSpeedStrangers.Text = "Speed w/Strangers";
            this.chkSpeedStrangers.UseVisualStyleBackColor = true;
            // 
            // lockstepCbox
            // 
            this.lockstepCbox.AutoSize = true;
            this.lockstepCbox.ForeColor = System.Drawing.Color.Black;
            this.lockstepCbox.Location = new System.Drawing.Point(184, 52);
            this.lockstepCbox.Name = "lockstepCbox";
            this.lockstepCbox.Size = new System.Drawing.Size(73, 19);
            this.lockstepCbox.TabIndex = 23;
            this.lockstepCbox.Text = "Lockstep";
            this.lockstepCbox.UseVisualStyleBackColor = true;
            // 
            // spamBubbleCbox
            // 
            this.spamBubbleCbox.AutoSize = true;
            this.spamBubbleCbox.ForeColor = System.Drawing.Color.Black;
            this.spamBubbleCbox.Location = new System.Drawing.Point(99, 52);
            this.spamBubbleCbox.Name = "spamBubbleCbox";
            this.spamBubbleCbox.Size = new System.Drawing.Size(88, 19);
            this.spamBubbleCbox.TabIndex = 22;
            this.spamBubbleCbox.Text = "Spam Block";
            this.spamBubbleCbox.UseVisualStyleBackColor = true;
            // 
            // bubbleBlockCbox
            // 
            this.bubbleBlockCbox.AutoSize = true;
            this.bubbleBlockCbox.ForeColor = System.Drawing.Color.Black;
            this.bubbleBlockCbox.Location = new System.Drawing.Point(7, 52);
            this.bubbleBlockCbox.Name = "bubbleBlockCbox";
            this.bubbleBlockCbox.Size = new System.Drawing.Size(95, 19);
            this.bubbleBlockCbox.TabIndex = 21;
            this.bubbleBlockCbox.Text = "Bubble Block";
            this.bubbleBlockCbox.UseVisualStyleBackColor = true;
            // 
            // rangerStopCbox
            // 
            this.rangerStopCbox.AutoSize = true;
            this.rangerStopCbox.Checked = true;
            this.rangerStopCbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rangerStopCbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rangerStopCbox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rangerStopCbox.ForeColor = System.Drawing.Color.Red;
            this.rangerStopCbox.Location = new System.Drawing.Point(234, 128);
            this.rangerStopCbox.Name = "rangerStopCbox";
            this.rangerStopCbox.Size = new System.Drawing.Size(109, 21);
            this.rangerStopCbox.TabIndex = 20;
            this.rangerStopCbox.Text = "Stop if ranger";
            this.rangerStopCbox.UseVisualStyleBackColor = true;
            // 
            // walkSpeedLbl
            // 
            this.walkSpeedLbl.AutoSize = true;
            this.walkSpeedLbl.ForeColor = System.Drawing.Color.Black;
            this.walkSpeedLbl.Location = new System.Drawing.Point(281, 114);
            this.walkSpeedLbl.Name = "walkSpeedLbl";
            this.walkSpeedLbl.Size = new System.Drawing.Size(25, 15);
            this.walkSpeedLbl.TabIndex = 19;
            this.walkSpeedLbl.Text = "150";
            // 
            // walkFastLbl
            // 
            this.walkFastLbl.AutoSize = true;
            this.walkFastLbl.ForeColor = System.Drawing.Color.Black;
            this.walkFastLbl.Location = new System.Drawing.Point(336, 114);
            this.walkFastLbl.Name = "walkFastLbl";
            this.walkFastLbl.Size = new System.Drawing.Size(28, 15);
            this.walkFastLbl.TabIndex = 18;
            this.walkFastLbl.Text = "Fast";
            // 
            // walkSlowLbl
            // 
            this.walkSlowLbl.AutoSize = true;
            this.walkSlowLbl.ForeColor = System.Drawing.Color.Black;
            this.walkSlowLbl.Location = new System.Drawing.Point(217, 114);
            this.walkSlowLbl.Name = "walkSlowLbl";
            this.walkSlowLbl.Size = new System.Drawing.Size(32, 15);
            this.walkSlowLbl.TabIndex = 17;
            this.walkSlowLbl.Text = "Slow";
            // 
            // walkSpeedSldr
            // 
            this.walkSpeedSldr.AutoSize = false;
            this.walkSpeedSldr.LargeChange = 25;
            this.walkSpeedSldr.Location = new System.Drawing.Point(212, 68);
            this.walkSpeedSldr.Maximum = 1500;
            this.walkSpeedSldr.Minimum = 150;
            this.walkSpeedSldr.Name = "walkSpeedSldr";
            this.walkSpeedSldr.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.walkSpeedSldr.RightToLeftLayout = true;
            this.walkSpeedSldr.Size = new System.Drawing.Size(156, 41);
            this.walkSpeedSldr.TabIndex = 16;
            this.walkSpeedSldr.TickFrequency = 200;
            this.walkSpeedSldr.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.walkSpeedSldr.Value = 150;
            this.walkSpeedSldr.Scroll += new System.EventHandler(this.walkSpeedSldr_Scroll);
            // 
            // walkAllClientsBtn
            // 
            this.walkAllClientsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.walkAllClientsBtn.ForeColor = System.Drawing.Color.Black;
            this.walkAllClientsBtn.Location = new System.Drawing.Point(6, 115);
            this.walkAllClientsBtn.Name = "walkAllClientsBtn";
            this.walkAllClientsBtn.Size = new System.Drawing.Size(202, 30);
            this.walkAllClientsBtn.TabIndex = 15;
            this.walkAllClientsBtn.Text = "SET FOR ALL CLIENTS";
            this.walkAllClientsBtn.UseVisualStyleBackColor = true;
            this.walkAllClientsBtn.Click += new System.EventHandler(this.walkAllClientsBtn_Click);
            // 
            // walkBtn
            // 
            this.walkBtn.BackColor = System.Drawing.Color.White;
            this.walkBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.walkBtn.ForeColor = System.Drawing.Color.Black;
            this.walkBtn.Location = new System.Drawing.Point(133, 77);
            this.walkBtn.Name = "walkBtn";
            this.walkBtn.Size = new System.Drawing.Size(75, 33);
            this.walkBtn.TabIndex = 14;
            this.walkBtn.Text = "Walk";
            this.walkBtn.UseVisualStyleBackColor = false;
            this.walkBtn.Click += new System.EventHandler(this.walkBtn_Click);
            // 
            // walkMapCombox
            // 
            this.walkMapCombox.ForeColor = System.Drawing.Color.Black;
            this.walkMapCombox.FormattingEnabled = true;
            this.walkMapCombox.Items.AddRange(new object[] {
            "WayPoints",
            "Aman Jungle",
            "Aman Skills/Spells",
            "Andor Lobby",
            "Andor 80",
            "Andor 140",
            "Blackstar",
            "Chadul Mileth 1",
            "Chaos 1",
            "CR 31",
            "Crystal Caves",
            "Lost Ruins",
            "Mines",
            "MTG 10",
            "MTG 13",
            "MTG 16",
            "MTG 25",
            "Noam",
            "Nobis Storage",
            "Nobis 2-5",
            "Nobis 2-11",
            "Nobis 3-5",
            "Nobis 3-11",
            "Plamit Lobby",
            "Plamit Boss",
            "Shinewood 22",
            "Shinewood 36",
            "Shinewood 38",
            "Shinewood 43",
            "Succi Hair",
            "Tavaly",
            "Water Dungeon",
            "Yowien Territory",
            "YT 24",
            "YT Vine Rooms",
            "SW Lure"});
            this.walkMapCombox.Location = new System.Drawing.Point(6, 77);
            this.walkMapCombox.Name = "walkMapCombox";
            this.walkMapCombox.Size = new System.Drawing.Size(121, 23);
            this.walkMapCombox.TabIndex = 13;
            this.walkMapCombox.Text = "Select Location";
            // 
            // tilesLbl
            // 
            this.tilesLbl.AutoSize = true;
            this.tilesLbl.ForeColor = System.Drawing.Color.Black;
            this.tilesLbl.Location = new System.Drawing.Point(312, 23);
            this.tilesLbl.Name = "tilesLbl";
            this.tilesLbl.Size = new System.Drawing.Size(28, 15);
            this.tilesLbl.TabIndex = 12;
            this.tilesLbl.Text = "tiles";
            // 
            // followDistanceNum
            // 
            this.followDistanceNum.ForeColor = System.Drawing.Color.Black;
            this.followDistanceNum.Location = new System.Drawing.Point(272, 20);
            this.followDistanceNum.Name = "followDistanceNum";
            this.followDistanceNum.Size = new System.Drawing.Size(34, 23);
            this.followDistanceNum.TabIndex = 11;
            this.followDistanceNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.followDistanceNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // distanceLbl
            // 
            this.distanceLbl.AutoSize = true;
            this.distanceLbl.ForeColor = System.Drawing.Color.Black;
            this.distanceLbl.Location = new System.Drawing.Point(191, 23);
            this.distanceLbl.Name = "distanceLbl";
            this.distanceLbl.Size = new System.Drawing.Size(78, 15);
            this.distanceLbl.TabIndex = 5;
            this.distanceLbl.Text = "at distance of";
            // 
            // followText
            // 
            this.followText.ForeColor = System.Drawing.Color.Black;
            this.followText.Location = new System.Drawing.Point(107, 20);
            this.followText.Name = "followText";
            this.followText.Size = new System.Drawing.Size(78, 23);
            this.followText.TabIndex = 4;
            // 
            // followCbox
            // 
            this.followCbox.AutoSize = true;
            this.followCbox.ForeColor = System.Drawing.Color.Black;
            this.followCbox.Location = new System.Drawing.Point(6, 22);
            this.followCbox.Name = "followCbox";
            this.followCbox.Size = new System.Drawing.Size(95, 19);
            this.followCbox.TabIndex = 3;
            this.followCbox.Text = "Follow target";
            this.followCbox.UseVisualStyleBackColor = true;
            // 
            // botOptionsGroup
            // 
            this.botOptionsGroup.Controls.Add(this.rangerLogCbox);
            this.botOptionsGroup.Controls.Add(this.oneLineWalkCbox);
            this.botOptionsGroup.Controls.Add(this.hideLinesCbox);
            this.botOptionsGroup.Controls.Add(this.autoStaffCbox);
            this.botOptionsGroup.ForeColor = System.Drawing.Color.Black;
            this.botOptionsGroup.Location = new System.Drawing.Point(6, 6);
            this.botOptionsGroup.Name = "botOptionsGroup";
            this.botOptionsGroup.Size = new System.Drawing.Size(601, 48);
            this.botOptionsGroup.TabIndex = 5;
            this.botOptionsGroup.TabStop = false;
            this.botOptionsGroup.Text = "Options";
            // 
            // rangerLogCbox
            // 
            this.rangerLogCbox.AutoSize = true;
            this.rangerLogCbox.ForeColor = System.Drawing.Color.Black;
            this.rangerLogCbox.Location = new System.Drawing.Point(393, 20);
            this.rangerLogCbox.Name = "rangerLogCbox";
            this.rangerLogCbox.Size = new System.Drawing.Size(122, 19);
            this.rangerLogCbox.TabIndex = 21;
            this.rangerLogCbox.Text = "Ranger/GM Safety";
            this.rangerLogCbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rangerLogCbox.UseVisualStyleBackColor = true;
            // 
            // oneLineWalkCbox
            // 
            this.oneLineWalkCbox.AutoSize = true;
            this.oneLineWalkCbox.ForeColor = System.Drawing.Color.Black;
            this.oneLineWalkCbox.Location = new System.Drawing.Point(262, 20);
            this.oneLineWalkCbox.Name = "oneLineWalkCbox";
            this.oneLineWalkCbox.Size = new System.Drawing.Size(97, 19);
            this.oneLineWalkCbox.TabIndex = 20;
            this.oneLineWalkCbox.Text = "One line walk";
            this.oneLineWalkCbox.UseVisualStyleBackColor = true;
            // 
            // hideLinesCbox
            // 
            this.hideLinesCbox.AutoSize = true;
            this.hideLinesCbox.ForeColor = System.Drawing.Color.Black;
            this.hideLinesCbox.Location = new System.Drawing.Point(133, 20);
            this.hideLinesCbox.Name = "hideLinesCbox";
            this.hideLinesCbox.Size = new System.Drawing.Size(105, 19);
            this.hideLinesCbox.TabIndex = 1;
            this.hideLinesCbox.Text = "Hide spell lines";
            this.hideLinesCbox.UseVisualStyleBackColor = true;
            // 
            // autoStaffCbox
            // 
            this.autoStaffCbox.AutoSize = true;
            this.autoStaffCbox.Checked = true;
            this.autoStaffCbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoStaffCbox.ForeColor = System.Drawing.Color.Black;
            this.autoStaffCbox.Location = new System.Drawing.Point(7, 20);
            this.autoStaffCbox.Name = "autoStaffCbox";
            this.autoStaffCbox.Size = new System.Drawing.Size(115, 19);
            this.autoStaffCbox.TabIndex = 0;
            this.autoStaffCbox.Text = "Auto staff switch";
            this.autoStaffCbox.UseVisualStyleBackColor = true;
            // 
            // dispelsGroup
            // 
            this.dispelsGroup.Controls.Add(this.aoPoisonCbox);
            this.dispelsGroup.Controls.Add(this.aoCurseCbox);
            this.dispelsGroup.Controls.Add(this.aoSuainCbox);
            this.dispelsGroup.ForeColor = System.Drawing.Color.Black;
            this.dispelsGroup.Location = new System.Drawing.Point(6, 272);
            this.dispelsGroup.Name = "dispelsGroup";
            this.dispelsGroup.Size = new System.Drawing.Size(376, 45);
            this.dispelsGroup.TabIndex = 0;
            this.dispelsGroup.TabStop = false;
            this.dispelsGroup.Text = "Dispels";
            // 
            // aoPoisonCbox
            // 
            this.aoPoisonCbox.AutoSize = true;
            this.aoPoisonCbox.ForeColor = System.Drawing.Color.Black;
            this.aoPoisonCbox.Location = new System.Drawing.Point(262, 22);
            this.aoPoisonCbox.Name = "aoPoisonCbox";
            this.aoPoisonCbox.Size = new System.Drawing.Size(89, 19);
            this.aoPoisonCbox.TabIndex = 10;
            this.aoPoisonCbox.Text = "Ao Puinsein";
            this.aoPoisonCbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.aoPoisonCbox.UseVisualStyleBackColor = true;
            // 
            // aoCurseCbox
            // 
            this.aoCurseCbox.AutoSize = true;
            this.aoCurseCbox.ForeColor = System.Drawing.Color.Black;
            this.aoCurseCbox.Location = new System.Drawing.Point(133, 22);
            this.aoCurseCbox.Name = "aoCurseCbox";
            this.aoCurseCbox.Size = new System.Drawing.Size(74, 19);
            this.aoCurseCbox.TabIndex = 9;
            this.aoCurseCbox.Text = "Ao Curse";
            this.aoCurseCbox.UseVisualStyleBackColor = true;
            // 
            // aoSuainCbox
            // 
            this.aoSuainCbox.AutoSize = true;
            this.aoSuainCbox.ForeColor = System.Drawing.Color.Black;
            this.aoSuainCbox.Location = new System.Drawing.Point(6, 22);
            this.aoSuainCbox.Name = "aoSuainCbox";
            this.aoSuainCbox.Size = new System.Drawing.Size(73, 19);
            this.aoSuainCbox.TabIndex = 8;
            this.aoSuainCbox.Text = "Ao Suain";
            this.aoSuainCbox.UseVisualStyleBackColor = true;
            // 
            // lootGroup
            // 
            this.lootGroup.Controls.Add(this.dropTrashCbox);
            this.lootGroup.Controls.Add(this.removeTrashBtn);
            this.lootGroup.Controls.Add(this.addTrashBtn);
            this.lootGroup.Controls.Add(this.pickupItemsCbox);
            this.lootGroup.Controls.Add(this.pickupGoldCbox);
            this.lootGroup.Controls.Add(this.lootPickupLbl);
            this.lootGroup.Controls.Add(this.lootDropLbl);
            this.lootGroup.Controls.Add(this.addTrashText);
            this.lootGroup.Controls.Add(this.trashList);
            this.lootGroup.ForeColor = System.Drawing.Color.Black;
            this.lootGroup.Location = new System.Drawing.Point(613, 6);
            this.lootGroup.Name = "lootGroup";
            this.lootGroup.Size = new System.Drawing.Size(219, 260);
            this.lootGroup.TabIndex = 2;
            this.lootGroup.TabStop = false;
            this.lootGroup.Text = "Loot && Trash";
            // 
            // dropTrashCbox
            // 
            this.dropTrashCbox.AutoSize = true;
            this.dropTrashCbox.ForeColor = System.Drawing.Color.Black;
            this.dropTrashCbox.Location = new System.Drawing.Point(146, 111);
            this.dropTrashCbox.Name = "dropTrashCbox";
            this.dropTrashCbox.Size = new System.Drawing.Size(53, 34);
            this.dropTrashCbox.TabIndex = 17;
            this.dropTrashCbox.Text = "Drop\r\nTrash";
            this.dropTrashCbox.UseVisualStyleBackColor = true;
            // 
            // removeTrashBtn
            // 
            this.removeTrashBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeTrashBtn.ForeColor = System.Drawing.Color.Black;
            this.removeTrashBtn.Location = new System.Drawing.Point(132, 180);
            this.removeTrashBtn.Name = "removeTrashBtn";
            this.removeTrashBtn.Size = new System.Drawing.Size(81, 44);
            this.removeTrashBtn.TabIndex = 16;
            this.removeTrashBtn.Text = "Remove Selected";
            this.removeTrashBtn.UseVisualStyleBackColor = true;
            this.removeTrashBtn.Click += new System.EventHandler(this.removeTrashBtn_Click);
            // 
            // addTrashBtn
            // 
            this.addTrashBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addTrashBtn.ForeColor = System.Drawing.Color.Black;
            this.addTrashBtn.Location = new System.Drawing.Point(132, 230);
            this.addTrashBtn.Name = "addTrashBtn";
            this.addTrashBtn.Size = new System.Drawing.Size(81, 23);
            this.addTrashBtn.TabIndex = 15;
            this.addTrashBtn.Text = "Add Trash";
            this.addTrashBtn.UseVisualStyleBackColor = true;
            this.addTrashBtn.Click += new System.EventHandler(this.addTrashBtn_Click);
            // 
            // pickupItemsCbox
            // 
            this.pickupItemsCbox.AutoSize = true;
            this.pickupItemsCbox.ForeColor = System.Drawing.Color.Black;
            this.pickupItemsCbox.Location = new System.Drawing.Point(146, 69);
            this.pickupItemsCbox.Name = "pickupItemsCbox";
            this.pickupItemsCbox.Size = new System.Drawing.Size(55, 19);
            this.pickupItemsCbox.TabIndex = 13;
            this.pickupItemsCbox.Text = "Items";
            this.pickupItemsCbox.UseVisualStyleBackColor = true;
            // 
            // pickupGoldCbox
            // 
            this.pickupGoldCbox.AutoSize = true;
            this.pickupGoldCbox.ForeColor = System.Drawing.Color.Black;
            this.pickupGoldCbox.Location = new System.Drawing.Point(146, 44);
            this.pickupGoldCbox.Name = "pickupGoldCbox";
            this.pickupGoldCbox.Size = new System.Drawing.Size(51, 19);
            this.pickupGoldCbox.TabIndex = 12;
            this.pickupGoldCbox.Text = "Gold";
            this.pickupGoldCbox.UseVisualStyleBackColor = true;
            // 
            // lootPickupLbl
            // 
            this.lootPickupLbl.AutoSize = true;
            this.lootPickupLbl.ForeColor = System.Drawing.Color.Black;
            this.lootPickupLbl.Location = new System.Drawing.Point(149, 22);
            this.lootPickupLbl.Name = "lootPickupLbl";
            this.lootPickupLbl.Size = new System.Drawing.Size(46, 15);
            this.lootPickupLbl.TabIndex = 11;
            this.lootPickupLbl.Text = "Pickup:";
            // 
            // lootDropLbl
            // 
            this.lootDropLbl.AutoSize = true;
            this.lootDropLbl.ForeColor = System.Drawing.Color.Black;
            this.lootDropLbl.Location = new System.Drawing.Point(50, 22);
            this.lootDropLbl.Name = "lootDropLbl";
            this.lootDropLbl.Size = new System.Drawing.Size(36, 15);
            this.lootDropLbl.TabIndex = 10;
            this.lootDropLbl.Text = "Drop:";
            // 
            // addTrashText
            // 
            this.addTrashText.AcceptsReturn = true;
            this.addTrashText.ForeColor = System.Drawing.Color.Black;
            this.addTrashText.Location = new System.Drawing.Point(6, 230);
            this.addTrashText.MaxLength = 25;
            this.addTrashText.Name = "addTrashText";
            this.addTrashText.Size = new System.Drawing.Size(120, 23);
            this.addTrashText.TabIndex = 9;
            this.addTrashText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // trashList
            // 
            this.trashList.BackColor = System.Drawing.Color.White;
            this.trashList.ForeColor = System.Drawing.Color.Black;
            this.trashList.FormattingEnabled = true;
            this.trashList.ItemHeight = 15;
            this.trashList.Items.AddRange(new object[] {
            "Amber Necklace",
            "Blue Potion",
            "Bone Necklace",
            "Cordovan Boots",
            "fior athar",
            "fior creag",
            "fior sal",
            "fior srad",
            "Goblin Helmet",
            "Gold Jade Necklace",
            "Half Talisman",
            "Holy Apollo",
            "Holy Diana",
            "Holy Gaea",
            "Hy-brasyl Belt",
            "Hy-brasyl Bracer",
            "Hy-brasyl Gauntlet",
            "Iron Greaves",
            "Light Belt",
            "Magma Boots",
            "Magus Apollo",
            "Magus Diana",
            "Magus Gaea",
            "Passion Flower",
            "Purple Potion",
            "Shagreen Boots",
            "Useless Boot"});
            this.trashList.Location = new System.Drawing.Point(6, 40);
            this.trashList.Name = "trashList";
            this.trashList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.trashList.Size = new System.Drawing.Size(120, 169);
            this.trashList.Sorted = true;
            this.trashList.TabIndex = 0;
            // 
            // dionHealsGroup
            // 
            this.dionHealsGroup.Controls.Add(this.aoSithCbox);
            this.dionHealsGroup.Controls.Add(this.deireasFaileasCbox);
            this.dionHealsGroup.Controls.Add(this.healCbox);
            this.dionHealsGroup.Controls.Add(this.dionCbox);
            this.dionHealsGroup.Controls.Add(this.healPctNum);
            this.dionHealsGroup.Controls.Add(this.healCombox);
            this.dionHealsGroup.Controls.Add(this.dionPctNum);
            this.dionHealsGroup.Controls.Add(this.dionWhenCombox);
            this.dionHealsGroup.Controls.Add(this.dionWhenLbl);
            this.dionHealsGroup.Controls.Add(this.dionCombox);
            this.dionHealsGroup.ForeColor = System.Drawing.Color.Black;
            this.dionHealsGroup.Location = new System.Drawing.Point(6, 60);
            this.dionHealsGroup.Name = "dionHealsGroup";
            this.dionHealsGroup.Size = new System.Drawing.Size(376, 100);
            this.dionHealsGroup.TabIndex = 1;
            this.dionHealsGroup.TabStop = false;
            this.dionHealsGroup.Text = "Dion && Heals";
            // 
            // aoSithCbox
            // 
            this.aoSithCbox.AutoSize = true;
            this.aoSithCbox.ForeColor = System.Drawing.Color.Black;
            this.aoSithCbox.Location = new System.Drawing.Point(262, 76);
            this.aoSithCbox.Name = "aoSithCbox";
            this.aoSithCbox.Size = new System.Drawing.Size(77, 19);
            this.aoSithCbox.TabIndex = 13;
            this.aoSithCbox.Text = "Ao Sithed";
            this.aoSithCbox.UseVisualStyleBackColor = true;
            // 
            // deireasFaileasCbox
            // 
            this.deireasFaileasCbox.AutoSize = true;
            this.deireasFaileasCbox.Location = new System.Drawing.Point(262, 51);
            this.deireasFaileasCbox.Name = "deireasFaileasCbox";
            this.deireasFaileasCbox.Size = new System.Drawing.Size(102, 19);
            this.deireasFaileasCbox.TabIndex = 12;
            this.deireasFaileasCbox.Text = "Deireas Faileas";
            this.deireasFaileasCbox.UseVisualStyleBackColor = true;
            // 
            // healCbox
            // 
            this.healCbox.AutoSize = true;
            this.healCbox.Location = new System.Drawing.Point(6, 67);
            this.healCbox.Name = "healCbox";
            this.healCbox.Size = new System.Drawing.Size(15, 14);
            this.healCbox.TabIndex = 12;
            this.healCbox.UseVisualStyleBackColor = true;
            // 
            // dionCbox
            // 
            this.dionCbox.AutoSize = true;
            this.dionCbox.Location = new System.Drawing.Point(6, 26);
            this.dionCbox.Name = "dionCbox";
            this.dionCbox.Size = new System.Drawing.Size(15, 14);
            this.dionCbox.TabIndex = 11;
            this.dionCbox.UseVisualStyleBackColor = true;
            // 
            // healPctNum
            // 
            this.healPctNum.ForeColor = System.Drawing.Color.Black;
            this.healPctNum.Location = new System.Drawing.Point(154, 63);
            this.healPctNum.Name = "healPctNum";
            this.healPctNum.Size = new System.Drawing.Size(43, 23);
            this.healPctNum.TabIndex = 10;
            this.healPctNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.healPctNum.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // healCombox
            // 
            this.healCombox.ForeColor = System.Drawing.Color.Black;
            this.healCombox.FormattingEnabled = true;
            this.healCombox.Items.AddRange(new object[] {
            "nuadhaich",
            "ard ioc",
            "mor ioc",
            "ioc",
            "beag ioc",
            "Cold Blood",
            "Spirit Essence"});
            this.healCombox.Location = new System.Drawing.Point(27, 63);
            this.healCombox.Name = "healCombox";
            this.healCombox.Size = new System.Drawing.Size(121, 23);
            this.healCombox.TabIndex = 6;
            this.healCombox.Text = "nuadhaich";
            // 
            // dionPctNum
            // 
            this.dionPctNum.ForeColor = System.Drawing.Color.Black;
            this.dionPctNum.Location = new System.Drawing.Point(321, 22);
            this.dionPctNum.Name = "dionPctNum";
            this.dionPctNum.Size = new System.Drawing.Size(43, 23);
            this.dionPctNum.TabIndex = 5;
            this.dionPctNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.dionPctNum.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // dionWhenCombox
            // 
            this.dionWhenCombox.ForeColor = System.Drawing.Color.Black;
            this.dionWhenCombox.FormattingEnabled = true;
            this.dionWhenCombox.Items.AddRange(new object[] {
            "Always",
            "In Danger",
            "Taking Damage",
            "At Percent",
            "Green Not Nearby"});
            this.dionWhenCombox.Location = new System.Drawing.Point(194, 22);
            this.dionWhenCombox.Name = "dionWhenCombox";
            this.dionWhenCombox.Size = new System.Drawing.Size(121, 23);
            this.dionWhenCombox.TabIndex = 4;
            this.dionWhenCombox.Text = "Always";
            // 
            // dionWhenLbl
            // 
            this.dionWhenLbl.AutoSize = true;
            this.dionWhenLbl.ForeColor = System.Drawing.Color.Black;
            this.dionWhenLbl.Location = new System.Drawing.Point(154, 25);
            this.dionWhenLbl.Name = "dionWhenLbl";
            this.dionWhenLbl.Size = new System.Drawing.Size(39, 15);
            this.dionWhenLbl.TabIndex = 3;
            this.dionWhenLbl.Text = "when:";
            // 
            // dionCombox
            // 
            this.dionCombox.ForeColor = System.Drawing.Color.Black;
            this.dionCombox.FormattingEnabled = true;
            this.dionCombox.Items.AddRange(new object[] {
            "mor dion",
            "Iron Skin",
            "Wings of Protection",
            "Draco Stance",
            "dion",
            "Stone Skin",
            "Glowing Stone"});
            this.dionCombox.Location = new System.Drawing.Point(27, 22);
            this.dionCombox.Name = "dionCombox";
            this.dionCombox.Size = new System.Drawing.Size(121, 23);
            this.dionCombox.TabIndex = 1;
            this.dionCombox.Text = "mor dion";
            // 
            // fasAiteGroup
            // 
            this.fasAiteGroup.Controls.Add(this.fasCbox);
            this.fasAiteGroup.Controls.Add(this.aiteCbox);
            this.fasAiteGroup.Controls.Add(this.fasCombox);
            this.fasAiteGroup.Controls.Add(this.aiteCombox);
            this.fasAiteGroup.ForeColor = System.Drawing.Color.Black;
            this.fasAiteGroup.Location = new System.Drawing.Point(6, 166);
            this.fasAiteGroup.Name = "fasAiteGroup";
            this.fasAiteGroup.Size = new System.Drawing.Size(162, 100);
            this.fasAiteGroup.TabIndex = 0;
            this.fasAiteGroup.TabStop = false;
            this.fasAiteGroup.Text = "Aite &&  Fas";
            // 
            // fasCbox
            // 
            this.fasCbox.AutoSize = true;
            this.fasCbox.Location = new System.Drawing.Point(6, 65);
            this.fasCbox.Name = "fasCbox";
            this.fasCbox.Size = new System.Drawing.Size(15, 14);
            this.fasCbox.TabIndex = 3;
            this.fasCbox.UseVisualStyleBackColor = true;
            // 
            // aiteCbox
            // 
            this.aiteCbox.AutoSize = true;
            this.aiteCbox.Location = new System.Drawing.Point(6, 26);
            this.aiteCbox.Name = "aiteCbox";
            this.aiteCbox.Size = new System.Drawing.Size(15, 14);
            this.aiteCbox.TabIndex = 2;
            this.aiteCbox.UseVisualStyleBackColor = true;
            // 
            // fasCombox
            // 
            this.fasCombox.ForeColor = System.Drawing.Color.Black;
            this.fasCombox.FormattingEnabled = true;
            this.fasCombox.Items.AddRange(new object[] {
            "ard fas nadur",
            "mor fas nadur",
            "fas nadur",
            "beag fas nadur"});
            this.fasCombox.Location = new System.Drawing.Point(27, 61);
            this.fasCombox.Name = "fasCombox";
            this.fasCombox.Size = new System.Drawing.Size(121, 23);
            this.fasCombox.TabIndex = 1;
            this.fasCombox.Text = "ard fas nadur";
            // 
            // aiteCombox
            // 
            this.aiteCombox.ForeColor = System.Drawing.Color.Black;
            this.aiteCombox.FormattingEnabled = true;
            this.aiteCombox.Items.AddRange(new object[] {
            "ard naomh aite",
            "mor naomh aite",
            "naomh aite",
            "beag naomh aite"});
            this.aiteCombox.Location = new System.Drawing.Point(27, 22);
            this.aiteCombox.Name = "aiteCombox";
            this.aiteCombox.Size = new System.Drawing.Size(121, 23);
            this.aiteCombox.TabIndex = 0;
            this.aiteCombox.Text = "ard naomh aite";
            // 
            // nearbyAllyTab
            // 
            this.nearbyAllyTab.Controls.Add(this.nearbyAllyTable);
            this.nearbyAllyTab.Location = new System.Drawing.Point(4, 24);
            this.nearbyAllyTab.Name = "nearbyAllyTab";
            this.nearbyAllyTab.Size = new System.Drawing.Size(838, 492);
            this.nearbyAllyTab.TabIndex = 100;
            this.nearbyAllyTab.Text = "Nearby";
            this.nearbyAllyTab.UseVisualStyleBackColor = true;
            // 
            // nearbyAllyTable
            // 
            this.nearbyAllyTable.ColumnCount = 6;
            this.nearbyAllyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.nearbyAllyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.nearbyAllyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.nearbyAllyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.nearbyAllyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.nearbyAllyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.nearbyAllyTable.Location = new System.Drawing.Point(0, 0);
            this.nearbyAllyTable.Margin = new System.Windows.Forms.Padding(0);
            this.nearbyAllyTable.Name = "nearbyAllyTable";
            this.nearbyAllyTable.RowCount = 3;
            this.nearbyAllyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.nearbyAllyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.nearbyAllyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.nearbyAllyTable.Size = new System.Drawing.Size(838, 492);
            this.nearbyAllyTable.TabIndex = 0;
            // 
            // mainMonstersTab
            // 
            this.mainMonstersTab.Controls.Add(this.monsterTabControl);
            this.mainMonstersTab.ForeColor = System.Drawing.Color.Black;
            this.mainMonstersTab.Location = new System.Drawing.Point(4, 24);
            this.mainMonstersTab.Margin = new System.Windows.Forms.Padding(0);
            this.mainMonstersTab.Name = "mainMonstersTab";
            this.mainMonstersTab.Size = new System.Drawing.Size(842, 516);
            this.mainMonstersTab.TabIndex = 2;
            this.mainMonstersTab.Text = "Monsters";
            this.mainMonstersTab.UseVisualStyleBackColor = true;
            // 
            // monsterTabControl
            // 
            this.monsterTabControl.AllowDrop = true;
            this.monsterTabControl.Controls.Add(this.nearbyEnemyTab);
            this.monsterTabControl.Location = new System.Drawing.Point(0, 0);
            this.monsterTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.monsterTabControl.Name = "monsterTabControl";
            this.monsterTabControl.Padding = new System.Drawing.Point(0, 0);
            this.monsterTabControl.SelectedIndex = 0;
            this.monsterTabControl.Size = new System.Drawing.Size(846, 520);
            this.monsterTabControl.TabIndex = 0;
            // 
            // nearbyEnemyTab
            // 
            this.nearbyEnemyTab.BackColor = System.Drawing.Color.White;
            this.nearbyEnemyTab.Controls.Add(this.nearbyEnemyTable);
            this.nearbyEnemyTab.Location = new System.Drawing.Point(4, 24);
            this.nearbyEnemyTab.Margin = new System.Windows.Forms.Padding(0);
            this.nearbyEnemyTab.Name = "nearbyEnemyTab";
            this.nearbyEnemyTab.Size = new System.Drawing.Size(838, 492);
            this.nearbyEnemyTab.TabIndex = 1;
            this.nearbyEnemyTab.Text = "Nearby";
            // 
            // nearbyEnemyTable
            // 
            this.nearbyEnemyTable.ColumnCount = 3;
            this.nearbyEnemyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.nearbyEnemyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.nearbyEnemyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.nearbyEnemyTable.Location = new System.Drawing.Point(0, 0);
            this.nearbyEnemyTable.Name = "nearbyEnemyTable";
            this.nearbyEnemyTable.RowCount = 3;
            this.nearbyEnemyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.nearbyEnemyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.nearbyEnemyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.nearbyEnemyTable.Size = new System.Drawing.Size(838, 492);
            this.nearbyEnemyTable.TabIndex = 0;
            // 
            // bashingTab
            // 
            this.bashingTab.Controls.Add(this.groupBox7);
            this.bashingTab.Controls.Add(this.label5);
            this.bashingTab.Controls.Add(this.ProtectGbx);
            this.bashingTab.Controls.Add(this.groupBox5);
            this.bashingTab.Controls.Add(this.groupBox4);
            this.bashingTab.Controls.Add(this.groupBox3);
            this.bashingTab.Controls.Add(this.groupBox2);
            this.bashingTab.Controls.Add(this.bashingSkillsToUseGrp);
            this.bashingTab.Controls.Add(this.btnBashing);
            this.bashingTab.Location = new System.Drawing.Point(4, 24);
            this.bashingTab.Name = "bashingTab";
            this.bashingTab.Padding = new System.Windows.Forms.Padding(3);
            this.bashingTab.Size = new System.Drawing.Size(842, 516);
            this.bashingTab.TabIndex = 13;
            this.bashingTab.Text = "Bashing";
            this.bashingTab.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkRandomWaypoints);
            this.groupBox7.Location = new System.Drawing.Point(165, 321);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(349, 80);
            this.groupBox7.TabIndex = 186;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Random Waypoints";
            // 
            // chkRandomWaypoints
            // 
            this.chkRandomWaypoints.AutoSize = true;
            this.chkRandomWaypoints.Location = new System.Drawing.Point(18, 22);
            this.chkRandomWaypoints.Name = "chkRandomWaypoints";
            this.chkRandomWaypoints.Size = new System.Drawing.Size(101, 34);
            this.chkRandomWaypoints.TabIndex = 179;
            this.chkRandomWaypoints.Text = "Enable \r\n(Current Map)";
            this.chkRandomWaypoints.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(332, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(345, 15);
            this.label5.TabIndex = 166;
            this.label5.Text = "THE ORDER YOU SELECT SKILLS IS THE ORDER THEYRE USED";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProtectGbx
            // 
            this.ProtectGbx.Controls.Add(this.Protected2Tbx);
            this.ProtectGbx.Controls.Add(this.Protect2Cbx);
            this.ProtectGbx.Controls.Add(this.Protected1Tbx);
            this.ProtectGbx.Controls.Add(this.Protect1Cbx);
            this.ProtectGbx.Location = new System.Drawing.Point(165, 233);
            this.ProtectGbx.Name = "ProtectGbx";
            this.ProtectGbx.Size = new System.Drawing.Size(213, 82);
            this.ProtectGbx.TabIndex = 183;
            this.ProtectGbx.TabStop = false;
            this.ProtectGbx.Text = "Protect";
            // 
            // Protected2Tbx
            // 
            this.Protected2Tbx.Location = new System.Drawing.Point(82, 49);
            this.Protected2Tbx.Name = "Protected2Tbx";
            this.Protected2Tbx.Size = new System.Drawing.Size(118, 23);
            this.Protected2Tbx.TabIndex = 183;
            // 
            // Protect2Cbx
            // 
            this.Protect2Cbx.AutoSize = true;
            this.Protect2Cbx.Location = new System.Drawing.Point(6, 51);
            this.Protect2Cbx.Name = "Protect2Cbx";
            this.Protect2Cbx.Size = new System.Drawing.Size(70, 19);
            this.Protect2Cbx.TabIndex = 182;
            this.Protect2Cbx.Text = "Protect2";
            this.Protect2Cbx.UseVisualStyleBackColor = true;
            // 
            // Protected1Tbx
            // 
            this.Protected1Tbx.Location = new System.Drawing.Point(82, 20);
            this.Protected1Tbx.Name = "Protected1Tbx";
            this.Protected1Tbx.Size = new System.Drawing.Size(118, 23);
            this.Protected1Tbx.TabIndex = 181;
            // 
            // Protect1Cbx
            // 
            this.Protect1Cbx.AutoSize = true;
            this.Protect1Cbx.Location = new System.Drawing.Point(6, 22);
            this.Protect1Cbx.Name = "Protect1Cbx";
            this.Protect1Cbx.Size = new System.Drawing.Size(70, 19);
            this.Protect1Cbx.TabIndex = 179;
            this.Protect1Cbx.Text = "Protect1";
            this.Protect1Cbx.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkBashAssails);
            this.groupBox5.Controls.Add(this.btnRepairAuto);
            this.groupBox5.Controls.Add(this.chkAutoRepairBash);
            this.groupBox5.Controls.Add(this.chkCrasher);
            this.groupBox5.Controls.Add(this.chkUseSkillsFromRange);
            this.groupBox5.Controls.Add(this.ChargeToTargetCbx);
            this.groupBox5.Location = new System.Drawing.Point(6, 116);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(153, 199);
            this.groupBox5.TabIndex = 182;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Misc Options";
            // 
            // chkBashAssails
            // 
            this.chkBashAssails.AutoSize = true;
            this.chkBashAssails.Checked = true;
            this.chkBashAssails.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBashAssails.Location = new System.Drawing.Point(11, 97);
            this.chkBashAssails.Name = "chkBashAssails";
            this.chkBashAssails.Size = new System.Drawing.Size(83, 19);
            this.chkBashAssails.TabIndex = 187;
            this.chkBashAssails.Text = "Use Assails";
            this.chkBashAssails.UseVisualStyleBackColor = true;
            // 
            // btnRepairAuto
            // 
            this.btnRepairAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRepairAuto.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRepairAuto.Location = new System.Drawing.Point(6, 152);
            this.btnRepairAuto.Name = "btnRepairAuto";
            this.btnRepairAuto.Size = new System.Drawing.Size(138, 35);
            this.btnRepairAuto.TabIndex = 186;
            this.btnRepairAuto.Text = "Trigger Repair";
            this.btnRepairAuto.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRepairAuto.UseVisualStyleBackColor = true;
            // 
            // chkAutoRepairBash
            // 
            this.chkAutoRepairBash.AutoSize = true;
            this.chkAutoRepairBash.Location = new System.Drawing.Point(11, 129);
            this.chkAutoRepairBash.Name = "chkAutoRepairBash";
            this.chkAutoRepairBash.Size = new System.Drawing.Size(118, 19);
            this.chkAutoRepairBash.TabIndex = 185;
            this.chkAutoRepairBash.Text = "Automatic Repair";
            this.chkAutoRepairBash.UseVisualStyleBackColor = true;
            // 
            // chkCrasher
            // 
            this.chkCrasher.AutoSize = true;
            this.chkCrasher.Location = new System.Drawing.Point(11, 72);
            this.chkCrasher.Name = "chkCrasher";
            this.chkCrasher.Size = new System.Drawing.Size(71, 19);
            this.chkCrasher.TabIndex = 178;
            this.chkCrasher.Text = "Crashers\r\n";
            this.chkCrasher.UseVisualStyleBackColor = true;
            // 
            // chkUseSkillsFromRange
            // 
            this.chkUseSkillsFromRange.AutoSize = true;
            this.chkUseSkillsFromRange.Location = new System.Drawing.Point(11, 22);
            this.chkUseSkillsFromRange.Name = "chkUseSkillsFromRange";
            this.chkUseSkillsFromRange.Size = new System.Drawing.Size(119, 19);
            this.chkUseSkillsFromRange.TabIndex = 176;
            this.chkUseSkillsFromRange.Text = "Skills From Range";
            this.chkUseSkillsFromRange.UseVisualStyleBackColor = true;
            // 
            // ChargeToTargetCbx
            // 
            this.ChargeToTargetCbx.AutoSize = true;
            this.ChargeToTargetCbx.Location = new System.Drawing.Point(11, 47);
            this.ChargeToTargetCbx.Name = "ChargeToTargetCbx";
            this.ChargeToTargetCbx.Size = new System.Drawing.Size(113, 19);
            this.ChargeToTargetCbx.TabIndex = 177;
            this.ChargeToTargetCbx.Text = "Charge to Target";
            this.ChargeToTargetCbx.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.numAssitantStray);
            this.groupBox4.Controls.Add(this.radioAssitantStray);
            this.groupBox4.Controls.Add(this.assistBasherChk);
            this.groupBox4.Controls.Add(this.radioLeaderTarget);
            this.groupBox4.Controls.Add(this.leadBasherTxt);
            this.groupBox4.Location = new System.Drawing.Point(520, 233);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(188, 168);
            this.groupBox4.TabIndex = 181;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Multi Basher Support";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(144, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 15);
            this.label4.TabIndex = 174;
            this.label4.Text = "Tiles.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numAssitantStray
            // 
            this.numAssitantStray.ForeColor = System.Drawing.Color.Black;
            this.numAssitantStray.Location = new System.Drawing.Point(95, 131);
            this.numAssitantStray.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numAssitantStray.Name = "numAssitantStray";
            this.numAssitantStray.Size = new System.Drawing.Size(43, 23);
            this.numAssitantStray.TabIndex = 175;
            this.numAssitantStray.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numAssitantStray.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // radioAssitantStray
            // 
            this.radioAssitantStray.AutoSize = true;
            this.radioAssitantStray.Location = new System.Drawing.Point(10, 132);
            this.radioAssitantStray.Name = "radioAssitantStray";
            this.radioAssitantStray.Size = new System.Drawing.Size(85, 19);
            this.radioAssitantStray.TabIndex = 174;
            this.radioAssitantStray.Text = "Stay Within";
            this.radioAssitantStray.UseVisualStyleBackColor = true;
            // 
            // assistBasherChk
            // 
            this.assistBasherChk.AutoSize = true;
            this.assistBasherChk.ForeColor = System.Drawing.Color.Black;
            this.assistBasherChk.Location = new System.Drawing.Point(32, 22);
            this.assistBasherChk.Name = "assistBasherChk";
            this.assistBasherChk.Size = new System.Drawing.Size(125, 34);
            this.assistBasherChk.TabIndex = 170;
            this.assistBasherChk.Text = "Assist other basher\r\n(Must be an alt)";
            this.assistBasherChk.UseVisualStyleBackColor = true;
            // 
            // radioLeaderTarget
            // 
            this.radioLeaderTarget.AutoSize = true;
            this.radioLeaderTarget.Checked = true;
            this.radioLeaderTarget.Location = new System.Drawing.Point(10, 107);
            this.radioLeaderTarget.Name = "radioLeaderTarget";
            this.radioLeaderTarget.Size = new System.Drawing.Size(103, 19);
            this.radioLeaderTarget.TabIndex = 173;
            this.radioLeaderTarget.TabStop = true;
            this.radioLeaderTarget.Text = "Leader\'s Target";
            this.radioLeaderTarget.UseVisualStyleBackColor = true;
            // 
            // leadBasherTxt
            // 
            this.leadBasherTxt.BackColor = System.Drawing.Color.White;
            this.leadBasherTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leadBasherTxt.ForeColor = System.Drawing.Color.Black;
            this.leadBasherTxt.Location = new System.Drawing.Point(39, 69);
            this.leadBasherTxt.Margin = new System.Windows.Forms.Padding(0);
            this.leadBasherTxt.MaxLength = 12;
            this.leadBasherTxt.Name = "leadBasherTxt";
            this.leadBasherTxt.Size = new System.Drawing.Size(114, 23);
            this.leadBasherTxt.TabIndex = 169;
            this.leadBasherTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkFrostStrike);
            this.groupBox3.Controls.Add(this.numPFCounter);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(153, 104);
            this.groupBox3.TabIndex = 180;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Crowd Control";
            // 
            // chkFrostStrike
            // 
            this.chkFrostStrike.AutoSize = true;
            this.chkFrostStrike.ForeColor = System.Drawing.Color.Black;
            this.chkFrostStrike.Location = new System.Drawing.Point(11, 47);
            this.chkFrostStrike.Name = "chkFrostStrike";
            this.chkFrostStrike.Size = new System.Drawing.Size(84, 19);
            this.chkFrostStrike.TabIndex = 165;
            this.chkFrostStrike.Text = "Frost Strike";
            this.chkFrostStrike.UseVisualStyleBackColor = true;
            // 
            // numPFCounter
            // 
            this.numPFCounter.ForeColor = System.Drawing.Color.Black;
            this.numPFCounter.Location = new System.Drawing.Point(104, 22);
            this.numPFCounter.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPFCounter.Name = "numPFCounter";
            this.numPFCounter.Size = new System.Drawing.Size(43, 23);
            this.numPFCounter.TabIndex = 163;
            this.numPFCounter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPFCounter.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 15);
            this.label3.TabIndex = 164;
            this.label3.Text = "PF/PD Mob Size:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkWaitForCradh);
            this.groupBox2.Controls.Add(this.chkWaitForFas);
            this.groupBox2.Location = new System.Drawing.Point(384, 233);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(130, 82);
            this.groupBox2.TabIndex = 179;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Spells";
            // 
            // chkWaitForCradh
            // 
            this.chkWaitForCradh.AutoSize = true;
            this.chkWaitForCradh.ForeColor = System.Drawing.Color.Black;
            this.chkWaitForCradh.Location = new System.Drawing.Point(6, 51);
            this.chkWaitForCradh.Name = "chkWaitForCradh";
            this.chkWaitForCradh.Size = new System.Drawing.Size(103, 19);
            this.chkWaitForCradh.TabIndex = 162;
            this.chkWaitForCradh.Text = "Wait for Cradh";
            this.chkWaitForCradh.UseVisualStyleBackColor = true;
            // 
            // chkWaitForFas
            // 
            this.chkWaitForFas.AutoSize = true;
            this.chkWaitForFas.ForeColor = System.Drawing.Color.Black;
            this.chkWaitForFas.Location = new System.Drawing.Point(6, 22);
            this.chkWaitForFas.Name = "chkWaitForFas";
            this.chkWaitForFas.Size = new System.Drawing.Size(88, 19);
            this.chkWaitForFas.TabIndex = 1;
            this.chkWaitForFas.Text = "Wait for Fas";
            this.chkWaitForFas.UseVisualStyleBackColor = true;
            // 
            // bashingSkillsToUseGrp
            // 
            this.bashingSkillsToUseGrp.Location = new System.Drawing.Point(165, 34);
            this.bashingSkillsToUseGrp.Name = "bashingSkillsToUseGrp";
            this.bashingSkillsToUseGrp.Size = new System.Drawing.Size(671, 193);
            this.bashingSkillsToUseGrp.TabIndex = 178;
            this.bashingSkillsToUseGrp.TabStop = false;
            this.bashingSkillsToUseGrp.Text = "Skills";
            // 
            // btnBashing
            // 
            this.btnBashing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBashing.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBashing.Location = new System.Drawing.Point(619, 450);
            this.btnBashing.Name = "btnBashing";
            this.btnBashing.Size = new System.Drawing.Size(217, 60);
            this.btnBashing.TabIndex = 177;
            this.btnBashing.Text = "Start Bashing";
            this.btnBashing.UseVisualStyleBackColor = true;
            // 
            // toolsTab
            // 
            this.toolsTab.Controls.Add(this.groupBox9);
            this.toolsTab.Controls.Add(this.resetGroup);
            this.toolsTab.Controls.Add(this.customLinesGroup);
            this.toolsTab.Controls.Add(this.unifiedGuildChatCbox);
            this.toolsTab.Controls.Add(this.dmuGroup);
            this.toolsTab.Controls.Add(this.renamedStaffsGrp);
            this.toolsTab.Controls.Add(this.stuffGrp);
            this.toolsTab.Controls.Add(this.mapFlagsGroup);
            this.toolsTab.Controls.Add(this.clientHacksGroup);
            this.toolsTab.ForeColor = System.Drawing.Color.Black;
            this.toolsTab.Location = new System.Drawing.Point(4, 24);
            this.toolsTab.Name = "toolsTab";
            this.toolsTab.Size = new System.Drawing.Size(842, 516);
            this.toolsTab.TabIndex = 8;
            this.toolsTab.Text = "Tools";
            this.toolsTab.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.comboSelector);
            this.groupBox9.Controls.Add(this.numComboImgSelect);
            this.groupBox9.Controls.Add(this.picComboSkill);
            this.groupBox9.Controls.Add(this.btnAddSkillCombo);
            this.groupBox9.Location = new System.Drawing.Point(461, 112);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(166, 187);
            this.groupBox9.TabIndex = 134;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Add Combo to Skills";
            // 
            // comboSelector
            // 
            this.comboSelector.AutoCompleteCustomSource.AddRange(new string[] {
            "Combo 1",
            "Combo 2",
            "Combo 3",
            "Combo 4"});
            this.comboSelector.FormattingEnabled = true;
            this.comboSelector.Items.AddRange(new object[] {
            "Combo One",
            "Combo Two",
            "Combo Three",
            "Combo Four"});
            this.comboSelector.Location = new System.Drawing.Point(21, 116);
            this.comboSelector.Name = "comboSelector";
            this.comboSelector.Size = new System.Drawing.Size(112, 23);
            this.comboSelector.TabIndex = 135;
            this.comboSelector.Text = "Select Combo";
            // 
            // numComboImgSelect
            // 
            this.numComboImgSelect.ForeColor = System.Drawing.Color.Black;
            this.numComboImgSelect.Location = new System.Drawing.Point(20, 86);
            this.numComboImgSelect.Maximum = new decimal(new int[] {
            112,
            0,
            0,
            0});
            this.numComboImgSelect.Name = "numComboImgSelect";
            this.numComboImgSelect.Size = new System.Drawing.Size(62, 23);
            this.numComboImgSelect.TabIndex = 136;
            this.numComboImgSelect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numComboImgSelect.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // picComboSkill
            // 
            this.picComboSkill.Location = new System.Drawing.Point(20, 22);
            this.picComboSkill.Name = "picComboSkill";
            this.picComboSkill.Size = new System.Drawing.Size(62, 58);
            this.picComboSkill.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picComboSkill.TabIndex = 135;
            this.picComboSkill.TabStop = false;
            // 
            // btnAddSkillCombo
            // 
            this.btnAddSkillCombo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddSkillCombo.Location = new System.Drawing.Point(21, 145);
            this.btnAddSkillCombo.Name = "btnAddSkillCombo";
            this.btnAddSkillCombo.Size = new System.Drawing.Size(112, 31);
            this.btnAddSkillCombo.TabIndex = 133;
            this.btnAddSkillCombo.Text = "Add Skill";
            this.btnAddSkillCombo.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.resetGroup.Controls.Add(this.resetLbl);
            this.resetGroup.Controls.Add(this.btnResetAllStatus);
            this.resetGroup.Location = new System.Drawing.Point(233, 112);
            this.resetGroup.Name = "groupBox6";
            this.resetGroup.Size = new System.Drawing.Size(223, 187);
            this.resetGroup.TabIndex = 133;
            this.resetGroup.TabStop = false;
            this.resetGroup.Text = "Reset";
            // 
            // label7
            // 
            this.resetLbl.AutoSize = true;
            this.resetLbl.Location = new System.Drawing.Point(52, 101);
            this.resetLbl.Name = "label7";
            this.resetLbl.Size = new System.Drawing.Size(130, 30);
            this.resetLbl.TabIndex = 134;
            this.resetLbl.Text = "resets booleans related \r\nto casting and walking";
            this.resetLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnResetAllStatus
            // 
            this.btnResetAllStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetAllStatus.Location = new System.Drawing.Point(18, 22);
            this.btnResetAllStatus.Name = "btnResetAllStatus";
            this.btnResetAllStatus.Size = new System.Drawing.Size(188, 49);
            this.btnResetAllStatus.TabIndex = 133;
            this.btnResetAllStatus.Text = "Reset All Status";
            this.btnResetAllStatus.UseVisualStyleBackColor = true;
            this.btnResetAllStatus.Click += new System.EventHandler(this.btnResetAllStatus_Click);
            // 
            // customLinesGroup
            // 
            this.customLinesGroup.Controls.Add(this.customLinesBox);
            this.customLinesGroup.Location = new System.Drawing.Point(6, 112);
            this.customLinesGroup.Name = "customLinesGroup";
            this.customLinesGroup.Size = new System.Drawing.Size(222, 187);
            this.customLinesGroup.TabIndex = 132;
            this.customLinesGroup.TabStop = false;
            this.customLinesGroup.Text = "Custom Spell Lines";
            // 
            // customLinesBox
            // 
            this.customLinesBox.Location = new System.Drawing.Point(14, 22);
            this.customLinesBox.Multiline = true;
            this.customLinesBox.Name = "customLinesBox";
            this.customLinesBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.customLinesBox.Size = new System.Drawing.Size(191, 154);
            this.customLinesBox.TabIndex = 193;
            // 
            // unifiedGuildChatCbox
            // 
            this.unifiedGuildChatCbox.AutoSize = true;
            this.unifiedGuildChatCbox.ForeColor = System.Drawing.Color.Black;
            this.unifiedGuildChatCbox.Location = new System.Drawing.Point(633, 28);
            this.unifiedGuildChatCbox.Name = "unifiedGuildChatCbox";
            this.unifiedGuildChatCbox.Size = new System.Drawing.Size(123, 19);
            this.unifiedGuildChatCbox.TabIndex = 1;
            this.unifiedGuildChatCbox.Text = "Unified Guild Chat";
            this.unifiedGuildChatCbox.UseVisualStyleBackColor = true;
            this.unifiedGuildChatCbox.CheckedChanged += new System.EventHandler(this.unifiedGuildChatCbox_CheckedChanged);
            // 
            // dmuGroup
            // 
            this.dmuGroup.Controls.Add(this.viewDMUCbox);
            this.dmuGroup.Controls.Add(this.toggleShareCbox);
            this.dmuGroup.Controls.Add(this.saveDmuBtn);
            this.dmuGroup.Controls.Add(this.toggleGenderCbox);
            this.dmuGroup.Controls.Add(this.toggleDmuCbox);
            this.dmuGroup.Controls.Add(this.acc3ColorNum);
            this.dmuGroup.Controls.Add(this.acc3Num);
            this.dmuGroup.Controls.Add(this.acc2ColorNum);
            this.dmuGroup.Controls.Add(this.acc2Num);
            this.dmuGroup.Controls.Add(this.acc1ColorNum);
            this.dmuGroup.Controls.Add(this.acc1Num);
            this.dmuGroup.Controls.Add(this.hairColorLbl);
            this.dmuGroup.Controls.Add(this.shieldNum);
            this.dmuGroup.Controls.Add(this.hairColorNum);
            this.dmuGroup.Controls.Add(this.weaponNum);
            this.dmuGroup.Controls.Add(this.bootsNum);
            this.dmuGroup.Controls.Add(this.overcoatNum);
            this.dmuGroup.Controls.Add(this.bodyNum);
            this.dmuGroup.Controls.Add(this.headNum);
            this.dmuGroup.Controls.Add(this.headLbl);
            this.dmuGroup.Controls.Add(this.shieldLbl);
            this.dmuGroup.Controls.Add(this.weaponLbl);
            this.dmuGroup.Controls.Add(this.bootsLbl);
            this.dmuGroup.Controls.Add(this.armorLbl);
            this.dmuGroup.Controls.Add(this.acc3ColorLbl);
            this.dmuGroup.Controls.Add(this.acc2ColorLbl);
            this.dmuGroup.Controls.Add(this.acc1ColorLbl);
            this.dmuGroup.Controls.Add(this.acc3Lbl);
            this.dmuGroup.Controls.Add(this.acc2Lbl);
            this.dmuGroup.Controls.Add(this.acc1Lbl);
            this.dmuGroup.Controls.Add(this.bodyLbl);
            this.dmuGroup.Controls.Add(this.faceLbl);
            this.dmuGroup.Controls.Add(this.faceNum);
            this.dmuGroup.Location = new System.Drawing.Point(6, 299);
            this.dmuGroup.Name = "dmuGroup";
            this.dmuGroup.Size = new System.Drawing.Size(450, 213);
            this.dmuGroup.TabIndex = 131;
            this.dmuGroup.TabStop = false;
            this.dmuGroup.Text = "Dress Me Up";
            // 
            // viewDMUCbox
            // 
            this.viewDMUCbox.AutoSize = true;
            this.viewDMUCbox.Checked = true;
            this.viewDMUCbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewDMUCbox.ForeColor = System.Drawing.Color.Black;
            this.viewDMUCbox.Location = new System.Drawing.Point(110, 165);
            this.viewDMUCbox.Name = "viewDMUCbox";
            this.viewDMUCbox.Size = new System.Drawing.Size(112, 19);
            this.viewDMUCbox.TabIndex = 161;
            this.viewDMUCbox.Text = "See Other DMUs";
            this.viewDMUCbox.UseVisualStyleBackColor = true;
            // 
            // toggleShareCbox
            // 
            this.toggleShareCbox.AutoSize = true;
            this.toggleShareCbox.ForeColor = System.Drawing.Color.Black;
            this.toggleShareCbox.Location = new System.Drawing.Point(110, 188);
            this.toggleShareCbox.Name = "toggleShareCbox";
            this.toggleShareCbox.Size = new System.Drawing.Size(93, 19);
            this.toggleShareCbox.TabIndex = 160;
            this.toggleShareCbox.Text = "Show Others";
            this.toggleShareCbox.UseVisualStyleBackColor = true;
            // 
            // saveDmuBtn
            // 
            this.saveDmuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveDmuBtn.Location = new System.Drawing.Point(10, 146);
            this.saveDmuBtn.Name = "saveDmuBtn";
            this.saveDmuBtn.Size = new System.Drawing.Size(75, 32);
            this.saveDmuBtn.TabIndex = 159;
            this.saveDmuBtn.Text = "Save";
            this.saveDmuBtn.UseVisualStyleBackColor = true;
            // 
            // toggleGenderCbox
            // 
            this.toggleGenderCbox.AutoSize = true;
            this.toggleGenderCbox.ForeColor = System.Drawing.Color.Black;
            this.toggleGenderCbox.Location = new System.Drawing.Point(110, 142);
            this.toggleGenderCbox.Name = "toggleGenderCbox";
            this.toggleGenderCbox.Size = new System.Drawing.Size(95, 19);
            this.toggleGenderCbox.TabIndex = 158;
            this.toggleGenderCbox.Text = "Male/Female";
            this.toggleGenderCbox.UseVisualStyleBackColor = true;
            // 
            // toggleDmuCbox
            // 
            this.toggleDmuCbox.AutoSize = true;
            this.toggleDmuCbox.ForeColor = System.Drawing.Color.Black;
            this.toggleDmuCbox.Location = new System.Drawing.Point(17, 188);
            this.toggleDmuCbox.Name = "toggleDmuCbox";
            this.toggleDmuCbox.Size = new System.Drawing.Size(68, 19);
            this.toggleDmuCbox.TabIndex = 2;
            this.toggleDmuCbox.Text = "Enabled";
            this.toggleDmuCbox.UseVisualStyleBackColor = true;
            // 
            // acc3ColorNum
            // 
            this.acc3ColorNum.Location = new System.Drawing.Point(395, 167);
            this.acc3ColorNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.acc3ColorNum.Name = "acc3ColorNum";
            this.acc3ColorNum.Size = new System.Drawing.Size(49, 23);
            this.acc3ColorNum.TabIndex = 157;
            // 
            // acc3Num
            // 
            this.acc3Num.Location = new System.Drawing.Point(395, 138);
            this.acc3Num.Maximum = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.acc3Num.Name = "acc3Num";
            this.acc3Num.Size = new System.Drawing.Size(49, 23);
            this.acc3Num.TabIndex = 156;
            // 
            // acc2ColorNum
            // 
            this.acc2ColorNum.Location = new System.Drawing.Point(395, 109);
            this.acc2ColorNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.acc2ColorNum.Name = "acc2ColorNum";
            this.acc2ColorNum.Size = new System.Drawing.Size(49, 23);
            this.acc2ColorNum.TabIndex = 155;
            // 
            // acc2Num
            // 
            this.acc2Num.Location = new System.Drawing.Point(395, 80);
            this.acc2Num.Maximum = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.acc2Num.Name = "acc2Num";
            this.acc2Num.Size = new System.Drawing.Size(49, 23);
            this.acc2Num.TabIndex = 154;
            // 
            // acc1ColorNum
            // 
            this.acc1ColorNum.Location = new System.Drawing.Point(395, 51);
            this.acc1ColorNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.acc1ColorNum.Name = "acc1ColorNum";
            this.acc1ColorNum.Size = new System.Drawing.Size(49, 23);
            this.acc1ColorNum.TabIndex = 153;
            // 
            // acc1Num
            // 
            this.acc1Num.Location = new System.Drawing.Point(395, 22);
            this.acc1Num.Maximum = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.acc1Num.Name = "acc1Num";
            this.acc1Num.Size = new System.Drawing.Size(49, 23);
            this.acc1Num.TabIndex = 152;
            // 
            // hairColorLbl
            // 
            this.hairColorLbl.AutoSize = true;
            this.hairColorLbl.Location = new System.Drawing.Point(9, 82);
            this.hairColorLbl.Name = "hairColorLbl";
            this.hairColorLbl.Size = new System.Drawing.Size(36, 15);
            this.hairColorLbl.TabIndex = 144;
            this.hairColorLbl.Text = "Color";
            this.hairColorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // shieldNum
            // 
            this.shieldNum.Location = new System.Drawing.Point(173, 109);
            this.shieldNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.shieldNum.Name = "shieldNum";
            this.shieldNum.Size = new System.Drawing.Size(49, 23);
            this.shieldNum.TabIndex = 151;
            // 
            // hairColorNum
            // 
            this.hairColorNum.Location = new System.Drawing.Point(57, 80);
            this.hairColorNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.hairColorNum.Name = "hairColorNum";
            this.hairColorNum.Size = new System.Drawing.Size(49, 23);
            this.hairColorNum.TabIndex = 148;
            // 
            // weaponNum
            // 
            this.weaponNum.Location = new System.Drawing.Point(173, 80);
            this.weaponNum.Maximum = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.weaponNum.Name = "weaponNum";
            this.weaponNum.Size = new System.Drawing.Size(49, 23);
            this.weaponNum.TabIndex = 150;
            // 
            // bootsNum
            // 
            this.bootsNum.Location = new System.Drawing.Point(173, 51);
            this.bootsNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.bootsNum.Name = "bootsNum";
            this.bootsNum.Size = new System.Drawing.Size(49, 23);
            this.bootsNum.TabIndex = 149;
            // 
            // overcoatNum
            // 
            this.overcoatNum.Location = new System.Drawing.Point(173, 22);
            this.overcoatNum.Maximum = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.overcoatNum.Name = "overcoatNum";
            this.overcoatNum.Size = new System.Drawing.Size(49, 23);
            this.overcoatNum.TabIndex = 147;
            // 
            // bodyNum
            // 
            this.bodyNum.Location = new System.Drawing.Point(57, 109);
            this.bodyNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.bodyNum.Name = "bodyNum";
            this.bodyNum.Size = new System.Drawing.Size(49, 23);
            this.bodyNum.TabIndex = 146;
            // 
            // headNum
            // 
            this.headNum.Location = new System.Drawing.Point(57, 51);
            this.headNum.Maximum = new decimal(new int[] {
            1500,
            0,
            0,
            0});
            this.headNum.Name = "headNum";
            this.headNum.Size = new System.Drawing.Size(49, 23);
            this.headNum.TabIndex = 145;
            // 
            // headLbl
            // 
            this.headLbl.AutoSize = true;
            this.headLbl.Location = new System.Drawing.Point(10, 53);
            this.headLbl.Name = "headLbl";
            this.headLbl.Size = new System.Drawing.Size(35, 15);
            this.headLbl.TabIndex = 143;
            this.headLbl.Text = "Head";
            this.headLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // shieldLbl
            // 
            this.shieldLbl.AutoSize = true;
            this.shieldLbl.Location = new System.Drawing.Point(120, 111);
            this.shieldLbl.Name = "shieldLbl";
            this.shieldLbl.Size = new System.Drawing.Size(39, 15);
            this.shieldLbl.TabIndex = 142;
            this.shieldLbl.Text = "Shield";
            this.shieldLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // weaponLbl
            // 
            this.weaponLbl.AutoSize = true;
            this.weaponLbl.Location = new System.Drawing.Point(107, 82);
            this.weaponLbl.Name = "weaponLbl";
            this.weaponLbl.Size = new System.Drawing.Size(51, 15);
            this.weaponLbl.TabIndex = 141;
            this.weaponLbl.Text = "Weapon";
            this.weaponLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bootsLbl
            // 
            this.bootsLbl.AutoSize = true;
            this.bootsLbl.Location = new System.Drawing.Point(122, 53);
            this.bootsLbl.Name = "bootsLbl";
            this.bootsLbl.Size = new System.Drawing.Size(37, 15);
            this.bootsLbl.TabIndex = 140;
            this.bootsLbl.Text = "Boots";
            this.bootsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // armorLbl
            // 
            this.armorLbl.AutoSize = true;
            this.armorLbl.Location = new System.Drawing.Point(118, 24);
            this.armorLbl.Name = "armorLbl";
            this.armorLbl.Size = new System.Drawing.Size(41, 15);
            this.armorLbl.TabIndex = 139;
            this.armorLbl.Text = "Armor";
            this.armorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // acc3ColorLbl
            // 
            this.acc3ColorLbl.AutoSize = true;
            this.acc3ColorLbl.Location = new System.Drawing.Point(247, 169);
            this.acc3ColorLbl.Name = "acc3ColorLbl";
            this.acc3ColorLbl.Size = new System.Drawing.Size(94, 15);
            this.acc3ColorLbl.TabIndex = 138;
            this.acc3ColorLbl.Text = "Acc. Three Color";
            this.acc3ColorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // acc2ColorLbl
            // 
            this.acc2ColorLbl.AutoSize = true;
            this.acc2ColorLbl.Location = new System.Drawing.Point(255, 111);
            this.acc2ColorLbl.Name = "acc2ColorLbl";
            this.acc2ColorLbl.Size = new System.Drawing.Size(86, 15);
            this.acc2ColorLbl.TabIndex = 137;
            this.acc2ColorLbl.Text = "Acc. Two Color";
            this.acc2ColorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // acc1ColorLbl
            // 
            this.acc1ColorLbl.AutoSize = true;
            this.acc1ColorLbl.Location = new System.Drawing.Point(255, 53);
            this.acc1ColorLbl.Name = "acc1ColorLbl";
            this.acc1ColorLbl.Size = new System.Drawing.Size(87, 15);
            this.acc1ColorLbl.TabIndex = 136;
            this.acc1ColorLbl.Text = "Acc. One Color";
            this.acc1ColorLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // acc3Lbl
            // 
            this.acc3Lbl.AutoSize = true;
            this.acc3Lbl.Location = new System.Drawing.Point(279, 140);
            this.acc3Lbl.Name = "acc3Lbl";
            this.acc3Lbl.Size = new System.Drawing.Size(62, 15);
            this.acc3Lbl.TabIndex = 135;
            this.acc3Lbl.Text = "Acc. Three";
            this.acc3Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // acc2Lbl
            // 
            this.acc2Lbl.AutoSize = true;
            this.acc2Lbl.Location = new System.Drawing.Point(287, 82);
            this.acc2Lbl.Name = "acc2Lbl";
            this.acc2Lbl.Size = new System.Drawing.Size(54, 15);
            this.acc2Lbl.TabIndex = 134;
            this.acc2Lbl.Text = "Acc. Two";
            this.acc2Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // acc1Lbl
            // 
            this.acc1Lbl.AutoSize = true;
            this.acc1Lbl.Location = new System.Drawing.Point(287, 24);
            this.acc1Lbl.Name = "acc1Lbl";
            this.acc1Lbl.Size = new System.Drawing.Size(55, 15);
            this.acc1Lbl.TabIndex = 133;
            this.acc1Lbl.Text = "Acc. One";
            this.acc1Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bodyLbl
            // 
            this.bodyLbl.AutoSize = true;
            this.bodyLbl.Location = new System.Drawing.Point(11, 112);
            this.bodyLbl.Name = "bodyLbl";
            this.bodyLbl.Size = new System.Drawing.Size(34, 15);
            this.bodyLbl.TabIndex = 132;
            this.bodyLbl.Text = "Body";
            this.bodyLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // faceLbl
            // 
            this.faceLbl.AutoSize = true;
            this.faceLbl.Location = new System.Drawing.Point(14, 24);
            this.faceLbl.Name = "faceLbl";
            this.faceLbl.Size = new System.Drawing.Size(31, 15);
            this.faceLbl.TabIndex = 131;
            this.faceLbl.Text = "Face";
            this.faceLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // faceNum
            // 
            this.faceNum.Location = new System.Drawing.Point(57, 22);
            this.faceNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.faceNum.Name = "faceNum";
            this.faceNum.Size = new System.Drawing.Size(49, 23);
            this.faceNum.TabIndex = 0;
            // 
            // renamedStaffsGrp
            // 
            this.renamedStaffsGrp.Controls.Add(this.removeRenameBtn);
            this.renamedStaffsGrp.Controls.Add(this.renameLbl);
            this.renamedStaffsGrp.Controls.Add(this.renameList);
            this.renamedStaffsGrp.Controls.Add(this.addRenameBtn);
            this.renamedStaffsGrp.Controls.Add(this.renameText);
            this.renamedStaffsGrp.Controls.Add(this.renameCombox);
            this.renamedStaffsGrp.Location = new System.Drawing.Point(462, 300);
            this.renamedStaffsGrp.Name = "renamedStaffsGrp";
            this.renamedStaffsGrp.Size = new System.Drawing.Size(375, 213);
            this.renamedStaffsGrp.TabIndex = 129;
            this.renamedStaffsGrp.TabStop = false;
            this.renamedStaffsGrp.Text = "Renamed Staffs";
            // 
            // removeRenameBtn
            // 
            this.removeRenameBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeRenameBtn.Location = new System.Drawing.Point(301, 34);
            this.removeRenameBtn.Name = "removeRenameBtn";
            this.removeRenameBtn.Size = new System.Drawing.Size(68, 56);
            this.removeRenameBtn.TabIndex = 132;
            this.removeRenameBtn.Text = "Remove\r\nSelected";
            this.removeRenameBtn.UseVisualStyleBackColor = true;
            // 
            // renameLbl
            // 
            this.renameLbl.AutoSize = true;
            this.renameLbl.Location = new System.Drawing.Point(75, 133);
            this.renameLbl.Name = "renameLbl";
            this.renameLbl.Size = new System.Drawing.Size(220, 45);
            this.renameLbl.TabIndex = 130;
            this.renameLbl.Text = "include stars in name\r\nseperate from saved profiles\r\nwill load renamed staffs upo" +
    "n logging in";
            this.renameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // renameList
            // 
            this.renameList.FormattingEnabled = true;
            this.renameList.ItemHeight = 15;
            this.renameList.Location = new System.Drawing.Point(7, 22);
            this.renameList.Name = "renameList";
            this.renameList.Size = new System.Drawing.Size(288, 79);
            this.renameList.TabIndex = 130;
            // 
            // addRenameBtn
            // 
            this.addRenameBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addRenameBtn.Location = new System.Drawing.Point(301, 107);
            this.addRenameBtn.Name = "addRenameBtn";
            this.addRenameBtn.Size = new System.Drawing.Size(68, 23);
            this.addRenameBtn.TabIndex = 131;
            this.addRenameBtn.Text = "Add";
            this.addRenameBtn.UseVisualStyleBackColor = true;
            // 
            // renameText
            // 
            this.renameText.Location = new System.Drawing.Point(185, 107);
            this.renameText.Name = "renameText";
            this.renameText.Size = new System.Drawing.Size(110, 23);
            this.renameText.TabIndex = 130;
            // 
            // renameCombox
            // 
            this.renameCombox.FormattingEnabled = true;
            this.renameCombox.Location = new System.Drawing.Point(7, 107);
            this.renameCombox.Name = "renameCombox";
            this.renameCombox.Size = new System.Drawing.Size(172, 23);
            this.renameCombox.TabIndex = 128;
            this.renameCombox.Text = "Select Staff Type";
            // 
            // stuffGrp
            // 
            this.stuffGrp.Controls.Add(this.deformCbox);
            this.stuffGrp.Controls.Add(this.safeScreenCbox);
            this.stuffGrp.Controls.Add(this.formNum);
            this.stuffGrp.Controls.Add(this.formCbox);
            this.stuffGrp.Location = new System.Drawing.Point(462, 6);
            this.stuffGrp.Name = "stuffGrp";
            this.stuffGrp.Size = new System.Drawing.Size(165, 100);
            this.stuffGrp.TabIndex = 5;
            this.stuffGrp.TabStop = false;
            this.stuffGrp.Text = "Stuff";
            // 
            // deformCbox
            // 
            this.deformCbox.AutoSize = true;
            this.deformCbox.Location = new System.Drawing.Point(8, 47);
            this.deformCbox.Name = "deformCbox";
            this.deformCbox.Size = new System.Drawing.Size(151, 19);
            this.deformCbox.TabIndex = 10;
            this.deformCbox.Text = "Deform nearby stranger";
            this.deformCbox.UseVisualStyleBackColor = true;
            this.deformCbox.CheckedChanged += new System.EventHandler(this.deformCbox_CheckedChanged);
            // 
            // safeScreenCbox
            // 
            this.safeScreenCbox.AutoSize = true;
            this.safeScreenCbox.ForeColor = System.Drawing.Color.Black;
            this.safeScreenCbox.Location = new System.Drawing.Point(8, 72);
            this.safeScreenCbox.Name = "safeScreenCbox";
            this.safeScreenCbox.Size = new System.Drawing.Size(86, 19);
            this.safeScreenCbox.TabIndex = 9;
            this.safeScreenCbox.Text = "Safe Screen";
            this.safeScreenCbox.UseVisualStyleBackColor = true;
            this.safeScreenCbox.CheckedChanged += new System.EventHandler(this.safeScreenCbox_CheckedChanged);
            // 
            // formNum
            // 
            this.formNum.ForeColor = System.Drawing.Color.Black;
            this.formNum.Location = new System.Drawing.Point(108, 18);
            this.formNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.formNum.Name = "formNum";
            this.formNum.Size = new System.Drawing.Size(43, 23);
            this.formNum.TabIndex = 8;
            this.formNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.formNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.formNum.ValueChanged += new System.EventHandler(this.formNum_ValueChanged);
            // 
            // formCbox
            // 
            this.formCbox.AutoSize = true;
            this.formCbox.ForeColor = System.Drawing.Color.Black;
            this.formCbox.Location = new System.Drawing.Point(8, 20);
            this.formCbox.Name = "formCbox";
            this.formCbox.Size = new System.Drawing.Size(101, 19);
            this.formCbox.TabIndex = 7;
            this.formCbox.Text = "Monster Form";
            this.formCbox.UseVisualStyleBackColor = true;
            this.formCbox.CheckedChanged += new System.EventHandler(this.formCbox_CheckedChanged);
            // 
            // mapFlagsGroup
            // 
            this.mapFlagsGroup.Controls.Add(this.darknessCbox);
            this.mapFlagsGroup.Controls.Add(this.mapSnowCbox);
            this.mapFlagsGroup.Controls.Add(this.mapSnowTileCbox);
            this.mapFlagsGroup.Controls.Add(this.mapTabsCbox);
            this.mapFlagsGroup.Controls.Add(this.mapFlagsEnableCbox);
            this.mapFlagsGroup.Location = new System.Drawing.Point(234, 6);
            this.mapFlagsGroup.Name = "mapFlagsGroup";
            this.mapFlagsGroup.Size = new System.Drawing.Size(222, 100);
            this.mapFlagsGroup.TabIndex = 1;
            this.mapFlagsGroup.TabStop = false;
            this.mapFlagsGroup.Text = "Map Flags Override";
            // 
            // darknessCbox
            // 
            this.darknessCbox.AutoSize = true;
            this.darknessCbox.Enabled = false;
            this.darknessCbox.ForeColor = System.Drawing.Color.Gray;
            this.darknessCbox.Location = new System.Drawing.Point(6, 72);
            this.darknessCbox.Name = "darknessCbox";
            this.darknessCbox.Size = new System.Drawing.Size(73, 19);
            this.darknessCbox.TabIndex = 5;
            this.darknessCbox.Text = "Darkness";
            this.darknessCbox.UseVisualStyleBackColor = true;
            // 
            // mapSnowCbox
            // 
            this.mapSnowCbox.AutoSize = true;
            this.mapSnowCbox.Enabled = false;
            this.mapSnowCbox.ForeColor = System.Drawing.Color.Gray;
            this.mapSnowCbox.Location = new System.Drawing.Point(6, 47);
            this.mapSnowCbox.Name = "mapSnowCbox";
            this.mapSnowCbox.Size = new System.Drawing.Size(55, 19);
            this.mapSnowCbox.TabIndex = 1;
            this.mapSnowCbox.Text = "Snow";
            this.mapSnowCbox.UseVisualStyleBackColor = true;
            // 
            // mapSnowTileCbox
            // 
            this.mapSnowTileCbox.AutoSize = true;
            this.mapSnowTileCbox.Enabled = false;
            this.mapSnowTileCbox.ForeColor = System.Drawing.Color.Gray;
            this.mapSnowTileCbox.Location = new System.Drawing.Point(98, 72);
            this.mapSnowTileCbox.Name = "mapSnowTileCbox";
            this.mapSnowTileCbox.Size = new System.Drawing.Size(91, 19);
            this.mapSnowTileCbox.TabIndex = 4;
            this.mapSnowTileCbox.Text = "Snow Tileset";
            this.mapSnowTileCbox.UseVisualStyleBackColor = true;
            // 
            // mapTabsCbox
            // 
            this.mapTabsCbox.AutoSize = true;
            this.mapTabsCbox.Enabled = false;
            this.mapTabsCbox.ForeColor = System.Drawing.Color.Gray;
            this.mapTabsCbox.Location = new System.Drawing.Point(98, 47);
            this.mapTabsCbox.Name = "mapTabsCbox";
            this.mapTabsCbox.Size = new System.Drawing.Size(90, 19);
            this.mapTabsCbox.TabIndex = 3;
            this.mapTabsCbox.Text = "No Tab Map";
            this.mapTabsCbox.UseVisualStyleBackColor = true;
            // 
            // mapFlagsEnableCbox
            // 
            this.mapFlagsEnableCbox.AutoSize = true;
            this.mapFlagsEnableCbox.ForeColor = System.Drawing.Color.Black;
            this.mapFlagsEnableCbox.Location = new System.Drawing.Point(6, 22);
            this.mapFlagsEnableCbox.Name = "mapFlagsEnableCbox";
            this.mapFlagsEnableCbox.Size = new System.Drawing.Size(61, 19);
            this.mapFlagsEnableCbox.TabIndex = 0;
            this.mapFlagsEnableCbox.Text = "Enable";
            this.mapFlagsEnableCbox.UseVisualStyleBackColor = true;
            // 
            // clientHacksGroup
            // 
            this.clientHacksGroup.Controls.Add(this.ghostHackCbox);
            this.clientHacksGroup.Controls.Add(this.ignoreCollisionCbox);
            this.clientHacksGroup.Controls.Add(this.hideForegroundCbox);
            this.clientHacksGroup.Controls.Add(this.mapZoomCbox);
            this.clientHacksGroup.Controls.Add(this.seeHiddenCbox);
            this.clientHacksGroup.Controls.Add(this.noBlindCbox);
            this.clientHacksGroup.Location = new System.Drawing.Point(6, 6);
            this.clientHacksGroup.Name = "clientHacksGroup";
            this.clientHacksGroup.Size = new System.Drawing.Size(222, 100);
            this.clientHacksGroup.TabIndex = 0;
            this.clientHacksGroup.TabStop = false;
            this.clientHacksGroup.Text = "Client Hacks";
            // 
            // ghostHackCbox
            // 
            this.ghostHackCbox.AutoSize = true;
            this.ghostHackCbox.ForeColor = System.Drawing.Color.Black;
            this.ghostHackCbox.Location = new System.Drawing.Point(98, 47);
            this.ghostHackCbox.Name = "ghostHackCbox";
            this.ghostHackCbox.Size = new System.Drawing.Size(83, 19);
            this.ghostHackCbox.TabIndex = 4;
            this.ghostHackCbox.Text = "See Ghosts";
            this.ghostHackCbox.UseVisualStyleBackColor = true;
            this.ghostHackCbox.CheckedChanged += new System.EventHandler(this.ghostHackCbox_CheckedChanged);
            // 
            // ignoreCollisionCbox
            // 
            this.ignoreCollisionCbox.AutoSize = true;
            this.ignoreCollisionCbox.ForeColor = System.Drawing.Color.Black;
            this.ignoreCollisionCbox.Location = new System.Drawing.Point(6, 72);
            this.ignoreCollisionCbox.Name = "ignoreCollisionCbox";
            this.ignoreCollisionCbox.Size = new System.Drawing.Size(79, 19);
            this.ignoreCollisionCbox.TabIndex = 2;
            this.ignoreCollisionCbox.Text = "GM Mode";
            this.ignoreCollisionCbox.UseVisualStyleBackColor = true;
            this.ignoreCollisionCbox.CheckedChanged += new System.EventHandler(this.ignoreCollisionCbox_CheckedChanged);
            // 
            // hideForegroundCbox
            // 
            this.hideForegroundCbox.AutoSize = true;
            this.hideForegroundCbox.Enabled = false;
            this.hideForegroundCbox.ForeColor = System.Drawing.Color.Gray;
            this.hideForegroundCbox.Location = new System.Drawing.Point(98, 72);
            this.hideForegroundCbox.Name = "hideForegroundCbox";
            this.hideForegroundCbox.Size = new System.Drawing.Size(112, 19);
            this.hideForegroundCbox.TabIndex = 5;
            this.hideForegroundCbox.Text = "No Foregrounds";
            this.hideForegroundCbox.UseVisualStyleBackColor = true;
            this.hideForegroundCbox.CheckedChanged += new System.EventHandler(this.hideForegroundCbox_CheckedChanged);
            // 
            // mapZoomCbox
            // 
            this.mapZoomCbox.AutoSize = true;
            this.mapZoomCbox.ForeColor = System.Drawing.Color.Black;
            this.mapZoomCbox.Location = new System.Drawing.Point(98, 22);
            this.mapZoomCbox.Name = "mapZoomCbox";
            this.mapZoomCbox.Size = new System.Drawing.Size(107, 19);
            this.mapZoomCbox.TabIndex = 3;
            this.mapZoomCbox.Text = "Zoomable Map";
            this.mapZoomCbox.UseVisualStyleBackColor = true;
            this.mapZoomCbox.CheckedChanged += new System.EventHandler(this.mapZoomCbox_CheckedChanged);
            // 
            // seeHiddenCbox
            // 
            this.seeHiddenCbox.AutoSize = true;
            this.seeHiddenCbox.Checked = true;
            this.seeHiddenCbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.seeHiddenCbox.ForeColor = System.Drawing.Color.Black;
            this.seeHiddenCbox.Location = new System.Drawing.Point(6, 47);
            this.seeHiddenCbox.Name = "seeHiddenCbox";
            this.seeHiddenCbox.Size = new System.Drawing.Size(86, 19);
            this.seeHiddenCbox.TabIndex = 1;
            this.seeHiddenCbox.Text = "See Hidden";
            this.seeHiddenCbox.UseVisualStyleBackColor = true;
            this.seeHiddenCbox.CheckedChanged += new System.EventHandler(this.seeHiddenCbox_CheckedChanged);
            // 
            // noBlindCbox
            // 
            this.noBlindCbox.AutoSize = true;
            this.noBlindCbox.Checked = true;
            this.noBlindCbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.noBlindCbox.ForeColor = System.Drawing.Color.Black;
            this.noBlindCbox.Location = new System.Drawing.Point(6, 22);
            this.noBlindCbox.Name = "noBlindCbox";
            this.noBlindCbox.Size = new System.Drawing.Size(72, 19);
            this.noBlindCbox.TabIndex = 0;
            this.noBlindCbox.Text = "No Blind";
            this.noBlindCbox.UseVisualStyleBackColor = true;
            this.noBlindCbox.CheckedChanged += new System.EventHandler(this.noBlindCbox_CheckedChanged);
            // 
            // mainTasksTab
            // 
            this.mainTasksTab.BackColor = System.Drawing.Color.White;
            this.mainTasksTab.Controls.Add(this.groupBox10);
            this.mainTasksTab.Controls.Add(this.groupBox1);
            this.mainTasksTab.Controls.Add(this.TailorGroup);
            this.mainTasksTab.Controls.Add(this.ascendGroup);
            this.mainTasksTab.Controls.Add(this.prayerGroup);
            this.mainTasksTab.Controls.Add(this.itemFindingGB);
            this.mainTasksTab.Controls.Add(this.votingGroup);
            this.mainTasksTab.Controls.Add(this.fishingGroup);
            this.mainTasksTab.Controls.Add(this.farmGroup);
            this.mainTasksTab.Controls.Add(this.hubaeGroup);
            this.mainTasksTab.Controls.Add(this.laborGroup);
            this.mainTasksTab.Controls.Add(this.polishGroup);
            this.mainTasksTab.Controls.Add(this.ascensionGroup);
            this.mainTasksTab.Location = new System.Drawing.Point(4, 24);
            this.mainTasksTab.Name = "mainTasksTab";
            this.mainTasksTab.Size = new System.Drawing.Size(842, 516);
            this.mainTasksTab.TabIndex = 12;
            this.mainTasksTab.Text = "Tasks";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.btnCheckLoginTime);
            this.groupBox10.Controls.Add(this.btnConLogin);
            this.groupBox10.Location = new System.Drawing.Point(212, 95);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(205, 100);
            this.groupBox10.TabIndex = 170;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Consecutive Login";
            // 
            // btnCheckLoginTime
            // 
            this.btnCheckLoginTime.BackColor = System.Drawing.Color.White;
            this.btnCheckLoginTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckLoginTime.Location = new System.Drawing.Point(9, 54);
            this.btnCheckLoginTime.Name = "btnCheckLoginTime";
            this.btnCheckLoginTime.Size = new System.Drawing.Size(98, 36);
            this.btnCheckLoginTime.TabIndex = 9;
            this.btnCheckLoginTime.Text = "Check Time";
            this.btnCheckLoginTime.UseVisualStyleBackColor = false;
            // 
            // btnConLogin
            // 
            this.btnConLogin.BackColor = System.Drawing.Color.White;
            this.btnConLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConLogin.Location = new System.Drawing.Point(113, 54);
            this.btnConLogin.Name = "btnConLogin";
            this.btnConLogin.Size = new System.Drawing.Size(86, 36);
            this.btnConLogin.TabIndex = 8;
            this.btnConLogin.Text = "Start";
            this.btnConLogin.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnFood);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(423, 399);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 100);
            this.groupBox1.TabIndex = 169;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Buy and Deposit Food";
            // 
            // btnFood
            // 
            this.btnFood.BackColor = System.Drawing.Color.White;
            this.btnFood.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFood.Location = new System.Drawing.Point(57, 28);
            this.btnFood.Name = "btnFood";
            this.btnFood.Size = new System.Drawing.Size(92, 36);
            this.btnFood.TabIndex = 8;
            this.btnFood.Text = "Start Buying";
            this.btnFood.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(193, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "*Leave five inventory slots available";
            // 
            // TailorGroup
            // 
            this.TailorGroup.Controls.Add(this.TailorBtn);
            this.TailorGroup.Controls.Add(this.label2);
            this.TailorGroup.Location = new System.Drawing.Point(634, 293);
            this.TailorGroup.Name = "TailorGroup";
            this.TailorGroup.Size = new System.Drawing.Size(205, 100);
            this.TailorGroup.TabIndex = 10;
            this.TailorGroup.TabStop = false;
            this.TailorGroup.Text = "Tailoring";
            // 
            // TailorBtn
            // 
            this.TailorBtn.BackColor = System.Drawing.Color.White;
            this.TailorBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TailorBtn.Location = new System.Drawing.Point(62, 31);
            this.TailorBtn.Name = "TailorBtn";
            this.TailorBtn.Size = new System.Drawing.Size(75, 36);
            this.TailorBtn.TabIndex = 8;
            this.TailorBtn.Text = "Tailor";
            this.TailorBtn.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(190, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "*Leave first slot in inventory empty";
            // 
            // ascendGroup
            // 
            this.ascendGroup.Controls.Add(this.ascendAllBtn);
            this.ascendGroup.Controls.Add(this.deathOptionCbx);
            this.ascendGroup.Controls.Add(this.ascendOptionCbx);
            this.ascendGroup.Controls.Add(this.ascendBtn);
            this.ascendGroup.Location = new System.Drawing.Point(3, 95);
            this.ascendGroup.Name = "ascendGroup";
            this.ascendGroup.Size = new System.Drawing.Size(205, 86);
            this.ascendGroup.TabIndex = 11;
            this.ascendGroup.TabStop = false;
            this.ascendGroup.Text = "Ascension";
            // 
            // ascendAllBtn
            // 
            this.ascendAllBtn.BackColor = System.Drawing.Color.White;
            this.ascendAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ascendAllBtn.Location = new System.Drawing.Point(87, 22);
            this.ascendAllBtn.Name = "ascendAllBtn";
            this.ascendAllBtn.Size = new System.Drawing.Size(112, 25);
            this.ascendAllBtn.TabIndex = 167;
            this.ascendAllBtn.Text = "SET FOR ALL";
            this.ascendAllBtn.UseVisualStyleBackColor = false;
            // 
            // deathOptionCbx
            // 
            this.deathOptionCbx.AutoSize = true;
            this.deathOptionCbx.Location = new System.Drawing.Point(132, 55);
            this.deathOptionCbx.Name = "deathOptionCbx";
            this.deathOptionCbx.Size = new System.Drawing.Size(69, 19);
            this.deathOptionCbx.TabIndex = 11;
            this.deathOptionCbx.Text = "Die@PC";
            this.deathOptionCbx.UseVisualStyleBackColor = true;
            // 
            // ascendOptionCbx
            // 
            this.ascendOptionCbx.ForeColor = System.Drawing.Color.Black;
            this.ascendOptionCbx.FormattingEnabled = true;
            this.ascendOptionCbx.Items.AddRange(new object[] {
            "HP",
            "MP"});
            this.ascendOptionCbx.Location = new System.Drawing.Point(87, 53);
            this.ascendOptionCbx.Name = "ascendOptionCbx";
            this.ascendOptionCbx.Size = new System.Drawing.Size(42, 23);
            this.ascendOptionCbx.TabIndex = 157;
            this.ascendOptionCbx.Text = "HP";
            // 
            // ascendBtn
            // 
            this.ascendBtn.BackColor = System.Drawing.Color.White;
            this.ascendBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ascendBtn.Location = new System.Drawing.Point(6, 31);
            this.ascendBtn.Name = "ascendBtn";
            this.ascendBtn.Size = new System.Drawing.Size(75, 36);
            this.ascendBtn.TabIndex = 9;
            this.ascendBtn.Text = "Ascend";
            this.ascendBtn.UseVisualStyleBackColor = false;
            // 
            // prayerGroup
            // 
            this.prayerGroup.Controls.Add(this.prayerAcceptCbox);
            this.prayerGroup.Controls.Add(this.prayerAssistCbox);
            this.prayerGroup.Controls.Add(this.prayerAssistTbox);
            this.prayerGroup.Controls.Add(this.togglePrayingBtn);
            this.prayerGroup.Location = new System.Drawing.Point(633, 194);
            this.prayerGroup.Name = "prayerGroup";
            this.prayerGroup.Size = new System.Drawing.Size(205, 86);
            this.prayerGroup.TabIndex = 167;
            this.prayerGroup.TabStop = false;
            this.prayerGroup.Text = "Praying";
            // 
            // prayerAcceptCbox
            // 
            this.prayerAcceptCbox.AutoSize = true;
            this.prayerAcceptCbox.Location = new System.Drawing.Point(140, 24);
            this.prayerAcceptCbox.Name = "prayerAcceptCbox";
            this.prayerAcceptCbox.Size = new System.Drawing.Size(63, 19);
            this.prayerAcceptCbox.TabIndex = 167;
            this.prayerAcceptCbox.Text = "Accept";
            this.prayerAcceptCbox.UseVisualStyleBackColor = true;
            // 
            // prayerAssistCbox
            // 
            this.prayerAssistCbox.AutoSize = true;
            this.prayerAssistCbox.Location = new System.Drawing.Point(85, 24);
            this.prayerAssistCbox.Name = "prayerAssistCbox";
            this.prayerAssistCbox.Size = new System.Drawing.Size(56, 19);
            this.prayerAssistCbox.TabIndex = 166;
            this.prayerAssistCbox.Text = "Assist";
            this.prayerAssistCbox.UseVisualStyleBackColor = true;
            // 
            // prayerAssistTbox
            // 
            this.prayerAssistTbox.AcceptsReturn = true;
            this.prayerAssistTbox.BackColor = System.Drawing.Color.White;
            this.prayerAssistTbox.ForeColor = System.Drawing.Color.Black;
            this.prayerAssistTbox.Location = new System.Drawing.Point(87, 50);
            this.prayerAssistTbox.Name = "prayerAssistTbox";
            this.prayerAssistTbox.Size = new System.Drawing.Size(112, 23);
            this.prayerAssistTbox.TabIndex = 15;
            // 
            // togglePrayingBtn
            // 
            this.togglePrayingBtn.BackColor = System.Drawing.Color.White;
            this.togglePrayingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.togglePrayingBtn.Location = new System.Drawing.Point(6, 31);
            this.togglePrayingBtn.Name = "togglePrayingBtn";
            this.togglePrayingBtn.Size = new System.Drawing.Size(75, 36);
            this.togglePrayingBtn.TabIndex = 165;
            this.togglePrayingBtn.Text = "Pray";
            this.togglePrayingBtn.UseVisualStyleBackColor = false;
            // 
            // itemFindingGB
            // 
            this.itemFindingGB.Controls.Add(this.btnItemRemove);
            this.itemFindingGB.Controls.Add(this.btnItemAdd);
            this.itemFindingGB.Controls.Add(this.listOfItems);
            this.itemFindingGB.Controls.Add(this.btnLoadItemList);
            this.itemFindingGB.Controls.Add(this.listToFind);
            this.itemFindingGB.Controls.Add(this.btnItemFinder);
            this.itemFindingGB.Location = new System.Drawing.Point(3, 293);
            this.itemFindingGB.Name = "itemFindingGB";
            this.itemFindingGB.Size = new System.Drawing.Size(414, 220);
            this.itemFindingGB.TabIndex = 16;
            this.itemFindingGB.TabStop = false;
            this.itemFindingGB.Text = "Item Retreival";
            // 
            // btnItemRemove
            // 
            this.btnItemRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnItemRemove.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnItemRemove.Location = new System.Drawing.Point(218, 174);
            this.btnItemRemove.Name = "btnItemRemove";
            this.btnItemRemove.Size = new System.Drawing.Size(42, 40);
            this.btnItemRemove.TabIndex = 162;
            this.btnItemRemove.Text = "<";
            this.btnItemRemove.UseVisualStyleBackColor = true;
            // 
            // btnItemAdd
            // 
            this.btnItemAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnItemAdd.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnItemAdd.Location = new System.Drawing.Point(218, 30);
            this.btnItemAdd.Name = "btnItemAdd";
            this.btnItemAdd.Size = new System.Drawing.Size(42, 40);
            this.btnItemAdd.TabIndex = 161;
            this.btnItemAdd.Text = ">";
            this.btnItemAdd.UseVisualStyleBackColor = true;
            // 
            // listOfItems
            // 
            this.listOfItems.FormattingEnabled = true;
            this.listOfItems.HorizontalScrollbar = true;
            this.listOfItems.ItemHeight = 15;
            this.listOfItems.Items.AddRange(new object[] {
            "ao Dochas Bloom",
            "Blossom of Betrayal",
            "Bocan Bough",
            "Cadal Bells",
            "Dochas Bloom",
            "Ill-Star Flower",
            "Spark Flower",
            "raineach",
            "RLily Pads",
            "Squirtweed",
            "Lochran Tendrils",
            "Kobold Tail",
            "Eidheann",
            "Kabine Blossom",
            "Cactus Flower",
            "Folly Flower",
            "Amethyst Ring",
            "Bat\'s Wing",
            "Battle Sword",
            "Bee\'s Sting",
            "Beryl Earrings",
            "Blouse",
            "Borim",
            "Center Secret",
            "Centipede\'s Gland",
            "Charm: Blue Moon",
            "Charm: Clover",
            "Charm: Heart",
            "Charm: Horseshoe",
            "Charm: Pot of Gold",
            "Charm: Rainbow",
            "Charm: Red Balloon",
            "Charm: Star",
            "Cherry",
            "Coral Earrings",
            "Crab\'s Claw",
            "Faerie\'s Wing",
            "Fine Light Dagger",
            "Fine Two-handed Claidhmore",
            "Fox Ribs",
            "Frog\'s Leg",
            "Gargoyle\'s Skull",
            "Gargoyle Fiend\'s Skull",
            "Ghast\'s Skull",
            "Goblin\'s Skull",
            "Gog\'s Maw",
            "Goo",
            "Good Two-handed Claidhmore",
            "Grapes",
            "Great Bat\'s Wing",
            "Gremlin\'s Ear",
            "Gruesomefly Antennae",
            "Gruesomefly Wing",
            "Hobgoblin\'s Skull",
            "Jade Ring",
            "Kobold\'s Skull",
            "Leaf Key",
            "Leather Greaves",
            "Leech\'s Tail",
            "Love Key",
            "Mantis\'s Eye",
            "Magus Diana",
            "Magus Kronos",
            "Magus Zeus",
            "Magus Gaea",
            "Magus Apollo",
            "Mold",
            "Mushroom",
            "Pantica",
            "Polyp Sac",
            "Raw Wax",
            "Rock Cobbler\'s Scale",
            "Rotten Apple",
            "Rotten Grapes",
            "Rotten Tomato",
            "Scorpion\'s Sting",
            "Shirt",
            "Slime",
            "Spider\'s Eye",
            "Spider\'s Silk",
            "Spore Sac",
            "Stilla",
            "Succubus\'s Hair",
            "Talgonite Axe",
            "Two-handed Claidhmore",
            "Viper\'s Gland",
            "White Bat\'s Wing",
            "Wine",
            "Wolf\'s Fur",
            "Wolf\'s Lock",
            "Wolf\'s Skin",
            "Wolf\'s Teeth"});
            this.listOfItems.Location = new System.Drawing.Point(87, 30);
            this.listOfItems.Name = "listOfItems";
            this.listOfItems.Size = new System.Drawing.Size(125, 169);
            this.listOfItems.TabIndex = 160;
            // 
            // btnLoadItemList
            // 
            this.btnLoadItemList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadItemList.Location = new System.Drawing.Point(6, 73);
            this.btnLoadItemList.Name = "btnLoadItemList";
            this.btnLoadItemList.Size = new System.Drawing.Size(75, 40);
            this.btnLoadItemList.TabIndex = 159;
            this.btnLoadItemList.Text = "Load Item List";
            this.btnLoadItemList.UseVisualStyleBackColor = true;
            // 
            // listToFind
            // 
            this.listToFind.FormattingEnabled = true;
            this.listToFind.HorizontalScrollbar = true;
            this.listToFind.ItemHeight = 15;
            this.listToFind.Location = new System.Drawing.Point(266, 30);
            this.listToFind.Name = "listToFind";
            this.listToFind.Size = new System.Drawing.Size(142, 169);
            this.listToFind.TabIndex = 133;
            // 
            // btnItemFinder
            // 
            this.btnItemFinder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnItemFinder.Location = new System.Drawing.Point(6, 31);
            this.btnItemFinder.Name = "btnItemFinder";
            this.btnItemFinder.Size = new System.Drawing.Size(75, 36);
            this.btnItemFinder.TabIndex = 158;
            this.btnItemFinder.Text = "Find";
            this.btnItemFinder.UseVisualStyleBackColor = true;
            // 
            // votingGroup
            // 
            this.votingGroup.Controls.Add(this.voteForAllBtn);
            this.votingGroup.Controls.Add(this.voteText);
            this.votingGroup.Controls.Add(this.toggleVotingBtn);
            this.votingGroup.Location = new System.Drawing.Point(422, 194);
            this.votingGroup.Name = "votingGroup";
            this.votingGroup.Size = new System.Drawing.Size(205, 86);
            this.votingGroup.TabIndex = 14;
            this.votingGroup.TabStop = false;
            this.votingGroup.Text = "Voting";
            // 
            // voteForAllBtn
            // 
            this.voteForAllBtn.BackColor = System.Drawing.Color.White;
            this.voteForAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.voteForAllBtn.Location = new System.Drawing.Point(87, 22);
            this.voteForAllBtn.Name = "voteForAllBtn";
            this.voteForAllBtn.Size = new System.Drawing.Size(112, 25);
            this.voteForAllBtn.TabIndex = 166;
            this.voteForAllBtn.Text = "SET FOR ALL";
            this.voteForAllBtn.UseVisualStyleBackColor = false;
            // 
            // voteText
            // 
            this.voteText.AcceptsReturn = true;
            this.voteText.BackColor = System.Drawing.Color.White;
            this.voteText.ForeColor = System.Drawing.Color.Black;
            this.voteText.Location = new System.Drawing.Point(87, 50);
            this.voteText.Name = "voteText";
            this.voteText.Size = new System.Drawing.Size(112, 23);
            this.voteText.TabIndex = 15;
            // 
            // toggleVotingBtn
            // 
            this.toggleVotingBtn.BackColor = System.Drawing.Color.White;
            this.toggleVotingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleVotingBtn.Location = new System.Drawing.Point(6, 31);
            this.toggleVotingBtn.Name = "toggleVotingBtn";
            this.toggleVotingBtn.Size = new System.Drawing.Size(75, 36);
            this.toggleVotingBtn.TabIndex = 165;
            this.toggleVotingBtn.Text = "Vote";
            this.toggleVotingBtn.UseVisualStyleBackColor = false;
            // 
            // fishingGroup
            // 
            this.fishingGroup.Controls.Add(this.fishingCombox);
            this.fishingGroup.Controls.Add(this.keepFishRdio);
            this.fishingGroup.Controls.Add(this.sellFishRdio);
            this.fishingGroup.Controls.Add(this.toggleFishingBtn);
            this.fishingGroup.Location = new System.Drawing.Point(212, 194);
            this.fishingGroup.Name = "fishingGroup";
            this.fishingGroup.Size = new System.Drawing.Size(205, 86);
            this.fishingGroup.TabIndex = 13;
            this.fishingGroup.TabStop = false;
            this.fishingGroup.Text = "Fishing";
            // 
            // fishingCombox
            // 
            this.fishingCombox.ForeColor = System.Drawing.Color.Black;
            this.fishingCombox.FormattingEnabled = true;
            this.fishingCombox.Items.AddRange(new object[] {
            "Abel Port",
            "Kas Mines",
            "Loures Harbor"});
            this.fishingCombox.Location = new System.Drawing.Point(87, 50);
            this.fishingCombox.Name = "fishingCombox";
            this.fishingCombox.Size = new System.Drawing.Size(112, 23);
            this.fishingCombox.TabIndex = 151;
            this.fishingCombox.Text = "Abel Port";
            // 
            // keepFishRdio
            // 
            this.keepFishRdio.AutoSize = true;
            this.keepFishRdio.Location = new System.Drawing.Point(148, 25);
            this.keepFishRdio.Name = "keepFishRdio";
            this.keepFishRdio.Size = new System.Drawing.Size(51, 19);
            this.keepFishRdio.TabIndex = 11;
            this.keepFishRdio.TabStop = true;
            this.keepFishRdio.Text = "Keep";
            this.keepFishRdio.UseVisualStyleBackColor = true;
            // 
            // sellFishRdio
            // 
            this.sellFishRdio.AutoSize = true;
            this.sellFishRdio.Checked = true;
            this.sellFishRdio.Location = new System.Drawing.Point(87, 25);
            this.sellFishRdio.Name = "sellFishRdio";
            this.sellFishRdio.Size = new System.Drawing.Size(43, 19);
            this.sellFishRdio.TabIndex = 10;
            this.sellFishRdio.TabStop = true;
            this.sellFishRdio.Text = "Sell";
            this.sellFishRdio.UseVisualStyleBackColor = true;
            // 
            // toggleFishingBtn
            // 
            this.toggleFishingBtn.BackColor = System.Drawing.Color.White;
            this.toggleFishingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleFishingBtn.Location = new System.Drawing.Point(6, 31);
            this.toggleFishingBtn.Name = "toggleFishingBtn";
            this.toggleFishingBtn.Size = new System.Drawing.Size(75, 36);
            this.toggleFishingBtn.TabIndex = 9;
            this.toggleFishingBtn.Text = "Go Fish";
            this.toggleFishingBtn.UseVisualStyleBackColor = false;
            // 
            // farmGroup
            // 
            this.farmGroup.Controls.Add(this.setNearestBankBtn);
            this.farmGroup.Controls.Add(this.returnFarmBtn);
            this.farmGroup.Controls.Add(this.setCustomBankBtn);
            this.farmGroup.Controls.Add(this.toggleFarmBtn);
            this.farmGroup.Location = new System.Drawing.Point(425, 95);
            this.farmGroup.Name = "farmGroup";
            this.farmGroup.Size = new System.Drawing.Size(414, 86);
            this.farmGroup.TabIndex = 12;
            this.farmGroup.TabStop = false;
            this.farmGroup.Text = "Item Farming";
            // 
            // setNearestBankBtn
            // 
            this.setNearestBankBtn.BackColor = System.Drawing.Color.White;
            this.setNearestBankBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setNearestBankBtn.Location = new System.Drawing.Point(141, 22);
            this.setNearestBankBtn.Name = "setNearestBankBtn";
            this.setNearestBankBtn.Size = new System.Drawing.Size(115, 25);
            this.setNearestBankBtn.TabIndex = 164;
            this.setNearestBankBtn.Text = "Set Nearest Bank";
            this.setNearestBankBtn.UseVisualStyleBackColor = false;
            // 
            // returnFarmBtn
            // 
            this.returnFarmBtn.BackColor = System.Drawing.Color.White;
            this.returnFarmBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.returnFarmBtn.Location = new System.Drawing.Point(271, 26);
            this.returnFarmBtn.Name = "returnFarmBtn";
            this.returnFarmBtn.Size = new System.Drawing.Size(115, 47);
            this.returnFarmBtn.TabIndex = 159;
            this.returnFarmBtn.Text = "Return Here After Banking";
            this.returnFarmBtn.UseVisualStyleBackColor = false;
            // 
            // setCustomBankBtn
            // 
            this.setCustomBankBtn.BackColor = System.Drawing.Color.White;
            this.setCustomBankBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setCustomBankBtn.Location = new System.Drawing.Point(141, 53);
            this.setCustomBankBtn.Name = "setCustomBankBtn";
            this.setCustomBankBtn.Size = new System.Drawing.Size(115, 25);
            this.setCustomBankBtn.TabIndex = 158;
            this.setCustomBankBtn.Text = "Set Custom Bank";
            this.setCustomBankBtn.UseVisualStyleBackColor = false;
            // 
            // toggleFarmBtn
            // 
            this.toggleFarmBtn.BackColor = System.Drawing.Color.White;
            this.toggleFarmBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleFarmBtn.Location = new System.Drawing.Point(6, 31);
            this.toggleFarmBtn.Name = "toggleFarmBtn";
            this.toggleFarmBtn.Size = new System.Drawing.Size(75, 36);
            this.toggleFarmBtn.TabIndex = 9;
            this.toggleFarmBtn.Text = "Farm";
            this.toggleFarmBtn.UseVisualStyleBackColor = false;
            // 
            // hubaeGroup
            // 
            this.hubaeGroup.Controls.Add(this.finishedWhiteBtn);
            this.hubaeGroup.Controls.Add(this.teacherLbl);
            this.hubaeGroup.Controls.Add(this.teacherText);
            this.hubaeGroup.Controls.Add(this.killedCrabBtn);
            this.hubaeGroup.Controls.Add(this.killedBatBtn);
            this.hubaeGroup.Controls.Add(this.toggleHubaeBtn);
            this.hubaeGroup.Location = new System.Drawing.Point(425, 3);
            this.hubaeGroup.Name = "hubaeGroup";
            this.hubaeGroup.Size = new System.Drawing.Size(414, 86);
            this.hubaeGroup.TabIndex = 11;
            this.hubaeGroup.TabStop = false;
            this.hubaeGroup.Text = "Hubae";
            // 
            // finishedWhiteBtn
            // 
            this.finishedWhiteBtn.BackColor = System.Drawing.Color.White;
            this.finishedWhiteBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.finishedWhiteBtn.Location = new System.Drawing.Point(211, 19);
            this.finishedWhiteBtn.Name = "finishedWhiteBtn";
            this.finishedWhiteBtn.Size = new System.Drawing.Size(68, 60);
            this.finishedWhiteBtn.TabIndex = 14;
            this.finishedWhiteBtn.Text = "Finished White Dugon";
            this.finishedWhiteBtn.UseVisualStyleBackColor = false;
            // 
            // teacherLbl
            // 
            this.teacherLbl.AutoSize = true;
            this.teacherLbl.Location = new System.Drawing.Point(329, 24);
            this.teacherLbl.Name = "teacherLbl";
            this.teacherLbl.Size = new System.Drawing.Size(52, 15);
            this.teacherLbl.TabIndex = 13;
            this.teacherLbl.Text = "Teacher?";
            // 
            // teacherText
            // 
            this.teacherText.AcceptsReturn = true;
            this.teacherText.BackColor = System.Drawing.Color.White;
            this.teacherText.ForeColor = System.Drawing.Color.Black;
            this.teacherText.Location = new System.Drawing.Point(298, 50);
            this.teacherText.Name = "teacherText";
            this.teacherText.Size = new System.Drawing.Size(112, 23);
            this.teacherText.TabIndex = 12;
            // 
            // killedCrabBtn
            // 
            this.killedCrabBtn.BackColor = System.Drawing.Color.White;
            this.killedCrabBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.killedCrabBtn.Location = new System.Drawing.Point(130, 50);
            this.killedCrabBtn.Name = "killedCrabBtn";
            this.killedCrabBtn.Size = new System.Drawing.Size(75, 29);
            this.killedCrabBtn.TabIndex = 11;
            this.killedCrabBtn.Text = "Killed Crab";
            this.killedCrabBtn.UseVisualStyleBackColor = false;
            // 
            // killedBatBtn
            // 
            this.killedBatBtn.BackColor = System.Drawing.Color.White;
            this.killedBatBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.killedBatBtn.Location = new System.Drawing.Point(130, 19);
            this.killedBatBtn.Name = "killedBatBtn";
            this.killedBatBtn.Size = new System.Drawing.Size(75, 29);
            this.killedBatBtn.TabIndex = 10;
            this.killedBatBtn.Text = "Killed Bat";
            this.killedBatBtn.UseVisualStyleBackColor = false;
            // 
            // toggleHubaeBtn
            // 
            this.toggleHubaeBtn.BackColor = System.Drawing.Color.White;
            this.toggleHubaeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleHubaeBtn.Location = new System.Drawing.Point(6, 22);
            this.toggleHubaeBtn.Name = "toggleHubaeBtn";
            this.toggleHubaeBtn.Size = new System.Drawing.Size(104, 50);
            this.toggleHubaeBtn.TabIndex = 9;
            this.toggleHubaeBtn.Text = "Make Hubs";
            this.toggleHubaeBtn.UseVisualStyleBackColor = false;
            // 
            // laborGroup
            // 
            this.laborGroup.Controls.Add(this.fastLaborCbox);
            this.laborGroup.Controls.Add(this.laborNameText);
            this.laborGroup.Controls.Add(this.laborBtn);
            this.laborGroup.Location = new System.Drawing.Point(3, 194);
            this.laborGroup.Name = "laborGroup";
            this.laborGroup.Size = new System.Drawing.Size(205, 86);
            this.laborGroup.TabIndex = 7;
            this.laborGroup.TabStop = false;
            this.laborGroup.Text = "Labor";
            // 
            // fastLaborCbox
            // 
            this.fastLaborCbox.AutoSize = true;
            this.fastLaborCbox.Location = new System.Drawing.Point(125, 24);
            this.fastLaborCbox.Name = "fastLaborCbox";
            this.fastLaborCbox.Size = new System.Drawing.Size(47, 19);
            this.fastLaborCbox.TabIndex = 10;
            this.fastLaborCbox.Text = "Fast";
            this.fastLaborCbox.UseVisualStyleBackColor = true;
            // 
            // laborNameText
            // 
            this.laborNameText.AcceptsReturn = true;
            this.laborNameText.BackColor = System.Drawing.Color.White;
            this.laborNameText.ForeColor = System.Drawing.Color.Black;
            this.laborNameText.Location = new System.Drawing.Point(87, 49);
            this.laborNameText.Name = "laborNameText";
            this.laborNameText.Size = new System.Drawing.Size(112, 23);
            this.laborNameText.TabIndex = 10;
            // 
            // laborBtn
            // 
            this.laborBtn.BackColor = System.Drawing.Color.White;
            this.laborBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.laborBtn.Location = new System.Drawing.Point(6, 31);
            this.laborBtn.Name = "laborBtn";
            this.laborBtn.Size = new System.Drawing.Size(75, 36);
            this.laborBtn.TabIndex = 9;
            this.laborBtn.Text = "Labor";
            this.laborBtn.UseVisualStyleBackColor = false;
            // 
            // polishGroup
            // 
            this.polishGroup.Controls.Add(this.polishUncutCbox);
            this.polishGroup.Controls.Add(this.polishBtn);
            this.polishGroup.Controls.Add(this.polishAssistCbox);
            this.polishGroup.Controls.Add(this.assistText);
            this.polishGroup.Controls.Add(this.polishLbl);
            this.polishGroup.Location = new System.Drawing.Point(423, 293);
            this.polishGroup.Name = "polishGroup";
            this.polishGroup.Size = new System.Drawing.Size(205, 100);
            this.polishGroup.TabIndex = 6;
            this.polishGroup.TabStop = false;
            this.polishGroup.Text = "Gem Polish";
            // 
            // polishUncutCbox
            // 
            this.polishUncutCbox.AutoSize = true;
            this.polishUncutCbox.Location = new System.Drawing.Point(145, 24);
            this.polishUncutCbox.Name = "polishUncutCbox";
            this.polishUncutCbox.Size = new System.Drawing.Size(58, 19);
            this.polishUncutCbox.TabIndex = 9;
            this.polishUncutCbox.Text = "Uncut";
            this.polishUncutCbox.UseVisualStyleBackColor = true;
            // 
            // polishBtn
            // 
            this.polishBtn.BackColor = System.Drawing.Color.White;
            this.polishBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.polishBtn.Location = new System.Drawing.Point(6, 31);
            this.polishBtn.Name = "polishBtn";
            this.polishBtn.Size = new System.Drawing.Size(75, 36);
            this.polishBtn.TabIndex = 8;
            this.polishBtn.Text = "Polish";
            this.polishBtn.UseVisualStyleBackColor = false;
            // 
            // polishAssistCbox
            // 
            this.polishAssistCbox.AutoSize = true;
            this.polishAssistCbox.Location = new System.Drawing.Point(87, 24);
            this.polishAssistCbox.Name = "polishAssistCbox";
            this.polishAssistCbox.Size = new System.Drawing.Size(56, 19);
            this.polishAssistCbox.TabIndex = 1;
            this.polishAssistCbox.Text = "Assist";
            this.polishAssistCbox.UseVisualStyleBackColor = true;
            // 
            // assistText
            // 
            this.assistText.AcceptsReturn = true;
            this.assistText.BackColor = System.Drawing.Color.White;
            this.assistText.ForeColor = System.Drawing.Color.Black;
            this.assistText.Location = new System.Drawing.Point(87, 49);
            this.assistText.Name = "assistText";
            this.assistText.Size = new System.Drawing.Size(112, 23);
            this.assistText.TabIndex = 0;
            // 
            // polishLbl
            // 
            this.polishLbl.AutoSize = true;
            this.polishLbl.Location = new System.Drawing.Point(4, 75);
            this.polishLbl.Name = "polishLbl";
            this.polishLbl.Size = new System.Drawing.Size(190, 15);
            this.polishLbl.TabIndex = 8;
            this.polishLbl.Text = "*Leave first slot in inventory empty";
            // 
            // ascensionGroup
            // 
            this.ascensionGroup.Controls.Add(this.tocYNum);
            this.ascensionGroup.Controls.Add(this.tocYLbl);
            this.ascensionGroup.Controls.Add(this.tocXNum);
            this.ascensionGroup.Controls.Add(this.tocXLbl);
            this.ascensionGroup.Controls.Add(this.tocLbl);
            this.ascensionGroup.Controls.Add(this.statLbl);
            this.ascensionGroup.Controls.Add(this.statCombox);
            this.ascensionGroup.Controls.Add(this.statsNum);
            this.ascensionGroup.Controls.Add(this.statsPerLbl);
            this.ascensionGroup.Controls.Add(this.toggleAscensionBtn);
            this.ascensionGroup.Location = new System.Drawing.Point(3, 3);
            this.ascensionGroup.Name = "ascensionGroup";
            this.ascensionGroup.Size = new System.Drawing.Size(414, 86);
            this.ascensionGroup.TabIndex = 5;
            this.ascensionGroup.TabStop = false;
            this.ascensionGroup.Text = "Stat Ascension";
            // 
            // tocYNum
            // 
            this.tocYNum.ForeColor = System.Drawing.Color.Black;
            this.tocYNum.Location = new System.Drawing.Point(351, 52);
            this.tocYNum.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.tocYNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tocYNum.Name = "tocYNum";
            this.tocYNum.Size = new System.Drawing.Size(43, 23);
            this.tocYNum.TabIndex = 156;
            this.tocYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tocYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tocYLbl
            // 
            this.tocYLbl.AutoSize = true;
            this.tocYLbl.Location = new System.Drawing.Point(322, 55);
            this.tocYLbl.Name = "tocYLbl";
            this.tocYLbl.Size = new System.Drawing.Size(23, 15);
            this.tocYLbl.TabIndex = 155;
            this.tocYLbl.Text = ",  Y";
            this.tocYLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tocXNum
            // 
            this.tocXNum.ForeColor = System.Drawing.Color.Black;
            this.tocXNum.Location = new System.Drawing.Point(273, 52);
            this.tocXNum.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.tocXNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tocXNum.Name = "tocXNum";
            this.tocXNum.Size = new System.Drawing.Size(43, 23);
            this.tocXNum.TabIndex = 154;
            this.tocXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tocXNum.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // tocXLbl
            // 
            this.tocXLbl.AutoSize = true;
            this.tocXLbl.Location = new System.Drawing.Point(253, 55);
            this.tocXLbl.Name = "tocXLbl";
            this.tocXLbl.Size = new System.Drawing.Size(14, 15);
            this.tocXLbl.TabIndex = 153;
            this.tocXLbl.Text = "X";
            this.tocXLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tocLbl
            // 
            this.tocLbl.AutoSize = true;
            this.tocLbl.Location = new System.Drawing.Point(266, 22);
            this.tocLbl.Name = "tocLbl";
            this.tocLbl.Size = new System.Drawing.Size(94, 15);
            this.tocLbl.TabIndex = 152;
            this.tocLbl.Text = "ToC Coordinates";
            this.tocLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statLbl
            // 
            this.statLbl.AutoSize = true;
            this.statLbl.Location = new System.Drawing.Point(116, 22);
            this.statLbl.Name = "statLbl";
            this.statLbl.Size = new System.Drawing.Size(75, 15);
            this.statLbl.TabIndex = 151;
            this.statLbl.Text = "Stat To Raise:";
            this.statLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statCombox
            // 
            this.statCombox.ForeColor = System.Drawing.Color.Black;
            this.statCombox.FormattingEnabled = true;
            this.statCombox.Items.AddRange(new object[] {
            "STR",
            "INT",
            "WIS",
            "CON",
            "DEX"});
            this.statCombox.Location = new System.Drawing.Point(197, 19);
            this.statCombox.Name = "statCombox";
            this.statCombox.Size = new System.Drawing.Size(49, 23);
            this.statCombox.TabIndex = 150;
            this.statCombox.Text = "STR";
            // 
            // statsNum
            // 
            this.statsNum.ForeColor = System.Drawing.Color.Black;
            this.statsNum.Location = new System.Drawing.Point(197, 50);
            this.statsNum.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.statsNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.statsNum.Name = "statsNum";
            this.statsNum.Size = new System.Drawing.Size(39, 23);
            this.statsNum.TabIndex = 149;
            this.statsNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statsNum.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // statsPerLbl
            // 
            this.statsPerLbl.AutoSize = true;
            this.statsPerLbl.Location = new System.Drawing.Point(122, 52);
            this.statsPerLbl.Name = "statsPerLbl";
            this.statsPerLbl.Size = new System.Drawing.Size(69, 15);
            this.statsPerLbl.TabIndex = 148;
            this.statsPerLbl.Text = "Per Ascend:";
            this.statsPerLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleAscensionBtn
            // 
            this.toggleAscensionBtn.BackColor = System.Drawing.Color.White;
            this.toggleAscensionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleAscensionBtn.Location = new System.Drawing.Point(6, 22);
            this.toggleAscensionBtn.Name = "toggleAscensionBtn";
            this.toggleAscensionBtn.Size = new System.Drawing.Size(104, 50);
            this.toggleAscensionBtn.TabIndex = 1;
            this.toggleAscensionBtn.Text = "Ascend";
            this.toggleAscensionBtn.UseVisualStyleBackColor = false;
            // 
            // dojoTab
            // 
            this.dojoTab.Controls.Add(this.flowerCbox);
            this.dojoTab.Controls.Add(this.dojo2SpaceCbox);
            this.dojoTab.Controls.Add(this.rescueCbox);
            this.dojoTab.Controls.Add(this.flowerText);
            this.dojoTab.Controls.Add(this.dojoRefreshBtn);
            this.dojoTab.Controls.Add(this.dojoCounterAttackCbox);
            this.dojoTab.Controls.Add(this.dojoRegenerationCbox);
            this.dojoTab.Controls.Add(this.unmaxedSpellsGroup);
            this.dojoTab.Controls.Add(this.dojoOptionsGroup);
            this.dojoTab.Controls.Add(this.unmaxedSkillsGroup);
            this.dojoTab.Location = new System.Drawing.Point(4, 24);
            this.dojoTab.Name = "dojoTab";
            this.dojoTab.Size = new System.Drawing.Size(842, 516);
            this.dojoTab.TabIndex = 10;
            this.dojoTab.Text = "Dojo";
            this.dojoTab.UseVisualStyleBackColor = true;
            // 
            // flowerCbox
            // 
            this.flowerCbox.AutoSize = true;
            this.flowerCbox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowerCbox.Location = new System.Drawing.Point(626, 409);
            this.flowerCbox.Name = "flowerCbox";
            this.flowerCbox.Size = new System.Drawing.Size(76, 25);
            this.flowerCbox.TabIndex = 9;
            this.flowerCbox.Text = "Flower";
            this.flowerCbox.UseVisualStyleBackColor = true;
            // 
            // dojo2SpaceCbox
            // 
            this.dojo2SpaceCbox.AutoSize = true;
            this.dojo2SpaceCbox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dojo2SpaceCbox.Location = new System.Drawing.Point(658, 252);
            this.dojo2SpaceCbox.Name = "dojo2SpaceCbox";
            this.dojo2SpaceCbox.Size = new System.Drawing.Size(132, 25);
            this.dojo2SpaceCbox.TabIndex = 12;
            this.dojo2SpaceCbox.Text = "2 Spaces Away";
            this.dojo2SpaceCbox.UseVisualStyleBackColor = true;
            // 
            // rescueCbox
            // 
            this.rescueCbox.AutoSize = true;
            this.rescueCbox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rescueCbox.Location = new System.Drawing.Point(658, 221);
            this.rescueCbox.Name = "rescueCbox";
            this.rescueCbox.Size = new System.Drawing.Size(122, 25);
            this.rescueCbox.TabIndex = 11;
            this.rescueCbox.Text = "Spam Rescue";
            this.rescueCbox.UseVisualStyleBackColor = true;
            // 
            // flowerText
            // 
            this.flowerText.Location = new System.Drawing.Point(708, 412);
            this.flowerText.Name = "flowerText";
            this.flowerText.Size = new System.Drawing.Size(112, 23);
            this.flowerText.TabIndex = 10;
            // 
            // dojoRefreshBtn
            // 
            this.dojoRefreshBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dojoRefreshBtn.Location = new System.Drawing.Point(658, 329);
            this.dojoRefreshBtn.Name = "dojoRefreshBtn";
            this.dojoRefreshBtn.Size = new System.Drawing.Size(132, 49);
            this.dojoRefreshBtn.TabIndex = 8;
            this.dojoRefreshBtn.Text = "Refresh Skills/Spells";
            this.dojoRefreshBtn.UseVisualStyleBackColor = true;
            this.dojoRefreshBtn.Click += new System.EventHandler(this.dojoRefreshBtn_Click);
            // 
            // dojoCounterAttackCbox
            // 
            this.dojoCounterAttackCbox.AutoSize = true;
            this.dojoCounterAttackCbox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dojoCounterAttackCbox.Location = new System.Drawing.Point(658, 190);
            this.dojoCounterAttackCbox.Name = "dojoCounterAttackCbox";
            this.dojoCounterAttackCbox.Size = new System.Drawing.Size(132, 25);
            this.dojoCounterAttackCbox.TabIndex = 7;
            this.dojoCounterAttackCbox.Text = "Counter Attack";
            this.dojoCounterAttackCbox.UseVisualStyleBackColor = true;
            // 
            // dojoRegenerationCbox
            // 
            this.dojoRegenerationCbox.AutoSize = true;
            this.dojoRegenerationCbox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dojoRegenerationCbox.Location = new System.Drawing.Point(658, 159);
            this.dojoRegenerationCbox.Name = "dojoRegenerationCbox";
            this.dojoRegenerationCbox.Size = new System.Drawing.Size(122, 25);
            this.dojoRegenerationCbox.TabIndex = 6;
            this.dojoRegenerationCbox.Text = "Regeneration";
            this.dojoRegenerationCbox.UseVisualStyleBackColor = true;
            // 
            // unmaxedSpellsGroup
            // 
            this.unmaxedSpellsGroup.Location = new System.Drawing.Point(23, 308);
            this.unmaxedSpellsGroup.Name = "unmaxedSpellsGroup";
            this.unmaxedSpellsGroup.Size = new System.Drawing.Size(525, 191);
            this.unmaxedSpellsGroup.TabIndex = 5;
            this.unmaxedSpellsGroup.TabStop = false;
            this.unmaxedSpellsGroup.Text = "Unmaxed Spells";
            // 
            // dojoOptionsGroup
            // 
            this.dojoOptionsGroup.Controls.Add(this.autoLbl);
            this.dojoOptionsGroup.Controls.Add(this.dojoAutoStaffCbox);
            this.dojoOptionsGroup.Controls.Add(this.toggleDojoBtn);
            this.dojoOptionsGroup.Controls.Add(this.dojoBonusCbox);
            this.dojoOptionsGroup.Location = new System.Drawing.Point(23, 23);
            this.dojoOptionsGroup.Name = "dojoOptionsGroup";
            this.dojoOptionsGroup.Size = new System.Drawing.Size(799, 95);
            this.dojoOptionsGroup.TabIndex = 4;
            this.dojoOptionsGroup.TabStop = false;
            this.dojoOptionsGroup.Text = "Dojo Options";
            // 
            // autoLbl
            // 
            this.autoLbl.AutoSize = true;
            this.autoLbl.Location = new System.Drawing.Point(363, 56);
            this.autoLbl.Name = "autoLbl";
            this.autoLbl.Size = new System.Drawing.Size(224, 30);
            this.autoLbl.TabIndex = 6;
            this.autoLbl.Text = "This will automatically unequip/equip\r\nweapons/staffs as needed for kicks/spells";
            this.autoLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dojoAutoStaffCbox
            // 
            this.dojoAutoStaffCbox.AutoSize = true;
            this.dojoAutoStaffCbox.Checked = true;
            this.dojoAutoStaffCbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dojoAutoStaffCbox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dojoAutoStaffCbox.Location = new System.Drawing.Point(403, 33);
            this.dojoAutoStaffCbox.Name = "dojoAutoStaffCbox";
            this.dojoAutoStaffCbox.Size = new System.Drawing.Size(147, 25);
            this.dojoAutoStaffCbox.TabIndex = 132;
            this.dojoAutoStaffCbox.Text = "Auto Staff Switch";
            this.dojoAutoStaffCbox.UseVisualStyleBackColor = true;
            // 
            // toggleDojoBtn
            // 
            this.toggleDojoBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleDojoBtn.Location = new System.Drawing.Point(18, 22);
            this.toggleDojoBtn.Name = "toggleDojoBtn";
            this.toggleDojoBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleDojoBtn.TabIndex = 1;
            this.toggleDojoBtn.Text = "Enable";
            this.toggleDojoBtn.UseVisualStyleBackColor = true;
            this.toggleDojoBtn.Click += new System.EventHandler(this.toggleDojoBtn_Click);
            // 
            // dojoBonusCbox
            // 
            this.dojoBonusCbox.AutoSize = true;
            this.dojoBonusCbox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dojoBonusCbox.Location = new System.Drawing.Point(183, 33);
            this.dojoBonusCbox.Name = "dojoBonusCbox";
            this.dojoBonusCbox.Size = new System.Drawing.Size(190, 25);
            this.dojoBonusCbox.TabIndex = 2;
            this.dojoBonusCbox.Text = "Use Skill/Spell Bonuses";
            this.dojoBonusCbox.UseVisualStyleBackColor = true;
            // 
            // unmaxedSkillsGroup
            // 
            this.unmaxedSkillsGroup.Location = new System.Drawing.Point(23, 135);
            this.unmaxedSkillsGroup.Name = "unmaxedSkillsGroup";
            this.unmaxedSkillsGroup.Size = new System.Drawing.Size(525, 167);
            this.unmaxedSkillsGroup.TabIndex = 0;
            this.unmaxedSkillsGroup.TabStop = false;
            this.unmaxedSkillsGroup.Text = "Unmaxed Skills";
            // 
            // mainEventsTab
            // 
            this.mainEventsTab.Controls.Add(this.candyTreatsGroup);
            this.mainEventsTab.Controls.Add(this.parchmentMaskGroup);
            this.mainEventsTab.Controls.Add(this.scavengerHuntGroup);
            this.mainEventsTab.Controls.Add(this.seasonalDblGroup);
            this.mainEventsTab.Controls.Add(this.groupBox8);
            this.mainEventsTab.Controls.Add(this.MAWGroup);
            this.mainEventsTab.Controls.Add(this.fowlGroup);
            this.mainEventsTab.Controls.Add(this.pigChaseGroup);
            this.mainEventsTab.Controls.Add(this.yuleGroup);
            this.mainEventsTab.Controls.Add(this.bugGroup);
            this.mainEventsTab.Location = new System.Drawing.Point(4, 24);
            this.mainEventsTab.Name = "mainEventsTab";
            this.mainEventsTab.Size = new System.Drawing.Size(842, 516);
            this.mainEventsTab.TabIndex = 11;
            this.mainEventsTab.Text = "Events";
            this.mainEventsTab.UseVisualStyleBackColor = true;
            // 
            // candyTreatsGroup
            // 
            this.candyTreatsGroup.BackColor = System.Drawing.Color.White;
            this.candyTreatsGroup.Controls.Add(this.candyTreatsLbl);
            this.candyTreatsGroup.Controls.Add(this.toggleCandyTreatsBtn);
            this.candyTreatsGroup.Location = new System.Drawing.Point(423, 386);
            this.candyTreatsGroup.Name = "candyTreatsGroup";
            this.candyTreatsGroup.Size = new System.Drawing.Size(408, 88);
            this.candyTreatsGroup.TabIndex = 161;
            this.candyTreatsGroup.TabStop = false;
            this.candyTreatsGroup.Text = "Candy Treats";
            // 
            // candyTreatsLbl
            // 
            this.candyTreatsLbl.Location = new System.Drawing.Point(179, 18);
            this.candyTreatsLbl.Name = "candyTreatsLbl";
            this.candyTreatsLbl.Size = new System.Drawing.Size(223, 66);
            this.candyTreatsLbl.TabIndex = 156;
            this.candyTreatsLbl.Text = "Picks up Candy Treats then shoves them down a little girls mug for a crappy Hallo" +
    "ween Box. Please make inventory space for treats.";
            this.candyTreatsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleCandyTreatsBtn
            // 
            this.toggleCandyTreatsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleCandyTreatsBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleCandyTreatsBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleCandyTreatsBtn.Name = "toggleCandyTreatsBtn";
            this.toggleCandyTreatsBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleCandyTreatsBtn.TabIndex = 2;
            this.toggleCandyTreatsBtn.Text = "Enable";
            this.toggleCandyTreatsBtn.UseVisualStyleBackColor = true;
            this.toggleCandyTreatsBtn.Click += new System.EventHandler(this.toggleCandyTreatsBtn_Click);
            // 
            // parchmentMaskGroup
            // 
            this.parchmentMaskGroup.BackColor = System.Drawing.Color.White;
            this.parchmentMaskGroup.Controls.Add(this.parchmentMaskLbl);
            this.parchmentMaskGroup.Controls.Add(this.toggleParchmentMaskBtn);
            this.parchmentMaskGroup.Location = new System.Drawing.Point(9, 385);
            this.parchmentMaskGroup.Name = "parchmentMaskGroup";
            this.parchmentMaskGroup.Size = new System.Drawing.Size(408, 88);
            this.parchmentMaskGroup.TabIndex = 160;
            this.parchmentMaskGroup.TabStop = false;
            this.parchmentMaskGroup.Text = "Parchment Mask";
            // 
            // parchmentMaskLbl
            // 
            this.parchmentMaskLbl.Location = new System.Drawing.Point(184, 32);
            this.parchmentMaskLbl.Name = "parchmentMaskLbl";
            this.parchmentMaskLbl.Size = new System.Drawing.Size(218, 31);
            this.parchmentMaskLbl.TabIndex = 156;
            this.parchmentMaskLbl.Text = "Picks up Parchment and makes a mask! Please make inventory space!";
            this.parchmentMaskLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleParchmentMaskBtn
            // 
            this.toggleParchmentMaskBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleParchmentMaskBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleParchmentMaskBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleParchmentMaskBtn.Name = "toggleParchmentMaskBtn";
            this.toggleParchmentMaskBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleParchmentMaskBtn.TabIndex = 2;
            this.toggleParchmentMaskBtn.Text = "Enable";
            this.toggleParchmentMaskBtn.UseVisualStyleBackColor = true;
            this.toggleParchmentMaskBtn.Click += new System.EventHandler(this.toggleParchmentMaskBtn_Click);
            // 
            // scavengerHuntGroup
            // 
            this.scavengerHuntGroup.Controls.Add(this.scavengerHuntLbl);
            this.scavengerHuntGroup.Controls.Add(this.toggleScavengerHuntBtn);
            this.scavengerHuntGroup.Location = new System.Drawing.Point(423, 292);
            this.scavengerHuntGroup.Name = "scavengerHuntGroup";
            this.scavengerHuntGroup.Size = new System.Drawing.Size(408, 88);
            this.scavengerHuntGroup.TabIndex = 160;
            this.scavengerHuntGroup.TabStop = false;
            this.scavengerHuntGroup.Text = "Oren Ball Scavenger Hunt";
            // 
            // scavengerHuntLbl
            // 
            this.scavengerHuntLbl.AutoSize = true;
            this.scavengerHuntLbl.Location = new System.Drawing.Point(176, 24);
            this.scavengerHuntLbl.Name = "scavengerHuntLbl";
            this.scavengerHuntLbl.Size = new System.Drawing.Size(198, 15);
            this.scavengerHuntLbl.TabIndex = 159;
            this.scavengerHuntLbl.Text = "Find some items to get a ball crown!";
            this.scavengerHuntLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleScavengerHuntBtn
            // 
            this.toggleScavengerHuntBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleScavengerHuntBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleScavengerHuntBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleScavengerHuntBtn.Name = "toggleScavengerHuntBtn";
            this.toggleScavengerHuntBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleScavengerHuntBtn.TabIndex = 157;
            this.toggleScavengerHuntBtn.Text = "Enable";
            this.toggleScavengerHuntBtn.UseVisualStyleBackColor = true;
            this.toggleScavengerHuntBtn.Click += new System.EventHandler(this.toggleScavengerHuntBtn_Click);
            // 
            // seasonalDblGroup
            // 
            this.seasonalDblGroup.Controls.Add(this.seasonalDblLbl);
            this.seasonalDblGroup.Controls.Add(this.toggleSeaonalDblBtn);
            this.seasonalDblGroup.Location = new System.Drawing.Point(9, 291);
            this.seasonalDblGroup.Name = "seasonalDblGroup";
            this.seasonalDblGroup.Size = new System.Drawing.Size(408, 88);
            this.seasonalDblGroup.TabIndex = 159;
            this.seasonalDblGroup.TabStop = false;
            this.seasonalDblGroup.Text = "Double Bonus";
            // 
            // seasonalDblLbl
            // 
            this.seasonalDblLbl.AutoSize = true;
            this.seasonalDblLbl.Location = new System.Drawing.Point(206, 32);
            this.seasonalDblLbl.Name = "seasonalDblLbl";
            this.seasonalDblLbl.Size = new System.Drawing.Size(170, 15);
            this.seasonalDblLbl.TabIndex = 156;
            this.seasonalDblLbl.Text = "Let\'s collect a stack of doubles!";
            this.seasonalDblLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleSeaonalDblBtn
            // 
            this.toggleSeaonalDblBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleSeaonalDblBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleSeaonalDblBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleSeaonalDblBtn.Name = "toggleSeaonalDblBtn";
            this.toggleSeaonalDblBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleSeaonalDblBtn.TabIndex = 2;
            this.toggleSeaonalDblBtn.Text = "Enable";
            this.toggleSeaonalDblBtn.UseVisualStyleBackColor = true;
            this.toggleSeaonalDblBtn.Click += new System.EventHandler(this.toggleSeasonalDblBtn_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label8);
            this.groupBox8.Controls.Add(this.toggleCatchLeprechaunBtn);
            this.groupBox8.Location = new System.Drawing.Point(423, 198);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(408, 88);
            this.groupBox8.TabIndex = 159;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Catch a Leprechaun";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(176, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(212, 45);
            this.label8.TabIndex = 159;
            this.label8.Text = "Heads to Suomi to craft net and hit \r\nLeprechauns. Will then route to Mileth \r\nto" +
    " buy Dirks and finish the quest.";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleCatchLeprechaunBtn
            // 
            this.toggleCatchLeprechaunBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleCatchLeprechaunBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleCatchLeprechaunBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleCatchLeprechaunBtn.Name = "toggleCatchLeprechaunBtn";
            this.toggleCatchLeprechaunBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleCatchLeprechaunBtn.TabIndex = 157;
            this.toggleCatchLeprechaunBtn.Text = "Enable";
            this.toggleCatchLeprechaunBtn.UseVisualStyleBackColor = true;
            this.toggleCatchLeprechaunBtn.Click += new System.EventHandler(this.toggleCatchLeprechaunBtn_Click);
            // 
            // MAWGroup
            // 
            this.MAWGroup.Controls.Add(this.mawLbl);
            this.MAWGroup.Controls.Add(this.toggleMAWBtn);
            this.MAWGroup.Location = new System.Drawing.Point(9, 198);
            this.MAWGroup.Name = "MAWGroup";
            this.MAWGroup.Size = new System.Drawing.Size(408, 88);
            this.MAWGroup.TabIndex = 158;
            this.MAWGroup.TabStop = false;
            this.MAWGroup.Text = "Make A Wish Event";
            // 
            // mawLbl
            // 
            this.mawLbl.AutoSize = true;
            this.mawLbl.Location = new System.Drawing.Point(206, 32);
            this.mawLbl.Name = "mawLbl";
            this.mawLbl.Size = new System.Drawing.Size(177, 30);
            this.mawLbl.TabIndex = 156;
            this.mawLbl.Text = "Will walk near fountain and stop\r\nfor you to pick a prize";
            this.mawLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleMAWBtn
            // 
            this.toggleMAWBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleMAWBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleMAWBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleMAWBtn.Name = "toggleMAWBtn";
            this.toggleMAWBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleMAWBtn.TabIndex = 2;
            this.toggleMAWBtn.Text = "Enable";
            this.toggleMAWBtn.UseVisualStyleBackColor = true;
            this.toggleMAWBtn.Click += new System.EventHandler(this.toggleMAWBtn_Click);
            // 
            // fowlGroup
            // 
            this.fowlGroup.Controls.Add(this.fowlLbl);
            this.fowlGroup.Controls.Add(this.toggleFowlBtn);
            this.fowlGroup.Location = new System.Drawing.Point(423, 104);
            this.fowlGroup.Name = "fowlGroup";
            this.fowlGroup.Size = new System.Drawing.Size(408, 88);
            this.fowlGroup.TabIndex = 157;
            this.fowlGroup.TabStop = false;
            this.fowlGroup.Text = "Fowl Event";
            // 
            // fowlLbl
            // 
            this.fowlLbl.AutoSize = true;
            this.fowlLbl.Location = new System.Drawing.Point(224, 32);
            this.fowlLbl.Name = "fowlLbl";
            this.fowlLbl.Size = new System.Drawing.Size(133, 30);
            this.fowlLbl.TabIndex = 156;
            this.fowlLbl.Text = "Will beep when you are \r\nready to turn in.";
            this.fowlLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleFowlBtn
            // 
            this.toggleFowlBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleFowlBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleFowlBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleFowlBtn.Name = "toggleFowlBtn";
            this.toggleFowlBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleFowlBtn.TabIndex = 2;
            this.toggleFowlBtn.Text = "Enable";
            this.toggleFowlBtn.UseVisualStyleBackColor = true;
            this.toggleFowlBtn.Click += new System.EventHandler(this.toggleFowlBtn_Click);
            // 
            // pigChaseGroup
            // 
            this.pigChaseGroup.Controls.Add(this.pigChaseLbl);
            this.pigChaseGroup.Controls.Add(this.togglePigChaseBtn);
            this.pigChaseGroup.Location = new System.Drawing.Point(9, 104);
            this.pigChaseGroup.Name = "pigChaseGroup";
            this.pigChaseGroup.Size = new System.Drawing.Size(408, 88);
            this.pigChaseGroup.TabIndex = 6;
            this.pigChaseGroup.TabStop = false;
            this.pigChaseGroup.Text = "Pig Chase Event";
            // 
            // pigChaseLbl
            // 
            this.pigChaseLbl.AutoSize = true;
            this.pigChaseLbl.Location = new System.Drawing.Point(225, 32);
            this.pigChaseLbl.Name = "pigChaseLbl";
            this.pigChaseLbl.Size = new System.Drawing.Size(133, 30);
            this.pigChaseLbl.TabIndex = 156;
            this.pigChaseLbl.Text = "Will beep when you are \r\nready to turn in.";
            this.pigChaseLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // togglePigChaseBtn
            // 
            this.togglePigChaseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.togglePigChaseBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.togglePigChaseBtn.Location = new System.Drawing.Point(25, 22);
            this.togglePigChaseBtn.Name = "togglePigChaseBtn";
            this.togglePigChaseBtn.Size = new System.Drawing.Size(132, 49);
            this.togglePigChaseBtn.TabIndex = 2;
            this.togglePigChaseBtn.Text = "Enable";
            this.togglePigChaseBtn.UseVisualStyleBackColor = true;
            this.togglePigChaseBtn.Click += new System.EventHandler(this.togglePigChaseBtn_Click);
            // 
            // yuleGroup
            // 
            this.yuleGroup.Controls.Add(this.yuleLbl);
            this.yuleGroup.Controls.Add(this.toggleYuleBtn);
            this.yuleGroup.Location = new System.Drawing.Point(423, 10);
            this.yuleGroup.Name = "yuleGroup";
            this.yuleGroup.Size = new System.Drawing.Size(408, 88);
            this.yuleGroup.TabIndex = 5;
            this.yuleGroup.TabStop = false;
            this.yuleGroup.Text = "Yule Logs Event";
            // 
            // yuleLbl
            // 
            this.yuleLbl.AutoSize = true;
            this.yuleLbl.Location = new System.Drawing.Point(176, 26);
            this.yuleLbl.Name = "yuleLbl";
            this.yuleLbl.Size = new System.Drawing.Size(220, 45);
            this.yuleLbl.TabIndex = 158;
            this.yuleLbl.Text = "Please have six fior srad in inventory\r\nand have at least four open slots.\r\nBegga" +
    "r old Torbjorn axes before starting.";
            this.yuleLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleYuleBtn
            // 
            this.toggleYuleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleYuleBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleYuleBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleYuleBtn.Name = "toggleYuleBtn";
            this.toggleYuleBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleYuleBtn.TabIndex = 157;
            this.toggleYuleBtn.Text = "Enable";
            this.toggleYuleBtn.UseVisualStyleBackColor = true;
            this.toggleYuleBtn.Click += new System.EventHandler(this.toggleYuleBtn_Click);
            // 
            // bugGroup
            // 
            this.bugGroup.Controls.Add(this.bugLbl);
            this.bugGroup.Controls.Add(this.toggleBugBtn);
            this.bugGroup.Location = new System.Drawing.Point(9, 10);
            this.bugGroup.Name = "bugGroup";
            this.bugGroup.Size = new System.Drawing.Size(408, 88);
            this.bugGroup.TabIndex = 4;
            this.bugGroup.TabStop = false;
            this.bugGroup.Text = "Bug Event";
            // 
            // bugLbl
            // 
            this.bugLbl.AutoSize = true;
            this.bugLbl.Location = new System.Drawing.Point(216, 33);
            this.bugLbl.Name = "bugLbl";
            this.bugLbl.Size = new System.Drawing.Size(150, 30);
            this.bugLbl.TabIndex = 156;
            this.bugLbl.Text = "IDs will need to be updated\r\neach time it comes out";
            this.bugLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toggleBugBtn
            // 
            this.toggleBugBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toggleBugBtn.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toggleBugBtn.Location = new System.Drawing.Point(25, 22);
            this.toggleBugBtn.Name = "toggleBugBtn";
            this.toggleBugBtn.Size = new System.Drawing.Size(132, 49);
            this.toggleBugBtn.TabIndex = 2;
            this.toggleBugBtn.Text = "Enable";
            this.toggleBugBtn.UseVisualStyleBackColor = true;
            this.toggleBugBtn.Click += new System.EventHandler(this.toggleBugBtn_Click);
            // 
            // comboTab
            // 
            this.comboTab.Controls.Add(this.legendGroup);
            this.comboTab.Controls.Add(this.comboGroup);
            this.comboTab.Location = new System.Drawing.Point(4, 24);
            this.comboTab.Name = "comboTab";
            this.comboTab.Size = new System.Drawing.Size(842, 516);
            this.comboTab.TabIndex = 9;
            this.comboTab.Text = "Combo";
            this.comboTab.UseVisualStyleBackColor = true;
            // 
            // legendGroup
            // 
            this.legendGroup.Controls.Add(this.legend3Lbl);
            this.legendGroup.Controls.Add(this.legend2Lbl);
            this.legendGroup.Controls.Add(this.exampleComboBtn);
            this.legendGroup.Controls.Add(this.legend1Lbl);
            this.legendGroup.Location = new System.Drawing.Point(21, 390);
            this.legendGroup.Name = "legendGroup";
            this.legendGroup.Size = new System.Drawing.Size(797, 125);
            this.legendGroup.TabIndex = 13;
            this.legendGroup.TabStop = false;
            this.legendGroup.Text = "Legend";
            // 
            // legend3Lbl
            // 
            this.legend3Lbl.AutoSize = true;
            this.legend3Lbl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.legend3Lbl.ForeColor = System.Drawing.Color.Black;
            this.legend3Lbl.Location = new System.Drawing.Point(118, 19);
            this.legend3Lbl.Name = "legend3Lbl";
            this.legend3Lbl.Size = new System.Drawing.Size(232, 100);
            this.legend3Lbl.TabIndex = 196;
            this.legend3Lbl.Text = "Spells:\r\nCast SpellName\r\nCast SpellName 5\r\nCast SpellName on TargetName\r\nCast Spe" +
    "llName 5 on TargetName\r\n";
            this.legend3Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // legend2Lbl
            // 
            this.legend2Lbl.AutoSize = true;
            this.legend2Lbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.legend2Lbl.ForeColor = System.Drawing.Color.Black;
            this.legend2Lbl.Location = new System.Drawing.Point(356, 25);
            this.legend2Lbl.Name = "legend2Lbl";
            this.legend2Lbl.Size = new System.Drawing.Size(228, 90);
            this.legend2Lbl.TabIndex = 195;
            this.legend2Lbl.Text = "Other Stuff:\r\nRemove Staff (immediately changes lines)\r\nRemove Weapon (removes on" +
    "ly weapon)\r\nRemove Shield (removes only shield)\r\nAssail (spacebar assails)\r\nSlee" +
    "p 1000 (pauses for 1000ms) \r\n";
            this.legend2Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // exampleComboBtn
            // 
            this.exampleComboBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exampleComboBtn.ForeColor = System.Drawing.Color.Black;
            this.exampleComboBtn.Location = new System.Drawing.Point(632, 41);
            this.exampleComboBtn.Name = "exampleComboBtn";
            this.exampleComboBtn.Size = new System.Drawing.Size(144, 47);
            this.exampleComboBtn.TabIndex = 194;
            this.exampleComboBtn.Text = "Example Combo";
            this.exampleComboBtn.UseVisualStyleBackColor = true;
            // 
            // legend1Lbl
            // 
            this.legend1Lbl.AutoSize = true;
            this.legend1Lbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.legend1Lbl.ForeColor = System.Drawing.Color.Black;
            this.legend1Lbl.Location = new System.Drawing.Point(11, 19);
            this.legend1Lbl.Name = "legend1Lbl";
            this.legend1Lbl.Size = new System.Drawing.Size(71, 90);
            this.legend1Lbl.TabIndex = 18;
            this.legend1Lbl.Text = "Skills, Items:\r\nSkillName\r\nSkillName 5\r\nItemName\r\nItemSlot 5\r\nSkillSlot 5";
            this.legend1Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboGroup
            // 
            this.comboGroup.Controls.Add(this.comboLbl);
            this.comboGroup.Controls.Add(this.dontCBox4);
            this.comboGroup.Controls.Add(this.combo4Btn);
            this.comboGroup.Controls.Add(this.combo4List);
            this.comboGroup.Controls.Add(this.dontCBox3);
            this.comboGroup.Controls.Add(this.combo3Btn);
            this.comboGroup.Controls.Add(this.combo3List);
            this.comboGroup.Controls.Add(this.dontCBox2);
            this.comboGroup.Controls.Add(this.combo2Btn);
            this.comboGroup.Controls.Add(this.combo2List);
            this.comboGroup.Controls.Add(this.dontCbox1);
            this.comboGroup.Controls.Add(this.combo1Btn);
            this.comboGroup.Controls.Add(this.combo1List);
            this.comboGroup.Location = new System.Drawing.Point(21, 14);
            this.comboGroup.Name = "comboGroup";
            this.comboGroup.Size = new System.Drawing.Size(797, 370);
            this.comboGroup.TabIndex = 12;
            this.comboGroup.TabStop = false;
            this.comboGroup.Text = "Combo Configuration";
            // 
            // comboLbl
            // 
            this.comboLbl.AutoSize = true;
            this.comboLbl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboLbl.ForeColor = System.Drawing.Color.Red;
            this.comboLbl.Location = new System.Drawing.Point(-2, 18);
            this.comboLbl.Name = "comboLbl";
            this.comboLbl.Size = new System.Drawing.Size(799, 20);
            this.comboLbl.TabIndex = 197;
            this.comboLbl.Text = "SET HOTKEYS ON THE OPTIONS PAGE. WHEN YOU PRESS THE KEY IT WILL KNOW WHAT WINDOW " +
    "YOU ARE ON";
            this.comboLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dontCBox4
            // 
            this.dontCBox4.AutoSize = true;
            this.dontCBox4.Checked = true;
            this.dontCBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dontCBox4.Location = new System.Drawing.Point(659, 346);
            this.dontCBox4.Name = "dontCBox4";
            this.dontCBox4.Size = new System.Drawing.Size(90, 19);
            this.dontCBox4.TabIndex = 210;
            this.dontCBox4.Text = "Don\'t Waste";
            this.dontCBox4.UseVisualStyleBackColor = true;
            // 
            // combo4Btn
            // 
            this.combo4Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo4Btn.ForeColor = System.Drawing.Color.Black;
            this.combo4Btn.Location = new System.Drawing.Point(626, 316);
            this.combo4Btn.Name = "combo4Btn";
            this.combo4Btn.Size = new System.Drawing.Size(150, 24);
            this.combo4Btn.TabIndex = 212;
            this.combo4Btn.Text = "Enable Combo Four";
            this.combo4Btn.UseVisualStyleBackColor = true;
            // 
            // combo4List
            // 
            this.combo4List.Location = new System.Drawing.Point(626, 48);
            this.combo4List.Multiline = true;
            this.combo4List.Name = "combo4List";
            this.combo4List.Size = new System.Drawing.Size(150, 262);
            this.combo4List.TabIndex = 211;
            this.combo4List.WordWrap = false;
            // 
            // dontCBox3
            // 
            this.dontCBox3.AutoSize = true;
            this.dontCBox3.Checked = true;
            this.dontCBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dontCBox3.Location = new System.Drawing.Point(464, 346);
            this.dontCBox3.Name = "dontCBox3";
            this.dontCBox3.Size = new System.Drawing.Size(90, 19);
            this.dontCBox3.TabIndex = 204;
            this.dontCBox3.Text = "Don\'t Waste";
            this.dontCBox3.UseVisualStyleBackColor = true;
            // 
            // combo3Btn
            // 
            this.combo3Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo3Btn.ForeColor = System.Drawing.Color.Black;
            this.combo3Btn.Location = new System.Drawing.Point(431, 316);
            this.combo3Btn.Name = "combo3Btn";
            this.combo3Btn.Size = new System.Drawing.Size(150, 24);
            this.combo3Btn.TabIndex = 206;
            this.combo3Btn.Text = "Enable Combo Three";
            this.combo3Btn.UseVisualStyleBackColor = true;
            // 
            // combo3List
            // 
            this.combo3List.Location = new System.Drawing.Point(431, 48);
            this.combo3List.Multiline = true;
            this.combo3List.Name = "combo3List";
            this.combo3List.Size = new System.Drawing.Size(150, 262);
            this.combo3List.TabIndex = 205;
            this.combo3List.WordWrap = false;
            // 
            // dontCBox2
            // 
            this.dontCBox2.AutoSize = true;
            this.dontCBox2.Checked = true;
            this.dontCBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dontCBox2.Location = new System.Drawing.Point(261, 346);
            this.dontCBox2.Name = "dontCBox2";
            this.dontCBox2.Size = new System.Drawing.Size(90, 19);
            this.dontCBox2.TabIndex = 198;
            this.dontCBox2.Text = "Don\'t Waste";
            this.dontCBox2.UseVisualStyleBackColor = true;
            // 
            // combo2Btn
            // 
            this.combo2Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo2Btn.ForeColor = System.Drawing.Color.Black;
            this.combo2Btn.Location = new System.Drawing.Point(229, 316);
            this.combo2Btn.Name = "combo2Btn";
            this.combo2Btn.Size = new System.Drawing.Size(150, 24);
            this.combo2Btn.TabIndex = 200;
            this.combo2Btn.Text = "Enable Combo Two";
            this.combo2Btn.UseVisualStyleBackColor = true;
            // 
            // combo2List
            // 
            this.combo2List.Location = new System.Drawing.Point(229, 48);
            this.combo2List.Multiline = true;
            this.combo2List.Name = "combo2List";
            this.combo2List.Size = new System.Drawing.Size(150, 262);
            this.combo2List.TabIndex = 199;
            this.combo2List.WordWrap = false;
            // 
            // dontCbox1
            // 
            this.dontCbox1.AutoSize = true;
            this.dontCbox1.Checked = true;
            this.dontCbox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dontCbox1.Location = new System.Drawing.Point(58, 346);
            this.dontCbox1.Name = "dontCbox1";
            this.dontCbox1.Size = new System.Drawing.Size(90, 19);
            this.dontCbox1.TabIndex = 158;
            this.dontCbox1.Text = "Don\'t Waste";
            this.dontCbox1.UseVisualStyleBackColor = true;
            // 
            // combo1Btn
            // 
            this.combo1Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.combo1Btn.ForeColor = System.Drawing.Color.Black;
            this.combo1Btn.Location = new System.Drawing.Point(29, 316);
            this.combo1Btn.Name = "combo1Btn";
            this.combo1Btn.Size = new System.Drawing.Size(150, 24);
            this.combo1Btn.TabIndex = 193;
            this.combo1Btn.Text = "Enable Combo One";
            this.combo1Btn.UseVisualStyleBackColor = true;
            // 
            // combo1List
            // 
            this.combo1List.Location = new System.Drawing.Point(29, 48);
            this.combo1List.Multiline = true;
            this.combo1List.Name = "combo1List";
            this.combo1List.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.combo1List.Size = new System.Drawing.Size(150, 262);
            this.combo1List.TabIndex = 192;
            this.combo1List.WordWrap = false;
            // 
            // infoTab
            // 
            this.infoTab.BackColor = System.Drawing.Color.White;
            this.infoTab.Controls.Add(this.mapImageGroup);
            this.infoTab.Controls.Add(this.mappingGroup);
            this.infoTab.Controls.Add(this.dialogGroup);
            this.infoTab.Controls.Add(this.effectGroup);
            this.infoTab.Controls.Add(this.mapToolsGroup);
            this.infoTab.Location = new System.Drawing.Point(4, 24);
            this.infoTab.Name = "infoTab";
            this.infoTab.Size = new System.Drawing.Size(842, 516);
            this.infoTab.TabIndex = 13;
            this.infoTab.Text = "Info";
            // 
            // mapImageGroup
            // 
            this.mapImageGroup.Controls.Add(this.mapImageResizeLbl);
            this.mapImageGroup.Controls.Add(this.resizeMapTBox);
            this.mapImageGroup.Controls.Add(this.saveMapImageCbox);
            this.mapImageGroup.Controls.Add(this.mapImageNameLvl);
            this.mapImageGroup.Controls.Add(this.mapNameRdio);
            this.mapImageGroup.Controls.Add(this.mapNumberRdio);
            this.mapImageGroup.Controls.Add(this.mapImageCbox);
            this.mapImageGroup.Location = new System.Drawing.Point(284, 196);
            this.mapImageGroup.Name = "mapImageGroup";
            this.mapImageGroup.Size = new System.Drawing.Size(487, 121);
            this.mapImageGroup.TabIndex = 127;
            this.mapImageGroup.TabStop = false;
            this.mapImageGroup.Text = "Map Image Generation";
            // 
            // mapImageResizeLbl
            // 
            this.mapImageResizeLbl.AutoSize = true;
            this.mapImageResizeLbl.Location = new System.Drawing.Point(17, 60);
            this.mapImageResizeLbl.Name = "mapImageResizeLbl";
            this.mapImageResizeLbl.Size = new System.Drawing.Size(62, 15);
            this.mapImageResizeLbl.TabIndex = 197;
            this.mapImageResizeLbl.Text = "Resize Pct:";
            // 
            // resizeMapTBox
            // 
            this.resizeMapTBox.Location = new System.Drawing.Point(85, 56);
            this.resizeMapTBox.Name = "resizeMapTBox";
            this.resizeMapTBox.Size = new System.Drawing.Size(44, 23);
            this.resizeMapTBox.TabIndex = 134;
            this.resizeMapTBox.Text = "100";
            this.resizeMapTBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // saveMapImageCbox
            // 
            this.saveMapImageCbox.AutoSize = true;
            this.saveMapImageCbox.ForeColor = System.Drawing.Color.Black;
            this.saveMapImageCbox.Location = new System.Drawing.Point(20, 87);
            this.saveMapImageCbox.Name = "saveMapImageCbox";
            this.saveMapImageCbox.Size = new System.Drawing.Size(173, 19);
            this.saveMapImageCbox.TabIndex = 133;
            this.saveMapImageCbox.Text = "Save Map Files (Map Name)";
            this.saveMapImageCbox.UseVisualStyleBackColor = true;
            // 
            // mapImageNameLvl
            // 
            this.mapImageNameLvl.AutoSize = true;
            this.mapImageNameLvl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mapImageNameLvl.Location = new System.Drawing.Point(306, 22);
            this.mapImageNameLvl.Name = "mapImageNameLvl";
            this.mapImageNameLvl.Size = new System.Drawing.Size(123, 21);
            this.mapImageNameLvl.TabIndex = 132;
            this.mapImageNameLvl.Text = "Naming Format:";
            this.mapImageNameLvl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mapNameRdio
            // 
            this.mapNameRdio.AutoSize = true;
            this.mapNameRdio.Checked = true;
            this.mapNameRdio.Location = new System.Drawing.Point(272, 54);
            this.mapNameRdio.Name = "mapNameRdio";
            this.mapNameRdio.Size = new System.Drawing.Size(84, 19);
            this.mapNameRdio.TabIndex = 9;
            this.mapNameRdio.TabStop = true;
            this.mapNameRdio.Text = "Map Name";
            this.mapNameRdio.UseVisualStyleBackColor = true;
            // 
            // mapNumberRdio
            // 
            this.mapNumberRdio.AutoSize = true;
            this.mapNumberRdio.Location = new System.Drawing.Point(373, 54);
            this.mapNumberRdio.Name = "mapNumberRdio";
            this.mapNumberRdio.Size = new System.Drawing.Size(96, 19);
            this.mapNumberRdio.TabIndex = 8;
            this.mapNumberRdio.Text = "Map Number";
            this.mapNumberRdio.UseVisualStyleBackColor = true;
            // 
            // mapImageCbox
            // 
            this.mapImageCbox.AutoSize = true;
            this.mapImageCbox.ForeColor = System.Drawing.Color.Black;
            this.mapImageCbox.Location = new System.Drawing.Point(20, 32);
            this.mapImageCbox.Name = "mapImageCbox";
            this.mapImageCbox.Size = new System.Drawing.Size(141, 19);
            this.mapImageCbox.TabIndex = 7;
            this.mapImageCbox.Text = "Generate Map Images";
            this.mapImageCbox.UseVisualStyleBackColor = true;
            // 
            // mappingGroup
            // 
            this.mappingGroup.Controls.Add(this.removeWarpLbl);
            this.mappingGroup.Controls.Add(this.changeWarpText);
            this.mappingGroup.Controls.Add(this.addWarpBtn);
            this.mappingGroup.Controls.Add(this.removeWarpBtn);
            this.mappingGroup.Controls.Add(this.chkMappingToggle);
            this.mappingGroup.Location = new System.Drawing.Point(17, 196);
            this.mappingGroup.Name = "mappingGroup";
            this.mappingGroup.Size = new System.Drawing.Size(261, 121);
            this.mappingGroup.TabIndex = 126;
            this.mappingGroup.TabStop = false;
            this.mappingGroup.Text = "Mapping Tools";
            // 
            // removeWarpLbl
            // 
            this.removeWarpLbl.AutoSize = true;
            this.removeWarpLbl.Location = new System.Drawing.Point(7, 21);
            this.removeWarpLbl.Name = "removeWarpLbl";
            this.removeWarpLbl.Size = new System.Drawing.Size(245, 30);
            this.removeWarpLbl.TabIndex = 6;
            this.removeWarpLbl.Text = "Remove: mapId, x, y\r\nAdd: sourceMapId, sX, sY, targetMapId, tX, tY";
            // 
            // changeWarpText
            // 
            this.changeWarpText.Location = new System.Drawing.Point(10, 54);
            this.changeWarpText.Name = "changeWarpText";
            this.changeWarpText.Size = new System.Drawing.Size(148, 23);
            this.changeWarpText.TabIndex = 5;
            // 
            // addWarpBtn
            // 
            this.addWarpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addWarpBtn.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.addWarpBtn.Location = new System.Drawing.Point(10, 83);
            this.addWarpBtn.Name = "addWarpBtn";
            this.addWarpBtn.Size = new System.Drawing.Size(100, 29);
            this.addWarpBtn.TabIndex = 4;
            this.addWarpBtn.Text = "Add Warp";
            this.addWarpBtn.UseVisualStyleBackColor = true;
            this.addWarpBtn.Click += new System.EventHandler(this.addWarpBtn_Click);
            // 
            // removeWarpBtn
            // 
            this.removeWarpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeWarpBtn.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.removeWarpBtn.Location = new System.Drawing.Point(116, 83);
            this.removeWarpBtn.Name = "removeWarpBtn";
            this.removeWarpBtn.Size = new System.Drawing.Size(100, 29);
            this.removeWarpBtn.TabIndex = 3;
            this.removeWarpBtn.Text = "Remove Warp";
            this.removeWarpBtn.UseVisualStyleBackColor = true;
            this.removeWarpBtn.Click += new System.EventHandler(this.removeWarpBtn_Click);
            // 
            // chkMappingToggle
            // 
            this.chkMappingToggle.AutoSize = true;
            this.chkMappingToggle.Enabled = false;
            this.chkMappingToggle.ForeColor = System.Drawing.Color.Black;
            this.chkMappingToggle.Location = new System.Drawing.Point(164, 56);
            this.chkMappingToggle.Name = "chkMappingToggle";
            this.chkMappingToggle.Size = new System.Drawing.Size(74, 19);
            this.chkMappingToggle.TabIndex = 2;
            this.chkMappingToggle.Text = "Mapping";
            this.chkMappingToggle.UseVisualStyleBackColor = true;
            this.chkMappingToggle.CheckedChanged += new System.EventHandler(this.chkMappingToggle_CheckedChanged);
            // 
            // dialogGroup
            // 
            this.dialogGroup.Controls.Add(this.dialogIdLbl);
            this.dialogGroup.Controls.Add(this.pursuitLbl);
            this.dialogGroup.Controls.Add(this.npcText);
            this.dialogGroup.Location = new System.Drawing.Point(257, 18);
            this.dialogGroup.Name = "dialogGroup";
            this.dialogGroup.Size = new System.Drawing.Size(514, 172);
            this.dialogGroup.TabIndex = 9;
            this.dialogGroup.TabStop = false;
            this.dialogGroup.Text = "NPC Information";
            // 
            // dialogIdLbl
            // 
            this.dialogIdLbl.AutoSize = true;
            this.dialogIdLbl.Location = new System.Drawing.Point(322, 94);
            this.dialogIdLbl.Name = "dialogIdLbl";
            this.dialogIdLbl.Size = new System.Drawing.Size(58, 15);
            this.dialogIdLbl.TabIndex = 196;
            this.dialogIdLbl.Text = "Dialog ID:";
            // 
            // pursuitLbl
            // 
            this.pursuitLbl.AutoSize = true;
            this.pursuitLbl.Location = new System.Drawing.Point(322, 33);
            this.pursuitLbl.Name = "pursuitLbl";
            this.pursuitLbl.Size = new System.Drawing.Size(61, 15);
            this.pursuitLbl.TabIndex = 194;
            this.pursuitLbl.Text = "Pursuit ID:";
            // 
            // npcText
            // 
            this.npcText.Location = new System.Drawing.Point(17, 33);
            this.npcText.Multiline = true;
            this.npcText.Name = "npcText";
            this.npcText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.npcText.Size = new System.Drawing.Size(288, 105);
            this.npcText.TabIndex = 193;
            // 
            // effectGroup
            // 
            this.effectGroup.Controls.Add(this.effectBtn);
            this.effectGroup.Controls.Add(this.effectNum);
            this.effectGroup.Location = new System.Drawing.Point(17, 112);
            this.effectGroup.Name = "effectGroup";
            this.effectGroup.Size = new System.Drawing.Size(222, 78);
            this.effectGroup.TabIndex = 8;
            this.effectGroup.TabStop = false;
            this.effectGroup.Text = "Display Spell Effect";
            // 
            // effectBtn
            // 
            this.effectBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.effectBtn.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.effectBtn.Location = new System.Drawing.Point(116, 32);
            this.effectBtn.Name = "effectBtn";
            this.effectBtn.Size = new System.Drawing.Size(100, 29);
            this.effectBtn.TabIndex = 2;
            this.effectBtn.Text = "Show!";
            this.effectBtn.UseVisualStyleBackColor = true;
            this.effectBtn.Click += new System.EventHandler(this.effectBtn_Click);
            // 
            // effectNum
            // 
            this.effectNum.Location = new System.Drawing.Point(31, 36);
            this.effectNum.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.effectNum.Name = "effectNum";
            this.effectNum.Size = new System.Drawing.Size(49, 23);
            this.effectNum.TabIndex = 1;
            // 
            // mapToolsGroup
            // 
            this.mapToolsGroup.Controls.Add(this.spellBarIdsBtn);
            this.mapToolsGroup.Controls.Add(this.mapNodeIdsBtn);
            this.mapToolsGroup.Controls.Add(this.classDetectorBtn);
            this.mapToolsGroup.Controls.Add(this.pursuitIdsBtn);
            this.mapToolsGroup.Location = new System.Drawing.Point(17, 18);
            this.mapToolsGroup.Name = "mapToolsGroup";
            this.mapToolsGroup.Size = new System.Drawing.Size(222, 88);
            this.mapToolsGroup.TabIndex = 7;
            this.mapToolsGroup.TabStop = false;
            this.mapToolsGroup.Text = "ID Tools";
            // 
            // spellBarIdsBtn
            // 
            this.spellBarIdsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spellBarIdsBtn.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.spellBarIdsBtn.Location = new System.Drawing.Point(6, 50);
            this.spellBarIdsBtn.Name = "spellBarIdsBtn";
            this.spellBarIdsBtn.Size = new System.Drawing.Size(100, 29);
            this.spellBarIdsBtn.TabIndex = 0;
            this.spellBarIdsBtn.Text = "Spell Icon IDs";
            this.spellBarIdsBtn.UseVisualStyleBackColor = true;
            this.spellBarIdsBtn.Click += new System.EventHandler(this.effectBarIdsBtn_Click);
            // 
            // mapNodeIdsBtn
            // 
            this.mapNodeIdsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mapNodeIdsBtn.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.mapNodeIdsBtn.Location = new System.Drawing.Point(6, 15);
            this.mapNodeIdsBtn.Name = "mapNodeIdsBtn";
            this.mapNodeIdsBtn.Size = new System.Drawing.Size(100, 29);
            this.mapNodeIdsBtn.TabIndex = 0;
            this.mapNodeIdsBtn.Text = "WorldMap Info";
            this.mapNodeIdsBtn.UseVisualStyleBackColor = true;
            this.mapNodeIdsBtn.Click += new System.EventHandler(this.mapNodeIdsBtn_Click);
            // 
            // classDetectorBtn
            // 
            this.classDetectorBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.classDetectorBtn.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.classDetectorBtn.Location = new System.Drawing.Point(116, 50);
            this.classDetectorBtn.Name = "classDetectorBtn";
            this.classDetectorBtn.Size = new System.Drawing.Size(100, 29);
            this.classDetectorBtn.TabIndex = 124;
            this.classDetectorBtn.Text = "Class Detecter";
            this.classDetectorBtn.UseVisualStyleBackColor = true;
            this.classDetectorBtn.Click += new System.EventHandler(this.classDetectorBtn_Click);
            // 
            // pursuitIdsBtn
            // 
            this.pursuitIdsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pursuitIdsBtn.Font = new System.Drawing.Font("Segoe UI", 7F);
            this.pursuitIdsBtn.Location = new System.Drawing.Point(116, 15);
            this.pursuitIdsBtn.Name = "pursuitIdsBtn";
            this.pursuitIdsBtn.Size = new System.Drawing.Size(100, 29);
            this.pursuitIdsBtn.TabIndex = 0;
            this.pursuitIdsBtn.Text = "Pursuit IDs";
            this.pursuitIdsBtn.UseVisualStyleBackColor = true;
            this.pursuitIdsBtn.Click += new System.EventHandler(this.pursuitIdsBtn_Click);
            // 
            // packetTab
            // 
            this.packetTab.Controls.Add(this.packetPanel);
            this.packetTab.Controls.Add(this.packetBottomPanel);
            this.packetTab.Controls.Add(this.packetStrip);
            this.packetTab.Location = new System.Drawing.Point(4, 24);
            this.packetTab.Name = "packetTab";
            this.packetTab.Size = new System.Drawing.Size(842, 516);
            this.packetTab.TabIndex = 9;
            this.packetTab.Text = "Packet Console";
            this.packetTab.UseVisualStyleBackColor = true;
            // 
            // packetPanel
            // 
            this.packetPanel.Controls.Add(this.packetList);
            this.packetPanel.Controls.Add(this.packetSidePanel);
            this.packetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packetPanel.Location = new System.Drawing.Point(0, 25);
            this.packetPanel.Name = "packetPanel";
            this.packetPanel.Size = new System.Drawing.Size(842, 391);
            this.packetPanel.TabIndex = 0;
            // 
            // packetList
            // 
            this.packetList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packetList.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetList.FormattingEnabled = true;
            this.packetList.ItemHeight = 14;
            this.packetList.Location = new System.Drawing.Point(0, 0);
            this.packetList.Name = "packetList";
            this.packetList.Size = new System.Drawing.Size(642, 391);
            this.packetList.TabIndex = 0;
            this.packetList.SelectedIndexChanged += new System.EventHandler(this.packetList_SelectedIndexChanged);
            this.packetList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.packetList_KeyDown);
            this.packetList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.packetList_MouseDoubleClick);
            // 
            // packetSidePanel
            // 
            this.packetSidePanel.Controls.Add(this.packetHexText);
            this.packetSidePanel.Controls.Add(this.packetCharText);
            this.packetSidePanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.packetSidePanel.Location = new System.Drawing.Point(642, 0);
            this.packetSidePanel.Name = "packetSidePanel";
            this.packetSidePanel.Size = new System.Drawing.Size(200, 391);
            this.packetSidePanel.TabIndex = 0;
            // 
            // packetHexText
            // 
            this.packetHexText.BackColor = System.Drawing.SystemColors.Window;
            this.packetHexText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packetHexText.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetHexText.Location = new System.Drawing.Point(0, 0);
            this.packetHexText.Name = "packetHexText";
            this.packetHexText.ReadOnly = true;
            this.packetHexText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.packetHexText.Size = new System.Drawing.Size(200, 295);
            this.packetHexText.TabIndex = 0;
            this.packetHexText.Text = "";
            // 
            // packetCharText
            // 
            this.packetCharText.BackColor = System.Drawing.SystemColors.Window;
            this.packetCharText.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.packetCharText.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetCharText.Location = new System.Drawing.Point(0, 295);
            this.packetCharText.Name = "packetCharText";
            this.packetCharText.ReadOnly = true;
            this.packetCharText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.packetCharText.Size = new System.Drawing.Size(200, 96);
            this.packetCharText.TabIndex = 0;
            this.packetCharText.Text = "";
            // 
            // packetBottomPanel
            // 
            this.packetBottomPanel.Controls.Add(this.packetInputText);
            this.packetBottomPanel.Controls.Add(this.packetBottomRightPanel);
            this.packetBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.packetBottomPanel.Location = new System.Drawing.Point(0, 416);
            this.packetBottomPanel.Name = "packetBottomPanel";
            this.packetBottomPanel.Size = new System.Drawing.Size(842, 100);
            this.packetBottomPanel.TabIndex = 0;
            // 
            // packetInputText
            // 
            this.packetInputText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packetInputText.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetInputText.Location = new System.Drawing.Point(0, 0);
            this.packetInputText.Name = "packetInputText";
            this.packetInputText.Size = new System.Drawing.Size(642, 100);
            this.packetInputText.TabIndex = 0;
            this.packetInputText.Text = "";
            // 
            // packetBottomRightPanel
            // 
            this.packetBottomRightPanel.Controls.Add(this.sendPacketToClientBtn);
            this.packetBottomRightPanel.Controls.Add(this.sendPacketToServerBtn);
            this.packetBottomRightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.packetBottomRightPanel.Location = new System.Drawing.Point(642, 0);
            this.packetBottomRightPanel.Name = "packetBottomRightPanel";
            this.packetBottomRightPanel.Size = new System.Drawing.Size(200, 100);
            this.packetBottomRightPanel.TabIndex = 0;
            // 
            // sendPacketToClientBtn
            // 
            this.sendPacketToClientBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sendPacketToClientBtn.Location = new System.Drawing.Point(0, 77);
            this.sendPacketToClientBtn.Name = "sendPacketToClientBtn";
            this.sendPacketToClientBtn.Size = new System.Drawing.Size(200, 23);
            this.sendPacketToClientBtn.TabIndex = 0;
            this.sendPacketToClientBtn.Text = "Send to client";
            this.sendPacketToClientBtn.UseVisualStyleBackColor = true;
            this.sendPacketToClientBtn.Click += new System.EventHandler(this.sendPacketToClientBtn_Click);
            // 
            // sendPacketToServerBtn
            // 
            this.sendPacketToServerBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.sendPacketToServerBtn.Location = new System.Drawing.Point(0, 0);
            this.sendPacketToServerBtn.Name = "sendPacketToServerBtn";
            this.sendPacketToServerBtn.Size = new System.Drawing.Size(200, 23);
            this.sendPacketToServerBtn.TabIndex = 0;
            this.sendPacketToServerBtn.Text = "Send to server";
            this.sendPacketToServerBtn.UseVisualStyleBackColor = true;
            this.sendPacketToServerBtn.Click += new System.EventHandler(this.sendPacketToServerBtn_Click);
            // 
            // packetStrip
            // 
            this.packetStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.packetStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.packetStripLbl,
            this.toggleLogSendBtn,
            this.toggleLogRecvBtn,
            this.toolStripSeparator1,
            this.clearPacketLogBtn,
            this.toolStripSeparator2,
            this.toggleDialogBtn});
            this.packetStrip.Location = new System.Drawing.Point(0, 0);
            this.packetStrip.Name = "packetStrip";
            this.packetStrip.Size = new System.Drawing.Size(842, 25);
            this.packetStrip.TabIndex = 0;
            // 
            // packetStripLbl
            // 
            this.packetStripLbl.Name = "packetStripLbl";
            this.packetStripLbl.Size = new System.Drawing.Size(30, 22);
            this.packetStripLbl.Text = "Log:";
            // 
            // toggleLogSendBtn
            // 
            this.toggleLogSendBtn.CheckOnClick = true;
            this.toggleLogSendBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toggleLogSendBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleLogSendBtn.Name = "toggleLogSendBtn";
            this.toggleLogSendBtn.Size = new System.Drawing.Size(37, 22);
            this.toggleLogSendBtn.Text = "Send";
            // 
            // toggleLogRecvBtn
            // 
            this.toggleLogRecvBtn.CheckOnClick = true;
            this.toggleLogRecvBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toggleLogRecvBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleLogRecvBtn.Name = "toggleLogRecvBtn";
            this.toggleLogRecvBtn.Size = new System.Drawing.Size(36, 22);
            this.toggleLogRecvBtn.Text = "Recv";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // clearPacketLogBtn
            // 
            this.clearPacketLogBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.clearPacketLogBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearPacketLogBtn.Name = "clearPacketLogBtn";
            this.clearPacketLogBtn.Size = new System.Drawing.Size(38, 22);
            this.clearPacketLogBtn.Text = "Clear";
            this.clearPacketLogBtn.Click += new System.EventHandler(this.clearPacketLogBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toggleDialogBtn
            // 
            this.toggleDialogBtn.CheckOnClick = true;
            this.toggleDialogBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toggleDialogBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toggleDialogBtn.Name = "toggleDialogBtn";
            this.toggleDialogBtn.Size = new System.Drawing.Size(82, 22);
            this.toggleDialogBtn.Text = "Block Dialogs";
            this.toggleDialogBtn.Click += new System.EventHandler(this.toggleDialogBtn_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.creatureHashListBox);
            this.tabPage1.Controls.Add(this.worldObjectListBox);
            this.tabPage1.Controls.Add(this.targetName);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.spellName);
            this.tabPage1.Controls.Add(this.button13);
            this.tabPage1.Controls.Add(this.button7);
            this.tabPage1.Controls.Add(this.textBox4);
            this.tabPage1.Controls.Add(this.textBox5);
            this.tabPage1.Controls.Add(this.textBox6);
            this.tabPage1.Controls.Add(this.button8);
            this.tabPage1.Controls.Add(this.button9);
            this.tabPage1.Controls.Add(this.button10);
            this.tabPage1.Controls.Add(this.button11);
            this.tabPage1.Controls.Add(this.button12);
            this.tabPage1.ForeColor = System.Drawing.Color.Black;
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(842, 516);
            this.tabPage1.TabIndex = 14;
            this.tabPage1.Text = "DEBUG";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(294, 85);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(117, 23);
            this.button3.TabIndex = 156;
            this.button3.Text = "Route Find Once";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(294, 114);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(117, 23);
            this.button2.TabIndex = 155;
            this.button2.Text = "print map exits";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(413, 260);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 23);
            this.button1.TabIndex = 154;
            this.button1.Text = "RemoveShield";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(208, 320);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 15);
            this.label13.TabIndex = 153;
            this.label13.Text = "Creature IDs";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(34, 320);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(96, 15);
            this.label12.TabIndex = 152;
            this.label12.Text = "World Object IDs";
            // 
            // creatureHashListBox
            // 
            this.creatureHashListBox.FormattingEnabled = true;
            this.creatureHashListBox.ItemHeight = 15;
            this.creatureHashListBox.Location = new System.Drawing.Point(190, 349);
            this.creatureHashListBox.Name = "creatureHashListBox";
            this.creatureHashListBox.Size = new System.Drawing.Size(106, 154);
            this.creatureHashListBox.TabIndex = 151;
            // 
            // worldObjectListBox
            // 
            this.worldObjectListBox.FormattingEnabled = true;
            this.worldObjectListBox.ItemHeight = 15;
            this.worldObjectListBox.Location = new System.Drawing.Point(27, 349);
            this.worldObjectListBox.Name = "worldObjectListBox";
            this.worldObjectListBox.Size = new System.Drawing.Size(103, 154);
            this.worldObjectListBox.TabIndex = 150;
            // 
            // targetName
            // 
            this.targetName.Location = new System.Drawing.Point(251, 261);
            this.targetName.Name = "targetName";
            this.targetName.Size = new System.Drawing.Size(125, 23);
            this.targetName.TabIndex = 149;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(433, 117);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 15);
            this.label11.TabIndex = 148;
            this.label11.Text = "Y";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(433, 88);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 15);
            this.label10.TabIndex = 147;
            this.label10.Text = "X";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(417, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(42, 15);
            this.label9.TabIndex = 146;
            this.label9.Text = "mapID";
            // 
            // spellName
            // 
            this.spellName.Location = new System.Drawing.Point(112, 261);
            this.spellName.Name = "spellName";
            this.spellName.Size = new System.Drawing.Size(100, 23);
            this.spellName.TabIndex = 145;
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(17, 261);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(75, 23);
            this.button13.TabIndex = 144;
            this.button13.Text = "Cast Spell";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(610, 74);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(139, 23);
            this.button7.TabIndex = 143;
            this.button7.Text = "Stop Walking";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(465, 114);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 23);
            this.textBox4.TabIndex = 142;
            this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(465, 85);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 23);
            this.textBox5.TabIndex = 141;
            this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(465, 56);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(100, 23);
            this.textBox6.TabIndex = 140;
            this.textBox6.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(294, 53);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(117, 23);
            this.button8.TabIndex = 139;
            this.button8.Text = "Route Find";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(179, 74);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 138;
            this.button9.Text = "E";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(17, 74);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 23);
            this.button10.TabIndex = 137;
            this.button10.Text = "W";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(98, 90);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(75, 23);
            this.button11.TabIndex = 136;
            this.button11.Text = "S";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(98, 56);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(75, 23);
            this.button12.TabIndex = 135;
            this.button12.Text = "N";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.White;
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startStrip,
            this.clearStrip,
            this.loadStrip,
            this.saveStrip,
            this.deleteStrip,
            this.waypointsMenu,
            this.walkToMenu,
            this.windowOptions});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip.Size = new System.Drawing.Size(850, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // startStrip
            // 
            this.startStrip.ForeColor = System.Drawing.Color.Black;
            this.startStrip.Name = "startStrip";
            this.startStrip.Padding = new System.Windows.Forms.Padding(0);
            this.startStrip.Size = new System.Drawing.Size(35, 24);
            this.startStrip.Text = "Start";
            this.startStrip.Click += new System.EventHandler(this.startStrip_Click_1);
            // 
            // clearStrip
            // 
            this.clearStrip.ForeColor = System.Drawing.Color.Black;
            this.clearStrip.Name = "clearStrip";
            this.clearStrip.Padding = new System.Windows.Forms.Padding(0);
            this.clearStrip.Size = new System.Drawing.Size(38, 24);
            this.clearStrip.Text = "Clear";
            this.clearStrip.Click += new System.EventHandler(this.clearStrip_Click);
            // 
            // loadStrip
            // 
            this.loadStrip.ForeColor = System.Drawing.Color.Black;
            this.loadStrip.Name = "loadStrip";
            this.loadStrip.Padding = new System.Windows.Forms.Padding(0);
            this.loadStrip.Size = new System.Drawing.Size(37, 24);
            this.loadStrip.Text = "Load";
            this.loadStrip.MouseEnter += new System.EventHandler(this.loadStrip_Enter);
            // 
            // saveStrip
            // 
            this.saveStrip.Name = "saveStrip";
            this.saveStrip.Size = new System.Drawing.Size(43, 24);
            this.saveStrip.Text = "Save";
            this.saveStrip.Click += new System.EventHandler(this.saveStrip_Click);
            // 
            // deleteStrip
            // 
            this.deleteStrip.Name = "deleteStrip";
            this.deleteStrip.Size = new System.Drawing.Size(52, 24);
            this.deleteStrip.Text = "Delete";
            this.deleteStrip.MouseEnter += new System.EventHandler(this.deleteStrip_MouseEnter);
            // 
            // waypointsMenu
            // 
            this.waypointsMenu.Name = "waypointsMenu";
            this.waypointsMenu.Size = new System.Drawing.Size(75, 24);
            this.waypointsMenu.Text = "Waypoints";
            this.waypointsMenu.Click += new System.EventHandler(this.waypointsMenu_Click_1);
            // 
            // walkToMenu
            // 
            this.walkToMenu.Name = "walkToMenu";
            this.walkToMenu.Size = new System.Drawing.Size(57, 24);
            this.walkToMenu.Text = "WalkTo";
            this.walkToMenu.Click += new System.EventHandler(this.WalkToMenu_Click);
            // 
            // windowOptions
            // 
            this.windowOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleHideToolStripMenuItem,
            this.smallToolStripMenuItem,
            this.largeToolStripMenuItem,
            this.fullscreenToolStripMenuItem});
            this.windowOptions.Name = "windowOptions";
            this.windowOptions.Size = new System.Drawing.Size(63, 24);
            this.windowOptions.Text = "Window";
            // 
            // toggleHideToolStripMenuItem
            // 
            this.toggleHideToolStripMenuItem.Name = "toggleHideToolStripMenuItem";
            this.toggleHideToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.toggleHideToolStripMenuItem.Text = "Toggle Hide";
            // 
            // smallToolStripMenuItem
            // 
            this.smallToolStripMenuItem.Name = "smallToolStripMenuItem";
            this.smallToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.smallToolStripMenuItem.Text = "Small";
            // 
            // largeToolStripMenuItem
            // 
            this.largeToolStripMenuItem.Name = "largeToolStripMenuItem";
            this.largeToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.largeToolStripMenuItem.Text = "Large";
            // 
            // fullscreenToolStripMenuItem
            // 
            this.fullscreenToolStripMenuItem.Name = "fullscreenToolStripMenuItem";
            this.fullscreenToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.fullscreenToolStripMenuItem.Text = "Fullscreen";
            // 
            // expBonusCooldownTimer
            // 
            this.expBonusCooldownTimer.Interval = 1000;
            this.expBonusCooldownTimer.Tick += new System.EventHandler(this.bonusCooldownTimer_Tick);
            // 
            // mushroomBonusCooldownTimer
            // 
            this.mushroomBonusCooldownTimer.Interval = 1000;
            // 
            // currentAction
            // 
            this.currentAction.Location = new System.Drawing.Point(407, 2);
            this.currentAction.Name = "currentAction";
            this.currentAction.Size = new System.Drawing.Size(377, 18);
            this.currentAction.TabIndex = 6;
            this.currentAction.Text = "Current Action: ";
            this.currentAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chatPanel
            // 
            this.chatPanel.AutoDetectUrls = true;
            this.chatPanel.BackColor = System.Drawing.Color.White;
            this.chatPanel.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatPanel.ForeColor = System.Drawing.Color.Black;
            this.chatPanel.Location = new System.Drawing.Point(121, 253);
            this.chatPanel.Name = "chatPanel";
            this.chatPanel.ReadOnly = true;
            this.chatPanel.Size = new System.Drawing.Size(508, 225);
            this.chatPanel.TabIndex = 0;
            this.chatPanel.Text = "";
            // 
            // ClientTab
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.currentAction);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.clientTabControl);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ClientTab";
            this.Size = new System.Drawing.Size(850, 568);
            this.Load += new System.EventHandler(this.ClientTab_Load);
            this.clientTabControl.ResumeLayout(false);
            this.mainCoverTab.ResumeLayout(false);
            this.mainCoverTab.PerformLayout();
            this.coverMapInfoGrp.ResumeLayout(false);
            this.friendsGroup.ResumeLayout(false);
            this.strangerGroup.ResumeLayout(false);
            this.groupGroup.ResumeLayout(false);
            this.addEnemyGroup.ResumeLayout(false);
            this.addEnemyGroup.PerformLayout();
            this.coverEXPGrp.ResumeLayout(false);
            this.coverEXPGrp.PerformLayout();
            this.mainAislingsTab.ResumeLayout(false);
            this.aislingTabControl.ResumeLayout(false);
            this.selfTab.ResumeLayout(false);
            this.stuffGroup.ResumeLayout(false);
            this.stuffGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLastStepTime)).EndInit();
            this.overrideGroup.ResumeLayout(false);
            this.overrideGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overrideDistanceNum)).EndInit();
            this.alertsGroup.ResumeLayout(false);
            this.alertsGroup.PerformLayout();
            this.usableItemsGroup.ResumeLayout(false);
            this.usableItemsGroup.PerformLayout();
            this.classSpecificGroup.ResumeLayout(false);
            this.classSpecificGroup.PerformLayout();
            this.walkGroup.ResumeLayout(false);
            this.walkGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.walkSpeedSldr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.followDistanceNum)).EndInit();
            this.botOptionsGroup.ResumeLayout(false);
            this.botOptionsGroup.PerformLayout();
            this.dispelsGroup.ResumeLayout(false);
            this.dispelsGroup.PerformLayout();
            this.lootGroup.ResumeLayout(false);
            this.lootGroup.PerformLayout();
            this.dionHealsGroup.ResumeLayout(false);
            this.dionHealsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.healPctNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dionPctNum)).EndInit();
            this.fasAiteGroup.ResumeLayout(false);
            this.fasAiteGroup.PerformLayout();
            this.nearbyAllyTab.ResumeLayout(false);
            this.mainMonstersTab.ResumeLayout(false);
            this.monsterTabControl.ResumeLayout(false);
            this.nearbyEnemyTab.ResumeLayout(false);
            this.bashingTab.ResumeLayout(false);
            this.bashingTab.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ProtectGbx.ResumeLayout(false);
            this.ProtectGbx.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAssitantStray)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPFCounter)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolsTab.ResumeLayout(false);
            this.toolsTab.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numComboImgSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picComboSkill)).EndInit();
            this.resetGroup.ResumeLayout(false);
            this.resetGroup.PerformLayout();
            this.customLinesGroup.ResumeLayout(false);
            this.customLinesGroup.PerformLayout();
            this.dmuGroup.ResumeLayout(false);
            this.dmuGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.acc3ColorNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc3Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc2ColorNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc2Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc1ColorNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.acc1Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shieldNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hairColorNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.weaponNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bootsNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overcoatNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bodyNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.headNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.faceNum)).EndInit();
            this.renamedStaffsGrp.ResumeLayout(false);
            this.renamedStaffsGrp.PerformLayout();
            this.stuffGrp.ResumeLayout(false);
            this.stuffGrp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formNum)).EndInit();
            this.mapFlagsGroup.ResumeLayout(false);
            this.mapFlagsGroup.PerformLayout();
            this.clientHacksGroup.ResumeLayout(false);
            this.clientHacksGroup.PerformLayout();
            this.mainTasksTab.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.TailorGroup.ResumeLayout(false);
            this.TailorGroup.PerformLayout();
            this.ascendGroup.ResumeLayout(false);
            this.ascendGroup.PerformLayout();
            this.prayerGroup.ResumeLayout(false);
            this.prayerGroup.PerformLayout();
            this.itemFindingGB.ResumeLayout(false);
            this.votingGroup.ResumeLayout(false);
            this.votingGroup.PerformLayout();
            this.fishingGroup.ResumeLayout(false);
            this.fishingGroup.PerformLayout();
            this.farmGroup.ResumeLayout(false);
            this.hubaeGroup.ResumeLayout(false);
            this.hubaeGroup.PerformLayout();
            this.laborGroup.ResumeLayout(false);
            this.laborGroup.PerformLayout();
            this.polishGroup.ResumeLayout(false);
            this.polishGroup.PerformLayout();
            this.ascensionGroup.ResumeLayout(false);
            this.ascensionGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tocYNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tocXNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statsNum)).EndInit();
            this.dojoTab.ResumeLayout(false);
            this.dojoTab.PerformLayout();
            this.dojoOptionsGroup.ResumeLayout(false);
            this.dojoOptionsGroup.PerformLayout();
            this.mainEventsTab.ResumeLayout(false);
            this.candyTreatsGroup.ResumeLayout(false);
            this.parchmentMaskGroup.ResumeLayout(false);
            this.scavengerHuntGroup.ResumeLayout(false);
            this.scavengerHuntGroup.PerformLayout();
            this.seasonalDblGroup.ResumeLayout(false);
            this.seasonalDblGroup.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.MAWGroup.ResumeLayout(false);
            this.MAWGroup.PerformLayout();
            this.fowlGroup.ResumeLayout(false);
            this.fowlGroup.PerformLayout();
            this.pigChaseGroup.ResumeLayout(false);
            this.pigChaseGroup.PerformLayout();
            this.yuleGroup.ResumeLayout(false);
            this.yuleGroup.PerformLayout();
            this.bugGroup.ResumeLayout(false);
            this.bugGroup.PerformLayout();
            this.comboTab.ResumeLayout(false);
            this.legendGroup.ResumeLayout(false);
            this.legendGroup.PerformLayout();
            this.comboGroup.ResumeLayout(false);
            this.comboGroup.PerformLayout();
            this.infoTab.ResumeLayout(false);
            this.mapImageGroup.ResumeLayout(false);
            this.mapImageGroup.PerformLayout();
            this.mappingGroup.ResumeLayout(false);
            this.mappingGroup.PerformLayout();
            this.dialogGroup.ResumeLayout(false);
            this.dialogGroup.PerformLayout();
            this.effectGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.effectNum)).EndInit();
            this.mapToolsGroup.ResumeLayout(false);
            this.packetTab.ResumeLayout(false);
            this.packetTab.PerformLayout();
            this.packetPanel.ResumeLayout(false);
            this.packetSidePanel.ResumeLayout(false);
            this.packetBottomPanel.ResumeLayout(false);
            this.packetBottomRightPanel.ResumeLayout(false);
            this.packetStrip.ResumeLayout(false);
            this.packetStrip.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }






        #endregion
        internal TabControl clientTabControl;
        internal TabPage mainCoverTab;
        internal GroupBox coverMapInfoGrp;
        internal Label mapInfoInfoLbl;
        internal Label lastClickedSpriteLbl;
        internal Label mpLbl;
        internal Label hpLbl;
        internal GroupBox friendsGroup;
        internal Button removeFriendBtn;
        internal Button targetFriendBtn;
        internal Button friendAltsBtn;
        internal Button groupFriendBtn;
        internal ListBox friendList;
        internal GroupBox strangerGroup;
        internal Button lastSeenBtn;
        internal Button friendStrangerBtn;
        internal Button targetStrangerBtn;
        internal Button groupStrangerBtn;
        internal ListBox strangerList;
        internal GroupBox groupGroup;
        internal Button friendGroupBtn;
        internal Button groupAltsBtn;
        internal Button kickGroupedBtn;
        internal ListBox groupList;
        internal GroupBox addEnemyGroup;
        internal Button addBossBtn;
        internal Label addMonsterLbl;
        internal Label lastClickedNameLbl;
        internal Label addAislingLbl;
        internal Label lastClickedIDLbl;
        internal Button addAltsBtn;
        internal Button addGroupBtn;
        internal TextBox addAislingText;
        internal Button addAislingBtn;
        internal TextBox addMonsterText;
        internal Button addMonsterBtn;
        internal Button addAllMonstersBtn;
        internal GroupBox coverEXPGrp;
        internal Label expBoxedLbl;
        internal Label goldHourLbl;
        internal Label apHourLbl;
        internal Label expGemsLbl;
        internal Label doublesLbl;
        internal Button useGemBtn;
        internal Button useDoubleBtn;
        internal CheckBox autoGemCbox;
        internal CheckBox autoDoubleCbox;
        internal Button calcResetBtn;
        internal ComboBox expGemsCombox;
        internal ComboBox doublesCombox;
        internal Label expHourLbl;
        internal Label expSessionLbl;
        internal TabPage mainAislingsTab;
        internal TabControl aislingTabControl;
        internal TabPage selfTab;
        private GroupBox stuffGroup;
        internal CheckBox optionsSkullSurrbox;
        internal TextBox safeFSTbox;
        internal CheckBox safeFSCbox;
        internal CheckBox optionsSkullCbox;
        internal GroupBox overrideGroup;
        internal Label overrideDistanceLbl;
        internal NumericUpDown overrideDistanceNum;
        internal Button getSpriteBtn;
        internal CheckBox toggleOverrideCbox;
        internal Button removeOverrideBtn;
        internal Button addOverrideBtn;
        internal TextBox overrideText;
        internal ListBox overrideList;
        internal GroupBox alertsGroup;
        internal CheckBox alertItemCapCbox;
        internal CheckBox alertRangerCbox;
        internal CheckBox alertEXPCbox;
        internal CheckBox alertSkulledCbox;
        internal CheckBox alertDuraCbox;
        internal CheckBox alertStrangerCbox;
        internal GroupBox usableItemsGroup;
        internal CheckBox autoRedCbox;
        internal CheckBox mantidScentCbox;
        internal CheckBox fungusExtractCbox;
        internal GroupBox classSpecificGroup;
        internal TextBox vineText;
        internal ComboBox vineCombox;
        internal CheckBox vineyardCbox;
        internal CheckBox monsterCallCbox;
        internal CheckBox wakeScrollCbox;
        internal CheckBox muscleStimulantCbox;
        internal CheckBox nerveStimulantCbox;
        internal CheckBox dragonScaleCbox;
        internal CheckBox dragonsFireCbox;
        internal CheckBox vanishingElixirCbox;
        internal TextBox fasSpioradText;
        internal CheckBox manaWardCbox;
        internal CheckBox fasSpioradCbox;
        internal CheckBox hideCbox;
        internal CheckBox asgallCbox;
        internal CheckBox armachdCbox;
        internal CheckBox disenchanterCbox;
        internal CheckBox beagCradhCbox;
        internal CheckBox druidFormCbox;
        internal CheckBox regenerationCbox;
        internal CheckBox mistCbox;
        internal CheckBox perfectDefenseCbox;
        internal CheckBox aegisSphereCbox;
        internal GroupBox walkGroup;
        internal CheckBox spamBubbleCbox;
        internal CheckBox bubbleBlockCbox;
        internal CheckBox rangerStopCbox;
        internal Label walkSpeedLbl;
        internal Label walkFastLbl;
        internal Label walkSlowLbl;
        internal TrackBar walkSpeedSldr;
        internal Button walkAllClientsBtn;
        internal Button walkBtn;
        internal ComboBox walkMapCombox;
        internal Label tilesLbl;
        internal NumericUpDown followDistanceNum;
        internal Label distanceLbl;
        internal TextBox followText;
        internal CheckBox followCbox;
        internal GroupBox botOptionsGroup;
        internal CheckBox rangerLogCbox;
        internal CheckBox oneLineWalkCbox;
        internal CheckBox hideLinesCbox;
        internal CheckBox autoStaffCbox;
        internal GroupBox dispelsGroup;
        internal CheckBox aoPoisonCbox;
        internal CheckBox aoCurseCbox;
        internal CheckBox aoSuainCbox;
        internal GroupBox lootGroup;
        internal CheckBox dropTrashCbox;
        internal Button removeTrashBtn;
        internal Button addTrashBtn;
        internal CheckBox pickupItemsCbox;
        internal CheckBox pickupGoldCbox;
        internal Label lootPickupLbl;
        internal Label lootDropLbl;
        internal TextBox addTrashText;
        internal ListBox trashList;
        internal GroupBox dionHealsGroup;
        internal CheckBox aoSithCbox;
        internal CheckBox deireasFaileasCbox;
        internal CheckBox healCbox;
        internal CheckBox dionCbox;
        internal NumericUpDown healPctNum;
        internal ComboBox healCombox;
        internal NumericUpDown dionPctNum;
        internal ComboBox dionWhenCombox;
        internal Label dionWhenLbl;
        internal ComboBox dionCombox;
        internal GroupBox fasAiteGroup;
        internal CheckBox fasCbox;
        internal CheckBox aiteCbox;
        internal ComboBox fasCombox;
        internal ComboBox aiteCombox;
        internal TabPage nearbyAllyTab;
        internal TableLayoutPanel nearbyAllyTable;
        internal TabPage mainMonstersTab;
        internal TabControl monsterTabControl;
        internal TabPage nearbyEnemyTab;
        internal TableLayoutPanel nearbyEnemyTable;
        private TabPage bashingTab;
        private GroupBox groupBox7;
        internal CheckBox chkRandomWaypoints;
        internal Label label5;
        private GroupBox ProtectGbx;
        internal TextBox Protected2Tbx;
        internal CheckBox Protect2Cbx;
        internal TextBox Protected1Tbx;
        internal CheckBox Protect1Cbx;
        private GroupBox groupBox5;
        internal CheckBox chkBashAssails;
        internal Button btnRepairAuto;
        internal CheckBox chkAutoRepairBash;
        internal CheckBox chkCrasher;
        internal CheckBox chkUseSkillsFromRange;
        internal CheckBox ChargeToTargetCbx;
        private GroupBox groupBox4;
        internal Label label4;
        internal NumericUpDown numAssitantStray;
        internal RadioButton radioAssitantStray;
        internal CheckBox assistBasherChk;
        internal RadioButton radioLeaderTarget;
        internal TextBox leadBasherTxt;
        private GroupBox groupBox3;
        internal CheckBox chkFrostStrike;
        internal NumericUpDown numPFCounter;
        internal Label label3;
        private GroupBox groupBox2;
        internal CheckBox chkWaitForCradh;
        internal CheckBox chkWaitForFas;
        internal GroupBox bashingSkillsToUseGrp;
        internal Button btnBashing;
        internal TabPage toolsTab;
        internal GroupBox groupBox9;
        internal ComboBox comboSelector;
        internal NumericUpDown numComboImgSelect;
        private PictureBox picComboSkill;
        internal Button btnAddSkillCombo;
        internal GroupBox resetGroup;
        internal Label resetLbl;
        internal Button btnResetAllStatus;
        internal GroupBox customLinesGroup;
        internal TextBox customLinesBox;
        internal GroupBox dmuGroup;
        internal CheckBox viewDMUCbox;
        internal CheckBox toggleShareCbox;
        internal Button saveDmuBtn;
        internal CheckBox toggleGenderCbox;
        internal CheckBox toggleDmuCbox;
        internal NumericUpDown acc3ColorNum;
        internal NumericUpDown acc3Num;
        internal NumericUpDown acc2ColorNum;
        internal NumericUpDown acc2Num;
        internal NumericUpDown acc1ColorNum;
        internal NumericUpDown acc1Num;
        internal Label hairColorLbl;
        internal NumericUpDown shieldNum;
        internal NumericUpDown hairColorNum;
        internal NumericUpDown weaponNum;
        internal NumericUpDown bootsNum;
        internal NumericUpDown overcoatNum;
        internal NumericUpDown bodyNum;
        internal NumericUpDown headNum;
        internal Label headLbl;
        internal Label shieldLbl;
        internal Label weaponLbl;
        internal Label bootsLbl;
        internal Label armorLbl;
        internal Label acc3ColorLbl;
        internal Label acc2ColorLbl;
        internal Label acc1ColorLbl;
        internal Label acc3Lbl;
        internal Label acc2Lbl;
        internal Label acc1Lbl;
        internal Label bodyLbl;
        internal Label faceLbl;
        internal NumericUpDown faceNum;
        internal GroupBox renamedStaffsGrp;
        internal Button removeRenameBtn;
        internal Label renameLbl;
        internal ListBox renameList;
        internal Button addRenameBtn;
        internal TextBox renameText;
        internal ComboBox renameCombox;
        internal GroupBox stuffGrp;
        internal CheckBox safeScreenCbox;
        internal NumericUpDown formNum;
        internal CheckBox formCbox;
        internal CheckBox unifiedGuildChatCbox;
        internal CheckBox chkTavWallHacks;
        internal CheckBox chkTavWallStranger;
        internal GroupBox mapFlagsGroup;
        internal CheckBox darknessCbox;
        internal CheckBox mapSnowCbox;
        internal CheckBox mapSnowTileCbox;
        internal CheckBox mapTabsCbox;
        internal CheckBox mapFlagsEnableCbox;
        internal GroupBox clientHacksGroup;
        internal CheckBox ghostHackCbox;
        internal CheckBox ignoreCollisionCbox;
        internal CheckBox hideForegroundCbox;
        internal CheckBox mapZoomCbox;
        internal CheckBox seeHiddenCbox;
        internal CheckBox noBlindCbox;
        internal TabPage mainTasksTab;
        internal GroupBox groupBox10;
        internal Button btnCheckLoginTime;
        internal Button btnConLogin;
        internal GroupBox groupBox1;
        internal Button btnFood;
        internal Label label6;
        internal GroupBox TailorGroup;
        internal Button TailorBtn;
        internal Label label2;
        internal GroupBox ascendGroup;
        internal Button ascendAllBtn;
        internal CheckBox deathOptionCbx;
        internal ComboBox ascendOptionCbx;
        internal Button ascendBtn;
        internal GroupBox prayerGroup;
        internal CheckBox prayerAcceptCbox;
        internal CheckBox prayerAssistCbox;
        internal TextBox prayerAssistTbox;
        internal Button togglePrayingBtn;
        internal GroupBox itemFindingGB;
        internal Button btnItemRemove;
        internal Button btnItemAdd;
        internal ListBox listOfItems;
        internal Button btnLoadItemList;
        internal ListBox listToFind;
        internal Button btnItemFinder;
        internal GroupBox votingGroup;
        internal Button voteForAllBtn;
        internal TextBox voteText;
        internal Button toggleVotingBtn;
        internal GroupBox fishingGroup;
        internal ComboBox fishingCombox;
        internal RadioButton keepFishRdio;
        internal RadioButton sellFishRdio;
        internal Button toggleFishingBtn;
        internal GroupBox farmGroup;
        internal Button setNearestBankBtn;
        internal Button returnFarmBtn;
        internal Button setCustomBankBtn;
        internal Button toggleFarmBtn;
        internal GroupBox hubaeGroup;
        internal Button finishedWhiteBtn;
        internal Label teacherLbl;
        internal TextBox teacherText;
        internal Button killedCrabBtn;
        internal Button killedBatBtn;
        internal Button toggleHubaeBtn;
        internal GroupBox laborGroup;
        internal CheckBox fastLaborCbox;
        internal TextBox laborNameText;
        internal Button laborBtn;
        internal GroupBox polishGroup;
        internal CheckBox polishUncutCbox;
        internal Button polishBtn;
        internal CheckBox polishAssistCbox;
        internal TextBox assistText;
        internal Label polishLbl;
        internal GroupBox ascensionGroup;
        internal NumericUpDown tocYNum;
        internal Label tocYLbl;
        internal NumericUpDown tocXNum;
        internal Label tocXLbl;
        internal Label tocLbl;
        internal Label statLbl;
        internal ComboBox statCombox;
        internal NumericUpDown statsNum;
        internal Label statsPerLbl;
        internal Button toggleAscensionBtn;
        internal TabPage dojoTab;
        internal CheckBox flowerCbox;
        internal CheckBox dojo2SpaceCbox;
        internal CheckBox rescueCbox;
        internal TextBox flowerText;
        internal Button dojoRefreshBtn;
        internal CheckBox dojoCounterAttackCbox;
        internal CheckBox dojoRegenerationCbox;
        internal GroupBox unmaxedSpellsGroup;
        internal GroupBox dojoOptionsGroup;
        internal Label autoLbl;
        internal CheckBox dojoAutoStaffCbox;
        internal Button toggleDojoBtn;
        internal CheckBox dojoBonusCbox;
        internal GroupBox unmaxedSkillsGroup;
        internal TabPage mainEventsTab;
        internal GroupBox groupBox8;
        internal Label label8;
        internal Button toggleCatchLeprechaunBtn;
        internal GroupBox MAWGroup;
        internal Label mawLbl;
        internal Button toggleMAWBtn;
        internal GroupBox fowlGroup;
        internal Label fowlLbl;
        internal Button toggleFowlBtn;
        internal GroupBox pigChaseGroup;
        internal Label pigChaseLbl;
        internal Button togglePigChaseBtn;
        internal GroupBox yuleGroup;
        internal Label yuleLbl;
        internal Button toggleYuleBtn;
        internal GroupBox bugGroup;
        internal Label bugLbl;
        internal Button toggleBugBtn;
        internal TabPage comboTab;
        internal GroupBox legendGroup;
        internal Label legend3Lbl;
        internal Label legend2Lbl;
        internal Button exampleComboBtn;
        internal Label legend1Lbl;
        internal GroupBox comboGroup;
        internal Label comboLbl;
        internal CheckBox dontCBox4;
        internal Button combo4Btn;
        internal TextBox combo4List;
        internal CheckBox dontCBox3;
        internal Button combo3Btn;
        internal TextBox combo3List;
        internal CheckBox dontCBox2;
        internal Button combo2Btn;
        internal TextBox combo2List;
        internal CheckBox dontCbox1;
        internal Button combo1Btn;
        internal TextBox combo1List;
        internal MenuStrip menuStrip;
        internal ToolStripMenuItem startStrip;
        internal ToolStripMenuItem clearStrip;
        internal ToolStripMenuItem loadStrip;
        private ToolStripMenuItem saveStrip;
        private ToolStripMenuItem deleteStrip;
        internal ToolStripMenuItem waypointsMenu;
        internal ToolStripMenuItem walkToMenu;
        internal ToolStripMenuItem windowOptions;
        private ToolStripMenuItem toggleHideToolStripMenuItem;
        private ToolStripMenuItem smallToolStripMenuItem;
        private ToolStripMenuItem largeToolStripMenuItem;
        private ToolStripMenuItem fullscreenToolStripMenuItem;
        private TabPage infoTab;
        internal GroupBox mappingGroup;
        internal CheckBox chkMappingToggle;
        internal Label removeWarpLbl;
        internal TextBox changeWarpText;
        internal Button addWarpBtn;
        internal Button removeWarpBtn;
        internal CheckBox mapImageCbox;
        internal GroupBox mapImageGroup;
        internal Label mapImageNameLvl;
        internal RadioButton mapNameRdio;
        internal RadioButton mapNumberRdio;
        internal CheckBox saveMapImageCbox;
        internal TextBox resizeMapTBox;
        internal Label mapImageResizeLbl;
        private TabPage packetTab;
        internal Panel packetPanel;
        internal ListBox packetList;
        internal Panel packetSidePanel;
        internal RichTextBox packetHexText;
        internal RichTextBox packetCharText;
        internal Panel packetBottomPanel;
        internal RichTextBox packetInputText;
        internal Panel packetBottomRightPanel;
        internal Button sendPacketToClientBtn;
        internal Button sendPacketToServerBtn;
        internal ToolStrip packetStrip;
        internal ToolStripLabel packetStripLbl;
        internal ToolStripButton toggleLogSendBtn;
        internal ToolStripButton toggleLogRecvBtn;
        internal ToolStripSeparator toolStripSeparator1;
        internal ToolStripButton clearPacketLogBtn;
        internal ToolStripSeparator toolStripSeparator2;
        internal ToolStripButton toggleDialogBtn;
        internal Button classDetectorBtn;
        internal GroupBox effectGroup;
        internal Button effectBtn;
        internal NumericUpDown effectNum;
        internal GroupBox dialogGroup;
        internal TextBox npcText;
        internal Label pursuitLbl;
        internal Label dialogIdLbl;
        internal GroupBox mapToolsGroup;
        internal Button spellBarIdsBtn;
        internal Button mapNodeIdsBtn;
        internal Button pursuitIdsBtn;
        internal ChatPanel chatPanel;
        internal TabPage tabPage1;
        private TextBox spellName;
        private Button button13;
        private Button button7;
        private TextBox textBox4;
        private TextBox textBox5;
        private TextBox textBox6;
        private Button button8;
        private Button button9;
        private Button button10;
        private Button button11;
        private Button button12;
        private Label label10;
        private Label label9;
        private Label label11;
        private TextBox targetName;
        internal ListBox creatureHashListBox;
        internal ListBox worldObjectListBox;
        private Label label13;
        private Label label12;
        private Button button1;
        private System.Windows.Forms.Timer expBonusCooldownTimer;
        internal Label currentAction;
        private Button button2;
        private Button button3;
        internal CheckBox chkLastStepF5;
        internal CheckBox chkIgnoreDionWaypoints;
        internal CheckBox chkNoCastOffense;
        private Label lblSecs;
        internal CheckBox equipmentrepairCbox;
        internal CheckBox lockstepCbox;
        internal CheckBox chkSpeedStrangers;
        internal NumericUpDown numLastStepTime;
        internal CheckBox deformCbox;
        internal CheckBox autoMushroomCbox;
        internal ComboBox mushroomCombox;
        internal GroupBox seasonalDblGroup;
        internal Label seasonalDblLbl;
        internal Button toggleSeaonalDblBtn;
        internal GroupBox scavengerHuntGroup;
        internal Label scavengerHuntLbl;
        internal Button toggleScavengerHuntBtn;
        internal GroupBox candyTreatsGroup;
        internal Label candyTreatsLbl;
        internal Button toggleCandyTreatsBtn;
        internal GroupBox parchmentMaskGroup;
        internal Label parchmentMaskLbl;
        internal Button toggleParchmentMaskBtn;
    }
}
