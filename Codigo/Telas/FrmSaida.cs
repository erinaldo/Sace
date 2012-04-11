﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Negocio;
using Dominio;
using Dados;
using Util;

namespace Telas
{
    public partial class FrmSaida : Form
    {
        private EstadoFormulario estado;
        private Produto produto;
        private Saida saida;
        private SaidaProduto saidaProduto;

        public FrmSaida()
        {
            InitializeComponent();
            saida = new Saida();
            saidaProduto = new SaidaProduto();
            produto = new Produto();
        }

        private void FrmSaida_Load(object sender, EventArgs e)
        {
            //GerenciadorSeguranca.getInstance().verificaPermissao(this, Global.SAIDA, Principal.Autenticacao.CodUsuario);

            GerenciadorSaida.getInstace().atualizarPedidosComDocumentosFiscais();

            this.tb_saidaTableAdapter.Fill(this.saceDataSet.tb_saida);
            this.tb_produtoTableAdapter.Fill(this.saceDataSet.tb_produto, Global.ACRESCIMO_PADRAO);
            tb_saidaBindingSource.MoveLast();
            quantidadeTextBox.Text = "1";
            precoVendatextBox.Text = "0,00";
            habilitaBotoes(true);
            estado = EstadoFormulario.ESPERA;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //Telas.FrmPreVendaPesquisa FrmPreVendaPesquisa = new Telas.FrmPreVendaPesquisa();
            //FrmPreVendaPesquisa.ShowDialog();
            //if (FrmPreVendaPesquisa.getCodGrupo() != -1)
            //{
            //    tb_grupoBindingSource.Position = tb_grupoBindingSource.Find("codGrupo", FrmPreVendaPesquisa.getCodGrupo());
            //}
            //FrmPreVendaPesquisa.Dispose();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            saida.CodCliente = Global.CLIENTE_PADRAO;
            saida.CodProfissional = Global.PROFISSIONAL_PADRAO;
            saida.DataSaida = DateTime.Now;
            saida.Desconto = 0;
            saida.NumeroCartaoVenda = 0;
            saida.PedidoGerado = null;
            saida.TipoSaida = Saida.TIPO_ORCAMENTO;
            saida.Total = 0;
            saida.TotalLucro = 0;
            saida.TotalPago = 0;

            saida.CodSituacaoPagamentos = SituacaoPagamentos.ABERTA;
            saida.Troco = 0;
            saida.Nfe = null;
            entregaRealizadaCheckBox.Checked = true;
            saida.EntregaRealizada = true;
                
                
            GerenciadorSaida.getInstace().inserir(saida);
            tb_saidaTableAdapter.Fill(saceDataSet.tb_saida);
            tb_saidaBindingSource.MoveLast();
                
            codProdutoComboBox.Focus();
            habilitaBotoes(false);
            estado = EstadoFormulario.INSERIR_DETALHE;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            codProdutoComboBox.Focus();
            habilitaBotoes(false);
            estado = EstadoFormulario.INSERIR_DETALHE;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirma exclusão?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GerenciadorSaida.getInstace().remover(Int32.Parse(codSaidaTextBox.Text));
                tb_saidaTableAdapter.Fill(saceDataSet.tb_saida);
                tb_saidaBindingSource.MoveLast(); 
            }
            estado = EstadoFormulario.ESPERA;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            //tbsaidaprodutoBindingSource.CancelEdit();
            //tbsaidaprodutoBindingSource.EndEdit();
                
            if ((tb_saida_produtoDataGridView.RowCount == 0) && (estado.Equals(EstadoFormulario.INSERIR_DETALHE)))
            {
                GerenciadorSaida.getInstace().remover(Int32.Parse(codSaidaTextBox.Text));
                tb_saidaTableAdapter.Fill(saceDataSet.tb_saida);
                tb_saidaBindingSource.MoveLast();
                habilitaBotoes(true);
                estado = EstadoFormulario.ESPERA;
                btnNovo.Focus();
            }
            else if ((tb_saida_produtoDataGridView.RowCount > 0) && (estado.Equals(EstadoFormulario.INSERIR_DETALHE)))
            {
                btnEncerrar_Click(sender, e);
            }
            else
            {
                this.Close();
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            saidaProduto = new SaidaProduto();
            saidaProduto.CodProduto = produto.CodProduto;
            saidaProduto.CodSaida = long.Parse(codSaidaTextBox.Text);
            saidaProduto.Desconto = 10;
            saidaProduto.Quantidade = decimal.Parse(quantidadeTextBox.Text);
            saidaProduto.ValorVenda = decimal.Parse(precoVendaSemDescontoTextBox.Text);
            saidaProduto.ValorVendaAVista = decimal.Parse(precoVendatextBox.Text);
            saidaProduto.DataValidade = Convert.ToDateTime(data_validadeDateTimePicker.Text);
            
            IGerenciadorSaidaProduto gSaidaProduto = GerenciadorSaidaProduto.getInstace();
            if (estado.Equals(EstadoFormulario.INSERIR_DETALHE))
            {
                gSaidaProduto.inserir(saidaProduto);
                codSaidaTextBox_TextChanged(sender, e);
                tbsaidaprodutoBindingSource.MoveLast();

                if (tb_saida_produtoDataGridView.RowCount > 0)
                {
                    Saida saida = GerenciadorSaida.getInstace().obterSaida(Convert.ToInt64(codSaidaTextBox.Text));
                    totalTextBox.Text = saida.Total.ToString();
                    totalAVistaTextBox.Text = saida.TotalAVista.ToString();
                }


                codProdutoComboBox.Focus();
            }
        }

