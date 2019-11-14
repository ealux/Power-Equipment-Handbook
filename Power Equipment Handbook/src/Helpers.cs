using Power_Equipment_Handbook.src;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace Power_Equipment_Handbook
{
    /// <summary>
    /// Вспомогательные методы MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Helpers

        /// <summary>
        /// Проверка ввода цифр для Номера Узла (вкладка узлы)
        /// </summary>
        private void DigitCheckerForNodes(object sender, TextCompositionEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl)) return; //Отлавливаем Ctrl
            TextBox tb = (TextBox)sender;
            ChangeTxtColor(sender, false);
            if(!char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        /// <summary>
        /// Проверка ввода цифр
        /// </summary>
        private void DigitChecker(object sender, TextCompositionEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl)) return; //Отлавливаем Ctrl
            ComboBox tb = (ComboBox)sender;
            ChangeCmbColor(sender, false);
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
        /// Расчет параметров Линии по Длине и Удельным парамертрам (автоподстановка)
        /// </summary>
        private void Line_Calculations(object sender, TextChangedEventArgs e)
        {
            //TODO обработчики неверного заполнения (цвета)
            DotCommaReplacer(sender: sender, e: e); //Обработка запятых
            if(txtLength_L.Text == "") return;     //Проверка поля Длина

            var use = (TextBox)sender;
            if(use.Text.StartsWith(".")) return;

            NumberFormatInfo provider = new NumberFormatInfo() { NumberDecimalSeparator = "." };

            double L = Convert.ToDouble(txtLength_L.Text, provider);

            if(use != txtLength_L)
            {
                double res;
                if(use.Text != "") res = Convert.ToDouble(use.Text, provider) * L;
                else return;

                if(use == txtr0_L) txtR_L.Text = res.ToString(); if(use == txtx0_L) txtX_L.Text = res.ToString();
                if(use == txtb0_L) txtB_L.Text = res.ToString(); if(use == txtg0_L) txtG_L.Text = res.ToString();
            }
            else if(use == txtLength_L)
            {
                if(txtr0_L.Text != "") txtR_L.Text = (Convert.ToDouble(txtr0_L.Text, provider) * L).ToString();
                if(txtx0_L.Text != "") txtX_L.Text = (Convert.ToDouble(txtx0_L.Text, provider) * L).ToString();
                if(txtb0_L.Text != "") txtB_L.Text = (Convert.ToDouble(txtb0_L.Text, provider) * L).ToString();
                if(txtg0_L.Text != "") txtG_L.Text = (Convert.ToDouble(txtg0_L.Text, provider) * L).ToString();
            }
        }

        /// <summary>
        /// Расчет коэффициентов трансформации
        /// </summary>
        private void Ktr_Calculations(object sender, TextChangedEventArgs e)
        {
            //TODO обработчики неверного заполнения(цвета)
            DotCommaReplacer(sender: sender, e: e); //Обработка запятых

            var HV = txtUnomHigh_T; var LV = txtUnomLowDouble_T;
            var MHV = txtUnomMid_T; var LHV = txtUnomLow_T;

            if(HV.Text.StartsWith(".")) return;
            if(cmbType_T.Text == "двух.")
            {
                if(!string.IsNullOrEmpty(HV.Text) & !string.IsNullOrEmpty(LV.Text))
                {
                    if(LV.Text.StartsWith(".")) return;
                    txtKH_KML_T.Text = (Convert.ToDouble(LV.Text, CultureInfo.InvariantCulture) / Convert.ToDouble(HV.Text, CultureInfo.InvariantCulture)).ToString();
                }
            }
            if(cmbType_T.Text == "тр./АТ")
            {
                txtKH_KML_T.Text = "1";

                if(!string.IsNullOrEmpty(HV.Text) & !string.IsNullOrEmpty(MHV.Text))
                {
                    if(MHV.Text.StartsWith(".")) return;
                    txtKHM_T.Text = (Convert.ToDouble(MHV.Text, CultureInfo.InvariantCulture) / Convert.ToDouble(HV.Text, CultureInfo.InvariantCulture)).ToString();
                }
                if(!string.IsNullOrEmpty(HV.Text) & !string.IsNullOrEmpty(LHV.Text))
                {
                    if(LHV.Text.StartsWith(".")) return;
                    txtKHL_T.Text = (Convert.ToDouble(LHV.Text, CultureInfo.InvariantCulture) / Convert.ToDouble(HV.Text, CultureInfo.InvariantCulture)).ToString();
                }
            }
        }

        /// <summary>
        /// Изменяет цвет TextBox'a в зависимости от аргумента error метода
        /// </summary>
        private void ChangeTxtColor(object sender, bool error)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                TextBox tb = (TextBox)sender;
                if(error == true) tb.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else { tb.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); }
            });
        }

        /// <summary>
        /// Изменяет цвет ComboBox'a в зависимости от аргумента error метода
        /// </summary>
        private void ChangeCmbColor(object sender, bool error)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                ComboBox tb = (ComboBox)sender;
                if(error == true) tb.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else { tb.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); }
            });
        }

        /// <summary>
        /// Проверяет наличие аналогичной ветви и смещает номер параллельности в случае обнаружения
        /// </summary>
        private bool BranchChecker(Branch br, object cmbStart, object cmbEnd)
        {
            bool result = NodeChecker(br, cmbStart, cmbEnd);
            if(result)
            {
                var other = track.Branches.Where((b) => ((b.Start == br.Start & b.End == br.End) || (b.Start == br.End & b.End == br.Start)) & (b.Type != br.Type)).ToList();
                if(other.Count > 0) { Log.Show("Узлы начала и конца совпадают для ветви другого типа!"); return false; }

                other = track.Branches.Where((b) => b.Equals(br)).OrderByDescending((b) => b.Npar).ToList();
                if(other != null && other.Count > 0) { Log.Show("Добавлена паралельная ветвь", LogClass.LogType.Information); ; br.Npar = other[0].Npar + 1; }
            }
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
        private void GetData(string type, int unom, DBProvider provider)
        {
            //Таблица Lines
            if(type == "Line")
            {
                Lines.Clear();

                using(var sqldata = provider.Command_Query(String.Format(@"SELECT * FROM [Lines] WHERE [Unom] = {0}", unom), provider.Connection))
                {
                    if(sqldata.HasRows == false) return;

                    while(sqldata.Read())
                    {
                        Lines.Add(new Line()
                        {
                            Unom = sqldata["Unom"] as int?,
                            TypeName = sqldata["TypeName"] as string,
                            R0 = sqldata["R0"] as double?,
                            X0 = sqldata["X0"] as double?,
                            B0 = sqldata["B0"] as double?,
                            G0 = sqldata["G0"] as double?,
                            Idd = sqldata["Idd"] as double?,
                            Source = sqldata["Source"] as string
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
                if(cmbType_T.Text == "двух.")
                {
                    Trans.Clear();

                    using(var sqldata = provider.Command_Query(String.Format(@"SELECT * FROM [Trans] WHERE [Unom] = {0}", unom), provider.Connection))
                    {
                        if(sqldata.HasRows == false) return;

                        while(sqldata.Read())
                        {
                            Trans.Add(new Trans()
                            {
                                Unom = sqldata["Unom"] as int?,
                                TypeName = sqldata["TypeName"] as string,
                                UnomH = sqldata["UnomH"] as double?,
                                UnomL = sqldata["UnomL"] as double?,
                                R = sqldata["R"] as double?,
                                X = sqldata["X"] as double?,
                                B = sqldata["B"] as double?,
                                G = sqldata["G"] as double?,
                                Source = sqldata["Source"] as string
                            });
                        }
                    }

                    cmbTypeName_T.ItemsSource = Trans;
                    cmbTypeName_T.DisplayMemberPath = "TypeName";
                }
                //Трехобмоточные и Автотрансформаторы
                if(cmbType_T.Text == "тр./АТ")
                {
                    MultiTrans.Clear();

                    using(var sqldata = provider.Command_Query(String.Format(@"SELECT * FROM [MultiTrans] WHERE [Unom] = {0}", unom), provider.Connection))
                    {
                        if(sqldata.HasRows == false) return;

                        while(sqldata.Read())
                        {
                            MultiTrans.Add(new MultiTrans()
                            {
                                Unom = sqldata["Unom"] as int?,
                                TypeName = sqldata["TypeName"] as string,
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
                                Source = sqldata["Source"] as string
                            });
                        }
                    }

                    cmbTypeName_T.ItemsSource = MultiTrans;
                    cmbTypeName_T.DisplayMemberPath = "TypeName";
                }
            }
        }

        /// <summary>
        /// Селектор узлов для трёхобмоточных трансов
        /// </summary>
        private void TransEndNodeSelector(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate ()
            {
                ComboBox tb = (ComboBox)sender;

                if(tb.Text == "" || tb.SelectedIndex == -1)
                {
                    cmbTypeName_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = MultiTrans });
                    cmbTypeName_T.DisplayMemberPath = "TypeName";
                }

                if(e.AddedItems.Count == 0) return; //Проверка селектора

                try { var t = (Node)e.AddedItems[0]; }
                catch(Exception) { return; }

                if(tb == txtEndMidNode_T)
                {
                    ObservableCollection<Node> l = new ObservableCollection<Node>();

                    foreach(Node i in txtStartNode_T.ItemsSource)
                    {
                        if(i.Number != ((Node)e.AddedItems[0]).Number &
                            i.Number != ((Node)txtStartNode_T.SelectedItem).Number &
                            i.Unom != ((Node)txtStartNode_T.SelectedItem).Unom &
                            i.Unom != ((Node)e.AddedItems[0]).Unom)
                        {
                            l.Add(i);
                        }
                    }
                    txtEndLowNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l });
                    txtEndLowNode_T.DisplayMemberPath = "Number";

                    cmbUnom_T.SelectedItem = cmbUnom_T.SelectedItem;

                    if(MultiTrans.Count != 0)
                    {
                        ObservableCollection<MultiTrans> mt;
                        if(txtEndLowNode_T.SelectedIndex == -1) mt = new ObservableCollection<MultiTrans>(MultiTrans.Where(t => t.UnomM >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomM <= 1.2 * ((Node)e.AddedItems[0]).Unom).ToList());
                        else
                        {
                            mt = new ObservableCollection<MultiTrans>(MultiTrans.Where(t => (t.UnomM >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomM <= 1.2 * ((Node)e.AddedItems[0]).Unom) &
                                                                                            (t.UnomL >= 0.8 * ((Node)txtEndLowNode_T.SelectedItem).Unom & t.UnomL <= 1.2 * ((Node)txtEndLowNode_T.SelectedItem).Unom)).ToList());
                        }
                        cmbTypeName_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = mt });
                        cmbTypeName_T.DisplayMemberPath = "TypeName";
                    }
                }
                else if(tb == txtEndLowNode_T)
                {
                    ObservableCollection<Node> l = new ObservableCollection<Node>();

                    foreach(Node i in txtStartNode_T.ItemsSource)
                    {
                        if(i.Number != ((Node)e.AddedItems[0]).Number &
                            i.Number != ((Node)txtStartNode_T.SelectedItem).Number &
                            i.Unom != ((Node)txtStartNode_T.SelectedItem).Unom &
                            i.Unom != ((Node)e.AddedItems[0]).Unom)
                        {
                            l.Add(i);
                        }
                    }
                    txtEndMidNode_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = l });
                    txtEndMidNode_T.DisplayMemberPath = "Number";

                    cmbUnom_T.SelectedItem = cmbUnom_T.SelectedItem;

                    if(MultiTrans.Count != 0)
                    {
                        ObservableCollection<MultiTrans> mt;
                        if(txtEndMidNode_T.Text == "" || txtEndMidNode_T.SelectedIndex == -1) mt = new ObservableCollection<MultiTrans>(MultiTrans.Where(t => t.UnomL >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomL <= 1.2 * ((Node)e.AddedItems[0]).Unom).ToList());
                        else
                        {
                            mt = new ObservableCollection<MultiTrans>(MultiTrans.Where(t => (t.UnomL >= 0.8 * ((Node)e.AddedItems[0]).Unom & t.UnomL <= 1.2 * ((Node)e.AddedItems[0]).Unom) &
                                                                                            (t.UnomM >= 0.8 * ((Node)txtEndMidNode_T.SelectedItem).Unom & t.UnomM <= 1.2 * ((Node)txtEndMidNode_T.SelectedItem).Unom)).ToList());
                        }
                        cmbTypeName_T.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = mt });
                        cmbTypeName_T.DisplayMemberPath = "TypeName";
                    }
                }
            });
        }

        /// <summary>
        /// Перекрашивает в белый цвет поля узлов при выпадении списка
        /// </summary>
        private void DropDownNodeReflector(object sender, EventArgs e)
        {
            ChangeCmbColor(sender, false);
            Log.Clear();
        }

        #endregion Helpers

        #region Power Network Methods

        /// <summary>
        /// Проверка связности схемы
        /// </summary>
        public bool Connectivity()
        {
            if(track.Nodes.Count == 0 || track.Branches.Count == 0)
            {
                //MessageBox.Show("Отсутствуют узлы или ветви!");
                return false;
            }

            List<int> imutNodes = track.Nodes.OrderBy(n => n.Number).Select(n => n.Number).ToList(); //Список узлов
            var branches = track.Branches.Distinct(new BranchEqualityComparer()).ToList();          //Список уникальных ветвей

            ObservableCollection<int> exNodes = new ObservableCollection<int>(imutNodes);  //Список узлов для исключения
            //List<Branch> exBranches = new List<Branch>(branches);  //Список ветвей для исключения

            for(int i = 0; i < imutNodes.Count; i++)
            {
                if(!branches.Any(b => (b.Start == imutNodes[i]) | (b.End == imutNodes[i]))) return false;
                if(exNodes.Contains(imutNodes[i])) exNodes.Remove(imutNodes[i]);

                List<int> linked = new List<int>();
                foreach(var b in branches)
                {
                    if(b.Start == imutNodes[i]) linked.Add(b.End);
                    else if(b.End == imutNodes[i]) linked.Add(b.Start);
                }

                RecurseFinder(linked);

                if(exNodes.Count == 0) return true;
            }

            void RecurseFinder(List<int> linked)
            {
                foreach(int j in linked)
                {
                    if(exNodes.Contains(j)) exNodes.Remove(j);
                }

                if(exNodes.Count == 0) return;

                List<int> linker = new List<int>();

                foreach(int link in linked)
                {
                    foreach(var b in branches)
                    {
                        if(b.Start == link & exNodes.Contains(b.End)) linker.Add(b.End);
                        else if(b.End == link & exNodes.Contains(b.Start)) linker.Add(b.Start);
                    }

                    if(linker.Count != 0) RecurseFinder(new List<int>(linker));
                    else return;
                }
            }

            return false;
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

            //TESTER
            Stopwatch sw = Stopwatch.StartNew(); //STOPWATCH
            sw.Start(); //STOPWATCH
            bool p = Connectivity();
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds.ToString());
            MessageBox.Show(p.ToString());
            //TESTER

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
            DataGridTracker localTracker = new DataGridTracker();

            switch(extension.ToLower())
            {
                case ".peh":
                    try
                    {
                        localTracker = serializator.fromXML();

                        track.Nodes.Clear(); track.Branches.Clear();                    //Очистка старых значений в колекциях Узлов и Ветвей
                        Log.Show($"Извлечение данных из файла: {filename}. Процесс...", LogClass.LogType.Information);

                        foreach(var i in localTracker.Nodes) track.Nodes.Add(i);        //Добавление Узлов 
                        foreach(var i in localTracker.Branches) track.Branches.Add(i);  //Добавление Ветвей
                    }
                    catch(Exception)
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

        #endregion Save/Open (Serialize/Deserialize) Methods

    }
}
