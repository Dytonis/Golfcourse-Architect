using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    private Vector3 _pos;
    public Vector3 PositionFine
    {
        get
        {
            return _pos;
        }
        set
        {
            _pos = value;
            PositionUpdated(value);
        }
    }

    private void PositionUpdated(Vector3 value)
    {
        transform.position = value;
    }
}
