using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BCGPhysics2
{
    public class World
    {
        // Events
        public event EventHandler<CollisionEventArgs> collisionStart;
        public event EventHandler<CollisionEventArgs> collisionEnd;

        // Variables
        private float gravity { get; set; }
        private int runningId { get; set; }

        // Lists for objects of the world
        public List<BCGObject> objects = new List<BCGObject>();

        // List for collisions
        private List<Collision> collisions = new List<Collision>();
        private List<Collision> oldCollisions = new List<Collision>();

        public World(float grav)
        {
            gravity = grav;
            runningId = 0;
        }

        #region ADDING OBJECTS TO THE WORLD
        // Add rectangle to the world
        public BCGRectangle addObject(float x, float y, float width, float height)
        {
            // Add new rectangle with given settings
            BCGRectangle rectangle = new BCGRectangle(x, y, width, height, runningId);
            objects.Add(rectangle);

            // Increment runningId
            runningId++;

            return rectangle;
        }

        // Add circle to the world
        public BCGCircle addObject(float x, float y, float radius)
        {
            // Add new circle with given settings
            BCGCircle circle = new BCGCircle(x, y, radius, runningId);
            objects.Add(circle);

            // Increment runningId
            runningId++;

            return circle;
        }
        #endregion

        #region UPDATE WORLD
        // Updates world for 1/60 of the second
        public void update()
        {
            // Update position of rectangels
            foreach(BCGObject o in objects)
            {
                if(!o.isStationary)
                {
                    // Add gravity for y-axis. Gravity to added = gravity / (1 / 2) * (1 / 60)^2 = gravity / 7200
                    o.y_speed += gravity / 7200;

                    // Update positions
                    o.y += o.y_speed;
                    o.x += o.x_speed;
                }
            }

            checkCollisions();
            handleCollisions();
            removeObjects();
            raiseEvents();
            clearCollisions();
        }
        #endregion

        #region COLLISION CHECKS
        private void checkCollisions()
        {
            int objectCount = objects.Count;

            for(int i = 0 ; i < objectCount ; i++)
            {
                for(int j = i + 1 ; j < objectCount ; j++)
                {
                    int id1 = objects[i].id, id2 = objects[j].id;
                    if(objects[i].type == "rectangle" && objects[j].type == "rectangle")
                    {
                        if(CollisionDetector.RectangleXRectangle((BCGRectangle)objects[i], (BCGRectangle)objects[j]))
                            collisions.Add(new Collision(id1, id2, Collision.collisionType.rectangles));
                    }
                    else if(objects[i].type == "circle" && objects[j].type == "circle")
                    {
                        if(CollisionDetector.CircleXCircle((BCGCircle)objects[i], (BCGCircle)objects[j]))
                            collisions.Add(new Collision(id1, id2, Collision.collisionType.circles));
                    }
                    else if(objects[i].type == "rectangle")
                    {
                        if(CollisionDetector.RectangleXCircle((BCGRectangle)objects[i], (BCGCircle)objects[j]))
                            collisions.Add(new Collision(id1, id2, Collision.collisionType.both));
                    }
                    else
                    {
                        if(CollisionDetector.RectangleXCircle((BCGRectangle)objects[j], (BCGCircle)objects[i]))
                            collisions.Add(new Collision(id2, id1, Collision.collisionType.both));
                    }
                }
            }
        }
        #endregion

        #region COLLISON HANDLING
        private void handleCollisions()
        {
            foreach(Collision C in collisions)
            {
                // Get objects
                BCGObject O1 = (BCGObject)objects[getIndex(C.firstId)];
                BCGObject O2 = (BCGObject)objects[getIndex(C.secondId)];

                // Ignore of both are stationaty or another is ghost
                if(O1.isStationary && O2.isStationary) continue;
                if(O1.isGhost || O2.isGhost) continue;

                // Get collision variables
                CollisionVariables CV;
                if(C.collType == Collision.collisionType.rectangles) CV = new CollisionVariables((BCGRectangle)O1, (BCGRectangle)O2);
                else if(C.collType == Collision.collisionType.circles) CV = new CollisionVariables((BCGCircle)O1, (BCGCircle)O2);
                else CV = new CollisionVariables((BCGObject)O1, (BCGObject)O2);

                // Calculate relative speed and perform dot product with it and collision normal
                Vec2 relativeSpeed = new Vec2((O2.x_speed - O1.x_speed), (O2.y_speed - O1.y_speed));
                float velAlongNormal = (relativeSpeed.x * CV.normal.x) + (relativeSpeed.y * CV.normal.y);

                // Ignore collision of objects are seperating
                if(velAlongNormal > 0) continue;

                // Calculate restitution
                float restitution = Math.Min(O1.restitution, O2.restitution);

                // Calculate impulse scalar and impulse vector
                float impulseScalar = -(1 + restitution) * velAlongNormal;
                impulseScalar /= O1.getInvMass() + O2.getInvMass();

                Vec2 impulse = new Vec2(CV.normal.x * impulseScalar, CV.normal.y * impulseScalar);

                // Apply impulse
                O1.applyImpulse(impulse.negative(), O1.getInvMass());
                O2.applyImpulse(impulse, O2.getInvMass());
                
                // Calculate and apply positional correction
                float percent = .2f;
                float slop = .1f;
                float peneSlop = CV.penetration - slop;
                float corrX = (Math.Max(peneSlop, .0f) / (O1.getInvMass() + O2.getInvMass())) * percent * CV.normal.x;
                float corrY = (Math.Max(peneSlop, .0f) / (O1.getInvMass() + O2.getInvMass())) * percent * CV.normal.y;
                Vec2 correction = new Vec2(corrX, corrY);

                O1.move(correction.negative(), O1.getInvMass());
                O2.move(correction, O2.getInvMass());
            }
        }
        #endregion

        #region REMOVE OBJECTS
        public void removeObjects()
        {
            List<BCGObject> removeList = objects.Where(y => y.toBeRemoved).ToList();

            if(removeList.Count > 0)
            {
                collisions.RemoveAll(x => removeList.Any(y => y.id == x.firstId) || removeList.Any(y => y.id == x.secondId));
                oldCollisions.RemoveAll(x => removeList.Any(y => y.id == x.firstId) || removeList.Any(y => y.id == x.secondId));
                objects.RemoveAll(x => x.toBeRemoved);
            }
        }
        #endregion

        #region RAISE EVENTS
        private void raiseEvents()
        {
            List<Collision> startingCollisions = new List<Collision>(collisions.Except(oldCollisions));

            // New collisions
            foreach(Collision C in startingCollisions)
            {
                EventHandler<CollisionEventArgs> collisionStartHandler = collisionStart;
                if(collisionStartHandler != null) collisionStartHandler(this, new CollisionEventArgs(C, objects[getIndex(C.firstId)], objects[getIndex(C.secondId)]));
            }

            List<Collision> endingCollisions = new List<Collision>(oldCollisions.Except(collisions));

            // Old collisions
            foreach(Collision C in endingCollisions)
            {
                EventHandler<CollisionEventArgs> collisionEndHandler = collisionEnd;
                if(collisionEndHandler != null) collisionEndHandler(this, new CollisionEventArgs(C, objects[getIndex(C.firstId)], objects[getIndex(C.secondId)]));
            }
        }
        #endregion

        #region CLEAR COLLISIONS
        private void clearCollisions()
        {
            oldCollisions = new List<Collision>(collisions);
            collisions.Clear();
        }
        #endregion

        public BCGObject lastAdded()
        {
            return objects[objects.Count - 1];
        }

        private int getIndex(int objectId)
        {
            return objects.FindIndex(x => x.id == objectId);
        }
    }
}