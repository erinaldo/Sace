﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dominio;
using Negocio;

namespace Telas
{
    public partial class FrmSubgrupoPesquisa : Form
    {
        public Subgrupo SubgrupoSelected { get; set; }
        public Grupo GrupoSelected { get; set; }

        public FrmSubgrupoPesquisa()
        {
            InitializeComponent();
            SubgrupoSelected = null;
            GrupoSelected = null;
        }

        private void FrmSubgrupoPesquisa_Load(object sender, EventArgs e)
        {
            subgrupoBindingSource.DataSource = GerenciadorSubgrupo.GetInstance().ObterTodos();
            cmbBusca.SelectedIndex = 1;
        }

        private void txtTexto_TextChanged(object sender, EventArgs e)
        {
            if ((cmbBusca.SelectedIndex == 0) && !txtTexto.Text.Equals(""))
                subgrupoBindingSource.DataSource = GerenciadorSubgrupo.GetInstance().Obter(Convert.ToInt32(txtTexto.Text));
            else
                subgrupoBindingSource.DataSource = GerenciadorSubgrupo.GetInstance().ObterPorDescricao(txtTexto.Text);                
            
        }

        private void tb_bancoDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SubgrupoSelected = (Subgrupo) subgrupoBindingSource.Current;
            GrupoSelected = GerenciadorGrupo.GetInstance().Obter(SubgrupoSelected.CodGrupo).ElementAt(0);
            this.Close();
        }

        private void FrmSubgrupoPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tb_bancoDataGridView_CellClick(sender, null);
            } 
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            } 
            else if ((e.KeyCode == Keys.Down) && (txtTexto.Focused))
            {
                subgrupoBindingSource.MoveNext();
            }
            else if ((e.KeyCode == Keys.Up) && (txtTexto.Focused))
            {
                subgrupoBindingSource.MovePrevious();
            }
        }

        private void cmbBusca_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTexto.Text = "";
        }
    }
}
