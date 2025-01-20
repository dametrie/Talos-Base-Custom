namespace Talos.Options
{
    partial class SpriteOverride
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
            this.removeOverrideBtn = new System.Windows.Forms.Button();
            this.normalSpritesRBtn = new System.Windows.Forms.RadioButton();
            this.disableAllRBtn = new System.Windows.Forms.RadioButton();
            this.enableOverrideRBtn = new System.Windows.Forms.RadioButton();
            this.spriteOverBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.spriteNewNum = new System.Windows.Forms.NumericUpDown();
            this.spriteOldNum = new System.Windows.Forms.NumericUpDown();
            this.effectLbox = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.spriteNewNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spriteOldNum)).BeginInit();
            this.SuspendLayout();
            // 
            // removeOverrideBtn
            // 
            this.removeOverrideBtn.BackColor = System.Drawing.Color.White;
            this.removeOverrideBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeOverrideBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeOverrideBtn.Location = new System.Drawing.Point(231, 164);
            this.removeOverrideBtn.Name = "removeOverrideBtn";
            this.removeOverrideBtn.Size = new System.Drawing.Size(118, 65);
            this.removeOverrideBtn.TabIndex = 19;
            this.removeOverrideBtn.Text = "Remove Selected";
            this.removeOverrideBtn.UseVisualStyleBackColor = false;
            this.removeOverrideBtn.Click += new System.EventHandler(this.removeOverrideBtn_Click);
            // 
            // normalSpritesRBtn
            // 
            this.normalSpritesRBtn.AutoSize = true;
            this.normalSpritesRBtn.BackColor = System.Drawing.SystemColors.Control;
            this.normalSpritesRBtn.Checked = true;
            this.normalSpritesRBtn.Location = new System.Drawing.Point(39, 173);
            this.normalSpritesRBtn.Name = "normalSpritesRBtn";
            this.normalSpritesRBtn.Size = new System.Drawing.Size(114, 19);
            this.normalSpritesRBtn.TabIndex = 18;
            this.normalSpritesRBtn.TabStop = true;
            this.normalSpritesRBtn.Text = "Normal Behavior";
            this.normalSpritesRBtn.UseVisualStyleBackColor = false;
            // 
            // disableAllRBtn
            // 
            this.disableAllRBtn.AutoSize = true;
            this.disableAllRBtn.BackColor = System.Drawing.SystemColors.Control;
            this.disableAllRBtn.Location = new System.Drawing.Point(39, 147);
            this.disableAllRBtn.Name = "disableAllRBtn";
            this.disableAllRBtn.Size = new System.Drawing.Size(118, 19);
            this.disableAllRBtn.TabIndex = 17;
            this.disableAllRBtn.Text = "Disable All Effects";
            this.disableAllRBtn.UseVisualStyleBackColor = false;
            // 
            // enableOverrideRBtn
            // 
            this.enableOverrideRBtn.AutoSize = true;
            this.enableOverrideRBtn.BackColor = System.Drawing.SystemColors.Control;
            this.enableOverrideRBtn.Location = new System.Drawing.Point(39, 120);
            this.enableOverrideRBtn.Name = "enableOverrideRBtn";
            this.enableOverrideRBtn.Size = new System.Drawing.Size(108, 19);
            this.enableOverrideRBtn.TabIndex = 16;
            this.enableOverrideRBtn.Text = "Effect Overrides";
            this.enableOverrideRBtn.UseVisualStyleBackColor = false;
            // 
            // spriteOverBtn
            // 
            this.spriteOverBtn.BackColor = System.Drawing.Color.White;
            this.spriteOverBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.spriteOverBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spriteOverBtn.Location = new System.Drawing.Point(231, 86);
            this.spriteOverBtn.Name = "spriteOverBtn";
            this.spriteOverBtn.Size = new System.Drawing.Size(118, 41);
            this.spriteOverBtn.TabIndex = 15;
            this.spriteOverBtn.Text = "Add Override";
            this.spriteOverBtn.UseVisualStyleBackColor = false;
            this.spriteOverBtn.Click += new System.EventHandler(this.spriteOverBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(161, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 15);
            this.label2.TabIndex = 14;
            this.label2.Text = "Effect to override:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(150, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "New effect number:";
            // 
            // spriteNewNum
            // 
            this.spriteNewNum.BackColor = System.Drawing.Color.White;
            this.spriteNewNum.ForeColor = System.Drawing.Color.Black;
            this.spriteNewNum.Location = new System.Drawing.Point(288, 44);
            this.spriteNewNum.Maximum = new decimal(new int[] {
            387,
            0,
            0,
            0});
            this.spriteNewNum.Name = "spriteNewNum";
            this.spriteNewNum.Size = new System.Drawing.Size(51, 23);
            this.spriteNewNum.TabIndex = 12;
            this.spriteNewNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // spriteOldNum
            // 
            this.spriteOldNum.BackColor = System.Drawing.Color.White;
            this.spriteOldNum.ForeColor = System.Drawing.Color.Black;
            this.spriteOldNum.Location = new System.Drawing.Point(288, 14);
            this.spriteOldNum.Maximum = new decimal(new int[] {
            387,
            0,
            0,
            0});
            this.spriteOldNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spriteOldNum.Name = "spriteOldNum";
            this.spriteOldNum.Size = new System.Drawing.Size(51, 23);
            this.spriteOldNum.TabIndex = 11;
            this.spriteOldNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // effectLbox
            // 
            this.effectLbox.BackColor = System.Drawing.Color.White;
            this.effectLbox.ForeColor = System.Drawing.Color.Black;
            this.effectLbox.FormattingEnabled = true;
            this.effectLbox.ItemHeight = 15;
            this.effectLbox.Location = new System.Drawing.Point(355, 16);
            this.effectLbox.Name = "effectLbox";
            this.effectLbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.effectLbox.Size = new System.Drawing.Size(97, 229);
            this.effectLbox.TabIndex = 10;
            // 
            // SpriveOverride
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.removeOverrideBtn);
            this.Controls.Add(this.normalSpritesRBtn);
            this.Controls.Add(this.disableAllRBtn);
            this.Controls.Add(this.enableOverrideRBtn);
            this.Controls.Add(this.spriteOverBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.spriteNewNum);
            this.Controls.Add(this.spriteOldNum);
            this.Controls.Add(this.effectLbox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "SpriveOverride";
            this.Size = new System.Drawing.Size(490, 258);
            ((System.ComponentModel.ISupportInitialize)(this.spriteNewNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spriteOldNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button removeOverrideBtn;
        private System.Windows.Forms.RadioButton normalSpritesRBtn;
        private System.Windows.Forms.RadioButton disableAllRBtn;
        private System.Windows.Forms.RadioButton enableOverrideRBtn;
        private System.Windows.Forms.Button spriteOverBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown spriteNewNum;
        private System.Windows.Forms.NumericUpDown spriteOldNum;
        private System.Windows.Forms.ListBox effectLbox;
    }
}
