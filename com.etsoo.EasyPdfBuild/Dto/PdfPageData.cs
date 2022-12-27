using com.etsoo.EasyPdf.Content;
using System.Drawing;

namespace com.etsoo.EasyPdf.Dto
{
    /// <summary>
    /// PDF page data interface
    /// PDF 页面数据接口
    /// </summary>
    internal interface IPdfPageData
    {
        /// <summary>
        /// Define the boundaries of the physical medium on which the page shall be displayed or printed
        /// </summary>
        Rectangle? PageSize { get; }

        /// <summary>
        /// Page margin
        /// </summary>
        PdfStyleSpace? Margin { get; }

        /// <summary>
        /// Default font
        /// </summary>
        string? Font { get; }

        /// <summary>
        /// Default font size
        /// </summary>
        float? FontSize { get; }

        /// <summary>
        /// The number of degrees by which the page shall be rotated clockwise when displayed or printed.
        /// The value shall be a multiple of 90, default is 0.
        /// </summary>
        int? Rotate { get; }
    }

    /// <summary>
    /// PDF page data
    /// PDF 页面数据
    /// </summary>
    public record PdfPageData : IPdfPageData
    {
        public Rectangle? PageSize { get; set; }

        public PdfStyleSpace? Margin { get; set; }

        public string? Font { get; set; }

        public float? FontSize { get; set; }

        public int? Rotate { get; set; }
    }

    /// <summary>
    /// PDF readonly page data
    /// PDF 只读页面数据
    /// </summary>
    public record PdfReadonlyPageData : IPdfPageData
    {
        public Rectangle? PageSize { get; init; }

        public PdfStyleSpace? Margin { get; init; }

        public string? Font { get; init; }

        public float? FontSize { get; init; }

        public int? Rotate { get; init; }
    }
}
