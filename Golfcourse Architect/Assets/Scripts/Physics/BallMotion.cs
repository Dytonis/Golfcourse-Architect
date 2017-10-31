//This class manages the hit detection and motion of golf balls.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Physics
{
    public class BallMotionPutt : BallPhysics
    {
        private const float gravity = 0.0381f;

        public override RailPoint[] CalculateRail(Vector3 startingPosition, Vector3 startingVelocity, float spin, float sideSpin)
        {
            //create rail point with starting position and velocity
            //start iteration loop
            ////raycast from position by length velocity
            ////if hit
            //////do hole calculation (should the ball fall in a hole)
            //////set grounded
            //////set point to hit location
            //////add a point to the rail
            //////bounce velocity
            ////else
            //////grounded is false
            //////apply drag
            ////raycast from position down by length gravity
            ////if hit
            //////set grounded
            //////set point to hit location
            //////add a point to the rail
            ////else
            //////add gravity to velocity
            ////add position to velocity
            ////add a point
            //return

            List<RailPoint> rail = new List<RailPoint>(); //create rail

            RailPoint rp = new RailPoint() //create rail point to be used by the loop
            {
                point = startingPosition,
                velocity = startingVelocity
            };

            SlopePackage package = new SlopePackage(); //the slope package to be used (hit detection info)
            SlopePackage gravityPackage = new SlopePackage(); //the slope package to be used to check if gravity should move the ball below the ground

            for(int i = 0; i < 250; i++) //iteration loop
            {
                package = GetAccelTowards(rp.point, rp.velocity.normalized, 0f, rp.velocity.magnitude); //raycast from position by length velocity

                if(package.detected) //if hit
                {
                    //hole stuff

                    rp.grounded = true;
                    rp.point = package.hit.point;
                    rail.Add(rp.Copy());
                    rp.velocity = Bounce(rp.velocity, package.normal, package.groundType.restitution, package.groundType.friction);

                    if (rp.velocity.y < 0.1f)
                        rp.clamped = true;
                }
                else
                {
                    rp.grounded = false;
                    rp.velocity *= 1 - (0.05f * rp.velocity.magnitude);

                    if (rp.velocity.y < 0.1f)
                        rp.clamped = true;

                    rail.Add(rp.Copy());
                }

                gravityPackage = GetAccelTowards(rail.Last().point + rp.velocity, Vector3.down, 0.25f, gravity + 0.25f); //raycast down at the end of the latest velocity vector to make sure gravity does not pull the ball below the ground

                if(gravityPackage.detected) //if hit
                {
                    rp.grounded = true;
                    rp.point = gravityPackage.hit.point;
                    rail.Add(rp.Copy());
                    rp.velocity = Slide(new Vector3(rp.velocity.x, rp.velocity.y - gravity, rp.velocity.z), gravityPackage.normal, gravityPackage.groundType.friction); //shortens vector by friction and gravity relative to slope
                    if (rp.grounded && rp.velocity.magnitude < 0.05f) break;
                    else continue;
                }
                else
                {
                    rp.velocity = new Vector3(rp.velocity.x, rp.velocity.y - gravity, rp.velocity.z); //apply gravity
                }

                rp.point += rp.velocity;

                if (rp.grounded && rp.velocity.magnitude < 0.05f) break;
            }

            RailPoint last = rp.Copy();
            last.frozen = true;

            rail[rail.Count - 1] = last;

            return rail.ToArray();
        }
    }
}
