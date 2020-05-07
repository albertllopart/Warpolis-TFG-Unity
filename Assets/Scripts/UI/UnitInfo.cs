using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite caniInfantry;
    public Sprite caniTransport;
    public Sprite caniTank;
    public Sprite caniAerial;
    public Sprite caniGunner;
    public Sprite caniRanged;
    public Sprite hipsterInfantry;
    public Sprite hipsterTransport;
    public Sprite hipsterTank;
    public Sprite hipsterAerial;
    public Sprite hipsterGunner;
    public Sprite hipsterRanged;

    [Header("Prefabs")]
    public GameObject spritePrefab;

    UnitType unitType;
    UnitData unitData;

    //important
    [Header("Structure")]
    public GameObject movementNumber;
    public GameObject attackRangeFrom;
    public GameObject attackRangeTo;
    public GameObject strongSprites;
    public GameObject weakSprites;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildInfo(UnitType unitType)
    {
        this.unitType = unitType;

        GetUnitData(); //obtenim les dades del json

        SetStats();
        SetStrong();
        SetWeak();
    }

    void SetStats()
    {
        Debug.Log("UnitInfo::SetStats - Setting Stats for Unit: " + unitData.name);

        int movement = unitData.movementRange;
        int from = unitData.attackRangeFrom;
        int to = unitData.attackRangeTo;

        Debug.Log("UnitInfo::SetStats - " + movement + ", " + from + ", " + to);

        movementNumber.GetComponent<Number>().SetNumber(movement);
        attackRangeFrom.GetComponent<Number>().SetNumber(from);
        attackRangeTo.GetComponent<Number>().SetNumber(to);
    }

    void SetStrong()
    {
        List<Sprite> sprites = new List<Sprite>();
        BuildSpriteList(sprites);

        List<Sprite> strongList = new List<Sprite>();

        if (sprites.Count == 6)
        {
            if (unitData.vsInfantry >= 0.6f)
                strongList.Add(sprites[0]);

            if (unitData.vsTransport >= 0.6f)
                strongList.Add(sprites[1]);

            if (unitData.vsTank >= 0.6f)
                strongList.Add(sprites[2]);

            if (unitData.vsAerial >= 0.6f)
                strongList.Add(sprites[3]);

            if (unitData.vsGunner >= 0.6f)
                strongList.Add(sprites[4]);

            if (unitData.vsRanged >= 0.6f)
                strongList.Add(sprites[5]);
        }
        else
        {
            Debug.LogError("UnitInfo::SetStrong - sprites.Count != 6");
        }

        PositionSpriteList(strongSprites, strongList);
    }

    void SetWeak()
    {
        List<Sprite> sprites = new List<Sprite>();
        BuildSpriteList(sprites);

        List<Sprite> weakList = new List<Sprite>();

        if (sprites.Count == 6)
        {
            if (unitData.fromInfantry >= 0.6f)
                weakList.Add(sprites[0]);

            if (unitData.fromTransport >= 0.6f)
                weakList.Add(sprites[1]);

            if (unitData.fromTank >= 0.6f)
                weakList.Add(sprites[2]);

            if (unitData.fromAerial >= 0.6f)
                weakList.Add(sprites[3]);

            if (unitData.fromGunner >= 0.6f)
                weakList.Add(sprites[4]);

            if (unitData.fromRanged >= 0.6f)
                weakList.Add(sprites[5]);
        }
        else
        {
            Debug.LogError("UnitInfo::SetWeak - sprites.Count != 6");
        }

        PositionSpriteList(weakSprites, weakList);
    }

    void BuildSpriteList(List<Sprite> sprites)
    {
        switch (FindObjectOfType<CutsceneController>().currentTurn)
        {
            case GameplayController.Turn.CANI:
                sprites.Add(hipsterInfantry);
                sprites.Add(hipsterTransport);
                sprites.Add(hipsterTank);
                sprites.Add(hipsterAerial);
                sprites.Add(hipsterGunner);
                sprites.Add(hipsterRanged);
                break;

            case GameplayController.Turn.HIPSTER:
                sprites.Add(caniInfantry);
                sprites.Add(caniTransport);
                sprites.Add(caniTank);
                sprites.Add(caniAerial);
                sprites.Add(caniGunner);
                sprites.Add(caniRanged);
                break;
        }
    }

    void PositionSpriteList(GameObject parent, List<Sprite> sprites)
    {
        int counter = 0;

        foreach(Sprite sprite in sprites)
        {
            GameObject newSprite = Instantiate(spritePrefab);
            newSprite.GetComponent<SpriteRenderer>().sprite = sprite;
            newSprite.transform.SetParent(parent.transform);
            newSprite.transform.position = parent.transform.position + new Vector3(counter + counter++ * 0.5f, 0, 0);
        }
    }

    void GetUnitData()
    {
        if (FindObjectOfType<JSONHandler>().unitDataCollection.unitDataList.Count == 6)
        {
            switch (unitType)
            {
                case UnitType.INFANTRY:
                    unitData = FindObjectOfType<JSONHandler>().unitDataCollection.unitDataList[0];
                    break;

                case UnitType.TRANSPORT:
                    unitData = FindObjectOfType<JSONHandler>().unitDataCollection.unitDataList[1];
                    break;

                case UnitType.TANK:
                    unitData = FindObjectOfType<JSONHandler>().unitDataCollection.unitDataList[2];
                    break;

                case UnitType.AERIAL:
                    unitData = FindObjectOfType<JSONHandler>().unitDataCollection.unitDataList[3];
                    break;

                case UnitType.GUNNER:
                    unitData = FindObjectOfType<JSONHandler>().unitDataCollection.unitDataList[4];
                    break;

                case UnitType.RANGED:
                    unitData = FindObjectOfType<JSONHandler>().unitDataCollection.unitDataList[5];
                    break;
            }
        }
        else
        {
            unitData = new UnitData();
        }
    }
}
