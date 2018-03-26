using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Intelektika
{
    class EdgeGraphics
    {
        public Node NodeA { get; set; }
        public Node NodeB { get; set; }
        private EdgeDirection _edgeDirection;
        private Label pathCostLabel = new Label();
        private Polygon polygon;
        public UIElement Edge { get => polygon; }
        public UIElement PathCost { get => pathCostLabel; }
        public EdgeDirection EdgeDirection
        {
            get { return _edgeDirection; }
            set
            {
                _edgeDirection = value;
                Node_Moved(NodeA, EventArgs.Empty);
            }
        }
        public EdgeGraphics(Node a, Node b, EdgeDirection direction)
        {
            this.NodeA = a;
            this.NodeB = b;
            this._edgeDirection = direction;

            polygon = new Polygon();
            RenderOptions.SetEdgeMode(polygon, EdgeMode.Unspecified);
            polygon.Stroke = new SolidColorBrush(Colors.Black);
            polygon.Fill = polygon.Stroke;

            NodeA.Moved += Node_Moved;
            NodeB.Moved += Node_Moved;

            Node_Moved(NodeA, EventArgs.Empty);
        }
        public bool Contains(Node a, Node b)
        {
            return Contains(a) && Contains(b);
        }
        public bool Contains(Node node)
        {
            return NodeA == node || NodeB == node;
        }

        public void Refresh()
        {
            Node_Moved(NodeA, EventArgs.Empty);
        }
        public override string ToString()
        {
            string r = NodeA.Name;
            if (_edgeDirection == EdgeDirection.AB)
                r += " -> ";
            else if (_edgeDirection == EdgeDirection.ABBA)
                r += " <-> ";
            else
                r += " <- ";
            r += NodeB.Name;

            return r;
        }

        private Point GetCenter(FrameworkElement p)
        {
            return new Point(p.ActualWidth / 2 + p.Margin.Left,
                             p.ActualHeight / 2 + p.Margin.Top);
        }
        private void SetLabel(double x, double y, double angle)
        {
            double fontScale = 12;
            pathCostLabel.FontSize = fontScale * NodeA.Scale * 0.75;

            pathCostLabel.LayoutTransform = new RotateTransform(-angle);
            double radX = Math.Cos(angle.ToRadians());
            double radY = Math.Sin(angle.ToRadians());

            x = x + (pathCostLabel.ActualWidth / 2) * ((radX > 0) ? -radX : radX);
            y = y + (pathCostLabel.ActualHeight / 2 - 3) * ((radY > 0) ? -radY : radY);
            pathCostLabel.Margin = new Thickness(x, y, 0, 0);

            //pathCostLabel.BorderBrush = new SolidColorBrush(Colors.Black);
            //pathCostLabel.BorderThickness = new Thickness(1);

            pathCostLabel.Content = "";
            if (_edgeDirection == EdgeDirection.ABBA)
            {
                if (NodeA.GetEdge(NodeB).Cost == NodeB.GetEdge(NodeA).Cost)
                {
                    pathCostLabel.Content = $"{NodeA.GetEdge(NodeB)?.Cost}";
                }
                else
                {
                    pathCostLabel.Content = $"< {NodeA.GetEdge(NodeB)?.Cost} | {NodeB.GetEdge(NodeA)?.Cost} >";
                }
            }
            else if (_edgeDirection == EdgeDirection.AB)
            {
                pathCostLabel.Content = $"{NodeA.GetEdge(NodeB)?.Cost}";
            }
            else if (_edgeDirection == EdgeDirection.BA)
                pathCostLabel.Content = $"{NodeB.GetEdge(NodeA)?.Cost}";
        }
        private void Node_Moved(object sender, EventArgs e)
        {



            double increaseScale = 0.8;
            double offset = 2 * NodeA.Scale * increaseScale;
            double arrowLength = -6 * NodeA.Scale * increaseScale;


            Point centerOfNodeA = GetCenter(NodeA);
            Point centerOfNodeB = GetCenter(NodeB);
            double x1 = centerOfNodeA.X;
            double x2 = centerOfNodeB.X;
            double y1 = centerOfNodeA.Y;
            double y2 = centerOfNodeB.Y;


            double angle = -(180 / Math.PI * Math.Atan2(y2 - y1, x2 - x1)) + 180;
            x1 = (x1 + (NodeA.ActualWidth / 2 + 5) * Math.Sin((angle - 90).ToRadians()));
            y1 = (y1 + (NodeA.ActualHeight / 2 + 5) * Math.Cos((angle - 90).ToRadians()));

            x2 = (x2 + (NodeB.ActualWidth / 2 + 3) * Math.Sin((angle + 90).ToRadians()));
            y2 = (y2 + (NodeB.ActualHeight / 2 + 3) * Math.Cos((angle + 90).ToRadians()));

            angle = -(180 / Math.PI * Math.Atan2(y2 - y1, x2 - x1)) + 180;

            Point edgeCenter = new Point(Math.Abs(x1 + x2) / 2, Math.Abs(y1 + y2) / 2);
            SetLabel(edgeCenter.X, edgeCenter.Y, angle);



            PointCollection collection = new PointCollection();
            if (_edgeDirection == EdgeDirection.BA || _edgeDirection == EdgeDirection.ABBA)
            {
                double dx11 = (x1 - arrowLength * Math.Sin((angle - 90).ToRadians())) + offset * Math.Sin(angle.ToRadians());
                double dy11 = (y1 - arrowLength * Math.Cos((angle - 90).ToRadians())) + offset * Math.Cos(angle.ToRadians());
                double dx12 = (x1 - arrowLength * Math.Sin((angle - 90).ToRadians())) - offset * Math.Sin(angle.ToRadians());
                double dy12 = (y1 - arrowLength * Math.Cos((angle - 90).ToRadians())) - offset * Math.Cos(angle.ToRadians());

                collection.Add(new Point(x1, y1));
                collection.Add(new Point(dx11, dy11));
                collection.Add(new Point(dx12, dy12));
                collection.Add(new Point(x1, y1));

                if (_edgeDirection == EdgeDirection.BA)
                    collection.Add(new Point(x2, y2));
            }

            if (_edgeDirection == EdgeDirection.AB || _edgeDirection == EdgeDirection.ABBA)
            {
                double dx21 = (x2 + arrowLength * Math.Sin((angle - 90).ToRadians())) + offset * Math.Sin(angle.ToRadians());
                double dy21 = (y2 + arrowLength * Math.Cos((angle - 90).ToRadians())) + offset * Math.Cos(angle.ToRadians());
                double dx22 = (x2 + arrowLength * Math.Sin((angle - 90).ToRadians())) - offset * Math.Sin(angle.ToRadians());
                double dy22 = (y2 + arrowLength * Math.Cos((angle - 90).ToRadians())) - offset * Math.Cos(angle.ToRadians());

                collection.Add(new Point(x2, y2));
                collection.Add(new Point(dx21, dy21));
                collection.Add(new Point(dx22, dy22));
                collection.Add(new Point(x2, y2));

                if (_edgeDirection == EdgeDirection.AB)
                    collection.Add(new Point(x1, y1));
            }
            polygon.Points = collection;

        }
    }

    enum EdgeDirection
    {
        AB,
        BA,
        ABBA
    }
}
