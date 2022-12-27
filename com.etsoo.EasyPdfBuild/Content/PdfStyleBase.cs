namespace com.etsoo.EasyPdf.Content
{
    /// <summary>
    /// PDF line style
    /// PDF 线条样式
    /// </summary>
    public enum PdfLineStyle
    {
        Solid,
        Dotted
    }

    /// <summary>
    /// PDF line kind
    /// PDf 线条类型
    /// </summary>
    public enum PdfLineKind
    {
        Underline,
        LineThrough
    }

    /// <summary>
    /// PDF text decoration
    /// PDF 文字修饰
    /// </summary>
    public record PdfTextDecoration
    {
        /// <summary>
        /// Kind
        /// 类型
        /// </summary>
        public PdfLineKind Kind { get; set; }

        /// <summary>
        /// Line style
        /// 线条样式
        /// </summary>
        public PdfLineStyle Style { get; set; }

        /// <summary>
        /// Color
        /// 颜色
        /// </summary>
        public PdfColor? Color { get; set; }

        /// <summary>
        /// Thickness
        /// 厚度
        /// </summary>
        public ushort Thickness { get; set; }
    }

    /// <summary>
    /// The font styles
    /// 字体样式
    /// </summary>
    [Flags]
    public enum PdfFontStyle
    {
        /// <summary>
        /// Regular
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Bold
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Italic
        /// </summary>
        Italic = 2,

        /// <summary>
        /// Bold and Italic
        /// </summary>
        BoldItalic = 3
    }

    /// <summary>
    /// PDF base styles
    /// PDF 基本样式
    /// </summary>
    public record PdfStyleBase
    {
        /// <summary>
        /// Color
        /// 颜色
        /// </summary>
        public PdfColor? Color { get; set; }

        /// <summary>
        /// Font
        /// 字体
        /// </summary>
        public string? Font { get; set; }

        /// <summary>
        /// Font size
        /// 字体大小
        /// </summary>
        public float? FontSize { get; set; }

        /// <summary>
        /// Font style
        /// 字体样式
        /// </summary>
        public PdfFontStyle? FontStyle { get; set; }

        /// <summary>
        /// Letter spacing
        /// 字母间距
        /// </summary>
        public float? LetterSpacing { get; set; }

        /// <summary>
        /// Margin
        /// 外延距离
        /// </summary>
        public PdfStyleSpace? Margin { get; set; }

        /// <summary>
        /// Text decoration
        /// 文字修饰
        /// </summary>
        public PdfTextDecoration? TextDecoration { get; set; }

        /// <summary>
        /// Word spacing
        /// 字间距
        /// </summary>
        public float? WordSpacing { get; set; }
    }
}
