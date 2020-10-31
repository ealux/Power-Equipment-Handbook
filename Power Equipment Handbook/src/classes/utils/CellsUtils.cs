using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Serialization;
using Power_Equipment_Handbook.src;

namespace Power_Equipment_Handbook
{

    /// <summary>
    /// Класс ячейки, присоединяемой к узлу
    /// </summary>
    /// <typeparam name="T">Тип элемента ячейки</typeparam>
    [Serializable]
    public class Cell : INotifyPropertyChanged
    {
        int nodenumber;
        string name;
        double unom;
        //int elemscount;


        [XmlAttribute] public int NodeNumber { get => nodenumber; set => SetProperty(ref nodenumber, value); }
        [XmlAttribute] public string Name { get => name; set => SetProperty(ref name, value); }
        [XmlAttribute] public double Unom { get => unom; set => SetProperty(ref unom, value); }

        [XmlIgnore] public int ElemsCount { get => CellElements.Count; } // set => SetProperty(ref unom, CellElements.Count); }

        [XmlArrayItem("CellElements", Type = typeof(Element))] public ObservableCollection<Element> CellElements { get; set; } = new ObservableCollection<Element>();


        public Cell() { }
        public Cell(int nodeNumber, string name, double unom)
        {
            NodeNumber = nodeNumber;
            Name = name;
            Unom = unom;
        }

        #region INotifyPropertyChanged interface block

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value; OnPropertyChanged(propertyName); return true;
        }

