using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Intelektika.Algorithms;
using Intelektika;

namespace Intelektika
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graph graph;
        private Task task;
        private CancellationTokenSource cancelTokenSource;
        private SolidColorBrush markedNode = new SolidColorBrush(Color.FromArgb(220, 120, 93, 93));
        private SolidColorBrush goalNode = new SolidColorBrush(Color.FromArgb(240, 23, 116, 77));
        private SolidColorBrush unmarkedNode = new SolidColorBrush(Color.FromArgb(100, 205, 206, 218));

        public MainWindow()
        {
            InitializeComponent();

            graph = new Graph(mainCanvas);
            cancelTokenSource = new CancellationTokenSource();
            //graph.SetGraphMap();
            //graph.LoadGraphFromFile(@"binary");

            cbSearchFor.ItemsSource = graph.observableCollectionOfNodeNames;
            cbSearchFrom.ItemsSource = graph.observableCollectionOfNodeNames;
            cbSuccesor.ItemsSource = graph.observableCollectionOfNodeNames;

            graph.NodeSelectedChanged += Graph_NodeSelectedChanged;
            UseLayoutRounding = true;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            graph.Refresh();
            labelAnimationSpeed.Content = $"Animation speed - {Math.Round(sliderAnimationSpeed.Value, 2)} s";
            checkBockPathVisible.IsChecked = graph.PathCostVisible;
            checkBockPathVisible_Copy.IsChecked = graph.NodeCostVisible;
            Graph_NodeSelectedChanged(null, null);
            try
            {
                var files = Directory.GetFiles("..\\..\\Resources");
                cbSavedGraphs.ItemsSource = GetXmlFilesFromResourcesFolder();
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message);}


            Button_Click_2(null, null);

        }

        //NODE
        private void buttonAddNewNode_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxNewNodeName1.Text.Trim().Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("Empty spaces");
                return;
            }
            if (graph.GetNodeByName(textBoxNewNodeName1.Text.Trim()) != null)
            {
                System.Windows.Forms.MessageBox.Show("Node with such name already exists");
                return;
            }

            graph.AddNode(new Node(textBoxNewNodeName1.Text.Trim()));
            textBoxNewNodeName1.Text = "";
        }
        private void Graph_NodeSelectedChanged(object sender, EventArgs e)
        {

            Node n = sender as Node;

            if (n == null)
            {
                tbNodeSelected.Text = "";
                cbSuccesor.IsEnabled = false;
                tbNodeSelected.IsEnabled = false;
            }
            else
            {
                tbNodeSelected.Text = n.Name;
                //tbNodeSelected.IsEnabled = true;
                cbSuccesor.IsEnabled = true;
            }
            cbSuccesor.SelectedValue = "";
            tbPathCost.IsEnabled = false;
            tbPathCost.Text = "";
            buttonRemovePath.IsEnabled = false;
            buttonAddPath.IsEnabled = false;

        }
        private void tbNodeSelected_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (graph.NodeSelected == null)
                return;

            graph.NodeSelected.Name = tbNodeSelected.Text;

        }
        private void tbNodeSelected_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cbSuccesor.Focus();
            }
        }
        private void cbSuccesor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (graph.NodeSelected == null)
                return;

            Node node = graph.NodeSelected.GetSuccesor(cbSuccesor.SelectedItem?.ToString());
            if (node == null)//means dont have selected succesor
            {
                buttonAddPath.IsEnabled = true;
                buttonRemovePath.IsEnabled = false;
                tbPathCost.IsEnabled = true;
            }
            else
            {
                tbPathCost.Text = graph.NodeSelected.GetEdge(node).Cost.ToString();
                tbPathCost.IsEnabled = true;
                buttonAddPath.IsEnabled = false;
                buttonRemovePath.IsEnabled = true;
            }
        }
        private void cbSuccesor_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void buttonAddPath_Click(object sender, RoutedEventArgs e)
        {
            if (graph.NodeSelected == null)
                return;

            Node edgeTo = graph.GetNodeByName(cbSuccesor.SelectedItem?.ToString());
            if (edgeTo == null)
            {
                System.Windows.Forms.MessageBox.Show("Error ..");
            }
            bool added = graph.NodeSelected.AddEdgeToNode(edgeTo, (tbPathCost.Text.Trim().Length == 0) ? 0 : double.Parse(tbPathCost.Text));
            //cbSuccesor_SelectionChanged(cbSuccesor, null);
        }
        private void buttonRemovePath_Click(object sender, RoutedEventArgs e)
        {
            if (graph.NodeSelected == null)
                return;

            Node remove = graph.GetNodeByName(cbSuccesor.SelectedItem?.ToString());

            graph.NodeSelected.RemoveEdge(remove.Name);
            cbSuccesor_SelectionChanged(cbSuccesor, null);
        }
        private void tbPathCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (graph.NodeSelected == null)
                return;

            Edge edge = graph.NodeSelected.GetEdge(cbSuccesor.SelectedItem?.ToString());
            if (edge == null)
                return;
            try
            {

                if (tbPathCost.Text.Trim().Length == 0)
                {
                    edge.Cost = 0;
                }
                else
                {
                    edge.Cost = double.Parse(tbPathCost.Text);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }



        }
        private void tbPathCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void textBoxNewNodeName1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonAddNewNode_Click(sender, null);
            }
        }

        //ALGORITHMS
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (task != null && task.Status == TaskStatus.Running)
            {
                cancelTokenSource.Cancel();

                while (task.Status == TaskStatus.Running)
                {
                    Thread.Sleep(10);
                }
                return;
            }


            foreach (Node n in graph.Nodes)
                n.Background = unmarkedNode;


            IAlgorithm algorithm = null;
            string alg = cbAlgorithm.SelectedItem.ToString().Split(':')[1].Trim();
            switch (alg)
            {

                case "Breadth-first search":
                    {
                        BreadthFirstSearch bfs = new BreadthFirstSearch();
                        bfs.Search(graph.GetNodeByName(cbSearchFrom.SelectedItem?.ToString()),
                                                       cbSearchFor.SelectedItem?.ToString());
                        algorithm = bfs;
                        break;
                    }
                case "Depth-first search":
                    {
                        DepthFirstSearch dfs = new DepthFirstSearch();
                        dfs.Search(graph.GetNodeByName(cbSearchFrom.SelectedItem?.ToString()),
                                                       cbSearchFor.SelectedItem?.ToString());
                        algorithm = dfs;
                        break;
                    }
                case "Depth-limited search":
                    {
                        DepthLimitedSearch dfs = new DepthLimitedSearch();
                        dfs.Search(graph.GetNodeByName(cbSearchFrom.SelectedItem?.ToString()),
                                                       cbSearchFor.SelectedItem?.ToString(),
                                                       tbDepthLimit.Text.Length == 0 ? 0 : int.Parse(tbDepthLimit.Text), out int d);
                        algorithm = dfs;
                        break;
                    }
                case "Uniform-cost search":
                    {
                        UniformCostSearch dfs = new UniformCostSearch();
                        dfs.Search(graph.GetNodeByName(cbSearchFrom.SelectedItem?.ToString()),
                                                       cbSearchFor.SelectedItem?.ToString());
                        algorithm = dfs;
                        break;
                    }
                case "Iterative deepening depth-first search":
                    {
                        IterativeDeepeningSearch ids = new IterativeDeepeningSearch();
                        ids.Search(graph.GetNodeByName(cbSearchFrom.SelectedItem?.ToString()),
                                                       cbSearchFor.SelectedItem?.ToString());
                        algorithm = ids;
                        break;
                    }
                case "Greedy best-first search":
                    {
                        GreedyBestFirstSearch dfs = new GreedyBestFirstSearch();
                        dfs.Search(graph.GetNodeByName(cbSearchFrom.SelectedItem?.ToString()),
                                                       cbSearchFor.SelectedItem?.ToString());
                        algorithm = dfs;
                        break;
                    }
                case "A* search":
                    {
                        AstarSearch dfs = new AstarSearch();
                        dfs.Search(graph.GetNodeByName(cbSearchFrom.SelectedItem?.ToString()),
                                                       cbSearchFor.SelectedItem?.ToString());
                        algorithm = dfs;
                        break;
                    }
                case "Memory-bounded heuristic search":
                    {
                        MemoryBoundedHeuristicSearch dfs = new MemoryBoundedHeuristicSearch();
                        dfs.Search(graph.GetNodeByName(cbSearchFrom.SelectedItem?.ToString()),
                                                       cbSearchFor.SelectedItem?.ToString());
                        algorithm = dfs;
                        break;
                    }


                default: break;
            }
            if (algorithm == null)
                return;


            cancelTokenSource = new CancellationTokenSource();
            task = new Task(() =>
            {
                DateTime dateTime;
                foreach (Node n in algorithm.VisitedNodes)
                {
                    double delay = 0;
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        delay = sliderAnimationSpeed.Value;

                    }));

                    dateTime = DateTime.Now;
                    while (dateTime.AddSeconds(delay) > DateTime.Now)
                    {
                        if (cancelTokenSource.IsCancellationRequested)
                            return;
                        Thread.Sleep(10);

                    }
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        if (n.Background == markedNode)//iterative deepening on stage
                        {
                            foreach (Node nn in graph.Nodes)
                                nn.Background = unmarkedNode;
                        }

                        n.Background = markedNode;

                    }));
                }

                if (algorithm.Result != null)
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        algorithm.Result.Background = goalNode;

                    }));

            }, cancelTokenSource.Token);
            task.Start();

        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            labelAnimationSpeed.Content = $"Animation speed - {Math.Round(e.NewValue, 2)} s";
        }
        private void cbAlgorithm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string alg = cbAlgorithm.SelectedItem.ToString().Split(':')[1].Trim();

            if (ContentDepthLimitedSearch == null)
                return;
            ContentDepthLimitedSearch.Visibility = Visibility.Collapsed;


            switch (alg)
            {
                case "Depth-limited search":
                    {
                        ContentDepthLimitedSearch.Visibility = Visibility.Visible;
                        break;
                    }

            }
        }

        private void tbDepthLimit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        //GRAPHS
        private void ButtonGraph1_Click(object sender, RoutedEventArgs e)
        {
            graph.SetGraphMap();
            Task.Run(() =>
            {
                Thread.Sleep(1);
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { graph.Refresh(); }));
            });

        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            graph.SetGraphBinaryTree();
            Task.Run(() =>
            {
                Thread.Sleep(1);
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { graph.Refresh(); }));
            });
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            graph.SetGraphRandom();
            Task.Run(() =>
            {
                Thread.Sleep(1);
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { graph.Refresh(); }));
            });
        }
        private void Button_Click(object sender, RoutedEventArgs e)//clear
        {
            graph.Clear();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)//load
        {
            string fileName = cbSavedGraphs.SelectedItem?.ToString();
            if (fileName == null || fileName.Length == 0)
                return;
            try
            {
                graph.LoadGraphFromFile(fileName);
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message); }

        }
        private void Button_Click_3(object sender, RoutedEventArgs e)//save graph
        {
            if (tbSaveGraph.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("Empty spaces");
                return;
            }

            if (!string.IsNullOrEmpty(tbSaveGraph.Text) && tbSaveGraph.Text.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
            {
                System.Windows.Forms.MessageBox.Show("Invalid characters");
                return;
            }
            try
            {
                graph.SaveCurrentGraph(tbSaveGraph.Text);
                tbSaveGraph.Text = "";
                var files = Directory.GetFiles("..\\..\\Resources");
                cbSavedGraphs.ItemsSource = GetXmlFilesFromResourcesFolder();


            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)//delete
        {
            string selection = cbSavedGraphs.SelectedItem?.ToString();
            if (selection == null)
            {
                return;
            }

            try
            {
                var result = System.Windows.Forms.MessageBox.Show("Really?", "Message", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    File.Delete("..\\..\\Resources\\" + selection + ".xml");
                    cbSavedGraphs.ItemsSource = GetXmlFilesFromResourcesFolder();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        //OPTIONS
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            graph.PathCostVisible = false;
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            graph.PathCostVisible = true;
        }
        private void checkBockPathVisible_Copy_Checked(object sender, RoutedEventArgs e)
        {
            graph.NodeCostVisible = true;
        }
        private void checkBockPathVisible_Copy_Unchecked(object sender, RoutedEventArgs e)
        {
            graph.NodeCostVisible = false;
        }


        private string[] GetXmlFilesFromResourcesFolder()
        {
            var files = Directory.GetFiles("..\\..\\Resources");

            return files.Where(x => System.IO.Path.GetExtension(x) == ".xml")
                        .Select(x => System.IO.Path.GetFileNameWithoutExtension(x)).ToArray();
        }


    }
}
