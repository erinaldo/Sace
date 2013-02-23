﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using Dados.saceDataSetTableAdapters;
using MySql.Data.MySqlClient;
using Dados;
using Dominio;
using Negocio;
using Util;


namespace Telas
{
    public partial class FrmReceberPagamentoPessoa : Form
    {
        private EstadoFormulario estado;

        Pessoa pessoa;
        private DateTime dataInicio;
        private DateTime dataFim;
        private bool abertaChecked;
        private bool quitadaChecked;
        private decimal descontoCalculado;
        private bool alterouDesconto;

        public FrmReceberPagamentoPessoa()
        {
            InitializeComponent();
        }

        private void FrmReceberPagamentoPessoa_Load(object sender, EventArgs e)
        {
            pessoaBindingSource.DataSource = GerenciadorPessoa.GetInstance().ObterTodos();
            formaPagamentoBindingSource.DataSource = GerenciadorFormaPagamento.GetInstance().ObterTodos();
            cartaoCreditoBindingSource.DataSource = GerenciadorCartaoCredito.GetInstance().ObterTodos();
            codCartaoComboBox.SelectedIndex = 1;
            habilitaBotoes(true);
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            totalContasTextBox.Text = "0,00";
            totalPagamentosTextBox.Text = "0,00";
            faltaReceberTextBox.Text = "0,00";
            descontoTextBox.Text = "0,00";
            totalAVistaTextBox.Text = "0,00";
            valorPagamentoTextBox.Text = "0,00";
            codClienteComboBox.Focus();
            habilitaBotoes(false);
            estado = EstadoFormulario.INSERIR;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            habilitaBotoes(true);
            btnNovo.Focus();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirma registro de pagamento?", "Confirmar Pagamento", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int formaPagamento = int.Parse(codFormaPagamentoComboBox.SelectedValue.ToString());
                decimal valorPagamento = Convert.ToDecimal(valorPagamentoTextBox.Text);
                decimal totalPago = Convert.ToDecimal(totalPagamentosTextBox.Text);
                decimal faltaReceber = Convert.ToDecimal(faltaReceberTextBox.Text);

                if (valorPagamento > faltaReceber)
                {
                    codFormaPagamentoComboBox.Focus();
                    throw new TelaException("O valor do pagamento não pode ser maior que o valor a receber.");
                }

                List<long> listaContas = new List<long>();
                HashSet<long> listaSaidas = new HashSet<long>();
                for (int i = contasPessoaDataGridView.SelectedRows.Count - 1; i >= 0; i--)
                {
                    listaContas.Add(Convert.ToInt64(contasPessoaDataGridView.SelectedRows[i].Cells[0].Value.ToString())); //codConta 
                    listaSaidas.Add(Convert.ToInt64(contasPessoaDataGridView.SelectedRows[i].Cells[1].Value.ToString())); //codSaida
                }


                // cupom fiscal pode ser impresso quando todas as contas associadas a uma saída estiverem selecionadas
                bool podeImprimirCF = (valorPagamento == faltaReceber);
                if (podeImprimirCF)
                {
                    foreach (long codSaida in listaSaidas)
                    {
                        List<Conta> contas = (List<Conta>)GerenciadorConta.GetInstance().ObterPorSaida(codSaida);
                        foreach (Conta conta in contas)
                        {
                            if ((!listaContas.Contains(conta.CodConta)) || ((conta.CF != null) && !conta.CF.Trim().Equals("")))
                            {
                                podeImprimirCF = false;
                                break;
                            }
                        }
                        if (!podeImprimirCF)
                            break;
                    }
                }

                if (formaPagamento.Equals(FormaPagamento.CARTAO) && !podeImprimirCF)
                {
                    throw new TelaException("Não é possível realizar o pagamento com cartão de crédito. Verifique se algum cupom referente às contas selecionadas já foi impresso OU se todas as contas referente às saídas escolhidas estão selecionadas.");
                }

                if (formaPagamento.Equals(FormaPagamento.DINHEIRO) || (formaPagamento.Equals(FormaPagamento.CARTAO) && podeImprimirCF))
                {
                    // atualiza descontos das contas de acordo com o especificado
                    if (alterouDesconto)
                    {
                        for (int i = contasPessoaDataGridView.SelectedRows.Count - 1; i >= 0; i--)
                        {
                            decimal valorDescontoConta = (decimal)contasPessoaDataGridView.SelectedRows[i].Cells[6].Value;
                            long codConta = (long)contasPessoaDataGridView.SelectedRows[i].Cells[0].Value;
                            Conta conta = GerenciadorConta.GetInstance().Obter(codConta).ElementAt(0);
                            if (conta.CodSituacao.Equals(SituacaoConta.SITUACAO_ABERTA))
                            {
                                GerenciadorConta.GetInstance().Atualizar(conta.CodSituacao, valorDescontoConta, conta.CodConta);
                            }
                        }
                    }

                    if (formaPagamento.Equals(FormaPagamento.DINHEIRO))
                    {
                        MovimentacaoConta movimentacao = new MovimentacaoConta();
                        movimentacao.CodConta = listaContas.ElementAt(0); // valor é irrelevante
                        movimentacao.CodContaBanco = Global.CAIXA_PADRAO;
                        movimentacao.CodResponsavel = pessoa.CodPessoa;
                        movimentacao.CodTipoMovimentacao = TipoMovimentacaoConta.RECEBIMENTO_CREDIARIO;
                        movimentacao.DataHora = DateTime.Now;
                        movimentacao.Valor = valorPagamento;
                        GerenciadorMovimentacaoConta.getInstace().Inserir(movimentacao, listaContas);
                        if (podeImprimirCF && MessageBox.Show("Deseja imprimir cupom fiscal das contas selecionadas?", "Confirmar Impressão", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaidaPagamento saidaPagamento = new SaidaPagamento();
                            FormaPagamento formaPagamentoDinheiro = GerenciadorFormaPagamento.GetInstance().Obter(FormaPagamento.DINHEIRO).ElementAt(0);
                            saidaPagamento.MapeamentoFormaPagamento = formaPagamentoDinheiro.Mapeamento;
                            saidaPagamento.DescricaoFormaPagamento = formaPagamentoDinheiro.Descricao;
                            saidaPagamento.Valor = valorPagamento;
                            GerenciadorSaida.GetInstance().GerarDocumentoFiscal(listaSaidas, new List<SaidaPagamento>() { saidaPagamento }, valorPagamento);
                        }
                    }
                    else if (formaPagamento.Equals(FormaPagamento.CARTAO))
                    {
                        List<SaidaPagamento> listaSaidaPagamento = new List<SaidaPagamento>();
                        if (totalPago > 0)
                        {
                            SaidaPagamento saidaPagamentoDinheiro = new SaidaPagamento();
                            FormaPagamento formaPagamentoDinheiro = GerenciadorFormaPagamento.GetInstance().Obter(FormaPagamento.DINHEIRO).ElementAt(0);
                            saidaPagamentoDinheiro.CodFormaPagamento = FormaPagamento.DINHEIRO;
                            saidaPagamentoDinheiro.MapeamentoFormaPagamento = formaPagamentoDinheiro.Mapeamento;
                            saidaPagamentoDinheiro.DescricaoFormaPagamento = formaPagamentoDinheiro.Descricao;
                            saidaPagamentoDinheiro.Valor = valorPagamento - totalPago;
                            listaSaidaPagamento.Add(saidaPagamentoDinheiro);
                        }

                        SaidaPagamento saidaPagamentoCartao = new SaidaPagamento();

                        int codCartao = Convert.ToInt32(codCartaoComboBox.SelectedValue.ToString());
                        int parcelas = Convert.ToInt32(parcelasTextBox.Text);
                        CartaoCredito cartaoCredito = GerenciadorCartaoCredito.GetInstance().Obter(codCartao).ElementAt(0);
                        saidaPagamentoCartao.CodFormaPagamento = FormaPagamento.CARTAO;
                        saidaPagamentoCartao.MapeamentoFormaPagamento = cartaoCredito.Mapeamento;
                        saidaPagamentoCartao.MapeamentoCartao = cartaoCredito.Mapeamento;
                        saidaPagamentoCartao.NomeCartaoCredito = cartaoCredito.Nome;
                        saidaPagamentoCartao.DescricaoFormaPagamento = cartaoCredito.Nome;
                        saidaPagamentoCartao.Valor = valorPagamento;
                        listaSaidaPagamento.Add(saidaPagamentoCartao);
                        GerenciadorSaida.GetInstance().GerarDocumentoFiscal(listaSaidas, listaSaidaPagamento, valorPagamento);
                        if (MessageBox.Show("A compra foi confirmada pela administradora do cartão selecionado?", "Confirma Cartão de Crédito", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            GerenciadorConta.GetInstance().SubstituirContas(listaContas, valorPagamento, cartaoCredito, parcelas);
                        }
                    }
                    codClienteComboBox_Leave(sender, e);
                }
            }
            habilitaBotoes(true);
            btnNovo.Focus();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            HashSet<long> codSaidas = new HashSet<long>();

            for (int i = contasPessoaDataGridView.SelectedRows.Count - 1; i >= 0; i--)
            {
                codSaidas.Add(Convert.ToInt64(contasPessoaDataGridView.SelectedRows[i].Cells[1].Value.ToString())); //pre-venda
            }

            decimal total = Convert.ToDecimal(totalContasTextBox.Text);
            decimal totalAVista = Convert.ToDecimal(totalAVistaTextBox.Text);
            decimal desconto = Convert.ToDecimal(descontoTextBox.Text);

            FrmSaidaDAV frmSaidaDAV = new FrmSaidaDAV(codSaidas, total, totalAVista, desconto);
            frmSaidaDAV.ShowDialog();
            frmSaidaDAV.Dispose();
        }

        private void btnCFNfe_Click(object sender, EventArgs e)
        {
            HashSet<long> codSaidas = new HashSet<long>();

            string pedidoGerado = contasPessoaDataGridView.SelectedRows[0].Cells[3].Value.ToString().Trim();

            if (!pedidoGerado.Trim().Equals(""))
            {
                long codSaida = Convert.ToInt64(contasPessoaDataGridView.SelectedRows[0].Cells[1].Value.ToString());
                Saida saida = GerenciadorSaida.GetInstance().Obter(codSaida);

                FrmSaidaNF frmSaidaNF = new FrmSaidaNF(saida);
                frmSaidaNF.ShowDialog();
                frmSaidaNF.Dispose();
            }
            else
            {

                for (int i = contasPessoaDataGridView.SelectedRows.Count - 1; i >= 0; i--)
                {
                    codSaidas.Add(Convert.ToInt64(contasPessoaDataGridView.SelectedRows[i].Cells[1].Value.ToString())); //pre-venda
                }

                decimal total = Convert.ToDecimal(totalContasTextBox.Text);
                decimal totalAVista = Convert.ToDecimal(totalAVistaTextBox.Text);

                if (MessageBox.Show("Confirma emisssão do Cupom Fiscal das Contas Selecionadas?", "Confirmar Impressão Cupom Fiscal", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaidaPagamento saidaPagamento = new SaidaPagamento();
                    FormaPagamento dinheiro = GerenciadorFormaPagamento.GetInstance().Obter(FormaPagamento.DINHEIRO).ElementAt(0);
                    saidaPagamento.CodFormaPagamento = FormaPagamento.DINHEIRO;
                    saidaPagamento.MapeamentoFormaPagamento = dinheiro.Mapeamento;
                    saidaPagamento.DescricaoFormaPagamento = dinheiro.Descricao;
                    saidaPagamento.Valor = Convert.ToDecimal(valorPagamentoTextBox.Text);
                    GerenciadorSaida.GetInstance().GerarDocumentoFiscal(codSaidas, new List<SaidaPagamento>() { saidaPagamento }, saidaPagamento.Valor);
                }
            }
        }

        private void FrmReceberPagamentoPessoa_KeyDown(object sender, KeyEventArgs e)
        {
            if (estado.Equals(EstadoFormulario.ESPERA))
            {
                if (e.KeyCode == Keys.F3)
                {
                    btnNovo_Click(sender, e);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    SendKeys.Send("{tab}");
                }
                else if (e.KeyCode == Keys.F6)
                {
                    btnSalvar_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F7)
                {
                    btnImprimir_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F8)
                {
                    btnCFNfe_Click(sender, e);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    btnCancelar_Click(sender, e);
                }
            }
            if (e.KeyCode == Keys.F12)
            {
                tb_movimentacao_contaDataGridView.Focus();
            }
            else if ((e.KeyCode == Keys.Delete) && (tb_movimentacao_contaDataGridView.Focused == true))
            {
                ExcluirMovimentacao(sender, e);
            }
        }

        private void ExcluirMovimentacao(object sender, KeyEventArgs e)
        {
            if (MessageBox.Show("Confirma exclusão do pagamento?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (tb_movimentacao_contaDataGridView.Rows.Count > 0)
                {
                    long codMovimentacaoConta = long.Parse(tb_movimentacao_contaDataGridView.SelectedRows[0].Cells[0].Value.ToString());
                    Negocio.GerenciadorMovimentacaoConta.getInstace().Remover(codMovimentacaoConta);
                }
                ListarContasPessoa();
            }
        }


        private void habilitaBotoes(Boolean habilita)
        {
            btnSalvar.Enabled = !(habilita);
            btnCancelar.Enabled = !(habilita);
            btnNovo.Enabled = habilita;

            btnImprimir.Enabled = contasPessoaDataGridView.RowCount > 0;
            btnCFNfe.Enabled = contasPessoaDataGridView.RowCount > 0;

            if (habilita)
            {
                estado = EstadoFormulario.ESPERA;
            }
        }


        private void codClienteComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.Parse(e.KeyChar.ToString().ToUpper());
        }

        private void codClienteComboBox_Leave(object sender, EventArgs e)
        {
            if (estado != EstadoFormulario.ESPERA)
            {
                List<Pessoa> pessoas = GerenciadorPessoa.GetInstance().ObterPorNomeFantasia(codClienteComboBox.Text).ToList();
                if ((pessoas.Count == 0) || (pessoas.Count > 1))
                {
                    Telas.FrmPessoaPesquisa frmPessoaPesquisa = new Telas.FrmPessoaPesquisa(codClienteComboBox.Text);
                    frmPessoaPesquisa.ShowDialog();
                    if (frmPessoaPesquisa.PessoaSelected != null)
                    {
                        pessoaBindingSource.Position = pessoaBindingSource.List.IndexOf(frmPessoaPesquisa.PessoaSelected);
                        codClienteComboBox.Text = pessoa.NomeFantasia;
                    }
                    else
                    {
                        codClienteComboBox.Focus();
                    }
                    frmPessoaPesquisa.Dispose();
                }
                else
                {
                    pessoa = pessoas[0];
                    pessoaBindingSource.Position = pessoaBindingSource.List.IndexOf(pessoa);
                }

                if ((pessoa != null) && (!pessoa.CodPessoa.Equals(Global.CLIENTE_PADRAO)))
                {
                    // Obter todas as contas da pessoa em aberto
                    contasPessoaDataGridView.DataSource = GerenciadorConta.GetInstance().ObterPorSituacaoPessoa(SituacaoConta.SITUACAO_ABERTA.ToString(), pessoa.CodPessoa);
                    //descontoTextBox.Text = "0";
                    if (contasPessoaDataGridView.RowCount > 0)
                    {
                        //Obter maior e menor data de vencimento para preencher corrretamente
                        dataInicioDateTimePicker.Text = contasPessoaDataGridView.Rows[0].Cells[2].Value.ToString();
                        int ultimaLinha = contasPessoaDataGridView.RowCount - 1;
                        dataFinalDateTimePicker.Text = contasPessoaDataGridView.Rows[ultimaLinha].Cells[2].Value.ToString();

                        // Guarda os dados das variáveis para verificar se há necessidade de uma nova consulta
                        dataInicio = Convert.ToDateTime(dataInicioDateTimePicker.Text);
                        dataFim = Convert.ToDateTime(dataFinalDateTimePicker.Text);
                        abertaChecked = abertaCheckBox.Checked;
                        quitadaChecked = quitadaCheckBox.Checked;
                    }

                    contasPessoaDataGridView.SelectAll();
                    habilitaBotoes(false);
                }
            }
            alterouDesconto = false;
        }

        private string obterListaSituacao()
        {
            if (abertaCheckBox.Checked && quitadaCheckBox.Checked)
                return SituacaoConta.SITUACAO_ABERTA + "," + "," + SituacaoConta.SITUACAO_QUITADA;
            else if (abertaCheckBox.Checked)
                return SituacaoConta.SITUACAO_ABERTA;
            else if (quitadaCheckBox.Checked)
                return SituacaoConta.SITUACAO_QUITADA;
            else
                return "";
        }

        private void dataInicioDateTimePicker_Leave(object sender, EventArgs e)
        {
            // Se houve alteração nos valores da consulta
            if (!dataInicioDateTimePicker.Value.Equals(dataInicio) || !dataFinalDateTimePicker.Value.Equals(dataFim) ||
                (abertaCheckBox.Checked != abertaChecked) || (quitadaCheckBox.Checked != quitadaChecked))
            {
                ListarContasPessoa();

            }

        }

        private void ListarContasPessoa()
        {
            List<string> situacoes = new List<string>();

            if (abertaCheckBox.Checked)
            {
                situacoes.Add(SituacaoConta.SITUACAO_ABERTA.ToString());
            }
            if (quitadaCheckBox.Checked)
            {
                situacoes.Add(SituacaoConta.SITUACAO_QUITADA.ToString());
            }

            dataInicio = Convert.ToDateTime(dataInicioDateTimePicker.Text);
            dataFim = Convert.ToDateTime(dataFinalDateTimePicker.Text);
            abertaChecked = abertaCheckBox.Checked;
            quitadaChecked = quitadaCheckBox.Checked;
            if (situacoes.Count == 2)
            {
                contasPessoaDataGridView.DataSource = GerenciadorConta.GetInstance().ObterPorSituacaoPessoaPeriodo(situacoes[0], situacoes[1], pessoa.CodPessoa, dataInicio, dataFim);
            }
            else if (situacoes.Count == 1)
            {
                contasPessoaDataGridView.DataSource = GerenciadorConta.GetInstance().ObterPorSituacaoPessoaPeriodo(situacoes[0], situacoes[0], pessoa.CodPessoa, dataInicio, dataFim);
            }
            else
            {
                contasPessoaDataGridView.DataSource = null;
            }
            contasPessoaDataGridView.SelectAll();
            
        }

        private void DescontoTextBox_Leave(object sender, EventArgs e)
        {
            FormatTextBox.NumeroCom2CasasDecimais((TextBox)sender);
            CalcularDescontos();
        }

        private void CalcularDescontos()
        {
            decimal descontoAtual = Convert.ToDecimal(descontoTextBox.Text);
            if (descontoAtual != descontoCalculado)
            {
                alterouDesconto = true;
                for (int i = 0; i < contasPessoaDataGridView.SelectedRows.Count; i++)
                {
                    decimal valorConta = (decimal)contasPessoaDataGridView.SelectedRows[i].Cells[5].Value;
                    // valor do desconto
                    contasPessoaDataGridView.SelectedRows[i].Cells[6].Value = (valorConta * (descontoAtual / 100)).ToString("N2");
                    decimal valorDescontoConta = (decimal)contasPessoaDataGridView.SelectedRows[i].Cells[6].Value;
                    // valor da conta a vista
                    contasPessoaDataGridView.SelectedRows[i].Cells[7].Value = valorConta - valorDescontoConta;
                }
                CalcularTotalContasSelecionadas();
            }

        }

        private void ListarPagamentosContasSelecionadas()
        {
            var contasExibidas = new List<Int64>();
            for (int i = 0; i < contasPessoaDataGridView.SelectedRows.Count; i++)
            {
                contasExibidas.Add((long)contasPessoaDataGridView.SelectedRows[i].Cells[0].Value);
            }
            movimentacaoContaBindingSource.DataSource = GerenciadorMovimentacaoConta.getInstace().ObterPorContas(contasExibidas);
        }


        private void CalcularTotalContasSelecionadas()
        {
            decimal totalContas = 0;
            decimal totalDesconto = 0;
            decimal totalPagamentos = 0;
            for (int i = 0; i < contasPessoaDataGridView.SelectedRows.Count; i++)
            {
                totalContas += Convert.ToDecimal(contasPessoaDataGridView.SelectedRows[i].Cells[5].Value.ToString()); //total
                totalDesconto += Convert.ToDecimal(contasPessoaDataGridView.SelectedRows[i].Cells[6].Value.ToString()); //totalPagar
            }

            foreach (MovimentacaoConta movimentacaoConta in movimentacaoContaBindingSource)
            {
                totalPagamentos += movimentacaoConta.Valor;
            }
            totalContasTextBox.Text = totalContas.ToString("N2");
            totalAVistaTextBox.Text = (totalContas - totalDesconto).ToString("N2");
            totalPagamentosTextBox.Text = totalPagamentos.ToString("N2");
            faltaReceberTextBox.Text = (totalContas - totalDesconto - totalPagamentos).ToString("N2");
            if (totalDesconto > 0)
            {
                descontoTextBox.Text = ((totalDesconto / totalContas) * 100).ToString("N2");
            }
            else
            {
                descontoTextBox.Text = "0,00";
            }
            valorPagamentoTextBox.Text = faltaReceberTextBox.Text;
            descontoCalculado = Convert.ToDecimal(descontoTextBox.Text);
        }

        private void ContasPessoaDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            ListarPagamentosContasSelecionadas();
            CalcularTotalContasSelecionadas();
            DestacarContasEmAtraso();
        }

        private void DestacarContasEmAtraso()
        {
            for (int i = 0; i < contasPessoaDataGridView.RowCount; i++)
            {
                DateTime dataConta = Convert.ToDateTime(contasPessoaDataGridView.Rows[i].Cells[2].Value);
                string situacaoConta = contasPessoaDataGridView.Rows[i].Cells[4].Value.ToString();
                if ((dataConta < DateTime.Now) && (situacaoConta.Trim().Equals("ABERTA")))
                {
                    contasPessoaDataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    contasPessoaDataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            for (int i = 0; i < contasPessoaDataGridView.SelectedRows.Count; i++)
            {
                DateTime dataConta = Convert.ToDateTime(contasPessoaDataGridView.SelectedRows[i].Cells[2].Value);
                string situacaoConta = contasPessoaDataGridView.SelectedRows[i].Cells[4].Value.ToString();
                if ((dataConta < DateTime.Now) && (situacaoConta.Trim().Equals("ABERTA")))
                {
                    contasPessoaDataGridView.SelectedRows[i].DefaultCellStyle.SelectionForeColor = Color.Red;
                }
                else
                {
                    contasPessoaDataGridView.SelectedRows[i].DefaultCellStyle.SelectionForeColor = Color.Black;
                }
            }

        }

        private void codFormaPagamentoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (codFormaPagamentoComboBox.SelectedValue != null)
            {
                int formaPagamento = int.Parse(codFormaPagamentoComboBox.SelectedValue.ToString());
                codCartaoComboBox.Enabled = (formaPagamento == FormaPagamento.CARTAO);
                valorPagamentoTextBox.Enabled = (formaPagamento != FormaPagamento.CARTAO);
                parcelasTextBox.Enabled = (formaPagamento == FormaPagamento.CARTAO);
            }
        }

        private void codFormaPagamentoComboBox_Leave(object sender, EventArgs e)
        {
            int formaPagamento = int.Parse(codFormaPagamentoComboBox.SelectedValue.ToString());
            if ((formaPagamento != FormaPagamento.DINHEIRO) && (formaPagamento != FormaPagamento.CARTAO))
            {
                codFormaPagamentoComboBox.Focus();
                throw new TelaException("Essa forma de pagamento não pode ser utilizada no recebimento de contas.");
            }

        }

        private void faltaReceberTextBox_Leave(object sender, EventArgs e)
        {
            FormatTextBox.NumeroCom2CasasDecimais((TextBox)sender);
        }



    }
}
