using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGPhysics2
{
    static public class CollisionDetector
    {
        // Check if two rectangles collides
        static public bool RectangleXRectangle(BCGRectangle A, BCGRectangle B)
        {
            if(A.x + A.width < B.x || A.x > B.x + B.width) return false;
            if(A.y + A.height < B.y || A.y > B.y + B.height) return false;

            return true;
        }

        // Check if two circles collides
        static public bool CircleXCircle(BCGCircle A, BCGCircle B)
        {
            float middleDistance = (float)(Math.Pow((A.x - B.x), 2) + Math.Pow((A.y - B.y), 2));
            float totalRadii = A.radius + B.radius;
            totalRadii *= totalRadii;

            if(middleDistance > totalRadii) return false;
            
            return true;
        }

        // Check if rectangle and circle collides
        static public bool RectangleXCircle(BCGObject A, BCGObject B)
        {
            float distanceX = Math.Abs((A.x + A.width / 2) - B.x);
            float distanceY = Math.Abs((A.y + A.height / 2) - B.y);

            if(distanceX > (A.width / 2 + B.radius)) return false;
            if(distanceY > (A.height / 2 + B.radius)) return false;

            if(distanceX <= (A.width / 2)) return true;
            if(distanceY <= (A.height / 2)) return true;

            float cornerDistance = (float)(Math.Pow((distanceX - A.width / 2), 2) + Math.Pow((distanceY - A.height / 2), 2));

            return cornerDistance <= Math.Pow(B.radius, 2);
        }
    }
}