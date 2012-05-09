﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Dominio;
using Negocio;
using Util;
using System.Windows.Forms;

namespace Telas
{
    public partial class FrmSaidaPagamento : Form
    {
        private Saida saida;
        private decimal faltaReceber = 0;

        public FrmSaidaPagamento(Saida saida)
        {
            InitializeComponent();
            this.saida = saida;
        }

        private void FrmSaidaPagamento_Load(object sender, EventArgs e)
        {
            codSaidaTextBox.Text = saida.CodSaida.ToString();

            this.tb_tipo_saidaTableAdapter.Fill(this.saceDataSet.tb_tipo_saida);
            this.tb_saida_forma_pagamentoTableAdapter.FillByCodSaida(saceDataSet.tb_saida_forma_pagamento, saida.CodSaida);
            this.tb_forma_pagamentoTableAdapter.Fill(this.saceDataSet.tb_forma_pagamento);
            this.tb_pessoaTableAdapter.Fill(this.saceDataSet.tb_pessoa);
            this.tb_conta_bancoTableAdapter.Fill(this.saceDataSet.tb_conta_banco);
            this.tb_cartao_creditoTableAdapter.Fill(this.saceDataSet.tb_cartao_credito);
            this.tb_documento_pagamentoTableAdapter.Fill(this.saceDataSet.tb_documento_pagamento);

            this.tb_saidaTableAdapter.Fill(this.saceDataSet.tb_saida);

            tb_saidaBindingSource.Position = tb_saidaBindingSource.Find("codSaida", saida.CodSaida);

            codCartaoComboBox.SelectedIndex = 0;
            codFormaPagamentoComboBox.SelectedIndex = 0;
            codDocumentoPagamentoComboBox.SelectedIndex = 0;
            codContaBancoComboBox.SelectedIndex = 0;
            intervaloDiasTextBox.Text = Global.QUANTIDADE_DIAS_CREDIARIO.ToString();


            saida.Desconto = (1 - (saida.TotalAVista / saida.Total)) * 100;
            descontoTextBox.Text = saida.Desconto.ToString("N3");  

            
            atualizaValores();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            SaidaPagamento saidaPagamento = new SaidaPagamento();
            saidaPagamento.CodContaBanco = int.Parse(codContaBancoComboBox.SelectedValue.ToString());
            saidaPagamento.CodFormaPagamento = int.Parse(codFormaPagamentoComboBox.SelectedValue.ToString());
            saidaPagamento.CodCartaoCredito = int.Parse(codCartaoComboBox.SelectedValue.ToString());
            saidaPagamento.Data = DateTime.Now;
            saidaPagamento.Valor = decimal.Parse(valorRecebidoTextBox.Text);
            saidaPagamento.CodSaida = saida.CodSaida;
            saidaPagamento.CodDocumentoPagamento = int.Parse(codDocumentoPagamentoComboBox.SelectedValue.ToString());
            saidaPagamento.CodPessoaResponsavel = long.Parse(codClienteComboBox.SelectedValue.ToString());
            saidaPagamento.IntervaloDias = Convert.ToInt32(intervaloDiasTextBox.Text);
            saidaPagamento.Parcelas = Convert.ToInt32(parcelasTextBox.Text);

            saida.CodProfissional = long.Parse(codProfissionalComboBox.SelectedValue.ToString());
            saida.CodCliente = long.Parse(codClienteComboBox.SelectedValue.ToString());
            saida.Desconto = decimal.Parse(descontoTextBox.Text);
            saida.CpfCnpj = cpf_CnpjTextBox.Text;
            saida.Total = decimal.Parse(totalTextBox.Text);
            saida.TotalPago = Convert.ToDecimal(totalRecebidoLabel.Text);
            
            GerenciadorSaidaPagamento.getInstace().inserir(saidaPagamento, saida);

            atualizaValores();
            
            this.tb_saida_forma_pagamentoTableAdapter.FillByCodSaida(saceDataSet.tb_saida_forma_pagamento, long.Parse(codSaidaTextBox.Text));
            
            if (faltaReceber <= 0)
            {
                btnEncerrar_Click(sender, e);
            }
            else
            {
                codFormaPagamentoComboBox.Focus();
            }
            valorRecebidoTextBox.Enabled = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            decimal totalPagamentos = GerenciadorSaidaPagamento.getInstace().totalPagamentos(saida.CodSaida);
            if (totalPagamentos < saida.TotalAVista)
            {
                if (MessageBox.Show("Deseja sair sem lançar os pagamentos?", "Confirmar Saída", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }

        }

        private void FrmSaidaPagamento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6)
            {
                btnSalvar_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F7)
            {
                btnEncerrar_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnCancelar_Click(sender, e);
            }
            // Coloca o foco na grid caso ela não possua
            else if ((e.KeyCode == Keys.F12) && (tb_saida_forma_pagamentoDataGridView.Focused))
            {
                codFormaPagamentoComboBox.Focus();
            }
            else if (e.KeyCode == Keys.F12)
            {
                tb_saida_forma_pagamentoDataGridView.Focus();
            }

            // permite excluir um contato quando o foco está na grid
            else if ((e.KeyCode == Keys.Delete) && (tb_saida_forma_pagamentoDataGridView.Focused == true))
            {
                excluirPagamento(sender, e);
            }
            
            
            else if (e.KeyCode == Keys.Enter)
            {
                if (codClienteComboBox.Focused)
                {
                    codClienteComboBox_Leave(sender, e);
                }
                else if (codProfissionalComboBox.Focused)
                {
                    codProfissionalComboBox_Leave(sender, e);
                }
                
                e.Handled = true;
                SendKeys.Send("{tab}");
            }

            else if ((e.KeyCode == Keys.F3) && (codClienteComboBox.Focused))
            {
                Telas.FrmPessoa frmPessoa = new Telas.FrmPessoa();
                frmPessoa.ShowDialog();
                if (frmPessoa.CodPessoa > 0)
                {
                    this.tb_pessoaTableAdapter.Fill(this.saceDataSet.tb_pessoa);
                    tbpessoaBindingSource.Position = tbpessoaBindingSource.Find("codPessoa", frmPessoa.CodPessoa);
                }
                frmPessoa.Dispose();
            }

            else if ((e.KeyCode == Keys.F3) && (codProfissionalComboBox.Focused))
            {
                Telas.FrmPessoa frmPessoa = new Telas.FrmPessoa();
                frmPessoa.ShowDialog();
                if (frmPessoa.CodPessoa > 0)
                {
                    this.tb_pessoaTableAdapter.Fill(this.saceDataSet.tb_pessoa);
                    tbpessoaBindingSource1.Position = tbpessoaBindingSource1.Find("codPessoa", frmPessoa.CodPessoa);
                }
                frmPessoa.Dispose();
            }
            else if ((e.KeyCode == Keys.F2) && (codDocumentoPagamentoComboBox.Focused))
            {
                Telas.FrmDocumentoPagamentoPesquisa frmDocumentoPagamentoPesquisa = new Telas.FrmDocumentoPagamentoPesquisa();
                frmDocumentoPagamentoPesquisa.ShowDialog();
                if (frmDocumentoPagamentoPesquisa.CodDocumentoPagamento != -1)
                {
                    tbdocumentopagamentoBindingSource.Position = tbdocumentopagamentoBindingSource.Find("codDocumentoPagamento", frmDocumentoPagamentoPesquisa.CodDocumentoPagamento);
                }
                frmDocumentoPagamentoPesquisa.Dispose();
            }
            else if ((e.KeyCode == Keys.F3) && (codDocumentoPagamentoComboBox.Focused))
            {
                Int32 formaPagamentoSelecionada = Convert.ToInt32(codFormaPagamentoComboBox.SelectedValue.ToString());

                Int32 codTipoDocumento = 0;
                if (formaPagamentoSelecionada == FormaPagamento.BOLETO)
                    codTipoDocumento = DocumentoPagamento.TIPO_BOLETO;
                else if (formaPagamentoSelecionada == FormaPagamento.CHEQUE)
                    codTipoDocumento = DocumentoPagamento.TIPO_CHEQUE;
                else
                    codTipoDocumento = DocumentoPagamento.TIPO_PROMISSORIA;

                Int32 codPessoa = Convert.ToInt32(codClienteComboBox.SelectedValue);

                Telas.FrmDocumentoPagamento frmDocumentoPagamento = new Telas.FrmDocumentoPagamento(codTipoDocumento, codPessoa);
                frmDocumentoPagamento.ShowDialog();
                if (frmDocumentoPagamento.CodDocumentoPagamento > 0)
                {
                    this.tb_documento_pagamentoTableAdapter.Fill(this.saceDataSet.tb_documento_pagamento);
                    tbdocumentopagamentoBindingSource.Position = tbdocumentopagamentoBindingSource.Find("codDocumentoPagamento", frmDocumentoPagamento.CodDocumentoPagamento);
                }
                frmDocumentoPagamento.Dispose();
            }
        }

