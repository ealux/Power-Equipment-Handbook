using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Power_Equipment_Handbook;
using System.Windows;

namespace Power_Equipment_Handbook.src
{
    public class DBProvider
    {
        private string status;

        /// <summary>
        /// Поле доступа к объекту connection
        /// </summary>
        public SQLiteConnection Connection { get; set; }

        /// <summary>
        /// Поле отображения статуса объекта connection
        /// </summary>
        public string Status
        {
            get { if (Connection.State == ConnectionState.Open) status = "Подключен"; else status = "Отключен"; return status; }
            set => status = value;
        }

        /// <summary>
        /// Конструкторы
        /// </summary>
        public DBProvider() { }

        /// <summary>
        /// Конструктор с подключением
        /// </summary>
        /// <param name="соnnection_string">Имя базы данных (+путь)</param>
        public DBProvider(string соnnection_string)
        {
            Connect(соnnection_string);
        }


        //TODO Проверка на наличие файла базы в папке

        /// <summary>
        /// Создание подключения
        /// </summary>
        /// <param name="DB_name">Имя базы данных</param>
        public SQLiteConnection Connect (string DB_name)
        {
            if (!System.IO.File.Exists(DB_name))
            {
                MessageBox.Show("Database file NOT found!"); return null;
            }
            SQLiteConnection.SharedFlags = SQLiteConnectionFlags.NoCreateModule;
            Connection = new SQLiteConnection("Data Source=" + DB_name + "; Version=3;");
            try { Connection.Open(); }
            catch (SQLiteException ex) { MessageBox.Show(ex.Message); }
            return Connection;
        }

        /// <summary>
        /// Переподключиться к базе. 
        /// Если подключение существует - производится переподключение по старому пути
        /// Если подключение отсутствует - производится подключение по новому пути
        /// </summary>
        /// <param name="DB_name">Путь к новой базе (по умолчанию - пустая строка)</param>
        /// <returns></returns>
        public SQLiteConnection ReConnect (string DB_name = "")
        {
            if (Status == "Подключен")
            {
                Connection.Close();
                Connection = new SQLiteConnection("Data Source=" + Connection.ConnectionString + "; Version=3;");
                try { Connection.Open(); }
                catch (SQLiteException ex) { Console.WriteLine(ex.Message); }
                return Connection;
            }
            else { Status = "Отключен"; return Connect(DB_name); }
        }

        /// <summary>
        /// Выполнение запроса без возвращаемого значения
        /// </summary>
        /// <param name="cmd">Текст входной команды</param>
        /// <param name="conn">SQLiteConnection переменная (метод Connect)</param>
        public void Command_NonQuery(string cmd, SQLiteConnection conn)
        {
            SQLiteCommand command = new SQLiteCommand(cmd, conn);

            try { command.ExecuteNonQuery(); }
            catch (SQLiteException ex) { Console.WriteLine(ex.Message); }
        }

        /// <summary>
        /// Выполнение зпроса с возвратом SQLiteDataReader объекта 
        /// </summary>
        /// <param name="cmd">Текст входной команды</param>
        /// <param name="conn">SQLiteConnection переменная (метод Connect)</param>
        /// <returns>SQLiteDataReader объект</returns>
        public SQLiteDataReader Command_Query(string cmd, SQLiteConnection conn)
        {
            SQLiteCommand command = new SQLiteCommand(cmd, conn);
            SQLiteDataReader dr = command.ExecuteReader();

            try { dr.Read(); }
            catch (SQLiteException ex) { Console.WriteLine(ex.Message); }

            return dr;
        }
    }
}
