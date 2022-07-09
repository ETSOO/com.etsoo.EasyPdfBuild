using System.Text;

namespace com.etsoo.EasyPdf.Types
{
    /// <summary>
    /// PDF real, local decimal applied
    /// PDF 实数，本地用 decimal 实现
    /// </summary>
    internal record PdfReal(decimal Value) : IPdfType<decimal>
    {
        public bool KeyEquals(string item)
        {
            return false;
        }

        public async Task WriteToAsync(Stream stream)
        {
            var bytes = Encoding.ASCII.GetBytes(Value.ToString("0.0############################"));
            await stream.WriteAsync(bytes);
        }
    }
}
