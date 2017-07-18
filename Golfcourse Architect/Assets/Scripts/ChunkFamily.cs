using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkFamily : MonoBehaviour
{
    public Vector2 Size;

    public Chunk[,] chunkList;

    public Chunk ChunkPrefab;

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

                chunkList[x, y] = c;
            }
        }
    }

    public void CauseRevalidationAroundTile(int chunkX, int chunkY, int tileX, int tileY)
    {

    }

    public void GetAllCoordsNearPoint(int chunkX, int chunkY, int tileX, int tileY)
    {

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
