using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public enum MainMenuState
    {
        MODE, MAP, OPTIONS, TUTORIAL
    }

    public MainMenuState state = MainMenuState.MODE;

    GameObject mode;
    GameObject map;
    GameObject options;
    GameObject tutorial;

    bool afterStart = true;

    // Start is called before the first frame update
    void Start()
    {
        mode = transform.Find("Mode").gameObject;

        map = transform.Find("Map").gameObject;
        options = transform.Find("Options").gameObject;
        options.SetActive(false);
        tutorial = transform.Find("Tutorial").gameObject;
        tutorial.SetActive(false);

        FindObjectOfType<FadeTo>().finishedDecreasing.AddListener(SubscribeToEvents);
    }

    void AfterStart()
    {
        afterStart = false;

        map.GetComponent<MainMenuMap>().AfterStart();
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

        Debug.Log("MainMenuController::UnsubscribeFromEvents - Unsubscribed from Events");
    }
}
