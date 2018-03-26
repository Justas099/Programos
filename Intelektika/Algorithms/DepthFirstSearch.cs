using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    class DepthFirstSearch : IAlgorithm
    {
        private List<Node> visitedNodes { get; set; } = new List<Node>();
        private Node result { get; set; }
        Node IAlgorithm.Result { get => result; }
        List<Node> IAlgorithm.VisitedNodes { get => visitedNodes; }

        public Node Search(Node head, string nameOfNodeInSearch)
        {
            if (head == null) return null;

            Stack<Node> q = new Stack<Node>();
            q.Push(head);
            visitedNodes = new List<Node>();

            while (q.Count > 0)
            {
                Node currentN = q.Pop();
                if (currentN == null)
                    continue;
                visitedNodes.Add(currentN);

                if (currentN.Name == nameOfNodeInSearch)
                {
                    result = currentN;
                    return currentN;
                }

                for (int i = currentN.GetSuccesorsCount() - 1; i >= 0; --i)
                {
                    Node succ = currentN.GetSuccesor(i);
                    if (!visitedNodes.Contains(succ))
                        q.Push(succ);
                }

            }
            return null;
        }




    }
}

