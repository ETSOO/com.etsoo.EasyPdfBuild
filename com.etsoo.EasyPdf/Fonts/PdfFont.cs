using com.etsoo.EasyPdf.Objects;

namespace com.etsoo.EasyPdf.Fonts
{
    /// <summary>
    /// PDF font
    /// PDF 字体
    /// </summary>
    public abstract class PdfFont : PdfObjectDic
    {
        public override string Type => "Font";

        public abstract string Subtype { get; }
    }
}
