using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Класс расширений методов-валидаторов Ветвей
    /// </summary>
    public static class ValidatorBranchExtentions
    {

        /// <summary>
        /// Проверка типа Ветви
        /// </summary>
        /// <param name="node">Проверяемая Ветвь</param>
        public static void ValidateBranchType(this Branch branch)
        {
            //Check if PV
            var r = branch.R == 0.0;
            var x = branch.X == 0.0;
            var g = branch.G == 0.0;
            var b = branch.B == 0.0;


            if (branch.Type == "Тр-р")
            {
                if (branch.Ktr.HasValue & (branch.Ktr.Value == 0.0 | branch.Ktr.Value == 1))
                {
                    if(r & x & b & g) branch.Type = "Выкл.";
                    else branch.Type = "ЛЭП";
                }
            }
            else if (branch.Type == "ЛЭП")
            {
                if (branch.Ktr.HasValue && (branch.Ktr.Value != 0.0 & branch.Ktr.Value < 1)) branch.Type = "Тр-р";
                else
                {
                    if (r & x & b & g) branch.Type = "Выкл.";
                }
            }
            else if(branch.Type == "Выкл.")
            {
                if (!r | !x)
                {
                    if (branch.Ktr.HasValue && (branch.Ktr.Value != 0.0 & branch.Ktr.Value < 1)) branch.Type = "Тр-р";
                    else branch.Type = "ЛЭП";
                }
            }
        }
    }
}
