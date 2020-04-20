using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public GameObject selectedUnit;

    private float keyDownCounterW; //per calcular el temps que el botó porta premut
    private float keyDownCounterA;
    private float keyDownCounterS;
    private float keyDownCounterD;
            
    private float nextMoveCounterW; //per calcular el temps que cal esperar a moure una casella mentre el botó està premut
    private float nextMoveCounterA;
    private float nextMoveCounterS;
    private float nextMoveCounterD;

    public float keyDownSpeed = 0.25f;
    public float nextMoveSpeed = 0.1f;

    private uint xCameraMovementOffset = 2;
    private uint yCameraMovementOffset = 1; // la x és 2 perquè necessita tenir en compte la oclusió dels laterals

    //events
    public UnityEvent playerMoved;
    public UnityEvent playerMovedSound;

    // Start is called before the first frame update
    void Start()
    {
        playerMoved = new UnityEvent();
        playerMovedSound = new UnityEvent();

        keyDownCounterW = 0.0f;
        keyDownCounterA = 0.0f;
        keyDownCounterS = 0.0f;
        keyDownCounterD = 0.0f;

        nextMoveCounterW = 0.0f;
        nextMoveCounterA = 0.0f;
        nextMoveCounterS = 0.0f;
        nextMoveCounterD = 0.0f;

        DeterminePlayerLocation();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        LogPosition();
    }

    public void MyOnEnable()
    {
        SubscribeToEvents();
        CheckArrow();
    }
    public void MyOnDisable()
    {
        UnsubscribeFromEvents();
    }

    void DeterminePlayerLocation()
    {
        uint cameraMiddle = (uint)Camera.main.gameObject.GetComponent<CameraController>().GetTopLeftCorner().x +
                                  Camera.main.gameObject.GetComponent<CameraController>().GetCameraWidth() / 2;

        if (transform.position.x < cameraMiddle)
            GetComponentInParent<GameplayController>().playerLocation = GameplayController.PlayerLocation.LEFT;
        else
            GetComponentInParent<GameplayController>().playerLocation = GameplayController.PlayerLocation.RIGHT;
    }

    // Moviment
    void Move()
    {
        //he traslladat la major part del moviment a events. El que hi he deixat no pot escoltar events perquè rep paràmetres

        if (Input.GetKeyUp("w"))
        {
            resetCounterParameters(0);
        }
        if (Input.GetKeyUp("a"))
        {
            resetCounterParameters(1);
        }
        if (Input.GetKeyUp("s"))
        {
            resetCounterParameters(2);
        }
        if (Input.GetKeyUp("d"))
        {
            resetCounterParameters(3);
        }
    }

    void resetCounterParameters(uint key)
    {
        if (key == 0) // W
        {
            keyDownCounterW = 0.0f;
            nextMoveCounterW = 0.0f;
        }
        if (key == 1) // A
        {
            keyDownCounterA = 0.0f;
            nextMoveCounterA = 0.0f;
        }
        if (key == 2) // S
        {
            keyDownCounterS = 0.0f;
            nextMoveCounterS = 0.0f;
        }
        if (key == 3) // D
        {
            keyDownCounterD = 0.0f;
            nextMoveCounterD = 0.0f;
        }
    }

    void checkEnhancedW()
    {
        if (keyDownCounterW < keyDownSpeed)
            keyDownCounterW += Time.deltaTime;

        nextMoveCounterW += Time.deltaTime;

        if (keyDownCounterW >= keyDownSpeed)
        {
            if (nextMoveCounterW >= nextMoveSpeed)
            {
                MovePlayerUp();
                nextMoveCounterW = 0.0f;
            }
        }
    }

    void checkEnhancedA()
    {
        if (keyDownCounterA < keyDownSpeed)
            keyDownCounterA += Time.deltaTime;

        nextMoveCounterA += Time.deltaTime;

        if (keyDownCounterA >= keyDownSpeed)
        {
            if (nextMoveCounterA >= nextMoveSpeed)
            {
                MovePlayerLeft();
                nextMoveCounterA = 0.0f;
            }
        }
    }

    void checkEnhancedS()
    {
        if (keyDownCounterS < keyDownSpeed)
            keyDownCounterS += Time.deltaTime;

        nextMoveCounterS += Time.deltaTime;

        if (keyDownCounterS >= keyDownSpeed)
        {
            if (nextMoveCounterS >= nextMoveSpeed)
            {
                MovePlayerDown();
                nextMoveCounterS = 0.0f;
            }
        }
    }

    void checkEnhancedD()
    {
        if (keyDownCounterD < keyDownSpeed)
            keyDownCounterD += Time.deltaTime;

        nextMoveCounterD += Time.deltaTime;

        if (keyDownCounterD >= keyDownSpeed)
        {
            if (nextMoveCounterD >= nextMoveSpeed)
            {
                MovePlayerRight();
                nextMoveCounterD = 0.0f;
            }
        }
    }

    public void MovePlayerUp()
    {
        if (Camera.main.GetComponent<CameraController>().GetTopLeftCorner().y > transform.position.y)
        {
            transform.position += new Vector3(0, 1, 0);

            if (CheckCameraBoundaries(0))
                Camera.main.GetComponent<CameraController>().MoveCameraUp();

            OnMove();
            playerMovedSound.Invoke();
        }
    }

    public void MovePlayerLeft()
    {
        if (Camera.main.GetComponent<CameraController>().GetTopLeftCorner().x < transform.position.x - 1) // -1 per tenir en compte la oclusió
        {
            transform.position += new Vector3(-1, 0, 0);

            if (CheckCameraBoundaries(1))
                Camera.main.GetComponent<CameraController>().MoveCameraLeft();

            OnMove();
            playerMovedSound.Invoke();
        }
    }

    public void MovePlayerDown()
    {
        if (Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y < transform.position.y)
        {
            transform.position += new Vector3(0, -1, 0);

            if (CheckCameraBoundaries(2))
                Camera.main.GetComponent<CameraController>().MoveCameraDown();

            OnMove();
            playerMovedSound.Invoke();
        }
    }

    public void MovePlayerRight()
    {
        if (Camera.main.GetComponent<CameraController>().GetBottomRightCorner().x > transform.position.x + 1) // +1 per tenir en compte la oclusió
        {
            transform.position += new Vector3(1, 0, 0);

            if (CheckCameraBoundaries(3))
                Camera.main.GetComponent<CameraController>().MoveCameraRight();

            OnMove();
            playerMovedSound.Invoke();
        }
    }

    void OnMove()
    {
        CheckArrow();
        DeterminePlayerLocation();

        playerMoved.Invoke();

        //TODO: fer que això es cridi amb event
        GameObject.Find("UI Controller").transform.Find("Tile_info").GetComponent<TileInfo>().UpdateInfo(transform.position);
        GameObject.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdatePosition();
    }

    public void Reposition()
    {
        GameplayController.Turn turn = GetComponentInParent<GameplayController>().GetTurn();

        switch (turn)
        {
            case GameplayController.Turn.CANI:
                transform.position = GameObject.Find("Data Controller").GetComponent<DataController>().playerCaniPosition;
                OnMove();
                break;

            case GameplayController.Turn.HIPSTER:
                transform.position = GameObject.Find("Data Controller").GetComponent<DataController>().playerHipsterPosition;
                OnMove();
                break;
        }
    }

    public void CheckArrow()
    {
        Vector2Int myPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        if (selectedUnit != null &&
            selectedUnit.GetComponent<Unit>().GetState() == UnitState.SELECTED &&
            selectedUnit.GetComponent<Unit>().myTurn &&
            GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.visited.Contains(myPos))
        {
            GameObject.Find("Map Controller").GetComponent<MapController>().DrawArrow();
        }
    }

    bool CheckCameraBoundaries(int direction)
    {
        Vector2 cameraTopLeft = Camera.main.GetComponent<CameraController>().GetTopLeftCorner();
        Vector2 cameraBottomRight = Camera.main.GetComponent<CameraController>().GetBottomRightCorner();
        Vector2 mapTopLeft = GameObject.Find("Map Controller").GetComponent<MapController>().GetTopLeftCorner();
        Vector2 mapBottomRight = GameObject.Find("Map Controller").GetComponent<MapController>().GetBottomRightCorner();

        switch (direction)
        {
            case 0: // W
                if (cameraTopLeft.y - yCameraMovementOffset < gameObject.transform.position.y
                    && cameraTopLeft.y < mapTopLeft.y)
                    return true;
                break;

            case 1: // A
                if (cameraTopLeft.x + xCameraMovementOffset > gameObject.transform.position.x
                    && cameraTopLeft.x > mapTopLeft.x)
                    return true;
                break;

            case 2: // S
                if (cameraBottomRight.y + yCameraMovementOffset > gameObject.transform.position.y
                    && cameraBottomRight.y > mapBottomRight.y)
                    return true;
                break;

            case 3: // D
                if (cameraBottomRight.x - xCameraMovementOffset < gameObject.transform.position.x
                    && cameraBottomRight.x < mapBottomRight.x)
                    return true;
                break;
        }

        return false;
    }

    // Interacció

    public bool InteractUnits()
    {
        List<RaycastHit2D> results = new List<RaycastHit2D>();

        RaycastHit2D resultCani = RayCast(LayerMask.GetMask("Cani_units"));
        if (resultCani.collider != null)
            results.Add(resultCani);

        RaycastHit2D resultHipster = RayCast(LayerMask.GetMask("Hipster_units"));
        if (resultHipster.collider != null)
            results.Add(resultHipster);

        foreach (RaycastHit2D result in results)
        {
            if (result.collider != null)
            {
                if (result.collider.gameObject.GetComponent<Unit>().GetState() != UnitState.WAITING)
                {
                    Debug.Log("PlayerController::Interact - Interacting with " + result.collider.gameObject.name);

                    //seleccionar unitat
                    selectedUnit = result.collider.gameObject;

                    //cridar mètode OnSelected
                    selectedUnit.GetComponent<Unit>().OnSelected();

                    return true;
                }
                else if (result.collider.gameObject.GetComponent<Unit>().GetState() == UnitState.WAITING) //perquè el gameplay controller sàpiga que hi ha una unitat a la casella tot i no ser seleccionable
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool InteractUnitsForAttackRange()
    {
        List<RaycastHit2D> results = new List<RaycastHit2D>();

        RaycastHit2D resultCani = RayCast(LayerMask.GetMask("Cani_units"));
        if (resultCani.collider != null)
            results.Add(resultCani);

        RaycastHit2D resultHipster = RayCast(LayerMask.GetMask("Hipster_units"));
        if (resultHipster.collider != null)
            results.Add(resultHipster);

        foreach (RaycastHit2D result in results)
        {
            if (result.collider != null)
            {
                if (result.collider.gameObject.GetComponent<Unit>().GetState() != UnitState.WAITING && result.collider.gameObject.GetComponent<Unit>().basePower > 0)
                {
                    Debug.Log("PlayerController::Interact - Interacting for Attack Range with " + result.collider.gameObject.name);

                    //seleccionar unitat
                    selectedUnit = result.collider.gameObject;

                    //cridar mètode OnSelected
                    selectedUnit.GetComponent<Unit>().OnSelectedForAttackRange();

                    return true;
                }
            }
        }

        return false;
    }

    public bool InteractBuildings()
    {
        RaycastHit2D result;
        string tag = null;

        if (GetComponentInParent<GameplayController>().GetTurn() == GameplayController.Turn.CANI)
        {
            result = RayCast(LayerMask.GetMask("Cani_buildings"));
            tag = "Factory_cani";
        }
        else
        {
            result = RayCast(LayerMask.GetMask("Hipster_buildings"));
            tag = "Factory_hipster";
        }

        if (result.collider != null && result.collider.gameObject.CompareTag(tag))
        {
            Debug.Log("PlayerController::Interact - Interacting with " + result.collider.gameObject.name);
            return true;
        }

        return false;
    }

    public RaycastHit2D RayCast(int layer)
    {
        Vector2 from = transform.position + new Vector3(0.5f, -0.5f, 0);
        Vector2 to = from;

        return Physics2D.Linecast(from, to, layer);
    }

    void DeselectUnit()
    {
        GameObject.Find("Map Controller").GetComponent<MapController>().UndrawArrow();
        selectedUnit.GetComponent<Unit>().OnDeselected();
        selectedUnit = null;
    }

    void MoveUnit()
    {
        if (selectedUnit != null && selectedUnit.GetComponent<Unit>().myTurn)
        {
            Vector2Int goal = new Vector2Int((int)transform.position.x, (int)transform.position.y);

            //faig un raig per mirar si hi ha una unitat per saber si podem moure allà
            RaycastHit2D result;

            if (GetComponentInParent<GameplayController>().GetTurn() == GameplayController.Turn.CANI)
                result = RayCast(LayerMask.GetMask("Cani_units"));
            else
                result = RayCast(LayerMask.GetMask("Hipster_units"));

            if (result.collider == null || result.collider.gameObject == selectedUnit) //la segona comprovació és per poder fer una acció al mateix lloc on la unitat era
            {
                if (selectedUnit != null && GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.visited.Contains(goal))
                {
                    GameObject.Find("Map Controller").GetComponent<MapController>().UndrawArrow();
                    selectedUnit.GetComponent<Unit>().OnMove(goal);
                }
            }
            else if (selectedUnit.GetComponent<Unit>().unitType == (uint)UnitType.INFANTRY && result.collider.gameObject.GetComponent<Unit>().unitType == UnitType.TRANSPORT)
            {
                if (result.collider.gameObject.GetComponent<UnitTransport>().loadedUnit == null) //mirem que no hi hagi ja una unitat carregada
                {
                    if (selectedUnit != null && GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.visited.Contains(goal))
                    {
                        GameObject.Find("Map Controller").GetComponent<MapController>().UndrawArrow();
                        selectedUnit.GetComponent<Unit>().OnMove(goal);
                    }
                }
            }
        }
    }

    // Debug

    void LogPosition()
    {
        if (Input.GetKeyDown("f1"))
        {
            string locationString = null;

            switch (GetComponentInParent<GameplayController>().playerLocation)
            {
                case GameplayController.PlayerLocation.LEFT:
                    locationString = "Left";
                    break;

                case GameplayController.PlayerLocation.RIGHT:
                    locationString = "Right";
                    break;
            }

            MyTile tile = GameObject.Find("Map Controller").GetComponent<MapController>().pathfinding.MyTilemap[(int)transform.position.x, -(int)transform.position.y];
            string tileType = "null";
            string isWalkable = "null";

            switch (tile.type)
            {
                case MyTileType.NEUTRAL:
                    tileType = "Neutral";
                    break;

                case MyTileType.ROAD:
                    tileType = "Road";
                    break;

                case MyTileType.SEA:
                    tileType = "Sea";
                    break;

                case MyTileType.CONE:
                    tileType = "Cone";
                    break;

                case MyTileType.PLANTPOT:
                    tileType = "Plantpot";
                    break;

                case MyTileType.LAMP:
                    tileType = "Lamp";
                    break;

                case MyTileType.BUILDING:
                    tileType = "Building";
                    break;
            }
            Debug.Log("PlayerController::LogPosition - " + transform.position + 
                      ", Player Location = " + locationString + 
                      ", Tile info: " + tileType + ", Is Walkable = " + tile.isWalkable +  
                      ", Contains Cani: " + tile.containsCani + ", Contains Hipster: " + tile.containsHipster);
        }
    }

    void SubscribeToEvents()
    {
        //controls
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_w.AddListener(checkEnhancedW);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_w_down.AddListener(MovePlayerUp);

        GameObject.Find("Controls").GetComponent<Controls>().keyboard_a.AddListener(checkEnhancedA);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_a_down.AddListener(MovePlayerLeft);

        GameObject.Find("Controls").GetComponent<Controls>().keyboard_s.AddListener(checkEnhancedS);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_s_down.AddListener(MovePlayerDown);

        GameObject.Find("Controls").GetComponent<Controls>().keyboard_d.AddListener(checkEnhancedD);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_d_down.AddListener(MovePlayerRight);

        //gameplay
        GetComponentInParent<GameplayController>().deselectUnit.AddListener(DeselectUnit);
        GetComponentInParent<GameplayController>().moveUnit.AddListener(MoveUnit);

        //cutscene
        GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().repositionPlayer.AddListener(Reposition);
    }

    void UnsubscribeFromEvents()
    {
        //controls
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_w.RemoveListener(checkEnhancedW);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_w_down.RemoveListener(MovePlayerUp);

        GameObject.Find("Controls").GetComponent<Controls>().keyboard_a.RemoveListener(checkEnhancedA);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_a_down.RemoveListener(MovePlayerLeft);

        GameObject.Find("Controls").GetComponent<Controls>().keyboard_s.RemoveListener(checkEnhancedS);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_s_down.RemoveListener(MovePlayerDown);

        GameObject.Find("Controls").GetComponent<Controls>().keyboard_d.RemoveListener(checkEnhancedD);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_d_down.RemoveListener(MovePlayerRight);

        //gameplay
        GetComponentInParent<GameplayController>().deselectUnit.RemoveListener(DeselectUnit);
        GetComponentInParent<GameplayController>().moveUnit.RemoveListener(MoveUnit);

        //cutscene
        GameObject.Find("Cutscene Controller").GetComponent<CutsceneController>().repositionPlayer.RemoveListener(Reposition);
    }
}
