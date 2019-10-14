using System;
using System.Diagnostics;
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
            ChangeTextBoxColor(sender, false);
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

        /// <summary>
        /// Изменяет цвет TextBox'a в зависимости от аргумента error метода 
        /// </summary>
        private void ChangeTextBoxColor(object sender, bool error)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                TextBox tb = (TextBox)sender;
                if (error) tb.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else { tb.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); }
            });

        }

        /// <summary>
        /// Проверяет наличие аналогичной ветви и смещает номер параллельности в случае обнаружения
        /// </summary>
        private Branch BranchChecker(Branch br)
        {
            NodeChecker(br);
            var other = track.Branches.Where((b) => b.Equals(br)).OrderByDescending((b) => b.Npar).ToList();
            if(other != null && other.Count > 0) br.Npar = other[0].Npar + 1;
            return br;
        }
        
        /// <summary>
        /// Проверка наличия узлов по параметрам ввода начала и конца ветви
        /// </summary>
        private void NodeChecker(Branch br)
        {
            int start = br.Start;
            int end = br.End;

            var nodesSt = track.Nodes.Where((n) => n.Number == start).ToList();
            var nodesEn = track.Nodes.Where((n) => n.Number == end).ToList();
            if (nodesSt.Count == 0) track.AddNode(new Node(number: start, unom: 0, type: "нагр."));
            if (nodesEn.Count == 0) track.AddNode(new Node(number: end, unom: 0, type: "нагр."));
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
            int start = default(int);
            int end = default(int);
            if (txtStartNode_L.Text == "") { ChangeTextBoxColor(txtStartNode_L, true);} else { start = int.Parse(txtStartNode_L.Text); }
            if (txtEndNode_L.Text == "" ) { ChangeTextBoxColor(txtEndNode_L, true);} else { end = int.Parse(txtEndNode_L.Text); }
            if (txtStartNode_L.Text == txtEndNode_L.Text) { ChangeTextBoxColor(txtStartNode_L, true); ChangeTextBoxColor(txtEndNode_L, true); return; }
            else { ChangeTextBoxColor(txtStartNode_L, false); ChangeTextBoxColor(txtEndNode_L, false); }
            if (start == default(int) || end == default(int)) return;

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

            track.AddBranch(BranchChecker(new Branch(start: start, end: end, type: type, 
                                       state: state, typename:typename, name: name, npar: npar,
                                       r: r, x: x, b: b, g: g, 
                                       ktr: ktr, idd: idd, region: region)));
            Tab_Data.SelectedIndex = 1;
        }

        /// <summary>
        /// Добавить трансформатор в список ветвей
        /// </summary>
        private void BtnAdd_T_Click(object sender, RoutedEventArgs e)
        {
            if (cmbType_T.Text == "двух.")
            {
                int start = default(int);
                int end = default(int);
                if (txtStartNode_T.Text == "") { ChangeTextBoxColor(txtStartNode_T, true); } else { start = int.Parse(txtStartNode_T.Text); }
                if (txtEndHighNode_T.Text == "") { ChangeTextBoxColor(txtEndHighNode_T, true); } else { end = int.Parse(txtEndHighNode_T.Text); }
                if (start == default(int) || end == default(int)) return;

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

                track.AddBranch(BranchChecker(new Branch(start: start, end: end, type: type,
                                       state: state, typename:typename, name: name, npar: npar,
                                       r: r, x: x, b: b, g: g,
                                       ktr: ktr, idd: idd, region: region)));

                Tab_Data.SelectedIndex = 1;
                return;
            }
            else if (cmbType_T.Text == "тр./АТ")
            {
                int start = default(int);
                int endH = default(int);
                int endM = default(int);
                int endL = default(int);
                if (txtStartNode_T.Text == "") { ChangeTextBoxColor(txtStartNode_T, true); } else { start = int.Parse(txtStartNode_T.Text); }
                if (txtEndHighNode_T.Text == "") { ChangeTextBoxColor(txtEndHighNode_T, true); } else { endH = int.Parse(txtEndHighNode_T.Text); }
                if (txtEndMidNode_T.Text == "") { ChangeTextBoxColor(txtEndMidNode_T, true); } else { endM = int.Parse(txtEndMidNode_T.Text); }
                if (txtEndLowNode_T.Text == "") { ChangeTextBoxColor(txtEndLowNode_T, true); } else { endL = int.Parse(txtEndLowNode_T.Text); }
                if (start == default(int) || endH == default(int) || endM == default(int) || endL == default(int)) return;

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

                track.AddBranch(BranchChecker(new Branch(start: start, end: endH, type: type,
                                       state: state, typename: typename, name: nameH, npar: npar,
                                       r: rH, x: xH, b: bH, g: gH,
                                       ktr: ktrH, idd: idd, region: region)));
                track.AddBranch(BranchChecker(new Branch(start: endH, end: endM, type: type,
                                       state: state, typename: typename, name: nameM, npar: npar,
                                       r: rM, x: xM, b: bM, g: gM,
                                       ktr: ktrM, idd: idd, region: region)));
                track.AddBranch(BranchChecker(new Branch(start: endH, end: endL, type: type,
                                       state: state, typename: typename, name: nameL, npar: npar,
                                       r: rL, x: xL, b: bL, g: gL,
                                       ktr: ktrL, idd: idd, region: region)));
                Tab_Data.SelectedIndex = 1;
                return;
            }
            
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
