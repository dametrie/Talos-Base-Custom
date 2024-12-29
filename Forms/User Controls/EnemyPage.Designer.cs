namespace Talos.Forms
{
    partial class EnemyPage
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
            this.attackComboxTwo = new System.Windows.Forms.ComboBox();
            this.priorityGroup = new System.Windows.Forms.GroupBox();
            this.priorityOnlyCbox = new System.Windows.Forms.CheckBox();
            this.priorityCbox = new System.Windows.Forms.CheckBox();
            this.priorityAddBtn = new System.Windows.Forms.Button();
            this.priorityRemoveBtn = new System.Windows.Forms.Button();
            this.priorityTbox = new System.Windows.Forms.TextBox();
            this.priorityLbox = new System.Windows.Forms.ListBox();
            this.enemySettingsGrp = new System.Windows.Forms.GroupBox();
            this.NearestFirstCbx = new System.Windows.Forms.CheckBox();
            this.settings2 = new System.Windows.Forms.Panel();
            this.fasFirstRbtn = new System.Windows.Forms.RadioButton();
            this.curseFirstRbtn = new System.Windows.Forms.RadioButton();
            this.fasLbl = new System.Windows.Forms.Label();
            this.settings3 = new System.Windows.Forms.Panel();
            this.spellOneRbtn = new System.Windows.Forms.RadioButton();
            this.spellAllRbtn = new System.Windows.Forms.RadioButton();
            this.settings1 = new System.Windows.Forms.Panel();
            this.pramhFirstRbtn = new System.Windows.Forms.RadioButton();
            this.spellFirstRbtn = new System.Windows.Forms.RadioButton();
            this.controlLbl = new System.Windows.Forms.Label();
            this.ignoreGroup = new System.Windows.Forms.GroupBox();
            this.ignoreCbox = new System.Windows.Forms.CheckBox();
            this.ignoreAddBtn = new System.Windows.Forms.Button();
            this.ignoreRemoveBtn = new System.Windows.Forms.Button();
            this.ignoreTbox = new System.Windows.Forms.TextBox();
            this.ignoreLbox = new System.Windows.Forms.ListBox();
            this.expectedNoteLbl = new System.Windows.Forms.Label();
            this.expectedHitsNum = new System.Windows.Forms.NumericUpDown();
            this.expectedHitsLbl = new System.Windows.Forms.Label();
            this.mpndDioned = new System.Windows.Forms.CheckBox();
            this.mpndSilenced = new System.Windows.Forms.CheckBox();
            this.mspgPct = new System.Windows.Forms.CheckBox();
            this.attackCboxTwo = new System.Windows.Forms.CheckBox();
            this.mspgSilenced = new System.Windows.Forms.CheckBox();
            this.attackComboxOne = new System.Windows.Forms.ComboBox();
            this.enemyRemoveBtn = new System.Windows.Forms.Button();
            this.enemyPicture = new System.Windows.Forms.PictureBox();
            this.spellsFasCombox = new System.Windows.Forms.ComboBox();
            this.spellsCurseCombox = new System.Windows.Forms.ComboBox();
            this.enemySpellsGrp = new System.Windows.Forms.GroupBox();
            this.spellsControlCbox = new System.Windows.Forms.CheckBox();
            this.spellsControlCombox = new System.Windows.Forms.ComboBox();
            this.spellsFasCbox = new System.Windows.Forms.CheckBox();
            this.spellsCurseCbox = new System.Windows.Forms.CheckBox();
            this.enemyTargetGrp = new System.Windows.Forms.GroupBox();
            this.targetLbl = new System.Windows.Forms.Label();
            this.targetCursedCbox = new System.Windows.Forms.CheckBox();
            this.targetFassedCbox = new System.Windows.Forms.CheckBox();
            this.targetCombox = new System.Windows.Forms.ComboBox();
            this.targetCbox = new System.Windows.Forms.CheckBox();
            this.enemyAttackGrp = new System.Windows.Forms.GroupBox();
            this.attackCboxOne = new System.Windows.Forms.CheckBox();
            this.FarthestFirstCbx = new System.Windows.Forms.CheckBox();
            this.priorityGroup.SuspendLayout();
            this.enemySettingsGrp.SuspendLayout();
            this.settings2.SuspendLayout();
            this.settings3.SuspendLayout();
            this.settings1.SuspendLayout();
            this.ignoreGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expectedHitsNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.enemyPicture)).BeginInit();
            this.enemySpellsGrp.SuspendLayout();
            this.enemyTargetGrp.SuspendLayout();
            this.enemyAttackGrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // attackComboxTwo
            // 
            this.attackComboxTwo.ForeColor = System.Drawing.Color.Black;
            this.attackComboxTwo.FormattingEnabled = true;
            this.attackComboxTwo.Items.AddRange(new object[] {
            "MSPG",
            "Cursed Tune",
            "Supernova Shot",
            "Volley",
            "Unholy Explosion",
            "M/DSG",
            "Shock Arrow"});
            this.attackComboxTwo.Location = new System.Drawing.Point(48, 48);
            this.attackComboxTwo.Name = "attackComboxTwo";
            this.attackComboxTwo.Size = new System.Drawing.Size(121, 21);
            this.attackComboxTwo.TabIndex = 23;
            this.attackComboxTwo.Text = "MSPG";
            // 
            // priorityGroup
            // 
            this.priorityGroup.Controls.Add(this.priorityOnlyCbox);
            this.priorityGroup.Controls.Add(this.priorityCbox);
            this.priorityGroup.Controls.Add(this.priorityAddBtn);
            this.priorityGroup.Controls.Add(this.priorityRemoveBtn);
            this.priorityGroup.Controls.Add(this.priorityTbox);
            this.priorityGroup.Controls.Add(this.priorityLbox);
            this.priorityGroup.Location = new System.Drawing.Point(140, 86);
            this.priorityGroup.Name = "priorityGroup";
            this.priorityGroup.Size = new System.Drawing.Size(159, 164);
            this.priorityGroup.TabIndex = 38;
            this.priorityGroup.TabStop = false;
            this.priorityGroup.Text = "Priority";
            this.priorityGroup.Visible = false;
            // 
            // priorityOnlyCbox
            // 
            this.priorityOnlyCbox.AutoSize = true;
            this.priorityOnlyCbox.Location = new System.Drawing.Point(81, 44);
            this.priorityOnlyCbox.Name = "priorityOnlyCbox";
            this.priorityOnlyCbox.Size = new System.Drawing.Size(50, 17);
            this.priorityOnlyCbox.TabIndex = 5;
            this.priorityOnlyCbox.Text = "Only";
            this.priorityOnlyCbox.UseVisualStyleBackColor = true;
            // 
            // priorityCbox
            // 
            this.priorityCbox.AutoSize = true;
            this.priorityCbox.Location = new System.Drawing.Point(81, 21);
            this.priorityCbox.Name = "priorityCbox";
            this.priorityCbox.Size = new System.Drawing.Size(71, 17);
            this.priorityCbox.TabIndex = 4;
            this.priorityCbox.Text = "Prioritize";
            this.priorityCbox.UseVisualStyleBackColor = true;
            // 
            // priorityAddBtn
            // 
            this.priorityAddBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.priorityAddBtn.Location = new System.Drawing.Point(81, 135);
            this.priorityAddBtn.Name = "priorityAddBtn";
            this.priorityAddBtn.Size = new System.Drawing.Size(71, 22);
            this.priorityAddBtn.TabIndex = 3;
            this.priorityAddBtn.Text = "Add";
            this.priorityAddBtn.UseVisualStyleBackColor = true;
            this.priorityAddBtn.Click += new System.EventHandler(this.priorityAddBtn_Click);
            // 
            // priorityRemoveBtn
            // 
            this.priorityRemoveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.priorityRemoveBtn.Location = new System.Drawing.Point(81, 83);
            this.priorityRemoveBtn.Name = "priorityRemoveBtn";
            this.priorityRemoveBtn.Size = new System.Drawing.Size(71, 46);
            this.priorityRemoveBtn.TabIndex = 2;
            this.priorityRemoveBtn.Text = "Remove Selected";
            this.priorityRemoveBtn.UseVisualStyleBackColor = true;
            this.priorityRemoveBtn.Click += new System.EventHandler(this.priorityRemoveBtn_Click);
            // 
            // priorityTbox
            // 
            this.priorityTbox.Location = new System.Drawing.Point(6, 135);
            this.priorityTbox.Name = "priorityTbox";
            this.priorityTbox.Size = new System.Drawing.Size(69, 22);
            this.priorityTbox.TabIndex = 1;
            // 
            // priorityLbox
            // 
            this.priorityLbox.FormattingEnabled = true;
            this.priorityLbox.Location = new System.Drawing.Point(6, 21);
            this.priorityLbox.Name = "priorityLbox";
            this.priorityLbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.priorityLbox.Size = new System.Drawing.Size(69, 108);
            this.priorityLbox.TabIndex = 0;
            // 
            // enemySettingsGrp
            // 
            this.enemySettingsGrp.Controls.Add(this.FarthestFirstCbx);
            this.enemySettingsGrp.Controls.Add(this.NearestFirstCbx);
            this.enemySettingsGrp.Controls.Add(this.settings2);
            this.enemySettingsGrp.Controls.Add(this.settings3);
            this.enemySettingsGrp.Controls.Add(this.settings1);
            this.enemySettingsGrp.Location = new System.Drawing.Point(531, 86);
            this.enemySettingsGrp.Name = "enemySettingsGrp";
            this.enemySettingsGrp.Size = new System.Drawing.Size(186, 180);
            this.enemySettingsGrp.TabIndex = 39;
            this.enemySettingsGrp.TabStop = false;
            this.enemySettingsGrp.Text = "Settings";
            // 
            // NearestFirstCbx
            // 
            this.NearestFirstCbx.AutoSize = true;
            this.NearestFirstCbx.Location = new System.Drawing.Point(51, 82);
            this.NearestFirstCbx.Name = "NearestFirstCbx";
            this.NearestFirstCbx.Size = new System.Drawing.Size(90, 17);
            this.NearestFirstCbx.TabIndex = 25;
            this.NearestFirstCbx.Text = "Nearest First";
            this.NearestFirstCbx.UseVisualStyleBackColor = true;
            this.NearestFirstCbx.CheckedChanged += new System.EventHandler(this.NearestFirstCbx_CheckedChanged);
            // 
            // settings2
            // 
            this.settings2.Controls.Add(this.fasFirstRbtn);
            this.settings2.Controls.Add(this.curseFirstRbtn);
            this.settings2.Controls.Add(this.fasLbl);
            this.settings2.Location = new System.Drawing.Point(6, 50);
            this.settings2.Name = "settings2";
            this.settings2.Size = new System.Drawing.Size(174, 30);
            this.settings2.TabIndex = 24;
            // 
            // fasFirstRbtn
            // 
            this.fasFirstRbtn.AutoSize = true;
            this.fasFirstRbtn.Location = new System.Drawing.Point(107, 8);
            this.fasFirstRbtn.Name = "fasFirstRbtn";
            this.fasFirstRbtn.Size = new System.Drawing.Size(42, 17);
            this.fasFirstRbtn.TabIndex = 15;
            this.fasFirstRbtn.Text = "Fas";
            this.fasFirstRbtn.UseVisualStyleBackColor = true;
            // 
            // curseFirstRbtn
            // 
            this.curseFirstRbtn.AutoSize = true;
            this.curseFirstRbtn.Checked = true;
            this.curseFirstRbtn.Location = new System.Drawing.Point(45, 7);
            this.curseFirstRbtn.Name = "curseFirstRbtn";
            this.curseFirstRbtn.Size = new System.Drawing.Size(54, 17);
            this.curseFirstRbtn.TabIndex = 16;
            this.curseFirstRbtn.TabStop = true;
            this.curseFirstRbtn.Text = "Curse";
            this.curseFirstRbtn.UseMnemonic = false;
            this.curseFirstRbtn.UseVisualStyleBackColor = true;
            // 
            // fasLbl
            // 
            this.fasLbl.AutoSize = true;
            this.fasLbl.Location = new System.Drawing.Point(3, 9);
            this.fasLbl.Name = "fasLbl";
            this.fasLbl.Size = new System.Drawing.Size(32, 13);
            this.fasLbl.TabIndex = 15;
            this.fasLbl.Text = "First:";
            // 
            // settings3
            // 
            this.settings3.Controls.Add(this.spellOneRbtn);
            this.settings3.Controls.Add(this.spellAllRbtn);
            this.settings3.Location = new System.Drawing.Point(6, 120);
            this.settings3.Name = "settings3";
            this.settings3.Size = new System.Drawing.Size(174, 54);
            this.settings3.TabIndex = 23;
            // 
            // spellOneRbtn
            // 
            this.spellOneRbtn.AutoSize = true;
            this.spellOneRbtn.Location = new System.Drawing.Point(45, 27);
            this.spellOneRbtn.Name = "spellOneRbtn";
            this.spellOneRbtn.Size = new System.Drawing.Size(94, 17);
            this.spellOneRbtn.TabIndex = 16;
            this.spellOneRbtn.Text = "One at a time";
            this.spellOneRbtn.UseVisualStyleBackColor = true;
            // 
            // spellAllRbtn
            // 
            this.spellAllRbtn.AutoSize = true;
            this.spellAllRbtn.Checked = true;
            this.spellAllRbtn.Location = new System.Drawing.Point(45, 7);
            this.spellAllRbtn.Name = "spellAllRbtn";
            this.spellAllRbtn.Size = new System.Drawing.Size(79, 17);
            this.spellAllRbtn.TabIndex = 15;
            this.spellAllRbtn.TabStop = true;
            this.spellAllRbtn.Text = "All at once";
            this.spellAllRbtn.UseVisualStyleBackColor = true;
            // 
            // settings1
            // 
            this.settings1.Controls.Add(this.pramhFirstRbtn);
            this.settings1.Controls.Add(this.spellFirstRbtn);
            this.settings1.Controls.Add(this.controlLbl);
            this.settings1.Location = new System.Drawing.Point(6, 21);
            this.settings1.Name = "settings1";
            this.settings1.Size = new System.Drawing.Size(174, 29);
            this.settings1.TabIndex = 21;
            // 
            // pramhFirstRbtn
            // 
            this.pramhFirstRbtn.AutoSize = true;
            this.pramhFirstRbtn.Location = new System.Drawing.Point(107, 2);
            this.pramhFirstRbtn.Name = "pramhFirstRbtn";
            this.pramhFirstRbtn.Size = new System.Drawing.Size(57, 17);
            this.pramhFirstRbtn.TabIndex = 19;
            this.pramhFirstRbtn.Text = "Pramh";
            this.pramhFirstRbtn.UseMnemonic = false;
            this.pramhFirstRbtn.UseVisualStyleBackColor = true;
            // 
            // spellFirstRbtn
            // 
            this.spellFirstRbtn.AutoSize = true;
            this.spellFirstRbtn.Checked = true;
            this.spellFirstRbtn.Location = new System.Drawing.Point(45, 2);
            this.spellFirstRbtn.Name = "spellFirstRbtn";
            this.spellFirstRbtn.Size = new System.Drawing.Size(50, 17);
            this.spellFirstRbtn.TabIndex = 18;
            this.spellFirstRbtn.TabStop = true;
            this.spellFirstRbtn.Text = "Spell";
            this.spellFirstRbtn.UseVisualStyleBackColor = true;
            // 
            // controlLbl
            // 
            this.controlLbl.AutoSize = true;
            this.controlLbl.Location = new System.Drawing.Point(3, 4);
            this.controlLbl.Name = "controlLbl";
            this.controlLbl.Size = new System.Drawing.Size(32, 13);
            this.controlLbl.TabIndex = 17;
            this.controlLbl.Text = "First:";
            // 
            // ignoreGroup
            // 
            this.ignoreGroup.Controls.Add(this.ignoreCbox);
            this.ignoreGroup.Controls.Add(this.ignoreAddBtn);
            this.ignoreGroup.Controls.Add(this.ignoreRemoveBtn);
            this.ignoreGroup.Controls.Add(this.ignoreTbox);
            this.ignoreGroup.Controls.Add(this.ignoreLbox);
            this.ignoreGroup.Location = new System.Drawing.Point(140, 272);
            this.ignoreGroup.Name = "ignoreGroup";
            this.ignoreGroup.Size = new System.Drawing.Size(159, 163);
            this.ignoreGroup.TabIndex = 46;
            this.ignoreGroup.TabStop = false;
            this.ignoreGroup.Text = "Ignore";
            this.ignoreGroup.Visible = false;
            // 
            // ignoreCbox
            // 
            this.ignoreCbox.AutoSize = true;
            this.ignoreCbox.Location = new System.Drawing.Point(81, 21);
            this.ignoreCbox.Name = "ignoreCbox";
            this.ignoreCbox.Size = new System.Drawing.Size(60, 17);
            this.ignoreCbox.TabIndex = 10;
            this.ignoreCbox.Text = "Ignore";
            this.ignoreCbox.UseVisualStyleBackColor = true;
            // 
            // ignoreAddBtn
            // 
            this.ignoreAddBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ignoreAddBtn.Location = new System.Drawing.Point(81, 135);
            this.ignoreAddBtn.Name = "ignoreAddBtn";
            this.ignoreAddBtn.Size = new System.Drawing.Size(71, 22);
            this.ignoreAddBtn.TabIndex = 9;
            this.ignoreAddBtn.Text = "Add";
            this.ignoreAddBtn.UseVisualStyleBackColor = true;
            this.ignoreAddBtn.Click += new System.EventHandler(this.ignoreAddBtn_Click);
            // 
            // ignoreRemoveBtn
            // 
            this.ignoreRemoveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ignoreRemoveBtn.Location = new System.Drawing.Point(81, 83);
            this.ignoreRemoveBtn.Name = "ignoreRemoveBtn";
            this.ignoreRemoveBtn.Size = new System.Drawing.Size(71, 46);
            this.ignoreRemoveBtn.TabIndex = 8;
            this.ignoreRemoveBtn.Text = "Remove Selected";
            this.ignoreRemoveBtn.UseVisualStyleBackColor = true;
            this.ignoreRemoveBtn.Click += new System.EventHandler(this.ignoreRemoveBtn_Click);
            // 
            // ignoreTbox
            // 
            this.ignoreTbox.Location = new System.Drawing.Point(6, 135);
            this.ignoreTbox.Name = "ignoreTbox";
            this.ignoreTbox.Size = new System.Drawing.Size(69, 22);
            this.ignoreTbox.TabIndex = 7;
            // 
            // ignoreLbox
            // 
            this.ignoreLbox.FormattingEnabled = true;
            this.ignoreLbox.Location = new System.Drawing.Point(6, 21);
            this.ignoreLbox.Name = "ignoreLbox";
            this.ignoreLbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ignoreLbox.Size = new System.Drawing.Size(69, 108);
            this.ignoreLbox.TabIndex = 6;
            // 
            // expectedNoteLbl
            // 
            this.expectedNoteLbl.AutoSize = true;
            this.expectedNoteLbl.Location = new System.Drawing.Point(415, 385);
            this.expectedNoteLbl.Name = "expectedNoteLbl";
            this.expectedNoteLbl.Size = new System.Drawing.Size(76, 13);
            this.expectedNoteLbl.TabIndex = 45;
            this.expectedNoteLbl.Text = "0 = don\'t use";
            // 
            // expectedHitsNum
            // 
            this.expectedHitsNum.Location = new System.Drawing.Point(451, 357);
            this.expectedHitsNum.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.expectedHitsNum.Name = "expectedHitsNum";
            this.expectedHitsNum.Size = new System.Drawing.Size(40, 22);
            this.expectedHitsNum.TabIndex = 44;
            // 
            // expectedHitsLbl
            // 
            this.expectedHitsLbl.AutoSize = true;
            this.expectedHitsLbl.Location = new System.Drawing.Point(328, 362);
            this.expectedHitsLbl.Name = "expectedHitsLbl";
            this.expectedHitsLbl.Size = new System.Drawing.Size(110, 13);
            this.expectedHitsLbl.TabIndex = 43;
            this.expectedHitsLbl.Text = "Expected hits to kill:";
            // 
            // mpndDioned
            // 
            this.mpndDioned.AutoSize = true;
            this.mpndDioned.Location = new System.Drawing.Point(551, 404);
            this.mpndDioned.Name = "mpndDioned";
            this.mpndDioned.Size = new System.Drawing.Size(146, 17);
            this.mpndDioned.TabIndex = 41;
            this.mpndDioned.Text = "A/M/PND Dioned Mobs";
            this.mpndDioned.UseVisualStyleBackColor = true;
            this.mpndDioned.Visible = false;
            // 
            // mpndSilenced
            // 
            this.mpndSilenced.AutoSize = true;
            this.mpndSilenced.Location = new System.Drawing.Point(551, 358);
            this.mpndSilenced.Name = "mpndSilenced";
            this.mpndSilenced.Size = new System.Drawing.Size(152, 17);
            this.mpndSilenced.TabIndex = 40;
            this.mpndSilenced.Text = "A/M/PND While Silenced";
            this.mpndSilenced.UseVisualStyleBackColor = true;
            // 
            // mspgPct
            // 
            this.mspgPct.AutoSize = true;
            this.mspgPct.Location = new System.Drawing.Point(551, 427);
            this.mspgPct.Name = "mspgPct";
            this.mspgPct.Size = new System.Drawing.Size(173, 17);
            this.mspgPct.TabIndex = 42;
            this.mspgPct.Text = "Only MSPG above 80% Mana";
            this.mspgPct.UseVisualStyleBackColor = true;
            // 
            // attackCboxTwo
            // 
            this.attackCboxTwo.AutoSize = true;
            this.attackCboxTwo.Location = new System.Drawing.Point(20, 51);
            this.attackCboxTwo.Name = "attackCboxTwo";
            this.attackCboxTwo.Size = new System.Drawing.Size(15, 14);
            this.attackCboxTwo.TabIndex = 24;
            this.attackCboxTwo.UseVisualStyleBackColor = true;
            // 
            // mspgSilenced
            // 
            this.mspgSilenced.AutoSize = true;
            this.mspgSilenced.Location = new System.Drawing.Point(551, 381);
            this.mspgSilenced.Name = "mspgSilenced";
            this.mspgSilenced.Size = new System.Drawing.Size(135, 17);
            this.mspgSilenced.TabIndex = 47;
            this.mspgSilenced.Text = "MSPG While Silenced";
            this.mspgSilenced.UseVisualStyleBackColor = true;
            // 
            // attackComboxOne
            // 
            this.attackComboxOne.ForeColor = System.Drawing.Color.Black;
            this.attackComboxOne.FormattingEnabled = true;
            this.attackComboxOne.Items.AddRange(new object[] {
            "Hail of Feathers",
            "Keeter",
            "Groo",
            "Torch",
            "Mermaid",
            "Star Arrow",
            "Barrage",
            "A/M/PND",
            "A/DS",
            "Deception of Life",
            "Dragon Blast",
            "Frost Arrow",
            "lamh"});
            this.attackComboxOne.Location = new System.Drawing.Point(48, 21);
            this.attackComboxOne.Name = "attackComboxOne";
            this.attackComboxOne.Size = new System.Drawing.Size(121, 21);
            this.attackComboxOne.TabIndex = 22;
            this.attackComboxOne.Text = "Hail of Feathers";
            // 
            // enemyRemoveBtn
            // 
            this.enemyRemoveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.enemyRemoveBtn.Location = new System.Drawing.Point(390, 49);
            this.enemyRemoveBtn.Name = "enemyRemoveBtn";
            this.enemyRemoveBtn.Size = new System.Drawing.Size(75, 23);
            this.enemyRemoveBtn.TabIndex = 34;
            this.enemyRemoveBtn.Text = "Remove";
            this.enemyRemoveBtn.UseVisualStyleBackColor = true;
            this.enemyRemoveBtn.Click += new System.EventHandler(this.enemyRemoveBtn_Click);
            // 
            // enemyPicture
            // 
            this.enemyPicture.Location = new System.Drawing.Point(115, 86);
            this.enemyPicture.Name = "enemyPicture";
            this.enemyPicture.Size = new System.Drawing.Size(195, 179);
            this.enemyPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.enemyPicture.TabIndex = 33;
            this.enemyPicture.TabStop = false;
            // 
            // spellsFasCombox
            // 
            this.spellsFasCombox.ForeColor = System.Drawing.Color.Black;
            this.spellsFasCombox.FormattingEnabled = true;
            this.spellsFasCombox.Items.AddRange(new object[] {
            "ard fas nadur",
            "mor fas nadur",
            "fas nadur",
            "beag fas nadur"});
            this.spellsFasCombox.Location = new System.Drawing.Point(48, 83);
            this.spellsFasCombox.Name = "spellsFasCombox";
            this.spellsFasCombox.Size = new System.Drawing.Size(121, 21);
            this.spellsFasCombox.TabIndex = 11;
            this.spellsFasCombox.Text = "ard fas nadur";
            // 
            // spellsCurseCombox
            // 
            this.spellsCurseCombox.ForeColor = System.Drawing.Color.Black;
            this.spellsCurseCombox.FormattingEnabled = true;
            this.spellsCurseCombox.Items.AddRange(new object[] {
            "Demon Seal",
            "Demise",
            "Darker Seal",
            "Dark Seal",
            "ard cradh",
            "mor cradh",
            "cradh",
            "beag cradh"});
            this.spellsCurseCombox.Location = new System.Drawing.Point(48, 33);
            this.spellsCurseCombox.Name = "spellsCurseCombox";
            this.spellsCurseCombox.Size = new System.Drawing.Size(121, 21);
            this.spellsCurseCombox.TabIndex = 10;
            this.spellsCurseCombox.Text = "Demise";
            // 
            // enemySpellsGrp
            // 
            this.enemySpellsGrp.Controls.Add(this.spellsControlCbox);
            this.enemySpellsGrp.Controls.Add(this.spellsFasCombox);
            this.enemySpellsGrp.Controls.Add(this.spellsControlCombox);
            this.enemySpellsGrp.Controls.Add(this.spellsFasCbox);
            this.enemySpellsGrp.Controls.Add(this.spellsCurseCbox);
            this.enemySpellsGrp.Controls.Add(this.spellsCurseCombox);
            this.enemySpellsGrp.Location = new System.Drawing.Point(322, 86);
            this.enemySpellsGrp.Name = "enemySpellsGrp";
            this.enemySpellsGrp.Size = new System.Drawing.Size(186, 180);
            this.enemySpellsGrp.TabIndex = 35;
            this.enemySpellsGrp.TabStop = false;
            this.enemySpellsGrp.Text = "Spells";
            // 
            // spellsControlCbox
            // 
            this.spellsControlCbox.AutoSize = true;
            this.spellsControlCbox.Location = new System.Drawing.Point(20, 136);
            this.spellsControlCbox.Name = "spellsControlCbox";
            this.spellsControlCbox.Size = new System.Drawing.Size(15, 14);
            this.spellsControlCbox.TabIndex = 13;
            this.spellsControlCbox.UseVisualStyleBackColor = true;
            // 
            // spellsControlCombox
            // 
            this.spellsControlCombox.ForeColor = System.Drawing.Color.Black;
            this.spellsControlCombox.FormattingEnabled = true;
            this.spellsControlCombox.Items.AddRange(new object[] {
            "Mesmerize",
            "pramh",
            "suain",
            "beag pramh"});
            this.spellsControlCombox.Location = new System.Drawing.Point(48, 133);
            this.spellsControlCombox.Name = "spellsControlCombox";
            this.spellsControlCombox.Size = new System.Drawing.Size(121, 21);
            this.spellsControlCombox.TabIndex = 11;
            this.spellsControlCombox.Text = "Mesmerize";
            // 
            // spellsFasCbox
            // 
            this.spellsFasCbox.AutoSize = true;
            this.spellsFasCbox.Location = new System.Drawing.Point(20, 86);
            this.spellsFasCbox.Name = "spellsFasCbox";
            this.spellsFasCbox.Size = new System.Drawing.Size(15, 14);
            this.spellsFasCbox.TabIndex = 13;
            this.spellsFasCbox.UseVisualStyleBackColor = true;
            // 
            // spellsCurseCbox
            // 
            this.spellsCurseCbox.AutoSize = true;
            this.spellsCurseCbox.Location = new System.Drawing.Point(20, 36);
            this.spellsCurseCbox.Name = "spellsCurseCbox";
            this.spellsCurseCbox.Size = new System.Drawing.Size(15, 14);
            this.spellsCurseCbox.TabIndex = 12;
            this.spellsCurseCbox.UseVisualStyleBackColor = true;
            // 
            // enemyTargetGrp
            // 
            this.enemyTargetGrp.Controls.Add(this.targetLbl);
            this.enemyTargetGrp.Controls.Add(this.targetCursedCbox);
            this.enemyTargetGrp.Controls.Add(this.targetFassedCbox);
            this.enemyTargetGrp.Controls.Add(this.targetCombox);
            this.enemyTargetGrp.Controls.Add(this.targetCbox);
            this.enemyTargetGrp.Location = new System.Drawing.Point(322, 272);
            this.enemyTargetGrp.Name = "enemyTargetGrp";
            this.enemyTargetGrp.Size = new System.Drawing.Size(186, 80);
            this.enemyTargetGrp.TabIndex = 36;
            this.enemyTargetGrp.TabStop = false;
            this.enemyTargetGrp.Text = "Target";
            // 
            // targetLbl
            // 
            this.targetLbl.AutoSize = true;
            this.targetLbl.Location = new System.Drawing.Point(6, 50);
            this.targetLbl.Name = "targetLbl";
            this.targetLbl.Size = new System.Drawing.Size(42, 13);
            this.targetLbl.TabIndex = 21;
            this.targetLbl.Text = "Status:";
            // 
            // targetCursedCbox
            // 
            this.targetCursedCbox.AutoSize = true;
            this.targetCursedCbox.Location = new System.Drawing.Point(54, 49);
            this.targetCursedCbox.Name = "targetCursedCbox";
            this.targetCursedCbox.Size = new System.Drawing.Size(62, 17);
            this.targetCursedCbox.TabIndex = 20;
            this.targetCursedCbox.Text = "Cursed";
            this.targetCursedCbox.UseVisualStyleBackColor = true;
            // 
            // targetFassedCbox
            // 
            this.targetFassedCbox.AutoSize = true;
            this.targetFassedCbox.Location = new System.Drawing.Point(119, 49);
            this.targetFassedCbox.Name = "targetFassedCbox";
            this.targetFassedCbox.Size = new System.Drawing.Size(61, 17);
            this.targetFassedCbox.TabIndex = 19;
            this.targetFassedCbox.Text = "Fassed";
            this.targetFassedCbox.UseVisualStyleBackColor = true;
            // 
            // targetCombox
            // 
            this.targetCombox.ForeColor = System.Drawing.Color.Black;
            this.targetCombox.FormattingEnabled = true;
            this.targetCombox.Items.AddRange(new object[] {
            "Nearest",
            "Cluster 29",
            "Cluster 13",
            "Cluster 5"});
            this.targetCombox.Location = new System.Drawing.Point(48, 21);
            this.targetCombox.Name = "targetCombox";
            this.targetCombox.Size = new System.Drawing.Size(121, 21);
            this.targetCombox.TabIndex = 20;
            this.targetCombox.Text = "Nearest";
            // 
            // targetCbox
            // 
            this.targetCbox.AutoSize = true;
            this.targetCbox.Location = new System.Drawing.Point(20, 24);
            this.targetCbox.Name = "targetCbox";
            this.targetCbox.Size = new System.Drawing.Size(15, 14);
            this.targetCbox.TabIndex = 19;
            this.targetCbox.UseVisualStyleBackColor = true;
            // 
            // enemyAttackGrp
            // 
            this.enemyAttackGrp.Controls.Add(this.attackCboxTwo);
            this.enemyAttackGrp.Controls.Add(this.attackComboxTwo);
            this.enemyAttackGrp.Controls.Add(this.attackComboxOne);
            this.enemyAttackGrp.Controls.Add(this.attackCboxOne);
            this.enemyAttackGrp.Location = new System.Drawing.Point(531, 272);
            this.enemyAttackGrp.Name = "enemyAttackGrp";
            this.enemyAttackGrp.Size = new System.Drawing.Size(186, 80);
            this.enemyAttackGrp.TabIndex = 37;
            this.enemyAttackGrp.TabStop = false;
            this.enemyAttackGrp.Text = "Attack with";
            // 
            // attackCboxOne
            // 
            this.attackCboxOne.AutoSize = true;
            this.attackCboxOne.Location = new System.Drawing.Point(20, 24);
            this.attackCboxOne.Name = "attackCboxOne";
            this.attackCboxOne.Size = new System.Drawing.Size(15, 14);
            this.attackCboxOne.TabIndex = 21;
            this.attackCboxOne.UseVisualStyleBackColor = true;
            // 
            // FarthestFirstCbx
            // 
            this.FarthestFirstCbx.AutoSize = true;
            this.FarthestFirstCbx.Location = new System.Drawing.Point(51, 102);
            this.FarthestFirstCbx.Name = "FarthestFirstCbx";
            this.FarthestFirstCbx.Size = new System.Drawing.Size(93, 17);
            this.FarthestFirstCbx.TabIndex = 26;
            this.FarthestFirstCbx.Text = "Farthest First";
            this.FarthestFirstCbx.UseVisualStyleBackColor = true;
            this.FarthestFirstCbx.CheckedChanged += new System.EventHandler(this.FarthestFirstCbx_CheckedChanged);
            // 
            // EnemyPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.priorityGroup);
            this.Controls.Add(this.enemySettingsGrp);
            this.Controls.Add(this.ignoreGroup);
            this.Controls.Add(this.expectedNoteLbl);
            this.Controls.Add(this.expectedHitsNum);
            this.Controls.Add(this.expectedHitsLbl);
            this.Controls.Add(this.mpndDioned);
            this.Controls.Add(this.mpndSilenced);
            this.Controls.Add(this.mspgPct);
            this.Controls.Add(this.mspgSilenced);
            this.Controls.Add(this.enemyRemoveBtn);
            this.Controls.Add(this.enemyPicture);
            this.Controls.Add(this.enemySpellsGrp);
            this.Controls.Add(this.enemyTargetGrp);
            this.Controls.Add(this.enemyAttackGrp);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "EnemyPage";
            this.Size = new System.Drawing.Size(838, 492);
            this.priorityGroup.ResumeLayout(false);
            this.priorityGroup.PerformLayout();
            this.enemySettingsGrp.ResumeLayout(false);
            this.enemySettingsGrp.PerformLayout();
            this.settings2.ResumeLayout(false);
            this.settings2.PerformLayout();
            this.settings3.ResumeLayout(false);
            this.settings3.PerformLayout();
            this.settings1.ResumeLayout(false);
            this.settings1.PerformLayout();
            this.ignoreGroup.ResumeLayout(false);
            this.ignoreGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expectedHitsNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.enemyPicture)).EndInit();
            this.enemySpellsGrp.ResumeLayout(false);
            this.enemySpellsGrp.PerformLayout();
            this.enemyTargetGrp.ResumeLayout(false);
            this.enemyTargetGrp.PerformLayout();
            this.enemyAttackGrp.ResumeLayout(false);
            this.enemyAttackGrp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ComboBox attackComboxTwo;
        internal System.Windows.Forms.GroupBox priorityGroup;
        internal System.Windows.Forms.CheckBox priorityOnlyCbox;
        internal System.Windows.Forms.CheckBox priorityCbox;
        internal System.Windows.Forms.Button priorityAddBtn;
        internal System.Windows.Forms.Button priorityRemoveBtn;
        internal System.Windows.Forms.TextBox priorityTbox;
        internal System.Windows.Forms.ListBox priorityLbox;
        private System.Windows.Forms.GroupBox enemySettingsGrp;
        internal System.Windows.Forms.CheckBox NearestFirstCbx;
        private System.Windows.Forms.Panel settings2;
        internal System.Windows.Forms.RadioButton fasFirstRbtn;
        internal System.Windows.Forms.RadioButton curseFirstRbtn;
        internal System.Windows.Forms.Label fasLbl;
        private System.Windows.Forms.Panel settings3;
        internal System.Windows.Forms.RadioButton spellOneRbtn;
        internal System.Windows.Forms.RadioButton spellAllRbtn;
        private System.Windows.Forms.Panel settings1;
        internal System.Windows.Forms.RadioButton pramhFirstRbtn;
        internal System.Windows.Forms.RadioButton spellFirstRbtn;
        internal System.Windows.Forms.Label controlLbl;
        internal System.Windows.Forms.GroupBox ignoreGroup;
        internal System.Windows.Forms.CheckBox ignoreCbox;
        internal System.Windows.Forms.Button ignoreAddBtn;
        internal System.Windows.Forms.Button ignoreRemoveBtn;
        internal System.Windows.Forms.TextBox ignoreTbox;
        internal System.Windows.Forms.ListBox ignoreLbox;
        private System.Windows.Forms.Label expectedNoteLbl;
        internal System.Windows.Forms.NumericUpDown expectedHitsNum;
        private System.Windows.Forms.Label expectedHitsLbl;
        internal System.Windows.Forms.CheckBox mpndDioned;
        internal System.Windows.Forms.CheckBox mpndSilenced;
        internal System.Windows.Forms.CheckBox mspgPct;
        internal System.Windows.Forms.CheckBox attackCboxTwo;
        internal System.Windows.Forms.CheckBox mspgSilenced;
        internal System.Windows.Forms.ComboBox attackComboxOne;
        internal System.Windows.Forms.Button enemyRemoveBtn;
        internal System.Windows.Forms.PictureBox enemyPicture;
        internal System.Windows.Forms.ComboBox spellsFasCombox;
        internal System.Windows.Forms.ComboBox spellsCurseCombox;
        internal System.Windows.Forms.GroupBox enemySpellsGrp;
        internal System.Windows.Forms.CheckBox spellsControlCbox;
        internal System.Windows.Forms.ComboBox spellsControlCombox;
        internal System.Windows.Forms.CheckBox spellsFasCbox;
        internal System.Windows.Forms.CheckBox spellsCurseCbox;
        internal System.Windows.Forms.GroupBox enemyTargetGrp;
        internal System.Windows.Forms.Label targetLbl;
        internal System.Windows.Forms.CheckBox targetCursedCbox;
        internal System.Windows.Forms.CheckBox targetFassedCbox;
        internal System.Windows.Forms.ComboBox targetCombox;
        internal System.Windows.Forms.CheckBox targetCbox;
        internal System.Windows.Forms.GroupBox enemyAttackGrp;
        internal System.Windows.Forms.CheckBox attackCboxOne;
        internal System.Windows.Forms.CheckBox FarthestFirstCbx;
    }
}
