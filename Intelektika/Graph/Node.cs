using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Intelektika
{
    class Node : Border
    {
        private TextBlock textBlock = new TextBlock();
        private List<Edge> edges = new List<Edge>();
        private double cost = 0;
        private string name;

        public event EventHandler EdgeAdded;
        public event EventHandler EdgeRemoved;
        public event EventHandler NameChanged;
        public event EventHandler Moved;
        private double _scale = 1;
        private double scaleStep = 1.05;
        private bool showCost = false;


        public double Scale
        {
            get => _scale;
            set
            {
                _scale = value;
            }
        }

        public new Thickness Margin
        {
            get => base.Margin;
            set
            {
                base.Margin = value;
                Moved?.Invoke(this, EventArgs.Empty);
            }
        }
        public new string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (showCost)
                {
                    textBlock.Text = name + Environment.NewLine + $"({cost})";
                }
                else
                {
                    textBlock.Text = name;
                }

                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public int Depth { get; set; }
        public double Cost
        {
            get => cost;
            set
            {
                cost = value;
                if (showCost)
                {
                    textBlock.Text = name + Environment.NewLine + $"({cost})";
                }
                else
                {
                    textBlock.Text = name;
                }

            }
        }
        public bool ShowCost
        {
            set
            {
                showCost = value;
                Name = name;
            }
        }

        public Node(string name) : this(name, 0) { }
        public Node(string name, double cost)
        {
            CornerRadius = new CornerRadius(30);
            Background = new SolidColorBrush(Color.FromArgb(100, 205, 206, 218));
            BorderBrush = new SolidColorBrush(Color.FromArgb(50, 1, 1, 1));
            BorderThickness = new Thickness(1);
            textBlock.Margin = new Thickness(10, 5, 10, 5);

            this.cost = cost;
            this.Name = name;
            Child = textBlock;
            textBlock.TextAlignment = TextAlignment.Center;
        }
        public Node() : this("Node") { }


        public bool AddEdgeToNode(Node node, double edgeCost)
        {
            if (node == null || edges.Exists(x => x.Node == node) || this == node)
                return false;
            Edge e = new Edge(node, edgeCost);
            edges.Add(e);
            EdgeAdded?.Invoke(this, EventArgs.Empty);
            return true;
        }
        public bool AddEdgeToNode(Node node)
        {
            return AddEdgeToNode(node, 0);
        }
        public void RemoveEdge(int index)
        {
            if (index >= edges.Count || index < 0)
                return;

            Edge edge = edges[index];
            edges.Remove(edge);
            EdgeRemoved?.Invoke(new object[] { this, edge }, EventArgs.Empty);
        }
        public void RemoveEdge(string nodeName)
        {
            for (int i = 0; i < edges.Count; ++i)
            {
                if (edges[i].Node.Name == nodeName)
                {
                    RemoveEdge(i);
                    return;
                }
            }
        }

        public int GetEdgesCount() => edges.Count;
        public Edge GetEdge(int index)
        {
            if (index >= edges.Count || index < 0)
                return null;
            return edges[index];
        }
        public Edge GetEdge(Node node)
        {
            foreach (Edge e in edges)
                if (e.Node == node)
                    return e;
            return null;
        }
        public Edge GetEdge(string nodeName)
        {
            foreach (Edge e in edges)
                if (e.Node.Name == nodeName)
                    return e;
            return null;
        }
        public int GetSuccesorsCount() => edges.Count;
        public Node GetSuccesor(int index)
        {
            if (index >= edges.Count || index < 0)
                return null;
            return edges[index].Node;
        }
        public Node GetSuccesor(string name)
        {
            foreach (Edge e in edges)
            {
                if (e.Node.Name == name)
                    return e.Node;
            }


            return null;
        }

        public void DecraseSize()
        {
            Scale /= scaleStep;
            textBlock.FontSize /= scaleStep;
            base.Margin = new Thickness(Margin.Left / scaleStep, Margin.Top / scaleStep,
                                        Margin.Right / scaleStep, Margin.Bottom / scaleStep);
        }
        public void IncreaseSize()
        {
            Scale *= scaleStep;
            textBlock.FontSize *= scaleStep;
            base.Margin = new Thickness(Margin.Left * scaleStep, Margin.Top * scaleStep,
                                        Margin.Right * scaleStep, Margin.Bottom * scaleStep);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}