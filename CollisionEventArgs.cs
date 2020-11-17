using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGPhysics2
{
    public class CollisionEventArgs : EventArgs
    {
        public Collision collision;
        public BCGObject obj1, obj2;

        public CollisionEventArgs(Collision c, BCGObject o1, BCGObject o2)
        {
            collision = c;
            obj1 = o1;
            obj2 = o2;
        }
    }
}