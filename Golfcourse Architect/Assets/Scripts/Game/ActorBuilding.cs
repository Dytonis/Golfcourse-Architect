using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBuilding : Building
{
    public Animator AnimationController;

    public Vector2 FlatPosition
    {
        get
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }
}
