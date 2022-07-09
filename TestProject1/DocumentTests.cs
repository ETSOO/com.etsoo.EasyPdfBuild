using com.etsoo.EasyPdf;
using com.etsoo.EasyPdf.Content;
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
            var path = "D:\\a.pdf";
            File.Delete(path);

            // PDF document
            var stream = File.OpenWrite(path);
            var pdf = new PdfDocument(stream);
            pdf.PageData.FontSize = 24;

            // Get writer and start writing
            var w = await pdf.GetWriterAsync();

            // Paragraph
            var p = new PdfParagraph();
            p.Add("Hello World, (ETSOO).\nA new line. X");
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
            pn.Add("第二行 - How will it show?");
            pn.Add("Simple CSS Units is a free tool for Web Designers & Front End Developers to simplify the process of converting CSS units. Simply enter your units in the fields below and watch your unit get converted in realtime, eg Points to Pixels.", true);

            await w.AddAsync(pn);

            // Close
            await pdf.CloseAsync();
        }
    }
}