        private void excluirPagamento(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirma exclusão do pagamento?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (tb_saida_forma_pagamentoDataGridView.Rows.Count > 0)
                {
                    long codSaidaPagamento = long.Parse(tb_saida_forma_pagamentoDataGridView.SelectedRows[0].Cells[0].Value.ToString());
                    
                    
                    
                    
                    Negocio.GerenciadorSaidaPagamento.getInstace().remover(codSaidaPagamento, saida);
                    this.tb_saida_forma_pagamentoTableAdapter.FillByCodSaida(saceDataSet.tb_saida_forma_pagamento, saida.CodSaida);

                    codCartaoComboBox.SelectedIndex = 0;
                    codFormaPagamentoComboBox.SelectedIndex = 0;
                    codDocumentoPagamentoComboBox.SelectedIndex = 0;
                    codContaBancoComboBox.SelectedIndex = 0;
                    intervaloDiasTextBox.Text = Global.QUANTIDADE_DIAS_CREDIARIO.ToString(); 

                    atualizaValores();

                    codFormaPagamentoComboBox.Focus();
                }
            }
        }

        private void atualizaValores()
        {  
            totalRecebidoLabel.Text = saida.TotalPago.ToString("N2"); 
            faltaReceber = saida.TotalAVista - saida.TotalPago;
            if (faltaReceber > 0)
                faltaReceberTextBox.Text = faltaReceber.ToString("N2");
            else
                faltaReceberTextBox.Text = "0";
            if (saida.Troco > 0)
            {
                trocoTextBox.Text = saida.Troco.ToString("N2");
            }
            else
            {
                trocoTextBox.Text = "0,00";
            }


            Int32 codFormaPagamento = Convert.ToInt32(codFormaPagamentoComboBox.SelectedValue);
            if ((codFormaPagamento != FormaPagamento.CHEQUE) && (codFormaPagamento != FormaPagamento.BOLETO))
            {
                valorRecebidoTextBox.Text = faltaReceberTextBox.Text;
            }
            descontoTextBox.Text = saida.Desconto.ToString("N2");
        }
        
