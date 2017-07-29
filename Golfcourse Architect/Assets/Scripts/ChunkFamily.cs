using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkFamily : MonoBehaviour
{
    public Vector2 Size;
    public float EdgeSize;
    public float TreeEdgeSize;
    public float TreeFeather;

    public Chunk[,] chunkList;

    public Chunk ChunkPrefab;
    public GameObject ChunkEdgePrefab;
    GameObject edgeTreeParent;

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
        edgeTreeParent = new GameObject();
        edgeTreeParent.transform.SetParent(transform);

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
        GenerateRandomNature();
        FastBuildAllChunks();
        PlaceBoundries();
        StartCoroutine(InitialPlaceOfObjects());
        StartCoroutine(EnumeratePlacePrimitiveEdgeNature());
    }

    public IEnumerator InitialPlaceOfObjects()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (Chunk c in chunkList)
        {
            c.PlaceAllObjectsPrimitive(c.data);
            yield return new WaitForEndOfFrame();
        }
    }

    public void BuildAllChunks()
    {
        foreach (Chunk c in chunkList)
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

    public void PlaceBoundries()
    {
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();

                if (y == 0)
                {
                    vertices = new List<Vector3>();
                    triangles = new List<int>();
                    for (int v = 0; v < ChunkPrefab.Size.x + 1; v++)
                    {
                        Vector3 vertToCopy = chunkList[x, y].vertices[chunkList[x, y].getVertByXY(v, 0)];
                        vertices.Add(new Vector3(vertToCopy.x, vertToCopy.y + 0.03f, vertToCopy.z));
                        Vector3 alt = new Vector3(vertToCopy.x, vertToCopy.y + 0.03f, -EdgeSize);
                        vertices.Add(alt);
                    }

                    if (x == 0) //generate corner
                    {
                        Vector3 vertToCopy = chunkList[x, y].vertices[chunkList[x, y].getVertByXY(0, 0)];
                        vertices.Add(new Vector3(-EdgeSize, vertToCopy.y + 0.03f, vertToCopy.z));
                    }

                    if (x == Size.x - 1) //generate corner
                    {
                        Vector3 vertToCopy = chunkList[x, y].vertices[chunkList[x, y].getVertByXY((int)ChunkPrefab.Size.x, 0)];
                        vertices.Add(new Vector3(EdgeSize + vertToCopy.x, vertToCopy.y + 0.03f, vertToCopy.z));
                    }

                    for (int v = 0; v <= vertices.Count - 4; v += 2) // 
                    {
                        triangles.Add(v);
                        triangles.Add(v + 2);
                        triangles.Add(v + 1);

                        triangles.Add(v + 2);
                        triangles.Add(v + 3);
                        triangles.Add(v + 1);
                    }

                    if (x == 0) //generate corner triangle
                    {
                        triangles.Add(0);
                        triangles.Add(1);
                        triangles.Add(vertices.Count - 1);
                    }
                    if (x == Size.x - 1) //generate corner triangle
                    {
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 3);
                    }

                    GameObject g = Instantiate(ChunkEdgePrefab, new Vector3(x * ChunkPrefab.Size.x, 0, y * ChunkPrefab.Size.y), ChunkEdgePrefab.transform.rotation) as GameObject;
                    Mesh m = g.GetComponent<MeshFilter>().mesh = new Mesh();

                    m.vertices = vertices.ToArray();
                    m.triangles = triangles.ToArray();
                    m.RecalculateNormals();
                    m.RecalculateBounds();
                    m.RecalculateTangents();
                    g.GetComponent<MeshCollider>().sharedMesh = m;
                }

                if (x == 0)
                {
                    vertices = new List<Vector3>();
                    triangles = new List<int>();
                    for (int v = 0; v < ChunkPrefab.Size.y + 1; v++)
                    {
                        Vector3 vertToCopy = chunkList[x, y].vertices[chunkList[x, y].getVertByXY(0, v)];
                        vertices.Add(new Vector3(vertToCopy.x, vertToCopy.y + 0.03f, vertToCopy.z));
                        Vector3 alt = new Vector3(-EdgeSize, vertToCopy.y + 0.03f, vertToCopy.z);
                        vertices.Add(alt);
                    }

                    for (int v = 0; v <= vertices.Count - 4; v += 2)
                    {
                        triangles.Add(v + 1);
                        triangles.Add(v + 2);
                        triangles.Add(v);

                        triangles.Add(v + 1);
                        triangles.Add(v + 3);
                        triangles.Add(v + 2);
                    }

                    GameObject g = Instantiate(ChunkEdgePrefab, new Vector3(x * ChunkPrefab.Size.x, 0, y * ChunkPrefab.Size.y), ChunkEdgePrefab.transform.rotation) as GameObject;
                    Mesh m = g.GetComponent<MeshFilter>().mesh = new Mesh();

                    m.vertices = vertices.ToArray();
                    m.triangles = triangles.ToArray();
                    m.RecalculateNormals();
                    m.RecalculateBounds();
                    m.RecalculateTangents();
                    g.GetComponent<MeshCollider>().sharedMesh = m;
                }

                if (y == Size.y - 1)
                {
                    vertices = new List<Vector3>();
                    triangles = new List<int>();
                    for (int v = 0; v < ChunkPrefab.Size.x + 1; v++)
                    {
                        Vector3 vertToCopy = chunkList[x, y].vertices[chunkList[x, y].getVertByXY(v, (int)ChunkPrefab.Size.y)];
                        vertices.Add(new Vector3(vertToCopy.x, vertToCopy.y + 0.03f, vertToCopy.z));
                        Vector3 alt = new Vector3(vertToCopy.x, vertToCopy.y + 0.03f, EdgeSize + ChunkPrefab.Size.y);
                        vertices.Add(alt);
                    }

                    if (x == 0) //generate corner
                    {
                        Vector3 vertToCopy = chunkList[x, y].vertices[chunkList[x, y].getVertByXY(0, (int)ChunkPrefab.Size.y)];
                        vertices.Add(new Vector3(-EdgeSize, vertToCopy.y + 0.03f, vertToCopy.z));
                    }

                    if (x == Size.x - 1) //generate corner
                    {
                        Vector3 vertToCopy = chunkList[x, y].vertices[chunkList[x, y].getVertByXY((int)ChunkPrefab.Size.x, (int)ChunkPrefab.Size.y)];
                        vertices.Add(new Vector3(EdgeSize + vertToCopy.x, vertToCopy.y + 0.03f, vertToCopy.z));
                    }

                    for (int v = 0; v <= vertices.Count - 4; v += 2)
                    {
                        triangles.Add(v + 1);
                        triangles.Add(v + 2);
                        triangles.Add(v + 0);

                        triangles.Add(v + 1);
                        triangles.Add(v + 3);
                        triangles.Add(v + 2);
                    }

                    if (x == 0) //generate corner triangle
                    {
                        triangles.Add(vertices.Count - 1);
                        triangles.Add(1);
                        triangles.Add(0);
                    }
                    if (x == Size.x - 1) //generate corner triangle
                    {
                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);
                    }

                    GameObject g = Instantiate(ChunkEdgePrefab, new Vector3(x * ChunkPrefab.Size.x, 0, y * ChunkPrefab.Size.y), ChunkEdgePrefab.transform.rotation) as GameObject;
                    Mesh m = g.GetComponent<MeshFilter>().mesh = new Mesh();

                    m.vertices = vertices.ToArray();
                    m.triangles = triangles.ToArray();
                    m.RecalculateNormals();
                    m.RecalculateBounds();
                    m.RecalculateTangents();
                    g.GetComponent<MeshCollider>().sharedMesh = m;
                }

                if (x == Size.x - 1)
                {
                    vertices = new List<Vector3>();
                    triangles = new List<int>();
                    for (int v = 0; v < ChunkPrefab.Size.y + 1; v++)
                    {
                        Vector3 vertToCopy = chunkList[x, y].vertices[chunkList[x, y].getVertByXY((int)ChunkPrefab.Size.y, v)];
                        vertices.Add(new Vector3(vertToCopy.x, vertToCopy.y + 0.03f, vertToCopy.z));
                        Vector3 alt = new Vector3(EdgeSize + ChunkPrefab.Size.x, vertToCopy.y + 0.03f, vertToCopy.z);
                        vertices.Add(alt);
                    }

                    for (int v = 0; v <= vertices.Count - 4; v += 2)
                    {
                        triangles.Add(v + 0);
                        triangles.Add(v + 2);
                        triangles.Add(v + 1);

                        triangles.Add(v + 2);
                        triangles.Add(v + 3);
                        triangles.Add(v + 1);
                    }

                    GameObject g = Instantiate(ChunkEdgePrefab, new Vector3(x * ChunkPrefab.Size.x, 0, y * ChunkPrefab.Size.y), ChunkEdgePrefab.transform.rotation) as GameObject;
                    Mesh m = g.GetComponent<MeshFilter>().mesh = new Mesh();

                    m.vertices = vertices.ToArray();
                    m.triangles = triangles.ToArray();
                    m.RecalculateNormals();
                    m.RecalculateBounds();
                    m.RecalculateTangents();
                    g.GetComponent<MeshCollider>().sharedMesh = m;
                }
            }
        }
    }

    public void GenerateRandomHeights()
    {
        float noise = 0;
        Vector2 start = new Vector2(1111.1f, 1111.1f);
        float power = 8;
        float scale = .9f;

        for (int y = 0; y < (Size.y * (ChunkPrefab.Size.y) + 1); y++)
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

    public void GenerateRandomNature()
    {
        float noise = 0;
        Vector2 start = new Vector2(1111.1f, 1111.1f);
        float power = 8;
        float scale = 4f;
        float threshold = 4f;

        for (float y = -TreeEdgeSize; y < (Size.y * (ChunkPrefab.Size.y) + 1 + TreeEdgeSize); y += 2)
        {
            for (float x = -TreeEdgeSize; x < (Size.x * (ChunkPrefab.Size.x) + 1 + TreeEdgeSize); x += 2)
            {
                //if (x % ChunkPrefab.Size.x == 0)
                //    continue;
                //if (y % ChunkPrefab.Size.y == 0)
                //    continue;

                //noise = Random.Range(-0.025f, 0.025f);

                float xCoord = start.x + (x / (float)((int)Size.x * ((int)ChunkPrefab.Size.x)) + 1) * scale;
                float yCoord = start.y + (y / (float)((int)Size.y * ((int)ChunkPrefab.Size.y)) + 1) * scale;

                float e = Mathf.PerlinNoise(xCoord, yCoord) * power + noise;
                if ((x >= 0 && x < (Size.x * (ChunkPrefab.Size.x) + 1)) && (y >= 0 && y < (Size.y * (ChunkPrefab.Size.y) + 1)))
                {
                    if (e >= threshold && Random.Range((int)0, 8) >= 3)
                        ModifyChunkDataObjectIDGlobally((int)x, (int)y, ObjectID.NATURE_TREE_TEMPORATE1);
                    else
                    {
                        if (Random.Range(0f, 40f) >= 39)
                            ModifyChunkDataObjectIDGlobally((int)x, (int)y, ObjectID.NATURE_TREE_TEMPORATE1);
                    }
                }
                else
                {
                    if (e >= threshold && Random.Range((int)0, 8) >= 3)
                        EdgePrimitiveTrees.Add(new Vector2(x, y), ObjectID.NATURE_TREE_TEMPORATE1);
                    else
                    {
                        if (Random.Range(0f, 25f) >= 24)
                            EdgePrimitiveTrees.Add(new Vector2(x, y), ObjectID.NATURE_TREE_TEMPORATE1);
                    }
                }
            }
        }
    }

    public Dictionary<Vector2, ObjectID> EdgePrimitiveTrees = new Dictionary<Vector2, ObjectID>();
    public IEnumerator EnumeratePlacePrimitiveEdgeNature()
    {
        int index = 0;
        foreach (KeyValuePair<Vector2, ObjectID> kvp in EdgePrimitiveTrees)
        {
            if (index % 30 == 0)
                yield return new WaitForEndOfFrame();

            ObjectID id = kvp.Value;
            Vector2 pos = kvp.Key;

            Vector2 closest = new Vector2();

            if(pos.x <= 0)
            {
                closest.Set(0, closest.y);
            }

            if(pos.y <= 0)
            {
                closest.Set(closest.x, 0);
            }

            if (pos.y >= ChunkPrefab.Size.y * Size.y)
            {
                closest.Set(closest.x, ChunkPrefab.Size.y * Size.y);
            }
            if (pos.x >= ChunkPrefab.Size.x * Size.x)
            {
                closest.Set(ChunkPrefab.Size.x * Size.x, closest.y);
            }

            if(!(pos.y <= 0) && !(pos.y >= ChunkPrefab.Size.y * Size.y))
            {
                closest.Set(closest.x, pos.y);
            }
            if (!(pos.x <= 0) && !(pos.x >= ChunkPrefab.Size.x * Size.x))
            {
                closest.Set(pos.x, closest.y);
            }

            float distance = Vector2.Distance(closest, pos);

            float distanceNomralized = distance / (TreeEdgeSize * TreeFeather);
            float rand = Random.Range((float)0, 1);

            if (rand > Mathf.SmoothStep(0, 1, distanceNomralized))
            {

                NatureObject o = Instantiate(NatureObject.GetObjectPrefabFromID(id), new Vector3(pos.x, GetElevationUnderPointGlobal(pos.x, pos.y), pos.y), transform.rotation);
                o.transform.SetParent(edgeTreeParent.transform);
                o.source = null;
                o.localPosition = pos;
                o.transform.localScale = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
                o.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360f), 0));
            }
            index++;
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

        c.data.SetElevation((int)local.x, (int)local.y, newElevation);
        BridgeSameVertexHeight((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)local.x, (int)local.y, newElevation);
    }

    public float GetChunkDataPointElevationGlobally(int x, int y)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        return c.data[(int)local.x, (int)local.y].elevation;
    }

    public void ModifyChunkDataPointTypeGlobally(int x, int y, GA.Ground.GroundType type)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.data.SetGroundType((int)local.x, (int)local.y, type);
    }

    public void ModifyChunkDataObjectIDGlobally(int x, int y, ObjectID id)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.data.SetObjectID((int)local.x, (int)local.y, id);
    }

    public GA.Ground.GroundType GetChunkDataPointGroundTypeGlobally(int x, int y)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        return c.data[(int)local.x, (int)local.y].type;
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

    public float GetElevationUnderPointGlobal(float x, float y)
    {
        RaycastHit hit;
        Vector3 pos = new Vector3(x, 1000, y);
        Debug.DrawRay(pos, Vector3.down * 2000, Color.magenta, 10f);
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, 2000))
        {
            return hit.point.y;
        }

        return 0;
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

        for (int y = (int)(pos.y - radius); y < pos.y + radius; y++)
        {
            for (int x = (int)(pos.x - radius); x < pos.x + radius; x++)
            {
                if (Vector3.Distance(new Vector3(x, 0, y), new Vector3(pos.x, 0, pos.y)) <= radius)
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

                if (vertexY == 0)
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
