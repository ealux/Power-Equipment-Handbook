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
        readonly DBProvider db;

        public Library(DBProvider dbProvider)
        {
            InitializeComponent();
            this.db = dbProvider;

            LinesGrid.ItemsSource = db.Command_Query("Select * from [Lines]", db.Connection);
            TransGrid.ItemsSource = db.Command_Query("Select * from [Trans]", db.Connection);
            MTransGrid.ItemsSource = db.Command_Query("Select * from [Multitrans]", db.Connection);
            CablesGrid.ItemsSource = db.Command_Query("Select * from [Cables]", db.Connection);
            BreakersGrid.ItemsSource = db.Command_Query("Select * from [Breakers]", db.Connection);
            DisconnectorsGrid.ItemsSource = db.Command_Query("Select * from [Disconnector]", db.Connection);
            SCGrid.ItemsSource = db.Command_Query("Select * from [Short-circuiter]", db.Connection);
            TTGrid.ItemsSource = db.Command_Query("Select * from [TT]", db.Connection);
        }

        /// <summary>
        /// Блокирует закрытие окна Библиотеки Оборудования
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        /// <summary>
        /// Нажатие клавиши сброса сортировки/фильтрации
        /// </summary>
        private void btnUndoFilters_Click(object sender, RoutedEventArgs e)
        {
            switch (this.tabLib.SelectedIndex)
            {
                case 0:
                    LinesGrid.ItemsSource = db.Command_Query("Select * from [Lines]", db.Connection);
                    break;
                case 1:
                    CablesGrid.ItemsSource = db.Command_Query("Select * from [Cables]", db.Connection);
                    break;
                case 2:
                    TransGrid.ItemsSource = db.Command_Query("Select * from [Trans]", db.Connection);
                    break;
                case 3:
                    MTransGrid.ItemsSource = db.Command_Query("Select * from [Multitrans]", db.Connection);
                    break;
                case 4:
                    BreakersGrid.ItemsSource = db.Command_Query("Select * from [Breakers]", db.Connection);
                    break;
                case 5:
                    DisconnectorsGrid.ItemsSource = db.Command_Query("Select * from [Disconnector]", db.Connection);
                    break;
                case 6:
                    SCGrid.ItemsSource = db.Command_Query("Select * from [Short-circuiter]", db.Connection);
                    break;
                case 7:
                    TTGrid.ItemsSource = db.Command_Query("Select * from [TT]", db.Connection);
                    break;

            }        
        }
    }
}
