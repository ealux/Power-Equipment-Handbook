using Power_Equipment_Handbook.src;
using Power_Equipment_Handbook.src.windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Power_Equipment_Handbook
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //private readonly string MainTitle = "Power Equipment Handbook";

        private List<DataGridTracker> tracks;   //Список сетей !!!ДОДЕЛАТЬ
        private DataGridTracker track;
        private DBProvider db_prv;

        private readonly LogClass Log;

        public ObservableCollection<Line> Lines;
        public ObservableCollection<Trans> Trans;
        public ObservableCollection<MultiTrans> MultiTrans;

        public Library lib;
        public LinkedNodes lnodes;

        public bool isCable { get; set; }
        private GridLength SideWidth { get; set; }

        public CellElementsOptions ComplectOptions; //Настройки комплекта при добавлении оборудования


        /// <summary>
        /// Конструктор главного окна 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            track = new DataGridTracker(grdNodes, grdBranches, grdCells);
            tracks = new List<DataGridTracker>();   //Список сетей !!!ДОДЕЛАТЬ

            db_prv = new DBProvider("test.db");                     //Инициализация подключения к встроенной БД оборудования
            Status_Text.Text = "Состояние подключения:   " + db_prv.Status;

            cmbType_T.SelectedIndex = 1; cmbType_T.SelectedIndex = 0;

            txtStartNode_L.ItemsSource = track.Nodes; 
            txtEndNode_L.ItemsSource = track.Nodes;
            txtStartNode_T.ItemsSource = track.Nodes;
            txtEndHighNode_T.ItemsSource = track.Nodes;
            txtEndMidNode_T.ItemsSource = track.Nodes;
            txtEndLowNode_T.ItemsSource = track.Nodes;
            txtStartNode_B.ItemsSource = track.Nodes;
            txtEndNode_B.ItemsSource = track.Nodes;
            txtStartNode_C.ItemsSource = track.Nodes;

            chk_Line_Cable.DataContext = this;

            Lines = new ObservableCollection<Line>();               //Коллекция объектов Линия
            Trans = new ObservableCollection<Trans>();              //Коллекция объектов Двухобмоточный Трансформатор
            MultiTrans = new ObservableCollection<MultiTrans>();    //Коллекция объектов Трехобмоточный Трансформатор/Автотрансформатор

            Log = new LogClass(txtLog);                             //Инициализация Лога приложения

            isCable = false;

            ComplectOptions = new CellElementsOptions();            //Инициализация опций комплекта при добавлении оборудования
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
                    txtr0_L.Clear(); txtx0_L.Clear(); txtb0_L.Clear(); txtg0_L.Clear(); txtIdd_L.Clear();
                    return;
                }
                if (Lines.Count == 0) return;

                Line l = Lines.First(n => n.TypeName == ((Line) cmb.SelectedItem).TypeName);

                txtr0_L.DataContext = l; txtx0_L.DataContext = l; txtb0_L.DataContext = l; txtg0_L.DataContext = l;
                txtIdd_L.DataContext = l;
            }

            //Выбор марки трансформатора
            if (cmb.Equals(cmbTypeName_T))
            {
                //Двухобмоточные трансформаторы
                if (cmbType_T.Text == "двух.")
                {
                    if (cmb.SelectedItem == null)
                    {
                        txtRH_T.Clear(); txtXH_T.Clear(); txtBH_T.Clear(); txtGH_T.Clear(); txtUnomHigh_T.Clear(); txtUnomLowDouble_T.Clear();
                        txtIddH_T.Clear();
                        return;
                    }
                    if (Trans.Count == 0) return;

                    cmbUnom_T.SelectedItem = cmbUnom_T.SelectedItem;

                    Trans t = Trans.First(n => n.TypeName == ((Trans) cmb.SelectedItem).TypeName);

                    txtRH_T.SetBinding(TextBox.TextProperty, new Binding("R") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtXH_T.SetBinding(TextBox.TextProperty, new Binding("X") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtBH_T.SetBinding(TextBox.TextProperty, new Binding("B") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtGH_T.SetBinding(TextBox.TextProperty, new Binding("G") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtIddH_T.SetBinding(TextBox.TextProperty, new Binding("Inom") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    txtUnomHigh_T.SetBinding(TextBox.TextProperty, new Binding("UnomH") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtUnomLowDouble_T.SetBinding(TextBox.TextProperty, new Binding("UnomL") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
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
                        txtIddH_T.Clear(); txtIddM_T.Clear(); txtIddL_T.Clear();
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

                    txtIddH_T.SetBinding(TextBox.TextProperty, new Binding("Inomh") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtIddM_T.SetBinding(TextBox.TextProperty, new Binding("Inomm") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtIddL_T.SetBinding(TextBox.TextProperty, new Binding("Inoml") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                }
            }
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

                Application.Current.Dispatcher?.BeginInvoke((Action)delegate () { Log.Show($"Добавлен узел:\tНомер: {number}\t Unom: {unom}кВ\tНаименование: {name}", LogClass.LogType.Success); });

                #region Clear controls

                ClearNodes();
                Log.Clear();

                #endregion Clear controls
            });
        }

        /// <summary>
        /// Выгрузка дынных из базы по факту выбора номинального напряжения
        /// </summary>
        private void CmbUnom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke((Action)delegate ()
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
                    if (((ListBoxItem)cmb.SelectedItem).Content.ToString() == "0.4") GetData("Trans", 0.4, db_prv);
                    else if (((ListBoxItem)cmb.SelectedItem).Content.ToString() == "27.5") GetData("Trans", 27.5, db_prv);
                    else GetData("Trans", double.Parse(((ListBoxItem)cmb.SelectedItem).Content.ToString()), db_prv);
                }
            });
        }

        #region Отлов горячих клавиш

        /// <summary>
        /// Отчистка (Ctrl+N) + Ввод данных (Enter)
        /// </summary>
        private void Tab_Elements_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key == Key.N)) //Нажатие сочетания Ctrl+N - отчстка полей ввода
            {
                switch(Tab_Elements.SelectedIndex)
                {
                    case 0: //Узлы
                        ClearNodes();
                        break;
                    case 1: //Линии
                        ClearLines();
                        break;
                    case 2: //Трансформаторы
                        ClearTrans();
                        break;
                    case 3: //Выключатели
                        ClearBreakers();
                        break;
                }
            }
            else if(e.Key == Key.Enter) // Нажатие Enter - Ввод
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

        /// <summary>
        /// Блокировка таблиц данных (Ctrl+B)
        /// </summary>
        private void BlockHotKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key == Key.B)) //Нажатие сочетания Ctrl+B - Блокировка таблиц данных
            {
                LockUnlockDataTable_Click(btnBlockTables, null);
            }
        }

        #endregion Отлов горячих клавиш

        #endregion Обработчики конкретных событий


        #region Работа с дизайном окна

        /// <summary>
        /// Показать/Скрыть панель ввода данных
        /// </summary>
        private void ShowHideInput_Click(object sender, RoutedEventArgs e)
        {
            if (Tab_Elements.Visibility == Visibility.Collapsed)
            { 
                Tab_Elements.Visibility = Visibility.Visible;
                btnHideInputs.Content = "^        ^        ^";
            }
            else
            {
                Tab_Elements.Visibility = Visibility.Collapsed;
                btnHideInputs.Content = "v        v        v";
            }
        }

        /// <summary>
        /// Блокировка таблиц данных
        /// </summary>
        private void LockUnlockDataTable_Click(object sender, RoutedEventArgs e)
        {
            if(grdNodes.IsReadOnly == true)
            {
                grdNodes.IsReadOnly = false;
                grdBranches.IsReadOnly = false;
                grdCells.IsReadOnly = false;
                imgBlock.Source = new BitmapImage(new Uri("pack://application:,,,/../src/res/unlock.png"));
            }
            else
            {
                grdNodes.IsReadOnly = true;
                grdBranches.IsReadOnly = true;
                grdCells.IsReadOnly = true;
                imgBlock.Source = new BitmapImage(new Uri("pack://application:,,,/../src/res/lock.png"));
            }
        }

        /// <summary>
        /// Сокрытие/Открытие сайдбара с двойного щелчка
        /// </summary>
        private void GridSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(Tab_Side.Visibility != Visibility.Collapsed)
            {
                this.SideWidth = this.gridGlobal.ColumnDefinitions[0].Width;
                this.gridGlobal.ColumnDefinitions[0].MinWidth = 0;
                this.gridGlobal.ColumnDefinitions[0].MaxWidth = 0;
                this.spliterGridGlobal.Cursor = Cursors.ScrollE;
                this.gridGlobal.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                Tab_Side.Visibility = Visibility.Collapsed;
                this.btnCollapse.Content = ">";
            }
            else
            {
                this.gridGlobal.ColumnDefinitions[0].MinWidth = 150;
                this.gridGlobal.ColumnDefinitions[0].MaxWidth = Single.MaxValue;
                this.gridGlobal.ColumnDefinitions[0].Width = this.SideWidth;
                this.spliterGridGlobal.Cursor = Cursors.SizeWE;
                Tab_Side.Visibility = Visibility.Visible;
                this.btnCollapse.Content = "<";
            }
        }

        /// <summary>
        /// Сокрытие/Открытие сайдбара
        /// </summary>
        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            GridSplitter_MouseDoubleClick(sender: spliterGridGlobal, e:null);
        }

        #endregion Работа с дизайном окна

    }
}