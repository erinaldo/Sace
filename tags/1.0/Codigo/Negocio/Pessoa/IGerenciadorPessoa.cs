﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio;


namespace Negocio
{
    public interface IGerenciadorPessoa: IGerenciadorNegocio<Pessoa, Int64>
    {
        Pessoa obterPessoa(Int64 codPessoa);

        Pessoa obterPessoaNomeIgual(String nome);
    }
}
