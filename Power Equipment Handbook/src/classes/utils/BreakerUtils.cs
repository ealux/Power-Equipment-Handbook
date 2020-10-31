using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Power_Equipment_Handbook.src;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Power_Equipment_Handbook
{

    /// <summary>
    /// Класс выключателя
    /// </summary>
    public class Breaker : INotifyPropertyChanged
    {
        private double? unom;

        #region Properties
        public double? Unom
        {
            get { return unom; }
            set { SetProperty(ref unom, value); }
        }
        #endregion Properties

        public Breaker() { }

        #region INotifyPropertyChanged interface block

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

        #endregion INotifyPropertyChanged interface block
    }


    /// <summary>
    /// Вспомогательные методы MainWindow для Выключателей
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Выбор стартового узла для Выключателя
        /// </summary>
        private void TxtStartNode_B_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                if (txtStartNode_B.SelectedIndex == txtStartNode_B.Items.Count - 1 | txtStartNode_B.SelectedIndex == -1)
                {
                    txtEndNode_B.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = track.Nodes });
                }

                if (e.AddedItems.Count == 0) return;

                try { var t = (Node)e.AddedItems[0]; }
                catch (Exception) { return; }

                if (txtStartNode_B.SelectedIndex != -1)
                {
                    ObservableCollection<Node> l = new ObservableCollection<Node>();

                    foreach (Node i in txtStartNode_B.ItemsSource)
                    {
                        if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom == ((Node)e.AddedItems[0]).Unom) l.Add(i);
                    }
                    txtEndNode_B.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l });

                    double unom = track.Nodes.Where(n => n.Number == ((Node)e.AddedItems[0]).Number).Select(n => n.Unom).First();
                    foreach (ListBoxItem i in cmbUnom_B.Items)
                    {
                        if (double.Parse(i.Content.ToString(), CultureInfo.InvariantCulture) == unom)
                        {
                            cmbUnom_B.SelectedItem = i;
                            return;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Отчистка полей Выключателей
        /// </summary>
        private void ClearBreakers()
        {
            int state = cmbUnom_B.SelectedIndex;
            cmbUnom_B.SelectedIndex = -1;
            cmbUnom_B.SelectedIndex = state;
            txtEndNode_B.SelectedIndex = -1;
            txtName_B.Text = "";
            txtState_B.Text = "0";
            txtRegion_B.Text = "";
        }



        /// <summary>
        /// Добавить Выключатель в список ветвей
        /// </summary>
        private void BtnAdd_B_Click(object sender, RoutedEventArgs e)
        {
            int start = default;
            int end = default;
            Application.Current.Dispatcher?.Invoke((Action)delegate ()
            {
                if (string.IsNullOrEmpty(txtStartNode_B.Text)) { ChangeCmbColor(txtStartNode_B, true); } else { start = int.Parse(txtStartNode_B.Text); }
                if (string.IsNullOrEmpty(txtEndNode_B.Text)) { ChangeCmbColor(txtEndNode_B, true); Log.Show("Ошибка ввода узлов Выключателя!"); return; } else { end = int.Parse(txtEndNode_B.Text); }
                if (txtStartNode_B.Text == txtEndNode_B.Text) { ChangeCmbColor(txtStartNode_B, true); ChangeCmbColor(txtEndNode_B, true); Log.Show("Узлы начала и конца совпали!"); return; }
                else { ChangeCmbColor(txtStartNode_B, false); ChangeCmbColor(txtEndNode_B, false); }

                if (start == default) { ChangeCmbColor(txtStartNode_B, true); return; }
                if (end == default) { ChangeCmbColor(txtEndNode_B, true); return; }

                int state = (string.IsNullOrWhiteSpace(txtState_B.Text) || int.Parse(txtState_B.Text) == 0) ? 0 : 1;
                string type = "Выкл.";
                string name = txtName_B.Text;
                int region = (string.IsNullOrWhiteSpace(txtRegion_B.Text) || int.Parse(txtRegion_B.Text) == 0) ? 0 : int.Parse(txtRegion_B.Text);

                Branch br = new Branch(start: start, end: end, type: type,
                                           state: state, name: name,
                                           ktr: null, region: region);

                if (BranchChecker(br, txtStartNode_L, txtEndNode_L) == true) track.AddBranch(br);
                else return;

                Application.Current.Dispatcher?.BeginInvoke((Action)delegate () { Log.Show($"Добавлен выключатель:\t{start} - {end}\t{name}", LogClass.LogType.Success); });

                Tab_Data.SelectedIndex = 1;

                #region Clear controls

                ClearBreakers();
                Log.Clear();

                #endregion Clear controls
            });
        }
    }
}
