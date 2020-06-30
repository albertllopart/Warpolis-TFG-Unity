using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public enum MainMenuState
    {
        MODE, MAP, OPTIONS, TUTORIAL, BATTLE
    }

    public MainMenuState state = MainMenuState.MODE;

    GameObject mode;
    GameObject map;
    GameObject battle;
    GameObject options;
    GameObject tutorial;

    bool afterStart = true;

    // Start is called before the first frame update
    void Start()
    {
        mode = transform.Find("Mode").gameObject;

        map = transform.Find("Map").gameObject;
        battle = transform.Find("Battle").gameObject;
        options = transform.Find("Options").gameObject;
        options.SetActive(false);
        tutorial = transform.Find("Tutorial").gameObject;
        tutorial.SetActive(false);

        FindObjectOfType<FadeTo>().finishedDecreasing.AddListener(SubscribeToEvents);
    }

    void AfterStart()
    {
        afterStart = false;

        map.GetComponent<MainMenuMap>().AfterStart(MainMenuMap.MapMode.VERSUS);
        battle.GetComponent<MainMenuMap>().AfterStart(MainMenuMap.MapMode.BATTLE);
        tutorial.GetComponent<MainMenuTutorial>().AfterStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (afterStart)
            AfterStart();
    }

    void JudgeO()
    {
        switch (state)
        {
            case MainMenuState.MODE:
                mode.GetComponent<MainMenuMode>().PressSelectedButton();
                break;

            case MainMenuState.MAP:
                map.GetComponent<MainMenuMap>().TransitionToNextScene();
                break;

            case MainMenuState.BATTLE:
                battle.GetComponent<MainMenuMap>().TransitionToNextScene();
                break;

            case MainMenuState.TUTORIAL:
                tutorial.GetComponent<MainMenuTutorial>().TransitionToNextScene();
                break;
        }
    }

    void JudgeK()
    {
        switch (state)
        {
            case MainMenuState.MODE:
                TransitionToPreviousScene();
                break;

            case MainMenuState.MAP:
                map.GetComponent<MainMenuMap>().TransitionToMode();
                break;

            case MainMenuState.BATTLE:
                battle.GetComponent<MainMenuMap>().TransitionToMode();
                break;

            case MainMenuState.TUTORIAL:
                tutorial.GetComponent<MainMenuTutorial>().TransitionToMode();
                break;
        }
    }

    void JudgeW()
    {
        switch (state)
        {
            case MainMenuState.MODE:
                mode.GetComponent<MainMenuMode>().RotateDown();
                break;
        }
    }

    void JudgeS()
    {
        switch (state)
        {
            case MainMenuState.MODE:
                mode.GetComponent<MainMenuMode>().RotateUp();
                break;
        }
    }

    void TransitionToPreviousScene()
    {
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(LoadPreviousScene);

        FindObjectOfType<SoundController>().PlayBack();
    }

    void LoadPreviousScene()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(LoadPreviousScene);
        Loader.Load(Loader.Scene.title);
    }

    void TransitionToNextScene()
    {
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(LoadNextScene);
    }

    void LoadNextScene()
    {
        FindObjectOfType<SoundController>().StopTitle();
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(LoadNextScene);
        Loader.Load(Loader.Scene.game);
    }

    void TransitionToMap()
    {
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(SwitchToMap);
    }

    void SwitchToMap()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(SwitchToMap);
        DisableMode();
        EnableMap();
        FindObjectOfType<FadeTo>().FadeFromSetup();
    }

    void TransitionToBattle()
    {
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(SwitchToBattle);
    }

    void SwitchToBattle()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(SwitchToBattle);
        DisableMode();
        EnableBattle();
        FindObjectOfType<FadeTo>().FadeFromSetup();
    }

    void TransitionToTutorial()
    {
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(SwitchToTutorial);
    }

    void SwitchToTutorial()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(SwitchToTutorial);
        DisableMode();
        EnableTutorial();
        FindObjectOfType<FadeTo>().FadeFromSetup();
    }

    void TransitionToQuit()
    {
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(Quit);
    }

    void Quit()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(Quit);
        FindObjectOfType<SoundController>().StopTitle();
        Loader.Quit();
    }

    public void EnableMode()
    {
        mode.SetActive(true);
        state = MainMenuState.MODE;
    }

    void DisableMode()
    {
        mode.SetActive(false);
    }

    void EnableMap()
    {
        map.SetActive(true);
        state = MainMenuState.MAP;
        map.GetComponent<MainMenuMap>().MyOnEnable();
    }

    void DisableMap()
    {
        map.SetActive(false);
    }

    void EnableBattle()
    {
        battle.SetActive(true);
        state = MainMenuState.BATTLE;
        battle.GetComponent<MainMenuMap>().MyOnEnable();
    }

    void EnableOptions()
    {
        options.SetActive(true);
        state = MainMenuState.OPTIONS;
    }

    void DisableOptions()
    {
        options.SetActive(false);
    }

    void EnableTutorial()
    {
        tutorial.SetActive(true);
        state = MainMenuState.TUTORIAL;
        tutorial.GetComponent<MainMenuTutorial>().MyOnEnable();
    }

    void DisableTutorial()
    {
        tutorial.SetActive(false);
    }

    public void SubscribeToEvents()
    {
        //controls
        FindObjectOfType<Controls>().keyboard_o_down.AddListener(JudgeO);
        FindObjectOfType<Controls>().keyboard_k_down.AddListener(JudgeK);

        FindObjectOfType<Controls>().keyboard_w_down.AddListener(JudgeW);
        FindObjectOfType<Controls>().keyboard_w.AddListener(JudgeW);
        FindObjectOfType<Controls>().keyboard_s_down.AddListener(JudgeS);
        FindObjectOfType<Controls>().keyboard_s.AddListener(JudgeS);

        //children
        mode.GetComponent<MainMenuMode>().versusPressed.AddListener(TransitionToMap);
        mode.GetComponent<MainMenuMode>().battlePressed.AddListener(TransitionToBattle);
        mode.GetComponent<MainMenuMode>().quitPressed.AddListener(TransitionToQuit);
        mode.GetComponent<MainMenuMode>().tutorialPressed.AddListener(TransitionToTutorial);

        Debug.Log("MainMenuController::SubscribeToEvents - Subscribed to Events");
    }

    public void UnsubscribeFromEvents()
    {
        //controls
        FindObjectOfType<Controls>().keyboard_o_down.RemoveListener(JudgeO);
        FindObjectOfType<Controls>().keyboard_k_down.RemoveListener(JudgeK);

        FindObjectOfType<Controls>().keyboard_w_down.RemoveListener(JudgeW);
        FindObjectOfType<Controls>().keyboard_w.RemoveListener(JudgeW);
        FindObjectOfType<Controls>().keyboard_s_down.RemoveListener(JudgeS);
        FindObjectOfType<Controls>().keyboard_s.RemoveListener(JudgeS);

        //children
        mode.GetComponent<MainMenuMode>().versusPressed.RemoveListener(TransitionToMap);
        mode.GetComponent<MainMenuMode>().battlePressed.RemoveListener(TransitionToBattle);
        mode.GetComponent<MainMenuMode>().quitPressed.RemoveListener(TransitionToQuit);
        mode.GetComponent<MainMenuMode>().tutorialPressed.RemoveListener(TransitionToTutorial);

        Debug.Log("MainMenuController::UnsubscribeFromEvents - Unsubscribed from Events");
    }
}
