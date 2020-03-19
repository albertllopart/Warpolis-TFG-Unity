using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayController : MonoBehaviour
{
    public enum PlayerState
    {
        NAVIGATING, OPTIONS, INTERACTING, SHOP, WAITING, TARGETING
    };

    public enum Turn
    {
        CANI, HIPSTER
    };

    public PlayerState playerState;
    private Turn turn;

    //controllers
    public GameObject UIController;
    public GameObject unitsController;

    //events
    public UnityEvent openMenuOptions;
    public UnityEvent closeMenuOptions;
    public UnityEvent deselectUnit;
    public UnityEvent moveUnit;
    public UnityEvent attackUnit;
    public UnityEvent endTurnCani;
    public UnityEvent endTurnHipster;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        GetControllers();

        openMenuOptions = new UnityEvent();
        closeMenuOptions = new UnityEvent();
        deselectUnit = new UnityEvent();
        moveUnit = new UnityEvent();
        attackUnit = new UnityEvent();
        endTurnCani = new UnityEvent();
        endTurnHipster = new UnityEvent();

        playerState = PlayerState.NAVIGATING;
        turn = Turn.CANI;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetControllers()
    {
        UIController = GameObject.Find("UI Controller");
        unitsController = GameObject.Find("Units Controller");
    }

    public Turn GetTurn()
    {
        return turn;
    }

    public void EndTurn()
    {
        switch (turn)
        {
            case Turn.CANI:
                //posar en idle els canis
                endTurnCani.Invoke();
                turn = Turn.HIPSTER;
                break;

            case Turn.HIPSTER:
                //posar en idle els hipsters
                endTurnHipster.Invoke();
                turn = Turn.CANI;
                break;
        }

        DisableMenuOptions();
        EnablePlayer();

        playerState = PlayerState.NAVIGATING;
    }

    void SubscribeToEvents()
    {
        GetComponentInParent<Controls>().keyboard_o_down.AddListener(JudgeO);
        GetComponentInParent<Controls>().keyboard_k_down.AddListener(JudgeK);
    }

    void JudgeO()
    {
        switch(playerState)
        {
            case PlayerState.NAVIGATING:
                //cridar funcio interact de player i mirar què retorna
                if (transform.Find("Player").GetComponent<PlayerController>().InteractUnits())
                {
                    if (transform.Find("Player").GetComponent<PlayerController>().selectedUnit != null)
                    {
                        playerState = PlayerState.INTERACTING;
                    }
                    else
                    {
                        TransitionToOptionsMenu();
                        playerState = PlayerState.OPTIONS;
                    }

                    break;
                }
                else if(transform.Find("Player").GetComponent<PlayerController>().InteractBuildings())
                {
                    DisablePlayer();
                    EnableMenuShop();
                    playerState = PlayerState.SHOP;

                    break;
                }

                TransitionToOptionsMenu();
                playerState = PlayerState.OPTIONS;

                break;

            case PlayerState.INTERACTING:
                //interactuar amb la unitat
                moveUnit.Invoke();

                break;

            case PlayerState.OPTIONS:
                //interactuar amb el menú
                break;

            case PlayerState.WAITING:
                
                break;

            case PlayerState.TARGETING:

                attackUnit.Invoke();
                DisableMenuUnit();

                playerState = PlayerState.NAVIGATING;

                break;
        }
    }

    void JudgeK()
    {
        switch (playerState)
        {
            case PlayerState.NAVIGATING:
                //interactuar amb la casella
                break;

            case PlayerState.INTERACTING:

                deselectUnit.Invoke();

                //propi
                playerState = PlayerState.NAVIGATING;
                break;

            case PlayerState.OPTIONS:
                
                DisableMenuOptions();
                EnablePlayer();

                //propi
                playerState = PlayerState.NAVIGATING;
                break;

            case PlayerState.SHOP:

                DisableMenuShop();
                EnablePlayer();

                //propi
                playerState = PlayerState.NAVIGATING;

                break;

            case PlayerState.WAITING:

                CancelMenuUnit();
                EnablePlayer();

                //propi
                playerState = PlayerState.INTERACTING;

                break;

            case PlayerState.TARGETING:

                ShowMenuUnit();

                break;
        }
    }

    void TransitionToOptionsMenu()
    {
        DisablePlayer();
        EnableMenuOptions();
    }

    public void EnablePlayer()
    {
        transform.Find("Player").gameObject.SetActive(true);
        transform.Find("Player").GetComponent<PlayerController>().MyOnEnable();
    }

    public void DisablePlayer()
    {
        transform.Find("Player").GetComponent<PlayerController>().MyOnDisable();
        transform.Find("Player").gameObject.SetActive(false);
    }

    void EnableMenuOptions()
    {
        UIController.transform.Find("Menu_options").gameObject.SetActive(true);
        UIController.transform.Find("Menu_options").GetComponent<MenuOptionsController>().MyOnEnable();
    }

    void DisableMenuOptions()
    {
        UIController.transform.Find("Menu_options").GetComponent<MenuOptionsController>().MyOnDisable();
        UIController.transform.Find("Menu_options").gameObject.SetActive(false);
    }

    void EnableMenuShop()
    {
        UIController.transform.Find("Menu_shop").gameObject.SetActive(true);
        UIController.transform.Find("Menu_shop").GetComponent<MenuShopController>().MyOnEnable();
    }

    void DisableMenuShop()
    {
        UIController.transform.Find("Menu_shop").GetComponent<MenuShopController>().MyOnDisable();
        UIController.transform.Find("Menu_shop").gameObject.SetActive(false);
    }

    public void DisableMenuUnit()
    {
        //d'aquest no hi ha enable perquè qui l'activa és la unitat

        MenuUnitController menu = UIController.transform.Find("Menu_unit").GetComponent<MenuUnitController>();
        menu.MyOnDisable();
        menu.gameObject.SetActive(false);

        EnablePlayer();
        transform.Find("Player").GetComponent<PlayerController>().selectedUnit = null;

        playerState = PlayerState.NAVIGATING;
    }

    public void ShowMenuUnit() // as opposed to hide
    {
        GameObject menu = UIController.transform.Find("Menu_unit").gameObject;
        MenuUnitController menuController = menu.GetComponent<MenuUnitController>();

        menu.SetActive(true);
        menuController.selectedUnit.GetComponent<Unit>().OnMenu();
        menuController.selectedUnit.GetComponent<Unit>().UnsubscribeFromEvents();

        playerState = PlayerState.WAITING;
    }

    public void HideMenuUnit()
    {
        //per quan la unitat hagi d'atacar
        UIController.transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnHide();
        UIController.transform.Find("Menu_unit").gameObject.SetActive(false);

        playerState = PlayerState.TARGETING;
    }

    void CancelMenuUnit()
    {
        UIController.transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnCancel();
        UIController.transform.Find("Menu_unit").gameObject.SetActive(false);
    }
}
