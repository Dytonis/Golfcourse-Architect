using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GA.Ground;
using UnityEngine.EventSystems;
using System;
using GA.UI;
using GA.UI.Windows.HoleInfoMenu;
using GA;

public class ScreenClick : MonoBehaviour
{
    public ClickAction ClickAction = new ClickAction();
    public ChunkFamily Family;
    public StandardGamemode Gamemode;
    public UIController UIController;

    public Tees teesPrefab;
    public Pin pinPrefab;
    public Hole holePrefab;

    public Vector2 mouseVelocityLastFrame;

    // Use this for initialization
    void Start()
    {

    }

    private Vector2 mousePositionLastFrame = Vector2.zero;
    private Vector2 lastTileHovered = new Vector2();
    private Vector2 lastSubTileHovered = new Vector2();
    private bool newTileThisFrame = false;
    private bool newSubTileThisFrame = false;
    private Vector2 currentTileHovered = new Vector2();
    private Vector2 currentSubTileHovered = new Vector2();

    public void CalculateNewTilesThisFrame()
    {
        ScreenMousePositionInfo info = GetScreenMousePositionInfo();

        currentSubTileHovered = info.PreciseGlobal;
        currentTileHovered = info.Global;

        if (currentSubTileHovered == lastSubTileHovered)
            newSubTileThisFrame = false;
        else newSubTileThisFrame = true;

        if (currentTileHovered == lastTileHovered)
            newTileThisFrame = false;
        else newTileThisFrame = true;

        lastTileHovered = currentTileHovered;
        lastSubTileHovered = currentSubTileHovered;
    }

