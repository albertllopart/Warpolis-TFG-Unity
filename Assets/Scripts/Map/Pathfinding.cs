using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BFS_Node
{
    public BFS_Node(Vector2Int data, Vector2Int parent)
    {
        this.data = data;
        this.parent = parent;
    }

    public Vector2Int data;
    public Vector2Int parent;

    public bool Equals(BFS_Node other)
    {
        if (data == other.data && parent == other.parent)
            return true;
        return false;
    }
}

public enum MyTileType
{
    NEUTRAL, ROAD, CONTAINER, LAMP, BUILDING, PLANTPOT, SEA, CONE
};

public enum Applicant
{
    HUMAN, AI
};

public class MyTile
{
    public MyTile(Vector2Int position, bool isWalkable, MyTileType type, uint maxWidth, uint maxHeight)
    {
        this.position = position;
        this.isWalkable = isWalkable;
        this.type = type;
        this.maxWidth = maxWidth;
        this.maxHeight = maxHeight;
    }

    //info
    public uint maxWidth;
    public uint maxHeight;

    public Vector2Int position;
    public bool isWalkable;
    public MyTileType type;

    public bool containsCani;
    public bool containsHipster;
}

public class Pathfinding
{
    public MyTile[,] MyTilemap; //això s'inicialitza des del MapController amb les mides del mapa

    public Queue<BFS_Node> frontier = new Queue<BFS_Node>();
    public List<Vector2Int> visited = new List<Vector2Int>();
    public List<Vector2Int> attackRange = new List<Vector2Int>();
    public List<BFS_Node> backtrack = new List<BFS_Node>();

    //ranged
    public Queue<BFS_Node> rangedFrontier = new Queue<BFS_Node>();
    public List<Vector2Int> rangedVisited = new List<Vector2Int>();
    public List<Vector2Int> rangedAttackRange = new List<Vector2Int>();
    public List<BFS_Node> rangedBacktrack = new List<BFS_Node>();

    //AI
    public Queue<BFS_Node> AIFrontier = new Queue<BFS_Node>();
    public List<Vector2Int> AIVisited = new List<Vector2Int>();
    public List<BFS_Node> AIBacktrack = new List<BFS_Node>();

    public void ResetBFS(Vector2Int position)
    {
        frontier.Clear();
        visited.Clear();
        attackRange.Clear();
        backtrack.Clear();

        BFS_Node node = new BFS_Node(position, new Vector2Int(-1, -1));
        frontier.Enqueue(node);
        visited.Add(node.data);
        attackRange.Add(node.data);
        backtrack.Add(node);
    }

    public void ResetRangedBFS(Vector2Int position)
    {
        rangedFrontier.Clear();
        rangedVisited.Clear();
        rangedAttackRange.Clear();
        rangedBacktrack.Clear();

        BFS_Node node = new BFS_Node(position, new Vector2Int(-1, -1));
        rangedFrontier.Enqueue(node);
        rangedVisited.Add(node.data);
        rangedBacktrack.Add(node);
    }

    public void ResetAIBFS(Vector2Int position)
    {
        AIFrontier.Clear();
        AIVisited.Clear();
        AIBacktrack.Clear();

        BFS_Node node = new BFS_Node(position, new Vector2Int(-1, -1));
        AIFrontier.Enqueue(node);
        AIVisited.Add(node.data);
        AIBacktrack.Add(node);
    }

