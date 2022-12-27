namespace com.etsoo.EasyPdf.Content
{
    /// <summary>
    /// PDF block styles
    /// PDF内容块样式
    /// </summary>
    public record PdfBlockStyle : PdfStyleBase
    {
        /// <summary>
        /// Padding
        /// 填充距离
        /// </summary>
        public PdfStyleSpace? Padding { get; set; }
    }
}
