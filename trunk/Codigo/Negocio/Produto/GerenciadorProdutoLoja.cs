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
    public class GerenciadorProdutoLoja 
    {
        private static GerenciadorProdutoLoja gProdutoLoja;
        private static RepositorioGenerico<ProdutoLojaE, SaceEntities> repProdutoLoja;
        
        public static GerenciadorProdutoLoja GetInstance()
        {
            if (gProdutoLoja == null)
            {
                gProdutoLoja = new GerenciadorProdutoLoja();
                repProdutoLoja = new RepositorioGenerico<ProdutoLojaE, SaceEntities>("chave");
            }
            return gProdutoLoja;
        }

        /// <summary>
        /// Insere um novo produto na loja
        /// </summary>
        /// <param name="produtoLoja"></param>
        /// <returns></returns>
        public Int64 Inserir(ProdutoLoja produtoLoja)
        {
            try
            {
                ProdutoLojaE _produtoLojaE = new ProdutoLojaE();
                _produtoLojaE.codLoja = produtoLoja.CodLoja;
                _produtoLojaE.codProduto = produtoLoja.CodProduto;
                _produtoLojaE.estoqueMaximo = produtoLoja.EstoqueMaximo;
                _produtoLojaE.localizacao = produtoLoja.Localizacao;
                _produtoLojaE.localizacao2 = produtoLoja.Localizacao2;
                _produtoLojaE.qtdEstoque = produtoLoja.QtdEstoque;
                _produtoLojaE.qtdEstoqueAux = produtoLoja.QtdEstoqueAux;

                repProdutoLoja.Inserir(_produtoLojaE);
                repProdutoLoja.SaveChanges();
                return produtoLoja.CodProduto;
            }
            catch (Exception e)
            {
                throw new DadosException("Produto Loja", e.Message, e);
            }
        }

        /// <summary>
        /// Atualiza os dados de um produto na loja
        /// </summary>
        /// <param name="produtoLoja"></param>
        public void Atualizar(ProdutoLoja produtoLoja)
        {
            try
            {
                ProdutoLojaE _produtoLojaE = repProdutoLoja.ObterEntidade(pl => pl.codProduto == produtoLoja.CodProduto && pl.codLoja == produtoLoja.CodLoja);
                _produtoLojaE.estoqueMaximo = produtoLoja.EstoqueMaximo;
                _produtoLojaE.localizacao = produtoLoja.Localizacao;
                _produtoLojaE.localizacao2 = produtoLoja.Localizacao2;
                _produtoLojaE.qtdEstoque = produtoLoja.QtdEstoque;
                _produtoLojaE.qtdEstoqueAux = produtoLoja.QtdEstoqueAux;

                repProdutoLoja.SaveChanges();
                
                AtualizarEstoqueEntradasProduto(produtoLoja.CodProduto);
            }
            catch (Exception e)
            {
                throw new DadosException("Produto", e.Message, e);
            }
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="produtoLojaPK"></param>
        public void Remover(long codProduto, int codLoja)
        {
            try
            {
                repProdutoLoja.Remover(pl => pl.codProduto == codProduto && pl.codLoja == codLoja);
                repProdutoLoja.SaveChanges();
                AtualizarEstoqueEntradasProduto(codProduto);
            }
            catch (Exception e)
            {
                throw new DadosException("Produto", e.Message, e);
            }
        }

        /// <summary>
        /// Consulta simples para retornar dados da entidade
        /// </summary>
        /// <returns></returns>
        private IQueryable<ProdutoLoja> GetQuery()
        {
            var saceEntities = (SaceEntities)repProdutoLoja.ObterContexto();
            var query = from produtoLoja in saceEntities.ProdutoLojaSet
                        select new ProdutoLoja
                        {
                            CodLoja = produtoLoja.codLoja,
                            CodProduto = produtoLoja.codProduto,
                            EstoqueMaximo = produtoLoja.estoqueMaximo,
                            Localizacao = produtoLoja.localizacao,
                            Localizacao2 = produtoLoja.localizacao2,
                            QtdEstoque = produtoLoja.qtdEstoque,
                            QtdEstoqueAux = produtoLoja.qtdEstoqueAux
                        };
            return query;
        }

        /// <summary>
        /// Obter produto na loja
        /// </summary>
        /// <param name="codProduto"></param>
        /// <param name="codLoja"></param>
        /// <returns></returns>
        public IEnumerable<ProdutoLoja> Obter(long codProduto, int codLoja)
        {
            return GetQuery().Where(pl => pl.CodProduto == codProduto && pl.CodLoja == codLoja).ToList();
        }

        /// <summary>
        /// Obter dados de um produto em várias lojas
        /// </summary>
        /// <param name="codProduto"></param>
        /// <returns></returns>
        public IEnumerable<ProdutoLoja> ObterPorProduto(long codProduto)
        {
            return GetQuery().Where(pl => pl.CodProduto == codProduto).ToList();
        }

       
        /// <summary>
        /// Adiciona quantida e quantidadeAux ao produto loja
        /// </summary>
        /// <param name="quantidade"></param>
        /// <param name="quantidadeAux"></param>
        /// <param name="codLoja"></param>
        /// <param name="codProduto"></param>
        public void AdicionaQuantidade(decimal quantidade, decimal quantidadeAux, Int32 codLoja, long codProduto)
        {
            ProdutoLojaE _produtoLojaE = repProdutoLoja.ObterEntidade(pl => pl.codProduto == codProduto && pl.codLoja == codLoja);


            if (_produtoLojaE != null)
            {
                _produtoLojaE.qtdEstoque += quantidade;
                _produtoLojaE.qtdEstoqueAux += quantidadeAux;
            }
            else
            {
                _produtoLojaE = new ProdutoLojaE();
                _produtoLojaE.codLoja = codLoja;
                _produtoLojaE.codProduto = codProduto;
                _produtoLojaE.qtdEstoque = quantidade;
                _produtoLojaE.qtdEstoqueAux = quantidadeAux;
                repProdutoLoja.Inserir(_produtoLojaE);
            }
            repProdutoLoja.SaveChanges();
        }

        
        private void AtualizarEstoqueEntradasProduto(long codProduto)
        {
            IEnumerable<ProdutoLoja> listaProdutosLoja = ObterPorProduto(codProduto);


            decimal quantidadeEstoquePrincipalLojas = listaProdutosLoja.Sum(pl => pl.QtdEstoque);
            decimal quantidadeEstoqueAuxLojas = listaProdutosLoja.Sum(pl => pl.QtdEstoqueAux);
            
            decimal quantidadeEstoquePrincipalEntradaProduto = GerenciadorEntradaProduto.getInstace().ObterEstoquePrincipalDisponivel(codProduto);
            decimal quantidadeEstoqueAuxEntradaProduto = GerenciadorEntradaProduto.getInstace().ObterEstoqueAuxDisponivel(codProduto);
            
            // Atualiza as entradas principais com os valores do estoque totais dos produto / loja
            if (quantidadeEstoquePrincipalLojas != quantidadeEstoquePrincipalEntradaProduto)
            {
                List<EntradaProduto> entradasProduto = GerenciadorEntradaProduto.getInstace().ObterEntradasPrincipais(codProduto);

                for (int i = 0; (entradasProduto != null) && (i < entradasProduto.Count); i++)
                {
                    // Vai decremetar o contador até organizar a quantidade disponível dos lotes de entrada
                    if (quantidadeEstoquePrincipalLojas > 0)
                    {
                        if (entradasProduto[i].Quantidade < quantidadeEstoquePrincipalLojas)
                        {
                            entradasProduto[i].QuantidadeDisponivel = entradasProduto[i].Quantidade;
                            quantidadeEstoquePrincipalLojas -= entradasProduto[i].Quantidade;
                        }
                        else
                        {
                            entradasProduto[i].QuantidadeDisponivel = quantidadeEstoquePrincipalLojas;
                            quantidadeEstoquePrincipalLojas = 0;
                        }
                    }
                    else
                    {
                        entradasProduto[i].QuantidadeDisponivel = 0;
                    }
                    GerenciadorEntradaProduto.getInstace().atualizar(entradasProduto[i]);
                }

            }


            // Atualiza as entradas auxiliares com os valores do estoque totais dos produto / loja
            if (quantidadeEstoqueAuxLojas != quantidadeEstoqueAuxEntradaProduto)
            {
                List<EntradaProduto> entradasProduto = GerenciadorEntradaProduto.getInstace().ObterEntradasAuxiliar(codProduto);

                for (int i = 0; (entradasProduto != null) && (i < entradasProduto.Count); i++)
                {
                    // Vai decremetar o contador até organizar a quantidade disponível dos lotes de entrada
                    if (quantidadeEstoqueAuxLojas > 0)
                    {
                        if (entradasProduto[i].Quantidade < quantidadeEstoqueAuxLojas)
                        {
                            entradasProduto[i].QuantidadeDisponivel = entradasProduto[i].Quantidade;
                            quantidadeEstoqueAuxLojas -= entradasProduto[i].Quantidade;
                        }
                        else
                        {
                            entradasProduto[i].QuantidadeDisponivel = quantidadeEstoqueAuxLojas;
                            quantidadeEstoqueAuxLojas = 0;
                        }
                    }
                    else
                    {
                        entradasProduto[i].QuantidadeDisponivel = 0;
                    }
                    GerenciadorEntradaProduto.getInstace().atualizar(entradasProduto[i]);
                }

            }
        }
    }
}