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

        private void Tab_Elements_SelectionChanged()
        {

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
