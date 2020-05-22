using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class MapController : MonoBehaviour
{
    public enum TilesetCode
    {
        RED, BLUE, YELLOW, BASE_CANI, BASE_HIPSTER, FACTORY_CANI, FACTORY_HIPSTER, FACTORY_NEUTRAL, BUILDING_CANI, BUILDING_HIPSTER, BUILDING_NEUTRAL,
        NEUTRAL, ROAD, CROSSWALK1, CROSSWALK2, CONTAINER, LAMP, PLANTPOT, SEA, CONE1, CONE2, CONE3, CONE4, CONE5, CONE6, CONE7, CONE8
    };

    public enum Pathfinder
    {
        MAIN, AUXILIAR
    };

    public Dictionary<int, string> tilesetDictionary;

    [Header("Tilemaps")]
    public Tilemap tilemapBase;
    public Tilemap tilemapBuildings;
    public Tilemap tilemapHazards;
    public Tilemap tilemapWalkability;
    public Tilemap tilemapPathfinding;
    public Tilemap tilemapPlayer;

    [Header("Prefabs")]
    public GameObject baseCaniPrefab;
    public GameObject baseHipsterPrefab;
    public GameObject factoryCaniPrefab;
    public GameObject factoryHipsterPrefab;
    public GameObject factoryNeutralPrefab;
    public GameObject buildingCaniPrefab;
    public GameObject buildingHipsterPrefab;
    public GameObject buildingNeutralPrefab;

    private uint width;
    private uint height;
    private uint xOffset;
    private uint yOffset;

    private Vector2 topLeftCorner;
    private Vector2 bottomRightCorner;

    //Pathfinding

    public Pathfinding pathfinding;
    public Pathfinding auxiliarPathfinding; //per buscar rutes complexes alternatives AI

    [Header("Pathfinding")]
    public AnimatedTile blueTile;
    public AnimatedTile redTile;
    public Tile arrowRight;
    public Tile arrowLeft;
    public Tile arrowUp;
    public Tile arrowDown;
    public Tile arrowVertical;
    public Tile arrowHorizontal;
    public Tile arrowTopRight;
    public Tile arrowTopLeft;
    public Tile arrowBottomRight;
    public Tile arrowBottomLeft;

    //events
    public UnityEvent mapUnloaded;
    public UnityEvent mapLoaded;

    //important primera vegada que es crida l'unload hi haurà el gym carregat, les properes vegades s'ha de mirar que el gym no hi sigui
    bool isLoaded = true;

    // Start is called before the first frame update
    void Start()
    {
        mapUnloaded = new UnityEvent();
        mapLoaded = new UnityEvent();

        if (tilemapBase != null)
        {
            width = (uint)tilemapBase.size.x;
            height = (uint)tilemapBase.size.y;

            Debug.Log("MapController::Start - Map size is " + width + " x " + height);
        }
        else
        {
            Debug.LogError("MapController::Start - No Tilemap found");
        }

        HelloWorld();
        BuildDictionary();
        SpawnBuildings();
        InitializePathfinding();
        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    void BuildDictionary()
    {
        tilesetDictionary = new Dictionary<int, string>();

        //base
        tilesetDictionary.Add((int)TilesetCode.NEUTRAL, "tileset_0");
        tilesetDictionary.Add((int)TilesetCode.ROAD, "tileset_1");
        tilesetDictionary.Add((int)TilesetCode.CROSSWALK1, "tileset_2");
        tilesetDictionary.Add((int)TilesetCode.CROSSWALK2, "tileset_3");
        tilesetDictionary.Add((int)TilesetCode.SEA, "animated_water");
        tilesetDictionary.Add((int)TilesetCode.CONE1, "tileset_35");
        tilesetDictionary.Add((int)TilesetCode.CONE2, "tileset_36");
        tilesetDictionary.Add((int)TilesetCode.CONE3, "tileset_37");
        tilesetDictionary.Add((int)TilesetCode.CONE4, "tileset_38");
        tilesetDictionary.Add((int)TilesetCode.CONE5, "tileset_39");
        tilesetDictionary.Add((int)TilesetCode.CONE6, "tileset_40");
        tilesetDictionary.Add((int)TilesetCode.CONE7, "tileset_41");
        tilesetDictionary.Add((int)TilesetCode.CONE8, "tileset_42");

        //buildings
        tilesetDictionary.Add((int)TilesetCode.BASE_CANI, "tileset_12");
        tilesetDictionary.Add((int)TilesetCode.BASE_HIPSTER, "tileset_13");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_CANI, "tileset_16");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_HIPSTER, "tileset_17");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_NEUTRAL, "tileset_18");
        tilesetDictionary.Add((int)TilesetCode.BUILDING_CANI, "tileset_43");
        tilesetDictionary.Add((int)TilesetCode.BUILDING_HIPSTER, "tileset_44");
        tilesetDictionary.Add((int)TilesetCode.BUILDING_NEUTRAL, "tileset_45");

        //hazards
        tilesetDictionary.Add((int)TilesetCode.CONTAINER, "tileset_5");
        tilesetDictionary.Add((int)TilesetCode.LAMP, "tileset_10");
        tilesetDictionary.Add((int)TilesetCode.PLANTPOT, "tileset_33");

        //walkability
        tilesetDictionary.Add((int)TilesetCode.RED, "tileset_14");
        tilesetDictionary.Add((int)TilesetCode.BLUE, "tileset_15");
        tilesetDictionary.Add((int)TilesetCode.YELLOW, "tileset_19");
    }

    public uint GetWidth()
    {
        return width;
    }

    public uint GetHeight()
    {
        return height;
    }

    public uint GetxOffset()
    {
        return xOffset;
    }

    public uint GetyOffset()
    {
        return yOffset;
    }

    // Posicionament inicial

    void HelloWorld()
    {
        if (width % 2 == 0)
            xOffset = width / 2;
        else
            xOffset = (width - 1) / 2;

        if (height % 2 == 0)
            yOffset = height / 2;
        else
            yOffset = (height - 1) / 2;

        Debug.Log("MapController::HelloWorld - xOffset = " + xOffset + ", yOffset = " + yOffset);

        Vector3 offset = new Vector3(xOffset, -yOffset, 0);

        // Aplico el mateix offset a la càmera
        Camera.main.GetComponent<Transform>().position = new Vector3(0, 0, -10) + offset;
        Camera.main.GetComponent<CameraController>().cameraMoved.Invoke();

        topLeftCorner = transform.position;
        bottomRightCorner = (Vector2)transform.position + new Vector2(width - 1, -height + 1);

        Debug.Log("MapController::HelloWorld - TopLeft = " + topLeftCorner + " , BottomRight = " + bottomRightCorner);
    }

    public void LoadMap(GameObject toLoad)
    {
        GameObject newMap = Instantiate(toLoad);

        newMap.name = "Map";
        newMap.transform.parent = transform;

        tilemapBase = newMap.transform.Find("Tilemap_base").GetComponent<Tilemap>();
        tilemapBuildings = newMap.transform.Find("Tilemap_buildings").GetComponent<Tilemap>();
        tilemapHazards = newMap.transform.Find("Tilemap_hazards").GetComponent<Tilemap>();
        tilemapWalkability = newMap.transform.Find("Tilemap_walkability").GetComponent<Tilemap>();
        tilemapPathfinding = newMap.transform.Find("Tilemap_pathfinding").GetComponent<Tilemap>();
        tilemapPlayer = newMap.transform.Find("Tilemap_player").GetComponent<Tilemap>();

        if (tilemapBase != null)
        {
            width = (uint)tilemapBase.size.x;
            height = (uint)tilemapBase.size.y;

            Debug.Log("MapController::Start - Map size is " + width + " x " + height);
        }
        else
        {
            Debug.LogError("MapController::Start - No Tilemap found");
            return;
        }

        HelloWorld();
        SpawnBuildings();
        InitializePathfinding();

        mapLoaded.Invoke();

        isLoaded = true;
    }

    public void UnloadMap()
    {
        if (isLoaded)
        {
            tilemapBase = null;
            tilemapBuildings = null;
            tilemapHazards = null;
            tilemapWalkability = null;
            tilemapPathfinding = null;
            tilemapPlayer = null;

            DestroyBuildings();
            Destroy(transform.Find("Map").gameObject);

            GameObject.Find("Camera").GetComponent<CameraController>().fadeToWhiteRest.RemoveListener(UnloadMap);

            mapUnloaded.Invoke();

            isLoaded = false;
        }
    }

    public Vector2 GetTopLeftCorner()
    {
        return topLeftCorner;
    }

    public Vector2 GetBottomRightCorner()
    {
        return bottomRightCorner;
    }

    void SpawnBuildings()
    {
        // aquest mètode itera totes les caselles del tilemapBuildings i spawneja un prefab de cada building allà on hi troba cert tipus de tile

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j > -height; j--)
            {
                TileBase tile = tilemapBuildings.GetTile(WorldToTilemap(new Vector3(i, j, 0)));

                if (tile != null)
                {
                    if (tile.name != tilesetDictionary[(int)TilesetCode.RED]) // descartem les caselles vermelles perquè marquen els límits del mapa en cada tilemap
                    {
                        SpawnBuildingByTile(tile, new Vector3(i, j, 0));
                    }
                }
            }
        }
    }

    void SpawnBuildingByTile(TileBase tile, Vector3 pos)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.BASE_CANI])
            SpawnBaseCani(pos);

        else if (tile.name == tilesetDictionary[(int)TilesetCode.BASE_HIPSTER])
            SpawnBaseHipster(pos);

        else if (tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_CANI])
            SpawnFactoryCani(pos);

        else if (tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_HIPSTER])
            SpawnFactoryHipster(pos);

        else if (tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_NEUTRAL])
            SpawnFactoryNeutral(pos);

        else if (tile.name == tilesetDictionary[(int)TilesetCode.BUILDING_CANI])
            SpawnBuildingCani(pos);

        else if (tile.name == tilesetDictionary[(int)TilesetCode.BUILDING_HIPSTER])
            SpawnBuildingHipster(pos);

        else if (tile.name == tilesetDictionary[(int)TilesetCode.BUILDING_NEUTRAL])
            SpawnBuildingNeutral(pos);
    }

    void SpawnBaseCani(Vector3 pos)
    {
        GameObject newGO = Instantiate(baseCaniPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;

        GameObject.Find("Data Controller").transform.Find("Buildings Controller").GetComponent<BuildingsController>().caniBase = newGO;
        Debug.Log("MapController::SpawnBaseCani - Created Base_cani in " + newGO.transform.position);
    }

    void SpawnBaseHipster(Vector3 pos)
    {
        GameObject newGO = Instantiate(baseHipsterPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;

        GameObject.Find("Data Controller").transform.Find("Buildings Controller").GetComponent<BuildingsController>().hipsterBase = newGO;
        Debug.Log("MapController::SpawnBaseCani - Created Base_hipster in " + newGO.transform.position);
    }

    void SpawnFactoryCani(Vector3 pos)
    {
        GameObject newGO = Instantiate(factoryCaniPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;
    }

    void SpawnFactoryHipster(Vector3 pos)
    {
        GameObject newGO = Instantiate(factoryHipsterPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;
    }

    void SpawnFactoryNeutral(Vector3 pos)
    {
        GameObject newGO = Instantiate(factoryNeutralPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;
    }

    void SpawnBuildingCani(Vector3 pos)
    {
        GameObject newGO = Instantiate(buildingCaniPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;
    }

    void SpawnBuildingHipster(Vector3 pos)
    {
        GameObject newGO = Instantiate(buildingHipsterPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;
    }

    void SpawnBuildingNeutral(Vector3 pos)
    {
        GameObject newGO = Instantiate(buildingNeutralPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;
    }

    void DestroyBuildings()
    {
        GameObject.Find("Data Controller").transform.Find("Buildings Controller").GetComponent<BuildingsController>().DestroyAllBuildings();
    }

    Vector3Int WorldToTilemap(Vector3 pos)
    {
        return new Vector3Int((int)pos.x, (int)(pos.y - 1), 0); // per algun motiu estrany que desconec el tilemap sempre comença a (0, -1) quan el top left està al (0, 0)
    }

    Vector3Int WorldToTilemap(Vector2Int pos)
    {
        return new Vector3Int(pos.x, (pos.y - 1), 0); // per algun motiu estrany que desconec el tilemap sempre comença a (0, -1) quan el top left està al (0, 0)
    }

    //pathfinding

    void InitializePathfinding()
    {
        pathfinding = new Pathfinding();
        auxiliarPathfinding = new Pathfinding();

        pathfinding.MyTilemap = new MyTile[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j > -height; j--)
            {
                Vector2Int pos = new Vector2Int(i, j);
                bool isWalkable = IsWalkable(pos);
                MyTileType type = DetermineMyTileType(pos);

                pathfinding.MyTilemap[i, -j] = new MyTile(pos, isWalkable, type, width - 1, height - 1);

                /*if(!isWalkable)
                    Debug.Log("MapController::InitializePathfinding - MyTile: " + pos + ", " + isWalkable + ", " + type);*/
            }
        }

        auxiliarPathfinding.MyTilemap = pathfinding.MyTilemap;
    }

    public bool IsWalkable(Vector2Int pos)
    {
        TileBase tile = tilemapWalkability.GetTile(WorldToTilemap(new Vector3(pos.x, pos.y, 0)));

        if (tile != null)
        {
            if (tile.name != tilesetDictionary[(int)TilesetCode.RED])
            {
                return true;
            }
        }

        return false;
    }

    public MyTile GetMyTile(Vector2 pos)
    {
        return pathfinding.MyTilemap[(int)pos.x, -(int)pos.y];
    }

    MyTileType DetermineMyTileType(Vector2Int pos)
    {
        // aquest mètode mira la posició pos a tots els tilemaps per determinar quin tipus de tile és. Retorna neutral si no coincideix amb cap tile especial

        TileBase tile = tilemapBase.GetTile(WorldToTilemap(new Vector3(pos.x, pos.y, 0)));

        if (tile != null)
        {
            if (IsRoad(tile))
            {
                return MyTileType.ROAD;
            }
            else if (IsSea(tile))
            {
                return MyTileType.SEA;
            }
            else if (IsCone(tile))
            {
                return MyTileType.CONE;
            }
        }

        tile = tilemapBuildings.GetTile(WorldToTilemap(new Vector3(pos.x, pos.y, 0)));

        if (tile != null)
        {
            if (IsBuilding(tile))
            {
                return MyTileType.BUILDING;
            }
        }

        tile = tilemapHazards.GetTile(WorldToTilemap(new Vector3(pos.x, pos.y, 0)));

        if (tile != null)
        {
            if (IsContainer(tile))
            {
                return MyTileType.CONTAINER;
            }
            else if (IsLamp(tile))
            {
                return MyTileType.LAMP;
            }
            else if (IsPlantpot(tile))
            {
                return MyTileType.PLANTPOT;
            }
        }

        return MyTileType.NEUTRAL;
    }

    bool IsNeutral(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.NEUTRAL])
            return true;
        return false;
    }

    bool IsRoad(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.ROAD] || tile.name == tilesetDictionary[(int)TilesetCode.CROSSWALK1] || tile.name == tilesetDictionary[(int)TilesetCode.CROSSWALK2])
            return true;
        return false;
    }

    bool IsSea(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.SEA])
            return true;
        return false;
    }

    bool IsCone(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.CONE1] || tile.name == tilesetDictionary[(int)TilesetCode.CONE2] || tile.name == tilesetDictionary[(int)TilesetCode.CONE3]
            || tile.name == tilesetDictionary[(int)TilesetCode.CONE4] || tile.name == tilesetDictionary[(int)TilesetCode.CONE5] || tile.name == tilesetDictionary[(int)TilesetCode.CONE6]
            || tile.name == tilesetDictionary[(int)TilesetCode.CONE7] || tile.name == tilesetDictionary[(int)TilesetCode.CONE8])
            return true;
        return false;
    }

    bool IsContainer(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.CONTAINER])
            return true;
        return false;
    }

    bool IsLamp(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.LAMP])
            return true;
        return false;
    }

    bool IsPlantpot(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.PLANTPOT])
            return true;
        return false;
    }

    bool IsBuilding(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.BASE_CANI] || tile.name == tilesetDictionary[(int)TilesetCode.BASE_HIPSTER]
            || tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_CANI] || tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_HIPSTER]
            || tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_NEUTRAL] || tile.name == tilesetDictionary[(int)TilesetCode.BUILDING_CANI]
            || tile.name == tilesetDictionary[(int)TilesetCode.BUILDING_HIPSTER] || tile.name == tilesetDictionary[(int)TilesetCode.BUILDING_NEUTRAL])
            return true;
        return false;
    }

    public void DrawPathfinding(bool shouldDraw, Pathfinder pathfinder)
    {
        Pathfinding current = new Pathfinding();

        switch (pathfinder)
        {
            case Pathfinder.MAIN:
                current = pathfinding;
                break;

            case Pathfinder.AUXILIAR:
                current = auxiliarPathfinding;
                break;
        }

        foreach(Vector2Int pos in current.visited)
        {
            if (shouldDraw)
                tilemapPathfinding.SetTile(WorldToTilemap(new Vector3Int(pos.x, pos.y, 0)), blueTile);
            else
                tilemapPathfinding.SetTile(WorldToTilemap(new Vector3Int(pos.x, pos.y, 0)), null);
        }
    }

    public void DrawAttackRange(bool shouldDraw)
    {
        //draw attackRange
        foreach (Vector2Int pos in pathfinding.attackRange)
        {
            if (shouldDraw)
                tilemapPathfinding.SetTile(WorldToTilemap(new Vector3Int(pos.x, pos.y, 0)), redTile);
            else
                tilemapPathfinding.SetTile(WorldToTilemap(new Vector3Int(pos.x, pos.y, 0)), null);
        }
    }

    public void DrawRangedAttackRange(bool shouldDraw)
    {
        //draw rangedAttackRange
        foreach (Vector2Int pos in pathfinding.rangedAttackRange)
        {
            if (shouldDraw)
                tilemapPathfinding.SetTile(WorldToTilemap(new Vector3Int(pos.x, pos.y, 0)), redTile);
            else
                tilemapPathfinding.SetTile(WorldToTilemap(new Vector3Int(pos.x, pos.y, 0)), null);
        }
    }

    Pathfinding GetPathfinder(Pathfinder pathfinder)
    {
        switch (pathfinder)
        {
            case Pathfinder.MAIN:
                return pathfinding;

            case Pathfinder.AUXILIAR:
                return auxiliarPathfinding;
        }

        return pathfinding;
    }

    public void ExecutePathfinding(Pathfinder pathfinder, GameObject unit)
    {
        Pathfinding current = GetPathfinder(pathfinder);

        current.ResetBFS(new Vector2Int((int)unit.transform.position.x, (int)unit.transform.position.y)); //resetegem el pathfinding a la posició de la unitat
        current.PropagateBFS(unit);

        if (!FindObjectOfType<AIController>().inControl)
            DrawPathfinding(true, pathfinder);
    }

    public void ExecutePathfinding(Pathfinder pathfinder, Vector2Int position, GameObject unit)
    {
        Pathfinding current = GetPathfinder(pathfinder);

        current.ResetBFS(new Vector2Int(position.x, position.y)); //resetegem el pathfinding a la posició manual
        current.PropagateBFS(unit);

        if (!FindObjectOfType<AIController>().inControl)
            DrawPathfinding(true, pathfinder);
    }

    public void ExecutePathfinding(Pathfinder pathfinder, Vector2Int position, GameObject unit, uint range)
    {
        Pathfinding current = GetPathfinder(pathfinder);

        current.ResetBFS(new Vector2Int(position.x, position.y)); //resetegem el pathfinding a la posició manual
        current.PropagateBFS(unit, range);

        if (!FindObjectOfType<AIController>().inControl)
            DrawPathfinding(true, pathfinder);
    }

    public void ExecuteRangedPathfinding(Pathfinder pathfinder, GameObject unit)
    {
        Pathfinding current = GetPathfinder(pathfinder);

        current.ResetRangedBFS(new Vector2Int((int)unit.transform.position.x, (int)unit.transform.position.y)); //resetegem el pathfinding a la posició de la unitat
        current.PropagateRangedBFS(unit);
    }

    public void ExecutePathfindingForAttackRange(Pathfinder pathfinder, GameObject unit)
    {
        Pathfinding current = GetPathfinder(pathfinder);

        current.ResetBFS(new Vector2Int((int)unit.transform.position.x, (int)unit.transform.position.y)); //resetegem el pathfinding a la posició de la unitat
        current.PropagateBFS(unit);

        if (!FindObjectOfType<AIController>().inControl)
            DrawAttackRange(true);
    }

    public void ExecuteRangedPathfindingForAttackRange(Pathfinder pathfinder, GameObject unit)
    {
        Pathfinding current = GetPathfinder(pathfinder);

        current.ResetRangedBFS(new Vector2Int((int)unit.transform.position.x, (int)unit.transform.position.y)); //resetegem el pathfinding a la posició de la unitat
        current.PropagateRangedBFS(unit);
        DrawRangedAttackRange(true);
    }

    public void ExecutePathfindingForAI(Pathfinder pathfinder, uint range, GameObject unit)
    {
        Pathfinding current = GetPathfinder(pathfinder);

        current.ResetAIBFS(new Vector2Int((int)unit.transform.position.x, (int)unit.transform.position.y));
        current.PropagateAIBFS(range, unit);

        Debug.Log("MapController::ExecutePathfindingForAI - Checked " + current.AIVisited.Count + " tiles");
    }

    public List<Vector2Int> GetTilesInCommon()
    {
        //retorna una llista de posicions que existeixen tant en el pathfinding MAIN com en l'AUXILIAR
        List<Vector2Int> ret = new List<Vector2Int>();

        foreach (Vector2Int tile in auxiliarPathfinding.visited)
        {
            if (pathfinding.visited.Contains(tile))
            {
                ret.Add(tile);
            }
        }

        return ret;
    }

    public void DrawArrow()
    {
        UndrawArrow();

        Vector3 player = GameObject.Find("Player").transform.position;
        Vector2Int playerPos = new Vector2Int((int)player.x, (int)player.y);
        BFS_Node playerNode = new BFS_Node(playerPos, playerPos); //tota aquesta parafernàlia és per tenir un BFS_Node amb la posició del player i facilitat la vida amb el GetPath()

        if (pathfinding.backtrack.Count > 0 && playerPos != pathfinding.visited[0])
        {
            List<BFS_Node> path = pathfinding.GetReversePath(playerNode, Applicant.HUMAN);
            Tile lastTile = null;

            //fletxa final
            if (path[0].parent.x < path[0].data.x)
                lastTile = arrowLeft;
            else if (path[0].parent.x > path[0].data.x)
                lastTile = arrowRight;
            else if (path[0].parent.y < path[0].data.y)
                lastTile = arrowDown;
            else if (path[0].parent.y > path[0].data.y)
                lastTile = arrowUp;

            tilemapPlayer.SetTile(WorldToTilemap(path[0].data), lastTile);

            if (pathfinding.backtrack.Count > 1)
            { 
                for (int i = 1; i < path.Count - 1; i++) // el -1 és per descartar l'origen i així no hi pinti flexta
                {
                    //mirar anterior i pare per saber tile

                    // l'anterior és a l'esquerra
                    if (path[i - 1].data.x < path[i].data.x)
                    {
                        // la següent és a la dreta
                        if (path[i].parent.x > path[i].data.x)
                        {
                            lastTile = arrowHorizontal;
                        }
                        // la següent és avall
                        else if (path[i].parent.y < path[i].data.y)
                        {
                            lastTile = arrowBottomLeft;
                        }
                        //la següent és amunt
                        else if (path[i].parent.y > path[i].data.y)
                        {
                            lastTile = arrowTopLeft;
                        }
                    }

                    // l'anterior és a la dreta
                    else if (path[i - 1].data.x > path[i].data.x)
                    {
                        // la següent és a l'esquerra
                        if (path[i].parent.x < path[i].data.x)
                        {
                            lastTile = arrowHorizontal;
                        }
                        // la següent és avall
                        else if (path[i].parent.y < path[i].data.y)
                        {
                            lastTile = arrowBottomRight;
                        }
                        //la següent és amunt
                        else if (path[i].parent.y > path[i].data.y)
                        {
                            lastTile = arrowTopRight;
                        }
                    }

                    // l'anterior és avall
                    else if (path[i - 1].data.y < path[i].data.y)
                    {
                        // la següent és a la dreta
                        if (path[i].parent.x > path[i].data.x)
                        {
                            lastTile = arrowBottomRight;
                        }
                        // la següent és a l'esquerra
                        else if (path[i].parent.x < path[i].data.x)
                        {
                            lastTile = arrowBottomLeft;
                        }
                        //la següent és amunt
                        else if (path[i].parent.y > path[i].data.y)
                        {
                            lastTile = arrowVertical;
                        }
                    }

                    // l'anterior és amunt
                    else if (path[i - 1].data.y > path[i].data.y)
                    {
                        // la següent és a la dreta
                        if (path[i].parent.x > path[i].data.x)
                        {
                            lastTile = arrowTopRight;
                        }
                        // la següent és a l'esquerra
                        else if (path[i].parent.x < path[i].data.x)
                        {
                            lastTile = arrowTopLeft;
                        }
                        //la següent és avall
                        else if (path[i].parent.y < path[i].data.y)
                        {
                            lastTile = arrowVertical;
                        }
                    }

                    tilemapPlayer.SetTile(WorldToTilemap(path[i].data), lastTile);
                }
            }
        }
    }

    public void UndrawArrow()
    {
        tilemapPlayer.ClearAllTiles();
    }

    public void LogTileInfo(Vector3 position)
    {
        MyTile tile = pathfinding.MyTilemap[(int)position.x, -(int)position.y];
        string tileType = "null";

        switch (tile.type)
        {
            case MyTileType.NEUTRAL:
                tileType = "Neutral";
                break;

            case MyTileType.ROAD:
                tileType = "Road";
                break;

            case MyTileType.SEA:
                tileType = "Sea";
                break;

            case MyTileType.CONE:
                tileType = "Cone";
                break;

            case MyTileType.PLANTPOT:
                tileType = "Plantpot";
                break;

            case MyTileType.LAMP:
                tileType = "Lamp";
                break;

            case MyTileType.BUILDING:
                tileType = "Building";
                break;
        }
        Debug.Log("MapController::LogTileInfo - " + position +
                  ", Tile info: " + tileType + ", Is Walkable = " + tile.isWalkable +
                  ", Contains Cani: " + tile.containsCani + ", Contains Hipster: " + tile.containsHipster);
    }

    //events

    void SubscribeToEvents()
    {
        
    }
}
