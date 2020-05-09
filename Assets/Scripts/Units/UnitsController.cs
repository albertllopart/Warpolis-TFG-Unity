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

    void DestroyAllUnits()
    {
        foreach (GameObject unit in caniUnits)
        {
            Destroy(unit);
        }

        foreach (GameObject unit in hipsterUnits)
        {
            Destroy(unit);
        }

        caniUnits = new List<GameObject>();
        hipsterUnits = new List<GameObject>();
    }

    public GameObject FindNextActiveUnit()
    {
        switch (FindObjectOfType<GameplayController>().GetTurn())
        {
            case GameplayController.Turn.CANI:
                foreach (GameObject unit in caniUnits)
                {
                    if (unit.GetComponent<Unit>().state != UnitState.WAITING)
                    {
                        return unit;
                    }
                }
                break;

            case GameplayController.Turn.HIPSTER:
                foreach (GameObject unit in hipsterUnits)
                {
                    if (unit.GetComponent<Unit>().state != UnitState.WAITING)
                    {
                        return unit;
                    }
                }
                break;
        }

        return null;
    }

    void SubscribeToEvents()
    {
        GameObject gameplay = GameObject.Find("Gameplay Controller");

        gameplay.GetComponent<GameplayController>().endTurnCani.AddListener(ResetCani);
        gameplay.GetComponent<GameplayController>().endTurnHipster.AddListener(ResetHipster);

        GameObject.Find("Map Controller").GetComponent<MapController>().mapUnloaded.AddListener(DestroyAllUnits);
    }
}
