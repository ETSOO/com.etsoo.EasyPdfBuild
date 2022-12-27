using com.etsoo.EasyPdf.Types;

namespace com.etsoo.EasyPdf.Objects
{
    /// <summary>
    /// Procedures
    /// Beginning with PDF 1.4, this feature is considered obsolete
    /// </summary>
    public enum PdfPageResourceProcedure
    {
        PDF,
        Text,
        ImageB,
        ImageC,
        ImageI
    }

    /// <summary>
    /// PDF page resource
    /// PDF 页面资源
    /// </summary>
    internal record PdfPageResource : PdfDictionary
    {
        public List<PdfPageResourceProcedure> ProcSet { get; } = new() { PdfPageResourceProcedure.PDF };

        public Dictionary<string, PdfObject> XObject { get; } = new Dictionary<string, PdfObject>();

        public Dictionary<string, PdfObject> Font { get; } = new Dictionary<string, PdfObject>();

        public override Task WriteToAsync(Stream stream)
        {
            if (XObject.Any())
            {
                AddNameDic(nameof(XObject), XObject);
                ProcSet.AddRange(new[] { PdfPageResourceProcedure.ImageB, PdfPageResourceProcedure.ImageC, PdfPageResourceProcedure.ImageI });
            }

            if (Font.Any())
            {
                ProcSet.Add(PdfPageResourceProcedure.Text);
                AddNameDic(nameof(Font), Font);
            }

            AddNameArray(nameof(ProcSet), ProcSet.Select(p => new PdfName(p.ToString())));

            return base.WriteToAsync(stream);
        }
    }
}
