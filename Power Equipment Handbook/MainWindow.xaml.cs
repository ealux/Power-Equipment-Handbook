using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using Power_Equipment_Handbook.src;

namespace Power_Equipment_Handbook
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataGridTracker track;
        public MainWindow()
        {
            InitializeComponent();
            track = new DataGridTracker(grdNodes, grdBranches);

            track.AddNode(new Node(1, 110, "нагр."));
            track.AddNode(new Node(2, 110, "нагр."));
            track.AddBranch(new Branch(1, 2, "ЛЭП", npar: 1));
            track.AddBranch(new Branch(1, 2, "ЛЭП", npar: 2));
        }


        #region Helpers

        /// <summary>
        /// Проверка ввода цифр
        /// </summary>
        private void DigitChecker(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Проверка ввода вещественных чисел
        /// </summary>
        private void DoubleChecker(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if ((e.Text.Contains(".") || e.Text.Contains(",")) &
                (tb.Text.Contains(".") || tb.Text.Contains(","))) e.Handled = true;

            if (!(Char.IsDigit(e.Text, 0) | Char.IsPunctuation(e.Text, 0)))
            {
                e.Handled = true;
            }
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
                    if (tb.SelectedText.Contains(','))
                    {
                        tb.SelectedText = tb.SelectedText.Replace(',', '.');
                    }
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

        #endregion


        #region Обработчики конкретных событий

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
        /// Добавить линию в список ветвей
        /// </summary>
        private void BtnAdd_L_Click(object sender, RoutedEventArgs e)
        {
            //TODO Сделать проверки (цвета)
            //TODO Добавка узлов 
            //TODO Проверка на наличие + смещение по счетчику парал. линий
            int start = int.Parse(txtStartNode_L.Text);
            int end = int.Parse(txtEndNode_L.Text);
            int state = (string.IsNullOrWhiteSpace(txtState_L.Text) || int.Parse(txtState_L.Text) == 0) ? 0 : 1;
            string type = "ЛЭП";
            int npar = (string.IsNullOrWhiteSpace(txtNpar_L.Text) || int.Parse(txtNpar_L.Text) == 0) ? 0 : int.Parse(txtNpar_L.Text);
            string name = txtName_L.Text;
            double r = (string.IsNullOrWhiteSpace(txtR_L.Text) || double.Parse(txtR_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtR_L.Text, CultureInfo.InvariantCulture);
            double x = (string.IsNullOrWhiteSpace(txtX_L.Text) || double.Parse(txtX_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtX_L.Text, CultureInfo.InvariantCulture);
            double b = (string.IsNullOrWhiteSpace(txtB_L.Text) || double.Parse(txtB_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtB_L.Text, CultureInfo.InvariantCulture);
            double g = (string.IsNullOrWhiteSpace(txtG_L.Text) || double.Parse(txtG_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtG_L.Text, CultureInfo.InvariantCulture);
            double ktr = 1;
            double idd = (string.IsNullOrWhiteSpace(txtIdd_L.Text) || double.Parse(txtIdd_L.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtIdd_L.Text, CultureInfo.InvariantCulture);
            int region = (string.IsNullOrWhiteSpace(txtRegion_L.Text) || int.Parse(txtRegion_L.Text) == 0) ? 0 : int.Parse(txtRegion_L.Text);

            track.AddBranch(new Branch(start: start, end: end, type: type, 
                                       state: state, name: name, npar: npar,
                                       r: r, x: x, b: b, g: g, 
                                       ktr: ktr, idd: idd, region: region));
            Tab_Data.SelectedIndex = 1;
        }

        private void BtnAdd_T_Click(object sender, RoutedEventArgs e)
        {
            //TODO Сделать проверки (цвета)
            //TODO Добавка узлов 
            //TODO Проверка на наличие + смещение по счетчику парал. тр-ров
            if (cmbType_T.Text == "двух.")
            {
                int start = int.Parse(txtStartNode_T.Text);
                int end = int.Parse(txtEndHighNode_T.Text);
                int state = (string.IsNullOrWhiteSpace(txtState_T.Text) || int.Parse(txtState_T.Text) == 0) ? 0 : 1;
                string type = "Тр-р";
                int npar = 0;
                string name = txtName_L.Text;
                double r = (string.IsNullOrWhiteSpace(txtRH_T.Text) || double.Parse(txtRH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtRH_T.Text, CultureInfo.InvariantCulture);
                double x = (string.IsNullOrWhiteSpace(txtXH_T.Text) || double.Parse(txtXH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtXH_T.Text, CultureInfo.InvariantCulture);
                double b = (string.IsNullOrWhiteSpace(txtBH_T.Text) || double.Parse(txtBH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtBH_T.Text, CultureInfo.InvariantCulture);
                double g = (string.IsNullOrWhiteSpace(txtGH_T.Text) || double.Parse(txtGH_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtGH_T.Text, CultureInfo.InvariantCulture);
                double ktr = (string.IsNullOrWhiteSpace(txtKH_KML_T.Text) || double.Parse(txtKH_KML_T.Text, CultureInfo.InvariantCulture) == 0) ? 0 : double.Parse(txtKH_KML_T.Text, CultureInfo.InvariantCulture);
                double idd = 0;
                int region = (string.IsNullOrWhiteSpace(txtRegion_T.Text) || int.Parse(txtRegion_T.Text) == 0) ? 0 : int.Parse(txtRegion_T.Text);

                track.AddBranch(new Branch(start: start, end: end, type: type,
                                       state: state, name: name, npar: npar,
                                       r: r, x: x, b: b, g: g,
                                       ktr: ktr, idd: idd, region: region));

                Tab_Data.SelectedIndex = 1;
                return;
            }
            //TODO Доделать другой тип транса
            
        }

        #endregion

        //DBProvider db_prv = new DBProvider("test.db");
        //Status_Text.Text = "Состояние подключения:   " + db_prv.Status;

        //string str_com = "DROP TABLE IF EXISTS [workers];"
        //                    + "CREATE TABLE [workers] ("
        //                    + "[id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,"
        //                    + "[name] char(100) NOT NULL,"
        //                    + "[family] char(100) NOT NULL,"
        //                    + "[age] int NOT NULL,"
        //                    + "[profession] char(100) NOT NULL"
        //                    + "); ";

        //db_prv.Command_NonQuery(str_com, db_prv.Connection);

        //Line l1 = new Line(Unom: 110, type_name: "AC-150")
        //{
        //    Base_params = new Base_Params(),
        //    Line_params = new Line_Params()
        //};

        //Trans t1 = new Trans(Unom: 220, type_name: "ТРДН-4000/50");

        //MultiTrans mt1 = new MultiTrans(Unom: 220, type_name: "АДЦТДНЕКСЫВЦО-500000/1000000")
        //{

        //};

    }
}
