using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace Talos.Options
{
    partial class AutoAscend
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
            this.tabCharSelect = new System.Windows.Forms.TabControl();
            this.tabGroupOne = new System.Windows.Forms.TabPage();
            this.charList1 = new System.Windows.Forms.ListBox();
            this.tabGroupTwo = new System.Windows.Forms.TabPage();
            this.charList2 = new System.Windows.Forms.ListBox();
            this.tabGroupThree = new System.Windows.Forms.TabPage();
            this.charList3 = new System.Windows.Forms.ListBox();
            this.groupBox22 = new System.Windows.Forms.GroupBox();
            this.numMapId = new System.Windows.Forms.NumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.groupBox25 = new System.Windows.Forms.GroupBox();
            this.comboWalk = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboHunt = new System.Windows.Forms.ComboBox();
            this.groupBox24 = new System.Windows.Forms.GroupBox();
            this.comboWaypoint = new System.Windows.Forms.ComboBox();
            this.txtChar = new System.Windows.Forms.TextBox();
            this.removeCharBtn = new System.Windows.Forms.Button();
            this.addCharBtn = new System.Windows.Forms.Button();
            this.chkNotGrouped = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkTriggerAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.manaRadio = new System.Windows.Forms.RadioButton();
            this.hpRadio = new System.Windows.Forms.RadioButton();
            this.chkAutoAscendEnable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabCharSelect.SuspendLayout();
            this.tabGroupOne.SuspendLayout();
            this.tabGroupTwo.SuspendLayout();
            this.tabGroupThree.SuspendLayout();
            this.groupBox22.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMapId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            this.groupBox25.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox24.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabCharSelect
            // 
            this.tabCharSelect.Controls.Add(this.tabGroupOne);
            this.tabCharSelect.Controls.Add(this.tabGroupTwo);
            this.tabCharSelect.Controls.Add(this.tabGroupThree);
            this.tabCharSelect.Location = new System.Drawing.Point(3, 36);
            this.tabCharSelect.Name = "tabCharSelect";
            this.tabCharSelect.SelectedIndex = 0;
            this.tabCharSelect.Size = new System.Drawing.Size(117, 218);
            this.tabCharSelect.TabIndex = 34;
            // 
            // tabGroupOne
            // 
            this.tabGroupOne.Controls.Add(this.charList1);
            this.tabGroupOne.Location = new System.Drawing.Point(4, 22);
            this.tabGroupOne.Name = "tabGroupOne";
            this.tabGroupOne.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabGroupOne.Size = new System.Drawing.Size(109, 192);
            this.tabGroupOne.TabIndex = 0;
            this.tabGroupOne.Text = "Group 1";
            this.tabGroupOne.UseVisualStyleBackColor = true;
            // 
            // charList1
            // 
            this.charList1.BackColor = System.Drawing.Color.White;
            this.charList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charList1.ForeColor = System.Drawing.Color.Black;
            this.charList1.FormattingEnabled = true;
            this.charList1.Location = new System.Drawing.Point(3, 3);
            this.charList1.Name = "charList1";
            this.charList1.Size = new System.Drawing.Size(103, 186);
            this.charList1.Sorted = true;
            this.charList1.TabIndex = 10;
            // 
            // tabGroupTwo
            // 
            this.tabGroupTwo.Controls.Add(this.charList2);
            this.tabGroupTwo.Location = new System.Drawing.Point(4, 22);
            this.tabGroupTwo.Name = "tabGroupTwo";
            this.tabGroupTwo.Size = new System.Drawing.Size(109, 192);
            this.tabGroupTwo.TabIndex = 1;
            this.tabGroupTwo.Text = "Group 2";
            this.tabGroupTwo.UseVisualStyleBackColor = true;
            // 
            // charList2
            // 
            this.charList2.BackColor = System.Drawing.Color.White;
            this.charList2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charList2.ForeColor = System.Drawing.Color.Black;
            this.charList2.FormattingEnabled = true;
            this.charList2.Location = new System.Drawing.Point(0, 0);
            this.charList2.Name = "charList2";
            this.charList2.Size = new System.Drawing.Size(109, 192);
            this.charList2.Sorted = true;
            this.charList2.TabIndex = 11;
            // 
            // tabGroupThree
            // 
            this.tabGroupThree.Controls.Add(this.charList3);
            this.tabGroupThree.Location = new System.Drawing.Point(4, 22);
            this.tabGroupThree.Name = "tabGroupThree";
            this.tabGroupThree.Size = new System.Drawing.Size(109, 192);
            this.tabGroupThree.TabIndex = 2;
            this.tabGroupThree.Text = "Group 3";
            this.tabGroupThree.UseVisualStyleBackColor = true;
            // 
            // charList3
            // 
            this.charList3.BackColor = System.Drawing.Color.White;
            this.charList3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.charList3.ForeColor = System.Drawing.Color.Black;
            this.charList3.FormattingEnabled = true;
            this.charList3.Location = new System.Drawing.Point(0, 0);
            this.charList3.Name = "charList3";
            this.charList3.Size = new System.Drawing.Size(109, 192);
            this.charList3.Sorted = true;
            this.charList3.TabIndex = 11;
            // 
            // groupBox22
            // 
            this.groupBox22.Controls.Add(this.numMapId);
            this.groupBox22.Controls.Add(this.label24);
            this.groupBox22.Controls.Add(this.numX);
            this.groupBox22.Controls.Add(this.label25);
            this.groupBox22.Controls.Add(this.label21);
            this.groupBox22.Controls.Add(this.numY);
            this.groupBox22.Location = new System.Drawing.Point(126, 4);
            this.groupBox22.Name = "groupBox22";
            this.groupBox22.Size = new System.Drawing.Size(219, 79);
            this.groupBox22.TabIndex = 173;
            this.groupBox22.TabStop = false;
            this.groupBox22.Text = "Safe Return";
            // 
            // numMapId
            // 
            this.numMapId.ForeColor = System.Drawing.Color.Black;
            this.numMapId.Location = new System.Drawing.Point(10, 50);
            this.numMapId.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numMapId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMapId.Name = "numMapId";
            this.numMapId.Size = new System.Drawing.Size(53, 20);
            this.numMapId.TabIndex = 158;
            this.numMapId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMapId.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(69, 53);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(14, 13);
            this.label24.TabIndex = 159;
            this.label24.Text = "X";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numX
            // 
            this.numX.ForeColor = System.Drawing.Color.Black;
            this.numX.Location = new System.Drawing.Point(89, 50);
            this.numX.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(43, 20);
            this.numX.TabIndex = 160;
            this.numX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(17, 16);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(165, 26);
            this.label25.TabIndex = 157;
            this.label25.Text = "Map ID and coords to wait safely \r\nfor the rest of your group:";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(138, 53);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(23, 13);
            this.label21.TabIndex = 161;
            this.label21.Text = ",  Y";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numY
            // 
            this.numY.ForeColor = System.Drawing.Color.Black;
            this.numY.Location = new System.Drawing.Point(167, 50);
            this.numY.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(43, 20);
            this.numY.TabIndex = 162;
            this.numY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox25
            // 
            this.groupBox25.Controls.Add(this.comboWalk);
            this.groupBox25.Location = new System.Drawing.Point(126, 89);
            this.groupBox25.Name = "groupBox25";
            this.groupBox25.Size = new System.Drawing.Size(219, 51);
            this.groupBox25.TabIndex = 176;
            this.groupBox25.TabStop = false;
            this.groupBox25.Text = "Profile to load when walking back:";
            // 
            // comboWalk
            // 
            this.comboWalk.ForeColor = System.Drawing.Color.Black;
            this.comboWalk.FormattingEnabled = true;
            this.comboWalk.Location = new System.Drawing.Point(10, 19);
            this.comboWalk.Name = "comboWalk";
            this.comboWalk.Size = new System.Drawing.Size(200, 21);
            this.comboWalk.TabIndex = 169;
            this.comboWalk.Text = "Select a profile...";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboHunt);
            this.groupBox1.Location = new System.Drawing.Point(126, 146);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(219, 51);
            this.groupBox1.TabIndex = 177;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Profile to load when ready to hunt:";
            // 
            // comboHunt
            // 
            this.comboHunt.ForeColor = System.Drawing.Color.Black;
            this.comboHunt.FormattingEnabled = true;
            this.comboHunt.Location = new System.Drawing.Point(10, 19);
            this.comboHunt.Name = "comboHunt";
            this.comboHunt.Size = new System.Drawing.Size(200, 21);
            this.comboHunt.TabIndex = 169;
            this.comboHunt.Text = "Select a profile...";
            // 
            // groupBox24
            // 
            this.groupBox24.Controls.Add(this.comboWaypoint);
            this.groupBox24.Location = new System.Drawing.Point(126, 203);
            this.groupBox24.Name = "groupBox24";
            this.groupBox24.Size = new System.Drawing.Size(219, 49);
            this.groupBox24.TabIndex = 178;
            this.groupBox24.TabStop = false;
            this.groupBox24.Text = "Select waypoints if leader:";
            // 
            // comboWaypoint
            // 
            this.comboWaypoint.ForeColor = System.Drawing.Color.Black;
            this.comboWaypoint.FormattingEnabled = true;
            this.comboWaypoint.Location = new System.Drawing.Point(10, 19);
            this.comboWaypoint.Name = "comboWaypoint";
            this.comboWaypoint.Size = new System.Drawing.Size(200, 21);
            this.comboWaypoint.TabIndex = 171;
            this.comboWaypoint.Text = "Select a waypoint...";
            // 
            // txtChar
            // 
            this.txtChar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChar.Location = new System.Drawing.Point(351, 155);
            this.txtChar.Name = "txtChar";
            this.txtChar.Size = new System.Drawing.Size(112, 20);
            this.txtChar.TabIndex = 181;
            this.txtChar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtChar.TextChanged += new System.EventHandler(this.txtChar_TextChanged);
            // 
            // removeCharBtn
            // 
            this.removeCharBtn.BackColor = System.Drawing.Color.White;
            this.removeCharBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeCharBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeCharBtn.Location = new System.Drawing.Point(351, 215);
            this.removeCharBtn.Name = "removeCharBtn";
            this.removeCharBtn.Size = new System.Drawing.Size(112, 28);
            this.removeCharBtn.TabIndex = 180;
            this.removeCharBtn.Text = "Remove Selected";
            this.removeCharBtn.UseVisualStyleBackColor = false;
            this.removeCharBtn.Click += new System.EventHandler(this.removeCharBtn_Click);
            // 
            // addCharBtn
            // 
            this.addCharBtn.BackColor = System.Drawing.Color.White;
            this.addCharBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addCharBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addCharBtn.Location = new System.Drawing.Point(351, 181);
            this.addCharBtn.Name = "addCharBtn";
            this.addCharBtn.Size = new System.Drawing.Size(112, 28);
            this.addCharBtn.TabIndex = 179;
            this.addCharBtn.Text = "Add";
            this.addCharBtn.UseVisualStyleBackColor = false;
            this.addCharBtn.Click += new System.EventHandler(this.addCharBtn_Click);
            // 
            // chkNotGrouped
            // 
            this.chkNotGrouped.AutoSize = true;
            this.chkNotGrouped.ForeColor = System.Drawing.Color.Black;
            this.chkNotGrouped.Location = new System.Drawing.Point(6, 22);
            this.chkNotGrouped.Name = "chkNotGrouped";
            this.chkNotGrouped.Size = new System.Drawing.Size(87, 17);
            this.chkNotGrouped.TabIndex = 182;
            this.chkNotGrouped.Text = "Not Grouped";
            this.chkNotGrouped.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkTriggerAll);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.manaRadio);
            this.groupBox2.Controls.Add(this.hpRadio);
            this.groupBox2.Controls.Add(this.chkNotGrouped);
            this.groupBox2.Location = new System.Drawing.Point(353, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(110, 125);
            this.groupBox2.TabIndex = 183;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Optional:";
            // 
            // chkTriggerAll
            // 
            this.chkTriggerAll.AutoSize = true;
            this.chkTriggerAll.ForeColor = System.Drawing.Color.Black;
            this.chkTriggerAll.Location = new System.Drawing.Point(6, 44);
            this.chkTriggerAll.Name = "chkTriggerAll";
            this.chkTriggerAll.Size = new System.Drawing.Size(107, 17);
            this.chkTriggerAll.TabIndex = 187;
            this.chkTriggerAll.Text = "Trigger All Clients";
            this.chkTriggerAll.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 185;
            this.label1.Text = "Exp Conversion:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // manaRadio
            // 
            this.manaRadio.AutoSize = true;
            this.manaRadio.Location = new System.Drawing.Point(26, 104);
            this.manaRadio.Name = "manaRadio";
            this.manaRadio.Size = new System.Drawing.Size(52, 17);
            this.manaRadio.TabIndex = 184;
            this.manaRadio.Text = "Mana";
            this.manaRadio.UseVisualStyleBackColor = true;
            // 
            // hpRadio
            // 
            this.hpRadio.AutoSize = true;
            this.hpRadio.Checked = true;
            this.hpRadio.Location = new System.Drawing.Point(26, 81);
            this.hpRadio.Name = "hpRadio";
            this.hpRadio.Size = new System.Drawing.Size(56, 17);
            this.hpRadio.TabIndex = 183;
            this.hpRadio.TabStop = true;
            this.hpRadio.Text = "Health";
            this.hpRadio.UseVisualStyleBackColor = true;
            // 
            // chkAutoAscendEnable
            // 
            this.chkAutoAscendEnable.AutoSize = true;
            this.chkAutoAscendEnable.ForeColor = System.Drawing.Color.Black;
            this.chkAutoAscendEnable.Location = new System.Drawing.Point(33, 4);
            this.chkAutoAscendEnable.Name = "chkAutoAscendEnable";
            this.chkAutoAscendEnable.Size = new System.Drawing.Size(59, 17);
            this.chkAutoAscendEnable.TabIndex = 184;
            this.chkAutoAscendEnable.Text = "Enable";
            this.chkAutoAscendEnable.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(351, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 186;
            this.label2.Text = "Enter Character Name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AutoAscend
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkAutoAscendEnable);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtChar);
            this.Controls.Add(this.removeCharBtn);
            this.Controls.Add(this.addCharBtn);
            this.Controls.Add(this.groupBox24);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox25);
            this.Controls.Add(this.groupBox22);
            this.Controls.Add(this.tabCharSelect);
            this.Name = "AutoAscend";
            this.Size = new System.Drawing.Size(509, 258);
            this.Load += new System.EventHandler(this.AutoAscend_Load);
            this.tabCharSelect.ResumeLayout(false);
            this.tabGroupOne.ResumeLayout(false);
            this.tabGroupTwo.ResumeLayout(false);
            this.tabGroupThree.ResumeLayout(false);
            this.groupBox22.ResumeLayout(false);
            this.groupBox22.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMapId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            this.groupBox25.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox24.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        private TabControl tabCharSelect;
        private TabPage tabGroupOne;
        public ListBox charList1;
        internal GroupBox groupBox22;
        internal NumericUpDown numMapId;
        internal Label label24;
        internal NumericUpDown numX;
        internal Label label25;
        internal Label label21;
        internal NumericUpDown numY;
        internal GroupBox groupBox25;
        internal ComboBox comboWalk;
        internal GroupBox groupBox1;
        internal ComboBox comboHunt;
        internal GroupBox groupBox24;
        internal ComboBox comboWaypoint;
        private TextBox txtChar;
        private Button removeCharBtn;
        private Button addCharBtn;
        internal CheckBox chkNotGrouped;
        internal GroupBox groupBox2;
        internal RadioButton manaRadio;
        internal RadioButton hpRadio;
        internal Label label1;
        public CheckBox chkAutoAscendEnable;
        internal Label label2;
        public CheckBox chkTriggerAll;
        private TabPage tabGroupTwo;
        public ListBox charList2;
        private TabPage tabGroupThree;
        public ListBox charList3;
        #endregion
    }
}