    public ScreenMousePositionInfo GetScreenMousePositionInfo()
    {
        ScreenMousePositionInfo info = new ScreenMousePositionInfo();

        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        info.RayUsed = r;
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 100f))
        {
            if (hit.collider.GetComponent<Chunk>() != null)
            {
                info.Excists = true;

                int triangle = hit.triangleIndex;

                Chunk c = hit.collider.GetComponent<Chunk>();

                info.ChunkOver = c;

                float y = ((triangle / 2) % (int)c.Size.x);
                float x = (triangle / ((int)c.Size.x * 2));

                info.Local = new Vector2(x, y);

                float globalY = c.getGlobalPointFromLocal(x, y).y;
                float globalX = c.getGlobalPointFromLocal(x, y).x;

                info.Global = new Vector2(globalX, globalY);

                float lpX = (int)((hit.point.x % 1) * 8);
                float lpY = (int)((hit.point.z % 1) * 8);

                info.LocalSubTile = new Vector2(lpX, lpY);

                float pX = (lpX / 8f) + globalX;
                float pY = (lpY / 8f) + globalY;

                info.PreciseGlobal = new Vector2(pX, pY);
            }
            else
            {
                info.Excists = false;
            }
        }
        else
        {
            info.Excists = false;
        }

        return info;
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.OverUIController)
        {
            ResetState();
            return;
        }

        ScreenMousePositionInfo info = GetScreenMousePositionInfo();
        CalculateNewTilesThisFrame();

        mouseVelocityLastFrame = new Vector2(Input.mousePosition.x - mousePositionLastFrame.x, Input.mousePosition.y - mousePositionLastFrame.y);

        if (Input.GetMouseButtonDown(0))
        {
            if (ClickAction.type == ClickType.PLACE_TILE_FAIRWAY)
                Family.SetGroundType<Fairway>(info.Global);
            else if (ClickAction.type == ClickType.PLACE_TILE_GREEN)
                Family.SetGroundType<Green>(info.Global);
            else if (ClickAction.type == ClickType.NONE)
            {
                TryActivateHole(info.Global);
            }
        }

        if (ClickAction.type == ClickType.EULER_VERTEX)
            Euler_Vertex(info.RayUsed);
        else if (ClickAction.type == ClickType.PLACE_HOLE_TEES)
            PlaceTeeObject(info.Global, info.ChunkOver);
        else if (ClickAction.type == ClickType.PLACE_HOLE_PIN)
            PlaceHoleObject(info.RayUsed, "pin");

        mousePositionLastFrame = Input.mousePosition;
    }

    private void TryActivateHole(Vector2 position)
    {
        if(Family.GetChunkDataPointTypeofGroundTypeGlobally((int)position.x, (int)position.y) == typeof(Teebox))
        {
            Teebox reference = Family.GetGroundTypeInstance(position) as Teebox;

            HoleInfoMenu prefab = (Resources.Load(ResourceFinder.UIElements.Menus.HoleInfoMenu) as GameObject).GetComponent<HoleInfoMenu>();
            HoleInfoMenu menu = UIElement.Pop(prefab);
            menu.SetHole(reference.TeesBoundTo.HoleBelongingTo);
        }
    }

    public void ResetState()
    {
        if (teeObject)
        {
            Destroy(teeObject.gameObject);
            Family.ResetChunkTempMemory(true);
        }
        if (pinObject)
        {
            Destroy(pinObject.gameObject);
        }

        if (Family.CurrentHoleCreating)
        {
            if (Family.CurrentHoleCreating.line)
            {
                Family.CurrentHoleCreating.line.enabled = false;
            }
        }
    }

    Tees teeObject;
    Pin pinObject;
    [System.Obsolete]
    private void PlaceHoleObject(Ray r, string obj) //needs refactoring, cyclomatic complexity is 30
    {
        if (mouseVelocityLastFrame == Vector2.zero && Input.GetMouseButtonUp(0) == false)
        {
        }
        else
        {
            if (obj == "tee")
            {
                if (teeObject == null)
                    teeObject = Instantiate(teesPrefab) as Tees;

                if (Family.CurrentHoleCreating == null)
                {
                    Family.CurrentHoleCreating = Instantiate(holePrefab);
                    Family.CurrentHoleCreating.Init(Gamemode, Family, UIController);
                    Family.CurrentHoleCreating.OnCreation();
                }

                else if (Family.CurrentHoleCreating.ConstructingCurrently == false)
                {
                    Family.CurrentHoleCreating.OnStartConstruction();
                    Family.CurrentHoleCreating.ConstructingCurrently = true;
                }

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

                        float lowest = Family.GetLowestElevationUnderTileGlobalRaycast(globalX, globalY);

                        teeObject.Position = new Vector3(globalX + 0.5f, lowest + 0.03f, globalY + 0.5f);
                        teeObject.family = Family;

                        if (newTileThisFrame)
                        {
                            if (Family.CurrentHoleCreating.currentPin && teeObject)
                                Family.CurrentHoleCreating.Construction_CalculateTempLine(teeObject, Family.CurrentHoleCreating.currentPin);
                        }

                        if (Family.CurrentHoleCreating != null)
                        {
                            if (Family.CurrentHoleCreating.line == null)
                                Family.CurrentHoleCreating.line = Family.CurrentHoleCreating.GetDrawLine();
                        }

                        Family.CurrentHoleCreating.lineTeeObject = teeObject;

                        if (Input.GetMouseButtonUp(0))
                        {
                            Family.ModifyChunkDataPointTileElevationGlobally((int)globalX, (int)globalY, lowest);
                            Family.ModifyChunkDataPointTypeGlobally((int)globalX, (int)globalY, new GA.Ground.Teebox());
                            Tees newTees = Instantiate(teeObject, teeObject.transform.position, teeObject.transform.rotation);
                            newTees.FlatPosition = new Vector2(globalX, globalY);
                            newTees.CreateFencing();
                            UIController.DeactivateMinors();
                            ClickAction.type = ClickType.NONE;
                            c.FastBuild(c.data);
                            c.BuildTexture(c.data);

                            newTees.OnPlacement(Family, UIController, Family.CurrentHoleCreating);
                            ResetState();
                        }
                    }
                }
            }
            else if (obj == "pin")
            {
                if (pinObject == null)
                    pinObject = Instantiate(pinPrefab) as Pin;

                if (Family.CurrentHoleCreating == null)
                {
                    Family.CurrentHoleCreating = Instantiate(holePrefab);
                    Family.CurrentHoleCreating.Init(Gamemode, Family, UIController);
                    Family.CurrentHoleCreating.OnCreation();
                }

                if (!Family.CurrentHoleCreating.ConstructingCurrently)
                {
                    Family.CurrentHoleCreating.OnStartConstruction();
                    Family.CurrentHoleCreating.ConstructingCurrently = true;
                }

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

                        float lpX = (int)((hit.point.x % 1) * 8);
                        float lpY = (int)((hit.point.z % 1) * 8);

                        float pX = (lpX / 8f) + globalX;
                        float pY = (lpY / 8f) + globalY;
                        float pE = Family.GetElevationUnderPointGlobalRaycast(pX, pY);

                        Material[] pinMaterials;
                        pinMaterials = pinObject.GetComponent<MeshRenderer>().materials;
                        if (Family.GetChunkDataPointTypeofGroundTypeGlobally((int)globalX, (int)globalY) != typeof(GA.Ground.Green))
                        {
                            foreach (Material m in pinMaterials)
                            {
                                m.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
                            }
                        }
                        else
                        {
                            foreach (Material m in pinMaterials)
                            {
                                m.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                            }
                        }

                        pinObject.PositionFine = new Vector3(pX + (1f / 16f), pE + 0.12f, pY + (1f / 16f));

                        if (newSubTileThisFrame)
                        {
                            if (Family.CurrentHoleCreating.TeesList.Count > 0 && pinObject)
                                Family.CurrentHoleCreating.Construction_CalculateTempLine(Family.CurrentHoleCreating.TeesList[0], pinObject);
                        }

                        if (Family.CurrentHoleCreating.line == null)
                            Family.CurrentHoleCreating.line = Family.CurrentHoleCreating.GetDrawLine();

                        if (Family.CurrentHoleCreating.TeesList.Count > 0)
                            Family.CurrentHoleCreating.lineTeeObject = Family.CurrentHoleCreating.TeesList[0];

                        Family.CurrentHoleCreating.pinLineObject = pinObject;

                        if (Input.GetMouseButtonUp(0) && Family.GetChunkDataPointTypeofGroundTypeGlobally((int)globalX, (int)globalY) == typeof(GA.Ground.Green))
                        {
                            c.SetPositionAsHole((int)x, (int)y, (int)(lpX), (int)(lpY));
                            ChunkData old = c.data.DeepCopy();
                            c.BuildSmartSingleTexture(c.data, old, (int)x, (int)y);
                            Pin newPin = Instantiate(pinObject, pinObject.transform.position, pinObject.transform.rotation);
                            newPin.PositionFine = new Vector3(pX + (1f / 16f), pE - 0.03f, pY + (1f / 16f));
                            UIController.DeactivateMinors();
                            ClickAction.InvokeAction();
                            ResetState();

                            newPin.OnPlacement(Family, UIController);
                        }
                    }
                }
            }
        }
    }

    private void PlaceTeeObject(Vector2 position, Chunk c)
    {
        if (teeObject == null)
            teeObject = Instantiate(teesPrefab) as Tees;                                    //Create the temporary tee object

        if (Family.CurrentHoleCreating == null)
        {
            Family.CurrentHoleCreating = Instantiate(holePrefab);                           //Initialize the hole if there isn't one
            Family.CurrentHoleCreating.Init(Gamemode, Family, UIController);
            Family.CurrentHoleCreating.OnCreation();
        }
        else if (Family.CurrentHoleCreating.ConstructingCurrently == false)                 //Start the construction process if it needs to be
        {
            Family.CurrentHoleCreating.OnStartConstruction();
            Family.CurrentHoleCreating.ConstructingCurrently = true;
        }

        float lowest = Family.GetLowestElevationUnderTileGlobalRaycast(position.x, position.y);
        teeObject.Position = new Vector3(position.x + 0.5f, lowest + 0.03f, position.y + 0.5f);
        teeObject.family = Family;

        if (newTileThisFrame)
        {
            if (Family.CurrentHoleCreating.currentPin && teeObject)
                Family.CurrentHoleCreating.Construction_CalculateTempLine(teeObject, Family.CurrentHoleCreating.currentPin);
        }

        if (Family.CurrentHoleCreating != null)
        {
            if (Family.CurrentHoleCreating.line == null)
                Family.CurrentHoleCreating.line = Family.CurrentHoleCreating.GetDrawLine();
        }

        Family.CurrentHoleCreating.lineTeeObject = teeObject;

        if (Input.GetMouseButtonUp(0))
        {
            Family.ModifyChunkDataPointTileElevationGlobally((int)position.x, (int)position.y, lowest);
            Tees newTees = Instantiate(teeObject, teeObject.transform.position, teeObject.transform.rotation);
            newTees.FlatPosition = new Vector2((int)position.x, (int)position.y);
            newTees.CreateFencing();
            Teebox teebox = new Teebox();
            teebox.TeesBoundTo = newTees;
            Family.SetComplexGroundType(position, teebox);
            UIController.DeactivateMinors();
            ClickAction.InvokeAction();
            c.FastBuild(c.data);

            newTees.OnPlacement(Family, UIController, Family.CurrentHoleCreating);

            ResetState();
        }
    }

    [System.Obsolete]
    private void ChangeTileType(Ray r, GA.Ground.GroundType type)
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
                    ChunkData old = c.data.DeepCopy();

                    int y = (triangle / 2) % (int)c.Size.x;
                    int x = triangle / ((int)c.Size.x * 2);

                    c.data.SetGroundType(x, y, type);
                    c.BuildSmartSingleTexture(c.data, old, x, y);
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

                if ((hit.point.x) % (c.Size.x) >= c.Size.x - 0.5f) //near boundry, need to switch tiles to next chunk over
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

[System.Serializable]
public class ClickAction
{
    public ClickType type;
    private Action Callback;

    public ClickAction()
    {
        Callback = new Action(() => { type = ClickType.NONE; });
    }

    public ClickAction(Action callback)
    {
        Callback = callback;
    }

    public ClickAction(ClickType type)
    {
        this.type = type;
    }

    public ClickAction(ClickType type, Action callback)
    {
        this.type = type;
        Callback = callback;
    }

    public void InvokeAction()
    {
        Callback.Invoke();
    }
}

public enum ClickType
{
    NONE = 0,
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

public struct ScreenMousePositionInfo
{
    public bool Excists;
    public Ray RayUsed;
    public Vector2 Local;
    public Vector2 LocalSubTile;
    public Chunk ChunkOver;
    public Vector2 Global;
    public Vector2 PreciseGlobal;
}
