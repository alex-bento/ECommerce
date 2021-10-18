using Entities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Ecommerce.Models
{
    public class HelpQrCode : Controller
    {

        private async Task<byte[]> GeraQrCode (string dadosBanco)
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();

            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(dadosBanco, QRCodeGenerator.ECCLevel.H);

            QRCode qRCode = new QRCode(qRCodeData);

            Bitmap qrCodeImage = qRCode.GetGraphic(20);

            var bitmapBytes = BtimapTBytes(qrCodeImage);

            return bitmapBytes;
        }

        private static byte[] BtimapTBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                return stream.ToArray();
            }
        }

        public async Task<IActionResult> Download(CompraUsuario compraUsuario, IWebHostEnvironment _enviroment)
        {
            using (var doc = new PdfSharpCore.Pdf.PdfDocument())
            {
                #region Configuracoes da folha
                var page = doc.AddPage();

                page.Size = PdfSharpCore.PageSize.A4;
                page.Orientation = PdfSharpCore.PageOrientation.Portrait;


                var grafico = XGraphics.FromPdfPage(page);
                var corFonte = XBrushes.Black;
                #endregion

                #region Numeração das Paginas
                int qtdPaginas = doc.PageCount;

                var numeracaoPagina = new PdfSharpCore.Drawing.Layout.XTextFormatter(grafico);
                numeracaoPagina.DrawString(Convert.ToString(qtdPaginas), new PdfSharpCore.Drawing.XFont("Arial", 10), corFonte, new PdfSharpCore.Drawing.XRect(575, 825, page.Width, page.Height));
                #endregion

                #region Logo

                var webRoot = _enviroment.WebRootPath;

                var logoFatura = string.Concat(webRoot, "/img/", "loja-virtual-1.png");

                XImage imagem = XImage.FromFile(logoFatura);

                grafico.DrawImage(imagem, 20, 5, 300, 50);
                #endregion

                #region Informações 1
                var relatorioCobranca = new PdfSharpCore.Drawing.Layout.XTextFormatter(grafico);

                var titulo = new PdfSharpCore.Drawing.XFont("Arial", 14, PdfSharpCore.Drawing.XFontStyle.Bold);

                relatorioCobranca.Alignment = PdfSharpCore.Drawing.Layout.XParagraphAlignment.Center;

                relatorioCobranca.DrawString("BOLETO ONLINE", titulo, corFonte, new XRect(0, 65, page.Width, page.Height));

                #endregion

                #region Informações 2

                var alturaRituloDetalhesY = 120;
                var detalhes = new PdfSharpCore.Drawing.Layout.XTextFormatter(grafico);
                var tituloInfo_1 = new PdfSharpCore.Drawing.XFont("Arial", 8, XFontStyle.Regular);

                detalhes.DrawString("Dados do Banco", tituloInfo_1, corFonte, new XRect(25, alturaRituloDetalhesY, page.Width, page.Height));

                detalhes.DrawString("Banco Itau 004", tituloInfo_1, corFonte, new XRect(150, alturaRituloDetalhesY, page.Width, page.Height));

                alturaRituloDetalhesY += 9;
                detalhes.DrawString("Código Gerado", tituloInfo_1, corFonte, new XRect(25, alturaRituloDetalhesY, page.Width, page.Height));
                detalhes.DrawString("000000 000000 000000 000000", tituloInfo_1, corFonte, new XRect(150, alturaRituloDetalhesY, page.Width, page.Height));

                alturaRituloDetalhesY += 9;

                detalhes.DrawString("Quantidade", tituloInfo_1, corFonte, new XRect(25, alturaRituloDetalhesY, page.Width, page.Height));
                detalhes.DrawString(compraUsuario.QuantidadeProdutos.ToString(), tituloInfo_1, corFonte, new XRect(150, alturaRituloDetalhesY, page.Width, page.Height));

                alturaRituloDetalhesY += 9;
                detalhes.DrawString("Valor Total", tituloInfo_1, corFonte, new XRect(25, alturaRituloDetalhesY, page.Width, page.Height));
                detalhes.DrawString(compraUsuario.QuantidadeProdutos.ToString(), tituloInfo_1, corFonte, new XRect(150, alturaRituloDetalhesY, page.Width, page.Height));

                var tituloInfo_2 = new PdfSharpCore.Drawing.XFont("Arial", 8, XFontStyle.Bold);

                try
                {
                    // Aqui pode mudar, aqui gerar um QrCode
                    var img = await GeraQrCode("Dados do banco aqui");

                    Stream streamImage = new MemoryStream(img);

                    XImage qrCode = XImage.FromStream(() => streamImage);

                    alturaRituloDetalhesY += 40;
                    grafico.DrawImage(qrCode, 140, alturaRituloDetalhesY, 310, 310);

                }
                catch (Exception err)
                {

                  
                }


                alturaRituloDetalhesY += 620;
                detalhes.DrawString("Canhoto com QrCode para pagamento online", tituloInfo_2, corFonte, new XRect(20, alturaRituloDetalhesY, page.Width, page.Height));

                #endregion


                using (MemoryStream stream = new MemoryStream())
                {
                    var contextType = "application/pdf";

                    doc.Save(stream, false);

                    return File(stream.ToArray(), contextType, "BoletoLojaOnline.pdf");
                }

            }
        }
    }

}

