using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Library : Window
    {
        DBProvider db;

        public Library(DBProvider dbProvider)
        {
            InitializeComponent();
            this.db = dbProvider;

            LinesGrid.ItemsSource = db.Command_Query("Select * from [Lines]", db.Connection);
            TransGrid.ItemsSource = db.Command_Query("Select * from [Trans]", db.Connection);
            MTransGrid.ItemsSource = db.Command_Query("Select * from [Multitrans]", db.Connection);
        }

        /// <summary>
        /// Блокирует закрытие окна Библиотеки Оборудования
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
