using System;

namespace Talos.Options
{
    partial class GeneralPage
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
            this.enableKom = new System.Windows.Forms.CheckBox();
            this.paranoiaCbox = new System.Windows.Forms.CheckBox();
            this.logOnStartup = new System.Windows.Forms.CheckBox();
            this.enableOverlay = new System.Windows.Forms.CheckBox();
            this.useDawnd = new System.Windows.Forms.CheckBox();
            this.removeSpamCbox = new System.Windows.Forms.CheckBox();
            this.daTransVal = new System.Windows.Forms.Label();
            this.daOpacityLbl = new System.Windows.Forms.Label();
            this.daOpacitySldr = new System.Windows.Forms.TrackBar();
            this.botTransVal = new System.Windows.Forms.Label();
            this.botOpacityLbl = new System.Windows.Forms.Label();
            this.botOpacitySldr = new System.Windows.Forms.TrackBar();
            this.whisperSound = new System.Windows.Forms.CheckBox();
            this.whisperFlash = new System.Windows.Forms.CheckBox();
            this.fullWindowOpt = new System.Windows.Forms.RadioButton();
            this.largeWindowOpt = new System.Windows.Forms.RadioButton();
            this.smallWindowOpt = new System.Windows.Forms.RadioButton();
            this.dataPathButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dataPath = new System.Windows.Forms.TextBox();
            this.darkAgesPathButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.darkAgesPath = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.daOpacitySldr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.botOpacitySldr)).BeginInit();
            this.SuspendLayout();
            // 
            // enableKom
            // 
            this.enableKom.AutoSize = true;
            this.enableKom.Checked = true;
            this.enableKom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableKom.Location = new System.Drawing.Point(28, 236);
            this.enableKom.Name = "enableKom";
            this.enableKom.Size = new System.Drawing.Size(111, 19);
            this.enableKom.TabIndex = 57;
            this.enableKom.Text = "Auto Buy Kom\'s";
            this.enableKom.UseVisualStyleBackColor = true;
            // 
            // paranoiaCbox
            // 
            this.paranoiaCbox.AutoSize = true;
            this.paranoiaCbox.Location = new System.Drawing.Point(255, 199);
            this.paranoiaCbox.Name = "paranoiaCbox";
            this.paranoiaCbox.Size = new System.Drawing.Size(203, 19);
            this.paranoiaCbox.TabIndex = 56;
            this.paranoiaCbox.Text = "Paranoid mode (stranger=ranger)";
            this.paranoiaCbox.UseVisualStyleBackColor = true;
            this.paranoiaCbox.CheckedChanged += new EventHandler(this.paranoiaCbox_CheckedChanged);
            // 
            // logOnStartup
            // 
            this.logOnStartup.AutoSize = true;
            this.logOnStartup.Location = new System.Drawing.Point(255, 225);
            this.logOnStartup.Name = "logOnStartup";
            this.logOnStartup.Size = new System.Drawing.Size(162, 19);
            this.logOnStartup.TabIndex = 55;
            this.logOnStartup.Text = "Enable logging on startup";
            this.logOnStartup.UseVisualStyleBackColor = true;
            // 
            // enableOverlay
            // 
            this.enableOverlay.AutoSize = true;
            this.enableOverlay.Location = new System.Drawing.Point(28, 174);
            this.enableOverlay.Name = "enableOverlay";
            this.enableOverlay.Size = new System.Drawing.Size(104, 19);
            this.enableOverlay.TabIndex = 54;
            this.enableOverlay.Text = "Enable Overlay";
            this.enableOverlay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.enableOverlay.UseVisualStyleBackColor = true;
            // 
            // useDawnd
            // 
            this.useDawnd.AutoSize = true;
            this.useDawnd.Checked = true;
            this.useDawnd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useDawnd.Location = new System.Drawing.Point(28, 149);
            this.useDawnd.Name = "useDawnd";
            this.useDawnd.Size = new System.Drawing.Size(95, 19);
            this.useDawnd.TabIndex = 53;
            this.useDawnd.Text = "Inject Dawnd";
            this.useDawnd.UseVisualStyleBackColor = true;
            // 
            // removeSpamCbox
            // 
            this.removeSpamCbox.AutoSize = true;
            this.removeSpamCbox.Location = new System.Drawing.Point(28, 199);
            this.removeSpamCbox.Name = "removeSpamCbox";
            this.removeSpamCbox.Size = new System.Drawing.Size(186, 34);
            this.removeSpamCbox.TabIndex = 52;
            this.removeSpamCbox.Text = "Remove commonly spammed\r\nspells from orange bar";
            this.removeSpamCbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.removeSpamCbox.UseVisualStyleBackColor = true;
            // 
            // daTransVal
            // 
            this.daTransVal.AutoSize = true;
            this.daTransVal.Location = new System.Drawing.Point(354, 222);
            this.daTransVal.Name = "daTransVal";
            this.daTransVal.Size = new System.Drawing.Size(0, 15);
            this.daTransVal.TabIndex = 51;
            // 
            // daOpacityLbl
            // 
            this.daOpacityLbl.AutoSize = true;
            this.daOpacityLbl.Location = new System.Drawing.Point(243, 164);
            this.daOpacityLbl.Name = "daOpacityLbl";
            this.daOpacityLbl.Size = new System.Drawing.Size(67, 15);
            this.daOpacityLbl.TabIndex = 50;
            this.daOpacityLbl.Text = "DA Opacity";
            // 
            // daOpacitySldr
            // 
            this.daOpacitySldr.AutoSize = false;
            this.daOpacitySldr.LargeChange = 20;
            this.daOpacitySldr.Location = new System.Drawing.Point(316, 149);
            this.daOpacitySldr.Maximum = 100;
            this.daOpacitySldr.Minimum = 20;
            this.daOpacitySldr.Name = "daOpacitySldr";
            this.daOpacitySldr.Size = new System.Drawing.Size(156, 44);
            this.daOpacitySldr.TabIndex = 49;
            this.daOpacitySldr.TickFrequency = 10;
            this.daOpacitySldr.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.daOpacitySldr.Value = 100;
            this.daOpacitySldr.Scroll += new EventHandler(this.daOpacitySldr_Scroll);

            // 
            // botTransVal
            // 
            this.botTransVal.AutoSize = true;
            this.botTransVal.Location = new System.Drawing.Point(377, 157);
            this.botTransVal.Name = "botTransVal";
            this.botTransVal.Size = new System.Drawing.Size(0, 15);
            this.botTransVal.TabIndex = 48;
            // 
            // botOpacityLbl
            // 
            this.botOpacityLbl.AutoSize = true;
            this.botOpacityLbl.Location = new System.Drawing.Point(241, 125);
            this.botOpacityLbl.Name = "botOpacityLbl";
            this.botOpacityLbl.Size = new System.Drawing.Size(69, 15);
            this.botOpacityLbl.TabIndex = 47;
            this.botOpacityLbl.Text = "Bot Opacity";
            // 
            // botOpacitySldr
            // 
            this.botOpacitySldr.AutoSize = false;
            this.botOpacitySldr.LargeChange = 20;
            this.botOpacitySldr.Location = new System.Drawing.Point(316, 110);
            this.botOpacitySldr.Maximum = 100;
            this.botOpacitySldr.Minimum = 20;
            this.botOpacitySldr.Name = "botOpacitySldr";
            this.botOpacitySldr.Size = new System.Drawing.Size(156, 44);
            this.botOpacitySldr.TabIndex = 46;
            this.botOpacitySldr.TickFrequency = 10;
            this.botOpacitySldr.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.botOpacitySldr.Value = 100;
            this.botOpacitySldr.Scroll += new EventHandler(this.botOpacitySldr_Scroll);

            // 
            // whisperSound
            // 
            this.whisperSound.AutoSize = true;
            this.whisperSound.Checked = true;
            this.whisperSound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.whisperSound.Location = new System.Drawing.Point(28, 99);
            this.whisperSound.Name = "whisperSound";
            this.whisperSound.Size = new System.Drawing.Size(172, 19);
            this.whisperSound.TabIndex = 45;
            this.whisperSound.Text = "Whisper Notification Sound";
            this.whisperSound.UseVisualStyleBackColor = true;
            // 
            // whisperFlash
            // 
            this.whisperFlash.AutoSize = true;
            this.whisperFlash.Checked = true;
            this.whisperFlash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.whisperFlash.Location = new System.Drawing.Point(28, 124);
            this.whisperFlash.Name = "whisperFlash";
            this.whisperFlash.Size = new System.Drawing.Size(146, 19);
            this.whisperFlash.TabIndex = 44;
            this.whisperFlash.Text = "Whisper Window Flash";
            this.whisperFlash.UseVisualStyleBackColor = true;
            // 
            // fullWindowOpt
            // 
            this.fullWindowOpt.AutoSize = true;
            this.fullWindowOpt.Location = new System.Drawing.Point(334, 74);
            this.fullWindowOpt.Name = "fullWindowOpt";
            this.fullWindowOpt.Size = new System.Drawing.Size(138, 19);
            this.fullWindowOpt.TabIndex = 43;
            this.fullWindowOpt.Text = "Windowed Fullscreen";
            this.fullWindowOpt.UseVisualStyleBackColor = true;
            // 
            // largeWindowOpt
            // 
            this.largeWindowOpt.AutoSize = true;
            this.largeWindowOpt.Location = new System.Drawing.Point(177, 74);
            this.largeWindowOpt.Name = "largeWindowOpt";
            this.largeWindowOpt.Size = new System.Drawing.Size(113, 19);
            this.largeWindowOpt.TabIndex = 42;
            this.largeWindowOpt.Text = "1280x960 (Large)";
            this.largeWindowOpt.UseVisualStyleBackColor = true;
            // 
            // smallWindowOpt
            // 
            this.smallWindowOpt.AutoSize = true;
            this.smallWindowOpt.BackColor = System.Drawing.SystemColors.Control;
            this.smallWindowOpt.Checked = true;
            this.smallWindowOpt.Location = new System.Drawing.Point(28, 74);
            this.smallWindowOpt.Name = "smallWindowOpt";
            this.smallWindowOpt.Size = new System.Drawing.Size(107, 19);
            this.smallWindowOpt.TabIndex = 41;
            this.smallWindowOpt.TabStop = true;
            this.smallWindowOpt.Text = "640x480 (Small)";
            this.smallWindowOpt.UseVisualStyleBackColor = false;
            // 
            // dataPathButton
            // 
            this.dataPathButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dataPathButton.Location = new System.Drawing.Point(412, 32);
            this.dataPathButton.Name = "dataPathButton";
            this.dataPathButton.Size = new System.Drawing.Size(75, 23);
            this.dataPathButton.TabIndex = 40;
            this.dataPathButton.Text = "Browse";
            this.dataPathButton.UseVisualStyleBackColor = true;
            this.darkAgesPathButton.Click += new EventHandler(this.darkAgesPathButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.TabIndex = 39;
            this.label2.Text = "Data Path";
            // 
            // dataPath
            // 
            this.dataPath.BackColor = System.Drawing.Color.White;
            this.dataPath.ForeColor = System.Drawing.Color.Black;
            this.dataPath.Location = new System.Drawing.Point(96, 32);
            this.dataPath.Name = "dataPath";
            this.dataPath.Size = new System.Drawing.Size(310, 23);
            this.dataPath.TabIndex = 38;
            // 
            // darkAgesPathButton
            // 
            this.darkAgesPathButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.darkAgesPathButton.Location = new System.Drawing.Point(412, 3);
            this.darkAgesPathButton.Name = "darkAgesPathButton";
            this.darkAgesPathButton.Size = new System.Drawing.Size(75, 23);
            this.darkAgesPathButton.TabIndex = 37;
            this.darkAgesPathButton.Text = "Browse";
            this.darkAgesPathButton.UseVisualStyleBackColor = true;
            this.dataPathButton.Click += new EventHandler(this.dataPathButton_Click);

            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 15);
            this.label1.TabIndex = 36;
            this.label1.Text = "Dark Ages Path";
            // 
            // darkAgesPath
            // 
            this.darkAgesPath.BackColor = System.Drawing.Color.White;
            this.darkAgesPath.ForeColor = System.Drawing.Color.Black;
            this.darkAgesPath.Location = new System.Drawing.Point(96, 3);
            this.darkAgesPath.Name = "darkAgesPath";
            this.darkAgesPath.Size = new System.Drawing.Size(310, 23);
            this.darkAgesPath.TabIndex = 35;
            // 
            // GeneralPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.enableKom);
            this.Controls.Add(this.paranoiaCbox);
            this.Controls.Add(this.logOnStartup);
            this.Controls.Add(this.enableOverlay);
            this.Controls.Add(this.useDawnd);
            this.Controls.Add(this.removeSpamCbox);
            this.Controls.Add(this.daTransVal);
            this.Controls.Add(this.daOpacityLbl);
            this.Controls.Add(this.daOpacitySldr);
            this.Controls.Add(this.botTransVal);
            this.Controls.Add(this.botOpacityLbl);
            this.Controls.Add(this.botOpacitySldr);
            this.Controls.Add(this.whisperSound);
            this.Controls.Add(this.whisperFlash);
            this.Controls.Add(this.fullWindowOpt);
            this.Controls.Add(this.largeWindowOpt);
            this.Controls.Add(this.smallWindowOpt);
            this.Controls.Add(this.dataPathButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataPath);
            this.Controls.Add(this.darkAgesPathButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.darkAgesPath);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "GeneralPage";
            this.Size = new System.Drawing.Size(490, 258);
            ((System.ComponentModel.ISupportInitialize)(this.daOpacitySldr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.botOpacitySldr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox enableKom;
        internal System.Windows.Forms.CheckBox paranoiaCbox;
        private System.Windows.Forms.CheckBox logOnStartup;
        private System.Windows.Forms.CheckBox enableOverlay;
        private System.Windows.Forms.CheckBox useDawnd;
        private System.Windows.Forms.CheckBox removeSpamCbox;
        private System.Windows.Forms.Label daTransVal;
        private System.Windows.Forms.Label daOpacityLbl;
        internal System.Windows.Forms.TrackBar daOpacitySldr;
        private System.Windows.Forms.Label botTransVal;
        private System.Windows.Forms.Label botOpacityLbl;
        internal System.Windows.Forms.TrackBar botOpacitySldr;
        private System.Windows.Forms.CheckBox whisperSound;
        private System.Windows.Forms.CheckBox whisperFlash;
        private System.Windows.Forms.RadioButton fullWindowOpt;
        private System.Windows.Forms.RadioButton largeWindowOpt;
        private System.Windows.Forms.RadioButton smallWindowOpt;
        private System.Windows.Forms.Button dataPathButton;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox dataPath;
        private System.Windows.Forms.Button darkAgesPathButton;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox darkAgesPath;
    }
}
