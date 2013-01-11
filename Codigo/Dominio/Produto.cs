﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace Dominio
{
    public class Produto : ProdutoPesquisa
    {
        public string UnidadeCompra { get; set; }
        public decimal QuantidadeEmbalagem { get; set; }
        public int CodGrupo { get; set; }
        public int CodSubgrupo { get; set; }
        public long CodFabricante { get; set; }
        public string ReferenciaFabricante { get; set; }
        public int Cfop { get; set; }
        public string Ncmsh { get; set; }
        public decimal Icms { get; set; }
        public decimal IcmsSubstituto { get; set; }
        public decimal Simples { get; set; }
        public decimal Ipi { get; set; }
        public decimal Frete { get; set; }
        public decimal Desconto { get; set; }
        public decimal UltimoPrecoCompra { get; set; }
        public decimal PrecoCusto
        {
            get 
            {
                if (Cfop == Global.VENDA_NORMAL)
                return CalculaPrecoCustoNormal(UltimoPrecoCompra, Icms,
                    Simples, Ipi, Frete, 0, 0);
                return CalculaPrecoCustoSubstituicao(UltimoPrecoCompra, IcmsSubstituto,
                    Simples, Ipi, Frete, 0, 0);
            }
        }
        public decimal LucroPrecoVendaVarejo { get; set; }
        public decimal PrecoVendaVarejoSugestao
        {
            get { return PrecoCusto + (PrecoCusto * LucroPrecoVendaVarejo / 100); }
        }
        public decimal LucroPrecoVendaAtacado { get; set; }
        public decimal PrecoVendaAtacadoSugestao
        {
            get { return PrecoCusto + (PrecoCusto * LucroPrecoVendaAtacado / 100); }
        }
        public Boolean ExibeNaListagem { get; set; }
        public DateTime DataUltimoPedido { get; set; }
        public Byte CodSituacaoProduto { get; set; }

        public decimal CalculaPrecoCustoNormal(decimal precoCompra, decimal creditoICMS, decimal simples, decimal ipi, decimal frete, decimal manutencao, decimal desconto)
        {
            // return precoCompra + (precoCompra * (1 / (100 - (Global.ICMS_LOCAL - creditoICMS))) * 10) + (precoCompra * (1 / (100 - simples)) * 10) +
            //    (precoCompra * ipi / 100) + (precoCompra * frete / 100) + (precoCompra * manutencao / 100);

            decimal precoCalculado = precoCompra + (precoCompra * (Global.ICMS_LOCAL - creditoICMS) / 100);
            precoCalculado = precoCalculado + (precoCalculado * simples / 100);
            precoCalculado = precoCalculado + (precoCalculado * ipi / 100);
            precoCalculado = precoCalculado + (precoCalculado * frete / 100);
            precoCalculado = precoCalculado + (precoCalculado * manutencao / 100);
            precoCalculado = precoCalculado - (precoCalculado * desconto / 100);
            return precoCalculado;
        }

        public decimal CalculaPrecoCustoSubstituicao(decimal precoCompra, decimal ICMSSubstituicao, decimal simples, decimal ipi, decimal frete, decimal manutencao, decimal desconto)
        {
            // return precoCompra + (precoCompra * (1 / (100 - ICMSSubstituicao)) *10) + (precoCompra * (1 / (100 - simples))*10) +
            //    (precoCompra * ipi / 100) + (precoCompra * frete / 100) + (precoCompra * manutencao / 100);

            decimal precoCalculado = precoCompra + (precoCompra * ICMSSubstituicao / 100);
            precoCalculado = precoCalculado + (precoCalculado * simples / 100);
            precoCalculado = precoCalculado + (precoCalculado * ipi / 100);
            precoCalculado = precoCalculado + (precoCalculado * frete / 100);
            precoCalculado = precoCalculado + (precoCalculado * manutencao / 100);
            precoCalculado = precoCalculado - (precoCalculado * desconto / 100);
            return precoCalculado;
        }

    }
}