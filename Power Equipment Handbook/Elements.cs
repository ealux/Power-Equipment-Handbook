using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook
{
    abstract class Element
    {
        public string Name { get; set; } //Имя элемента (АС, ТРДН и т.д.)
        public string Type { get; set; } //Тип элемента (Линия (L), Трансформатор (T))
        public int Unom { get; set; } //Класс номинального напряжения в кВ
        public int Region { get; set; } //Район

        public Params Params { get; set; } //Параметры

        public Element()
        {

        }
    }

    /// <summary>
    /// Группа параметров элемента
    /// </summary>
    class Params
    {
        public double R { get; set; } //Активное сопротивление R (Ом)
        public double X { get; set; } //Реактивное сопротивление X (Ом)
        public double G { get; set; } //Активная проводимость G (мкрСм)
        public double B { get; set; } //Реактивная проводимость B (мкрСм)
        public double _Lin_R { get; set; } //Погонное R (r0)
        public double _Lin_X { get; set; } //Погонное X (x0)
        public double _Lin_G { get; set; } //Погонное G (g0)
        public double _Lin_B { get; set; } //Погонное B (b0)
        
        public double Length { get; set; } = 0; //Длина (для линий)

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

    class Line: Element
    {

    }
    class Trans : Element
    {

    }
}
