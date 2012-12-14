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
using MySql.Data;

namespace Negocio
{
    public class GerenciadorBanco
    {

        private static GerenciadorBanco gBanco;
        private static RepositorioGenerico<BancoE, SaceEntities> repBanco;

        public static GerenciadorBanco GetInstace()
        {
            if (gBanco == null)
            {
                gBanco = new GerenciadorBanco();
                repBanco = new RepositorioGenerico<BancoE, SaceEntities>("chave");
            }
            return gBanco;
        }

        /// <summary>
        /// Insere dados do banco
        /// </summary>
        /// <param name="banco"></param>
        /// <returns></returns>
        public Int64 inserir(Banco banco)
        {
            try
            {
                BancoE _bancoE = new BancoE();
                _bancoE.codBanco = banco.CodBanco;
                _bancoE.nome = banco.Nome;

                repBanco.Inserir(_bancoE);
                repBanco.SaveChanges();

                return _bancoE.codBanco;
            }
            catch (Exception e)
            {
                throw new DadosException("Banco", e.Message, e);
            }
        }

        /// <summary>
        /// Atualiza dados do banco
        /// </summary>
        /// <param name="banco"></param>
        public void atualizar(Banco banco)
        {
            try
            {
                BancoE _bancoE = repBanco.ObterEntidade(b => b.codBanco == banco.CodBanco);
                _bancoE.nome = banco.Nome;

                repBanco.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DadosException("Banco", e.Message, e);
            }
        }

        /// <summary>
        /// Remove dados do banco
        /// </summary>
        /// <param name="codBanco"></param>
        public void remover(Int32 codBanco)
        {
            if (codBanco == 1)
                throw new NegocioException("O banco não pode ser removido.");
            try
            {
                repBanco.Remover(b => b.codBanco == codBanco);
                repBanco.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DadosException("Banco", e.Message, e);
            }
        }

        private IQueryable<Banco> GetQuery()
        {
            var saceEntities = (SaceEntities)repBanco.ObterContexto();
            var query = from banco in saceEntities.BancoSet
                        select new Banco
                        {
                            CodBanco = banco.codBanco,
                            Nome = banco.nome
                        };
            return query;
        }

        /// <summary>
        /// Obtém todos os banco cadastrados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Banco> ObterTodos()
        {
            return GetQuery().ToList();
        }

        /// <summary>
        /// Obtém banco com o código especificiado
        /// </summary>
        /// <param name="codBanco"></param>
        /// <returns></returns>
        public IEnumerable<Banco> Obter(int codBanco)
        {
            return GetQuery().Where(banco => banco.CodBanco == codBanco).ToList();
        }

        /// <summary>
        /// Obtém bancos que iniciam com o nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public IEnumerable<Banco> ObterPorNome(string nome)
        {
            return GetQuery().Where(banco => banco.Nome.StartsWith(nome)).ToList();
        }
    }
}