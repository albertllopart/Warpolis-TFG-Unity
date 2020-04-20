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
