using com.etsoo.EasyPdf.Types;

namespace com.etsoo.EasyPdf.Fonts
{
    internal class PdfFont0 : PdfFont
    {
        // 9.7.5.2 Predefined CMaps
        public static PdfFont0 Create(string? language)
        {
            string baseFont, encoding;

            switch (language)
            {
                case "zh-CN":
                case "zh-SG":
                    baseFont = "Helvetica";
                    encoding = "UniGB-UTF16-H";
                    break;
                case "zh-TW":
                case "zh-HK":
                    baseFont = "Helvetica";
                    encoding = "UniCNS-UTF16-H";
                    break;
                default:
                    baseFont = "Helvetica";
                    encoding = "Identity-H";
                    break;
            }

            return new PdfFont0(baseFont, encoding);
        }

        public override string Subtype => "Type0";

        public string BaseFont { get; set; }

        public string Encoding { get; set; }

        public PdfObject? DescendantFonts { get; set; }

        public PdfObject? ToUnicode { get; set; }

        public PdfFont0(string baseFont, string encoding)
        {
            BaseFont=baseFont;
            Encoding=encoding;
        }

        protected override void AddItems()
        {
            base.AddItems();

            Dic.AddNames(nameof(Subtype), Subtype);
            Dic.AddNames(nameof(BaseFont), BaseFont);
            Dic.AddNames(nameof(Encoding), Encoding);
            Dic.AddNameItem(nameof(DescendantFonts), DescendantFonts);
            Dic.AddNameItem(nameof(ToUnicode), ToUnicode);
        }
    }
}