    public void PropagateBFS(GameObject unit)
    {
        int safety = 0;

        //agafar dades de la unitat
        Unit unitScript = unit.GetComponent<Unit>();

        while (frontier.Count != 0)
        {
            BFS_Node popped = frontier.Dequeue();

            if (MyTilemap[popped.data.x, -popped.data.y] != null)
            {
                BFS_Node north = new BFS_Node(popped.data + new Vector2Int(0, 1), popped.data);
                BFS_Node south = new BFS_Node(popped.data + new Vector2Int(0, -1), popped.data);
                BFS_Node east = new BFS_Node(popped.data + new Vector2Int(1, 0), popped.data);
                BFS_Node west = new BFS_Node(popped.data + new Vector2Int(-1, 0), popped.data);

                List<BFS_Node> unorderedNodes = new List<BFS_Node>();

                if (CheckMapLimits(north.data))
                    unorderedNodes.Add(north);
                if (CheckMapLimits(south.data))
                    unorderedNodes.Add(south);
                if (CheckMapLimits(east.data))
                    unorderedNodes.Add(east);
                if (CheckMapLimits(west.data))
                    unorderedNodes.Add(west);

                List<BFS_Node> orderedNodes = OrderBFS_Nodes(unorderedNodes);

                foreach (BFS_Node node in orderedNodes)
                {
                    if (!CheckEnemyInPos(node.data, unit)) //primer mira si hi ha un enemic a la casella i després si està a rang i és walkable
                    {
                        if (IsInMoveRange(unitScript.movementRange, unitScript, visited[0], node, Applicant.HUMAN))
                        {
                            if (unitScript.unitType != UnitType.AERIAL)
                                CheckNode(node);
                            else
                                CheckNodeAerial(node);
                        }
                    }

                    CheckNodeForAttackRange(node);
                }
            }
            else
            {
                Debug.Log("Pathfinding::PropagateBFS - Found null Tile at pos: " + popped.data);
            }

            if (safety++ >= 10000)
                break;
        }
    }

    public void PropagateBFS(GameObject unit, uint range) //aquest serveix per establir un rang manualment mentre fa servir les restriccions de moviment de la unitat
    {
        int safety = 0;

        //agafar dades de la unitat
        Unit unitScript = unit.GetComponent<Unit>();

        while (frontier.Count != 0)
        {
            BFS_Node popped = frontier.Dequeue();

            if (MyTilemap[popped.data.x, -popped.data.y] != null)
            {
                BFS_Node north = new BFS_Node(popped.data + new Vector2Int(0, 1), popped.data);
                BFS_Node south = new BFS_Node(popped.data + new Vector2Int(0, -1), popped.data);
                BFS_Node east = new BFS_Node(popped.data + new Vector2Int(1, 0), popped.data);
                BFS_Node west = new BFS_Node(popped.data + new Vector2Int(-1, 0), popped.data);

                List<BFS_Node> unorderedNodes = new List<BFS_Node>();

                if (CheckMapLimits(north.data))
                    unorderedNodes.Add(north);
                if (CheckMapLimits(south.data))
                    unorderedNodes.Add(south);
                if (CheckMapLimits(east.data))
                    unorderedNodes.Add(east);
                if (CheckMapLimits(west.data))
                    unorderedNodes.Add(west);

                List<BFS_Node> orderedNodes = OrderBFS_Nodes(unorderedNodes);

                foreach (BFS_Node node in orderedNodes)
                {
                    if (!CheckEnemyInPos(node.data, unit)) //primer mira si hi ha un enemic a la casella i després si està a rang i és walkable
                    {
                        if (IsInMoveRange(range, unitScript, visited[0], node, Applicant.HUMAN))
                        {
                            if (unitScript.unitType != UnitType.AERIAL)
                                CheckNode(node);
                            else
                                CheckNodeAerial(node);
                        }
                    }

                    CheckNodeForAttackRange(node);
                }
            }
            else
            {
                Debug.Log("Pathfinding::PropagateBFS - Found null Tile at pos: " + popped.data);
            }

            if (safety++ >= 10000)
                break;
        }
    }

