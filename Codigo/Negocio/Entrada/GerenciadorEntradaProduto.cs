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
using System.Data.Objects;

namespace Negocio
{
    public class GerenciadorEntradaProduto 
    {
        private static GerenciadorEntradaProduto gEntradaProduto;
        private static SaceEntities saceContext;
        private static RepositorioGenerico<EntradaProdutoE> repEntradaProduto;
        
        public static GerenciadorEntradaProduto GetInstance(SaceEntities context)
        {
            if (gEntradaProduto == null)
            {
                gEntradaProduto = new GerenciadorEntradaProduto();
            }
            if (context == null)
            {
                repEntradaProduto = new RepositorioGenerico<EntradaProdutoE>();
            }
            else
            {
                repEntradaProduto = new RepositorioGenerico<EntradaProdutoE>(context);
            }
            saceContext = (SaceEntities)repEntradaProduto.ObterContexto();
            return gEntradaProduto;
        }

        /// <summary>
        /// Insere uma novo produto na entrada
        public Int64 Inserir(EntradaProduto entradaProduto, int codTipoEntrada)
        {

            if (entradaProduto.Quantidade == 0)
                throw new NegocioException("A quantidade do produto não pode ser igual a zero.");
            else if (entradaProduto.PrecoVendaVarejo <= 0)
                throw new NegocioException("O preço de venda deve ser maior que zero.");
            else if (entradaProduto.QuantidadeEmbalagem <= 0)
                throw new NegocioException("A quantidade de produtos em cada embalagem deve ser maior que zero.");

            //DbTransaction transaction = null;
            try
            {
                EntradaProdutoE _entradaProdutoE = new EntradaProdutoE();
                Atribuir(entradaProduto, _entradaProdutoE);

                //if (saceContext.Connection.State == System.Data.ConnectionState.Closed)
                //    saceContext.Connection.Open();
                //transaction = saceContext.Connection.BeginTransaction();

                saceContext.AddToEntradaProdutoSet(_entradaProdutoE);
                saceContext.SaveChanges();

                if ((codTipoEntrada == Entrada.TIPO_ENTRADA) || (codTipoEntrada == Entrada.TIPO_ENTRADA_AUX))
                {
                    // Incrementa o estoque na loja principal
                    GerenciadorProdutoLoja.GetInstance(saceContext).AdicionaQuantidade((entradaProduto.Quantidade * entradaProduto.QuantidadeEmbalagem), 0, Global.LOJA_PADRAO, entradaProduto.CodProduto);

                    Produto produto = GerenciadorProduto.GetInstance().Obter(new ProdutoPesquisa() { CodProduto = entradaProduto.CodProduto });
                    Atribuir(entradaProduto, produto);

                    if (entradaProduto.FornecedorEhFabricante)
                        produto.CodFabricante = entradaProduto.CodFornecedor;
                    
                    // Atualiza os dados do produto se não foi na entrada padrão
                    if (entradaProduto.CodEntrada != Global.ENTRADA_PADRAO) 
                        GerenciadorProduto.GetInstance().Atualizar(produto);
                }
                //transaction.Commit();
                return _entradaProdutoE.codEntrada;
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                throw new DadosException("EntradaProduto", e.Message, e);
            }
            //finally
            //{
            //    saceContext.Connection.Close();
            //}
        }

        
        /// <summary>
        /// Atualiza os dados de um produto na entrada
        /// </summary>
        /// <param name="entradaProduto"></param>
        public void Atualizar(EntradaProduto entradaProduto)
        {
            try
            {
                var query = from entradaProdutoE in saceContext.EntradaProdutoSet
                            where entradaProdutoE.codEntradaProduto == entradaProduto.CodEntradaProduto
                            select entradaProdutoE;

                foreach (EntradaProdutoE _entradaProdutoE in query)
                {
                    Atribuir(entradaProduto, _entradaProdutoE);
                }
                repEntradaProduto.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DadosException("EntradaProduto", e.Message, e);
            }
        }

