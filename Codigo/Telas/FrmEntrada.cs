﻿using System;
using System.Windows.Forms;
using Negocio;
using Dominio;
using Util;
using System.Data;
using Dados;
using System.Collections.Generic;

namespace Telas
{
    public partial class FrmEntrada : Form
    {
        private EstadoFormulario estado;
        private Entrada entrada;
        private EntradaProduto entradaProduto;
        private Int32 tipoEntrada;
        private ProdutoPesquisa produtoPesquisa;
        private string ultimoCodigoBarraLido = "";

        public FrmEntrada()
        {
            InitializeComponent();
        }

        private void FrmEntrada_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            GerenciadorSeguranca.getInstance().verificaPermissao(this, Global.ENTRADA_PRODUTOS, Principal.Autenticacao.CodUsuario);
            produtoBindingSource.DataSource = GerenciadorProduto.GetInstance().ObterTodos();
            fornecedorBindingSource.DataSource = GerenciadorPessoa.GetInstance().ObterTodos();
            empresaFreteBindingSource.DataSource = GerenciadorPessoa.GetInstance().ObterTodos();
            cfopBindingSource.DataSource = GerenciadorCfop.GetInstance().ObterTodos();
            cstBindingSource.DataSource = GerenciadorCst.GetInstance().ObterTodos();
            entradaBindingSource.DataSource = GerenciadorEntrada.GetInstance().ObterTodos();
            situacaoPagamentosBindingSource.DataSource = GerenciadorEntrada.GetInstance().ObterTodosSituacoesPagamentos();
            entradaBindingSource.MoveLast();
            entrada = new Entrada();
            entradaProduto = new EntradaProduto();
            tipoEntrada = Entrada.TIPO_ENTRADA;
            habilitaBotoes(true);

            Cursor.Current = Cursors.Default;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Telas.FrmEntradaPesquisa frmEntradaPesquisa = new Telas.FrmEntradaPesquisa();
            frmEntradaPesquisa.ShowDialog();
            if (frmEntradaPesquisa.EntradaSelected != null)
            {
                entradaBindingSource.Position = entradaBindingSource.List.IndexOf(frmEntradaPesquisa.EntradaSelected);
            }
            frmEntradaPesquisa.Dispose();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            entradaBindingSource.AddNew();
            entradaProdutoBindingSource.DataSource = new List<EntradaProduto>();
            codEntradaTextBox.Enabled = false;
            numeroNotaFiscalTextBox.Focus();
            codEmpresaFreteComboBox.SelectedIndex = 0;
            codFornecedorComboBox.SelectedIndex = 0;
            codSituacaoPagamentosComboBox.SelectedIndex = 0;
            fretePagoEmitenteCheckBox.Checked = true;
            habilitaBotoes(false);
            estado = EstadoFormulario.INSERIR;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            habilitaBotoes(false);
            numeroNotaFiscalTextBox.Focus();
            estado = EstadoFormulario.ATUALIZAR;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Confirma exclusão?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GerenciadorEntrada.GetInstance().Remover(Convert.ToInt64(codEntradaTextBox.Text));
                entradaBindingSource.RemoveCurrent();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            estado = EstadoFormulario.ESPERA;
            entradaBindingSource.CancelEdit();
            entradaBindingSource.EndEdit();
            entradaProdutoBindingSource.CancelEdit();
            entradaProdutoBindingSource.EndEdit();
            ProdutosGroupBox.Enabled = false;
            habilitaBotoes(true);
            codEntradaTextBox_TextChanged(sender, e);
            btnBuscar.Focus();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            entrada = (Entrada)entradaBindingSource.Current;

