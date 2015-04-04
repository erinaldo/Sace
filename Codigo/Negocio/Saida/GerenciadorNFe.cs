﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dados;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Dominio;
using Util;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Data.Objects.DataClasses;
using System.Xml.Linq;

namespace Negocio
{
    public class GerenciadorNFe
    {
        private static GerenciadorNFe gNFe;
        private static string nomeComputador;
        //private static Loja loja;


        public static GerenciadorNFe GetInstance()
        {
            if (gNFe == null)
            {
                gNFe = new GerenciadorNFe();
            }

            return gNFe;
        }

        private GerenciadorNFe()
        {
            nomeComputador = System.Windows.Forms.SystemInformation.ComputerName;
            //loja = GerenciadorLoja.GetInstance().ObterPorServidorNfe(nomeComputador).ElementAtOrDefault(0);
            //pastaNfeRetorno = loja != null ? loja.PastaNfeRetorno : "";
        }

        /// <summary>
        /// Insere os dados de uma conta bancária
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        public int Inserir(NfeControle nfe, Saida saida, bool ehNfeComplementar)
        {
            try
            {
                var repNfe = new RepositorioGenerico<tb_nfe>();
                var repSaida = new RepositorioGenerico<tb_saida>(repNfe.ObterContexto());

                tb_nfe _nfeE = new tb_nfe();
                Atribuir(nfe, _nfeE);


                IEnumerable<NfeControle> nfeControles = ObterPorSaida(saida.CodSaida);

                if (!ehNfeComplementar && nfeControles.Where(nfeC => nfeC.SituacaoNfe.Equals(NfeControle.SITUACAO_AUTORIZADA)).Count() > 0)
                {
                    throw new NegocioException("Uma NF-e já foi AUTORIZADA para esse pedido.");
                }
                if ((ehNfeComplementar) && nfeControles.Where(nfeC => nfeC.SituacaoNfe.Equals(NfeControle.SITUACAO_AUTORIZADA)).Count() == 0)
                {
                    throw new NegocioException("Uma NF-e Complementar só pode ser emitida quando existe uma NF-e enviada e Autorizada.");
                }

                repNfe.Inserir(_nfeE);
                

                IEnumerable<tb_saida> saidas;
                if (string.IsNullOrEmpty(saida.CupomFiscal))
                {
                     saidas = repSaida.Obter(s => s.codSaida == saida.CodSaida);
                }
                else
                {
                    saidas = repSaida.Obter(s => s.pedidoGerado == saida.CupomFiscal);
                }
                foreach (tb_saida _saidaE in saidas)
                {
                    _nfeE.tb_saida.Add(_saidaE);
                }
                
                repNfe.SaveChanges();

                return _nfeE.codNFe;
            }
            catch (Exception e)
            {
                throw new DadosException("Nota Fiscal Eletrônica", e.Message, e);
            }
        }

        /// <summary>
        /// Atualiza os dados de uma Nfe
        /// </summary>
        /// <param name="contaBanco"></param>
        public void Atualizar(NfeControle nfe)
        {
            try
            {
                var repNfe = new RepositorioGenerico<tb_nfe>();

                tb_nfe _nfe = repNfe.ObterEntidade(n => n.codNFe == nfe.CodNfe);
                Atribuir(nfe, _nfe);

                repNfe.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DadosException("Nota Fiscal Eletrônica", e.Message, e);
            }
        }

        /// <summary>
        /// Remove os dados de uma conta bancária
        /// </summary>
        /// <param name="codcontaBanco"></param>
        public void Remover(Int32 codNfe)
        {
            try
            {
                var repNfe = new RepositorioGenerico<tb_nfe>();

                repNfe.Remover(n => n.codNFe == codNfe);
                repNfe.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DadosException("Nota Fiscal Eletrônica", e.Message, e);
            }
        }

        /// <summary>
        /// Query Geral para obter dados das nfes
        /// </summary>
        /// <returns></returns>
        private IQueryable<NfeControle> GetQuery()
        {
            var repNfe = new RepositorioGenerico<tb_nfe>();

            var saceEntities = (SaceEntities)repNfe.ObterContexto();
            var query = from nfe in saceEntities.tb_nfe
                        select new NfeControle
                        {
                            Chave = nfe.chave,
                            CodNfe = nfe.codNFe,
                            JustificativaCancelamento = nfe.justificativaCancelamento,
                            MensagemSituacaoReciboEnvio = nfe.mensagemSituacaoReciboEnvio,
                            MensagemSitucaoProtocoloCancelamento = nfe.mensagemSituacaoProtocoloCancelamento,
                            MensagemSitucaoProtocoloUso = nfe.mensagemSituacaoProtocoloUso,
                            NumeroLoteEnvio = nfe.numeroLoteEnvio,
                            NumeroProtocoloCancelamento = nfe.numeroProtocoloCancelamento,
                            NumeroProtocoloUso = nfe.numeroProtocoloUso,
                            NumeroRecibo = nfe.numeroRecibo,
                            SituacaoNfe = nfe.situacaoNfe,
                            SituacaoProtocoloCancelamento = nfe.situacaoProtocoloCancelamento,
                            SituacaoProtocoloUso = nfe.situacaoProtocoloUso,
                            SituacaoReciboEnvio = nfe.situacaoReciboEnvio,
                            DataEmissao = nfe.dataEmissao,
                            DataCancelamento = nfe.dataCancelamento,
                            Correcao = nfe.correcao,
                            DataCartaCorrecao = nfe.dataCartaCorrecao,
                            MensagemSitucaoCartaCorrecao = nfe.mensagemSitucaoCartaCorrecao,
                            NumeroProtocoloCartaCorrecao = nfe.numeroProtocoloCartaCorrecao,
                            SeqCartaCorrecao = nfe.seqCartaCorrecao,
                            SituacaoProtocoloCartaCorrecao = nfe.situacaoProtocoloCartaCorrecao,
                        };
            return query;
        }


        /// <summary>
        /// Obtém todos as nfe
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NfeControle> ObterTodos()
        {
            return GetQuery().ToList();
        }

        /// <summary>
        /// Obter Nfes por Chave
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NfeControle> ObterPorChave(string chave)
        {
            return GetQuery().Where(nc => nc.Chave.Equals(chave)).ToList();
        }

        /// <summary>
        /// Obter Nfes por Lote
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NfeControle> ObterPorLote(string numeroLote)
        {
            return GetQuery().Where(nc => nc.NumeroLoteEnvio.Equals(numeroLote)).ToList();
        }

        /// <summary>
        /// Obter Nfes por recibo
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NfeControle> ObterPorRecibo(string numeroRecibo)
        {
            return GetQuery().Where(nc => nc.NumeroRecibo.Equals(numeroRecibo)).ToList();
        }
        /// <summary>
        /// Obtém todos as nfe relacioanadas a uma saída
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NfeControle> ObterPorSaida(long codSaida)
        {
            var repSaida = new RepositorioGenerico<tb_saida>();

            

            var saceEntities = (SaceEntities)repSaida.ObterContexto();
            var querySaida = from saida in saceEntities.tb_saida
                             where saida.codSaida == codSaida
                             select saida;

            tb_saida _saidaE = querySaida.ToList().ElementAtOrDefault(0);
            
            var query = from nfe in _saidaE.tb_nfe
                        select new NfeControle
                        {
                            Chave = nfe.chave,
                            CodNfe = nfe.codNFe,
                            CodSaida = codSaida,
                            JustificativaCancelamento = nfe.justificativaCancelamento,
                            MensagemSituacaoReciboEnvio = nfe.mensagemSituacaoReciboEnvio,
                            MensagemSitucaoProtocoloCancelamento = nfe.mensagemSituacaoProtocoloCancelamento,
                            MensagemSitucaoProtocoloUso = nfe.mensagemSituacaoProtocoloUso,
                            NumeroLoteEnvio = nfe.numeroLoteEnvio,
                            NumeroProtocoloCancelamento = nfe.numeroProtocoloCancelamento,
                            NumeroProtocoloUso = nfe.numeroProtocoloUso,
                            NumeroRecibo = nfe.numeroRecibo,
                            SituacaoNfe = nfe.situacaoNfe,
                            SituacaoProtocoloCancelamento = nfe.situacaoProtocoloCancelamento,
                            SituacaoProtocoloUso = nfe.situacaoProtocoloUso,
                            SituacaoReciboEnvio = nfe.situacaoReciboEnvio,
                            DataEmissao = nfe.dataEmissao,
                            DataCancelamento = nfe.dataCancelamento
                        };
            return query.ToList();
        }

        /// <summary>
        /// Obtém Nfe com o código especificiado
        /// </summary>
        /// <param name="codBanco"></param>
        /// <returns></returns>
        public IEnumerable<NfeControle> Obter(int codNfe)
        {
            return GetQuery().Where(nfe=> nfe.CodNfe == codNfe).ToList();
        }

