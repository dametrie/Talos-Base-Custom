using System;
using System.Collections.Generic;

namespace Talos.Structs
{
    internal struct Rectangle
    {
        internal short MapID { get; set; }
        internal short X { get; set; }
        internal short Y { get; set; }
        internal short Width { get; set; }
        internal short Height { get; set; }
        internal short Left => Y;

        internal short Top => X;

        internal short Right => (short)(X + Width);

        internal short Bottom => (short)(Y + Height);

        internal List<Location> Points
        {
            get
            {
                List<Location> list = new List<Location>();
                for (int i = X; i <= X + Width; i++)
                {
                    for (int j = Y; j <= Y + Height; j++)
                    {
                        list.Add(new Location(MapID, (short)i, (short)j));
                    }
                }
                return list;
            }
        }

        internal static Location[] CreateRectangleLocations(short mapId, short x, short y, short width, short height)
        {
            List<Location> list = new List<Location>();
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    list.Add(new Location(mapId, (short)i, (short)j));
                }
            }
            return list.ToArray();
        }

        internal Rectangle(short mapID, short x, short y, short width, short height) : this()
        {
            MapID = mapID;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal Rectangle(Point location, Size size) : this()
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }
        internal Rectangle(Location location, Size size) : this()
        {
            MapID = location.MapID;
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public static bool Equal(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Equals(rect2);
        }

        public static bool NotEqual(Rectangle rect1, Rectangle rect2)
        {
            return !rect1.Equals(rect2);
        }

        internal bool ContainsPoint(Point location)
        {
            return ContainsCoordinates(location.X, location.Y);
        }

        internal bool ContainsCoordinates(short x, short y)
        {
            if (x >= X && x < X + Width && y >= Y)
            {
                return y < Y + Height;
            }
            return false;
        }

        internal bool ContainsRectangle(Rectangle rect)
        {
            if (rect.X >= X && rect.Y >= Y && rect.X + rect.Width <= X + Width)
            {
                return rect.Y + rect.Height <= Y + Height;
            }
            return false;
        }

        public bool Equals(object obj)
        {
            if (!(obj is Rectangle))
            {
                return false;
            }
            Rectangle rectangle = (Rectangle)obj;
            if (rectangle.X == X && rectangle.Y == Y && rectangle.Width == Width)
            {
                return rectangle.Height == Height;
            }
            return false;
        }

        public int GetHashCode()
        {
            return (X << 24) + (Y << 16) + (Width << 8) + Height;
        }

        public string ToString()
        {
            return $"{X},{Y} {Width}x{Height}";
        }

        public static explicit operator Rectangle(System.Drawing.Rectangle v)
        {
            throw new NotImplementedException();
        }

        public static explicit operator System.Drawing.RectangleF(Rectangle v)
        {
            throw new NotImplementedException();
        }
    }
}
