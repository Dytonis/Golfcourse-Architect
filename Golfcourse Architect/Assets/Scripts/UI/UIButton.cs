using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerClickHandler
{
    public UIButton GroupParent;
    public ScreenClick clicker;
    public MajorButtonID Major;
    public TilesButtonID Tiles;
    public HoleObjectButtonID HoleObjects;

    public RectTransform Bar;
    public float DefaultXPostition = -2400f;
    public float ActivatedXPosition = -750f;
    public bool AddWidth;

    public UIController controller;

    public Texture Normal;
    public Texture Activated;

    public bool On = false;

    private bool major = false;

    public bool Enabled = true;

    public void Start()
    { 
        clicker = GameObject.FindGameObjectWithTag("cameraClicker").GetComponent<ScreenClick>();
        controller = GameObject.FindGameObjectWithTag("uiController").GetComponent<UIController>();

        if (Major != MajorButtonID.None)
        {
            controller.Majors.Add(this);
            major = true;
        }
        else
            controller.Minors.Add(this);

        controller.Buttons.Add(this);
    }

    public void EnableButton()
    {
        GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
        Enabled = true;
    }

    public void DisableButton()
    {
        GetComponent<RawImage>().color = new Color(0.5f, 0.5f, 0.5f, 1);
        Enabled = false;
    }

    public void Activate()
    {
        if (major)
            controller.DeactivateAll();
        else
            controller.DeactivateMinors();
        GetComponent<RawImage>().texture = Activated;
        On = true;
    }

    public void Deactivate()
    {
        clicker.ClickAction.type = ClickType.NONE;
        GetComponent<RawImage>().texture = Normal;
        On = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Enabled == false)
            return;

        if (On)
        {
            Deactivate();
            if (clicker.Family.CurrentHoleCreating)
            {
                if (clicker.Family.CurrentHoleCreating.TeesList.Count == 0 && clicker.Family.CurrentHoleCreating.currentPin == null) //stop construction if nothing has been placed
                {
                    clicker.Family.CurrentHoleCreating.OnLeaveStatsView();
                    clicker.Family.CurrentHoleCreating.ConstructingCurrently = false;
                    clicker.ResetState();
                }
            }
        }
        else
        {
            Activate();
        }

        if (Major != MajorButtonID.None)
        {
            //pop the tiles menu
            float now = Time.time;
            if (!On) //if it WAS on 
            {
                float pos = DefaultXPostition;
                if (AddWidth)
                    pos += (Bar.rect.width / 2f);
                StartCoroutine(controller.MoveRect(Bar, new Vector2(pos, Bar.anchoredPosition.y), Bar.anchoredPosition, 10));
            }
            else
            {
                float pos = ActivatedXPosition;
                if (AddWidth)
                    pos += (Bar.rect.width / 2f);
                StartCoroutine(controller.MoveRect(Bar, new Vector2(pos, Bar.anchoredPosition.y), Bar.anchoredPosition, 10));
                controller.BarOutCurrently = Bar;
            }
        }

        if (GroupParent != null)
        {
            if (On) //if it IS CURRENTLY on 
            {
                if (GroupParent.Major == MajorButtonID.Tiles)
                {
                    if (Tiles == TilesButtonID.Fairway)
                    {
                        clicker.ClickAction.type = ClickType.PLACE_TILE_FAIRWAY;
                    }
                    else if (Tiles == TilesButtonID.Rough)
                    {
                        clicker.ClickAction.type = ClickType.PLACE_TILE_ROUGH;
                    }
                    else if (Tiles == TilesButtonID.Green)
                    {
                        clicker.ClickAction.type = ClickType.PLACE_TILE_GREEN;
                    }
                }
                else if (GroupParent.Major == MajorButtonID.Hole)
                {
                    if (HoleObjects == HoleObjectButtonID.Pin)
                    {
                        clicker.ClickAction.type = ClickType.PLACE_HOLE_PIN;
                    }
                    else if (HoleObjects == HoleObjectButtonID.Teebox)
                    {
                        clicker.ClickAction.type = ClickType.PLACE_HOLE_TEES;
                    }
                }
            }
        }
    }
}

public enum MajorButtonID
{
    None,
    Tiles,
    Hole,
    Nature,
    Objects,
    Buildings,
    Landscaping,
    Paths
}

public enum TilesButtonID
{
    None,
    Green,
    Fairway,
    FastFairway,
    Rough,
    Bunker,
    Water,
    PotBunker,
    TallGrass,
    Underbrush,
}

public enum HoleObjectButtonID
{
    None,
    Teebox,
    Pin,
}
