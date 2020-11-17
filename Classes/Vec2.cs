using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGPhysics2
{
    public class Vec2
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vec2(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public void normalize() 
        {
            float magnitude = (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            this.x /= magnitude;
            this.y /= magnitude;
        }

        public Vec2 negative()
        {
            return new Vec2(-x, -y);
        }
    }
}