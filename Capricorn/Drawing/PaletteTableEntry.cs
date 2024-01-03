using System.Runtime.CompilerServices;

public class PaletteTableEntry
{
	[CompilerGenerated]
	private int int_0;

	[CompilerGenerated]
	private int int_1;

	[CompilerGenerated]
	private int int_2;

	public int Palette
	{
		get;
		set;
	}

	public int Max
	{
		get;
		set;
	}

	public int Min
	{
		get;
		set;
	}

	public PaletteTableEntry(int int_3, int int_4, int int_5)
	{
		Min = int_3;
		Max = int_4;
		Palette = int_5;
	}

	public virtual string ToString()
	{
		return "{Min = " + Min + ", Max = " + Max + ", Palette = " + Palette + "}";
	}
}
