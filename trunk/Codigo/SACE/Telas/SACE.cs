﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SACE
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        static void Main(string[] args)
        {
            Principal principal = new Principal();
            principal.ShowDialog();

        }

        private void Principal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                pessoasToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F8)
            {
                empresasToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnSair_Click(sender, e);
            }
        }

        private void btnCliente_Click(object sender, EventArgs e)
        {
            //FrmCliente formCliente = new FrmCliente();
            //formCliente.ShowDialog();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gruposDeProdutosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Telas.FrmGrupo frmGrupo = new Telas.FrmGrupo();
            frmGrupo.ShowDialog();
            frmGrupo.Dispose();
        }

        private void lojasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Telas.FrmLoja frmLoja = new Telas.FrmLoja();
            frmLoja.ShowDialog();
            frmLoja.Dispose();
        }

        private void btnVenda_Click(object sender, EventArgs e)
        {
            
        }

        private void btnProdutos_Click(object sender, EventArgs e)
        {

        }

        private void produtosToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pessoasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Telas.FrmPessoa frmPessoa = new Telas.FrmPessoa();
            frmPessoa.ShowDialog();
            frmPessoa.Dispose();
        }

        private void empresasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Telas.FrmEmpresa frmEmpresa = new Telas.FrmEmpresa();
            frmEmpresa.ShowDialog();
            frmEmpresa.Dispose();
        }

        private void bancosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Telas.FrmBanco frmBanco = new Telas.FrmBanco();
            frmBanco.ShowDialog();
            frmBanco.Dispose();
        }

        private void contasBancáriasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Telas.FrmContaBanco frmContaBanco = new Telas.FrmContaBanco();
            frmContaBanco.ShowDialog();
            frmContaBanco.Dispose();
        }

        private void cartõesDeCréditoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Telas.FrmCartaoCredito frmCartaoCredito = new Telas.FrmCartaoCredito();
            frmCartaoCredito.ShowDialog();
            frmCartaoCredito.Dispose();
        }

    }
}
