﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuOptionsController : MonoBehaviour
{
    //prefabs
    public GameObject confirmScreen;

    [Header("Buttons")]
    public GameObject buttonResume;
    public GameObject buttonExit;
    public GameObject buttonEndTurn;

    public List<GameObject> buttons;

    private uint xOffset = 2;
    private uint yOffset = 1;

    public GameObject selectedButton;

    //events
    public UnityEvent buttonPressed;

    // Start is called before the first frame update
    void Start()
    {
        buttonPressed = new UnityEvent();

        AddButtons();
        Collapse();

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MyOnEnable()
    {
        //actualitzar la posició segons la càmera i segons el player
        Vector2 cameraTopLeft = Camera.main.GetComponent<CameraController>().GetTopLeftCorner();
        transform.position = new Vector3(cameraTopLeft.x + xOffset, cameraTopLeft.y - yOffset, 0);

        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().playerLocation == GameplayController.PlayerLocation.LEFT)
        {
            transform.position += new Vector3(13, 0, 0); // magic number :(
        }

        //seleccionar primer botó de la llista
        SelectButton(0);

        //cridar tots els MyOnEnable dels fills que ho necessitin
        transform.Find("Cursor_options").GetComponent<CursorOptions>().MyOnEnable();

        SubscribeToEvents();
    }

    public void MyOnDisable()
    {
        UnsubscribeFromEvents();

        //cridar tots els MyOnDisable dels fills que ho necessitin
        transform.Find("Cursor_options").GetComponent<CursorOptions>().MyOnDisable();
    }

    void AddButtons()
    {
        if (buttons.Count == 0)
        {
            buttons = new List<GameObject>();

            buttons.Add(buttonResume);
            buttonResume.GetComponent<MyButton>().button = new MyButtonOptions();

            buttons.Add(buttonExit);
            buttonExit.GetComponent<MyButton>().button = new MyButtonQuit();

            buttons.Add(buttonEndTurn);
            buttonEndTurn.GetComponent<MyButton>().button = new MyButtonEndTurn();
        }
    }

    void Collapse()
    {
        float counter = 0.0f;

        foreach(GameObject button in buttons)
        {
            button.transform.position += new Vector3(0, -(28.0f/16.0f) * counter++, 0);
        }
    }

    public void SelectButton(GameObject button)
    {
        if (selectedButton != null)
            LessenSelected();

        selectedButton = button;
        HighlightSelected();
    }

    public void SelectButton(int index)
    {
        if (selectedButton != null)
            LessenSelected();

        selectedButton = buttons[index];
        HighlightSelected();
    }

    void HighlightSelected()
    {
        selectedButton.transform.position += new Vector3(1, 0, 0);
    }

    void LessenSelected()
    {
        selectedButton.transform.position += new Vector3(-1, 0, 0);
    }

    public List<GameObject> GetButtonList()
    {
        return buttons;
    }

    public GameObject GetSelectedButton()
    {
        return selectedButton;
    }

    public void PressSelectedButton()
    {
        FindObjectOfType<SoundController>().PlayButton();

        if (selectedButton.name == "Button_exit")
        {
            GameObject confirm = Instantiate(confirmScreen);
            confirm.transform.position = Camera.main.transform.position + new Vector3(0, 0, 10);

            FindObjectOfType<UIController>().OnConfirmScreen();
            FindObjectOfType<GameplayController>().playerState = GameplayController.PlayerState.CONFIRM;
        }
        else if (selectedButton.name == "Button_endturn")
        {
            GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().EndTurn(); // TODO: invocar un event del gameplay controller en comptes del mètode directament
        }
        else if (selectedButton.name == "Button_resume")
        {
            FindObjectOfType<Controls>().keyboard_k_down.Invoke();
        }
    }

    void SubscribeToEvents()
    {
        transform.Find("Cursor_options").GetComponent<CursorOptions>().sendO.AddListener(PressSelectedButton);
    }

    void UnsubscribeFromEvents()
    {
        transform.Find("Cursor_options").GetComponent<CursorOptions>().sendO.RemoveListener(PressSelectedButton);
    }
}
