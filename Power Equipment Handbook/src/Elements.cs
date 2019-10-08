using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Абстрактный класс элементов электрической сети
    /// </summary>
    public abstract class Element
    {
        public string Type { get; set; }
        public string Type_Name { get; set; }
        public string Name { get; set; }
        public int Unom { get; set; }
        public int Region { get; set; }
        public int State { get; set; } = 0;

        /// <summary>
        /// Конструктор тип Element
        /// </summary>
        /// <param name="type">Тип элемента (Линия (L), Трансформатор (T) и т.д.)</param>
        /// <param name="type_name">Имя элемента (АС, ТРДН и т.д.)</param>
        /// <param name="Unom">Класс номинального напряжения, кВ</param>
        /// <param name="name">Имя элемента (Москва - Воронеж-1, Т-1)</param>
        /// <param name="region">Район</param>
        public Element(string type, string type_name, int Unom,
                        string name = "", int region = default(int))
        {
            Type = type;
            Type_Name = type_name;
            Name = name;
            this.Unom = Unom;
            Region = region;
        }
    }

    /// <summary>
    /// Базовый класс Линий
    /// </summary>
    public class Line_Base : Element, ILine
    {
        public Line_Base(string type, string type_name, int Unom, string name = "", int region = 0) : base(type, type_name, Unom, name, region) { }

        public Base_Params Params { get; set; }
        public Line_Params LineParams { get; set; }
        public double Idd { get; set; }
    }

    /// <summary>
    /// Базовый класс Двухобмоточных Трансформаторов
    /// </summary>
    public class Trans_Base : Element, ITrans
    {
        public Trans_Base(string type, string type_name, int Unom, string name = "", int region = 0) : base(type, type_name, Unom, name, region) { }

        public Base_Params Params { get; set; }
        public Trans_Params TransParams { get; set; }
        public double Ktr { get; set; }
        public double Unom_High { get; set; } public double Unom_Low { get; set; }
        public double Snom { get; set; }
        public double Pxx { get; set; }
        public double Qxx { get; set; }
        public double Ixx { get; set; }
    }

    /// <summary>
    /// Базовый класс Трехобмоточных Трансфоматоров и Автотрансформаторов
    /// </summary>
    public class MultiTrans_Base : Element, IMultiTrans
    {
        public MultiTrans_Base(string type, string type_name, int Unom, string name = "", int region = 0) : base(type, type_name, Unom, name, region) { }

        public Base_Params Params_High { get; set; } public Base_Params Params_Mid { get; set; } public Base_Params Params_Low { get; set; }
        public Trans_Params TransParams_High { get; set; } public Trans_Params TransParams_Mid { get; set; } public Trans_Params TransParams_Low { get; set; }
        public double Ktr_High { get; set; } public double Ktr_Mid { get; set; } public double Ktr_Low { get; set; }
        public double Unom_High { get; set; } public double Unom_Mid { get; set; } public double Unom_Low { get; set; }
        public double Snom { get; set; }
        public double Pxx { get; set; }
        public double Qxx { get; set; }
        public double Ixx { get; set; }
    }
}
