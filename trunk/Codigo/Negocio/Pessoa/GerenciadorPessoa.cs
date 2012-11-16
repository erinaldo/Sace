﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dominio;
using Dados.saceDataSetTableAdapters;
using Dados;
using Util;
using System.Data.Common;

namespace Negocio
{
    public class GerenciadorPessoa
    {
        private static GerenciadorPessoa gPessoa;
        private static tb_pessoaTableAdapter tb_pessoaTA;
        
        public static GerenciadorPessoa getInstace()
        {
            if (gPessoa == null)
            {
                gPessoa = new GerenciadorPessoa();
                tb_pessoaTA = new tb_pessoaTableAdapter();
            }
            return gPessoa;
        }

        public Int64 inserir(Pessoa pessoa)
        {
            try
            {
                byte ehFabricanteByte = (byte)(pessoa.EhFabricante ? 1 : 0);
                byte imprimirCFByte = (byte)(pessoa.ImprimirCF ? 1 : 0);
                byte imprimirDAVByte = (byte)(pessoa.ImprimirDAV ? 1 : 0);


                tb_pessoaTA.Insert(pessoa.Nome, pessoa.CpfCnpj, pessoa.Endereco, pessoa.Numero, pessoa.Bairro,
                    pessoa.Cidade, pessoa.Complemento, pessoa.Cep, pessoa.Uf, pessoa.Fone1, pessoa.Fone2, pessoa.LimiteCompra, 
                    pessoa.ValorComissao, pessoa.Observacao, pessoa.Tipo.ToString(),
                    pessoa.Ie, pessoa.IeSubstituto, pessoa.Fone3, pessoa.Email, ehFabricanteByte, imprimirDAVByte, imprimirCFByte);

                return 0;
            }
            catch (Exception e)
            {
                throw new DadosException("Pessoa", e.Message, e);
            }
        }

        public void atualizar(Pessoa pessoa)
        {

            if (pessoa.CodPessoa == Global.CLIENTE_PADRAO)
                throw new NegocioException("Os dados dessa pessoa não podem ser alterados ou removidos");
            
            try
            {
                byte ehFabricanteByte = (byte)(pessoa.EhFabricante ? 1 : 0);
                byte imprimirCFByte = (byte)(pessoa.ImprimirCF ? 1 : 0);
                byte imprimirDAVByte = (byte)(pessoa.ImprimirDAV ? 1 : 0);

                tb_pessoaTA.Update(pessoa.Nome, pessoa.CpfCnpj, pessoa.Endereco, pessoa.Numero, pessoa.Bairro,
                    pessoa.Cidade, pessoa.Complemento, pessoa.Cep, pessoa.Uf, pessoa.Fone1, pessoa.Fone2, pessoa.LimiteCompra,
                    pessoa.ValorComissao, pessoa.Observacao, pessoa.Tipo.ToString(),
                    pessoa.Ie, pessoa.IeSubstituto, pessoa.Fone3, pessoa.Email, ehFabricanteByte, imprimirDAVByte, imprimirCFByte, pessoa.CodPessoa);
            }
            catch (Exception e)
            {
                throw new DadosException("Pessoa", e.Message, e);
            }
        }

        public void remover(Int64 codpessoa)
        {
            if (codpessoa == Global.CLIENTE_PADRAO)
                throw new NegocioException("Essa pessoa não pode ser removida.");

            try
            {
                tb_pessoaTA.Delete(codpessoa);
            }
            catch (Exception e)
            {
                throw new DadosException("Pessoa", e.Message, e);
            }
        }

