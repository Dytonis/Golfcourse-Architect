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

        for(int y = 0; y < chunk.Size.y + 1; y++)
        {
            data.Add(new List<ChunkDataPoint>());

            for(int x = 0; x < chunk.Size.x + 1; x++)
            {
                if(x == chunk.Size.x || y == chunk.Size.y)
                    data[y].Add(new ChunkDataPoint() { elevation = 0, type = new Max_Bounds() }); //Vertices that are not parents for tiles (max bounds)
                else
                    data[y].Add(new ChunkDataPoint() { elevation = 0, type = new Rough_Standard() });
            }
        }
    }

    public void SetGroundType(int x, int y, GroundType groundType)
    {
        data[y][x] = new ChunkDataPoint()
        {
            elevation = data[y][x].elevation,
            type = groundType
        };
    }

    public void SetElevation(int x, int y, float e)
    {
        data[y][x] = new ChunkDataPoint()
        {
            elevation = e,
            type = data[y][x].type
        };
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
                        type = value.type
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
}
