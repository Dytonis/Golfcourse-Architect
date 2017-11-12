using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GA.Objects;
using GA.Game.GroundTypes;

public class ChunkFamily : MonoBehaviour
{
    public Vector2 Size;
    public float EdgeSize;
    public float TreeEdgeSize;
    public float TreeFeather;

    public Chunk[,] chunkList;

    public Chunk ChunkPrefab;
    public GameObject ChunkEdgePrefab;
    public StandardGamemode Gamemode;
    public Hole CurrentHoleCreating;
    GameObject edgeTreeParent;

    public bool CreatingHole = false;

    public int InitState = 0;
    public bool Initialized = false;

    public Vector2 globalSize
    {
        get
        {
            return new Vector2(ChunkPrefab.Size.x * Size.x, ChunkPrefab.Size.y * Size.y);
        }
    }

    public void UpdateInit()
    {
        InitState++;

        if (InitState == 2)
        {
            Initialized = true;
            Gamemode.StartGame();
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

        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                Chunk c = chunkList[x, y];
                c.tempData = new ChunkData(c);
                c.tempData.Init(c, c.data.data);
            }
        }
    }

    public IEnumerator InitialPlaceOfObjects()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (Chunk c in chunkList)
        {
            c.PlaceAllObjectsPrimitive(c.data);
            yield return new WaitForEndOfFrame();
        }

        UpdateInit();
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

    public void FastTempBuildAllChunks()
    {
        foreach (Chunk c in chunkList)
        {
            c.FastBuild(c.tempData);
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
                NatureObject o = Instantiate(TileObject.GetObjectPrefabFromID<NatureObject>(id), new Vector3(pos.x, GetElevationUnderPointGlobalRaycast(pos.x, pos.y), pos.y), transform.rotation);
                o.transform.SetParent(edgeTreeParent.transform);
                o.source = null;
                o.FlatPosition = pos;
                o.transform.localScale = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
                o.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360f), 0));
            }
            index++;
        }

        UpdateInit();
    }

    public void ModifyChunkDataGlobally(int x, int y, Tile newData)
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

    public void ModifyChunkDataPointTileElevationGlobally(int x, int y, float newElevation)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.data.SetElevation((int)local.x, (int)local.y, newElevation);
        c.data.SetElevation((int)local.x + 1, (int)local.y, newElevation);
        c.data.SetElevation((int)local.x + 1, (int)local.y + 1, newElevation);
        c.data.SetElevation((int)local.x, (int)local.y + 1, newElevation);
        BridgeSameTileHeight((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)local.x, (int)local.y, newElevation);
    }

    public void ModifyChunkDataPointTileElevationGloballyTemporarily(int x, int y, float newElevation)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.tempData.SetElevation((int)local.x, (int)local.y, newElevation);
        c.tempData.SetElevation((int)local.x + 1, (int)local.y, newElevation);
        c.tempData.SetElevation((int)local.x + 1, (int)local.y + 1, newElevation);
        c.tempData.SetElevation((int)local.x, (int)local.y + 1, newElevation);
        BridgeSameTileHeightTemporarily((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)local.x, (int)local.y, newElevation);
    }

    public void ModifyChunkDataPointElevationGloballyTemporarily(int x, int y, float newElevation)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.tempData.SetElevation((int)local.x, (int)local.y, newElevation);
        BridgeSameVertexHeightTemporarily((int)c.GlobalPosition.x, (int)c.GlobalPosition.y, (int)local.x, (int)local.y, newElevation);
    }

    public float GetChunkDataPointElevationGlobally(int x, int y)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        return c.data[(int)local.x, (int)local.y].elevation;
    }

    [System.Obsolete]
    public void ModifyChunkDataPointTypeGlobally(int x, int y, GA.Game.GroundTypes.GroundType type)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.data.SetGroundType((int)local.x, (int)local.y, type);
    }

    /// <summary>
    /// Use this method to change a tile on a global x/y grid. This method will do a smart texture build.
    /// </summary>
    /// <param name="position">The tile to be changed (int truncated)</param>
    /// <param name="type">The type of the new tile.</param>
    /// <param name="Rebuild">If true, this method will automatically rebuild the texture for that chunk.</param>
    public void SetGroundType<T>(Vector2 position, bool Rebuild = true) where T : GroundType, new()
    {
        Vector2 local = LocalPositionFromChunkSize((int)position.x, (int)position.y);

        int x = (int)local.x;
        int y = (int)local.y;

        Chunk c = chunkFromGlobalTilePos((int)position.x, (int)position.y);
        ChunkData old = null;
        if (Rebuild)
            old = c.data.DeepCopy();
        
        c.data.SetGroundType(x, y, new T());
        if(Rebuild)
            c.BuildSmartSingleTexture(c.data, old, x, y);
    }

    /// <summary>
    /// Use this method to change a tile on a global x/y grid. This method will do a smart texture build. Use this method instead of SetGroundType when the type requires complex data (eg Teebox)
    /// </summary>
    /// <param name="position">The tile to be changed (int truncated)</param>
    /// <param name="type">The type of the new tile.</param>
    /// <param name="Rebuild">If true, this method will automatically rebuild the texture for that chunk.</param>
    public void SetComplexGroundType(Vector2 position, GA.Game.GroundTypes.GroundType type, bool Rebuild = true)
    {
        Vector2 local = LocalPositionFromChunkSize((int)position.x, (int)position.y);

        int x = (int)local.x;
        int y = (int)local.y;

        Chunk c = chunkFromGlobalTilePos((int)position.x, (int)position.y);
        ChunkData old = null;
        if (Rebuild)
            old = c.data.DeepCopy();

        c.data.SetGroundType(x, y, type);
        if (Rebuild)
            c.BuildSmartSingleTexture(c.data, old, x, y);
    }

    public void ModifyChunkDataPointTypeGloballyTemporarily(int x, int y, GA.Game.GroundTypes.GroundType type)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.tempData.SetGroundType((int)local.x, (int)local.y, type);
    }

    public void ModifyChunkDataObjectIDGlobally(int x, int y, ObjectID id)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        c.data.SetObjectID((int)local.x, (int)local.y, id);
    }

    public GA.Game.GroundTypes.GroundType GetChunkDataPointGroundTypeGlobally(int x, int y)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        return c.data[(int)local.x, (int)local.y].type;
    }

    public GA.Game.GroundTypes.GroundType GetChunkDataPointGroundTypeGlobally(Vector2 pos)
    {
        Chunk c = chunkFromGlobalTilePos((int)pos.x, (int)pos.y);

        Vector2 local = LocalPositionFromChunkSize((int)pos.x, (int)pos.y);

        return c.data[(int)local.x, (int)local.y].type;
    }

    public System.Type GetChunkDataPointTypeofGroundTypeGlobally(int x, int y)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        return c.data[(int)local.x, (int)local.y].type.GetType();
    }

    /// <summary>
    /// Use this method to return the unique instance of the ground type at the position.
    /// </summary>
    /// <param name="position">The position to get</param>
    /// <returns>The unique instance of the ground type.</returns>
    public GA.Game.GroundTypes.GroundType GetGroundTypeInstance(Vector2 position)
    {
        Chunk c = chunkFromGlobalTilePos((int)position.x, (int)position.y);

        Vector2 local = LocalPositionFromChunkSize((int)position.x, (int)position.y);

        return c.data[(int)local.x, (int)local.y].type;
    }

    public System.Type GetChunkDataPointTypeofGroundTypeGloballyTemporarily(int x, int y)
    {
        Chunk c = chunkFromGlobalTilePos(x, y);

        Vector2 local = LocalPositionFromChunkSize(x, y);

        return c.tempData[(int)local.x, (int)local.y].type.GetType();
    }

    public Chunk chunkFromGlobalTilePos(int globalTileX, int globalTileY)
    {
        Vector2 pos = new Vector2((globalTileX / (int)ChunkPrefab.Size.x) % (int)Size.x, (globalTileY / (int)ChunkPrefab.Size.y) % (int)Size.y);

        if (globalTileX == ChunkPrefab.Size.x * Size.x)
            pos.x = Size.x - 1;
        if (globalTileY == ChunkPrefab.Size.y * Size.y)
            pos.y = Size.y - 1;

        try
        {
            return chunkList[(int)pos.x, (int)pos.y];
        }
        catch
        {
            return null;
        }
    }

    public float GetElevationUnderPointGlobalRaycast(float x, float y)
    {
        RaycastHit hit;
        Vector3 pos = new Vector3(x, 1000, y);
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, 2000))
        {
            return hit.point.y;
        }

        return 0;
    }

    public float GetLowestElevationUnderTileGlobalRaycast(float x, float y)
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        RaycastHit hit;
        Vector3 pos = new Vector3(x, 1000, y);
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, 2000))
        {
            hits.Add(hit);
        }
        RaycastHit hit2;
        Vector3 pos2 = new Vector3(x + 1, 1000, y);
        if (Physics.Raycast(new Ray(pos2, Vector3.down), out hit2, 2000))
        {
            hits.Add(hit2);
        }
        RaycastHit hit3;
        Vector3 pos3 = new Vector3(x, 1000, y + 1);
        if (Physics.Raycast(new Ray(pos3, Vector3.down), out hit3, 2000))
        {
            hits.Add(hit3);
        }
        RaycastHit hit4;
        Vector3 pos4 = new Vector3(x + 1, 1000, y + 1);
        if (Physics.Raycast(new Ray(pos4, Vector3.down), out hit4, 2000))
        {
            hits.Add(hit4);
        }

        return hits.Min(h => h.point.y);
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

    private List<Chunk> tempChunks = new List<Chunk>();
    public void AddChunkToTemp(Chunk c)
    {
        tempChunks.Add(c);
    }

    public void ResetChunkTempMemory(bool build = false)
    {
        foreach (Chunk c in tempChunks)
        {
            c.ResetTempData();
            if(build)
                c.FastBuild(c.data);
        }

        tempChunks.Clear();
    }

    public void BridgeSameTileHeight(int chunkX, int chunkY, int tileX, int tileY, float elevation)
    {
        List<Chunk> chunksToFastBuild = new List<Chunk>();

        try
        {
            if (tileX == 0) //left side
            {
                Chunk other = chunkList[chunkX - 1, chunkY];
                other.data.SetElevation((int)other.Size.x, tileY, elevation);
                other.data.SetElevation((int)other.Size.x, tileY + 1, elevation);
                chunksToFastBuild.Add(other);

                if (tileY == 0)
                {
                    Chunk extra = chunkList[chunkX - 1, chunkY - 1];
                    extra.data.SetElevation((int)extra.Size.x, (int)extra.Size.y, elevation);
                    chunksToFastBuild.Add(extra);
                }
                else if (tileY == ChunkPrefab.Size.y - 1)
                {
                    Chunk extra = chunkList[chunkX - 1, chunkY + 1];
                    extra.data.SetElevation((int)extra.Size.x, 0, elevation);
                    chunksToFastBuild.Add(extra);
                }
            }
        }
        catch { }
        try
        {
            if (tileX == (int)chunkList[chunkX + 1, chunkY].Size.x - 1) //right side
            {
                Chunk other = chunkList[chunkX + 1, chunkY];
                other.data.SetElevation(0, tileY, elevation);
                other.data.SetElevation(0, tileY + 1, elevation);
                chunksToFastBuild.Add(other);

                if (tileY == 0)
                {
                    Chunk extra = chunkList[chunkX + 1, chunkY - 1];
                    extra.data.SetElevation(0, (int)extra.Size.y, elevation);
                    chunksToFastBuild.Add(extra);
                }
                else if (tileY == ChunkPrefab.Size.y - 1)
                {
                    Chunk extra = chunkList[chunkX + 1, chunkY + 1];
                    extra.data.SetElevation(0, 0, elevation);
                    chunksToFastBuild.Add(extra);
                }
            }
        }
        catch { }
        try
        {
            if (tileY == 0) //bottom side
            {
                Chunk other = chunkList[chunkX, chunkY - 1];
                other.data.SetElevation(tileX, (int)other.Size.y, elevation);
                other.data.SetElevation(tileX + 1, (int)other.Size.y, elevation);
                if(!chunksToFastBuild.Contains(other))
                    chunksToFastBuild.Add(other);
            }
        }
        catch { }
        try
        {
            if (tileY == (int)chunkList[chunkX, chunkY + 1].Size.y - 1) //top side
            {
                Chunk other = chunkList[chunkX, chunkY + 1];
                other.data.SetElevation(tileX, 0, elevation);
                other.data.SetElevation(tileX + 1, 0, elevation);
                if (!chunksToFastBuild.Contains(other))
                    chunksToFastBuild.Add(other);
            }
        }
        catch { }

        foreach (Chunk c in chunksToFastBuild)
            c.FastBuild(c.data);
    }

    public void BridgeSameTileHeightTemporarily(int chunkX, int chunkY, int tileX, int tileY, float elevation)
    {
        List<Chunk> chunksToFastBuild = new List<Chunk>();

        try
        {
            if (tileX == 0) //left side
            {
                Chunk other = chunkList[chunkX - 1, chunkY];
                other.tempData.SetElevation((int)other.Size.x, tileY, elevation);
                other.tempData.SetElevation((int)other.Size.x, tileY + 1, elevation);
                chunksToFastBuild.Add(other);

                if (tileY == 0)
                {
                    Chunk extra = chunkList[chunkX - 1, chunkY - 1];
                    extra.tempData.SetElevation((int)extra.Size.x, (int)extra.Size.y, elevation);
                    chunksToFastBuild.Add(extra);
                }
                else if (tileY == ChunkPrefab.Size.y - 1)
                {
                    Chunk extra = chunkList[chunkX - 1, chunkY + 1];
                    extra.tempData.SetElevation((int)extra.Size.x, 0, elevation);
                    chunksToFastBuild.Add(extra);
                }
            }
        }
        catch { }
        try
        {
            if (tileX == (int)chunkList[chunkX + 1, chunkY].Size.x - 1) //right side
            {
                Chunk other = chunkList[chunkX + 1, chunkY];
                other.tempData.SetElevation(0, tileY, elevation);
                other.tempData.SetElevation(0, tileY + 1, elevation);
                chunksToFastBuild.Add(other);

                if (tileY == 0)
                {
                    Chunk extra = chunkList[chunkX + 1, chunkY - 1];
                    extra.tempData.SetElevation(0, (int)extra.Size.y, elevation);
                    chunksToFastBuild.Add(extra);
                }
                else if (tileY == ChunkPrefab.Size.y - 1)
                {
                    Chunk extra = chunkList[chunkX + 1, chunkY + 1];
                    extra.tempData.SetElevation(0, 0, elevation);
                    chunksToFastBuild.Add(extra);
                }
            }
        }
        catch { }
        try
        {
            if (tileY == 0) //bottom side
            {
                Chunk other = chunkList[chunkX, chunkY - 1];
                other.tempData.SetElevation(tileX, (int)other.Size.y, elevation);
                other.tempData.SetElevation(tileX + 1, (int)other.Size.y, elevation);
                if (!chunksToFastBuild.Contains(other))
                    chunksToFastBuild.Add(other);
            }
        }
        catch { }
        try
        {
            if (tileY == (int)chunkList[chunkX, chunkY + 1].Size.y - 1) //top side
            {
                Chunk other = chunkList[chunkX, chunkY + 1];
                other.tempData.SetElevation(tileX, 0, elevation);
                other.tempData.SetElevation(tileX + 1, 0, elevation);
                if (!chunksToFastBuild.Contains(other))
                    chunksToFastBuild.Add(other);
            }
        }
        catch { }

        foreach (Chunk c in chunksToFastBuild)
        {
            AddChunkToTemp(c);
            c.FastBuild(c.tempData);
        }
    }


    public void BridgeSameVertexHeightTemporarily(int chunkX, int chunkY, int vertexX, int vertexY, float elevation)
    {
        try
        {
            if (vertexX == 0) //left side
            {
                Chunk other = chunkList[chunkX - 1, chunkY];
                other.tempData.SetElevation((int)other.Size.x, vertexY, elevation);
                other.FastBuild(other.tempData);

                if (vertexY == 0)
                {
                    Chunk extra = chunkList[chunkX - 1, chunkY - 1];
                    extra.tempData.SetElevation((int)extra.Size.x, (int)extra.Size.y, elevation);
                    extra.FastBuild(extra.tempData);
                }
            }
        }
        catch { }
        try
        {
            if (vertexX == (int)chunkList[chunkX + 1, chunkY].Size.x) //right side
            {
                Chunk other = chunkList[chunkX + 1, chunkY];
                other.tempData.SetElevation(0, vertexY, elevation);
                other.FastBuild(other.tempData);

                if (vertexY == 0)
                {
                    Chunk extra = chunkList[chunkX, chunkY - 1];
                    extra.tempData.SetElevation((int)extra.Size.x, (int)extra.Size.y, elevation);
                    extra.FastBuild(extra.tempData);
                }
            }
        }
        catch { }
        try
        {
            if (vertexY == 0) //bottom side
            {
                Chunk other = chunkList[chunkX, chunkY - 1];
                other.tempData.SetElevation(vertexX, (int)other.Size.y, elevation);
                other.FastBuild(other.tempData);
            }
        }
        catch { }
        try
        {
            if (vertexY == (int)chunkList[chunkX, chunkY + 1].Size.y) //top side
            {
                Chunk other = chunkList[chunkX, chunkY + 1];
                other.tempData.SetElevation(vertexX, 0, elevation);
                other.FastBuild(other.tempData);
            }
        }
        catch { }
    }
}
