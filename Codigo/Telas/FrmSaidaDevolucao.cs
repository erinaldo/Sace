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
using Dados;

namespace Telas
{
    public partial class FrmSaidaDevolucao : Form
    {
        private Saida saida;

        public FrmSaidaDevolucao(Saida saida)
        {
            InitializeComponent();
            this.saida = saida;
        }

        private void FrmSaidaDevolucao_Load(object sender, EventArgs e)
        {
            codSaidaTextBox.Text = saida.CodSaida.ToString();

            this.tb_tipo_saidaTableAdapter.Fill(this.saceDataSet.tb_tipo_saida);
            this.tb_saidaTableAdapter.Fill(this.saceDataSet.tb_saida);
            IEnumerable<Loja> lojas = GerenciadorLoja.GetInstance().ObterTodos();
            lojaBindingSourceDestino.DataSource = lojas;
            lojaBindingSourceOrigem.DataSource = lojas;
            IEnumerable<Pessoa> pessoas = GerenciadorPessoa.GetInstance().ObterTodos();
            pessoaDestinoBindingSource.DataSource = pessoas;
            pessoaFreteBindingSource.DataSource = pessoas;
            
            tb_saidaBindingSource.Position = tb_saidaBindingSource.Find("codSaida", saida.CodSaida);
        }  

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            saida.CodCliente = Convert.ToInt64(codClienteComboBox.SelectedValue.ToString());
            saida.CodEmpresaFrete = Convert.ToInt64( codEmpresaFreteComboBox.SelectedValue.ToString() );
            saida.Desconto = Convert.ToDecimal(descontoTextBox.Text);
            saida.EspecieVolumes = especieVolumesTextBox.Text;
            saida.Marca = marcaTextBox.Text;
            saida.Numero = Convert.ToDecimal(numeroTextBox.Text);
            saida.OutrasDespesas = Convert.ToDecimal(outrasDespesasTextBox.Text);
            saida.PesoBruto = Convert.ToDecimal(pesoBrutoTextBox.Text);
            saida.PesoLiquido = Convert.ToDecimal(pesoLiquidoTextBox.Text);
            saida.QuantidadeVolumes = Convert.ToDecimal(quantidadeVolumesTextBox.Text);
            saida.TotalNotaFiscal = Convert.ToDecimal(totalNotaFiscalTextBox.Text);
            saida.ValorFrete = Convert.ToDecimal(valorFreteTextBox.Text);
            saida.ValorICMS = Convert.ToDecimal(valorICMSTextBox.Text);
            saida.ValorICMSSubst = Convert.ToDecimal(valorICMSSubstTextBox.Text);
            saida.ValorIPI = Convert.ToDecimal(valorIPITextBox.Text);
            saida.ValorSeguro = Convert.ToDecimal(valorSeguroTextBox.Text);
            saida.BaseCalculoICMS = Convert.ToDecimal(baseCalculoICMSTextBox.Text);
            saida.BaseCalculoICMSSubst = Convert.ToDecimal(baseCalculoICMSSubstTextBox.Text);
            saida.CodSituacaoPagamentos = SituacaoPagamentos.LANCADOS;
            saida.TotalLucro = 0;

