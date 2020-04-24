﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuMode : MonoBehaviour
{
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

    //events
    public UnityEvent versusPressed;

    // Start is called before the first frame update
    void Awake()
    {
        versusPressed = new UnityEvent();

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

    public void RotateUp()
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

    public void RotateDown()
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
        GetComponentInParent<MainMenuController>().UnsubscribeFromEvents();
        HideInfo();
        rotate = true;

        FindObjectOfType<SoundController>().PlayPlayerMove();
    }

    void RotateWheelUpdater()
    {
        rotate = !RotateWheel();

        if (!rotate)
        {
            GetComponentInParent<MainMenuController>().SubscribeToEvents();
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

    public void PressSelectedButton()
    {
        switch (selectedButton.name)
        {
            case "Versus":
                versusPressed.Invoke();
                break;
        }

        FindObjectOfType<SoundController>().PlayButton();
    }
}
