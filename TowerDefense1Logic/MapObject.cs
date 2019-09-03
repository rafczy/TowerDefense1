using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TowerDefense1Logic
{
    [DebuggerDisplay("{Location.X},{Location.Y}")]
    public abstract class MapObject
    {
        public Point Location { get; set; }

        public MapObject()
        {
            Location = new Point(-1, -1);
        }

        public static bool IsOnMap(Point p)
        {
            return p.X >= 0 && p.Y >= 0;
        }

        public bool IsOnMap()
        {
            return IsOnMap(this.Location);
        }

    }
}
