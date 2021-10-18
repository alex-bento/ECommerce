using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.InterfaceServices
{
    public interface IIServiceCompraUsuario
    {
        public Task<CompraUsuario> CarrinhoCompra(string userId);
        public Task<CompraUsuario> ProdutosComprados(string userId);

    }
}
