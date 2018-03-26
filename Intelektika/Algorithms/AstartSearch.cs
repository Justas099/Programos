using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    class AstarSearch : IAlgorithm
    {
        private List<Node> visitedNodes { get; set; } = new List<Node>();
        private Node result { get; set; }
        Node IAlgorithm.Result { get => result; }
        List<Node> IAlgorithm.VisitedNodes { get => visitedNodes; }

        public Node Search(Node head, string nameOfNodeInSearch)
        {
            if (head == null) return null;

            double minCost;
            Queue<Node> q = new Queue<Node>();
            q.Enqueue(head);
            visitedNodes = new List<Node>();

            while (q.Count > 0)
            {
                Node currentN = q.Dequeue();
                visitedNodes.Add(currentN);
                if (currentN == null)
                    continue;

                if (currentN.Name == nameOfNodeInSearch)
                {
                    result = currentN;
                    return currentN;
                }

                if (currentN.GetEdgesCount() > 0)
                {
                    minCost = currentN.Cost;
                    Node wanted = currentN;
                    for (int i = 0; i < currentN.GetEdgesCount(); ++i)
                    {
                        Edge edge = currentN.GetEdge(i);
                        if (i == 0)
                        {
                            minCost = edge.Cost + edge.Node.Cost;
                            wanted = currentN.GetEdge(i).Node;
                            continue;
                        }

                        double currentCost = edge.Cost + edge.Node.Cost;
                        if (currentCost < minCost)
                        {
                            minCost = currentCost;
                            wanted = edge.Node;
                        }

                    }
                    if (!visitedNodes.Contains(wanted))
                        q.Enqueue(wanted);
                }
            }

            return null;
        }
    }
}
