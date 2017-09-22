using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Physics
{
    public class BallMotionPutt : BallPhysics
    {
        public const float BounceSpeed = 0.08f;

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

            bool SkipPackage = false;
            SlopePackage package = new SlopePackage();

            for (int i = 0; i < 250; i++)
            {
                if(SkipPackage == false)
                    package = GetAccelTowards(rp.point, rp.velocity.normalized, 0f, rp.velocity.magnitude);

                if (package.detected)
                {
                    //detected ground

                    if (gamemode.PositionsForAllCurrentHoles.Any(x => Vector3.Distance(rp.point, x) < 0.125f))
                    {
                        if (rp.velocity.ToFlatVector3().magnitude < BounceSpeed)
                        {
                            package.detected = false;
                            SkipPackage = true;
                            rp.inHole = true;
                        }
                        else if (rp.velocity.ToFlatVector3().magnitude > BounceSpeed)
                        {
                            float vertical = Math.InverseNormalizeRange(rp.velocity.ToFlatVector3().magnitude, BounceSpeed, 1f, 0.02f, 0.1f);
                            rp.velocity /= 2;

                            rp.velocity.Set(rp.velocity.x, vertical, rp.velocity.z);
                        }
                    }

                    rp.grounded = true;
                    rp.point = package.hit.point;
                    rail.Add(rp.Copy()); //add a point where it touched the ground
                    rp.velocity = Bounce(rp.velocity, package.normal, package.groundType.restitution, package.groundType.friction); //calculate bounce                              

                    if (rp.velocity.y < 0.1f)
                        rp.clamped = true;
                }
                else
                {
                    rp.grounded = false;

                    rp.point += rp.velocity;

                    rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y - 0.0381f, rp.velocity.z); //gravity
                    rp.velocity *= 1 - (0.05f * rp.velocity.magnitude); //drag

                    if (rp.velocity.y > 0.1f)
                        rp.clamped = false;

                    rail.Add(rp.Copy());
                }

                if (rp.grounded && rp.velocity.magnitude < 0.05f)
                    break;
            }

            RailPoint last = rp.Copy();
            last.frozen = true;

            rail[rail.Count - 1] = last;

            return rail.ToArray();
        }
    }
}

