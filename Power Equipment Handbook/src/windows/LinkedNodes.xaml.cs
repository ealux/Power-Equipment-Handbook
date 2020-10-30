using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для LinkedNodes.xaml
    /// </summary>
    public partial class LinkedNodes : Window
    {

        ObservableCollection<Node> localNodes;
        private int Count { get; set; }
        //ObservableCollection<Branch> localBranches { get; set; }

        //View-элемент для списка узлов
        CollectionViewSource nodesView { get; set; }

        /// <summary>
        /// Генерация View-элемента для отображения в datagrid
        /// </summary>
        internal void GenerateView()
        {
            this.nodesView = new CollectionViewSource();
            this.nodesView.Source = this.localNodes.OrderBy(n => n.Unom).ThenBy(n => n.Number);
            this.nodesView.GroupDescriptions.Add(new PropertyGroupDescription("Unom"));
        }


        public LinkedNodes(DataGridTracker dgt)
        {
            InitializeComponent();
            this.localNodes = dgt.Nodes;
            //this.localBranches = dgt.Branches;

            //Привязка узлов к датагриду
            //this.LinkedGrid.ItemsSource = localNodes;

            //Группировка по Unom
            Count = localNodes.Count;
            GenerateView();
            this.LinkedGrid.ItemsSource = nodesView.View;
            localNodes.CollectionChanged += LocalNodes_CollectionChanged;

        }

        private void LocalNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher?.Invoke(() => 
            {
                Window_IsVisibleChanged(this.LinkedGrid, new DependencyPropertyChangedEventArgs());
                Count = localNodes.Count;
            });            
        }

        /// <summary>
        /// Блокирует закрытие окна Связного списка узлов
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            nodesView = null;
            e.Cancel = true;
        }

        /// <summary>
        /// Открытие/закрытие окна списка узлов
        /// </summary>
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(this.LinkedGrid.IsVisible == true)
            {
                nodesView = new CollectionViewSource();
                nodesView.Source = this.localNodes.OrderBy(n => n.Unom).ThenBy(n => n.Number);
                nodesView.GroupDescriptions.Add(new PropertyGroupDescription("Unom"));
                this.LinkedGrid.ItemsSource = nodesView.View;
            }
            else
            {
                nodesView = null;
            }
            
        }

        private void TextBlock_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            this.Count = localNodes.Count;
        }

        /// <summary>
        /// Нажатие клавиши сброса сортировки/фильтрации
        /// </summary>
        private void btnUndoFilters_Click(object sender, RoutedEventArgs e)
        {
            GenerateView();
            this.LinkedGrid.ItemsSource = nodesView.View;
        }


        
    }
}