    bool CheckMapLimits(Vector2Int pos)
    {
        return (pos.x <= MyTilemap[0, 0].maxWidth && pos.x >= 0 &&
                -pos.y <= MyTilemap[0, 0].maxHeight && -pos.y >= 0);
    }
    List<BFS_Node> OrderBFS_Nodes(List<BFS_Node> unorderedNodes)
    {
        //aquesta funció ordena els nodes segons el cost per evitar fer camins subòptims
        List<BFS_Node> orderedNodes = new List<BFS_Node>();

        if (unorderedNodes.Count > 0)
        {
            foreach (BFS_Node node in unorderedNodes)
            {
                if (MyTilemap[node.data.x, -node.data.y] != null)
                {
                    if (MyTilemap[node.data.x, -node.data.y].type == MyTileType.NEUTRAL ||
                        MyTilemap[node.data.x, -node.data.y].type == MyTileType.ROAD ||
                        MyTilemap[node.data.x, -node.data.y].type == MyTileType.BUILDING ||
                        MyTilemap[node.data.x, -node.data.y].type == MyTileType.CONE ||
                        MyTilemap[node.data.x, -node.data.y].type == MyTileType.SEA)
                    {
                        orderedNodes.Add(node);
                    }
                }
            }

            foreach (BFS_Node node in orderedNodes)
            {
                unorderedNodes.Remove(node);
            }
        }

        if (unorderedNodes.Count > 0)
        {
            foreach (BFS_Node node in unorderedNodes)
            {
                if (MyTilemap[node.data.x, -node.data.y] != null)
                {
                    if (MyTilemap[node.data.x, -node.data.y].type == MyTileType.PLANTPOT)
                    {
                        orderedNodes.Add(node);
                    }
                }
            }

            foreach (BFS_Node node in orderedNodes)
            {
                unorderedNodes.Remove(node);
            }
        }

        if (unorderedNodes.Count > 0)
        {
            foreach (BFS_Node node in unorderedNodes)
            {
                if (MyTilemap[node.data.x, -node.data.y] != null)
                {
                    if (MyTilemap[node.data.x, -node.data.y].type == MyTileType.LAMP)
                    {
                        orderedNodes.Add(node);
                    }
                }
            }

            foreach (BFS_Node node in orderedNodes)
            {
                unorderedNodes.Remove(node);
            }
        }

        return orderedNodes;
    }

    public void CheckNode(BFS_Node node)
    {
        if (!visited.Contains(node.data))
        {
            if (MyTilemap[node.data.x, -node.data.y].isWalkable)
            {
                frontier.Enqueue(node);
                visited.Add(node.data);
                backtrack.Add(node);
            }
        }
    }

    public void CheckNodeAerial(BFS_Node node)
    {
        if (!visited.Contains(node.data))
        {
            frontier.Enqueue(node);
            visited.Add(node.data);
            backtrack.Add(node);
        }
    }

    public void CheckNodeForAttackRange(BFS_Node node)
    {
        if (!attackRange.Contains(node.data))
        {
            attackRange.Add(node.data);
        }
    }

    public void CheckNodeForAI(BFS_Node node)
    {
        if (!AIVisited.Contains(node.data))
        {
            if (MyTilemap[node.data.x, -node.data.y].isWalkable)
            {
                AIFrontier.Enqueue(node);
                AIVisited.Add(node.data);
                AIBacktrack.Add(node);
            }
        }
    }

    public void CheckNodeAerialForAI(BFS_Node node)
    {
        if (!AIVisited.Contains(node.data))
        {
            AIFrontier.Enqueue(node);
            AIVisited.Add(node.data);
            AIBacktrack.Add(node);
        }
    }

    bool CheckEnemyInPos(Vector2Int pos, GameObject unit)
    {
        if (unit.GetComponent<Unit>().army == UnitArmy.CANI)
            return MyTilemap[pos.x, -pos.y].containsHipster;
        else
            return MyTilemap[pos.x, -pos.y].containsCani;
    }

