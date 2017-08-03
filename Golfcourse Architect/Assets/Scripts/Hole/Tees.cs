using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tees : MonoBehaviour
{
    public TeeTypes TeeType;
    public Color TeeColor = Color.white;
    public ChunkFamily family;

    public GameObject[] RaycastObjects;

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
            PositionUpdated(value);
        }
    }
	// Use this for initialization
	void Start () {
		
	}

    public void UpdateHeights()
    {
        foreach (GameObject g in RaycastObjects)
        {
            float e = family.GetElevationUnderPointGlobalRaycast(g.transform.position.x, g.transform.position.z);

            float point = g.transform.parent.InverseTransformPoint(0, e, 0).y;

            g.transform.localPosition = new Vector3(g.transform.localPosition.x, point, g.transform.localPosition.z);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(family)
        UpdateHeights();
	}

    private void PositionUpdated(Vector3 newPos)
    {
        
    }

    public void OnPlacement(ChunkFamily family, UIController controller)
    {
        //disable tees button
        controller.TeeButton.DisableButton();
        family.CurrentHoleCreating.TeesList.Add(this);

        if (family.CurrentHoleCreating.currentPin)
        {
            //this placement has finished the hole
            family.CurrentHoleCreating.Valid = true;

            family.CurrentHoleCreating.CalculateTargetLine();
            family.CurrentHoleCreating.OnValidation();
        }
    }
}

public enum TeeTypes
{
    Child,
    Forward,
    Executive,
    Standard,
    Far,
    Championship,
    Sponsor,
}

public enum DefaultTeeTypeColors
{
    Purple,
    Red,
    Gold,
    White,
    Blue,
    Black,
    Sponsor
}
