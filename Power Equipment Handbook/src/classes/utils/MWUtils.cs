using Power_Equipment_Handbook.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using Microsoft.Win32;

namespace Power_Equipment_Handbook
{
    /// <summary>
    /// Вспомогательные методы MainWindow
    /// </summary>
    public partial class MainWindow
    {

        #region Helpers

        /// <summary>
        /// Проверка ввода цифр для Номера Узла (вкладка узлы)
        /// </summary>
        private void DigitCheckerForNodes(object sender, TextCompositionEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl)) return; //Отлавливаем Ctrl
            ChangeTxtColor(sender, false);
            if(!char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        /// <summary>
        /// Проверка ввода вещественных чисел
        /// </summary>
        private void DoubleChecker(object sender, TextCompositionEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl)) return; //Отлавливаем Ctrl
            TextBox tb = (TextBox)sender;
            if((e.Text.Contains(".") || e.Text.Contains(",")) & (tb.Text.Contains(".") || tb.Text.Contains(","))) e.Handled = true;
            if(!(Char.IsDigit(e.Text, 0) | Char.IsPunctuation(e.Text, 0))) e.Handled = true;
        }

        /// <summary>
        /// Замена введенной запятой на точку
        /// </summary>
        private void DotCommaReplacer(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            using(tb.DeclareChangeBlock())
            {
                foreach(var c in e.Changes)
                {
                    if(c.AddedLength == 0) continue;
                    tb.Select(c.Offset, c.AddedLength);
                    if(tb.SelectedText.Contains(',')) tb.SelectedText = tb.SelectedText.Replace(',', '.');
                    tb.Select(c.Offset + c.AddedLength, 0);
                }
            }
        }

        /// <summary>
        /// Изменяет цвет TextBox'a в зависимости от аргумента error метода
        /// </summary>
        private void ChangeTxtColor(object sender, bool error)
        {
            Application.Current.Dispatcher?.Invoke(delegate()
            {
                TextBox tb = (TextBox)sender;
                if(error) tb.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else { tb.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); }
            });
        }

        /// <summary>
        /// Изменяет цвет ComboBox'a в зависимости от аргумента error метода
        /// </summary>
        private void ChangeCmbColor(object sender, bool error)
        {
            Application.Current.Dispatcher?.Invoke(delegate ()
            {
                ComboBox tb = (ComboBox)sender;
                if(error) tb.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else { tb.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); }
            });
        }

        /// <summary>
        /// Проверяет наличие аналогичной ветви и смещает номер параллельности в случае обнаружения
        /// </summary>
        private bool BranchChecker(Branch br, object cmbStart, object cmbEnd)
        {
            bool result = NodeChecker(br, cmbStart, cmbEnd);
            Application.Current.Dispatcher?.Invoke(delegate ()
            {
                if(result)
                {
                    var other = track.Branches.Where((b) => ((b.Start == br.Start & b.End == br.End) || (b.Start == br.End & b.End == br.Start)) & (b.Type != br.Type)).ToList();
                    if(other.Count > 0)
                    {
                        Log.Show("Узлы начала и конца совпадают для ветви другого типа!");
                        result = false;
                        return;
                    }

                    other = track.Branches.Where((b) => b.Equals(br)).OrderByDescending((b) => b.Npar).ToList();
                    if (!(other.Count > 0)) return;
                    Log.Show("Добавлена паралельная ветвь", LogClass.LogType.Information);
                    br.Npar = other[0].Npar + 1;
                }
            });
            return result;
        }

        /// <summary>
        /// Проверка наличия узлов по параметрам ввода начала и конца ветви
        /// </summary>
        private bool NodeChecker(Branch br, object cmbStart, object cmbEnd)
        {
            int start = br.Start;
            int end = br.End;

            var nodesSt = track.Nodes.Where((n) => n.Number == start).ToList();
            var nodesEn = track.Nodes.Where((n) => n.Number == end).ToList();
            var reverse_br = track.Branches.Where((b) => (b.Start == br.End) & (b.End == br.Start)).ToList();

            if(reverse_br.Count != 0) { ChangeCmbColor(cmbStart, true); ChangeCmbColor(cmbEnd, true); Log.Show("Проверить начало и конец ветви! Уже существует ветвь с заданными узлами!"); return false; }
            if(nodesSt.Count == 0) ChangeCmbColor(cmbStart, true);
            if(nodesEn.Count == 0) ChangeCmbColor(cmbEnd, true);

            if(nodesSt.Count == 0 || nodesEn.Count == 0) return false;
            else return true;
        }

        /// <summary>
        /// Извлечение данных из базы
        /// </summary>
        /// <param name="type">Тип элемента ("Line" или "Trans")</param>
        /// <param name="unom">Номинальное напряжение для отбора</param>
        /// <param name="provider">Провайдер БД</param>
        private void GetData(string type, double unom, DBProvider provider)
        {
            //Таблица Lines
            if(type == "Line")
            {
                Lines.Clear();
                string comm;

                if (isCable == true) comm = $"SELECT * FROM [Cables] WHERE [Unom] = {unom}".Replace(',','.');
                else comm = $"SELECT * FROM [Lines] WHERE [Unom] = {unom}".Replace(',', '.');

                using (var sqldata = provider.Command_Query(comm, provider.Connection))
                {
                    if(sqldata.HasRows == false) return;

                    while(sqldata.Read())
                    {
                        Lines.Add(new Line()
                        {
                            Unom = sqldata["Unom"] as double?,
                            TypeName = sqldata["Марка"] as string,
                            R0 = sqldata["R0"] as double?,
                            X0 = sqldata["X0"] as double?,
                            B0 = sqldata["B0"] as double?,
                            G0 = sqldata["G0"] as double?,
                            Idd = sqldata["Iдд"] as double?,
                            Source = sqldata["Источник"] as string
                        });
                    }
                }
                cmbTypeName_L.ItemsSource = Lines;
                cmbTypeName_L.DisplayMemberPath = "TypeName";
            }
            //Таблица Trans
            if(type == "Trans")
            {
                //Двухобмоточные
                if(cmbType_T.Text == "двух." & cmbType_T.SelectedIndex == 0)
                {
                    Trans.Clear();
                    string comm = $"SELECT * FROM [Trans] WHERE [Unom] = {unom}".Replace(',', '.');

                    using(var sqldata = provider.Command_Query(comm, provider.Connection))
                    {
                        if(sqldata.HasRows == false) return;

                        while(sqldata.Read())
                        {
                            var snom_loc = sqldata["Snom"] as double?;
                            var unom_loc = sqldata["UnomH"] as double?;

                            Trans.Add(new Trans()
                            {
                                Unom = sqldata["Unom"] as double?,
                                TypeName = sqldata["Марка"] as string,
                                UnomH = sqldata["UnomH"] as double?,
                                UnomL = sqldata["UnomL"] as double?,
                                R = sqldata["R"] as double?,
                                X = sqldata["X"] as double?,
                                B = sqldata["B"] as double?,
                                G = sqldata["G"] as double?,
                                Source = sqldata["Источник"] as string,
                                Inom = (snom_loc / unom_loc / Math.Sqrt(3)).HasValue ? (double?)Math.Round((double)(snom_loc / unom_loc / Math.Sqrt(3)), 2) : null
                            });
                        }
                    }

                    cmbTypeName_T.ItemsSource = Trans;
                    cmbTypeName_T.DisplayMemberPath = "TypeName";
                }
                //Трехобмоточные и Автотрансформаторы
                if(cmbType_T.Text == "тр./АТ" & cmbType_T.SelectedIndex == 1)
                {
                    MultiTrans.Clear();
                    string comm = $"SELECT * FROM [MultiTrans] WHERE [Unom] = {unom}".Replace(',', '.');

                    using(var sqldata = provider.Command_Query(comm, provider.Connection))
                    {
                        if(sqldata.HasRows == false) return;

                        while(sqldata.Read())
                        {
                            var snom_loc = sqldata["Snom"] as double?;
                            var unomh_loc = sqldata["UnomH"] as double?;
                            var unomm_loc = sqldata["UnomM"] as double?;
                            var unoml_loc = sqldata["UnomL"] as double?;

                            MultiTrans.Add(new MultiTrans()
                            {
                                Unom = sqldata["Unom"] as double?,
                                TypeName = sqldata["Марка"] as string,
                                UnomH = sqldata["UnomH"] as double?,
                                UnomM = sqldata["UnomM"] as double?,
                                UnomL = sqldata["UnomL"] as double?,
                                RH = sqldata["RH"] as double?,
                                RM = sqldata["RM"] as double?,
                                RL = sqldata["RL"] as double?,
                                XH = sqldata["XH"] as double?,
                                XM = sqldata["XM"] as double?,
                                XL = sqldata["XL"] as double?,
                                B = sqldata["B"] as double?,
                                G = sqldata["G"] as double?,
                                Source = sqldata["Source"] as string,
                                Inomh = (snom_loc / unomh_loc / Math.Sqrt(3)).HasValue ? (double?)Math.Round((double)(snom_loc / unomh_loc / Math.Sqrt(3)), 2) : null,
                                Inomm = (snom_loc / unomm_loc / Math.Sqrt(3)).HasValue ? (double?)Math.Round((double)(snom_loc / unomm_loc / Math.Sqrt(3)), 2) : null,
                                Inoml = (snom_loc / unoml_loc / Math.Sqrt(3)).HasValue ? (double?)Math.Round((double)(snom_loc / unoml_loc / Math.Sqrt(3)), 2) : null
                            });
                        }
                    }

                    cmbTypeName_T.ItemsSource = MultiTrans;
                    cmbTypeName_T.DisplayMemberPath = "TypeName";
                }
            }
        }

        /// <summary>
        /// Перекрашивает в белый цвет поля узлов при выпадении списка
        /// </summary>
        private void DropDownNodeReflector(object sender, EventArgs e)
        {
            ChangeCmbColor(sender, false);
            Log.Clear();
        }




        /// <summary>
        /// Отображение окна Бибилиотеки оборудования
        /// </summary>
        private void Get_Library(object sender, RoutedEventArgs e)
        {
            if(lib != null)
            { 
                if(lib.IsVisible) lib.Hide();
                else lib.Show();
            }
            else if (this.lib == null || !this.lib.IsVisible)
            {
                this.lib = new src.windows.Library(this.db_prv) { Owner = this };
                this.lib.Show();
            }
        }

        /// <summary>
        /// Отображение окна Связного списка узлов
        /// </summary>
        private void Get_LinkedNodes(object sender, RoutedEventArgs e)
        {
            if (lnodes != null)
            {
                if (lnodes.IsVisible) lnodes.Hide();
                else lnodes.Show();
            }
            else if (this.lnodes == null || !this.lnodes.IsVisible)
            {
                this.lnodes = new src.windows.LinkedNodes(this.track) { Owner = this };
                this.lnodes.Show();
            }
        }

        /// <summary>
        /// Отбражение окна менеджера режимов
        /// </summary>
        private void Get_PowerFlowManager(object sender, RoutedEventArgs e)
        {
            if (PFM != null)
            {
                if (PFM.IsVisible) PFM.Hide();
                else PFM.Show();
            }
            else if (this.PFM == null || !this.PFM.IsVisible)
            {
                this.PFM = new src.windows.PowerFlowManager(ref this.track.Nodes, ref this.track.Branches) { Owner = this };
                this.PFM.Show();
            }
        }

        #endregion Helpers

        #region Утилиты

        /// <summary>
        /// Отчистка полей узлов
        /// </summary>
        private void ClearNodes()
        {
            txtType_N.SelectedIndex = 0;
            txtName_N.Text = "";
            txtPn_N.Text = "";
            txtQn_N.Text = "";
            txtPg_N.Text = "";
            txtQg_N.Text = "";
            txtVzd_N.Text = "";
            txtQmin_N.Text = "";
            txtQmax_N.Text = "";
            txtBsh_N.Text = "";
            txtRegion_N.Text = "";
            txtState_N.Text = "0";
        }

        /// <summary>
        /// Сброс сортировки
        /// </summary>
        private void btnUndoFilters_Click(object sender, RoutedEventArgs e)
        {
            switch (this.Tab_Data.SelectedIndex)
            {
                case 0:
                    this.track.grdNodes.ItemsSource = null;
                    this.track.grdNodes.ItemsSource = this.track.Nodes;
                    break;
                case 1:
                    this.track.grdBranches.ItemsSource = null;
                    this.track.grdBranches.ItemsSource = this.track.Branches;
                    break;
                case 2:
                    this.track.GenerateViewForCells();
                    break;
            }
        }

        #endregion Утилиты        

        #region Power Network Methods

        /// <summary>
        /// Проверка связности схемы
        /// </summary>
        public void Connectivity(object sender, RoutedEventArgs e)
        {
            if(track.Nodes.Count == 0 || track.Branches.Count == 0)                                                 //Отсутствие узлов или ветвей
            {
                Log.Show("Отсутствуют узлы/ветви в схеме для проверки связанности!", LogClass.LogType.Error);
                return;
            }
                                                
            var branches = track.Branches.Distinct(new BranchEqualityComparer()).OrderBy(b=>b.Start).ToList();      //Список уникальных ветвей

            var exNodes = track.Nodes.OrderBy(n => n.Number).Select(n => n.Number).ToList();                        //Список узлов для исключения

            if (!branches.Any(b => (b.Start == exNodes[0]) | (b.End == exNodes[0])))                                //Отлов узлов-сирот (для первого узла)
            {
                Log.Show("Связанность отсутствует!", LogClass.LogType.Error);
                return;
            }                   

            List<int> linked = new List<int>();

            foreach(var b in branches) { if(b.Start == exNodes[0]) linked.Add(b.End); else if(b.End == exNodes[0]) linked.Add(b.Start); } //Формирование списка связности для первого узла

            exNodes.Remove(exNodes[0]); //Исключение первого узла из списка узлов (список - индикатор)

            RecurseFinder(linked, ref exNodes);

            if (exNodes.Count == 0)
            {
                Log.Show("Сеть является связанной", LogClass.LogType.Information);
                return;
            }

            Log.Show("Связанность отсутствует!", LogClass.LogType.Error);

            void RecurseFinder(List<int> Linked, ref List<int> Exnodes) //Рекурсивная функция обхода графа в глубину
            {
                foreach(int j in Linked)
                {
                    if(Exnodes.Contains(j)) Exnodes.Remove(j);
                }

                if(Exnodes.Count == 0) return;

                foreach(int link in Linked)
                {
                    var linker = new List<int>();

                    foreach(var b in branches)
                    {
                        if(b.Start == link & Exnodes.Contains(b.End)) linker.Add(b.End);
                        else if(b.End == link & Exnodes.Contains(b.Start)) linker.Add(b.Start);
                    }

                    if(linker.Count != 0) RecurseFinder(new List<int>(linker), ref Exnodes);
                }
            }
        }


        public void ShowLinkedNodes(object sender, RoutedEventArgs e)
        {

        }

        #endregion Power Network Methods

        #region Save/Open (Serialize/Deserialize) Methods

        /// <summary>
        /// Выгрузка данных
        /// </summary>
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if(track.Nodes.Count == 0 | track.Branches.Count == 0)
            {
                Log.Show("Отсутствуют узлы/ветви!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PEH files|*.peh| Excel (*.xlsx)|*.xlsx",       // | Rastr_rgm (*.rg2)|*.rg2",
                OverwritePrompt = true,
                AddExtension = true
                //InitialDirectory = "./Save/", 
            };

            if(sfd.ShowDialog() == false) return;

            string filename = sfd.FileName;                 //Получение абсолютного пути к сохраняемому файлу
            string extension = Path.GetExtension(filename); //получения расширения файла для выбора типа сериализатора

            UniverseSerializator serializator = new UniverseSerializator(file: filename, tracker: track);       //Сериализатор

            switch(extension)
            {
                case ".peh":
                    Log.Show($"Запись данных в файла: {filename}. Процесс...", LogClass.LogType.Information);
                    serializator.toXML();
                    break;
                case ".xlsx":
                    Log.Show($"Запись данных в файла: {filename}. Процесс...", LogClass.LogType.Information);
                    serializator.toXLSX();
                    break;
                
                //case ".rg2":
                //    serializator.toRG2();
                //    break;
            }

            Log.Show($"Успешно записано в файл: {filename}", LogClass.LogType.Information); //Информирует об записи в файл
        }

        /// <summary>
        /// Загрузка данных
        /// </summary>
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog
            {
                Filter = "PEH Files|*.peh",
                Multiselect = false
            };

            if(sfd.ShowDialog() == false) return;

            string filename = sfd.FileName;                 //Получение абсолютного пути к сохраняемому файлу
            string extension = Path.GetExtension(filename); //получения расширения файла для выбора типа сериализатора

            UniverseSerializator serializator = new UniverseSerializator(file: filename, tracker: track);       //Сериализатор
            DataGridTracker localTracker;

            switch(extension.ToLower())
            {
                case ".peh":
                    try
                    {
                        localTracker = serializator.fromXML();

                        track.Nodes.Clear(); track.Branches.Clear(); track.Cells.Clear();                     //Очистка старых значений в колекциях Узлов, Ветвей, Ячеек
                        this.grdCommutation.ItemsSource = null; this.grdElements.ItemsSource = null;

                        Log.Show($"Извлечение данных из файла: {filename}. Процесс...", LogClass.LogType.Information);

                        foreach(var i in localTracker.Nodes) track.Nodes.Add(i);        //Добавление Узлов 
                        foreach(var i in localTracker.Branches) track.Branches.Add(i);  //Добавление Ветвей
                        foreach (var i in localTracker.Cells) track.Cells.Add(i);       //Добавление Ячеек

                        FullUpdate();
                    }
                    catch (Exception)
                    {
                        Log.Show($"Ошибка чтения файла: {filename}"); //Информирует об ошибке импорта
                        return;
                    }
                    break;

                    //case ".rg2":
                    //    serializator.toRG2();
                    //    break;
            }

            Log.Show($"Успешно прочитан файл {filename}", LogClass.LogType.Information); //Информирует об успешном чтении файла
        }


        /// <summary>
        /// Выгрузка и структурирование данных по Оборудованию и Ячейкам в xlsx документ
        /// </summary>
        private void EquipmentSerialize_Click(object sender, RoutedEventArgs e)
        {
            if (track.Nodes.Count == 0) //Условие наличия узлов
            {
                Log.Show("Отсутствуют узлы!");
                return;
            }
            if (track.Cells.Count == 0) //Условие наличия узлов ячеек
            {
                Log.Show("Отсутствуют ячейки оборудования!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",       // | Rastr_rgm (*.rg2)|*.rg2",
                OverwritePrompt = true,
                AddExtension = true
                //InitialDirectory = "./Save/", 
            };

            if (sfd.ShowDialog() == false) return;

            string filename = sfd.FileName;                 //Получение абсолютного пути к сохраняемому файлу

            UniverseSerializator serializator = new UniverseSerializator(file: filename, tracker: track);       //Сериализатор

            Log.Show($"Запись данных в файла: {filename}. Процесс...", LogClass.LogType.Information);
            serializator.EquipmentToXLSX();

            Log.Show($"Успешно записано в файл: {filename}", LogClass.LogType.Information); //Информирует об записи в файл
        }

        #endregion Save/Open (Serialize/Deserialize) Methods
    }
}
