﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenClick : MonoBehaviour
{
    public ClickType ClickAction;
    public ChunkFamily Family;
    public UIController UIController;

    public Tees teesPrefab;

    public Vector2 mouseVelocityLastFrame;

    // Use this for initialization
    void Start()
    {

    }

    private Vector2 mousePositionLastFrame = Vector2.zero;
    // Update is called once per frame
    void Update()
    {
        if (UIController.OverUIController)
        {
            ResetState();
            return;
        }

        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

        mouseVelocityLastFrame = new Vector2(Input.mousePosition.x - mousePositionLastFrame.x, Input.mousePosition.y - mousePositionLastFrame.y);

        if (ClickAction == ClickType.PLACE_TILE_FAIRWAY)
            ChangeTileType(r);
        else if (ClickAction == ClickType.EULER_VERTEX)
            Euler_Vertex(r);
        else if (ClickAction == ClickType.PLACE_HOLE_TEES)
            PlaceHoleObject(r, "tee");

        mousePositionLastFrame = Input.mousePosition;
    }

    private void ResetState()
    {
        if (teeObject)
        {
            Destroy(teeObject.gameObject);
            Family.ResetChunkTempMemory(true);
        }
    }

    Tees teeObject = new Tees();
    private void PlaceHoleObject(Ray r, string obj)
    {
        if (mouseVelocityLastFrame == Vector2.zero && Input.GetMouseButtonUp(0) == false)
            return;

        if (teeObject == null)
            teeObject = Instantiate(teesPrefab) as Tees;

        Debug.DrawRay(Camera.main.transform.position, r.direction * 100, Color.blue, 0.1f);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 100f))
        {
            if (hit.collider.GetComponent<Chunk>() != null)
            {
                int triangle = hit.triangleIndex;

                Chunk c = hit.collider.GetComponent<Chunk>();

                float y = ((triangle / 2) % (int)c.Size.x);
                float x = (triangle / ((int)c.Size.x * 2));

                float globalY = c.getGlobalPointFromLocal(x, y).y;
                float globalX = c.getGlobalPointFromLocal(x, y).x;

                Family.ResetChunkTempMemory(true);

                float lowest = Family.GetLowestElevationUnderTileGlobalRaycast(globalX, globalY);

                Family.ModifyChunkDataPointTileElevationGloballyTemporarily((int)globalX, (int)globalY, lowest);

                c.FastBuild(c.tempData);
                Family.AddChunkToTemp(c);

                teeObject.Position = new Vector3(globalX + 0.5f, lowest + 0.03f, globalY + 0.5f);

                if (Input.GetMouseButtonUp(0))
                {
                    Family.ModifyChunkDataPointTileElevationGlobally((int)globalX, (int)globalY, lowest);
                    Family.ModifyChunkDataPointTypeGlobally((int)globalX, (int)globalY, new GA.Ground.Fairway());
                    Instantiate(teeObject, teeObject.transform.position, teeObject.transform.rotation);
                    UIController.DeactivateMinors();
                    ClickAction = ClickType.NONE;
                    c.FastBuild(c.data);
                    c.BuildTexture(c.data);
                }
            }
        }
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
                    c.BuildTexture(c.data);

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
    NONE,
    PLACE_TILE_FAIRWAY,
    EULER_VERTEX,
    EULER_FACE,
    EULER_GROUP,
    PLACE_TILE_GREEN,
    PLACE_TILE_ROUGH,
    PLACE_TILE_FASTFAIRWAY,
    PLACE_TILE_BUNKER,
    PLACE_TILE_WATER,
    PLACE_HOLE_PIN,
    PLACE_HOLE_TEES,
}
