using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfo : MonoBehaviour
{
    //position
    Vector3 leftPosition;
    Vector3 rightPosition;

    //numbers
    [Header("Numbers")]
    public Sprite zero;
    public Sprite one;
    public Sprite two;
    public Sprite three;
    public Sprite four;
    public Sprite five;
    public Sprite six;
    public Sprite seven;
    public Sprite eight;
    public Sprite nine;

    //tiles
    [Header("Tiles")]
    public Sprite road;
    public Sprite neutral;
    public Sprite plantpot;
    public Sprite lamp;
    public Sprite sea;
    public Sprite cone;
    public Sprite cani_base;
    public Sprite hipster_base;
    public Sprite neutral_factory;
    public Sprite cani_factory;
    public Sprite hipster_factory;
    public Sprite neutral_building;
    public Sprite cani_building;
    public Sprite hipster_building;

    //units
    [Header("Units")]
    public Sprite cani_infantry;
    public Sprite hipster_infantry;
    public Sprite cani_transport;
    public Sprite hipster_transport;
    public Sprite cani_tank;
    public Sprite hipster_tank;
    public Sprite cani_aerial;
    public Sprite hipster_aerial;
    public Sprite cani_gunner;
    public Sprite hipster_gunner;
    public Sprite cani_ranged;
    public Sprite hipster_ranged;

    //elements
    [Header("Elements")]
    public GameObject map;
    public GameObject unit;

    [HideInInspector]
    public List<Sprite> numberSprites;

    // Start is called before the first frame update
    void Start()
    {
        SetUpNumberSprites();

        map = transform.Find("Map").gameObject;
        unit = transform.Find("Unit").gameObject;

        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateInfo(Vector3 position)
    {
        UpdatePosition();
        //mirar la posició del player per determinar quin myTile agafar
        Vector2Int pos = new Vector2Int((int)position.x, (int)position.y);
        MyTile tile = GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.MyTilemap[pos.x, -pos.y];

        switch (tile.type)
        {
            case MyTileType.ROAD:
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = road;
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[0];
                break;

            case MyTileType.NEUTRAL:
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = neutral;
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[1];
                break;

            case MyTileType.CONE:
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = cone;
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[0];
                break;

            case MyTileType.SEA:
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = sea;
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[0];
                break;

            case MyTileType.PLANTPOT:
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = plantpot;
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[2];
                break;

            case MyTileType.BUILDING:
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[3];
                CheckBuilding(position);
                break;

            case MyTileType.LAMP:
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = lamp;
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[4];
                break;
        }

        //informació que no sempre estarà activada
        if (tile.type != MyTileType.BUILDING)
        {
            map.transform.Find("Capture").gameObject.SetActive(false);
        }

        //unit
        if (tile.containsCani || tile.containsHipster)
            CheckUnit(position);
        else
            unit.SetActive(false);
    }

    public void UpdatePosition()
    {
        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().playerLocation == GameplayController.PlayerLocation.RIGHT)
        {
            transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetTopLeftCorner().x + 1.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y + 2.5f, 0);

            unit.transform.position = transform.position + new Vector3(2, 0, 0);
            unit.transform.Find("Load").transform.position = unit.transform.position + new Vector3(2, -0.75f, 0);
        }
        else
        {
            transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().x - 2.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y + 2.5f, 0);

            unit.transform.position = transform.position + new Vector3(-2, 0, 0);
            unit.transform.Find("Load").transform.position = unit.transform.position + new Vector3(-1.5f, -0.75f, 0);
        }
    }

    public void UpdatePosition(Vector3 position)
    {
        GameplayController.PlayerLocation location = DeterminePositionLocation(position);

        switch (location)
        {
            case GameplayController.PlayerLocation.LEFT:

                transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().x - 2.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y + 2.5f, 0);

                unit.transform.position = transform.position + new Vector3(-2, 0, 0);
                unit.transform.Find("Load").transform.position = unit.transform.position + new Vector3(-1.5f, -0.75f, 0);

                break;

            case GameplayController.PlayerLocation.RIGHT:

                transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetTopLeftCorner().x + 1.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y + 2.5f, 0);

                unit.transform.position = transform.position + new Vector3(2, 0, 0);
                unit.transform.Find("Load").transform.position = unit.transform.position + new Vector3(2, -0.75f, 0);

                break;
        }
    }

    public GameplayController.PlayerLocation DeterminePositionLocation(Vector3 position)
    {
        uint cameraMiddle = (uint)Camera.main.gameObject.GetComponent<CameraController>().GetTopLeftCorner().x +
                                  Camera.main.gameObject.GetComponent<CameraController>().GetCameraWidth() / 2;

        if (position.x < cameraMiddle)
            return GameplayController.PlayerLocation.LEFT;
        else
            return GameplayController.PlayerLocation.RIGHT;
    }

    public void CheckBuilding(Vector3 pos)
    {
        RaycastHit2D result = RayCast(pos, LayerMask.GetMask("Cani_buildings"));

        if (result.collider != null)
        {
            UpdateBuildingInfo(result.collider.gameObject);
        }

        result = RayCast(pos, LayerMask.GetMask("Hipster_buildings"));

        if (result.collider != null)
        {
            UpdateBuildingInfo(result.collider.gameObject);
        }

        result = RayCast(pos, LayerMask.GetMask("Neutral_buildings"));

        if (result.collider != null)
        {
            UpdateBuildingInfo(result.collider.gameObject);
        }
    }

    RaycastHit2D RayCast(Vector3 pos, int layer)
    {
        Vector2 from = pos + new Vector3(0.5f, -0.5f, 0);
        Vector2 to = from;

        return Physics2D.Linecast(from, to, layer);
    }

    void UpdateBuildingInfo(GameObject building)
    {
        int currentHP = building.GetComponent<Building>().currentHP;
        int maxHP = building.GetComponent<Building>().maxHP;

        if (currentHP != maxHP)
        {
            map.transform.Find("Capture").gameObject.SetActive(true);
            map.transform.Find("Capture").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[currentHP];
        }
        else
        {
            map.transform.Find("Capture").gameObject.SetActive(false);
        }

        SetBuildingSprite(building);
    }

    void SetBuildingSprite(GameObject building)
    {
        switch (building.tag)
        {
            case "Base_cani":
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = cani_base;
                break;

            case "Base_hipster":
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = hipster_base;
                break;

            case "Factory_neutral":
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = neutral_factory;
                break;

            case "Factory_cani":
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = cani_factory;
                break;

            case "Factory_hipster":
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = hipster_factory;
                break;

            case "Building_neutral":
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = neutral_building;
                break;

            case "Building_cani":
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = cani_building;
                break;

            case "Building_hipster":
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = hipster_building;
                break;
        }
    }

    void CheckUnit(Vector3 pos)
    {
        RaycastHit2D caniResult = RayCast(pos, LayerMask.GetMask("Cani_units"));
        RaycastHit2D hipsterResult = RayCast(pos, LayerMask.GetMask("Hipster_units"));

        if (caniResult.collider != null)
        {
            unit.SetActive(true);
            UpdateUnitInfo(caniResult.collider.gameObject);
            UpdateUnitLoadCani(caniResult.collider.gameObject);
        }        
        else if (hipsterResult.collider != null)
        {
            unit.SetActive(true);
            UpdateUnitInfo(hipsterResult.collider.gameObject);
            UpdateUnitLoadHipster(hipsterResult.collider.gameObject);
        }
    }

    void UpdateUnitInfo(GameObject unitGO)
    {
        unit.transform.Find("Hitpoints").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[
            (int)unitGO.GetComponent<Unit>().CalculateUIHitpoints()]; //hitpoints

        SetUnitSprite(unitGO);
    }

    void SetUnitSprite(GameObject unitGO)
    {
        switch (unitGO.GetComponent<Unit>().army)
        {
            case UnitArmy.CANI:
                unit.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = DetermineCaniUnitSprite(unitGO);
                break;

            case UnitArmy.HIPSTER:
                unit.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = DetermineHipsterUnitSprite(unitGO);
                break;
        }
    }

    Sprite DetermineCaniUnitSprite(GameObject unitGO)
    {
        switch (unitGO.GetComponent<Unit>().unitType)
        {
            case UnitType.INFANTRY:
                return cani_infantry;

            case UnitType.TRANSPORT:
                return cani_transport;

            case UnitType.TANK:
                return cani_tank;

            case UnitType.AERIAL:
                return cani_aerial;

            case UnitType.GUNNER:
                return cani_gunner;

            case UnitType.RANGED:
                return cani_ranged;
        }

        return null;
    }

    Sprite DetermineHipsterUnitSprite(GameObject unitGO)
    {
        switch (unitGO.GetComponent<Unit>().unitType)
        {
            case UnitType.INFANTRY:
                return hipster_infantry;

            case UnitType.TRANSPORT:
                return hipster_transport;

            case UnitType.TANK:
                return hipster_tank;

            case UnitType.AERIAL:
                return hipster_aerial;

            case UnitType.GUNNER:
                return hipster_gunner;

            case UnitType.RANGED:
                return hipster_ranged;
        }

        return null;
    }

    void UpdateUnitLoadCani(GameObject transport)
    {
        if (transport.GetComponent<Unit>().unitType == UnitType.TRANSPORT && transport.GetComponent<UnitTransport>().loadedUnit != null)
        {
            unit.transform.Find("Load").gameObject.SetActive(true);
            unit.transform.Find("Load").transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = cani_infantry;
        }
        else
        {
            unit.transform.Find("Load").gameObject.SetActive(false);
        }
    }

    void UpdateUnitLoadHipster(GameObject transport)
    {
        if (transport.GetComponent<Unit>().unitType == UnitType.TRANSPORT && transport.GetComponent<UnitTransport>().loadedUnit != null)
        {
            unit.transform.Find("Load").gameObject.SetActive(true);
            unit.transform.Find("Load").transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = hipster_infantry;
        }
        else
        {
            unit.transform.Find("Load").gameObject.SetActive(false);
        }
    }

    void SetUpNumberSprites()
    {
        numberSprites = new List<Sprite>();

        numberSprites.Add(zero);
        numberSprites.Add(one);
        numberSprites.Add(two);
        numberSprites.Add(three);
        numberSprites.Add(four);
        numberSprites.Add(five);
        numberSprites.Add(six);
        numberSprites.Add(seven);
        numberSprites.Add(eight);
        numberSprites.Add(nine);
    }

    void SubscribeToEvents()
    {
        
    }
}
