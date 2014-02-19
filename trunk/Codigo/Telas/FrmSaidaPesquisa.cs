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

namespace Telas
{
    public partial class FrmSaidaPesquisa : Form
    {
        public Saida SaidaSelected { get; set; }
        
        public FrmSaidaPesquisa()
        {
            InitializeComponent();
            SaidaSelected = null;
        }

        private void FrmSaidaPesquisa_Load(object sender, EventArgs e)
        {
            saidaBindingSource.DataSource = GerenciadorSaida.GetInstance(null).ObterPreVendasPendentes();
            cmbBusca.SelectedIndex = 1;
         }

        private void txtTexto_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbBusca.SelectedIndex == 0)
                {
                    saidaBindingSource.DataSource = GerenciadorSaida.GetInstance(null).ObterPreVendasPendentes();
                }
                else
                    if (txtTexto.Text.Trim().Length > 3)
                    {
                        if (cmbBusca.SelectedIndex == 1)
                            saidaBindingSource.DataSource = GerenciadorSaida.GetInstance(null).Obter(long.Parse(txtTexto.Text));
                        else if (cmbBusca.SelectedIndex == 2)
                            saidaBindingSource.DataSource = GerenciadorSaida.GetInstance(null).ObterPorPedido(txtTexto.Text);
                        else if (cmbBusca.SelectedIndex == 3)
                            saidaBindingSource.DataSource = GerenciadorSaida.GetInstance(null).ObterPorNomeCliente(txtTexto.Text);
                    }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void tb_saidaDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (saidaBindingSource.Count > 0)
            {
                SaidaSelected = (Saida)saidaBindingSource.Current;
            }
            this.Close();
        }

        private void FrmSaidaPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tb_saidaDataGridView_CellClick(sender, null);
            } 
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            } 
            else if ((e.KeyCode == Keys.Down) && (txtTexto.Focused))
            {
                saidaBindingSource.MoveNext();
            }
            else if ((e.KeyCode == Keys.Up) && (txtTexto.Focused))
            {
                saidaBindingSource.MovePrevious();
            }
        }        

        private void cmbBusca_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTexto.Text = "";
            if (cmbBusca.SelectedIndex == 0)
            {
                saidaBindingSource.DataSource = GerenciadorSaida.GetInstance(null).ObterPreVendasPendentes();
            }
        }
      
    }
}
