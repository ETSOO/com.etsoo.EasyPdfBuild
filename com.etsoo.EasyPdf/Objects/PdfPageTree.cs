using com.etsoo.EasyPdf.Dto;
using com.etsoo.EasyPdf.Types;

namespace com.etsoo.EasyPdf.Objects
{
    internal class PdfPageTree : PdfObjectDic
    {
        /// <summary>
        /// Mediabox field name
        /// </summary>
        public const string MediaBoxField = "MediaBox";

        public override string Type => "Pages";

        public override PdfObject Obj { get => base.Obj!; }

        /// <summary>
        /// The immediate parent
        /// </summary>
        public PdfObject? Parent { get; set; }

        /// <summary>
        /// An array of indirect references to the immediate children of this node
        /// </summary>
        public List<PdfObject> Kids { get; }

        /// <summary>
        /// The number of leaf nodes (page objects)
        /// </summary>
        public int Count => Kids.Count;

        /// <summary>
        /// Shared page data
        /// 共享的页数据
        /// </summary>
        public PdfPageData PageData { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="obj">Obj</param>
        /// <param name="pageData">Shared page data</param>
        public PdfPageTree(PdfObject obj, PdfPageData pageData) : base(obj)
        {
            Obj = obj;
            PageData = pageData;
            Kids = new List<PdfObject>();
        }

        public PdfPageTree(PdfObject obj, PdfDictionary dic) : base(obj, dic)
        {
            Parent = dic.Get<PdfObject>(nameof(Parent));

            var kids = dic.GetRequired<PdfArray>(nameof(Kids)).Value.Select(k => (k as PdfObject)!);
            Kids = new List<PdfObject>(kids);

            var mediaBoxArray = dic.Get<PdfArray>(MediaBoxField);
            var MediaBox = mediaBoxArray.ToRectangle();
            PageData = new PdfPageData { PageSize = MediaBox };
        }

        protected override void AddItems()
        {
            base.AddItems();

            Dic.AddNameItem(nameof(Parent), Parent);
            Dic.AddNameArray(nameof(Kids), Kids.ToArray());
            Dic.AddNameInt(nameof(Count), Count);
            Dic.AddNameRect(MediaBoxField, PageData.PageSize);
        }
    }
}
