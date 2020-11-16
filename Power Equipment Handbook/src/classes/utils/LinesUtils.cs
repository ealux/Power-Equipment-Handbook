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
    /// Класс Линии из базы
    /// </summary>
    public class Line : INotifyPropertyChanged
    {
        private double? unom;
        private string typename;
        private double? r0;
        private double? x0;
        private double? b0;
        private double? g0;
        private double? idd;
        private string source;

        #region Properties

        public double? Unom
        {
            get { return unom; }
            set { SetProperty(ref unom, value); }
        }

        public string TypeName
        {
            get { return typename; }
            set { SetProperty(ref typename, value); }
        }

        public double? R0
        {
            get { return r0; }
            set { SetProperty(ref r0, value); }
        }

        public double? X0
        {
            get { return x0; }
            set { SetProperty(ref x0, value); }
        }

        public double? B0
        {
            get { return b0; }
            set { SetProperty(ref b0, value); }
        }

        public double? G0
        {
            get { return g0; }
            set { SetProperty(ref g0, value); }
        }

        public string Source
        {
            get { return source; }
            set { SetProperty(ref source, value); }
        }

        public double? Idd
        {
            get { return idd; }
            set { SetProperty(ref idd, value); }
        }

        #endregion Properties

        public Line() { }

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
    /// Вспомогательные методы MainWindow для Линий
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Расчет параметров Линии по Длине и Удельным парамертрам (автоподстановка)
        /// </summary>
        private void Line_Calculations(object sender, TextChangedEventArgs e)
        {
            DotCommaReplacer(sender: sender, e: e); //Обработка запятых
            if (txtLength_L.Text == "") return;     //Проверка поля Длина

            Line_Calculations_Internal(sender);
        }

        /// <summary>
        /// Расчет параметров Линии по Длине и Удельным парамертрам (внутренняя реализация)
        /// </summary>
        private void Line_Calculations_Internal(object sender)
        {
            var use = (TextBox)sender;
            if (use.Text.StartsWith(".")) return;

            NumberFormatInfo provider = new NumberFormatInfo() { NumberDecimalSeparator = "." };

            double x_null_ratio = 1;
            switch (cmbGroundWire_L.SelectedIndex)
            {
                case 0:
                    x_null_ratio = 3.5;
                    break;
                case 1:
                    x_null_ratio = 3.0;
                    break;
                case 2:
                    x_null_ratio = 2.0;
                    break;
                case 3:
                    x_null_ratio = 5.5;
                    break;
                case 4:
                    x_null_ratio = 4.7;
                    break;
                case 5:
                    x_null_ratio = 3.0;
                    break;
            }

            double L = txtLength_L.Text != "" ? Convert.ToDouble(txtLength_L.Text, provider) : 0.0;

            if (use != txtLength_L)
            {
                double res;
                if (use.Text != "") res = Convert.ToDouble(use.Text, provider) * L;
                else return;

                if (use == txtr0_L)
                {
                    txtR_L.Text = res.ToString();
                    if (isCable == false) txtR0_L.Text = ((Convert.ToDouble(txtr0_L.Text, provider) + 0.15) * L).ToString();
                    else if (isCable == true) txtR0_L.Text = (res * 10).ToString();
                }
                if (use == txtx0_L)
                {
                    txtX_L.Text = res.ToString();
                    if (isCable == false) txtX0_L.Text = (res * x_null_ratio).ToString();
                    else if (isCable == true) txtX0_L.Text = (res * 4).ToString();
                }
                if (use == txtb0_L)
                {
                    txtB_L.Text = res.ToString();
                    txtB0_L.Text = (res * 0.575).ToString();
                }
                if (use == txtg0_L)
                {
                    txtG_L.Text = res.ToString();
                    txtG0_L.Text = res.ToString();
                }

            }
            else if (use == txtLength_L)
            {
                if (txtr0_L.Text != "") txtR_L.Text = (Convert.ToDouble(txtr0_L.Text, provider) * L).ToString();
                if (txtx0_L.Text != "") txtX_L.Text = (Convert.ToDouble(txtx0_L.Text, provider) * L).ToString();
                if (txtb0_L.Text != "") txtB_L.Text = (Convert.ToDouble(txtb0_L.Text, provider) * L).ToString();
                if (txtg0_L.Text != "") txtG_L.Text = (Convert.ToDouble(txtg0_L.Text, provider) * L).ToString();

                if (isCable == false) //Если линия
                {
                    if (txtr0_L.Text != "") txtR0_L.Text = ((Convert.ToDouble(txtr0_L.Text, provider) + 0.15) * L).ToString();
                    if (txtx0_L.Text != "") txtX0_L.Text = ((Convert.ToDouble(txtx0_L.Text, provider) * x_null_ratio) * L).ToString();
                    if (txtb0_L.Text != "") txtB0_L.Text = ((Convert.ToDouble(txtb0_L.Text, provider) * 0.575) * L).ToString();
                    if (txtg0_L.Text != "") txtG0_L.Text = (Convert.ToDouble(txtg0_L.Text, provider) * L).ToString();
                }
                else if (isCable == true) //Если кабель
                {
                    if (txtr0_L.Text != "") txtR0_L.Text = ((Convert.ToDouble(txtr0_L.Text, provider) * 10) * L).ToString();
                    if (txtx0_L.Text != "") txtX0_L.Text = ((Convert.ToDouble(txtx0_L.Text, provider) * 4) * L).ToString();
                    if (txtb0_L.Text != "") txtB0_L.Text = ((Convert.ToDouble(txtb0_L.Text, provider) * 0.575) * L).ToString();
                    if (txtg0_L.Text != "") txtG0_L.Text = (Convert.ToDouble(txtg0_L.Text, provider) * L).ToString();
                }
            }
        }

        /// <summary>
        /// Выбор типа грозотросса
        /// </summary>
        private void CmbGroundedWire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtx0_L != null) Line_Calculations_Internal(txtx0_L);
        }

        /// <summary>
        /// Выбор стартового узла для Линии
        /// </summary>
        private void TxtStartNode_L_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.BeginInvoke((Action)delegate ()
            {
                if (txtStartNode_L.SelectedIndex == txtStartNode_L.Items.Count - 1 | txtStartNode_L.SelectedIndex == -1) txtEndNode_L.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = Lines });

                if (e.AddedItems.Count == 0) return;

                try { var t = (Node)e.AddedItems[0]; }
                catch (Exception) { return; }

                if (txtStartNode_L.SelectedIndex != -1)
                {
                    ObservableCollection<Node> l = new ObservableCollection<Node>();

                    foreach (Node i in txtStartNode_L.ItemsSource)
                    {
                        if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom == ((Node)e.AddedItems[0]).Unom) l.Add(i);
                    }
                    txtEndNode_L.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = l });

                    double unom = track.Nodes.Where(n => n.Number == ((Node)e.AddedItems[0]).Number).Select(n => n.Unom).First();
                    foreach (ListBoxItem i in cmbUnom_L.Items)
                    {
                        if (double.Parse(i.Content.ToString(), CultureInfo.InvariantCulture) == unom)
                        {
                            cmbUnom_L.SelectedItem = i;
                            return;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Смена режима кабель/линия
        /// </summary>
        private void Chk_Line_Cable_Checked(object sender, RoutedEventArgs e)
        {
            CmbUnom_SelectionChanged(this.cmbUnom_L, null);
            if (isCable)
            {
                chk_Line_Cable.Content = "Кабель";
                cmbGroundWire_L.Visibility = Visibility.Hidden;
                lblGroundWireName_L.Visibility = Visibility.Hidden;
                ClearLines();
            }
            else
            {
                chk_Line_Cable.Content = "Линия";
                cmbGroundWire_L.Visibility = Visibility.Visible;
                lblGroundWireName_L.Visibility = Visibility.Visible;
                cmbGroundWire_L.SelectedIndex = 0;
                ClearLines();
            }
        }

        /// <summary>
        /// Отчистка полей Линий
        /// </summary>
        private void ClearLines()
        {
            txtEndNode_L.SelectedIndex = -1;
            int state = cmbUnom_L.SelectedIndex;
            cmbUnom_L.SelectedIndex = -1;
            cmbUnom_L.SelectedIndex = state;
            cmbTypeName_L.SelectedIndex = -1;
            txtName_L.Text = "";
            txtLength_L.Text = "";
            txtr0_L.DataContext = ""; txtR_L.Text = ""; txtR0_L.Text = "";
            txtx0_L.DataContext = ""; txtX_L.Text = ""; txtX0_L.Text = "";
            txtg0_L.DataContext = ""; txtG_L.Text = ""; txtG0_L.Text = "";
            txtb0_L.DataContext = ""; txtB_L.Text = ""; txtB0_L.Text = "";
            txtNpar_L.Text = ""; ; txtIdd_L.DataContext = "";
            txtState_L.IsChecked = true;
            txtRegion_L.Text = "";
            txtN_L.Text = "1";
        }



        /// <summary>
        /// Добавить Линию в список ветвей
        /// </summary>
        private void BtnAdd_L_Click(object sender, RoutedEventArgs e)
        {
            int start = default;
            int end = default;
            Application.Current.Dispatcher?.BeginInvoke((Action)delegate ()
            {
                if (txtStartNode_L.Text == "" || txtStartNode_L.Text == null) { ChangeCmbColor(txtStartNode_L, true); } else { start = int.Parse(txtStartNode_L.Text); }
                if (txtEndNode_L.Text == "" || txtEndNode_L.Text == null) { ChangeCmbColor(txtEndNode_L, true); Log.Show("Ошибка ввода узлов Линии!"); return; } else { end = int.Parse(txtEndNode_L.Text); }
                if (txtStartNode_L.Text == txtEndNode_L.Text) { ChangeCmbColor(txtStartNode_L, true); ChangeCmbColor(txtEndNode_L, true); Log.Show("Узлы начала и конца совпали!"); return; }
                else { ChangeCmbColor(txtStartNode_L, false); ChangeCmbColor(txtEndNode_L, false); }

                if (start == default) { ChangeCmbColor(txtStartNode_L, true); return; }
                if (end == default) { ChangeCmbColor(txtEndNode_L, true); return; }

                bool state = txtState_L.IsChecked == false ? false : true;
                string type = "ЛЭП";
                int npar = (string.IsNullOrWhiteSpace(txtNpar_L.Text) || int.Parse(txtNpar_L.Text) == 0) ? 0 : int.Parse(txtNpar_L.Text);
                string typename = cmbTypeName_L.Text;
                string name = txtName_L.Text;
                double r = (string.IsNullOrWhiteSpace(txtR_L.Text) || double.Parse(txtR_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtR_L.Text, CultureInfo.InvariantCulture);
                double x = (string.IsNullOrWhiteSpace(txtX_L.Text) || double.Parse(txtX_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtX_L.Text, CultureInfo.InvariantCulture);
                double b = (string.IsNullOrWhiteSpace(txtB_L.Text) || double.Parse(txtB_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtB_L.Text, CultureInfo.InvariantCulture);
                double g = (string.IsNullOrWhiteSpace(txtG_L.Text) || double.Parse(txtG_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtG_L.Text, CultureInfo.InvariantCulture);
                double r0 = (string.IsNullOrWhiteSpace(txtR0_L.Text) || double.Parse(txtR0_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtR0_L.Text, CultureInfo.InvariantCulture);
                double x0 = (string.IsNullOrWhiteSpace(txtX0_L.Text) || double.Parse(txtX0_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtX0_L.Text, CultureInfo.InvariantCulture);
                double b0 = (string.IsNullOrWhiteSpace(txtB0_L.Text) || double.Parse(txtB0_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtB0_L.Text, CultureInfo.InvariantCulture);
                double g0 = (string.IsNullOrWhiteSpace(txtG0_L.Text) || double.Parse(txtG0_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtG0_L.Text, CultureInfo.InvariantCulture);
                double idd = (string.IsNullOrWhiteSpace(txtIdd_L.Text) || double.Parse(txtIdd_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtIdd_L.Text, CultureInfo.InvariantCulture);
                int region = (string.IsNullOrWhiteSpace(txtRegion_L.Text) || int.Parse(txtRegion_L.Text) == 0) ? 0 : int.Parse(txtRegion_L.Text);

                int quant = (string.IsNullOrWhiteSpace(txtN_L.Text) || int.Parse(txtN_L.Text) <= 0) ? 1 : int.Parse(txtN_L.Text); //Количество вводимых ветвей
                int i = 0;

                do
                {
                    Branch br = new Branch(start: start, end: end, type: type,
                                        state: state, typename: typename, name: name, npar: npar + i,
                                        r: r, x: x, b: b, g: g,
                                        r0: r0, x0: x0, b0: b0, g0: g0,
                                        ktr: null, idd: idd, region: region);
                    if (BranchChecker(br, txtStartNode_L, txtEndNode_L) == true) track.AddBranch(br);
                    else return;
                    i++;

                } while (i < quant);

                Application.Current.Dispatcher?.BeginInvoke((Action)delegate () { Log.Show($"Добавлена линия:\t{start} - {end}\t{name}", LogClass.LogType.Success); });

                Tab_Data.SelectedIndex = 1;

                #region Clear controls

                ClearLines();
                Log.Clear();

                #endregion Clear controls
            });
        }
    }
}
