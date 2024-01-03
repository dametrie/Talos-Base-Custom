using System.Runtime.CompilerServices;

public class DATFileEntry

{
	private string name;
	private long startAddress;
	private long endAddress;

    public long FileSize
    {
        get
        {
            return endAddress - startAddress;
        }
    }

    public long EndAddress
    {
        get
        {
            return endAddress;
        }
    }

    public long StartAddress
    {
        get
        {
            return startAddress;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public DATFileEntry(string name, long long_2, long long_3)
	{
		Name = name;
		this.startAddress = long_2;
		this.endAddress = long_3;
	}

	public virtual string ToString()
	{
		return "{Name = " + Name + ", Size = " + FileSize.ToString("###,###,###,###,###0") + "}";
	}
}
