using System.Runtime.CompilerServices;

public class MapTile
{
    private ushort floor;
    private ushort leftWall;
    private ushort rightWall;

    public ushort RightWall
    {
        get
        {
            return rightWall;
        }
        set
        {
            rightWall = value;
        }
    }

    public ushort LeftWall
    {
        get
        {
            return leftWall;
        }
        set
        {
            leftWall = value;
        }
    }

    public ushort FloorTile
    {
        get
        {
            return floor;
        }
        set
        {
            floor = value;
        }
    }

    public MapTile(ushort floor, ushort leftWall, ushort rightWall)
	{
		FloorTile = floor;
		LeftWall = leftWall;
		RightWall = rightWall;
	}

	public virtual string ToString()
	{
        return "{Floor = " + floor.ToString() + ", Left Wall = " + leftWall.ToString() + ", Right Wall = " + rightWall.ToString() + "}";
    }
}
