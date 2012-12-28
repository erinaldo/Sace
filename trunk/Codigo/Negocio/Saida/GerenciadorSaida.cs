﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Dominio;
using Dados.saceDataSetTableAdapters;
using Dados;
using Util;
using System.Data.Common;
using System.IO.Ports;


namespace Negocio
{
    public class GerenciadorSaida
    {
        private static GerenciadorSaida gSaida;
        private static tb_saidaTableAdapter tb_SaidaTA;
        private static tb_saida_pedidoTableAdapter tb_saida_pedidoTA;

        public static GerenciadorSaida getInstace()
        {
            if (gSaida == null)
            {
                gSaida = new GerenciadorSaida();
                tb_SaidaTA = new tb_saidaTableAdapter();
                tb_saida_pedidoTA = new tb_saida_pedidoTableAdapter();
            }
            return gSaida;
        }

        /// <summary>
        /// Insere dados de uma saída (orçamento/pré-venda/venda/saída depósito)
        /// </summary>
        /// <param name="saida"></param>
        /// <returns></returns>
        public Int64 inserir(Saida saida)
        {
            try
            {

                tb_SaidaTA.Insert(saida.DataSaida, saida.CodCliente, saida.TipoSaida, saida.CodProfissional, saida.NumeroCartaoVenda, saida.PedidoGerado, saida.Total, saida.TotalAVista, saida.Desconto,
                    saida.TotalPago, saida.TotalLucro, saida.CpfCnpj, saida.CodSituacaoPagamentos, saida.Troco, saida.EntregaRealizada,
                    saida.Nfe, saida.BaseCalculoICMS, saida.ValorICMS, saida.BaseCalculoICMSSubst, saida.ValorICMSSubst,
                    saida.ValorFrete, saida.ValorSeguro, saida.OutrasDespesas, saida.ValorIPI, saida.TotalNotaFiscal,
                    saida.QuantidadeVolumes, saida.EspecieVolumes, saida.Marca, saida.Numero, saida.PesoBruto, saida.PesoLiquido, saida.CodEmpresaFrete);
                
                return (Int64)tb_SaidaTA.GetUltimoCodSaida();


            }
            catch (Exception e)
            {
                throw new DadosException("Saída", e.Message, e);
            }
        }

        /// <summary>
        /// Atualiza dados de uma saída (orçamento/pré-venda/venda/saída depósito)
        /// </summary>
        /// <param name="saida"></param>
        public void atualizar(Saida saida)
        {
            try
            {
                tb_SaidaTA.Update(saida.DataSaida, saida.CodCliente, saida.TipoSaida, saida.CodProfissional,
                    saida.NumeroCartaoVenda, saida.PedidoGerado, saida.Total, saida.TotalAVista, saida.Desconto,
                    saida.TotalPago, saida.TotalLucro, saida.CpfCnpj, saida.CodSituacaoPagamentos, saida.Troco, saida.EntregaRealizada,
                    saida.Nfe, saida.BaseCalculoICMS, saida.ValorICMS, saida.BaseCalculoICMSSubst, saida.ValorICMSSubst,
                    saida.ValorFrete, saida.ValorSeguro, saida.OutrasDespesas, saida.ValorIPI, saida.TotalNotaFiscal,
                    saida.QuantidadeVolumes, saida.EspecieVolumes, saida.Marca, saida.Numero, saida.PesoBruto, saida.PesoLiquido, saida.CodEmpresaFrete, saida.CodSaida);
            }
            catch (Exception e)
            {
                throw new DadosException("Saída", e.Message, e);
            }
        }

        /// <summary>
        /// Atualizar situação pagamentos de uma saída
        /// </summary>
        /// <param name="codSituacaoPagamentos"></param>
        /// <param name="codSaida"></param>
        public void atualizar(int codSituacaoPagamentos, long codSaida)
        {
            try
            {
                tb_SaidaTA.UpdateSituacaoPagamentos(codSituacaoPagamentos, codSaida);
            }
            catch (Exception e)
            {
                throw new DadosException("Saida", e.Message, e);
            }
        }

