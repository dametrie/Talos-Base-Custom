using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Structs
{
    internal struct Rectangle
    {
        internal short X
        {
            get;
            set;
        }

        internal short Y
        {
            get;
            set;
        }

        internal short Width
        {
            get;
            set;
        }

        internal short Height
        {
            get;
            set;
        }

        internal short Left => Y;

        internal short Top => X;

        internal short Right => (short)(X + Width);

        internal short Bottom => (short)(Y + Height);

        internal List<Location> LocationList
        {
            get
            {
                List<Location> list = new List<Location>();
                for (int i = X; i <= X + Width; i++)
                {
                    for (int j = Y; j <= Y + Height; j++)
                    {
                        list.Add(new Location(0, (short)i, (short)j));
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

        internal Rectangle(short x, short y, short width, short height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal Rectangle(Point location, Point size) : this()
        {
            X = location.X;
            Y = location.Y;
            Width = size.X;
            Height = size.Y;
        }

        public static bool Equal(Rectangle rect1, Rectangle rect2)
        {
            return rect1.Equals(rect2);
        }

        public static bool NotEqual(Rectangle rect1, Rectangle rect2)
        {
            return !rect1.Equals(rect2);
        }

        internal bool method_0(Point location)
        {
            return method_1(location.X, location.Y);
        }

        internal bool method_1(short x, short y)
        {
            if (x >= this.X && x < this.X + Width && y >= this.Y)
            {
                return y < this.Y + Height;
            }
            return false;
        }

        internal bool method_2(Rectangle rect)
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
