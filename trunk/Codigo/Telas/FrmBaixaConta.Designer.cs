﻿namespace SACE.Telas
{
    partial class FrmBaixaConta
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label valorLabel;
            System.Windows.Forms.Label dataPgtoLabel;
            this.baixaButton = new System.Windows.Forms.Button();
            this.pesquisaPanel = new System.Windows.Forms.Panel();
            this.txtTexto = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBusca = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.contasDataGridView = new System.Windows.Forms.DataGridView();
            this.baixaPanel = new System.Windows.Forms.Panel();
            this.codPessoaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codPlanoContaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codSaidaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.documentoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codEntradaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codContaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataVencimentoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.situacaoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.observacaoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tipoContaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tb_contasBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.saceDataSet1 = new SACE.Dados.saceDataSet();
            this.tb_contaTableAdapter = new SACE.Dados.saceDataSetTableAdapters.tb_contaTableAdapter();
            this.tableAdapterManager1 = new SACE.Dados.saceDataSetTableAdapters.TableAdapterManager();
            this.tb_baixa_contaTableAdapter = new SACE.Dados.saceDataSetTableAdapters.tb_baixa_contaTableAdapter();
            this.valorMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.dataDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.tb_baixaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            valorLabel = new System.Windows.Forms.Label();
            dataPgtoLabel = new System.Windows.Forms.Label();
            this.pesquisaPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contasDataGridView)).BeginInit();
            this.baixaPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_contasBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.saceDataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_baixaBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // baixaButton
            // 
            this.baixaButton.Location = new System.Drawing.Point(394, 235);
            this.baixaButton.Name = "baixaButton";
            this.baixaButton.Size = new System.Drawing.Size(75, 23);
            this.baixaButton.TabIndex = 1;
            this.baixaButton.Text = "Dar Baixa";
            this.baixaButton.UseVisualStyleBackColor = true;
            this.baixaButton.Click += new System.EventHandler(this.baixaButton_Click);
            // 
            // pesquisaPanel
            // 
            this.pesquisaPanel.Controls.Add(this.txtTexto);
            this.pesquisaPanel.Controls.Add(this.label2);
            this.pesquisaPanel.Controls.Add(this.cmbBusca);
            this.pesquisaPanel.Controls.Add(this.label1);
            this.pesquisaPanel.Controls.Add(this.contasDataGridView);
            this.pesquisaPanel.Location = new System.Drawing.Point(7, 12);
            this.pesquisaPanel.Name = "pesquisaPanel";
            this.pesquisaPanel.Size = new System.Drawing.Size(873, 217);
            this.pesquisaPanel.TabIndex = 9;
            // 
            // txtTexto
            // 
            this.txtTexto.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTexto.Location = new System.Drawing.Point(142, 23);
            this.txtTexto.Name = "txtTexto";
            this.txtTexto.Size = new System.Drawing.Size(308, 20);
            this.txtTexto.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(139, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Texto:";
            // 
            // cmbBusca
            // 
            this.cmbBusca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBusca.FormattingEnabled = true;
            this.cmbBusca.ImeMode = System.Windows.Forms.ImeMode.On;
            this.cmbBusca.Items.AddRange(new object[] {
            "Codigo Conta",
            "Codigo Pessoa",
            "Codigo Entrada",
            "Codigo Saída",
            "Data Vencimento",
            "Data Pagamento"});
            this.cmbBusca.Location = new System.Drawing.Point(6, 23);
            this.cmbBusca.Name = "cmbBusca";
            this.cmbBusca.Size = new System.Drawing.Size(121, 21);
            this.cmbBusca.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Buscar Por:";
            // 
            // contasDataGridView
            // 
            this.contasDataGridView.AllowUserToAddRows = false;
            this.contasDataGridView.AllowUserToDeleteRows = false;
            this.contasDataGridView.AutoGenerateColumns = false;
            this.contasDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.contasDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.codPessoaDataGridViewTextBoxColumn,
            this.codPlanoContaDataGridViewTextBoxColumn,
            this.codSaidaDataGridViewTextBoxColumn,
            this.documentoDataGridViewTextBoxColumn,
            this.codEntradaDataGridViewTextBoxColumn,
            this.codContaDataGridViewTextBoxColumn,
            this.dataVencimentoDataGridViewTextBoxColumn,
            this.valorDataGridViewTextBoxColumn,
            this.situacaoDataGridViewTextBoxColumn,
            this.observacaoDataGridViewTextBoxColumn,
            this.tipoContaDataGridViewTextBoxColumn});
            this.contasDataGridView.DataSource = this.tb_contasBindingSource;
            this.contasDataGridView.Location = new System.Drawing.Point(4, 56);
            this.contasDataGridView.Name = "contasDataGridView";
            this.contasDataGridView.ReadOnly = true;
            this.contasDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.contasDataGridView.Size = new System.Drawing.Size(864, 150);
            this.contasDataGridView.TabIndex = 9;
            // 
            // baixaPanel
            // 
            this.baixaPanel.Controls.Add(this.valorMaskedTextBox);
            this.baixaPanel.Controls.Add(valorLabel);
            this.baixaPanel.Controls.Add(dataPgtoLabel);
            this.baixaPanel.Controls.Add(this.dataDateTimePicker);
            this.baixaPanel.Location = new System.Drawing.Point(7, 262);
            this.baixaPanel.Name = "baixaPanel";
            this.baixaPanel.Size = new System.Drawing.Size(873, 144);
            this.baixaPanel.TabIndex = 10;
            // 
            // codPessoaDataGridViewTextBoxColumn
            // 
            this.codPessoaDataGridViewTextBoxColumn.DataPropertyName = "codPessoa";
            this.codPessoaDataGridViewTextBoxColumn.HeaderText = "codPessoa";
            this.codPessoaDataGridViewTextBoxColumn.Name = "codPessoaDataGridViewTextBoxColumn";
            this.codPessoaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // codPlanoContaDataGridViewTextBoxColumn
            // 
            this.codPlanoContaDataGridViewTextBoxColumn.DataPropertyName = "codPlanoConta";
            this.codPlanoContaDataGridViewTextBoxColumn.HeaderText = "codPlanoConta";
            this.codPlanoContaDataGridViewTextBoxColumn.Name = "codPlanoContaDataGridViewTextBoxColumn";
            this.codPlanoContaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // codSaidaDataGridViewTextBoxColumn
            // 
            this.codSaidaDataGridViewTextBoxColumn.DataPropertyName = "codSaida";
            this.codSaidaDataGridViewTextBoxColumn.HeaderText = "codSaida";
            this.codSaidaDataGridViewTextBoxColumn.Name = "codSaidaDataGridViewTextBoxColumn";
            this.codSaidaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // documentoDataGridViewTextBoxColumn
            // 
            this.documentoDataGridViewTextBoxColumn.DataPropertyName = "documento";
            this.documentoDataGridViewTextBoxColumn.HeaderText = "documento";
            this.documentoDataGridViewTextBoxColumn.Name = "documentoDataGridViewTextBoxColumn";
            this.documentoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // codEntradaDataGridViewTextBoxColumn
            // 
            this.codEntradaDataGridViewTextBoxColumn.DataPropertyName = "codEntrada";
            this.codEntradaDataGridViewTextBoxColumn.HeaderText = "codEntrada";
            this.codEntradaDataGridViewTextBoxColumn.Name = "codEntradaDataGridViewTextBoxColumn";
            this.codEntradaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // codContaDataGridViewTextBoxColumn
            // 
            this.codContaDataGridViewTextBoxColumn.DataPropertyName = "codConta";
            this.codContaDataGridViewTextBoxColumn.HeaderText = "codConta";
            this.codContaDataGridViewTextBoxColumn.Name = "codContaDataGridViewTextBoxColumn";
            this.codContaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dataVencimentoDataGridViewTextBoxColumn
            // 
            this.dataVencimentoDataGridViewTextBoxColumn.DataPropertyName = "dataVencimento";
            this.dataVencimentoDataGridViewTextBoxColumn.HeaderText = "dataVencimento";
            this.dataVencimentoDataGridViewTextBoxColumn.Name = "dataVencimentoDataGridViewTextBoxColumn";
            this.dataVencimentoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // valorDataGridViewTextBoxColumn
            // 
            this.valorDataGridViewTextBoxColumn.DataPropertyName = "valor";
            this.valorDataGridViewTextBoxColumn.HeaderText = "valor";
            this.valorDataGridViewTextBoxColumn.Name = "valorDataGridViewTextBoxColumn";
            this.valorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // situacaoDataGridViewTextBoxColumn
            // 
            this.situacaoDataGridViewTextBoxColumn.DataPropertyName = "situacao";
            this.situacaoDataGridViewTextBoxColumn.HeaderText = "situacao";
            this.situacaoDataGridViewTextBoxColumn.Name = "situacaoDataGridViewTextBoxColumn";
            this.situacaoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // observacaoDataGridViewTextBoxColumn
            // 
            this.observacaoDataGridViewTextBoxColumn.DataPropertyName = "observacao";
            this.observacaoDataGridViewTextBoxColumn.HeaderText = "observacao";
            this.observacaoDataGridViewTextBoxColumn.Name = "observacaoDataGridViewTextBoxColumn";
            this.observacaoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tipoContaDataGridViewTextBoxColumn
            // 
            this.tipoContaDataGridViewTextBoxColumn.DataPropertyName = "tipoConta";
            this.tipoContaDataGridViewTextBoxColumn.HeaderText = "tipoConta";
            this.tipoContaDataGridViewTextBoxColumn.Name = "tipoContaDataGridViewTextBoxColumn";
            this.tipoContaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tb_contasBindingSource
            // 
            this.tb_contasBindingSource.DataMember = "tb_conta";
            this.tb_contasBindingSource.DataSource = this.saceDataSet1;
            // 
            // saceDataSet1
            // 
            this.saceDataSet1.DataSetName = "saceDataSet";
            this.saceDataSet1.Prefix = "SACE";
            this.saceDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tb_contaTableAdapter
            // 
            this.tb_contaTableAdapter.ClearBeforeFill = true;
            // 
            // tableAdapterManager1
            // 
            this.tableAdapterManager1.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager1.tb_baixa_contaTableAdapter = this.tb_baixa_contaTableAdapter;
            this.tableAdapterManager1.tb_bancoTableAdapter = null;
            this.tableAdapterManager1.tb_cartao_creditoTableAdapter = null;
            this.tableAdapterManager1.tb_cfopTableAdapter = null;
            this.tableAdapterManager1.tb_configuracao_sistemaTableAdapter = null;
            this.tableAdapterManager1.tb_conta_bancoTableAdapter = null;
            this.tableAdapterManager1.tb_contaTableAdapter = this.tb_contaTableAdapter;
            this.tableAdapterManager1.tb_contato_empresaTableAdapter = null;
            this.tableAdapterManager1.tb_entrada_produtoTableAdapter = null;
            this.tableAdapterManager1.tb_entradaTableAdapter = null;
            this.tableAdapterManager1.tb_forma_pagamentoTableAdapter = null;
            this.tableAdapterManager1.tb_funcionalidadeTableAdapter = null;
            this.tableAdapterManager1.tb_grupo_contaTableAdapter = null;
            this.tableAdapterManager1.tb_grupoTableAdapter = null;
            this.tableAdapterManager1.tb_lojaTableAdapter = null;
            this.tableAdapterManager1.tb_movimentacao_contaTableAdapter = null;
            this.tableAdapterManager1.tb_perfil_funcionalidadeTableAdapter = null;
            this.tableAdapterManager1.tb_perfilTableAdapter = null;
            this.tableAdapterManager1.tb_permissaoTableAdapter = null;
            this.tableAdapterManager1.tb_pessoaTableAdapter = null;
            this.tableAdapterManager1.tb_plano_contaTableAdapter = null;
            this.tableAdapterManager1.tb_produto_lojaTableAdapter = null;
            this.tableAdapterManager1.tb_produtoTableAdapter = null;
            this.tableAdapterManager1.tb_saida_produtoTableAdapter = null;
            this.tableAdapterManager1.tb_saidaTableAdapter = null;
            this.tableAdapterManager1.tb_tipo_movimentacao_contaTableAdapter = null;
            this.tableAdapterManager1.tb_usuarioTableAdapter = null;
            this.tableAdapterManager1.UpdateOrder = SACE.Dados.saceDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // tb_baixa_contaTableAdapter
            // 
            this.tb_baixa_contaTableAdapter.ClearBeforeFill = true;
            // 
            // valorMaskedTextBox
            // 
            this.valorMaskedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tb_contasBindingSource, "valor", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "0", "N2"));
            this.valorMaskedTextBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.valorMaskedTextBox.Location = new System.Drawing.Point(441, 71);
            this.valorMaskedTextBox.Mask = "0000000000,00";
            this.valorMaskedTextBox.Name = "valorMaskedTextBox";
            this.valorMaskedTextBox.Size = new System.Drawing.Size(100, 20);
            this.valorMaskedTextBox.TabIndex = 58;
            // 
            // valorLabel
            // 
            valorLabel.AutoSize = true;
            valorLabel.Location = new System.Drawing.Point(438, 53);
            valorLabel.Name = "valorLabel";
            valorLabel.Size = new System.Drawing.Size(34, 13);
            valorLabel.TabIndex = 57;
            valorLabel.Text = "Valor:";
            // 
            // dataPgtoLabel
            // 
            dataPgtoLabel.AutoSize = true;
            dataPgtoLabel.Location = new System.Drawing.Point(331, 53);
            dataPgtoLabel.Name = "dataPgtoLabel";
            dataPgtoLabel.Size = new System.Drawing.Size(77, 13);
            dataPgtoLabel.TabIndex = 56;
            dataPgtoLabel.Text = "Data da Baixa:";
            // 
            // dataDateTimePicker
            // 
            this.dataDateTimePicker.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.tb_contasBindingSource, "dataVencimento", true));
            this.dataDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dataDateTimePicker.Location = new System.Drawing.Point(334, 72);
            this.dataDateTimePicker.Name = "dataDateTimePicker";
            this.dataDateTimePicker.Size = new System.Drawing.Size(100, 20);
            this.dataDateTimePicker.TabIndex = 55;
            // 
            // tb_baixaBindingSource
            // 
            this.tb_baixaBindingSource.DataMember = "tb_baixa_conta";
            this.tb_baixaBindingSource.DataSource = this.saceDataSet1;
            // 
            // FrmBaixaConta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 423);
            this.Controls.Add(this.baixaPanel);
            this.Controls.Add(this.pesquisaPanel);
            this.Controls.Add(this.baixaButton);
            this.Name = "FrmBaixaConta";
            this.Text = "Baixa de contas";
            this.Load += new System.EventHandler(this.FrmBaixaConta_Load);
            this.pesquisaPanel.ResumeLayout(false);
            this.pesquisaPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contasDataGridView)).EndInit();
            this.baixaPanel.ResumeLayout(false);
            this.baixaPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_contasBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.saceDataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_baixaBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource tb_contasBindingSource;
        private SACE.Dados.saceDataSetTableAdapters.tb_contaTableAdapter tb_contaTableAdapter;
        private SACE.Dados.saceDataSet saceDataSet1;
        private SACE.Dados.saceDataSetTableAdapters.TableAdapterManager tableAdapterManager1;
        private SACE.Dados.saceDataSetTableAdapters.tb_baixa_contaTableAdapter tb_baixa_contaTableAdapter;
        private System.Windows.Forms.Button baixaButton;
        private System.Windows.Forms.Panel pesquisaPanel;
        private System.Windows.Forms.TextBox txtTexto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBusca;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView contasDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn codPessoaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn codPlanoContaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn codSaidaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn documentoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn codEntradaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn codContaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataVencimentoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn situacaoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn observacaoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tipoContaDataGridViewTextBoxColumn;
        private System.Windows.Forms.Panel baixaPanel;
        private System.Windows.Forms.MaskedTextBox valorMaskedTextBox;
        private System.Windows.Forms.DateTimePicker dataDateTimePicker;
        private System.Windows.Forms.BindingSource tb_baixaBindingSource;
    }
}