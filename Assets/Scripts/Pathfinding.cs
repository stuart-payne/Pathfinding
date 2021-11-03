using System;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public static T[] BreadthFillSearch<T>(T start, T goal) where T: INode
    {
        var frontier = new Queue<T>();
        frontier.Enqueue(start);
        var cameFrom = new Dictionary<T, T>();

        T current;
        bool found = false;
        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();
            if (Equals(current, goal))
            {
                found = true;
                break;   
            }
            foreach (T next in current.GetNeighbours())
            {
                if(cameFrom.ContainsKey(next))
                    continue;
                frontier.Enqueue(next);
                cameFrom.Add(next, current);
            }
        }
        
        var path = new List<T>();
        if (!found)
            return Array.Empty<T>();
        current = goal;
        while (!Equals(current, start))
        {
            var nextNode = cameFrom[current];
            path.Add(nextNode);
            current = nextNode;
        }

        return path.ToArray();
    }
}
