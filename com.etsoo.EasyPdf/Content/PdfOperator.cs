using System.Numerics;
using System.Text;

namespace com.etsoo.EasyPdf.Content
{
    /// <summary>
    /// PDF operators
    /// https://www.syncfusion.com/succinctly-free-ebooks/pdf/text-operators
    /// A.1 Operator Summary
    /// PDF 运算符
    /// </summary>
    internal static class PdfOperator
    {
        /// <summary>
        /// Begin a text object
        /// </summary>
        public readonly static byte[] BT = new byte[] { 66, 84, PdfConstants.LineFeedByte };

        /// <summary>
        /// End a text object
        /// </summary>
        public readonly static byte[] ET = new byte[] { 69, 84, PdfConstants.LineFeedByte };

        /// <summary>
        /// Show a text string
        /// </summary>
        public readonly static byte[] Tj = new byte[] { PdfConstants.SpaceByte, 84, 106, PdfConstants.LineFeedByte };

        /// <summary>
        /// T* Move to the start of the next line
        /// </summary>
        public readonly static byte[] T42 = new byte[] { 84, 42, PdfConstants.LineFeedByte };

        /// <summary>
        /// Single quote
        /// moves to the next line then displays the text. This is the exact same functionality as T* followed by Tj
        /// </summary>
        public readonly static byte[] SQ = new byte[] { 39, PdfConstants.LineFeedByte };

        /// <summary>
        /// Set RGB color for stroking operations
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>Bytes</returns>
        public static byte[] RG(PdfColor color)
        {
            return Encoding.ASCII.GetBytes($"{color}").Concat(new byte[]
            {
                PdfConstants.SpaceByte,
                82,
                71,
                PdfConstants.LineFeedByte
            }).ToArray();
        }

        /// <summary>
        /// Set RGB color for stroking and nonstroking operations
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>Bytes</returns>
        public static byte[] RG2(PdfColor color)
        {
            var bytes = RG(color);
            bytes[^1] = PdfConstants.SpaceByte;
            return bytes.Concat(rg(color)).ToArray();
        }

        /// <summary>
        /// Set RGB color for nonstroking operations
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>Bytes</returns>
        public static byte[] rg(PdfColor color)
        {
            return Encoding.ASCII.GetBytes($"{color}").Concat(new byte[]
            {
                PdfConstants.SpaceByte,
                114,
                103,
                PdfConstants.LineFeedByte
            }).ToArray();
        }

        /// <summary>
        /// Move to the start of the next line, offset from the start of the current line by (tx , ty )
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Bytes</returns>
        public static byte[] Td(Vector2 point)
        {
            return Encoding.ASCII.GetBytes($"{point.X} {point.Y}").Concat(new byte[]
            {
                PdfConstants.SpaceByte,
                84,
                100,
                PdfConstants.LineFeedByte
            }).ToArray();
        }

        /// <summary>
        /// Set text font and size
        /// </summary>
        /// <returns>Bytes</returns>
        public static byte[] Tf(string font, float size)
        {
            return Encoding.ASCII.GetBytes($"/{font} {size}").Concat(new byte[]
            {
                PdfConstants.SpaceByte,
                84,
                102,
                PdfConstants.LineFeedByte
            }).ToArray();
        }

        /// <summary>
        /// Set the text leading
        /// </summary>
        /// <param name="leading">Leading</param>
        /// <returns>Bytes</returns>
        public static byte[] TL(float leading)
        {
            return Encoding.ASCII.GetBytes($"{leading}").Concat(new byte[]
            {
                PdfConstants.SpaceByte,
                84,
                76,
                PdfConstants.LineFeedByte
            }).ToArray();
        }

        /// <summary>
        /// Offsets the vertical position of the text to create superscripts or subscripts
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <returns>Bytes</returns>
        public static byte[] Ts(float offset)
        {
            return Encoding.ASCII.GetBytes($"{offset}").Concat(new byte[]
            {
                PdfConstants.SpaceByte,
                84,
                115,
                PdfConstants.LineFeedByte
            }).ToArray();
        }
    }
}
