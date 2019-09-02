using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TowerDefense1Logic
{
    public class Turret: MapObject
    {
        public char Key { get; set; }
        public int AttackRange { get; set; }
        public int ShotFreq { get; set; }
        public int Damage { get; set; } = 1;

        

        public bool IsInRange(Point location)
        {
            return true;
        }
    }
}
