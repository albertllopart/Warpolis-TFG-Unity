using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRanged : MonoBehaviour
{
    public uint minRange;
    public uint maxRange;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Unit>().UITarget = transform.Find("Targeting").gameObject;
        GetComponent<Unit>().EnableUITarget(false);
        GetComponent<Unit>().UIDamageInfo = transform.Find("Damage_info").gameObject;
        GetComponent<Unit>().EnableUIDamageInfo(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManageButtons()
    {
        if (GetComponent<Unit>().lastPosition == transform.position)
        {
            SearchForTargets();
        }

        if (GetComponent<Unit>().targets.Count > 0)
            GetComponent<Unit>().EnableAttackButton(true);
        else
            GetComponent<Unit>().EnableAttackButton(false);
    }

    void SearchForTargets()
    {
        GameObject mapController = GameObject.Find("Map Controller");
        mapController.GetComponent<MapController>().ExecuteRangedPathfinding(MapController.Pathfinder.MAIN, gameObject);

        List<Vector2Int> nodes = mapController.GetComponent<MapController>().pathfinding.rangedAttackRange;

        int layer = 0;

        if (GetComponent<Unit>().army == UnitArmy.CANI)
        {
            layer = LayerMask.GetMask("Hipster_units");
        }
        else if (GetComponent<Unit>().army == UnitArmy.HIPSTER)
        {
            layer = LayerMask.GetMask("Cani_units");
        }

        GetComponent<Unit>().targets = new List<GameObject>();

        foreach (Vector2Int node in nodes)
        {
            Vector2 from = node; from += new Vector2(0.5f, -0.5f); //establim el punt de partida al centre de la casella
            Vector2 to = from;

            RaycastHit2D result = Physics2D.Linecast(from, to, layer);
            if (result.collider != null)
            {
                if (result.collider.gameObject.GetComponent<Unit>().unitType != UnitType.AERIAL) //ranged no pot atacar aerial
                {
                    GetComponent<Unit>().targets.Add(result.collider.gameObject);
                    Debug.Log("UnitRanged::SearchForTargets - Found target: " + result.collider.gameObject.name + " at position: " + result.collider.transform.position);
                }
            }
        }      
    }
}
