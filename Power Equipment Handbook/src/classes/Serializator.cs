using System;
using System.IO;
using System.Xml.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading.Tasks;
using System.Linq;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Класс универсального сериализатора, содержащий методы для экспорта/импорта данных программы 
    /// </summary>
    public class UniverseSerializator
    {
        private readonly string filename;        //Абсолютный путь к файлу
        private DataGridTracker track;          //Класс, содержащий данные с текущим состоянием таблиц Ветвей и Узлов

        /// <summary>
        /// Инициализация обобщенного Сериализатора
        /// </summary>
        /// <param name="file">Абсолютный путь к файлу записи</param>
        /// <param name="tracker">Объект с набором данных (Узлы, Ветви, Ячейки) типа DataGridTracker</param>
        public UniverseSerializator(string file, DataGridTracker tracker)
        {
            this.filename = file;
            this.track = tracker;
        }

        #region *.peh (xml) serializator behevior

        /// <summary>
        /// Сериализация в XML-структуру с внутренним расширением *.peh
        /// </summary>
        public void toXML()
        #pragma warning restore IDE1006 // Стили именования
        {
            Task.Run(()=>
            {
                XmlSerializer serializer = new XmlSerializer(type: typeof(DataGridTracker));

                using(FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    serializer.Serialize(fs, track);
                }
            });
        }

        /// <summary>
        /// Импорт из XML-структуры в внутренним расширением *.peh
        /// </summary>
        /// <returns>Возвращает объект типа DataGridTracker</returns>
        public DataGridTracker fromXML()
        {
            XmlSerializer serializer = new XmlSerializer(type: typeof(DataGridTracker));
            track = new DataGridTracker();

            using(StreamReader fs = new StreamReader(filename))
            {
                track = (DataGridTracker)serializer.Deserialize(fs);
                //fs.Close();
            }
            return track;
        }

        #endregion *.pen (xml) serializator behevior

        #region *.xlsx serializator behevior

        /// <summary>
        /// Сериализация в Excel файл с расширением OpenXML *.xlsx
        /// </summary>
        public void toXLSX()
        {
            Task.Run(()=> 
            {
                FileInfo fileInfo = new FileInfo(filename);

                if(fileInfo.Exists) fileInfo.Delete();

                using(ExcelPackage p = new ExcelPackage(fileInfo))
                {
                    var wb = p.Workbook;

                    var sheetNodes = wb.Worksheets.Add("Nodes");
                    var sheetBranches = wb.Worksheets.Add("Branches");

                    #region Excel PreDesign

                    string[] lblNode = new string[] { "O", "S", "Тип", "Номер", "Название" , "U_nom", "N_схн" , "Район" , "P_н" ,
                                                      "Q_н", "P_г", "Q_г", "V_зд", "Q_min", "Q_max", "B_ш", "V", "Delta", "Район2"};
                    string[] lblBranch = new string[] { "O", "S", "Тип", "N_нач", "N_кон", 
                                                        "N_п", "ID Группы", "Название", 
                                                        "R", "X", "G", "B", "R0", "X0", "G0", "B0",
                                                        "Kт/r", "N_анц", "БД_нач", "P_нач", "Q_нач", 
                                                        "Na", "I max", "I загр.", "Idop_25"};
                    //Подготовка узлов
                    for(int i = 1; i < 20; i++)
                    {
                        sheetNodes.Cells[1, i].Value = lblNode[i - 1];                                              //Вставка значения
                        sheetNodes.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;                       //Закраска ячейки
                        sheetNodes.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);     //Закраска ячейки
                    }
                    //Подготовка ветвей
                    for(int i = 1; i < 26; i++)
                    {
                        sheetBranches.Cells[1, i].Value = lblBranch[i - 1];                                         //Вставка значения
                        sheetBranches.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;                    //Закраска ячейки
                        sheetBranches.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);  //Закраска ячейки
                        
                    }

                    sheetNodes.Cells[1, 1, 1, 19].AutoFilter = true;                                                //Добавка фильтра
                    sheetBranches.Cells[1, 1, 1, 25].AutoFilter = true;                                             //Добавка фильтра

                    #endregion Excel PreDesign

                    //Цикл по узлам схемы
                    for(int i = 1; i <= track.Nodes.Count; i++)
                    {
                        sheetNodes.Cells[i + 1, 1].Value = String.Empty;
                        sheetNodes.Cells[i + 1, 2].Value = track.Nodes[i - 1].State == 0 ? String.Empty : "1";
                        sheetNodes.Cells[i + 1, 3].Value = track.Nodes[i - 1].Type;
                        sheetNodes.Cells[i + 1, 4].Value = track.Nodes[i - 1].Number;
                        sheetNodes.Cells[i + 1, 5].Value = track.Nodes[i - 1].Name;
                        sheetNodes.Cells[i + 1, 6].Value = track.Nodes[i - 1].Unom;
                        sheetNodes.Cells[i + 1, 7].Value = String.Empty;
                        sheetNodes.Cells[i + 1, 8].Value = track.Nodes[i - 1].Region == 0 ? String.Empty : track.Nodes[i - 1].Region.ToString();
                        sheetNodes.Cells[i + 1, 9].Value = track.Nodes[i - 1].P_n;
                        sheetNodes.Cells[i + 1, 10].Value = track.Nodes[i - 1].Q_n;
                        sheetNodes.Cells[i + 1, 11].Value = track.Nodes[i - 1].P_g;
                        sheetNodes.Cells[i + 1, 12].Value = track.Nodes[i - 1].Q_g;
                        sheetNodes.Cells[i + 1, 13].Value = track.Nodes[i - 1].Vzd;
                        sheetNodes.Cells[i + 1, 14].Value = track.Nodes[i - 1].Q_min;
                        sheetNodes.Cells[i + 1, 15].Value = track.Nodes[i - 1].Q_max;
                        sheetNodes.Cells[i + 1, 16].Value = track.Nodes[i - 1].B_sh;
                        sheetNodes.Cells[i + 1, 17].Value = String.Empty;
                        sheetNodes.Cells[i + 1, 18].Value = String.Empty;
                        sheetNodes.Cells[i + 1, 19].Value = String.Empty;

                        for(int j = 1; j < 20; j++)
                        {
                            sheetNodes.Cells[i, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheetNodes.Cells[i, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            sheetNodes.Cells[i + 1, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheetNodes.Cells[i + 1, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }
                    //Цикл по ветвям схемы
                    for(int i = 1; i <= track.Branches.Count; i++)
                    {
                        sheetBranches.Cells[i + 1, 1].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 2].Value = track.Branches[i - 1].State == 0 ? String.Empty : "1";
                        sheetBranches.Cells[i + 1, 3].Value = track.Branches[i - 1].Type;
                        sheetBranches.Cells[i + 1, 4].Value = track.Branches[i - 1].Start;
                        sheetBranches.Cells[i + 1, 5].Value = track.Branches[i - 1].End;
                        sheetBranches.Cells[i + 1, 6].Value = track.Branches[i - 1].Npar == 0 ? String.Empty : track.Branches[i - 1].Npar.ToString();
                        sheetBranches.Cells[i + 1, 7].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 8].Value = track.Branches[i - 1].Name;
                        sheetBranches.Cells[i + 1, 9].Value = track.Branches[i - 1].R;
                        sheetBranches.Cells[i + 1, 10].Value = track.Branches[i - 1].X;
                        sheetBranches.Cells[i + 1, 11].Value = track.Branches[i - 1].G;
                        sheetBranches.Cells[i + 1, 12].Value = track.Branches[i - 1].B;
                        sheetBranches.Cells[i + 1, 13].Value = track.Branches[i - 1].R0;
                        sheetBranches.Cells[i + 1, 14].Value = track.Branches[i - 1].X0;
                        sheetBranches.Cells[i + 1, 15].Value = track.Branches[i - 1].G0;
                        sheetBranches.Cells[i + 1, 16].Value = track.Branches[i - 1].B0;
                        sheetBranches.Cells[i + 1, 17].Value = track.Branches[i - 1].Ktr == null ? String.Empty : track.Branches[i - 1].Ktr.ToString(); //Check
                        sheetBranches.Cells[i + 1, 18].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 19].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 20].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 21].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 22].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 23].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 24].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 25].Value = track.Branches[i - 1].Idd / 1000;

                        for(int j = 1; j < 26; j++)
                        {
                            sheetBranches.Cells[i, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheetBranches.Cells[i, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            sheetBranches.Cells[i + 1, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheetBranches.Cells[i + 1, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }

                    p.Save(); //Сохранение выходного документа
                }
            });
        }

        /// <summary>
        /// Сериализация в Excel файл с расширением OpenXML *.xlsx Таблица оборудования
        /// </summary>
        public void EquipmentToXLSX()
        {
            Task.Run(() =>
            {
                FileInfo fileInfo = new FileInfo(filename);

                if (fileInfo.Exists) fileInfo.Delete();

                var tmp = new FileInfo(Environment.CurrentDirectory + @"\src\res\_template.xlsx");

                if (!tmp.Exists) tmp = fileInfo;

                using (ExcelPackage p = new ExcelPackage(tmp))
                {
                    var wb = p.Workbook;
                    ExcelWorksheet sheetEquipment;

                    int row;

                    if (!tmp.Exists)
                    {
                        sheetEquipment = wb.Worksheets.Add("Основное оборудование");    //Страница оборудования

                        row = 3; //Стартовая позиция строки
                        int col = 1; //Стартовая позиция столбца

                        #region Excel PreDesign

                        string[] lbl = new string[] { "Наименование ПС", "№ ячейки", "Uном, кВ", "Оборудование", "Марка",
                                                      "I ном, А", "I откл.ном, кА", "I тер.ном, кА", "t тер.ном, с", "B к.норм, кА2с", "i пр.скв, кА",
                                                      "I п0, А", "t сумм, с", "B к.норм, кА2с", "i уд, кА" , "ta, с", "t осн.защ, с", "b расч, %"};

                        sheetEquipment.Cells[row, col].Value = "Оборудование"; sheetEquipment.Cells[row, col, row, col + 4].Merge = true;
                        sheetEquipment.Cells[row, col + 5].Value = "Нормируемые параметры"; sheetEquipment.Cells[row, col + 5, row, col + 10].Merge = true;
                        sheetEquipment.Cells[row, col + 11].Value = "Расчётные параметры"; sheetEquipment.Cells[row, col + 11, row, col + 17].Merge = true;
                        sheetEquipment.Cells[row + 1, col + 11].Value = "_Режим_1_"; sheetEquipment.Cells[row + 1, col + 11, row + 1, col + 17].Merge = true;

                        for (int i = 0; i < lbl.Length; i++)
                        {
                            if (i <= 10)
                            {
                                sheetEquipment.Cells[4, i + 1, 5, i + 1].Merge = true;
                                sheetEquipment.Cells[4, i + 1].Value = lbl[i];
                            }
                            else
                            {
                                sheetEquipment.Cells[5, i + 1].Value = lbl[i];
                            }
                        }

                        row = row + 3;

                        #endregion Excel PreDesign
                    }
                    else
                    {
                        sheetEquipment = wb.Worksheets[1];                     //Страница оборудования
                        row = 6;                                               //Установка стартовой позиции строки для вывода ячеек 
                    }


                    var nodes = track.Cells.Select(n => n.NodeNumber).Distinct();   //Список уникальных узлов где есть ячейки

                    foreach (var node in nodes) //Цикл по узлам
                    {
                        sheetEquipment.Cells[row, 1].Value = track.Nodes.Where(n => n.Number == node).First().Name;//Вывод имени узла
                        sheetEquipment.Cells[row, 3].Value = track.Nodes.Where(n => n.Number == node).First().Unom;//Вывод Unom узла

                        var cells = track.Cells.Where(n => n.NodeNumber == node);

                        int node_bias = -1;
                        foreach (var cell in cells)
                        {
                            if (cell.ElemsCount == 0) node_bias += 1;
                            else node_bias += cell.ElemsCount;
                        }

                        sheetEquipment.Cells[row, 1, row + node_bias, 1].Merge = true; //Объединение полей (наименование узла)
                        sheetEquipment.Cells[row, 3, row + node_bias, 3].Merge = true; //Объединение полей (Unom узла)

                        foreach (var cell in cells) //Цикл по ячейкам
                        {
                            sheetEquipment.Cells[row, 2].Value = cell.Name;         //Вывод имени ячейки                            

                            if (cell.CellElements.Count != 0)                       //Условие наличия элементов ячейки
                            {
                                var elemsCount = cell.CellElements.Count - 1;
                                sheetEquipment.Cells[row, 2, row + elemsCount, 2].Merge = true; //Объединение полей (наименование ячейки)

                                foreach (var elem in cell.CellElements) //Цикл по элементам ячейки
                                {
                                    sheetEquipment.Cells[row, 4].Value = elem.SerializeType();  //Вывод Типа оборудования
                                    sheetEquipment.Cells[row, 5].Value = elem.Name;             //Вывод Марки оборудования
                                    sheetEquipment.Cells[row, 6].Value = elem.Inom == null ? "-" : elem.Inom.ToString();             //Вывод Inom оборудования
                                    sheetEquipment.Cells[row, 7].Value = elem.Iotkl == null ? "-" : elem.Iotkl.ToString();            //Вывод Iоткл оборудования
                                    sheetEquipment.Cells[row, 8].Value = elem.Iterm == null ? "-" : elem.Iterm.ToString();            //Вывод Iтерм оборудования
                                    sheetEquipment.Cells[row, 9].Value = elem.Tterm == null ? "-" : elem.Tterm.ToString();            //Вывод tтерм оборудования
                                    sheetEquipment.Cells[row, 10].Value = elem.Bterm == null ? "-" : elem.Bterm.ToString();           //Вывод Bтерм оборудования
                                    sheetEquipment.Cells[row, 11].Value = elem.Iudar == null ? "-" : elem.Iudar.ToString();           //Вывод iуд оборудования

                                    row += 1;
                                }
                            }
                            else row += 1;
                        }
                    }


                    #region PostDesign

                    for (int i = 1; i < 19; i++)
                    {
                        for (int j = 3; j <= row - 1; j++)
                        {
                            if (sheetEquipment.Cells[j, i].Value == null) sheetEquipment.Cells[j, i].Value = "";
                            sheetEquipment.Cells[j, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        }
                    }
                    foreach (var item in sheetEquipment.MergedCells) sheetEquipment.Cells[item].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    sheetEquipment.Cells[3, 1, row - 1, 5].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    sheetEquipment.Cells[3, 6, row - 1, 11].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    sheetEquipment.Cells[3, 12, row - 1, 18].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    sheetEquipment.Cells[3, 1, 5, 18].Style.Border.BorderAround(ExcelBorderStyle.Thick);

                    sheetEquipment.Cells.AutoFitColumns();
                    sheetEquipment.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheetEquipment.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    sheetEquipment.Column(1).Style.WrapText = true;
                    sheetEquipment.Column(2).Style.WrapText = true;

                    sheetEquipment.Cells.Style.Font.Name = "Times New Romans";
                    sheetEquipment.Cells.Style.Font.Size = 10;

                    #endregion PostDesign

                    p.SaveAs(fileInfo); //Сохранение выходного документа
                }                
            });
        }

        #endregion *.xlsx serializator behevior
    }
}