        private void codTipoSaidaComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.Parse(e.KeyChar.ToString().ToUpper());
        }

              
        private void totalPagarTextBox_Leave(object sender, EventArgs e)
        {
            FormatTextBox.NumeroCom2CasasDecimais((TextBox)sender);
            saida.TotalAVista = Convert.ToDecimal(totalPagarTextBox.Text);
            atualizaValores();
        }

        private void codClienteComboBox_Leave(object sender, EventArgs e)
        {
            Pessoa pessoa = GerenciadorPessoa.getInstace().obterPessoaNomeIgual(codClienteComboBox.Text);
            if (pessoa == null)
            {
                Telas.FrmPessoaPesquisa frmPessoaPesquisa = new Telas.FrmPessoaPesquisa(codClienteComboBox.Text);
                frmPessoaPesquisa.ShowDialog();
                if (frmPessoaPesquisa.CodPessoa != -1)
                {
                    tbpessoaBindingSource.Position = tbpessoaBindingSource.Find("codPessoa", frmPessoaPesquisa.CodPessoa);
                }
                else
                {
                    codClienteComboBox.Focus();
                }
                frmPessoaPesquisa.Dispose();
            }
            else
            {
                tbpessoaBindingSource.Position = tbpessoaBindingSource.Find("codPessoa", pessoa.CodPessoa);
            }
        }

        private void codProfissionalComboBox_Leave(object sender, EventArgs e)
        {
            Pessoa pessoa = GerenciadorPessoa.getInstace().obterPessoaNomeIgual(codProfissionalComboBox.Text);
            if (pessoa == null)
            {
                Telas.FrmPessoaPesquisa frmPessoaPesquisa = new Telas.FrmPessoaPesquisa(codProfissionalComboBox.Text);
                frmPessoaPesquisa.ShowDialog();
                if (frmPessoaPesquisa.CodPessoa != -1)
                {
                    tbpessoaBindingSource1.Position = tbpessoaBindingSource1.Find("codPessoa", frmPessoaPesquisa.CodPessoa);
                }
                else
                {
                    codProfissionalComboBox.Focus();
                }
                frmPessoaPesquisa.Dispose();
            }
            else
            {
                tbpessoaBindingSource1.Position = tbpessoaBindingSource.Find("codPessoa", pessoa.CodPessoa);
            }
        }

        private void valorRecebidoTextBox_Leave(object sender, EventArgs e)
        {
            FormatTextBox.NumeroCom2CasasDecimais((TextBox)sender);
        }

        private void btnEncerrar_Click(object sender, EventArgs e)
        {
            long codSaida = Int64.Parse(codSaidaTextBox.Text);
            Saida saida = GerenciadorSaida.getInstace().obterSaida(codSaida);

            bool temPagamentoCrediario = GerenciadorSaidaPagamento.getInstace().obterSaidaPagamentosPorFormaPagamento(codSaida, FormaPagamento.CREDIARIO).Count > 0;

            if (temPagamentoCrediario)
            {
                if (MessageBox.Show("Confirma formas de pagamento?", "Confirmar Pagamentos", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    GerenciadorSaida.getInstace().encerrar(saida);

                    if (MessageBox.Show("Deseja gerar documento Fiscal?", "Gerar Documento", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        GerenciadorSaida.getInstace().gerarDocumentoFiscal(saida);
                    }
                    this.Close();
                }

            } else {

                if (MessageBox.Show("Confirma formas de pagamentos e impressão fiscal?", "Confirmar Pagamentos", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    GerenciadorSaida.getInstace().encerrar(saida);
                    GerenciadorSaida.getInstace().gerarDocumentoFiscal(saida);
                    this.Close();
                }
            }
        }

        private void descontoTextBox_Leave(object sender, EventArgs e)
        {
            const decimal ERRO = 0.02M;
            
            FormatTextBox.NumeroCom2CasasDecimais((TextBox)sender);
            saida.Desconto = Convert.ToDecimal(descontoTextBox.Text);
            decimal totalCalculado = saida.Total * (1 - (saida.Desconto / 100));

            if ( ((totalCalculado - saida.TotalAVista) > ERRO) || ((totalCalculado - saida.TotalAVista) < ERRO))
            {
                saida.TotalAVista = totalCalculado;
            }
            
            totalPagarTextBox.Text = saida.TotalAVista.ToString("N2");
        }

        private void codFormaPagamentoComboBox_Leave(object sender, EventArgs e)
        {
            if (codFormaPagamentoComboBox.SelectedValue == null)
            {
                codFormaPagamentoComboBox.Focus();
                throw new TelaException("Uma forma de pagamento válida precisa ser selecionada.");
            }
            else {
                Int32 codFormaPagamento = Convert.ToInt32(codFormaPagamentoComboBox.SelectedValue);
                Int32 codCliente = Convert.ToInt32(codClienteComboBox.SelectedValue);

                if (((codFormaPagamento == FormaPagamento.BOLETO) || (codFormaPagamento == FormaPagamento.CHEQUE) ||
                (codFormaPagamento== FormaPagamento.CREDIARIO)) && (codCliente == Global.CLIENTE_PADRAO) )
                {
                    codFormaPagamentoComboBox.Focus();
                    this.tb_forma_pagamentoTableAdapter.Fill(this.saceDataSet.tb_forma_pagamento);
                    codFormaPagamentoComboBox.SelectedIndex = 0;
                    throw new TelaException("Para utilizar essa forma de pagamento é necessário selecionar um cliente.");
                    
                }
            }
        }

        private void codCartaoComboBox_Leave(object sender, EventArgs e)
        {
            if (codCartaoComboBox.SelectedValue == null)
            {
                codCartaoComboBox.Focus();
                throw new TelaException("Um cartão de crédito válido precisa ser selecionado.");
            }            
        }

        private void codDocumentoPagamentoComboBox_Leave(object sender, EventArgs e)
        {
            if (codDocumentoPagamentoComboBox.SelectedValue == null)
            {
                codDocumentoPagamentoComboBox.Focus();
                throw new TelaException("Um documento válido precisa ser selecionado.");
            }
            long codDocumentoPagamento = Convert.ToInt64(codDocumentoPagamentoComboBox.SelectedValue.ToString());

            DocumentoPagamento documento =  GerenciadorDocumentoPagamento.getInstace().obterDocumentoPagamento( codDocumentoPagamento );

            valorRecebidoTextBox.Text = documento.Valor.ToString();
            valorRecebidoTextBox.Enabled = false;
        }

        private void codContaBancoComboBox_Leave(object sender, EventArgs e)
        {
            if (codContaBancoComboBox.SelectedValue == null)
            {
                codContaBancoComboBox.Focus();
                throw new TelaException("Uma conta de banco / caixa válida precisa ser selecionada.");
            }
        }

        private void parcelasTextBox_Leave(object sender, EventArgs e)
        {
            if (Convert.ToInt32(parcelasTextBox.Text) > 1)
            {
                intervaloDiasTextBox.TabStop = true;
            }
        }

        private void faltaReceberTextBox_TextChanged(object sender, EventArgs e)
        {
            faltaReceber = Convert.ToDecimal(faltaReceberTextBox.Text);
            btnEncerrar.Enabled = (faltaReceber == 0);
        }

        private void codClienteComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( codClienteComboBox.SelectedValue != null) 
            {
                cpf_CnpjTextBox.Enabled = Convert.ToInt64(codClienteComboBox.SelectedValue.ToString()) == 1;
            }
        }

        private void codFormaPagamentoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (codFormaPagamentoComboBox.SelectedValue != null)
            {
                int formaPagamento = int.Parse(codFormaPagamentoComboBox.SelectedValue.ToString());
                parcelasTextBox.Enabled = (formaPagamento == FormaPagamento.CARTAO) || (formaPagamento == FormaPagamento.PROMISSORIA);
                codCartaoComboBox.Enabled = (formaPagamento == FormaPagamento.CARTAO);
                codContaBancoComboBox.Enabled = (formaPagamento == FormaPagamento.DEPOSITO);
                codDocumentoPagamentoComboBox.Enabled = (formaPagamento == FormaPagamento.CHEQUE) || (formaPagamento == FormaPagamento.BOLETO);
                valorRecebidoTextBox.Enabled = !((formaPagamento == FormaPagamento.CHEQUE) || (formaPagamento == FormaPagamento.BOLETO));
                intervaloDiasTextBox.Enabled = (formaPagamento == FormaPagamento.CREDIARIO) || (formaPagamento == FormaPagamento.DEPOSITO) || (formaPagamento == FormaPagamento.PROMISSORIA);
            }
        }

    }
}