using System.Text;

namespace Ceng382_25_26_202311031.Services
{
    public class SimplePdfService
    {
        public byte[] CreatePdf(string title, List<string> lines)
        {
            var content = new StringBuilder();
            content.AppendLine("BT");
            content.AppendLine("/F1 18 Tf");
            content.AppendLine("50 780 Td");
            content.AppendLine($"({Escape(title)}) Tj");
            content.AppendLine("/F1 11 Tf");

            int yMove = 30;

            foreach (var line in lines)
            {
                content.AppendLine($"0 -{yMove} Td");
                content.AppendLine($"({Escape(line)}) Tj");
                yMove = 18;
            }

            content.AppendLine("ET");

            var streamContent = content.ToString();
            var streamBytes = Encoding.ASCII.GetBytes(streamContent);

            var pdf = new StringBuilder();

            pdf.AppendLine("%PDF-1.4");

            var offsets = new List<int>();

            void AddObject(string obj)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
                pdf.Append(obj);
            }

            AddObject("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

            AddObject("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");

            AddObject("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>\nendobj\n");

            AddObject("4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n");

            AddObject($"5 0 obj\n<< /Length {streamBytes.Length} >>\nstream\n{streamContent}\nendstream\nendobj\n");

            var xrefPosition = Encoding.ASCII.GetByteCount(pdf.ToString());

            pdf.AppendLine("xref");
            pdf.AppendLine("0 6");
            pdf.AppendLine("0000000000 65535 f ");

            foreach (var offset in offsets)
            {
                pdf.AppendLine($"{offset:D10} 00000 n ");
            }

            pdf.AppendLine("trailer");
            pdf.AppendLine("<< /Size 6 /Root 1 0 R >>");
            pdf.AppendLine("startxref");
            pdf.AppendLine(xrefPosition.ToString());
            pdf.AppendLine("%%EOF");

            return Encoding.ASCII.GetBytes(pdf.ToString());
        }

        private string Escape(string text)
        {
            return text
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("₺", "TL");
        }
    }
}