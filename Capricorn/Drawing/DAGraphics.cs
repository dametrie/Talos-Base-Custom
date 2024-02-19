using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Talos.Capricorn.Drawing
{
    /// <summary>
    /// Image Type Enumeration
    /// </summary>
    public enum ImageType
    {
        EPF,
        MPF,
        HPF,
        SPF,
        EFA,
        ZP,
        Tile
    }

    /// <summary>
    /// Dark Ages Graphics Class
    /// </summary>
    public class DAGraphics
    {
        /// <summary>
        /// Renders a HPF image to a standard bitmap image.
        /// </summary>
        /// <param name="hpf">HPF image to render.</param>
        /// <param name="palette">Palette of colors to use.</param>
        /// <returns>Bitmap of rendered image.</returns>
        public unsafe static Bitmap RenderImage(HPFImage hpf, Palette256 palette)
        {
            return SimpleRender(hpf.Width, hpf.Height, hpf.RawData, palette, ImageType.HPF);
        }

        /// <summary>
        /// Renders an EPF image to a standard bitmap image.
        /// </summary>
        /// <param name="epf">EPF frame image to render.</param>
        /// <param name="palette">Palette of colors to use.</param>
        /// <returns>Bitmap of rendered image.</returns>
        public unsafe static Bitmap RenderImage(EPFFrame epf, Palette256 palette)
        {
            return SimpleRender(epf.Width, epf.Height, epf.RawData, palette, ImageType.EPF);
        }

        /// <summary>
        /// Renders a MPF image to a standard bitmap image.
        /// </summary>
        /// <param name="mpf">MPF frame image to render.</param>
        /// <param name="palette">Palette of colors to use.</param>
        /// <returns>Bitmap of rendered image.</returns>
        public unsafe static Bitmap RenderImage(MPFFrame mpf, Palette256 palette)
        {
            return SimpleRender(mpf.Width, mpf.Height, mpf.RawData, palette, ImageType.MPF);
        }

        /// <summary>
        /// Renders a single tile to a standard bitmap image.
        /// </summary>
        /// <param name="tileData">Tile data to render.</param>
        /// <param name="palette">Palette of colors to use.</param>
        /// <returns>Bitmap of rendered tile image.</returns>
        public unsafe static Bitmap RenderTile(byte[] tileData, Palette256 palette)
        {
            return SimpleRender(Tileset.TileWidth, Tileset.TileHeight, tileData, palette, ImageType.Tile);
        }

        /// <summary>
        /// Internal function used to render images.
        /// </summary>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="data">Raw data bits of the image.</param>
        /// <param name="palette">Palette to use to render the image.</param>
        /// <param name="type">Image type to render.</param>
        /// <returns>Bitmap of the rendered image.</returns>
        private unsafe static Bitmap SimpleRender(int width, int height, byte[] data, Palette256 palette, ImageType type)
        {
            // Create Bitmap
            Bitmap image = new Bitmap(width, height);

            // Lock Bits
            BitmapData bmd = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.WriteOnly,
                image.PixelFormat);

            // Render Image
            for (int y = 0; y < bmd.Height; y++)
            {
                byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                for (int x = 0; x < bmd.Width; x++)
                {
                    #region Get Value from Raw Data
                    int colorIndex = 0;
                    if (type == ImageType.EPF)
                    {
                        colorIndex = data[x * height + y];
                    }
                    else
                    {
                        colorIndex = data[y * width + x];
                    }
                    #endregion

                    if (colorIndex > 0)
                    {
                        #region 32 Bit Render
                        if (bmd.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            row[x * 4] = palette[colorIndex].B;
                            row[x * 4 + 1] = palette[colorIndex].G;
                            row[x * 4 + 2] = palette[colorIndex].R;
                            row[x * 4 + 3] = palette[colorIndex].A;
                        }
                        #endregion

                        #region 24 Bit Render
                        else if (bmd.PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            row[x * 3] = palette[colorIndex].B;
                            row[x * 3 + 1] = palette[colorIndex].G;
                            row[x * 3 + 2] = palette[colorIndex].R;
                        }
                        #endregion

                        #region 15 Bit Render
                        else if (bmd.PixelFormat == PixelFormat.Format16bppRgb555)
                        {
                            // Get 15-Bit Color
                            ushort colorWORD = (ushort)(((palette[colorIndex].R & 248) << 7) +
                                ((palette[colorIndex].G & 248) << 2) +
                                (palette[colorIndex].B >> 3));

                            row[x * 2] = (byte)(colorWORD % 256);
                            row[x * 2 + 1] = (byte)(colorWORD / 256);
                        }
                        #endregion

                        #region 16 Bit Render
                        else if (bmd.PixelFormat == PixelFormat.Format16bppRgb565)
                        {
                            // Get 16-Bit Color
                            ushort colorWORD = (ushort)(((palette[colorIndex].R & 248) << 8)
                                + ((palette[colorIndex].G & 252) << 3) +
                                (palette[colorIndex].B >> 3));

                            row[x * 2] = (byte)(colorWORD % 256);
                            row[x * 2 + 1] = (byte)(colorWORD / 256);
                        }
                        #endregion
                    }
                }
            }

            // Unlock Bits
            image.UnlockBits(bmd);

            // Flip Image
            if (type == ImageType.EPF)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipX);
            }

            // Return Bitmap
            return image;
        }

        /// <summary>
        /// Renders a map file, given the parameters.
        /// </summary>
        /// <param name="map">Map file to render.</param>
        /// <param name="tiles">Tileset to use.</param>
        /// <param name="tileTable">Tile palette table.</param>
        /// <param name="wallTable">Wall palette table.</param>
        /// <param name="wallSource">Wall source data archive.</param>
        /// <returns>Render map image.</returns>
        public static Bitmap RenderMap(MAPFile map, Tileset tiles,
            PaletteTable tileTable, PaletteTable wallTable,
                DATArchive wallSource)
        {
            int additionalTop = 256, additionalBottom = 96;

            Bitmap mapImage = new Bitmap(
                Tileset.TileWidth * map.Width,
                Tileset.TileHeight * (map.Height + 1) + additionalTop + additionalBottom);

            Graphics g = Graphics.FromImage(mapImage);

            #region Render Floor

            // Set Origins
            int xOrigin = ((mapImage.Width / 2) - 1) - Tileset.TileWidth / 2 + 1;
            int yOrigin = additionalTop;

            // Cached Tiles
            Dictionary<int, Bitmap> cachedFloor = new Dictionary<int, Bitmap>();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    // Get Floor Value
                    int floor = map[x, y].FloorTile;

                    if (floor > 0)
                        floor -= 1;

                    // Cache the Tile if Not Cached Already
                    if (!cachedFloor.ContainsKey(floor))
                    {
                        Bitmap floorTile = DAGraphics.RenderTile(tiles[floor], tileTable[floor + 2]);
                        cachedFloor.Add(floor, floorTile);
                    }

                    // Render Image
                    g.DrawImageUnscaled(cachedFloor[floor],
                        xOrigin + x * Tileset.TileWidth / 2,
                        yOrigin + x * (Tileset.TileHeight + 1) / 2);
                }

                // Offset Origin
                xOrigin -= Tileset.TileWidth / 2;
                yOrigin += (Tileset.TileHeight + 1) / 2;
            }
            #endregion

            #region Render Walls

            // Set Origins
            xOrigin = ((mapImage.Width / 2) - 1) - Tileset.TileWidth / 2 + 1;
            yOrigin = additionalTop;

            // Cached Tiles
            Dictionary<int, Bitmap> cachedWalls = new Dictionary<int, Bitmap>();

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    #region Render Left Wall
                    // Get Left Wall Value
                    int leftWall = map[x, y].LeftWall;

                    // Cache the HPF if Not Cached Already
                    if (!cachedWalls.ContainsKey(leftWall))
                    {
                        HPFImage hpf = HPFImage.FromArchive("stc" + leftWall.ToString().PadLeft(5, '0') + ".hpf", true, wallSource);
                        Bitmap wall = DAGraphics.RenderImage(hpf, wallTable[leftWall + 1]);
                        cachedWalls.Add(leftWall, wall);
                    }

                    // Render Image
                    if ((leftWall % 10000) > 1)
                    {
                        g.DrawImageUnscaled(cachedWalls[leftWall],
                            xOrigin + x * Tileset.TileWidth / 2,
                            yOrigin + (x + 1) * (Tileset.TileHeight + 1) / 2 -
                                cachedWalls[leftWall].Height +
                                (Tileset.TileHeight + 1) / 2);
                    }
                    #endregion

                    #region Render Right Wall
                    // Get Right Wall Value
                    int rightWall = map[x, y].RightWall;

                    // Cache the HPF if Not Cached Already
                    if (!cachedWalls.ContainsKey(rightWall))
                    {
                        HPFImage hpf = HPFImage.FromArchive("stc" + rightWall.ToString().PadLeft(5, '0') + ".hpf", true, wallSource);
                        Bitmap wall = DAGraphics.RenderImage(hpf, wallTable[rightWall + 1]);
                        cachedWalls.Add(rightWall, wall);
                    }

                    // Render Image
                    if ((rightWall % 10000) > 1)
                    {
                        g.DrawImageUnscaled(cachedWalls[rightWall],
                            xOrigin + (x + 1) * Tileset.TileWidth / 2,
                            yOrigin + (x + 1) * (Tileset.TileHeight + 1) / 2 -
                                cachedWalls[rightWall].Height +
                                (Tileset.TileHeight + 1) / 2);
                    }
                    #endregion
                }

                // Offset Origin
                xOrigin -= Tileset.TileWidth / 2;
                yOrigin += (Tileset.TileHeight + 1) / 2;
            }
            #endregion

            #region Draw Text
            SolidBrush brush = new SolidBrush(Color.White);
            g.DrawString(map.Name.ToUpper(),
                new Font("04b03b", 6.0f, FontStyle.Regular),
                brush,
                16, 16);

            g.DrawString("LOD" + map.ID.ToString() + ".MAP",
                new Font("04b03b", 6.0f, FontStyle.Regular),
                brush,
                16, 26);

            g.DrawString(map.Width.ToString() + "x" + map.Height.ToString() + " TILES",
                new Font("04b03b", 6.0f, FontStyle.Regular),
                brush,
                16, 36);
            brush.Dispose();
            #endregion

            g.Dispose();
            return mapImage;
        }
    }
}

