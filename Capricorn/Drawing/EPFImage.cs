using System.IO;

public class EPFImage
{
	private int expectedFrames;
	private int width;
	private int height;
	private int unknown;
	private long tocAddress;
	private EPFFrame[] frames;
	private byte[] rawData;

	public EPFFrame this[int index]
	{
		get
		{
			return frames[index];
		}
		set
		{
			frames[index] = value;
		}
	}

	public EPFFrame[] Frames
	{
		get
		{
			return frames;
		}
	}

	public long TOCAddress
	{
		get
		{
			return tocAddress;
		}
	}

	public int Unknown
	{
		get
		{
			return unknown;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
	}

	public byte[] RawData
	{
		get { return rawData; }
	}

	public int Width
	{
		get
		{
			return width;
		}
	}

	public int ExpectedFrames
	{
		get
		{
			return expectedFrames;
		}
	}

	public static EPFImage FromFile(string file)
	{
		return LoadEPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
	}

	public static EPFImage FromRawData(byte[] data)
	{
		return LoadEPF(new MemoryStream(data));
	}

	public static EPFImage FromArchive(string file, DATArchive archive)
	{
		if (archive.Contains(file))
		{
			return FromRawData(archive.ExtractFile(file));
		}
		return null;
	}

	public static EPFImage FromArchive(string file, bool ignoreCase, DATArchive archive)
	{
		if (archive.Contains(file, ignoreCase))
		{
			return FromRawData(archive.ExtractFiles(file, ignoreCase));
		}
		return null;
	}

	private static EPFImage LoadEPF(Stream stream)
	{
		stream.Seek(0L, SeekOrigin.Begin);
		BinaryReader binaryReader = new BinaryReader(stream);
		EPFImage epfImage = new EPFImage
		{
			expectedFrames = binaryReader.ReadUInt16(),
			width = binaryReader.ReadUInt16(),
			height = binaryReader.ReadUInt16(),
			unknown = binaryReader.ReadUInt16(),
			tocAddress = binaryReader.ReadUInt32() + 12
		};
		if (epfImage.expectedFrames <= 0)
		{
			return epfImage;
		}
		epfImage.frames = new EPFFrame[epfImage.expectedFrames];
		for (int i = 0; i < epfImage.expectedFrames; i++)
		{
			binaryReader.BaseStream.Seek(epfImage.tocAddress + i * 16, SeekOrigin.Begin);
			int left = binaryReader.ReadUInt16();
			int top = binaryReader.ReadUInt16();
			ushort num3 = binaryReader.ReadUInt16();
			int num4 = binaryReader.ReadUInt16();
			int width = num3 - left;
			int height = num4 - top;
			uint num7 = binaryReader.ReadUInt32() + 12;
			uint num8 = binaryReader.ReadUInt32() + 12;
			binaryReader.BaseStream.Seek(num7, SeekOrigin.Begin);
			epfImage.rawData = ((num8 - num7 == width * height) ? binaryReader.ReadBytes((int)(num8 - num7)) : binaryReader.ReadBytes((int)(epfImage.tocAddress - num7)));
			epfImage.frames[i] = new EPFFrame(left, top, width, height, epfImage.rawData);
		}
		return epfImage;
	}

	public virtual string ToString()
	{
		return "{Frames = " + expectedFrames + ", Width = " + width + ", Height = " + height + ", TOC Address = 0x" + tocAddress.ToString("X").PadLeft(8, '0') + "}";
	}
}
