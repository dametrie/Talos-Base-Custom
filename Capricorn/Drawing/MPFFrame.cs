using System.Runtime.CompilerServices;

public class MPFFrame
{
	private int left;
	private int top;
	private int width;
	private int height;
	private byte[] rawData;
	private int xOffset;
	private int yOffset;


	public bool IsValid
	{
		get
		{
			if (rawData != null && rawData.Length >= 1 && Width >= 1 && height >= 1)
			{
				return Width * height == rawData.Length;
			}
			return false;
		}
	}

	public int OffsetY
	{
		get;
	}

	public int OffsetX
	{
		get;
	}

	public byte[] RawData
	{
		get;
	}

	public int Height
	{
		get;
	}

	public int Width
	{
		get;
	}

	public int Top
	{
		get;
		set;
	}

	public int Left
	{
		get;
		set;
	}

	public MPFFrame(int left, int top, int width, int height, int xOffset, int yOffset, byte[] rawData)
	{
		Left = left;
		Top = top;
		Width = width;
		Height = height;
		OffsetX = xOffset;
		OffsetY = yOffset;
		RawData = rawData;
	}

	public virtual string ToString()
	{
		return "{X = " + Left + ", Y = " + Top + ", Width = " + Width + ", Height = " + height + ", Offset = (" + OffsetX + ", " + OffsetY + ")}";
	}
}
