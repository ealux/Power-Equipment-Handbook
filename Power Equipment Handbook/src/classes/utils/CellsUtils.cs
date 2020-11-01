using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using Power_Equipment_Handbook.src.windows;

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

        [XmlIgnore] public int ElemsCount { get => CellElements.Count; }

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

        [XmlAttribute] public override string Type { get; } = "Выкл.";

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

        /// <summary>
        /// Возвращает полное наименование типа для сериализации
        /// </summary>
        public override string SerializeType() => "Выключатель";
    }

    /// <summary>
    /// Разъединитель ячейки
    /// </summary>
    public class DisconnectorCell : Element
    {
        [XmlAttribute] public override string Type { get; } = "Разъед.";

        public DisconnectorCell() { }
        public DisconnectorCell(string name, double? inom, double unom,
                                double? iotkl, double? iterm, double? iudar,
                                double? tterm, double? bterm) : base(name, inom, unom, iotkl, iterm, iudar, tterm, bterm) { }

        /// <summary>
        /// Возвращает полное наименование типа для сериализации
        /// </summary>
        public override string SerializeType() => "Разъединитель";
    }

    /// <summary>
    /// Отделитель/Короткозамыкатель ячейки
    /// </summary>
    public class ShortCircuiterCell : Element
    {
        double? totkl;

        [XmlAttribute] public override string Type { get; } = "От./КЗ";
        [XmlIgnore] public double? Totkl { get => totkl; set => SetProperty(ref totkl, value); }

        #region Properties to Text (serialize)

        [XmlElement("Totkl")] public string TotklAsText { get => (Totkl.HasValue) ? Inom.ToString() : null; set => Totkl = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }

        #endregion Properties to Text (serialize)

        public ShortCircuiterCell() { }
        public ShortCircuiterCell(double? totkl, //special
                                  string name, double? inom, double unom,
                                  double? iotkl, double? iterm, double? iudar,
                                  double? tterm, double? bterm) : base(name, inom, unom, iotkl, iterm, iudar, tterm, bterm) => this.totkl = totkl;

        /// <summary>
        /// Возвращает полное наименование типа для сериализации
        /// </summary>
        public override string SerializeType() => "Отдел./КЗ";
    }

    /// <summary>
    /// Трансформатор тока ячейки
    /// </summary>
    public class TTCell : Element
    {
        int iperv, ivtor;

        [XmlAttribute] public override string Type { get; } = "ТТ";
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

        /// <summary>
        /// Возвращает полное наименование типа для сериализации
        /// </summary>
        public override string SerializeType() => "ТТ";

    }

    /// <summary>
    /// Ошиновка ячейки
    /// </summary>
    public class BusbarCell : Element  //Допилить (ошиновка)
    {
        [XmlAttribute] public override string Type { get; } = "Ошин.";
        public BusbarCell() { }
        public BusbarCell(string name, double? inom, double unom,
                                  double? iotkl, double? iterm, double? iudar,
                                  double? tterm, double? bterm) : base(name, inom, unom, iotkl, iterm, iudar, tterm, bterm) { }

        /// <summary>
        /// Возвращает полное наименование типа для сериализации
        /// </summary>
        public override string SerializeType() => "Ошиновка";
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
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {                
                if (e.AddedItems.Count == 0) return;

                try { var t = (Node)e.AddedItems[0]; }
                catch (Exception) { return; }

                if (txtStartNode_C.SelectedIndex != -1)
                {
                    double unom = track.Nodes.Where(n => n.Number == ((Node)e.AddedItems[0]).Number).Select(n => n.Unom).First();
                    this.grdCommutation.ItemsSource = new ObservableCollection<Cell>(this.track.Cells.Where(c => c.NodeNumber == ((Node)e.AddedItems[0]).Number));
                    lblElementCell.Content = "";
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
                if (c != null)
                {
                    this.grdElements.ItemsSource = new ObservableCollection<Element>(c.CellElements);
                    lblElementCell.Content = String.Format("Узел: {0}\nЯчейка: \"{1}\", ", c.NodeNumber, c.Name);
                }
                
            }
            else if (grd == this.grdCells)
            {
                try { c = (Cell)this.grdCells.SelectedItem; }
                catch (Exception) { return; }
                if (c != null)
                {
                    txtStartNode_C.SelectedIndex = txtStartNode_C.Items.IndexOf(txtStartNode_C.Items.OfType<Node>().First(n => n.Number == c.NodeNumber));
                    this.grdCommutation.ItemsSource = new ObservableCollection<Cell>(this.track.Cells.Where(x => c.NodeNumber == x.NodeNumber));
                }                    
            }
        }

        /// <summary>
        /// Потеря фокуса и сокрытие деталей общей таблицы ячеек
        /// </summary>
        private void grdCells_LostFocus(object sender, RoutedEventArgs e)
        {
            this.track.grdCells.SelectedIndex = -1;
        }

        /// <summary>
        /// Обновить все датагриды (Ячейки - общий, ячейки, элементы)
        /// </summary>
        private void FullUpdate()
        {          
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {                
                if(txtStartNode_C.SelectedItem != null & txtStartNode_C.SelectedIndex != -1 & !String.IsNullOrEmpty(txtStartNode_C.Text)) 
                    this.grdCommutation.ItemsSource = new ObservableCollection<Cell>(this.track.Cells.Where(c => c.NodeNumber == Convert.ToInt32(txtStartNode_C.Text)));
                grdCommutation.UpdateLayout();      //Обновление grdCommunication

                grdElements.UpdateLayout();         //Обновление grdElements

                this.track.GenerateViewForCells();
                grdCells_LostFocus(grdCells, null); //Обновление grdCells
            });
        } 


        #region Ячейки

        /// <summary>
        /// Добавление ячейки 
        /// </summary>
        private void btnAddCell_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                if (txtStartNode_C.SelectedIndex != -1)
                {
                    int num, index;

                    try
                    {
                        num = ((Node)txtStartNode_C.SelectedItem).Number;
                        index = txtStartNode_C.SelectedIndex;
                    }
                    catch (Exception) { return; }

                    double unom = track.Nodes.Where(n => n.Number == num).Select(n => n.Unom).First();

                    this.track.Cells.Add(new Cell(num, $"_ячейка_узла_{num}", unom));

                    FullUpdate();

                    txtStartNode_C.SelectedIndex = -1;
                    txtStartNode_C.SelectedIndex = index;
                }
            });            
        }

        /// <summary>
        /// Удаление ячейки 
        /// </summary>
        private void btnRemoveCell_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                int index = txtStartNode_C.SelectedIndex;

                Cell row = (Cell)grdCommutation.SelectedItem;
                this.track.Cells.Remove(row);

                grdCommutation.UpdateLayout();

                FullUpdate();

                txtStartNode_C.SelectedIndex = -1;
                txtStartNode_C.SelectedIndex = index;
            });
        }

        #endregion Ячейки



        #region Элементы

        /// <summary>
        /// Добавление оборудования. Пустой элемент 
        /// </summary>
        private void btnAddElemString_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                if ((Cell)grdCommutation.SelectedItem != null & grdCommutation.SelectedIndex != -1)
                {
                    int index_comm = grdCommutation.SelectedIndex;
                    int index_txt = txtStartNode_C.SelectedIndex;
                    var tmp_cell = (Cell)grdCommutation.SelectedItem;

                    CellElementAddSimple cr = new CellElementAddSimple(ref tmp_cell);
                    cr.ShowDialog();

                    FullUpdate();

                    grdCommutation.SelectedIndex = -1;
                    txtStartNode_C.SelectedIndex = -1;
                    txtStartNode_C.SelectedIndex = index_txt;
                    grdCommutation.SelectedIndex = index_comm;
                }
            });
        }

        /// <summary>
        /// Добавление оборудования. Комплект пустых элементов (5шт - выкл., разъед., отд./кз., тт, ошиновка)
        /// </summary>
        private void btnAddElemComplex_Click(object sender, RoutedEventArgs e)
        {            
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                if ((Cell)grdCommutation.SelectedItem != null & grdCommutation.SelectedIndex != -1)
                {
                    int index_comm = grdCommutation.SelectedIndex;
                    int index_txt = txtStartNode_C.SelectedIndex;
                    var tmp_cell = (Cell)grdCommutation.SelectedItem;


                    var added_br = new BreakerCell(tvkl: null, totkl: null,
                                                name: "_Выключатель_ячейки_", inom: null, unom: tmp_cell.Unom,
                                                iotkl: null, iterm: null, iudar: null,
                                                tterm: null, bterm: null);

                    var added_discon = new DisconnectorCell(name: "_Разъединитель_ячейки_", inom: null, unom: tmp_cell.Unom,
                                                    iotkl: null, iterm: null, iudar: null,
                                                    tterm: null, bterm: null);

                    var added_SC = new ShortCircuiterCell(totkl: null,
                                                       name: "_Отдел./Короткозамык._ячейки_", inom: null, unom: tmp_cell.Unom,
                                                       iotkl: null, iterm: null, iudar: null,
                                                       tterm: null, bterm: null);

                    var added_TT = new TTCell(iperv: 0, ivtor: 0,
                                           name: "_Трансформатор_тока_ячейки_", inom: null, unom: tmp_cell.Unom,
                                           iotkl: null, iterm: null, iudar: null,
                                           tterm: null, bterm: null);

                    var added_bus = new BusbarCell(name: "_Ошиновка_ячейки_", inom: null, unom: tmp_cell.Unom,
                                               iotkl: null, iterm: null, iudar: null,
                                               tterm: null, bterm: null);

                    tmp_cell.CellElements.Add(added_br);
                    tmp_cell.CellElements.Add(added_discon);
                    tmp_cell.CellElements.Add(added_SC);
                    tmp_cell.CellElements.Add(added_TT);
                    tmp_cell.CellElements.Add(added_bus);


                    FullUpdate();

                    grdCommutation.SelectedIndex = -1;
                    txtStartNode_C.SelectedIndex = -1;
                    txtStartNode_C.SelectedIndex = index_txt;
                    grdCommutation.SelectedIndex = index_comm;
                }
            });
        }

        /// <summary>
        /// Удаление оборудования 
        /// </summary>
        private void btnRemoveElement_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                int index_comm = grdCommutation.SelectedIndex;
                int index_txt = txtStartNode_C.SelectedIndex;
                var tmp_cell = (Cell)grdCommutation.SelectedItem;

                Element row = (Element)grdElements.SelectedItem;
                tmp_cell.CellElements.Remove(row);

                grdCommutation.UpdateLayout();

                FullUpdate();

                grdCommutation.SelectedIndex = -1;
                txtStartNode_C.SelectedIndex = -1;
                txtStartNode_C.SelectedIndex = index_txt;
                grdCommutation.SelectedIndex = index_comm;
            });
        }
        #endregion Элементы
    }
}