        /// <summary>
        /// REmove um produto de uma entrada
        /// </summary>
        /// <param name="codEntradaProduto"></param>
        public void Remover(EntradaProduto entradaProduto, int codTipoEntrada)
        {
            //DbTransaction transaction = null;
            try
            {
                //if (saceContext.Connection.State == System.Data.ConnectionState.Closed)
                //    saceContext.Connection.Open();
                //transaction = saceContext.Connection.BeginTransaction();


                repEntradaProduto.Remover(ep => ep.codEntradaProduto == entradaProduto.CodEntradaProduto);
                repEntradaProduto.SaveChanges();

                if ((codTipoEntrada == Entrada.TIPO_ENTRADA) || (codTipoEntrada == Entrada.TIPO_ENTRADA_AUX))
                {
                    // Decrementa o estoque na loja principal
                    GerenciadorProdutoLoja.GetInstance(saceContext).AdicionaQuantidade((entradaProduto.Quantidade * entradaProduto.QuantidadeEmbalagem * (-1)), 0, Global.LOJA_PADRAO, entradaProduto.CodProduto);
                }
                //transaction.Commit();
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                throw new DadosException("EntradaProduto", e.Message, e);
            }
            finally
            {
                saceContext.Connection.Close();
            }
        }

        /// <summary>
        /// Consulta para retornar dados da entidade
        /// </summary>
        /// <returns></returns>
        private IQueryable<EntradaProduto> GetQuery()
        {
            var query = from entradaProduto in saceContext.EntradaProdutoSet
                        join produto in saceContext.ProdutoSet on entradaProduto.codProduto equals produto.codProduto
                        select new EntradaProduto
                        {
                            BaseCalculoICMS = (decimal)entradaProduto.baseCalculoICMS,
                            BaseCalculoICMSST = (decimal)entradaProduto.baseCalculoICMSST,
                            Cfop = entradaProduto.cfop,
                            CodCST = entradaProduto.codCST,
                            CodEntrada = entradaProduto.codEntrada,
                            CodEntradaProduto = entradaProduto.codEntradaProduto,
                            CodProduto = entradaProduto.codProduto,
                            NomeProduto = produto.nome,
                            DataValidade = (DateTime)entradaProduto.data_validade,
                            Desconto = (decimal)entradaProduto.desconto,
                            PrecoCusto = (decimal)entradaProduto.preco_custo,
                            Quantidade = (decimal)entradaProduto.quantidade,
                            QuantidadeDisponivel = (decimal)entradaProduto.quantidade_disponivel,
                            QuantidadeEmbalagem = (decimal)entradaProduto.quantidadeEmbalagem,
                            UnidadeCompra = entradaProduto.unidadeCompra,
                            ValorUnitario = (decimal)entradaProduto.valorUnitario
                        };
            return query;
        }

        /// <summary>
        /// Obter uma determinada Entrada Produto
        /// </summary>
        /// <param name="codEntradaProduto"></param>
        /// <returns></returns>
        public IEnumerable<EntradaProduto> Obter(long codEntradaProduto)
        {
            return GetQuery().Where(ep => ep.CodEntradaProduto == codEntradaProduto).ToList();
        }

        /// <summary>
        /// Obter por Entrada 
        /// </summary>
        /// <param name="codEntradaProduto"></param>
        /// <returns></returns>
        public IEnumerable<EntradaProduto> ObterPorEntrada(long codEntrada)
        {
            return GetQuery().Where(ep => ep.CodEntrada == codEntrada).ToList();
        }

        /// <summary>
        /// Obter todas as Entrada Produto
        /// </summary>
        /// <param name="codEntradaProduto"></param>
        /// <returns></returns>
        public IEnumerable<EntradaProduto> Obter(long codEntrada, long codProduto)
        {
            return GetQuery().Where(ep => ep.CodEntrada == codEntrada && ep.CodProduto == codProduto).ToList();
        }

