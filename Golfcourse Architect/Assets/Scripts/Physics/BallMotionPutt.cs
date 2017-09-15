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

                SlopePackage package = GetAccelTowards(rp.point, rp.velocity.normalized, 0.15f, rp.velocity.magnitude + 0.3f);

                if (package.detected)
                {
                    //detected ground

                    rp.point = package.hit.point;
                    rail.Add(rp.Copy()); //add a point where it touched the ground

                    rp.velocity = Bounce(rp.velocity, package.normal, package.groundType.restitution, package.groundType.friction); //calculate bounce                              

                    rp.grounded = true;
                }
                else
                {
                    rp.grounded = false;
                }
                Debug.DrawRay(rp.point, rp.velocity, Color.black, 1f);
                RailPoint point = rp.Copy();

                if (rp.velocity.magnitude < 0.03f && rp.grounded)
                {
                    rp.frozen = true;
                    rail.Add(rp.Copy());
                    break;
                }

                rp.point += rp.velocity;

                rail.Add(point); //add a point
            }

            return rail.ToArray();
        }
    }
}
