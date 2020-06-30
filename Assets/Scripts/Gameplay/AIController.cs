using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

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

    //timer
    float time = 0.0f;
    float timer = 0.0f;
    public UnityEvent finishedAITimer;

    // Start is called before the first frame update
    void Start()
    {
        finishedAITimer = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (inControl)
        {
            Control();
        }
    }

    public void MyOnDisable()
    {
        inControl = false;
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

        return ret;
    }

    public void SetInControl(bool set)
    {
        inControl = !set;

        if (inControl)
            GetAvailableEntities();
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

        //ordenar unitats
        List<GameObject> availableTransports = new List<GameObject>();

        switch (currentArmy)
        {
            case UnitArmy.CANI:
                foreach (GameObject unit in FindObjectOfType<UnitsController>().caniUnits)
                {
                    if (unit.activeSelf)
                    {
                        if (unit.GetComponent<Unit>().unitType != UnitType.TRANSPORT)
                            availableUnits.Add(unit);
                        else
                            availableTransports.Add(unit);
                    }
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
                    {
                        if (unit.GetComponent<Unit>().unitType != UnitType.TRANSPORT)
                            availableUnits.Add(unit);
                        else
                            availableTransports.Add(unit);
                    }
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

        if (availableUnits.Count > 0)
            SortUnits(availableTransports);
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

    void SortUnits(List<GameObject> transports)
    {
        foreach (GameObject transport in transports)
            availableUnits.Add(transport);
    }

    void Control()
    {
        switch(state)
        {
            case AIState.IDLE:

                if (SelectEntity())
                {
                    state = AIState.BUSSY;
                    CommandEntity();
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

            state = AIState.IDLE;
        }
    }

    void CommandUnit()
    {
        currentUnit.GetComponent<Unit>().finishedAI.AddListener(ToIdle);
        FindObjectOfType<CutsceneController>().finishedCameraTargeting.AddListener(currentUnit.GetComponent<Unit>().OnAI);
        FindObjectOfType<CutsceneController>().TargetCameraSetup(currentUnit.transform.position);
    }

    void CommandFactory()
    {
        int turnCount = FindObjectOfType<DataController>().currentTurn;

        if (turnCount > 2)
        {
            switch (JudgeTriangle())
            {
                case UnitType.TANK:
                    if (CreateTank())
                        return;
                    break;

                case UnitType.AERIAL:
                    if (CreateAerial())
                        return;
                    break;

                case UnitType.GUNNER:
                    if (CreateGunner())
                        return;
                    break;
            }
        }

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

        if (availableUnits.Count == 0)
        {
            SetAITimer(1);
            finishedAITimer.AddListener(TestingTimer);
        }
        else
            state = AIState.IDLE;
    }

    void TestingTimer()
    {
        finishedAITimer.RemoveListener(TestingTimer);
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
        else if (transportCount > 0 && infantryCount / transportCount < 6f && infantryCount < 6)
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

    UnitType JudgeTriangle()
    {
        UnitType ret = UnitType.TANK;

        int money = 0;
        UnitArmy enemy = currentArmy;
        switch (currentArmy)
        {
            case UnitArmy.CANI:
                enemy = UnitArmy.HIPSTER;
                money = FindObjectOfType<DataController>().caniMoney;
                break;

            case UnitArmy.HIPSTER:
                enemy = UnitArmy.CANI;
                money = FindObjectOfType<DataController>().hipsterMoney;
                break;
        }

        uint myTankCount = FindObjectOfType<UnitsController>().GetUnitCount(currentArmy, UnitType.TANK);
        uint enemyTankCount = FindObjectOfType<UnitsController>().GetUnitCount(enemy, UnitType.TANK);
        uint myAerialCount = FindObjectOfType<UnitsController>().GetUnitCount(currentArmy, UnitType.AERIAL);
        uint enemyAerialCount = FindObjectOfType<UnitsController>().GetUnitCount(enemy, UnitType.AERIAL);
        uint myGunnerCount = FindObjectOfType<UnitsController>().GetUnitCount(currentArmy, UnitType.GUNNER);
        uint enemyGunnerCount = FindObjectOfType<UnitsController>().GetUnitCount(enemy, UnitType.GUNNER);

        if (myTankCount < enemyGunnerCount)
            ret = UnitType.TANK;
        else if (myAerialCount < enemyTankCount && money >= FindObjectOfType<UnitsController>().caniAerial.GetComponent<Unit>().shopValue)
            ret = UnitType.AERIAL;
        else if (myGunnerCount < enemyAerialCount && money >= FindObjectOfType<UnitsController>().caniGunner.GetComponent<Unit>().shopValue)
            ret = UnitType.GUNNER;

        return ret;
    }

    bool CheckIfAvailable(Vector3 position)
    {
        //retorna false si troba una unitat a la casella, true si no

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

    public bool CheckRoutine(System.Diagnostics.Stopwatch st)
    {
        //retorna true si considera que la rutina s'ha d'interrompre
        bool ret = false;

        st.Stop();
        if (st.ElapsedMilliseconds > 12)
        {
            st.Reset();
            Debug.Log("AIController::CheckRoutine - Paused Routine");
            ret = true;
        }
        st.Start();

        return ret;
    }

    public void SetAITimer(float seconds)
    {
        timer = 0.0f;
        time = seconds;

        StartCoroutine(AITimer());
    }

    IEnumerator AITimer()
    {
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        finishedAITimer.Invoke();
        yield break;
    }

    bool CreateInfantry()
    {
        return FindObjectOfType<UnitsController>().InstantiateUnit(currentArmy, UnitType.INFANTRY, currentFactory.transform.position);
    }

    bool CreateTransport()
    {
        return FindObjectOfType<UnitsController>().InstantiateUnit(currentArmy, UnitType.TRANSPORT, currentFactory.transform.position);
    }

    bool CreateTank()
    {
        return FindObjectOfType<UnitsController>().InstantiateUnit(currentArmy, UnitType.TANK, currentFactory.transform.position);
    }

    bool CreateAerial()
    {
        return FindObjectOfType<UnitsController>().InstantiateUnit(currentArmy, UnitType.AERIAL, currentFactory.transform.position);
    }

    bool CreateGunner()
    {
        return FindObjectOfType<UnitsController>().InstantiateUnit(currentArmy, UnitType.GUNNER, currentFactory.transform.position);
    }

    bool CreateRanged()
    {
        return FindObjectOfType<UnitsController>().InstantiateUnit(currentArmy, UnitType.RANGED, currentFactory.transform.position);
    }
}
