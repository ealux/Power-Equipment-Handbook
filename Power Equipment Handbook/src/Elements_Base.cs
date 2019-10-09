using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Power_Equipment_Handbook;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Абстрактный класс элементов электрической сети
    /// </summary>
    public abstract class Element
    {
        public string Type { get; set; }
        public string Type_Name { get; set; }
        /// <summary>
        /// Имя элемента
        /// </summary>
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
    public abstract class Line_Base : Element
    {
        public Line_Base(int Unom, string type_name, string type, string name = "", int region = 0) : base(type, type_name, Unom, name, region) { }

        public Base_Params Base_params { get; set; }
        public Line_Params Line_params { get; set; }
        public double Idd { get; set; }
    }

    /// <summary>
    /// Базовый класс Двухобмоточных Трансформаторов
    /// </summary>
    public abstract class Trans_Base : Element
    {
        public Trans_Base(int Unom, string type_name, string type, string name = "", int region = 0) : base(type, type_name, Unom, name, region) { }

        public Base_Params Base_params { get; set; }
        public Trans_Params Trans_params { get; set; }
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
    public abstract class MultiTrans_Base : Element
    {
        public MultiTrans_Base(int Unom, string type_name, string type, string name = "", int region = 0) : base(type, type_name, Unom, name, region) { }

        public Base_Params Base_params_High { get; set; } public Base_Params Base_params_Mid { get; set; } public Base_Params Base_params_Low { get; set; }
        public Trans_Params Trans_params_High { get; set; } public Trans_Params Trans_params_Mid { get; set; } public Trans_Params Trans_params_Low { get; set; }
        public double Ktr_High { get; set; } public double Ktr_Mid { get; set; } public double Ktr_Low { get; set; }
        public double Unom_High { get; set; } public double Unom_Mid { get; set; } public double Unom_Low { get; set; }
        public double Snom { get; set; }
        public double Pxx { get; set; }
        public double Qxx { get; set; }
        public double Ixx { get; set; }
    }
}
