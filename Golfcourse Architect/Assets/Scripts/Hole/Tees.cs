using System.Collections;
using System.Collections.Generic;
using GA.Objects;
using UnityEngine;

public class Tees : GA.Objects.TileObject
{
    public TeeTypes TeeType;
    public Color TeeColor = Color.white;
    public ChunkFamily family;

    public GameObject[] RaycastObjects;

    public GameObject FencingPrefab;
    public GameObject Fencing;

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
	
    public void CreateFencing()
    {
        Fencing = Instantiate(FencingPrefab, transform);
        Fencing.transform.localPosition = new Vector3(0, -5, 0);
        Fencing.transform.localRotation = Quaternion.Euler(0, -transform.localRotation.eulerAngles.y, 0);
        StartMoveFence(-5, 0, 1);
    }

    public void RemoveFencing()
    {
        if (Fencing)
        {
            StartMoveFence(0, -5, 1);
        }
    }

    private void StartMoveFence(float from, float to, float speed)
    {
        StartCoroutine(MoveFence(from, to, speed));
    }

    private IEnumerator MoveFence(float from, float to, float speed)
    {
        if (Fencing)
        {
            float time = 0;
            while (Fencing.transform.localPosition.y != to)
            {
                time += Time.deltaTime * speed;
                Fencing.transform.localPosition = Vector3.Lerp(new Vector3(Fencing.transform.localPosition.x, from, Fencing.transform.localPosition.z), new Vector3(Fencing.transform.localPosition.x, to, Fencing.transform.localPosition.z), time);
                yield return new WaitForEndOfFrame();
            }
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

            family.CurrentHoleCreating.Construction_CalculateTargetLine();
            family.CurrentHoleCreating.OnValidation();
        }

        if(family.CurrentHoleCreating.line)
            Destroy(family.CurrentHoleCreating.line.gameObject);
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
