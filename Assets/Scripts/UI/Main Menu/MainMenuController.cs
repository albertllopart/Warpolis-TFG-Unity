using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public enum MainMenuState
    {
        MAIN, VERSUS
    }

    public MainMenuState state = MainMenuState.MAIN;

    //main
    public GameObject versusButton;
    public GameObject onlineButton;
    public GameObject tutorialButton;
    public GameObject optionsButton;

    public Vector2 selectedButtonPosition;
    public Vector2 lowerButtonPosition;
    public Vector2 upperButtonPosition;
    public Vector2 fadeDownButtonPosition;
    public Vector2 fadeUpButtonPosition;

    GameObject upperButton;
    GameObject selectedButton;
    GameObject lowerButton;
    GameObject invisibleButton;

    List<GameObject> mainButtons;
    List<Vector2> wheelPositions;
    bool rotate;
    public float rotateTime;
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<FadeTo>().finishedDecreasing.AddListener(SubscribeToEvents);

        //main
        mainButtons = new List<GameObject>();
        mainButtons.Add(versusButton);
        mainButtons.Add(onlineButton);
        mainButtons.Add(tutorialButton);
        mainButtons.Add(optionsButton);

        wheelPositions = new List<Vector2>();
        wheelPositions.Add(fadeUpButtonPosition);
        wheelPositions.Add(upperButtonPosition);
        wheelPositions.Add(selectedButtonPosition);
        wheelPositions.Add(lowerButtonPosition);
        wheelPositions.Add(fadeDownButtonPosition);

        RepositionMainButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
            RotateWheelUpdater();
    }

    void JudgeO() 
    { 
        switch (state)
        {
            case MainMenuState.MAIN:
                PressSelectedButton();
                break;
        }
    }

    void JudgeK()
    {
        switch (state)
        {
            case MainMenuState.MAIN:
                TransitionToPreviousScene();
                break;
        }
    }

    void JudgeW()
    {
        switch (state)
        {
            case MainMenuState.MAIN:
                RotateDown();
                break;
        }
    }

    void JudgeS()
    {
        switch (state)
        {
            case MainMenuState.MAIN:
                RotateUp();
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

    void SubscribeToEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.AddListener(JudgeO);
        FindObjectOfType<Controls>().keyboard_k_down.AddListener(JudgeK);

        FindObjectOfType<Controls>().keyboard_w_down.AddListener(JudgeW);
        FindObjectOfType<Controls>().keyboard_w.AddListener(JudgeW);
        FindObjectOfType<Controls>().keyboard_s_down.AddListener(JudgeS);
        FindObjectOfType<Controls>().keyboard_s.AddListener(JudgeS);
    }

    void UnsubscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.RemoveListener(JudgeO);
        FindObjectOfType<Controls>().keyboard_k_down.RemoveListener(JudgeK);

        FindObjectOfType<Controls>().keyboard_w_down.RemoveListener(JudgeW);
        FindObjectOfType<Controls>().keyboard_w.RemoveListener(JudgeW);
        FindObjectOfType<Controls>().keyboard_s_down.RemoveListener(JudgeS);
        FindObjectOfType<Controls>().keyboard_s.RemoveListener(JudgeS);
    }

    //main

    void RepositionMainButtons()
    {
        versusButton.transform.position = selectedButtonPosition;
        onlineButton.transform.position = lowerButtonPosition;
        tutorialButton.transform.position = fadeDownButtonPosition;
        optionsButton.transform.position = upperButtonPosition;

        selectedButton = versusButton;
        upperButton = optionsButton;
        lowerButton = onlineButton;
        invisibleButton = tutorialButton;

        UpdateInfo();
    }

    void UpdateWheel()
    {
        selectedButton.transform.position = selectedButtonPosition;
        lowerButton.transform.position = lowerButtonPosition;
        upperButton.transform.position = upperButtonPosition;
        invisibleButton.transform.position = fadeDownButtonPosition;
    }

    void RotateUp()
    {
        GameObject oldInvisibleButton = invisibleButton;

        invisibleButton = upperButton;
        upperButton.GetComponent<MainMenuButton>().goal = fadeUpButtonPosition;
        upperButton = selectedButton;
        selectedButton.GetComponent<MainMenuButton>().goal = upperButtonPosition;
        selectedButton = lowerButton;
        lowerButton.GetComponent<MainMenuButton>().goal = selectedButtonPosition;
        lowerButton = oldInvisibleButton;
        oldInvisibleButton.transform.position = fadeDownButtonPosition;
        oldInvisibleButton.GetComponent<MainMenuButton>().goal = lowerButtonPosition;

        RotateWheelSetup();
    }

    void RotateDown()
    {
        GameObject oldInvisibleButton = invisibleButton;

        invisibleButton = lowerButton;
        lowerButton.GetComponent<MainMenuButton>().goal = fadeDownButtonPosition;
        lowerButton = selectedButton;
        selectedButton.GetComponent<MainMenuButton>().goal = lowerButtonPosition;
        selectedButton = upperButton;
        upperButton.GetComponent<MainMenuButton>().goal = selectedButtonPosition;
        upperButton = oldInvisibleButton;
        oldInvisibleButton.transform.position = fadeUpButtonPosition;
        oldInvisibleButton.GetComponent<MainMenuButton>().goal = upperButtonPosition;

        RotateWheelSetup();
    }

    void RotateWheelSetup()
    {
        UnsubscribeFromEvents();
        HideInfo();
        rotate = true;
    }

    void RotateWheelUpdater()
    {
        rotate = !RotateWheel();

        if (!rotate)
        {
            SubscribeToEvents();
            UpdateInfo();
        }
    }

    bool RotateWheel()
    {
        timer += Time.deltaTime;
        bool[] checkers = new bool[4];

        if (timer >= rotateTime)
        {
            timer = 0.0f;
            int index = 0;

            foreach (GameObject button in mainButtons)
            {
                Vector2 currentPosition = button.transform.position;

                if (currentPosition != button.GetComponent<MainMenuButton>().goal)
                {
                    Vector2 direction = button.GetComponent<MainMenuButton>().goal - currentPosition;

                    if (direction.magnitude <= 0.2f)
                        button.transform.position = button.GetComponent<MainMenuButton>().goal;
                    else
                    {
                        direction = direction.normalized * 0.15f;
                        button.transform.position += new Vector3(direction.x, direction.y, 0);
                    }
                }
                else
                {
                    checkers[index++] = true;
                }
            }
        }

        if (checkers[0] && checkers[1] && checkers[2] && checkers[3])
            return true;
        else
            return false;
    }

    void UpdateInfo()
    {
        foreach (GameObject button in mainButtons)
        {
            button.GetComponent<MainMenuButton>().EnableInfo(button == selectedButton);
        }
    }

    void HideInfo()
    {
        foreach (GameObject button in mainButtons)
        {
            button.GetComponent<MainMenuButton>().EnableInfo(false);
        }
    }

    void PressSelectedButton()
    {
        switch (selectedButton.name)
        {
            case "Versus":
                TransitionToNextScene();
                break;
        }
    }
}
