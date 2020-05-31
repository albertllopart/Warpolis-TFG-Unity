using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuController : MonoBehaviour
{
    //controllers
    GameObject cameraController;
    GameObject gameplayController;
    GameObject mapController;

    //events
    public UnityEvent newGame;
    public UnityEvent endGame;

    //internal
    GameObject turnLimitGO;

    public enum TurnLimit { INFINITE, FINITE };
    public TurnLimit turnLimit = TurnLimit.INFINITE;
    public int turnLimitAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        newGame = new UnityEvent();
        endGame = new UnityEvent();

        cameraController = GameObject.Find("Camera");
        gameplayController = GameObject.Find("Gameplay Controller");
        mapController = GameObject.Find("Map Controller");

        turnLimitGO = transform.Find("Preparation").transform.Find("Background").transform.Find("Turn Limit").gameObject;

        FindObjectOfType<FadeTo>().finishedDecreasing.AddListener(SubscribeToEvents);
    }

    public void AfterStart()
    {
        DisableGameplay();

        if (turnLimit == TurnLimit.INFINITE)
            turnLimitGO.transform.Find("Number").GetComponent<Number>().SetInfinite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MyOnEnable()
    {
        SubscribeToEvents();
    }

    void MyOnDisable()
    {
        UnsubscribeFromEvents();
        gameObject.SetActive(false);
    }

    void EnablePreparation()
    {
        transform.Find("Preparation").gameObject.SetActive(true);
        transform.Find("Preparation").transform.position = cameraController.transform.position + new Vector3(0, 0, 10);
    }

    void DisablePreparation()
    {
        transform.Find("Preparation").gameObject.SetActive(false);
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

    void IncreaseTurnLimit()
    {
        switch (turnLimit)
        {
            case TurnLimit.INFINITE:
                turnLimit = TurnLimit.FINITE;
                turnLimitAmount = 15;
                UpdateTurnLimit();
                break;

            case TurnLimit.FINITE:
                turnLimitAmount += 5;

                if (turnLimitAmount > 50)
                    turnLimit = TurnLimit.INFINITE;

                UpdateTurnLimit();
                break;
        }

        FindObjectOfType<SoundController>().PlayPlayerMove();
    }

    void DecreaseTurnLimit()
    {
        switch (turnLimit)
        {
            case TurnLimit.INFINITE:
                turnLimit = TurnLimit.FINITE;
                turnLimitAmount = 50;
                UpdateTurnLimit();
                break;

            case TurnLimit.FINITE:
                turnLimitAmount -= 5;

                if (turnLimitAmount < 15)
                    turnLimit = TurnLimit.INFINITE;

                UpdateTurnLimit();
                break;
        }

        FindObjectOfType<SoundController>().PlayPlayerMove();
    }

    void UpdateTurnLimit()
    {
        switch (turnLimit)
        {
            case TurnLimit.INFINITE:
                turnLimitGO.transform.Find("Number").GetComponent<Number>().SetInfinite();
                break;

            case TurnLimit.FINITE:
                turnLimitGO.transform.Find("Number").GetComponent<Number>().CreateNumber(turnLimitAmount);
                break;
        }
    }

    void StartGame()
    {
        UnsubscribeFromEvents();

        SetTurnLimit();

        cameraController.GetComponent<CameraController>().FadeToWhiteSetup(1.0f);
        cameraController.GetComponent<CameraController>().fadeToWhiteRest.AddListener(NewGame);

        cameraController.GetComponent<CameraController>().fadeToWhiteEnd.AddListener(GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().NewGameSetup);

        //startGame.Invoke(); //de moment això no crida res

        FindObjectOfType<SoundController>().PlayButton();
    }

    void SetTurnLimit()
    {
        switch (turnLimit)
        {
            case TurnLimit.INFINITE:
                FindObjectOfType<DataController>().SetTurnLimit(0);
                break;

            case TurnLimit.FINITE:
                FindObjectOfType<DataController>().SetTurnLimit(turnLimitAmount);
                break;
        }
    }

    void NewGame()
    {
        cameraController.GetComponent<CameraController>().fadeToWhiteRest.RemoveListener(NewGame);

        DisablePreparation();
        UnsubscribeFromEvents();

        //carregar mapa
        LoadMap(FindObjectOfType<DataTransferer>().map);

        newGame.Invoke();
    }

    void LoadMap(GameObject selectedMap)
    {
        //eliminar gym
        mapController.GetComponent<MapController>().UnloadMap();

        //instanciar mapa
        mapController.GetComponent<MapController>().LoadMap(selectedMap); //selectedMap
    }

    public void EndGame()
    {
        endGame.Invoke();
    }

    public void OnGameEnded()
    {
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(LoadResults);
        FindObjectOfType<DataController>().TransferResults();

        //EnablePreparation();
        cameraController.GetComponent<CameraController>().fadeToWhiteRest.RemoveListener(MyOnEnable);

        FindObjectOfType<SoundController>().StopCani();
        FindObjectOfType<SoundController>().StopHipster();
    }

    void LoadResults()
    {
        Loader.Load(Loader.Scene.results);
    }

    public void CompleteGameLoop()
    {
        SubscribeToEvents();
        cameraController.GetComponent<CameraController>().fadeToWhiteEnd.RemoveListener(CompleteGameLoop);
    }

    void SubscribeToEvents()
    {
        FindObjectOfType<FadeTo>().finishedDecreasing.RemoveListener(SubscribeToEvents);

        GameObject.Find("Controls").GetComponent<Controls>().keyboard_o_down.AddListener(StartGame);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_d_down.AddListener(IncreaseTurnLimit);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_a_down.AddListener(DecreaseTurnLimit);
    }

    void UnsubscribeFromEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_o_down.RemoveListener(StartGame);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_d_down.RemoveListener(IncreaseTurnLimit);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_a_down.RemoveListener(DecreaseTurnLimit);
    }
}
