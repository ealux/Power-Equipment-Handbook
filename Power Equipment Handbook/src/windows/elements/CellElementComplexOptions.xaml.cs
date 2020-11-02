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
    /// <summary>
    /// Логика взаимодействия для CellElementComplexOptions.xaml
    /// </summary>
    public partial class CellElementComplexOptions : Window
    {
        private CellElementsOptions opt;
        public CellElementsOptions options { get=>opt; set => opt = value; }

        public CellElementComplexOptions(ref CellElementsOptions options)
        {
            InitializeComponent();
            this.DataContext = this;
            this.options = options;
        }
    }
}