        /// <summary>
        /// Remove os dados de uma saída. No caso de vendas e pré-vendas transforma em orçamento.
        /// </summary>
        /// <param name="saida"></param>
        public void remover(Saida saida)
        {
            try
            {
                if (saida.TipoSaida == Saida.TIPO_ORCAMENTO)
                {
                    GerenciadorSaidaPagamento.getInstace().removerPagamentos(saida);
                    tb_SaidaTA.Delete(saida.CodSaida);
                }
                else if (saida.TipoSaida.Equals(Saida.TIPO_PRE_VENDA) || saida.TipoSaida.Equals(Saida.TIPO_VENDA) )
                {
                    GerenciadorSaidaPagamento.getInstace().removerPagamentos(saida);
                    List<SaidaProduto> saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutos(saida.CodSaida);
                    registrarEstornoEstoque(saidaProdutos);
                    saida.TipoSaida = Saida.TIPO_ORCAMENTO;
                    saida.PedidoGerado = "";
                    atualizar(saida);
                }
                else if (saida.TipoSaida == Saida.TIPO_SAIDA_DEPOSITO)
                {
                    // para garantir que nenhuma outra estãção emitiu a nota de transferência
                    saida = GerenciadorSaida.getInstace().obterSaida(saida.CodSaida);
                    if ((saida.Nfe != null) && (!saida.Nfe.Equals(""))) { 
                        throw new NegocioException("Não é possível remover uma saída para depósito cuja nota fiscal de trasnferência já foi emitida.");
                    } else {
                        tb_SaidaTA.Delete(saida.CodSaida);
                    }
                }
                else if (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FRONECEDOR)
                {
                    // para garantir que nenhuma outra estãção emitiu a nota de transferência
                    saida = GerenciadorSaida.getInstace().obterSaida(saida.CodSaida);
                    if ((saida.Nfe != null) && (!saida.Nfe.Equals("")))
                    {
                        throw new NegocioException("Não é possível remover uma devolução para fornecedor cuja nota fiscal de transferência já foi emitida.");
                    }
                    else
                    {
                        tb_SaidaTA.Delete(saida.CodSaida);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DadosException("Saída", e.Message, e);
            }
        }

        /// <summary>
        /// Obtme todos os dados de uma saída
        /// </summary>
        /// <param name="codSaida"></param>
        /// <returns></returns>
        public Saida obterSaida(Int64 codSaida)
        {
            Saida saida = null;
            Dados.saceDataSetTableAdapters.tb_saidaTableAdapter tb_saidaTA = new tb_saidaTableAdapter();
            Dados.saceDataSet.tb_saidaDataTable saidaDT = tb_saidaTA.GetDataByCodSaida(codSaida);

            if (saidaDT.Count > 0)
            {
                saida = new Saida();
                saida.CodSaida = codSaida;
                saida.DataSaida = DateTime.Parse(saidaDT.Rows[0]["dataSaida"].ToString());
                saida.TipoSaida = int.Parse(saidaDT.Rows[0]["codTipoSaida"].ToString());
                saida.CodCliente = long.Parse(saidaDT.Rows[0]["codCliente"].ToString());
                saida.CodProfissional = long.Parse(saidaDT.Rows[0]["codProfissional"].ToString());
                saida.NumeroCartaoVenda = int.Parse(saidaDT.Rows[0]["numeroCartaoVenda"].ToString());
                saida.PedidoGerado = saidaDT.Rows[0]["pedidoGerado"].ToString();
                saida.Desconto = decimal.Parse(saidaDT.Rows[0]["desconto"].ToString());
                if (!saidaDT.Rows[0]["total"].ToString().Equals(""))
                {
                    saida.Total = decimal.Parse(saidaDT.Rows[0]["total"].ToString());
                    saida.TotalAVista = decimal.Parse(saidaDT.Rows[0]["totalAVista"].ToString());
                    saida.TotalLucro = decimal.Parse(saidaDT.Rows[0]["totalLucro"].ToString());
                    saida.TotalPago = decimal.Parse(saidaDT.Rows[0]["totalPago"].ToString());
                }
                saida.CodSituacaoPagamentos = Convert.ToInt32(saidaDT.Rows[0]["codSituacaoPagamentos"].ToString());
                saida.EntregaRealizada = Convert.ToBoolean(saidaDT.Rows[0]["entregaRealizada"].ToString());
                saida.Nfe = saidaDT.Rows[0]["nfe"].ToString();
                saida.Troco = Convert.ToDecimal(saidaDT.Rows[0]["troco"].ToString());
                saida.CpfCnpj = saidaDT.Rows[0]["cpf_cnpj"].ToString();
                saida.BaseCalculoICMS = Convert.ToDecimal(saidaDT.Rows[0]["baseCalculoICMS"].ToString());
                saida.ValorICMS = Convert.ToDecimal(saidaDT.Rows[0]["valorICMS"].ToString());
                saida.BaseCalculoICMSSubst = Convert.ToDecimal(saidaDT.Rows[0]["baseCalculoICMSSubst"].ToString());
                saida.ValorICMSSubst = Convert.ToDecimal(saidaDT.Rows[0]["valorICMSSubst"].ToString());
                saida.ValorFrete = Convert.ToDecimal(saidaDT.Rows[0]["valorFrete"].ToString());
                saida.ValorSeguro = Convert.ToDecimal(saidaDT.Rows[0]["valorSeguro"].ToString());
                saida.OutrasDespesas = Convert.ToDecimal(saidaDT.Rows[0]["outrasDespesas"].ToString());
                saida.ValorIPI = Convert.ToDecimal(saidaDT.Rows[0]["valorIPI"].ToString());
                saida.TotalNotaFiscal = Convert.ToDecimal(saidaDT.Rows[0]["totalNotaFiscal"].ToString());
                saida.QuantidadeVolumes = Convert.ToDecimal(saidaDT.Rows[0]["quantidadeVolumes"].ToString());
                saida.EspecieVolumes = saidaDT.Rows[0]["especieVolumes"].ToString();
                saida.Marca = saidaDT.Rows[0]["marca"].ToString();
                saida.Numero = Convert.ToDecimal(saidaDT.Rows[0]["numero"].ToString());
                saida.PesoBruto = Convert.ToDecimal(saidaDT.Rows[0]["pesoBruto"].ToString());
                saida.PesoLiquido = Convert.ToDecimal(saidaDT.Rows[0]["pesoLiquido"].ToString());
                saida.CodEmpresaFrete = Convert.ToInt64(saidaDT.Rows[0]["codEmpresaFrete"].ToString());
                saida.DescricaoTipoSaida = saidaDT.Rows[0]["descricaoTipoSaida"].ToString();
                saida.NomeCliente = saidaDT.Rows[0]["nomeCliente"].ToString();
                saida.DescricaoSituacaoPagamentos = saidaDT.Rows[0]["descricaoSituacaoPagamentos"].ToString();
            }
            return saida;
        }

        /// <summary>
        /// Encerra uma saída fazendo movimentações de estoque e lançamentos no contas a pagar/receber
        /// </summary>
        /// <param name="saida"></param>
        /// <param name="tipoSaidaEncerramento"></param>
        public void encerrar(Saida saida, int tipoSaidaEncerramento)
        {
            if (saida.TipoSaida.Equals(Saida.TIPO_ORCAMENTO) && tipoSaidaEncerramento.Equals(Saida.TIPO_ORCAMENTO))
            {
                saida.TipoSaida = Saida.TIPO_ORCAMENTO;
                atualizar(saida);
            }
            else if (saida.TipoSaida.Equals(Saida.TIPO_ORCAMENTO) && tipoSaidaEncerramento.Equals(Saida.TIPO_PRE_VENDA))
            {
                saida.TipoSaida = Saida.TIPO_PRE_VENDA;
                saida.CodSituacaoPagamentos = SituacaoPagamentos.LANCADOS;

                List<SaidaProduto> saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutos(saida.CodSaida);
                Decimal somaPrecosCusto = registrarBaixaEstoque(saidaProdutos);

                saida.TotalLucro = saida.TotalAVista - somaPrecosCusto;
                 atualizar(saida);

                List<SaidaPagamento> saidaPagamentos = GerenciadorSaidaPagamento.getInstace().obterSaidaPagamentos(saida.CodSaida);
                registrarPagamentosSaida(saidaPagamentos, saida);
            }
            else if (tipoSaidaEncerramento.Equals(Saida.TIPO_SAIDA_DEPOSITO))
            {
                saida.TipoSaida = Saida.TIPO_SAIDA_DEPOSITO;
                if ((saida.Nfe != null) && (!saida.Nfe.Equals("")))
                {
                    throw new NegocioException("Não é possível finalizar uma saída para Depósito cuja nota fiscal já foi emitida.");
                }

                Loja lojaDestino = GerenciadorLoja.getInstace().obterByCodPessoa(saida.CodCliente);
                if (lojaDestino.CodLoja == Global.LOJA_PADRAO)
                {
                    throw new NegocioException("Não pode ser feita transferência de produtos para a mesma loja.");
                }

                List<SaidaProduto> saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutos(saida.CodSaida);
                saida.Nfe = ObterNumeroProximaNotaFiscal().ToString();
                atualizar(saida);
                registrarTransferenciaEstoque(saidaProdutos, Global.LOJA_PADRAO, lojaDestino.CodLoja);
            }
            else if (tipoSaidaEncerramento.Equals(Saida.TIPO_DEVOLUCAO_FRONECEDOR))
            {
                saida.TipoSaida = Saida.TIPO_DEVOLUCAO_FRONECEDOR;
                if ((saida.Nfe == null) || (saida.Nfe.Equals("")))
                {
                    saida.Nfe = ObterNumeroProximaNotaFiscal().ToString();
                    List<SaidaProduto> saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutos(saida.CodSaida);
                    registrarBaixaEstoque(saidaProdutos);
                }
            }
        }
        /// <summary>
        /// Calculo do toatal da nota de acordo com a existência de substituição tributária
        /// </summary>
        /// <param name="saida"></param>
        /// <returns></returns>
        public decimal ObterTotalNotaFiscal(Saida saida)
        {
            if (saida.BaseCalculoICMSSubst > 0)
                return saida.Total + saida.ValorICMSSubst + saida.ValorFrete + saida.ValorSeguro - saida.Desconto + saida.OutrasDespesas + saida.ValorIPI;
            else
                return saida.Total + saida.ValorICMS + saida.ValorFrete + saida.ValorSeguro - saida.Desconto + saida.OutrasDespesas + saida.ValorIPI;
        }
        
        /// <summary>
        /// Verifica na saída se produto tem data de vencimento aceitável.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="dataVencimento"></param>
        /// <returns></returns>
        public Boolean dataVencimentoProdutoAceitavel(Produto produto, DateTime dataVencimento)
        {
            if (produto.TemVencimento)
            {
                DateTime dataMaisAntigo = GerenciadorEntradaProduto.getInstace().getDataProdutoMaisAntigoEstoque(produto);
                return (dataMaisAntigo >= dataVencimento);
            }
            return true;
        }

        /// <summary>
        /// Regista formas de pagamentos de uma saída
        /// </summary>
        /// <param name="pagamentos"></param>
        /// <param name="saida"></param>
        public void registrarPagamentosSaida(List<SaidaPagamento> pagamentos, Saida saida)
        {
            decimal totalRegistrado = 0;

            foreach (SaidaPagamento pagamento in pagamentos)
            {

                List<Conta> contas = GerenciadorConta.GetInstance().ObterPorSaidaPagamento(saida.CodSaida, pagamento.CodSaidaPagamento).ToList();

                if (contas.Count > 0)
                {
                    totalRegistrado += pagamento.Valor;
                    continue;
                }
                // Para cada pagamento é criada uma nova conta
                Conta conta = new Conta();
                conta.CodPessoa = saida.CodCliente;
                conta.CodPlanoConta = PlanoConta.SAIDA_PRODUTOS;
                conta.CodSaida = saida.CodSaida;
                conta.CodEntrada = Global.ENTRADA_PADRAO; // entrada não válida
                conta.CodPessoa = saida.CodCliente;
                conta.CodPagamento = pagamento.CodSaidaPagamento;
                conta.Desconto = 0;

                // Quando o pagamento é realizado em dinheiro a conta já é inserida quitada
                if (pagamento.CodFormaPagamento == FormaPagamento.DINHEIRO)
                    conta.CodSituacao = Conta.SITUACAO_QUITADA.ToString();
                else
                    conta.CodSituacao = Conta.SITUACAO_ABERTA.ToString();

                if (pagamento.CodFormaPagamento == FormaPagamento.CARTAO)
                {
                    conta.CodPessoa = GerenciadorCartaoCredito.GetInstance().Obter(pagamento.CodCartaoCredito).ElementAt(0).CodPessoa;
                }

                conta.CodDocumento = pagamento.CodDocumentoPagamento;
                conta.TipoConta = Conta.CONTA_RECEBER.ToString();

                if (((totalRegistrado + pagamento.Valor) >= saida.TotalAVista) && (pagamento.CodFormaPagamento == FormaPagamento.DINHEIRO))
                {
                    conta.Valor = (saida.TotalAVista - totalRegistrado) / pagamento.Parcelas;
                }
                else if (pagamento.CodFormaPagamento == FormaPagamento.CREDIARIO) {
                        conta.Valor = (saida.Total / pagamento.Parcelas);
                        conta.Desconto = (saida.Total - saida.TotalAVista) / pagamento.Parcelas;
                }
                else
                {
                    conta.Valor = pagamento.Valor / pagamento.Parcelas;
                }

                //Int64 codConta = -1;

                for (int i = 0; i < pagamento.Parcelas; i++)
                {
                    if (pagamento.CodFormaPagamento == (FormaPagamento.CARTAO))
                    {
                        CartaoCredito cartaoCredito = GerenciadorCartaoCredito.GetInstance().Obter(pagamento.CodCartaoCredito).ElementAt(0);
                        pagamento.Data = pagamento.Data.AddDays((double) cartaoCredito.DiaBase);
                        conta.DataVencimento = pagamento.Data;
                    }
                    else if ((pagamento.CodFormaPagamento == FormaPagamento.BOLETO) || (pagamento.CodFormaPagamento == FormaPagamento.CHEQUE))
                    {
                        DocumentoPagamento documento = GerenciadorDocumentoPagamento.getInstace().obterDocumentoPagamento(pagamento.CodDocumentoPagamento);
                        conta.DataVencimento = documento.DataVencimento;
                        conta.Valor = documento.Valor;
                    }
                    else if ((pagamento.CodFormaPagamento == FormaPagamento.CREDIARIO) || (pagamento.CodFormaPagamento == FormaPagamento.DEPOSITO) ||
                      (pagamento.CodFormaPagamento == FormaPagamento.PROMISSORIA))
                    {
                        conta.DataVencimento = saida.DataSaida.AddDays(pagamento.IntervaloDias);
                    }
                    else
                    {
                        conta.DataVencimento = pagamento.Data;
                    }

                    conta.CodConta = GerenciadorConta.GetInstance().Inserir(conta);
                }

                totalRegistrado += pagamento.Valor;



                if (pagamento.CodFormaPagamento == FormaPagamento.DINHEIRO)
                {
                    MovimentacaoConta movimentacao = new MovimentacaoConta();
                    movimentacao.CodContaBanco = pagamento.CodContaBanco;
                    movimentacao.CodConta = conta.CodConta;
                    movimentacao.CodResponsavel = saida.CodCliente;
                    movimentacao.DataHora = DateTime.Now;
                    if (totalRegistrado > saida.TotalAVista)
                    {
                        movimentacao.Valor = pagamento.Valor - saida.Troco;
                    }
                    else
                    {
                        movimentacao.Valor = pagamento.Valor;
                    }

                    movimentacao.CodTipoMovimentacao = (movimentacao.Valor > 0) ? MovimentacaoConta.RECEBIMENTO_CLIENTE : MovimentacaoConta.DEVOLUCAO_CLIENTE;
                    
                    GerenciadorMovimentacaoConta.getInstace().inserir(movimentacao, conta);
                }
            }
        }

        /// <summary>
        /// Registra estorno de estoque no caso de devolução ou exclusão da saída
        /// </summary>
        /// <param name="saidaProdutos"></param>
        public void registrarEstornoEstoque(List<SaidaProduto> saidaProdutos)
        {
            foreach (SaidaProduto saidaProduto in saidaProdutos)
            {
                Produto produto = GerenciadorProduto.getInstace().obterProduto(saidaProduto.CodProduto);

                if (produto.CodCST != Produto.ST_OUTRAS)
                {
                    GerenciadorProdutoLoja.getInstace().adicionaQuantidade(saidaProduto.Quantidade, 0, Global.LOJA_PADRAO, saidaProduto.CodProduto);
                }
                else
                {
                    GerenciadorProdutoLoja.getInstace().adicionaQuantidade(0, saidaProduto.Quantidade, Global.LOJA_PADRAO, saidaProduto.CodProduto);
                }

                GerenciadorEntradaProduto.getInstace().baixaItensVendidosEstoque(produto, saidaProduto.DataValidade, saidaProduto.Quantidade);
            }
        }

        /// <summary>
        /// Decrementa a quantidade de produtos na loja matriz e atualiza o lote de
        /// entrada determinando que produtos foram vendidos de um determinado lote.
        /// </summary>
        /// <param name="saidaProdutos"></param>
        /// <returns> A soma dos preços de custo dos produtos baixados para determinar o lucro</returns>
        private Decimal registrarBaixaEstoque(List<SaidaProduto> saidaProdutos)
        {
            Decimal somaPrecosCusto = 0;
            foreach (SaidaProduto saidaProduto in saidaProdutos)
            {
                Produto produto = GerenciadorProduto.getInstace().obterProduto(saidaProduto.CodProduto);
                decimal custoAtual = GerenciadorProduto.getInstace().calculaPrecoCusto(produto) * saidaProduto.Quantidade;

                // Baixa sempre o estoque da loja matriz
                if (produto.CodCST != Produto.ST_OUTRAS)
                {
                    GerenciadorProdutoLoja.getInstace().adicionaQuantidade(saidaProduto.Quantidade * (-1), 0, Global.LOJA_PADRAO, saidaProduto.CodProduto);
                }
                else
                {
                    GerenciadorProdutoLoja.getInstace().adicionaQuantidade(0, saidaProduto.Quantidade * (-1), Global.LOJA_PADRAO, saidaProduto.CodProduto);
                }

                decimal custoEstoque = GerenciadorEntradaProduto.getInstace().baixaItensVendidosEstoque(produto, saidaProduto.DataValidade, saidaProduto.Quantidade);
                // Se não houver preço de custo do produto
                if (custoAtual <= 0){
                   custoAtual = Convert.ToDecimal(0.8) * produto.PrecoVendaVarejo * saidaProduto.Quantidade;
                } else if (custoAtual >= (produto.PrecoVendaVarejo * saidaProduto.Quantidade)) {
                    custoAtual = produto.PrecoVendaVarejo * saidaProduto.Quantidade;
                }
                
                if (custoEstoque <= 0) {
                    custoEstoque = Convert.ToDecimal(0.8) * produto.PrecoVendaVarejo * saidaProduto.Quantidade;
                } else if (custoEstoque >= (produto.PrecoVendaVarejo * saidaProduto.Quantidade)) {
                    custoEstoque = produto.PrecoVendaVarejo * saidaProduto.Quantidade;
                }
                
                if ((Convert.ToDecimal(0.8) * custoAtual) > custoEstoque) {
                    somaPrecosCusto += custoAtual;
                } else {
                    somaPrecosCusto += custoEstoque;
                }
            }
            return somaPrecosCusto;
        }

        /// <summary>
        /// Registra transferência de estoque entre lojas
        /// </summary>
        /// <param name="saidaProdutos"></param>
        /// <param name="lojaOrigem"></param>
        /// <param name="lojaDestino"></param>
        private void registrarTransferenciaEstoque(List<SaidaProduto> saidaProdutos, int lojaOrigem, int lojaDestino)
        {
            foreach (SaidaProduto saidaProduto in saidaProdutos)
            {
                Produto produto = GerenciadorProduto.getInstace().obterProduto(saidaProduto.CodProduto);

                GerenciadorProdutoLoja.getInstace().adicionaQuantidade(saidaProduto.Quantidade * (-1), 0, lojaOrigem, saidaProduto.CodProduto);

                GerenciadorProdutoLoja.getInstace().adicionaQuantidade(saidaProduto.Quantidade, 0, lojaDestino, saidaProduto.CodProduto);
            }
        }

        public void GerarDocumentoFiscal(HashSet<long> listaCodSaidas, List<SaidaPagamento> saidaPagamentos)
        {
            //if (GerenciadorSaida.getInstace().AtualizarPedidosComDocumentosFiscais())
            //{

                List<Saida> saidas = new List<Saida>();
                foreach (long codSaida in listaCodSaidas)
                {
                    Saida saida = obterSaida(codSaida);
                    if (saida.TipoSaida == Saida.TIPO_VENDA)
                    {
                        throw new NegocioException("Cupom Fiscal referente a essa pré-venda já foi impresso.");
                    }
                    saidas.Add(saida);
                }

                if (saidas.Count > 0)
                {
                    DirectoryInfo pastaECF = new DirectoryInfo(Global.PASTA_COMUNICACAO_FRENTE_LOJA);

                    if (pastaECF.Exists)
                    {
                        // nome do arquivo é igual ao primeiro da lista
                        String nomeArquivo = Global.PASTA_COMUNICACAO_FRENTE_LOJA + saidas[0].CodSaida + ".txt";
                        StreamWriter arquivo = new StreamWriter(nomeArquivo, false, Encoding.ASCII);

                        // imprime dados do cliente no cupom fiscal
                        if (!saidas[0].CpfCnpj.Trim().Equals(""))
                            arquivo.WriteLine("<CPF>" + saidas[0].CpfCnpj);
                        decimal desconto = 0;
                        decimal precoTotalProdutosVendidos = 0;
                        decimal precoTotalProdutosVendidosAVista = 0;
                        int quantidadeProdutosImpressos = 0;
                        // imprime produtos dos cupons fiscais
                        foreach (Saida saida in saidas)
                        {
                            List<SaidaProduto> saidaProdutos= new List<SaidaProduto>();
                            Pessoa cliente = (Pessoa) GerenciadorPessoa.GetInstance().Obter(saida.CodCliente).ElementAt(0);
                            if (cliente.ImprimirCF) 
                                saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutos(saida.CodSaida);
                            else 
                                saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutosSemCST(saida.CodSaida, Produto.ST_OUTRAS);
                               
                            if (saidaProdutos.Count > 0)
                            {
                                // associa as saídas ao pedido que foi gerado para emissão do cupom fiscal
                                if (tb_saida_pedidoTA.GetDataByCodSaida(saida.CodSaida).Rows.Count == 0)
                                {
                                    tb_saida_pedidoTA.Insert(saida.CodSaida, saidas[0].CodSaida);
                                }
                                foreach (SaidaProduto saidaProduto in saidaProdutos)
                                {
                                    if ((saidaProduto.Quantidade > 0) && (saidaProduto.ValorVenda > 0))
                                    {
                                        String situacaoFiscal = GerenciadorProduto.getInstace().ehProdututoTributadoIntegral(saidaProduto.CodCST) ? "01" : "FF";

                                        arquivo.Write(saidaProduto.CodProduto + ";");
                                        arquivo.Write(saidaProduto.Nome + ";");
                                        arquivo.Write(saidaProduto.Quantidade.ToString() + ";");
                                        arquivo.Write(saidaProduto.ValorVenda.ToString() + ";");
                                        arquivo.Write(situacaoFiscal + ";");
                                        arquivo.Write("0;");
                                        arquivo.Write(saidaProduto.ValorVenda + ";");
                                        arquivo.WriteLine(saidaProduto.Unidade + ";");

                                        precoTotalProdutosVendidos += saidaProduto.Subtotal;
                                        precoTotalProdutosVendidosAVista += saidaProduto.SubtotalAVista;
                                        quantidadeProdutosImpressos++;
                                    }
                                    else
                                    {
                                        desconto += Math.Abs(saidaProduto.SubtotalAVista);
                                    }
                                }
                            }
                            else
                            {
                                tb_SaidaTA.UpdatePedidoGerado(Saida.TIPO_VENDA, "", saida.CodSaida);
                            }
                        }
                        if (quantidadeProdutosImpressos > 0)
                        {
                            // imprime detalhes do cliente
                            if (!saidas[0].CodCliente.Equals(Global.CLIENTE_PADRAO))
                            {
                                arquivo.WriteLine("<NOME> Cliente: " + saidas[0].NomeCliente);
                                arquivo.WriteLine("<CPF> CPF/CNPJ: " + saidas[0].CpfCnpj);
                            }

                            // imprime desconto
                            desconto += (precoTotalProdutosVendidos - precoTotalProdutosVendidosAVista);
                            if (desconto > 0)
                            {
                                arquivo.WriteLine("<DESCONTO>" + desconto.ToString("N2"));
                            }

                            if ((saidaPagamentos == null) || (saidaPagamentos.Count == 0))
                            {
                                saidaPagamentos = GerenciadorSaidaPagamento.getInstace().obterSaidaPagamentos(listaCodSaidas.ToList());
                            }
                            foreach (SaidaPagamento saidaPagamento in saidaPagamentos)
                            {
                                if (saidaPagamento.CodFormaPagamento != FormaPagamento.CARTAO)
                                {
                                    arquivo.Write("<PGTO>" + saidaPagamento.MapeamentoFormaPagamento + ";");
                                    arquivo.Write(saidaPagamento.DescricaoFormaPagamento + ";");
                                    arquivo.Write(saidaPagamento.Valor + ";");
                                    arquivo.WriteLine("N;"); //N ou V
                                }
                                else
                                {
                                    arquivo.Write("<PGTO>" + saidaPagamento.MapeamentoCartao + ";");
                                    arquivo.Write(saidaPagamento.NomeCartaoCredito + ";");
                                    arquivo.Write(saidaPagamento.Valor + ";");
                                    arquivo.WriteLine("V;"); //N ou V vinculado ao TEF
                                }
                            }
                            arquivo.Close();
                        }
                        else
                        {
                            arquivo.Close();
                            ExcluirDocumentoFiscal(saidas[0].CodSaida);
                        }
                    }
                }
           ////; }
        }

               
        public void ExcluirDocumentoFiscal(long codPedido)
        {
            try
            {
                String arquivo = Global.PASTA_COMUNICACAO_FRENTE_LOJA + codPedido + ".txt";

                DirectoryInfo pastaECF = new DirectoryInfo(Global.PASTA_COMUNICACAO_FRENTE_LOJA);
                if (pastaECF.Exists)
                {
                    FileInfo cupomFiscal = new FileInfo(arquivo);

                    if (cupomFiscal.Exists)
                    {
                        cupomFiscal.Delete();
                        tb_saida_pedidoTA.DeleteByCodPedido(codPedido);
                    }
                }
            }
            catch (Exception)
            {
                throw new NegocioException("Não é possível editar essa Pré-Venda. É provável que o Cupom Fiscal esteja sendo impresso.");
            }
        }


        public Boolean AtualizarPedidosComDocumentosFiscais()
        {
            Boolean atualizou = false;
            try
            {
                DirectoryInfo Dir = new DirectoryInfo(Global.PASTA_COMUNICACAO_FRENTE_LOJA_RETORNO);
                string nomeComputador = System.Windows.Forms.SystemInformation.ComputerName;
                if (Dir.Exists && nomeComputador.Equals(Global.NOME_SERVIDOR))
                {
                    // Busca automaticamente todos os arquivos em todos os subdiretórios
                    FileInfo[] Files = Dir.GetFiles("*", SearchOption.TopDirectoryOnly);
                    if (Files.Length == 0)
                    {
                        atualizou = true;
                    }
                    else
                    {
                        foreach (FileInfo file in Files)
                        {
                            bool sucesso = false;
                            String numeroCF = null;
                            String linha = null;
                            StreamReader reader = new StreamReader(file.FullName);
                            
                            // sucesso = true quando cupum fiscal foi impresso
                            if ((linha = reader.ReadLine()) != null)
                            {
                                sucesso = linha.Equals("OK");
                                if (sucesso && ((linha = reader.ReadLine()) != null))
                                {
                                    numeroCF = linha;
                                }
                            }
                            reader.Close();
                            
                            // quando cupom fiscal impresso com sucesso atualiza saidas
                            if (sucesso)
                            {
                                long codPedido = Convert.ToInt64(file.Name.Replace(".TXT", ""));
                                saceDataSet.tb_saida_pedidoDataTable tb_saida_pedidoDT = tb_saida_pedidoTA.GetDataByCodPedido(codPedido);

                                for (int i = 0; i < tb_saida_pedidoDT.Count; i++) {
                                    long codSaida = Convert.ToInt64(tb_saida_pedidoDT.Rows[i]["codSaida"].ToString());
                                    tb_SaidaTA.UpdatePedidoGerado(Saida.TIPO_VENDA, numeroCF, codSaida);
                                    atualizou = true;
                                }
                                tb_saida_pedidoTA.DeleteByCodPedido(codPedido);
                            }
                            else
                            {
                                long codPedido = Convert.ToInt64(file.Name.Replace(".TXT", ""));
                                saceDataSet.tb_saida_pedidoDataTable tb_saida_pedidoDT = tb_saida_pedidoTA.GetDataByCodPedido(codPedido);

                                for (int i = 0; i < tb_saida_pedidoDT.Count; i++)
                                {
                                    long codSaida = Convert.ToInt64(tb_saida_pedidoDT.Rows[i]["codSaida"].ToString());
                                    bool temPagamentoCrediario = GerenciadorSaidaPagamento.getInstace().obterSaidaPagamentosPorFormaPagamento(codSaida, FormaPagamento.CREDIARIO).Count > 0;
                                    if (!temPagamentoCrediario)
                                    {
                                        remover(obterSaida(codSaida));
                                    }
                                }
                                tb_saida_pedidoTA.DeleteByCodPedido(codPedido);
                            }
                            file.CopyTo(Global.PASTA_COMUNICACAO_FRENTE_LOJA_BACKUP + file.Name, true);
                            file.Delete();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Essa exceção não precisa ser tratada. Apenas os cupons fiscais não são recuperados.
                //throw new NegocioException("Ocorreram problemas na recuperação dos dados dos cupons fiscais. Favor contactar o administrador informando o erro " + e.Message);
            }
            return atualizou;
        }

        public void imprimirDAV(List<Saida> saidas, decimal total, decimal totalAVista, decimal desconto, bool comprimido)
        {

            if (comprimido)
                imprimirDAVComprimido(saidas, total, totalAVista, desconto);
            else
                imprimirDAVNormal(saidas, total, totalAVista, desconto);
        }

        private bool imprimirDAVComprimido(List<Saida> saidas, decimal total, decimal totalAVista, decimal desconto)
        {
            try
            {
                ImprimeTexto imp = new ImprimeTexto();
                if (!imp.Inicio(Global.PORTA_IMPRESSORA_REDUZIDA))
                {
                    return false;
                }

                Loja loja = GerenciadorLoja.getInstace().obter(Global.LOJA_PADRAO);
                Pessoa pessoaLoja = (Pessoa)GerenciadorPessoa.GetInstance().Obter(loja.CodPessoa).ElementAt(0);

                imp.Imp(imp.Comprimido);
                imp.ImpLF(Global.LINHA_COMPRIMIDA);
                if (saidas[0].TipoSaida == Saida.TIPO_ORCAMENTO)
                {
                    imp.ImpColLFCentralizado(0, 59, "DOCUMENTO AUXILIAR DE VENDA - ORCAMENTO");
                }
                else
                {
                    imp.ImpColLFCentralizado(0, 59, "DOCUMENTO AUXILIAR DE VENDA - PEDIDO");
                }
                imp.Pula(1);
                imp.ImpColLFCentralizado(0, 59, "NAO E DOCUMENTO FISCAL - NAO E VALIDO COMO RECIBO ");
                imp.ImpColLFCentralizado(0, 59, "E COMO GARANTIA DE MERCADORIA - NAO COMPROVA PAGAMENTO");
                imp.ImpLF(Global.LINHA_COMPRIMIDA);
                imp.ImpColLFCentralizado(0, 59, imp.NegritoOn + pessoaLoja.Nome + imp.NegritoOff);
                imp.ImpColLFCentralizado(0, 59, pessoaLoja.Endereco + "  Fone: " + pessoaLoja.Fone1);
                imp.ImpLF(Global.LINHA_COMPRIMIDA);

                Pessoa cliente = (Pessoa) GerenciadorPessoa.GetInstance().Obter(saidas[0].CodCliente).ElementAt(0);
                imp.ImpLF("Cliente: " + cliente.NomeFantasia);
                //imp.ImpColLF(39, "CPF/CNPJ: " + cliente.CpfCnpj);
                if (saidas.Count == 1)
                {
                    imp.Imp("No do Documento: " + saidas[0].CodSaida);
                    imp.ImpColLF(30, "No do Documento Fiscal: " + saidas[0].PedidoGerado);
                    imp.ImpLF("Data: " + saidas[0].DataSaida.ToShortDateString());
                }
                imp.ImpLF(Global.LINHA_COMPRIMIDA);
                imp.ImpLF("Cod  Produto                                   Qtdade    UN ");
                imp.ImpLF("                                      Preco(R$) Subtotal(R$)");
                foreach (Saida saida in saidas)
                {
                    if (saidas.Count > 1)
                    {
                        imp.ImpLF("==> Documento: " + saida.CodSaida + "    Data: " + saida.DataSaida.ToShortDateString() +"     CF: " + saida.PedidoGerado);
                    }

                    List<SaidaProduto> saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutos(saida.CodSaida);
                    foreach (SaidaProduto produto in saidaProdutos)
                    {
                        imp.ImpColDireita(0, 3, produto.CodProduto.ToString());

                        if (produto.Nome.Length > 40)
                        {
                            imp.ImpCol(5, produto.Nome.Substring(1, 40));
                        }
                        else
                        {
                            imp.ImpCol(5, produto.Nome);
                        }

                        imp.ImpColDireita(46, 52, produto.Quantidade.ToString());
                        imp.ImpColLFDireita(57, 58, produto.Unidade);
                        imp.ImpColDireita(38, 46, produto.ValorVenda.ToString("N2"));
                        imp.ImpColLFDireita(48, 59, produto.Subtotal.ToString("N2"));
                    }
                }

                imp.ImpLF(Global.LINHA_COMPRIMIDA);
                imp.ImpLF("Total Venda: " + total + "     Desconto: " + desconto.ToString("N2") + "%");
                imp.ImpColLFDireita(30, 59, imp.NegritoOn + "Total Pagar:" + totalAVista.ToString("N2") + imp.NegritoOff);
                imp.ImpLF(Global.LINHA_COMPRIMIDA);
                imp.ImpColLFCentralizado(0, 59, "E vedada a autenticacao deste documento");
                if (!saidas[0].PedidoGerado.Equals(""))
                {
                    imp.ImpColLFCentralizado(0, 59, "Documento Válido por 3 (tres) dias");
                }
                imp.ImpLF(Global.LINHA_COMPRIMIDA);
                if (!saidas[0].CodCliente.Equals(Global.CLIENTE_PADRAO))
                {
                    imp.Pula(1);
                    imp.ImpColLF(0, "Recebido por:");
                    imp.ImpLF(Global.LINHA_COMPRIMIDA);
                }
                


                imp.Pula(8);
                imp.Imp(imp.Normal);
                imp.Fim();
                return true;
            }
            catch (Exception)
            {
                throw new NegocioException("Não foi possível realizar a impressão. Por Favor Verifique se a impressora MATRICIAL está LIGADA.");
            }
        }

        private bool imprimirDAVNormal(List<Saida> saidas, decimal total, decimal totalAVista, decimal desconto)
        {
            try
            {
                ImprimeTexto imp = new ImprimeTexto();

                if (!imp.Inicio(Global.PORTA_IMPRESSORA_NORMAL))
                {
                    return false;
                }

                Loja loja = GerenciadorLoja.getInstace().obter(Global.LOJA_PADRAO);
                Pessoa pessoaLoja = (Pessoa) GerenciadorPessoa.GetInstance().Obter(loja.CodPessoa).ElementAt(0);

                
                imp.ImpLF(Global.LINHA);
                if (saidas[0].TipoSaida == Saida.TIPO_ORCAMENTO)
                {
                    imp.ImpColLFCentralizado(0, 79, "DOCUMENTO AUXILIAR DE VENDA - ORCAMENTO");
                }
                else
                {
                    imp.ImpColLFCentralizado(0, 79, "DOCUMENTO AUXILIAR DE VENDA - PEDIDO");
                }
                imp.Pula(1);
                imp.ImpColLFCentralizado(0, 79, "NAO E DOCUMENTO FISCAL - NAO E VALIDO COMO RECIBO E COMO GARANTIA DE MERCADORIA");
                imp.ImpColLFCentralizado(0, 79, "- NAO COMPROVA PAGAMENTO");
                imp.ImpLF(Global.LINHA);
                imp.ImpColLFCentralizado(0, 79, imp.NegritoOn + pessoaLoja.Nome + imp.NegritoOff);
                imp.ImpColLFCentralizado(0, 79, pessoaLoja.Endereco + "                                     Fone: " + pessoaLoja.Fone1);
                imp.ImpLF(Global.LINHA);

                Pessoa cliente = (Pessoa) GerenciadorPessoa.GetInstance().Obter(saidas[0].CodCliente).ElementAt(0);
                imp.Imp("Cliente: " + cliente.NomeFantasia);
                imp.ImpColLF(50, "CPF/CNPJ: " + cliente.CpfCnpj);
                if (saidas.Count == 1)
                {
                    imp.ImpLF("Data : " + saidas[0].DataSaida.ToShortDateString());
                    imp.Imp("No do Documento: " + saidas[0].CodSaida);
                    imp.ImpColLF(50, "No do Documento Fiscal: " + saidas[0].PedidoGerado);
                }
                imp.ImpLF(Global.LINHA);
                imp.ImpLF("Cod  Produto                                   Qtdade  UN Preco(R$) Subtotal(R$)");

                foreach (Saida saida in saidas)
                {
                    if (saidas.Count > 1)
                    {
                        imp.ImpLF("==> Documento: " + saida.CodSaida + "    Data: " + saida.DataSaida.ToShortDateString() + "     CF: " + saida.PedidoGerado);
                    }

                    List<SaidaProduto> saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutos(saida.CodSaida);
                    foreach (SaidaProduto produto in saidaProdutos)
                    {
                        imp.ImpColDireita(0, 3, produto.CodProduto.ToString());

                        if (produto.Nome.Length > 40)
                        {
                            imp.ImpCol(5, produto.Nome.Substring(1, 40));
                        }
                        else
                        {
                            imp.ImpCol(5, produto.Nome);
                        }

                        imp.ImpColDireita(46, 52, produto.Quantidade.ToString());
                        imp.ImpColDireita(55, 56, produto.Unidade);
                        imp.ImpColDireita(58, 66, produto.ValorVenda.ToString("N2"));
                        imp.ImpColLFDireita(68, 79, produto.Subtotal.ToString("N2"));
                    }

                }
                imp.ImpLF(Global.LINHA);
                imp.Imp("Total Venda: " + total + "            Desconto: " + desconto.ToString("N2") + "%");
                imp.ImpColLFDireita(55, 80, imp.NegritoOn + "Total Pagar:" + totalAVista.ToString("N2") + imp.NegritoOff);
                imp.ImpLF(Global.LINHA);
                imp.ImpColLFCentralizado(0, 79, "E vedada a autenticacao deste documento");
                imp.ImpLF(Global.LINHA);

                imp.Pula(2);
                imp.Fim();
                return true;
            }
            catch (Exception)
            {
                throw new NegocioException("Não foi possível realizar a impressão. Por Favor Verifique se a impressora MATRICIAL está LIGADA.");
            }
        }

        public void imprimirNotaFiscal(Saida saida)
        {

            if (saida.TipoSaida == Saida.TIPO_ORCAMENTO)
            {
                throw new NegocioException("O Documento Fiscal não pode ser impresso a partir de um ORÇAMENTO. É necessário transformá-lo numa PRÉ-VENDA.");
            }
                
            try
            {
                if (saida.TipoSaida == Saida.TIPO_PRE_VENDA)
                {
                    GerarDocumentoFiscal(new HashSet<long>() {saida.CodSaida}, null);
                }
                else if ((saida.TipoSaida == Saida.TIPO_VENDA) || (saida.TipoSaida == Saida.TIPO_SAIDA_DEPOSITO) || (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FRONECEDOR) )
                {
                    ImprimeTexto imp = new ImprimeTexto();

                    imp.Inicio(Global.PORTA_IMPRESSORA_NORMAL);

                    Pessoa cliente = (Pessoa) GerenciadorPessoa.GetInstance().Obter(saida.CodCliente).ElementAt(0);

                    imprimirNotaFiscalCabecalho(saida, cliente, imp);

                    //linha 23 
                    List<SaidaProduto> saidaProdutos;
                    if (saida.TipoSaida == Saida.TIPO_VENDA)
                    {
                        saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutosPorPedido(saida.PedidoGerado);
                    }
                    else
                    {
                        saidaProdutos = GerenciadorSaidaProduto.getInstace().obterSaidaProdutos(saida.CodSaida);
                    }
                        int numeroProdutosImpressos = 0;
                    int numeroPaginas = 1;
                    decimal subtotal = 0;
                    decimal subtotalAVista = 0;

                    imp.Imp(imp.Comprimido);
                    foreach (SaidaProduto saidaProduto in saidaProdutos)
                    {
                        if (numeroProdutosImpressos >= 17)
                        {
                            numeroProdutosImpressos = 0;
                            numeroPaginas++;

                            imprimirNotaFiscalRodape(saida, imp, numeroPaginas, subtotal, subtotalAVista, false);
                            imp.Eject();
                            imprimirNotaFiscalCabecalho(saida, cliente, imp);
                        }

                        if (numeroProdutosImpressos == 0)
                        {
                            if (numeroPaginas > 1)
                            {
                                imp.Pula(3);
                                imp.Imp(imp.Comprimido);
                                imp.ImpCol(13, "VALOR TRANSPORTADO DA PAG    " + (numeroPaginas - 1) + " ->");
                                imp.ImpColDireita(100, 116, subtotal.ToString("N2"));
                                imp.Pula(2);
                            }
                            else
                            {
                                imp.Pula(3);
                            }
                        }
                        
                        imp.ImpColDireita(5, 9, saidaProduto.CodProduto.ToString());
                        if (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FRONECEDOR)
                            imp.ImpCol(14, GerenciadorProduto.getInstace().obterProduto(saidaProduto.CodProduto).NomeProdutoFabricante);
                        else
                            imp.ImpCol(14, saidaProduto.Nome);
                        if ( saida.TipoSaida == Saida.TIPO_SAIDA_DEPOSITO )
                            imp.ImpCol(70, "041");
                        else
                            imp.ImpCol(70, saidaProduto.CodCST);
                        imp.ImpCol(75, saidaProduto.Unidade);
                        imp.ImpColDireita(81, 87, saidaProduto.Quantidade.ToString("N2"));
                        imp.ImpColDireita(89, 105, saidaProduto.ValorVenda.ToString("N2"));
                        imp.ImpColDireita(115, 128, saidaProduto.Subtotal.ToString("N2"));
                        if (saida.TipoSaida == Saida.TIPO_SAIDA_DEPOSITO)
                        {
                            imp.ImpCol(133, "0%");
                        }
                        else if (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FRONECEDOR)
                        {
                           imp.ImpCol(133, GerenciadorProduto.getInstace().obterProduto(saidaProduto.CodProduto).Icms.ToString("N1"));
                        }
                        else
                        {
                            if (GerenciadorProduto.getInstace().ehProdututoTributadoIntegral(saidaProduto.CodCST))
                                imp.ImpCol(133, "17%");
                            else
                                imp.ImpCol(133, "0%");
                        }
                        imp.Pula(1);

                        subtotal += saidaProduto.Subtotal;
                        subtotalAVista += saidaProduto.SubtotalAVista;
                        numeroProdutosImpressos++;
                     }

                    imp.Pula(17 - numeroProdutosImpressos);


                    imprimirNotaFiscalRodape(saida, imp, numeroPaginas, subtotal, subtotalAVista, true);

                    imp.Eject();
                    imp.Fim(); 
                }
            }
            catch (Exception)
            {
                throw new NegocioException("Não foi possível realizar a impressão. Por Favor Verifique se a impressora MATRICIAL está LIGADA.");
            }
        }

        private void imprimirNotaFiscalCabecalho(Saida saida, Pessoa cliente, ImprimeTexto imp)
        {
            imp.Imp(imp.Normal);
            imp.Pula(4);
            // linha 4
            imp.ImpCol(52, "XX");
            imp.ImpCol(75, saida.Nfe);
            imp.Pula(4);

            // linha 8
            if (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FRONECEDOR)
            {                   
                imp.ImpCol(2, "DEVOLUCAO DE COMPRA P/COM.");
                imp.ImpCol(28, "6.202");
                imp.Pula(2);
            }
            else if (saida.TipoSaida == Saida.TIPO_SAIDA_DEPOSITO)
            {
                imp.ImpCol(2, "REMESSA DEPOSITO FECHADO");
                imp.ImpCol(28, "5.905");
                imp.Pula(2);
            }
            else
            {
                imp.ImpCol(2, "VENDA TRIBUTADA PELA ECF");
                imp.ImpCol(28, "5.929");
                imp.Pula(2);
            }
            
            // linha 10
            imp.ImpCol(2, cliente.Nome);
            imp.ImpCol(55, cliente.CpfCnpj);
            imp.ImpCol(70, DateTime.Now.ToShortDateString());
            imp.Pula(1);
            
            // linha 12
            imp.ImpCol(2, cliente.Endereco + ", " + cliente.Numero);
            imp.ImpCol(35, cliente.Bairro);
            imp.ImpCol(60, cliente.Cep);
            imp.ImpCol(70, saida.DataSaida.ToShortDateString());
            imp.Pula(2);

            // linha 14
            imp.ImpCol(2, cliente.Cidade);
            imp.ImpCol(35, cliente.Fone1);
            imp.ImpCol(48, cliente.Uf);
            imp.ImpCol(54, cliente.Ie);
            imp.ImpCol(74, saida.DataSaida.ToShortTimeString());
            imp.Pula(7);
        }

        private void imprimirNotaFiscalRodape(Saida saida, ImprimeTexto imp, int numeroPagina, decimal subtotal, decimal subtotalAvista, bool ultimaPagina)
        {
            if (ultimaPagina)
            {
                if ( (saida.TipoSaida == Saida.TIPO_SAIDA_DEPOSITO) || (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FRONECEDOR))
                {
                    imp.Pula(3);
                }
                else
                {

                    imp.Imp(imp.Comprimido);
                    imp.ImpCol(13, "DESCONTO PROMOCIONAL ---------> R$" + (subtotal - subtotalAvista).ToString("N2"));

                    imp.Pula(1);
                    imp.Imp(imp.Normal);
                    imp.ImpCol(13, "***  REF. AO CUMPOM FISCAL NUMERO " + saida.PedidoGerado + "/001   ***");

                    imp.Pula(2);
                }
                imp.Imp(imp.Normal);
                // linha 45
                if (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FRONECEDOR)
                {
                    imp.ImpColDireita(35, 47, saida.BaseCalculoICMSSubst.ToString("N2")); 
                    imp.ImpColDireita(50, 62, saida.ValorICMSSubst.ToString("N2")); //valor do icms substituto
                }
                imp.ImpColLFDireita(65, 78, subtotalAvista.ToString("N2")); // total produtos
                imp.Pula(1);

                // linha 46
                if (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FRONECEDOR)
                {

                    imp.ImpColDireita(05, 15, saida.ValorFrete.ToString("N2")); // frete
                    imp.ImpColDireita(18, 30, saida.ValorSeguro.ToString("N2")); // seguro
                    imp.ImpColDireita(33, 45, saida.OutrasDespesas.ToString("N2")); // outras despesas
                    imp.ImpColDireita(50, 62, saida.ValorIPI.ToString("N2")); //ipi
                    imp.ImpColDireita(65, 78, (saida.TotalAVista + saida.ValorIPI + saida.ValorICMSSubst + saida.ValorFrete).ToString("N2")); //total nota
                }
                else
                {
                    imp.ImpColDireita(65, 78, (subtotalAvista).ToString("N2")); //total nota
                }
                imp.Pula(2);
                Pessoa empresaFrete = (Pessoa) GerenciadorPessoa.GetInstance().Obter(saida.CodEmpresaFrete).ElementAt(0);

                // linha 49
                if (empresaFrete.Nome.Length > 35)
                {
                    imp.ImpCol(2, empresaFrete.Nome.Substring(0, 34));
                }
                else
                {
                    imp.ImpCol(2, empresaFrete.Nome);
                }

                imp.ImpCol(49, "1");
                imp.ImpCol(65, empresaFrete.CpfCnpj);
                imp.Pula(2);

                // linha 51
                imp.ImpCol(2, empresaFrete.Endereco + ", " + empresaFrete.Numero);
                imp.ImpCol(40, empresaFrete.Cidade);
                imp.ImpCol(61, empresaFrete.Uf);
                imp.ImpColLF(65, empresaFrete.Ie);
                imp.ImpCol(2, saida.QuantidadeVolumes.ToString("N2"));
                imp.ImpCol(15, saida.EspecieVolumes);
                imp.ImpCol(29, saida.Marca);
                imp.ImpCol(40, saida.Numero.ToString());
                imp.ImpColDireita(55, 64, saida.PesoBruto.ToString("N2"));
                imp.ImpColDireita(69, 78, saida.PesoLiquido.ToString("N2"));
                imp.Pula(4);

                // linha 56
                if (saida.TipoSaida == Saida.TIPO_SAIDA_DEPOSITO)
                {
                    imp.ImpColLF(3, "");
                    imp.ImpColLF(3, "Nao Incidencia de ICMS conforme Art 2o,");
                    imp.ImpColLF(3, "XI do RICMS/SE");
                    imp.Pula(2);
                    imp.ImpCol(75, saida.Nfe);
                } 
                else
                {
                    imp.ImpCol(3, "VEND:   0   CLI: " + saida.CodCliente);
                    imp.Pula(5);
                    imp.ImpCol(75, saida.Nfe);
                }
                
            }
            else
            {
                imp.Imp(imp.Comprimido);
                imp.ImpCol(13, "VALOR A TRASNPORTAR P/PAG    " + numeroPagina + " -->" );
                imp.ImpColDireita(100, 116, subtotal.ToString("N2"));
                imp.Imp(imp.Normal);
                imp.Pula(10);
                imp.ImpCol(75, saida.Nfe);
            }
            imp.Imp(imp.Normal);
        }

        public Int64 ObterNumeroProximaNotaFiscal()
        {
            String ultimoNumero = tb_SaidaTA.GetUltimoNumeroNF();
            if ( (ultimoNumero != null) && (!ultimoNumero.Trim().Equals("")))
            {
                return Convert.ToInt64(ultimoNumero) + 1;
            }
            return 1;
        }
    }
}