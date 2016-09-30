﻿using System;
using System.Windows.Forms;
using Dados;
using Dominio;
using Negocio;
using Util;


namespace Telas
{
    public partial class FrmCartaoCredito : Form
    {
        private EstadoFormulario estado;

        public FrmCartaoCredito()
        {
            InitializeComponent();
        }

        private void FrmCartaoCredito_Load(object sender, EventArgs e)
        {
            GerenciadorSeguranca.getInstance().verificaPermissao(this, Global.CARTÕES_DE_CREDITO, Principal.Autenticacao.CodUsuario);
            cartaoCreditoBindingSource.DataSource = GerenciadorCartaoCredito.GetInstance().ObterTodos();
            contaBancoBindingSource.DataSource = GerenciadorContaBanco.GetInstance().ObterTodos();
            pessoaBindingSource.DataSource = GerenciadorPessoa.GetInstance().ObterPorTipoPessoa(Pessoa.PESSOA_JURIDICA);
            
            habilitaBotoes(true);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Telas.FrmCartaoCreditoPesquisa frmCartaoCreditoPesquisa = new Telas.FrmCartaoCreditoPesquisa();
            frmCartaoCreditoPesquisa.ShowDialog();
            if (frmCartaoCreditoPesquisa.CartaoCreditoSelected != null)
            {
                cartaoCreditoBindingSource.Position = cartaoCreditoBindingSource.List.IndexOf(frmCartaoCreditoPesquisa.CartaoCreditoSelected);
            }
            frmCartaoCreditoPesquisa.Dispose();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            cartaoCreditoBindingSource.AddNew();
            codCartaoTextBox.Enabled = false;
            habilitaBotoes(false);
            codContaBancoComboBox.SelectedIndex = 0;
            codPessoaComboBox.SelectedIndex = 0;
            CartaoCredito cartaoCredito = (CartaoCredito)cartaoCreditoBindingSource.Current;
            cartaoCredito.CodContaBanco = ((ContaBanco)contaBancoBindingSource.Current).CodContaBanco;
            cartaoCredito.CodPessoa = ((Pessoa)pessoaBindingSource.Current).CodPessoa;
            estado = EstadoFormulario.INSERIR;
            nomeTextBox.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            nomeTextBox.Focus();
            habilitaBotoes(false);
            estado = EstadoFormulario.ATUALIZAR;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirma exclusão?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GerenciadorCartaoCredito.GetInstance().Remover(int.Parse(codCartaoTextBox.Text));
                cartaoCreditoBindingSource.RemoveCurrent();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            cartaoCreditoBindingSource.CancelEdit();
            cartaoCreditoBindingSource.EndEdit();
            habilitaBotoes(true);
            btnBuscar.Focus();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                CartaoCredito _cartaoCredito = (CartaoCredito) cartaoCreditoBindingSource.Current;
                
                if (estado.Equals(EstadoFormulario.INSERIR))
                {
                    long codCartao = GerenciadorCartaoCredito.GetInstance().Inserir(_cartaoCredito);
                    codCartaoTextBox.Text = codCartao.ToString();
                }
                else
                {
                    GerenciadorCartaoCredito.GetInstance().Atualizar(_cartaoCredito);
                }
                cartaoCreditoBindingSource.EndEdit();
            }
            catch (DadosException de)
            {
                cartaoCreditoBindingSource.CancelEdit();
                throw de;
            }
            finally
            {
                habilitaBotoes(true);
                btnBuscar.Focus();
            }
        }

