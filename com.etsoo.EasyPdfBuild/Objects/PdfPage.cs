using com.etsoo.EasyPdf.Content;
using com.etsoo.EasyPdf.Dto;
using com.etsoo.EasyPdf.Fonts;
using com.etsoo.EasyPdf.Support;
using com.etsoo.EasyPdf.Types;
using System.Drawing;
using System.Numerics;

namespace com.etsoo.EasyPdf.Objects
{
    /// <summary>
    /// PDF page object
    /// PDF 页面对象
    /// </summary>
    internal class PdfPage : PdfObjectDic
    {
        public override string Type => "Page";

        public override PdfObject Obj { get => base.Obj!; }

        public PdfPageTree Parent { get; }

        public PdfObject ParentObj { get; }

        public PdfPageResource Resources { get; } = new PdfPageResource();

        public DateTime? LastModified { get; set; }

        public PdfObject? Contents { get; set; }

        public Stream Stream { get; }

        public PdfReadonlyPageData Data { get; }

        public int Index { get; }

        private readonly Rectangle pageSize;
        private readonly PdfStyleSpace space;
        private Vector2? lastPoint;

        private readonly string defaultFont;
        private readonly float defaultFontSize;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="obj">Obj</param>
        /// <param name="parent">Parent page tree</param>
        /// <param name="data">Page data</param>
        public PdfPage(PdfObject obj, PdfPageTree parent, PdfReadonlyPageData data) : base(obj)
        {
            Parent = parent;
            ParentObj = parent.Obj.AsRef();
            Stream = PdfConstants.StreamManager.GetStream();
            Data = data;
            Index = parent.Count;

            pageSize = data.PageSize ?? parent.PageData.PageSize ?? PdfPageSize.A4;
            space = data.Margin ?? parent.PageData.Margin ?? new PdfStyleSpace(60);

            defaultFont = data.Font ?? parent.PageData.Font ?? "Arial";
            defaultFontSize = data.FontSize ?? parent.PageData.FontSize ?? 12;
        }

        /*
        public PdfPage(PdfObject obj, PdfDictionary dic) : base(obj, dic)
        {
            Parent = dic.GetRequired<PdfObject>("Parent");

            LastModified = dic.GetValue<DateTime?>("LastModified");

            var mediaBoxArray = dic.Get<PdfArray>("MediaBox");
            PageSize = mediaBoxArray.ToRectangle();

            Contents = dic.Get<PdfObject>("Contents");

            // Match the stream with contents
            Stream = default!;

            Rotate = dic.GetValue<int?>("Rotate");

            // Resources
            var resources = dic.GetRequired<PdfDictionary>("Resources");
        }
        */

        protected override void AddItems()
        {
            base.AddItems();

            Dic.AddNameItem(nameof(Parent), ParentObj);

            if (Data.PageSize != null && !Data.PageSize.Equals(Parent.PageData.PageSize))
            {
                // No necessary to write duplicate size
                Dic.AddNameRect(PdfPageTree.MediaBoxField, Data.PageSize);
            }

            Dic.AddNameInt(nameof(Data.Rotate), Data.Rotate);
            Dic.AddNameItem(nameof(Contents), Contents);
            Dic.AddNameItem(nameof(Resources), Resources);
            Dic.AddNameDate(nameof(LastModified), LastModified);
        }

        private Vector2 CalculatePoint(Vector2 point)
        {
            return point with
            {
                X = point.X + space.Left,
                Y = pageSize.Height - point.Y - space.Top
            };
        }

        public async Task PrepareAsync()
        {
            await Stream.WriteAsync(PdfOperator.BT);

            lastPoint = new Vector2();
            await Stream.WriteAsync(PdfOperator.Td(CalculatePoint(lastPoint.Value)));
        }

        private bool gStateSaved;

        // Restore graphics state
        private void RestoreGState()
        {
            if (gStateSaved)
            {
                Stream.Write(PdfOperator.Q);

                gStateSaved = false;
            }
        }

        // Save graphics state
        private void SaveGState()
        {
            Stream.Write(PdfOperator.q);
            gStateSaved = true;
        }

        public async Task WriteEndAsync()
        {
            await Stream.WriteAsync(PdfOperator.ET);

            // Back to begin
            Stream.Position = 0;
        }

