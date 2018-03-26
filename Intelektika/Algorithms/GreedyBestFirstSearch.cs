using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    class GreedyBestFirstSearch : IAlgorithm
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
                visitedNodes.Add(currentN);
                if (currentN == null)
                    continue;

                if (currentN.Name == nameOfNodeInSearch)
                {
                    result = currentN;
                    return currentN;
                }

                if (currentN.GetSuccesorsCount() > 0)
                {
                    Node wanted = currentN.GetSuccesor(0);
                    for (int i = 1; i < currentN.GetSuccesorsCount(); ++i)
                    {
                        Node succ = currentN.GetSuccesor(i);
                        if (succ.Cost < wanted.Cost)
                            wanted = succ;
                    }
                    if (!visitedNodes.Contains(wanted))
                        q.Push(wanted);
                }
            }

            return null;
        }
    }
}
