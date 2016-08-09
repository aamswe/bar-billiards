using System;
using AForge.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;

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

        public void AddOrUpdateBall(Blob blob, int frameId)
        {
            if (IsBall(blob))
            {
                var ballFound = false;
                foreach (var ball in Balls)
                {
                    if (ball.IsWithinToleratedDistance(new Point(blob.Rectangle.X, blob.Rectangle.Y)))
                    {
                        ballFound = true;
                        ball.frameId = frameId;
                        ball.History.Add(ball.rectangle);
                        ball.rectangle = blob.Rectangle;
                    }
                }
                if (!ballFound)
                {
                    // todo detect red ball
                    var newId = Balls.Any() ? Balls.Max(b => b.Id) + 1 : 1;
                    Balls.Add(new Ball { Id = newId, frameId = frameId, DoublePoints = IsRed(blob), rectangle = blob.Rectangle });
                }
            }
        }

        private bool IsRed(Blob blob)
        {
            return false;
        }

        private bool IsBall(Blob blob)
        {

            if (blob.Rectangle.Width < 25 && blob.Rectangle.Height < 25)
                return false;

            foreach (var item in Mushrooms)
                if (item.IsInside(new Point(blob.Rectangle.X, blob.Rectangle.Y)))
                    return false;

            foreach (var item in Pockets)
                if (item.IsInside(new Point(blob.Rectangle.X, blob.Rectangle.Y)))
                    return false;
            
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
        public int frameId { get; set; }
        public Rectangle rectangle { get; set; }
        public List<Rectangle> History { get; set; }

        public PoolObject()
        {
            History = new List<Rectangle>();
        }

        public Point GetCenterPoint()
        {
            return new Point(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
        }

        public bool IsInside(Point point)
        {
            var tolerance = 8;
            return point.X > rectangle.X - tolerance
                   && point.X < rectangle.X + tolerance
                   && point.Y > rectangle.Y - tolerance
                   && point.Y < rectangle.Y + tolerance
                ? true
                : false;
        }

        public bool IsWithinToleratedDistance(Point point)
        {
            var tolerance = 60;
            return (rectangle.X - point.X)*(rectangle.X - point.X) + (rectangle.Y - point.Y)*(rectangle.Y - point.Y) < tolerance * tolerance; 
        }
    }
    
}
