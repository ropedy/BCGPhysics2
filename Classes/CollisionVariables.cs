using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BCGPhysics2
{
    public class CollisionVariables
    {
        public Vec2 normal { get; set; }
        public float penetration { get; set; }

        public CollisionVariables(BCGRectangle A, BCGRectangle B)
        {
            // Calculate vector from second rectangles center to first rectangles center
            Vec2 centerVector = new Vec2(B.getCenter().x - A.getCenter().x, B.getCenter().y - A.getCenter().y);

            // Calculate overlap lengths
            float x_overlap = A.width / 2 + B.width / 2 - Math.Abs(centerVector.x);
            float y_overlap = A.height / 2 + B.height / 2 - Math.Abs(centerVector.y);

            // Set correct normal depending on direction of collision
            // Penetration will be the smaller overlap value
            if(x_overlap > y_overlap)
            {
                if(centerVector.y < 0) normal = new Vec2(0, -1);
                else normal = new Vec2(0, 1);

                penetration = y_overlap;
            }
            else
            {
                if(centerVector.x < 0) normal = new Vec2(-1, 0);
                else normal = new Vec2(1, 0);

                penetration = x_overlap;
            }
        }

        public CollisionVariables(BCGCircle A, BCGCircle B)
        {
            // Calculate vector from second circles center to first circles center
            Vec2 centerVector = new Vec2(B.x - A.x, B.y - A.y);

            // Calculate total radius and distance(from centers) of circles
            float radii = A.radius + B.radius;
            float distance = (float)Math.Sqrt(Math.Pow(centerVector.x, 2) + Math.Pow(centerVector.y, 2));

            // Normalize normal and calculate penetration
            // If distance = 0 setup fill normal and set penetration as bigger radius
            if(distance != 0)
            {
                penetration = radii - distance;

                normal = new Vec2(centerVector.x / distance, centerVector.y / distance);
            }
            else
            {
                penetration = Math.Max(A.radius, B.radius);
                normal = new Vec2(1, 0);
            }
        }

        public CollisionVariables(BCGObject A, BCGObject B)
        {
            BCGRectangle R;
            BCGCircle C;

            if(A.type == "rectangle")
            {
                R = (BCGRectangle)A;
                C = (BCGCircle)B;
            }
            else
            {
                R = (BCGRectangle)B;
                C = (BCGCircle)A;
            }

            // Calculate vector from circles center to rectangles center
            Vec2 centerVector = new Vec2(C.x - R.getCenter().x, C.y - R.getCenter().y);

            // Calculate vector from [closest point of rectangles sides to circles center] to rectangles center
            Vec2 closest = new Vec2(centerVector.x, centerVector.y);
            closest = clamp(closest, R.width / 2, R.height / 2);

            bool inside = false;

            // If circles center is inside the rectangle, the center of circle gets moved to the closest edge of the rectangle
            if(centerVector.x == closest.x && centerVector.y == closest.y)
            {
                inside = true;

                if(Math.Abs(centerVector.x) > Math.Abs(centerVector.y))
                {
                    closest.x = R.width / -2;
                    if(closest.x > 0) closest.x *= -1;
                }
                else
                {
                    closest.y = R.height / -2;
                    if(closest.y > 0) closest.y *= -1;
                }
            }

            // Calculate normal and penetration
            Vec2 norm = new Vec2(centerVector.x - closest.x, centerVector.y - closest.y);
            float distance = (float)Math.Sqrt((Math.Pow(norm.x, 2) + Math.Pow(norm.y, 2)));

            if(inside) normal = new Vec2(-norm.x, -norm.y);
            else normal = new Vec2(norm.x, norm.y);
            normal.normalize();

            penetration = C.radius - distance;
        }

        // Limit c-vectors length to maximum of w(x-axis) and h(y-axis)
        private Vec2 clamp(Vec2 c, float w, float h)
        {
            float x, y;

            if(c.x > w) x = w;
            else if(c.x < -w) x = -w;
            else x = c.x;

            if(c.y > h) y = h;
            else if(c.y < -h) y = -h;
            else y = c.y;

            return new Vec2(x, y);
        }
    }
}