using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Power_Equipment_Handbook.src
{
    [Serializable]
    [XmlType("SchemaContainer")]
    public class DataGridTracker
    {

        [XmlIgnore]
        public DataGrid grdNodes { get; set; }

        [XmlIgnore]
        public DataGrid grdBranches { get; set; }

        [XmlArrayItem("Node", Type =typeof(Node))]
        public ObservableCollection<Node> Nodes = new ObservableCollection<Node>();

        [XmlArrayItem("Branch", Type = typeof(Branch))]
        public ObservableCollection<Branch> Branches = new ObservableCollection<Branch>();

        public DataGridTracker() { }

        public DataGridTracker(DataGrid grdNodes, DataGrid grdBranches)
        {
            this.grdNodes = grdNodes; this.grdNodes.ItemsSource = Nodes;
            this.grdBranches = grdBranches; this.grdBranches.ItemsSource = Branches;
        }

        /// <summary>
        /// Добавить узел в список узлов
        /// </summary>
        /// <param name="node">Добавляемый узел</param>
        public void AddNode(Node node) => Application.Current.Dispatcher?.BeginInvoke((Action)delegate{ Nodes.Add(node); grdNodes.UpdateLayout(); });

        /// <summary>
        /// Добавить ветвь в список ветвей
        /// </summary>
        /// <param name="branch">Добавляемая ветвь</param>
        public void AddBranch(Branch branch) => Application.Current.Dispatcher?.BeginInvoke((Action)delegate{ Branches.Add(branch); grdBranches.UpdateLayout(); });
    }

    /// <summary>
    /// Тип ветви в сети
    /// </summary>
    public class Branch : INotifyPropertyChanged, IEquatable<Branch>
    {
        private int state;
        private string type;
        private int start; private int end;
        private int npar;
        private string name;
        private string typename;
        private double r; private double x; private double g; private double b;
        private double r0; private double x0; private double g0; private double b0;
        private double? ktr;
        private double idd;
        private int region;

        #region Properties

        [XmlAttribute]
        public int State
        {
            get { return state; }
            set { SetProperty(ref state, value); }
        }

        [XmlAttribute]
        public string Type
        {
            get => type;
            set { SetProperty(ref type, value); }
        }

        [XmlAttribute]
        public int Start
        {
            get => start;
            set { SetProperty(ref start, value); }
        }

        [XmlAttribute]
        public int End
        {
            get => end;
            set { SetProperty(ref end, value); }
        }

        [XmlAttribute]
        public int Npar
        {
            get => npar;
            set { SetProperty(ref npar, value); }
        }

        [XmlAttribute]
        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }

        [XmlAttribute]
        public string TypeName
        {
            get => typename;
            set { SetProperty(ref typename, value); }
        }

        [XmlAttribute]
        public double R
        {
            get => r;
            set { SetProperty(ref r, value); }
        }

        [XmlAttribute]
        public double X
        {
            get => x;
            set { SetProperty(ref x, value); }
        }

        [XmlAttribute]
        public double G
        {
            get => g;
            set { SetProperty(ref g, value); }
        }

        [XmlAttribute]
        public double B
        {
            get => b;
            set { SetProperty(ref b, value); }
        }
        [XmlAttribute]
        public double R0
        {
            get => r0;
            set { SetProperty(ref r0, value); }
        }

        [XmlAttribute]
        public double X0
        {
            get => x0;
            set { SetProperty(ref x0, value); }
        }

        [XmlAttribute]
        public double G0
        {
            get => g0;
            set { SetProperty(ref g0, value); }
        }

        [XmlAttribute]
        public double B0
        {
            get => b0;
            set { SetProperty(ref b0, value); }
        }

        [XmlIgnore]
        public double? Ktr
        {
            get => ktr;
            set { SetProperty(ref ktr, value); }
        }

        #region Ktr property to Text
        [XmlElement("Ktr")]
        public string AgeAsText
        {
            get { return (Ktr.HasValue) ? Ktr.ToString() : null; }
            set { Ktr = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }
        }
        #endregion Ktr property to Text

        [XmlAttribute]
        public double Idd
        {
            get => idd;
            set { SetProperty(ref idd, value); }
        }

        [XmlAttribute]
        public int Region
        {
            get => region;
            set { SetProperty(ref region, value); }
        }

        #endregion Properties

        public Branch()
        {
        }

        public Branch(int start, int end, string type, double? ktr, int state = 0, string typename = "", string name = "",
                      int npar = 0, 
                      double r = 0, double x = 0, double b = 0, double g = 0,
                      double r0 = 0, double x0 = 0, double b0 = 0, double g0 = 0,
                      double idd = 0, int region = 0)
        {
            State = state;
            Type = type;
            Start = start;
            End = end;
            Npar = npar;
            TypeName = typename; Name = name;
            R = r; X = x; B = b; G = g;
            R0 = r0; X0 = x0; B0 = b0; G0 = g0;
            Ktr = ktr;
            Idd = idd;
            Region = region;
        }

        #region INotifyPropertyChanged interface block

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value; OnPropertyChanged(propertyName); return true;
        }

        #endregion INotifyPropertyChanged interface block

        #region IEquatable interface block

        public new int GetHashCode()
        {
            return ((object)this).GetHashCode();
        }

        /// <summary>
        /// Возвращает результат сравнения Branch с другим экземпляром Branch
        /// </summary>
        public bool Equals(Branch other)
        {
            if (other == null) return false;
            else if (type == other.type & start == other.start & end == other.end) return true;
            else return false;
        }

        #endregion IEquatable interface block
    }

    /// <summary>
    /// Тип узла в сети
    /// </summary>
    public class Node : INotifyPropertyChanged, IEquatable<Node>
    {
        private int state;
        private string type;
        private int number;
        private double unom;
        private string name;
        private double p_n; private double q_n; private double p_g; private double q_g;
        private double vzd;
        private double q_min; private double q_max;
        private double b_sh;
        private int region;

        #region Properties

        [XmlAttribute]
        public int State
        {
            get { return state; }
            set { SetProperty(ref state, value); }
        }

        [XmlAttribute]
        public string Type
        {
            get => type;
            set { SetProperty(ref type, value); }
        }

        [XmlAttribute]
        public int Number
        {
            get => number;
            set { SetProperty(ref number, value); }
        }

        [XmlAttribute]
        public double Unom
        {
            get => unom;
            set { SetProperty(ref unom, value); }
        }

        [XmlAttribute]
        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }

        [XmlAttribute]
        public double P_n
        {
            get => p_n;
            set { SetProperty(ref p_n, value); }
        }

        [XmlAttribute]
        public double Q_n
        {
            get => q_n;
            set { SetProperty(ref q_n, value); }
        }

        [XmlAttribute]
        public double P_g
        {
            get => p_g;
            set { SetProperty(ref p_g, value); }
        }

        [XmlAttribute]
        public double Q_g
        {
            get => q_g;
            set { SetProperty(ref q_g, value); }
        }

        [XmlAttribute]
        public double Vzd
        {
            get => vzd;
            set { SetProperty(ref vzd, value); }
        }

        [XmlAttribute]
        public double Q_min
        {
            get => q_min;
            set { SetProperty(ref q_min, value); }
        }

        [XmlAttribute]
        public double Q_max
        {
            get => q_max;
            set { SetProperty(ref q_max, value); }
        }

        [XmlAttribute]
        public double B_sh
        {
            get => b_sh;
            set { SetProperty(ref b_sh, value); }
        }

        [XmlAttribute]
        public int Region
        {
            get => region;
            set { SetProperty(ref region, value); }
        }

        #endregion Properties

        public Node()
        {
        }

        public Node(int number, double unom, string type, int state = 0, string name = "",
                      double p_n = 0, double q_n = 0, double p_g = 0, double q_g = 0,
                      double vzd = 0, double q_min = 0, double q_max = 0, double b_sh = 0,
                      int region = 0)
        {
            State = state;
            Type = type;
            Number = number;
            Unom = unom;
            Name = name;
            P_n = p_n; Q_n = q_n; P_g = p_g; Q_g = q_g;
            Vzd = vzd; Q_min = q_min; Q_max = q_max; B_sh = b_sh;
            Region = region;
        }

        #region INotifyPropertyChanged interface block

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value; OnPropertyChanged(propertyName); return true;
        }

        #endregion INotifyPropertyChanged interface block

        #region IEquatable interface block

        public new int GetHashCode()
        {
            return this.GetHashCode();
        }

        /// <summary>
        /// Возвращает результат сравнения Node с другим экземпляром Node
        /// </summary>
        public bool Equals(Node other)
        {
            if (other == null) return false;
            if (type == other.type & number == other.number) return true;
            return false;
        }

        #endregion IEquatable interface block
    }

    #region IEqualityComparer interface block

    /// <summary>
    /// IEqualityComparer fo Branches
    /// </summary>
    class BranchEqualityComparer : IEqualityComparer<Branch>
    {
        public bool Equals(Branch b1, Branch b2)
        {
            if (b1 == null || b2 == null) return false;
            return (b1.Start == b2.Start) & (b1.End == b2.End);
        }

        public int GetHashCode(Branch obj)
        {
            return obj.Start.GetHashCode()^obj.End.GetHashCode()^obj.Type.GetHashCode();
        }
    }

    #endregion IEqualityComparer interface block
}