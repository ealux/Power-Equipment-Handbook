using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Класс универсального сериализатора, содержащий методы для экспорта/импорта данных программы 
    /// </summary>
    public class UniverseSerializator
    {
        string filename;        //Абсолютный путь к файлу
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
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
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
            FileInfo fileInfo = new FileInfo(filename);

            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                using(ExcelPackage p = new ExcelPackage(fileInfo))
                {
                    var wb = p.Workbook;

                    var sheetNodes = wb.Worksheets.Add("Nodes");
                    var sheetBranches = wb.Worksheets.Add("Branches");

                    #region Excel PreDesign

                    sheetNodes.Cells[1, 1].Value = "O";
                    sheetNodes.Cells[1, 2].Value = "S";
                    sheetNodes.Cells[1, 3].Value = "Тип";
                    sheetNodes.Cells[1, 4].Value = "Номер";
                    sheetNodes.Cells[1, 5].Value = "Название";
                    sheetNodes.Cells[1, 6].Value = "U_nom";
                    sheetNodes.Cells[1, 7].Value = "N_схн";
                    sheetNodes.Cells[1, 8].Value = "Район";
                    sheetNodes.Cells[1, 9].Value = "P_н";
                    sheetNodes.Cells[1, 10].Value = "Q_н";
                    sheetNodes.Cells[1, 11].Value = "P_г";
                    sheetNodes.Cells[1, 12].Value = "Q_г";
                    sheetNodes.Cells[1, 13].Value = "V_зд";
                    sheetNodes.Cells[1, 14].Value = "Q_min";
                    sheetNodes.Cells[1, 15].Value = "Q_max";
                    sheetNodes.Cells[1, 16].Value = "B_ш";
                    sheetNodes.Cells[1, 17].Value = "V";
                    sheetNodes.Cells[1, 18].Value = "Delta";
                    sheetNodes.Cells[1, 19].Value = "Район2";
                    //sheetNodes.Cells[1,1,1,19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //Выравнивание по центру содержимого
                    //sheetNodes.Cells[1, 1, 1, 19].Style.Border.BorderAround(ExcelBorderStyle.Thin);         //Отрисовка границ


                    sheetBranches.Cells[1, 1].Value = "O";
                    sheetBranches.Cells[1, 2].Value = "S";
                    sheetBranches.Cells[1, 3].Value = "Тип";
                    sheetBranches.Cells[1, 4].Value = "N_нач";
                    sheetBranches.Cells[1, 5].Value = "N_кон";
                    sheetBranches.Cells[1, 6].Value = "N_п";
                    sheetBranches.Cells[1, 7].Value = "ID Группы";
                    sheetBranches.Cells[1, 8].Value = "Название";
                    sheetBranches.Cells[1, 9].Value = "R";
                    sheetBranches.Cells[1, 10].Value = "X";
                    sheetBranches.Cells[1, 11].Value = "G";
                    sheetBranches.Cells[1, 12].Value = "B";
                    sheetBranches.Cells[1, 13].Value = "Kт/r";
                    sheetBranches.Cells[1, 14].Value = "N_анц";
                    sheetBranches.Cells[1, 15].Value = "БД_нач";
                    sheetBranches.Cells[1, 16].Value = "P_нач";
                    sheetBranches.Cells[1, 17].Value = "Q_нач";
                    sheetBranches.Cells[1, 18].Value = "Na";
                    sheetBranches.Cells[1, 19].Value = "I max";
                    sheetBranches.Cells[1, 20].Value = "I загр.";
                    sheetBranches.Cells[1, 21].Value = "Idop_25";
                    sheetBranches.Cells[1, 1, 1, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //Выравнивание по центру содержимого
                    sheetBranches.Cells[1, 1, 1, 21].Style.Border.BorderAround(ExcelBorderStyle.Thin);              //Отрисовка границ

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
                            sheetNodes.Cells[i+1, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheetNodes.Cells[i+1, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            if(i == 1)
                            {
                                sheetNodes.Cells[i, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                sheetNodes.Cells[i, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                            }
                        }
                    }

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
                        sheetBranches.Cells[i + 1, 21].Value = track.Branches[i - 1].Idd/1000;

                        for(int j = 1; j < 22; j++)
                        {
                            sheetBranches.Cells[i, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheetBranches.Cells[i, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            sheetBranches.Cells[i + 1, j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheetBranches.Cells[i + 1, j].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            if(i == 1)
                            {
                                sheetBranches.Cells[i, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                sheetBranches.Cells[i, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                            }
                        }
                    }

                    p.Save(); //Сохранение выходного документа
                }
            });
        }

        #endregion *.xlsx serializator behevior
    }
}
