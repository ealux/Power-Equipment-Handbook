using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
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
            DotCommaReplacer(sender: sender, e: e);
            if (txtLength_L.Text == "") return;

            var use = (TextBox)sender;

            System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            double L = Convert.ToDouble(txtLength_L.Text, provider);

            if (use != txtLength_L)
            {
                double res;
                if (use.Text != "") res = Convert.ToDouble(use.Text, provider) * L;
                else return;

                if (use == txtr0_L) txtR_L.Text = res.ToString();
                if (use == txtx0_L) txtX_L.Text = res.ToString();
                if (use == txtb0_L) txtB_L.Text = res.ToString();
                if (use == txtg0_L) txtG_L.Text = res.ToString();
            }
            else if(use == txtLength_L)
            {
                if(txtr0_L.Text != "") txtR_L.Text = (Convert.ToDouble(txtr0_L.Text, provider) * L).ToString();
                if (txtx0_L.Text != "") txtX_L.Text = (Convert.ToDouble(txtx0_L.Text, provider) * L).ToString();
                if (txtb0_L.Text != "") txtB_L.Text = (Convert.ToDouble(txtb0_L.Text, provider) * L).ToString();
                if (txtg0_L.Text != "") txtG_L.Text = (Convert.ToDouble(txtg0_L.Text, provider) * L).ToString();
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
                grpMid.Visibility = grpLow.Visibility = Visibility.Visible;
                lblKH_KML_T.Content = "Kтр В(Ктр С-Н)";
                lblEndHighNode_T.Content = "Ср. точка";
                elUnom_T_2.Visibility = lblUnomLowDouble_T.Visibility = txtUnomLowDouble_T.Visibility = Visibility.Hidden;
                elUnom_T_1.Width = 65;
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
