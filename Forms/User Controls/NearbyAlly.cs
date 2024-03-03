using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Talos.Base;
using Talos.Capricorn.Drawing;
using Talos.Capricorn.IO;
using Talos.Objects;
using Talos.Properties;

namespace Talos.Forms.User_Controls
{
    internal partial class NearbyAlly : UserControl
    {
        private EPFImage bodyEPFImage;

        private Player Player { get; set; }
        private Client Client { get; set; }

        private string Gender { get; set; }

        internal PaletteTable palb = new PaletteTable();
        internal PaletteTable palm = new PaletteTable();
        internal PaletteTable palu = new PaletteTable();
        internal PaletteTable palh = new PaletteTable();
        internal PaletteTable palc = new PaletteTable();
        internal PaletteTable pale = new PaletteTable();
        internal PaletteTable palf = new PaletteTable();
        internal PaletteTable pali = new PaletteTable();
        internal PaletteTable pall = new PaletteTable();
        internal PaletteTable palw = new PaletteTable();
        internal PaletteTable palp = new PaletteTable();
        internal Palette256 headDye;
        internal Palette256 headDye2;
        internal Palette256 headDye3;
        internal Palette256 bootDye;
        internal Palette256 overcoatDye;
        internal Palette256 dye;
        internal Palette256 accessoryDye;
        internal Palette256 accessoryDye2;
        internal Palette256 accessory2Dye;
        internal Palette256 accessory2Dye2;
        internal Palette256 accessory3Dye;
        internal Palette256 accessory3Dye2;
        private Bitmap overcoatBitmap = new Bitmap(1, 1);
        private Bitmap bodyBitmap = new Bitmap(1, 1);
        internal bool isLoaded;

        internal NearbyAlly(Player player, Client client)
        {
            InitializeComponent();
            Player = player;
            Client = client;
            nearbyAllyAddBtn.Text = "Add " + Player.Name;
            if ((player.BodySprite & 0x10) != 16 && (player.BodySprite & 0x20) != 32)
            {
                if (player.BodySprite != 0)
                {
                    return;
                }
                string fileName = $"MNS{(Player.SpriteID == 0 ? 910 : Player.SpriteID):D3}.MPF";
                string hadesFilePath = Settings.Default.DarkAgesPath.Replace("Darkages.exe", "hades.dat");

                DATArchive hades = DATArchive.FromFile(hadesFilePath);

                if (hades.Contains(fileName))
                {
                    MPFImage image = MPFImage.FromArchive(fileName, hades);
                    int frameIndex = CalculateFrameIndex(image);

                    Palette256 palette = Palette256.FromArchive(image.palette, hades);
                    Bitmap bitmap = DAGraphics.RenderImage(image[frameIndex], palette);
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);

                    pictureCharacter.SizeMode = GetSizeMode(bitmap);
                    pictureCharacter.Image = bitmap;
                }
                return;
            }
            palm.LoadTables("palm", MainForm.khanFiles["khanpal.dat"]);
            palm.LoadPalettes("palm", MainForm.khanFiles["khanpal.dat"]);
            palb.LoadTables("palb", MainForm.khanFiles["khanpal.dat"]);
            palb.LoadPalettes("palb", MainForm.khanFiles["khanpal.dat"]);
            pale.LoadTables("pale", MainForm.khanFiles["khanpal.dat"]);
            pale.LoadPalettes("pale", MainForm.khanFiles["khanpal.dat"]);
            palf.LoadTables("palf", MainForm.khanFiles["khanpal.dat"]);
            palf.LoadPalettes("palf", MainForm.khanFiles["khanpal.dat"]);
            palh.LoadTables("palh", MainForm.khanFiles["khanpal.dat"]);
            palh.LoadPalettes("palh", MainForm.khanFiles["khanpal.dat"]);
            pall.LoadTables("pall", MainForm.khanFiles["khanpal.dat"]);
            pall.LoadPalettes("pall", MainForm.khanFiles["khanpal.dat"]);
            pali.LoadTables("pali", MainForm.khanFiles["khanpal.dat"]);
            pali.LoadPalettes("pali", MainForm.khanFiles["khanpal.dat"]);
            palu.LoadTables("palu", MainForm.khanFiles["khanpal.dat"]);
            palu.LoadPalettes("palu", MainForm.khanFiles["khanpal.dat"]);
            palp.LoadTables("palp", MainForm.khanFiles["khanpal.dat"]);
            palp.LoadPalettes("palp", MainForm.khanFiles["khanpal.dat"]);
            palw.LoadTables("palw", MainForm.khanFiles["khanpal.dat"]);
            palw.LoadPalettes("palw", MainForm.khanFiles["khanpal.dat"]);
            palc.LoadTables("palc", MainForm.khanFiles["khanpal.dat"]);
            palc.LoadPalettes("palc", MainForm.khanFiles["khanpal.dat"]);
            DisplayBody(Player);
            DisplayOvercoat(Player);
            DrawPlayer();

        }

