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
    abstract class Element
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

    //public class Line : Element, ILine
    //{
    //    public Params Paramss { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Trans_Params Line_Paramss { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Idd { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //}

    //public class Trans : Element, ITrans
    //{
    //    public Params Paramss { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Trans_Params Trans_Paramss { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Ktr { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Unom_High { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Unom_Low { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Snom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Pxx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Qxx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Ixx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //}

    //public class MultiTrans : Element, IMultiTrans
    //{
    //    public Params Paramss_High { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Params Paramss_Mid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Params Paramss_Low { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Trans_Params Trans_Paramss_High { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Trans_Params Trans_Paramss_Mid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Trans_Params Trans_Paramss_Low { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Ktr_High { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Ktr_Mid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Ktr_Low { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Unom_High { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Unom_Mid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Unom_Low { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Snom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Pxx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Qxx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public double Ixx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //}
}
