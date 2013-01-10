﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominio
{
    public class Cst
    {
        public string CodCST { get; set; }
        public string Descricao { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.CodCST.Equals(((Cst)obj).CodCST);    
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.CodCST.GetHashCode();
        }
    }
}
