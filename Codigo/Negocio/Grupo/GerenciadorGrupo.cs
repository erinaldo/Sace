﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dominio;
using Dados.saceDataSetTableAdapters;
using Dados;
using Util;
using System.Data.Common;

namespace Negocio
{
    public class GerenciadorGrupo 
    {
        private static GerenciadorGrupo gGrupo;
        private static tb_grupoTableAdapter tb_grupoTA;
        
        
        public static GerenciadorGrupo getInstace()
        {
            if (gGrupo == null)
            {
                gGrupo = new GerenciadorGrupo();
                tb_grupoTA = new tb_grupoTableAdapter();
            }
            return gGrupo;
        }

        public Int64 inserir(Grupo grupo)
        {
            try
            {
                tb_grupoTA.Insert(grupo.Descricao);
                Subgrupo subgrupo = new Subgrupo();
                subgrupo.CodGrupo = Convert.ToInt32(tb_grupoTA.GetMaxCodGrupo());  
                subgrupo.Descricao = "---- NAO DEFINIDO ----";
                GerenciadorSubgrupo.getInstace().inserir(subgrupo);
                return subgrupo.CodGrupo;
            }
            catch (Exception e)
            {
                throw new DadosException("Grupo", e.Message, e);
            }
        }

        public void atualizar(Grupo grupo)
        {
            try
            {
                tb_grupoTA.Update(grupo.Descricao, grupo.CodGrupo);
            }
            catch (Exception e)
            {
                throw new DadosException("Grupo", e.Message, e);
            }
        }

        public void remover(Int32 codGrupo)
        {
            if (codGrupo == 1)
                throw new NegocioException("Esse grupo não pode ser excluído para manter a consistência da base de dados");
                 
            
            try
            {
                tb_grupoTA.Delete(codGrupo);
            }
            catch (Exception e)
            {
                throw new DadosException("Grupo", e.Message, e);
            }
        }
    }
}