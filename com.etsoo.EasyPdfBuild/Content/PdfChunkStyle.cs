namespace com.etsoo.EasyPdf.Content
{
    /// <summary>
    /// PDF chunk text style
    /// PDf 块文字样式
    /// </summary>
    public enum PdfChunkTextStyle
    {
        Normal,

        /// <summary>
        /// Superscript
        /// 上标
        /// </summary>
        SuperScript,

        /// <summary>
        /// Subscript
        /// 下标
        /// </summary>
        SubScript
    }

    /// <summary>
    /// PDF chunk styles
    /// PDF 块样式
    /// </summary>
    public record PdfChunkStyle : PdfStyleBase
    {
        public PdfChunkTextStyle? TextStyle { get; set; }
    }
}
