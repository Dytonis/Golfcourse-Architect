using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Physics
{
    public class BallMotionPutt : BallPhysics
    {
        public override RailPoint[] CalculateRail(Vector3 startingPosition, Vector3 startingVelocity, float spin, float sideSpin)
        {
            //raycast from normal velocity by ball size / 2
            //if hit, add point on hit posision and update velocity

            List<RailPoint> rail = new List<RailPoint>();

            RailPoint rp = new RailPoint()
            {
                point = startingPosition,
                velocity = startingVelocity
            };

            for (int i = 0; i < 250; i++)
            {
                Vector3 oldVelocity = rp.velocity;

                rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y - 0.0381f, rp.velocity.z); //gravity
                rp.velocity *= 1 - (0.05f * rp.velocity.magnitude); //drag

                SlopePackage package = GetAccelTowards(rp.point, rp.velocity.normalized, rp.velocity.magnitude / 2, rp.velocity.magnitude);

                if (package.detected)
                {
                    //detected ground

                    if (rp.velocity.y >= -0.2f)
                    {
                        //dont bounce, roll
                        rp.velocity = new Vector3(rp.velocity.x, 0, rp.velocity.z); //remove vertical speed
                        rp.velocity += package.dirNormalized * (package.magnitude / 300); //accel gained from slope
                        rp.velocity *= package.groundType.friction; //friction
                        rp.bouncing = false;
                    }
                    else
                    {
                        rp.velocity = Bounce(rp.velocity, package.normal, package.groundType.restitution, package.groundType.friction); //calculate bounce                  
                        rp.bouncing = true;
                    }

                    rp.point = package.hit.point; //set the position to where it hit the ground
                    rp.grounded = true;

                    RailPoint groundPoint = rp.Copy();
                    groundPoint.velocity = oldVelocity;

                    rail.Add(groundPoint); //add a point where it touched the ground
                }
                else
                {
                    rp.grounded = false;
                }
                Debug.DrawRay(rp.point, rp.velocity, Color.black, 1f);
                RailPoint point = rp.Copy();
                point.velocity = oldVelocity;

                if (rp.velocity.magnitude < 0.03f && rp.grounded)
                {
                    rp.frozen = true;
                    rail.Add(rp.Copy());
                    break;
                }

                rail.Add(point); //add a point

                rp.point += rp.velocity;
            }

            return rail.ToArray();
        }
    }
}
