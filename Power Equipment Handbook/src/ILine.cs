using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    interface ILine
    {
        //Основные параметры линии
        Base_Params Params { get; set; }
        //Дополнительные параметры линии
        Line_Params LineParams { get; set; }
        //Длительно допустимый ток линии, А
        double Idd { get; set; }
    }
}
