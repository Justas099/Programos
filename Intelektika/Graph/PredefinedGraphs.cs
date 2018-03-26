using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    static class PredefinedGraphs
    {
        static public List<Node> Map()
        {

            Node arad = new Node("Arad");
            Node sibiu = new Node("Sibiu");
            Node fagaras = new Node("Fagaras");
            Node Oradea = new Node("Oradea");
            Node RimnicuVilcea = new Node("Rimvicu Vilcea");
            Node buharest = new Node("Buharest");
            Node craiova = new Node("Craiova");
            Node pitesti = new Node("Pitesti");
            Node timisoara = new Node("Timisoara");
            Node zerind = new Node("Zerind");

            /* straight-line distances to bucharest - Node.Cost */
            arad.Cost = 366;
            sibiu.Cost = 253;
            fagaras.Cost = 176;
            Oradea.Cost = 380;
            RimnicuVilcea.Cost = 193;
            buharest.Cost = 0;
            craiova.Cost = 160;
            pitesti.Cost = 100;
            timisoara.Cost = 329;
            zerind.Cost = 374;

            /* using overload AddEdgeToNode(Node, double) to add edges cost */
            arad.AddEdgeToNode(sibiu, 140);
            arad.AddEdgeToNode(zerind, 75);
            arad.AddEdgeToNode(timisoara, 118);

            sibiu.AddEdgeToNode(arad, 140);
            sibiu.AddEdgeToNode(fagaras, 99);
            sibiu.AddEdgeToNode(Oradea, 151);
            sibiu.AddEdgeToNode(RimnicuVilcea, 80);

            fagaras.AddEdgeToNode(sibiu, 99);
            fagaras.AddEdgeToNode(buharest, 211);

            RimnicuVilcea.AddEdgeToNode(pitesti, 97);
            RimnicuVilcea.AddEdgeToNode(craiova, 146);
            RimnicuVilcea.AddEdgeToNode(sibiu, 80);

            pitesti.AddEdgeToNode(buharest, 101);
            pitesti.AddEdgeToNode(craiova, 138);
            pitesti.AddEdgeToNode(RimnicuVilcea, 97);

            timisoara.AddEdgeToNode(arad, 118);
            zerind.AddEdgeToNode(arad, 75);

            arad.Margin = new System.Windows.Thickness(270, 48, 0, 0);
            sibiu.Margin = new System.Windows.Thickness(154, 146, 0, 0);
            zerind.Margin = new System.Windows.Thickness(263, 146, 0, 0);
            timisoara.Margin = new System.Windows.Thickness(388, 147, 0, 0);
            fagaras.Margin = new System.Windows.Thickness(29, 237, 0, 0);
            Oradea.Margin = new System.Windows.Thickness(145, 236, 0, 0);
            RimnicuVilcea.Margin = new System.Windows.Thickness(275, 234, 0, 0);
            buharest.Margin = new System.Windows.Thickness(43, 372, 0, 0);
            pitesti.Margin = new System.Windows.Thickness(168, 312, 0, 0);
            craiova.Margin = new System.Windows.Thickness(293, 384, 0, 0);


            return new List<Node> {
                arad, sibiu, fagaras, Oradea,
                RimnicuVilcea, buharest, craiova,
                pitesti, timisoara, zerind
            };
        }
        static public List<Node> BinaryTree(int treeDepth)
        {
            if (treeDepth < 2)
                return new List<Node>();
            int nodesCount = NodesCount(treeDepth);
            Random random = new Random();

            List<Node> nodes = new List<Node>();
            for (int i = 0; i < nodesCount; ++i)
                nodes.Add(new Node((i + 1).ToString(), random.Next(100)));


            for (int i = 0; i < nodesCount - (int)Math.Pow(2, treeDepth - 1); ++i)
            {
                nodes[i].AddEdgeToNode(nodes[i * 2 + 1], random.Next(100));
                nodes[i].AddEdgeToNode(nodes[i * 2 + 2], random.Next(100));
            }


            int firstIndex = NodesCount(treeDepth - 1);
            int lastIndex = NodesCount(treeDepth);

            int marginX = 55;
            double marginY = 55;
            double multiplierY = 1;
            double power = 4;
            nodes[firstIndex].Margin = new System.Windows.Thickness(30,
                                           ((Math.Pow(treeDepth, power - .3))), 0, 0);
            for (int k = firstIndex + 1; k < lastIndex; ++k)
            {
                nodes[k].Margin = new System.Windows.Thickness(nodes[k - 1].Margin.Left + nodes[k - 1].ActualWidth + marginX,
                                                               nodes[k - 1].Margin.Top, 0, 0);


            }
            int depth = treeDepth - 2;
            while (depth >= 0)
            {
                int first = NodesCount(depth);
                int last = first + (int)(Math.Pow(2, depth) / 2);
                for (int i = first; i < last; ++i)
                {
                    nodes[i].Margin = new System.Windows.Thickness((nodes[i * 2 + 2].Margin.Left + nodes[i * 2 + 1].Margin.Left) / 2,
                                        nodes[i * 2 + 2].Margin.Top - marginY - ((multiplierY * Math.Pow(depth, power))), 0, 0);


                }
                first = last;
                last = first + (int)(Math.Pow(2, depth) / 2);
                for (int i = first; i < last; ++i)
                {
                    nodes[i].Margin = new System.Windows.Thickness((nodes[i * 2 + 1].Margin.Left + nodes[i * 2 + 2].Margin.Left) / 2,
                                            nodes[i * 2 + 1].Margin.Top - marginY - (multiplierY * Math.Pow(depth, power)), 0, 0);
                }
                depth--;
            }

            nodes[0].Margin = new System.Windows.Thickness((nodes[1].Margin.Left + nodes[2].Margin.Left) / 2,
                                                            nodes[1].Margin.Top - marginY, 0, 0);

            return nodes;
        }
        static public List<Node> Random()
        {
            try
            {
                string[] randomWords = File.ReadAllText("randomWords.txt").Replace(",", "").
                                            Replace(".", "").Replace(";", "").Split(' ');
                Random random = new Random();

                List<Node> nodes = new List<Node>();
                for (int i = 0; i < random.Next(25) + 10; ++i)
                {
                    Node node = new Node(randomWords[random.Next(randomWords.Length) - 1]);
                    node.Margin = new System.Windows.Thickness(random.Next(500), random.Next(500), 0, 0);
                    nodes.Add(node);
                }

                return nodes;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return null;
            }
        }

        static private int NodesCount(int depth)
        {
            int nodesCount = 0;
            for (int i = 0; i < depth; ++i)
                nodesCount += (int)Math.Pow(2, i);
            return nodesCount;
        }
    }
}
