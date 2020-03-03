using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayController : MonoBehaviour
{
    public enum PlayerState
    {
        NAVIGATING, OPTIONS, INTERACTING
    };

    public enum Turn
    {
        CANI, HIPSTER
    };

    private PlayerState playerState;
    private Turn turn;

    //events
    public UnityEvent OpenMenuOptions;
    public UnityEvent CloseMenuOptions;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        OpenMenuOptions = new UnityEvent();
        CloseMenuOptions = new UnityEvent();

        playerState = PlayerState.NAVIGATING;
        turn = Turn.CANI;
    }

    // Update is called once per frame
    void Update()
    {
       
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
                //interactuar amb la casella
                transform.Find("Player").gameObject.SetActive(false);
                GameObject.Find("UI Controller").transform.Find("Menu_options").gameObject.SetActive(true);
                OpenMenuOptions.Invoke();
                playerState = PlayerState.OPTIONS;
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
                //interactuar amb la unitat
                break;

            case PlayerState.OPTIONS:
                //interactuar amb el menú
                GameObject.Find("UI Controller").transform.Find("Menu_options").gameObject.SetActive(false);
                transform.Find("Player").gameObject.SetActive(true);
                CloseMenuOptions.Invoke();
                playerState = PlayerState.NAVIGATING;
                break;
        }
    }
}
