using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    IDLE, SELECTED, MOVING, DECIDING
};

public enum UnitDirection
{
    RIGHT, LEFT, UP, DOWN
};

public class Unit : MonoBehaviour
{
    private UnitState state;
    private UnitDirection direction;

    private Animator animator;

    //stats
    public uint movementRange;
    public uint neutralCost;
    public uint containerCost;
    public uint lampCost;

    void Start()
    {
        state = UnitState.IDLE;
        
        transform.parent = GameObject.Find("Units Controller").transform;

        if (CompareTag("Unit_cani"))
            SetCani();
        else if (CompareTag("Unit_hipster"))
            SetHipster();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    void SetCani()
    {
        direction = UnitDirection.RIGHT;
        GetComponentInParent<UnitsController>().caniUnits.Add(gameObject);
    }

    void SetHipster()
    {
        direction = UnitDirection.LEFT;
        GetComponentInParent<UnitsController>().hipsterUnits.Add(gameObject);
    }

    UnitState GetState()
    {
        return state;
    }

    void SetState(UnitState state)
    {
        this.state = state;
    }

    public void OnSelected()
    {
        state = UnitState.SELECTED;
        UpdateAnimator();

        //mapa
        GameObject.Find("Map Controller").GetComponent<MapController>().ExecutePathfinding(gameObject);
    }

    public void OnDeselected()
    {
        state = UnitState.IDLE;
        UpdateAnimator();

        //mapa
        GameObject.Find("Map Controller").GetComponent<MapController>().DrawPathfinding(false);
    }

    void UpdateAnimator()
    {
        animator.SetInteger("state", (int)state);
        animator.SetInteger("direction", (int)direction);
    }
}