        /// <summary>
        /// Obter produtos disponíveis ordenado pela data de validade
        /// </summary>
        /// <param name="codProduto"></param>
        /// <returns></returns>
        public IEnumerable<EntradaProduto> ObterDisponiveisOrdenadoPorValidade(long codProduto)
        {
            return GetQuery().Where(ep => ep.CodProduto == codProduto
                && ep.QuantidadeDisponivel > 0).OrderBy(ep => ep.DataValidade).ToList();
        }

        /// <summary>
        /// Obter produtos ordenado pela data de validade
        /// </summary>
        /// <param name="codProduto"></param>
        /// <returns></returns>
        public IEnumerable<EntradaProduto> ObterOrdenadoPorValidade(long codProduto)
        {
            return GetQuery().Where(ep => ep.CodProduto == codProduto).OrderBy(ep => ep.DataValidade).ToList();
        }

        /// <summary>
        /// Obtém os lotes que já foram vendidos ordenados pela data de validade
        /// </summary>
        /// <param name="codProduto"></param>
        /// <returns></returns>
        private List<EntradaProduto> ObterVendidosOrdenadoPorValidade(long codProduto)
        {
            return GetQuery().Where(ep => ep.CodProduto == codProduto && ep.Quantidade > ep.QuantidadeDisponivel).
                OrderBy(ep => ep.DataValidade).ToList();
        }

        /// <summary>
        /// Obtém a data do lote de produtos disponíveis mais antigo do estoque
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        public DateTime GetDataProdutoMaisAntigoEstoque(ProdutoPesquisa produto)
        {
            List<EntradaProduto> entradaProdutos = (List<EntradaProduto>) ObterDisponiveisOrdenadoPorValidade(produto.CodProduto);
            if (entradaProdutos.Count > 0)
            {
                return entradaProdutos[0].DataValidade;
            }
            return DateTime.Now;
        }

