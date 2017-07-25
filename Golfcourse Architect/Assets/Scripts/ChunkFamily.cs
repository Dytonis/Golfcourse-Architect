using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkFamily : MonoBehaviour
{
    public Vector2 Size;

    public Chunk[,] chunkList;

    public Chunk ChunkPrefab;

    public Vector2 globalSize
    {
        get
        {
            return new Vector2(ChunkPrefab.Size.x * Size.x, ChunkPrefab.Size.y * Size.y);
        }
    }

    public void Start()
    {
        chunkList = new Chunk[(int)Size.x, (int)Size.y];

        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                Chunk c = Instantiate(ChunkPrefab, transform);

                if (x == 0 || x == (int)Size.x - 1 || y == 0 || y == (int)Size.y - 1) //make the bordering chunks unviable
                    c.Viable = false;
                else
                    c.Viable = true;

                c.transform.position = new Vector3(x * ChunkPrefab.Size.x, 0, y * ChunkPrefab.Size.y);
                c.GlobalPosition = new Vector2(x, y);
                c.data = new ChunkData(c);
                c.data.Init(c);

                chunkList[x, y] = c;
            }
        }

        BuildAllChunks();
        GenerateRandomHeights();
        FastBuildAllChunks();
    }

    public void BuildAllChunks()
    {
        foreach(Chunk c in chunkList)
        {
            c.Build(c.data);
        }
    }

    public void FastBuildAllChunks()
    {
        foreach (Chunk c in chunkList)
        {
            c.FastBuild(c.data);
        }
    }

    public void Awake()
    {
        

    }

    public void GenerateRandomHeights()
    {
        float noise = 0;
        Vector2 start = new Vector2(1111.1f, 1111.1f);
        float power = 8;
        float scale = .9f;

        for(int y = 0; y < (Size.y * (ChunkPrefab.Size.y) + 1); y++)
        {
            for (int x = 0; x < (Size.x * (ChunkPrefab.Size.x) + 1); x++)
            {
                //if (x % ChunkPrefab.Size.x == 0)
                //    continue;
                //if (y % ChunkPrefab.Size.y == 0)
                //    continue;

                //noise = Random.Range(-0.025f, 0.025f);

                float xCoord = start.x + (x / (float)((int)Size.x * ((int)ChunkPrefab.Size.x)) + 1) * scale;
                float yCoord = start.y + (y / (float)((int)Size.y * ((int)ChunkPrefab.Size.y)) + 1) * scale;

                float e = Mathf.PerlinNoise(xCoord, yCoord) * power + noise;

                ModifyChunkDataPointElevationGlobally(x, y, e);
            }
        }
    }

    public void ModifyChunkDataGlobally(int x, int y, ChunkDataPoint newData)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.data[(int)local.x, (int)local.y] = newData;
    }

    public void ModifyChunkDataPointElevationGlobally(int x, int y, float newElevation)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        if (x == 15)
            Debug.Log("15");

        c.data.SetElevation((int)local.x, (int)local.y, newElevation);
        BridgeSameVertexHeight((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)local.x, (int)local.y, newElevation);
    }

    public void ModifyChunkDataPointTypeGlobally(int x, int y, GA.Ground.GroundType type)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.data.SetGroundType(x, y, type);
    }

    public Chunk chunkFromGlobalTilePos(int globalTileX, int globalTileY)
    {
        Vector2 pos = new Vector2((globalTileX / (int)ChunkPrefab.Size.x) % (int)Size.x, (globalTileY / (int)ChunkPrefab.Size.y) % (int)Size.y);

        if (globalTileX == ChunkPrefab.Size.x * Size.x)
            pos.x = Size.x - 1;
        if (globalTileY == ChunkPrefab.Size.y * Size.y)
            pos.y = Size.y - 1;

        return chunkList[(int)pos.x, (int)pos.y];
    }

    public void CauseRevalidationAroundTile(int chunkX, int chunkY, int tileX, int tileY, float radius)
    {
        foreach (Vector2 v in GetAllCoordsNearPoint(chunkX, chunkY, tileX, tileY, radius))
        {
            if (v.x >= 0 && v.x <= ChunkPrefab.Size.x * Size.x &&
                v.y >= 0 && v.y <= ChunkPrefab.Size.y * Size.y)
            {
                Chunk c = chunkFromGlobalTilePos((int)v.x, (int)v.y);
                Vector2 local = LocalPositionFromChunkSize(v);

                try
                {
                    QuadReference q = c.quadFromXY((int)local.x, (int)local.y);
                    q.quality = false;


                    c.SetQuad(q, (int)v.x, (int)v.y);
                }
                catch
                {

                }
            }
        }
    }

    private Vector2 LocalPositionFromChunkSize(Vector2 v)
    {
        float x;
        float y;

        if (v.x == (Size.x * (ChunkPrefab.Size.x)))
            x = ChunkPrefab.Size.x;
        else
            x = (int)v.x % (int)ChunkPrefab.Size.x;
        if (v.y == (Size.y * (ChunkPrefab.Size.y)))
            y = ChunkPrefab.Size.y;
        else
            y = (int)v.y % (int)ChunkPrefab.Size.y;

        return new Vector2(x, y);
    }

    private Vector2 LocalPositionFromChunkSize(float x, float y)
    {
        return LocalPositionFromChunkSize(new Vector2(x, y));
    }

    public Vector2[] GetAllCoordsNearPoint(int chunkX, int chunkY, int tileX, int tileY, float radius)
    {
        int chunkSize = (int)ChunkPrefab.Size.x;

        Vector2 pos = new Vector2((chunkX * chunkSize) + tileX, (chunkY * chunkSize) + tileY);

        List<Vector2> list = new List<Vector2>();

        for(int y = (int)(pos.y - radius); y < pos.y + radius; y++)
        {
            for (int x = (int)(pos.x - radius); x < pos.x + radius; x++)
            {
                if(Vector3.Distance(new Vector3(x, 0, y), new Vector3(pos.x, 0, pos.y)) <= radius)
                {
                    list.Add(new Vector2(x, y));
                }
            }
        }

        return list.ToArray();
    }

    public void BridgeSameVertexHeight(int chunkX, int chunkY, int vertexX, int vertexY, float elevation)
    {
        try
        {
            if (vertexX == 0) //left side
            {
                Chunk other = chunkList[chunkX - 1, chunkY];
                other.data.SetElevation((int)other.Size.x, vertexY, elevation);
                other.FastBuild(other.data);

                if(vertexY == 0)
                {
                    Chunk extra = chunkList[chunkX - 1, chunkY - 1];
                    extra.data.SetElevation((int)extra.Size.x, (int)extra.Size.y, elevation);
                    extra.FastBuild(extra.data);
                }
            }
        }
        catch { }
        try
        {
            if (vertexX == (int)chunkList[chunkX + 1, chunkY].Size.x) //right side
            {
                Chunk other = chunkList[chunkX + 1, chunkY];
                other.data.SetElevation(0, vertexY, elevation);
                other.FastBuild(other.data);

                if (vertexY == 0)
                {
                    Chunk extra = chunkList[chunkX, chunkY - 1];
                    extra.data.SetElevation((int)extra.Size.x, (int)extra.Size.y, elevation);
                    extra.FastBuild(extra.data);
                }
            }
        }
        catch { }
        try
        {
            if (vertexY == 0) //bottom side
            {
                Chunk other = chunkList[chunkX, chunkY - 1];
                other.data.SetElevation(vertexX, (int)other.Size.y, elevation);
                other.FastBuild(other.data);
            }
        }
        catch { }
        try
        {
            if (vertexY == (int)chunkList[chunkX, chunkY + 1].Size.y) //top side
            {
                Chunk other = chunkList[chunkX, chunkY + 1];
                other.data.SetElevation(vertexX, 0, elevation);
                other.FastBuild(other.data);
            }
        }
        catch { }
    }
}
