﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dados.saceDataSetTableAdapters;
using Dados;
using Dominio;
using Negocio;
using Util;


namespace Telas
{
    public partial class FrmBanco : Form
    {
        private EstadoFormulario estado;
        
        public Int32 CodBanco;

        public FrmBanco()
        {
            InitializeComponent();
        }

        private void FrmBanco_Load(object sender, EventArgs e)
        {
            GerenciadorSeguranca.getInstance().verificaPermissao(this, Global.BANCOS, Principal.Autenticacao.CodUsuario);

            bancoBindingSource.DataSource = GerenciadorBanco.GetInstace().ObterTodos(); 
            habilitaBotoes(true);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Telas.FrmBancoPesquisa frmBancoPesquisa = new Telas.FrmBancoPesquisa();
            frmBancoPesquisa.ShowDialog();
            if (frmBancoPesquisa.CodBanco != -1)
            {
                Banco _banco = GerenciadorBanco.GetInstace().Obter(frmBancoPesquisa.CodBanco).ElementAt(0);
                bancoBindingSource.Position = bancoBindingSource.List.IndexOf(_banco);
            }
            frmBancoPesquisa.Dispose();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            bancoBindingSource.AddNew();
            codBancoTextBox.Enabled = false;
            nomeTextBox.Focus();
            habilitaBotoes(false);
            estado = EstadoFormulario.INSERIR;
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
                GerenciadorBanco.GetInstace().remover(int.Parse(codBancoTextBox.Text));
                bancoBindingSource.DataSource = GerenciadorBanco.GetInstace().ObterTodos();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            bancoBindingSource.CancelEdit();
            bancoBindingSource.EndEdit();
            habilitaBotoes(true);
            btnBuscar.Focus();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Banco banco = new Banco();
                banco.CodBanco = Int32.Parse(codBancoTextBox.Text);
                banco.Nome = nomeTextBox.Text;

                GerenciadorBanco gBanco = GerenciadorBanco.GetInstace();
                if (estado.Equals(EstadoFormulario.INSERIR))
                {
                    gBanco.inserir(banco);
                    bancoBindingSource.DataSource = GerenciadorBanco.GetInstace().ObterTodos();
                    bancoBindingSource.MoveLast();
                }
                else
                {
                    gBanco.atualizar(banco);
                    bancoBindingSource.EndEdit();
                }
            }
            catch (DadosException de)
            {
                bancoBindingSource.CancelEdit();
                throw de;
            }
            finally
            {
                habilitaBotoes(true);
                btnBuscar.Focus();
            }
        }

        private void FrmBanco_KeyDown(object sender, KeyEventArgs e)
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
                    bancoBindingSource.MoveLast();
                }
                else if (e.KeyCode == Keys.Home)
                {
                    bancoBindingSource.MoveFirst();
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    bancoBindingSource.MovePrevious();
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    bancoBindingSource.MoveNext();
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
                } else if ((e.KeyCode == Keys.F7) || (e.KeyCode == Keys.Escape))
                {
                    btnCancelar_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F6)
                {
                    btnSalvar_Click(sender, e);
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
            tb_bancoBindingNavigator.Enabled = habilita;
            if (habilita)
            {
                estado = EstadoFormulario.ESPERA;
            }
        }

        private void FrmBanco_FormClosing(object sender, FormClosingEventArgs e)
        {
            CodBanco = int.Parse(codBancoTextBox.Text);
        }
    }
}