using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsController : MonoBehaviour
{
    [Header("Cani Units")]
    public GameObject caniInfantry;
    public GameObject caniTransport;
    public GameObject caniTank;
    public GameObject caniAerial;
    public GameObject caniGunner;
    public GameObject caniRanged;

    [Header("Hipster Units")]
    public GameObject hipsterInfantry;
    public GameObject hipsterTransport;
    public GameObject hipsterTank;
    public GameObject hipsterAerial;
    public GameObject hipsterGunner;
    public GameObject hipsterRanged;

    [Header("Lists")]
    public List<GameObject> caniUnits;
    public List<GameObject> hipsterUnits;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetCani()
    {
        foreach (GameObject unit in caniUnits)
        {
            unit.GetComponent<Unit>().OnIdle();
        }
    }

    void ResetHipster()
    {
        foreach (GameObject unit in hipsterUnits)
        {
            unit.GetComponent<Unit>().OnIdle();
        }
    }

    void DestroyAllUnits()
    {
        foreach (GameObject unit in caniUnits)
        {
            Destroy(unit);
        }

        foreach (GameObject unit in hipsterUnits)
        {
            Destroy(unit);
        }

        caniUnits = new List<GameObject>();
        hipsterUnits = new List<GameObject>();
    }

    public GameObject FindNextActiveUnit()
    {
        switch (FindObjectOfType<GameplayController>().GetTurn())
        {
            case GameplayController.Turn.CANI:
                foreach (GameObject unit in caniUnits)
                {
                    if (unit.GetComponent<Unit>().state != UnitState.WAITING)
                    {
                        return unit;
                    }
                }
                break;

            case GameplayController.Turn.HIPSTER:
                foreach (GameObject unit in hipsterUnits)
                {
                    if (unit.GetComponent<Unit>().state != UnitState.WAITING)
                    {
                        return unit;
                    }
                }
                break;
        }

        return null;
    }

    void SubscribeToEvents()
    {
        GameObject gameplay = GameObject.Find("Gameplay Controller");

        gameplay.GetComponent<GameplayController>().endTurnCani.AddListener(ResetCani);
        gameplay.GetComponent<GameplayController>().endTurnHipster.AddListener(ResetHipster);

        GameObject.Find("Map Controller").GetComponent<MapController>().mapUnloaded.AddListener(DestroyAllUnits);
    }

    public uint GetUnitCount(UnitArmy army, UnitType type)
    {
        uint ret = 0;

        switch (army)
        {
            case UnitArmy.CANI:

                foreach (GameObject unit in caniUnits)
                {
                    if (unit.GetComponent<Unit>().unitType == type)
                        ret++;
                }

                break;

            case UnitArmy.HIPSTER:

                foreach (GameObject unit in hipsterUnits)
                {
                    if (unit.GetComponent<Unit>().unitType == type)
                        ret++;
                }

                break;
        }

        return ret;
    }

    public bool InstantiateUnit(UnitArmy army, UnitType type, Vector3 position)
    {
        switch (army)
        {
            case UnitArmy.CANI:

                switch (type)
                {
                    case UnitType.INFANTRY:
                        if (FindObjectOfType<DataController>().caniMoney >= caniInfantry.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(caniInfantry, position, Quaternion.identity);
                            FindObjectOfType<DataController>().caniMoney -= (int)caniInfantry.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.TRANSPORT:
                        if (FindObjectOfType<DataController>().caniMoney >= caniTransport.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(caniTransport, position, Quaternion.identity);
                            FindObjectOfType<DataController>().caniMoney -= (int)caniTransport.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.TANK:
                        break;

                    case UnitType.AERIAL:
                        break;

                    case UnitType.GUNNER:
                        break;

                    case UnitType.RANGED:
                        break;
                }

                break;

            case UnitArmy.HIPSTER:

                switch (type)
                {
                    case UnitType.INFANTRY:
                        if (FindObjectOfType<DataController>().hipsterMoney >= hipsterInfantry.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(hipsterInfantry, position, Quaternion.identity);
                            FindObjectOfType<DataController>().hipsterMoney -= (int)hipsterInfantry.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.TRANSPORT:
                        if (FindObjectOfType<DataController>().hipsterMoney >= hipsterTransport.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(hipsterTransport, position, Quaternion.identity);
                            FindObjectOfType<DataController>().hipsterMoney -= (int)hipsterTransport.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.TANK:
                        break;

                    case UnitType.AERIAL:
                        break;

                    case UnitType.GUNNER:
                        break;

                    case UnitType.RANGED:
                        break;
                }

                break;
        }

        return false;
    }
}
