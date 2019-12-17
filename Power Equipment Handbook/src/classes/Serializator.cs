using System;
using System.IO;
using System.Xml.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Класс универсального сериализатора, содержащий методы для экспорта/импорта данных программы 
    /// </summary>
    public class UniverseSerializator
    {
        private string filename;        //Абсолютный путь к файлу
        DataGridTracker track;  //Класс, содержащий данные с текущим состоянием таблиц Ветвей и Узлов

        /// <summary>
        /// Инициализация обобщенного Сериализатора
        /// </summary>
        /// <param name="file">Абсолютный путь к файлу записи</param>
        /// <param name="tracker">Объект с наборо данных (Узлы и Ветви) типа DataGridTracker</param>
        public UniverseSerializator(string file, DataGridTracker tracker)
        {
            this.filename = file;
            this.track = tracker;
        }

        #region *.pen (xml) serializator behevior

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
                fs.Close();
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
                    string[] lblBranch = new string[] { "O", "S", "Тип", "N_нач", "N_кон", "N_п", "ID Группы", "Название", "R", "X", "G", "B",
                                                        "Kт/r", "N_анц", "БД_нач", "P_нач", "Q_нач", "Na", "I max", "I загр.", "Idop_25"};
                    //Подготовка узлов
                    for(int i = 1; i < 20; i++)
                    {
                        sheetNodes.Cells[1, i].Value = lblNode[i - 1];                                              //Вставка значения
                        sheetNodes.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;                       //Закраска ячейки
                        sheetNodes.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);     //Закраска ячейки
                    }
                    //Подготовка ветвей
                    for(int i = 1; i < 22; i++)
                    {
                        sheetBranches.Cells[1, i].Value = lblBranch[i - 1];                                         //Вставка значения
                        sheetBranches.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;                    //Закраска ячейки
                        sheetBranches.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);  //Закраска ячейки
                        
                    }

                    sheetNodes.Cells[1, 1, 1, 19].AutoFilter = true;                                                //Добавка фильтра
                    sheetBranches.Cells[1, 1, 1, 21].AutoFilter = true;                                             //Добавка фильтра

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
                        sheetBranches.Cells[i + 1, 13].Value = track.Branches[i - 1].Ktr == null ? String.Empty : track.Branches[i - 1].Ktr.ToString(); //Check
                        sheetBranches.Cells[i + 1, 14].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 15].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 16].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 17].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 18].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 19].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 20].Value = String.Empty;
                        sheetBranches.Cells[i + 1, 21].Value = track.Branches[i - 1].Idd / 1000;

                        for(int j = 1; j < 22; j++)
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

        #endregion *.xlsx serializator behevior
    }
}
