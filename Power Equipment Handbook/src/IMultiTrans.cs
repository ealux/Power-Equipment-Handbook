using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    interface IMultiTrans
    {
        //Основные параметры по каждой из сторон трансформатора
        Params Paramss_High { get; set; }
        Params Paramss_Mid { get; set; }
        Params Paramss_Low { get; set; }

        //Дополнительные параметры по каждой из сторон трансформатора
        Trans_Params Trans_Paramss_High { get; set; }
        Trans_Params Trans_Paramss_Mid { get; set; }
        Trans_Params Trans_Paramss_Low { get; set; }

        //Коэффициенты трансформации
        double Ktr_High { get; set; }
        double Ktr_Mid { get; set; }
        double Ktr_Low { get; set; }

        //Номинальные напряжения
        double Unom_High { get; set; }
        double Unom_Mid { get; set; }
        double Unom_Low { get; set; }

        //Номинальная мощность
        double Snom { get; set; }

        //Параметры холостого хода (общие для всех сторон трансформатора)
        double Pxx { get; set; }
        double Qxx { get; set; }
        double Ixx { get; set; }
    }
}
