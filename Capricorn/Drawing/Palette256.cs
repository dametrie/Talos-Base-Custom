using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Talos.Properties;

public class Palette256
{
    private System.Drawing.Color[] colors = new System.Drawing.Color[256];

    public System.Drawing.Color this[int index]
    {
        get
        {
            return colors[index];
        }
        set
        {
            colors[index] = value;
        }
    }

    public System.Drawing.Color[] Colors
    {
        get
        {
            return colors;
        }
    }


	public static Palette256 FromFile(string file)
	{
		return LoadPalette(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
	}

	public static Palette256 FromRawData(byte[] data)
	{
		return LoadPalette(new MemoryStream(data));
	}

	public static Palette256 FromArchive(string file, DATArchive archive)
	{
		if (archive.Contains(file))
		{
			return FromRawData(archive.ExtractFile(file));
		}
		return null;
	}

	public static Palette256 FromArchive(string file, bool ignoreCase, DATArchive archive)
	{
		if (archive.Contains(file, ignoreCase))
		{
			return FromRawData(archive.ExtractFiles(file, ignoreCase));
		}
		return null;
	}

	private static Palette256 LoadPalette(Stream stream)
	{
		stream.Seek(0L, SeekOrigin.Begin);
		BinaryReader binaryReader = new BinaryReader(stream);
		Palette256 palette256 = new Palette256();
		for (int i = 0; i < 256; i++)
		{
			palette256.colors[i] = System.Drawing.Color.FromArgb(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());
		}
		return palette256;
	}

	public static Palette256 ApplyDye(Palette256 pal, int dye)//Adam
	{
		if (dye <= 0) 
			return pal;
		StreamReader streamReader = new StreamReader(new MemoryStream(Resources.color));
		System.Drawing.Color[,] colorArray = new System.Drawing.Color[Convert.ToInt32(streamReader.ReadLine()), 6];
            while (!streamReader.EndOfStream)
            {
                int int32_1 = Convert.ToInt32(streamReader.ReadLine());
                for (int index = 0; index < 6; ++index)
                {
                    string[] strArray = streamReader.ReadLine().Trim().Split(',');
                    if (strArray.Length == 3)
                    {
                        int int32_2 = Convert.ToInt32(strArray[0]);
                        int int32_3 = Convert.ToInt32(strArray[1]);
                        int int32_4 = Convert.ToInt32(strArray[2]);
                        if (int32_2 > (int)byte.MaxValue)
                            int32_2 -= (int)byte.MaxValue;
                        if (int32_3 > (int)byte.MaxValue)
                            int32_3 -= (int)byte.MaxValue;
                        if (int32_4 > (int)byte.MaxValue)
                            int32_4 -= (int)byte.MaxValue;
                        colorArray[int32_1, index] = System.Drawing.Color.FromArgb((int)byte.MaxValue, int32_2, int32_3, int32_4);
				}
			}
		}
		streamReader.Close();
		Palette256 palette256 = new Palette256();
		for (int j = 0; j < 256; j++) {
			palette256[j] = pal[j];
		}
		palette256[98] = colorArray[dye, 0];
		palette256[99] = colorArray[dye, 1];
		palette256[100] = colorArray[dye, 2];
		palette256[101] = colorArray[dye, 3];
		palette256[102] = colorArray[dye, 4];
		palette256[103] = colorArray[dye, 5];
		return palette256;
	}
}
