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

    //maps
    public GameObject mapTurdIsland;
    public GameObject mapSpanIsland;
    public GameObject big;
    public GameObject alphaIsland;

    //events
    public UnityEvent newGame;
    public UnityEvent endGame;

    // Start is called before the first frame update
    void Start()
    {
        newGame = new UnityEvent();
        endGame = new UnityEvent();

        cameraController = GameObject.Find("Camera");
        gameplayController = GameObject.Find("Gameplay Controller");
        mapController = GameObject.Find("Map Controller");
    }

    public void AfterStart()
    {
        DisableGameplay();
        SubscribeToEvents();
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

    void EnableMainMenu()
    {
        transform.Find("Main Menu").gameObject.SetActive(true);
        transform.Find("Main Menu").transform.position = cameraController.transform.position + new Vector3(0, 0, 10);
    }

    void DisableMainMenu()
    {
        transform.Find("Main Menu").gameObject.SetActive(false);
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

    void StartGame()
    {
        cameraController.GetComponent<CameraController>().FadeToWhiteSetup(1.0f);
        cameraController.GetComponent<CameraController>().fadeToWhiteRest.AddListener(NewGame);

        cameraController.GetComponent<CameraController>().fadeToWhiteEnd.AddListener(GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().NewGame);

        //startGame.Invoke(); //de moment això no crida res
    }

    void NewGame()
    {
        cameraController.GetComponent<CameraController>().fadeToWhiteRest.RemoveListener(NewGame);

        DisableMainMenu();
        UnsubscribeFromEvents();

        //carregar mapa
        LoadMap(/*selectedMap*/);

        newGame.Invoke();
    }

    void LoadMap(/*GameObject selectedMap*/)
    {
        //eliminar gym
        mapController.GetComponent<MapController>().UnloadMap();

        //instanciar mapa
        mapController.GetComponent<MapController>().LoadMap(alphaIsland); //selectedMap
    }

    public void EndGame()
    {
        endGame.Invoke();
    }

    public void OnGameEnded()
    {
        EnableMainMenu();
        cameraController.GetComponent<CameraController>().fadeToWhiteRest.RemoveListener(MyOnEnable);

        FindObjectOfType<SoundController>().StopCani();
        FindObjectOfType<SoundController>().StopHipster();
    }

    public void CompleteGameLoop()
    {
        SubscribeToEvents();
        cameraController.GetComponent<CameraController>().fadeToWhiteEnd.RemoveListener(CompleteGameLoop);
    }

    void SubscribeToEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_o_down.AddListener(StartGame);
    }

    void UnsubscribeFromEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_o_down.RemoveListener(StartGame);
    }
}
