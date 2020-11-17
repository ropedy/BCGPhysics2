using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGPhysics2
{
    public class BCGRectangle : BCGObject
    {
        override public int id { get; set; }
        override public float x { get; set; }
        override public float y { get; set; }
        override public float width { get; set; }
        override public float height { get; set; }
        override public float x_speed { get; set; }
        override public float y_speed { get; set; }
        override public float restitution { get; set; }

        override public string name { get; set; }

        override public bool isStationary { get; set; }
        override public bool isGhost { get; set; }
        override public bool toBeRemoved { get; set; }

        public BCGRectangle(float X, float Y, float WIDTH, float HEIGHT, int ID)
        {
            type = "rectangle";
            x = X;
            y = Y;
            width = WIDTH;
            height = HEIGHT;
            id = ID;
            x_speed = 0;
            y_speed = 0;
            restitution = .2f;

            isStationary = false;
            isGhost = false;
            toBeRemoved = false;

            name = "unknown";
        }

        public Vec2 getCenter() 
        {
            return new Vec2(x + width / 2, y + height / 2);
        }

        public float getMass()
        {
            return (float)Math.Sqrt(width * height);
        }

        override public float getInvMass()
        {
            if(isStationary) return 0;
            return 1 / (float)Math.Sqrt(width * height);
        }

        override public void applyImpulse(Vec2 impulse, float multiplier = 1)
        {
            x_speed += multiplier * impulse.x;
            y_speed += multiplier * impulse.y;
        }

        override public void move(Vec2 movement, float multiplier = 1)
        {
            x += multiplier * movement.x;
            y += multiplier * movement.y;
        }
    }
}