using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa
{
    public static class Nota
    {
        private static string caminho = Environment.CurrentDirectory + "\\dll\\Nota Promissória.pdf";

        public static string Caminho
        {
            get { return caminho; }
        }

        public static bool ExisteNota
        {
            get
            {
                return File.Exists(caminho);
            }
        }

        public static void AbrirNota()
        {
            Process.Start(caminho);
        }

        public static void GerarNota()
        {
            Document documento = new Document(PageSize.A4);
            documento.SetMargins(30, 30, 30, 30);
            var writer = PdfWriter.GetInstance(documento, new FileStream(caminho, FileMode.Create));
            documento.Open();
            PrimeiraCamada(documento);
            SegundaCamada(documento, writer);
            documento.Close();
        }

        public static void PrimeiraCamada(Document documento)
        {
            PdfPTable table = new PdfPTable(2);
            table.TableEvent = new RoundedEvent();
            var widths = new float[] { 2f, 8f };
            table.SetWidths(widths);
            var cell = new PdfPCell(new Phrase("AVALISTA(S)"));
            var fill = new PdfPCell(new Phrase(Fill(documento)));
            fill.BorderWidthBottom = 0.5f;
            fill.BorderWidthLeft = 0;
            fill.BorderWidthTop = 0;
            fill.PaddingBottom = 10;
            fill.SetLeading(20, 0);
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BorderWidthBottom = 0;
            table.AddCell(cell);
            for (int i = 0; i < 2; i++)
            {
                cell = new PdfPCell(new Phrase("Nome"));
                cell.BorderWidthTop = 0;
                cell.BorderWidthBottom = 0.5f;
                cell.BorderWidthRight = 0;
                cell.PaddingBottom = 10;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                table.AddCell(fill);
                cell.Phrase = new Phrase("CPF/CPNJ");
                table.AddCell(cell);
                table.AddCell(fill);
            }
            documento.Add(table);
        }

        public static void SegundaCamada(Document documento, PdfWriter writer)
        {
            var table = new PdfPTable(1);
            table.TableEvent = new RoundedEvent();
            table.TotalWidth = 72;
            table.PaddingTop = 20;
            var cell = new PdfPCell(new Phrase("Vencimento ________ de _________________ de ________"));
            cell.Colspan = 7;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Rotation = 270;
            table.AddCell(cell);
            cell.Colspan = 1;
            cell.Phrase = new Phrase("Nº ");
            table.AddCell(cell);
            PdfTemplate template = writer.DirectContent.CreateTemplate(120, 80);
            template.SetColorFill(BaseColor.LIGHT_GRAY);
            template.Rectangle(0, 0, 120, 80);
            template.Fill();
            writer.ReleaseTemplate(template);
            table.AddCell(Image.GetInstance(template));
            cell.Phrase = new Phrase("");
            table.AddCell(cell);
            cell.Phrase = new Phrase("R$");
            table.AddCell(cell);
            table.AddCell(Image.GetInstance(template));
            table.WriteSelectedRows(0, -1, 0, 0, writer.DirectContent);
            documento.Add(table);
        }

        public static string Fill(Document doc)
        {
            float n = doc.GetRight(65)/5;
            string s = "";
            for (int i = 0; i < n; i++)
            {
                s += "_";
            }
            return s;
        }

    }

    public class RoundedEvent : IPdfPTableEvent
    {
        public void TableLayout(PdfPTable table, float[][] widths, float[] heights, int headerRows, int rowStart, PdfContentByte[] canvases)
        {
            PdfContentByte background = canvases[PdfPTable.BASECANVAS];
            background.SaveState();
            background.SetCMYKColorFill(0,0,40,0);
            background.RoundRectangle(widths[0][0], heights.Last(),
                widths[0][1] - widths[0][0], heights[0] - heights.Last(), 4);
            background.Fill();
            background.RestoreState();
        }
    }

}
