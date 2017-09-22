using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Physics
{
    public class BallMotionStandard : BallPhysics
    {
        public override RailPoint[] CalculateRail(Vector3 startingPosition, Vector3 startingVelocity, float spin, float sideSpin)
        {
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
                if (SkipPackage == false)
                    package = GetAccelTowards(rp.point, rp.velocity.normalized, 0f, rp.velocity.magnitude);

                if (package.detected)
                {
                    //detected ground

                    if (gamemode.PositionsForAllCurrentHoles.Any(x => Vector3.Distance(rp.point, x) < 0.125f))
                    {
                        if (rp.velocity.ToFlatVector3().magnitude < 0.08f)
                        {
                            package.detected = false;
                            SkipPackage = true;
                            rp.inHole = true;
                        }
                        else if (rp.velocity.ToFlatVector3().magnitude > 0.08f)
                        {
                            float vertical = Math.InverseNormalizeRange(rp.velocity.ToFlatVector3().magnitude, 0.08f, 1f, 0.02f, 0.1f);
                            rp.velocity /= 2;

                            rp.velocity.Set(rp.velocity.x, vertical, rp.velocity.z);
                        }
                    }

                    rp.grounded = true;
                    rp.point = package.hit.point;
                    rail.Add(rp.Copy()); //add a point where it touched the ground
                    rp.velocity = Bounce(rp.velocity, package.normal, package.groundType.restitution, package.groundType.friction); //calculate bounce                              
                }
                else
                {
                    rp.grounded = false;

                    rp.point += rp.velocity;

                    rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y - 0.0381f, rp.velocity.z); //gravity
                    rp.velocity *= 1 - (0.05f * rp.velocity.magnitude); //drag
                    Vector3 horzVelocity = new Vector3(rp.velocity.x, 0, rp.velocity.z);
                    rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y + (horzVelocity.magnitude / 10), rp.velocity.z); //lift

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
