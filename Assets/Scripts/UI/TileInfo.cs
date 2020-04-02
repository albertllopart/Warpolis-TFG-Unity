﻿using System.Collections;
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
    public Sprite container;
    public Sprite lamp;
    public Sprite cani_base;
    public Sprite hipster_base;
    public Sprite neutral_factory;
    public Sprite cani_factory;
    public Sprite hipster_factory;

    //units
    [Header("Units")]
    public Sprite cani_infantry;
    public Sprite hipster_infantry;

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

            case MyTileType.CONTAINER:
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = container;
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
        }
        else
        {
            transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().x - 2.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y + 2.5f, 0);

            unit.transform.position = transform.position + new Vector3(-2, 0, 0);
        }
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
        }        
        else if (hipsterResult.collider != null)
        {
            unit.SetActive(true);
            UpdateUnitInfo(hipsterResult.collider.gameObject);
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
            case 0: //infantry
                return cani_infantry;
        }

        return null;
    }

    Sprite DetermineHipsterUnitSprite(GameObject unitGO)
    {
        switch (unitGO.GetComponent<Unit>().unitType)
        {
            case 0: //infantry
                return hipster_infantry;
        }

        return null;
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