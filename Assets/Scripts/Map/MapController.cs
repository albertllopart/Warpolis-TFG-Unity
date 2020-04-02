using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class MapController : MonoBehaviour
{
    public enum TilesetCode
    {
        RED, BLUE, YELLOW, BASE_CANI, BASE_HIPSTER, FACTORY_CANI, FACTORY_HIPSTER, FACTORY_NEUTRAL, NEUTRAL, ROAD, CROSSWALK1, CROSSWALK2, CONTAINER, LAMP
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

    private uint width;
    private uint height;
    private uint xOffset;
    private uint yOffset;

    private Vector2 topLeftCorner;
    private Vector2 bottomRightCorner;

    //Pathfinding

    public Pathfinding pathfinding;

    [Header("Pathfinding")]
    public Tile blueTile;
    public Tile redTile;
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

    //important primera vegada que es crida l'unload hi haurà el gym carregat, les properes vegades s'ha de mirar que el gym no hi sigui
    bool isLoaded = true;

    // Start is called before the first frame update
    void Start()
    {
        mapUnloaded = new UnityEvent();

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

        //buildings
        tilesetDictionary.Add((int)TilesetCode.BASE_CANI, "tileset_12");
        tilesetDictionary.Add((int)TilesetCode.BASE_HIPSTER, "tileset_13");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_CANI, "tileset_16");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_HIPSTER, "tileset_17");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_NEUTRAL, "tileset_18");

        //hazards
        tilesetDictionary.Add((int)TilesetCode.CONTAINER, "tileset_5");
        tilesetDictionary.Add((int)TilesetCode.LAMP, "tileset_10");

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
    }

    void SpawnBaseCani(Vector3 pos)
    {
        GameObject newGO = Instantiate(baseCaniPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;
    }

    void SpawnBaseHipster(Vector3 pos)
    {
        GameObject newGO = Instantiate(baseHipsterPrefab, pos, Quaternion.identity);
        newGO.transform.parent = tilemapBuildings.transform;
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
    }

    bool IsWalkable(Vector2Int pos)
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

    bool IsBuilding(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)TilesetCode.BASE_CANI] || tile.name == tilesetDictionary[(int)TilesetCode.BASE_HIPSTER]
            || tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_CANI] || tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_HIPSTER]
            || tile.name == tilesetDictionary[(int)TilesetCode.FACTORY_NEUTRAL])
            return true;
        return false;
    }

    public void DrawPathfinding(bool shouldDraw)
    {
        //draw visited
        foreach(Vector2Int pos in pathfinding.visited)
        {
            if (shouldDraw)
                tilemapPathfinding.SetTile(WorldToTilemap(new Vector3Int(pos.x, pos.y, 0)), blueTile);
            else
                tilemapPathfinding.SetTile(WorldToTilemap(new Vector3Int(pos.x, pos.y, 0)), null);
        }
    }

    public void ExecutePathfinding(GameObject unit)
    {
        pathfinding.ResetBFS(new Vector2Int((int)unit.transform.position.x, (int)unit.transform.position.y)); //resetegem el pathfinding a la posició de la unitat
        pathfinding.PropagateBFS(unit);
        DrawPathfinding(true);
    }

    public void DrawArrow()
    {
        UndrawArrow();

        Vector3 player = GameObject.Find("Player").transform.position;
        Vector2Int playerPos = new Vector2Int((int)player.x, (int)player.y);
        BFS_Node playerNode = new BFS_Node(playerPos, playerPos); //tota aquesta parafernàlia és per tenir un BFS_Node amb la posició del player i facilitat la vida amb el GetPath()

        if (pathfinding.backtrack.Count > 0 && playerPos != pathfinding.visited[0])
        {
            List<BFS_Node> path = pathfinding.GetReversePath(playerNode);
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

    //events

    void SubscribeToEvents()
    {
        
    }
}
