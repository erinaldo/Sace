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
    public partial class FrmSaidaDeposito : Form
    {
        private Saida saida;

        public FrmSaidaDeposito(Saida saida)
        {
            InitializeComponent();
            this.saida = saida;
        }

        private void FrmSaidaDeposito_Load(object sender, EventArgs e)
        {
            codSaidaTextBox.Text = saida.CodSaida.ToString();
            saidaBindingSource.DataSource = GerenciadorSaida.GetInstance(null).Obter(saida.CodSaida);
            saida = (Saida) saidaBindingSource.Current;
            List<Loja> listaLojas = GerenciadorLoja.GetInstance().ObterTodos();
            lojaBindingSourceDestino.DataSource = listaLojas;
            lojaBindingSourceOrigem.DataSource = listaLojas;
            if (saida.TipoSaida.Equals(Saida.TIPO_PRE_REMESSA_DEPOSITO))
            {
                codPessoaComboBoxOrigem.SelectedIndex = 0;
                codPessoaComboBoxDestino.SelectedIndex = 1;
            }
            else if (saida.TipoSaida.Equals(Saida.TIPO_PRE_RETORNO_DEPOSITO))
            {
                codPessoaComboBoxOrigem.SelectedIndex = 1;
                codPessoaComboBoxDestino.SelectedIndex = 0;
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            saida.CodProfissional = Global.CLIENTE_PADRAO;
            saida.CodCliente = long.Parse(codPessoaComboBoxDestino.SelectedValue.ToString());
            
            GerenciadorSaida.GetInstance(null).Atualizar(saida);
            if (saida.TipoSaida.Equals(Saida.TIPO_PRE_REMESSA_DEPOSITO))
            {
                if (MessageBox.Show("Confirma REMESSA para DEPÓSITO?", "Confirmar Remessa", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    GerenciadorSaida.GetInstance(null).Encerrar(saida, Saida.TIPO_REMESSA_DEPOSITO, null);

                    FrmSaidaNFe frmSaidaNF = new FrmSaidaNFe(saida.CodSaida);
                    frmSaidaNF.ShowDialog();
                    frmSaidaNF.Dispose();
                    this.Close();
                }
            }
            else if (saida.TipoSaida.Equals(Saida.TIPO_PRE_RETORNO_DEPOSITO))
            {
                if (MessageBox.Show("Confirma RETORNO de DEPÓSITO FECHADO?", "Confirmar Retorno", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    GerenciadorSaida.GetInstance(null).Encerrar(saida, Saida.TIPO_RETORNO_DEPOSITO, null);

                    FrmSaidaNFe frmSaidaNF = new FrmSaidaNFe(saida.CodSaida);
                    frmSaidaNF.ShowDialog();
                    frmSaidaNF.Dispose();
                    this.Close();
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSaidaDeposito_KeyDown(object sender, KeyEventArgs e)
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
    }
}