        public IEnumerable<EntradaProduto> ObterPorProdutoTipoEntrada(long codProduto, int tipoEntrada)
        {
            var repEntradaProduto = new RepositorioGenerico<EntradaProdutoE>();

            var saceEntities = (SaceEntities)repEntradaProduto.ObterContexto();
            var query = from entradaProduto in saceEntities.EntradaProdutoSet
                        join entrada in saceEntities.EntradaSet on entradaProduto.codEntrada equals entrada.codEntrada
                        where entrada.codTipoEntrada == Entrada.TIPO_ENTRADA && entradaProduto.codProduto == codProduto 
                        select new EntradaProduto
                        {
                            BaseCalculoICMS = (decimal)entradaProduto.baseCalculoICMS,
                            BaseCalculoICMSST = (decimal)entradaProduto.baseCalculoICMSST,
                            Cfop = entradaProduto.cfop,
                            CodCST = entradaProduto.codCST,
                            CodEntrada = entradaProduto.codEntrada,
                            CodEntradaProduto = entradaProduto.codEntradaProduto,
                            CodProduto = entradaProduto.codProduto,
                            DataValidade = (DateTime)entradaProduto.data_validade,
                            Desconto = (decimal)entradaProduto.desconto,
                            PrecoCusto = (decimal)entradaProduto.preco_custo,
                            Quantidade = (decimal)entradaProduto.quantidade,
                            QuantidadeDisponivel = (decimal)entradaProduto.quantidade_disponivel,
                            QuantidadeEmbalagem = (decimal)entradaProduto.quantidadeEmbalagem,
                            UnidadeCompra = entradaProduto.unidadeCompra,
                            ValorUnitario = (decimal)entradaProduto.valorUnitario
                        };
            return query.ToList();
        }

        
        /// <summary>
        /// Estorna dos lotes do estoque uma determinada quantidade
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="dataValidade"></param>
        /// <param name="quantidadeDevolvida"></param>
        private void EstornarItensVendidosEstoque(ProdutoPesquisa produto, DateTime dataValidade, Decimal quantidadeDevolvida)
        {
            List<EntradaProduto> entradaProdutos = ObterVendidosOrdenadoPorValidade(produto.CodProduto);
            Decimal quantidadeRetornada = 0;

            if (entradaProdutos != null)
            {
                if (produto.TemVencimento)
                {
                    foreach (EntradaProduto entradaProduto in entradaProdutos)
                    {
                        if ((entradaProduto.DataValidade.Date.Equals(dataValidade.Date)) && (quantidadeRetornada < quantidadeDevolvida))
                        {
                            if (entradaProduto.Quantidade <= (entradaProduto.QuantidadeDisponivel + (quantidadeDevolvida - quantidadeRetornada)))
                            {
                                entradaProduto.QuantidadeDisponivel += quantidadeDevolvida - quantidadeRetornada;
                                quantidadeRetornada += quantidadeDevolvida - quantidadeRetornada;
                                Atualizar(entradaProduto);
                            }
                        }
                        if (quantidadeDevolvida == quantidadeRetornada)
                            break;
                    }
                }

                // acontece quando uma data de validade não existe no estoque
                if (quantidadeRetornada < quantidadeDevolvida)
                {

                    foreach (EntradaProduto entradaProduto in entradaProdutos)
                    {
                        if (entradaProduto.Quantidade >= (entradaProduto.QuantidadeDisponivel + (quantidadeDevolvida - quantidadeRetornada)))
                        {
                            entradaProduto.QuantidadeDisponivel += quantidadeDevolvida - quantidadeRetornada;
                            quantidadeRetornada += quantidadeDevolvida - quantidadeRetornada;
                        }
                        else
                        {
                            quantidadeRetornada += entradaProduto.Quantidade - entradaProduto.QuantidadeDisponivel;
                            entradaProduto.QuantidadeDisponivel = entradaProduto.Quantidade;
                        }
                        Atualizar(entradaProduto);
                        if (quantidadeDevolvida == quantidadeRetornada)
                            break;
                    }
                }
            }
            
            // acontece quando uma data de validade não existe no estoque
            if (quantidadeRetornada < quantidadeDevolvida)
            {
                BaixarItensVendidosEstoqueEntradaPadrao(produto, ((-1) * (quantidadeDevolvida-quantidadeRetornada)));
            }
        }
        
        /// <summary>
        /// Baixa dos lotes do estoque uma determinada quantidade vendida
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="dataValidade"></param>
        /// <param name="quantidadeVendida"></param>
        /// <returns></returns>
        public Decimal BaixarItensVendidosEstoque(ProdutoPesquisa produto, DateTime dataValidade, Decimal quantidadeVendida) {
            List<EntradaProduto> entradaProdutos = (List<EntradaProduto>) ObterDisponiveisOrdenadoPorValidade(produto.CodProduto);

            decimal somaPrecosCusto = 0;
            decimal quantidadeBaixas = 0;

            if (quantidadeVendida < 0)
            {
                EstornarItensVendidosEstoque(produto, dataValidade, Math.Abs(quantidadeVendida));
            } 
            else if (entradaProdutos.Count > 0)
            {
                // reduz a quantidade de itens disponíveis nos lotes de entrada
                foreach (EntradaProduto entradaProduto in entradaProdutos)
                {
                    if (quantidadeVendida == quantidadeBaixas)
                        break;
                    if (produto.TemVencimento)
                    {
                        // quando produto tem vencimento
                        if ((entradaProduto.DataValidade.Date.Equals(dataValidade.Date)) && (quantidadeBaixas < quantidadeVendida))
                        {
                            if (entradaProduto.QuantidadeDisponivel >= (quantidadeVendida - quantidadeBaixas))
                            {
                                entradaProduto.QuantidadeDisponivel -= quantidadeVendida - quantidadeBaixas;
                                somaPrecosCusto += (quantidadeVendida - quantidadeBaixas) * entradaProduto.PrecoCusto;
                                quantidadeBaixas += (quantidadeVendida - quantidadeBaixas);
                            }
                        }
                    }
                    else
                    {
                        if (quantidadeBaixas < quantidadeVendida)
                        {
                            if (entradaProduto.QuantidadeDisponivel >= (quantidadeVendida - quantidadeBaixas))
                            {
                                entradaProduto.QuantidadeDisponivel -= quantidadeVendida - quantidadeBaixas;
                                somaPrecosCusto += (quantidadeVendida - quantidadeBaixas) * entradaProduto.PrecoCusto;
                                quantidadeBaixas += (quantidadeVendida - quantidadeBaixas);
                            }
                            else
                            {
                                quantidadeBaixas += entradaProduto.QuantidadeDisponivel;
                                somaPrecosCusto += entradaProduto.QuantidadeDisponivel * entradaProduto.PrecoCusto;
                                entradaProduto.QuantidadeDisponivel = 0;
                            }
                        }
                    }
                    Atualizar(entradaProduto);
                }
            }

            if (quantidadeBaixas < quantidadeVendida)
            {
                somaPrecosCusto += BaixarItensVendidosEstoqueEntradaPadrao(produto, (quantidadeVendida - quantidadeBaixas));
            }
            saceContext.SaveChanges();
            return somaPrecosCusto;
        }