    public bool IsInMoveRange(uint range, Unit unitScript, Vector2Int origin, BFS_Node goal, Applicant applicant)
    {
        uint totalCost = 0; // el que farem servir per retornar true o false

        int lastParent = 0;

        List<BFS_Node> currentBacktrack = new List<BFS_Node>();

        switch (applicant)
        {
            case Applicant.HUMAN:
                currentBacktrack = backtrack;
                break;

            case Applicant.AI:
                currentBacktrack = AIBacktrack;
                break;
        }

        int count = currentBacktrack.Count;

        if (count > 0)
        {
            BFS_Node current = goal;

            totalCost += AddCost(current.data, unitScript);

            for (int i = 0; i < count; i++)
            {
                lastParent = i;

                if (currentBacktrack[i].data == goal.parent)
                {
                    current = currentBacktrack[i];
                    break;
                }
            }

            while (current.data != origin)
            {
                totalCost += AddCost(current.data, unitScript);

                for (int i = 0; i < lastParent; i++)
                {
                    if (currentBacktrack[i].data == current.parent)
                    {
                        lastParent = i;
                        current = currentBacktrack[i];

                        break;
                    }
                }
            }
        }

        if (totalCost <= range)
            return true;
        else
            return false;
    }

    uint AddCost(Vector2Int pos, Unit unitScript)
    {
        //dades de la unitat
        uint neutralCost = unitScript.neutralCost;
        uint plantpotCost = unitScript.plantpotCost;
        uint lampCost = unitScript.lampCost;
        uint coneCost = unitScript.coneCost;

        if (MyTilemap[pos.x, -pos.y].type == MyTileType.NEUTRAL)
        {
            return neutralCost;
        }
        else if (MyTilemap[pos.x, -pos.y].type == MyTileType.CONTAINER)
        {
            return plantpotCost;
        }
        else if (MyTilemap[pos.x, -pos.y].type == MyTileType.LAMP)
        {
            return lampCost;
        }
        else if (MyTilemap[pos.x, -pos.y].type == MyTileType.CONE)
        {
            return coneCost;
        }

        //si no és ni neutral, ni jardinera ni fanal, retornem 1
        return 1;
    }

    public List<Vector2Int> GetReversePath(Vector2Int goal, Applicant applicant)
    {
        //aquest mètode retorna una llista de posicions ordenades des de la casella objectiu fins l'origen del pathfinding

        List<Vector2Int> path = new List<Vector2Int>();
        List<BFS_Node> currentBacktrack = new List<BFS_Node>();

        switch (applicant)
        {
            case Applicant.HUMAN:
                currentBacktrack = backtrack;
                break;

            case Applicant.AI:
                currentBacktrack = AIBacktrack;
                break;
        }

        int goalIndex = 0;

        if (currentBacktrack.Count > 0)
        {
            BFS_Node lastAdded = currentBacktrack[0];

            foreach (BFS_Node node in currentBacktrack)
            {
                if (node.data == goal)
                {
                    lastAdded = node;
                    path.Add(node.data);
                    break;
                }

                goalIndex++;
            }

            while (lastAdded != currentBacktrack[0])
            {
                foreach(BFS_Node node in currentBacktrack)
                {
                    if(node.data == lastAdded.parent)
                    {
                        lastAdded = node;
                        path.Add(node.data);
                        break;
                    }
                }
            }
        }

        return path;
    }

    public List<BFS_Node> GetReversePath(BFS_Node goal, Applicant applicant)
    {
        //aquest mètode retorna una llista de posicions ordenades des de la casella objectiu fins l'origen del pathfinding

        List<BFS_Node> path = new List<BFS_Node>();
        List<BFS_Node> currentBacktrack = new List<BFS_Node>();

        switch (applicant)
        {
            case Applicant.HUMAN:
                currentBacktrack = backtrack;
                break;

            case Applicant.AI:
                currentBacktrack = AIBacktrack;
                break;
        }

        int goalIndex = 0;

        if (currentBacktrack.Count > 0)
        {
            BFS_Node lastAdded = currentBacktrack[0];

            foreach (BFS_Node node in currentBacktrack)
            {
                if (node.data == goal.data)
                {
                    lastAdded = node;
                    path.Add(node);
                    break;
                }

                goalIndex++;
            }

            while (lastAdded != currentBacktrack[0])
            {
                foreach (BFS_Node node in currentBacktrack)
                {
                    if (node.data == lastAdded.parent)
                    {
                        lastAdded = node;
                        path.Add(node);
                        break;
                    }
                }
            }
        }

        return path;
    }

