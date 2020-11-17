using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGPhysics2
{
    public abstract class BCGObject
    {
        public string type { get; set; }
        public abstract int id { get; set; }
        public abstract float x { get; set; }
        public abstract float y { get; set; }
        public virtual float width { get; set; }
        public virtual float height { get; set; }
        public abstract float x_speed { get; set; }
        public abstract float y_speed { get; set; }
        public virtual float radius { get; set; }
        public abstract float restitution { get; set; }

        public abstract string name { get; set; }

        public abstract bool isStationary { get; set; }
        public abstract bool isGhost { get; set; }
        public abstract bool toBeRemoved { get; set; }

        abstract public float getInvMass();
        abstract public void applyImpulse(Vec2 impulse, float multiplier = 1);
        abstract public void move(Vec2 movement, float multiplier = 1);
        
        virtual public void remove()
        {
            toBeRemoved = true;
        }
    }
}