        /// <summary>
        /// Baixar uma certa quantidade de produtos da entrada padrão que contém todos os produtos sem entradas associadas
        /// </summary>
        /// <param name="produtoPesquisa"></param>
        /// <param name="quantidade"></param>
        /// <returns></returns>
        private decimal BaixarItensVendidosEstoqueEntradaPadrao(ProdutoPesquisa produtoPesquisa, decimal quantidade) {
            List<EntradaProduto> entradaProdutos = (List<EntradaProduto>) Obter(Global.ENTRADA_PADRAO, produtoPesquisa.CodProduto);
            Produto produto = GerenciadorProduto.GetInstance().Obter(produtoPesquisa);
            EntradaProduto entradaProduto = null;
            if (entradaProdutos.Count > 0)
            {
                entradaProduto = entradaProdutos[0];
            }

            if (entradaProduto != null)
            {
                entradaProduto.QuantidadeDisponivel -= quantidade;
                Atualizar(entradaProduto);
            }
            else
            {
                entradaProduto = new EntradaProduto();
                entradaProduto.CodEntrada = Global.ENTRADA_PADRAO;
                entradaProduto.CodProduto = produto.CodProduto; 

                entradaProduto.UnidadeCompra = produto.Unidade;
                entradaProduto.Quantidade = (produto.TemVencimento) ? quantidade : 10000;
                entradaProduto.QuantidadeEmbalagem = 1;
                entradaProduto.ValorUnitario = produto.UltimoPrecoCompra;
                //entradaProduto.ValorTotal = 0;
                entradaProduto.BaseCalculoICMS = 0;
                entradaProduto.BaseCalculoICMSST = 0;
                entradaProduto.DataValidade = DateTime.Now;
                // para não perder os dados do produto
                entradaProduto.LucroPrecoVendaAtacado = produto.LucroPrecoVendaAtacado;
                entradaProduto.LucroPrecoVendaVarejo = produto.LucroPrecoVendaVarejo;
                entradaProduto.PrecoVendaAtacado = produto.PrecoVendaAtacado;
                entradaProduto.PrecoVendaVarejo = produto.PrecoVendaVarejo;
                entradaProduto.Frete = produto.Frete;
                entradaProduto.Icms = produto.Icms;
                entradaProduto.IcmsSubstituto = produto.IcmsSubstituto;
                entradaProduto.Ipi = produto.Ipi;
                entradaProduto.Ncmsh = produto.Ncmsh;
                entradaProduto.QtdProdutoAtacado = produto.QtdProdutoAtacado;
                entradaProduto.DataEntrada = produto.DataUltimoPedido;
                entradaProduto.Desconto = produto.Desconto;
                entradaProduto.PrecoCusto = produto.PrecoCusto; 
                entradaProduto.QuantidadeDisponivel = (produto.TemVencimento) ? 0 : 10000 - quantidade; 
                entradaProduto.ValorDesconto = 0;
                entradaProduto.CodCST = produto.CodCST;
                entradaProduto.Cfop = 9999;

                Inserir(entradaProduto, Entrada.TIPO_ENTRADA);
            }
            
            return 0;
        }

