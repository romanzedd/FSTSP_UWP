using System;
using System.Collections.Generic;
using System.Linq;
//using Common;

namespace FSTSP_UWP
{
    public class SearchEngine
    {
        public event EventHandler Updated;
        private void OnUpdated()
        {
            Updated?.Invoke(null, EventArgs.Empty);
        }
        public Map Map { get; set; }
        public Graph Start { get; set; }
        public Graph End { get; set; }
        public int NodeVisits { get; private set; }
        public double ShortestPathLength { get; set; }
        public double ShortestPathCost { get; private set; }

        public SearchEngine(Map map)
        {
            Map = map;
            End = map.EndNode;
            Start = map.StartNode;
        }

        public List<Graph> GetShortestPathDijikstra()
        {
            DijkstraSearch();
            var shortestPath = new List<Graph>();
            shortestPath.Add(End);
            BuildShortestPath(shortestPath, End);
            shortestPath.Reverse();
            return shortestPath;
        }

        private void BuildShortestPath(List<Graph> list, Graph node)
        {
            if (node.NearestToStart == null)
                return;
            list.Add(node.NearestToStart);
            ShortestPathLength += node.connections.Single(x => x.ConnectedNode == node.NearestToStart).Length;
            ShortestPathCost += node.connections.Single(x => x.ConnectedNode == node.NearestToStart).Cost;
            BuildShortestPath(list, node.NearestToStart);
        }

        private void DijkstraSearch()
        {
            NodeVisits = 0;
            Start.MinCostToStart = 0;
            var prioQueue = new List<Graph>();
            prioQueue.Add(Start);
            do
            {
                NodeVisits++;
                prioQueue = prioQueue.OrderBy(x => x.MinCostToStart.Value).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                foreach (var cnn in node.connections.OrderBy(x => x.Cost))
                {
                    var childNode = cnn.ConnectedNode;
                    if (childNode.Visited)
                        continue;
                    if (childNode.MinCostToStart == null ||
                        node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
                        childNode.NearestToStart = node;
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }
                node.Visited = true;
                if (node == End)
                    return;
            } while (prioQueue.Any());
        }

        public List<Graph> GetShortestPathAstart()
        {
            foreach (var node in Map.Nodes)
                node.StraightLineDistanceToEnd = node.StraightLineDistanceTo(End);
            AstarSearch();
            var shortestPath = new List<Graph>();
            shortestPath.Add(End);
            BuildShortestPath(shortestPath, End);
            shortestPath.Reverse();
            return shortestPath;
        }

        private void AstarSearch()
        {
            NodeVisits = 0;
            Start.MinCostToStart = 0;
            var prioQueue = new List<Graph>();
            prioQueue.Add(Start);
            do
            {
                prioQueue = prioQueue.OrderBy(x => x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                NodeVisits++;
                foreach (var cnn in node.connections.OrderBy(x => x.Cost))
                {
                    var childNode = cnn.ConnectedNode;
                    if (childNode.Visited)
                        continue;
                    if (childNode.MinCostToStart == null ||
                        node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
                        childNode.NearestToStart = node;
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }
                node.Visited = true;
                if (node == End)
                    return;
            } while (prioQueue.Any());
        }
    }
}
