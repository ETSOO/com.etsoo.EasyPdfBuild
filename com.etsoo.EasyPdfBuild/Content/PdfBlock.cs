namespace com.etsoo.EasyPdf.Content
{
    /// <summary>
    /// PDF block, behaves like HTML DIV
    /// PDF 内容块
    /// </summary>
    public record PdfBlock
    {
        /// <summary>
        /// All chunks
        /// 全部块
        /// </summary>
        public List<PdfChunk> Chunks { get; } = new List<PdfChunk>();

        /// <summary>
        /// Style
        /// 样式
        /// </summary>
        public PdfBlockStyle Style { get; } = new PdfBlockStyle();

        /// <summary>
        /// Add chunk
        /// 添加块
        /// </summary>
        /// <param name="chunk">Chunk</param>
        public void Add(PdfChunk chunk)
        {
            Chunks.Add(chunk);
        }

        /// <summary>
        /// Add text content
        /// 添加文本内容
        /// </summary>
        /// <param name="content">Content</param>
        /// <param name="newline">Is new line</param>
        public PdfChunk Add(ReadOnlySpan<char> content, bool newline = false)
        {
            var chunk = new PdfChunk(content) { NewLine = newline };
            Add(chunk);
            return chunk;
        }
    }
}
