using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    class BreadthFirstSearch : IAlgorithm
    {
        private List<Node> visitedNodes { get; set; } = new List<Node>();
        private Node result { get; set; }
        Node IAlgorithm.Result { get => result; }
        List<Node> IAlgorithm.VisitedNodes { get => visitedNodes; }

        public Node Search(Node head, string nameOfNodeInSearch)
        {
            if (head == null) return null;

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

                for (int i = 0; i < currentN.GetSuccesorsCount(); ++i)
                {
                    Node succ = currentN.GetSuccesor(i);
                    if (!visitedNodes.Contains(succ))
                        q.Enqueue(succ);
                }

            }

            return null;
        }
    }
}
