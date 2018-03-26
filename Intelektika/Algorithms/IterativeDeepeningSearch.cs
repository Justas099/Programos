using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika.Algorithms
{
    class IterativeDeepeningSearch : IAlgorithm
    {
        private List<Node> visitedNodes { get; set; } = new List<Node>();
        private Node result { get; set; }
        Node IAlgorithm.Result { get => result; }
        List<Node> IAlgorithm.VisitedNodes { get => visitedNodes; }


        public Node Search(Node head, string nameOfNodeInSearch)
        {
            if (head == null)
                return null;

            visitedNodes = new List<Node>();
            DepthLimitedSearch dls = new DepthLimitedSearch();
            int depth = 0;
            int depthReached = 0;
            while (true)
            {
                Node result = dls.Search(head, nameOfNodeInSearch, depth, out depthReached);
                foreach (Node n in ((IAlgorithm)dls).VisitedNodes)
                    visitedNodes.Add(n);
                if (result != null)
                {
                    this.result = result;
                    return result;
                }
                if (depth != depthReached)
                    break;
                depth++;
            }
            return null;
        }



    }
}
