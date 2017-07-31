using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Building : MonoBehaviour
{
    ChunkFamily family;

    public Vector2 Size;

    public Mesh ColliderBounds;

    public void Start()
    {
        family = GameObject.FindGameObjectWithTag("family").GetComponent<ChunkFamily>();
        StartCoroutine(waiter());
    }

    public IEnumerator waiter()
    {
        yield return new WaitForSeconds(5);
        StartCoroutine(PlaceBuilding(40, 40));
    }

    public IEnumerator PlaceBuilding(int PosX, int PosY)
    {
        float averageHeight = 0;

        transform.position = new Vector3(PosX, averageHeight, PosY);
        List<float> heights = new List<float>();
        List<Vector3> pos = new List<Vector3>();
        List<Chunk> uniqueChunks = new List<Chunk>();
        float maxX = 0;
        float maxY = 0;

        for (float y = -(Size.y / 2); y < (Size.y / 2) + 1; y += 0.25f)
        {
            for (float x = -(Size.x / 2); x < (Size.x / 2) + 1; x += 0.25f)
            {
                Vector3 local = transform.TransformPoint(new Vector3(x, 1000, y));
                RaycastHit hit;
                Ray r = new Ray(local, Vector3.down);

                Debug.DrawRay(local, Vector3.down * 1300, Color.yellow, 5f);

                if(Physics.Raycast(r, out hit, 1300))
                {
                    if (local.x > maxX)
                        maxX = local.x;
                    if (local.z > maxY)
                        maxY = local.z;

                    heights.Add(hit.point.y);
                    pos.Add(local);
                    if (!uniqueChunks.Contains(family.chunkFromGlobalTilePos((int)local.x, (int)local.z)))
                    {
                        uniqueChunks.Add(family.chunkFromGlobalTilePos((int)local.x, (int)local.z));
                    }
                }
            }
        }

        float sum = 0;

        foreach (float h in heights)
            sum += h;

        averageHeight = sum / heights.Count;

        foreach(Vector3 p in pos)
        {
            family.ModifyChunkDataPointElevationGlobally((int)p.x, (int)p.z, averageHeight);
            if (p.x >= maxX - 1)
                continue;
            if (p.z >= maxY - 1)
                continue;
            family.ModifyChunkDataPointTypeGlobally((int)p.x, (int)p.z, new GA.Ground.Concrete());
            family.ModifyChunkDataObjectIDGlobally((int)p.x, (int)p.z, ObjectID.EMPTY);   
        }

        foreach(Chunk c in uniqueChunks)
        {
            c.FastBuild(c.data);
            c.UpdateObjects(c.data);
            yield return new WaitForEndOfFrame();
            c.BuildTexture(c.data);
        }

        transform.position = new Vector3(PosX, averageHeight, PosY);
        yield break;
    }
}
