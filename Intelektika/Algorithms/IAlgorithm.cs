using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    interface IAlgorithm
    {
        Node Result { get; }
        List<Node> VisitedNodes { get; }

    }
}
