using System;
using System.Drawing;
using System.Windows.Forms;

namespace Talos
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.launchDA = new System.Windows.Forms.ToolStripMenuItem();
            this.clientTabControl = new System.Windows.Forms.TabControl();
            this.mapCacheMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapCacheMenuStrip = new System.Windows.Forms.MenuStrip();
            this.menuStrip1.SuspendLayout();
            this.mapCacheMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Window;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchDA});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(955, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // launchDA
            // 
            this.launchDA.Name = "launchDA";
            this.launchDA.Size = new System.Drawing.Size(58, 20);
            this.launchDA.Text = "Launch";
            this.launchDA.Click += new System.EventHandler(this.launchDA_Click);
            // 
            // clientTabControl
            // 
            this.clientTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.clientTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.clientTabControl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clientTabControl.ItemSize = new System.Drawing.Size(25, 100);
            this.clientTabControl.Location = new System.Drawing.Point(0, 24);
            this.clientTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.clientTabControl.Multiline = true;
            this.clientTabControl.Name = "clientTabControl";
            this.clientTabControl.Padding = new System.Drawing.Point(0, 0);
            this.clientTabControl.SelectedIndex = 0;
            this.clientTabControl.Size = new System.Drawing.Size(957, 578);
            this.clientTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.clientTabControl.TabIndex = 3;
            this.clientTabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.clientTabControl_DrawItem);
            // 
            // mapCacheMenuItem
            // 
            this.mapCacheMenuItem.Name = "mapCacheMenuItem";
            this.mapCacheMenuItem.Padding = new System.Windows.Forms.Padding(0);
            this.mapCacheMenuItem.Size = new System.Drawing.Size(68, 24);
            this.mapCacheMenuItem.Text = "MapCache";
            this.mapCacheMenuItem.Click += new System.EventHandler(this.mapCacheMenuItem_Click);
            // 
            // mapCacheMenuStrip
            // 
            this.mapCacheMenuStrip.AutoSize = false;
            this.mapCacheMenuStrip.BackColor = System.Drawing.Color.White;
            this.mapCacheMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mapCacheMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapCacheMenuItem});
            this.mapCacheMenuStrip.Location = new System.Drawing.Point(882, 0);
            this.mapCacheMenuStrip.Name = "mapCacheMenuStrip";
            this.mapCacheMenuStrip.Padding = new System.Windows.Forms.Padding(0);
            this.mapCacheMenuStrip.Size = new System.Drawing.Size(68, 24);
            this.mapCacheMenuStrip.TabIndex = 4;
            this.mapCacheMenuStrip.Text = "menuStrip2";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 601);
            this.Controls.Add(this.mapCacheMenuStrip);
            this.Controls.Add(this.clientTabControl);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::Talos.Properties.Resources.Talos;
            this.MainMenuStrip = this.mapCacheMenuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Talos";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.mapCacheMenuStrip.ResumeLayout(false);
            this.mapCacheMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void clientTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics graphics = e.Graphics;
            TabPage tabPage = clientTabControl.TabPages[e.Index];
            Rectangle tabRect = clientTabControl.GetTabRect(e.Index);
            Brush brush;
            Font font;
            if (e.State == DrawItemState.Selected)
            {
                brush = new SolidBrush(System.Drawing.Color.Black);
                graphics.FillRectangle(Brushes.White, (RectangleF)tabRect);
                font = new Font("Segoe UI", 14f, FontStyle.Bold, GraphicsUnit.Pixel);
            }
            else
            {
                brush = new SolidBrush(e.ForeColor);
                graphics.FillRectangle(Brushes.WhiteSmoke, (RectangleF)tabRect);
                font = new Font("Segoe UI", 14f, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            //Console.WriteLine("**************************Tabpage name:" + tabPage.Text);
            graphics.DrawString(tabPage.Text, font, brush, (RectangleF)tabRect, format);
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem launchDA;
        private System.Windows.Forms.TabControl clientTabControl;
        private ToolStripMenuItem mapCacheMenuItem;
        private MenuStrip mapCacheMenuStrip;
    }
}