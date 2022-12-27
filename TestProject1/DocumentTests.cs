using com.etsoo.EasyPdf;
using com.etsoo.EasyPdf.Content;
using com.etsoo.EasyPdf.Fonts;
using System.Runtime.Versioning;

namespace TestProject1
{
    [RequiresPreviewFeatures]
    [TestClass]
    public class DocumentTests
    {
        [TestMethod]
        public async Task Parse()
        {
            var parser = await PdfParser.ParseAsync(File.OpenRead("Resources\\etsoo.pdf"));

            Assert.AreEqual(1.4M, parser.Version);
        }

        [TestMethod]
        public async Task SaveAsync()
        {
            /*
            var fc = new PdfFontCollection();
            await fc.LoadAsync("C:\\Windows\\Fonts\\WINGDNG2.TTF");
            await fc.LoadAsync("C:\\Windows\\Fonts\\arialbi.ttf");
            await fc.LoadAsync("C:\\Windows\\Fonts\\simfang.ttf");
            await fc.LoadAsync("C:\\Windows\\Fonts\\msyh.ttc");
            */

            var path = "D:\\a.pdf";
            File.Delete(path);

            // PDF document
            var stream = File.OpenWrite(path);
            var pdf = new PdfDocument(stream);
            pdf.DocInfo.Title = "亿速PDF";

            pdf.PageData.FontSize = 18;

            // Fonts
            // await pdf.Fonts.LoadAsync("D:\\subset.ttf");
            await pdf.Fonts.LoadAsync("C:\\Windows\\Fonts\\simsun.ttc");
            pdf.PageData.Font = "宋体";

            //await pdf.Fonts.LoadAsync("C:\\Windows\\Fonts\\msyh.ttc");
            //pdf.PageData.Font = "微软雅黑";

            // Get writer and start writing
            var w = await pdf.GetWriterAsync();
            await w.NewPageAsync();

            // Paragraph
            var hp = new PdfParagraph();
            hp.Style.Font = PdfFontCollection.FontHelvetica;
            hp.Style.FontSize = 24;
            hp.Style.FontStyle = PdfFontStyle.BoldItalic;
            hp.Add("a").Style.TextStyle = PdfChunkTextStyle.SuperScript;
            hp.Add("Hello, PDF");
            hp.Add("2").Style.TextStyle = PdfChunkTextStyle.SubScript;
            await w.AddAsync(hp);

            var p = new PdfParagraph();
            p.Add("亿速思维, (ETSOO).\nA new line. X");
            p.Add("2").Style.TextStyle = PdfChunkTextStyle.SubScript;
            p.Add(", X");
            p.Add("2").Style.TextStyle = PdfChunkTextStyle.SuperScript;
            p.Add(" - Line 1 ends");

            p.Add("Chinese - Second line. X", true).Style.FontSize = 12;
            p.Add("2").Style.TextStyle = PdfChunkTextStyle.SubScript;
            p.Add(", X").Style.FontSize = 12;
            p.Add("2").Style.TextStyle = PdfChunkTextStyle.SuperScript;
            p.Add("Third line: Simple CSS Units", true).Style.Color = PdfColor.Parse("#e21821");

            await w.AddAsync(p);

            var pn = new PdfParagraph();
            pn.Add("第二行 - How will it show?").Style.FontStyle = PdfFontStyle.Bold | PdfFontStyle.Italic;
            pn.Add("亿速思维").Style.FontStyle = PdfFontStyle.Bold;
            pn.Add("亿速思维 - Simple CSS Units is a free tool for Web Designers & Front End Developers to simplify the process of converting CSS units. Simply enter your units in the fields below and watch your unit get converted in realtime, eg Points to Pixels.", true);

            await w.AddAsync(pn);

            // Close
            await pdf.CloseAsync();
        }
    }
}
