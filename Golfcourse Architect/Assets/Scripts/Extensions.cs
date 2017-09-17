using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA
{
    public static class Extensions
    {
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector3 ToFlatVector3(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }
    }
}
