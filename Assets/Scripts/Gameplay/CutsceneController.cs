using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneController : MonoBehaviour
{
    bool afterStart = true;
    GameObject gameplayController;
    GameObject dataController;
    GameplayController.Turn currentTurn = GameplayController.Turn.CANI;

    float timer = 0.0f;

    //money
    bool addMoney = false;
    uint moneyToAdd;
    float moneyTimer = 0.25f;

    //dying
    bool dying = false;
    GameObject dyingUnit;
    public GameObject attackingUnit;
    float dyingAlpha = 0.0f;
    float dyingTimer = 0.1f;

    //extermination
    bool exterminating = false;
    UnitArmy exterminatedArmy;
    GameObject nextToExterminate;

    //events
    public UnityEvent unitDied;
    public UnityEvent repositionPlayer;

    // Start is called before the first frame update
    void Start()
    {
        unitDied = new UnityEvent();
        repositionPlayer = new UnityEvent();

        gameplayController = GameObject.Find("Gameplay Controller");
        dataController = GameObject.Find("Data Controller");
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
    }

    public void NewGame()
    {
        GameObject cameraController = GameObject.Find("Camera");

        cameraController.GetComponent<CameraController>().fadeToWhiteEnd.RemoveListener(NewGame);

        cameraController.transform.Find("UI Controller").GetComponent<UIController>().EnableMoneyInfo();
        cameraController.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)dataController.GetComponent<DataController>().caniMoney);
        FirstTurn((uint)dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().caniBuildings.Count * 1000);
    }

    void NewTurnCani()
    {
        Camera.main.transform.Find("UI Controller").GetComponent<UIController>().EnableMoneyInfo();
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)dataController.GetComponent<DataController>().caniMoney);
        MoneySetup((uint)dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().caniBuildings.Count * 1000);
    }

    void NewTurnHipster()
    {
        Camera.main.transform.Find("UI Controller").GetComponent<UIController>().EnableMoneyInfo();
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)dataController.GetComponent<DataController>().hipsterMoney);
        MoneySetup((uint)dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().hipsterBuildings.Count * 1000);
    }

    public void FirstTurn(uint amount)
    {
        moneyToAdd = amount;
        addMoney = true;
        timer = 0.0f;
    }
    public void MoneySetup(uint amount)
    {
        moneyToAdd = amount;
        addMoney = true;
        timer = 0.0f;

        currentTurn = gameplayController.GetComponent<GameplayController>().GetTurn();
        DisableGameplay();
    }

    void AddMoney()
    {
        timer += Time.deltaTime;

        if (timer >= moneyTimer && moneyToAdd > 0)
        {
            switch (currentTurn)
            {
                case GameplayController.Turn.CANI:
                    dataController.GetComponent<DataController>().AddCaniMoney(1000);
                    moneyToAdd -= 1000;
                    break;

                case GameplayController.Turn.HIPSTER:
                    dataController.GetComponent<DataController>().AddHipsterMoney(1000);
                    moneyToAdd -= 1000;
                    break;
            }

            timer = 0.0f;
        }

        if (moneyToAdd == 0)
        {
            addMoney = false;
            EnableGameplay();
            repositionPlayer.Invoke();
        }
    }

    //unit death
    public void DyingSetup(GameObject unit)
    {
        dyingUnit = unit;
        dying = true;
        timer = 0.0f;
        dyingAlpha = 1.0f;
    }

    void UnitDeath()
    {
        timer += Time.deltaTime;

        if (dyingAlpha < 0)
        {
            Destroy(dyingUnit);
            unitDied.Invoke();

            if (attackingUnit != null)
            { 
                attackingUnit.GetComponent<Unit>().OnWait();
            }
            else
            {
                gameplayController.GetComponent<GameplayController>().DisableMenuUnit();
                gameplayController.GetComponent<GameplayController>().playerState = GameplayController.PlayerState.NAVIGATING;
            }

            dying = false;
            attackingUnit = null;
            dyingUnit = null;
            dyingAlpha = 1.0f;
            return;
        }

        if (timer >= dyingTimer && dyingAlpha != 0.0f)
        {
            dyingUnit.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, dyingAlpha);
            dyingAlpha -= 0.1f;
            timer = 0.0f;
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

        if (nextToExterminate.GetComponent<Unit>().GetState() != UnitState.DYING)
            nextToExterminate.GetComponent<Unit>().MyOnExterminate(); //cridem aquesta funcio per canviar estat de la unitat, animació etc

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

        if (timer >= dyingTimer && dyingAlpha != 0.0f)
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

        if (nextToExterminate.GetComponent<Unit>().GetState() != UnitState.DYING)
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

        if (timer >= dyingTimer && dyingAlpha != 0.0f)
        {
            nextToExterminate.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, dyingAlpha);
            dyingAlpha -= 0.1f;
            timer = 0.0f;
        }
    }

    void EnableGameplay()
    {
        gameplayController.SetActive(true);
        gameplayController.GetComponent<GameplayController>().MyOnEnable();
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
    }
}
