using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuShopController : MonoBehaviour
{
    [Header("Buttons")]
    [Header("Cani")]
    public GameObject buttonCaniInfantry;
    public GameObject buttonCaniTransport;
    public GameObject buttonCaniTank;
    public GameObject buttonCaniAerial;
    public List<GameObject> caniButtons;

    [Header("Prefabs")]
    public GameObject caniInfantry;
    public GameObject caniTransport;
    public GameObject caniTank;
    public GameObject caniAerial;

    [Header("Buttons")]
    [Header("Hipster")]
    public GameObject buttonHipsterInfantry;
    public GameObject buttonHipsterTransport;
    public GameObject buttonHipsterTank;
    public GameObject buttonHipsterAerial;
    public List<GameObject> hipsterButtons;

    [Header("Prefabs")]
    public GameObject hipsterInfantry;
    public GameObject hipsterTransport;
    public GameObject hipsterTank;
    public GameObject hipsterAerial;

    private uint xOffset = 2;
    private float yOffset = 2.5f;

    [Header("Selected")]
    public GameObject selectedButton;

    //events
    public UnityEvent caniUnitCreated;
    public UnityEvent hipsterUnitCreated;

    // Start is called before the first frame update
    void Start()
    {
        caniUnitCreated = new UnityEvent();
        hipsterUnitCreated = new UnityEvent();
    }

    public void AfterStart()
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
            buttonCaniInfantry.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniInfantry.GetComponent<MyButton>().shopValue);
            caniButtons.Add(buttonCaniTransport);
            buttonCaniTransport.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniTransport.GetComponent<MyButton>().shopValue);
            caniButtons.Add(buttonCaniTank);
            buttonCaniTank.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniTank.GetComponent<MyButton>().shopValue);
            caniButtons.Add(buttonCaniAerial);
            buttonCaniAerial.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniAerial.GetComponent<MyButton>().shopValue);

            hipsterButtons = new List<GameObject>();
            hipsterButtons.Add(buttonHipsterInfantry);
            buttonHipsterInfantry.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterInfantry.GetComponent<MyButton>().shopValue);
            hipsterButtons.Add(buttonHipsterTransport);
            buttonHipsterTransport.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterTransport.GetComponent<MyButton>().shopValue);
            hipsterButtons.Add(buttonHipsterTank);
            buttonHipsterTank.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterTank.GetComponent<MyButton>().shopValue);
            hipsterButtons.Add(buttonHipsterAerial);
            buttonHipsterAerial.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterAerial.GetComponent<MyButton>().shopValue);
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
        if (selectedButton.GetComponent<MyButton>().isEnabled)
        {
            GameObject gameplayController = GameObject.Find("Gameplay Controller");

            if (selectedButton.name.Contains("cani"))
            {
                caniUnitCreated.Invoke(); //encara ningú escolta això
            }
            else if (selectedButton.name.Contains("hipster"))
            {
                hipsterUnitCreated.Invoke();
            }

            if (selectedButton.name == "Button_cani_infantry")
            {
                Instantiate(caniInfantry, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddCaniMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_hipster_infantry")
            {
                Instantiate(hipsterInfantry, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddHipsterMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_cani_transport")
            {
                Instantiate(caniTransport, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddCaniMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_hipster_transport")
            {
                Instantiate(hipsterTransport, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddHipsterMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_cani_tank")
            {
                Instantiate(caniTank, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddCaniMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_hipster_tank")
            {
                Instantiate(hipsterTank, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddHipsterMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_cani_aerial")
            {
                Instantiate(caniAerial, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddCaniMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_hipster_aerial")
            {
                Instantiate(hipsterAerial, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddHipsterMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }

            GameObject.Find("Controls").GetComponent<Controls>().keyboard_k_down.Invoke();
        }
    }

    void DisableInactiveButtons()
    {
        //aquest mètode desactiva els botons de l'exèrcit que no mou i fa enable o disable en funció del preu
        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn() == GameplayController.Turn.CANI)
        {
            foreach (GameObject button in hipsterButtons)
            {
                button.SetActive(false);
            }
            foreach (GameObject button in caniButtons)
            {
                button.SetActive(true);

                if (GameObject.Find("Data Controller").GetComponent<DataController>().caniMoney >= button.GetComponent<MyButton>().shopValue)
                    button.GetComponent<MyButton>().OnEnabled();
                else
                    button.GetComponent<MyButton>().OnDisabled();
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

                if (GameObject.Find("Data Controller").GetComponent<DataController>().hipsterMoney >= button.GetComponent<MyButton>().shopValue)
                    button.GetComponent<MyButton>().OnEnabled();
                else
                    button.GetComponent<MyButton>().OnDisabled();
            }
        }
    }

    void SubscribeToEvents()
    {
        transform.Find("Cursor_shop").GetComponent<CursorShop>().sendO.AddListener(PressSelectedButton);
    }

    void UnsubscribeFromEvents()
    {
        transform.Find("Cursor_shop").GetComponent<CursorShop>().sendO.RemoveListener(PressSelectedButton);
    }
}
