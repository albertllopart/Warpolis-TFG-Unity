using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuMode : MonoBehaviour
{
    public GameObject versusButton;
    public GameObject quitButton;
    public GameObject tutorialButton;
    public GameObject optionsButton;
    public GameObject battleButton;

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
    List<GameObject> waitingButtons;
    List<Vector2> wheelPositions;
    bool rotate;
    public float rotateTime;
    float timer = 0.0f;

    //events
    public UnityEvent versusPressed;
    public UnityEvent quitPressed;
    public UnityEvent battlePressed;

    // Start is called before the first frame update
    void Awake()
    {
        versusPressed = new UnityEvent();
        quitPressed = new UnityEvent();
        battlePressed = new UnityEvent();

        mainButtons = new List<GameObject>();
        mainButtons.Add(versusButton);
        mainButtons.Add(tutorialButton);
        mainButtons.Add(battleButton);

        waitingButtons = new List<GameObject>();
        waitingButtons.Add(quitButton);
        waitingButtons.Add(optionsButton);

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
        tutorialButton.transform.position = lowerButtonPosition;
        battleButton.transform.position = upperButtonPosition;

        foreach (GameObject button in waitingButtons)
        {
            button.transform.position = fadeDownButtonPosition;
        }

        selectedButton = versusButton;
        upperButton = battleButton;
        lowerButton = tutorialButton;

        UpdateInfo();
    }

    void UpdateWheel()
    {
        selectedButton.transform.position = selectedButtonPosition;
        lowerButton.transform.position = lowerButtonPosition;
        upperButton.transform.position = upperButtonPosition;
        invisibleButton.transform.position = fadeDownButtonPosition;
    }

    GameObject PopFirstWaitingButton()
    {
        GameObject ret = null;
        ret = waitingButtons[0];
        waitingButtons.Remove(ret);

        return ret;
    }

    GameObject PopLastWaitingButton()
    {
        GameObject ret = null;
        ret = waitingButtons[waitingButtons.Count - 1];
        waitingButtons.Remove(ret);

        return ret;
    }

    void AddFirstWaitingButton(GameObject button)
    {
        waitingButtons.Insert(0, button);
    }

    void AddLastWaitingButton(GameObject button)
    {
        waitingButtons.Add(button);
    }

    public void RotateUp()
    {
        GameObject oldInvisibleButton = PopFirstWaitingButton();
        mainButtons.Add(oldInvisibleButton);

        invisibleButton = upperButton;
        upperButton.GetComponent<MainMenuButton>().goal = fadeUpButtonPosition;
        upperButton = selectedButton;
        selectedButton.GetComponent<MainMenuButton>().goal = upperButtonPosition;
        selectedButton = lowerButton;
        lowerButton.GetComponent<MainMenuButton>().goal = selectedButtonPosition;
        lowerButton = oldInvisibleButton;
        oldInvisibleButton.transform.position = fadeDownButtonPosition;
        oldInvisibleButton.GetComponent<MainMenuButton>().goal = lowerButtonPosition;

        AddLastWaitingButton(invisibleButton);

        RotateWheelSetup();
    }

    public void RotateDown()
    {
        GameObject oldInvisibleButton = PopLastWaitingButton();
        mainButtons.Add(oldInvisibleButton);

        invisibleButton = lowerButton;
        lowerButton.GetComponent<MainMenuButton>().goal = fadeDownButtonPosition;
        lowerButton = selectedButton;
        selectedButton.GetComponent<MainMenuButton>().goal = lowerButtonPosition;
        selectedButton = upperButton;
        upperButton.GetComponent<MainMenuButton>().goal = selectedButtonPosition;
        upperButton = oldInvisibleButton;
        oldInvisibleButton.transform.position = fadeUpButtonPosition;
        oldInvisibleButton.GetComponent<MainMenuButton>().goal = upperButtonPosition;

        AddFirstWaitingButton(invisibleButton);

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
            mainButtons.Remove(invisibleButton);
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

            case "Battle":
                battlePressed.Invoke();
                break;

            case "Quit":
                quitPressed.Invoke();
                break;
        }

        FindObjectOfType<SoundController>().PlayButton();
    }
}

