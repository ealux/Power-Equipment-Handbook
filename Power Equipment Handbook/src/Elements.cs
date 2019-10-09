using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    public sealed class Line : Line_Base
    {
        public Line(int Unom, string type_name,
                    string type = "L", string name = "",
                    int region = 0) : base(Unom, type_name, type, name, region)
        {

        }
    }

    public sealed class Trans : Trans_Base
    {
        public Trans(int Unom, string type_name,
                     string type = "T", string name = "",
                     int region = 0) : base(Unom, type_name, type, name, region)
        {

        }
    }

    public sealed class MultiTrans : MultiTrans_Base
    {
        public MultiTrans(int Unom, string type_name,
                         string type = "MT", string name = "",
                         int region = 0) : base(Unom, type_name, type, name, region)
        {

        }
    }
}
