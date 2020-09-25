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
    /// Класс двухобмоточного Трансформатора из базы
    /// </summary>
    public class Trans : INotifyPropertyChanged
    {
        private double? unom;
        private string type;
        private string typename;
        private double? inom;
        private double? unomh; private double? unoml;
        private double? r; private double? x; private double? b; private double? g;
        private string source;

        #region Properties

        public double? Unom
        {
            get { return unom; }
            set { SetProperty(ref unom, value); }
        }

        public string Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        public string TypeName
        {
            get { return typename; }
            set { SetProperty(ref typename, value); }
        }

        public double? Inom
        {
            get { return inom; }
            set { SetProperty(ref inom, value); }
        }

        public double? UnomH
        {
            get { return unomh; }
            set { SetProperty(ref unomh, value); }
        }

        public double? UnomL
        {
            get { return unoml; }
            set { SetProperty(ref unoml, value); }
        }

        public double? R
        {
            get { return r; }
            set { SetProperty(ref r, value); }
        }

        public double? X
        {
            get { return x; }
            set { SetProperty(ref x, value); }
        }

        public double? B
        {
            get { return b; }
            set { SetProperty(ref b, value); }
        }

        public double? G
        {
            get { return g; }
            set { SetProperty(ref g, value); }
        }

        public string Source
        {
            get { return source; }
            set { SetProperty(ref source, value); }
        }

        #endregion Properties

        public Trans()
        {
        }

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
    /// Класс трехобмоточного (Автотрансформатора) Трансформатора из базы
    /// </summary>
    public class MultiTrans : INotifyPropertyChanged
    {
        private double? unom;
        private string type;
        private string typename;
        private double? inomh, inomm, inoml;
        private double? unomh; private double? unomm; private double? unoml;
        private double? rh; private double? rm; private double? rl;
        private double? xh; private double? xm; private double? xl;
        private double? b; private double? g;
        private string source;

        #region Properties

        public double? Unom
        {
            get { return unom; }
            set { SetProperty(ref unom, value); }
        }

        public string Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        public string TypeName
        {
            get { return typename; }
            set { SetProperty(ref typename, value); }
        }

        public double? Inomh
        {
            get { return inomh; }
            set { SetProperty(ref inomh, value); }
        }

        public double? Inomm
        {
            get { return inomm; }
            set { SetProperty(ref inomm, value); }
        }

        public double? Inoml
        {
            get { return inoml; }
            set { SetProperty(ref inoml, value); }
        }

        public double? UnomH
        {
            get { return unomh; }
            set { SetProperty(ref unomh, value); }
        }

        public double? UnomM
        {
            get { return unomm; }
            set { SetProperty(ref unomm, value); }
        }

        public double? UnomL
        {
            get { return unoml; }
            set { SetProperty(ref unoml, value); }
        }

        public double? RH
        {
            get { return rh; }
            set { SetProperty(ref rh, value); }
        }

        public double? RM
        {
            get { return rm; }
            set { SetProperty(ref rm, value); }
        }

        public double? RL
        {
            get { return rl; }
            set { SetProperty(ref rl, value); }
        }

        public double? XH
        {
            get { return xh; }
            set { SetProperty(ref xh, value); }
        }

        public double? XM
        {
            get { return xm; }
            set { SetProperty(ref xm, value); }
        }

        public double? XL
        {
            get { return xl; }
            set { SetProperty(ref xl, value); }
        }

        public double? B
        {
            get { return b; }
            set { SetProperty(ref b, value); }
        }

        public double? G
        {
            get { return g; }
            set { SetProperty(ref g, value); }
        }

        public string Source
        {
            get { return source; }
            set { SetProperty(ref source, value); }
        }

        #endregion Properties

        public MultiTrans() { }

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
    /// Вспомогательные методы MainWindow для Трансформаторов
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Расчет коэффициентов трансформации
        /// </summary>
        private void Ktr_Calculations(object sender, TextChangedEventArgs e)
        {
            DotCommaReplacer(sender, e); //Обработка запятых

            var HV = txtUnomHigh_T; var LV = txtUnomLowDouble_T;
            var MHV = txtUnomMid_T; var LHV = txtUnomLow_T;

            if (HV.Text.StartsWith(".")) return;
            if (cmbType_T.Text == "двух.")
            {
                if (!string.IsNullOrEmpty(HV.Text) & !string.IsNullOrEmpty(LV.Text))
                {
                    if (LV.Text.StartsWith(".")) return;
                    txtKH_KML_T.Text = (Convert.ToDouble(LV.Text, CultureInfo.InvariantCulture) / Convert.ToDouble(HV.Text, CultureInfo.InvariantCulture)).ToString();
                }
            }
            if (cmbType_T.Text == "тр./АТ")
            {
                txtKH_KML_T.Text = "1";

                if (!string.IsNullOrEmpty(HV.Text) & !string.IsNullOrEmpty(MHV.Text))
                {
                    if (MHV.Text.StartsWith(".")) return;
                    txtKHM_T.Text = (Convert.ToDouble(MHV.Text, CultureInfo.InvariantCulture) / Convert.ToDouble(HV.Text, CultureInfo.InvariantCulture)).ToString();
                }
                if (!string.IsNullOrEmpty(HV.Text) & !string.IsNullOrEmpty(LHV.Text))
                {
                    if (LHV.Text.StartsWith(".")) return;
                    txtKHL_T.Text = (Convert.ToDouble(LHV.Text, CultureInfo.InvariantCulture) / Convert.ToDouble(HV.Text, CultureInfo.InvariantCulture)).ToString();
                }
            }
        }

        /// <summary>
        /// Выбор стартового узла для Трансформатора
        /// </summary>
        private void TxtStartNode_T_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke((Action)delegate ()
            {
                if (txtStartNode_T.SelectedIndex == txtStartNode_T.Items.Count - 1 | txtStartNode_T.SelectedIndex == -1)
                {
                    if (cmbType_T.Text == "двух.") txtEndHighNode_T.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = Trans });
                    else if (cmbType_T.Text == "тр./АТ") txtEndHighNode_T.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = MultiTrans });
                }

                if (e.AddedItems.Count == 0) return;

                try { var t = (Node)e.AddedItems[0]; }
                catch (Exception) { return; }

                if (txtStartNode_T.SelectedIndex != -1)
                {
                    if (cmbType_T.Text == "двух.")
                    {
                        ObservableCollection<Node> l = new ObservableCollection<Node>();

                        foreach (Node i in txtStartNode_T.ItemsSource)
                        {
                            if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom != ((Node)e.AddedItems[0]).Unom) l.Add(i);
                        }

                        txtEndHighNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l });

                        double unom = track.Nodes.Where(n => n.Number == ((Node)e.AddedItems[0]).Number).Select(n => n.Unom).First();

                        foreach (ListBoxItem i in cmbUnom_T.Items)
                        {
                            if (double.Parse(i.Content.ToString(), CultureInfo.InvariantCulture) == unom)
                            {
                                cmbUnom_T.SelectedItem = i;
                                return;
                            }
                        }
                    }
                    else if (cmbType_T.Text == "тр./АТ")
                    {
                        ObservableCollection<Node> l = new ObservableCollection<Node>();
                        ObservableCollection<Node> l2 = new ObservableCollection<Node>();

                        foreach (Node i in txtStartNode_T.ItemsSource)
                        {
                            if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom == ((Node)e.AddedItems[0]).Unom) l.Add(i);
                            if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom != ((Node)e.AddedItems[0]).Unom) l2.Add(i);
                        }
                        txtEndHighNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l });
                        txtEndMidNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l2 });
                        txtEndLowNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l2 });

                        double unom = track.Nodes.Where(n => n.Number == ((Node)e.AddedItems[0]).Number).Select(n => n.Unom).First();
                        foreach (ListBoxItem i in cmbUnom_T.Items)
                        {
                            if (double.Parse(i.Content.ToString(), CultureInfo.InvariantCulture) == unom)
                            {
                                cmbUnom_T.SelectedItem = i;
                                return;
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Селектор узлов для трёхобмоточных трансов
        /// </summary>
        private void TransEndNodeSelector(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke(delegate ()
            {
                ComboBox tb = (ComboBox)sender;

                if (tb.Text == "" || tb.SelectedIndex == -1)
                {
                    cmbTypeName_T.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = MultiTrans });
                    cmbTypeName_T.DisplayMemberPath = "TypeName";
                }

                if (e.AddedItems.Count == 0) return; //Проверка селектора

                try { var t = (Node)e.AddedItems[0]; }
                catch (Exception) { return; }

                if (tb == txtEndMidNode_T)
                {
                    ObservableCollection<Node> l = new ObservableCollection<Node>();

                    foreach (Node i in txtStartNode_T.ItemsSource)
                    {
                        if (i.Number != ((Node)e.AddedItems[0]).Number &
                            i.Number != ((Node)txtStartNode_T.SelectedItem).Number &
                            i.Unom != ((Node)txtStartNode_T.SelectedItem).Unom &
                            i.Unom != ((Node)e.AddedItems[0]).Unom)
                        {
                            l.Add(i);
                        }
                    }
                    txtEndLowNode_T.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = l });
                    txtEndLowNode_T.DisplayMemberPath = "Number";

                    cmbUnom_T.SelectedItem = cmbUnom_T.SelectedItem;

                    if (MultiTrans.Count != 0)
                    {
                        ObservableCollection<MultiTrans> mt;
                        if (txtEndLowNode_T.SelectedIndex == -1) mt = new ObservableCollection<MultiTrans>(MultiTrans.Where(t => t.UnomM >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomM <= 1.2 * ((Node)e.AddedItems[0]).Unom).ToList());
                        else
                        {
                            mt = new ObservableCollection<MultiTrans>(MultiTrans.Where(t => (t.UnomM >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomM <= 1.2 * ((Node)e.AddedItems[0]).Unom) &
                                                                                            (t.UnomL >= 0.8 * ((Node)txtEndLowNode_T.SelectedItem).Unom & t.UnomL <= 1.2 * ((Node)txtEndLowNode_T.SelectedItem).Unom)).ToList());
                        }
                        cmbTypeName_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = mt });
                        cmbTypeName_T.DisplayMemberPath = "TypeName";
                    }
                }
                else if (tb == txtEndLowNode_T)
                {
                    ObservableCollection<Node> l = new ObservableCollection<Node>();

                    foreach (Node i in txtStartNode_T.ItemsSource)
                    {
                        if (i.Number != ((Node)e.AddedItems[0]).Number &
                            i.Number != ((Node)txtStartNode_T.SelectedItem).Number &
                            i.Unom != ((Node)txtStartNode_T.SelectedItem).Unom &
                            i.Unom != ((Node)e.AddedItems[0]).Unom)
                        {
                            l.Add(i);
                        }
                    }
                    txtEndMidNode_T.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = l });
                    txtEndMidNode_T.DisplayMemberPath = "Number";

                    cmbUnom_T.SelectedItem = cmbUnom_T.SelectedItem;

                    if (MultiTrans.Count != 0)
                    {
                        ObservableCollection<MultiTrans> mt;
                        if (txtEndMidNode_T.Text == "" || txtEndMidNode_T.SelectedIndex == -1) mt = new ObservableCollection<MultiTrans>(MultiTrans.Where(t => t.UnomL >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomL <= 1.2 * ((Node)e.AddedItems[0]).Unom).ToList());
                        else
                        {
                            mt = new ObservableCollection<MultiTrans>(MultiTrans.Where(t => (t.UnomL >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomL <= 1.2 * ((Node)e.AddedItems[0]).Unom) &
                                                                                            (t.UnomM >= 0.8 * ((Node)txtEndMidNode_T.SelectedItem).Unom & t.UnomM <= 1.2 * ((Node)txtEndMidNode_T.SelectedItem).Unom)).ToList());
                        }
                        cmbTypeName_T.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = mt });
                        cmbTypeName_T.DisplayMemberPath = "TypeName";
                    }
                }
            });
        }

        /// <summary>
        /// Селектор узлов по нижней стороне (двух.)
        /// </summary>
        private void TxtEndHighNode_T_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                ComboBox tb = (ComboBox)sender;

                if (cmbType_T.Text == "двух.")
                {
                    cmbUnom_T.SelectedItem = cmbUnom_T.SelectedItem;

                    if (tb.Text == "" || tb.SelectedIndex == -1)
                    {
                        cmbTypeName_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = Trans });
                        cmbTypeName_T.DisplayMemberPath = "TypeName";
                    }

                    if (e.AddedItems.Count == 0) return;

                    if (Trans.Count != 0)
                    {
                        ObservableCollection<Trans> mt = new ObservableCollection<Trans>(Trans.Where(t => t.UnomL >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomL <= 1.2 * ((Node)e.AddedItems[0]).Unom).ToList());
                        cmbTypeName_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = mt });
                        cmbTypeName_T.DisplayMemberPath = "TypeName";
                    }
                }
            });
        }

        /// <summary>
        /// Селектор узлов средней стороны (тр./АТ)
        /// </summary>
        private void TxtEndMidNode_T_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TransEndNodeSelector(sender, e);
        }

        /// <summary>
        /// Селектор узлов низкой стороны (тр./АТ)
        /// </summary>
        private void TxtEndLowNode_T_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TransEndNodeSelector(sender, e);
        }

        /// <summary>
        /// Обработка внешнего вида формы при изменении типа трансформатора
        /// </summary>
        private void CmbType_T_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke(delegate ()
            {
                if ((e.AddedItems.Count != 0) && ((ListBoxItem)e.AddedItems[0] == cmbType_T.Items[0]))
                {
                    cmbType_T.SelectedItem = cmbType_T.Items[0];

                    int state = txtStartNode_T.SelectedIndex;
                    int state_unom = cmbUnom_T.SelectedIndex;

                    txtStartNode_T.SelectedIndex = -1; txtStartNode_T.SelectedIndex = state;
                    cmbUnom_T.SelectedItem = null; cmbUnom_T.SelectedIndex = state_unom;

                    txtKH_KML_T.Clear();
                    grpMid.Visibility = grpLow.Visibility = Visibility.Hidden;
                    lblKH_KML_T.Content = "Kтр ВН";
                    lblEndHighNode_T.Content = "Конец";
                    lblUnomLowDouble_T.Visibility = txtUnomLowDouble_T.Visibility = Visibility.Visible;
                }
                else if ((e.AddedItems.Count != 0) && ((ListBoxItem)e.AddedItems[0] == cmbType_T.Items[1]))
                {
                    cmbType_T.SelectedItem = cmbType_T.Items[1];

                    int state = txtStartNode_T.SelectedIndex;
                    int state_unom = cmbUnom_T.SelectedIndex;

                    txtStartNode_T.SelectedIndex = -1; txtStartNode_T.SelectedIndex = state;
                    cmbUnom_T.SelectedItem = null; cmbUnom_T.SelectedIndex = state_unom;

                    txtKH_KML_T.Text = "1";
                    txtKHL_T.Clear(); txtKHM_T.Clear();
                    grpMid.Visibility = grpLow.Visibility = Visibility.Visible;
                    lblKH_KML_T.Content = "Kтр В(Ктр С-Н)";
                    lblEndHighNode_T.Content = "Ср. точка";
                    lblUnomLowDouble_T.Visibility = txtUnomLowDouble_T.Visibility = Visibility.Hidden;
                }

                if (cmbUnom_T.Text != "") GetData("Trans", Convert.ToDouble(cmbUnom_T.Text, CultureInfo.InvariantCulture), db_prv);
            });
        }

        /// <summary>
        /// Изменение типа трансформатора
        /// </summary>
        private void CmbType_T_DropDownClosed(object sender, EventArgs e)
        {
            if (e != null) if (cmbUnom_T.Text != "") GetData("Trans", Convert.ToDouble(cmbUnom_T.Text, CultureInfo.InvariantCulture), db_prv);
        }

        /// <summary>
        /// Отчистка полей Трансформаторов
        /// </summary>
        private void ClearTrans()
        {
            int state = cmbType_T.SelectedIndex;
            cmbType_T.SelectedItem = null;
            cmbType_T.SelectedIndex = state;
            state = cmbUnom_T.SelectedIndex;
            cmbUnom_T.SelectedIndex = -1;
            cmbUnom_T.SelectedIndex = state;
            cmbTypeName_T.SelectedIndex = -1;
            txtName_T.Text = "";
            txtEndHighNode_T.SelectedIndex = -1; txtEndLowNode_T.SelectedIndex = -1; txtEndMidNode_T.SelectedIndex = -1;
            txtUnomHigh_T.DataContext = ""; txtUnomLowDouble_T.DataContext = ""; txtUnomLow_T.DataContext = ""; txtUnomMid_T.DataContext = "";
            txtUnomHigh_T.Clear(); txtUnomLowDouble_T.Clear(); txtUnomLow_T.Clear(); txtUnomMid_T.Clear();
            txtKHL_T.DataContext = ""; txtKHM_T.DataContext = ""; txtKH_KML_T.DataContext = ""; txtKHL_T.Clear(); txtKHM_T.Clear(); txtKH_KML_T.Clear();
            txtRH_T.DataContext = ""; txtRL_T.DataContext = ""; txtRM_T.DataContext = ""; txtRH_T.Clear(); txtRL_T.Clear(); txtRM_T.Clear();
            txtXH_T.DataContext = ""; txtXL_T.DataContext = ""; txtXM_T.DataContext = ""; txtXH_T.Clear(); txtXL_T.Clear(); txtXM_T.Clear();
            txtBH_T.DataContext = ""; txtGH_T.DataContext = ""; txtBH_T.Clear(); txtGH_T.Clear();
            txtState_T.Text = "0";
            txtRegion_T.Text = "";
            txtN_T.Text = "1";
            txtIddH_T.DataContext = ""; txtIddM_T.DataContext = ""; txtIddL_T.DataContext = "";
            txtIddH_T.Clear(); txtIddM_T.Clear(); txtIddL_T.Clear();
        }



        /// <summary>
        /// Добавить Трансформатор в список ветвей
        /// </summary>
        private void BtnAdd_T_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke((Action)delegate ()
            {
                if (cmbType_T.Text == "двух.")
                {
                    int start = default;
                    int end = default;
                    if (string.IsNullOrEmpty(txtStartNode_T.Text)) { ChangeCmbColor(txtStartNode_T, true); } else { start = int.Parse(txtStartNode_T.Text); }
                    if (string.IsNullOrEmpty(txtEndHighNode_T.Text)) { ChangeCmbColor(txtEndHighNode_T, true); } else { end = int.Parse(txtEndHighNode_T.Text); }
                    if (txtStartNode_T.Text == txtEndHighNode_T.Text) { ChangeCmbColor(txtStartNode_T, true); ChangeCmbColor(txtEndHighNode_T, true); return; }

                    if (start == default) { ChangeCmbColor(txtStartNode_T, true); Log.Show("Ошибка ввода узлов Трансофрматора!"); return; }
                    if (end == default) { ChangeCmbColor(txtStartNode_T, true); Log.Show("Ошибка ввода узлов Трансофрматора!"); return; }

                    int state = (string.IsNullOrWhiteSpace(txtState_T.Text) || int.Parse(txtState_T.Text) == 0) ? 0 : 1;
                    string type = "Тр-р";
                    int npar = 0;
                    string typename = cmbTypeName_T.Text;
                    string name = txtName_T.Text;
                    double r = (string.IsNullOrWhiteSpace(txtRH_T.Text) || double.Parse(txtRH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtRH_T.Text, CultureInfo.InvariantCulture);
                    double x = (string.IsNullOrWhiteSpace(txtXH_T.Text) || double.Parse(txtXH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtXH_T.Text, CultureInfo.InvariantCulture);
                    double b = (string.IsNullOrWhiteSpace(txtBH_T.Text) || double.Parse(txtBH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtBH_T.Text, CultureInfo.InvariantCulture);
                    double g = (string.IsNullOrWhiteSpace(txtGH_T.Text) || double.Parse(txtGH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtGH_T.Text, CultureInfo.InvariantCulture);
                    double ktr = (string.IsNullOrWhiteSpace(txtKH_KML_T.Text) || double.Parse(txtKH_KML_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtKH_KML_T.Text, CultureInfo.InvariantCulture);
                    double idd = (string.IsNullOrWhiteSpace(txtIddH_T.Text) || double.Parse(txtIddH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtIddH_T.Text, CultureInfo.InvariantCulture);
                    int region = (string.IsNullOrWhiteSpace(txtRegion_T.Text) || int.Parse(txtRegion_T.Text) == 0) ? 0 : int.Parse(txtRegion_T.Text);

                    int quant = (string.IsNullOrWhiteSpace(txtN_T.Text) || int.Parse(txtN_T.Text) <= 0) ? 1 : int.Parse(txtN_T.Text); //Количество вводимых ветвей
                    int i = 0;

                    do
                    {
                        Branch br = new Branch(start: start, end: end, type: type,
                                           state: state, typename: typename, name: name, npar: npar + i,
                                           r: r, x: x, b: b, g: g,
                                           r0: r, x0: x, b0: b, g0: g,
                                           ktr: ktr, idd: idd, region: region);
                        if (BranchChecker(br, txtStartNode_T, txtEndHighNode_T) == true) track.AddBranch(br);
                        else return;
                        i++;

                    } while (i < quant);

                    Application.Current.Dispatcher?.BeginInvoke((Action)delegate () { Log.Show($"Добавлен трансформатор:\t{start} - {end}\tТип: двух.\tНаименование: {name}", LogClass.LogType.Success); });

                    Tab_Data.SelectedIndex = 1;

                    #region Clear controls

                    ClearTrans();
                    Log.Clear();

                    #endregion Clear controls
                }
                else if (cmbType_T.Text == "тр./АТ")
                {
                    int start = default;
                    int endH = default;
                    int endM = default;
                    int endL = default;
                    if (string.IsNullOrEmpty(txtStartNode_T.Text)) { ChangeCmbColor(txtStartNode_T, true); } else { start = int.Parse(txtStartNode_T.Text); }
                    if (string.IsNullOrEmpty(txtEndHighNode_T.Text)) { ChangeCmbColor(txtEndHighNode_T, true); } else { endH = int.Parse(txtEndHighNode_T.Text); }
                    if (string.IsNullOrEmpty(txtEndMidNode_T.Text)) { ChangeCmbColor(txtEndMidNode_T, true); } else { endM = int.Parse(txtEndMidNode_T.Text); }
                    if (string.IsNullOrEmpty(txtEndLowNode_T.Text)) { ChangeCmbColor(txtEndLowNode_T, true); } else { endL = int.Parse(txtEndLowNode_T.Text); }

                    if (start == default) { ChangeCmbColor(txtStartNode_T, true); Log.Show("Ошибка ввода узлов Трансофрматора!"); return; }
                    if (endH == default) { ChangeCmbColor(txtEndHighNode_T, true); Log.Show("Ошибка ввода узлов Трансофрматора!"); return; }
                    if (endM == default) { ChangeCmbColor(txtEndMidNode_T, true); Log.Show("Ошибка ввода узлов Трансофрматора!"); return; }
                    if (endL == default) { ChangeCmbColor(txtEndLowNode_T, true); Log.Show("Ошибка ввода узлов Трансофрматора!"); return; }

                    int state = (string.IsNullOrWhiteSpace(txtState_T.Text) || int.Parse(txtState_T.Text) == 0) ? 0 : 1;
                    string type = "Тр-р";
                    int npar = 0;
                    string typename = cmbTypeName_T.Text;
                    string nameH = txtName_T.Text + " ВН"; string nameM = txtName_T.Text + " СН"; string nameL = txtName_T.Text + " НН";
                    //ВН
                    double rH = (string.IsNullOrWhiteSpace(txtRH_T.Text) || double.Parse(txtRH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtRH_T.Text, CultureInfo.InvariantCulture);
                    double xH = (string.IsNullOrWhiteSpace(txtXH_T.Text) || double.Parse(txtXH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtXH_T.Text, CultureInfo.InvariantCulture);
                    double bH = (string.IsNullOrWhiteSpace(txtBH_T.Text) || double.Parse(txtBH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtBH_T.Text, CultureInfo.InvariantCulture);
                    double gH = (string.IsNullOrWhiteSpace(txtGH_T.Text) || double.Parse(txtGH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtGH_T.Text, CultureInfo.InvariantCulture);
                    double ktrH = (string.IsNullOrWhiteSpace(txtKH_KML_T.Text) || double.Parse(txtKH_KML_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtKH_KML_T.Text, CultureInfo.InvariantCulture);
                    double iddH = (string.IsNullOrWhiteSpace(txtIddH_T.Text) || double.Parse(txtIddH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtIddH_T.Text, CultureInfo.InvariantCulture);
                    //СН
                    double rM = (string.IsNullOrWhiteSpace(txtRM_T.Text) || double.Parse(txtRM_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtRM_T.Text, CultureInfo.InvariantCulture);
                    double xM = (string.IsNullOrWhiteSpace(txtXM_T.Text) || double.Parse(txtXM_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtXM_T.Text, CultureInfo.InvariantCulture);
                    double bM = 0; double gM = 0;
                    double ktrM = (string.IsNullOrWhiteSpace(txtKHM_T.Text) || double.Parse(txtKHM_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtKHM_T.Text, CultureInfo.InvariantCulture);
                    double iddM = (string.IsNullOrWhiteSpace(txtIddM_T.Text) || double.Parse(txtIddM_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtIddM_T.Text, CultureInfo.InvariantCulture);
                    //НН
                    double rL = (string.IsNullOrWhiteSpace(txtRL_T.Text) || double.Parse(txtRL_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtRL_T.Text, CultureInfo.InvariantCulture);
                    double xL = (string.IsNullOrWhiteSpace(txtXM_T.Text) || double.Parse(txtXL_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtXL_T.Text, CultureInfo.InvariantCulture);
                    double bL = 0; double gL = 0;
                    double ktrL = (string.IsNullOrWhiteSpace(txtKHL_T.Text) || double.Parse(txtKHL_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtKHL_T.Text, CultureInfo.InvariantCulture);
                    double iddL = (string.IsNullOrWhiteSpace(txtIddL_T.Text) || double.Parse(txtIddL_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtIddL_T.Text, CultureInfo.InvariantCulture);

                    int region = (string.IsNullOrWhiteSpace(txtRegion_T.Text) || int.Parse(txtRegion_T.Text) == 0) ? 0 : int.Parse(txtRegion_T.Text);

                    int quant = (string.IsNullOrWhiteSpace(txtN_T.Text) || int.Parse(txtN_T.Text) <= 0) ? 1 : int.Parse(txtN_T.Text); //Количество вводимых ветвей
                    int i = 0;

                    do
                    {

                        Branch br1 = new Branch(start: start, end: endH, type: type,
                                           state: state, typename: typename, name: nameH, npar: npar,
                                           r: rH, x: xH, b: bH, g: gH,
                                           r0: rH, x0: xH, b0: bH, g0: gH,
                                           ktr: ktrH, idd: iddH, region: region);

                        Branch br2 = new Branch(start: endH, end: endM, type: type,
                                               state: state, typename: typename, name: nameM, npar: npar,
                                               r: rM, x: xM, b: bM, g: gM,
                                               r0: rM, x0: xM, b0: bM, g0: gM,
                                               ktr: ktrM, idd: iddM, region: region);

                        Branch br3 = new Branch(start: endH, end: endL, type: type,
                                               state: state, typename: typename, name: nameL, npar: npar,
                                               r: rL, x: xL, b: bL, g: gL,
                                               r0: rL, x0: xL, b0: bL, g0: gL,
                                               ktr: ktrL, idd: iddL, region: region);

                        bool result1 = BranchChecker(br1, txtStartNode_T, txtEndHighNode_T);
                        bool result2 = BranchChecker(br2, txtEndHighNode_T, txtEndMidNode_T);
                        bool result3 = BranchChecker(br3, txtEndHighNode_T, txtEndLowNode_T);

                        if (result1 == true & result2 == true & result3 == true)
                        {
                            track.AddBranch(br1);
                            track.AddBranch(br2);
                            track.AddBranch(br3);
                        }
                        else return;
                        i++;

                    } while (i < quant);

                    Application.Current.Dispatcher?.BeginInvoke((Action)delegate () { Log.Show($"Добавлен трансформатор:\tВН: {start}, СН: {endM}, НН: {endL}\tТип: трёх./АТ\tНаименование: {nameH}", LogClass.LogType.Success); });

                    Tab_Data.SelectedIndex = 1;

                    #region Clear controls

                    ClearTrans();
                    Log.Clear();

                    #endregion Clear controls

                }
            });
        }
    }
}
