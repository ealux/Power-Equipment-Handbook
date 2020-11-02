using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Power_Equipment_Handbook.src.windows
{
    /// <summary>
    /// Логика взаимодействия для CellElementHandbook.xaml
    /// </summary>
    public partial class CellElementHandbook : Window, INotifyPropertyChanged
    {
        readonly Cell cell;
        readonly DBProvider db;

        List<BreakerCell> Br = new List<BreakerCell>();
        List<DisconnectorCell> Dis = new List<DisconnectorCell>();
        List<ShortCircuiterCell> SC = new List<ShortCircuiterCell>();
        List<TTCell> TT = new List<TTCell>();
        //List<BusbarCell> BB = new List<BusbarCell>();

        private int _idCounter;
        public int IdCounter
        {
            get { return _idCounter; }
            set
            {
                if (value != _idCounter)
                {
                    _idCounter = value;
                    OnPropertyChanged("IdCounter");
                }
            }
        }


        /// <summary>
        /// Конструктор окна упрощенного добавления оборудования
        /// </summary>
        /// <param name="cell">Передаваемый элемент ячейки для добавления</param>
        /// <param name="db">Доступ к базе данных</param>
        public CellElementHandbook(ref Cell cell, DBProvider db)
        {
            InitializeComponent();
            this.cell = cell;
            this.db = db;

            lblCount.DataContext = this;
            lblCellInfo.Content = $"Узел: {cell.NodeNumber}\nЯчейка: {cell.Name}\nUном: {cell.Unom} кВ";

            try     //Вставка Unom
            {
                double unom = this.cell.Unom;
                foreach (ListBoxItem i in cmbUnom_E.Items)
                {
                    if (double.Parse(i.Content.ToString(), CultureInfo.InvariantCulture) == unom)
                    {
                        cmbUnom_E.SelectedItem = i;
                        return;
                    }
                }
            }
            catch (Exception) { }            
        }


        #region Обработчики конкретных событий

        /// <summary>
        /// Выгрузка данных из базы по факту выбора типа Оборудования
        /// </summary>
        private void cmbType_E_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke((Action)delegate ()
            {
                ComboBox cmb = (ComboBox)sender;

                if (cmb.SelectedItem == null) return;

                cmbTypeName_E.ItemsSource = null;   //Отчистка списка полученного ранее оборудования
                cmbTypeName_E.Text = String.Empty;
                txtInom_E.Clear();
                txtIotkl_E.Clear();
                txtIterm_E.Clear();
                txtTterm_E.Clear();
                txtBterm_E.Clear();
                txtIudar_E.Clear();

                double unom;
                string type;

                if (cmbUnom_E.SelectedIndex == -1 | cmbUnom_E.Text == "") unom = 0.0;              //Получение Unom
                else unom = double.Parse(cmbUnom_E.Text, CultureInfo.InvariantCulture);

                type = ((ListBoxItem)cmb.SelectedItem).Content.ToString();  //Получение типа оборудования

                GetData(type, unom, this.db);
            });
        }

        /// <summary>
        /// Выгрузка данных из базы по факту выбора типа Unom
        /// </summary>
        private void CmbUnom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke((Action)delegate ()
            {
                ComboBox cmb = (ComboBox)sender;
                
                if (cmbType_E.Text == "" | cmb.SelectedItem == null) return;

                cmbTypeName_E.ItemsSource = null;   //Отчистка списка полученного ранее оборудования
                cmbTypeName_E.Text = String.Empty;
                txtInom_E.Clear();
                txtIotkl_E.Clear();
                txtIterm_E.Clear();
                txtTterm_E.Clear();
                txtBterm_E.Clear();
                txtIudar_E.Clear();

                double unom;
                string type;

                if (cmb.SelectedIndex == -1 | ((ListBoxItem)cmb.SelectedItem).Content.ToString() == "") unom = 0;              //Получение Unom
                else unom = double.Parse(((ListBoxItem)cmb.SelectedItem).Content.ToString(), CultureInfo.InvariantCulture);

                type = ((ListBoxItem)cmbType_E.SelectedItem).Content.ToString();  //Получение типа оборудования

                GetData(type, unom, this.db);
            });
        }

        /// <summary>
        /// Извлечение данных из базы
        /// </summary>
        /// <param name="type">Тип элемента</param>
        /// <param name="unom">Номинальное напряжение для отбора</param>
        /// <param name="provider">Провайдер БД</param>
        private void GetData(string type, double unom, DBProvider provider)
        {
            string comm;

            switch (type)
            {
                case "Выключатель":
                    Br.Clear();

                    if (cmbUnom_E.SelectedIndex != -1 & unom != 0.0) comm = $"SELECT * FROM [Breakers] WHERE [Unom] = {unom}".Replace(',', '.');
                    else comm = $"SELECT * FROM [Breakers]".Replace(',', '.');

                    using (var sqldata = provider.Command_Query(comm, provider.Connection))
                    {
                        if (sqldata.HasRows == false) return;

                        while (sqldata.Read())
                        {
                            Br.Add(new BreakerCell()
                            {
                                Name = sqldata["Марка"] as string,
                                Inom = sqldata["Inom"] as double?,
                                Iotkl = sqldata["Iоткл"] as double?,
                                Iterm = sqldata["Iтерм"] as double?,
                                Tterm = sqldata["tтерм"] as double?,
                                Iudar = sqldata["iуд"] as double?
                            });
                        }
                    }
                    cmbTypeName_E.ItemsSource = Br;
                    cmbTypeName_E.DisplayMemberPath = "Name";

                    break;

                case "Разъединитель":
                    Dis.Clear();

                    if (cmbUnom_E.SelectedIndex != -1 & unom != 0) comm = $"SELECT * FROM [Disconnector] WHERE [Unom] = {unom}".Replace(',', '.');
                    else comm = $"SELECT * FROM [Disconnector]".Replace(',', '.');

                    using (var sqldata = provider.Command_Query(comm, provider.Connection))
                    {
                        if (sqldata.HasRows == false) return;

                        while (sqldata.Read())
                        {
                            Dis.Add(new DisconnectorCell()
                            {
                                Name = sqldata["Марка"] as string,
                                Inom = sqldata["Inom"] as double?,
                                //Iotkl = sqldata["Iоткл"] as double?,
                                Iterm = sqldata["Iтерм"] as double?,
                                Tterm = sqldata["tтерм"] as double?,
                                Iudar = sqldata["iуд"] as double?
                            });
                        }
                    }
                    cmbTypeName_E.ItemsSource = Dis;
                    cmbTypeName_E.DisplayMemberPath = "Name";

                    break;

                case "Отд./КЗ":
                    SC.Clear();

                    if (cmbUnom_E.SelectedIndex != -1 & unom != 0.0) comm = $"SELECT * FROM [Short-circuiter] WHERE [Unom] = {unom}".Replace(',', '.');
                    else comm = $"SELECT * FROM [Short-circuiter]".Replace(',', '.');

                    using (var sqldata = provider.Command_Query(comm, provider.Connection))
                    {
                        if (sqldata.HasRows == false) return;

                        while (sqldata.Read())
                        {
                            SC.Add(new ShortCircuiterCell()
                            {
                                Name = sqldata["Марка"] as string,
                                Inom = sqldata["Inom"] as double?,
                                //Iotkl = sqldata["Iоткл"] as double?,
                                Iterm = sqldata["Iтерм"] as double?,
                                Tterm = sqldata["tтерм"] as double?,
                                Iudar = sqldata["iуд"] as double?
                            });
                        }
                    }
                    cmbTypeName_E.ItemsSource = SC;
                    cmbTypeName_E.DisplayMemberPath = "Name";

                    break;

                case "ТТ":
                    TT.Clear();

                    if (cmbUnom_E.SelectedIndex != -1 & unom != 0.0) comm = $"SELECT * FROM [TT] WHERE [Unom] = {unom}".Replace(',', '.');
                    else comm = $"SELECT * FROM [TT]".Replace(',', '.');

                    using (var sqldata = provider.Command_Query(comm, provider.Connection))
                    {
                        if (sqldata.HasRows == false) return;

                        while (sqldata.Read())
                        {
                            TT.Add(new TTCell()
                            {
                                Name = sqldata["Марка"] as string,
                                Inom = sqldata["Iперв"] as double?,
                                //Iotkl = sqldata["Iоткл"] as double?,
                                Iterm = sqldata["Iтерм"] as double?,
                                Tterm = sqldata["tтерм"] as double?,
                                Iudar = sqldata["iуд"] as double?
                            });
                        }
                    }
                    cmbTypeName_E.ItemsSource = TT;
                    cmbTypeName_E.DisplayMemberPath = "Name";

                    break;

                    //case "Ошиновка":
                    //    BB.Clear();

                    //    if (cmbUnom_E.SelectedIndex != -1 | unom != 0.0) comm = $"SELECT * FROM [Busbar] WHERE [Unom] = {unom}".Replace(',', '.');
                    //    else comm = $"SELECT * FROM [Busbar]".Replace(',', '.');

                    //    break;                  

            }
        }


        /// <summary>
        /// Выбор марки оборудования
        /// </summary>
        private void CmbTypeName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            if (cmb.SelectedItem == null) return;

            Element el = ((IEnumerable<Element>)cmb.ItemsSource).First(n => n.Name == ((Element)cmb.SelectedItem).Name);

            txtInom_E.DataContext = el; txtIotkl_E.DataContext = el; txtIterm_E.DataContext = el;
            txtBterm_E.DataContext = el; txtTterm_E.DataContext = el;
            txtIudar_E.DataContext = el;
        }

        #endregion Обработчики конкретных событий



        #region Helpers

        /// <summary>
        /// Проверка ввода вещественных чисел
        /// </summary>
        private void DoubleChecker(object sender, TextCompositionEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl)) return; //Отлавливаем Ctrl
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
        /// Расчет Iтерм по заданному Iоткл, а также Bтерм по параметрам Iтерм и tтерм
        /// </summary>
        private void Calculations(object sender, TextChangedEventArgs e)
        {
            var txt = (TextBox)sender;

            if (txt.Text.StartsWith(".")) return;   //Проверка на корректный ввод

            if (txt == txtIterm_E | txt == txtTterm_E) //Расчёт термического импульса
            {
                DotCommaReplacer(sender: sender, e: e); //Обработка запятых
                if (txtIterm_E.Text == "" | txtIterm_E.Text == "") return;  //Проверка полей

                NumberFormatInfo provider = new NumberFormatInfo() { NumberDecimalSeparator = "." };   //Культура сепаратора (точка)

                double I = txtIterm_E.Text != "" ? Convert.ToDouble(txtIterm_E.Text, provider) : 0.0;
                double T = txtTterm_E.Text != "" ? Convert.ToDouble(txtTterm_E.Text, provider) : 0.0;

                txtBterm_E.Text = (I * I * T).ToString(); 
            }

            if (txt == txtIotkl_E) //Расчёт термического тока
            {
                DotCommaReplacer(sender: sender, e: e); //Обработка запятых
                if (txtIotkl_E.Text == "") return;  //Проверка полей

                NumberFormatInfo provider = new NumberFormatInfo() { NumberDecimalSeparator = "." };   //Культура сепаратора (точка)

                double I = txtIotkl_E.Text != "" ? Convert.ToDouble(txtIotkl_E.Text, provider) : 0.0;

                txtIterm_E.Text = I.ToString();
            }
        }



        /// <summary>
        /// Отчистка полей (Ctrl+N) - Отлов горячих клавиш
        /// </summary>
        private void Tab_Elements_KeyDown(object sender, KeyEventArgs e)
        {
            //Нажатие сочетания Ctrl+N - отчстка полей ввода
            if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && (e.Key == Key.N)) Clear();
        }

        /// <summary>
        /// Отчистка полей - Метод
        /// </summary>
        private void Clear()
        {
            cmbType_E.SelectedIndex = -1;
            cmbTypeName_E.SelectedIndex = -1;
            cmbTypeName_E.ItemsSource = null;
            cmbTypeName_E.Text = String.Empty;
            txtInom_E.Clear();
            txtIotkl_E.Clear();
            txtIterm_E.Clear();
            txtTterm_E.Clear();
            txtBterm_E.Clear();
            txtIudar_E.Clear();
        }

        #endregion Helpers        

        /// <summary>
        /// Добавление ячейки
        /// </summary>
        private void btnAdd_E_Click(object sender, RoutedEventArgs e)
        {
            if (cmbType_E.Text == "" | cmbTypeName_E.Text == "" ) return;

            var name = cmbTypeName_E.Text;
            var inom = (string.IsNullOrWhiteSpace(txtInom_E.Text) || double.Parse(txtInom_E.Text, CultureInfo.InvariantCulture) == 0) ? new double?() : double.Parse(txtInom_E.Text, CultureInfo.InvariantCulture);
            var iotkl = (string.IsNullOrWhiteSpace(txtIotkl_E.Text) || double.Parse(txtIotkl_E.Text, CultureInfo.InvariantCulture) == 0) ? new double?() : double.Parse(txtIotkl_E.Text, CultureInfo.InvariantCulture);
            var iterm = (string.IsNullOrWhiteSpace(txtIterm_E.Text) || double.Parse(txtIterm_E.Text, CultureInfo.InvariantCulture) == 0) ? new double?() : double.Parse(txtIterm_E.Text, CultureInfo.InvariantCulture);
            var iud = (string.IsNullOrWhiteSpace(txtIudar_E.Text) || double.Parse(txtIudar_E.Text, CultureInfo.InvariantCulture) == 0) ? new double?() : double.Parse(txtIudar_E.Text, CultureInfo.InvariantCulture);
            var tterm = (string.IsNullOrWhiteSpace(txtTterm_E.Text) || double.Parse(txtTterm_E.Text, CultureInfo.InvariantCulture) == 0) ? new double?() : double.Parse(txtTterm_E.Text, CultureInfo.InvariantCulture);
            var bterm = (string.IsNullOrWhiteSpace(txtBterm_E.Text) || double.Parse(txtBterm_E.Text, CultureInfo.InvariantCulture) == 0) ? new double?() : double.Parse(txtBterm_E.Text, CultureInfo.InvariantCulture);
            var unom = cell.Unom;

            switch (cmbType_E.Text)
            {
                case "Выключатель":
                    cell.CellElements.Add(new BreakerCell() { Name = name, Inom = inom, Iotkl = iotkl, Iterm = iterm, Iudar = iud, Bterm = bterm, Tterm = tterm, Unom = unom });
                    break;
                case "Разъединитель":
                    cell.CellElements.Add(new DisconnectorCell() { Name = name, Inom = inom, Iotkl = iotkl, Iterm = iterm, Iudar = iud, Bterm = bterm, Tterm = tterm, Unom = unom });
                    break;
                case "Отд./КЗ":
                    cell.CellElements.Add(new ShortCircuiterCell() { Name = name, Inom = inom, Iotkl = iotkl, Iterm = iterm, Iudar = iud, Bterm = bterm, Tterm = tterm, Unom = unom });
                    break;
                case "ТТ":
                    cell.CellElements.Add(new TTCell() { Name = name, Inom = inom, Iotkl = iotkl, Iterm = iterm, Iudar = iud, Bterm = bterm, Tterm = tterm, Unom = unom });
                    break;
                case "Ошиновка":
                    cell.CellElements.Add(new BusbarCell() { Name = name, Inom = inom, Iotkl = iotkl, Iterm = iterm, Iudar = iud, Bterm = bterm, Tterm = tterm, Unom = unom });
                    break;
            }

            Clear();

            IdCounter++;

            if (chkCloser.IsChecked == true) this.Close();
        }


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            var handler = System.Threading.Interlocked.CompareExchange(ref PropertyChanged, null, null);
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged Implementation
    }
}
