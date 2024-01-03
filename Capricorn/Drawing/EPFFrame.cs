using System.Runtime.CompilerServices;

public class EPFFrame
{
    private int left;
    private int top;
    private int width;
    private int height;
    private byte[] rawData;

    public bool Boolean_0
	{
		get
		{
			if (RawData != null && RawData.Length >= 1 && Width >= 1 && Height >= 1)
			{
				return Width * Height == RawData.Length;
			}
			return false;
		}
	}

    public byte[] RawData
    {
        get
        {
            return rawData;
        }
    }

    public int Height
    {
        get
        {
            return height;
        }
    }

    public int Width
    {
        get
        {
            return width;
        }
    }

    public int Top
    {
        get
        {
            return top;
        }
        set
        {
            top = value;
        }
    }

    public int Left
    {
        get
        {
            return left;
        }
        set
        {
            left = value;
        }
    }
    public EPFFrame(int left, int top, int width, int height, byte[] rawData)
	{
		Left = left;
		Top = top;
		this.width = width;
		this.height = height;
		this.rawData = rawData;
	}

	public virtual string ToString()
	{
		return "{X = " + Left + ", Y = " + Top + ", Width = " + Width + ", Height = " + Height + "}";
	}
}