    public List<Vector2Int> GetReversePath(Vector2Int goal, List<BFS_Node> myBacktrack)
    {
        //aquest mètode retorna una llista de posicions ordenades des de la casella objectiu fins l'origen del pathfinding

        List<Vector2Int> path = new List<Vector2Int>();

        int goalIndex = 0;

        if (myBacktrack.Count > 0)
        {
            BFS_Node lastAdded = myBacktrack[0];

            foreach (BFS_Node node in myBacktrack)
            {
                if (node.data == goal)
                {
                    lastAdded = node;
                    path.Add(node.data);
                    break;
                }

                goalIndex++;
            }

            while (lastAdded != myBacktrack[0])
            {
                foreach (BFS_Node node in myBacktrack)
                {
                    if (node.data == lastAdded.parent)
                    {
                        lastAdded = node;
                        path.Add(node.data);
                        break;
                    }
                }
            }
        }

        return path;
    }

    public List<Vector2Int> GetPath(Vector2Int goal, Applicant applicant)
    {
        //aquest mètode retorna una llista de posicions ordenades des de l'origen fins la casella objectiu del pathfinding

        List<Vector2Int> reversePath = GetReversePath(goal, applicant);
        List<Vector2Int> path = new List<Vector2Int>();

        for (int i = reversePath.Count - 1; i >= 0; i--)
        {
            path.Add(reversePath[i]);
        }

        return path;
    }

    public void PropagateRangedBFS(GameObject unit) // per les unitats ranged
    {
        int safety = 0;

        //agafar dades de la unitat
        UnitRanged unitScript = unit.GetComponent<UnitRanged>();

        while (rangedFrontier.Count != 0)
        {
            BFS_Node popped = rangedFrontier.Dequeue();

            if (MyTilemap[popped.data.x, -popped.data.y] != null)
            {
                BFS_Node north = new BFS_Node(popped.data + new Vector2Int(0, 1), popped.data);
                BFS_Node south = new BFS_Node(popped.data + new Vector2Int(0, -1), popped.data);
                BFS_Node east = new BFS_Node(popped.data + new Vector2Int(1, 0), popped.data);
                BFS_Node west = new BFS_Node(popped.data + new Vector2Int(-1, 0), popped.data);

                List<BFS_Node> unorderedNodes = new List<BFS_Node>();

                if (CheckMapLimits(north.data))
                    unorderedNodes.Add(north);
                if (CheckMapLimits(south.data))
                    unorderedNodes.Add(south);
                if (CheckMapLimits(east.data))
                    unorderedNodes.Add(east);
                if (CheckMapLimits(west.data))
                    unorderedNodes.Add(west);

                List<BFS_Node> orderedNodes = OrderBFS_Nodes(unorderedNodes);

                foreach (BFS_Node node in orderedNodes)
                {
                    //mirar si està a rang màxim
                    if (CheckRange(rangedVisited[0], node) <= unitScript.maxRange)
                    {
                        if (CheckRange(rangedVisited[0], node) >= unitScript.minRange) //mirar si està a rang minim
                        {
                            CheckNodeRangedForAttack(node);
                        }

                        CheckNodeRanged(node);
                    }
                }
            }
            else
            {
                Debug.Log("Pathfinding::PropagateRangedBFS - Found null Tile at pos: " + popped.data);
            }

            if (safety++ >= 10000)
                break;
        }
    }

    public int CheckRange(Vector2Int origin, BFS_Node goal)
    {
        int deltaX = Mathf.Abs(origin.x - goal.data.x);
        int deltaY = Mathf.Abs(origin.y - goal.data.y);

        return deltaX + deltaY;
    }

