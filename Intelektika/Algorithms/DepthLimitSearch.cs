using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    class DepthLimitedSearch : IAlgorithm
    {
        private List<Node> visitedNodes { get; set; } = new List<Node>();
        private Node result { get; set; }
        Node IAlgorithm.Result { get => result; }
        List<Node> IAlgorithm.VisitedNodes { get => visitedNodes; }

        public Node Search(Node node, string nameOfNodeInSearch, int depthLimit, out int depthReached)
        {
            depthReached = 0;
            if (node == null)
                return null;

            Stack<Node> q = new Stack<Node>();
            q.Push(node);
            visitedNodes = new List<Node>();
            node.Depth = 0;

            while (q.Count > 0)
            {
                Node currentN = q.Pop();
                if (currentN == null)
                    continue;
                visitedNodes.Add(currentN);
                depthReached = currentN.Depth;


                if (currentN.Name == nameOfNodeInSearch)
                {
                    result = currentN;
                    return currentN;
                }
                if (currentN.Depth >= depthLimit)
                    continue;

                for (int i = currentN.GetSuccesorsCount() - 1; i >= 0; --i)
                {
                    Node succ = currentN.GetSuccesor(i);
                    succ.Depth = currentN.Depth + 1;
                    if (!visitedNodes.Contains(succ))
                        q.Push(succ);
                }

            }
            return null;
        }

    }
}