            GerenciadorEntrada gEntrada = GerenciadorEntrada.GetInstance();
            if (estado.Equals(EstadoFormulario.INSERIR))
            {
                entrada.CodTipoEntrada = tipoEntrada;
                entrada.CodSituacaoPagamentos = SituacaoPagamentos.ABERTA;
                entrada.CodEntrada = gEntrada.Inserir(entrada);
                codEntradaTextBox.Text = entrada.CodEntrada.ToString();
                habilitaBotoes(true);
                btnProdutos.Focus();
            }
            else if (estado.Equals(EstadoFormulario.INSERIR_DETALHE))
            {
                GerenciadorEntradaProduto gEntradaProduto = GerenciadorEntradaProduto.GetInstance(null);
                entradaProduto = (EntradaProduto)entradaProdutoBindingSource.Current;
                entradaProduto.CodProduto = Convert.ToInt32(codProdutoComboBox.SelectedValue.ToString());
                entradaProduto.BaseCalculoICMS = Convert.ToDecimal(baseCalculoICMSTextBox.Text);
                entradaProduto.BaseCalculoICMSST = Convert.ToDecimal(baseCalculoICMSSTTextBox.Text);
                entradaProduto.Cfop = Convert.ToInt32(cfopComboBox.SelectedValue.ToString());
                entradaProduto.CodCST = codCSTComboBox.SelectedValue.ToString();
                entradaProduto.CodEntrada = entrada.CodEntrada;
                entradaProduto.DataEntrada = entrada.DataEntrada;
                entradaProduto.DataValidade = Convert.ToDateTime(data_validadeDateTimePicker.Text);
                entradaProduto.Frete = Convert.ToDecimal(freteTextBox.Text);
                entradaProduto.LucroPrecoVendaAtacado = Convert.ToDecimal(lucroPrecoVendaAtacadoTextBox.Text);
                entradaProduto.LucroPrecoVendaVarejo = Convert.ToDecimal(lucroPrecoVendaVarejoTextBox.Text);
                entradaProduto.PrecoCusto = Convert.ToDecimal(preco_custoTextBox.Text);
                entradaProduto.PrecoVendaAtacado = Convert.ToDecimal(precoVendaAtacadoTextBox.Text);
                entradaProduto.PrecoVendaVarejo = Convert.ToDecimal(precoVendaVarejoTextBox.Text);

                entradaProduto.PrecoRevenda = Convert.ToDecimal(precoRevendaTextBox.Text);
                entradaProduto.LucroPrecoRevenda = Convert.ToDecimal(lucroPrecoRevendaTextBox.Text);

                entradaProduto.Quantidade = Convert.ToDecimal(quantidadeTextBox.Text);
                entradaProduto.QuantidadeEmbalagem = Convert.ToDecimal(quantidadeEmbalagemTextBox.Text);
                entradaProduto.QuantidadeDisponivel = entradaProduto.Quantidade * entradaProduto.QuantidadeEmbalagem;
                entradaProduto.Simples = Convert.ToDecimal(simplesTextBox.Text);
                entradaProduto.Icms = Convert.ToDecimal(icmsTextBox.Text);
                entradaProduto.IcmsSubstituto = Convert.ToDecimal(icms_substitutoTextBox.Text);
                entradaProduto.Ipi = Convert.ToDecimal(ipiTextBox.Text);
                entradaProduto.Ncmsh = ncmshTextBox.Text;
                entradaProduto.UnidadeCompra = unidadeCompraTextBox.Text;
                entradaProduto.ValorUnitario = Convert.ToDecimal(valorUnitarioTextBox.Text);
                entradaProduto.QtdProdutoAtacado = Convert.ToDecimal(qtdProdutoAtacadoTextBox.Text);
                entradaProduto.Desconto = Convert.ToDecimal(descontoProdutoTextBox.Text);

                entradaProduto.FornecedorEhFabricante = ((Pessoa)fornecedorBindingSource.Current).EhFabricante;
                entradaProduto.CodFornecedor = ((Pessoa)fornecedorBindingSource.Current).CodPessoa;

                GerenciadorEntradaProduto.GetInstance(null).Inserir(entradaProduto, entrada.CodTipoEntrada);
                codEntradaTextBox_TextChanged(sender, e);
                btnProdutos_Click(sender, e);
            }
            else
            {
                gEntrada.Atualizar(entrada);
                produtoBindingSource.Position = 0;
                habilitaBotoes(true);
                btnProdutos.Focus();
            }
            entradaBindingSource.EndEdit();
            entradaProdutoBindingSource.EndEdit();
        }

