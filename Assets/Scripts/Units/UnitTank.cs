using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTank : MonoBehaviour
{
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

    public IEnumerator OnAI()
    {
        System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
        st.Start();

        FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.MAIN, gameObject);
        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        //buscar infanteria enemiga capturant
        List<GameObject> targets = GetComponent<Unit>().GetTargetsFromAttackRange();
        GameObject target = GetComponent<Unit>().FindCapturingInfantry(targets);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitTank::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
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
                Debug.Log("UnitTank::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.RANGED);
        }

        //buscar gunner enemic dins de rang de moviment
        target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.GUNNER);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitTank::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.GUNNER);
        }

        //buscar tank enemic dins de rang de moviment
        target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.TANK);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitTank::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.TANK);
        }

        //buscar transport enemic dins de rang de moviment
        target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.TRANSPORT);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitTank::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.TRANSPORT);
        }

        //buscar infanteria enemic dins de rang de moviment
        target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.INFANTRY);

        while (target != null)
        {
            targets.Remove(target);

            if (GetComponent<Unit>().AttackEnemyInRange(target))
            {
                Debug.Log("UnitTank::OnAI - Found Target: " + target.name + "in Position: " + target.transform.position);
                yield break;
            }

            target = GetComponent<Unit>().FindClosestUnit(targets, UnitType.INFANTRY);
        }

        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        //buscar enemic més proper fora del rang de moviment
        FindObjectOfType<MapController>().ExecutePathfindingForAI(MapController.Pathfinder.MAIN, 50, gameObject);
        if (FindObjectOfType<AIController>().CheckRoutine(st))
            yield return null;

        targets = GetComponent<Unit>().GetTargetsFromAIPathfinding();
        target = GetComponent<Unit>().FindClosestUnitAndAvoid(targets, UnitType.AERIAL);

        if (target != null)
        {
            RoadToEnemy(target);
            yield break;
        }

        if (GetComponent<Unit>().ClearFactory())
        {
            GetComponent<Unit>().finishedMoving.AddListener(Decide);
            yield break;
        }

        //si no ha trobat res per fer toca moure cap al punt d'interès més proper (el punt d'interès és una casella col·locada a dit per orientar la IA pel mapa)
        Decide();
    }

    void RoadToEnemy(GameObject target)
    {
        Debug.Log("UnitTank::RoadToEnemy");

        Vector2Int goal = new Vector2Int((int)target.transform.position.x, (int)target.transform.position.y);
        Vector2Int nextStep = new Vector2Int(-1, -1);

        FindObjectOfType<MapController>().ExecutePathfinding(MapController.Pathfinder.AUXILIAR, goal, gameObject, 50); //executem pathfinding al revés, és a dir des de la casella objectiu
        List<Vector2Int> intersections = FindObjectOfType<MapController>().GetTilesInCommon();

        foreach (Vector2Int intersection in intersections)
        {
            if (GetComponent<Unit>().CheckTileForAlly(new Vector3(intersection.x, intersection.y)) == null)
            {
                nextStep = intersection;
                Debug.Log("UnitTank::RoadToEnemy - Found Closest Available Tile to Goal at Position: " + nextStep);
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
        GetComponent<Unit>().OnWait();
    }
}