        #endregion INotifyPropertyChanged interface block
    }


    /// <summary>
    /// Выключатель ячейки
    /// </summary>
    public class BreakerCell: Element
    {
        double? tvkl, totkl;

        [XmlAttribute] public string Type { get; } = "Выкл.";

        [XmlIgnore] public double? Tvkl { get => tvkl; set => SetProperty(ref tvkl, value); }
        [XmlIgnore] public double? Totkl { get => totkl; set => SetProperty(ref totkl, value); }

        #region Properties to Text (serialize)

        [XmlElement("Tvkl")] public string TvklAsText { get => (Tvkl.HasValue) ? Inom.ToString() : null; set => Tvkl = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }
        [XmlElement("Totkl")] public string TotklAsText { get => (Totkl.HasValue) ? Inom.ToString() : null; set => Totkl = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }

        #endregion Properties to Text (serialize)

        public BreakerCell() { }
        public BreakerCell(double? tvkl, double? totkl,
                           string name, double? inom, double unom,
                           double? iotkl, double? iterm, double? iudar,
                           double? tterm, double? bterm) : base(name, inom, unom, iotkl, iterm, iudar, tterm, bterm)
        {
            this.tvkl = tvkl;
            this.totkl = totkl;
        }
    }

    /// <summary>
    /// Разъединитель ячейки
    /// </summary>
    public class DisconnectorCell : Element
    {
        [XmlAttribute] public string Type { get; } = "Разъед.";

        public DisconnectorCell() { }
        public DisconnectorCell(string name, double? inom, double unom,
                                double? iotkl, double? iterm, double? iudar,
                                double? tterm, double? bterm) : base(name, inom, unom, iotkl, iterm, iudar, tterm, bterm) { }
    }

    /// <summary>
    /// Отделитель/Короткозамыкатель ячейки
    /// </summary>
    public class ShortCircuiterCell : Element
    {
        double? totkl;

        [XmlAttribute] public string Type { get; } = "От./КЗ";
        [XmlIgnore] public double? Totkl { get => totkl; set => SetProperty(ref totkl, value); }

        #region Properties to Text (serialize)

        [XmlElement("Totkl")] public string TotklAsText { get => (Totkl.HasValue) ? Inom.ToString() : null; set => Totkl = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }

        #endregion Properties to Text (serialize)

        public ShortCircuiterCell() { }
        public ShortCircuiterCell(double? totkl, //special
                                  string name, double? inom, double unom,
                                  double? iotkl, double? iterm, double? iudar,
                                  double? tterm, double? bterm) : base(name, inom, unom, iotkl, iterm, iudar, tterm, bterm) => this.totkl = totkl;
    }

    /// <summary>
    /// Трансформатор тока ячейки
    /// </summary>
    public class TTCell : Element
    {
        int iperv, ivtor;

        [XmlAttribute] public string Type { get; } = "ТТ";
        [XmlAttribute] public int Iperv { get => iperv; set => SetProperty(ref iperv, value); }
        [XmlAttribute] public int Ivtor { get => ivtor; set => SetProperty(ref ivtor, value); }

        public TTCell() { }
        public TTCell(int iperv, int ivtor, //special
                      string name, double? inom, double unom,
                                  double? iotkl, double? iterm, double? iudar,
                                  double? tterm, double? bterm) : base(name, inom, unom, iotkl, iterm, iudar, tterm, bterm)
        {
            this.iperv = iperv;
            this.ivtor = ivtor;
        }

    }

    /// <summary>
    /// Ошиновка ячейки
    /// </summary>
    public class BusbarCell : Element  //Допилить (ошиновка)
    {
        [XmlAttribute] public string Type { get; } = "Ошин.";
        public BusbarCell() { }
        public BusbarCell(string name, double? inom, double unom,
                                  double? iotkl, double? iterm, double? iudar,
                                  double? tterm, double? bterm) : base(name, inom, unom, iotkl, iterm, iudar, tterm, bterm) { }
    }




    /// <summary>
    /// Вспомогательные методы MainWindow для Ячеек
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Выбор узла привязки для Ячейки
        /// </summary>
        private void TxtStartNode_C_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.track.Cells.Add(new Cell(1, "ф.23", 6.3));
            this.track.Cells.Add(new Cell(2, "ф.24", 6.3));

            this.track.Cells[0].CellElements.Add(new BreakerCell(0.1, 0.1, "VD4-12", null, 0.1, null, null, null, null, null));
            this.track.Cells[0].CellElements.Add(new DisconnectorCell("РВЗ-14-47-89 1а УХЛЗ2", null, 0.1, null, null, null, null, null));
            this.track.Cells[0].CellElements.Add(new ShortCircuiterCell(0.1, "РВЗ-14-47-89 1а УХЛЗ2 РВЗ-14-47-89 1а УХЛЗ2", null, 0.1, null, null, null, null, null));

            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {                
                if (e.AddedItems.Count == 0) return;

                try { var t = (Node)e.AddedItems[0]; }
                catch (Exception) { return; }

                if (txtStartNode_C.SelectedIndex != -1)
                {
                    double unom = track.Nodes.Where(n => n.Number == ((Node)e.AddedItems[0]).Number).Select(n => n.Unom).First();
                    this.grdCommutation.ItemsSource = new ObservableCollection<Cell>(this.track.Cells.Where(c => c.NodeNumber == ((Node)e.AddedItems[0]).Number));
                    foreach (ListBoxItem i in cmbUnom_C.Items)
                    {
                        if (double.Parse(i.Content.ToString(), System.Globalization.CultureInfo.InvariantCulture) == unom)
                        {
                            cmbUnom_C.SelectedItem = i;
                            return;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Вывод подробностей состава ячейки при выборе ячейки в общем списке ячеек или в меню редактирования ячеек (коммутация)
        /// </summary>
        private void grdCommutation_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var grd = (DataGrid)sender;
            Cell c;

            if (grd == this.grdCommutation)
            {                
                try { c = (Cell)this.grdCommutation.SelectedItem; }
                catch (Exception) { return; }
                if (c != null) this.grdElements.ItemsSource = new ObservableCollection<Element>(c.CellElements);
            }
            else if (grd == this.grdCells)
            {
                try { c = (Cell)this.grdCells.SelectedItem; }
                catch (Exception) { return; }
                if (c != null) this.grdCommutation.ItemsSource = new ObservableCollection<Cell>(this.track.Cells.Where(x => c.NodeNumber == x.NodeNumber));
            }
        }

        /// <summary>
        /// Потеря фокуса и сокрытие деталей общей таблицы ячеек
        /// </summary>
        private void grdCells_LostFocus(object sender, RoutedEventArgs e)
        {
            this.track.grdCells.SelectedIndex = -1;
        }

    }
}