        private void excluirProduto(object sender, EventArgs e)
        {

            if (MessageBox.Show("Confirma exclusão do produto?", "Confirmar Exclusão", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (tb_entrada_produtoDataGridView.Rows.Count > 0)
                {
                    EntradaProduto _entradaProduto = (EntradaProduto)entradaProdutoBindingSource.Current;
                    Entrada _entrada = (Entrada)entradaBindingSource.Current;
                    GerenciadorEntradaProduto.GetInstance(null).Remover(_entradaProduto, entrada.CodTipoEntrada);
                }
            }
            codEntradaTextBox_TextChanged(sender, e);
        }

        private void quantidadeTextBox_Validated(object sender, EventArgs e)
        {
            entradaProduto = (EntradaProduto)entradaProdutoBindingSource.Current;
            if (entradaProduto != null)
            {
                if (entradaProduto.EhTributacaoIntegral)
                    baseCalculoICMSTextBox.Text = entradaProduto.ValorTotal.ToString();
                else
                    baseCalculoICMSSTTextBox.Text = entradaProduto.ValorTotal.ToString();
                Produto produto = (Produto) produtoBindingSource.Current;
                entradaProduto.Icms = produto.Icms;
                entradaProduto.IcmsSubstituto = produto.IcmsSubstituto;
                entradaProduto.Ipi = produto.Ipi;

                produto.UltimoPrecoCompra = entradaProduto.ValorUnitario;
                produto.Desconto = Convert.ToDecimal(descontoProdutoTextBox.Text);
                valorICMSTextBox.Text = entradaProduto.ValorICMS.ToString();
                valorICMSSTTextBox.Text = entradaProduto.ValorICMSST.ToString();
                valorIPITextBox.Text = entradaProduto.ValorIPI.ToString();
                    preco_custoTextBox.Text = produto.PrecoCusto.ToString();
                precoAtacadoSugestaoTextBox.Text = produto.PrecoVendaAtacadoSugestao.ToString();
                precoVarejoSugestaoTextBox.Text = produto.PrecoVendaVarejoSugestao.ToString();
            }
        }


        private void codigoFornecedorComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = e.KeyChar.ToString().ToUpper().ToCharArray()[0];
        }

        private void btnProdutos_Click(object sender, EventArgs e)
        {
            entradaProdutoBindingSource.AddNew();
            if (tb_entrada_produtoDataGridView.RowCount == 0)
            {
                codProdutoComboBox.SelectedIndex = 0;
                cfopComboBox.SelectedIndex = 0;
            }
            ProdutosGroupBox.Enabled = true;
            codProdutoComboBox.Focus();
            habilitaBotoes(false);
            estado = EstadoFormulario.INSERIR_DETALHE;

        }

        private void codFornecedorComboBox_Leave(object sender, EventArgs e)
        {
            ComponentesLeave.PessoaComboBox_Leave(sender, e, codFornecedorComboBox, estado, fornecedorBindingSource, false);
            codEntradaTextBox_Leave(sender, e);
        }

        private void codEmpresaFreteComboBox_Leave(object sender, EventArgs e)
        {
            ComponentesLeave.PessoaComboBox_Leave(sender, e, codEmpresaFreteComboBox, estado, empresaFreteBindingSource, false);
            codEntradaTextBox_Leave(sender, e);
        }

        private void codProdutoComboBox_Leave(object sender, EventArgs e)
        {
            if (estado.Equals(EstadoFormulario.INSERIR_DETALHE))
            {
                produtoPesquisa = ComponentesLeave.ProdutoComboBox_Leave(sender, e, codProdutoComboBox, estado, produtoBindingSource, ref ultimoCodigoBarraLido, true);

                EntradaProduto entradaProduto = (EntradaProduto)entradaProdutoBindingSource.Current;
                if (produtoPesquisa != null)
                {
                    data_validadeDateTimePicker.Enabled = produtoPesquisa.TemVencimento;
                    entradaProduto.NomeProduto = produtoPesquisa.Nome;
                    if (produtoPesquisa.QuantidadeEmbalagem <= 0)
                        quantidadeEmbalagemTextBox.Text = "1";
                    if (entradaProduto.Quantidade <= 0)
                        quantidadeTextBox.Text = "1";
                    if (entradaProduto.ValorUnitario <= 0)
                        entradaProduto.ValorUnitario = produtoPesquisa.UltimoPrecoCompra;
                    if (!produtoPesquisa.EhTributacaoIntegral)
                    {
                        produtoPesquisa = (ProdutoPesquisa)produtoBindingSource.Current;
                        entrada = (Entrada) entradaBindingSource.Current;
                        if (entrada.TotalProdutosST > 0)
                        {
                            produtoPesquisa.IcmsSubstituto = Math.Round(entrada.TotalSubstituicao / entrada.TotalProdutosST * 100, 2);
                        }
                        else
                        {
                            produtoPesquisa.IcmsSubstituto = 0;
                        }
                    }
                    cfopComboBox.SelectedIndex = 0;
                    codCSTComboBox_SelectedIndexChanged(sender, e);
                    codEntradaTextBox_Leave(sender, e);
                }
            }
        }

