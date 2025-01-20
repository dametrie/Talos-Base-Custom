using System;
using System.Drawing;
using System.Windows.Forms;
using Talos.Base;
using Talos.Capricorn.Drawing;
using Talos.Capricorn.IO;
using Talos.Objects;
using Talos.Properties;

namespace Talos.Forms.User_Controls
{
    internal partial class NearbyEnemy : UserControl
    {
        private Creature NPC { get; set; }
        private Client Client { get; set; }

        internal bool _isLoaded;

        internal NearbyEnemy(Creature npc, Client client)
        {

            NPC = npc;
            Client = client;
            InitializeComponent();

            LoadEnemySprite(npc);
        }

        private void LoadEnemySprite(Creature npc)
        {
 
            string spriteFileName = $"MNS{npc.SpriteID:D3}.MPF";
            string archivePath = Settings.Default.DarkAgesPath.Replace("Darkages.exe", "hades.dat");

            DATArchive archive = null;
            try
            {
                archive = DATArchive.FromFile(archivePath);
                if (archive.Contains(spriteFileName))
                {
                    var spriteImage = LoadSpriteFromArchive(spriteFileName, archive);
                    ConfigureEnemyPicture(spriteImage);
                    nearbyEnemySpriteLbl.Text = $"Sprite: {NPC.SpriteID}";
                    Name = NPC.SpriteID.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading sprite for {spriteFileName}: {ex.Message}");
            }
            finally
            {
                // DATArchive doesn't have a dispose?
            }

        }

        private Bitmap LoadSpriteFromArchive(string spriteFileName, DATArchive archive)
        {
            MPFImage mpfImage = MPFImage.FromArchive(spriteFileName, archive);
            int frameIndex = CalculateFrameIndex(mpfImage);

            Palette256 palette = Palette256.FromArchive(mpfImage.palette, archive);
            Bitmap renderedImage = DAGraphics.RenderImage(mpfImage[frameIndex], palette);
 
            return renderedImage;
        }

        private int CalculateFrameIndex(MPFImage mpfImage)
        {
            int frameIndex = ((mpfImage.idleLength == 0) || (mpfImage.walkStart == mpfImage.idleStart)) ? (mpfImage.walkStart + mpfImage.walkLength) : (mpfImage.walkStart - mpfImage.idleLength);
            if (frameIndex < 0) frameIndex = 0;
            if (frameIndex >= mpfImage.expectedFrames) frameIndex = mpfImage.expectedFrames - 1;

            return frameIndex;
        }

        private void ConfigureEnemyPicture(Bitmap spriteImage)
        {
            nearbyEnemyPicture.SizeMode = (spriteImage.Width > 100 || spriteImage.Height > 100) ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
            nearbyEnemyPicture.Image = spriteImage;

            //flip the image
            spriteImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            nearbyEnemyPicture.Image = spriteImage;
        }

        private void NearbyEnemy_Load(object sender, EventArgs e)
        {
            _isLoaded = true;
        }

        private void nearbyEnemyAddBtn_Click(object sender, MouseEventArgs e)
        {
            ushort spriteID;
            if ((e.Button == MouseButtons.Left) && ushort.TryParse(Name, out spriteID))
            {
                Client.ClientTab.AddEnemyPage(spriteID);
            }
        }
    }
}
