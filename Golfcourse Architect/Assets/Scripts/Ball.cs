using GA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float Spin;
    public float SpinAxisDelta;
    public Vector3 Velocity;
    public float gravity;

    public Vector2 FlatPosition
    {
        get
        {
            return transform.position.ToVector2();
        }
    }
}
