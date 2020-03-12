using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    // Start is called before the first frame update
    void Start()
    { 
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
        Camera.main.GetComponent<Transform>().position += offset;
        //Camera.main.GetComponent<CameraController>().SetCameraMoved(true); <-------- No entenc per què l'Start de la càmera es fa després que el de mapa, aquesta línia és per si en algun moment deixa de passar

        topLeftCorner = transform.position;
        bottomRightCorner = (Vector2)transform.position + new Vector2(width - 1, -height + 1);

        Debug.Log("MapController::HelloWorld - TopLeft = " + topLeftCorner + " , BottomRight = " + bottomRightCorner);
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

    Vector3Int WorldToTilemap(Vector3 pos)
    {
        return new Vector3Int((int)pos.x, (int)(pos.y - 1), 0); // per algun motiu estrany que desconec el tilemap sempre comença a (0, -1) quan el top left està al (0, 0)
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

                pathfinding.MyTilemap[i, -j] = new MyTile(pos, isWalkable, type);

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
}
