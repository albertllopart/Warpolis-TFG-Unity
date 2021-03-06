﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayController : MonoBehaviour
{
    public enum PlayerState
    {
        NAVIGATING, OPTIONS, INTERACTING, ATTACKRANGE, SHOP, WAITING, TARGETING, DROPPING, CONFIRM, UNITINFO
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
    public UnityEvent destroyUnitInfo;

    //unit
    public UnityEvent deselectUnit;
    public UnityEvent deselectUnitAttackRange;
    public UnityEvent moveUnit;
    public UnityEvent attackUnit;
    public UnityEvent dropUnit;

    //gameplay
    public UnityEvent endTurnCani;
    public UnityEvent endTurnHipster;

    void Awake()
    {
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
        destroyUnitInfo = new UnityEvent();

        //unit
        deselectUnit = new UnityEvent();
        deselectUnitAttackRange = new UnityEvent();
        moveUnit = new UnityEvent();
        attackUnit = new UnityEvent();
        dropUnit = new UnityEvent();

        //gameplay
        endTurnCani = new UnityEvent();
        endTurnHipster = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        SetControllers();

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

        UnsubscribeFromEvents();
    }

    void JudgeO()
    {
        switch(playerState)
        {
            case PlayerState.NAVIGATING:

                FindObjectOfType<SoundController>().PlayButton();

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
                FindObjectOfType<SoundController>().PlayButton();

                moveUnit.Invoke();

                break;

            case PlayerState.OPTIONS:
                //interactuar amb el menú
                break;

            case PlayerState.WAITING:
                
                break;

            case PlayerState.TARGETING:

                FindObjectOfType<SoundController>().PlayButton();

                attackUnit.Invoke();
                dropUnit.Invoke();

                break;

            case PlayerState.CONFIRM:

                UnsubscribeFromEvents();
                Destroy(GameObject.Find("ConfirmScreen(Clone)"));
                FindObjectOfType<FadeTo>().FadeToSetup();
                FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(BackToTitle);

                FindObjectOfType<SoundController>().PlayButton();

                break;
        }
    }

    void JudgeK()
    {
        switch (playerState)
        {
            case PlayerState.NAVIGATING:
               
                if (transform.Find("Player").GetComponent<PlayerController>().InteractUnitsForAttackRange())
                {
                    FindObjectOfType<SoundController>().PlayButton();

                    DisableMoneyInfo();
                    playerState = PlayerState.ATTACKRANGE;
                }

                break;

            case PlayerState.ATTACKRANGE:

                FindObjectOfType<SoundController>().PlayBack();

                deselectUnitAttackRange.Invoke();
                EnableMoneyInfo();

                //propi
                playerState = PlayerState.NAVIGATING;

                break;

            case PlayerState.INTERACTING:

                FindObjectOfType<SoundController>().PlayBack();

                deselectUnit.Invoke();
                EnableMoneyInfo();

                //propi
                playerState = PlayerState.NAVIGATING;
                break;

            case PlayerState.OPTIONS:

                FindObjectOfType<SoundController>().PlayBack();

                DisableMenuOptions();
                EnablePlayer();

                //propi
                playerState = PlayerState.NAVIGATING;
                break;

            case PlayerState.SHOP:

                FindObjectOfType<SoundController>().PlayBack();

                DisableMenuShop();
                EnablePlayer();

                //propi
                playerState = PlayerState.NAVIGATING;

                break;

            case PlayerState.WAITING:

                FindObjectOfType<SoundController>().PlayBack();

                transform.Find("Player").gameObject.SetActive(true); //alerta, codi brut però necessito activar el player perquè cancelMenuUnit necessita la seva posició
                CancelMenuUnit();
                EnablePlayer();

                //propi
                playerState = PlayerState.INTERACTING;

                break;

            case PlayerState.TARGETING:

                FindObjectOfType<SoundController>().PlayBack();

                ShowMenuUnit();

                break;

            case PlayerState.CONFIRM:

                EnableMenuOptions();
                playerState = PlayerState.OPTIONS;
                Destroy(GameObject.Find("ConfirmScreen(Clone)"));

                FindObjectOfType<SoundController>().PlayBack();

                break;

            case PlayerState.UNITINFO:

                DestroyUnitInfo();
                EnablePlayer();
                playerState = PlayerState.NAVIGATING;

                FindObjectOfType<SoundController>().PlayBack();

                break;
        }
    }

    void JudgeL()
    {
        switch (playerState)
        {
            case PlayerState.NAVIGATING:

                GameObject unit = FindObjectOfType<UnitsController>().FindNextActiveUnit();

                if (unit != null)
                {
                    transform.Find("Player").GetComponent<PlayerController>().TargetPosition(unit.transform.position);
                }

                break;
        }
    }

    void JudgeR()
    {
        switch (playerState)
        {
            case PlayerState.NAVIGATING:

                GameObject unit = transform.Find("Player").GetComponent<PlayerController>().InteractUnitsForUnitInfo();

                if (unit != null)
                {
                    InstantiateUnitInfo(unit);
                    DisablePlayer();
                    playerState = PlayerState.UNITINFO;

                    FindObjectOfType<SoundController>().PlayButton();
                }

                break;
        }
    }

    void InstantiateUnitInfo(GameObject unit)
    {
        FindObjectOfType<UIController>().InstantiateUnitInfo(unit);
    }

    void DestroyUnitInfo()
    {
        destroyUnitInfo.Invoke();
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

    public void DisableMenuOptions()
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

    void BackToTitle()
    {
        FindObjectOfType<SoundController>().StopCani();
        FindObjectOfType<SoundController>().StopHipster();
        Loader.Load(Loader.Scene.title);
    }

    public void UnitDiedCallback()
    {
        FindObjectOfType<CutsceneController>().unitDied.RemoveListener(UnitDiedCallback);
        SubscribeToEvents();
    }

    public void SubscribeToEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.AddListener(JudgeO);
        FindObjectOfType<Controls>().keyboard_k_down.AddListener(JudgeK);
        FindObjectOfType<Controls>().keyboard_q_down.AddListener(JudgeL);
        FindObjectOfType<Controls>().keyboard_e_down.AddListener(JudgeR);
    }

    public void UnsubscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.RemoveListener(JudgeO);
        FindObjectOfType<Controls>().keyboard_k_down.RemoveListener(JudgeK);
        FindObjectOfType<Controls>().keyboard_q_down.RemoveListener(JudgeL);
        FindObjectOfType<Controls>().keyboard_e_down.RemoveListener(JudgeR);
    }
}
