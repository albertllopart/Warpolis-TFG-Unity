﻿using System.Collections;
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

    //events
    public UnityEvent openMenuOptions;
    public UnityEvent closeMenuOptions;
    public UnityEvent deselectUnit;
    public UnityEvent moveUnit;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        openMenuOptions = new UnityEvent();
        closeMenuOptions = new UnityEvent();
        deselectUnit = new UnityEvent();
        moveUnit = new UnityEvent();

        playerState = PlayerState.NAVIGATING;
        turn = Turn.CANI;
    }

    // Update is called once per frame
    void Update()
    {

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
                turn = Turn.HIPSTER;
                break;

            case Turn.HIPSTER:
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
                    playerState = PlayerState.INTERACTING;
                }
                else if(transform.Find("Player").GetComponent<PlayerController>().InteractBuildings())
                {
                    DisablePlayer();
                    EnableMenuShop();
                    playerState = PlayerState.SHOP;
                }
                else
                {
                    //player
                    DisablePlayer();

                    //menú
                    EnableMenuOptions();

                    //propi
                    playerState = PlayerState.OPTIONS;
                }

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
        GameObject.Find("UI Controller").transform.Find("Menu_options").gameObject.SetActive(true);
        GameObject.Find("UI Controller").transform.Find("Menu_options").GetComponent<MenuOptionsController>().MyOnEnable();
    }

    void DisableMenuOptions()
    {
        GameObject.Find("UI Controller").transform.Find("Menu_options").GetComponent<MenuOptionsController>().MyOnDisable();
        GameObject.Find("UI Controller").transform.Find("Menu_options").gameObject.SetActive(false);
    }

    void EnableMenuShop()
    {
        GameObject.Find("UI Controller").transform.Find("Menu_shop").gameObject.SetActive(true);
        GameObject.Find("UI Controller").transform.Find("Menu_shop").GetComponent<MenuShopController>().MyOnEnable();
    }

    void DisableMenuShop()
    {
        GameObject.Find("UI Controller").transform.Find("Menu_shop").GetComponent<MenuShopController>().MyOnDisable();
        GameObject.Find("UI Controller").transform.Find("Menu_shop").gameObject.SetActive(false);
    }

    public void DisableMenuUnit()
    {
        //d'aquest no hi ha enable perquè qui l'activa és la unitat

        MenuUnitController menu = GameObject.Find("UI Controller").transform.Find("Menu_unit").GetComponent<MenuUnitController>();
        menu.MyOnDisable();
        menu.gameObject.SetActive(false);

        EnablePlayer();
        transform.Find("Player").GetComponent<PlayerController>().selectedUnit = null;

        playerState = PlayerState.NAVIGATING;
    }

    public void ShowMenuUnit() // as opposed to hide
    {
        GameObject menu = GameObject.Find("UI Controller").transform.Find("Menu_unit").gameObject;
        MenuUnitController menuController = menu.GetComponent<MenuUnitController>();

        menu.SetActive(true);
        menuController.selectedUnit.GetComponent<Unit>().OnMenu();
        menuController.selectedUnit.GetComponent<Unit>().UnsubscribeFromEvents();

        playerState = PlayerState.WAITING;
    }

    public void HideMenuUnit()
    {
        //per quan la unitat hagi d'atacar
        GameObject.Find("UI Controller").transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnHide();
        GameObject.Find("UI Controller").transform.Find("Menu_unit").gameObject.SetActive(false);

        playerState = PlayerState.TARGETING;
    }

    void CancelMenuUnit()
    {
        GameObject.Find("UI Controller").transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnCancel();
        GameObject.Find("UI Controller").transform.Find("Menu_unit").gameObject.SetActive(false);
    }
}
