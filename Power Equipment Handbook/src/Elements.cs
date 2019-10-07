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

    /// <summary>
    /// Группа параметров элемента
    /// </summary>
    internal class Params
    {
        public double R { get; set; }
        public double X { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double _Lin_R { get; set; }
        public double _Lin_X { get; set; }
        public double _Lin_G { get; set; }
        public double _Lin_B { get; set; }
        
        public double Length { get; set; } = 0;

        /// <summary>
        /// Параметры активные и реактивные элемента сети (для трансформаторов и линий)
        /// </summary>
        /// <param name="r0">Удельное акт. сопротивление, Ом/км</param>
        /// <param name="x0">Удельное реакт. сопротивление, Ом/км</param>
        /// <param name="g0">Удельная акт. проводимость, мкрСм/км</param>
        /// <param name="b0">Удельное реакт. проводимость, мкрСм/км</param>
        /// <param name="r">Акт. сопротивление, Ом</param>
        /// <param name="x">Реакт. сопротивление, Ом</param>
        /// <param name="g">Акт. проводимость, мкрСм</param>
        /// <param name="b">Реакт. проводимость, мкрСм</param>
        /// <param name="length">Длина линии, км</param>
        public Params(double r0 = 0, double x0 = 0,
                      double g0 = 0, double b0 = 0,
                      double r = 0, double x = 0,
                      double g = 0, double b = 0,
                      double length = 0)
        {
            _Lin_R = r0;
            _Lin_X = x0;
            _Lin_G = g0;
            _Lin_B = b0;
            R = r;
            X = x;
            G = g;
            B = b;

            if(length != 0)
            {
                Length = length;
                R = r0 * length;
                X = x0 * length;
                G = g0 * length;
                B = b0 * length;
            }
        }
    }

    //public class Line: Element
    //{
    //    public Line() : base()
    //    {
    
    //    }
    //}
    //public class Trans : Element
    //{
    //    public Trans() : base()
    //    {

    //    }
    //}
}
