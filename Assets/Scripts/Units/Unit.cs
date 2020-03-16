using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    IDLE, SELECTED, MOVING, DECIDING, WAIT
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
    private SpriteRenderer renderer;
    private int layerDifference = 2;

    //stats
    public uint movementRange;
    public uint neutralCost;
    public uint containerCost;
    public uint lampCost;

    //movement
    List<Vector2Int> path = new List<Vector2Int>();
    Vector2Int goal;
    Vector2Int nextPos;
    int nextPosIndex;
    float moveInterval = 0.025f;
    float moveTimer = 0.0f;
    float moveSpeed = 0.25f;

    public Vector3 lastPosition;
    public Vector2Int lastGoal;

    //menu
    private bool[] activeButtons;

    void Start()
    {
        state = UnitState.IDLE;
        
        transform.parent = GameObject.Find("Units Controller").transform;

        if (CompareTag("Unit_cani"))
            SetCani();
        else if (CompareTag("Unit_hipster"))
            SetHipster();

        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();

        activeButtons = new bool[3]; // 3 és el nombre màxim de botons actius al menú d'unitat
        activeButtons[0] = false; //capture
        activeButtons[1] = false; //attack
        activeButtons[2] = true; //wait sempre és true perquè sempre hi haurà la opció
    }

    void Update()
    {
        if (state == UnitState.MOVING)
        {
            moveTimer += Time.deltaTime;
            Move();
        }
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

    public UnitState GetState()
    {
        return state;
    }

    void SetState(UnitState state)
    {
        this.state = state;
    }

    public bool[] GetActiveButtons()
    {
        return activeButtons;
    }
    
    void Highlight(bool what)
    {
        if (what)
            renderer.sortingOrder += layerDifference;
        else
            renderer.sortingOrder -= layerDifference;
    }

    public void OnSelected()
    {
        Highlight(true);

        state = UnitState.SELECTED;
        UpdateAnimator();

        //mapa
        GameObject.Find("Map Controller").GetComponent<MapController>().ExecutePathfinding(gameObject);
    }

    public void OnDeselected()
    {
        Highlight(false);

        state = UnitState.IDLE;
        UpdateAnimator();

        //mapa
        GameObject.Find("Map Controller").GetComponent<MapController>().DrawPathfinding(false);
    }

    public void OnMenu()
    {
        state = UnitState.DECIDING;

        //canviar l'estat del gameplay oju
        GameplayController gameplay = GameObject.Find("Gameplay Controller").GetComponent<GameplayController>();

        gameplay.playerState = GameplayController.PlayerState.WAITING;
        gameplay.DisablePlayer();

        //obrir el menú oju
        EnableMenuUnit();
    }

    public void OnCancelMovement()
    {
        transform.position = lastPosition;

        state = UnitState.SELECTED;
        ResetDirection();
        UpdateAnimator();

        //mapa
        GameObject.Find("Map Controller").GetComponent<MapController>().ExecutePathfinding(gameObject);
    }

    void ResetDirection()
    {
        if (CompareTag("Unit_cani"))
            direction = UnitDirection.RIGHT;
        else if (CompareTag("Unit_hipster"))
            direction = UnitDirection.LEFT;
    }

    void EnableMenuUnit()
    {
        GameObject.Find("UI Controller").transform.Find("Menu_unit").gameObject.SetActive(true);
        GameObject.Find("UI Controller").transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnEnable(gameObject);
    }

    void DisableMenuUnit()
    {
        GameObject.Find("UI Controller").transform.Find("Menu_unit").GetComponent<MenuUnitController>().MyOnDisable();
        GameObject.Find("UI Controller").transform.Find("Menu_unit").gameObject.SetActive(false);
    }

    public void OnWait()
    {
        Highlight(false);

        state = UnitState.WAIT;

        ResetDirection();
        UpdateAnimator();
    }

    public void OnMove(Vector2Int goal)
    {
        //guardo la posició per si es cancel·la l'acció de moure
        lastPosition = transform.position;

        GetPath(goal);
        state = UnitState.MOVING;

        //mapa
        GameObject.Find("Map Controller").GetComponent<MapController>().DrawPathfinding(false);
    }

    void UpdateAnimator()
    {
        animator.SetInteger("state", (int)state);
        animator.SetInteger("direction", (int)direction);
    }

    void UpdateActiveButtons()
    {

    }

    void GetPath(Vector2Int goal)
    {
        if (path.Count > 0)
            path.Clear();

        this.goal = goal;
        path = GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.GetPath(goal);
        nextPosIndex = 1;
        nextPos = path[nextPosIndex];
    }

    void Move()
    {
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0.0f;

            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

            if (myPos != goal && path.Count > 0)
            {
                if (myPos != nextPos)
                {
                    //determinar direcció de la nextPos respecte la posició actual

                    if (nextPos.x > (int)myPos.x)                  //arrodonim cap avall
                        direction = UnitDirection.RIGHT;
                    else if (nextPos.x < Mathf.Ceil(myPos.x))      //arrodonim cap amunt
                        direction = UnitDirection.LEFT;
                    else if (-nextPos.y < Mathf.Ceil(-myPos.y))    //canviem signe perquè no podem arrodonir negatius al revés
                        direction = UnitDirection.UP;
                    else if (nextPos.y < (int)myPos.y)             //arrodonim cap avall
                        direction = UnitDirection.DOWN;

                    UpdateAnimator();

                    switch (direction)
                    {
                        case UnitDirection.RIGHT:
                            transform.position += new Vector3(moveSpeed, 0, 0);
                            break;

                        case UnitDirection.LEFT:
                            transform.position += new Vector3(-moveSpeed, 0, 0);
                            break;

                        case UnitDirection.UP:
                            transform.position += new Vector3(0, moveSpeed, 0);
                            break;

                        case UnitDirection.DOWN:
                            transform.position += new Vector3(0, -moveSpeed, 0);
                            break;
                    }
                }
                else
                {
                    //aquí s'entrarà quan ja s'hagi arribat a la següent posició del path
                    nextPos = path[++nextPosIndex];
                }
            }

            if (myPos == goal)
            {
                // buscar targets
                // actualitzar estat

                OnMenu();
            }
        }
    }
}
