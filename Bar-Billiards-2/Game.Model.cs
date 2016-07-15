using System.Collections.Generic;
using System.Drawing;

namespace Bar_Billiards_2
{
    public class Game
    {
        public List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; }
        public List<Mushroom> Mushrooms { get; set; }
        public List<Pocket> Pockets { get; set; }
        public List<Ball> Balls { get; set; }
    }

    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public int Bank { get; set; }
    }
    
    public class Mushroom : PoolObject
    {
        public bool FullReset { get; set; }
    }

    public class Pocket : PoolObject
    {
        public int Points { get; set; }
    }

    public class Ball : PoolObject
    {
        public bool DoublePoints { get; set; }
    }

    public abstract class PoolObject
    {
        public Point Point { get; set; }
        public Size Size { get; set; }
        public List<Rectangle> History { get; set; }
    }
    
}
