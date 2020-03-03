using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionsController : MonoBehaviour
{
    public GameObject buttonOptions;
    public GameObject buttonQuit;
    public GameObject buttonEndTurn;

    private List<GameObject> buttons;

    private uint xOffset = 2;
    private uint yOffset = 1;

    private GameObject selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
        AddButtons();
        Collapse();
        SelectButton(buttonOptions);
        OnActivate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnActivate()
    {
        //actualitzar la posició segons la càmera
        Vector2 cameraTopLeft = Camera.main.GetComponent<CameraController>().GetTopLeftCorner();
        transform.position = new Vector3(cameraTopLeft.x + xOffset, cameraTopLeft.y - yOffset, 0);

        //seleccionar primer botó de la llista
        SelectButton(0);
    }

    void AddButtons()
    {
        buttons = new List<GameObject>();

        buttons.Add(buttonOptions);
        buttons.Add(buttonQuit);
        buttons.Add(buttonEndTurn);
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

    void SubscribeToEvents()
    {
        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().OpenMenuOptions.AddListener(OnActivate);
    }
}
