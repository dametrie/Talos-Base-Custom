namespace Talos.Forms.User_Controls
{
    partial class NearbyEnemy
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
            this.nearbyEnemyAddBtn = new System.Windows.Forms.Button();
            this.nearbyEnemySpriteLbl = new System.Windows.Forms.Label();
            this.nearbyEnemyPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nearbyEnemyPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // nearbyEnemyAddBtn
            // 
            this.nearbyEnemyAddBtn.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nearbyEnemyAddBtn.ForeColor = System.Drawing.Color.Black;
            this.nearbyEnemyAddBtn.Location = new System.Drawing.Point(-1, 91);
            this.nearbyEnemyAddBtn.Name = "nearbyEnemyAddBtn";
            this.nearbyEnemyAddBtn.Size = new System.Drawing.Size(267, 55);
            this.nearbyEnemyAddBtn.TabIndex = 12;
            this.nearbyEnemyAddBtn.Text = "Add Creature";
            this.nearbyEnemyAddBtn.UseVisualStyleBackColor = true;
            this.nearbyEnemyAddBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.nearbyEnemyAddBtn_Click);
            // 
            // nearbyEnemySpriteLbl
            // 
            this.nearbyEnemySpriteLbl.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nearbyEnemySpriteLbl.ForeColor = System.Drawing.Color.Black;
            this.nearbyEnemySpriteLbl.Location = new System.Drawing.Point(14, 3);
            this.nearbyEnemySpriteLbl.Name = "nearbyEnemySpriteLbl";
            this.nearbyEnemySpriteLbl.Size = new System.Drawing.Size(124, 85);
            this.nearbyEnemySpriteLbl.TabIndex = 11;
            this.nearbyEnemySpriteLbl.Text = "Sprite: ";
            this.nearbyEnemySpriteLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // nearbyEnemyPicture
            // 
            this.nearbyEnemyPicture.Location = new System.Drawing.Point(160, -1);
            this.nearbyEnemyPicture.Margin = new System.Windows.Forms.Padding(0);
            this.nearbyEnemyPicture.Name = "nearbyEnemyPicture";
            this.nearbyEnemyPicture.Size = new System.Drawing.Size(106, 100);
            this.nearbyEnemyPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.nearbyEnemyPicture.TabIndex = 10;
            this.nearbyEnemyPicture.TabStop = false;
            // 
            // NearbyEnemy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.nearbyEnemyAddBtn);
            this.Controls.Add(this.nearbyEnemySpriteLbl);
            this.Controls.Add(this.nearbyEnemyPicture);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "NearbyEnemy";
            this.Size = new System.Drawing.Size(265, 145);
            this.Load += new System.EventHandler(this.NearbyEnemy_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nearbyEnemyPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button nearbyEnemyAddBtn;
        private System.Windows.Forms.Label nearbyEnemySpriteLbl;
        private System.Windows.Forms.PictureBox nearbyEnemyPicture;
    }
}
