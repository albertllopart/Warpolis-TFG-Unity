using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuUnitController : MonoBehaviour
{
    public GameObject buttonWait;
    public GameObject buttonAttack;
    public GameObject buttonCapture;
    public GameObject buttonLoad;
    public GameObject buttonDropCani;
    public GameObject buttonDropHipster;

    public List<GameObject> buttons; // a diferència del menú d'opcions on sempre hi ha tots els botons actius, aquí només hi haurà els que estiguin disponibles segons la unitat concreta

    private uint xOffset = 2;
    private uint yOffset = 1;

    public GameObject selectedButton;

    //unitat
    public GameObject selectedUnit;

    //events
    public UnityEvent buttonPressed;

    // Start is called before the first frame update
    void Start()
    {
        buttonPressed = new UnityEvent();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MyOnEnable(GameObject unit)
    {
        //actualitzar la posició segons la càmera i el player
        Vector2 cameraTopLeft = Camera.main.GetComponent<CameraController>().GetTopLeftCorner();
        transform.position = new Vector3(cameraTopLeft.x + xOffset, cameraTopLeft.y - yOffset, 0);

        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().playerLocation == GameplayController.PlayerLocation.LEFT)
        {
            transform.position += new Vector3(13, 0, 0); // magic number :(
        }

        //afegir botons actius i posar-los en ordre en collapse
        selectedUnit = unit;
        AddButtons(selectedUnit.GetComponent<Unit>().GetActiveButtons());

        //seleccionar primer botó de la llista
        SelectButton(0);

        //cridar tots els MyOnEnable dels fills que ho necessitin
        transform.Find("Cursor_unit").GetComponent<CursorUnit>().MyOnEnable();

        SubscribeToEvents();
    }

    public void MyOnDisable()
    {
        selectedUnit = null;
        selectedButton = null;

        UnsubscribeFromEvents();

        //cridar tots els MyOnDisable dels fills que ho necessitin
        transform.Find("Cursor_unit").GetComponent<CursorUnit>().MyOnDisable();
    }

    public void MyOnCancel()
    {
        selectedUnit.GetComponent<Unit>().OnCancelMovement();
        selectedButton = null;

        UnsubscribeFromEvents();

        //cridar tots els MyOnDisable dels fills que ho necessitin
        transform.Find("Cursor_unit").GetComponent<CursorUnit>().MyOnDisable();
    }

    public void MyOnHide()
    {
        selectedButton = null;

        UnsubscribeFromEvents();

        //cridar tots els MyOnDisable dels fills que ho necessitin
        transform.Find("Cursor_unit").GetComponent<CursorUnit>().MyOnDisable();
    }

    void AddButtons(bool[] activeButtons)
    {
        buttons = new List<GameObject>();
        float counter = 0.0f;

        if (activeButtons[0]) // capture
        {
            buttonCapture.SetActive(true);
            buttons.Add(buttonCapture);
            buttonCapture.GetComponent<MyButton>().button = new MyButtonOptions(); // TODO

            buttonCapture.transform.localPosition = new Vector3(0, 0, 0);
            buttonCapture.transform.position += new Vector3(0, -(28.0f / 16.0f) * counter++, 0);
        }
        else
        {
            buttonCapture.SetActive(false);
        }

        if (activeButtons[1]) // attack
        {
            buttonAttack.SetActive(true);
            buttons.Add(buttonAttack);
            buttonAttack.GetComponent<MyButton>().button = new MyButtonQuit(); // TODO

            buttonAttack.transform.localPosition = new Vector3(0, 0, 0);
            buttonAttack.transform.position += new Vector3(0, -(28.0f / 16.0f) * counter++, 0);
        }
        else
        {
            buttonAttack.SetActive(false);
        }

        if (activeButtons[4]) // drop
        {
            GameplayController.Turn turn = GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn();

            switch (turn)
            {
                case GameplayController.Turn.CANI:
                    buttonDropCani.SetActive(true);
                    buttons.Add(buttonDropCani);

                    buttonDropCani.transform.localPosition = new Vector3(0, 0, 0);
                    buttonDropCani.transform.position += new Vector3(0, -(28.0f / 16.0f) * counter++, 0);

                    buttonDropHipster.SetActive(false);
                    break;

                case GameplayController.Turn.HIPSTER:
                    buttonDropHipster.SetActive(true);
                    buttons.Add(buttonDropHipster);

                    buttonDropHipster.transform.localPosition = new Vector3(0, 0, 0);
                    buttonDropHipster.transform.position += new Vector3(0, -(28.0f / 16.0f) * counter++, 0);

                    buttonDropCani.SetActive(false);
                    break;
            }
        }
        else
        {
            buttonDropCani.SetActive(false);
            buttonDropHipster.SetActive(false);
        }

        if (activeButtons[2]) // wait
        {
            buttonWait.SetActive(true);
            buttons.Add(buttonWait);
            buttonWait.GetComponent<MyButton>().button = new MyButtonWait(); // TODO

            buttonWait.transform.localPosition = new Vector3(0, 0, 0);
            buttonWait.transform.position += new Vector3(0, -(28.0f / 16.0f) * counter++, 0);
        }
        else
        {
            buttonWait.SetActive(false);
        }

        if (activeButtons[3]) // load
        {
            buttonLoad.SetActive(true);
            buttons.Add(buttonLoad);

            buttonLoad.transform.localPosition = new Vector3(0, 0, 0);
            buttonLoad.transform.position += new Vector3(0, -(28.0f / 16.0f) * counter++, 0);
        }
        else
        {
            buttonLoad.SetActive(false);
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
        if (selectedButton.name == "Button_wait")
        {
            selectedUnit.GetComponent<Unit>().OnWait();
        }
        else if (selectedButton.name == "Button_attack")
        {
            selectedUnit.GetComponent<Unit>().OnTargeting();
            GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().HideMenuUnit();
        }
        else if (selectedButton.name == "Button_capture")
        {
            selectedUnit.GetComponent<UnitInfantry>().OnCapture();
        }
        else if (selectedButton.name == "Button_load")
        {
            selectedUnit.GetComponent<UnitInfantry>().OnLoad();
        }
        else if (selectedButton.name.Contains("Button_drop"))
        {
            selectedUnit.GetComponent<UnitTransport>().OnTargetingDropPositions();
            GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().HideMenuUnit();
        }

        FindObjectOfType<SoundController>().PlayButton();
    }

    void UpdateTileInfo()
    {
        //actualitzar tileinfo
        transform.parent.transform.Find("Tile_info").GetComponent<TileInfo>().CheckBuilding(GameObject.Find("Player").transform.position);
    }

    void SubscribeToEvents()
    {
        transform.Find("Cursor_unit").GetComponent<CursorUnit>().sendO.AddListener(PressSelectedButton);
    }

    void UnsubscribeFromEvents()
    {
        transform.Find("Cursor_unit").GetComponent<CursorUnit>().sendO.RemoveListener(PressSelectedButton);
    }
}