using com.etsoo.EasyPdf.Content;
using com.etsoo.EasyPdf.Dto;
using com.etsoo.EasyPdf.Fonts;
using com.etsoo.EasyPdf.Objects;
using com.etsoo.EasyPdf.Types;
using System.Text;

namespace com.etsoo.EasyPdf
{
    /// <summary>
    /// PDF content writer
    /// PDF 内容编写器
    /// </summary>
    public class PdfWriter : IAsyncDisposable
    {
        private readonly SortedDictionary<ushort, PdfReference> refs = new() { [0] = new PdfReference(0, 65535, true) };
        private readonly Stream stream;
        private readonly PdfPageTree pageTree;

        private bool disposed = false;

        private ushort objIndex = 0;
        private PdfPage? currentPage;
        private PdfObject? dicInfoObj;
        private PdfObject? fontObj;
        private string? language;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="saveStream">Save stream</param>
        /// <param name="pageData">Page data</param>
        public PdfWriter(Stream saveStream, PdfPageData pageData)
        {
            stream = saveStream;

            var obj = CreateObj();
            pageTree = new PdfPageTree(obj, pageData);
        }

        /// <summary>
        /// 1 0 obj
        /// Create reference obj
        /// 创建索引对象
        /// </summary>
        /// <returns>Result</returns>
        private PdfObject CreateObj()
        {
            // Next index
            objIndex++;

            // Reference obj
            return new PdfObject(objIndex, false);
        }

        /// <summary>
        /// Async add dictionary data object
        /// 异步添加字典数据对象
        /// </summary>
        /// <param name="dic">Dictionary data object</param>
        /// <returns>Task</returns>
        public async Task<PdfObject> WriteDicAsync(PdfObjectDic dic)
        {
            // Current position
            var pos = (uint)stream.Position;

            // When obj is null
            if (dic.Obj == null)
            {
                dic.Obj = CreateObj();
            }

            // Write to stream
            await dic.WriteToAsync(stream);

            // Add a reference
            refs.Add(dic.Obj.Value, new PdfReference(pos, 0));

            // Return
            return dic.Obj.AsRef();
        }

        /// <summary>
        /// Write document information
        /// 输出文档信息
        /// </summary>
        /// <param name="info">Information</param>
        /// <returns>Task</returns>
        public async Task WriteDocInfoAsync(PdfDocInfo info)
        {
            dicInfoObj = await WriteDicAsync(info);

            // Default font
            language = info.Language;

            var font = PdfFont1.Create();
            fontObj = await WriteDicAsync(font);
        }

        /// <summary>
        /// Start a new page, first page is implicitly created
        /// 开始一个新页面，第一页是隐式创建的
        /// </summary>
        /// <param name="data">Page data</param>
        /// <returns>Task</returns>
        public async Task NewPageAsync(PdfReadonlyPageData? data = null)
        {
            if (currentPage != null)
            {
                await WritePageAsync(currentPage);
            }

            // Default page data
            data ??= new PdfReadonlyPageData();

            // Create a new page
            currentPage = new PdfPage(CreateObj(), pageTree, data);
            await currentPage.PrepareAsync();

            // Default font
            currentPage.Resources.Font.Add("F0", fontObj!);
        }

        /// <summary>
        /// Add paragraph
        /// 添加段落
        /// </summary>
        /// <param name="p">Paragraph</param>
        /// <returns>Task</returns>
        public async Task AddAsync(PdfBlock p)
        {
            if (await currentPage!.WriteAsync(p))
            {
                await NewPageAsync(currentPage.Data);
            }
        }

        private async Task WritePageAsync(PdfPage page)
        {
            await using (page.Stream)
            {
                // Finish writing
                await page.WriteEndAsync();

                // Pdf stream
                var pageStream = new PdfStreamDic(page.Stream, null);
                page.Contents = await WriteDicAsync(pageStream);

                // Dispose
                await page.Stream.DisposeAsync();
            }

            // Write page
            var pageObj = await WriteDicAsync(page);

            // Add to kids
            pageTree.Kids.Add(pageObj);
        }

        private async Task WriteXref()
        {
            await stream.WriteAsync(PdfConstants.XrefBytes);
            stream.WriteByte(PdfConstants.LineFeedByte);

            await stream.WriteAsync(Encoding.ASCII.GetBytes($"0 {refs.Count}"));
            stream.WriteByte(PdfConstants.LineFeedByte);

            foreach (var r in refs)
            {
                await r.Value.WriteToAsync(stream);
            }
        }

        /// <summary>
        /// Async dispose
        /// 异步释放
        /// </summary>
        /// <returns>Task</returns>
        public async ValueTask DisposeAsync()
        {
            // Avoid multiple calls
            if (disposed) return;

            if (currentPage != null)
            {
                await WritePageAsync(currentPage);
            }

            // Write page tree
            await WriteDicAsync(pageTree);

            // catalog / root
            var catalog = new PdfCatalog(pageTree.Obj.AsRef()) { Lang = language };
            var catalogObj = await WriteDicAsync(catalog);

            // startxref
            var startxref = (int)stream.Position;

            // xref
            await WriteXref();

            // trailer & startxref
            var trailer = new PdfTrailer(startxref, refs.Count, catalogObj)
            {
                Info = dicInfoObj
            };
            await trailer.WriteToAsync(stream);

            // End of File
            var eof = new PdfEOF();
            await eof.WriteToAsync(stream);

            // Update flag
            disposed = true;
        }
    }
}
