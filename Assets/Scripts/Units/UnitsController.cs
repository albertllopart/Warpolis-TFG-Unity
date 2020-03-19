﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsController : MonoBehaviour
{
    public List<GameObject> caniUnits;
    public List<GameObject> hipsterUnits;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetCani()
    {
        foreach (GameObject unit in caniUnits)
        {
            unit.GetComponent<Unit>().OnIdle();
        }
    }

    void ResetHipster()
    {
        foreach (GameObject unit in hipsterUnits)
        {
            unit.GetComponent<Unit>().OnIdle();
        }
    }

    void SubscribeToEvents()
    {
        GameObject gameplay = GameObject.Find("Gameplay Controller");

        gameplay.GetComponent<GameplayController>().endTurnCani.AddListener(ResetCani);
        gameplay.GetComponent<GameplayController>().endTurnHipster.AddListener(ResetHipster);
    }
}
