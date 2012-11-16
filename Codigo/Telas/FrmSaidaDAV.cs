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
    public partial class FrmSaidaDAV : Form
    {

        public Int64 CodSaida { get; set; }
        
        public FrmSaidaDAV(Int64 codSaida)
        {
            InitializeComponent();
            CodSaida = codSaida;
        }

        private void btnNotmal_Click(object sender, EventArgs e)
        {
            this.Close();
            Saida saida = GerenciadorSaida.getInstace().obterSaida(CodSaida);
            GerenciadorSaida.getInstace().imprimirDAV(new List<Saida>(){saida}, saida.Total, saida.TotalAVista, saida.Desconto, false);
        }

        private void btnReduzido_Click(object sender, EventArgs e)
        {
            this.Close();
            Saida saida = GerenciadorSaida.getInstace().obterSaida(CodSaida);
            GerenciadorSaida.getInstace().imprimirDAV(new List<Saida>() { saida }, saida.Total, saida.TotalAVista, saida.Desconto, true);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSaidaDAV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SendKeys.Send("{tab}");
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnCancelar_Click(sender, e);
            }
        }
    }
}
