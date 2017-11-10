using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool DisableScrollOnHover;
    public bool DisableMovementOnHover;
    public bool DisableRotationOnHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DisableMovementOnHover)
            GA.Game.InputStatus.AllowCameraMovement = false;
        if (DisableRotationOnHover)
            GA.Game.InputStatus.AllowCameraRotation = false;
        if (DisableScrollOnHover)
            GA.Game.InputStatus.AllowCameraScroll = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GA.Game.InputStatus.AllowCameraMovement = true;
        GA.Game.InputStatus.AllowCameraRotation = true;
        GA.Game.InputStatus.AllowCameraScroll = true;
    }
}
