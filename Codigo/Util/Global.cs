using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public static class Global
    {
        public static int
        PRODUTOS = 1,
        PESSOAS = 2,
        EMPRESAS = 3,
        LOJAS = 4,
        BANCOS = 5,
        PLANO_DE_CONTAS = 6,
        CONTAS_BANCO_CAIXA = 7,
        CARTÕES_DE_CREDITO = 8,
        GRUPOS_DE_PRODUTOS = 9,
        GRUPOS_DE_CONTAS = 10,
        USUARIOS = 11,
        SAIDA = 12,
        PEDIDOS = 13,
        ENTRADA_PRODUTOS = 14,
        TRANFORMACOES = 15,
        CONTAS_PAGAR = 16,
        CONTAS_RECEBER = 17,
        BAIXAS = 18,
        MOVIMENTACAO_CONTAS = 19,
        ATUALIZAÇÃO_PREÇOS = 20,
        AJUSTE_ESTOQUE = 21,
        CONFIGURAÇÕES_SISTEMA = 22,
        BACKUP = 23,
        RELATORIO_CLIENTE = 24,
        RELATORIO_LISTAGEM_PRECOS = 25,
        RELATORIO_ESTOQUE = 26,
        RELATORIO_VENDAS = 27,
        RELATORIO_COMPRAS = 28,
        RELATORIO_CONTAS = 29,
        LOJA_PADRAO = 1,
        CLIENTE_PADRAO = 1,
        PROFISSIONAL_PADRAO = 1,
        VENDA_NORMAL = 5102,
        VENDA_ST = 5403,
        VENDA_OUTRAS = 9999,
        QUANTIDADE_DIAS_CREDIARIO = 30,
        DOCUMENTO_PADRAO = 1;

            
        public static Decimal
            ACRESCIMO_PADRAO = (decimal) 1.111111,
            DESCONTO_PADRAO = (decimal) 0.9,
            SIMPLES = (decimal) 8.0,
            ICMS_LOCAL = (decimal)19,
            CUSTO_MANUTENCAO_LOJA = 0;

        public static String
            ENDERECO_SERVIDOR = "\\\\SERVIDOR\\C\\",
            PASTA_COMUNICACAO_SERVIDOR = ENDERECO_SERVIDOR+"MKS\\PEDIDOS\\",
            PASTA_COMUNICACAO_SERVIDOR_RETORNO = ENDERECO_SERVIDOR +"MKS\\CAIXA\\Retorno\\",
            PASTA_COMUNICACAO_SERVIDOR_BACKUP = ENDERECO_SERVIDOR + "MKS\\CAIXA\\BACKUP\\",
            PASTA_BACKUP = "C:\\BACKUP_SACE\\",
            PASTA_MYSQL_SERVER = "C:\\Arquivos de Programas\\MySQL\\MySQL Server 5.5\\",
            NOME_SERVIDOR = "SERVIDOR",
            SGBD_NOME = "sace",
            SGBD_USUARIO = "sace",
            SGBD_SENHA = "sace",
            SGBD_IP = "192.168.0.3",
            SERIAL_IMPRESSORA = "COM3:",
            PORTA_IMPRESSORA = "LPT1";
            
    }
}