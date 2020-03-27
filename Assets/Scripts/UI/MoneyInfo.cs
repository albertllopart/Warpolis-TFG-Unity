using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyInfo : MonoBehaviour
{
    enum Turn
    {
        CANI, HIPSTER
    };

    Turn turn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AfterStart()
    {
        SubscribeToEvents();
        UpdateTurn();
        UpdatePosition();
        UpdateMoney();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateTurn()
    {
        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn() == GameplayController.Turn.CANI)
        {
            transform.Find("Background_hipster").gameObject.SetActive(false);
            transform.Find("Background_cani").gameObject.SetActive(true);

            turn = Turn.CANI;
        }
        else
        {
            transform.Find("Background_cani").gameObject.SetActive(false);
            transform.Find("Background_hipster").gameObject.SetActive(true);

            turn = Turn.HIPSTER;
        }
    }

    void EndTurnCani()
    {
        transform.Find("Background_cani").gameObject.SetActive(false);
        transform.Find("Background_hipster").gameObject.SetActive(true);

        turn = Turn.HIPSTER;
        UpdatePosition();

        //actualitzar diners
    }

    void EndTurnHipster()
    {
        transform.Find("Background_hipster").gameObject.SetActive(false);
        transform.Find("Background_cani").gameObject.SetActive(true);

        turn = Turn.CANI;
        UpdatePosition();

        //actualitzar diners
    }

    public void UpdatePosition()
    {
        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().playerLocation == GameplayController.PlayerLocation.RIGHT)
        {
            transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetTopLeftCorner().x + 1.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetTopLeftCorner().y - 0.5f, 0);

            UpdateLogo(true);
        }
        else
        {
            transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetTopRightCorner().x - 4.5f,
                                             (int)Camera.main.GetComponent<CameraController>().GetTopRightCorner().y - 0.5f, 0);

            UpdateLogo(false);
        }
    }

    void UpdateLogo(bool right)
    {
        switch (turn)
        {
            case Turn.CANI:
                if (right)
                    transform.Find("Background_cani").transform.Find("Logo").transform.position = transform.position + new Vector3(4, -4 / 16f, 0);
                else
                    transform.Find("Background_cani").transform.Find("Logo").transform.position = transform.position + new Vector3(-1, -4 / 16f, 0);
                break;

            case Turn.HIPSTER:
                if (right)
                    transform.Find("Background_hipster").transform.Find("Logo").transform.position = transform.position + new Vector3(4, -4 / 16f, 0);
                else
                    transform.Find("Background_hipster").transform.Find("Logo").transform.position = transform.position + new Vector3(-1, -4 / 16f, 0);
                break;
        }
    }

    void OnShop()
    {
        transform.position = new Vector3((int)Camera.main.GetComponent<CameraController>().GetTopLeftCorner().x + 1.5f,
                                         (int)Camera.main.GetComponent<CameraController>().GetTopLeftCorner().y - 0.5f, 0);

        UpdateLogo(true);
    }

    void SubscribeToEvents()
    {
        GameObject gameplay = GameObject.Find("Gameplay Controller");

        gameplay.GetComponent<GameplayController>().endTurnCani.AddListener(EndTurnCani);
        gameplay.GetComponent<GameplayController>().endTurnHipster.AddListener(EndTurnHipster);
        gameplay.GetComponent<GameplayController>().enableMenuShop.AddListener(OnShop);
        gameplay.GetComponent<GameplayController>().disableMenuShop.AddListener(UpdatePosition);
    }

    void UpdateMoney()
    {
        transform.Find("Money").GetComponent<Number>().CreateNumber(12300);
    }
}
