using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Класс расширений методов-валидаторов Узлов
    /// </summary>
    public static class ValidatorNodesExtentions
    {

        /// <summary>
        /// Проверка типа Узла
        /// </summary>
        /// <param name="node">Проверяемый Узел</param>
        public static void ValidateNodeType(this Node node)
        {
            //Check if PV
            var vpreN = node.Vzd == 0.0;
            var qminN = node.Q_min == 0.0;
            var qmaxN = node.Q_max == 0.0;

            if (node.Type == "Нагр")
            {
                if (!vpreN & !qminN & !qmaxN)
                {
                    node.Q_g = 0.0;
                    node.Type = "Ген";
                } 
            }
            else if (node.Type == "Ген")
            {
                if ((vpreN & qminN) | (qminN & qmaxN))
                {
                    node.Type = "Нагр";
                }
                else if (!qminN & qmaxN)
                {
                    node.Q_g = node.Q_min;
                    node.Type = "Нагр";
                }
                else if (qminN & !qmaxN)
                {
                    node.Q_g = 0.0;
                    node.Type = "Нагр";
                }
                else if (vpreN & (!qminN & !qmaxN))
                {
                    node.Q_g = 0.0;
                    node.Type = "Нагр";
                }
            }
        }
               

    }
}
