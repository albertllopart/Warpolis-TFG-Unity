using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    public enum TilesetCode
    {
        RED, BLUE, YELLOW, BASE_CANI, BASE_HIPSTER, FACTORY_CANI, FACTORY_HIPSTER, FACTORY_NEUTRAL
    };

    public Dictionary<int, string> tilesetDictionary;

    [Header("Tilemaps")]
    public Tilemap tilemapBase;
    public Tilemap tilemapBuildings;
    public Tilemap tilemapHazards;
    public Tilemap tilemapWalkability;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildDictionary()
    {
        tilesetDictionary = new Dictionary<int, string>();

        tilesetDictionary.Add((int)TilesetCode.RED, "tileset_14");
        tilesetDictionary.Add((int)TilesetCode.BLUE, "tileset_15");
        tilesetDictionary.Add((int)TilesetCode.YELLOW, "tileset_19");
        tilesetDictionary.Add((int)TilesetCode.BASE_CANI, "tileset_12");
        tilesetDictionary.Add((int)TilesetCode.BASE_HIPSTER, "tileset_13");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_CANI, "tileset_16");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_HIPSTER, "tileset_17");
        tilesetDictionary.Add((int)TilesetCode.FACTORY_NEUTRAL, "tileset_18");
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
}
