using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Intelektika
{
    class Graph
    {
        private List<Node> nodes;
        public Panel Parent { get; }
        private Node nodeOnEdit;
        private Point mousePos;
        private Point mouseDownAt;
        private bool mouseDown = false;
        private bool mouseWheelDown = false;
        private bool pathCostVisible = false;
        private bool nodeCostVisible = false;
        private List<EdgeGraphics> edgeGraphicsList = new List<EdgeGraphics>();
        private SolidColorBrush mouseEnterEdge = new SolidColorBrush(Colors.Blue);
        private SolidColorBrush mouseLeaveEdge = new SolidColorBrush(Colors.Black);
        public List<Node> Nodes { get => nodes; }
        public ObservableCollection<string> observableCollectionOfNodeNames = new ObservableCollection<string>();
        public Node NodeSelected { get; private set; }
        public event EventHandler NodeSelectedChanged;
        public bool PathCostVisible
        {
            get => pathCostVisible;
            set
            {
                if (pathCostVisible != value)
                {
                    pathCostVisible = value;
                    foreach (EdgeGraphics eg in edgeGraphicsList)
                    {
                        if (pathCostVisible)
                            eg.PathCost.Visibility = Visibility.Visible;
                        else
                            eg.PathCost.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        public bool NodeCostVisible
        {
            get => nodeCostVisible;
            set
            {
                nodeCostVisible = value;
                nodes.ForEach(x => x.ShowCost = NodeCostVisible);
                Refresh();
            }
        }

        public Graph(Panel parent)
        {
            nodes = new List<Node>();
            this.Parent = parent;
            Parent.MouseLeave += Parent_MouseLeave;
            Parent.MouseMove += Parent_MouseMove;
            Parent.MouseWheel += Parent_MouseWheel;
            Parent.MouseDown += Parent_MouseDown;
            Parent.MouseUp += Parent_MouseUp;
        }

        private Color NodeColorOnSelect = Colors.ForestGreen;
        private Color NodeColorDefault = Color.FromArgb(50, 1, 1, 1);


        #region Events
        private void Parent_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseWheelDown || nodeOnEdit != null)
            {
                Node_MouseMove(nodeOnEdit, e);
            }
        }
        private void Parent_MouseLeave(object sender, MouseEventArgs e)
        {
            nodeOnEdit = null;
            mouseDown = false;
        }
        private void Parent_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                mouseWheelDown = false;
                return;
            }
            mouseDown = false;
        }
        private void Parent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mousePos = e.GetPosition(Parent);
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                mouseWheelDown = true;
                return;
            }
            mouseDown = true;
            if (NodeSelected != null)
            {
                NodeSelected.BorderBrush = new SolidColorBrush(NodeColorDefault);
                NodeSelected = null;
                NodeSelectedChanged?.Invoke(null, EventArgs.Empty);
            }

        }
        private void Parent_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool increase = e.Delta > 0;

            foreach (Node n in nodes)
            {
                if (increase)
                {
                    n.IncreaseSize();
                }
                else
                {
                    n.DecraseSize();
                }
            }
            Refresh();
        }

        private void Node_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseWheelDown)
            {
                Point curr = e.GetPosition(Parent);
                foreach (Node n in nodes)
                {
                    MoveNode(n, curr.X - mousePos.X, curr.Y - mousePos.Y);
                }
                mousePos = curr;
                return;
            }

            if (nodeOnEdit != null && mouseDown)
            {
                Point curr = e.GetPosition(Parent);
                MoveNode(nodeOnEdit, curr.X - mousePos.X, curr.Y - mousePos.Y);
                mousePos = curr;
            }


        }
        private void Node_MouseEnter(object sender, MouseEventArgs e)
        {
            Node node = sender as Node;
            node.BorderThickness = new Thickness(1.5);
        }
        private void Node_MouseLeave(object sender, MouseEventArgs e)
        {
            Node node = sender as Node;
            node.BorderThickness = new Thickness(1);
        }
        private void Node_EdgeRemoved(object sender, EventArgs e)
        {
            Node node = (sender as object[])[0] as Node;
            Edge edge = (sender as object[])[1] as Edge;

            for (int i = 0; i < edgeGraphicsList.Count; ++i)
            {
                if (edgeGraphicsList[i].Contains(node, edge.Node))
                {
                    var eg = edgeGraphicsList[i];
                    if (eg.EdgeDirection != EdgeDirection.ABBA)
                    {
                        Parent.Children.Remove(eg.Edge);
                        Parent.Children.Remove(eg.PathCost);
                        edgeGraphicsList.RemoveAt(i);
                    }
                    else
                    {
                        if (eg.NodeA == node)
                        {
                            eg.EdgeDirection = EdgeDirection.BA;
                        }
                        else
                        {
                            eg.EdgeDirection = EdgeDirection.AB;
                        }
                    }
                    return;
                }
            }
        }
        private void Node_EdgeAdded(object sender, EventArgs e)
        {
            var node = sender as Node;

            var succ = node.GetSuccesor(node.GetSuccesorsCount() - 1);
            bool exists = false;
            foreach (EdgeGraphics eg in edgeGraphicsList)
            {
                if (eg.Contains(node, succ))
                {
                    eg.EdgeDirection = EdgeDirection.ABBA;
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                EdgeGraphics eg = new EdgeGraphics(node, succ, EdgeDirection.AB);
                edgeGraphicsList.Add(eg);
                Parent.Children.Add(eg.Edge);
                Parent.Children.Add(eg.PathCost);
                if (!pathCostVisible)
                {
                    eg.PathCost.Visibility = Visibility.Hidden;
                }
            }

        }
        private void Node_NameChanged(object sender, EventArgs e)
        {
            Node node = sender as Node;
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (node == nodes[i])
                {
                    if (i < observableCollectionOfNodeNames.Count)
                        observableCollectionOfNodeNames[i] = node.Name;
                    return;
                }
            }
        }
        private void Node_MouseUp(object sender, MouseButtonEventArgs e)
        {

            mouseWheelDown = false;
            nodeOnEdit = null;
            mouseDown = false;
            if (e.GetPosition(Parent) == mouseDownAt)
            {
                Node n = sender as Node;
                if (n != NodeSelected)
                {
                    if (NodeSelected != null)
                    {
                        NodeSelected.BorderBrush = new SolidColorBrush(NodeColorDefault);
                    }
                    NodeSelected = n;
                    NodeSelected.BorderBrush = new SolidColorBrush(NodeColorOnSelect);
                    NodeSelectedChanged?.Invoke(NodeSelected, EventArgs.Empty);
                }
            }
        }
        private void Node_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mousePos = e.GetPosition(Parent);
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                mouseWheelDown = true;
                return;
            }

            nodeOnEdit = sender as Node;
            mouseDownAt = e.GetPosition(Parent);
            mouseDown = true;
        }
        private void Node_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                mouseWheelDown = true;
            }
            else if (e.MiddleButton == MouseButtonState.Released)
            {
                mouseWheelDown = false;
            }
            else
            {
                Parent_MouseWheel(sender, e);
            }
        }
        private void Graph_NodeEdgeEdited(object sender, EventArgs e)
        {
            Edge edge = sender as Edge;
            foreach (EdgeGraphics eg in edgeGraphicsList)
            {
                eg.Refresh();
            }
        }
        #endregion

        private void MoveNode(Node node, double x, double y)
        {
            double left = node.Margin.Left + x;
            double top = node.Margin.Top + y;
            double right = node.Margin.Right;
            double bottom = node.Margin.Bottom;
            //if (left < 0 || top < 0
            //   || left > Parent.ActualWidth - node.ActualWidth
            //   || top > Parent.ActualHeight - node.ActualHeight)
            //    return;
            node.Margin = new Thickness(left, top, left + node.ActualWidth, top + node.ActualHeight);
        }
        public void AddNode(Node node)
        {
            if (node == null || nodes.Contains(node))
                return;

            node.MouseDown += Node_MouseDown;
            node.MouseUp += Node_MouseUp;
            node.MouseEnter += Node_MouseEnter;
            node.MouseLeave += Node_MouseLeave;
            node.MouseMove += Node_MouseMove;
            node.EdgeAdded += Node_EdgeAdded;
            node.NameChanged += Node_NameChanged;
            node.EdgeRemoved += Node_EdgeRemoved;
            node.MouseWheel += Node_MouseWheel;

            for (int i = 0; i < node.GetEdgesCount(); ++i)
            {
                node.GetEdge(i).Edited += Graph_NodeEdgeEdited;
            }

            for (int i = 0; i < node.GetSuccesorsCount(); ++i)
            {
                Node succ = node.GetSuccesor(i);

                bool exists = false;
                foreach (EdgeGraphics eg in edgeGraphicsList)
                {
                    if (eg.Contains(node, succ))
                    {
                        eg.EdgeDirection = EdgeDirection.ABBA;
                        exists = true;
                        break;
                    }
                }
                if (exists)
                    continue;

                var e = new EdgeGraphics(node, node.GetSuccesor(i), EdgeDirection.AB);
                edgeGraphicsList.Add(e);
                Parent.Children.Add(e.Edge);
                Parent.Children.Add(e.PathCost);
                if (!pathCostVisible)
                {
                    e.PathCost.Visibility = Visibility.Hidden;
                }
            }

            observableCollectionOfNodeNames.Add(node.Name);
            nodes.Add(node);
            Parent.Children.Add(node);

        }

        public void RemoveNode(Node node)
        {
            for (int i = 0; i < edgeGraphicsList.Count; ++i)
                if (edgeGraphicsList[i].Contains(node))
                {
                    Parent.Children.Remove(edgeGraphicsList[i].Edge);
                    Parent.Children.Remove(edgeGraphicsList[i].PathCost);
                    edgeGraphicsList.RemoveAt(i);
                    --i;
                }
            nodes.Remove(node);
        }
        public void Clear()
        {
            Parent.Children.Clear();
            edgeGraphicsList.Clear();
            nodes.Clear();
            observableCollectionOfNodeNames.Clear();
        }

        public Node GetNodeByName(string name)
        {
            foreach (Node n in nodes)
                if (n.Name == name)
                    return n;

            return null;
        }
        public void Refresh()
        {

            if (nodes.Count == 0)
                return;
            Task.Run(() =>
            {
                Thread.Sleep(1);
                nodes[0].Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    foreach (EdgeGraphics eg in edgeGraphicsList)
                        eg.Refresh();
                }));

            });

        }

        public void SetGraphMap()
        {
            Clear();
            SetGraph(PredefinedGraphs.Map());
        }

        public void SetGraphBinaryTree()
        {
            Clear();
            SetGraph(PredefinedGraphs.BinaryTree(5));
        }
        public void SetGraphRandom()
        {
            Clear();
            SetGraph(PredefinedGraphs.Random());
        }

        public void SaveCurrentGraph(string nameOfGraph)
        {
            try
            {

                if (Path.HasExtension(nameOfGraph))
                {
                    nameOfGraph = nameOfGraph.Replace(Path.GetExtension(nameOfGraph), "");
                }
                XMLConverter xML = new XMLConverter();
                string path = "..\\..\\Resources\\" + nameOfGraph + ".xml";

                using (StreamWriter file = new StreamWriter(path, false))
                {
                    file.WriteLine("<root>");
                    foreach (Node n in nodes)
                        file.WriteLine(xML.Convert(n));
                    file.WriteLine("</root>");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void SetGraph(List<Node> list)
        {
            if (list == null || list.Count == 0)
                return;

            foreach (Node n in list)
            {
                AddNode(n);
            }

            Task.Run(() =>
            {
                Thread.Sleep(2);
                Parent.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { this.Refresh(); }));
            });

        }

        public void LoadGraphFromFile(string fileName)
        {

            List<Node> nodesList = new List<Node>();
            try
            {
                string path = "..\\..\\Resources\\" + fileName + ".xml";
                var level1Elements = XElement.Load(path).Elements("node");
                foreach (var el in level1Elements)
                {
                    Node node = new Node(el.Element("name").Value);
                    var margins = el.Element("margin").Value.Split(',');
                    node.Margin = new Thickness(double.Parse(margins[0]), double.Parse(margins[1]),
                                                double.Parse(margins[2]), double.Parse(margins[3]));
                    nodesList.Add(node);
                }

                foreach (var el1 in level1Elements)
                {

                    Node node = nodesList.First(x => x.Name == el1.Element("name").Value);
                    foreach (var el2 in el1.Elements("edges"))
                    {
                        foreach (var el3 in el2.Elements("edge"))
                        {
                            string to = el3.Element("to")?.Value;
                            string cost = el3.Element("cost")?.Value;
                            node.AddEdgeToNode(nodesList.First(x => x.Name == to), cost == null ? 0 : double.Parse(cost));
                        }
                    }
                }

                SetGraph(nodesList);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}