        public NfeControle GerarChaveNFE(Saida saida, bool ehNfeComplementar)
        {
            if (saida.CodCliente.Equals(Global.CLIENTE_PADRAO))
            {
                throw new NegocioException("Não existe cliente associado a esse pedido. É necessário associar um CLIENTE que esteja com todos os dados cadastrados.");
            }
            try
            {
                //Verifica se a saída já possui uma chave gerada e cuja nf-e não foi validada
                IEnumerable<NfeControle> nfeControles = ObterPorSaida(saida.CodSaida);

                if (!ehNfeComplementar && nfeControles.Where(nfeC => nfeC.SituacaoNfe.Equals(NfeControle.SITUACAO_AUTORIZADA)).Count() > 0)
                {
                    throw new NegocioException("Uma NF-e já foi AUTORIZADA para esse pedido.");
                }
                if ((ehNfeComplementar) && nfeControles.Where(nfeC => nfeC.SituacaoNfe.Equals(NfeControle.SITUACAO_AUTORIZADA)).Count() == 0)
                {
                    throw new NegocioException("Uma NF-e Complementar só pode ser emitida quando existe uma NF-e enviada e Autorizada.");
                }
 

                IEnumerable<NfeControle> nfeControlesTentativasFalhas = nfeControles.Where(nfeC => nfeC.SituacaoNfe.Equals(NfeControle.SITUACAO_NAO_VALIDADA) 
                    || nfeC.SituacaoNfe.Equals(NfeControle.SITUACAO_SOLICITADA));

                
                NfeControle nfeControle;
                // Verifica se houve já alguma tentativa de gerar a chave
                if (nfeControlesTentativasFalhas.Count() > 0)
                {
                    nfeControle = nfeControlesTentativasFalhas.ElementAtOrDefault(0);
                    if (string.IsNullOrEmpty(nfeControle.Chave))
                    {
                        nfeControle.DataEmissao = DateTime.Now;
                    }
                } 
                else
                {
                    nfeControle = new NfeControle();
                    nfeControle.SituacaoNfe = NfeControle.SITUACAO_NAO_VALIDADA;
                    nfeControle.DataEmissao = DateTime.Now;
                    nfeControle.Correcao = "";
                    nfeControle.MensagemSitucaoCartaCorrecao = "";
                    nfeControle.SituacaoProtocoloCartaCorrecao = "";
                    nfeControle.NumeroProtocoloCartaCorrecao = "";
                    nfeControle.CodNfe = GerenciadorNFe.GetInstance().Inserir(nfeControle, saida, ehNfeComplementar);
                }

                
                // Verifica se chave já foi gerada
                Loja lojaOrigem = GerenciadorLoja.GetInstance().Obter(saida.CodLojaOrigem).ElementAtOrDefault(0);
                RecuperarChaveGerada(saida, 1, nfeControle, lojaOrigem.PastaNfeRetorno);
                if (nfeControle.Chave.Equals(""))
                {
                    //define um documento XML e carrega o seu conteúdo 
                    XmlDocument xmldoc = new XmlDocument();
                    
                    //Cria um novo elemento poemas  e define os elementos autor, titulo e conteudo
                    XmlElement novoGerarChave = xmldoc.CreateElement("gerarChave");
                    XmlElement xmlnNF = xmldoc.CreateElement("nNF");
                    XmlElement xmlserie = xmldoc.CreateElement("serie");
                    XmlElement xmlAAMM = xmldoc.CreateElement("AAMM");
                    XmlElement xmlcnpj = xmldoc.CreateElement("CNPJ");
                    XmlElement xmlMod = xmldoc.CreateElement("mod");

                    xmlnNF.InnerText = nfeControle.CodNfe.ToString().PadLeft(8, '0');
                    xmlserie.InnerText = "1";
                    xmlAAMM.InnerText = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString().PadLeft(2, '0');
                    long codPessoaLoja = GerenciadorLoja.GetInstance().Obter(saida.CodLojaOrigem)[0].CodPessoa;
                    xmlcnpj.InnerText = GerenciadorPessoa.GetInstance().Obter(codPessoaLoja).ElementAt(0).CpfCnpj;
                    xmlMod.InnerText = "55"; //55-NF-e 65-NFC-e

                    //inclui os novos elementos no elemento poemas
                    novoGerarChave.AppendChild(xmlnNF);
                    novoGerarChave.AppendChild(xmlserie);
                    novoGerarChave.AppendChild(xmlMod);
                    novoGerarChave.AppendChild(xmlAAMM);
                    novoGerarChave.AppendChild(xmlcnpj);

                    //inclui o novo elemento no XML
                    xmldoc.AppendChild(novoGerarChave);

                    //Salva a inclusão no arquivo XML
                    xmldoc.Save(lojaOrigem.PastaNfeEnvio + saida.Nfe + "-gerar-chave.xml");

                    RecuperarChaveGerada(saida, 10, nfeControle, lojaOrigem.PastaNfeRetorno);
                }
                return nfeControle;
            }
            catch (NegocioException nex)
            {
                throw nex;
            }
            
        }

        /// <summary>
        /// Primeiro passo para emissão da nota fiscal é a emissão da chave.
        /// Enquanto a chave não for gerada não pode seguir os próximos passos. 
        /// </summary>
        /// <param name="saida"></param>
        /// <param name="numeroTentativasGerarChave"></param>
        /// <returns></returns>
        private void RecuperarChaveGerada(Saida saida, int numeroTentativasGerarChave, NfeControle nfeControle, string pastaNfeRetorno)
        {
            string chaveNFe = "";
            int tentativas = 0;

            while (chaveNFe.Equals("") && tentativas < numeroTentativasGerarChave)
            {
                
                DirectoryInfo Dir = new DirectoryInfo(pastaNfeRetorno);
                if (Dir.Exists)
                {
                    // Busca automaticamente todos os arquivos em todos os subdiretórios
                    string arquivoRetornoChave = "*-ret-gerar-chave.xml";
                    FileInfo[] files = Dir.GetFiles(arquivoRetornoChave, SearchOption.TopDirectoryOnly);
                    if (files.Length > 0)
                    {
                        XmlDocument xmldocRetorno = new XmlDocument();
                        xmldocRetorno.Load(pastaNfeRetorno + saida.Nfe + "-ret-gerar-chave.xml");
                        chaveNFe = xmldocRetorno.DocumentElement.InnerText;
                        files[0].Delete();
                    }
                    else
                    {
                        Thread.Sleep(200);
                    }
                }
                tentativas++;
            }

            if (tentativas > 1 && tentativas >= numeroTentativasGerarChave && string.IsNullOrEmpty(chaveNFe) )
                throw new NegocioException("Ocorreram problemas/lentidão na geração da chave da NF-e. Verifique se o certificado está conectado e a internet disponível. Favor tentar novamente em alguns minutos.");
            nfeControle.Chave = chaveNFe;
            nfeControle.DataEmissao = DateTime.Now;
            nfeControle.SituacaoNfe = NfeControle.SITUACAO_SOLICITADA;
            Atualizar(nfeControle);
        }

        /// <summary>
        /// Após o envio da Nf-e o Unidanfe gera um número de lote automático para
        /// controlar na aplicação.
        /// </summary>
        /// <param name="nfeControle"></param>
        /// <returns></returns>
        public string RecuperarLoteEnvio(Loja loja)
        {
              DirectoryInfo Dir = new DirectoryInfo(loja.PastaNfeRetorno);
              string numeroLote = "";
              if (Dir.Exists)
              {
                  // Busca automaticamente todos os arquivos em todos os subdiretórios
                  string arquivoRetornoLote = "*.*";
                  FileInfo[] files = Dir.GetFiles(arquivoRetornoLote, SearchOption.TopDirectoryOnly);
                  for(int i = 0; i < files.Length; i++)
                  {
                      if (files[i].Name.Length > 45)
                      {
                          string chave = files[i].Name.Substring(0, 44);
                          NfeControle nfeControle = ObterPorChave(chave).ElementAtOrDefault(0);

                          if (nfeControle != null)
                          {
                              if (files[i].Name.Contains("nfe.err"))
                              {
                                  files[i].CopyTo(loja.PastaNfeErro + files[i].Name, true);
                                  nfeControle.SituacaoNfe = NfeControle.SITUACAO_NAO_VALIDADA;
                                  files[i].Delete();
                              }
                              else if (files[i].Name.Contains("-num-lot."))
                              {
                                  XmlDocument xmldocRetorno = new XmlDocument();
                                  xmldocRetorno.Load(loja.PastaNfeRetorno + files[i].Name);

                                  numeroLote = xmldocRetorno.DocumentElement.InnerText;
                                  nfeControle.NumeroLoteEnvio = numeroLote.PadLeft(15, '0');
                                  files[i].Delete();
                              }
                              GerenciadorNFe.GetInstance().Atualizar(nfeControle);
                          }
                      }
                  }
              }
              return numeroLote;
        }

        /// <summary>
        /// Obtém o resultado do envio de um lote pela receita.
        /// </summary>
        /// <param name="nfeControle"></param>
        /// <returns></returns>
        public string RecuperarReciboEnvioNfe(string pastaNfeRetorno)
        {
            DirectoryInfo Dir = new DirectoryInfo(pastaNfeRetorno);
            string numeroRecibo = "";
            if (Dir.Exists)
            {
                // Busca automaticamente todos os arquivos em todos os subdiretórios
                string arquivoRetornoEnvioNfe = "*-rec.*";
                FileInfo[] files = Dir.GetFiles(arquivoRetornoEnvioNfe, SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    // apenas exclui o arquivo texto que não será utilizado  
                    if (files[i].Name.Contains("-rec.err"))
                    {
                        // não processa nesse método
                    } else if (files[i].Name.Contains("-pro-rec."))
                    {
                        // não processa nesse método
                    }
                    else if (files[i].Name.Contains("-rec.txt"))
                    {
                        files[i].Delete();
                    }
                    else
                    {
                        XmlDocument xmldocRetorno = new XmlDocument();
                        xmldocRetorno.Load(pastaNfeRetorno + files[i].Name);
                        XmlNodeReader xmlReaderRetorno = new XmlNodeReader(xmldocRetorno.DocumentElement);
                        
                        string numeroLote = files[i].Name.Substring(0, 15);
                        NfeControle nfeControle = ObterPorLote(numeroLote).LastOrDefault();

                        if (nfeControle != null)
                        {
                            //MemoryStream memStream = new MemoryStream(
                            XmlSerializer serializer = new XmlSerializer(typeof(TRetEnviNFe));
                            TRetEnviNFe retornoEnvioNfe = (TRetEnviNFe)serializer.Deserialize(xmlReaderRetorno);
                            

                            if (retornoEnvioNfe.cStat.Equals(NfeStatusResposta.NFE103_LOTE_RECEBIDO_SUCESSO))
                            {
                                if (retornoEnvioNfe.Item != null)
                                {
                                    numeroRecibo = ((TRetEnviNFeInfRec)retornoEnvioNfe.Item).nRec;
                                    nfeControle.NumeroRecibo = numeroRecibo;
                                }
                            }
                            nfeControle.SituacaoReciboEnvio = retornoEnvioNfe.cStat;
                            nfeControle.MensagemSituacaoReciboEnvio = retornoEnvioNfe.xMotivo;
                            GerenciadorNFe.GetInstance().Atualizar(nfeControle);
                            files[0].Delete();
                        }
                    }
                }
            }
            return numeroRecibo;
        }

        public string RecuperarResultadoProcessamentoNfe(string pastaNfeRetorno)
        {
            DirectoryInfo Dir = new DirectoryInfo(pastaNfeRetorno);
            string numeroProtocolo = "";
            if (Dir.Exists)
            {
                // Busca automaticamente todos os arquivos em todos os subdiretórios
                string arquivoRetornoReciboNfe = "*-pro-rec.*";
                FileInfo[] files = Dir.GetFiles(arquivoRetornoReciboNfe, SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {


                    if (files[i].Name.Contains("-pro-rec.err"))
                    {
                        //TODO: faz nada
                    }
                    else if (files[i].Name.Contains("-pro-rec.txt"))
                    {
                        files[i].Delete(); // não precisa do arquivo texto
                    }
                    else
                    {
                        XmlDocument xmldocRetorno = new XmlDocument();
                        xmldocRetorno.Load(pastaNfeRetorno + files[i].Name);
                        XmlNodeReader xmlReaderRetorno = new XmlNodeReader(xmldocRetorno.DocumentElement);

                        string numeroRecibo = files[i].Name.Substring(0, 15);
                        NfeControle nfeControle = ObterPorRecibo(numeroRecibo).ElementAtOrDefault(0);
                        if (nfeControle != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(TRetConsReciNFe));
                            TRetConsReciNFe retornoConsReciboNfe = (TRetConsReciNFe)serializer.Deserialize(xmlReaderRetorno);
                            if (retornoConsReciboNfe.cStat.Equals(NfeStatusResposta.NFE104_LOTE_PROCESSADO))
                            {
                                TProtNFeInfProt protocoloNfe = retornoConsReciboNfe.protNFe[0].infProt;
                                if (protocoloNfe.chNFe.Equals(nfeControle.Chave))
                                {
                                    if (protocoloNfe.cStat.Equals(NfeStatusResposta.NFE100_AUTORIZADO_USO_NFE))
                                    {
                                        numeroProtocolo = protocoloNfe.nProt;
                                        nfeControle.NumeroProtocoloUso = protocoloNfe.nProt;
                                        nfeControle.SituacaoNfe = NfeControle.SITUACAO_AUTORIZADA;
                                    }
                                    else if (protocoloNfe.cStat.Equals(NfeStatusResposta.NFE110_USO_DENEGADO))
                                    {
                                        nfeControle.SituacaoNfe = NfeControle.SITUACAO_DENEGADA;
                                    }
                                    else
                                    {
                                        nfeControle.SituacaoNfe = NfeControle.SITUACAO_REJEITADA;
                                    }
                                    nfeControle.SituacaoProtocoloUso = protocoloNfe.cStat;
                                    nfeControle.MensagemSitucaoProtocoloUso = protocoloNfe.xMotivo;

                                }
                            }
                            nfeControle.SituacaoReciboEnvio = retornoConsReciboNfe.cStat;
                            nfeControle.MensagemSituacaoReciboEnvio = retornoConsReciboNfe.xMotivo;
                            GerenciadorNFe.GetInstance().Atualizar(nfeControle);
                            files[0].Delete();
                        }
                    }
                }
            }
            return numeroProtocolo;
        }

