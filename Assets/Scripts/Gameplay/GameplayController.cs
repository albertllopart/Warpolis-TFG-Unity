using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayController : MonoBehaviour
{
    public enum PlayerState
    {
        NAVIGATING, OPTIONS, INTERACTING, SHOP
    };

    public enum Turn
    {
        CANI, HIPSTER
    };

    private PlayerState playerState;
    private Turn turn;
    public uint pseudoTurn = 0;

    //events
    public UnityEvent openMenuOptions;
    public UnityEvent closeMenuOptions;
    public UnityEvent deselectUnit;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        openMenuOptions = new UnityEvent();
        closeMenuOptions = new UnityEvent();
        deselectUnit = new UnityEvent();

        playerState = PlayerState.NAVIGATING;
        turn = Turn.CANI;
    }

    // Update is called once per frame
    void Update()
    {
        if (pseudoTurn == 0)
            turn = Turn.CANI;
        else
            turn = Turn.HIPSTER;
    }
    public Turn GetTurn()
    {
        return turn;
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
                break;

            case PlayerState.OPTIONS:
                //interactuar amb el menú
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
        }
    }

    void EnablePlayer()
    {
        transform.Find("Player").gameObject.SetActive(true);
        transform.Find("Player").GetComponent<PlayerController>().MyOnEnable();
    }

    void DisablePlayer()
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
}
