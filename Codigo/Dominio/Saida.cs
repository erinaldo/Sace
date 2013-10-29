﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominio
{
    public class Saida
    {
        public const int TIPO_ORCAMENTO = 1;
        public const int TIPO_PRE_VENDA = 2;
        public const int TIPO_VENDA = 3;

        public const int TIPO_PRE_REMESSA = 9;
        public const int TIPO_REMESSA_DEPOSITO = 4;

        public const int TIPO_RETORNO_DEPOSITO = 11;
        public const int TIPO_PRE_RETORNO_DEPOSITO = 12;

        public const int TIPO_PRE_DEVOLUCAO = 10;
        public const int TIPO_DEVOLUCAO_FORNECEDOR = 7;
        
        //public const int TIPO_CONSUMO_INTERNO = 5;
        //public const int TIPO_PRODUTOS_DANIFICADOS = 6;

        public static List<int> LISTA_TIPOS_REMESSA_DEPOSITO = new List<int>() { Saida.TIPO_PRE_REMESSA, Saida.TIPO_REMESSA_DEPOSITO };
        public static List<int> LISTA_TIPOS_RETORNO_DEPOSITO = new List<int>() { Saida.TIPO_PRE_RETORNO_DEPOSITO, Saida.TIPO_RETORNO_DEPOSITO };
        public static List<int> LISTA_TIPOS_DEVOLUCAO_FORNECEDOR = new List<int>() { Saida.TIPO_PRE_DEVOLUCAO, Saida.TIPO_DEVOLUCAO_FORNECEDOR };
        public static List<int> LISTA_TIPOS_VENDA = new List<int>() { Saida.TIPO_ORCAMENTO, Saida.TIPO_PRE_VENDA, Saida.TIPO_VENDA };


        public long CodSaida { get; set; }
        public int CodLojaOrigem { get; set; }
        public DateTime DataSaida { get; set; }
        public int TipoSaida { get; set; }
        public long CodCliente { get; set; }
        public string CpfCnpj { get; set; }
        public long CodProfissional { get; set; }
        public long CodEntrada { get; set; }
        public int NumeroCartaoVenda { get; set; }
        public string CupomFiscal { get; set; }
        public DateTime DataEmissaoCupomFiscal { get; set; }
        public decimal Total { get; set; }
        public decimal TotalAVista { get; set; }
        public decimal Desconto { get; set; }
        public decimal TotalPago { get; set; }
        public decimal TotalLucro { get; set; }
        public int CodSituacaoPagamentos { get; set; }
        public decimal Troco { get; set; }
        public Boolean EntregaRealizada { get; set; }
        public string Nfe { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal BaseCalculoICMSSubst { get; set; }
        public decimal ValorICMSSubst { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal OutrasDespesas { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal TotalNotaFiscal { get; set; }
        public decimal QuantidadeVolumes { get; set; }
        public string EspecieVolumes { get; set; }
        public string Marca { get; set; }
        public decimal Numero { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public long CodEmpresaFrete { get; set; }
        public string DescricaoTipoSaida { get; set; }
        public string NomeCliente { get; set; }
        public string DescricaoSituacaoPagamentos { get; set; }
        public string Observacao { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.CodSaida.Equals(((Saida)obj).CodSaida);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.CodSaida.GetHashCode();
        }

        //TODO: colocar núemro da ecf no banco de dados.
        public string NumeroECF { get { return "01"; } }
    }
}