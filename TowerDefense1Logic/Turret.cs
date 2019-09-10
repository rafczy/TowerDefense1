using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TowerDefense1Logic
{
    public class Turret : MapObject
    {
        public char Key { get; set; }
        public int AttackRange { get; set; }
        public int ShotFreq { get; set; }
        public int Damage { get; set; } = 1;
        public double Angle { get; set; }



        public bool IsInRange(Point location)
        {
            bool isOnMap = this.IsOnMap() && IsOnMap(location);
            var inRange = isOnMap && (this.Location - location).Length <= AttackRange;
            return inRange;
        }

        public double AngleBetween(Point location)
        {
            Vector vector1 = (Vector)this.Location;
            Vector vector2 = (Vector)location;
            Double angleBetween;


            angleBetween = Vector.AngleBetween(vector1, vector2);

            return angleBetween;

        }
    }
}
