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

namespace Power_Equipment_Handbook.src
{
    public class Serializator
    {
        string filename;
        DataGridTracker track;

        /// <summary>
        /// Инициализация обобщенного Сериализатора
        /// </summary>
        /// <param name="file">Абсолютный путь к файлу записи</param>
        /// <param name="tracker">Объект с наборо данных (Узлы и Ветви) типа DataGridTracker</param>
        public Serializator(string file, DataGridTracker tracker)
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
    }
}
