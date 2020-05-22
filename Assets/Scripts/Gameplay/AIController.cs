using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIController : MonoBehaviour
{
    public enum AIState
    {
        IDLE, BUSSY
    };

    public AIState state;

    UnitArmy currentArmy;
    public bool inControl;

    List<GameObject> availableUnits;
    List<GameObject> availableFactories;
    GameObject currentUnit;
    GameObject currentFactory;

    public GameObject myBase;
    public GameObject enemyBase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inControl)
        {
            Control();
        }
    }

    public bool JudgeCommander()
    {
        //retorna true si el commander és humà, fals si és cpu (per decidir si activar GamePlayController)
        bool ret = true;

        SetCurrentArmy();

        switch (currentArmy)
        {
            case UnitArmy.CANI:

                switch (FindObjectOfType<DataController>().caniPlayer)
                {
                    case DataController.PlayerCommander.HUMAN:
                        ret = true;
                        break;

                    case DataController.PlayerCommander.COMPUTER:
                        ret = false;
                        break;
                }

                break;

            case UnitArmy.HIPSTER:

                switch (FindObjectOfType<DataController>().hipsterPlayer)
                {
                    case DataController.PlayerCommander.HUMAN:
                        ret = true;
                        break;

                    case DataController.PlayerCommander.COMPUTER:
                        ret = false;
                        break;
                }

                break;
        }

        inControl = !ret;

        if (inControl)
            GetAvailableEntities();

        return ret;
    }

    void SetCurrentArmy()
    {
        switch (FindObjectOfType<CutsceneController>().currentTurn)
        {
            case GameplayController.Turn.CANI:
                currentArmy = UnitArmy.CANI;
                break;

            case GameplayController.Turn.HIPSTER:
                currentArmy = UnitArmy.HIPSTER;
                break;
        }
    }

    void GetAvailableEntities()
    {
        availableUnits = new List<GameObject>();
        availableFactories = new List<GameObject>();

        switch (currentArmy)
        {
            case UnitArmy.CANI:
                foreach (GameObject unit in FindObjectOfType<UnitsController>().caniUnits)
                {
                    if (unit.activeSelf)
                        availableUnits.Add(unit);
                }
                foreach (GameObject factory in FindObjectOfType<BuildingsController>().caniBuildings)
                {
                    if (factory.GetComponent<Building>().type == BuildingType.FACTORY)
                        availableFactories.Add(factory);
                }
                break;

            case UnitArmy.HIPSTER:
                foreach (GameObject unit in FindObjectOfType<UnitsController>().hipsterUnits)
                {
                    if (unit.activeSelf)
                        availableUnits.Add(unit);
                }
                foreach (GameObject factory in FindObjectOfType<BuildingsController>().hipsterBuildings)
                {
                    if (factory.GetComponent<Building>().type == BuildingType.FACTORY)
                        availableFactories.Add(factory);
                }
                break;
        }

        Debug.Log("AIController::GetAvailableEntities - Added " + availableUnits.Count + " Units");
        Debug.Log("AIController::GetAvailableEntities - Added " + availableFactories.Count + " Factories");

        if (availableFactories.Count > 0)
            SortFactories();
    }

    void SortFactories()
    {
        //ordena les fàbriques de més propera a la base enemiga a més llunyana, per d'aquesta manera construir les unitats més importants més a prop del conflicte
        availableFactories.OrderBy(availableFactories => availableFactories.transform.position.x);

        switch (currentArmy)
        {
            case UnitArmy.CANI:
                myBase = FindObjectOfType<BuildingsController>().caniBase;
                enemyBase = FindObjectOfType<BuildingsController>().hipsterBase;
                break;

            case UnitArmy.HIPSTER:
                myBase = FindObjectOfType<BuildingsController>().hipsterBase;
                enemyBase = FindObjectOfType<BuildingsController>().caniBase;
                break;
        }

        if (myBase.transform.position.x > enemyBase.transform.position.x) //la base és a la dreta del mapa
        {
            availableFactories.Reverse();
        }
    }

    void Control()
    {
        switch(state)
        {
            case AIState.IDLE:

                if (SelectEntity())
                {
                    CommandEntity();
                    //state = AIState.BUSSY;
                }
                else
                {
                    //esperar uns segons i passar torn
                    inControl = false;
                    FindObjectOfType<GameplayController>().EndTurn();
                }

                break;

            case AIState.BUSSY:
                break;
        }
    }

    bool SelectEntity()
    {
        //retorna true si hi ha alguna entitat per controlar
        bool ret = false;

        if (availableUnits.Count > 0)
        {
            currentUnit = availableUnits[0];
            availableUnits.RemoveAt(0);
            ret = true;

            Debug.Log("AIController::SelectEntity - Currently deciding over Unit: " + currentUnit.name + " in Position: " + currentUnit.transform.position);
        }
        else if (availableFactories.Count > 0)
        {
            currentFactory = availableFactories[0];
            availableFactories.RemoveAt(0);
            ret = true;

            Debug.Log("AIController::SelectEntity - Currently deciding over Factory: " + currentFactory.name + " in Position: " + currentFactory.transform.position);
        }

        return ret;
    }

    void CommandEntity()
    {
        if (currentUnit != null)
        {
            CommandUnit();
        }
        else if (currentFactory != null)
        {
            //mirar si hi ha unitats al damunt
            if (CheckIfAvailable(currentFactory.transform.position))
                CommandFactory();
        }
    }

    void CommandUnit()
    {
        currentUnit.GetComponent<Unit>().finishedAI.AddListener(ToIdle);
        currentUnit.GetComponent<Unit>().OnAI();
    }

    void CommandFactory()
    {
        if (JudgeInfantry())
            CreateInfantry();
        else if (JudgeTransport())
            CreateTransport();
    }

    public void ToIdle()
    {
        if (currentUnit != null)
        {
            currentUnit.GetComponent<Unit>().finishedAI.RemoveListener(ToIdle);
            currentUnit = null;
        }

        state = AIState.IDLE;
    }

    bool JudgeInfantry()
    {
        uint infantryCount = FindObjectOfType<UnitsController>().GetUnitCount(currentArmy, UnitType.INFANTRY);
        uint transportCount = FindObjectOfType<UnitsController>().GetUnitCount(currentArmy, UnitType.TRANSPORT);

        if (infantryCount < 3)
        {
            return true;
        }
        else if (transportCount > 0 && infantryCount / transportCount < 6f)
        {
            return true;
        }

        return false;
    }

    bool JudgeTransport()
    {
        uint infantryCount = FindObjectOfType<UnitsController>().GetUnitCount(currentArmy, UnitType.INFANTRY);
        uint transportCount = FindObjectOfType<UnitsController>().GetUnitCount(currentArmy, UnitType.TRANSPORT);

        if (transportCount == 0)
        {
            return true;
        }
        else if (infantryCount / transportCount >= 6f)
        {
            return true;
        }

        return false;
    }

    bool CheckIfAvailable(Vector3 position)
    {
        //returna false si troba una unitat a la casella, true si no

        List<RaycastHit2D> results = new List<RaycastHit2D>();

        RaycastHit2D resultCani = RayCast(position, LayerMask.GetMask("Cani_units"));
        if (resultCani.collider != null)
            results.Add(resultCani);

        RaycastHit2D resultHipster = RayCast(position, LayerMask.GetMask("Hipster_units"));
        if (resultHipster.collider != null)
            results.Add(resultHipster);

        foreach (RaycastHit2D result in results)
        {
            if (result.collider != null)
            {
                return false;
            }
        }

        return true;
    }

    public RaycastHit2D RayCast(Vector3 position, int layer)
    {
        Vector2 from = position + new Vector3(0.5f, -0.5f, 0);
        Vector2 to = from;

        return Physics2D.Linecast(from, to, layer);
    }

    bool CreateInfantry()
    {
        return FindObjectOfType<UnitsController>().InstantiateUnit(currentArmy, UnitType.INFANTRY, currentFactory.transform.position);
    }

    bool CreateTransport()
    {
        return FindObjectOfType<UnitsController>().InstantiateUnit(currentArmy, UnitType.TRANSPORT, currentFactory.transform.position);
    }
}
