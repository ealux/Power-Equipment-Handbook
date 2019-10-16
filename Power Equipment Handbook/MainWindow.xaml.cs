using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Power_Equipment_Handbook.src;

namespace Power_Equipment_Handbook
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataGridTracker track;
        DBProvider db_prv;

        public ObservableCollection<Line> Lines;
        public ObservableCollection<Trans> Trans;
        public ObservableCollection<MultiTrans> MultiTrans;

        public MainWindow()
        {
            InitializeComponent();
            track = new DataGridTracker(grdNodes, grdBranches);

            db_prv = new DBProvider("test.db");
            Status_Text.Text = "Состояние подключения:   " + db_prv.Status;

            cmbType_T.SelectedIndex = 1;
            cmbType_T.SelectedIndex = 0;

            txtStartNode_L.ItemsSource = track.Nodes; txtStartNode_L.DisplayMemberPath = "Number";
            txtEndNode_L.ItemsSource = track.Nodes; txtEndNode_L.DisplayMemberPath = "Number";
            txtStartNode_T.ItemsSource = track.Nodes; txtStartNode_T.DisplayMemberPath = "Number";
            txtEndHighNode_T.ItemsSource = track.Nodes; txtEndHighNode_T.DisplayMemberPath = "Number";
            txtEndMidNode_T.ItemsSource = track.Nodes; txtEndMidNode_T.DisplayMemberPath = "Number";
            txtEndLowNode_T.ItemsSource = track.Nodes; txtEndLowNode_T.DisplayMemberPath = "Number";

            Lines = new ObservableCollection<Line>();
            Trans = new ObservableCollection<Trans>();
            MultiTrans = new ObservableCollection<MultiTrans>();
        }

        #region Helpers
        /// <summary>
        /// Проверка ввода цифр
        /// </summary>
        private void DigitChecker(object sender, TextCompositionEventArgs e)
        {
            object tb;

            try { tb = (ComboBox)sender; }
            catch (Exception) { tb = (TextBox)sender; }
            if (tb.GetType() == typeof(ComboBox)) ChangeCmbColor(sender, false);
            else if(tb.GetType() == typeof(TextBox)) ChangeTxtColor(sender, false);
            if (!char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        /// <summary>
        /// Проверка ввода вещественных чисел
        /// </summary>
        private void DoubleChecker(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if ((e.Text.Contains(".") || e.Text.Contains(",")) & (tb.Text.Contains(".") || tb.Text.Contains(","))) e.Handled = true;
            if (!(Char.IsDigit(e.Text, 0) | Char.IsPunctuation(e.Text, 0))) e.Handled = true;
        }

        /// <summary>
        /// Замена введенной запятой на точку
        /// </summary>
        private void DotCommaReplacer(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            using (tb.DeclareChangeBlock())
            {
                foreach (var c in e.Changes)
                {
                    if (c.AddedLength == 0) continue;
                    tb.Select(c.Offset, c.AddedLength);
                    if (tb.SelectedText.Contains(',')) tb.SelectedText = tb.SelectedText.Replace(',', '.');
                    tb.Select(c.Offset + c.AddedLength, 0);
                }
            }
        }

        /// <summary>
        /// Расчет параметров Линии по Длине и Удельным парамертрам (автоподстановка)
        /// </summary>
        private void Line_Calculations(object sender, TextChangedEventArgs e)
        {
            //TODO обработчики неверного заполнения (цвета)
            DotCommaReplacer(sender: sender, e: e); //Обработка запятых
            if (txtLength_L.Text == "") return;     //Проверка поля Длина

            var use = (TextBox)sender;
            if (use.Text.StartsWith(".")) return; 

            NumberFormatInfo provider = new NumberFormatInfo() { NumberDecimalSeparator = "."};

            double L = Convert.ToDouble(txtLength_L.Text, provider);

            if (use != txtLength_L)
            {
                double res;
                if (use.Text != "") res = Convert.ToDouble(use.Text, provider) * L;
                else return;

                if (use == txtr0_L) txtR_L.Text = res.ToString(); if (use == txtx0_L) txtX_L.Text = res.ToString();
                if (use == txtb0_L) txtB_L.Text = res.ToString(); if (use == txtg0_L) txtG_L.Text = res.ToString();
            }
            else if(use == txtLength_L)
            {
                if(txtr0_L.Text != "") txtR_L.Text = (Convert.ToDouble(txtr0_L.Text, provider) * L).ToString();
                if (txtx0_L.Text != "") txtX_L.Text = (Convert.ToDouble(txtx0_L.Text, provider) * L).ToString();
                if (txtb0_L.Text != "") txtB_L.Text = (Convert.ToDouble(txtb0_L.Text, provider) * L).ToString();
                if (txtg0_L.Text != "") txtG_L.Text = (Convert.ToDouble(txtg0_L.Text, provider) * L).ToString();
            }
        }
        
        /// <summary>
        /// Расчет коэффициентов трансформации
        /// </summary>
        private void Ktr_Calculations(object sender, TextChangedEventArgs e)
        {
            //TODO обработчики неверного заполнения(цвета)
            DotCommaReplacer(sender: sender, e: e); //Обработка запятых

            var HV = txtUnomHigh_T; var LV = txtUnomLowDouble_T;
            var MHV = txtUnomMid_T; var LHV = txtUnomLow_T;

            if (HV.Text.StartsWith(".")) return;
            if (cmbType_T.Text == "двух.")
            {
                if(!string.IsNullOrEmpty(HV.Text) & !string.IsNullOrEmpty(LV.Text))
                {
                    if (LV.Text.StartsWith(".")) return;
                    txtKH_KML_T.Text = (Convert.ToDouble(LV.Text, CultureInfo.InvariantCulture) / Convert.ToDouble(HV.Text, CultureInfo.InvariantCulture)).ToString();
                }
            }
            if(cmbType_T.Text == "тр./АТ")
            {
                txtKH_KML_T.Text = "1";

                if(!string.IsNullOrEmpty(HV.Text) & !string.IsNullOrEmpty(MHV.Text))
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
        /// Изменяет цвет TextBox'a в зависимости от аргумента error метода 
        /// </summary>
        private void ChangeTxtColor(object sender, bool error)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                TextBox tb = (TextBox)sender;
                if (error == true) tb.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else { tb.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); }
            });
        }

        /// <summary>
        /// Изменяет цвет ComboBox'a в зависимости от аргумента error метода 
        /// </summary>
        private void ChangeCmbColor(object sender, bool error)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                ComboBox tb = (ComboBox)sender;
                if (error == true) tb.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else { tb.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); }
            });
        }

        /// <summary>
        /// Проверяет наличие аналогичной ветви и смещает номер параллельности в случае обнаружения
        /// </summary>
        private bool BranchChecker(Branch br, object cmbStart, object cmbEnd)
        {
            bool result = NodeChecker(br, cmbStart, cmbEnd);
            if (result)
            {
                var other = track.Branches.Where((b) => b.Equals(br)).OrderByDescending((b) => b.Npar).ToList();
                if (other != null && other.Count > 0) br.Npar = other[0].Npar + 1;
            }

            return result;  
        }
        
        /// <summary>
        /// Проверка наличия узлов по параметрам ввода начала и конца ветви
        /// </summary>
        private bool NodeChecker(Branch br, object cmbStart, object cmbEnd)
        {
            int start = br.Start;
            int end = br.End;

            var nodesSt = track.Nodes.Where((n) => n.Number == start).ToList();
            var nodesEn = track.Nodes.Where((n) => n.Number == end).ToList();
            var reverse_br = track.Branches.Where((b) => (b.Start == br.End) & (b.End == br.Start)).ToList();

            if (reverse_br.Count != 0) { ChangeCmbColor(cmbStart, true); ChangeCmbColor(cmbEnd, true); return false; }
            if (nodesSt.Count == 0) ChangeCmbColor(cmbStart, true);
            if (nodesEn.Count == 0) ChangeCmbColor(cmbEnd, true);

            if (nodesSt.Count == 0 || nodesEn.Count == 0) return false;
            else return true;
        }

        /// <summary>
        /// Извлечение данных из базы
        /// </summary>
        /// <param name="type">Тип элемента ("Line" или "Trans")</param>
        /// <param name="unom">Номинальное напряжение для отбора</param>
        /// <param name="provider">Провайдер БД</param>
        private void GetData(string type, int unom, DBProvider provider)
        {
            //Таблица Lines
            if (type == "Line")
            {
                Lines.Clear();

                using (var sqldata = provider.Command_Query(String.Format(@"SELECT * FROM [Lines] WHERE [Unom] = {0}", unom), provider.Connection))
                {
                    if (sqldata.HasRows == false) return;

                    while (sqldata.Read())
                    {
                        Lines.Add(new Line()
                        {
                            Unom = sqldata["Unom"] as int?,
                            TypeName = sqldata["TypeName"] as string,
                            R0 = sqldata["R0"] as double?,
                            X0 = sqldata["X0"] as double?,
                            B0 = sqldata["B0"] as double?,
                            G0 = sqldata["G0"] as double?,
                            Idd = sqldata["Idd"] as double?,
                            Source = sqldata["Source"] as string
                        });
                    }
                }
                cmbTypeName_L.ItemsSource = Lines;
                cmbTypeName_L.DisplayMemberPath = "TypeName";
            }
            //Таблица Trans
            if(type == "Trans")
            {
                //Двухобмоточные
                if(cmbType_T.Text == "двух.")
                {
                    Trans.Clear();

                    using (var sqldata = provider.Command_Query(String.Format(@"SELECT * FROM [Trans] WHERE [Unom] = {0}", unom), provider.Connection))
                    {
                        if (sqldata.HasRows == false) return;

                        while (sqldata.Read())
                        {
                            Trans.Add(new Trans()
                            {
                                Unom = sqldata["Unom"] as int?,
                                TypeName = sqldata["TypeName"] as string,
                                UnomH = sqldata["UnomH"] as double?,
                                UnomL = sqldata["UnomL"] as double?,
                                R = sqldata["R"] as double?,
                                X = sqldata["X"] as double?,
                                B = sqldata["B"] as double?,
                                G = sqldata["G"] as double?,
                                Source = sqldata["Source"] as string
                            });
                        }
                    }

                    cmbTypeName_T.ItemsSource = Trans;
                    cmbTypeName_T.DisplayMemberPath = "TypeName";
                }
                //Трехобмоточные и Автотрансформаторы
                if(cmbType_T.Text == "тр./АТ")
                {
                    MultiTrans.Clear();

                    using (var sqldata = provider.Command_Query(String.Format(@"SELECT * FROM [MultiTrans] WHERE [Unom] = {0}", unom), provider.Connection))
                    {
                        if (sqldata.HasRows == false) return;

                        while (sqldata.Read())
                        {
                            MultiTrans.Add(new MultiTrans()
                            {
                                Unom = sqldata["Unom"] as int?,
                                TypeName = sqldata["TypeName"] as string,
                                UnomH = sqldata["UnomH"] as double?,
                                UnomM = sqldata["UnomM"] as double?,
                                UnomL = sqldata["UnomL"] as double?,
                                RH = sqldata["RH"] as double?,
                                RM = sqldata["RM"] as double?,
                                RL = sqldata["RL"] as double?,
                                XH = sqldata["XH"] as double?,
                                XM = sqldata["XM"] as double?,
                                XL = sqldata["XL"] as double?,
                                B = sqldata["B"] as double?,
                                G = sqldata["G"] as double?,
                                Source = sqldata["Source"] as string
                            });
                        }
                    }

                    cmbTypeName_T.ItemsSource = MultiTrans;
                    cmbTypeName_T.DisplayMemberPath = "TypeName";
                } 
            }
        }
        #endregion

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

                Line l = Lines.Where((n) => n.TypeName == (cmb.SelectedItem as Line).TypeName).First();

                txtr0_L.DataContext = l; txtx0_L.DataContext = l; txtb0_L.DataContext = l; txtg0_L.DataContext = l;
                txtIdd_L.DataContext = l; lblSource_L.DataContext = l;
            }

            //Выбор марки трансформатора
            if (cmb.Equals(cmbTypeName_T))
            {
                //Двухобмоточные трансформаторы
                if(cmbType_T.Text == "двух.")
                {
                    if (cmb.SelectedItem == null)
                    {
                        txtRH_T.Clear(); txtXH_T.Clear(); txtBH_T.Clear(); txtGH_T.Clear(); txtUnomHigh_T.Clear(); txtUnomLowDouble_T.Clear(); lblSource_T.Content = String.Empty;
                        return;
                    }
                    if (Trans.Count == 0) return;

                    Trans t = Trans.Where((n) => n.TypeName == (cmb.SelectedItem as Trans).TypeName).First();

                    txtRH_T.SetBinding(TextBox.TextProperty, new Binding("R") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtXH_T.SetBinding(TextBox.TextProperty, new Binding("X") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtBH_T.SetBinding(TextBox.TextProperty, new Binding("B") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtGH_T.SetBinding(TextBox.TextProperty, new Binding("G") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    txtUnomHigh_T.SetBinding(TextBox.TextProperty, new Binding("UnomH") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                    txtUnomLow_T.SetBinding(TextBox.TextProperty, new Binding("UnomL") { Source = t, Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                    lblSource_T.Content = t.Source;
                }
                //Трехобмоточные и Автотрансформаторы
                if(cmbType_T.Text == "тр./АТ")
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
                    if (Trans.Count == 0) return;

                    MultiTrans t = MultiTrans.Where((n) => n.TypeName == (cmb.SelectedItem as MultiTrans).TypeName).First();

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
            if (cmbUnom_T.Text != "") GetData("Trans", Convert.ToInt32(cmbUnom_T.Text), db_prv);
        }

        /// <summary>
        /// Добавить узел в список узлов
        /// </summary>
        private void BtnAdd_N_Click(object sender, RoutedEventArgs e)
        {
            int number = default(int);
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                if (txtNumber_N.Text == "" || txtNumber_N.Text == null) { ChangeTxtColor(txtNumber_N, true); return; } else { number = int.Parse(txtNumber_N.Text); }
                int state = (string.IsNullOrWhiteSpace(txtState_L.Text) || int.Parse(txtState_L.Text) == 0) ? 0 : 1;
                string type = txtType_N.Text;
                int unom = (string.IsNullOrWhiteSpace(cmbUnom_N.Text) || int.Parse(cmbUnom_N.Text) == 0) ? 0 : int.Parse(cmbUnom_N.Text);
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

                if (track.Nodes.Where((n) => n.Number == number).ToList().Count != 0) { ChangeTxtColor(txtNumber_N, true); return; }

                track.AddNode(new Node(number: number, unom: unom, type: type,
                                       state: state, name: name,
                                       p_n: p_n, q_n: q_n, p_g: p_g, q_g: q_g,
                                       vzd: vzd, q_min: q_min, q_max: q_max, b_sh: b_sh,
                                       region: region));

                Tab_Data.SelectedIndex = 0;
            });
        }

        /// <summary>
        /// Добавить линию в список ветвей
        /// </summary>
        private void BtnAdd_L_Click(object sender, RoutedEventArgs e)
        {
            int start = default(int);
            int end = default(int);
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                if (txtStartNode_L.Text == "" || txtStartNode_L.Text == null) { ChangeCmbColor(txtStartNode_L, true); } else { start = int.Parse(txtStartNode_L.Text); }
                if (txtEndNode_L.Text == ""  || txtEndNode_L.Text == null) { ChangeCmbColor(txtEndNode_L, true); return; } else { end = int.Parse(txtEndNode_L.Text); }
                if (txtStartNode_L.Text == txtEndNode_L.Text) { ChangeCmbColor(txtStartNode_L, true); ChangeCmbColor(txtEndNode_L, true); return; }
                else { ChangeCmbColor(txtStartNode_L, false); ChangeCmbColor(txtEndNode_L, false); }

                if (start == default(int)) { ChangeCmbColor(txtStartNode_L, true); return; }
                if (end == default(int)) { ChangeCmbColor(txtEndNode_L, true); return; }

                int state = (string.IsNullOrWhiteSpace(txtState_L.Text) || int.Parse(txtState_L.Text) == 0) ? 0 : 1;
                string type = "ЛЭП";
                int npar = (string.IsNullOrWhiteSpace(txtNpar_L.Text) || int.Parse(txtNpar_L.Text) == 0) ? 0 : int.Parse(txtNpar_L.Text);
                string typename = cmbTypeName_L.Text;
                string name = txtName_L.Text;
                double r = (string.IsNullOrWhiteSpace(txtR_L.Text) || double.Parse(txtR_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtR_L.Text, CultureInfo.InvariantCulture);
                double x = (string.IsNullOrWhiteSpace(txtX_L.Text) || double.Parse(txtX_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtX_L.Text, CultureInfo.InvariantCulture);
                double b = (string.IsNullOrWhiteSpace(txtB_L.Text) || double.Parse(txtB_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtB_L.Text, CultureInfo.InvariantCulture);
                double g = (string.IsNullOrWhiteSpace(txtG_L.Text) || double.Parse(txtG_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtG_L.Text, CultureInfo.InvariantCulture);
                double ktr = 1;
                double idd = (string.IsNullOrWhiteSpace(txtIdd_L.Text) || double.Parse(txtIdd_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtIdd_L.Text, CultureInfo.InvariantCulture);
                int region = (string.IsNullOrWhiteSpace(txtRegion_L.Text) || int.Parse(txtRegion_L.Text) == 0) ? 0 : int.Parse(txtRegion_L.Text);

                Branch br = new Branch(start: start, end: end, type: type,
                                           state: state, typename: typename, name: name, npar: npar,
                                           r: r, x: x, b: b, g: g,
                                           ktr: ktr, idd: idd, region: region);

                if (BranchChecker(br, txtStartNode_L, txtEndNode_L) == true) track.AddBranch(br);
                else return;

                Tab_Data.SelectedIndex = 1;
            });

        }

        /// <summary>
        /// Добавить трансформатор в список ветвей
        /// </summary>
        private void BtnAdd_T_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                if (cmbType_T.Text == "двух.")
                {
                    int start = default(int);
                    int end = default(int);
                    if (txtStartNode_T.Text == "" || txtStartNode_T.Text == null) { ChangeCmbColor(txtStartNode_T, true); } else { start = int.Parse(txtStartNode_T.Text); }
                    if (txtEndHighNode_T.Text == "" || txtEndHighNode_T.Text == null) { ChangeCmbColor(txtEndHighNode_T, true); } else { end = int.Parse(txtEndHighNode_T.Text); }
                    if(txtStartNode_T.Text == txtEndHighNode_T.Text) { ChangeCmbColor(txtStartNode_T, true); ChangeCmbColor(txtEndHighNode_T, true);  return; }

                    if (start == default(int)) { ChangeCmbColor(txtStartNode_T, true); return; }
                    if (end == default(int)) { ChangeCmbColor(txtStartNode_T, true); return; }

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

                    Branch br = new Branch(start: start, end: end, type: type,
                                           state: state, typename: typename, name: name, npar: npar,
                                           r: r, x: x, b: b, g: g,
                                           ktr: ktr, idd: idd, region: region);

                    if (BranchChecker(br, txtStartNode_T, txtEndHighNode_T) == true) track.AddBranch(br);
                    else return;

                    Tab_Data.SelectedIndex = 1;
                    return;
                }
                else if (cmbType_T.Text == "тр./АТ")
                {
                    int start = default(int);
                    int endH = default(int);
                    int endM = default(int);
                    int endL = default(int);
                    if (txtStartNode_T.Text == "" || txtStartNode_T.Text == null) { ChangeCmbColor(txtStartNode_T, true); } else { start = int.Parse(txtStartNode_T.Text); }
                    if (txtEndHighNode_T.Text == "" || txtEndHighNode_T.Text == null) { ChangeCmbColor(txtEndHighNode_T, true); } else { endH = int.Parse(txtEndHighNode_T.Text); }
                    if (txtEndMidNode_T.Text == "" || txtEndMidNode_T.Text == null) { ChangeCmbColor(txtEndMidNode_T, true); } else { endM = int.Parse(txtEndMidNode_T.Text); }
                    if (txtEndLowNode_T.Text == "" || txtEndLowNode_T.Text == null) { ChangeCmbColor(txtEndLowNode_T, true); } else { endL = int.Parse(txtEndLowNode_T.Text); }

                    if (start == default(int)) { ChangeCmbColor(txtStartNode_T, true); return; }
                    if (endH == default(int)) { ChangeCmbColor(txtEndHighNode_T, true); return; }
                    if (endM == default(int)) { ChangeCmbColor(txtEndMidNode_T, true); return; }
                    if (endL == default(int)) { ChangeCmbColor(txtEndLowNode_T, true); return; }

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

                    if (result1 ==true && result2== true && result3== true)
                    {
                        track.AddBranch(br1);
                        track.AddBranch(br2);
                        track.AddBranch(br3);
                    }
                    else return;
                        
                    Tab_Data.SelectedIndex = 1;
                    return;
                }
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
                if(cmb.SelectedItem == null)
                {
                    Lines.Clear();
                    return;
                }
                GetData("Line", Convert.ToInt32((cmb.SelectedItem as ListBoxItem).Content.ToString()), db_prv);
            }
            if (cmb.Equals(cmbUnom_T))
            {
                if (cmb.SelectedItem == null)
                {
                    Trans.Clear();
                    MultiTrans.Clear();
                    return;
                }
                GetData("Trans", Convert.ToInt32((cmb.SelectedItem as ListBoxItem).Content.ToString()), db_prv);
            }
        }
        #endregion

        
    }
}
