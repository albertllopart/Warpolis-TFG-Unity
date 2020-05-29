using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTransport : MonoBehaviour
{
    [Header("Load")]
    GameObject UILoadSign;
    public GameObject loadedUnit;

    [Header("Drop")]
    public List<Vector2> dropPositions;
    public Vector2 currentDropPosition;

    // Start is called before the first frame update
    void Start()
    {
        UILoadSign = transform.Find("Load").gameObject;
        UILoadSign.SetActive(false);

        GetComponent<Unit>().UITarget = transform.Find("Targeting").gameObject;
        GetComponent<Unit>().EnableUITarget(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MyOnDestroy()
    {
        if (loadedUnit != null)
        {
            switch (loadedUnit.GetComponent<Unit>().army)
            {
                case UnitArmy.CANI:
                    GetComponentInParent<UnitsController>().caniUnits.Remove(loadedUnit);
                    break;

                case UnitArmy.HIPSTER:
                    GetComponentInParent<UnitsController>().hipsterUnits.Remove(loadedUnit);
                    break;
            }

            Destroy(loadedUnit);
        }
    }

    public void EnableLoadSign(bool enable)
    {
        UILoadSign.SetActive(enable);
    }

    public void ManageButtons()
    {
        if (loadedUnit != null)
        {
            if (SearchForDropPositions().Count > 0)
                EnableDropButton(true);
            else
                EnableDropButton(false);
        }
        else
        {
            EnableDropButton(false);
        }
    }

    List<Vector2> SearchForDropPositions()
    {
        List<Vector2> ret = new List<Vector2>();

        Vector2 north = transform.position + new Vector3(0, 1, 0);
        Vector2 east = transform.position + new Vector3(1, 0, 0);
        Vector2 south = transform.position + new Vector3(0, -1, 0);
        Vector2 west = transform.position + new Vector3(-1, 0, 0);

        List<Vector2> positions = new List<Vector2>();
        positions.Add(north);
        positions.Add(east);
        positions.Add(south);
        positions.Add(west);

        GameObject map = GameObject.Find("Map Controller");

        foreach (Vector2 position in positions)
        {
            if (map.GetComponent<MapController>().GetMyTile(position).isWalkable)
            {
                if (!map.GetComponent<MapController>().GetMyTile(position).containsCani && !map.GetComponent<MapController>().GetMyTile(position).containsHipster)
                {
                    ret.Add(position);
                }
                else if (new Vector3(position.x, position.y, 0) == GetComponent<Unit>().lastPosition) //mirem la posició d'on prové el transport per no haver d'actualitzar tile info
                {
                    ret.Add(position);
                }
            }
        }

        return ret;
    }

    void EnableDropButton(bool enable)
    {
        GetComponent<Unit>().activeButtons[4] = enable;
    }   
    
    public void OnTargetingDropPositions()
    {
        GetComponent<Unit>().state = UnitState.TARGETING;

        dropPositions = SearchForDropPositions();

        if (dropPositions.Count > 0)
        {
            currentDropPosition = dropPositions[0];
            Target(currentDropPosition);
            GetComponent<Unit>().UITarget.SetActive(true);
        }

        SubscribeToEvents(); //necessito saber quan s'apreten les direccions
    }

    void Target(Vector2 position)
    {
        GetComponent<Unit>().UITarget.transform.position = position;

        GameObject.Find("UI Controller").GetComponent<UIController>().EnableTileInfo();
        GameObject.Find("Tile_info").GetComponent<TileInfo>().UpdateInfo(position);
    }

    void SelectNextDropPosition()
    {
        if (GetComponent<Unit>().switchTargetTimer >= GetComponent<Unit>().switchTargetInterval)
        {
            if (dropPositions.Count > 1)
                FindObjectOfType<SoundController>().PlayPlayerMove();

            if (dropPositions.Count > 0)
            {
                int currentDropPositionIndex = dropPositions.IndexOf(currentDropPosition);

                if (currentDropPositionIndex == dropPositions.Count - 1)
                    currentDropPosition = dropPositions[0];
                else
                    currentDropPosition = dropPositions[++currentDropPositionIndex];
            }

            Target(currentDropPosition);

            GetComponent<Unit>().switchTargetTimer = 0.0f;
        }
    }

    void SelectPreviousDropPosition()
    {
        if (GetComponent<Unit>().switchTargetTimer >= GetComponent<Unit>().switchTargetInterval)
        {
            if (dropPositions.Count > 1)
                FindObjectOfType<SoundController>().PlayPlayerMove();

            if (dropPositions.Count > 0)
            {
                int currentDropPositionIndex = dropPositions.IndexOf(currentDropPosition);

                if (currentDropPositionIndex == 0)
                    currentDropPosition = dropPositions[dropPositions.Count - 1];
                else
                    currentDropPosition = dropPositions[--currentDropPositionIndex];
            }


            Target(currentDropPosition);

            GetComponent<Unit>().switchTargetTimer = 0.0f;
        }
    }

    void OnDrop()
    {
        //infantry
        Vector2Int origin = new Vector2Int(0, 0);
        origin.x = (int)transform.position.x;
        origin.y = (int)transform.position.y;
        Vector2Int goal = new Vector2Int(0, 0);
        goal.x = (int)currentDropPosition.x;
        goal.y = (int)currentDropPosition.y;

        loadedUnit.SetActive(true);
        loadedUnit.transform.position = transform.position;
        loadedUnit.GetComponent<UnitInfantry>().OnDropped(origin, goal);
        loadedUnit.GetComponent<UnitInfantry>().SuccessfullyDropped.AddListener(SuccessfullyDroppedListener);
    }

    void SuccessfullyDroppedListener()
    {
        loadedUnit.GetComponent<UnitInfantry>().SuccessfullyDropped.RemoveListener(SuccessfullyDroppedListener);
        loadedUnit = null;

        UnsubscribeFromEvents();
        GetComponent<Unit>().OnWait();
        GetComponent<Unit>().Untarget();
        EnableLoadSign(false);
    }

    public IEnumerator OnAI()
    {
        System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
        st.Start();

        FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.MAIN, gameObject);
        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        FindObjectOfType<MapController>().ExecutePathfindingForAI(MapController.Pathfinder.MAIN, 50, gameObject);
        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        if (loadedUnit != null)
        {
            //buscar edifici més proper i descarregar-hi la unitat
            GameObject closestBuilding = GetComponent<Unit>().FindClosestEnemyBuilding();

            if (closestBuilding != null)
            {
                Debug.Log("UnitTransport::OnAI - Found Building: " + closestBuilding.name + " in Position: " + closestBuilding.transform.position);
                if (RoadToBuilding(closestBuilding))
                    yield break;
            }
        }
        else
        {
            //buscar una infanteria idle i amb vida màxima a la vora
            GameObject ally = LocateAllyInfantryAI();

            if (ally != null)
            {
                if (RoadToAlly(ally))
                    yield break;
            }

            //buscar la infanteria més propera a la base
            GameObject myBase = FindObjectOfType<AIController>().myBase;
            Vector2Int basePosition = new Vector2Int((int)myBase.transform.position.x, (int)myBase.transform.position.y);

            FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.AUXILIAR, basePosition, gameObject, 50);
            if (FindObjectOfType<AIController>().CheckRoutine(st))
                yield return null;

            ally = LocateAllyInfantryAuxiliar();

            if (ally != null)
            {
                if (RoadToAlly(ally))
                    yield break;
            }
        }

        if (GetComponent<Unit>().ClearFactory())
        {
            GetComponent<Unit>().finishedMoving.AddListener(Decide);
            yield break;
        }

        Decide();
    }

    GameObject LocateAllyInfantryAuxiliar()
    {
        GameObject ret = null;

        foreach (Vector2Int tile in FindObjectOfType<MapController>().auxiliarPathfinding.visited)
        {
            GameObject ally = GetComponent<Unit>().CheckTileForAlly(new Vector3(tile.x, tile.y));

            if (ally != null && ally.GetComponent<Unit>().unitType == UnitType.INFANTRY && ally.GetComponent<UnitInfantry>().currentCapture == null) //si la unitat és una infanteria i no està capturant
            {
                ret = ally;
                Debug.Log("UnitTransport::LocateAllyInfantryAuxiliar - Located Idle Ally: " + ally.name + "in Position: " + ally.transform.position);
                break;
            }
        }

        if (ret == null)
            Debug.Log("UnitTransport::LocateAllyInfantryAuxiliar - No Ally Infantry found");

        return ret;
    }

    GameObject LocateAllyInfantryAI()
    {
        GameObject ret = null;

        foreach (Vector2Int tile in FindObjectOfType<MapController>().pathfinding.AIVisited)
        {
            GameObject ally = GetComponent<Unit>().CheckTileForAlly(new Vector3(tile.x, tile.y));

            if (ally != null && ally.GetComponent<Unit>().unitType == UnitType.INFANTRY && ally.GetComponent<Unit>().hitPoints == 50 && ally.GetComponent<UnitInfantry>().currentCapture == null) //si la unitat és una infanteria, té tota la vida i no està capturant
            {
                ret = ally;
                Debug.Log("UnitTransport::LocateAllyInfantryAI - Located Idle Ally: " + ally.name + "in Position: " + ally.transform.position);
                break;
            }
        }

        if (ret == null)
            Debug.Log("UnitTransport::LocateAllyInfantryAI - No Ally Infantry found");

        return ret;
    }

    bool RoadToAlly(GameObject ally)
    {
        Vector2Int goal = new Vector2Int((int)ally.transform.position.x, (int)ally.transform.position.y);
        Vector2Int nextStep = new Vector2Int(-1, -1);

        if (FindObjectOfType<MapController>().pathfinding.visited.Contains(goal))
        {
            //executem pathfinding des de la casella de la infanteria per col·locar-nos dins del seu rang de movimen
            nextStep = GetFurthestTileFromAllyRange(ally);
        }
        else
        {
            FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.AUXILIAR, goal, gameObject, 50); //executem pathfinding al revés, és a dir des de la casella objectiu
            List<Vector2Int> intersections = FindObjectOfType<MapController>().GetTilesInCommon();

            foreach (Vector2Int intersection in intersections)
            {
                if (GetComponent<Unit>().CheckTileForAlly(new Vector3(intersection.x, intersection.y)) == null)
                {
                    nextStep = intersection;
                    Debug.Log("UnitTransport::RoadToAlly - Found Closest Available Tile to Goal at Position: " + nextStep);
                    break;
                }
            }
        }

        if (nextStep != new Vector2Int(-1, -1))
        {
            GetComponent<Unit>().OnMove(nextStep);
            GetComponent<Unit>().finishedMoving.AddListener(Decide);
            return true;
        }

        return false;
    }

    bool RoadToBuilding(GameObject building)
    {
        Vector2Int goal = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.y);
        Vector2Int nextStep = new Vector2Int(-1, -1);

        FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.AUXILIAR, goal, gameObject, 4);
        nextStep = GetClosestTileFromBuilding(building);

        if (nextStep != new Vector2Int(-1, -1)) //vol dir que la casella està dins del rang del transport
        {
            GetComponent<Unit>().OnMove(nextStep);
            GetComponent<Unit>().finishedMoving.AddListener(AttemptUnload);
            return true;
        }
        else
        {
            FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.AUXILIAR, goal, gameObject, 50); //executem pathfinding al revés, és a dir des de la casella objectiu
            List<Vector2Int> intersections = FindObjectOfType<MapController>().GetTilesInCommon();

            foreach (Vector2Int intersection in intersections)
            {
                if (GetComponent<Unit>().CheckTileForAlly(new Vector3(intersection.x, intersection.y)) == null)
                {
                    nextStep = intersection;
                    Debug.Log("UnitTransport::RoadToBuilding - Found Closest Available Tile to Goal at Position: " + nextStep);
                    break;
                }
            }
        }

        if (nextStep != new Vector2Int(-1, -1))
        {
            GetComponent<Unit>().OnMove(nextStep);
            GetComponent<Unit>().finishedMoving.AddListener(Decide);
            return true;
        }

        return false;
    }

    Vector2Int GetFurthestTileFromAllyRange(GameObject ally)
    {
        Vector2Int allyPosition = new Vector2Int((int)ally.transform.position.x, (int)ally.transform.position.y);
        FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.AUXILIAR, ally);

        foreach (Vector2Int tile in FindObjectOfType<MapController>().pathfinding.visited)
        {
            if (FindObjectOfType<MapController>().auxiliarPathfinding.visited.Contains(tile) && GetComponent<Unit>().CheckTileForAlly(new Vector3(tile.x, tile.y)) == null)
                return tile;
        }

        return new Vector2Int(-1, -1);
    }

    Vector2Int GetClosestTileFromBuilding(GameObject building)
    {
        Vector2Int buildingPosition = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.y);

        foreach (Vector2Int tile in FindObjectOfType<MapController>().pathfinding.visited)
        {
            if (FindObjectOfType<MapController>().auxiliarPathfinding.visited.Contains(tile) && GetComponent<Unit>().CheckTileForAlly(new Vector3(tile.x, tile.y)) == null)
                return tile;
        }

        return new Vector2Int(-1, -1);
    }

    void AttemptUnload()
    {
        GetComponent<Unit>().finishedMoving.RemoveListener(AttemptUnload);

        Vector2Int myPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.MAIN, myPos, gameObject, 1);

        foreach (Vector2Int tile in FindObjectOfType<MapController>().auxiliarPathfinding.visited)
        {
            if (FindObjectOfType<MapController>().pathfinding.visited.Contains(tile))
            {
                if (tile != myPos && GetComponent<Unit>().CheckTileForAlly(new Vector3(tile.x, tile.y)) == null)
                {
                    currentDropPosition = tile;
                    OnDrop();
                    return;
                }
            }
        }

        Decide();
    }

    void Decide()
    {
        Debug.Log("UnitTansport::Decide");

        GetComponent<Unit>().finishedMoving.RemoveListener(Decide);

        //GameObject closestBuilding = GetComponent<Unit>().FindClosestEnemyBuilding();

        GetComponent<Unit>().OnWait();
    }

    void SubscribeToEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_w_down.AddListener(SelectNextDropPosition);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_a_down.AddListener(SelectPreviousDropPosition);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_s_down.AddListener(SelectPreviousDropPosition);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_d_down.AddListener(SelectNextDropPosition);

        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().dropUnit.AddListener(OnDrop);
    }

    public void UnsubscribeFromEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_w_down.RemoveListener(SelectNextDropPosition);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_a_down.RemoveListener(SelectPreviousDropPosition);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_s_down.RemoveListener(SelectPreviousDropPosition);
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_d_down.RemoveListener(SelectNextDropPosition);

        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().dropUnit.RemoveListener(OnDrop);
    }
}
