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

    public RectTransform Bar;

    public UIController controller;

    public Texture Normal;
    public Texture Activated;

    public bool On = false;

    private bool major = false;

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
        clicker.ClickAction = ClickType.NONE;
        GetComponent<RawImage>().texture = Normal;
        On = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (On)
        {
            Deactivate();
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
                StartCoroutine(controller.MoveRect(Bar, new Vector2(-2000, Bar.anchoredPosition.y), Bar.anchoredPosition, 10));
            }
            else
            {
                StartCoroutine(controller.MoveRect(Bar, new Vector2(-750, Bar.anchoredPosition.y), Bar.anchoredPosition, 10));
                controller.BarOutCurrently = Bar;
            }
        }

        if (GroupParent != null)
        {
            if (GroupParent.Major == MajorButtonID.Tiles)
            {
                if (Tiles == TilesButtonID.Fairway)
                {
                    clicker.ClickAction = ClickType.PLACE_TILE_FAIRWAY;
                }
                else if (Tiles == TilesButtonID.Rough)
                {
                    clicker.ClickAction = ClickType.PLACE_TILE_ROUGH;
                }
                else if (Tiles == TilesButtonID.Green)
                {
                    clicker.ClickAction = ClickType.PLACE_TILE_GREEN;
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
    Utilities,
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
