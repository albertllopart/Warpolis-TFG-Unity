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
        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().DisableMenuUnit();
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
        GetComponent<Unit>().state = UnitState.WAITING;
        GetComponent<Unit>().ResetDirection();
        GetComponent<Unit>().UpdateAnimator();

        GetComponent<Unit>().Highlight(false);
        GetComponent<Unit>().lastPosition = transform.position;
        GetComponent<Unit>().UpdateTileInfo();
        GetComponent<Unit>().UpdateStatsBasedOnTile();

        GetComponent<Unit>().EnableUIHitPoints(true);

        //transport
        SuccessfullyDropped.Invoke();
    }
}
