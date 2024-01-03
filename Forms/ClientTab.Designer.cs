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
            this.packetList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // packetList
            // 
            this.packetList.FormattingEnabled = true;
            this.packetList.Location = new System.Drawing.Point(96, 63);
            this.packetList.Name = "packetList";
            this.packetList.Size = new System.Drawing.Size(580, 394);
            this.packetList.TabIndex = 0;
            this.packetList.SelectedIndexChanged += new System.EventHandler(this.packetList_SelectedIndexChanged);
            // 
            // ClientTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.packetList);
            this.ForeColor = System.Drawing.Color.Black;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ClientTab";
            this.Size = new System.Drawing.Size(850, 544);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox packetList;
    }
}
