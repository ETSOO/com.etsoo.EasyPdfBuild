using com.etsoo.EasyPdf.Types;

namespace com.etsoo.EasyPdf.Fonts
{
    internal class PdfFont1 : PdfFont
    {
        // 9.6.6 Predefined encodings
        public static readonly string[] PredefinedEncodings = new[]
        {
            "StandardEncoding", "MacRomanEncoding", "MacExpertEncoding", "WinAnsiEncoding"
        };

        // 9.6.2.2 Standard Type 1 Fonts (Standard 14 Fonts)
        public static readonly string[] StandardFonts = new[]
        {
            "Helvetica", "Helvetica Bold", "Helvetica Oblique", "Helvetica Bold-Oblique",
            "Courier", "Courier Bold", "Courier Oblique", "Courier Bold-Oblique",
            "Times Roman", "Times Bold", "Times Italic", "Times Bold-Italic",
            "Symbol", "Zapf Dingbats"
        };

        public static PdfFont1 Create()
        {
            return new PdfFont1(StandardFonts[0], PredefinedEncodings[0]);
        }

        public override string Subtype => "Type1";

        public string BaseFont { get; set; }

        public string Encoding { get; set; }

        public PdfObject? FontDescriptor { get; set; }

        public PdfObject? ToUnicode { get; set; }

        public PdfFont1(string baseFont, string encoding)
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
            Dic.AddNameItem(nameof(FontDescriptor), FontDescriptor);
            Dic.AddNameItem(nameof(ToUnicode), ToUnicode);
        }
    }
}
