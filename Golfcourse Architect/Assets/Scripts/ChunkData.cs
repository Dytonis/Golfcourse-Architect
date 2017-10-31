using System;
using System.Collections;
using System.Collections.Generic;
using GA.Ground;
using GA.Objects;
using UnityEngine;

public class ChunkData
{
    private Chunk source;

    internal List<List<Tile>> data = new List<List<Tile>>();

    public ChunkData DeepCopy()
    {
        ChunkData d = new ChunkData(source);

        d.data = new List<List<Tile>>();

        for(int y = 0; y < source.Size.y + 1; y++)
        {
            d.data.Add(new List<Tile>());

            for(int x = 0; x < source.Size.x + 1; x++)
            {
                d.data[y].Add(new Tile() { elevation = this[x, y].elevation, type = this[x, y].type, obj = this[x, y].obj });
            }
        }

        return d;
    }

    public ChunkData(Chunk chunk)
    {
        source = chunk;
    }

    public void Init(Chunk chunk)
    {
        for (int y = 0; y < chunk.Size.y + 1; y++)
        {
            data.Add(new List<Tile>());

            for (int x = 0; x < chunk.Size.x + 1; x++)
            {
                if (x == chunk.Size.x || y == chunk.Size.y)
                    data[y].Add(new Tile() { elevation = 0, type = new Max_Bounds() }); //Vertices that are not parents for tiles (max bounds)
                else
                {
                    data[y].Add(new Tile() { elevation = 0, type = new Rough_Standard() });
                }
            }
        }
    }

    public void Init(Chunk chunk, List<List<Tile>> copy)
    {
        for (int y = 0; y < chunk.Size.y + 1; y++)
        {
            data.Add(new List<Tile>());

            for (int x = 0; x < chunk.Size.x + 1; x++)
            {
                data[y].Add(new Tile() { elevation = copy[y][x].elevation, type = copy[y][x].type, obj = copy[y][x].obj }); //Vertices that are not parents for tiles (max bounds)
            }
        }
    }

    public void SetGroundType(int x, int y, GroundType groundType)
    {
        data[y][x] = new Tile()
        {
            elevation = data[y][x].elevation,
            type = groundType,
            obj = data[y][x].obj
        };
    }

    public void SetObjectID(int x, int y, ObjectID id)
    {
        data[y][x] = new Tile()
        {
            elevation = data[y][x].elevation,
            type = data[y][x].type,
            obj = id
        };
    }

    public void SetElevation(int x, int y, float e)
    {
        try
        {
            data[y][x] = new Tile()
            {
                elevation = e,
                type = data[y][x].type,
                obj = data[y][x].obj
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

    public Tile this[int x, int y]
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
                    data[y][x] = new Tile()
                    {
                        elevation = value.elevation,
                        type = value.type,
                        obj = data[y][x].obj
                    };
                    return;
                }
            }
            Debug.LogWarning(x + ", " + y + " on chunk " + source.name + " doesn't exist.");
        }
    }
}

public struct Tile
{
    public float elevation { get; set; }
    public GroundType type { get; set; }
    public ObjectID obj { get; set; }
}
