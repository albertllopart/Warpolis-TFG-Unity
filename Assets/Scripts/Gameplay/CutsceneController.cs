using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    bool afterStart = true;
    GameObject gameplayController;
    GameObject dataController;
    GameplayController.Turn currentTurn;

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

    // Start is called before the first frame update
    void Start()
    {
        gameplayController = GameObject.Find("Gameplay Controller");
        dataController = GameObject.Find("Data Controller");
    }

    void AfterStart()
    {
        //aquí s'han de cridar tots els AfterStart perquè s'inicialitzin bé tots els controladors que depenguin de coses que el cutscene controller manipularà
        GameObject.Find("UI Controller").GetComponent<UIController>().AfterStart();

        SubscribeToEvents();
        NewTurnCani();
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
    }

    void NewTurnCani()
    {
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)dataController.GetComponent<DataController>().caniMoney);
        MoneySetup((uint)dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().caniBuildings.Count * 1000);
    }

    void NewTurnHipster()
    {
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)dataController.GetComponent<DataController>().hipsterMoney);
        MoneySetup((uint)dataController.transform.Find("Buildings Controller").GetComponent<BuildingsController>().hipsterBuildings.Count * 1000);
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

    void EnableGameplay()
    {
        gameplayController.SetActive(true);
        gameplayController.GetComponent<GameplayController>().MyOnEnable();
    }

    void DisableGameplay()
    {
        gameplayController.SetActive(false);
    }

    void SubscribeToEvents()
    {
        gameplayController.GetComponent<GameplayController>().endTurnCani.AddListener(NewTurnHipster);
        gameplayController.GetComponent<GameplayController>().endTurnHipster.AddListener(NewTurnCani);
    }
}
