using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections.Generic;
using System;
using System.Collections;

public static class PathFinding
{
    private static GameObject _grid;
    public static GameObject grid
    {
        get {
            if (_grid == null)
            {
                _grid = GameObject.Find("Grid");
            }
            return _grid;

        }
        set { _grid = value; }
    }
    /// <summary>
    /// TODO - swap off using guard to a interface
    /// Assumes uniform x/y cell size, uses A*
    /// </summary>
    /// <param name="start"> in world pos</param>
    /// <param name="end"> in world pos</param>
    /// <param name="output"> a list of Vectors that all connect to form an unobstructed path </param>
    /// <returns>If a vaild path was found</returns>
    public static IEnumerator FindPath(Vector2 start, Vector2 end, Guard g , int xBounds = 200, int yBounds = 200, int iterationsPerFrame = 50 )
    {
        g.CurrentlyPathFinding = true;
        g.currentIndex = 0;
        g.path = null;
        var gridcomp = grid.GetComponent<Grid>();
        float gridSize = gridcomp.cellSize.x/2f ;
        int Heuristic((int, int) a, Vector3Int b)
        {
            return Math.Abs(a.Item1 - b.x) + Math.Abs(a.Item2 - b.y);
        }
        var endTile = gridcomp.WorldToCell(end);
        var startTile = gridcomp.WorldToCell(start);
        var opens = new PriorityQueue<(int, int), int>();
        opens.Enqueue(((int)startTile.x, (int)startTile.y), 0);
        var gScore = new Dictionary<(int, int), int> { [((int)startTile.x, (int)startTile.y)] = 0 };
        var cameFrom = new Dictionary<(int, int), (int, int)>();
        int Iterations = 0;
        //While we still have possible paths
        while(opens.Count > 0)
        {
            Iterations++;
            if (Iterations % iterationsPerFrame == 0)
            {
                Debug.Log("Yield");
                yield return null;
            }
            //grab the next most likely spot
            var current = opens.Dequeue();
            if(current.Item1 == endTile.x &&  current.Item2 == endTile.y)
            {
                var path = new List<Vector2>();
                while (cameFrom.ContainsKey(current))
                {
                    path.Add(gridcomp.CellToWorld(new Vector3Int(current.Item1, current.Item2)) + new Vector3(gridSize, gridSize, 0));
                    current = cameFrom[current];
                }
                path.Add(gridcomp.CellToWorld(startTile));
                path.Reverse();
                g.path = path.ToArray();
                Debug.Log("PathFound");
                g.CurrentlyPathFinding = false;
                Debug.Log(path);
                yield break;
            }
            var neighbors = new (int, int)[]
            {
                (current.Item1 - 1, current.Item2),
                (current.Item1 + 1, current.Item2),
                (current.Item1, current.Item2 - 1),
                (current.Item1, current.Item2 + 1)
            };
            foreach (var neighbor in neighbors)
            {
                int x = neighbor.Item1, y = neighbor.Item2;
                //check if walkable 
                if ((Math.Abs(startTile.x - x) <  Math.Abs(xBounds) && Math.Abs(startTile.y - y) <  Math.Abs(yBounds) && !Physics2D.OverlapPoint(gridcomp.CellToWorld(new Vector3Int(x,y)) + new Vector3(gridSize, gridSize, 0))) || (x == endTile.x) && (y == endTile.y))
                {
                    int ngs = gScore[current] + 1;
                    if (!gScore.ContainsKey(neighbor) || ngs < gScore[neighbor])
                    {
                        gScore[neighbor] = ngs;
                        int fScore = ngs + Heuristic(neighbor, startTile);
                        opens.Enqueue(neighbor, fScore);
                        cameFrom[neighbor] = current;
                    }
                }
            }
        }
        //failed to find a path

        Debug.Log("Failed To Find Path");
        g.state = GuardState.Patrol;
        g.CurrentlyPathFinding = false;
    }

}
public class PriorityQueue<TItem, TPriority> where TPriority : IComparable
{
    private readonly List<(TItem Item, TPriority Priority)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(TItem item, TPriority priority)
    {
        elements.Add((item, priority));
        elements.Sort((x, y) => x.Priority.CompareTo(y.Priority));
    }

    public TItem Dequeue()
    {
        var item = elements[0].Item;
        elements.RemoveAt(0);
        return item;
    }
}
