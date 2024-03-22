using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Base;
using Talos.Capricorn.Drawing;
using Talos.Capricorn.IO;
using Talos.Forms.UI;
using Talos.Helper;
using Talos.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Talos.Forms
{
    internal partial class EnemyPage : UserControl
    {
        internal Client Client { get; set; }
        internal Enemy Enemy { get; set; }

        internal EnemyPage(Enemy enemy, Client client)
        {

            Enemy = enemy;
            Client = client;
            UIHelper.Initialize(Client);
            InitializeComponent();

            LoadEnemySprite(enemy);
            ConfigureTargetComboBox(enemy);
            OnlyDisplaySpellsWeHave();
        }

        private void OnlyDisplaySpellsWeHave()
        {

            Dictionary<string, string> singleTarget = new Dictionary<string, string>
            {
                { "Hail of Feathers", "Hail of Feathers" },
                { "Keeter", "Keeter" },
                { "Groo", "Groo" },
                { "Torch", "Torch" },
                { "Mermaid", "Mermaid" },
                { "Star Arrow", "Star Arrow" },
                { "Barrage", "Barrage" },
                { "pian na dion", "A/M/PND" },
                { "deo searg", "A/DS" },
                { "Deception of Life", "DOL" },
                { "Dragon Blast", "Dragon Blast" },
                { "Frost Arrow", "Frost Arrow" },
                { "lamh", "lamh" }
            };
            Dictionary<string, string> multiTarget = new Dictionary<string, string> 
            { 
                { "mor strioch pian gar", "MSPG" },
                { "Cursed Tune", "Cursed Tune" },
                { "Supernova Shot", "Supernova Shot" },
                { "Volley", "Volley" },
                { "Unholy Explosion", "Unholy Explosion" },
                { "deo searg gar", "M/DSG" },
                { "Shock Arrow", "Shock Arrow" }
            };

            UIHelper.SetupComboBox(spellsCurseCombox, new[] { "Demise", "Darker Seal", "Dark Seal", "ard cradh", "mor cradh", "cradh", "beag cradh" }, spellsCurseCbox);
            UIHelper.SetupComboBox(spellsFasCombox, new[] { "ard fas nadur", "mor fas nadur", "fas nadur", "beag fas nadur" }, spellsFasCbox);
            UIHelper.SetupComboBox(spellsControlCombox, new[] { "Mesmerize", "pramh", "beag pramh", "suain" }, spellsControlCbox);
            UIHelper.SetupComboBox(attackComboxOne, new[] { "Hail of Feathers", "Keeter", "Groo", "Torch", "Mermaid", "Star Arrow", "Barrage", "pian na dion", "pian na dion", "deo searg", "Deception of Life", "Dragon Blast", "Frost Arrow", "lamh" }, attackCboxOne, null, null, null, partialMatch: true, abbreviations: singleTarget);
            UIHelper.SetupComboBox(attackComboxTwo, new[] { "mor strioch pian gar", "Cursed Tune", "Supernova Shot", "Volley", "Unholy Explosion", "deo searg gar", "Shock Arrow"}, attackCboxTwo, null, null, null, partialMatch: true, abbreviations: multiTarget);
            UIHelper.SetupCheckbox(mpndSilenced, "pian na dion");
            UIHelper.SetupCheckbox(mspgSilenced, "mor strioch pian gar");
            UIHelper.SetupCheckbox(mspgPct, "mor strioch pian gar");

        }



        private void LoadEnemySprite(Enemy enemy)
        {
            base.Name = enemy.SpriteID.ToString();
            string spriteFileName = $"MNS{enemy.SpriteID:D3}.MPF";
            string archivePath = Settings.Default.DarkAgesPath.Replace("Darkages.exe", "hades.dat");

            DATArchive archive = null;
            try
            {
                archive = DATArchive.FromFile(archivePath);
                if (archive.Contains(spriteFileName))
                {
                    var spriteImage = LoadSpriteFromArchive(spriteFileName, archive);
                    if (spriteImage != null)
                    {
                        ConfigureEnemyPicture(spriteImage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in InitializeEnemySprite: {ex.Message}");
            }
            finally
            {
                // DATArchive doesn't have a dispose?
            }
        }

        private Bitmap LoadSpriteFromArchive(string spriteFileName, DATArchive archive)
        {
            MPFImage sprite = MPFImage.FromArchive(spriteFileName, archive);
            int frameIndex = CalculateFrameIndex(sprite);

            Palette256 palette = Palette256.FromArchive(sprite.palette, archive);
            Bitmap renderedImage = DAGraphics.RenderImage(sprite[frameIndex], palette);  // Ensure RenderImage returns a Bitmap
            
            return renderedImage;
        }

        private int CalculateFrameIndex(MPFImage sprite)
        {
            int frameIndex = (sprite.idleLength == 0 || sprite.walkStart == sprite.idleStart) ? sprite.walkStart + sprite.walkLength : sprite.walkStart - sprite.idleLength;
            frameIndex = Math.Max(frameIndex, 0); // Ensure frameIndex is not negative
            frameIndex = Math.Min(frameIndex, sprite.expectedFrames - 1); // Ensure frameIndex does not exceed bounds
            return frameIndex;
        }

        private void ConfigureEnemyPicture(Bitmap spriteImage)
        {
            enemyPicture.SizeMode = (spriteImage.Width > 100 || spriteImage.Height > 100) ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
            enemyPicture.Image = spriteImage;  // Assign the Bitmap directly to the PictureBox

            //flip the image
            spriteImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            enemyPicture.Image = spriteImage;
        }

        private void ConfigureTargetComboBox(Enemy enemy)
        {
            if (!enemy.IsAllMonsters)
            {
                targetCombox.Items.Remove("Cluster 29");
                targetCombox.Items.Remove("Cluster 13");
                targetCombox.Items.Remove("Cluster 5");
            }
        }

        internal void enemyRemoveBtn_Click(object sender, EventArgs e)
        {
            foreach (Enemy enemy in new List<Enemy>(Client.Bot.ReturnEnemyList()))
            {
                if (enemy.EnemyPage == this)
                {
                    Client.Bot.ClearEnemyLists(enemy.SpriteID.ToString());
                }
            }
            if (Enemy.ToString() == "all monsters")
            {
                Client.Bot.EnemyPage = null;
                Client.ClientTab.monsterTabControl.TabPages.Add(Client.ClientTab.nearbyEnemyTab);
            }
            Parent.Dispose();
            Client.RequestRefresh(false);
        }


    }
}
