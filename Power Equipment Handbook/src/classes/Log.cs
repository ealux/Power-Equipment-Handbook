using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Класс объекта лога с привязкой к существующему TextBlock
    /// </summary>
    public class LogClass
    {
        TextBlock logBox;

        /// <summary>
        /// Конструктор класс LogClass - инициализирует объект работы с логом
        /// </summary>
        /// <param name="log">Исходный TextBlock для передачи функции лога</param>
        public LogClass(TextBlock log)
        {
            this.logBox = log;
            this.logBox.Text = "";
        }

        /// <summary>
        /// Вывод сообщения в лог
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="type">Тип сообщения (по умл. LogType.Error)</param>
        public void Show(string message, LogType type = LogType.Error)
        {
            Application.Current.Dispatcher?.Invoke(delegate
            {
                logBox.Text = message;

                if(type == LogType.Error)
                {
                    logBox.Foreground = Brushes.Red;
                    System.Media.SystemSounds.Exclamation.Play();
                }
                else if (type == LogType.Success) logBox.Foreground = Brushes.Green;
                else logBox.Foreground = Brushes.Black;
            });
        }

        /// <summary>
        /// Отчистка записи Лога
        /// </summary>
        public void Clear() => Application.Current.Dispatcher?.Invoke(delegate { this.logBox.Text = ""; });

        /// <summary>
        /// Тип сообщения в логе
        /// </summary>
        public enum LogType
        {
            Error = 0,
            Information = 1,
            Success = 2
        }
    }
}
