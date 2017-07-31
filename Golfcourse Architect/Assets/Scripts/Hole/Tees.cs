using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tees : MonoBehaviour
{
    public TeeTypes TeeType;
    public Color TeeColor = Color.white;
    private Vector3 _pos;
    public Vector3 Position
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
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void PositionUpdated(Vector3 newPos)
    {
        transform.position = newPos;
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
