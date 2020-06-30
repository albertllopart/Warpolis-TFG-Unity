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
    public GameObject buttonCaniGunner;
    public GameObject buttonCaniRanged;

    public List<GameObject> caniButtons;

    [Header("Prefabs")]
    public GameObject caniInfantry;
    public GameObject caniTransport;
    public GameObject caniTank;
    public GameObject caniAerial;
    public GameObject caniGunner;
    public GameObject caniRanged;

    [Header("Buttons")]
    [Header("Hipster")]
    public GameObject buttonHipsterInfantry;
    public GameObject buttonHipsterTransport;
    public GameObject buttonHipsterTank;
    public GameObject buttonHipsterAerial;
    public GameObject buttonHipsterGunner;
    public GameObject buttonHipsterRanged;

    public List<GameObject> hipsterButtons;

    [Header("Prefabs")]
    public GameObject hipsterInfantry;
    public GameObject hipsterTransport;
    public GameObject hipsterTank;
    public GameObject hipsterAerial;
    public GameObject hipsterGunner;
    public GameObject hipsterRanged;

    private uint xOffset = 2;
    private float yOffset = 2.5f;

    [Header("Selected")]
    public GameObject selectedButton;

    [Header("UnitInfo")]
    public GameObject unitInfo;
    GameObject currentUnitInfo;
    public GameObject unitDescription;
    GameObject currentUnitDescription;
    public GameObject toggleRight;
    public GameObject toggleLeft;
    GameObject toggle;

    //events
    public UnityEvent caniUnitCreated;
    public UnityEvent hipsterUnitCreated;
    public UnityEvent unavailableButtonPressed;

    // Start is called before the first frame update
    void Start()
    {
        caniUnitCreated = new UnityEvent();
        hipsterUnitCreated = new UnityEvent();
        unavailableButtonPressed = new UnityEvent();
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

        DestroyUnitInfo();

        //cridar tots els MyOnDisable dels fills que ho necessitin
        transform.Find("Cursor_shop").GetComponent<CursorShop>().MyOnDisable();
    }

    void AddButtons()
    {
        List<UnitType> allowedUnits = FindObjectOfType<DataTransferer>().allowedUnits;

        if (caniButtons.Count == 0 && hipsterButtons.Count == 0)
        {
            caniButtons = new List<GameObject>();
            hipsterButtons = new List<GameObject>();

            if (allowedUnits.Contains(UnitType.INFANTRY))
            {
                caniButtons.Add(buttonCaniInfantry);
                buttonCaniInfantry.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniInfantry.GetComponent<MyButton>().shopValue);
                hipsterButtons.Add(buttonHipsterInfantry);
                buttonHipsterInfantry.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterInfantry.GetComponent<MyButton>().shopValue);
            }
            if (allowedUnits.Contains(UnitType.TRANSPORT))
            {
                caniButtons.Add(buttonCaniTransport);
                buttonCaniTransport.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniTransport.GetComponent<MyButton>().shopValue);
                hipsterButtons.Add(buttonHipsterTransport);
                buttonHipsterTransport.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterTransport.GetComponent<MyButton>().shopValue);
            }
            if (allowedUnits.Contains(UnitType.TANK))
            {
                caniButtons.Add(buttonCaniTank);
                buttonCaniTank.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniTank.GetComponent<MyButton>().shopValue);
                hipsterButtons.Add(buttonHipsterTank);
                buttonHipsterTank.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterTank.GetComponent<MyButton>().shopValue);
            }
            if (allowedUnits.Contains(UnitType.AERIAL))
            {
                caniButtons.Add(buttonCaniAerial);
                buttonCaniAerial.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniAerial.GetComponent<MyButton>().shopValue);
                hipsterButtons.Add(buttonHipsterAerial);
                buttonHipsterAerial.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterAerial.GetComponent<MyButton>().shopValue);
            }
            if (allowedUnits.Contains(UnitType.GUNNER))
            {
                caniButtons.Add(buttonCaniGunner);
                buttonCaniGunner.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniGunner.GetComponent<MyButton>().shopValue);
                hipsterButtons.Add(buttonHipsterGunner);
                buttonHipsterGunner.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterGunner.GetComponent<MyButton>().shopValue);
            }
            if (allowedUnits.Contains(UnitType.RANGED))
            {
                caniButtons.Add(buttonCaniRanged);
                buttonCaniRanged.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonCaniRanged.GetComponent<MyButton>().shopValue);
                hipsterButtons.Add(buttonHipsterRanged);
                buttonHipsterRanged.transform.Find("Number").GetComponent<Number>().CreateNumber(buttonHipsterRanged.GetComponent<MyButton>().shopValue);
            }
        }

        DeactivateUnusedButtons();
    }

    void DeactivateUnusedButtons()
    {
        //infantry
        if (!caniButtons.Contains(buttonCaniInfantry))
            buttonCaniInfantry.SetActive(false);

        if (!hipsterButtons.Contains(buttonHipsterInfantry))
            buttonHipsterInfantry.SetActive(false);

        //transport
        if (!caniButtons.Contains(buttonCaniTransport))
            buttonCaniTransport.SetActive(false);

        if (!hipsterButtons.Contains(buttonHipsterTransport))
            buttonHipsterTransport.SetActive(false);

        //tank
        if (!caniButtons.Contains(buttonCaniTank))
            buttonCaniTank.SetActive(false);

        if (!hipsterButtons.Contains(buttonHipsterTank))
            buttonHipsterTank.SetActive(false);

        //aerial
        if (!caniButtons.Contains(buttonCaniAerial))
            buttonCaniAerial.SetActive(false);

        if (!hipsterButtons.Contains(buttonHipsterAerial))
            buttonHipsterAerial.SetActive(false);

        //gunner
        if (!caniButtons.Contains(buttonCaniGunner))
            buttonCaniGunner.SetActive(false);

        if (!hipsterButtons.Contains(buttonHipsterGunner))
            buttonHipsterGunner.SetActive(false);

        //ranged
        if (!caniButtons.Contains(buttonCaniRanged))
            buttonCaniRanged.SetActive(false);

        if (!hipsterButtons.Contains(buttonHipsterRanged))
            buttonHipsterRanged.SetActive(false);
    }

    void Collapse()
    {
        float counter = 0.0f;

        foreach (GameObject button in caniButtons)
        {
            button.transform.position += new Vector3(6.0f / 16.0f, -(22.0f / 16.0f) * counter++ -(7.0f / 16.0f), 0); //6 son els pixels de marge respecte el background que s'han d'aplicar tant a x com a y
        }

        counter = 0.0f;

        foreach (GameObject button in hipsterButtons)
        {
            button.transform.position += new Vector3(6.0f / 16.0f, -(22.0f / 16.0f) * counter++ - (7.0f / 16.0f), 0); //6 son els pixels de marge respecte el background que s'han d'aplicar tant a x com a y
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

        UpdateUnitInfo();
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
            else if (selectedButton.name == "Button_cani_gunner")
            {
                Instantiate(caniGunner, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddCaniMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_hipster_gunner")
            {
                Instantiate(hipsterGunner, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddHipsterMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_cani_ranged")
            {
                Instantiate(caniRanged, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddCaniMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }
            else if (selectedButton.name == "Button_hipster_ranged")
            {
                Instantiate(hipsterRanged, gameplayController.transform.Find("Player").transform.position, Quaternion.identity);
                GameObject.Find("Data Controller").GetComponent<DataController>().AddHipsterMoney(-selectedButton.GetComponent<MyButton>().shopValue);
            }

            FindObjectOfType<SoundController>().PlayMoney();

            FindObjectOfType<GameplayController>().disableMenuShop.Invoke();
            FindObjectOfType<GameplayController>().EnablePlayer();
            FindObjectOfType<GameplayController>().playerState = GameplayController.PlayerState.NAVIGATING;
        }
        else
        {
            FindObjectOfType<SoundController>().PlayUnavailable();
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

    void UpdateUnitInfo()
    {
        if (currentUnitInfo != null)
        {
            InstantiateUnitInfo();
            Destroy(currentUnitDescription);
        }
        else if (currentUnitDescription != null)
        {
            InstantiateUnitDescription();
            Destroy(currentUnitInfo);
        }
        else
        {
            InstantiateUnitInfo();
        }
    }

    void DestroyUnitInfo()
    {
        Destroy(currentUnitInfo);
        Destroy(currentUnitDescription);
    }

    void InstantiateUnitInfo()
    {
        if (currentUnitInfo != null)
            Destroy(currentUnitInfo);

        currentUnitInfo = Instantiate(unitInfo);
        currentUnitInfo.transform.SetParent(transform.Find("UnitInfo").transform);
        currentUnitInfo.transform.position = Camera.main.transform.position + new Vector3(5, 0, 10);

        currentUnitInfo.GetComponent<UnitInfo>().BuildInfo(GetUnitTypeFromSelectedButton());

        InstantiateToggleRight();

        toggle.transform.position = currentUnitInfo.transform.position + new Vector3(-5, 4.5f, 0);
    }

    void InstantiateUnitDescription()
    {
        if (currentUnitDescription != null)
            Destroy(currentUnitDescription);

        currentUnitDescription = Instantiate(unitDescription);
        currentUnitDescription.transform.SetParent(transform.Find("UnitDescription").transform);
        currentUnitDescription.transform.position = Camera.main.transform.position + new Vector3(5, 0, 10);

        currentUnitDescription.GetComponent<UnitDescription>().BuildDescription(GetUnitTypeFromSelectedButton());

        InstantiateToggleLeft();

        toggle.transform.position = currentUnitDescription.transform.position + new Vector3(-5, 4.5f, 0);
    }

    void ToggleUnitInfo()
    {
        if (currentUnitInfo == null)
        {
            Destroy(currentUnitDescription);

            InstantiateUnitInfo();
        }
    }

    void ToggleUnitDescription()
    {
        if (currentUnitDescription == null)
        {
            Destroy(currentUnitInfo);

            InstantiateUnitDescription();
        }
    }

    void InstantiateToggleRight()
    {
        Destroy(toggle);
        toggle = Instantiate(toggleRight);
        toggle.transform.SetParent(transform.Find("Toggle"));
    }

    void InstantiateToggleLeft()
    {
        Destroy(toggle);
        toggle = Instantiate(toggleLeft);
        toggle.transform.SetParent(transform.Find("Toggle"));
    }

    UnitType GetUnitTypeFromSelectedButton()
    {
        if (selectedButton.name == "Button_cani_infantry")
        {
            return UnitType.INFANTRY;
        }
        else if (selectedButton.name == "Button_hipster_infantry")
        {
            return UnitType.INFANTRY;
        }
        else if (selectedButton.name == "Button_cani_transport")
        {
            return UnitType.TRANSPORT;
        }
        else if (selectedButton.name == "Button_hipster_transport")
        {
            return UnitType.TRANSPORT;
        }
        else if (selectedButton.name == "Button_cani_tank")
        {
            return UnitType.TANK;
        }
        else if (selectedButton.name == "Button_hipster_tank")
        {
            return UnitType.TANK;
        }
        else if (selectedButton.name == "Button_cani_aerial")
        {
            return UnitType.AERIAL;
        }
        else if (selectedButton.name == "Button_hipster_aerial")
        {
            return UnitType.AERIAL;
        }
        else if (selectedButton.name == "Button_cani_gunner")
        {
            return UnitType.GUNNER;
        }
        else if (selectedButton.name == "Button_hipster_gunner")
        {
            return UnitType.GUNNER;
        }
        else if (selectedButton.name == "Button_cani_ranged")
        {
            return UnitType.RANGED;
        }
        else if (selectedButton.name == "Button_hipster_ranged")
        {
            return UnitType.RANGED;
        }

        return UnitType.INFANTRY;
    }

    void SubscribeToEvents()
    {
        transform.Find("Cursor_shop").GetComponent<CursorShop>().sendO.AddListener(PressSelectedButton);
        transform.Find("Cursor_shop").GetComponent<CursorShop>().toggleUnitDescription.AddListener(ToggleUnitDescription);
        transform.Find("Cursor_shop").GetComponent<CursorShop>().toggleUnitInfo.AddListener(ToggleUnitInfo);
    }

    void UnsubscribeFromEvents()
    {
        transform.Find("Cursor_shop").GetComponent<CursorShop>().sendO.RemoveListener(PressSelectedButton);
        transform.Find("Cursor_shop").GetComponent<CursorShop>().toggleUnitDescription.RemoveListener(ToggleUnitDescription);
        transform.Find("Cursor_shop").GetComponent<CursorShop>().toggleUnitInfo.RemoveListener(ToggleUnitInfo);
    }
}
