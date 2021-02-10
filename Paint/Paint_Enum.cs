using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    public enum Laser_Style
    {
        Point = 0x01,
        Line = 0x02
    };

    public struct line_struct
    {
        public float RCA;
        public float RCB;
        public double angle;
    }
}
