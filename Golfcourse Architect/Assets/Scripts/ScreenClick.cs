using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenClick : MonoBehaviour
{
    public ClickType ClickAction;
    public ChunkFamily Family;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (ClickAction == ClickType.PLACE_TILE_FAIRWAY)
            ChangeTileType(r);
        else if (ClickAction == ClickType.EULER_VERTEX)
            Euler_Vertex(r);
    }

    private void ChangeTileType(Ray r)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.DrawRay(Camera.main.transform.position, r.direction * 100, Color.blue, 0.1f);
            RaycastHit hit;

            if (Physics.Raycast(r, out hit, 100f))
            {
                if (hit.collider.GetComponent<Chunk>() != null)
                {
                    int triangle = hit.triangleIndex;

                    Chunk c = hit.collider.GetComponent<Chunk>();

                    int y = (triangle / 2) % (int)c.Size.x;
                    int x = triangle / ((int)c.Size.x * 2);

                    c.data.SetGroundType(x, y, new GA.Ground.Fairway());
                    c.BuildTexture();

                    Debug.Log("Triangle " + triangle);
                }
            }
        }
    }

    private void Euler_Vertex(Ray r)
    {
        Debug.DrawRay(Camera.main.transform.position, r.direction * 100, Color.blue, 0.01f);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, 100f))
        {
            Chunk c = hit.collider.GetComponent<Chunk>();
            if (c != null && c.Viable)
            {
                Vector2 vertex = new Vector2(Mathf.RoundToInt(hit.point.x) % (c.Size.x), Mathf.RoundToInt(hit.point.z) % (c.Size.y));

                if((hit.point.x) % (c.Size.x) >= c.Size.x - 0.5f) //near boundry, need to switch tiles to next chunk over
                {
                    c = Family.chunkList[(int)c.GlobalPosition.x + 1, (int)c.GlobalPosition.y];
                }

                if ((hit.point.z) % (c.Size.y) >= c.Size.y - 0.5f) //near boundry, need to switch tiles to next chunk over
                {
                    c = Family.chunkList[(int)c.GlobalPosition.x, (int)c.GlobalPosition.y + 1];
                }

                if (Input.GetMouseButton(0))
                {
                    float newElevation = c.data[(int)vertex.x, (int)vertex.y].elevation + (0.3f * Time.deltaTime);
                    c.data.SetElevation((int)vertex.x, (int)vertex.y, newElevation);
                    Family.BridgeSameVertexHeight((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)vertex.x, (int)vertex.y, newElevation);
                    //Family.CauseRevalidationAroundTile((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)vertex.x, (int)vertex.y, Mathf.Sqrt(2f) + 0.1f);
                    c.FastBuild(c.data);
                }
                if (Input.GetMouseButton(1))
                {
                    float newElevation = c.data[(int)vertex.x, (int)vertex.y].elevation + (-0.3f * Time.deltaTime);
                    c.data.SetElevation((int)vertex.x, (int)vertex.y, newElevation);
                    Family.BridgeSameVertexHeight((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)vertex.x, (int)vertex.y, newElevation);
                    //Family.CauseRevalidationAroundTile((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)vertex.x, (int)vertex.y, Mathf.Sqrt(2f) + 0.1f);
                    c.FastBuild(c.data);
                }
            }
        }
    }
}

public enum ClickType
{
    PLACE_TILE_FAIRWAY,
    EULER_VERTEX,
    EULER_FACE,
    EULER_GROUP
}
