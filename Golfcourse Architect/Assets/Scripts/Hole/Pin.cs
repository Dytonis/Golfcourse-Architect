﻿using GA;
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

    public Vector2 FlatPosition
    {
        get
        {
            return transform.position.ToVector2();
        }
    }

    private void PositionUpdated(Vector3 value)
    {
        transform.position = value;
    }

    public void OnPlacement(ChunkFamily family, UIController controller)
    {
        //disable pin button
        controller.PinButton.DisableButton();
        family.CurrentHoleCreating.pinPlacements.Add(this);
        family.CurrentHoleCreating.currentPin = this;

        if (family.CurrentHoleCreating.TeesList.Count >= 1)
        {
            //this placement has finished the hole
            family.CurrentHoleCreating.Valid = true;

            family.CurrentHoleCreating.Construction_CalculateTargetLine();
            family.CurrentHoleCreating.OnValidation();
        }

        if(family.CurrentHoleCreating.line)
            Destroy(family.CurrentHoleCreating.line.gameObject);
    }
}