        internal void DrawPlayer()
        {
            Bitmap image = new Bitmap(100, 85, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(image);
            //graphics.DrawImage(accessoryBitmap2, -5, 0);
            //graphics.DrawImage(accessory2Bitmap2, -5, 0);
            //graphics.DrawImage(accessory3Bitmap2, -5, 0);
            graphics.DrawImage(bodyBitmap, 45, 15);
            //graphics.DrawImage(dyeBitmap, 22, 0);
            //graphics.DrawImage(faceBitmap, 22, 0);
            //graphics.DrawImage(headBitmap, 22, 0);
            //graphics.DrawImage(headBitmap2, 22, 0);
            //graphics.DrawImage(headBitmap3, 22, 0);
            //graphics.DrawImage(bootBitmap, 22, 0);
            graphics.DrawImage(overcoatBitmap, 44, 28);
            //graphics.DrawImage(overcoatBitmap2, 22, 0);
            //graphics.DrawImage(weaponBitmap, -5, 0);
            //graphics.DrawImage(weaponBitmap2, -5, 0);
            //graphics.DrawImage(shieldBitmap, 22, 0);
            //graphics.DrawImage(accessoryBitmap, -5, 0);
            //graphics.DrawImage(accessory2Bitmap, -5, 0);
            //graphics.DrawImage(accessory3Bitmap, -5, 0);
            pictureCharacter.Image = image;
        }

        int CalculateFrameIndex(MPFImage image)
        {
            int num = image.idleLength == 0 || image.walkStart == image.idleStart ? image.walkStart + image.walkLength : image.walkStart - image.idleLength;
            num = Math.Max(0, num); // Ensure num is not negative
            return Math.Min(num, image.expectedFrames - 1); // Ensure num does not exceed the maximum frame index
        }

        private PictureBoxSizeMode GetSizeMode(Bitmap bitmap)
        {
            return (bitmap.Width > 100 || bitmap.Height > 100) ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
        }

        private DATArchive stringParser(string set, bool gender)
        {
            int num = int.Parse(Regex.Match(set, "\\d+").Value);

            var rangeToFileNames = new Dictionary<(int, int), string>
        {
            { (97, 100), gender ? "khanmad.dat" : "khanwad.dat" },
            { (101, 104), gender ? "khanmeh.dat" : "khanweh.dat" },
            { (105, 109), gender ? "khanmim.dat" : "khanwim.dat" },
            { (110, 115), gender ? "khanmns.dat" : "khanwns.dat" },
            { (116, 122), gender ? "khanmtz.dat" : "khanwtz.dat" }
        };

            foreach (var range in rangeToFileNames.Keys)
            {
                if (num >= range.Item1 && num <= range.Item2)
                {
                    return MainForm.khanFiles[rangeToFileNames[range]];
                }
            }

            return null;
        }

        internal void DisplayBody(Player player)
        {
            const int MaleFlag = 0x10; // 16
            const int FemaleFlag = 0x20; // 32
            Gender = (player.BodySprite & MaleFlag) == MaleFlag ? "male" :
                            (player.BodySprite & FemaleFlag) == FemaleFlag ? "woman" :
                            string.Empty;

            if (!string.IsNullOrEmpty(Gender))
            {
                string archiveKey = "109";
                bool isMale = Gender == "male";
                string epfFileName = isMale ? "mm00101.epf" : "wm00101.epf";

                DATArchive archive = stringParser(archiveKey, isMale);

                var bodyEPFImage = EPFImage.FromArchive(epfFileName, archive);
                bodyBitmap = DAGraphics.RenderImage(bodyEPFImage[5], palm[player.BodyColor]);

            }
        }

        internal void DisplayOvercoat(Player player)
        {
            // Determine if processing overcoat or armor sprite based on OvercoatSprite's value.
            bool isOvercoat = player.OvercoatSprite != 0;
            int spriteIndex = player.OvercoatSprite;
            int color = isOvercoat ? player.OvercoatColor : 0; // Use OvercoatColor if overcoat is present, otherwise default to 0 for armor.
            string genderPrefix = Gender == "male" ? "m" : "w";
            string archiveCode = spriteIndex < 1000 ? "117" : "105";
            string spriteTypePrefix = spriteIndex < 1000 ? (genderPrefix + "u") : (genderPrefix + "i");
            string armorPrefix = genderPrefix + "a";

            // Adjust spriteIndex for sprites >= 1000.
            if (spriteIndex >= 1000) spriteIndex -= 1000;

            // Parse the archive code and gender to get the DATArchive object.
            DATArchive archive = stringParser(archiveCode, Gender == "male");

            // Process upper body or inner sprite and corresponding armor sprite.
            ProcessSprite(archive, spriteTypePrefix, spriteIndex, color, isOvercoat);
            ProcessSprite(archive, armorPrefix, spriteIndex, color, false); // Armor does not use overcoat color, so isOvercoat is always false.
        }

        private void ProcessSprite(DATArchive archive, string prefix, int spriteIndex, int color, bool isOvercoat)
        {
            string spriteName = $"{prefix}{spriteIndex:D3}01.epf";
            EPFImage epfImage = EPFImage.FromArchive(spriteName, archive);
            if (epfImage != null)
            {
                Palette256 dye = Palette256.ApplyDye(GetPalette(prefix, spriteIndex, isOvercoat), color);
                //Bitmap bitmap = DAGraphics.RenderImage(epfImage[5], dye);
                overcoatBitmap = DAGraphics.RenderImage(epfImage[5], dye);
            }
        }

        private Palette256 GetPalette(string prefix, int spriteIndex, bool isOvercoat)
        {
            // Use gclass7_2 to retrieve the correct palette based on prefix, spriteIndex, and whether it's an overcoat.
            string paletteName = $"{prefix}{spriteIndex:D3}";
            return palu.GetPalette(paletteName);
        }

        private void nearbyAllyAddBtn_Click(object sender, EventArgs e)
        {
            Client.ClientTab.AddAllyPage(base.Name, pictureCharacter.Image);
        }
        private void NearbyAlly_Load(object sender, EventArgs e)
        {
            isLoaded = true;
        }
    }
}
