using System;
using System.Collections.Generic;
using System.IO;

public class PaletteTable
{
	private List<PaletteTableEntry> entries = new List<PaletteTableEntry>();

	public Dictionary<int, Palette256> palettes = new Dictionary<int, Palette256>();

	private List<PaletteTableEntry> overrides = new List<PaletteTableEntry>();

	public Palette256 this[int index]
	{
		get
		{
			if (index >= palettes.Count)
			{
				return palettes[0];
			}
			return palettes[index];
		}
	}

	public Palette256 GetPalette(string image)
	{
		int index = 0;
		int int32 = Convert.ToInt32(image.Substring(2, 3));
		if (image.StartsWith("w"))
		{
			foreach (PaletteTableEntry paletteTableEntry in overrides)
			{
				if (int32 >= paletteTableEntry.Min && int32 <= paletteTableEntry.Max)
				{
					index = paletteTableEntry.Palette;
				}
			}
		}
		else if (image.StartsWith("m"))
		{
			foreach (PaletteTableEntry entry in entries)
			{
				if (int32 >= entry.Min && int32 <= entry.Max)
				{
					index = entry.Palette;
				}
			}
		}
		if (index < 0 || index > palettes.Count)
		{
			index = 0;
		}
		return palettes[index];
	}

	private int LoadTableInternal(Stream stream)
	{
		stream.Seek(0L, SeekOrigin.Begin);
		StreamReader streamReader = new StreamReader(stream);
		entries.Clear();
		while (!streamReader.EndOfStream)
		{
			string[] array = streamReader.ReadLine().Split(' ');
			if (array.Length == 3)
			{
				entries.Add(new PaletteTableEntry(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2])));
			}
			else if (array.Length == 2)
			{
				int num = Convert.ToInt32(array[0]);
				int int_ = num;
				int int_2 = Convert.ToInt32(array[1]);
				entries.Add(new PaletteTableEntry(num, int_, int_2));
			}
		}
		streamReader.Close();
		return entries.Count;
	}

	public int LoadPalettes(string pattern, DATArchive archive)
	{
		palettes.Clear();
		foreach (DATFileEntry datFileEntry in archive.Files)
		{
			if (datFileEntry.Name.Length == 11 && datFileEntry.Name.ToUpper().EndsWith(".PAL") && datFileEntry.Name.ToUpper().StartsWith(pattern.ToUpper()))
			{
				palettes.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(datFileEntry.Name).Remove(0, pattern.Length)), Palette256.FromArchive(datFileEntry.Name, archive));
			}
		}
		return palettes.Count;
	}

	public int LoadPalettes(string string_0, string string_1)
	{
		string[] files = Directory.GetFiles(string_1, string_0 + "*.PAL", SearchOption.TopDirectoryOnly);
		palettes.Clear();
		string[] array = files;
		foreach (string text in array)
		{
			if (Path.GetFileName(text).ToUpper().EndsWith(".PAL") && Path.GetFileName(text).ToUpper().StartsWith(string_0.ToUpper()))
			{
				palettes.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(text).Remove(0, string_0.Length)), Palette256.FromFile(text));
			}
		}
		return palettes.Count;
	}

	public int LoadTables(string pattern, DATArchive archive)
	{
		entries.Clear();
		foreach (DATFileEntry entry in archive.Files)
		{
			if (entry.Name.ToUpper().EndsWith(".TBL") && entry.Name.ToUpper().StartsWith(pattern.ToUpper()) && Path.GetFileNameWithoutExtension(entry.Name).Remove(0, pattern.Length) != "ani")
			{
				StreamReader streamReader = new StreamReader(new MemoryStream(archive.ExtractFile(entry)));
				while (!streamReader.EndOfStream)
				{
					string[] array = streamReader.ReadLine().TrimEnd().Split(' ');
					if (array.Length == 3)
					{
						int num = Convert.ToInt32(array[0]);
						int num2 = Convert.ToInt32(array[1]);
						int num3 = Convert.ToInt32(array[2]);
						switch (num3)
						{
						default:
							entries.Add(new PaletteTableEntry(num, num2, num3));
							overrides.Add(new PaletteTableEntry(num, num2, num3));
							break;
						case -1:
							entries.Add(new PaletteTableEntry(num, num, num2));
							break;
						case -2:
							overrides.Add(new PaletteTableEntry(num, num, num2));
							break;
						}
					}
					else if (array.Length == 2)
					{
						int num4 = Convert.ToInt32(array[0]);
						int int_ = num4;
						int int_2 = Convert.ToInt32(array[1]);
						overrides.Add(new PaletteTableEntry(num4, int_, int_2));
						entries.Add(new PaletteTableEntry(num4, int_, int_2));
					}
				}
				streamReader.Close();
			}
		}
		return entries.Count;
	}

	public int LoadTables(string pattern, string path)
	{
		string[] files = Directory.GetFiles(path, pattern + "*.TBL", SearchOption.TopDirectoryOnly);
		entries.Clear();
		string[] array = files;
		foreach (string path1 in array)
		{
			if (!Path.GetFileName(path1).ToUpper().EndsWith(".TBL") || !Path.GetFileName(path1).ToUpper().StartsWith(pattern.ToUpper()))
			{
				continue;
			}
			string text = Path.GetFileNameWithoutExtension(path1).Remove(0, pattern.Length);
			if (!(text != "ani"))
			{
				continue;
			}
			string[] array2 = File.ReadAllLines(path1);
			foreach (string text2 in array2)
			{
				char[] separator = new char[1]
				{
					' '
				};
				string[] array3 = text2.Split(separator);
				if (array3.Length == 3)
				{
					int int_ = Convert.ToInt32(array3[0]);
					int int_2 = Convert.ToInt32(array3[1]);
					int int_3 = Convert.ToInt32(array3[2]);
					if (int.TryParse(text, out int _))
					{
						overrides.Add(new PaletteTableEntry(int_, int_2, int_3));
					}
					else
					{
						entries.Add(new PaletteTableEntry(int_, int_2, int_3));
					}
				}
				else if (array3.Length == 2)
				{
					int num = Convert.ToInt32(array3[0]);
					int int_4 = num;
					int int_5 = Convert.ToInt32(array3[1]);
					if (int.TryParse(text, out int _))
					{
						overrides.Add(new PaletteTableEntry(num, int_4, int_5));
					}
					else
					{
						entries.Add(new PaletteTableEntry(num, int_4, int_5));
					}
				}
			}
		}
		return entries.Count;
	}

	public static void LoadTables(PaletteTable paletteTable, string string_0)
	{
		if (!File.Exists(string_0))
		{
			return;
		}
		StreamReader streamReader = new StreamReader(new FileStream(string_0, FileMode.Open, FileAccess.Read));
		while (!streamReader.EndOfStream)
		{
			string[] array = streamReader.ReadLine().TrimEnd().Split(' ');
			if (array.Length == 3)
			{
				int min = Convert.ToInt32(array[0]);
				int int32_ = Convert.ToInt32(array[1]);
				int palette = Convert.ToInt32(array[2]);
				foreach (PaletteTableEntry item in paletteTable.overrides)
				{
					if (item.Min == min)
					{
						item.Palette = palette;
						break;
					}
				}
				foreach (PaletteTableEntry item2 in paletteTable.entries)
				{
					if (item2.Min == min)
					{
						item2.Palette = int32_;
						break;
					}
				}
			}
		}
		streamReader.Close();
	}

	public virtual string ToString()
	{
		return "{Entries = " + entries.Count + ", Palettes = " + palettes.Count + "}";
	}
}