        public Pessoa obterPessoa(Int64 codPessoa)
        {
            Pessoa pessoa = new Pessoa();
            Dados.saceDataSetTableAdapters.tb_pessoaTableAdapter tb_pessoaTA = new tb_pessoaTableAdapter();
            Dados.saceDataSet.tb_pessoaDataTable pessoaDT = tb_pessoaTA.GetDataByCodPessoa(codPessoa);

            pessoa.CodPessoa = int.Parse(pessoaDT.Rows[0]["codPessoa"].ToString());
            pessoa.Bairro = pessoaDT.Rows[0]["bairro"].ToString();
            pessoa.Cep = pessoaDT.Rows[0]["cep"].ToString();
            pessoa.Cidade = pessoaDT.Rows[0]["cidade"].ToString();
            pessoa.Complemento = pessoaDT.Rows[0]["complemento"].ToString();
            pessoa.CpfCnpj = pessoaDT.Rows[0]["cpf_cnpj"].ToString();
            pessoa.Email = pessoaDT.Rows[0]["email"].ToString();
            pessoa.Endereco = pessoaDT.Rows[0]["endereco"].ToString();
            pessoa.Fone1 = pessoaDT.Rows[0]["fone1"].ToString();
            pessoa.Fone2 = pessoaDT.Rows[0]["fone2"].ToString();
            pessoa.Fone3 = pessoaDT.Rows[0]["fone3"].ToString();
            pessoa.Ie = pessoaDT.Rows[0]["ie"].ToString();
            pessoa.IeSubstituto = pessoaDT.Rows[0]["ieSubstituto"].ToString();
            pessoa.LimiteCompra = Convert.ToDecimal(pessoaDT.Rows[0]["limiteCompra"].ToString());
            pessoa.Nome = pessoaDT.Rows[0]["nome"].ToString();
            pessoa.Numero = pessoaDT.Rows[0]["numero"].ToString();
            pessoa.Observacao = pessoaDT.Rows[0]["observacao"].ToString();
            pessoa.Tipo = Convert.ToChar(pessoaDT.Rows[0]["tipo"].ToString());
            pessoa.Uf = pessoaDT.Rows[0]["uf"].ToString();
            pessoa.ValorComissao = Convert.ToDecimal(pessoaDT.Rows[0]["valorComissao"].ToString());

            return pessoa;
        }

        public Pessoa obterPessoaNomeIgual(String nome)
        {
            Pessoa pessoa = null;
            Dados.saceDataSetTableAdapters.tb_pessoaTableAdapter tb_pessoaTA = new tb_pessoaTableAdapter();
            Dados.saceDataSet.tb_pessoaDataTable pessoaDT = tb_pessoaTA.GetDataByEqualsName(nome);

            if (pessoaDT.Count > 0)
            {
                pessoa = new Pessoa();
                pessoa.CodPessoa = int.Parse(pessoaDT.Rows[0]["codPessoa"].ToString());
                pessoa.Bairro = pessoaDT.Rows[0]["bairro"].ToString();
                pessoa.Cep = pessoaDT.Rows[0]["cep"].ToString();
                pessoa.Cidade = pessoaDT.Rows[0]["cidade"].ToString();
                pessoa.Complemento = pessoaDT.Rows[0]["complemento"].ToString();
                pessoa.CpfCnpj = pessoaDT.Rows[0]["cpf_cnpj"].ToString();
                pessoa.Email = pessoaDT.Rows[0]["email"].ToString();
                pessoa.Endereco = pessoaDT.Rows[0]["endereco"].ToString();
                pessoa.Fone1 = pessoaDT.Rows[0]["fone1"].ToString();
                pessoa.Fone2 = pessoaDT.Rows[0]["fone2"].ToString();
                pessoa.Fone3 = pessoaDT.Rows[0]["fone3"].ToString();
                pessoa.Ie = pessoaDT.Rows[0]["ie"].ToString();
                pessoa.IeSubstituto = pessoaDT.Rows[0]["ieSubstituto"].ToString();
                pessoa.LimiteCompra = Convert.ToDecimal(pessoaDT.Rows[0]["limiteCompra"].ToString());
                pessoa.Nome = pessoaDT.Rows[0]["nome"].ToString();
                pessoa.Numero = pessoaDT.Rows[0]["numero"].ToString();
                pessoa.Observacao = pessoaDT.Rows[0]["observacao"].ToString();
                pessoa.Tipo = Convert.ToChar(pessoaDT.Rows[0]["tipo"].ToString());
                pessoa.Uf = pessoaDT.Rows[0]["uf"].ToString();
                pessoa.ValorComissao = Convert.ToDecimal(pessoaDT.Rows[0]["valorComissao"].ToString());
            }

            return pessoa;
        }
    }
}
