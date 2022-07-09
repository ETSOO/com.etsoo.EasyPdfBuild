using com.etsoo.EasyPdf.Types;

namespace com.etsoo.EasyPdf.Objects
{
    public class PdfDocInfo : PdfObjectDic
    {
        public override string Type => "";

        public string? Title { get; set; }

        public string? Author { get; set; }

        public string? Subject { get; set; }

        public string? Keywords { get; set; }

        public string? Creator { get; set; }

        public string Producer { get; }

        public DateTime CreationDate { get; set; }

        public DateTime? ModDate { get; set; }

        public string? Language { get; set; }

        public PdfDocInfo() : base()
        {
            Producer = "Etsoo EasyPdf Renderer";
            CreationDate = DateTime.Now;
        }

        private PdfDocInfo(PdfObject obj, PdfDictionary dic) : base(obj, dic)
        {
            Title = dic.GetValue<string>(nameof(Title));
            Author = dic.GetValue<string>(nameof(Author));
            Subject = dic.GetValue<string>(nameof(Subject));
            Keywords = dic.GetValue<string>(nameof(Keywords));
            Creator = dic.GetValue<string>(nameof(Creator));
            Producer = dic.GetValue<string>(nameof(Producer)) ?? "Unknown";
            CreationDate = dic.GetValue<DateTime?>(nameof(CreationDate)) ?? DateTime.Now;
            ModDate = dic.GetValue<DateTime?>(nameof(ModDate));
        }

        protected override void AddItems()
        {
            base.AddItems();

            Dic.AddNameItem(nameof(Producer), new PdfString(Producer));
            Dic.AddNameItem(nameof(CreationDate), new PdfDateTime(CreationDate));

            Dic.AddNameBinary(nameof(Title), Title);
            Dic.AddNameBinary(nameof(Author), Author);
            Dic.AddNameBinary(nameof(Subject), Subject);
            Dic.AddNameBinary(nameof(Keywords), Keywords);
            Dic.AddNameBinary(nameof(Creator), Creator);
            Dic.AddNameDate(nameof(ModDate), ModDate);
        }
    }
}
