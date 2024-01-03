using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

public class DATArchive
{
	private string filename;

	private DATFileEntry[] files;

	private int expectedFiles;

	public DATFileEntry this[int index]
	{
		get
		{
			return files[index];
		}
		set
		{
			files[index] = value;
		}
	}

	public string FileName
	{
		get
		{
			return filename;
		}
		set
		{
			filename = value;
		}
	}


	public DATFileEntry[] Files
	{
		get
		{
			return files;
		}
	}

	public int ExpectedFiles
	{
		get
		{
			return expectedFiles;
		}
	}

	public static DATArchive FromFile(string files)
	{
		BinaryReader binaryReader = new BinaryReader(new FileStream(files, FileMode.Open, FileAccess.Read, FileShare.Read));
		DATArchive datArchive = new DATArchive
		{
			filename = files,
			expectedFiles = binaryReader.ReadInt32()
		};
		datArchive.files = new DATFileEntry[datArchive.expectedFiles - 1];
		for (int i = 0; i < datArchive.expectedFiles - 1; i++)
		{
			long startAddress = binaryReader.ReadUInt32();
			string name = Encoding.ASCII.GetString(binaryReader.ReadBytes(13));
			long endAddress = binaryReader.ReadUInt32();
			binaryReader.BaseStream.Seek(-4L, SeekOrigin.Current);
			int startIndex = name.IndexOf('\0');
			if (startIndex != -1)
			{
				name = name.Remove(startIndex, 13 - startIndex);
			}
			datArchive.files[i] = new DATFileEntry(name, startAddress, endAddress);
		}
		binaryReader.Close();
		return datArchive;
	}

	public bool Contains(string string_1)
	{
		DATFileEntry[] gClass23_ = files;
		int num = 0;
		while (true)
		{
			if (num < gClass23_.Length)
			{
				if (gClass23_[num].Name == string_1)
				{
					break;
				}
				num++;
				continue;
			}
			return false;
		}
		return true;
	}

	public bool Contains(string name, bool ignoreCase)
	{
		DATFileEntry[] gClass23_ = files;
		int num = 0;
		while (true)
		{
			if (num < gClass23_.Length)
			{
				DATFileEntry datFileEntry = gClass23_[num];
				if (ignoreCase)
				{
					if (datFileEntry.Name.ToUpper() == name.ToUpper())
					{
						return true;
					}
				}
				else if (datFileEntry.Name == name)
				{
					break;
				}
				num++;
				continue;
			}
			return false;
		}
		return true;
	}

	public int IndexOf(string string_1)
	{
		int num = 0;
		while (true)
		{
			if (num < files.Length)
			{
				if (files[num].Name == string_1)
				{
					break;
				}
				num++;
				continue;
			}
			return -1;
		}
		return num;
	}

	public int IndexOf(string string_1, bool bool_0)
	{
		int num = 0;
		while (true)
		{
			if (num < files.Length)
			{
				if (bool_0)
				{
					if (files[num].Name.ToUpper() == string_1.ToUpper())
					{
						return num;
					}
				}
				else if (files[num].Name == string_1)
				{
					break;
				}
				num++;
				continue;
			}
			return -1;
		}
		return num;
	}

	public byte[] ExtractFile(string name)
	{
		if (!Contains(name))
		{
			return null;
		}
		BinaryReader binaryReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
		int num = IndexOf(name);
		binaryReader.BaseStream.Seek(files[num].StartAddress, SeekOrigin.Begin);
		byte[] result = binaryReader.ReadBytes((int)files[num].FileSize);
		binaryReader.Close();
		return result;
	}

	public byte[] ExtractFiles(string name, bool ignoreCase)
	{
		if (!Contains(name, ignoreCase))
		{
			return null;
		}
		BinaryReader binaryReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
		int index = IndexOf(name, ignoreCase);
		binaryReader.BaseStream.Seek(files[index].StartAddress, SeekOrigin.Begin);
		byte[] result = binaryReader.ReadBytes((int)files[index].FileSize);
		binaryReader.Close();
		return result;
	}

	public byte[] ExtractFile(DATFileEntry entry)
	{
		if (!Contains(entry.Name))
		{
			return null;
		}
		BinaryReader binaryReader = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
		binaryReader.BaseStream.Seek(entry.StartAddress, SeekOrigin.Begin);
		byte[] result = binaryReader.ReadBytes((int)entry.FileSize);
		binaryReader.Close();
		return result;
	}

	public virtual string ToString()
	{
		return "{Name = " + filename + ", Files = " + expectedFiles + "}";
	}
}