        public TNfeProc LerNFE(string arquivo)
        {
            XmlDocument xmldocRetorno = new XmlDocument();
            xmldocRetorno.Load(arquivo);
            XmlNodeReader xmlReaderRetorno = new XmlNodeReader(xmldocRetorno.DocumentElement);
            XmlSerializer serializer = new XmlSerializer(typeof(TNfeProc));
            TNfeProc nfe = (TNfeProc)serializer.Deserialize(xmlReaderRetorno);
            
            return nfe;
        }
       
        public void EnviarNFE(Saida saida, NfeControle nfeControle, bool ehEspelho, bool ehNfeComplementar)
        {
            try
            {
                nfeControle = Obter(nfeControle.CodNfe).ElementAtOrDefault(0);
                // utilizado como padrão quando não especificado pelos produtos
                string cfopPadrao = GerenciadorSaida.GetInstance(null).ObterCfopTipoSaida(saida.TipoSaida).ToString();
                
                //string FORMATO_DATA = "yyyy-MM-dd";
                string FORMATO_DATA_HORA = "yyyy-MM-ddTHH:mm:sszzz";
                TNFe nfe = new TNFe();
                
                //Informacoes NFe
                TNFeInfNFe infNFe = new TNFeInfNFe();
                infNFe.versao = "3.10";
                infNFe.Id = "NFe" + nfeControle.Chave;
                nfe.infNFe = infNFe;
         

                //Ide
                TNFeInfNFeIde infNFeIde = new TNFeInfNFeIde();
                infNFeIde.cNF = nfeControle.Chave.Substring(35, 8); // código composto por 8 dígitos sequenciais
                infNFeIde.cDV = nfeControle.Chave.Substring(43, 1);
                
                Loja loja = GerenciadorLoja.GetInstance().Obter(saida.CodLojaOrigem).ElementAtOrDefault(0);
                infNFeIde.cMunFG = loja.CodMunicipioIBGE.ToString();
                infNFeIde.cUF = (TCodUfIBGE)Enum.Parse(typeof(TCodUfIBGE), "Item" + loja.CodMunicipioIBGE.ToString().Substring(0, 2));
                infNFeIde.mod = TMod.Item55;
                infNFeIde.dhEmi = ((DateTime)nfeControle.DataEmissao).ToString(FORMATO_DATA_HORA);

                //infNFeIde.dSaiEnt = saida.CupomFiscal.Equals("") ? saida.DataSaida.ToString(FORMATO_DATA) : saida.DataEmissaoCupomFiscal.ToString(FORMATO_DATA);
                infNFeIde.dhSaiEnt = ((DateTime)nfeControle.DataEmissao).ToString(FORMATO_DATA_HORA);
                if (ehNfeComplementar)
                    infNFeIde.finNFe = TFinNFe.Item2; //1 - Normal / 2 NF-e complementar / 3 - Nf-e Ajuste / 4 - devolução
                else
                    infNFeIde.finNFe = TFinNFe.Item1; 
                infNFeIde.indPag = (TNFeInfNFeIdeIndPag)0; // 0 - à Vista  1 - a prazo  2 - outros
                infNFeIde.natOp = GerenciadorCfop.GetInstance().Obter(Convert.ToInt32(cfopPadrao)).ElementAtOrDefault(0).Descricao;
                infNFeIde.nNF = nfeControle.CodNfe.ToString(); // número do Documento Fiscal
                infNFeIde.procEmi = TProcEmi.Item0; //0 - Emissão do aplicativo do contribuinte
                infNFeIde.serie = "1";
                infNFeIde.tpAmb = (TAmb)Enum.Parse(typeof(TAmb), "Item" + Global.AMBIENTE_NFE); // 1-produção / 2-homologação
                infNFeIde.tpEmis = TNFeInfNFeIdeTpEmis.Item1; // emissão Normal
                infNFeIde.tpImp = TNFeInfNFeIdeTpImp.Item1; // 1-Retratro / 2-Paisagem
                infNFeIde.tpNF = TNFeInfNFeIdeTpNF.Item1; // 0 - entrada / 1 - saída de produtos
                infNFeIde.verProc = "SACE 3.0"; //versão do aplicativo de emissão de nf-e   
                // Versão 3.1 da nf-e
                infNFeIde.idDest = TNFeInfNFeIdeIdDest.Item1; //1- interna; 2-interestadual; 3-exterior
                infNFeIde.indFinal = TNFeInfNFeIdeIndFinal.Item1; // 0 - normal; 1-consumidor final
                if (saida.TipoSaida.Equals(Saida.TIPO_PRE_VENDA) || saida.TipoSaida.Equals(Saida.TIPO_VENDA))
                    infNFeIde.indPres = TNFeInfNFeIdeIndPres.Item1; //1- presencial; 2-internet; 3-teleatendimento; 4-nfc-e com entrega domicilio 
                else
                    infNFeIde.indPres = TNFeInfNFeIdeIndPres.Item0; // 0 - não se aplica; 

                if (saida.TipoSaida.Equals(Saida.TIPO_VENDA))
                {
                    TNFeInfNFeIdeNFrefRefECF refEcf = new TNFeInfNFeIdeNFrefRefECF();
                    refEcf.mod = TNFeInfNFeIdeNFrefRefECFMod.Item2D;
                    refEcf.nCOO = saida.CupomFiscal;
                    refEcf.nECF = saida.NumeroECF;
                    TNFeInfNFeIdeNFref nfRef = new TNFeInfNFeIdeNFref();
                    nfRef.ItemElementName = ItemChoiceType1.refECF;
                    nfRef.Item = refEcf;
                    infNFeIde.NFref = new TNFeInfNFeIdeNFref[1];
                    infNFeIde.NFref[0] = nfRef;
                }
                NfeControle nfeControleAutorizada = null;
                if (ehNfeComplementar)
                {
                    TNFeInfNFeIdeNFref refNfe = new TNFeInfNFeIdeNFref();
                    refNfe.ItemElementName = ItemChoiceType1.refNFe;
                    nfeControleAutorizada = GerenciadorNFe.GetInstance().ObterPorSaida(saida.CodSaida).Where(nfeC => nfeC.SituacaoNfe == NfeControle.SITUACAO_AUTORIZADA).ElementAtOrDefault(0);
                    if (nfeControleAutorizada == null)
                        throw new NegocioException("Não é possível emitir uma NF-e Complementar de imposto quando não houve Nf-e Autorizadas.");
                    else
                        refNfe.Item = nfeControleAutorizada.Chave;
                    infNFeIde.NFref = new TNFeInfNFeIdeNFref[1];
                    infNFeIde.NFref[0] = refNfe;
                }

                nfe.infNFe.ide = infNFeIde;

                ////Endereco Emitente
                TEnderEmi enderEmit = new TEnderEmi();

                Pessoa pessoaloja = GerenciadorPessoa.GetInstance().Obter(loja.CodPessoa).ElementAtOrDefault(0);
                enderEmit.CEP = pessoaloja.Cep;
                enderEmit.cMun = pessoaloja.CodMunicipioIBGE.ToString();
                enderEmit.cPais = TEnderEmiCPais.Item1058;
                enderEmit.fone = pessoaloja.Fone1;
                enderEmit.nro = pessoaloja.Numero;
                enderEmit.UF = (TUfEmi)Enum.Parse(typeof(TUfEmi), pessoaloja.Uf);
                enderEmit.xBairro = pessoaloja.Bairro;
                if (!string.IsNullOrEmpty(pessoaloja.Complemento))
                    enderEmit.xCpl = pessoaloja.Complemento;
                enderEmit.xLgr = pessoaloja.Endereco;
                enderEmit.xMun = Util.StringUtil.RemoverAcentos(pessoaloja.NomeMunicipioIBGE);
                enderEmit.xPais = TEnderEmiXPais.BRASIL;
               
                ////Emitente
                TNFeInfNFeEmit emit = new TNFeInfNFeEmit();
                emit.CRT = TNFeInfNFeEmitCRT.Item1;   // 1- Simples Nacional
                emit.IE = pessoaloja.Ie.Trim();
                emit.enderEmit = enderEmit;
                emit.xFant = pessoaloja.NomeFantasia.Trim();
                emit.xNome = pessoaloja.Nome.Trim();
                emit.Item = pessoaloja.CpfCnpj.Trim();
                
                nfe.infNFe.emit = emit;

                ////Endereco destinatario
                TEndereco enderDest = new TEndereco();
                Pessoa destinatario = GerenciadorPessoa.GetInstance().Obter(saida.CodCliente).ElementAtOrDefault(0);
                enderDest.CEP = destinatario.Cep.Trim();
                enderDest.cMun = destinatario.CodMunicipioIBGE.ToString();
                enderDest.cPais = Tpais.Item1058;
                enderDest.fone = destinatario.Fone1;
                enderDest.nro = destinatario.Numero.Trim();
                enderDest.UF = (TUf)Enum.Parse(typeof(TUf), destinatario.Uf);
                enderDest.xBairro = destinatario.Bairro.Trim();
                if (!string.IsNullOrEmpty(destinatario.Complemento))
                    enderDest.xCpl = destinatario.Complemento.Trim();
                enderDest.xLgr = destinatario.Endereco.Trim();
                enderDest.xMun = Util.StringUtil.RemoverAcentos(destinatario.NomeMunicipioIBGE);
                enderDest.xPais = "Brasil";
                
                ////Destinatario
                TNFeInfNFeDest dest = new TNFeInfNFeDest();
                if (Global.AMBIENTE_NFE.Equals("1")) //produção
                    dest.xNome = destinatario.Nome.Trim();
                else
                    dest.xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL";
                dest.Item = destinatario.CpfCnpj;
                dest.ItemElementName = ItemChoiceType3.CPF;
                if ((destinatario.CpfCnpj.Length > 11) && !destinatario.Ie.StartsWith("I"))
                {
                    dest.ItemElementName = ItemChoiceType3.CNPJ;
                    dest.IE = destinatario.Ie;
                    dest.indIEDest = TNFeInfNFeDestIndIEDest.Item1; // 1-Contribuinte ICMS
                }
                else if (destinatario.CpfCnpj.Length > 11)
                {
                    dest.ItemElementName = ItemChoiceType3.CNPJ;
                    dest.IE = "ISENTO";
                    dest.indIEDest = TNFeInfNFeDestIndIEDest.Item2; // 2-Contribuinte ISENTO
                }
                else
                {
                    dest.indIEDest = TNFeInfNFeDestIndIEDest.Item9; // 9-Não contribuinte, que pode ou não possui ie
                }
                
                nfe.infNFe.dest = dest;
                dest.enderDest = enderDest;

                //totais da nota
                List<TNFeInfNFeDet> listaNFeDet = new List<TNFeInfNFeDet>();
                decimal totalProdutos = 0;
                decimal totalAVista = 0;
                decimal totalTributos = 0;
                decimal valorTotalDesconto = 0;
                decimal valorTotalNota = 0;

                if (ehNfeComplementar)
                {
                    TNFeInfNFeDetProd prod = new TNFeInfNFeDetProd();
                    prod.cProd = "CFOP9999";
                    prod.xProd = "Nota Fiscal Complementar da NF-e " + nfeControleAutorizada.CodNfe + " de " + nfeControleAutorizada.DataEmissao;
                    prod.cEAN = "";
                    prod.cEANTrib = "";
                    prod.CFOP = TCfop.Item6411;
                    prod.NCM = "00";
                    prod.uCom = "UN";
                    prod.qCom = formataValorNFe(0);
                    prod.vUnCom = formataValorNFe(0);
                    prod.vProd = formataValorNFe(0);
                    prod.vUnTrib = formataValorNFe(0);
                    prod.vOutro = formataValorNFe(saida.OutrasDespesas);
                    //prod.vDesc = formataValorNFe(0);
                    prod.uTrib = "UN";
                    prod.qTrib = formataValorNFe(0);
                    prod.indTot = (TNFeInfNFeDetProdIndTot)0; // Valor = 1 deve entrar no valor total da nota
                    //prod.vFrete = formataValorNFe(0);
                    TNFeInfNFeDetImpostoICMS icms = new TNFeInfNFeDetImpostoICMS();
                    TNFeInfNFeDetImpostoICMSICMSSN102 icms102 = new TNFeInfNFeDetImpostoICMSICMSSN102();
                    icms102.CSOSN = TNFeInfNFeDetImpostoICMSICMSSN102CSOSN.Item400;
                    icms102.orig = Torig.Item0;

                    icms.Item = icms102;

                    TNFeInfNFeDetImposto imp = new TNFeInfNFeDetImposto();
                    imp.Items = new object[] { icms };

                    TNFeInfNFeDetImpostoPISPISOutr pisOutr = new TNFeInfNFeDetImpostoPISPISOutr();
                    pisOutr.CST = TNFeInfNFeDetImpostoPISPISOutrCST.Item99;
                    pisOutr.vPIS = formataValorNFe(0);
                    pisOutr.Items = new string[2];
                    pisOutr.Items[0] = formataValorNFe(0);
                    pisOutr.Items[1] = formataValorNFe(0);
                    pisOutr.ItemsElementName = new ItemsChoiceType1[2];
                    pisOutr.ItemsElementName[0] = ItemsChoiceType1.vBC;
                    pisOutr.ItemsElementName[1] = ItemsChoiceType1.pPIS;

                    TNFeInfNFeDetImpostoPIS pis = new TNFeInfNFeDetImpostoPIS();
                    pis.Item = pisOutr;
                    imp.PIS = pis;

                    TNFeInfNFeDetImpostoCOFINSCOFINSOutr cofinsOutr = new TNFeInfNFeDetImpostoCOFINSCOFINSOutr();
                    cofinsOutr.CST = TNFeInfNFeDetImpostoCOFINSCOFINSOutrCST.Item99;
                    cofinsOutr.vCOFINS = formataValorNFe(0);
                    cofinsOutr.Items = new string[2];
                    cofinsOutr.Items[0] = formataValorNFe(0);
                    cofinsOutr.Items[1] = formataValorNFe(0);
                    cofinsOutr.ItemsElementName = new ItemsChoiceType3[2];
                    cofinsOutr.ItemsElementName[0] = ItemsChoiceType3.vBC;
                    cofinsOutr.ItemsElementName[1] = ItemsChoiceType3.pCOFINS;

                    TNFeInfNFeDetImpostoCOFINS cofins = new TNFeInfNFeDetImpostoCOFINS();
                    cofins.Item = cofinsOutr;
                    imp.COFINS = cofins;

                    TNFeInfNFeDet nfeDet = new TNFeInfNFeDet();
                    nfeDet.imposto = imp;
                    nfeDet.prod = prod;
                    nfeDet.nItem = "1"; // número de itens da nota
                    listaNFeDet.Add(nfeDet);
                }
                else
                {
                    List<SaidaProduto> saidaProdutos;
                    if (saida.TipoSaida == Saida.TIPO_VENDA)
                    {
                        if (destinatario.ImprimirCF)
                            saidaProdutos = GerenciadorSaidaProduto.GetInstance(null).ObterPorPedido(saida.CupomFiscal);
                        else
                            saidaProdutos = GerenciadorSaidaProduto.GetInstance(null).ObterPorPedidoSemCST(saida.CupomFiscal, Cst.ST_OUTRAS);
                    }
                    else
                    {
                        if (destinatario.ImprimirCF)
                            saidaProdutos = GerenciadorSaidaProduto.GetInstance(null).ObterPorSaida(saida.CodSaida);
                        else
                            saidaProdutos = GerenciadorSaidaProduto.GetInstance(null).ObterPorSaidaSemCST(saida.CodSaida, Cst.ST_OUTRAS);
                    }
                    saidaProdutos = GerenciadorSaida.GetInstance(null).ExcluirProdutosDevolvidosMesmoPreco(saidaProdutos);

                    int nItem = 1; // número do item processado

                    //decimal descontoDevolucoes = saidaProdutos.Where(sp => sp.Quantidade < 0).Sum(sp => sp.Subtotal);

                    if (saida.TipoSaida == Saida.TIPO_VENDA)
                    {
                        totalProdutos = saidaProdutos.Where(sp => sp.Quantidade > 0).Sum(sp => sp.Subtotal);
                        List<SaidaPesquisa> listaSaidas = GerenciadorSaida.GetInstance(null).ObterPorPedido(saida.CupomFiscal);
                        //totalAVista = listaSaidas.Where(s => s.TotalAVista > 0).Sum(s => s.TotalAVista);
                        totalAVista = listaSaidas.Sum(s => s.TotalAVista);
                    }
                    else
                    {
                        totalProdutos = saidaProdutos.Where(sp => sp.Quantidade > 0).Sum(sp => sp.SubtotalAVista);
                        totalAVista = saidaProdutos.Where(sp => sp.Quantidade > 0).Sum(sp => sp.SubtotalAVista) - saida.Desconto;
                    }
                    valorTotalDesconto = totalProdutos - totalAVista;
                    if (valorTotalDesconto >= 0)
                        valorTotalNota = totalAVista + saida.ValorFrete + saida.OutrasDespesas;
                    else
                        valorTotalNota = totalProdutos; // acontece quando são vendidos produtos que não podem sair no CF
                    // calcula fator de desconto para ser calculado sobre cada produto da nota
                    decimal fatorDesconto = valorTotalDesconto / totalProdutos;
                    decimal fatorValorOutros = saida.OutrasDespesas / totalProdutos;

                    // Atualiza os produtos com os valores de impostos
                    saidaProdutos = GerenciadorImposto.GetInstance().CalcularValorImpostoProdutos(saidaProdutos);
                    totalTributos = saidaProdutos.Sum(sp => sp.ValorImposto);

                    // produtos da nota
                    foreach (SaidaProduto saidaProduto in saidaProdutos)
                    {
                        if (saidaProduto.Quantidade > 0)
                        {
                            TNFeInfNFeDetProd prod = new TNFeInfNFeDetProd();
                            prod.cProd = saidaProduto.CodProduto.ToString();
                            ProdutoPesquisa produto = GerenciadorProduto.GetInstance().Obter(saidaProduto.CodProduto).ElementAtOrDefault(0);

                            if (Validacoes.ValidarEAN13(produto.CodigoBarra))
                            {
                                prod.cEANTrib = produto.CodigoBarra;
                                prod.cEAN = produto.CodigoBarra;
                            }
                            else
                            {
                                prod.cEANTrib = "";
                                prod.cEAN = "";
                            }
                            prod.CFOP = (TCfop)Enum.Parse(typeof(TCfop), "Item" + saidaProduto.CodCfop);
                            if ((saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FORNECEDOR) || (saida.TipoSaida == Saida.TIPO_REMESSA_CONSERTO))
                                prod.xProd = produto.NomeProdutoFabricante.Trim();
                            else
                                prod.xProd = produto.Nome.Trim();
                            prod.NCM = produto.Ncmsh;
                            prod.uCom = produto.Unidade;
                            prod.qCom = formataValorNFe(saidaProduto.Quantidade);
                            if (saida.TipoSaida == Saida.TIPO_VENDA)
                            {
                                prod.vUnCom = formataValorNFe(saidaProduto.ValorVenda);
                                prod.vProd = formataValorNFe(saidaProduto.Subtotal);
                                prod.vUnTrib = formataValorNFe(saidaProduto.ValorVenda);
                            }
                            else
                            {   
                                prod.vUnCom = formataValorNFe(saidaProduto.ValorVendaAVista, 3);
                                prod.vProd = formataValorNFe(saidaProduto.SubtotalAVista);
                                prod.vUnTrib = formataValorNFe(saidaProduto.ValorVendaAVista, 3);
                            }
                            if (Math.Round(saidaProduto.Subtotal * fatorDesconto, 2) > 0)
                                prod.vDesc = formataValorNFe(saidaProduto.Subtotal * fatorDesconto);
                            prod.uTrib = produto.Unidade;
                            prod.qTrib = formataQtdNFe(saidaProduto.Quantidade);
                            if (fatorValorOutros > 0)
                                prod.vOutro = formataValorNFe(saidaProduto.SubtotalAVista * fatorValorOutros);
                            prod.indTot = (TNFeInfNFeDetProdIndTot)1; // Valor = 1 deve entrar no valor total da nota

                            if (saida.ValorFrete > 0)
                                prod.vFrete = formataValorNFe(saida.ValorFrete / saida.TotalAVista * saidaProduto.SubtotalAVista);
                            TNFeInfNFeDetImpostoICMS icms = new TNFeInfNFeDetImpostoICMS();

                            //if ((saida.TipoSaida == Saida.TIPO_PRE_VENDA) || (saida.TipoSaida == Saida.TIPO_VENDA) ||
                            //    (saida.TipoSaida == Saida.TIPO_REMESSA_DEPOSITO) || (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FORNECEDOR))
                            //{
                            TNFeInfNFeDetImpostoICMSICMSSN102 icms102 = new TNFeInfNFeDetImpostoICMSICMSSN102();
                            icms102.CSOSN = TNFeInfNFeDetImpostoICMSICMSSN102CSOSN.Item400;
                            icms102.orig = Torig.Item0;

                            icms.Item = icms102;
                            //}

                            TNFeInfNFeDetImposto imp = new TNFeInfNFeDetImposto();
                            imp.Items = new object[] { icms };

                           
                            TNFeInfNFeDetImpostoPISPISOutr pisOutr = new TNFeInfNFeDetImpostoPISPISOutr();
                            pisOutr.CST = TNFeInfNFeDetImpostoPISPISOutrCST.Item99;
                            pisOutr.vPIS = formataValorNFe(0);
                            pisOutr.Items = new string[2];
                            pisOutr.Items[0] = formataValorNFe(0);
                            pisOutr.Items[1] = formataValorNFe(0);
                            pisOutr.ItemsElementName = new ItemsChoiceType1[2];
                            pisOutr.ItemsElementName[0] = ItemsChoiceType1.vBC;
                            pisOutr.ItemsElementName[1] = ItemsChoiceType1.pPIS;
                            

                            TNFeInfNFeDetImpostoPIS pis = new TNFeInfNFeDetImpostoPIS();
                            pis.Item = pisOutr;
                            imp.PIS = pis;
                            //imp.vTotTrib = formataValorNFe(saidaProduto.ValorImposto);


                            TNFeInfNFeDetImpostoCOFINSCOFINSOutr cofinsOutr = new TNFeInfNFeDetImpostoCOFINSCOFINSOutr();
                            cofinsOutr.CST = TNFeInfNFeDetImpostoCOFINSCOFINSOutrCST.Item99;
                            cofinsOutr.vCOFINS = formataValorNFe(0);
                            cofinsOutr.Items = new string[2];
                            cofinsOutr.Items[0] = formataValorNFe(0);
                            cofinsOutr.Items[1] = formataValorNFe(0);
                            cofinsOutr.ItemsElementName = new ItemsChoiceType3[2];
                            cofinsOutr.ItemsElementName[0] = ItemsChoiceType3.vBC;
                            cofinsOutr.ItemsElementName[1] = ItemsChoiceType3.pCOFINS;


                            TNFeInfNFeDetImpostoCOFINS cofins = new TNFeInfNFeDetImpostoCOFINS();
                            cofins.Item = cofinsOutr;
                            imp.COFINS = cofins;
                            
                            TNFeInfNFeDet nfeDet = new TNFeInfNFeDet();
                            nfeDet.imposto = imp;
                            nfeDet.prod = prod;
                            //nfeDet.infAdProd = detalhe.informacoesAdicionais;
                            nfeDet.nItem = nItem.ToString();
                            nItem++; // número do item na nf-e

                            listaNFeDet.Add(nfeDet);
                        }
                    }
                }
                nfe.infNFe.det = listaNFeDet.ToArray();
                
                // Totalizadores de tributos
                
                TNFeInfNFeTotalICMSTot icmsTot = new TNFeInfNFeTotalICMSTot();
                icmsTot.vBC = formataValorNFe(0); // o valor da base de cálculo deve ser a dos produtos.
                icmsTot.vICMS = formataValorNFe(0);
                icmsTot.vBCST = formataValorNFe(0);
                icmsTot.vST = formataValorNFe(0);
                icmsTot.vProd = formataValorNFe(totalProdutos);
                icmsTot.vFrete = formataValorNFe(saida.ValorFrete);
                icmsTot.vSeg = formataValorNFe(saida.ValorSeguro);
                //icmsTot.vTotTrib = formataValorNFe(totalTributos);


                if (saida.TipoSaida == Saida.TIPO_DEVOLUCAO_FORNECEDOR)
                {
                    icmsTot.vDesc = formataValorNFe(saida.Desconto);
                }
                else if (valorTotalDesconto >= 0)
                {
                    icmsTot.vDesc = formataValorNFe(valorTotalDesconto);
                } 
                else 
                {
                    // desconto fica negativo quand tirar cf de um item que não deveria entrar
                    icmsTot.vDesc = formataValorNFe(0);
                }
                icmsTot.vII = formataValorNFe(0);
                icmsTot.vIPI = formataValorNFe(0);
                icmsTot.vPIS = formataValorNFe(0);
                icmsTot.vCOFINS = formataValorNFe(0);
                icmsTot.vICMSDeson = formataValorNFe(0);
                icmsTot.vOutro = formataValorNFe(saida.OutrasDespesas);
                if (ehNfeComplementar)
                    icmsTot.vNF = formataValorNFe(saida.OutrasDespesas);
                else 
                    icmsTot.vNF = formataValorNFe(valorTotalNota);
                

                //}
                TNFeInfNFeTotal total = new TNFeInfNFeTotal();
                total.ICMSTot = icmsTot;
                nfe.infNFe.total = total;

                TNFeInfNFeTranspTransporta transporta = new TNFeInfNFeTranspTransporta();
                TNFeInfNFeTransp transp = new TNFeInfNFeTransp();
                if ((saida.CodEmpresaFrete == saida.CodCliente) || (saida.CodEmpresaFrete == Global.CLIENTE_PADRAO) || ehNfeComplementar)
                {
                    transp.modFrete = TNFeInfNFeTranspModFrete.Item9; // 9-Sem frete
                }
                else
                {
                    transp.modFrete = TNFeInfNFeTranspModFrete.Item1; // 1-Por conta do destinatário
                    transp.transporta = new TNFeInfNFeTranspTransporta();
                    Pessoa transportadora = GerenciadorPessoa.GetInstance().Obter(saida.CodEmpresaFrete).ElementAtOrDefault(0);
                    transporta.IE = transportadora.Ie;
                    transporta.UF = (TUf)Enum.Parse(typeof(TUf), transportadora.Uf);
                    transporta.UFSpecified = true; 
                    transporta.xEnder = transportadora.Endereco;
                    transporta.xMun = Util.StringUtil.RemoverAcentos(transportadora.Cidade);
                    transporta.xNome = transportadora.Nome;
                    transp.vol = new TNFeInfNFeTranspVol[1];
                    TNFeInfNFeTranspVol volumes = new TNFeInfNFeTranspVol();
                    volumes.esp = saida.EspecieVolumes;
                    volumes.marca = saida.Marca;
                    volumes.nVol = formataValorNFe(saida.Numero);
                    volumes.pesoB = formataPesoNFe(saida.PesoBruto);
                    volumes.pesoL = formataPesoNFe(saida.PesoLiquido);
                    volumes.qVol = saida.QuantidadeVolumes.ToString("N0");
                    
                    transp.vol[0] = volumes;
                    transp.transporta = transporta;
                    TNFeInfNFeTranspRetTransp retTransp = new TNFeInfNFeTranspRetTransp();
                    retTransp.CFOP = TCfopTransp.Item6352;
                    retTransp.cMunFG = transportadora.CodMunicipioIBGE.ToString();
                    retTransp.vServ = formataValorNFe(saida.ValorFrete);
                    retTransp.vBCRet = "0";
                    retTransp.pICMSRet = "0";
                    retTransp.vICMSRet = "0";
                    
                    transp.retTransp = retTransp; 
                    
                }

                nfe.infNFe.transp = transp;

                ////Informacoes Adicionais
                TNFeInfNFeInfAdic infAdic = new TNFeInfNFeInfAdic();
                //infAdic.infAdFisco = nfeSelected.informacoesAddFisco;
                decimal valorImpostoPercentual = totalTributos / totalAVista * 100;
                string mensagemTributos = "Valor Aproximado dos Tributos R$ " + totalTributos.ToString("N2") + " (" + valorImpostoPercentual.ToString("N2") + "%)  Fonte: IBPT. ";

                infAdic.infCpl += mensagemTributos;

                if (string.IsNullOrEmpty(saida.CupomFiscal))
                    infAdic.infCpl = Global.NFE_MENSAGEM_PADRAO + mensagemTributos + saida.Observacao;
                else
                    infAdic.infCpl = Global.NFE_MENSAGEM_PADRAO + mensagemTributos + saida.Observacao + ". ICMS RECOLHIDO NO";

                
                nfe.infNFe.infAdic = infAdic;

                MemoryStream memStream = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(TNFe));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                ns.Add("", "http://www.portalfiscal.inf.br/nfe");
                serializer.Serialize(memStream, nfe, ns);
                
                
                memStream.Position = 0;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(memStream);
                if (ehEspelho)
                {
                    xmlDoc.Save(loja.PastaNfeEspelho + nfeControle.Chave + "-nfe.xml");
                }
                else
                {
                    xmlDoc.Save(loja.PastaNfeEnvio + nfeControle.Chave + "-nfe.xml");
                    xmlDoc.Save(loja.PastaNfeEnviado + nfeControle.Chave + "-nfe.xml");
                    nfeControle.SituacaoNfe = NfeControle.SITUACAO_SOLICITADA;
                    Atualizar(nfeControle);
                }
            }
            catch (Exception ex)
            {
                throw new NegocioException("Problemas na geração do arquivo da Nota Fiscal Eletrônica. Favor consultar administrador do sistema.", ex);
            }
        }


