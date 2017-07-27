using System;
using System.Collections;
using System.Collections.Generic;
using GA.Ground;
using UnityEngine;

public class ChunkData
{
    private Chunk source;

    private List<List<ChunkDataPoint>> data = new List<List<ChunkDataPoint>>();

    public ChunkData(Chunk chunk)
    {
        source = chunk;
    }

    public void Init(Chunk chunk)
    {
        for (int y = 0; y < chunk.Size.y + 1; y++)
        {
            data.Add(new List<ChunkDataPoint>());

            for (int x = 0; x < chunk.Size.x + 1; x++)
            {
                if (x == chunk.Size.x || y == chunk.Size.y)
                    data[y].Add(new ChunkDataPoint() { elevation = 0, type = new Max_Bounds() }); //Vertices that are not parents for tiles (max bounds)
                else
                {
                    data[y].Add(new ChunkDataPoint() { elevation = 0, type = new Rough_Standard() });
                }
            }
        }
    }

    public void SetGroundType(int x, int y, GroundType groundType)
    {
        data[y][x] = new ChunkDataPoint()
        {
            elevation = data[y][x].elevation,
            type = groundType,
            ObjectResiding = data[y][x].ObjectResiding
        };
    }

    public void SetObjectID(int x, int y, ObjectID id)
    {
        data[y][x] = new ChunkDataPoint()
        {
            elevation = data[y][x].elevation,
            type = data[y][x].type,
            ObjectResiding = id
        };
    }

    public void SetElevation(int x, int y, float e)
    {
        try
        {
            data[y][x] = new ChunkDataPoint()
            {
                elevation = e,
                type = data[y][x].type,
                ObjectResiding = data[y][x].ObjectResiding
            };
        }
        catch
        {
            int start = source.getVertByXY(x, y);

            for(int i = 0; i < source.dupeVertCount; i++)
            {
                source.vertices[start + i] = new Vector3(source.vertices[start + 1].x, e, source.vertices[start + 1].z);
            }
        }
    }

    public ChunkDataPoint this[int x, int y]
    {
        get
        {
            if (data.Count >= y)
            {
                if (data[y].Count >= x)
                {
                    return data[y][x];
                }
            }
            throw new IndexOutOfRangeException();
        }
        set
        {
            if(data.Count >= y)
            {
                if(data[y].Count >= x)
                {
                    data[y][x] = new ChunkDataPoint()
                    {
                        elevation = value.elevation,
                        type = value.type,
                        ObjectResiding = data[y][x].ObjectResiding
                    };
                    return;
                }
            }
            Debug.LogWarning(x + ", " + y + " on chunk " + source.name + " doesn't exist.");
        }
    }
}

public struct ChunkDataPoint
{
    public float elevation { get; set; }
    public GroundType type { get; set; }
    public ObjectID ObjectResiding { get; set; }
}
