using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Группа основных параметров элемента
    /// </summary>
    public class Params
    {
        public double R { get; set; }
        public double X { get; set; }
        public double G { get; set; }
        public double B { get; set; }

        /// <summary>
        /// Параметры активные и реактивные элемента сети (для трансформаторов и линий)
        /// </summary>
        /// <param name="r">Акт. сопротивление, Ом</param>
        /// <param name="x">Реакт. сопротивление, Ом</param>
        /// <param name="g">Акт. проводимость, мкрСм</param>
        /// <param name="b">Реакт. проводимость, мкрСм</param>
        public Params(double r = 0, double x = 0,
                      double g = 0, double b = 0)
        {
            R = r;
            X = x;
            G = g;
            B = b;
        }
    }

    /// <summary>
    /// Группа дополнительных параметров линии
    /// </summary>
    public class Line_Params
    {
        public double Lin_R { get; set; }
        public double Lin_X { get; set; }
        public double Lin_G { get; set; }
        public double Lin_B { get; set; }
        public double Length { get; set; } = 0;

        /// <summary>
        /// Группа дополнительных параметров линии
        /// </summary>
        /// <param name="r0">Удельное акт. сопротивление, Ом/км</param>
        /// <param name="x0">Удельное реакт. сопротивление, Ом/км</param>
        /// <param name="g0">Удельная акт. проводимость, мкрСм/км</param>
        /// <param name="b0">Удельное реакт. проводимость, мкрСм/км</param>
        /// <param name="length">Длина линии, км</param>
        public Line_Params(double r0 = 0, double x0 = 0,
                           double g0 = 0, double b0 = 0,
                           double length = 0)
        {
            Lin_R = r0;
            Lin_X = x0;
            Lin_G = g0;
            Lin_B = b0;
            Length = length;
        }
    }

    /// <summary>
    /// Группа дополнительных параметров трансформатора
    /// </summary>
    public class Trans_Params
    {
        public double Ukz { get; set; }
        public double Pkz { get; set; }
        /// <summary>
        /// Группа дополнительных параметров трансформатора
        /// </summary>
        /// <param name="ukz">Напряжение короткого замыкания, %</param>
        /// <param name="pkz">Потери мощности короткого замыкания, кВт</param>
        public Trans_Params(double ukz = 0, double pkz = 0)
        {
            Ukz = ukz;
            Pkz = pkz;
        }
    }
}