        private void FrmSaida_KeyDown(object sender, KeyEventArgs e)
        {
            if (estado.Equals(EstadoFormulario.ESPERA))
            {
                if (e.KeyCode == Keys.F3)
                {
                    btnNovo_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F2)
                {
                    btnBuscar_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F4)
                {
                    btnEditar_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F5)
                {
                    btnExcluir_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F8)
                {
                    btnImprimir_Click(sender, e);
                }
                else if (e.KeyCode == Keys.End)
                {
                    tb_saidaBindingSource.MoveLast();
                }
                else if (e.KeyCode == Keys.Home)
                {
                    tb_saidaBindingSource.MoveFirst();
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    tb_saidaBindingSource.MovePrevious();
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    tb_saidaBindingSource.MoveNext();
                }
                // Coloca o foco na grid caso ela não possua
                if (e.KeyCode == Keys.F12)
                {
                    tb_saida_produtoDataGridView.Focus();
                }

                // permite excluir um contato quando o foco está na grid
                if ((e.KeyCode == Keys.Delete) && (tb_saida_produtoDataGridView.Focused == true))
                {
                    excluirProduto(sender, e);
                }
                if (e.KeyCode == Keys.Escape)
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
                if (e.KeyCode == Keys.F6)
                {
                    btnSalvar_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F7)
                {
                    btnEncerrar_Click(sender, e);
                }

                if (e.KeyCode == Keys.Escape)
                {
                    btnCancelar_Click(sender, e);
                }

                else if ((e.KeyCode == Keys.F3) && (codProdutoComboBox.Focused))
                {
                    Telas.FrmProduto frmProduto = new Telas.FrmProduto();
                    frmProduto.ShowDialog();
                    if (frmProduto.CodProduto > 0)
                    {
                        this.tb_produtoTableAdapter.Fill(this.saceDataSet.tb_produto, Global.ACRESCIMO_PADRAO);
                        tb_produtoBindingSource.Position = tb_produtoBindingSource.Find("codProduto", frmProduto.CodProduto);
                    }
                    frmProduto.Dispose();
                }
            }
        }

