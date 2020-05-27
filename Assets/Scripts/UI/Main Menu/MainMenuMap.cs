using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainMenuMap : MonoBehaviour
{
    public enum MapMode
    {
        VERSUS, BATTLE
    };

    MapMode mapMode;

    [Header("Versus Maps")]
    public GameObject alpha_island;
    public GameObject span_island;
    public GameObject turd_island;
    public GameObject map4;
    public GameObject map5;
    public GameObject map6;
    public GameObject map7;
    public GameObject map8;
    public GameObject map9;

    [Header("Minimap")]
    public GameObject minimapPrefab;

    List<GameObject> mapList;
    List<GameObject> minimapList;

    GameObject recyclerView;
    GameObject mapInfo;

    //minimap builder
    public Tile neutral;
    public Tile lamp;
    public Tile plantPot;
    public Tile cone;
    public Tile neutralBuilding;
    public Tile caniBuilding;
    public Tile hipsterBuilding;
    public Tile sea;

    //from map controller
    Dictionary<int, string> tilesetDictionary;

    // Start is called before the first frame update
    void Start()
    {
        BuildDictionary();

        mapList = new List<GameObject>();
        minimapList = new List<GameObject>();
    }

    public void AfterStart(MapMode mode)
    {
        BuildMapList(mode);

        recyclerView = transform.Find("RecyclerView").gameObject;
        mapInfo = transform.Find("MapInfo").gameObject;

        Debug.Log(mapInfo.name);

        foreach (GameObject map in mapList)
        {
            GameObject button = recyclerView.GetComponent<RecyclerView>().InstantiateButton(map.name);

            //generar minimapa
            GameObject newMinimap = GenerateMinimap(map, button.transform);
            minimapList.Add(newMinimap);
            SetMinimapPosition(newMinimap);
        }

        UpdateMapInfo(0);

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildMapList(MapMode mode)
    {
        mapMode = mode;

        switch (mode)
        {
            case MapMode.VERSUS:
                mapList.Add(alpha_island);
                mapList.Add(span_island);
                mapList.Add(turd_island);
                //mapList.Add(map4);
                //mapList.Add(map5);
                //mapList.Add(map6);
                //mapList.Add(map7);
                //mapList.Add(map8);
                //mapList.Add(map9);
                break;

            case MapMode.BATTLE:
                mapList.Add(alpha_island);
                mapList.Add(span_island);
                mapList.Add(turd_island);
                mapList.Add(map4);
                mapList.Add(map5);
                mapList.Add(map6);
                mapList.Add(map7);
                mapList.Add(map8);
                break;
        }
    }

    void BuildDictionary()
    {
        tilesetDictionary = new Dictionary<int, string>();

        //base
        tilesetDictionary.Add((int)MapController.TilesetCode.NEUTRAL, "tileset_0");
        tilesetDictionary.Add((int)MapController.TilesetCode.ROAD, "tileset_1");
        tilesetDictionary.Add((int)MapController.TilesetCode.CROSSWALK1, "tileset_2");
        tilesetDictionary.Add((int)MapController.TilesetCode.CROSSWALK2, "tileset_3");
        tilesetDictionary.Add((int)MapController.TilesetCode.SEA, "animated_water");
        tilesetDictionary.Add((int)MapController.TilesetCode.CONE1, "tileset_35");
        tilesetDictionary.Add((int)MapController.TilesetCode.CONE2, "tileset_36");
        tilesetDictionary.Add((int)MapController.TilesetCode.CONE3, "tileset_37");
        tilesetDictionary.Add((int)MapController.TilesetCode.CONE4, "tileset_38");
        tilesetDictionary.Add((int)MapController.TilesetCode.CONE5, "tileset_39");
        tilesetDictionary.Add((int)MapController.TilesetCode.CONE6, "tileset_40");
        tilesetDictionary.Add((int)MapController.TilesetCode.CONE7, "tileset_41");
        tilesetDictionary.Add((int)MapController.TilesetCode.CONE8, "tileset_42");

        //buildings
        tilesetDictionary.Add((int)MapController.TilesetCode.BASE_CANI, "tileset_12");
        tilesetDictionary.Add((int)MapController.TilesetCode.BASE_HIPSTER, "tileset_13");
        tilesetDictionary.Add((int)MapController.TilesetCode.FACTORY_CANI, "tileset_16");
        tilesetDictionary.Add((int)MapController.TilesetCode.FACTORY_HIPSTER, "tileset_17");
        tilesetDictionary.Add((int)MapController.TilesetCode.FACTORY_NEUTRAL, "tileset_18");
        tilesetDictionary.Add((int)MapController.TilesetCode.BUILDING_CANI, "tileset_43");
        tilesetDictionary.Add((int)MapController.TilesetCode.BUILDING_HIPSTER, "tileset_44");
        tilesetDictionary.Add((int)MapController.TilesetCode.BUILDING_NEUTRAL, "tileset_45");

        //hazards
        tilesetDictionary.Add((int)MapController.TilesetCode.LAMP, "tileset_10");
        tilesetDictionary.Add((int)MapController.TilesetCode.PLANTPOT, "tileset_33");

        //walkability
        tilesetDictionary.Add((int)MapController.TilesetCode.RED, "tileset_14");
        tilesetDictionary.Add((int)MapController.TilesetCode.BLUE, "tileset_15");
        tilesetDictionary.Add((int)MapController.TilesetCode.YELLOW, "tileset_19");
    }

    public void MyOnEnable()
    {
        FindObjectOfType<FadeTo>().finishedDecreasing.AddListener(SubscribeToEvents);
        recyclerView.GetComponent<RecyclerView>().MyOnEnable();
    }

    public void MyOnDisable()
    {
        gameObject.SetActive(false);
    }

    public void TransitionToMode()
    {
        GetComponentInParent<MainMenuController>().UnsubscribeFromEvents();
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(SwitchToMode);
    }

    void SwitchToMode()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(SwitchToMode);
        GetComponentInParent<MainMenuController>().EnableMode();
        FindObjectOfType<FadeTo>().FadeFromSetup();
        MyOnDisable();
    }

    public void TransitionToNextScene()
    {
        GetComponentInParent<MainMenuController>().UnsubscribeFromEvents();
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(SwitchToNextScene);
    }

    public void SwitchToNextScene()
    {
        FindObjectOfType<DataTransferer>().TransferMap(mapList[recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex()], minimapList[recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex()]);
        minimapList[recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex()].transform.SetParent(FindObjectOfType<DataTransferer>().transform);

        switch (mapMode)
        {
            case MapMode.VERSUS:
                FindObjectOfType<DataTransferer>().TransferCommanders(DataController.PlayerCommander.HUMAN, DataController.PlayerCommander.HUMAN);
                break;

            case MapMode.BATTLE:
                FindObjectOfType<DataTransferer>().TransferCommanders(DataController.PlayerCommander.HUMAN, DataController.PlayerCommander.COMPUTER);
                break;
        }

        FindObjectOfType<SoundController>().StopTitle();
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(SwitchToNextScene);
        Loader.Load(Loader.Scene.game);
    }

    public void MoveUp()
    {
        if (recyclerView.GetComponent<RecyclerView>().MoveUp())
        {
            int index = recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex();
            minimapList[index + 1].SetActive(false);

            UpdateMapInfo(index);
        }
    }

    public void MoveDown()
    {
        if (recyclerView.GetComponent<RecyclerView>().MoveDown())
        {
            int index = recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex();
            minimapList[index - 1].SetActive(false);

            UpdateMapInfo(index);
        }
    }

    void UpdateMapInfo(int index)
    {
        minimapList[index].SetActive(true);

        mapInfo.transform.Find("Size").transform.Find("Width").GetComponent<Number>().CreateNumber(minimapList[index].transform.Find("Tilemap").GetComponent<Tilemap>().size.x);
        mapInfo.transform.Find("Size").transform.Find("Height").GetComponent<Number>().CreateNumber(minimapList[index].transform.Find("Tilemap").GetComponent<Tilemap>().size.y);

        Vector2Int buildings = CountBuildings(index);

        mapInfo.transform.Find("Buildings").transform.Find("Building").transform.Find("Number").GetComponent<Number>().CreateNumber(buildings.x);
        mapInfo.transform.Find("Buildings").transform.Find("Factory").transform.Find("Number").GetComponent<Number>().CreateNumber(buildings.y);
    }

    Vector2Int CountBuildings(int index)
    {
        //aquest mètode itera el mapa corresponent i retorna un vector2Int on les X són el nombre d'edificis i les Y el nombre de fàbriques
        Vector2Int ret = new Vector2Int(0, 0);

        Tilemap tilemap = mapList[index].transform.Find("Tilemap_buildings").GetComponent<Tilemap>();

        for (int i = 0; i < tilemap.size.x; i++)
        {
            for (int j = 0; j > -tilemap.size.y; j--)
            {
                TileBase tile = tilemap.GetTile(WorldToTilemap(new Vector3(i, j, 0)));

                if (tile != null)
                {
                    if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.BUILDING_NEUTRAL] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.BUILDING_CANI]
                    || tile.name == tilesetDictionary[(int)MapController.TilesetCode.BUILDING_HIPSTER])
                        ret.x++;

                    else if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.FACTORY_NEUTRAL] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.FACTORY_CANI]
                        || tile.name == tilesetDictionary[(int)MapController.TilesetCode.FACTORY_HIPSTER])
                        ret.y++;
                }
            }
        }

        return ret;
    }

    GameObject GenerateMinimap(GameObject map, Transform parent)
    {
        GameObject newMinimap = Instantiate(minimapPrefab);
        newMinimap.transform.SetParent(parent);
        GameObject newTilemap = newMinimap.transform.Find("Tilemap").gameObject;

        Tilemap tilemapBase = map.transform.Find("Tilemap_base").GetComponent<Tilemap>();
        Tilemap tilemapHazards = map.transform.Find("Tilemap_hazards").GetComponent<Tilemap>();
        Tilemap tilemapBuildings = map.transform.Find("Tilemap_buildings").GetComponent<Tilemap>();

        for (int i = 0; i < tilemapBase.size.x; i++)
        {
            for (int j = 0; j > -tilemapBase.size.y; j--)
            {
                TileBase tile = tilemapBase.GetTile(WorldToTilemap(new Vector3(i, j, 0)));

                if (tile != null)
                {
                    if (tile.name != tilesetDictionary[(int)MapController.TilesetCode.RED])
                    {
                        newTilemap.GetComponent<Tilemap>().SetTile(WorldToTilemap(new Vector3Int(i, j, 0)), DetermineTileType(tile));
                    }
                }

                tile = tilemapHazards.GetTile(WorldToTilemap(new Vector3(i, j, 0)));

                if (tile != null)
                {
                    if (tile.name != tilesetDictionary[(int)MapController.TilesetCode.RED])
                    {
                        newTilemap.GetComponent<Tilemap>().SetTile(WorldToTilemap(new Vector3Int(i, j, 0)), DetermineTileType(tile));
                    }
                }

                tile = tilemapBuildings.GetTile(WorldToTilemap(new Vector3(i, j, 0)));

                if (tile != null)
                {
                    if (tile.name != tilesetDictionary[(int)MapController.TilesetCode.RED])
                    {
                        newTilemap.GetComponent<Tilemap>().SetTile(WorldToTilemap(new Vector3Int(i, j, 0)), DetermineTileType(tile));
                    }
                }
            }
        }

        return newMinimap;
    }

    Tile DetermineTileType(TileBase tile)
    {
        if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.NEUTRAL] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.ROAD]
            || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CROSSWALK1] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CROSSWALK2])
            return neutral;
        else if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.LAMP])
            return lamp;
        else if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.PLANTPOT])
            return plantPot;
        else if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.CONE1] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CONE2]
                || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CONE3] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CONE4]
                || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CONE5] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CONE6]
                || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CONE7] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.CONE8])
            return cone;
        else if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.BUILDING_NEUTRAL] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.FACTORY_NEUTRAL])
            return neutralBuilding;
        else if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.BUILDING_CANI] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.FACTORY_CANI]
                || tile.name == tilesetDictionary[(int)MapController.TilesetCode.BASE_CANI])
            return caniBuilding;
        else if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.BUILDING_HIPSTER] || tile.name == tilesetDictionary[(int)MapController.TilesetCode.FACTORY_HIPSTER]
                || tile.name == tilesetDictionary[(int)MapController.TilesetCode.BASE_HIPSTER])
            return hipsterBuilding;
        else if (tile.name == tilesetDictionary[(int)MapController.TilesetCode.SEA])
            return sea;

        return null;
    }

    Vector3Int WorldToTilemap(Vector3 pos)
    {
        return new Vector3Int((int)pos.x, (int)(pos.y - 1), 0); // per algun motiu estrany que desconec el tilemap sempre comença a (0, -1) quan el top left està al (0, 0)
    }

    Vector3Int WorldToTilemap(Vector2Int pos)
    {
        return new Vector3Int(pos.x, (pos.y - 1), 0); // per algun motiu estrany que desconec el tilemap sempre comença a (0, -1) quan el top left està al (0, 0)
    }

    void EnableMinimapInfo()
    {
        int index = recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex();
        minimapList[index].SetActive(true);
    }

    void DisableMinimapInfo()
    {
        int index = recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex();
        minimapList[index].SetActive(false);
    }

    void SetMinimapPosition(GameObject minimap)
    { 
        minimap.transform.SetParent(mapInfo.transform.Find("Layout").transform.Find("Minimap").transform);
        Tilemap tilemap = minimap.transform.Find("Tilemap").GetComponent<Tilemap>();
        tilemap.CompressBounds();
        minimap.transform.position = minimap.transform.parent.position - new Vector3(3 / 16f, 0, 0);
        minimap.transform.position += new Vector3(-(tilemap.size.x / 2f) * (3 / 16f), (tilemap.size.y / 2f) * (3 / 16f), 0);

        AllocateMinimapCorners(minimap, tilemap);

        minimap.SetActive(false);
    }

    void AllocateMinimapCorners(GameObject minimap, Tilemap tilemap)
    {
        minimap.transform.Find("Corners").transform.Find("TopRight").transform.position += new Vector3(tilemap.size.x * 3 / 16f, 0, 0);
        minimap.transform.Find("Corners").transform.Find("BottomLeft").transform.position += new Vector3(0, -tilemap.size.y * 3 / 16f, 0);
        minimap.transform.Find("Corners").transform.Find("BottomRight").transform.position += new Vector3(tilemap.size.x * 3 / 16f, -tilemap.size.y * 3 / 16f, 0);
    }

    void SubscribeToEvents()
    {
        //el primer que hem de fer és treure el listener del fadeTo
        FindObjectOfType<FadeTo>().finishedDecreasing.RemoveListener(SubscribeToEvents);

        FindObjectOfType<Controls>().keyboard_w_down.AddListener(MoveUp);
        FindObjectOfType<Controls>().keyboard_s_down.AddListener(MoveDown);
    }

    void UnsubscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_w_down.RemoveListener(MoveUp);
        FindObjectOfType<Controls>().keyboard_s_down.RemoveListener(MoveDown);
    }
}
