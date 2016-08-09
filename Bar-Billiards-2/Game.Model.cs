using System;
using AForge.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Bar_Billiards_2
{
    public class Game
    {
        public List<Player> Players { get; set; }
        public List<Mushroom> Mushrooms { get; set; }
        public List<Pocket> Pockets { get; set; }
        public List<Ball> Balls { get; set; }

        public Game()
        {
            Players = new List<Player>();
            Mushrooms = new List<Mushroom>();
            Pockets = new List<Pocket>();
            Balls = new List<Ball>();
        }

        public void AddOrUpdateBall(Blob blob)
        {
            if (IsBall(blob.Rectangle))
            {
                var ballFound = false;
                foreach (var ball in Balls)
                {
                    if (!Rectangle.Intersect(blob.Rectangle, ball.rectangle).IsEmpty)
                    {
                        var blobArea = blob.Rectangle.Width * blob.Rectangle.Height;
                        var ballArea = ball.rectangle.Width * ball.rectangle.Height;
                        if (Math.Abs(blobArea - ballArea) < 0.1)
                        {
                            ball.History.Add(ball.rectangle);
                            ball.rectangle = blob.Rectangle;
                        }
                    }
                }
                if (!ballFound)
                {
                    // todo detect red ball
                    Balls.Add(new Ball { Id = Balls.Max(b => b.Id) + 1, DoublePoints = IsRed(blob), rectangle = blob.Rectangle });
                }
            }
        }

        private bool IsRed(Blob blob)
        {
            return false;
        }

        private bool IsBall(Rectangle rect)
        {
            foreach (var mushroom in Mushrooms)
            {
                var intersect = Rectangle.Intersect(mushroom.rectangle, rect);
                if (!intersect.IsEmpty)
                {
                    var blobArea = intersect.Width*intersect.Height;
                    var mushroomArea = mushroom.rectangle.Width * mushroom.rectangle.Height;
                    if (mushroomArea * 0.9 < blobArea && mushroomArea * 1.1 > blobArea)
                    {
                        return false;
                    }
                }
            }

            foreach (var pocket in Pockets)
            {
                var intersect = Rectangle.Intersect(pocket.rectangle, rect);
                if (!intersect.IsEmpty)
                {
                    var blobArea = intersect.Width * intersect.Height;
                    var pocketArea = pocket.rectangle.Width * pocket.rectangle.Height;
                    if (pocketArea * 0.9 < blobArea && pocketArea * 1.1 > blobArea)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public class Player
    {
        public bool Current { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Bank { get; set; }
    }
    
    public class Mushroom : PoolObject
    {
        public bool Black { get; set; }
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
        public int Id { get; set; }
        public Rectangle rectangle { get; set; }
        public List<Rectangle> History { get; set; }

        public PoolObject()
        {
            History = new List<Rectangle>();
        }
    }
    
}