        private void FrmCartaoCredito_KeyDown(object sender, KeyEventArgs e)
        {
            if (estado.Equals(EstadoFormulario.ESPERA))
            {
                if (e.KeyCode == Keys.F2)
                {
                    btnBuscar_Click(sender, e);
                }
                if (e.KeyCode == Keys.F3)
                {
                    btnNovo_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F4)
                {
                    btnEditar_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F5)
                {
                    btnExcluir_Click(sender, e);
                }
                else if (e.KeyCode == Keys.End)
                {
                    cartaoCreditoBindingSource.MoveLast();
                }
                else if (e.KeyCode == Keys.Home)
                {
                    cartaoCreditoBindingSource.MoveFirst();
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    cartaoCreditoBindingSource.MovePrevious();
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    cartaoCreditoBindingSource.MoveNext();
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
                    if (codPessoaComboBox.Focused)
                    {
                        codPessoaComboBox_Leave(sender, e);
                    }
                    e.Handled = true;
                    SendKeys.Send("{tab}");
                }
                
                if (e.KeyCode == Keys.Escape)
                {
                    btnCancelar_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F6)
                {
                    btnSalvar_Click(sender, e);
                }
                else if ((e.KeyCode == Keys.F2) && (codContaBancoComboBox.Focused))
                {
                    Telas.FrmContaBancoPesquisa frmContaBancoPesquisa = new Telas.FrmContaBancoPesquisa();
                    frmContaBancoPesquisa.ShowDialog();
                    if (frmContaBancoPesquisa.ContaBancoSelected != null)
                    {
                        contaBancoBindingSource.Position = contaBancoBindingSource.List.IndexOf(frmContaBancoPesquisa.ContaBancoSelected);
                    }
                    frmContaBancoPesquisa.Dispose();
                }
                else if ((e.KeyCode == Keys.F3) && (codContaBancoComboBox.Focused))
                {
                    Telas.FrmContaBanco frmContaBanco = new Telas.FrmContaBanco();
                    frmContaBanco.ShowDialog();
                    if (frmContaBanco.ContaBancoSelected != null)
                    {
                        contaBancoBindingSource.Position = contaBancoBindingSource.List.IndexOf(frmContaBanco.ContaBancoSelected);
                    }
                    frmContaBanco.Dispose();
                }
                else if ((e.KeyCode == Keys.F3) && (codPessoaComboBox.Focused))
                {
                    Telas.FrmPessoa frmPessoa = new Telas.FrmPessoa();
                    frmPessoa.ShowDialog();
                    if (frmPessoa.PessoaSelected != null)
                    {
                        pessoaBindingSource.Position = pessoaBindingSource.List.IndexOf(frmPessoa.PessoaSelected);
                    }
                    frmPessoa.Dispose();
                }
            }
        }
        private void habilitaBotoes(Boolean habilita)
        {
            btnSalvar.Enabled = !(habilita);
            btnCancelar.Enabled = !(habilita);
            btnBuscar.Enabled = habilita;
            btnEditar.Enabled = habilita;
            btnNovo.Enabled = habilita;
            btnExcluir.Enabled = habilita;
            tb_cartao_creditoBindingNavigator.Enabled = habilita;
            codContaBancoComboBox.Enabled = !(habilita);
            if (habilita)
            {
                estado = EstadoFormulario.ESPERA;
            }
        }

        private void codContaBancoComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.Parse(e.KeyChar.ToString().ToUpper());
        }

        private void codContaBancoComboBox_Leave(object sender, EventArgs e)
        {
            if (codContaBancoComboBox.SelectedValue == null)
            {
                codContaBancoComboBox.Focus();
                throw new TelaException("Uma conta de banco válida precisa ser selecionada.");
            }
        }


        private void codPessoaComboBox_Leave(object sender, EventArgs e)
        {
            ComponentesLeave.PessoaComboBox_Leave(sender, e, codPessoaComboBox, estado, pessoaBindingSource, true, true);
        }

        private void codCartaoTextBox_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textbox = (TextBox)sender;
                textbox.BackColor = Global.BACKCOLOR_FOCUS;
            }
            else if (sender is ComboBox)
            {
                ComboBox combobox = (ComboBox) sender;
                combobox.BackColor = Global.BACKCOLOR_FOCUS;
            }
        }

        private void codCartaoTextBox_Leave(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox textbox = (TextBox)sender;
                textbox.BackColor = Global.BACKCOLOR_FOCUS_LEAVE;
            }
            else if (sender is ComboBox)
            {
                ComboBox combobox = (ComboBox)sender;
                combobox.BackColor = Global.BACKCOLOR_FOCUS_LEAVE;
            }
        }

        private void nomeTextBox_Leave(object sender, EventArgs e)
        {
            FormatTextBox.RemoverAcentos((TextBox)sender);
            codCartaoTextBox_Leave(sender, e);
        }

        private void mapeamentoLabel_Click(object sender, EventArgs e)
        {

        }

        private void mapeamentoTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void diaBaseTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void descontoTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void descontoLabel_Click(object sender, EventArgs e)
        {

        }

        private void diaBaseLabel_Click(object sender, EventArgs e)
        {

        }

        private void codPessoaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tipoCartaoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}