    public void CheckNodeRanged(BFS_Node node)
    {
        if (!rangedVisited.Contains(node.data))
        {
            rangedFrontier.Enqueue(node);
            rangedVisited.Add(node.data);
            rangedBacktrack.Add(node);
        }
    }

    public void CheckNodeRangedForAttack(BFS_Node node)
    {
        if (!rangedAttackRange.Contains(node.data))
        {
            rangedAttackRange.Add(node.data);
        }
    }

    public void PropagateAIBFS(uint range, GameObject unit) //aquest bfs no fa servir el rang de moviment de la unitat sinó qualsevol que se li passi al paràmetre
    {
        int safety = 0;

        //agafar dades de la unitat
        Unit unitScript = unit.GetComponent<Unit>();

        while (AIFrontier.Count != 0)
        {
            BFS_Node popped = AIFrontier.Dequeue();

            if (MyTilemap[popped.data.x, -popped.data.y] != null)
            {
                BFS_Node north = new BFS_Node(popped.data + new Vector2Int(0, 1), popped.data);
                BFS_Node south = new BFS_Node(popped.data + new Vector2Int(0, -1), popped.data);
                BFS_Node east = new BFS_Node(popped.data + new Vector2Int(1, 0), popped.data);
                BFS_Node west = new BFS_Node(popped.data + new Vector2Int(-1, 0), popped.data);

                List<BFS_Node> unorderedNodes = new List<BFS_Node>();

                if (CheckMapLimits(north.data))
                    unorderedNodes.Add(north);
                if (CheckMapLimits(south.data))
                    unorderedNodes.Add(south);
                if (CheckMapLimits(east.data))
                    unorderedNodes.Add(east);
                if (CheckMapLimits(west.data))
                    unorderedNodes.Add(west);

                List<BFS_Node> orderedNodes = OrderBFS_Nodes(unorderedNodes);

                foreach (BFS_Node node in orderedNodes)
                {
                    if (IsInMoveRange(range, unitScript, AIVisited[0], node, Applicant.AI))
                    {
                        if (unitScript.unitType != UnitType.AERIAL)
                            CheckNodeForAI(node);
                        else
                            CheckNodeAerialForAI(node);
                    }
                }
            }
            else
            {
                Debug.Log("Pathfinding::PropagateAIBFS - Found null Tile at pos: " + popped.data);
            }

            if (safety++ >= 10000)
                break;
        }
    }

    public Vector2Int GetIntersection(Vector2Int goal, List<BFS_Node> myBacktrack, List<Vector2Int> myVisited)
    {
        //retorna la primera casella d'intersecció entre un backtrack i un visited (per recórrer camins el final dels quals queda fora del rang de moviment i s'han de fer a més d'un torn vista)
        //goal == casella de destí
        //myBacktrack == el què farem servir per obtenir el camí llarg fins la meta (la meta ha de ser-hi dincs, lògicament)
        //myVisited == les caselles que tenim a rang de moviment

        List<Vector2Int> reversePath = GetReversePath(goal, myBacktrack);

        foreach (Vector2Int tile in reversePath)
        {
            if (myVisited.Contains(tile))
                return tile;
        }

        Debug.Log("Pathfinding::GetIntersection - No Intersection found");
        return new Vector2Int(-1, -1);
    }

    public Vector2Int GetIntersection(Vector2Int goal)
    {
        //retorna la primera casella d'intersecció entre un backtrack i un visited (per recórrer camins el final dels quals queda fora del rang de moviment i s'han de fer a més d'un torn vista)
        //goal == casella de destí
        //myBacktrack == el què farem servir per obtenir el camí llarg fins la meta (la meta ha de ser-hi dincs, lògicament)
        //myVisited == les caselles que tenim a rang de moviment

        List<Vector2Int> reversePath = GetReversePath(goal, AIBacktrack);

        foreach (Vector2Int tile in reversePath)
        {
            if (visited.Contains(tile))
                return tile;
        }

        Debug.Log("Pathfinding::GetIntersection - No Intersection found");
        return new Vector2Int(-1, -1);
    }
}

