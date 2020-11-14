using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Window=System.Windows.Window;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PowerFlowCore;
using PowerFlowCore.Data;
using Complex = System.Numerics.Complex;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;

using Power_Equipment_Handbook;
using Power_Equipment_Handbook.src;

namespace Power_Equipment_Handbook.src.windows
{
    /// <summary>
    /// Логика взаимодействия для PowerFlowManager.xaml
    /// </summary>
    public partial class PowerFlowManager : Window, INotifyPropertyChanged
    {
        public Engine e;
        EngineOptions options;
        public Converter conv;

        ObservableCollection<Node> Nodes;
        ObservableCollection<Branch> Branches;

        /// <summary>
        /// Преобразователь схем
        /// </summary>
        internal Converter Conv { get => conv; set => SetProperty(ref conv, value); }


        /// <summary>
        /// Расчетное ядро
        /// </summary>
        public Engine Engine { get => e; set => SetProperty(ref e, value); }

        /// <summary>
        /// Настройки расчетног ядра
        /// </summary>
        public EngineOptions Options { get => options; set => SetProperty(ref options, value); }

        ///// <summary>
        ///// Коллекция Узлов
        ///// </summary>
        //public ObservableCollection<Node> Nodes { get => nds; set => SetProperty(ref nds, value); }

        ///// <summary>
        ///// Коллекция Ветвей
        ///// </summary>
        //public ObservableCollection<Branch> Branches { get => brs; set => SetProperty(ref brs, value); }
        


        //ctor
        public PowerFlowManager(ref ObservableCollection<Node> nodes, ref ObservableCollection<Branch> branches)
        {
            InitializeComponent();

            this.Nodes = nodes;
            this.Branches = branches;            

            this.grdNodes.ItemsSource = this.Nodes;
            this.grdBranches.ItemsSource = this.Branches;

            this.Options = new EngineOptions();
        }


        /// <summary>
        /// Расчётный метод
        /// </summary>
        public void Calculate(int count = 3)
        {

            //ВАЛИДАТОРЫ!!!


            try
            {
                this.Conv = new Converter(this.Nodes, this.Branches);
                this.Engine = new Engine(this.Conv, this.Options);

                this.Nodes.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => this.Engine.NeedsToCalc = true;
                this.Branches.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => this.Engine.NeedsToCalc = true;

                for (int i = 0; i < count; i++)
                {
                    this.Engine.Calculate();
                }
                               

                this.Conv.FillElementsBack(this.Engine.desc, ref this.Nodes, ref this.Branches);

                this.Engine.NeedsToCalc = false;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region [Helpers]

        /// <summary>
        /// Блокирует закрытие окна Менеджера режимов
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        #endregion

        #region [INotifyPropertyChanged interface block]

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value; OnPropertyChanged(propertyName); return true;
        }

        #endregion [INotifyPropertyChanged interface block]

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Calculate();
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class Converter : IConverter
    {
        public IEnumerable<INode> Nodes { get; set; }
        public IEnumerable<IBranch> Branches { get; set; }


        public Converter(IEnumerable<src.Node> nodes, IEnumerable<Branch> branches)
        {
            this.Nodes = this.EnumerateNodes(nodes);
            this.Branches = this.EnumerateBranches(branches);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public IEnumerable<INode> EnumerateNodes(IEnumerable<Node> nodes)
        {
            var out_n = new List<INode>();

            foreach (var item in nodes)
            {
                var type = item.Type == "Нагр" ? NodeType.PQ : item.Type == "База" ? NodeType.Slack : NodeType.PV;

                //Check if PV
                var vpreN = item.Vzd == 0.0;
                var qminN = item.Q_min == 0.0;
                var qmaxN = item.Q_max == 0.0;
                if (type == NodeType.PQ)
                {                  
                    if (!vpreN & !qminN & !qmaxN)
                    {
                        type = NodeType.PV;
                        item.Type = "Ген";
                    }
                }
                else if(type == NodeType.PV)
                {
                    if ((vpreN & qminN) | (qminN & qmaxN))
                    {
                        type = NodeType.PQ;
                        item.Type = "Нагр";
                    }
                    else if (!qminN & qmaxN)
                    {
                        type = NodeType.PQ;
                        item.Q_g = item.Q_min;
                        item.Type = "Нагр";
                    }
                    else if (qminN & !qmaxN)
                    {
                        type = NodeType.PQ;
                        item.Type = "Нагр";
                    }
                }

                out_n.Add(new PFNode()
                {
                    Num = item.Number,
                    Unom = Complex.FromPolarCoordinates(item.Unom, item.Delta * Math.PI / 180),
                    Q_min = item.Q_min,
                    Q_max = item.Q_max,
                    S_gen = new Complex(item.P_g, item.Q_g),
                    S_load = new Complex(item.P_n, item.Q_n),
                    Vpre = item.Vzd,
                    Ysh = new Complex(item.G_sh * 1e-6, item.B_sh * 1e-6),
                    Type = type
                });
            }

            return out_n.AsEnumerable<INode>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="branches"></param>
        /// <returns></returns>
        public IEnumerable<IBranch> EnumerateBranches(IEnumerable<Branch> branches)
        {
            var out_b = new List<IBranch>();

            foreach (var item in branches)
            {                
                if (item.Ktr == null || (item.Ktr.Value <= 0 | item.Ktr.Value == 1)) 
                {
                    if (item.R == 0.0 & item.X == 0.0 & item.G == 0.0 & item.B == 0.0) item.Type = "Выкл.";
                    else item.Type = "ЛЭП";
                }

                var t = item.Type;
                var kt = ((t == "ЛЭП") | (t == "Выкл.")) ? 1.0 : item.Ktr.HasValue ? item.Ktr.Value : 1.0;
                var bsh = (t == "ЛЭП") ? item.B : -item.B; 

                out_b.Add(new PFBranch()
                {
                    Start = item.Start,
                    End = item.End,
                    Ktr = kt,
                    Y = item.Type != "Выкл." ? 1/new Complex(item.R, item.X) : 1 / new Complex(0.1, 0.1),
                    Ysh = new Complex(item.G * 1e-6, bsh * 1e-6)                    
                });
            }

            return out_b.AsEnumerable<IBranch>();
        }        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="nodes"></param>
        /// <param name="branches"></param>
        public void FillElementsBack(NetDescription description, ref ObservableCollection<Node> nodes, ref ObservableCollection<Branch> branches)
        {
            //nodes
            foreach (var t_node in nodes)
            {
                foreach (var s_node in description.Nodes)
                {
                    if (t_node.Number == s_node.Num)
                    {
                        t_node.U = s_node.U;
                        t_node.P_g = s_node.S_gen.Real;
                        t_node.Q_g = s_node.S_gen.Imaginary;
                        t_node.Delta = s_node.U.Phase * 180 / Math.PI;
                    }

                }
            }

            //branches
            foreach (var t_br in branches)
            {
                foreach (var s_br in description.Branches)
                {
                    if ((t_br.Start == s_br.Start) & (t_br.End == s_br.End))
                    {
                        t_br.S_start = s_br.S_start;
                        t_br.S_end = s_br.S_end;
                        t_br.I_start = s_br.I_start;
                        t_br.I_end = s_br.I_end;
                    }
                }
            }
        }
    }
}
