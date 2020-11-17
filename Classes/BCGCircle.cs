using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGPhysics2
{
    public class BCGCircle : BCGObject
    {  
        override public int id { get; set; }
        override public float x { get; set; }
        override public float y { get; set; }
        override public float radius { get; set; }
        override public float x_speed { get; set; }
        override public float y_speed { get; set; }
        override public float restitution { get; set; }

        override public string name { get; set; }

        override public bool isStationary { get; set; }
        override public bool isGhost { get; set; }
        override public bool toBeRemoved { get; set; }

        public BCGCircle(float X, float Y, float RADIUS, int ID)
        {
            type = "circle";
            x = X;
            y = Y;
            radius = RADIUS;
            id = ID;
            x_speed = 0;
            y_speed = 0;
            restitution = .2f;

            isStationary = false;
            isGhost = false;
            toBeRemoved = false;

            name = "unknown";
        }

        public float getMass()
        {
            return (float)Math.Sqrt((Math.PI * Math.Pow(radius, 2)));
        }

        override public float getInvMass()
        {
            if(isStationary) return 0;
            return (float)(1 / Math.Sqrt((Math.PI * Math.Pow(radius, 2))));
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