        /// <summary>
        /// Envia solicitação de cancelamanto usando Eventos.
        /// </summary>
        /// <param name="nfeControle"></param>
        public void EnviarSolicitacaoCancelamento(NfeControle nfeControle)
        {

            try
            {
                if (string.IsNullOrEmpty(nfeControle.JustificativaCancelamento))
                {
                    throw new NegocioException("É necessário adicionar uma justificativa para realizar o cancelamento da NF-e.");
                }

                Dominio.NFE2.TEnvEvento envEvento = new Dominio.NFE2.TEnvEvento();
                envEvento.idLote = nfeControle.CodNfe.ToString().PadLeft(15, '0');
                envEvento.versao = "1.00";

                Dominio.NFE2.TEvento evento = new Dominio.NFE2.TEvento();
                evento.versao = Dominio.NFE2.TVerEvento.Item100;

                Dominio.NFE2.TEventoInfEvento infEvento = new Dominio.NFE2.TEventoInfEvento();
                infEvento.chNFe = nfeControle.Chave;
                infEvento.cOrgao = (Dominio.NFE2.TCOrgaoIBGE)Enum.Parse(typeof(Dominio.NFE2.TCOrgaoIBGE), "Item" + Global.C_ORGAO_IBGE_SERGIPE);
                infEvento.tpAmb = (Dominio.NFE2.TAmb)Enum.Parse(typeof(Dominio.NFE2.TAmb), "Item" + Global.AMBIENTE_NFE); // 1-produção / 2-homologação
                Saida saida = GerenciadorSaida.GetInstance(null).Obter(nfeControle.CodSaida);
                Loja loja = GerenciadorLoja.GetInstance().Obter(saida.CodLojaOrigem).ElementAtOrDefault(0);
                Pessoa pessoa = GerenciadorPessoa.GetInstance().Obter(loja.CodPessoa).ElementAtOrDefault(0);
                
                if (pessoa.Tipo.Equals(Pessoa.PESSOA_FISICA))
                    infEvento.ItemElementName = Dominio.NFE2.ItemChoiceType7.CPF;
                else
                    infEvento.ItemElementName = Dominio.NFE2.ItemChoiceType7.CNPJ;
                infEvento.Item = pessoa.CpfCnpj;
                infEvento.dhEvento = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                infEvento.tpEvento = "110111";
                infEvento.nSeqEvento = "1"; // carta correção pode haver mais de uma
                infEvento.verEvento = "1.00";
                infEvento.Id = "ID" + infEvento.tpEvento + infEvento.chNFe + infEvento.nSeqEvento.PadLeft(2, '0');

                Dominio.NFE2.TEventoInfEventoDetEvento detEvento = new Dominio.NFE2.TEventoInfEventoDetEvento();
                
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                ns.Add("", "http://www.portalfiscal.inf.br/nfe");
                
                XmlAttribute[] atributos = new XmlAttribute[1];
                atributos[0] = xmlDoc.CreateAttribute("versao");
                atributos[0].Value = "1.00";
                detEvento.AnyAttr = atributos;
                
                XmlElement[] elementos = new XmlElement[3];
                elementos[0] =xmlDoc.CreateElement("descEvento", "http://www.portalfiscal.inf.br/nfe");
                elementos[0].InnerText= "Cancelamento";
                elementos[1] = xmlDoc.CreateElement("nProt", "http://www.portalfiscal.inf.br/nfe");
                elementos[1].InnerText = nfeControle.NumeroProtocoloUso;
                elementos[2] = xmlDoc.CreateElement("xJust", "http://www.portalfiscal.inf.br/nfe");
                elementos[2].InnerText = nfeControle.JustificativaCancelamento;
                detEvento.Any = elementos;
                
                infEvento.detEvento = detEvento;
                evento.infEvento = infEvento;
                envEvento.evento = new Dominio.NFE2.TEvento[1] { evento };

                MemoryStream memStream = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.NFE2.TEnvEvento));
                serializer.Serialize(memStream, envEvento, ns);
                memStream.Position = 0;
                xmlDoc.Load(memStream);