        private void codEntradaTextBox_TextChanged(object sender, EventArgs e)
        {
            decimal totalNotaCalculado = 0;
            if ((!codEntradaTextBox.Text.Trim().Equals("")) && (long.Parse(codEntradaTextBox.Text) > 1))
            {
                IEnumerable<EntradaProduto> listaEntradaProduto = GerenciadorEntradaProduto.GetInstance(null).ObterPorEntrada(long.Parse(codEntradaTextBox.Text));
                entradaProdutoBindingSource.DataSource = listaEntradaProduto;
                foreach (EntradaProduto entradaProduto in listaEntradaProduto)
                {
                    totalNotaCalculado += entradaProduto.ValorTotal;
                }
            }
            totalNotaCalculadoTextBox.Text = totalNotaCalculado.ToString("N2");
        }


        private void btnPagamentos_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt64(codEntradaTextBox.Text) > 0)
            {
                habilitaBotoes(true);
                estado = EstadoFormulario.ESPERA;
                entrada = GerenciadorEntrada.GetInstance().Obter(Convert.ToInt64(codEntradaTextBox.Text)).GetEnumerator().Current;
                //FrmEntradaPagamento frmEntradaPagamento = new FrmEntradaPagamento(entrada);
                //frmEntradaPagamento.ShowDialog();
                //frmEntradaPagamento.Dispose();
                btnNovo.Focus();
            }
        }

        private void btnImportarNfe_Click(object sender, EventArgs e)
        {
            if (openFileDialogNfe.ShowDialog() == DialogResult.OK)
            {
                string nomearquivo = openFileDialogNfe.FileName;
                TNfeProc nfe = GerenciadorNFe.GetInstance().LerNFE(nomearquivo);
                if (MessageBox.Show("Deseja importar CABEÇALHO da NF-e?", "Confirmar Importar NF-e", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    long codEntrada = GerenciadorEntrada.GetInstance().Importar(nfe);
                    fornecedorBindingSource.DataSource = GerenciadorPessoa.GetInstance().ObterTodos();
                    empresaFreteBindingSource.DataSource = GerenciadorPessoa.GetInstance().ObterTodos();
                    entradaBindingSource.DataSource = GerenciadorEntrada.GetInstance().ObterTodos();
                    entradaBindingSource.Position = entradaBindingSource.List.IndexOf(new Entrada() { CodEntrada = codEntrada });
                    Cursor.Current = Cursors.Default;
                }
                if (MessageBox.Show("Deseja importar PRODUTOS da NF-e?", "Confirmar Importar NF-e", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    List<EntradaProduto> listaEntradaProduto = GerenciadorEntradaProduto.GetInstance(null).Importar(nfe);
                    if (listaEntradaProduto.Count > 0)
                    {
                        Entrada entrada = (Entrada)entradaBindingSource.Current;
                        if (entrada.CodEntrada != listaEntradaProduto[0].CodEntrada)
                        {
                            entradaBindingSource.Position = entradaBindingSource.List.IndexOf(new Entrada() { CodEntrada = listaEntradaProduto[0].CodEntrada });
                        }
                    }
                    FrmEntradaImportar frmEntradaImportar = new FrmEntradaImportar(listaEntradaProduto);
                    frmEntradaImportar.ShowDialog();
                    frmEntradaImportar.Dispose();
                    codEntradaTextBox_TextChanged(sender, e);
                }
            }
        }

        private void precoVendaAtacadoTextBox_Leave(object sender, EventArgs e)
        {
            codEntradaTextBox_Leave(sender, e);
        }

        private void codCSTComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if ((codCSTComboBox.SelectedValue != null) && (ProdutosGroupBox.Enabled) && estado.Equals(EstadoFormulario.INSERIR_DETALHE))
            {
                
                Cst cst = new Cst() { CodCST = codCSTComboBox.SelectedValue.ToString() };
                bool ehTributadoIntegral = cst.EhTributacaoIntegral;

                baseCalculoICMSSTTextBox.ReadOnly = ehTributadoIntegral;
                baseCalculoICMSSTTextBox.TabStop = !baseCalculoICMSSTTextBox.ReadOnly;
                icms_substitutoTextBox.ReadOnly = ehTributadoIntegral;
                icms_substitutoTextBox.TabStop = !icms_substitutoTextBox.ReadOnly;

                baseCalculoICMSTextBox.ReadOnly = !ehTributadoIntegral;
                baseCalculoICMSTextBox.TabStop = !baseCalculoICMSTextBox.ReadOnly;
                icmsTextBox.ReadOnly = !ehTributadoIntegral;
                icmsTextBox.TabStop = !icmsTextBox.ReadOnly;

                entradaProduto = (EntradaProduto) entradaProdutoBindingSource.Current;
                entradaProduto.CodCST = cst.CodCST;
                if (!entradaProduto.EhTributacaoIntegral && (entrada.TotalProdutosST > 0))
                    entradaProduto.IcmsSubstituto = entrada.TotalSubstituicao / entrada.TotalProdutosST * 100;
                else
                    entradaProduto.IcmsSubstituto = 0;
                icms_substitutoTextBox.Text = entradaProduto.IcmsSubstituto.ToString("N2");

                if (entrada.ValorFrete > 0)
                    entradaProduto.Frete = ((entrada.ValorFrete / entrada.TotalProdutos) * 100);
                else
                    entradaProduto.Frete = 0;
                freteTextBox.Text = entradaProduto.Frete.ToString("N2");

                if (entrada.Desconto > 0)
                {
                    entradaProduto.Desconto = (entrada.Desconto / entrada.TotalProdutos) * 100;
                    descontoProdutoTextBox.Text = entradaProduto.Desconto.ToString("N2");
                }
            }
            codEntradaTextBox_Leave(sender, e);
            entradaProdutoBindingSource.ResumeBinding();
        }

        private void codEntradaTextBox_Enter(object sender, EventArgs e)
        {
            if ((sender is Control) && !(sender is Form))
            {
                Control control = (Control)sender;
                control.BackColor = Global.BACKCOLOR_FOCUS;
            }
        }

        private void codEntradaTextBox_Leave(object sender, EventArgs e)
        {
            if ((sender is Control) && !(sender is Form))
            {
                Control control = (Control)sender;
                control.BackColor = Global.BACKCOLOR_FOCUS_LEAVE;
            }
        }


        private void FrmEntrada_KeyDown(object sender, KeyEventArgs e)
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
                else if (e.KeyCode == Keys.F7)
                {
                    btnProdutos_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F8)
                {
                    btnPagamentos_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F9)
                {
                    btnImportarNfe_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F12)
                {
                    if (tipoEntrada == Entrada.TIPO_ENTRADA)
                    {
                        FrmEntrada.ActiveForm.Text = "Entrada de Produtox";
                        tipoEntrada = Entrada.TIPO_ENTRADA_AUX;
                    }
                    else
                    {
                        FrmEntrada.ActiveForm.Text = "Entrada de Produtos";
                        tipoEntrada = Entrada.TIPO_ENTRADA;
                    }
                }
                else if (e.KeyCode == Keys.End)
                {
                    entradaBindingSource.MoveLast();
                }
                else if (e.KeyCode == Keys.Home)
                {
                    entradaBindingSource.MoveFirst();
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    entradaBindingSource.MovePrevious();
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    entradaBindingSource.MoveNext();
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
                    if (codFornecedorComboBox.Focused)
                    {
                        codFornecedorComboBox_Leave(sender, e);
                    }
                    else if (codEmpresaFreteComboBox.Focused)
                    {
                        codEmpresaFreteComboBox_Leave(sender, e);
                    }
                    else if (codProdutoComboBox.Focused)
                    {
                        codProdutoComboBox_Leave(sender, e);
                    }

                    e.Handled = true;
                    SendKeys.Send("{tab}");
                }
                if ((e.KeyCode == Keys.F7) || (e.KeyCode == Keys.Escape))
                {
                    btnCancelar_Click(sender, e);
                }
                else if (e.KeyCode == Keys.F6)
                {
                    btnSalvar_Click(sender, e);
                }
                else if ((e.KeyCode == Keys.F2) && (codFornecedorComboBox.Focused))
                {
                    Telas.FrmPessoaPesquisa frmPessoaPesquisa = new Telas.FrmPessoaPesquisa();
                    frmPessoaPesquisa.ShowDialog();
                    if (frmPessoaPesquisa.PessoaSelected != null)
                    {
                        fornecedorBindingSource.Position = fornecedorBindingSource.List.IndexOf(frmPessoaPesquisa.PessoaSelected);
                    }
                    frmPessoaPesquisa.Dispose();
                }
                else if ((e.KeyCode == Keys.F3) && (codFornecedorComboBox.Focused))
                {
                    Telas.FrmPessoa frmPessoa = new Telas.FrmPessoa();
                    frmPessoa.ShowDialog();
                    if (frmPessoa.PessoaSelected != null)
                    {
                        fornecedorBindingSource.DataSource = GerenciadorPessoa.GetInstance().ObterTodos();
                        fornecedorBindingSource.Position = fornecedorBindingSource.List.IndexOf(frmPessoa.PessoaSelected);
                    }
                    frmPessoa.Dispose();
                }
                else if ((e.KeyCode == Keys.F2) && (codEmpresaFreteComboBox.Focused))
                {
                    Telas.FrmPessoaPesquisa frmPessoaPesquisa = new Telas.FrmPessoaPesquisa();
                    frmPessoaPesquisa.ShowDialog();
                    if (frmPessoaPesquisa.PessoaSelected != null)
                    {
                        empresaFreteBindingSource.Position = empresaFreteBindingSource.List.IndexOf(frmPessoaPesquisa.PessoaSelected);
                    }
                    frmPessoaPesquisa.Dispose();
                }
                else if ((e.KeyCode == Keys.F3) && (codEmpresaFreteComboBox.Focused))
                {
                    Telas.FrmPessoa frmPessoa = new Telas.FrmPessoa();
                    frmPessoa.ShowDialog();
                    if (frmPessoa.PessoaSelected != null)
                    {
                        empresaFreteBindingSource.DataSource = GerenciadorPessoa.GetInstance().ObterTodos();
                        empresaFreteBindingSource.Position = empresaFreteBindingSource.List.IndexOf(frmPessoa.PessoaSelected);
                    }
                    frmPessoa.Dispose();
                }
                else if ((e.KeyCode == Keys.F2) && (codProdutoComboBox.Focused))
                {
                    Telas.FrmProdutoPesquisaPreco frmProdutoPesquisaPreco = new Telas.FrmProdutoPesquisaPreco(true);
                    frmProdutoPesquisaPreco.ShowDialog();
                    if (frmProdutoPesquisaPreco.ProdutoPesquisa != null)
                    {
                        produtoBindingSource.Position = produtoBindingSource.List.IndexOf(frmProdutoPesquisaPreco.ProdutoPesquisa);
                    }
                    frmProdutoPesquisaPreco.Dispose();
                }
                else if ((e.KeyCode == Keys.F3) && (codProdutoComboBox.Focused))
                {
                    Telas.FrmProduto frmProduto = new Telas.FrmProduto();
                    frmProduto.ShowDialog();
                    if (frmProduto.ProdutoPesquisa != null)
                    {
                        produtoBindingSource.DataSource = GerenciadorProduto.GetInstance().ObterTodos();
                        produtoBindingSource.Position = produtoBindingSource.List.IndexOf(frmProduto.ProdutoPesquisa);
                    }
                    frmProduto.Dispose();
                }

            }
            // Coloca o foco na grid caso ela não possua
            if (e.KeyCode == Keys.F12)
            {
                btnCancelar_Click(sender, e);
                tb_entrada_produtoDataGridView.Focus();
                tb_entrada_produtoDataGridView.Select();
                if (tb_entrada_produtoDataGridView.RowCount == 1)
                {
                    tb_entrada_produtoDataGridView.SelectAll();
                }
            }

            // permite excluir um contato quando o foco está na grid
            if ((e.KeyCode == Keys.Delete) && (tb_entrada_produtoDataGridView.Focused == true))
            {
                excluirProduto(sender, e);
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
            btnProdutos.Enabled = habilita && (codEntradaTextBox.Text != "") && (long.Parse(codEntradaTextBox.Text) > 0);
            btnPagamentos.Enabled = habilita && (codEntradaTextBox.Text != "") && (long.Parse(codEntradaTextBox.Text) > 0);
            tb_entradaBindingNavigator.Enabled = habilita;
            if (habilita)
            {
                estado = EstadoFormulario.ESPERA;
            }
        }

        private void precoRevendaTextBox_Leave(object sender, EventArgs e)
        {
            codEntradaTextBox_Leave(sender, e);
            btnSalvar.Focus();
        }

    }
}