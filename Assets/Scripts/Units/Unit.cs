using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    IDLE, SELECTED, MOVING, DECIDING, WAITING, TARGETING
};

public enum UnitDirection
{
    RIGHT, LEFT, UP, DOWN
};

public enum UnitType
{
    INFANTRY
};

public enum UnitArmy
{
    CANI, HIPSTER
};

public class Unit : MonoBehaviour
{
    private UnitState state;
    private UnitDirection direction;
    public UnitArmy army;

    private Animator animator;
    private SpriteRenderer renderer;
    private int layerDifference = 2;

    //important
    [HideInInspector]
    public bool myTurn; // per saber si la unitat ha estat seleccionada al torn del seu exèrcit o no

    //stats
    [Header("Info")]
    public uint unitType; //el tipus d'unitat s'assigna des de l'inspector
    public uint movementRange;
    public uint neutralCost;
    public uint containerCost;
    public uint lampCost;

    [Header("Stats")]
    public int hitPoints;
    public uint basePower; //el basePower és el mal que faran a una unitat enemiga de la mateixa categoria. Després se li aplicaran multiplicadors en funció de a què s'enfrontin
    public uint defense; //la defensa base sempre serà 0 i estarà determinada per la casella

    [Header("Multipliers")]
    public float vsInfantry;

    [Header("Interaction")]
    public List<GameObject> targets;
    public GameObject currentTarget;
    public GameObject currentCapture;

    //movement
    List<Vector2Int> path = new List<Vector2Int>();
    Vector2Int goal;
    Vector2Int nextPos;
    int nextPosIndex;
    float moveInterval = 0.025f;
    float moveTimer = 0.0f;
    float moveSpeed = 0.25f;

    //targeting
    float switchTargetTimer = 0.0f;
    float switchTargetInterval = 0.1f;

    [HideInInspector]
    public Vector3 lastPosition;
    [HideInInspector]
    public Vector2Int lastGoal;

    //UI
    private bool[] activeButtons;
    GameObject UITarget;
    GameObject UIHitPoints;

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

        UITarget = transform.Find("Targeting").gameObject;
        UITarget.SetActive(false);
        UIHitPoints = transform.Find("Hitpoints").gameObject;

