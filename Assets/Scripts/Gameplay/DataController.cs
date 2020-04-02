using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataController : MonoBehaviour
{
    public int caniMoney = 0;
    public int hipsterMoney = 0;

    public int turnLimit = 15;
    public int currentTurn = 0;

    //events
    public UnityEvent baseCaptured;

    // Start is called before the first frame update
    void Start()
    {
        baseCaptured = new UnityEvent();
    }

    public void AfterStart()
    {
        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCaniMoney(int amount)
    {
        caniMoney += amount;
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)caniMoney);
    }

    public void AddHipsterMoney(int amount)
    {
        hipsterMoney += amount;
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)hipsterMoney);
    }

    void CheckWinConOnUnitDied()
    {
        GameplayController.Turn turn = GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn();

        switch (turn)
        {
            case GameplayController.Turn.CANI:
                if (transform.Find("Units Controller").GetComponent<UnitsController>().hipsterUnits.Count == 0)
                {
                    Debug.Log("DataController::CheckWinConOnUnitDied - Cani win by Elimination");

                    GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().attackingUnit.GetComponent<Unit>().OnWinCon();
                    GameObject.Find("Menu Controller").GetComponent<MenuController>().EndGame();
                }
                break;

            case GameplayController.Turn.HIPSTER:
                if (transform.Find("Units Controller").GetComponent<UnitsController>().caniUnits.Count == 0)
                {
                    Debug.Log("DataController::CheckWinConOnUnitDied - Hipster win by Elimination");

                    GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().attackingUnit.GetComponent<Unit>().OnWinCon();
                    GameObject.Find("Menu Controller").GetComponent<MenuController>().EndGame();
                }
                break;
        }
    }

    void CheckWinConOnBaseCaptured()
    {
        GameplayController.Turn turn = GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn();

        switch (turn)
        {
            case GameplayController.Turn.CANI:

                Debug.Log("DataController::CheckWinConOnBaseCaptured - Cani win by Invasion");

                GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().ExterminationSetup(UnitArmy.HIPSTER);
                break;

            case GameplayController.Turn.HIPSTER:

                Debug.Log("DataController::CheckWinConOnBaseCaptured - Hipster win by Invasion");

                GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().ExterminationSetup(UnitArmy.CANI);
                break;
        }
    }

    void ResetData()
    {
        caniMoney = 0;
        hipsterMoney = 0;
        currentTurn = 0;
    }

    void SubscribeToEvents()
    {
        //propis
        baseCaptured.AddListener(CheckWinConOnBaseCaptured);

        //foranis
        GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().unitDied.AddListener(CheckWinConOnUnitDied);
        GameObject.Find("Menu Controller").GetComponent<MenuController>().newGame.AddListener(ResetData);
    }
}
