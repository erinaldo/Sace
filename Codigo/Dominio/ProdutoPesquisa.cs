﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace Dominio
{
    public class ProdutoPesquisa
    {
        public long CodProduto { get; set; }
        public string CodigoBarra { get; set; }
        public string Nome { get; set; }
        public string NomeProdutoFabricante { get; set; }
        public string Unidade { get; set; }
        public DateTime UltimaDataAtualizacao { get; set; }
        public Boolean TemVencimento { get; set; }
        public string CodCST { get; set; }
        public decimal PrecoVendaVarejo { get; set; }
        public decimal PrecoVendaVarejoSemDesconto
        {
            get { return Math.Round((PrecoVendaVarejo * Global.ACRESCIMO_PADRAO), 2, MidpointRounding.AwayFromZero); }
        }
        public decimal QtdProdutoAtacado { get; set; }
        public decimal PrecoVendaAtacado { get; set; }
        public decimal PrecoVendaAtacadoSemDesconto
        {
            get { return Math.Round((PrecoVendaAtacado * Global.ACRESCIMO_PADRAO), 3, MidpointRounding.AwayFromZero); }
        }
        
        public bool EhTributacaoIntegral
        {
            get
            {
                Cst cst = new Cst() { CodCST = this.CodCST };
                return cst.EhTributacaoIntegral;
            }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            return CodProduto == ((ProdutoPesquisa)obj).CodProduto;    
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return CodProduto.GetHashCode();
        }
    }
}