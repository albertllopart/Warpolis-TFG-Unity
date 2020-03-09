using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuShopController : MonoBehaviour
{
    [Header("Buttons")]
    [Header("Cani")]
    public GameObject buttonCaniInfantry;
    public List<GameObject> caniButtons;

    [Header("Prefabs")]
    public GameObject caniInfantry;

    [Header("Buttons")]
    [Header("Hipster")]
    public GameObject buttonHipsterInfantry;
    public List<GameObject> hipsterButtons;

    [Header("Prefabs")]
    public GameObject hipsterInfantry;

    private uint xOffset = 2;
    private float yOffset = 2.5f;

    [Header("Selected")]
    public GameObject selectedButton;

    // Start is called before the first frame update
    void Start()
    {
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
        //actualitzar la posició segons la càmera
        Vector2 cameraTopLeft = Camera.main.GetComponent<CameraController>().GetTopLeftCorner();
        transform.position = new Vector3(cameraTopLeft.x + xOffset, cameraTopLeft.y - yOffset, 0);

        //seleccionar primer botó de la llista
        SelectButton(0);

        //cridar tots els MyOnEnable dels fills que ho necessitin
        transform.Find("Cursor_shop").GetComponent<CursorShop>().MyOnEnable();

        //desactivar els botons de l'exèrcit que no mou
        DisableInactiveButtons();

        SubscribeToEvents();
    }

    public void MyOnDisable()
    {
        UnsubscribeFromEvents();

        //cridar tots els MyOnDisable dels fills que ho necessitin
        transform.Find("Cursor_shop").GetComponent<CursorShop>().MyOnDisable();
    }

    void AddButtons()
    {
        if (caniButtons.Count == 0 && hipsterButtons.Count == 0)
        {
            caniButtons = new List<GameObject>();
            caniButtons.Add(buttonCaniInfantry);

            hipsterButtons = new List<GameObject>();
            hipsterButtons.Add(buttonHipsterInfantry);
        }
    }

    void Collapse()
    {
        float counter = 0.0f;

        foreach (GameObject button in caniButtons)
        {
            button.transform.position += new Vector3(6.0f / 16.0f, -(22.0f / 16.0f) * counter++ -(6.0f / 16.0f), 0); //6 son els pixels de marge respecte el background que s'han d'aplicar tant a x com a y
        }

        counter = 0.0f;

        foreach (GameObject button in hipsterButtons)
        {
            button.transform.position += new Vector3(6.0f / 16.0f, -(22.0f / 16.0f) * counter++ - (6.0f / 16.0f), 0); //6 son els pixels de marge respecte el background que s'han d'aplicar tant a x com a y
        }
    }

    public void SelectButton(int index)
    {
        if (selectedButton != null)
            LessenSelected();

        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn() == GameplayController.Turn.CANI)
            selectedButton = caniButtons[index];
        else
            selectedButton = hipsterButtons[index];

        HighlightSelected();
    }

    void HighlightSelected()
    {
        selectedButton.GetComponent<MyButton>().OnHighlight();
    }

    void LessenSelected()
    {
        selectedButton.GetComponent<MyButton>().OnIdle();
    }

    public List<GameObject> GetButtonList()
    {
        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn() == GameplayController.Turn.CANI)
            return caniButtons;
        else
            return hipsterButtons;
    }

    public GameObject GetSelectedButton()
    {
        return selectedButton;
    }

    public void PressSelectedButton()
    {
        GameObject gameplayController = GameObject.Find("Gameplay Controller");

        if (selectedButton.name == "Button_cani_infantry")
        {
            Instantiate(caniInfantry, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
        }
        else if (selectedButton.name == "Button_hipster_infantry")
        {
            Instantiate(hipsterInfantry, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
        }

        gameplayController.GetComponent<Controls>().keyboard_k_down.Invoke();
    }

    void SubscribeToEvents()
    {
        transform.Find("Cursor_shop").GetComponent<CursorShop>().sendO.AddListener(PressSelectedButton);
    }

    void UnsubscribeFromEvents()
    {
        transform.Find("Cursor_shop").GetComponent<CursorShop>().sendO.RemoveListener(PressSelectedButton);
    }

    void DisableInactiveButtons()
    {
        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn() == GameplayController.Turn.CANI)
        {
            foreach (GameObject button in hipsterButtons)
            {
                button.SetActive(false);
            }
            foreach (GameObject button in caniButtons)
            {
                button.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject button in caniButtons)
            {
                button.SetActive(false);
            }
            foreach (GameObject button in hipsterButtons)
            {
                button.SetActive(true);
            }
        }
    }
}