            if (MessageBox.Show("Confirma dados da Nota Fiscal?", "Confirmar Dados da Nota Fiscal", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GerenciadorSaida.getInstace().encerrar(saida, Saida.TIPO_DEVOLUCAO_FRONECEDOR);

                if (MessageBox.Show("Confirma Impressão da Nota Fiscal?", "Confirmar Impressão", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    GerenciadorSaida.getInstace().imprimirNotaFiscal(saida);
                }
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSaidaDevolucao_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F6)
            {
                btnSalvar_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnCancelar_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{tab}");
            }
        }

        private void valorFreteTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            FormatTextBox.NumeroCom2CasasDecimais(textBox);

            if (textBox.Name.Equals("valorFreteTextBox") || textBox.Name.Equals("valorSeguroTextBox") ||
                textBox.Name.Equals("descontoTextBox") || textBox.Name.Equals("outrasDespesasTextBox"))
            {
                saida.ValorFrete = Convert.ToDecimal(valorFreteTextBox.Text);
                saida.ValorSeguro = Convert.ToDecimal(valorSeguroTextBox.Text);
                saida.Desconto = Convert.ToDecimal(descontoTextBox.Text);
                saida.OutrasDespesas = Convert.ToDecimal(outrasDespesasTextBox.Text);

                totalNotaFiscalTextBox.Text = GerenciadorSaida.getInstace().ObterTotalNotaFiscal(saida).ToString("N2");
            }
            codSaidaTextBox_Leave(sender, e);
        }

        private void quantidadeVolumesTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            FormatTextBox.NumeroCom2CasasDecimais(textBox);
            codSaidaTextBox_Leave(sender, e);
        }

        private void codEmpresaFreteComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.Parse(e.KeyChar.ToString().ToUpper());
        }

        private void codEmpresaFreteComboBox_Leave(object sender, EventArgs e)
        {
            List<PessoaE> pessoas = (List<PessoaE>) GerenciadorPessoa.GetInstance().ObterPorNomeFantasia(codEmpresaFreteComboBox.Text);
            if (pessoas.Count == 0)
            {
                Telas.FrmPessoaPesquisa frmPessoaPesquisa = new Telas.FrmPessoaPesquisa(codEmpresaFreteComboBox.Text);
                frmPessoaPesquisa.ShowDialog();
                if (frmPessoaPesquisa.PessoaSelected != null)
                {
                    pessoaFreteBindingSource.Position = pessoaFreteBindingSource.List.IndexOf(frmPessoaPesquisa.PessoaSelected);
                }
                else
                {
                    codEmpresaFreteComboBox.Focus();
                }
                frmPessoaPesquisa.Dispose();
            }
            else
            {
                pessoaFreteBindingSource.Position = pessoaFreteBindingSource.List.IndexOf(pessoas[0]);
            }
            codSaidaTextBox_Leave(sender, e);
        }

        private void codClienteComboBox_Leave(object sender, EventArgs e)
        {
            List<PessoaE> pessoas = (List<PessoaE>) GerenciadorPessoa.GetInstance().ObterPorNomeFantasia(codClienteComboBox.Text);
            if (pessoas.Count == 0)
            {
                Telas.FrmPessoaPesquisa frmPessoaPesquisa = new Telas.FrmPessoaPesquisa(codClienteComboBox.Text);
                frmPessoaPesquisa.ShowDialog();
                if (frmPessoaPesquisa.PessoaSelected != null)
                {
                    pessoaDestinoBindingSource.Position = pessoaDestinoBindingSource.List.IndexOf(frmPessoaPesquisa.PessoaSelected);
                    codClienteComboBox.Text = frmPessoaPesquisa.PessoaSelected.Nome;
                }
                else
                {
                    codClienteComboBox.Focus();
                }
                frmPessoaPesquisa.Dispose();
            }
            else
            {
                pessoaDestinoBindingSource.Position = pessoaDestinoBindingSource.Find("codPessoa", pessoas[0].codPessoa);
            }
            codSaidaTextBox_Leave(sender, e);
        }

        private void codSaidaTextBox_Enter(object sender, EventArgs e)
        {
            if ((sender is Control) && !(sender is Form))
            {
                Control control = (Control)sender;
                control.BackColor = Global.BACKCOLOR_FOCUS;
            }
        }

        private void codSaidaTextBox_Leave(object sender, EventArgs e)
        {
            if ((sender is Control) && !(sender is Form))
            {
                Control control = (Control)sender;
                control.BackColor = Global.BACKCOLOR_FOCUS_LEAVE;
            }
        }

       
    }
}