        public async Task<bool> WriteAsync(PdfBlock block, IPdfWriter writer)
        {
            // Style
            var baseStyle = block.Style;
            baseStyle.Font ??= defaultFont;
            baseStyle.FontSize ??= defaultFontSize;

            // Last style
            var lastStyle = baseStyle with { };

            // Current font
            IPdfFont currentFont = default!;

            // Artificial draw
            var artificialDraw = false;

            for (var c = 0; c < block.Chunks.Count; c++)
            {
                var chunk = block.Chunks[c];

                // Font & size
                bool fontCreated = false;
                var font = chunk.Style.Font ?? lastStyle.Font ?? baseStyle.Font ?? defaultFont;
                var fontSize = chunk.Style.FontSize ?? lastStyle.FontSize ?? baseStyle.FontSize ?? defaultFontSize;
                var fontStyle = chunk.Style.FontStyle ?? baseStyle.FontStyle ?? PdfFontStyle.Regular;
                if (c == 0 || !font.Equals(lastStyle.Font) || !fontSize.Equals(lastStyle.FontSize) || !fontStyle.Equals(lastStyle.FontStyle))
                {
                    var newFont = writer.CreateFont(font, fontSize, fontStyle);
                    if (!newFont.Equals(currentFont))
                    {
                        currentFont = newFont;
                        fontCreated = true;

                        await Stream.WriteAsync(PdfOperator.Tf(currentFont.RefName, fontSize));
                        await Stream.WriteAsync(PdfOperator.TL(fontSize + currentFont.LineGap));
                    }
                }

                // Color
                var color = chunk.Style.Color;
                bool hasColor = false;
                if (c == 0 || (hasColor = color.HasValue && !color.Equals(baseStyle.Color)))
                {
                    var chunkColor = chunk.Style.Color ?? baseStyle.Color;
                    if (chunkColor.HasValue)
                    {
                        await Stream.WriteAsync(PdfOperator.RG2(chunkColor.Value));
                    }
                }

                // Artificial draw style
                if (!currentFont.IsMatch && fontStyle != PdfFontStyle.Regular)
                {
                    // When not match, always choose the normal font
                    // SaveGState();
                    artificialDraw = true;
                    if (!lastStyle.FontStyle.HasValue || !lastStyle.FontStyle.Value.HasFlag(fontStyle))
                    {
                        SaveGState();
                        await PdfOperator.SetupStyle(Stream, fontStyle, currentFont.Size);
                    }
                }
                else if (artificialDraw)
                {
                    //await Stream.WriteAsync(PdfOperator.w(0));
                    //artificialDraw = false;
                    RestoreGState();
                }

                // Superscript or subscript
                bool superscript;
                float scriptDistance = 0;
                if ((superscript = chunk.Style.TextStyle == PdfChunkTextStyle.SuperScript) || (chunk.Style.TextStyle == PdfChunkTextStyle.SubScript))
                {
                    var scriptItem = superscript ? currentFont.Superscript : currentFont.Subscript;
                    var scriptFontSize = scriptItem.Size;
                    scriptDistance = scriptItem.Offset;

                    if (!fontCreated)
                    {
                        // Script font
                        currentFont = writer.CreateFont(font, scriptFontSize, fontStyle);
                        await Stream.WriteAsync(PdfOperator.Tf(currentFont.RefName, scriptFontSize));
                    }

                    await Stream.WriteAsync(PdfOperator.Ts(scriptDistance));

                    lastStyle.Font = null;
                    lastStyle.FontSize = null;
                    lastStyle.FontStyle = null;
                }
                else
                {
                    lastStyle.Font = font;
                    lastStyle.FontSize = fontSize;
                    lastStyle.FontStyle = fontStyle;
                }

                // Content
                await currentFont.WriteAsync(Stream, chunk);

                // Restore graphics
                // RestoreGState();

                // Restore scripts
                if (scriptDistance != 0)
                {
                    // The default location of the baseline can be restored by setting the text rise to 0
                    await Stream.WriteAsync(PdfOperator.Ts(0));
                }

                // Restore color
                if (hasColor)
                {
                    await Stream.WriteAsync(PdfOperator.RG2(PdfColor.Black));
                }
            }

            // Move to start of next text line
            await Stream.WriteAsync(PdfOperator.T42);

            return false;
        }
    }
}
