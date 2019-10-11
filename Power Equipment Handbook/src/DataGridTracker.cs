using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Power_Equipment_Handbook.src
{
    public class DataGridTracker
    {
        public DataGrid grdNodes { get; set; }
        public DataGrid grdBranches { get; set; }

        public ObservableCollection<Node> Nodes = new ObservableCollection<Node>();
        public ObservableCollection<Branch> Branches = new ObservableCollection<Branch>();

        public DataGridTracker(DataGrid grdNodes, DataGrid grdBranches)
        {
            this.grdNodes = grdNodes;
            this.grdBranches = grdBranches;

            this.grdNodes.ItemsSource = Nodes;
            this.grdBranches.ItemsSource = Branches;
        }
        public void AddNode(Node node)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                                                {
                                                    Nodes.Add(node);
                                                    grdNodes.UpdateLayout();
                                                });
        }
        public void AddBranch(Branch branch)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                                                {
                                                    Branches.Add(branch);
                                                    grdBranches.UpdateLayout();
                                                });
        }
    }

    public class Branch : INotifyPropertyChanged
    {
        private int state;
        private string type;
        private int start; private int end;
        private int npar;
        private string name;
        private double r; private double x; private double g; private double b;
        private double ktr;
        private double idd;
        private int region;

        #region Properties
        public int State
        {
            get { return state; }
            set { SetProperty(ref state, value); }
        }
        public string Type
        {
            get => type;
            set { SetProperty(ref type, value); }
        }
        public int Start
        {
            get => start;
            set { SetProperty(ref start, value); }
        }
        public int End
        {
            get => end;
            set { SetProperty(ref end, value); }
        }
        public int Npar
        {
            get => npar;
            set { SetProperty(ref npar, value); }
        }
        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }
        public double R
        {
            get => r;
            set { SetProperty(ref r, value); }
        }
        public double X
        {
            get => x;
            set { SetProperty(ref x, value); }
        }
        public double G
        {
            get => g;
            set { SetProperty(ref g, value); }
        }
        public double B
        {
            get => b;
            set { SetProperty(ref b, value); }
        }
        public double Ktr
        {
            get => ktr;
            set { SetProperty(ref ktr, value); }
        }
        public double Idd
        {
            get => idd;
            set { SetProperty(ref idd, value); }
        }
        public int Region
        {
            get => region;
            set { SetProperty(ref region, value); }
        }
        #endregion
        
        public Branch() { }

        public Branch(int start, int end, string type, int state = 0, string name = "", int npar = 0, 
                      double r = 0, double x = 0, double b = 0, double g = 0, double ktr = 0,
                      double idd = 0, int region = 0)
        {
            State = state;
            Type = type;
            Start = start;
            End = end;
            Npar = npar;
            Name = name;
            R = r; X = x; B = b; G = g;
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
        #endregion
    }

    public class Node:INotifyPropertyChanged
    {
        private int state;
        private string type;
        private int number;
        private int unom;
        private string name;
        private double p_n; private double q_n; private double p_g; private double q_g;
        private double vzd;
        private double q_min; private double q_max;
        private double b_sh;
        private int region;

        #region Properties
        public int State
        {
            get { return state; }
            set { SetProperty(ref state, value); }
        }
        public string Type
        {
            get => type;
            set { SetProperty(ref type, value); }
        }
        public int Number
        {
            get => number;
            set { SetProperty(ref number, value); }
        }
        public int Unom
        {
            get => unom;
            set { SetProperty(ref unom, value); }
        }
        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }
        public double P_n
        {
            get => p_n;
            set { SetProperty(ref p_n, value); }
        }
        public double Q_n
        {
            get => q_n;
            set { SetProperty(ref q_n, value); }
        }
        public double P_g
        {
            get => p_g;
            set { SetProperty(ref p_g, value); }
        }
        public double Q_g
        {
            get => q_g;
            set { SetProperty(ref q_g, value); }
        }
        public double Vzd
        {
            get => vzd;
            set { SetProperty(ref vzd, value); }
        }
        public double Q_min
        {
            get => q_min;
            set { SetProperty(ref q_min, value); }
        }
        public double Q_max
        {
            get => q_max;
            set { SetProperty(ref q_max, value); }
        }
        public double B_sh
        {
            get => b_sh;
            set { SetProperty(ref b_sh, value); }
        }
        public int Region
        {
            get => region;
            set { SetProperty(ref region, value); }
        }
        #endregion

        public Node() { }
        public Node(int number, int unom, string type, int state = 0, string name = "",
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
        #endregion
    }
}