        /// <summary>
        /// Atribuição de entradaproduto para atualizar dados de produto
        /// </summary>
        /// <param name="entradaProduto"></param>
        /// <param name="produto"></param>
        private static void Atribuir(EntradaProduto entradaProduto, Produto produto)
        {
            produto.LucroPrecoVendaAtacado = entradaProduto.LucroPrecoVendaAtacado;
            produto.LucroPrecoVendaVarejo = entradaProduto.LucroPrecoVendaVarejo;
            produto.PrecoVendaAtacado = entradaProduto.PrecoVendaAtacado;
            produto.PrecoVendaVarejo = entradaProduto.PrecoVendaVarejo;
            produto.Frete = entradaProduto.Frete;
            produto.Icms = entradaProduto.Icms;
            produto.IcmsSubstituto = entradaProduto.IcmsSubstituto;
            produto.Ipi = entradaProduto.Ipi;
            produto.Ncmsh = entradaProduto.Ncmsh;
            produto.QtdProdutoAtacado = entradaProduto.QtdProdutoAtacado;
            produto.UltimaDataAtualizacao = DateTime.Now;
            produto.DataUltimoPedido = entradaProduto.DataEntrada;
            produto.UltimoPrecoCompra = entradaProduto.ValorUnitario / entradaProduto.QuantidadeEmbalagem;
            produto.CodCST = entradaProduto.CodCST;
            produto.Desconto = entradaProduto.Desconto;
            produto.QuantidadeEmbalagem = entradaProduto.QuantidadeEmbalagem;
            produto.UnidadeCompra = entradaProduto.UnidadeCompra;
        }

        /// <summary>
        /// Atribui a entidade para entidade persistente
        /// </summary>
        /// <param name="entradaProduto"></param>
        /// <param name="_entradaProdutoE"></param>
        private static void Atribuir(EntradaProduto entradaProduto, EntradaProdutoE _entradaProdutoE)
        {
            _entradaProdutoE.baseCalculoICMS = entradaProduto.BaseCalculoICMS;
            _entradaProdutoE.baseCalculoICMSST = entradaProduto.BaseCalculoICMSST;
            _entradaProdutoE.cfop = entradaProduto.Cfop;
            _entradaProdutoE.codCST = entradaProduto.CodCST;
            _entradaProdutoE.codEntrada = entradaProduto.CodEntrada;
            _entradaProdutoE.codEntradaProduto = entradaProduto.CodEntradaProduto;
            _entradaProdutoE.codProduto = entradaProduto.CodProduto;
            _entradaProdutoE.data_validade = entradaProduto.DataValidade;
            _entradaProdutoE.desconto = entradaProduto.Desconto;
            _entradaProdutoE.preco_custo = entradaProduto.PrecoCusto;
            _entradaProdutoE.quantidade = entradaProduto.Quantidade;
            _entradaProdutoE.quantidade_disponivel = entradaProduto.QuantidadeDisponivel;
            _entradaProdutoE.quantidadeEmbalagem = entradaProduto.QuantidadeEmbalagem;
            _entradaProdutoE.unidadeCompra = entradaProduto.UnidadeCompra;
            _entradaProdutoE.valorICMS = entradaProduto.ValorICMS;
            _entradaProdutoE.valorICMSST = entradaProduto.ValorICMSST;
            _entradaProdutoE.valorIPI = entradaProduto.ValorIPI;
            _entradaProdutoE.valorTotal = entradaProduto.ValorTotal;
            _entradaProdutoE.valorUnitario = entradaProduto.ValorUnitario;
        }

        
    }
}