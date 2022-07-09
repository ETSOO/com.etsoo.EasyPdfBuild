using com.etsoo.EasyPdf.Content;
using com.etsoo.EasyPdf.Dto;
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
        private readonly float defaultSpace;

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

            defaultFont = data.Font ?? parent.PageData.Font ?? "A0";
            defaultFontSize = data.FontSize ?? parent.PageData.FontSize ?? 12;
            defaultSpace = defaultFontSize * 0.2f;
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

        public async Task WriteEndAsync()
        {
            await Stream.WriteAsync(PdfOperator.ET);

            // Back to begin
            Stream.Position = 0;
        }

        public async Task<bool> WriteAsync(PdfBlock block)
        {
            // Font style
            var baseStyle = block.Style;
            baseStyle.Font ??= defaultFont;
            baseStyle.FontSize ??= defaultFontSize;
            await Stream.WriteAsync(PdfOperator.Tf(baseStyle.Font, baseStyle.FontSize.Value));

            // Leading
            // var leading = defaultSize + block.Style.Margin.Top;
            await Stream.WriteAsync(PdfOperator.TL(defaultFontSize + defaultSpace));

            // Color
            if (baseStyle.Color.HasValue)
            {
                await Stream.WriteAsync(PdfOperator.RG2(baseStyle.Color.Value));
            }

            // Last style
            var lastStyle = baseStyle with { };

            foreach (var chunk in block.Chunks)
            {
                // Font & size
                bool newFont = false;
                var font = chunk.Style.Font ?? lastStyle.Font ?? baseStyle.Font ?? defaultFont;
                var fontSize = chunk.Style.FontSize ?? lastStyle.FontSize ?? baseStyle.FontSize ?? defaultFontSize;
                if (!font.Equals(lastStyle.Font) || !fontSize.Equals(lastStyle.FontSize))
                {
                    newFont = true;
                    await Stream.WriteAsync(PdfOperator.Tf(font, fontSize));
                    await Stream.WriteAsync(PdfOperator.TL(fontSize + defaultSpace));
                }

                // Color
                var color = chunk.Style.Color;
                if (color.HasValue)
                {
                    await Stream.WriteAsync(PdfOperator.RG2(color.Value));
                }

                // Superscript or subscript
                bool superscript;
                float scriptDistance = 0;
                if ((superscript = chunk.Style.TextStyle == PdfChunkTextStyle.SuperScript) || (chunk.Style.TextStyle == PdfChunkTextStyle.SubScript))
                {
                    var scriptFontSize = fontSize * 0.6f;

                    if (!newFont)
                    {
                        await Stream.WriteAsync(PdfOperator.Tf(font, scriptFontSize));
                    }

                    scriptDistance = superscript ? fontSize - scriptFontSize * 0.9f : -scriptFontSize * 0.3f;
                    await Stream.WriteAsync(PdfOperator.Ts(scriptDistance));

                    lastStyle.Font = null;
                    lastStyle.FontSize = null;
                }
                else
                {
                    lastStyle.Font = font;
                    lastStyle.FontSize = fontSize;
                }

                // Content
                if (chunk.Content.Span.IsAllAscii())
                {
                    var content = new PdfString(chunk.Content.ToString());
                    await content.WriteToAsync(Stream);
                }
                else
                {
                    var content = new PdfBinaryString(chunk.Content.ToString());
                    await content.WriteToAsync(Stream);
                }

                if (chunk.NewLine)
                {
                    await Stream.WriteAsync(PdfOperator.SQ);
                }
                else
                {
                    await Stream.WriteAsync(PdfOperator.Tj);
                }

                // Restore scripts
                if (scriptDistance != 0)
                {
                    // The default location of the baseline can be restored by setting the text rise to 0
                    await Stream.WriteAsync(PdfOperator.Ts(0));
                }

                // Restore color
                if (color.HasValue)
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