        UpdateStatsBasedOnTile();
    }

    void Update()
    {
        if (state == UnitState.MOVING)
        {
            moveTimer += Time.deltaTime;
            Move();
        }
        else if (state == UnitState.TARGETING)
        {
            switchTargetTimer += Time.deltaTime;
        }

        DrawLines();

        if (Input.GetKeyDown("m"))
        {
            UpdateTileForDeath();
        }
    }

    void MyOnDestroy()
    {
        Debug.Log("Unit::MyOnDestroy - Died in position: " + transform.position);

        UpdateTileInfo();
        UpdateTileForDeath();

        if (army == UnitArmy.CANI)
            GetComponentInParent<UnitsController>().caniUnits.Remove(gameObject);
        else
            GetComponentInParent<UnitsController>().hipsterUnits.Remove(gameObject);

        Destroy(gameObject);
    }

    void SetCani()
    {
        direction = UnitDirection.RIGHT;
        army = UnitArmy.CANI;
        GetComponentInParent<UnitsController>().caniUnits.Add(gameObject);

        //li hem de dir al myTilemap que som allà
        GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.MyTilemap[(int)transform.position.x, -(int)transform.position.y].containsCani = true;
    }

    void SetHipster()
    {
        direction = UnitDirection.LEFT;
        army = UnitArmy.HIPSTER;
        GetComponentInParent<UnitsController>().hipsterUnits.Add(gameObject);

        //li hem de dir al myTilemap que som allà
        GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.MyTilemap[(int)transform.position.x, -(int)transform.position.y].containsHipster = true;
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

    public void OnIdle()
    {
        state = UnitState.IDLE;
        UpdateAnimator();
    }

    public void OnSelected()
    {
        Highlight(true);

        state = UnitState.SELECTED;
        UpdateAnimator();

        //mapa
        GameObject.Find("Map Controller").GetComponent<MapController>().ExecutePathfinding(gameObject);

        SetMyTurn();
    }

    void SetMyTurn()
    {
        if (GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().GetTurn() == GameplayController.Turn.CANI)
        {
            if (army == UnitArmy.CANI)
                myTurn = true;
            else
                myTurn = false;
        }
        else
        {
            if (army == UnitArmy.HIPSTER)
                myTurn = true;
            else
                myTurn = false;
        }
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
        SearchForTargets();

        if (unitType == (uint)UnitType.INFANTRY)
        {
            SearchForOtherBuilding(); // això és per la captura
        }

        UpdateStatsBasedOnTile();

        state = UnitState.DECIDING;

        //canviar l'estat del gameplay oju
        GameplayController gameplay = GameObject.Find("Gameplay Controller").GetComponent<GameplayController>();

        gameplay.playerState = GameplayController.PlayerState.WAITING;
        gameplay.DisablePlayer();

        //obrir el menú oju
        EnableMenuUnit();
        Untarget();
    }

    public void OnCancelMovement()
    {
        transform.position = lastPosition;
        UpdateStatsBasedOnTile();

        state = UnitState.SELECTED;
        ResetTargeting();
        ResetDirection();
        UpdateAnimator();

        //mapa
        GameObject.Find("Map Controller").GetComponent<MapController>().ExecutePathfinding(gameObject);
    }

    void ResetTargeting()
    {
        targets.Clear();
        currentTarget = null;
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

        state = UnitState.WAITING;

        UpdateTileInfo();

        lastPosition = transform.position;

        ResetTargeting();
        ResetDirection();
        UpdateAnimator();

        UnsubscribeFromEvents();
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

    public void OnTargeting()
    {
        state = UnitState.TARGETING;

        if (targets.Count > 0)
        {
            currentTarget = targets[0];
            Target(currentTarget);
            UITarget.SetActive(true);
        }

        UpdateAnimator();

        SubscribeToEvents(); //necessito saber quan s'apreten les direccions
    }

    public void OnAttack()
    {
        AttackTarget();
        Untarget();

        if (hitPoints > 0)
            OnWait();
    }

    void UpdateAnimator()
    {
        animator.SetInteger("state", (int)state);
        animator.SetInteger("direction", (int)direction);
    }

    void GetPath(Vector2Int goal)
    {
        if (path.Count > 0)
            path.Clear();

        this.goal = goal;
        path = GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.GetPath(goal);
        
        nextPosIndex = 1;
        if(path.Count > 1)
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
                    if (path.Count >= nextPosIndex)
                        nextPos = path[++nextPosIndex];
                }
            }

            if (myPos == goal)
            {
                OnMenu();
            }
        }
    }

    void UpdateTileInfo()
    {
        if (army == UnitArmy.CANI)
        {
            MapController map = GameObject.Find("Map Controller").GetComponent<MapController>();

            map.pathfinding.MyTilemap[(int)lastPosition.x, -(int)lastPosition.y].containsCani = false;
            map.pathfinding.MyTilemap[(int)transform.position.x, -(int)transform.position.y].containsCani = true;
        }
        
        if (army == UnitArmy.HIPSTER)
        {
            MapController map = GameObject.Find("Map Controller").GetComponent<MapController>();

            map.pathfinding.MyTilemap[(int)lastPosition.x, -(int)lastPosition.y].containsHipster = false;
            map.pathfinding.MyTilemap[(int)transform.position.x, -(int)transform.position.y].containsHipster = true;
        }
    }

    void UpdateTileForDeath()
    {
        if (army == UnitArmy.CANI)
        {
            GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.MyTilemap[(int)transform.position.x, -(int)transform.position.y].containsCani = false;

            Debug.Log("Unit::UpdateTileForDeath - Setting MyTilemap.containsCani to " + 
                GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.MyTilemap[(int)transform.position.x, -(int)transform.position.y].containsCani);
        }

        if (army == UnitArmy.HIPSTER)
        {
            GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.MyTilemap[(int)transform.position.x, -(int)transform.position.y].containsHipster = false;
        }
    }

    void UpdateStatsBasedOnTile()
    {
        switch(GetMyTileType(transform.position))
        {
            case MyTileType.ROAD:
                defense = 0;
                break;

            case MyTileType.NEUTRAL:
                defense = 1;
                break;

            case MyTileType.CONTAINER:
                defense = 2;
                break;

            case MyTileType.BUILDING:
                defense = 3;
                break;

            case MyTileType.LAMP:
                defense = 4;
                break;
        }
    }

    MyTileType GetMyTileType(Vector2 pos)
    {
        MapController map = GameObject.Find("Map Controller").GetComponent<MapController>();

        return map.GetMyTile(pos).type;
    }

    void SearchForTargets()
    {
        targets = new List<GameObject>();

        Vector2 from = transform.position; from += new Vector2(0.5f, -0.5f); //establim el punt de partida al centre de la casella

        Vector2 north = from + new Vector2(0, 1); // clockwise
        Vector2 east = from + new Vector2(1, 0);
        Vector2 south = from + new Vector2(0, -1);
        Vector2 west = from + new Vector2(-1, 0);

        int layer = 0;

        if (army == UnitArmy.CANI)
        {
            layer = LayerMask.GetMask("Hipster_units");
        }
        else if (army == UnitArmy.HIPSTER)
        {
            layer = LayerMask.GetMask("Cani_units");
        }

        RaycastHit2D result = Physics2D.Linecast(from, north, layer);
        if (result.collider != null)
        {
            targets.Add(result.collider.gameObject);
            Debug.Log("Unit::SearchForTargets - Found target: " + result.collider.gameObject.name + " in north position: " + result.collider.transform.position);
        }

        result = Physics2D.Linecast(from, east, layer);
        if (result.collider != null)
        {
            targets.Add(result.collider.gameObject);
            Debug.Log("Unit::SearchForTargets - Found target: " + result.collider.gameObject.name + " in east position: " + result.collider.transform.position);
        }

        result = Physics2D.Linecast(from, south, layer);
        if (result.collider != null)
        {
            targets.Add(result.collider.gameObject);
            Debug.Log("Unit::SearchForTargets - Found target: " + result.collider.gameObject.name + " in south position: " + result.collider.transform.position);
        }

        result = Physics2D.Linecast(from, west, layer);
        if (result.collider != null)
        {
            targets.Add(result.collider.gameObject);
            Debug.Log("Unit::SearchForTargets - Found target: " + result.collider.gameObject.name + " in west position: " + result.collider.transform.position);
        }

        //si hem trobat algun target activem el botó d'atacar
        if (targets.Count > 0)
            EnableAttackButton(true);
        else
            EnableAttackButton(false);
    }

    void EnableAttackButton(bool enable)
    {
        activeButtons[1] = enable;
    }

    GameObject SearchForOtherBuilding()
    {
        //aquest mètode retorna el building enemic o neutral que hi ha a la casella de la unitat

        Vector2 from = transform.position; from += new Vector2(0.5f, -0.5f); //establim el punt de partida al centre de la casella
        Vector2 to = from;

        RaycastHit2D neutral = Physics2D.Linecast(from, to, LayerMask.GetMask("Neutral_buildings")); ;

        if (neutral.collider != null)
        {
            EnableCaptureButton(true);
            return neutral.collider.gameObject;
        }
        else
        {
            if (army == UnitArmy.CANI)
            {
                RaycastHit2D hipster = Physics2D.Linecast(from, to, LayerMask.GetMask("Hipster_buildings"));
                if (hipster.collider != null)
                {
                    EnableCaptureButton(true);
                    return hipster.collider.gameObject;
                }
            }
            else if (army == UnitArmy.HIPSTER)
            {
                RaycastHit2D cani = Physics2D.Linecast(from, to, LayerMask.GetMask("Cani_buildings"));
                if (cani.collider != null)
                {
                    EnableCaptureButton(true);
                    return cani.collider.gameObject;
                }
            }
        }

        EnableCaptureButton(false);
        return null;
    }

    void EnableCaptureButton(bool enable)
    {
        activeButtons[0] = enable;
    }

    void DrawLines()
    {
        Vector2 from = transform.position; from += new Vector2(0.5f, -0.5f); //establim el punt de partida al centre de la casella

        Vector2 north = from + new Vector2(0, 1);
        Vector2 south = from + new Vector2(0, -1);
        Vector2 east = from + new Vector2(1, 0);
        Vector2 west = from + new Vector2(-1, 0);

        Debug.DrawLine(from, north, Color.green);
        Debug.DrawLine(from, south, Color.green);
        Debug.DrawLine(from, east, Color.green);
        Debug.DrawLine(from, west, Color.green);
    }

    void SelectNextTarget()
    {
        if (switchTargetTimer >= switchTargetInterval)
        {
            if (targets.Count > 0)
            {
                int currentTargetIndex = targets.IndexOf(currentTarget);

                if (currentTargetIndex == targets.Count - 1)
                    currentTarget = targets[0];
                else
                    currentTarget = targets[++currentTargetIndex];
            }

            Target(currentTarget);

            //actualitzem la direcció
            //if (currentTarget != null)
            //{
            //    direction = GetDirectionTo(currentTarget.transform.position);
            //}

            //UpdateAnimator();

            switchTargetTimer = 0.0f;
        }
    }

    void SelectPreviousTarget()
    {
        if (switchTargetTimer >= switchTargetInterval)
        {
            if (targets.Count > 0)
            {
                int currentTargetIndex = targets.IndexOf(currentTarget);

                if (currentTargetIndex == 0)
                    currentTarget = targets[targets.Count - 1];
                else
                    currentTarget = targets[--currentTargetIndex];
            }

            Target(currentTarget);

            //actualitzem la direcció
            //if (currentTarget != null)
            //{
            //    direction = GetDirectionTo(currentTarget.transform.position);
            //}

            //UpdateAnimator();

            switchTargetTimer = 0.0f;
        }
    }

    UnitDirection GetDirectionTo(Vector2 pos)
    {
        if (pos.x > transform.position.x)
            return UnitDirection.RIGHT;
        else if (pos.x < transform.position.x)
            return UnitDirection.LEFT;
        else if (pos.y < transform.position.y)
            return UnitDirection.DOWN;
        else if (pos.y > transform.position.y)
            return UnitDirection.UP;

        return UnitDirection.RIGHT;
    }

    void Target(GameObject target)
    {
        if (target != null)
        {
            UITarget.transform.position = target.transform.position;

            float estimateDamage = DamageFormula(target);
            Debug.Log("Unit::Target - Estimate damage: " + estimateDamage);
        }
    }

    void Untarget()
    {
        UITarget.transform.position = transform.position;
        UITarget.SetActive(false);
    }

    void AttackTarget()
    {
        //atacant
        float offensiveDamage = DamageFormula(currentTarget);
        currentTarget.GetComponent<Unit>().hitPoints -= RatioToInt(offensiveDamage);

        Debug.Log("Unit::AttackTarget - Enemy Hitpoints = " + currentTarget.GetComponent<Unit>().hitPoints);

        if (currentTarget.GetComponent<Unit>().hitPoints > 0)
        {
            UpdateHitpoints(currentTarget);

            float defensiveDamage = currentTarget.GetComponent<Unit>().DamageFormula(gameObject);
            hitPoints -= RatioToInt(defensiveDamage);

            if (hitPoints <= 0)
            {
                MyOnDestroy();
            }
        }
        else
        {
            currentTarget.GetComponent<Unit>().MyOnDestroy();
        }

        UpdateHitpoints(gameObject);
    }

    void UpdateHitpoints(GameObject unit)
    {
        if (unit != gameObject)
            unit.GetComponent<Unit>().UIHitPoints.GetComponent<Hitpoints>().SetSprite(unit.GetComponent<Unit>().CalculateUIHitpoints());
        else
            UIHitPoints.GetComponent<Hitpoints>().SetSprite(CalculateUIHitpoints());
    }

    float DamageFormula(GameObject enemy)
    {
        float damage = (float)basePower * 0.01f; // convertim 50 en 0.5

        //establim multiplicador de defensa segons la casella
        float defenseMultiplier = 1.0f - enemy.GetComponent<Unit>().defense * 0.1f; // 1 de defensa resta 0.1 al multiplicador

        //establim multiplicador segons a quina unitat ens enfrontem
        float typeMultiplier = 0.0f;
        switch (enemy.GetComponent<Unit>().unitType)
        {
            case (uint)UnitType.INFANTRY:
                typeMultiplier = 1.0f * vsInfantry;
                break;
        }

        //establim multiplicador segons la vida que tingui l'atacant
        float hpMultiplier = (hitPoints * 2) * 0.01f; //multiplico la vida x2 perquè el màxim és 50 i m'interessa tenir-la en base 100 per calcular el %

        //fórmula final amb tots els paràmetres
        damage = damage * defenseMultiplier * typeMultiplier * hpMultiplier;

        return damage;
    }

    int RatioToInt(float ratio)
    {
        //aquest mètode retorna el dany en forma de hitpoints

        float ret = 50.0f; // 50 és la vida màxima
        ret = ret * ratio;

        return Mathf.RoundToInt(ret);
    }

    uint CalculateUIHitpoints()
    {
        if (hitPoints > 45)
            return 5;
        else if (hitPoints <= 45 && hitPoints > 35)
            return 4;
        else if (hitPoints <= 35 && hitPoints > 25)
            return 3;
        else if (hitPoints <= 25 && hitPoints > 15)
            return 2;
        else if (hitPoints <= 15)
            return 1;

        return 5;
    }

    void SubscribeToEvents()
    {
        GameObject.Find("Gameplay Controller").GetComponent<Controls>().keyboard_w_down.AddListener(SelectNextTarget);
        GameObject.Find("Gameplay Controller").GetComponent<Controls>().keyboard_a_down.AddListener(SelectPreviousTarget);
        GameObject.Find("Gameplay Controller").GetComponent<Controls>().keyboard_s_down.AddListener(SelectPreviousTarget);
        GameObject.Find("Gameplay Controller").GetComponent<Controls>().keyboard_d_down.AddListener(SelectNextTarget);

        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().attackUnit.AddListener(OnAttack);
    }

    public void UnsubscribeFromEvents()
    {
        GameObject.Find("Gameplay Controller").GetComponent<Controls>().keyboard_w_down.RemoveListener(SelectNextTarget);
        GameObject.Find("Gameplay Controller").GetComponent<Controls>().keyboard_a_down.RemoveListener(SelectPreviousTarget);
        GameObject.Find("Gameplay Controller").GetComponent<Controls>().keyboard_s_down.RemoveListener(SelectPreviousTarget);
        GameObject.Find("Gameplay Controller").GetComponent<Controls>().keyboard_d_down.RemoveListener(SelectNextTarget);

        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().attackUnit.RemoveListener(OnAttack);
    }
}
