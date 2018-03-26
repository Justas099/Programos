using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PriorityQueueLib;

namespace Intelektika
{
    class UniformCostSearch : IAlgorithm
    {
        private List<Node> visitedNodes { get; set; } = new List<Node>();
        private Node result { get; set; }
        Node IAlgorithm.Result { get => result; }
        List<Node> IAlgorithm.VisitedNodes { get => visitedNodes; }

        public Node Search(Node node, string nameOfNodeInSearch)
        {
            if (node == null)
                return null;

            visitedNodes = new List<Node>();
            SimplePriorityQueue<Node> pQueue = new SimplePriorityQueue<Node>();
            pQueue.Enqueue(node, 0);

            while (pQueue.Count > 0)
            {
                Node n = pQueue.Dequeue();
                if (n == null)
                    continue;

                visitedNodes.Add(n);
                if (n.Name == nameOfNodeInSearch)
                {
                    result = n;
                    return n;
                }

                for (int i = 0; i < n.GetEdgesCount(); ++i)
                {
                    Edge edge = n.GetEdge(i);
                    if (visitedNodes.Contains(edge.Node))
                        continue;
                    pQueue.Enqueue(edge.Node, (float)edge.Cost);
                }

            }

            return null;
        }

    }
}
