using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    class XMLConverter
    {
        public string Convert(Node node)
        {
            string nodeContent = $"<node>";
            nodeContent += $"{Environment.NewLine}\t<name>{node.Name}</name>";
            nodeContent += $"{Environment.NewLine}\t<margin>{node.Margin}</margin>";

            for (int i = 0; i < node.GetEdgesCount(); ++i)
            {
                if (i == 0)
                {
                    nodeContent += $"{Environment.NewLine}\t<edges>";
                }
                Edge edge = node.GetEdge(i);
                nodeContent += $"{Environment.NewLine}\t\t<edge>";
                nodeContent += $"{Environment.NewLine}\t\t\t<to>{edge.Node.Name}</to>";
                nodeContent += $"{Environment.NewLine}\t\t\t<cost>{edge.Cost}</cost>";
                nodeContent += $"{Environment.NewLine}\t\t</edge>";

                if (i == node.GetEdgesCount() - 1)
                {
                    nodeContent += $"{Environment.NewLine}\t</edges>";
                }
            }
            return nodeContent + $"{Environment.NewLine}</node>";
        }
    }
}
