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
using System.Windows.Shapes;
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

            DBProvider db_prv = new DBProvider("test.db");
            Status_Text.Text = "Состояние подключения:   " + db_prv.Status;

            string str_com = "DROP TABLE IF EXISTS [workers];"
                                +"CREATE TABLE [workers] ("
                                +"[id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,"
                                + "[name] char(100) NOT NULL,"
                                + "[family] char(100) NOT NULL,"
                                + "[age] int NOT NULL,"
                                + "[profession] char(100) NOT NULL"
                                + "); ";

            db_prv.Command_NonQuery(str_com, db_prv.Connection);
        }
    }
}
