using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneController : MonoBehaviour
{
    bool afterStart = true;
    GameObject gameplayController;
    GameObject dataController;
    GameObject cameraController;
    GameObject mapController;
    GameplayController.Turn currentTurn = GameplayController.Turn.CANI;

    float timer = 0.0f;

    //money
    bool addMoney = false;
    uint moneyToAdd;
    float moneyTime = 0.25f;
    float moneyTimer = 0.0f;

    //dying
    bool dying = false;
    GameObject dyingUnit;
    public GameObject attackingUnit;
    float dyingAlpha = 0.0f;
    float dyingTime = 0.1f;

    //extermination
    bool exterminating = false;
    UnitArmy exterminatedArmy;
    GameObject nextToExterminate;

    //camera
    bool cameraTargeting = false;
    Vector3 target;
    Vector3 goal;
    float targetCameraTime = 0.0025f;
    float targetCameraTimer = 0.0f;
    float cameraSpeed = 0.1f;

    //healing
    bool healing = false;
    List<GameObject> toHeal;
    GameObject healingUnit;
    public GameObject healingSignPrefab;
    GameObject healingSign;

    //events
    public UnityEvent unitDied;
    public UnityEvent repositionPlayer;
    public UnityEvent finishedCameraTargeting;
    public UnityEvent finishedAddingMoney;
    public UnityEvent finishedHealing;
    public UnityEvent finishedAllCutscenes;

    // Start is called before the first frame update
    void Start()
    {
        unitDied = new UnityEvent();
        repositionPlayer = new UnityEvent();
        finishedCameraTargeting = new UnityEvent();
        finishedAddingMoney = new UnityEvent();
        finishedHealing = new UnityEvent();
        finishedAllCutscenes = new UnityEvent();

        gameplayController = GameObject.Find("Gameplay Controller");
        dataController = GameObject.Find("Data Controller");
        cameraController = GameObject.Find("Camera");
        mapController = GameObject.Find("Map Controller");
    }

    void AfterStart()
    {
        //aquí s'han de cridar tots els AfterStart perquè s'inicialitzin bé tots els controladors que depenguin de coses que el cutscene controller manipularà
        GameObject.Find("UI Controller").GetComponent<UIController>().AfterStart();
        GameObject.Find("Data Controller").GetComponent<DataController>().AfterStart();
        GameObject.Find("Camera").GetComponent<CameraController>().AfterStart();

        SubscribeToEvents();
        //NewTurnCani(); Això s'ha de cridar quan comenci la partida

        //per últim cridem l'after start del Menu Controller que s'encarregarà de desactivar tots els gameobjects necessaris, cancel·lar subscripció a events, etc
        GameObject.Find("Menu Controller").GetComponent<MenuController>().AfterStart();

        afterStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (afterStart)
            AfterStart();

        if (addMoney)
            AddMoney();

        if (dying)
            UnitDeath();

        if (exterminating)
            ExterminateArmy();

        if (cameraTargeting)
            TargetCamera();

        if (healing)
            UnitHeal();
    }

    public void NewGame()
    {
        currentTurn = GameplayController.Turn.CANI;

        GameObject cameraController = GameObject.Find("Camera");

        cameraController.GetComponent<CameraController>().fadeToWhiteEnd.RemoveListener(NewGame);

        cameraController.transform.Find("UI Controller").GetComponent<UIController>().EnableMoneyInfo();
        cameraController.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)dataController.GetComponent<DataController>().caniMoney);
        FirstTurn((uint)dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().caniBuildings.Count * 1000);

        finishedAddingMoney.AddListener(UnitHealSetup);

        FindObjectOfType<SoundController>().PlayCani();
    }

    void NewTurnCani()
    {
        currentTurn = GameplayController.Turn.CANI;

        Camera.main.transform.Find("UI Controller").GetComponent<UIController>().EnableMoneyInfo();
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)dataController.GetComponent<DataController>().caniMoney);
        MoneySetup((uint)dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().caniBuildings.Count * 1000);

        finishedAddingMoney.AddListener(UnitHealSetup);

        FindObjectOfType<SoundController>().PlayCani();
    }

    void NewTurnHipster()
    {
        currentTurn = GameplayController.Turn.HIPSTER;

        Camera.main.transform.Find("UI Controller").GetComponent<UIController>().EnableMoneyInfo();
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)dataController.GetComponent<DataController>().hipsterMoney);
        MoneySetup((uint)dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().hipsterBuildings.Count * 1000);

        finishedAddingMoney.AddListener(UnitHealSetup);

        FindObjectOfType<SoundController>().PlayHipster();
    }

    public void FirstTurn(uint amount)
    {
        moneyToAdd = amount;
        addMoney = true;
        moneyTimer = 0.0f;
    }
    public void MoneySetup(uint amount)
    {
        moneyToAdd = amount;
        addMoney = true;
        moneyTimer = 0.0f;

        DisableGameplay();
    }

    void AddMoney()
    {
        moneyTimer += Time.deltaTime;

        if (moneyTimer >= moneyTime && moneyToAdd > 0)
        {
            switch (currentTurn)
            {
                case GameplayController.Turn.CANI:
                    dataController.GetComponent<DataController>().AddCaniMoney((int)moneyToAdd);
                    moneyToAdd = 0;
                    break;

                case GameplayController.Turn.HIPSTER:
                    dataController.GetComponent<DataController>().AddHipsterMoney((int)moneyToAdd);
                    moneyToAdd = 0;
                    break;
            }

            moneyTimer = 0.0f;
            FindObjectOfType<SoundController>().PlayMoney();
        }

        if (moneyToAdd == 0)
        {
            addMoney = false;
            finishedAddingMoney.Invoke();
            //EnableGameplay();
        }
    }

    //unit death
    public void DyingSetup(GameObject unit)
    {
        dyingUnit = unit;
        dying = true;
        timer = 0.0f;
        dyingAlpha = 1.0f;

        FindObjectOfType<SoundController>().PlayDeath();
    }

    void UnitDeath()
    {
        timer += Time.deltaTime;

        if (dyingAlpha < 0)
        {
            Destroy(dyingUnit);
            unitDied.Invoke(); // de moment ningú el fa servir

            if (dataController.GetComponent<DataController>().CheckWinConOnUnitDied(attackingUnit.GetComponent<Unit>().army)) //ha mort l'última unitat
            {
                gameplayController.GetComponent<GameplayController>().DisableMenuUnit();
                DisableGameplay();
            }
            else
            {
                if (attackingUnit != null && attackingUnit != dyingUnit)
                {
                    attackingUnit.GetComponent<Unit>().OnWait();
                    attackingUnit = null;
                }
                else
                {
                    gameplayController.GetComponent<GameplayController>().DisableMenuUnit();
                }
            }

            dying = false;
            return;
        }

        if (timer >= dyingTime && dyingAlpha != 0.0f)
        {
            dyingUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, dyingAlpha);
            dyingAlpha -= 0.1f;
            timer = 0.0f;
        }
    }

    void CheckWinConUnitDeath(GameObject unit)
    {
        if (unit == attackingUnit)
        {
            Debug.Log("CutsceneController::CheckWinConUnitDeath - Last unit commited suicide");


        }
    }

    public void ExterminationSetup(UnitArmy army)
    {
        exterminating = true;
        exterminatedArmy = army;
        timer = 0.0f;
        dyingAlpha = 1.0f;

        gameplayController.GetComponent<GameplayController>().ResetParameters();
        DisableGameplay(); //últim moment en el qual el gameplay està actiu durant la partida
    }

    void ExterminateArmy()
    {
        timer += Time.deltaTime;

        switch (exterminatedArmy)
        {
            case UnitArmy.CANI:
                ExterminateCaniArmy();
                break;

            case UnitArmy.HIPSTER:
                ExterminateHipsterArmy();
                break;
        }
    }

    void ExterminateCaniArmy()
    {
        if (nextToExterminate == null && dataController.transform.Find("Units Controller").GetComponent<UnitsController>().caniUnits.Count > 0)
        {
            nextToExterminate = dataController.transform.Find("Units Controller").GetComponent<UnitsController>().caniUnits[0];
            TargetCameraSetup(nextToExterminate.transform.position);
        }

        if (nextToExterminate == null) // en cas que hi hagi 0 unitats a eliminar - és un cas que no es donarà mai en gameplay normal, però s'ha de cobrir
        {
            if (dataController.transform.Find("Units Controller").GetComponent<UnitsController>().caniUnits.Count == 0)
            {
                Debug.Log("CutsceneController::ExterminateCaniArmy - Extermination completed");
                exterminating = false;

                //GameObject.Find("Camera").GetComponent<CameraController>().FadeToWhiteSetup(1.0f);
                GameObject.Find("Menu Controller").GetComponent<MenuController>().EndGame();
            }

            return;
        }

        if (nextToExterminate.GetComponent<Unit>().GetState() != UnitState.DYING && !cameraTargeting)
        {
            nextToExterminate.GetComponent<Unit>().MyOnExterminate(); //cridem aquesta funcio per canviar estat de la unitat, animació etc
            FindObjectOfType<SoundController>().PlayDeath();
        }

        if (dyingAlpha < 0)
        {
            Destroy(nextToExterminate);

            nextToExterminate = null;
            dyingAlpha = 1.0f;
            timer = 0.0f;

            if (dataController.transform.Find("Units Controller").GetComponent<UnitsController>().caniUnits.Count == 0)
            {
                Debug.Log("CutsceneController::ExterminateCaniArmy - Extermination completed");
                exterminating = false;
                GameObject.Find("Menu Controller").GetComponent<MenuController>().EndGame();
            }

            return;
        }

        if (timer >= dyingTime && dyingAlpha != 0.0f && !cameraTargeting)
        {
            nextToExterminate.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, dyingAlpha);
            dyingAlpha -= 0.1f;
            timer = 0.0f;
        }
    }

    void ExterminateHipsterArmy()
    {
        if (nextToExterminate == null && dataController.transform.Find("Units Controller").GetComponent<UnitsController>().hipsterUnits.Count > 0)
        {
            nextToExterminate = dataController.transform.Find("Units Controller").GetComponent<UnitsController>().hipsterUnits[0];
            TargetCameraSetup(nextToExterminate.transform.position);
        }

        if (nextToExterminate == null) // en cas que hi hagi 0 unitats a eliminar - és un cas que no es donarà mai en gameplay normal, però s'ha de cobrir
        {
            if (dataController.transform.Find("Units Controller").GetComponent<UnitsController>().hipsterUnits.Count == 0)
            {
                Debug.Log("CutsceneController::ExterminateCaniArmy - Extermination completed");
                exterminating = false;

                //GameObject.Find("Camera").GetComponent<CameraController>().FadeToWhiteSetup(1.0f);
                GameObject.Find("Menu Controller").GetComponent<MenuController>().EndGame();
            }

            return;
        }

        if (nextToExterminate.GetComponent<Unit>().GetState() != UnitState.DYING && !cameraTargeting)
            nextToExterminate.GetComponent<Unit>().MyOnExterminate(); //cridem aquesta funcio per canviar estat de la unitat, animació etc

        if (dyingAlpha < 0)
        {
            Destroy(nextToExterminate);

            nextToExterminate = null;
            dyingAlpha = 1.0f;
            timer = 0.0f;

            if (dataController.transform.Find("Units Controller").GetComponent<UnitsController>().hipsterUnits.Count == 0)
            {
                Debug.Log("CutsceneController::ExterminateCaniArmy - Extermination completed");
                exterminating = false;
                GameObject.Find("Menu Controller").GetComponent<MenuController>().EndGame();
            }

            return;
        }

        if (timer >= dyingTime && dyingAlpha != 0.0f && !cameraTargeting)
        {
            nextToExterminate.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, dyingAlpha);
            dyingAlpha -= 0.1f;
            timer = 0.0f;
        }
    }

    public void TargetCameraSetup(Vector3 target)
    {
        this.target = target;
        if (this.target.z != -10)
            this.target.z = -10;

        cameraTargeting = true;
        targetCameraTimer = 0.0f;

        CalculateGoal();
    }

    void CalculateGoal()
    {
        Vector3 cameraPosition = cameraController.transform.position;
        goal = target;

        if (target == cameraPosition)
        {
            goal = cameraPosition;
            return;
        }

        if (cameraPosition.x == target.x)
        {
            //buscar la Y
            goal.y = CalculateY(target);
            return;
        }

        if (cameraPosition.y == target.y)
        {
            //buscar la X
            goal.x = CalculateX(target);
            return;
        }

        //buscar X i Y
        goal.y = CalculateY(target);
        goal.x = CalculateX(target);

        Debug.Log("CutsceneController::CalculateGoal - New Goal = " + goal);
    }

    public Vector3 CalculateGoal(Vector3 unit) //per moure la càmera de forma abrupta
    {
        Vector3 cameraPosition = cameraController.transform.position;
        Vector3 ret = unit;

        if (unit == cameraPosition)
        {
            ret = cameraPosition;
            return ret;
        }

        if (cameraPosition.x == unit.x)
        {
            //buscar la Y
            ret.y = CalculateY(unit);
            return ret; ;
        }

        if (cameraPosition.y == unit.y)
        {
            //buscar la X
            ret.x = CalculateX(unit);
            return ret; ;
        }

        //buscar X i Y
        ret.y = CalculateY(unit);
        ret.x = CalculateX(unit);

        return ret;
    }

    int CalculateY(Vector3 vec)
    {
        Vector3 cameraPosition = cameraController.transform.position;

        int topMargin = 6;
        int bottomMargin = 5;
        int targetMargin = 0;

        int topLimit = 0; //world position
        int bottomLimit = (int)mapController.GetComponent<MapController>().GetBottomRightCorner().y; //world position

        if (vec.y > cameraPosition.y)
        {
            targetMargin = topLimit - (int)vec.y;

            if (targetMargin >= topMargin)
                return (int)vec.y;
            else
            {
                return (int)vec.y - (topMargin - targetMargin);
            }
        }
        else
        {
            targetMargin = -bottomLimit + (int)vec.y;

            if (targetMargin > bottomMargin)
                return (int)vec.y;
            else
            {
                return (int)vec.y - (-bottomMargin + targetMargin);
            }
        }
    }

    int CalculateX(Vector3 vec)
    {
        Vector3 cameraPosition = cameraController.transform.position;

        int leftMargin = 10;
        int rightMargin = 10;
        int targetMargin = 0;

        int leftLimit = -1; //world position
        int rightLimit = (int)mapController.GetComponent<MapController>().GetBottomRightCorner().x; //world position

        if (vec.x < cameraPosition.x)
        {
            targetMargin = leftLimit + (int)vec.x;

            if (targetMargin >= leftMargin)
                return (int)vec.x;
            else
            {
                return (int)vec.x + (leftMargin - targetMargin);
            }
        }
        else
        {
            targetMargin = rightLimit - (int)vec.x;

            if (targetMargin > rightMargin)
                return (int)vec.x;
            else
            {
                return (int)vec.x + (-rightMargin + targetMargin);
            }
        }
    }

    void TargetCamera()
    {
        targetCameraTimer += Time.deltaTime;

        if (targetCameraTimer >= targetCameraTime)
        {
            targetCameraTimer = 0.0f;

            Vector3 direction = cameraController.transform.InverseTransformPoint(goal);

            if (direction.magnitude < cameraSpeed)
            {
                cameraController.transform.position += direction;
                cameraController.GetComponent<CameraController>().cameraMoved.Invoke();
                cameraTargeting = false;
                finishedCameraTargeting.Invoke();
                return;
            }

            direction.Normalize();
            direction *= cameraSpeed;

            cameraController.transform.position += direction;
        }
    }

    public void UnitHealSetup()
    {
        finishedAddingMoney.RemoveListener(UnitHealSetup);

        switch (currentTurn)
        {
            case GameplayController.Turn.CANI:
                toHeal = dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().GetUnitsOnBuildings(UnitArmy.CANI);
                break;

            case GameplayController.Turn.HIPSTER:
                toHeal = dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().GetUnitsOnBuildings(UnitArmy.HIPSTER);
                break;
        }

        if (toHeal.Count > 0)
            healing = true;
        else
        {
            finishedHealing.Invoke();
            finishedCameraTargeting.AddListener(LastCutsceneEnded);

            switch (currentTurn)
            {
                case GameplayController.Turn.CANI:
                    TargetCameraSetup(dataController.GetComponent<DataController>().playerCaniPosition);
                    break;

                case GameplayController.Turn.HIPSTER:
                    TargetCameraSetup(dataController.GetComponent<DataController>().playerHipsterPosition);
                    break;
            }
            
        }
    }

    void UnitHeal()
    {
        if (healingSign == null)
        {
            if (toHeal.Count > 0)
            {
                healingUnit = toHeal[0];              
                TargetCameraSetup(healingUnit.transform.position);
                finishedCameraTargeting.AddListener(InstantiateHealingSign);
                healing = false;
            }
            else
            {
                healing = false;
                finishedHealing.Invoke();
                finishedCameraTargeting.AddListener(LastCutsceneEnded);

                switch (currentTurn)
                {
                    case GameplayController.Turn.CANI:
                        TargetCameraSetup(dataController.GetComponent<DataController>().playerCaniPosition);
                        break;

                    case GameplayController.Turn.HIPSTER:
                        TargetCameraSetup(dataController.GetComponent<DataController>().playerHipsterPosition);
                        break;
                }
            }
        }
    }

    public void InstantiateHealingSign()
    {
        finishedCameraTargeting.RemoveListener(InstantiateHealingSign);
        healingSign = Instantiate(healingSignPrefab, healingUnit.transform.position, Quaternion.identity);
        healingUnit.GetComponent<Unit>().OnHealing();
        PopToHealFirst();
        healing = true;
    }

    public void PopToHealFirst()
    {
        if (toHeal.Count > 0)
            toHeal.Remove(toHeal[0]);
    }

    void LastCutsceneEnded()
    {
        finishedCameraTargeting.RemoveListener(LastCutsceneEnded);
        finishedAllCutscenes.Invoke();
    }

    void EnableGameplay()
    {
        gameplayController.SetActive(true);
        gameplayController.GetComponent<GameplayController>().MyOnEnable();

        repositionPlayer.Invoke();
    }

    void DisableGameplay()
    {
        gameplayController.GetComponent<GameplayController>().MyOnDisable();
        gameplayController.SetActive(false);
    }

    void SubscribeToEvents()
    {
        gameplayController.GetComponent<GameplayController>().endTurnCani.AddListener(NewTurnHipster);
        gameplayController.GetComponent<GameplayController>().endTurnHipster.AddListener(NewTurnCani);

        //meus
        finishedAllCutscenes.AddListener(EnableGameplay);
    }
}
