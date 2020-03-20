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
    public Sprite container;
    public Sprite building;
    public Sprite lamp;

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

        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateInfo()
    {
        UpdatePosition();

        //mirar la posició del player per determinar quin myTile agafar
        Vector2Int pos = new Vector2Int((int)GameObject.Find("Player").transform.position.x, (int)GameObject.Find("Player").transform.position.y);
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
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = building;
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[3];
                break;

            case MyTileType.LAMP:
                map.transform.Find("Tile").GetComponent<SpriteRenderer>().sprite = lamp;
                map.transform.Find("Defense").transform.Find("Number").GetComponent<SpriteRenderer>().sprite = numberSprites[4];
                break;
        }

        //informació que no sempre estarà activada
        if (tile.type == MyTileType.BUILDING)
        {
            map.transform.Find("Capture").gameObject.SetActive(true);
        }
        else
        {
            map.transform.Find("Capture").gameObject.SetActive(false);
        }
    }

    public void UpdatePosition()
    {
        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().playerLocation == GameplayController.PlayerLocation.RIGHT)
        {
            transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetTopLeftCorner().x + 1.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y + 2.5f, 0);
        }
        else
        {
            transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().x - 2.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y + 2.5f, 0);
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
