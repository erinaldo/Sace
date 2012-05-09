﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Negocio;
using Dados;
using Dominio;
using Util;

namespace Telas
{
    public partial class FrmGrupoConta : Form
    {
        private EstadoFormulario estado;
        
        private Int32 codGrupoConta;

        public Int32 CodGrupoConta
        {
            get { return codGrupoConta; }
            set { codGrupoConta = value; }
        }
        public FrmGrupoConta()
        {
            InitializeComponent();
        }

        private void FrmGrupoConta_Load(object sender, EventArgs e)
        {
            GerenciadorSeguranca.getInstance().verificaPermissao(this, Global.GRUPOS_DE_CONTAS, Principal.Autenticacao.CodUsuario);
            this.tb_grupo_contaTableAdapter.Fill(this.saceDataSet.tb_grupo_conta);
            habilitaBotoes(true);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Telas.FrmGrupoContaPesquisa frmTipoContaPesquisa = new Telas.FrmGrupoContaPesquisa();
            frmTipoContaPesquisa.ShowDialog();
            if (frmTipoContaPesquisa.CodGrupoConta != -1)
            {
                tb_grupo_contaBindingSource.Position = tb_grupo_contaBindingSource.Find("codGrupoConta", frmTipoContaPesquisa.CodGrupoConta);
            }
            frmTipoContaPesquisa.Dispose();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            tb_grupo_contaBindingSource.AddNew();
            codGrupoContaTextBox.Enabled = false;
            descricaoTextBox.Focus();
            habilitaBotoes(false);
            estado = EstadoFormulario.INSERIR;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            descricaoTextBox.Focus();
            habilitaBotoes(false);
            estado = EstadoFormulario.ATUALIZAR;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirma exclusão?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GerenciadorGrupoConta.getInstace().remover(Int32.Parse(codGrupoContaTextBox.Text));
                tb_grupo_contaTableAdapter.Fill(saceDataSet.tb_grupo_conta);
            }

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            tb_grupo_contaBindingSource.CancelEdit();
            tb_grupo_contaBindingSource.EndEdit();
            habilitaBotoes(true);
            btnBuscar.Focus();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                GrupoConta grupoConta = new GrupoConta();
                grupoConta.CodGrupoConta = Int32.Parse(codGrupoContaTextBox.Text);
                grupoConta.Descricao = descricaoTextBox.Text;

                IGerenciadorGrupoConta gGrupoConta = GerenciadorGrupoConta.getInstace();
                if (estado.Equals(EstadoFormulario.INSERIR))
                {
                    gGrupoConta.inserir(grupoConta);
                    tb_grupo_contaTableAdapter.Fill(saceDataSet.tb_grupo_conta);
                    tb_grupo_contaBindingSource.MoveLast();
                }
                else
                {
                    gGrupoConta.atualizar(grupoConta);
                    tb_grupo_contaBindingSource.EndEdit();
                }
            }
            catch (DadosException de)
            {
                tb_grupo_contaBindingSource.CancelEdit();
                throw de;
            }
            finally
            {
                habilitaBotoes(true);
                btnBuscar.Focus();
            }
            

        }

        private void FrmGrupoConta_KeyDown(object sender, KeyEventArgs e)
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
                    tb_grupo_contaBindingSource.MoveLast();
                }
                else if (e.KeyCode == Keys.Home)
                {
                    tb_grupo_contaBindingSource.MoveFirst();
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    tb_grupo_contaBindingSource.MovePrevious();
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    tb_grupo_contaBindingSource.MoveNext();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            else
            {
                if ((e.KeyCode == Keys.F7) || (e.KeyCode == Keys.Escape))
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
            tb_grupo_contaBindingNavigator.Enabled = habilita;
            if (habilita)
            {
                estado = EstadoFormulario.ESPERA;
            }
        }

        private void FrmGrupoConta_FormClosing(object sender, FormClosingEventArgs e)
        {
            CodGrupoConta = Int32.Parse(codGrupoContaTextBox.Text);
        }
    }
}