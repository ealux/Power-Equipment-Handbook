﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Equipment_Handbook.src
{
    public sealed class MultiTrans : MultiTrans_Base
    {
        public MultiTrans(string type, string type_name, 
                         int Unom, string name = "", 
                         int region = 0) : base(type, type_name, Unom, name, region)
        {
        }
    }
}
