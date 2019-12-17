using Power_Equipment_Handbook.src;
using Power_Equipment_Handbook.src.windows;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace Power_Equipment_Handbook
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private DataGridTracker track;
        private DBProvider db_prv;

        private LogClass Log;

        public ObservableCollection<Line> Lines;
        public ObservableCollection<Trans> Trans;
        public ObservableCollection<MultiTrans> MultiTrans;

        public Library lib;

        public MainWindow()
        {
            InitializeComponent();
            track = new DataGridTracker(grdNodes, grdBranches);

            db_prv = new DBProvider("test.db");                     //Инициализация подключения к встроенной БД оборудования
            Status_Text.Text = "Состояние подключения:   " + db_prv.Status;

            cmbType_T.SelectedIndex = 1; cmbType_T.SelectedIndex = 0;

            txtStartNode_L.ItemsSource = track.Nodes; txtStartNode_L.DisplayMemberPath = "Number";
            txtEndNode_L.ItemsSource = track.Nodes; txtEndNode_L.DisplayMemberPath = "Number";
            txtStartNode_T.ItemsSource = track.Nodes; txtStartNode_T.DisplayMemberPath = "Number";
            txtEndHighNode_T.ItemsSource = track.Nodes; txtEndHighNode_T.DisplayMemberPath = "Number";
            txtEndMidNode_T.ItemsSource = track.Nodes; txtEndMidNode_T.DisplayMemberPath = "Number";
            txtEndLowNode_T.ItemsSource = track.Nodes; txtEndLowNode_T.DisplayMemberPath = "Number";
            txtStartNode_B.ItemsSource = track.Nodes; txtStartNode_B.DisplayMemberPath = "Number";
            txtEndNode_B.ItemsSource = track.Nodes; txtEndNode_B.DisplayMemberPath = "Number";


            Lines = new ObservableCollection<Line>();               //Коллекция объектов Линия
            Trans = new ObservableCollection<Trans>();              //Коллекция объектов Двухобмоточный Трансформатор
            MultiTrans = new ObservableCollection<MultiTrans>();    //Коллекция объектов Трехобмоточный Трансформатор/Автотрансформатор

            Log = new LogClass(txtLog);                             //Инициализация Лога приложения
        }
        
        #region Обработчики конкретных событий

        /// <summary>
        /// Выгрузка параметров из базы по факту выбора Сечения/Марки
        /// </summary>
        private void CmbTypeName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            //Выбор марки провода для линии
            if (cmb.Equals(cmbTypeName_L))
            {
                if (cmb.SelectedItem == null)
                {
                    txtr0_L.Clear(); txtx0_L.Clear(); txtb0_L.Clear(); txtg0_L.Clear(); txtIdd_L.Clear(); lblSource_L.DataContext = String.Empty;
                    return;
                }
                if (Lines.Count == 0) return;

                Line l = Lines.First(n => n.TypeName == ((Line) cmb.SelectedItem).TypeName);

                txtr0_L.DataContext = l; txtx0_L.DataContext = l; txtb0_L.DataContext = l; txtg0_L.DataContext = l;
                txtIdd_L.DataContext = l; lblSource_L.DataContext = l;
            }

            //Выбор марки трансформатора
            if (cmb.Equals(cmbTypeName_T))
            {
                //Двухобмоточные трансформаторы
                if (cmbType_T.Text == "двух.")
                {
                    if (cmb.SelectedItem == null)
                    {
                        txtRH_T.Clear(); txtXH_T.Clear(); txtBH_T.Clear(); txtGH_T.Clear(); txtUnomHigh_T.Clear(); txtUnomLowDouble_T.Clear(); lblSource_T.Content = String.Empty;
                        return;
                    }
                    if (Trans.Count == 0) return;

                    cmbUnom_T.SelectedItem = cmbUnom_T.SelectedItem;

                    Trans t = Trans.First(n => n.TypeName == ((Trans) cmb.SelectedItem).TypeName);

                    txtRH_T.SetBinding(TextBox.TextProperty, new Binding("R") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtXH_T.SetBinding(TextBox.TextProperty, new Binding("X") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtBH_T.SetBinding(TextBox.TextProperty, new Binding("B") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtGH_T.SetBinding(TextBox.TextProperty, new Binding("G") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    txtUnomHigh_T.SetBinding(TextBox.TextProperty, new Binding("UnomH") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtUnomLowDouble_T.SetBinding(TextBox.TextProperty, new Binding("UnomL") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    lblSource_T.Content = t.Source;
                }
                //Трехобмоточные и Автотрансформаторы
                if (cmbType_T.Text == "тр./АТ")
                {
                    if (cmb.SelectedItem == null)
                    {
                        txtRH_T.Clear(); txtRM_T.Clear(); txtRL_T.Clear();
                        txtXH_T.Clear(); txtXM_T.Clear(); txtXL_T.Clear();
                        txtBH_T.Clear(); txtGH_T.Clear();
                        txtUnomHigh_T.Clear(); txtUnomMid_T.Clear(); txtUnomLow_T.Clear();
                        lblSource_T.Content = String.Empty;
                        return;
                    }
                    if (MultiTrans.Count == 0) return;

                    cmbUnom_T.SelectedItem = cmbUnom_T.SelectedItem;

                    MultiTrans t = MultiTrans.First(n => n.TypeName == ((MultiTrans) cmb.SelectedItem).TypeName);

                    txtRH_T.SetBinding(TextBox.TextProperty, new Binding("RH") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtRM_T.SetBinding(TextBox.TextProperty, new Binding("RM") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtRL_T.SetBinding(TextBox.TextProperty, new Binding("RL") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    txtXH_T.SetBinding(TextBox.TextProperty, new Binding("XH") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtXM_T.SetBinding(TextBox.TextProperty, new Binding("XM") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtXL_T.SetBinding(TextBox.TextProperty, new Binding("XL") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    txtBH_T.SetBinding(TextBox.TextProperty, new Binding("B") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtGH_T.SetBinding(TextBox.TextProperty, new Binding("G") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    txtUnomHigh_T.SetBinding(TextBox.TextProperty, new Binding("UnomH") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtUnomMid_T.SetBinding(TextBox.TextProperty, new Binding("UnomM") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtUnomLow_T.SetBinding(TextBox.TextProperty, new Binding("UnomL") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    lblSource_T.Content = t.Source;
                }
            }
        }

        /// <summary>
        /// Обработка внешнего вида формы при изменении типа трансформатора
        /// </summary>
        private void CmbType_T_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbType_T.SelectedValue == cmbType_T.Items[0])
            {
                grpMid.Visibility = grpLow.Visibility = Visibility.Hidden;
                lblKH_KML_T.Content = "Kтр ВН";
                lblEndHighNode_T.Content = "Конец";
                elUnom_T_2.Visibility = lblUnomLowDouble_T.Visibility = txtUnomLowDouble_T.Visibility = Visibility.Visible;
                elUnom_T_1.Width = 6;
            }
            else if (cmbType_T.SelectedValue == cmbType_T.Items[1])
            {
                txtKH_KML_T.Text = "1";
                grpMid.Visibility = grpLow.Visibility = Visibility.Visible;
                lblKH_KML_T.Content = "Kтр В(Ктр С-Н)";
                lblEndHighNode_T.Content = "Ср. точка";
                elUnom_T_2.Visibility = lblUnomLowDouble_T.Visibility = txtUnomLowDouble_T.Visibility = Visibility.Hidden;
                elUnom_T_1.Width = 65;
            }
        }

        /// <summary>
        /// Изменение типа трансформатора
        /// </summary>
        private void CmbType_T_DropDownClosed(object sender, EventArgs e)
        {
            int state = txtStartNode_T.SelectedIndex;
            txtStartNode_T.SelectedIndex = -1; txtStartNode_T.SelectedIndex = state;
            if(cmbUnom_T.Text != "") GetData("Trans", Convert.ToDouble(cmbUnom_T.Text, CultureInfo.InvariantCulture), db_prv);
        }

        /// <summary>
        /// Добавить узел в список узлов
        /// </summary>
        private void BtnAdd_N_Click(object sender, RoutedEventArgs e)
        {
            int number = default;
            Application.Current.Dispatcher?.Invoke((Action)delegate ()
            {
                if(cmbUnom_N.Text =="" || cmbUnom_N.SelectedIndex == -1) { Log.Show("Класс напряжения не выбран!"); return; }

                if (txtNumber_N.Text == "") { ChangeTxtColor(txtNumber_N, true); Log.Show("Введите номер узла!"); return; } else { number = int.Parse(txtNumber_N.Text); }
                int state = (string.IsNullOrWhiteSpace(txtState_L.Text) || int.Parse(txtState_L.Text) == 0) ? 0 : 1;
                string type = txtType_N.Text;
                double unom = (string.IsNullOrWhiteSpace(cmbUnom_N.Text) || double.Parse(cmbUnom_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(cmbUnom_N.Text, CultureInfo.InvariantCulture);
                string name = txtName_N.Text;
                double p_n = (string.IsNullOrWhiteSpace(txtPn_N.Text) || double.Parse(txtPn_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtPn_N.Text, CultureInfo.InvariantCulture);
                double q_n = (string.IsNullOrWhiteSpace(txtQn_N.Text) || double.Parse(txtQn_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtQn_N.Text, CultureInfo.InvariantCulture);
                double p_g = (string.IsNullOrWhiteSpace(txtPg_N.Text) || double.Parse(txtPg_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtPg_N.Text, CultureInfo.InvariantCulture);
                double q_g = (string.IsNullOrWhiteSpace(txtQg_N.Text) || double.Parse(txtQg_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtQg_N.Text, CultureInfo.InvariantCulture);
                double vzd = (string.IsNullOrWhiteSpace(txtVzd_N.Text) || double.Parse(txtVzd_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtVzd_N.Text, CultureInfo.InvariantCulture);
                double q_min = (string.IsNullOrWhiteSpace(txtQmin_N.Text) || double.Parse(txtQmin_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtQmin_N.Text, CultureInfo.InvariantCulture);
                double q_max = (string.IsNullOrWhiteSpace(txtQmax_N.Text) || double.Parse(txtQmax_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtQmax_N.Text, CultureInfo.InvariantCulture);
                double b_sh = (string.IsNullOrWhiteSpace(txtBsh_N.Text) || double.Parse(txtBsh_N.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtBsh_N.Text, CultureInfo.InvariantCulture);
                int region = (string.IsNullOrWhiteSpace(txtRegion_N.Text) || int.Parse(txtRegion_N.Text) == 0) ? 0 : int.Parse(txtRegion_N.Text);

                if (track.Nodes.Where((n) => n.Number == number).ToList().Count != 0)
                {
                    ChangeTxtColor(txtNumber_N, true);
                    Log.Show("Узел с заданными параметрами уже существует!");
                    return;
                }

                track.AddNode(new Node(number: number, unom: unom, type: type,
                                       state: state, name: name,
                                       p_n: p_n, q_n: q_n, p_g: p_g, q_g: q_g,
                                       vzd: vzd, q_min: q_min, q_max: q_max, b_sh: b_sh,
                                       region: region));

                Tab_Data.SelectedIndex = 0;

                #region Clear controls

                txtType_N.SelectedIndex = 0;
                txtName_N.Clear();
                txtPn_N.Clear(); txtQn_N.Clear(); txtPg_N.Clear(); txtQg_N.Clear();
                txtQmin_N.Clear(); txtQmax_N.Clear(); txtVzd_N.Clear(); txtBsh_N.Clear();
                txtRegion_N.Clear();
                txtState_N.Text = "0";

                Log.Clear();

                #endregion Clear controls
            });
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

                int state = (string.IsNullOrWhiteSpace(txtState_L.Text) || int.Parse(txtState_L.Text) == 0) ? 0 : 1;
                string type = "ЛЭП";
                int npar = (string.IsNullOrWhiteSpace(txtNpar_L.Text) || int.Parse(txtNpar_L.Text) == 0) ? 0 : int.Parse(txtNpar_L.Text);
                string typename = cmbTypeName_L.Text;
                string name = txtName_L.Text;
                double r = (string.IsNullOrWhiteSpace(txtR_L.Text) || double.Parse(txtR_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtR_L.Text, CultureInfo.InvariantCulture);
                double x = (string.IsNullOrWhiteSpace(txtX_L.Text) || double.Parse(txtX_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtX_L.Text, CultureInfo.InvariantCulture);
                double b = (string.IsNullOrWhiteSpace(txtB_L.Text) || double.Parse(txtB_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtB_L.Text, CultureInfo.InvariantCulture);
                double g = (string.IsNullOrWhiteSpace(txtG_L.Text) || double.Parse(txtG_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtG_L.Text, CultureInfo.InvariantCulture);
                double idd = (string.IsNullOrWhiteSpace(txtIdd_L.Text) || double.Parse(txtIdd_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtIdd_L.Text, CultureInfo.InvariantCulture);
                int region = (string.IsNullOrWhiteSpace(txtRegion_L.Text) || int.Parse(txtRegion_L.Text) == 0) ? 0 : int.Parse(txtRegion_L.Text);

                int quant = (string.IsNullOrWhiteSpace(txtN_L.Text) || int.Parse(txtN_L.Text) <= 0) ? 1 : int.Parse(txtN_L.Text); //Количество вводимых ветвей
                int i = 0;

                do
                {
                    Branch br = new Branch(start: start, end: end, type: type,
                                        state: state, typename: typename, name: name, npar: npar,
                                        r: r, x: x, b: b, g: g,
                                        ktr: null, idd: idd, region: region);
                    if(BranchChecker(br, txtStartNode_L, txtEndNode_L) == true) track.AddBranch(br);
                    else return;
                    i++;

                } while(i < quant);
                
                Tab_Data.SelectedIndex = 1;

                #region Clear controls

                txtEndNode_L.SelectedIndex = -1;
                cmbTypeName_L.SelectedIndex = -1;
                txtName_L.Clear(); txtLength_L.Clear();
                txtr0_L.DataContext = ""; txtx0_L.DataContext = ""; txtb0_L.DataContext = ""; txtg0_L.DataContext = "";
                txtR_L.Clear(); txtX_L.Clear(); txtB_L.Clear(); txtG_L.Clear();
                txtNpar_L.Clear(); txtIdd_L.DataContext = "";
                txtRegion_L.Clear();
                txtState_L.Text = "0";
                txtN_L.Text = "1";

                Log.Clear();

                #endregion Clear controls
            });
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
                    double idd = 0;
                    int region = (string.IsNullOrWhiteSpace(txtRegion_T.Text) || int.Parse(txtRegion_T.Text) == 0) ? 0 : int.Parse(txtRegion_T.Text);

                    int quant = (string.IsNullOrWhiteSpace(txtN_T.Text) || int.Parse(txtN_T.Text) <= 0) ? 1 : int.Parse(txtN_T.Text); //Количество вводимых ветвей
                    int i = 0;

                    do
                    {
                        Branch br = new Branch(start: start, end: end, type: type,
                                           state: state, typename: typename, name: name, npar: npar,
                                           r: r, x: x, b: b, g: g,
                                           ktr: ktr, idd: idd, region: region);
                        if(BranchChecker(br, txtStartNode_T, txtEndHighNode_T) == true) track.AddBranch(br);
                        else return;
                        i++;

                    } while(i < quant);                    

                    Tab_Data.SelectedIndex = 1;

                    #region Clear controls

                    txtEndHighNode_T.SelectedIndex = -1;
                    cmbTypeName_T.SelectedIndex = -1;
                    txtName_T.Clear();
                    txtUnomHigh_T.DataContext = ""; txtUnomLowDouble_T.DataContext = "";
                    txtKH_KML_T.Clear();
                    txtRH_T.DataContext = ""; txtXH_T.DataContext = ""; txtGH_T.DataContext = ""; txtBH_T.DataContext = "";
                    txtRegion_T.Clear();
                    txtState_T.Text = "0";
                    txtN_T.Text = "1";

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
                    //СН
                    double rM = (string.IsNullOrWhiteSpace(txtRM_T.Text) || double.Parse(txtRM_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtRM_T.Text, CultureInfo.InvariantCulture);
                    double xM = (string.IsNullOrWhiteSpace(txtXM_T.Text) || double.Parse(txtXM_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtXM_T.Text, CultureInfo.InvariantCulture);
                    double bM = 0; double gM = 0;
                    double ktrM = (string.IsNullOrWhiteSpace(txtKHM_T.Text) || double.Parse(txtKHM_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtKHM_T.Text, CultureInfo.InvariantCulture);
                    //НН
                    double rL = (string.IsNullOrWhiteSpace(txtRL_T.Text) || double.Parse(txtRL_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtRL_T.Text, CultureInfo.InvariantCulture);
                    double xL = (string.IsNullOrWhiteSpace(txtXM_T.Text) || double.Parse(txtXL_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtXL_T.Text, CultureInfo.InvariantCulture);
                    double bL = 0; double gL = 0;
                    double ktrL = (string.IsNullOrWhiteSpace(txtKHL_T.Text) || double.Parse(txtKHL_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtKHL_T.Text, CultureInfo.InvariantCulture);

                    double idd = 0;
                    int region = (string.IsNullOrWhiteSpace(txtRegion_T.Text) || int.Parse(txtRegion_T.Text) == 0) ? 0 : int.Parse(txtRegion_T.Text);

                    int quant = (string.IsNullOrWhiteSpace(txtN_T.Text) || int.Parse(txtN_T.Text) <= 0) ? 1 : int.Parse(txtN_T.Text); //Количество вводимых ветвей
                    int i = 0;

                    do
                    {

                        Branch br1 = new Branch(start: start, end: endH, type: type,
                                           state: state, typename: typename, name: nameH, npar: npar,
                                           r: rH, x: xH, b: bH, g: gH,
                                           ktr: ktrH, idd: idd, region: region);

                        Branch br2 = new Branch(start: endH, end: endM, type: type,
                                               state: state, typename: typename, name: nameM, npar: npar,
                                               r: rM, x: xM, b: bM, g: gM,
                                               ktr: ktrM, idd: idd, region: region);

                        Branch br3 = new Branch(start: endH, end: endL, type: type,
                                               state: state, typename: typename, name: nameL, npar: npar,
                                               r: rL, x: xL, b: bL, g: gL,
                                               ktr: ktrL, idd: idd, region: region);

                        bool result1 = BranchChecker(br1, txtStartNode_T, txtEndHighNode_T);
                        bool result2 = BranchChecker(br2, txtEndHighNode_T, txtEndMidNode_T);
                        bool result3 = BranchChecker(br3, txtEndHighNode_T, txtEndLowNode_T);

                        if(result1 == true & result2 == true & result3 == true)
                        {
                            track.AddBranch(br1);
                            track.AddBranch(br2);
                            track.AddBranch(br3);
                        }
                        else return;
                        i++;

                    } while(i < quant);

                    Tab_Data.SelectedIndex = 1;

                    #region Clear controls

                    txtEndHighNode_T.SelectedIndex = -1; txtEndMidNode_T.SelectedIndex = -1; txtEndLowNode_T.SelectedIndex = -1;
                    cmbTypeName_T.SelectedIndex = -1;
                    txtName_T.Clear();
                    txtUnomHigh_T.DataContext = ""; txtUnomLowDouble_T.DataContext = ""; txtUnomMid_T.DataContext = ""; txtUnomLow_T.DataContext = "";
                    txtKH_KML_T.DataContext = ""; txtKHM_T.DataContext = ""; txtKHL_T.DataContext = "";
                    txtRH_T.DataContext = ""; txtXH_T.DataContext = ""; txtGH_T.DataContext = ""; txtBH_T.DataContext = "";
                    txtRM_T.DataContext = ""; txtXM_T.DataContext = ""; txtRL_T.DataContext = ""; txtXL_T.DataContext = "";
                    txtRegion_T.Clear();
                    txtState_T.Text = "0";
                    txtN_T.Text = "1";

                    Log.Clear();

                    #endregion Clear controls

                }
            });
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
                if(string.IsNullOrEmpty(txtStartNode_B.Text)) { ChangeCmbColor(txtStartNode_B, true); } else { start = int.Parse(txtStartNode_B.Text); }
                if(string.IsNullOrEmpty(txtEndNode_B.Text)) { ChangeCmbColor(txtEndNode_B, true); Log.Show("Ошибка ввода узлов Выключателя!"); return; } else { end = int.Parse(txtEndNode_B.Text); }
                if(txtStartNode_B.Text == txtEndNode_B.Text) { ChangeCmbColor(txtStartNode_B, true); ChangeCmbColor(txtEndNode_B, true); Log.Show("Узлы начала и конца совпали!"); return; }
                else { ChangeCmbColor(txtStartNode_B, false); ChangeCmbColor(txtEndNode_B, false); }

                if(start == default) { ChangeCmbColor(txtStartNode_B, true); return; }
                if(end == default) { ChangeCmbColor(txtEndNode_B, true); return; }

                int state = (string.IsNullOrWhiteSpace(txtState_B.Text) || int.Parse(txtState_B.Text) == 0) ? 0 : 1;
                string type = "Выкл.";
                string name = txtName_B.Text;
                int region = (string.IsNullOrWhiteSpace(txtRegion_B.Text) || int.Parse(txtRegion_B.Text) == 0) ? 0 : int.Parse(txtRegion_B.Text);

                Branch br = new Branch(start: start, end: end, type: type,
                                           state: state, name: name,
                                           ktr: null, region: region);

                if(BranchChecker(br, txtStartNode_L, txtEndNode_L) == true) track.AddBranch(br);
                else return;

                Tab_Data.SelectedIndex = 1;

                #region Clear controls

                txtEndNode_B.SelectedIndex = -1;
                txtName_B.Clear();
                txtRegion_B.Clear();

                Log.Clear();

                #endregion Clear controls
            });
        }

        /// <summary>
        /// Выгрузка дынных из базы по факту выбора номинального напряжения
        /// </summary>
        private void CmbUnom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            if (cmb.Equals(cmbUnom_L))
            {
                if (cmb.SelectedItem == null)
                {
                    Lines.Clear();
                    return;
                }
                if(((ListBoxItem) cmb.SelectedItem).Content.ToString() == "0.4") GetData("Line", 0.4, db_prv);
                else if (((ListBoxItem) cmb.SelectedItem).Content.ToString() == "27.5") GetData("Line", 27.5, db_prv);
                else { GetData("Line", Convert.ToInt32(((ListBoxItem) cmbUnom_L.SelectedItem).Content.ToString()), db_prv); }
                
            }
            if (cmb.Equals(cmbUnom_T))
            {
                if (cmb.SelectedItem == null)
                {
                    Trans.Clear();
                    MultiTrans.Clear();
                    return;
                }
                if(((ListBoxItem) cmb.SelectedItem).Content.ToString() == "0.4") GetData("Trans", 0.4, db_prv);
                else if(((ListBoxItem) cmb.SelectedItem).Content.ToString() == "27.5") GetData("Trans", 27.5, db_prv);
                else GetData("Trans", double.Parse(((ListBoxItem) cmb.SelectedItem).Content.ToString()), db_prv);
            }
        }

        /// <summary>
        /// Выбор стартового узла для Линии
        /// </summary>
        private void TxtStartNode_L_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.BeginInvoke((Action)delegate ()
            {
                if (txtStartNode_L.SelectedIndex == txtStartNode_L.Items.Count - 1 | txtStartNode_L.SelectedIndex == -1)
                {
                    txtEndNode_L.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = Lines });
                    txtEndNode_L.DisplayMemberPath = "Number";
                }

                if(e.AddedItems.Count == 0) return;

                try { var t = (Node)e.AddedItems[0]; }
                catch(Exception) { return; }

                if (txtStartNode_L.SelectedIndex != -1)
                {
                    ObservableCollection<Node> l = new ObservableCollection<Node>();

                    foreach (Node i in txtStartNode_L.ItemsSource)
                    {
                        if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom == ((Node)e.AddedItems[0]).Unom)
                        {
                            l.Add(i);
                        }
                    }
                    txtEndNode_L.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = l });
                    txtEndNode_L.DisplayMemberPath = "Number";

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
        /// Выбор стартового узла для Трансформатора
        /// </summary>
        private void TxtStartNode_T_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke((Action)delegate ()
            {
                if(txtStartNode_T.SelectedIndex == txtStartNode_T.Items.Count - 1 | txtStartNode_T.SelectedIndex == -1)
                {
                    if(cmbType_T.Text == "двух.")
                    {
                        txtEndHighNode_T.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = Trans });
                        txtEndHighNode_T.DisplayMemberPath = "Number";
                    }
                    else if(cmbType_T.Text == "тр./АТ")
                    {
                        txtEndHighNode_T.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = MultiTrans });
                        txtEndHighNode_T.DisplayMemberPath = "Number";
                    }
                }

                if(e.AddedItems.Count == 0) return;

                try { var t = (Node)e.AddedItems[0]; }
                catch(Exception) { return; }

                if (txtStartNode_T.SelectedIndex != -1)
                {
                    if (cmbType_T.Text == "двух.")
                    {
                        ObservableCollection<Node> l = new ObservableCollection<Node>();

                        foreach (Node i in txtStartNode_T.ItemsSource)
                        {
                            if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom != ((Node)e.AddedItems[0]).Unom)
                            {
                                l.Add(i);
                            }
                        }
                        txtEndHighNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l });
                        txtEndHighNode_T.DisplayMemberPath = "Number";

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
                            if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom == ((Node)e.AddedItems[0]).Unom)
                            {
                                l.Add(i);
                            }
                            if (i.Number != ((Node)e.AddedItems[0]).Number & i.Unom != ((Node)e.AddedItems[0]).Unom)
                            {
                                l2.Add(i);
                            }
                        }
                        txtEndHighNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l });
                        txtEndHighNode_T.DisplayMemberPath = "Number";
                        txtEndMidNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l2 });
                        txtEndMidNode_T.DisplayMemberPath = "Number";
                        txtEndLowNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l2 });
                        txtEndLowNode_T.DisplayMemberPath = "Number";

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

                    if(e.AddedItems.Count == 0) return;

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
        /// Выбор стартового узла для Выключателя
        /// </summary>
        private void TxtStartNode_B_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                if(txtStartNode_B.SelectedIndex == txtStartNode_B.Items.Count - 1 | txtStartNode_B.SelectedIndex == -1)
                {
                    txtEndNode_B.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = track.Nodes });
                    txtEndNode_B.DisplayMemberPath = "Number";
                }

                if(e.AddedItems.Count == 0) return;

                try { var t = (Node)e.AddedItems[0]; }
                catch(Exception) { return; }

                if(txtStartNode_B.SelectedIndex != -1)
                {
                    ObservableCollection<Node> l = new ObservableCollection<Node>();

                    foreach(Node i in txtStartNode_B.ItemsSource)
                    {
                        if(i.Number != ((Node)e.AddedItems[0]).Number & i.Unom == ((Node)e.AddedItems[0]).Unom)
                        {
                            l.Add(i);
                        }
                    }
                    txtEndNode_B.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l });
                    txtEndNode_B.DisplayMemberPath = "Number";

                    double unom = track.Nodes.Where(n => n.Number == ((Node)e.AddedItems[0]).Number).Select(n => n.Unom).First();
                    foreach(ListBoxItem i in cmbUnom_B.Items)
                    {
                        if(double.Parse(i.Content.ToString(), CultureInfo.InvariantCulture) == unom)
                        {
                            cmbUnom_B.SelectedItem = i;
                            return;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Отлов горячих клавиш
        /// </summary>
        private void Tab_Elements_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key == Key.N)) //Нажатие сочетания Ctrl+N
            {
                int state = 0;
                switch(Tab_Elements.SelectedIndex)
                {
                    case 0: //Узлы
                        txtType_N.SelectedIndex = 0;
                        txtName_N.Text = "";
                        txtPn_N.Text = "";
                        txtQn_N.Text = "";
                        txtPg_N.Text = "";
                        txtQg_N.Text = "";
                        txtVzd_N.Text = "";
                        txtQmin_N.Text = "";
                        txtQmax_N.Text = "";
                        txtBsh_N.Text = "";
                        txtRegion_N.Text = "";
                        txtState_N.Text = "0";
                        break;
                    case 1: //Линии
                        txtEndNode_L.SelectedIndex = -1;
                        state = cmbUnom_L.SelectedIndex;
                        cmbUnom_L.SelectedIndex = -1;
                        cmbUnom_L.SelectedIndex = state;
                        cmbTypeName_L.SelectedIndex = -1;
                        txtName_L.Text = "";
                        txtLength_L.Text = "";
                        txtr0_L.DataContext = ""; ; txtR_L.Text = ""; ;
                        txtx0_L.DataContext = ""; ; txtX_L.Text = ""; ;
                        txtg0_L.DataContext = ""; ; txtG_L.Text = ""; ;
                        txtb0_L.DataContext = ""; ; txtB_L.Text = "";
                        txtNpar_L.Text = ""; ; txtIdd_L.DataContext = "";
                        txtState_L.Text = "0";
                        txtRegion_L.Text = "";
                        txtN_L.Text = "1";
                        break;
                    case 2: //Трансформаторы
                        state = cmbType_T.SelectedIndex;
                        cmbType_T.SelectedIndex = -1;
                        cmbType_T.SelectedIndex = state;
                        state = cmbUnom_T.SelectedIndex;
                        cmbUnom_T.SelectedIndex = -1;
                        cmbUnom_T.SelectedIndex = state;
                        cmbTypeName_T.SelectedIndex = -1;
                        txtName_T.Text = "";
                        txtEndHighNode_T.SelectedIndex = -1; txtEndLowNode_T.SelectedIndex = -1; txtEndMidNode_T.SelectedIndex = -1;
                        txtUnomHigh_T.DataContext = ""; txtUnomLowDouble_T.DataContext = ""; txtUnomLow_T.DataContext = ""; txtUnomMid_T.DataContext = "";
                        txtKHL_T.DataContext = ""; txtKHM_T.DataContext = ""; txtKH_KML_T.DataContext = "";
                        txtRH_T.DataContext = ""; txtRL_T.DataContext = ""; txtRM_T.DataContext = "";
                        txtXH_T.DataContext = ""; txtXL_T.DataContext = ""; txtXM_T.DataContext = "";
                        txtBH_T.DataContext = ""; txtGH_T.DataContext = "";
                        txtState_T.Text = "0";
                        txtRegion_T.Text = "";
                        txtN_T.Text = "1";
                        break;
                    case 3: //Выключатели
                        state = cmbUnom_B.SelectedIndex;
                        cmbUnom_B.SelectedIndex = -1;
                        cmbUnom_B.SelectedIndex = state;
                        txtEndNode_B.SelectedIndex = -1;
                        txtName_B.Text = "";
                        txtState_B.Text = "0";
                        txtRegion_B.Text = "";
                        break;
                }
            }
            else if(e.Key == Key.Enter) // Нажатие Enter
            {
                switch(Tab_Elements.SelectedIndex)
                {
                    case 0: //Узлы
                        BtnAdd_N_Click(btnAdd_N, new RoutedEventArgs());
                        break;
                    case 1: //Линии
                        BtnAdd_L_Click(btnAdd_L, new RoutedEventArgs());
                        break;
                    case 2: //Трансформаторы
                        BtnAdd_T_Click(btnAdd_T, new RoutedEventArgs());
                        break;
                    case 3: //Выключатели
                        BtnAdd_B_Click(btnAdd_B, new RoutedEventArgs());
                        break;
                }
            }
        }

        #endregion Обработчики конкретных событий


    }
}