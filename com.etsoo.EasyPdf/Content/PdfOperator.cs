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
        // 9.3.6 Text Rendering Mode
        // 9.2.3 Achieving Special Graphical Effects
        // Q to exit clipping
        public enum TrMode : byte
        {
            Fill = 0,
            Stroke = 1,
            FillThenStroke = 2,
            Invisible = 3,
            FillThenClipping = 4,
            StrokeThenClipping = 5,
            FillThenStrokeThenClipping = 6,
            Clipping = 7
        }

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
        /// Save graphics state
        /// </summary>
        public readonly static byte[] q = new byte[] { 113, PdfConstants.LineFeedByte };

        /// <summary>
        /// Restore graphics state
        /// </summary>
        public readonly static byte[] Q = new byte[] { 81, PdfConstants.LineFeedByte };

        /// <summary>
        /// Setup draw style
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="style">Font style</param>
        /// <param name="size">Font size</param>
        /// <returns>Task</returns>
        public static async Task SetupStyle(Stream stream, PdfFontStyle style, float size)
        {
            var mode = TrMode.Fill;
            if (style.HasFlag(PdfFontStyle.Italic))
                await stream.WriteAsync(Tm(1, 0, 0.21256f, 1, 0, 0, true));

            if (style.HasFlag(PdfFontStyle.Bold))
            {
                await stream.WriteAsync(w(size / 30));
                mode = TrMode.FillThenStroke;
            }

            await stream.WriteAsync(Tr(mode));
        }

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
        /// Set the text matrix
        /// Most of the other text positioning and text state commands are simply predefined operations on the transformation matrix
        /// 
        /// a b 0
        /// c d 0
        /// e f 1
        /// 
        /// The a and d values determine its horizontal and vertical scale (obtained by [ sx 0 0 sy 0 0 ])
        /// The e and f values determine the horizontal and vertical position of the text
        /// Rotations are produced by [ cos θ sin θ −sin θ cos θ 0 0 ]
        ///     which has the effect of rotating the coordinate system axes by an angle θ counterclockwise
        /// Skew is specified by [ 1 tan α tan β 1 0 0 ], which skews the x axis by an angle α and the y axis by an angle β
        /// cm manipulates the current transformation matrix (CTM), an element of the PDF graphics state, which defines the transformation from user space to device space
        /// https://stackoverflow.com/questions/34900352/pdf-image-positioning
        /// </summary>
        /// <returns></returns>
        public static byte[] Tm(float a, float b, float c, float d, float e, float f, bool cm = false)
        {
            // cm - Concatenate matrix to current transformation matrix
            return Encoding.ASCII.GetBytes($"{a} {b} {c} {d} {e} {f}").Concat(new byte[]
            {
                PdfConstants.SpaceByte,
                (byte)(cm ? 99 : 84),
                109,
                PdfConstants.LineFeedByte
            }).ToArray();
        }

        /// <summary>
        /// Text rendering mode
        /// </summary>
        /// <param name="mode">Mode</param>
        /// <returns>Bytes</returns>
        public static byte[] Tr(TrMode mode)
        {
            return new byte[]
            {
                (byte)(48 + (byte)mode),
                PdfConstants.SpaceByte,
                84,
                114,
                PdfConstants.LineFeedByte
            };
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

        /// <summary>
        /// Set line width
        /// </summary>
        /// <param name="width">Width</param>
        /// <returns>Bytes</returns>
        public static byte[] w(float width)
        {
            return Encoding.ASCII.GetBytes($"{width}").Concat(new byte[]
            {
                PdfConstants.SpaceByte,
                119,
                PdfConstants.LineFeedByte
            }).ToArray();
        }
    }
}