        private void excluirProduto(object sender, EventArgs e)
        {

            if (MessageBox.Show("Confirma exclusão do produto?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (tb_saida_produtoDataGridView.Rows.Count > 0)
                {
                    saidaProduto.CodSaidaProduto = long.Parse(tb_saida_produtoDataGridView.SelectedRows[0].Cells[0].Value.ToString());
                    saidaProduto.CodSaida = Convert.ToInt64(codSaidaTextBox.Text);
                    
                    Negocio.GerenciadorSaidaProduto.getInstace().remover(saidaProduto);
                }
            }
            codSaidaTextBox_TextChanged(sender, e);
        }

        private void habilitaBotoes(Boolean habilita)
        {
            panelBalcao.Visible = habilita;
            lblBalcao.Visible = habilita;
            btnSalvar.Enabled = !(habilita);
            btnEncerrar.Enabled = !(habilita);
            btnCancelar.Enabled = !(habilita);
            btnBuscar.Enabled = habilita;
            btnNovo.Enabled = habilita;
            btnImprimir.Enabled = habilita;
            btnEditar.Enabled = habilita;
            btnExcluir.Enabled = habilita;
            tb_saidaBindingNavigator.Enabled = habilita;
        }

        private void codProdutoComboBox_Leave(object sender, EventArgs e)
        {
            if (estado != EstadoFormulario.ESPERA)
            {
                quantidadeTextBox.Text = "1";
                if (Convert.ToInt32(codProdutoComboBox.SelectedValue) == 1)
                {
                    produto = null;
                    codProdutoComboBox.Text = "";
                }
                else
                {
                    produto = GerenciadorProduto.getInstace().obterProdutoNomeIgual(codProdutoComboBox.Text);
                }
                if (produto == null)
                {
                    Telas.FrmProdutoPesquisaPreco frmProdutoPesquisaPreco = new Telas.FrmProdutoPesquisaPreco(codProdutoComboBox.Text);
                    frmProdutoPesquisaPreco.ShowDialog();
                    if (frmProdutoPesquisaPreco.getCodProduto() != -1)
                    {
                        produto = GerenciadorProduto.getInstace().obterProduto(frmProdutoPesquisaPreco.getCodProduto());
                        codProdutoComboBox.Text = produto.Nome;
                        tb_produtoBindingSource.Position = tb_produtoBindingSource.Find("codProduto", frmProdutoPesquisaPreco.getCodProduto());
                    }
                    else
                    {
                        codProdutoComboBox.Focus();
                    }
                    frmProdutoPesquisaPreco.Dispose();
                } else 
                {
                    tb_produtoBindingSource.Position = tb_produtoBindingSource.Find("codProduto", produto.CodProduto);
                    precoVendatextBox.Text = produto.PrecoVendaVarejo.ToString("N3");
                    precoVendaSemDescontoTextBox.Text = produto.PrecoVendaVarejoSemDesconto.ToString("N3");
                }
            }
        }

        private void quantidadeTextBox_Leave(object sender, EventArgs e)
        {

            FormatTextBox.NumeroCom2CasasDecimais((TextBox) sender);

            decimal quantidade = Convert.ToDecimal(quantidadeTextBox.Text);

            if (produto != null)
            {
                if ((produto.QtdProdutoAtacado != 0) && (quantidade >= produto.QtdProdutoAtacado))
                {
                    precoVendaSemDescontoTextBox.Text = produto.PrecoVendaAtacadoSemDesconto.ToString();
                    precoVendatextBox.Text = produto.PrecoVendaAtacado.ToString();
                }
                else
                {
                    precoVendaSemDescontoTextBox.Text = produto.PrecoVendaVarejoSemDesconto.ToString();
                    precoVendatextBox.Text = produto.PrecoVendaVarejo.ToString();
                }
            }
        }
 


        private void codSaidaTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!codSaidaTextBox.Text.Trim().Equals(""))
            {
                tb_saida_produtoTableAdapter.FillByCodSaida(this.saceDataSet.tb_saida_produto, long.Parse(codSaidaTextBox.Text));
            }
        }

        private void codProdutoComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = e.KeyChar.ToString().ToUpper().ToCharArray()[0];
        }

        private void precoVendatextBox_Leave(object sender, EventArgs e)
        {
            FormatTextBox.NumeroCom2CasasDecimais(precoVendatextBox);
            produto.PrecoVendaVarejo = Convert.ToDecimal(precoVendatextBox.Text);
            precoVendaSemDescontoTextBox.Text = produto.PrecoVendaVarejoSemDesconto.ToString("N2");
        }

        private void btnEncerrar_Click(object sender, EventArgs e)
        {
            if (estado.Equals(EstadoFormulario.INSERIR_DETALHE))
            {
                habilitaBotoes(true);
                estado = EstadoFormulario.ESPERA;
               
                saida = GerenciadorSaida.getInstace().obterSaida(long.Parse(codSaidaTextBox.Text));
                saida.EntregaRealizada = entregaRealizadaCheckBox.Checked;

                FrmSaidaPagamento frmSaidaPagamento = new FrmSaidaPagamento(saida);
                frmSaidaPagamento.ShowDialog();
                frmSaidaPagamento.Dispose();

                GerenciadorSaida.getInstace().atualizarPedidosComDocumentosFiscais();
                this.tb_saidaTableAdapter.Fill(this.saceDataSet.tb_saida);
                tb_saidaBindingSource.MoveLast();
                tb_produtoBindingSource.MoveFirst();
                btnNovo.Focus();
            }
            
        }

        private void quantidadeTextBox_Enter(object sender, EventArgs e)
        {
            quantidadeTextBox.SelectAll();
        }

        private void descricaoTipoSaidaLabel_Click(object sender, EventArgs e)
        {

        }

        private void data_validadeDateTimePicker_Leave(object sender, EventArgs e)
        {
            DateTime dataVencimento = Convert.ToDateTime(data_validadeDateTimePicker.Text);
            if (!GerenciadorSaida.getInstace().dataVencimentoProdutoAceitavel(produto, dataVencimento))
            {
                if (MessageBox.Show("Existem Produtos no estoque com data de validade mais antiga. Manter o Produto lançado?", "Confirmar Data Validade", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    data_validadeDateTimePicker.Focus();
                }
            }
        }

        
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirma impressão?", "Confirmar Impressão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Saida saida = GerenciadorSaida.getInstace().obterSaida(long.Parse(codSaidaTextBox.Text));
                GerenciadorSaida.getInstace().gerarDocumentoFiscal(saida);
            }
        
        }
    }
}