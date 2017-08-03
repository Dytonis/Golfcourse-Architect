using GA.Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public AStarTile tile;

    public TextMesh g;
    public TextMesh h;
    public TextMesh f;

    public void Apply()
    {
        g.text = tile.G.ToString();
        h.text = tile.H.ToString();
        f.text = tile.F.ToString();
    }
}
