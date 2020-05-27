using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitInfantry : MonoBehaviour
{
    [Header("Capture")]
    public GameObject currentCapture;
    public GameObject lastCapture;
    public bool toStopCapture;
    GameObject UICaptureSign;

    //instantiate
    [Header("Instances")]
    public GameObject factory;
    public GameObject building;

    //events
    public UnityEvent SuccessfullyDropped;

    // Start is called before the first frame update
    void Start()
    {
        SuccessfullyDropped = new UnityEvent();

        GetComponent<Unit>().UITarget = transform.Find("Targeting").gameObject;
        GetComponent<Unit>().EnableUITarget(false);
        GetComponent<Unit>().UIDamageInfo = transform.Find("Damage_info").gameObject;
        GetComponent<Unit>().EnableUIDamageInfo(false);

        UICaptureSign = transform.Find("Capture").gameObject;
        UICaptureSign.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (toStopCapture)
        {
            StopCapture();
        }
    }

    public void ManageButtons()
    {
        if (SearchForOtherBuilding() != null) // això és per la captura
            EnableCaptureButton(true);
        else
            EnableCaptureButton(false);

        if (SearchForTransport() != null)
        {
            EnableLoadButton(true);
        }
        else
            EnableLoadButton(false);
    }

    void EnableCaptureButton(bool enable)
    {
        GetComponent<Unit>().activeButtons[0] = enable;
    }

    void EnableLoadButton(bool enable)
    {
        GetComponent<Unit>().activeButtons[3] = enable;
        GetComponent<Unit>().activeButtons[2] = !enable; //activar o desactivar wait en funció de si hi ha o no transport

        //desactivem la resta de botons ja que el load només pot ser-hi sol
        if (enable)
        {
            GetComponent<Unit>().activeButtons[0] = !enable;
            GetComponent<Unit>().activeButtons[1] = !enable;
        }
    }

    public void StopCapture()
    {
        if (currentCapture != null)
        {
            currentCapture.GetComponent<Building>().currentHP = currentCapture.GetComponent<Building>().maxHP;
            currentCapture = null;
        }

        EnableUICaptureSign(false);

        toStopCapture = false;
    }

    public void OnCapture()
    {
        //eliminem listeners de IA
        if (FindObjectOfType<AIController>().inControl)
        {
            GetComponent<Unit>().finishedMoving.RemoveListener(OnCapture);
        }

        //comprovem que l'edifici que volem capturar és el mateix que abans, en cas contrari resetegem l'anterior
        if (currentCapture != null && currentCapture != SearchForOtherBuilding())
            StopCapture();

        //obtenim edifici que volem capturar
        if (currentCapture == null)
            currentCapture = SearchForOtherBuilding();

        if (currentCapture != null)
        {
            EnableUICaptureSign(true);
            int capturePower = (int)GetComponent<Unit>().CalculateUIHitpoints();

            //restar vida al building i mirar si s'ha capturat
            if(currentCapture.GetComponent<Building>().ApplyCapture(capturePower))
            {
                GetComponent<Unit>().OnWait();

                if (currentCapture.name.Contains("Base"))
                {
                    GetComponent<Unit>().OnWinCon();
                    GameObject.Find("Data Controller").GetComponent<DataController>().baseCaptured.Invoke();
                    FindObjectOfType<AIController>().MyOnDisable();
                }

                DestroyAndCreate();
                currentCapture = null;

                return; // això és perquè un cop ha acabat la captura no segueixi en estat de captura
            }
        }

        GetComponent<Unit>().OnWaitWithCapture();
    }

    public void EnableUICaptureSign(bool enable)
    {
        UICaptureSign.SetActive(enable);
    }

    void DestroyAndCreate()
    {
        Debug.Log("UnitInfantry::DestroyAndCreate");

        if (currentCapture.GetComponent<Building>().type == BuildingType.BASE)
        {
            Instantiate(building, transform.position, Quaternion.identity);
        }
        else if (currentCapture.GetComponent<Building>().type == BuildingType.FACTORY)
        {
            Instantiate(factory, transform.position, Quaternion.identity);
        }
        else if (currentCapture.GetComponent<Building>().type == BuildingType.BUILDING)
        {
            Instantiate(building, transform.position, Quaternion.identity);
        }

        currentCapture.GetComponent<Building>().MyOnDestroy();

        FindObjectOfType<SoundController>().PlayCapture();
    }

    public GameObject SearchForOtherBuilding()
    {
        //aquest mètode retorna el building enemic o neutral que hi ha a la casella de la unitat

        Vector2 from = transform.position; from += new Vector2(0.5f, -0.5f); //establim el punt de partida al centre de la casella
        Vector2 to = from;

        RaycastHit2D neutral = Physics2D.Linecast(from, to, LayerMask.GetMask("Neutral_buildings")); ;

        if (neutral.collider != null)
        {
            return neutral.collider.gameObject;
        }
        else
        {
            if (GetComponent<Unit>().army == UnitArmy.CANI)
            {
                RaycastHit2D hipster = Physics2D.Linecast(from, to, LayerMask.GetMask("Hipster_buildings"));
                if (hipster.collider != null)
                {
                    return hipster.collider.gameObject;
                }
            }
            else if (GetComponent<Unit>().army == UnitArmy.HIPSTER)
            {
                RaycastHit2D cani = Physics2D.Linecast(from, to, LayerMask.GetMask("Cani_buildings"));
                if (cani.collider != null)
                {
                    return cani.collider.gameObject;
                }
            }
        }

        return null;
    }

    public GameObject SearchForTransport()
    {
        //aquest mètode retorna la unitat de transport que hi ha a la casella de la unitat

        Vector2 from = transform.position; from += new Vector2(0.5f, -0.5f); //establim el punt de partida al centre de la casella
        Vector2 to = from;

        GetComponent<Unit>().EnableOwnCollider(false); //desactivem el propi collider per mirar què hi ha sota

        if (GetComponent<Unit>().army == UnitArmy.CANI)
        {
            RaycastHit2D cani = Physics2D.Linecast(from, to, LayerMask.GetMask("Cani_units"));
            if (cani.collider != null && cani.collider.gameObject.GetComponent<Unit>().unitType == UnitType.TRANSPORT)
            {
                GetComponent<Unit>().EnableOwnCollider(true);
                return cani.collider.gameObject;
            }
        }
        else if (GetComponent<Unit>().army == UnitArmy.HIPSTER)
        {
            RaycastHit2D hipster = Physics2D.Linecast(from, to, LayerMask.GetMask("Hipster_units"));
            if (hipster.collider != null && hipster.collider.gameObject.GetComponent<Unit>().unitType == UnitType.TRANSPORT)
            {
                GetComponent<Unit>().EnableOwnCollider(true);
                return hipster.collider.gameObject;
            }
        }

        GetComponent<Unit>().EnableOwnCollider(true);
        return null;
    }

    public void OnLoad()
    {
        GameObject transport = SearchForTransport();

        transport.GetComponent<UnitTransport>().loadedUnit = gameObject;
        transport.GetComponent<UnitTransport>().EnableLoadSign(true);

        StopCapture();
        GetComponent<Unit>().UpdateTileInfo();
        GetComponent<Unit>().UnsubscribeFromEvents();
        GetComponent<Unit>().state = UnitState.DROPPED;

        gameObject.SetActive(false);

        if (!FindObjectOfType<AIController>().inControl)
            GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().DisableMenuUnit();
        else
        {
            GetComponent<Unit>().finishedMoving.RemoveListener(OnLoad);
            GetComponent<Unit>().finishedAI.Invoke();
        }
    }

    public void OnDropped(Vector2Int origin, Vector2Int goal)
    {
        GetPath(origin, goal);
        GetComponent<Unit>().state = UnitState.DROPPED;
    }

    void GetPath(Vector2Int origin, Vector2Int goal)
    {
        if (GetComponent<Unit>().path.Count > 0)
            GetComponent<Unit>().path.Clear();

        GetComponent<Unit>().goal = goal;
        GetComponent<Unit>().path = new List<Vector2Int>();
        GetComponent<Unit>().path.Add(origin);
        GetComponent<Unit>().path.Add(goal);

        GetComponent<Unit>().nextPosIndex = 1;
        if (GetComponent<Unit>().path.Count > 1)
            GetComponent<Unit>().nextPos = GetComponent<Unit>().path[GetComponent<Unit>().nextPosIndex];
    }

    public void MoveOnDropped()
    {
        if (GetComponent<Unit>().moveTimer >= GetComponent<Unit>().moveInterval)
        {
            GetComponent<Unit>().moveTimer = 0.0f;

            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

            if (myPos != GetComponent<Unit>().goal && GetComponent<Unit>().path.Count > 0)
            {
                if (myPos != GetComponent<Unit>().nextPos)
                {
                    //determinar direcció de la nextPos respecte la posició actual

                    if (GetComponent<Unit>().nextPos.x > (int)myPos.x)                  //arrodonim cap avall
                        GetComponent<Unit>().direction = UnitDirection.RIGHT;
                    else if (GetComponent<Unit>().nextPos.x < Mathf.Ceil(myPos.x))      //arrodonim cap amunt
                        GetComponent<Unit>().direction = UnitDirection.LEFT;
                    else if (-GetComponent<Unit>().nextPos.y < Mathf.Ceil(-myPos.y))    //canviem signe perquè no podem arrodonir negatius al revés
                        GetComponent<Unit>().direction = UnitDirection.UP;
                    else if (GetComponent<Unit>().nextPos.y < (int)myPos.y)             //arrodonim cap avall
                        GetComponent<Unit>().direction = UnitDirection.DOWN;

                    GetComponent<Unit>().UpdateAnimator();

                    switch (GetComponent<Unit>().direction)
                    {
                        case UnitDirection.RIGHT:
                            transform.position += new Vector3(GetComponent<Unit>().moveSpeed, 0, 0);
                            break;

                        case UnitDirection.LEFT:
                            transform.position += new Vector3(-GetComponent<Unit>().moveSpeed, 0, 0);
                            break;

                        case UnitDirection.UP:
                            transform.position += new Vector3(0, GetComponent<Unit>().moveSpeed, 0);
                            break;

                        case UnitDirection.DOWN:
                            transform.position += new Vector3(0, -GetComponent<Unit>().moveSpeed, 0);
                            break;
                    }
                }
            }

            if (myPos == GetComponent<Unit>().goal)
            {
                OnSuccessfullyDropped();
            }
        }
    }

    void OnSuccessfullyDropped()
    {
        if (!FindObjectOfType<AIController>().inControl)
            GetComponent<Unit>().Highlight(false);

        GetComponent<Unit>().state = UnitState.WAITING;
        GetComponent<Unit>().ResetDirection();
        GetComponent<Unit>().UpdateAnimator();
        GetComponent<Unit>().lastPosition = transform.position;
        GetComponent<Unit>().UpdateTileInfo();
        GetComponent<Unit>().UpdateStatsBasedOnTile();

        GetComponent<Unit>().EnableUIHitPoints(true);

        //transport
        SuccessfullyDropped.Invoke();
    }

    public IEnumerator OnAI()
    {
        System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
        st.Start();

        if (currentCapture != null) //si ja es troba capturant seguim capturant
        {
            OnCapture();
            yield break;
        }

        FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.MAIN, gameObject);
        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        //buscar edifici dins de rang de moviment
        FindObjectOfType<MapController>().ExecutePathfindingForAI(MapController.Pathfinder.MAIN, 30, gameObject);
        GameObject closestBuilding = GetComponent<Unit>().FindClosestEnemyBuilding(); //aquest edifici no conté cap unitat garantit
        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        if (closestBuilding != null)
        {
            Debug.Log("UnitInfantry::OnAI - Found Building: " + closestBuilding.name + " in Position: " + closestBuilding.transform.position);

            if (EnemyBuildingInRange(closestBuilding))
                yield break;
        }

        //buscar infanteria enemiga capturant
        List<GameObject> targets = GetComponent<Unit>().GetTargetsFromAttackRange();
        GameObject target = GetComponent<Unit>().FindCapturingInfantry(targets);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitInfantry::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindCapturingInfantry(targets);
        }

        //buscar ranged enemic dins de rang de moviment
        target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.RANGED);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitInfantry::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.RANGED);
        }

        //buscar infanteria enemiga dins de rang de moviment
        target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.INFANTRY);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitInfantry::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.INFANTRY);
        }

        //buscar transport enemic dins de rang de moviment
        target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.TRANSPORT);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitInfantry::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.TRANSPORT);
        }

        //buscar transport dins de rang de moviment
        List<GameObject> allies = GetComponent<Unit>().GetAlliesFromMoveRange();
        GameObject transport = FindAvailableTransport(allies);

        if (transport != null)
        {
            Debug.Log("UnitInfantry::OnAI - Found Available Transport: " + transport.name + " in Position: " + transport.transform.position);
            GetComponent<Unit>().OnMove(new Vector2Int((int)transport.transform.position.x, (int)transport.transform.position.y));
            GetComponent<Unit>().finishedMoving.AddListener(OnLoad);
            yield break;
        }

        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        //buscar edifici més proper fora de rang de moviment
        if (closestBuilding != null)
        {
            RoadToEnemyBuilding(closestBuilding);
            yield break;
        }

        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        if (GetComponent<Unit>().ClearFactory())
        {
            GetComponent<Unit>().finishedMoving.AddListener(Decide);
            yield break;
        }

        //si no ha trobat res per fer toca moure cap al punt d'interès més proper (el punt d'interès és una casella col·locada a dit per orientar la IA pel mapa)
        Decide();
    }

    bool EnemyBuildingInRange(GameObject building)
    {
        //retorna true si troba un edifici dins del rang de moviment de la unitat
        Vector2Int goal = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.y);
        Vector2Int nextStep = new Vector2Int(-1, -1);

        if (FindObjectOfType<MapController>().pathfinding.visited.Contains(goal) && GetComponent<Unit>().CheckTileForAlly(new Vector3(goal.x, goal.y)) == null)
            nextStep = goal;

        if (nextStep != new Vector2Int(-1, -1))
        {
            GetComponent<Unit>().OnMove(nextStep);
            GetComponent<Unit>().finishedMoving.AddListener(OnCapture);
            return true;
        }
        
        return false;
    }

    GameObject FindAvailableTransport(List<GameObject> allies)
    {
        GameObject ret = null;

        foreach (GameObject ally in allies)
        {
            if (ally.GetComponent<Unit>().unitType == UnitType.TRANSPORT)
            {
                if (ally.GetComponent<UnitTransport>().loadedUnit == null)//si no està ocupat
                {
                    ret = ally;
                    break;
                }
            }
        }

        return ret;
    }

    void RoadToEnemyBuilding(GameObject building)
    {
        Debug.Log("UnitInfantry::RoadToEnemyBuilding");
        
        Vector2Int goal = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.y);
        Vector2Int nextStep = new Vector2Int(-1, -1);

        FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.AUXILIAR, goal, gameObject, 30); //executem pathfinding al revés, és a dir des de la casella objectiu
        List<Vector2Int> intersections = FindObjectOfType<MapController>().GetTilesInCommon();

        foreach (Vector2Int intersection in intersections)
        {
            if (GetComponent<Unit>().CheckTileForAlly(new Vector3(intersection.x, intersection.y)) == null)
            {
                nextStep = intersection;
                Debug.Log("UnitInfantry::RoadToEnemyBuilding - Found Closest Available Tile to Goal at Position: " + nextStep);
                break;
            }
        }

        if (nextStep != new Vector2Int(-1, -1))
        {
            GetComponent<Unit>().OnMove(nextStep);
        }
        else
        {
            GetComponent<Unit>().OnMove(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        }

        GetComponent<Unit>().finishedMoving.AddListener(Decide);
    }

    void Decide()
    {
        GetComponent<Unit>().finishedMoving.RemoveListener(Decide);

        //GameObject closestBuilding = GetComponent<Unit>().FindClosestEnemyBuilding();

        GetComponent<Unit>().OnWait();
    }
}
