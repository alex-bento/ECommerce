

var ObjetoVenda = new Object();

ObjetoVenda.AdicionarCarrinho = function (idProduto) {


    var Nome = $("#nome_" + idProduto).val();
    var Qtd = $("#qtd_" + idProduto).val();

    $.ajax({
        type: 'POST',
        url: "/api/AdicionarProdutoCarrinho",
        dateType: "JSON",
        cache: false,
        async: true,
        data: {
            "id": idProduto,
            "nome": Nome,
            "qtd": Qtd
        },
        success: function (data) {
            if (data.sucesso) {
                // 1 alert sucesso, 2-alerta-perigo, alerta perigo 
                ObejtoAlerta.AlertarTela(1, "Produto adicionado no carrinho!");
            } else {
                // 1 alert sucesso, 2-alerta-perigo, alerta perigo 
                ObejtoAlerta.AlertarTela(2, "Necessaário efetuar o login!");
            }
        }
    });
}

ObjetoVenda.CarregaProduto = function()
{
    $.ajax({
        type: 'GET',
        url: "/api/ListaProdutosComEstoque",
        dateType: "JSON",
        cache: false,
        async: true,
        success: function (data) {
            var htmlConteudo = "";

            data.forEach(function (entitie) {

                htmlConteudo += "<div class='col-xs-12 col-sm-4 col-md-4 col-lg-4'>";

                var idNome = "nome_" + entitie.id;
                var idQtd = "qtd_" + entitie.id;

                htmlConteudo += "<label id='" + idNome + "' > Produtos: " + entitie.nome + "</label></br>"
                
                if (entitie.url != null && entitie.url != "" && entitie.url != undefined) {
                    htmlConteudo += "<img width='200' height='100' src='" + entitie.url + "'></br>";
                }

                htmlConteudo += "<label> Valor:" + entitie.valor + "</label></br>"

                htmlConteudo += "Quantidade : <input : <input type'number' value='1' id='" + idQtd + "'>" 

                htmlConteudo += "<input type='button' onclick='ObjetoVenda.AdicionarCarrinho(" + entitie.id + ")' value ='Comprar'> </br>";

                htmlConteudo += "</div>";
            });

            $("#DivVenda").html(htmlConteudo);
        }
    })
    
}

ObjetoVenda.CarregarQtdCarrinho = function () {
    // $("#qtdCarrinho").text("(0)");

    $.ajax({
        type: 'GET',
        url: "/api/QtdProdutosCarrinho",
        dateType: "JSON",
        cache: false,
        async: true,
        success: function (data) {
            if (data.sucesso) {
                $("#qtdCarrinho").text("(" + data.qtd + ")");
            }
        }
    });

    setTimeout(ObjetoVenda.CarregarQtdCarrinho, 10000);
}

$(function () {

    ObjetoVenda.CarregaProduto();
    ObjetoVenda.CarregarQtdCarrinho();

});