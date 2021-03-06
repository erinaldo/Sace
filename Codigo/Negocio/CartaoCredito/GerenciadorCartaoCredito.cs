﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dados;
using Dominio;

namespace Negocio
{
    public class GerenciadorCartaoCredito
    {
        private static GerenciadorCartaoCredito gCartaoCredito;
        
        public static GerenciadorCartaoCredito GetInstance()
        {
            if (gCartaoCredito == null)
            {
                gCartaoCredito = new GerenciadorCartaoCredito();
            }
            return gCartaoCredito;
        }

        /// <summary>
        /// Insere os dados de um cartão de crédito
        /// </summary>
        /// <param name="cartaoCredito"></param>
        /// <returns></returns>
        public Int64 Inserir(CartaoCredito cartaoCredito)
        {
            try
            {
                var repCartaoCredito = new RepositorioGenerico<CartaoCreditoE>();

                CartaoCreditoE _cartaoCredito = new CartaoCreditoE();
                Atribuir(cartaoCredito, _cartaoCredito);
                
                repCartaoCredito.Inserir(_cartaoCredito);
                repCartaoCredito.SaveChanges();
                
                return _cartaoCredito.codCartao;
            }
            catch (Exception e)
            {
                throw new DadosException("Cartão de Crédito", e.Message, e);
            }
        }
        
        /// <summary>
        /// Atualiza os dados de um cartão de crédito
        /// </summary>
        /// <param name="cartaoCredito"></param>
        public void Atualizar(CartaoCredito cartaoCredito)
        {
            try
            {
                var repCartaoCredito = new RepositorioGenerico<CartaoCreditoE>();

                CartaoCreditoE _cartaoCredito = repCartaoCredito.ObterEntidade(cartao => cartao.codCartao == cartaoCredito.CodCartao);
                Atribuir(cartaoCredito, _cartaoCredito);

                repCartaoCredito.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DadosException("Cartão de Crédito", e.Message, e);
            }
        }

        /// <summary>
        /// Remove os dados de um cartão de crédito
        /// </summary>
        /// <param name="codCartaoCredito"></param>
        public void Remover(int codCartaoCredito)
        {
            try
            {
                var repCartaoCredito = new RepositorioGenerico<CartaoCreditoE>();

                if (codCartaoCredito == 1)
                {
                    throw new NegocioException("Esse cartão de crédito não pode ser removido");
                }
                repCartaoCredito.Remover(cartao => cartao.codCartao == codCartaoCredito);
                repCartaoCredito.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DadosException("Cartão de Crédito", e.Message, e);
            }
        }


        /// <summary>
        /// Consulta padrão para retornar dados do cartão de crédito
        /// </summary>
        /// <returns></returns>
        private IQueryable<CartaoCredito> GetQuery()
        {
            var repCartaoCredito = new RepositorioGenerico<CartaoCreditoE>();
            var saceEntities = (SaceEntities)repCartaoCredito.ObterContexto();
            var query = from cartao in saceEntities.CartaoCreditoSet
                        join contaBanco in saceEntities.ContaBancoSet on cartao.codContaBanco equals contaBanco.codContaBanco
                        join pessoa in saceEntities.PessoaSet on cartao.codPessoa equals pessoa.codPessoa

                        select new CartaoCredito
                        {
                            CodCartao = cartao.codCartao,
                            CodContaBanco = cartao.codContaBanco,
                            CodPessoa = (int) cartao.codPessoa,
                            DiaBase = (int) cartao.diaBase,
                            Mapeamento = cartao.mapeamento,
                            Nome = cartao.nome,
                            DescricaoContaBanco = contaBanco.descricao,
                            NomePessoa = pessoa.nomeFantasia,
                            Desconto = cartao.desconto,
                            MapeamentoCappta = cartao.mapeamentoCappta,
                            TipoCartao = cartao.tipoCartao
                        };
            return query;
         
        }

        /// <summary>
        /// Obtém todos os cartões de crédito cadastrados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CartaoCredito> ObterTodos()
        {
            return GetQuery().ToList();
        }

        /// <summary>
        /// Obtém os dados do cartão pelo código
        /// </summary>
        /// <param name="codCartaoCredito"></param>
        /// <returns>código do cartão</returns>
        public IEnumerable<CartaoCredito> Obter(Int32 codCartaoCredito)
        {
            return GetQuery().Where(cartao => cartao.CodCartao == codCartaoCredito).ToList();
        }


        /// <summary>
        /// Obtém os dados do cartão pelo código
        /// </summary>
        /// <param name="codCartaoCredito"></param>
        /// <returns>código do cartão</returns>
        public IEnumerable<CartaoCredito> ObterPorMapeamentoCappta(String nomeBandeira)
        {
            return GetQuery().Where(cartao => cartao.MapeamentoCappta == nomeBandeira).ToList();
        }
        /// <summary>
        /// Obtém os dados do cartão pelo nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns>nome do cartão</returns>
        public IEnumerable<CartaoCredito> ObterPorNome(string nome)
        {
            return GetQuery().Where(cartao => cartao.Nome.StartsWith(nome)).ToList();
        }

        /// <summary>
        /// Atribuição ente entidade e entidade persistente
        /// </summary>
        /// <param name="cartaoCredito"></param>
        /// <param name="_cartaoCredito"></param>
        private void Atribuir(CartaoCredito cartaoCredito, CartaoCreditoE _cartaoCredito)
        {
            _cartaoCredito.codCartao = cartaoCredito.CodCartao;
            _cartaoCredito.codContaBanco = cartaoCredito.CodContaBanco;
            _cartaoCredito.codPessoa = cartaoCredito.CodPessoa;
            _cartaoCredito.diaBase = cartaoCredito.DiaBase;
            _cartaoCredito.mapeamento = cartaoCredito.Mapeamento;
            _cartaoCredito.nome = cartaoCredito.Nome;
            _cartaoCredito.desconto = cartaoCredito.Desconto;
            _cartaoCredito.mapeamentoCappta = cartaoCredito.MapeamentoCappta;
            _cartaoCredito.tipoCartao = cartaoCredito.TipoCartao;
        }
        
    }
}