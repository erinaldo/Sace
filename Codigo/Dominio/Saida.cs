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
        public const int TIPO_SAIDA_DEPOSITO = 4;
        public const int TIPO_CONSUMO_INTERNO = 5;
        public const int TIPO_PRODUTOS_DANIFICADOS = 6;
        public const int TIPO_DEVOLUCAO_FRONECEDOR = 7;
        
        public Int64 CodSaida { get; set; }
        public DateTime DataSaida { get; set; }
        public Int32 TipoSaida { get; set; }
        public Int64 CodCliente { get; set; }
        public String CpfCnpj { get; set; }
        public Int64 CodProfissional { get; set; }
        public Int32 NumeroCartaoVenda { get; set; }
        public String PedidoGerado { get; set; }
        public decimal Total { get; set; }
        public decimal TotalAVista { get; set; }
        public decimal Desconto { get; set; }
        public decimal TotalPago { get; set; }
        public decimal TotalLucro { get; set; }
        public Int32 CodSituacaoPagamentos { get; set; }
        public Decimal Troco { get; set; }
        public Boolean EntregaRealizada { get; set; }
        public String Nfe { get; set; }
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

    }
}