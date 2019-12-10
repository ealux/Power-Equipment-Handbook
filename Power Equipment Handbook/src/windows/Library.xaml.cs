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
using System.Windows.Shapes;

namespace Power_Equipment_Handbook.src.windows
{
    public partial class Library : Window
    {
        DBProvider db;

        public Library(DBProvider dBProvider)
        {
            InitializeComponent();
            this.db = dBProvider;

            string q_lines = "Select * from [Lines]";
            string q_trans = "Select * from [Trans]";
            string q_mtrans = "Select * from [Multitrans]";

            var lines = db.Command_Query(q_lines, db.Connection);
            var trans = db.Command_Query(q_trans, db.Connection);
            var mtrans = db.Command_Query(q_mtrans, db.Connection);

            LinesGrid.ItemsSource = lines;
            TransGrid.ItemsSource = trans;
            MTransGrid.ItemsSource = mtrans;
        }
    }
}
