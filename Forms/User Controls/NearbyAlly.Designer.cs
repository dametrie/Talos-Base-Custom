namespace Talos.Forms.User_Controls
{
    partial class NearbyAlly
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NearbyAlly));
            this.pictureCharacter = new System.Windows.Forms.PictureBox();
            this.nearbyAllyAddBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCharacter)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureCharacter
            // 
            this.pictureCharacter.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pictureCharacter, "pictureCharacter");
            this.pictureCharacter.Name = "pictureCharacter";
            this.pictureCharacter.TabStop = false;
            // 
            // nearbyAllyAddBtn
            // 
            resources.ApplyResources(this.nearbyAllyAddBtn, "nearbyAllyAddBtn");
            this.nearbyAllyAddBtn.ForeColor = System.Drawing.Color.Black;
            this.nearbyAllyAddBtn.Name = "nearbyAllyAddBtn";
            this.nearbyAllyAddBtn.UseVisualStyleBackColor = true;
            this.nearbyAllyAddBtn.Click += new System.EventHandler(nearbyAllyAddBtn_Click);
            // 
            // NearbyAlly
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pictureCharacter);
            this.Controls.Add(this.nearbyAllyAddBtn);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "NearbyAlly";
            ((System.ComponentModel.ISupportInitialize)(this.pictureCharacter)).EndInit();

            base.Load += new System.EventHandler(NearbyAlly_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureCharacter;
        private System.Windows.Forms.Button nearbyAllyAddBtn;
    }
}