                xmlDoc.Save(loja.PastaNfeEnvio + nfeControle.Chave + "-env-canc.xml");
                xmlDoc.Save(loja.PastaNfeEnviado + nfeControle.Chave + "-env-canc.xml");

                Atualizar(nfeControle);
            }
            catch (NegocioException ne)
            {
                throw ne;
            }
            catch (Exception ex)
            {
                throw new NegocioException("Problemas na geração do arquivo de cancelamento da Nota Fiscal Eletrônica. Favor consultar administrador do sistema.", ex);
            }
        }

        /// <summary>
        /// REtorna o resultado do pedido de cancelamento da NF-e
        /// </summary>
        /// <returns></returns>
        public string RecuperarResultadoCancelamentoNfe(string pastaNfeRetorno)
        {
            DirectoryInfo Dir = new DirectoryInfo(pastaNfeRetorno);
            string numeroProtocolo = "";
            if (Dir.Exists)
            {
                // Busca automaticamente todos os arquivos em todos os subdiretórios
                string arquivoRetornoReciboNfe = "*-ret-env-canc.*";
                FileInfo[] files = Dir.GetFiles(arquivoRetornoReciboNfe, SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    // apenas exclui o arquivo texto que não será utilizado  
                    if (files[i].Name.Contains("-ret-env-canc.txt"))
                    {
                        files[i].Delete();
                    }
                    else if (files[i].Name.Contains("-ret-env-canc.err"))
                    {
                        files[i].Delete();
                        string chave = files[i].Name.Substring(0, 44);
                        NfeControle nfeControle = ObterPorChave(chave).ElementAtOrDefault(0);
                        nfeControle.MensagemSitucaoProtocoloCancelamento = "Cancelamento NÃO REALIZADO";
                        Atualizar(nfeControle);
                    }
                    else
                    {
                        XmlDocument xmldocRetorno = new XmlDocument();
                        xmldocRetorno.Load(pastaNfeRetorno + files[i].Name);
                        XmlNodeReader xmlReaderRetorno = new XmlNodeReader(xmldocRetorno.DocumentElement);

                        string chave = files[i].Name.Substring(0, 44);
                        NfeControle nfeControle = ObterPorChave(chave).ElementAtOrDefault(0);
                        if (nfeControle != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.NFE2.TRetEnvEvento));
                            Dominio.NFE2.TRetEnvEvento retEventoCancelamento = (Dominio.NFE2.TRetEnvEvento)serializer.Deserialize(xmlReaderRetorno);
                            if (retEventoCancelamento.cStat.Equals(NfeStatusResposta.NFE128_LOTE_EVENTO_PROCESSADO))
                            {
                                Dominio.NFE2.TRetEventoInfEvento retornoEvento = retEventoCancelamento.retEvento[0].infEvento;
                                if (retornoEvento.cStat.Equals(NfeStatusResposta.NFE135_EVENTO_REGISTRADO_VINCULADO_NFE) ||
                                    retornoEvento.cStat.Equals(NfeStatusResposta.NFE136_EVENTO_REGISTRADO_NAO_VINCULADO_NFE))
                                {
                                    nfeControle.NumeroProtocoloCancelamento = retornoEvento.nProt;
                                    nfeControle.DataCancelamento = Convert.ToDateTime(retornoEvento.dhRegEvento);
                                    nfeControle.SituacaoNfe = NfeControle.SITUACAO_CANCELADA;
                                }
                                nfeControle.SituacaoProtocoloCancelamento = retornoEvento.cStat;
                                nfeControle.MensagemSitucaoProtocoloCancelamento = retornoEvento.xMotivo;
                                GerenciadorNFe.GetInstance().Atualizar(nfeControle);
                                files[i].Delete();    
                            }
                        }
                    }
                }
            }
            return numeroProtocolo;
        }

        /// <summary>
        /// Envia solicitação de inutilização de numeração de nf-e
        /// </summary>
        /// <param name="nfeControle"></param>
        public void EnviarSolicitacaoInutilizacao(NfeControle nfeControle)
        {

            try
            {
                if (string.IsNullOrEmpty(nfeControle.JustificativaCancelamento))
                {
                    throw new NegocioException("É necessário adicionar uma justificativa para realizar a inutilização da NF-e.");
                }

                TInutNFe inutilizacaoNfe = new TInutNFe();
                inutilizacaoNfe.versao = "2.00";
                Saida saida = GerenciadorSaida.GetInstance(null).Obter(nfeControle.CodSaida);
                Loja loja = GerenciadorLoja.GetInstance().Obter(saida.CodLojaOrigem).ElementAtOrDefault(0);
                Pessoa pessoa = GerenciadorPessoa.GetInstance().Obter(loja.CodPessoa).ElementAtOrDefault(0);

                TInutNFeInfInut infInutilizacaoNfe = new TInutNFeInfInut();
                infInutilizacaoNfe.ano = DateTime.Now.Year.ToString();
                infInutilizacaoNfe.CNPJ = pessoa.CpfCnpj;
                infInutilizacaoNfe.cUF = (TCodUfIBGE)Enum.Parse(typeof(TCodUfIBGE), "Item" + Global.C_ORGAO_IBGE_SERGIPE);
                infInutilizacaoNfe.mod = TMod.Item55;
                infInutilizacaoNfe.nNFFin = nfeControle.CodNfe.ToString();
                infInutilizacaoNfe.nNFIni = nfeControle.CodNfe.ToString();
                infInutilizacaoNfe.serie = "1";
                infInutilizacaoNfe.tpAmb = (TAmb)Enum.Parse(typeof(TAmb), "Item" + Global.AMBIENTE_NFE); // 1-produção / 2-homologação
                infInutilizacaoNfe.xJust = nfeControle.JustificativaCancelamento;
                infInutilizacaoNfe.xServ = TInutNFeInfInutXServ.INUTILIZAR;
                infInutilizacaoNfe.Id = "ID" + Global.C_ORGAO_IBGE_SERGIPE + infInutilizacaoNfe.ano.Substring(2, 2) +
                    infInutilizacaoNfe.CNPJ + "55" + "001" + infInutilizacaoNfe.nNFIni.PadLeft(9, '0') + infInutilizacaoNfe.nNFFin.PadLeft(9, '0');
                
                inutilizacaoNfe.infInut = infInutilizacaoNfe;
                
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                ns.Add("", "http://www.portalfiscal.inf.br/nfe");

                MemoryStream memStream = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(TInutNFe));
                serializer.Serialize(memStream, inutilizacaoNfe, ns);
                memStream.Position = 0;
                xmlDoc.Load(memStream);

                xmlDoc.Save(loja.PastaNfeEnvio + infInutilizacaoNfe.Id.Substring(2) + "-ped-inu.xml");
                xmlDoc.Save(loja.PastaNfeEnviado + infInutilizacaoNfe.Id.Substring(2) + "-ped-inu.xml");

                Atualizar(nfeControle);
            }
            catch (NegocioException ne)
            {
                throw ne;
            }
            catch (Exception ex)
            {
                throw new NegocioException("Problemas na geração do arquivo de cancelamento da Nota Fiscal Eletrônica. Favor consultar administrador do sistema.", ex);
            }
        }

        /// <summary>
        /// REtorna o resultado do pedido de cancelamento da NF-e
        /// </summary>
        /// <returns></returns>
        public string RecuperarResultadoInutilizacaoNfe(string pastaNfeRetorno)
        {
            DirectoryInfo Dir = new DirectoryInfo(pastaNfeRetorno);
            string numeroProtocolo = "";
            if (Dir.Exists)
            {
                // Busca automaticamente todos os arquivos em todos os subdiretórios
                string arquivoRetornoReciboNfe = "*-inu.*";
                FileInfo[] files = Dir.GetFiles(arquivoRetornoReciboNfe, SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    // apenas exclui o arquivo texto que não será utilizado  
                    if (files[i].Name.Contains("-inu.txt"))
                    {
                        files[i].Delete();
                    }
                    else if (files[i].Name.Contains("-inu.err"))
                    {
                        files[i].Delete();
                        string chave = files[i].Name.Substring(0, 44);
                        NfeControle nfeControle = ObterPorChave(chave).ElementAtOrDefault(0);
                        nfeControle.MensagemSitucaoProtocoloCancelamento = "Cancelamento NÃO REALIZADO";
                        Atualizar(nfeControle);
                    }
                    else
                    {
                        XmlDocument xmldocRetorno = new XmlDocument();
                        xmldocRetorno.Load(pastaNfeRetorno + files[i].Name);
                        XmlNodeReader xmlReaderRetorno = new XmlNodeReader(xmldocRetorno.DocumentElement);

                        string chave = files[i].Name.Substring(0, 44);
                        NfeControle nfeControle = ObterPorChave(chave).ElementAtOrDefault(0);
                        if (nfeControle != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.NFE2.TRetEnvEvento));
                            Dominio.NFE2.TRetEnvEvento retEventoCancelamento = (Dominio.NFE2.TRetEnvEvento)serializer.Deserialize(xmlReaderRetorno);
                            if (retEventoCancelamento.cStat.Equals(NfeStatusResposta.NFE128_LOTE_EVENTO_PROCESSADO))
                            {
                                Dominio.NFE2.TRetEventoInfEvento retornoEvento = retEventoCancelamento.retEvento[0].infEvento;
                                if (retornoEvento.cStat.Equals(NfeStatusResposta.NFE135_EVENTO_REGISTRADO_VINCULADO_NFE) ||
                                    retornoEvento.cStat.Equals(NfeStatusResposta.NFE136_EVENTO_REGISTRADO_NAO_VINCULADO_NFE))
                                {
                                    nfeControle.NumeroProtocoloCancelamento = retornoEvento.nProt;
                                    nfeControle.DataCancelamento = Convert.ToDateTime(retornoEvento.dhRegEvento);
                                    nfeControle.SituacaoNfe = NfeControle.SITUACAO_CANCELADA;
                                }
                                nfeControle.SituacaoProtocoloCancelamento = retornoEvento.cStat;
                                nfeControle.MensagemSitucaoProtocoloCancelamento = retornoEvento.xMotivo;
                                GerenciadorNFe.GetInstance().Atualizar(nfeControle);
                                files[i].Delete();
                            }
                        }
                    }
                }
            }
            return numeroProtocolo;
        }

        /// <summary>
        /// Envia solicitação de consulta a uma nf-e
        /// </summary>
        /// <param name="nfeControle"></param>
        public void EnviarConsultaNfe(NfeControle nfeControle)
        {

            try
            {
                TConsSitNFe consultaNfe = new TConsSitNFe();

                consultaNfe.chNFe = nfeControle.Chave;
                consultaNfe.tpAmb = (TAmb)Enum.Parse(typeof(TAmb), "Item" + Global.AMBIENTE_NFE); // 1-produção / 2-homologação
                consultaNfe.versao = TVerConsSitNFe.Item310;
                consultaNfe.xServ = TConsSitNFeXServ.CONSULTAR;
                
                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                ns.Add("", "http://www.portalfiscal.inf.br/nfe");

                MemoryStream memStream = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(TConsSitNFe));
                serializer.Serialize(memStream, consultaNfe, ns);
                memStream.Position = 0;
                xmlDoc.Load(memStream);

                Saida saida = GerenciadorSaida.GetInstance(null).Obter(nfeControle.CodSaida);
                Loja loja = GerenciadorLoja.GetInstance().Obter(saida.CodLojaOrigem).ElementAtOrDefault(0);
                xmlDoc.Save(loja.PastaNfeEnvio + nfeControle.Chave + "-ped-sit.xml");
                xmlDoc.Save(loja.PastaNfeEnviado + nfeControle.Chave + "-ped-sit.xml");
            }
            catch (NegocioException ne)
            {
                throw ne;
            }
            catch (Exception ex)
            {
                throw new NegocioException("Problemas na geração do arquivo de consulta da situação da Nota Fiscal Eletrônica. Favor consultar administrador do sistema.", ex);
            }
        }


        /// <summary>
        /// REtorna o resultado do pedido de cancelamento da NF-e
        /// </summary>
        /// <returns></returns>
        public void RecuperarResultadoConsultaNfe(string pastaNfeRetorno)
        {
            DirectoryInfo Dir = new DirectoryInfo(pastaNfeRetorno);
            if (Dir.Exists)
            {
                // Busca automaticamente todos os arquivos em todos os subdiretórios
                string arquivoRetornoConsultaNfe = "*-sit.*";
                FileInfo[] files = Dir.GetFiles(arquivoRetornoConsultaNfe, SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    // apenas exclui o arquivo texto que não será utilizado  
                    if (files[i].Name.Contains("-sit.txt"))
                    {
                        files[i].Delete();
                    }
                    else if (files[i].Name.Contains("-sit.err"))
                    {
                        files[i].Delete();
                    }
                    else
                    {
                        XmlDocument xmldocRetorno = new XmlDocument();
                        xmldocRetorno.Load(pastaNfeRetorno + files[i].Name);
                        XmlNodeReader xmlReaderRetorno = new XmlNodeReader(xmldocRetorno.DocumentElement);

                        string chave = files[i].Name.Substring(0, 44);
                        NfeControle nfeControle = ObterPorChave(chave).ElementAtOrDefault(0);
                        if (nfeControle != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(TRetConsSitNFe));
                            TRetConsSitNFe retConsulta = (TRetConsSitNFe)serializer.Deserialize(xmlReaderRetorno);

                            if (retConsulta.cStat.Equals(NfeStatusResposta.NFE217_NFE_INEXISTENTE))
                            {
                                nfeControle.SituacaoProtocoloUso = retConsulta.cStat;
                                nfeControle.MensagemSitucaoProtocoloUso = retConsulta.xMotivo;
                            }
                            else
                            {
                                TProtNFeInfProt protocoloNfe = retConsulta.protNFe.infProt;
                                if (protocoloNfe.chNFe.Equals(nfeControle.Chave))
                                {
                                    if (protocoloNfe.cStat.Equals(NfeStatusResposta.NFE100_AUTORIZADO_USO_NFE))
                                    {
                                        //numeroProtocolo = protocoloNfe.nProt;
                                        nfeControle.NumeroProtocoloUso = protocoloNfe.nProt;
                                        nfeControle.SituacaoNfe = NfeControle.SITUACAO_AUTORIZADA;
                                    }
                                    else if (protocoloNfe.cStat.Equals(NfeStatusResposta.NFE110_USO_DENEGADO))
                                    {
                                        nfeControle.SituacaoNfe = NfeControle.SITUACAO_DENEGADA;
                                    }
                                    else
                                    {
                                        nfeControle.SituacaoNfe = NfeControle.SITUACAO_REJEITADA;
                                    }
                                    nfeControle.SituacaoProtocoloUso = protocoloNfe.cStat;
                                    nfeControle.MensagemSitucaoProtocoloUso = protocoloNfe.xMotivo;
                                }
                            }
                            GerenciadorNFe.GetInstance().Atualizar(nfeControle);
                            files[0].Delete();
                                
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Envia solicitação de correção de uma nf-e
        /// </summary>
        /// <param name="nfeControle"></param>
        public void EnviarCartaCorrecaoNfe(NfeControle nfeControle)
        {
            try
            {
                if (nfeControle.SituacaoNfe != NfeControle.SITUACAO_AUTORIZADA)
                {
                    throw new NegocioException("Só é possível enviar cartas de correção de NF-e AUTORIZADAS.");
                }
                if (string.IsNullOrEmpty(nfeControle.Correcao))
                {
                    throw new NegocioException("É necessário adicionar uma texto de correção para enviar uma Carta de Correção da NF-e.");
                }


                Dominio.NFE2.TEnvEvento envEvento = new Dominio.NFE2.TEnvEvento();
                envEvento.idLote = nfeControle.CodNfe.ToString().PadLeft(15, '0');
                envEvento.versao = "1.00";


                Dominio.NFE2.TEvento evento = new Dominio.NFE2.TEvento();
                evento.versao = Dominio.NFE2.TVerEvento.Item100;

                Dominio.NFE2.TEventoInfEvento infEvento = new Dominio.NFE2.TEventoInfEvento();
                infEvento.chNFe = nfeControle.Chave;
                infEvento.cOrgao = (Dominio.NFE2.TCOrgaoIBGE)Enum.Parse(typeof(Dominio.NFE2.TCOrgaoIBGE), "Item" + Global.C_ORGAO_IBGE_SERGIPE);
                infEvento.tpAmb = (Dominio.NFE2.TAmb)Enum.Parse(typeof(Dominio.NFE2.TAmb), "Item" + Global.AMBIENTE_NFE); // 1-produção / 2-homologação
                
                Saida saida = GerenciadorSaida.GetInstance(null).Obter(nfeControle.CodSaida);
                Loja loja = GerenciadorLoja.GetInstance().Obter(saida.CodLojaOrigem).ElementAtOrDefault(0);
                Pessoa pessoa = GerenciadorPessoa.GetInstance().Obter(loja.CodPessoa).ElementAtOrDefault(0);

                if (pessoa.Tipo.Equals(Pessoa.PESSOA_FISICA))
                    infEvento.ItemElementName = Dominio.NFE2.ItemChoiceType7.CPF;
                else
                    infEvento.ItemElementName = Dominio.NFE2.ItemChoiceType7.CNPJ;
                infEvento.Item = pessoa.CpfCnpj;
                infEvento.chNFe = nfeControle.Chave;
                infEvento.dhEvento = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                infEvento.tpEvento = "110110";
                infEvento.nSeqEvento = (nfeControle.SeqCartaCorrecao + 1).ToString(); // carta correção pode haver mais de uma
                nfeControle.SeqCartaCorrecao = Convert.ToInt32(infEvento.nSeqEvento);
                infEvento.verEvento = "1.00";
                infEvento.Id = "ID" + infEvento.tpEvento + infEvento.chNFe + infEvento.nSeqEvento.PadLeft(2, '0');

                Dominio.NFE2.TEventoInfEventoDetEvento detEvento = new Dominio.NFE2.TEventoInfEventoDetEvento();

                XmlDocument xmlDoc = new XmlDocument();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

                ns.Add("", "http://www.portalfiscal.inf.br/nfe");

                XmlAttribute[] atributos = new XmlAttribute[1];
                atributos[0] = xmlDoc.CreateAttribute("versao");
                atributos[0].Value = "1.00";
                detEvento.AnyAttr = atributos;

                XmlElement[] elementos = new XmlElement[3];
                elementos[0] = xmlDoc.CreateElement("descEvento", "http://www.portalfiscal.inf.br/nfe");
                elementos[0].InnerText = "Carta de Correcao";
                elementos[1] = xmlDoc.CreateElement("xCorrecao", "http://www.portalfiscal.inf.br/nfe");
                elementos[1].InnerText = nfeControle.Correcao;
                elementos[2] = xmlDoc.CreateElement("xCondUso", "http://www.portalfiscal.inf.br/nfe");
                elementos[2].InnerText = ("A Carta de Correcao e disciplinada pelo paragrafo 1o-A do art. 7o do Convenio S/N, de 15 de dezembro de 1970 e pode ser utilizada para regularizacao de erro ocorrido na emissao de documento fiscal, desde que o erro nao esteja relacionado com: I - as variaveis que determinam o valor do imposto tais como: base de calculo, aliquota, diferenca de preco, quantidade, valor da operacao ou da prestacao; II - a correcao de dados cadastrais que implique mudanca do remetente ou do destinatario; III - a data de emissao ou de saida.");
                detEvento.Any = elementos;

                infEvento.detEvento = detEvento;
                evento.infEvento = infEvento;
                envEvento.evento = new Dominio.NFE2.TEvento[1] { evento };

                MemoryStream memStream = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.NFE2.TEnvEvento));
                serializer.Serialize(memStream, envEvento, ns);
                memStream.Position = 0;
                xmlDoc.Load(memStream);

                xmlDoc.Save(loja.PastaNfeEnvio + nfeControle.Chave + "-" + nfeControle.SeqCartaCorrecao.ToString().PadLeft(2, '0') + "-env-cce.xml");
                xmlDoc.Save(loja.PastaNfeEnviado + nfeControle.Chave + "-" + nfeControle.SeqCartaCorrecao.ToString().PadLeft(2, '0') + "-env-cce.xml");

                Atualizar(nfeControle);
            }
            catch (NegocioException ne)
            {
                throw ne;
            }
            catch (Exception ex)
            {
                throw new NegocioException("Problemas na geração do arquivo de cancelamento da Nota Fiscal Eletrônica. Favor consultar administrador do sistema.", ex);
            }
        }

        /// <summary>
        /// REtorna o resultado do pedido de cancelamento da NF-e
        /// </summary>
        /// <returns></returns>
        public string RecuperarResultadoCartaCorrecaoNfe(string pastaNfeRetorno)
        {
            DirectoryInfo Dir = new DirectoryInfo(pastaNfeRetorno);
            string numeroProtocolo = "";
            if (Dir.Exists)
            {
                // Busca automaticamente todos os arquivos em todos os subdiretórios
                string arquivoRetornoReciboNfe = "*-ret-env-cce.*";
                FileInfo[] files = Dir.GetFiles(arquivoRetornoReciboNfe, SearchOption.TopDirectoryOnly);
                for (int i = 0; i < files.Length; i++)
                {
                    // apenas exclui o arquivo texto que não será utilizado  
                    if (files[i].Name.Contains("-ret-env-cce.txt"))
                    {
                        files[i].Delete();
                    }
                    else if (files[i].Name.Contains("-ret-env-cce.err"))
                    {
                        files[i].Delete();
                        string chave = files[i].Name.Substring(0, 44);
                        NfeControle nfeControle = ObterPorChave(chave).ElementAtOrDefault(0);
                        nfeControle.MensagemSitucaoCartaCorrecao = "Carta de Correção com erros no layout.";
                        Atualizar(nfeControle);
                    }
                    else
                    {
                        XmlDocument xmldocRetorno = new XmlDocument();
                        xmldocRetorno.Load(pastaNfeRetorno + files[i].Name);
                        XmlNodeReader xmlReaderRetorno = new XmlNodeReader(xmldocRetorno.DocumentElement);

                        string chave = files[i].Name.Substring(0, 44);
                        NfeControle nfeControle = ObterPorChave(chave).ElementAtOrDefault(0);
                        if (nfeControle != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.NFE2.TRetEnvEvento));
                            Dominio.NFE2.TRetEnvEvento retEventoCartaCorrecao = (Dominio.NFE2.TRetEnvEvento)serializer.Deserialize(xmlReaderRetorno);
                            if (retEventoCartaCorrecao.cStat.Equals(NfeStatusResposta.NFE128_LOTE_EVENTO_PROCESSADO))
                            {
                                //retEventoCartaCorrecao.
                                Dominio.NFE2.TRetEventoInfEvento retornoEvento = retEventoCartaCorrecao.retEvento[0].infEvento;
                                if (retornoEvento.cStat.Equals(NfeStatusResposta.NFE135_EVENTO_REGISTRADO_VINCULADO_NFE) ||
                                    retornoEvento.cStat.Equals(NfeStatusResposta.NFE136_EVENTO_REGISTRADO_NAO_VINCULADO_NFE))
                                {
                                    nfeControle.NumeroProtocoloCartaCorrecao = retornoEvento.nProt;
                                    nfeControle.DataCartaCorrecao = Convert.ToDateTime(retornoEvento.dhRegEvento);
                                    nfeControle.SeqCartaCorrecao = Convert.ToInt32(retornoEvento.nSeqEvento);
                                    //nfeControle.SituacaoNfe = NfeControle.SITUACAO_CANCELADA;
                                }
                                nfeControle.SituacaoProtocoloCartaCorrecao = retornoEvento.cStat;
                                nfeControle.MensagemSitucaoCartaCorrecao = retornoEvento.xMotivo;
                                GerenciadorNFe.GetInstance().Atualizar(nfeControle);
                                files[i].Delete();
                            }
                        }
                    }
                }
            }
            return numeroProtocolo;
        }
        /// <summary>
        /// Recupera os vários retornos do processamento de Nfes
        /// </summary>
        public void RecuperarRetornosNfe()
        {
            try
            {
                IEnumerable<Loja> lojas = GerenciadorLoja.GetInstance().ObterTodos();
                foreach(Loja loja in lojas) {
                    DirectoryInfo Dir = new DirectoryInfo(loja.PastaNfeRetorno);
                    // Busca automaticamente todos os arquivos em todos os subdiretórios
                    if (Dir.Exists)
                    {
                        FileInfo[] Files = Dir.GetFiles("*", SearchOption.TopDirectoryOnly);
                        if (Files.Length > 0)
                        {
                            RecuperarLoteEnvio(loja);
                            RecuperarReciboEnvioNfe(loja.PastaNfeRetorno);
                            RecuperarResultadoProcessamentoNfe(loja.PastaNfeRetorno);
                            RecuperarResultadoCancelamentoNfe(loja.PastaNfeRetorno);
                            RecuperarResultadoConsultaNfe(loja.PastaNfeRetorno);
                            RecuperarResultadoCartaCorrecaoNfe(loja.PastaNfeRetorno);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string formataValorNFe(decimal? valor)
        {
            try
            {
                if (valor == null)
                    valor = 0;

                return ((decimal)valor).ToString("0.00", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string formataValorNFe(decimal? valor, int quantidadeDecimais)
        {
            try
            {
                if (valor == null)
                    valor = 0;
                if (quantidadeDecimais <= 2) 
                    return ((decimal)valor).ToString("0.00", CultureInfo.InvariantCulture);
                else 
                    return ((decimal)valor).ToString("0.000", CultureInfo.InvariantCulture);
                

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        

        private string formataQtdNFe(decimal? quantidade)
        {
            try
            {
                if (quantidade == null)
                    quantidade = 0;

                return ((decimal)quantidade).ToString("0.0000", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private string formataPesoNFe(decimal? quantidade)
        {
            try
            {
                if (quantidade == null)
                    quantidade = 0;

                return ((decimal)quantidade).ToString("0.000", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Atribui a classe Nfe às entidade persistente correpondente
        /// </summary>
        /// <param name="nfe"></param>
        /// <param name="_nfe"></param>
        private void Atribuir(NfeControle nfe, tb_nfe _nfe)
        {
            _nfe.chave = string.IsNullOrEmpty(nfe.Chave)?"":nfe.Chave;
            _nfe.codNFe = nfe.CodNfe;
            _nfe.justificativaCancelamento = truncate(nfe.JustificativaCancelamento,  200);
            _nfe.mensagemSituacaoProtocoloCancelamento = truncate(nfe.MensagemSitucaoProtocoloCancelamento, 100);
            _nfe.mensagemSituacaoProtocoloUso = truncate(nfe.MensagemSitucaoProtocoloUso, 100);
            _nfe.mensagemSituacaoReciboEnvio = truncate(nfe.MensagemSituacaoReciboEnvio, 100);
            
            _nfe.numeroLoteEnvio = nfe.NumeroLoteEnvio;
            _nfe.numeroProtocoloCancelamento = nfe.NumeroProtocoloCancelamento;
            _nfe.numeroProtocoloUso = nfe.NumeroProtocoloUso;
            _nfe.numeroRecibo = nfe.NumeroRecibo;
            _nfe.situacaoNfe = nfe.SituacaoNfe;
            _nfe.situacaoProtocoloCancelamento = nfe.SituacaoProtocoloCancelamento;
            _nfe.situacaoProtocoloUso = nfe.SituacaoProtocoloUso;
            _nfe.situacaoReciboEnvio = nfe.SituacaoReciboEnvio;
            _nfe.dataEmissao = nfe.DataEmissao;
            _nfe.dataCancelamento = nfe.DataCancelamento;
            
            _nfe.correcao = truncate(string.IsNullOrEmpty(nfe.Correcao)?"":nfe.Correcao, 200);
            _nfe.dataCartaCorrecao = nfe.DataCartaCorrecao;
            _nfe.mensagemSitucaoCartaCorrecao = truncate(string.IsNullOrEmpty(nfe.MensagemSitucaoCartaCorrecao)?"":nfe.MensagemSitucaoCartaCorrecao, 100);
            _nfe.numeroProtocoloCartaCorrecao = string.IsNullOrEmpty(nfe.NumeroProtocoloCartaCorrecao)?"":nfe.NumeroProtocoloCartaCorrecao;
            _nfe.seqCartaCorrecao = nfe.SeqCartaCorrecao;
            _nfe.situacaoProtocoloCartaCorrecao = string.IsNullOrEmpty(nfe.SituacaoProtocoloCartaCorrecao)?"":nfe.SituacaoProtocoloCartaCorrecao;

        }

        private string truncate(string texto, int tamanho)
        {
            if (texto.Length > tamanho)
                return texto.Substring(0, tamanho);
            return texto;
        }

        public void imprimirDANFE(NfeControle nfeControle)
        {
            if (nfeControle.SituacaoNfe == NfeControle.SITUACAO_AUTORIZADA)
            {
                Saida saida = GerenciadorSaida.GetInstance(null).Obter(nfeControle.CodSaida);
                Loja loja = GerenciadorLoja.GetInstance().Obter(saida.CodLojaOrigem).ElementAtOrDefault(0);
                try
                {
                    DateTime dataEmissao = (DateTime)nfeControle.DataEmissao;

                    Process unidanfe = new Process();
                    unidanfe.StartInfo.FileName = @"\\retaguarda\UniNFe\unidanfe.exe";
                    unidanfe.StartInfo.Arguments = " arquivo=\"" + loja.PastaNfeAutorizados
                        + dataEmissao.Year
                        + dataEmissao.Month.ToString("00")
                        + dataEmissao.Day.ToString("00") + "\\"
                        + nfeControle.Chave + "-nfe.xml";
                    unidanfe.Start();
                }
                catch (Exception ex)
                {
                    throw new NegocioException("Não foi possível realizar a impressão do DANFE. Favor contactar administrador.", ex);
                }
            }
        }

        
    }
}
