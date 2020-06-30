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
                        if (FindObjectOfType<DataController>().caniMoney >= caniTank.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(caniTank, position, Quaternion.identity);
                            FindObjectOfType<DataController>().caniMoney -= (int)caniTank.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.AERIAL:
                        if (FindObjectOfType<DataController>().caniMoney >= caniAerial.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(caniAerial, position, Quaternion.identity);
                            FindObjectOfType<DataController>().caniMoney -= (int)caniAerial.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.GUNNER:
                        if (FindObjectOfType<DataController>().caniMoney >= caniGunner.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(caniGunner, position, Quaternion.identity);
                            FindObjectOfType<DataController>().caniMoney -= (int)caniGunner.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.RANGED:
                        if (FindObjectOfType<DataController>().caniMoney >= caniRanged.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(caniRanged, position, Quaternion.identity);
                            FindObjectOfType<DataController>().caniMoney -= (int)caniRanged.GetComponent<Unit>().shopValue;
                            return true;
                        }
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
                        if (FindObjectOfType<DataController>().hipsterMoney >= hipsterTank.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(hipsterTank, position, Quaternion.identity);
                            FindObjectOfType<DataController>().hipsterMoney -= (int)hipsterTank.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.AERIAL:
                        if (FindObjectOfType<DataController>().hipsterMoney >= hipsterAerial.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(hipsterAerial, position, Quaternion.identity);
                            FindObjectOfType<DataController>().hipsterMoney -= (int)hipsterAerial.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.GUNNER:
                        if (FindObjectOfType<DataController>().hipsterMoney >= hipsterGunner.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(hipsterGunner, position, Quaternion.identity);
                            FindObjectOfType<DataController>().hipsterMoney -= (int)hipsterGunner.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;

                    case UnitType.RANGED:
                        if (FindObjectOfType<DataController>().hipsterMoney >= hipsterGunner.GetComponent<Unit>().shopValue)
                        {
                            Instantiate(hipsterGunner, position, Quaternion.identity);
                            FindObjectOfType<DataController>().hipsterMoney -= (int)hipsterGunner.GetComponent<Unit>().shopValue;
                            return true;
                        }
                        break;
                }

                break;
        }

        return false;
    }

    public void InstantiateUnitWithNoValue(UnitArmy army, UnitType type, Vector3 position)
    {
        GameObject newUnit = null;

        switch (army)
        {
            case UnitArmy.CANI:

                switch (type)
                {
                    case UnitType.INFANTRY:
                        newUnit = Instantiate(caniInfantry, position, Quaternion.identity);
                        break;

                    case UnitType.TRANSPORT:
                        newUnit = Instantiate(caniTransport, position, Quaternion.identity);
                        break;

                    case UnitType.TANK:
                        newUnit = Instantiate(caniTank, position, Quaternion.identity);
                        break;

                    case UnitType.AERIAL:
                        newUnit = Instantiate(caniAerial, position, Quaternion.identity);
                        break;

                    case UnitType.GUNNER:
                        newUnit = Instantiate(caniGunner, position, Quaternion.identity);
                        break;

                    case UnitType.RANGED:
                        newUnit = Instantiate(caniRanged, position, Quaternion.identity);
                        break;
                }

                break;

            case UnitArmy.HIPSTER:

                switch (type)
                {
                    case UnitType.INFANTRY:
                        newUnit = Instantiate(hipsterInfantry, position, Quaternion.identity);
                        break;

                    case UnitType.TRANSPORT:
                        newUnit = Instantiate(hipsterTransport, position, Quaternion.identity);
                        break;

                    case UnitType.TANK:
                        newUnit = Instantiate(hipsterTank, position, Quaternion.identity);
                        break;

                    case UnitType.AERIAL:
                        newUnit = Instantiate(hipsterAerial, position, Quaternion.identity);
                        break;

                    case UnitType.GUNNER:
                        newUnit = Instantiate(hipsterGunner, position, Quaternion.identity);
                        break;

                    case UnitType.RANGED:
                        newUnit = Instantiate(hipsterGunner, position, Quaternion.identity);
                        break;
                }

                break;
        }

        if (newUnit != null)
            newUnit.GetComponent<Unit>().generatedByTileset = true;
    }
}
