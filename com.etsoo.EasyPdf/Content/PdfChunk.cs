namespace com.etsoo.EasyPdf.Content
{
    /// <summary>
    /// PDF chunk (inline element), behaves like HTML SPAN
    /// PDF 块（行内元素）
    /// </summary>
    public record PdfChunk
    {
        /// <summary>
        /// Content
        /// 内容
        /// </summary>
        public ReadOnlyMemory<char> Content { get; init; }

        /// <summary>
        /// Is new line
        /// 是否为新行
        /// </summary>
        public bool NewLine { get; set; }

        /// <summary>
        /// Style
        /// 样式
        /// </summary>
        public PdfChunkStyle Style { get; } = new PdfChunkStyle();

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="content">Content</param>
        public PdfChunk(ReadOnlySpan<char> content)
        {
            Memory<char> cache = new char[content.Length];
            content.CopyTo(cache.Span);
            Content = cache;
        }
    }
}
