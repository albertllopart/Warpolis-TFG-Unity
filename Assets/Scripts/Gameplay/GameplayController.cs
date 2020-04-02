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

    public enum PlayerLocation
    {
        LEFT, RIGHT
    };

    public enum Turn
    {
        CANI, HIPSTER
    };

    public PlayerState playerState;
    public PlayerLocation playerLocation;
    private Turn turn;

    //controllers
    public GameObject UIController;
    public GameObject unitsController;

    //events
    //ui
    public UnityEvent enableMenuOptions;
    public UnityEvent disableMenuOptions;
    public UnityEvent enableMenuShop;
    public UnityEvent disableMenuShop;
    public UnityEvent disableMenuUnit;
    public UnityEvent showMenuUnit;
    public UnityEvent hideMenuUnit;
    public UnityEvent cancelMenuUnit;
    public UnityEvent enableMoneyInfo;
    public UnityEvent disableMoneyInfo;

    //unit
    public UnityEvent deselectUnit;
    public UnityEvent moveUnit;
    public UnityEvent attackUnit;

    //gameplay
    public UnityEvent endTurnCani;
    public UnityEvent endTurnHipster;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        SetControllers();

        //events
        //ui
        enableMenuOptions = new UnityEvent();
        disableMenuOptions = new UnityEvent();
        enableMenuShop = new UnityEvent();
        disableMenuShop = new UnityEvent();
        disableMenuUnit = new UnityEvent();
        showMenuUnit = new UnityEvent();
        hideMenuUnit = new UnityEvent();
        cancelMenuUnit = new UnityEvent();
        enableMoneyInfo = new UnityEvent();
        disableMoneyInfo = new UnityEvent();

        //unit
        deselectUnit = new UnityEvent();
        moveUnit = new UnityEvent();
        attackUnit = new UnityEvent();

        //gameplay
        endTurnCani = new UnityEvent();
        endTurnHipster = new UnityEvent();

        playerState = PlayerState.NAVIGATING;
        turn = Turn.CANI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MyOnEnable()
    {
        SubscribeToEvents();
        EnablePlayer();
        UIController.GetComponent<UIController>().EnableTileInfo();
        playerState = PlayerState.NAVIGATING;
    }

    public void MyOnDisable()
    {
        DisablePlayer();
        UnsubscribeFromEvents();
    }

    public void ResetParameters()
    {
        turn = Turn.CANI;
        //playerState = PlayerState.NAVIGATING; de moment no cal però si s'ha de fer a algun lloc és aquí
    }

    void SetControllers()
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
        DisableMenuOptions();

        switch (turn)
        {
            case Turn.CANI:
                //posar en idle els canis
                turn = Turn.HIPSTER;
                endTurnCani.Invoke();
                break;

            case Turn.HIPSTER:
                //posar en idle els hipsters
                turn = Turn.CANI;
                endTurnHipster.Invoke();         
                break;
        }
       
    }

    void SubscribeToEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_o_down.AddListener(JudgeO);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_k_down.AddListener(JudgeK);
    }

    void UnsubscribeFromEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_o_down.RemoveListener(JudgeO);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_k_down.RemoveListener(JudgeK);
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
                        DisableMoneyInfo();
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
                EnableMoneyInfo();

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

                transform.Find("Player").gameObject.SetActive(true); //alerta, codi brut però necessito activar el player perquè cancelMenuUnit necessita la seva posició
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
        enableMenuOptions.Invoke();
    }

    void DisableMenuOptions()
    {
        disableMenuOptions.Invoke();
    }

    void EnableMenuShop()
    {
        enableMenuShop.Invoke();
    }

    void DisableMenuShop()
    {
        disableMenuShop.Invoke();
    }

    //TODO fer que totes aquestes funcions es cridin des del UI Controller
    public void DisableMenuUnit()
    {
        EnablePlayer();
        transform.Find("Player").GetComponent<PlayerController>().selectedUnit = null;

        disableMenuUnit.Invoke();

        playerState = PlayerState.NAVIGATING;
    }

    public void ShowMenuUnit() // as opposed to hide
    {
        showMenuUnit.Invoke();

        playerState = PlayerState.WAITING;
    }

    public void HideMenuUnit()
    {
        //per quan la unitat hagi d'atacar
        hideMenuUnit.Invoke();

        playerState = PlayerState.TARGETING;
    }

    void CancelMenuUnit()
    {
        cancelMenuUnit.Invoke();
    }

    void EnableMoneyInfo()
    {
        enableMoneyInfo.Invoke();
    }

    void DisableMoneyInfo()
    {
        disableMoneyInfo.Invoke();
    }
}
