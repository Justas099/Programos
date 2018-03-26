using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektika
{
    class Edge
    {
        public event EventHandler Edited;

        private double _cost;
        private Node _node;

        public double Cost
        {
            get => _cost;
            set
            {
                if (_cost == value)
                    return;

                _cost = value;
                Edited?.Invoke(this, EventArgs.Empty);
            }
        }
        public Node Node
        {
            get => _node;
            set
            {
                if (_node == value)
                    return;

                _node = value;
                Edited?.Invoke(this, EventArgs.Empty);
            }
        }

        public Edge(Node node, double cost)
        {
            this.Node = node;
            this.Cost = cost;
        }
        public Edge(Node node)
        {
            this.Node = node;
            this.Cost = 0;
        }
    }
}
