using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGPhysics2
{
    public class Collision
    {
        public enum collisionType { rectangles, circles, both };

        public collisionType collType { get; set; }

        public int firstId { get; set; }
        public int secondId { get; set; }

        public Collision(int id1, int id2, collisionType CT)
        {
            collType = CT;
            firstId = id1;
            secondId = id2;
        }

        public override bool Equals(object obj)
        {
            Collision coll = (Collision)obj;

            if(collType == coll.collType && firstId == coll.firstId && secondId == coll.secondId) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return (int)(collType + 1) * (firstId + 1) * (secondId + 1);
        }
    }
}