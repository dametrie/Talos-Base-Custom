using System.IO;
using System.Runtime.CompilerServices;

public class MPFImage
{
	public byte walkStart;

	public byte walkLength;

	public byte attack1Start;

	public byte attack1Length;

	public byte idleStart;

	public byte idleLength;

	public ushort idleSpeed;

	public byte attack2Start;

	public byte attack2Length;

	public byte attack3Start;

	public byte attack3Length;

	public string palette;

	private MPFFrame[] frames;

	public MPFFrame this[int index]
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

	public bool isFFFormat
	{
		get;
		private set;
	}

	public bool isnewFormat
	{
		get;
		private set;
	}

	public uint ffUnkown
	{
		get;
		private set;
	}

	public uint expectedDataSize
	{
		get;
		private set;
	}

	public MPFFrame[] Frames
	{
		get
		{
			return frames;
		}
	}

	public int height
	{
		get;
		private set;
	}

	public int width
	{
		get;
		private set;
	}

	public int expectedFrames
	{
		get;
		private set;
	}

	public virtual string ToString()
	{
		return $"{{Frames = {expectedFrames}, Width = {width}, Height = {height}, WalkStart = {walkStart}, WalkLength = {walkLength}, Attack1Start = {attack1Start}, Attack1Length = {attack1Length}, IdleStart = {idleStart}, IdleLength = {idleLength}}}";
	}

	public static MPFImage FromFile(string file)
	{
		return LoadMPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
	}

	public static MPFImage FromRawData(byte[] data)
	{
		return LoadMPF(new MemoryStream(data));
	}

	public static MPFImage FromArchive(string file, DATArchive archive)
	{
		if (archive.Contains(file))
		{
			return FromRawData(archive.ExtractFile(file));
		}
		return null;
	}

	public static MPFImage FromArchive(string file, bool ignoreCase, DATArchive archive)
	{
		if (archive.Contains(file, ignoreCase))
		{
			return FromRawData(archive.ExtractFiles(file, ignoreCase));
		}
		return null;
	}

	private static MPFImage LoadMPF(Stream stream)
	{
		stream.Seek(0L, SeekOrigin.Begin);
		BinaryReader binaryReader = new BinaryReader(stream);
		MPFImage mpfImage = new MPFImage();
		if (binaryReader.ReadUInt32() == uint.MaxValue)
		{
			mpfImage.isFFFormat = true;
			mpfImage.ffUnkown = binaryReader.ReadUInt32();
		}
		else
		{
			binaryReader.BaseStream.Seek(-4L, SeekOrigin.Current);
		}
		mpfImage.expectedFrames = binaryReader.ReadByte();
		mpfImage.frames = new MPFFrame[mpfImage.expectedFrames];
		mpfImage.width = binaryReader.ReadUInt16();
		mpfImage.height = binaryReader.ReadUInt16();
		mpfImage.expectedDataSize = binaryReader.ReadUInt32();
		mpfImage.walkStart = binaryReader.ReadByte();
		mpfImage.walkLength = binaryReader.ReadByte();
		mpfImage.isnewFormat = (binaryReader.ReadUInt16() == ushort.MaxValue);
		if (mpfImage.isnewFormat)
		{
			mpfImage.idleStart = binaryReader.ReadByte();
			mpfImage.idleLength = binaryReader.ReadByte();
			mpfImage.idleSpeed = binaryReader.ReadUInt16();
			mpfImage.attack1Start = binaryReader.ReadByte();
			mpfImage.attack1Length = binaryReader.ReadByte();
			mpfImage.attack2Start = binaryReader.ReadByte();
			mpfImage.attack2Length = binaryReader.ReadByte();
			mpfImage.attack3Start = binaryReader.ReadByte();
			mpfImage.attack3Length = binaryReader.ReadByte();
		}
		else
		{
			binaryReader.BaseStream.Seek(-2L, SeekOrigin.Current);
			mpfImage.attack1Start = binaryReader.ReadByte();
			mpfImage.attack1Length = binaryReader.ReadByte();
			mpfImage.idleStart = binaryReader.ReadByte();
			mpfImage.idleLength = binaryReader.ReadByte();
			mpfImage.idleSpeed = binaryReader.ReadUInt16();
		}
		long num = binaryReader.BaseStream.Length - mpfImage.expectedDataSize;
		for (int i = 0; i < mpfImage.expectedFrames; i++)
		{
			int left = binaryReader.ReadUInt16();
			int top = binaryReader.ReadUInt16();
			int num4 = binaryReader.ReadUInt16();
			ushort num5 = binaryReader.ReadUInt16();
			int width = num4 - left;
			int height = num5 - top;
			int num8 = binaryReader.ReadUInt16();
			int num9 = binaryReader.ReadUInt16();
			int xOffset = (num8 % 256 << 8) + num8 / 256;
			int yOffset = (num9 % 256 << 8) + num9 / 256;
			long num10 = binaryReader.ReadUInt32();
			if (left == 65535 && num4 == 65535)
			{
				mpfImage.palette = $"mns{num10:D3}.pal";
				int num11 = --mpfImage.expectedFrames;
			}
			else
			{
				mpfImage.palette = "mns000.pal";
			}
			byte[] rawData = null;
			if (height > 0 && width > 0)
			{
				long position = binaryReader.BaseStream.Position;
				binaryReader.BaseStream.Seek(num + num10, SeekOrigin.Begin);
				rawData = binaryReader.ReadBytes(height * width);
				binaryReader.BaseStream.Seek(position, SeekOrigin.Begin);
			}
			mpfImage.frames[i] = new MPFFrame(left, top, width, height, xOffset, yOffset, rawData);
		}
		binaryReader.Close();
		return mpfImage;
	}
}
