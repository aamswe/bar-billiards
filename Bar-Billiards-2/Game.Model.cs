using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge;
using AForge.Imaging;

namespace Bar_Billiards_2
{
    public class Game
    {

    }

    public class Mushroom : PoolObject { }

    public class Pockets : PoolObject { }

    public class Ball : PoolObject
    {
        public bool DoublePoints { get; set; }
    }

    public abstract class PoolObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public List<Point> History { get; set; }


    }
    

}
