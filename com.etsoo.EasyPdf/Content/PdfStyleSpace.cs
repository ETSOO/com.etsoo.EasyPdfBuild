namespace com.etsoo.EasyPdf.Content
{
    /// <summary>
    /// PDF style space definition in pt (like padding and margin)
    /// PDF 样式空间定义
    /// </summary>
    /// <param name="Top">Top space</param>
    /// <param name="Right">Right space</param>
    /// <param name="Bottom">Bottom space</param>
    /// <param name="Left">Left space</param>
    public record PdfStyleSpace(int Top, int Right, int Bottom, int Left)
    {
        public PdfStyleSpace(int space) : this(space, space, space, space)
        {
        }

        public PdfStyleSpace(int vertical, int horizontal) : this(vertical, horizontal, vertical, horizontal)
        {
        }
    }
}
