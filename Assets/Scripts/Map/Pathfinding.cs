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
    NEUTRAL, ROAD, CONTAINER, LAMP, BUILDING
};

public class MyTile
{
    public MyTile(Vector2Int position, bool isWalkable, MyTileType type)
    {
        this.position = position;
        this.isWalkable = isWalkable;
        this.type = type;
    }

    public Vector2Int position;
    public bool isWalkable;
    public MyTileType type;
}

public class Pathfinding
{
    public MyTile[,] MyTilemap; //això s'inicialitza des del MapController amb les mides del mapa

    public Queue<BFS_Node> frontier = new Queue<BFS_Node>();
    public List<Vector2Int> visited = new List<Vector2Int>();
    public List<BFS_Node> backtrack = new List<BFS_Node>();

    public void ResetBFS(Vector2Int position)
    {
        frontier.Clear();
        visited.Clear();
        backtrack.Clear();

        BFS_Node node = new BFS_Node(position, new Vector2Int(-1, -1));
        frontier.Enqueue(node);
        visited.Add(node.data);
        backtrack.Add(node);
    }

    public void PropagateBFS(GameObject unit)
    {
        int safety = 0;

        //agafar dades de la unitat
        Unit unitScript = unit.GetComponent<Unit>();

        while (frontier.Count != 0)
        {
            BFS_Node popped = frontier.Dequeue();

            if (MyTilemap[popped.data.x, -popped.data.y].isWalkable)
            {
                BFS_Node north = new BFS_Node(popped.data + new Vector2Int(0, 1), popped.data);
                BFS_Node south = new BFS_Node(popped.data + new Vector2Int(0, -1), popped.data);
                BFS_Node east = new BFS_Node(popped.data + new Vector2Int(1, 0), popped.data);
                BFS_Node west = new BFS_Node(popped.data + new Vector2Int(-1, 0), popped.data);

                List<BFS_Node> unorderedNodes = new List<BFS_Node>();
                unorderedNodes.Add(north);
                unorderedNodes.Add(south);
                unorderedNodes.Add(east);
                unorderedNodes.Add(west);

                List<BFS_Node> orderedNodes = OrderBFS_Nodes(unorderedNodes);

                foreach(BFS_Node node in orderedNodes)
                {
                    if (IsInMoveRange(unitScript, visited[0], node))
                        CheckNode(node);
                }
                // TODO: mirar si la casella se surt del mapa per descartar-la
            }

            if (safety++ >= 500)
                break;
        }
    }

    List<BFS_Node> OrderBFS_Nodes(List<BFS_Node> unorderedNodes)
    {
        //aquesta funció ordena els nodes segons el cost per evitar fer camins subòptims
        List<BFS_Node> orderedNodes = new List<BFS_Node>();

        if (unorderedNodes.Count > 0)
        {
            foreach (BFS_Node node in unorderedNodes)
            {
                if (MyTilemap[node.data.x, -node.data.y].type == MyTileType.NEUTRAL ||
                    MyTilemap[node.data.x, -node.data.y].type == MyTileType.ROAD ||
                    MyTilemap[node.data.x, -node.data.y].type == MyTileType.BUILDING)
                {
                    orderedNodes.Add(node);
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
                if (MyTilemap[node.data.x, -node.data.y].type == MyTileType.CONTAINER)
                {
                    orderedNodes.Add(node);
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
                if (MyTilemap[node.data.x, -node.data.y].type == MyTileType.LAMP)
                {
                    orderedNodes.Add(node);
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

    public bool IsInMoveRange(Unit unitScript, Vector2Int origin, BFS_Node goal)
    {
        //dades de la unitat
        uint range = unitScript.movementRange;

        uint totalCost = 0; // el que farem servir per retornar true o false

        int lastParent = 0;
        int count = backtrack.Count;
        
        if (count > 0)
        {
            BFS_Node current = goal;

            totalCost += AddCost(current.data, unitScript);

            for (int i = 0; i < count; i++)
            {
                lastParent = i;

                if (backtrack[i].data == goal.parent)
                {
                    current = backtrack[i];
                    break;
                }
            }

            while (current.data != origin)
            {
                totalCost += AddCost(current.data, unitScript);

                for (int i = 0; i < lastParent; i++)
                {
                    if (backtrack[i].data == current.parent)
                    {
                        lastParent = i;
                        current = backtrack[i];

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
        uint containerCost = unitScript.containerCost;
        uint lampCost = unitScript.lampCost;

        if (MyTilemap[pos.x, -pos.y].type == MyTileType.NEUTRAL)
        {
            return neutralCost;
        }
        else if (MyTilemap[pos.x, -pos.y].type == MyTileType.CONTAINER)
        {
            return containerCost;
        }
        else if (MyTilemap[pos.x, -pos.y].type == MyTileType.LAMP)
        {
            return lampCost;
        }

        //si no és ni neutral, ni contenidor ni fanal, retornem 1
        return 1;
    }

    public List<Vector2Int> GetReversePath(Vector2Int goal)
    {
        //aquest mètode retorna una llista de posicions ordenades des de la casella objectiu fins l'origen del pathfinding

        List<Vector2Int> path = new List<Vector2Int>();

        int goalIndex = 0;

        if (backtrack.Count > 0)
        {
            BFS_Node lastAdded = backtrack[0];

            foreach (BFS_Node node in backtrack)
            {
                if (node.data == goal)
                {
                    lastAdded = node;
                    path.Add(node.data);
                    break;
                }

                goalIndex++;
            }

            while (lastAdded != backtrack[0])
            {
                foreach(BFS_Node node in backtrack)
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

    public List<BFS_Node> GetReversePath(BFS_Node goal)
    {
        //aquest mètode retorna una llista de posicions ordenades des de la casella objectiu fins l'origen del pathfinding

        List<BFS_Node> path = new List<BFS_Node>();

        int goalIndex = 0;

        if (backtrack.Count > 0)
        {
            BFS_Node lastAdded = backtrack[0];

            foreach (BFS_Node node in backtrack)
            {
                if (node.data == goal.data)
                {
                    lastAdded = node;
                    path.Add(node);
                    break;
                }

                goalIndex++;
            }

            while (lastAdded != backtrack[0])
            {
                foreach (BFS_Node node in backtrack)
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

    public List<Vector2Int> GetPath(Vector2Int goal)
    {
        //aquest mètode retorna una llista de posicions ordenades des de l'origen fins la casella objectiu del pathfinding

        List<Vector2Int> reversePath = GetReversePath(goal);
        List<Vector2Int> path = new List<Vector2Int>();

        for (int i = reversePath.Count - 1; i >= 0; i--)
        {
            path.Add(reversePath[i]);
        }

        return path;
    }
}

