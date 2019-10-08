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
        Params Paramss { get; set; }
        //Дополнительные параметры линии
        Trans_Params Line_Paramss { get; set; }
        //Длительно допустимый ток линии, А
        double Idd { get; set; }
    }
}
