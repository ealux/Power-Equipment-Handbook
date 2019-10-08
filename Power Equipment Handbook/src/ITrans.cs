using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    interface ITrans
    {
        //Основные параметры трансформатора
        Params Paramss { get; set; }
        //Дополнительные параметры трансформатора
        Trans_Params Trans_Paramss { get; set; }
        double Ktr { get; set; }
        //Номинальные напряжения сторон
        double Unom_High { get; set; }
        double Unom_Low { get; set; }
        //Номинальная мощность трансформатора
        double Snom { get; set; }
        //Параметры холостого хода трансформатора
        double Pxx { get; set; }
        double Qxx { get; set; }
        double Ixx { get; set; }
    }
}
