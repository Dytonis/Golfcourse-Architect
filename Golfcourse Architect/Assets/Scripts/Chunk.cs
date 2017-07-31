using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Chunk : MonoBehaviour
{
    public bool Longest = true;

    public Vector2 GlobalPosition;
    public Vector2 Size;

    public bool Viable;
    public Dictionary<Vector2, NatureObject> NatureObjects = new Dictionary<Vector2, NatureObject>();
    internal List<Vector3> vertices = new List<Vector3>();
    internal Vector3[] normals;
    public int[] triangles;
    internal Vector2[] uv;

    internal int dupeVertCount = 8;
    private int tileTexSize = 8;

    public Mesh mesh;
    public MeshFilter[] filters;

    public MeshRenderer L0;
    public MeshRenderer L1;
    public MeshRenderer L2;

    public ChunkData data;
    public ChunkData tempData;

    public void ApplyMesh()
    {
        foreach (MeshFilter f in filters)
        {
            f.mesh = mesh;
        }
    }

    public void ResetTempData()
    {
        tempData = new ChunkData(this);
        tempData.Init(this, data.data);
    }

    public void FastBuild(ChunkData cd)
    {
        for (int y = 0; y < Size.y + 1; y++)
        {
            for (int x = 0; x < Size.x + 1; x++)
            {
                try
                {
                    int vert = getVertByXY(x, y);

                    for (int c = 0; c < dupeVertCount; c++)
                    {
                        vertices[vert + c] = new Vector3(vertices[vert + c].x, cd[x, y].elevation, vertices[vert + c].z);
                    }
                }
                catch
                { }
            }
        }

        RebuildTriangleStructure();
        mesh.vertices = vertices.ToArray();
        mesh.SetUVs(0, uv.ToList());
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        ApplyMesh();
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void BuildTexture(ChunkData cd)
    {
        Texture2D texL0 = L0.material.mainTexture as Texture2D;
        Texture2D texL1 = L1.material.mainTexture as Texture2D;
        Texture2D texL2 = L2.material.mainTexture as Texture2D;

        if (texL0 == null)
        {
            texL0 = new Texture2D(tileTexSize * (int)Size.x, tileTexSize * (int)Size.y);
            L0.material.SetTexture(0, texL0);
        }
        if (texL1 == null)
        {
            texL1 = new Texture2D(tileTexSize * (int)Size.x, tileTexSize * (int)Size.y);
            L1.material.SetTexture(0, texL1);
        }
        if (texL2 == null)
        {
            texL2 = new Texture2D(tileTexSize * (int)Size.x, tileTexSize * (int)Size.y);
            L2.material.SetTexture(0, texL2);
        }

        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                int sizeX = x * tileTexSize;
                int sizeY = y * tileTexSize;

                texL0.SetPixels(sizeX, sizeY, tileTexSize, tileTexSize, cd[x, y].type.GetColorsFromTexture()[0]);
                texL1.SetPixels(sizeX, sizeY, tileTexSize, tileTexSize, cd[x, y].type.GetColorsFromTexture()[1]);
                texL2.SetPixels(sizeX, sizeY, tileTexSize, tileTexSize, cd[x, y].type.GetColorsFromTexture()[2]);
            }
        }

        texL0.filterMode = FilterMode.Point;
        texL1.filterMode = FilterMode.Point;
        texL2.filterMode = FilterMode.Point;

        L0.material.SetTexture("_MainTex", texL0);
        L1.material.SetTexture("_MainTex", texL1);
        L2.material.SetTexture("_MainTex", texL2);

        texL0.Apply();
        texL1.Apply();
        texL2.Apply();
    }

    private Dictionary<Vector2, QuadReference> TriangleQuadDict = new Dictionary<Vector2, QuadReference>();

    public void PlaceAllObjectsPrimitive(ChunkData data)
    {
        for(int y = 0; y < Size.y; y++)
        {
            for(int x = 0; x < Size.x; x++)
            {
                if(data[x, y].ObjectResiding != ObjectID.EMPTY)
                {
                    Vector2 globalPos = getGlobalPointFromLocal(x, y);
                    Vector3 pos = new Vector3(globalPos.x + 0.5f, getElevationUnderPointLocal(x + 0.5f, y + 0.5f), globalPos.y + 0.5f);

                    NatureObject o = Instantiate(NatureObject.GetObjectPrefabFromID(data[x, y].ObjectResiding), pos, transform.rotation);
                    o.transform.SetParent(transform);
                    o.source = this;
                    o.localPosition = new Vector2(x, y);
                    o.transform.localScale = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
                    o.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360f), 0));
                    NatureObjects.Add(o.localPosition, o);
                }
            }
        }
    }

    public void UpdateObjects(ChunkData data)
    {
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                ObjectID currentID = data[x, y].ObjectResiding;
                try
                {
                    if (NatureObjects[new Vector2(x, y)].objectID != currentID)
                    {
                        Destroy(NatureObjects[new Vector2(x, y)].gameObject);

                        if (currentID != ObjectID.EMPTY)
                        {
                            Vector2 globalPos = getGlobalPointFromLocal(x, y);
                            Vector3 pos = new Vector3(globalPos.x + 0.5f, getElevationUnderPointLocal(x + 0.5f, y + 0.5f), globalPos.y + 0.5f);

                            NatureObject o = Instantiate(NatureObject.GetObjectPrefabFromID(currentID), pos, transform.rotation);
                            o.transform.SetParent(transform);
                            o.source = this;
                            o.localPosition = new Vector2(x, y);
                            o.transform.localScale = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
                            o.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360f), 0));
                            NatureObjects.Add(o.localPosition, o);
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }

    public void Build(ChunkData data)
    {
        triangles = new int[(int)(Size.x * Size.y) * 6];

        mesh = new Mesh();

        vertices.Clear();
        TriangleQuadDict.Clear();
        dic.Clear();

        for (int y = 0; y < Size.y + 1; y++)
        {
            for (int x = 0; x < Size.x + 1; x++)
            {
                int index = x + y;
                for (int i = 0; i < dupeVertCount; i++)
                {
                    vertices.Add(new Vector3(x, 0, y));
                }
            }
        }

        normals = new Vector3[vertices.Count];
        uv = new Vector2[vertices.Count];

        int v = 0;
        for (int x = 0; x < (int)Size.x; x++)
        {
            for (int y = 0; y < (int)Size.y; y++)
            {
                triangles[v] = checkoutUniqueVertByXY(x, y);
                triangles[v + 1] = checkoutUniqueVertByXY(x, y + 1);
                triangles[v + 2] = checkoutUniqueVertByXY(x + 1, y);
                triangles[v + 3] = checkoutUniqueVertByXY(x + 1, y);
                triangles[v + 4] = checkoutUniqueVertByXY(x, y + 1);
                triangles[v + 5] = checkoutUniqueVertByXY(x + 1, y + 1);

                try
                {
                    TriangleQuadDict.Add(new Vector2(x, y), new QuadReference(new Vector3(triangles[v], triangles[v + 1], triangles[v + 2]), new Vector3(triangles[v + 3], triangles[v + 4], triangles[v + 5])));
                }
                catch { }
                v += 6;
            }
        }

        //elevation per point
        for (int y = 0; y < Size.y + 1; y++)
        {
            for (int x = 0; x < Size.x + 1; x++)
            {
                try
                {
                    Vector2 pos = new Vector2(x, y);
                    QuadReference q = TriangleQuadDict[pos];

                    for (int c = 0; c < dupeVertCount; c++)
                    {
                        int vert = getVertByXY(x, y) + c;
                        vertices[vert] = new Vector3(vertices[vert].x, data[x, y].elevation, vertices[vert].z);
                    }
                }
                catch
                { }
            }
        }

        //rebuild structure
        //check the 4 non quality tiles around for shortest length
        //if not, rotate the quad by 90 degrees
        RebuildTriangleStructure();

        BuildUVs();

        //RotateQuad(new Vector2(1, 1));
        //RotateQuad(new Vector2(0, 0));
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        ApplyMesh();
        BuildTexture(data);

        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void BuildUVs()
    {
        for (int y = 0, i = 0; y < (Size.y + 1) * dupeVertCount; y += dupeVertCount)
        {
            for (int x = 0; x < (Size.x + 1) * dupeVertCount; x += dupeVertCount, i += dupeVertCount)
            {
                Vector2 value = new Vector2(x / (Size.x * dupeVertCount), y / (Size.y * dupeVertCount));
                for (int c = 0; c < dupeVertCount; c++)
                {
                    uv[i + c] = value;
                }
            }
        }
    }

    private void RebuildTriangleStructure()
    {
        for (int y = 0; y < Size.y; y++)
        {
            //break;
            for (int x = 0; x < Size.x; x++)
            {
                try
                {
                    QuadReference q = TriangleQuadDict[new Vector2(x, y)];

                    Vector3 A, B, C, D;

                    if (q.rotated)
                    {
                        A = vertices[q.a];
                        B = vertices[q.c];
                        C = vertices[q.f];
                        D = vertices[q.b];
                    }
                    else
                    {
                        A = vertices[q.a];
                        B = vertices[q.f];
                        C = vertices[q.c];
                        D = vertices[q.e];
                    }

                    if (!Longest)
                    {
                        if (q.rotated)
                        {
                            if (Vector3.Distance(A, B) > Vector3.Distance(C, D))
                            {
                                RotateQuad(new Vector2(x, y), q.rotated);
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(A, B) < Vector3.Distance(C, D))
                            {
                                RotateQuad(new Vector2(x, y), q.rotated);
                            }
                        }
                    }
                    else
                    {
                        if (q.rotated)
                        {
                            if (Vector3.Distance(A, B) < Vector3.Distance(C, D))
                            {
                                RotateQuad(new Vector2(x, y), q.rotated);
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(A, B) > Vector3.Distance(C, D))
                            {
                                RotateQuad(new Vector2(x, y), q.rotated);
                            }
                        }
                        Debug.DrawLine(A, B, Color.red, 0.1f);
                        Debug.DrawLine(C, D, Color.blue, 0.1f);
                    }
                }
                catch { }
            }
        }
    }

    private void RotateQuad(Vector2 pos, bool invert = false)
    {
        if (!invert)
        {
            try
            {
                QuadReference q = TriangleQuadDict[pos];

                int offset = (((int)pos.x * ((int)Size.y)) + (int)pos.y) * 6;

                checkinUniqueVert(q.a);
                checkinUniqueVert(q.b);
                checkinUniqueVert(q.c);
                checkinUniqueVert(q.d);
                checkinUniqueVert(q.e);
                checkinUniqueVert(q.f);

                triangles[offset] = checkoutUniqueVertByXY((int)pos.x, (int)pos.y);
                triangles[offset + 1] = checkoutUniqueVertByXY((int)pos.x, (int)pos.y + 1);
                triangles[offset + 2] = checkoutUniqueVertByXY((int)pos.x + 1, (int)pos.y + 1);
                triangles[offset + 3] = checkoutUniqueVertByXY((int)pos.x, (int)pos.y);
                triangles[offset + 4] = checkoutUniqueVertByXY((int)pos.x + 1, (int)pos.y + 1);
                triangles[offset + 5] = checkoutUniqueVertByXY((int)pos.x + 1, (int)pos.y);

                q.a = triangles[offset];
                q.b = triangles[offset + 1];
                q.c = triangles[offset + 2];
                q.d = triangles[offset + 3];
                q.e = triangles[offset + 4];
                q.f = triangles[offset + 5];

                q.quality = true;
                q.rotated = !invert;

                TriangleQuadDict[pos] = q;
            }
            catch
            { }
        }
        else
        {
            try
            {
                QuadReference q = TriangleQuadDict[pos];

                int offset = (((int)pos.x * ((int)Size.y)) + (int)pos.y) * 6;

                checkinUniqueVert(q.a);
                checkinUniqueVert(q.b);
                checkinUniqueVert(q.c);
                checkinUniqueVert(q.d);
                checkinUniqueVert(q.e);
                checkinUniqueVert(q.f);

                triangles[offset] = checkoutUniqueVertByXY((int)pos.x, (int)pos.y);
                triangles[offset + 1] = checkoutUniqueVertByXY((int)pos.x, (int)pos.y + 1);
                triangles[offset + 2] = checkoutUniqueVertByXY((int)pos.x + 1, (int)pos.y);
                triangles[offset + 3] = checkoutUniqueVertByXY((int)pos.x + 1, (int)pos.y);
                triangles[offset + 4] = checkoutUniqueVertByXY((int)pos.x, (int)pos.y + 1);
                triangles[offset + 5] = checkoutUniqueVertByXY((int)pos.x + 1, (int)pos.y + 1);

                q.a = triangles[offset];
                q.b = triangles[offset + 1];
                q.c = triangles[offset + 2];
                q.d = triangles[offset + 3];
                q.e = triangles[offset + 4];
                q.f = triangles[offset + 5];

                q.quality = true;
                q.rotated = !invert;

                TriangleQuadDict[pos] = q;
            }
            catch
            { }
        }
    }

    private Dictionary<Vector2, int> dic = new Dictionary<Vector2, int>();
    private int getUniqueVertPerQuadByXY(int x, int y)
    {
        int v = 0;

        try
        {
            v = dic[new Vector2(x, y)];
            dic[new Vector2(x, y)] = ++v;
        }
        catch { dic.Add(new Vector2(x, y), v); }

        return ((x * dupeVertCount) + (y * dupeVertCount * ((int)Size.x + 1))) + v;
    }

    private Dictionary<int, bool> memory = new Dictionary<int, bool>();
    private int checkoutUniqueVertByXY(int x, int y)
    {
        int vert = -1;
        bool inUse = true;
        int check = ((x * dupeVertCount) + (y * dupeVertCount * ((int)Size.x + 1)));

        try
        {
            for (int i = 0; i < dupeVertCount; i++)
            {
                inUse = memory[check];
                if (inUse == true)
                    check++;
                else
                {
                    memory[check] = true;
                    vert = check;
                    return vert;
                }
            }
        }
        catch { memory.Add(check, true); }

        return check;
    }

    internal Vector2 globalXYToVertex(float globalX, float globalY)
    {
        return new Vector2(Mathf.RoundToInt(globalX) % (Size.x), Mathf.RoundToInt(globalY) % (Size.y));
    }

    private void checkinUniqueVert(int vert)
    {
        try
        {
            memory[vert] = false;
        }
        catch
        {

        }
    }

    private void returnUniqueVertPerQuadByXY(int x, int y)
    {
        int v = 0;

        try
        {
            v = dic[new Vector2(x, y)];
            if (v > 0)
                dic[new Vector2(x, y)] = --v;
        }
        catch { }
    }

    internal int getVertByXY(int x, int y)
    {
        if (x < 0 || x > Size.x + 2)
            return -1;
        if (y < 0 || y > Size.y + 2)
            return -1;

        int result = ((x * dupeVertCount) + (y * dupeVertCount * ((int)Size.x + 1)));
        return result;
    }

    private int getVertByXYWithQuad(int x, int y, QuadReference q)
    {
        if (x < 0 || x > Size.x + 1)
            return -1;
        if (y < 0 || y > Size.y + 1)
            return -1;

        int result = ((x * dupeVertCount) + (y * dupeVertCount * ((int)Size.x + 1)));

        if (q.a >= result && q.a < result + 4)
            return q.a;
        else if (q.b >= result && q.b < result + 4)
            return q.b;
        else if (q.c >= result && q.c < result + 4)
            return q.c;
        else if (q.d >= result && q.d < result + 4)
            return q.d;
        else if (q.e >= result && q.e < result + 4)
            return q.e;
        else if (q.f >= result && q.f < result + 4)
            return q.f;

        return -1;
    }

    internal QuadReference quadFromXY(int x, int y)
    {
        return TriangleQuadDict[new Vector2(x, y)];
    }

    internal void SetQuad(QuadReference q, int x, int y)
    {
        try
        {
            TriangleQuadDict[new Vector2(x, y)] = q;
        }
        catch
        {
            TriangleQuadDict.Add(new Vector2(x, y), q);
        }
    }

    internal float getElevationUnderPointLocal(float x, float y)
    {
        RaycastHit hit;
        Vector3 pos = new Vector3(x + (GlobalPosition.x * Size.x), 1000, y + (GlobalPosition.y * Size.y));
        Debug.DrawRay(pos, Vector3.down * 2000, Color.magenta, 10f);
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, 2000))
        {
            return hit.point.y;
        }

        return 0;
    }

    internal Vector2 getGlobalPointFromLocal(float x, float y)
    {
        return new Vector2(x + (GlobalPosition.x * Size.x), y + (GlobalPosition.y * Size.y));
    }
}

internal struct QuadReference
{
    public int a, b, c, d, e, f;
    public bool quality;
    public bool rotated;

    public QuadReference(Vector3 tri1, Vector3 tri2, bool quality = false, bool rotated = false)
    {
        a = (int)tri1.x;
        b = (int)tri1.y;
        c = (int)tri1.z;
        d = (int)tri2.x;
        e = (int)tri2.y;
        f = (int)tri2.z;
        this.quality = quality;
        this.rotated = rotated;
    }
}
