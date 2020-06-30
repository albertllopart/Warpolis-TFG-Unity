using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialController : MonoBehaviour
{
    public UnityEvent newGame;

    //tutorial events
    public UnityEvent steppedOnEnemyBase;
    public UnityEvent capturedBuilding;
    public UnityEvent unitDied;

    void Awake()
    {
        newGame = new UnityEvent();

        //tutorial events
        steppedOnEnemyBase = new UnityEvent();
        capturedBuilding = new UnityEvent();
        unitDied = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
    }

    public void AfterStart()
    {
        //StartGame();
        //DisableGameplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisableGameplay()
    {
        //FindObjectOfType<GameplayController>().MyOnDisable();
        //FindObjectOfType<GameplayController>().gameObject.SetActive(false);
    }

    void StartGame()
    {
        NewGame();
        FindObjectOfType<DataController>().SetTurnLimit(0);

        FindObjectOfType<DialogueController>().finishedFading.AddListener(FindObjectOfType<CutsceneController>().NewGameSetup);
    }

    void NewGame()
    {
        LoadMap(FindObjectOfType<DataTransferer>().map);

        newGame.Invoke();
    }

    void LoadMap(GameObject selectedMap)
    {
        //eliminar gym
        FindObjectOfType<MapController>().UnloadMap();

        //instanciar mapa
        FindObjectOfType<MapController>().LoadMap(selectedMap); //selectedMap
    }

    void CheckWinConBase()
    {
        if (FindObjectOfType<MapInfo>().mapName == "tutorialMap1" || FindObjectOfType<MapInfo>().mapName == "tutorialMap3" ||
            FindObjectOfType<MapInfo>().mapName == "tutorialMap4" || FindObjectOfType<MapInfo>().mapName == "tutorialMap5" ||
            FindObjectOfType<MapInfo>().mapName == "tutorialMap6" || FindObjectOfType<MapInfo>().mapName == "tutorialMap7" ||
            FindObjectOfType<MapInfo>().mapName == "tutorialMap8")
        {
            FindObjectOfType<GameplayController>().MyOnDisable();
            FindObjectOfType<UIController>().DisableMoneyInfo();
            FindObjectOfType<UIController>().DisableTileInfo();

            FindObjectOfType<DialogueController>().EndDialogue();
            FindObjectOfType<DialogueController>().finishedFading.AddListener(TransitionToMainMenu);
        }
    }

    void CheckWinConCapture()
    {
        if (FindObjectOfType<MapInfo>().mapName == "tutorialMap2")
        {
           if (FindObjectOfType<BuildingsController>().caniBuildings.Count >= 6)
            {
                FindObjectOfType<GameplayController>().MyOnDisable();
                FindObjectOfType<UIController>().DisableMoneyInfo();
                FindObjectOfType<UIController>().DisableTileInfo();

                FindObjectOfType<DialogueController>().EndDialogue();
                FindObjectOfType<DialogueController>().finishedFading.AddListener(TransitionToMainMenu);
            }
        }
    }

    void CheckWinConKill()
    {

    }

    void TransitionToMainMenu()
    {
        FindObjectOfType<DialogueController>().finishedFading.RemoveListener(TransitionToMainMenu);

        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(SwitchToMainMenu);
    }

    void SwitchToMainMenu()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(SwitchToMainMenu);

        FindObjectOfType<SoundController>().StopCani();
        FindObjectOfType<SoundController>().PlayTitle();

        Loader.Load(Loader.Scene.main_menu);
    }

    void SubscribeToEvents()
    {
        steppedOnEnemyBase.AddListener(CheckWinConBase);
        capturedBuilding.AddListener(CheckWinConCapture);
        unitDied.AddListener(CheckWinConKill);
    }
}
