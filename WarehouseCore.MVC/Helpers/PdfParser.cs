using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Linq;
using WarehouseCore.MVC.ViewModels;

namespace WarehouseCore.MVC.Helpers
{
    public class PdfParser
    {
        public void BookingParser(string filePath)
        {
            ParserVm parser = new ParserVm();

            PdfReader reader = new PdfReader(filePath);

            int numPages = reader.NumberOfPages;

            string booking = "";

            for (int pageNum = 1; pageNum <= numPages; pageNum++)
            {
                string text = PdfTextExtractor.GetTextFromPage(reader, pageNum);
                booking = booking + text;
            }
            string[] lines = booking.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            string booking_confirmation_line = lines.FirstOrDefault(l => l.StartsWith("Booking"));

            int booking_line_index = Array.IndexOf(lines, booking_confirmation_line);

            string booking_confirmation = lines[booking_line_index + 1];

            string shipment_line = lines.FirstOrDefault(l => l.StartsWith("SHIPMENT"));

            string shipment = shipment_line.Replace("SHIPMENT","").Trim();

            string shipper_line = lines.FirstOrDefault(l => l.StartsWith("SHIPPER"));

            string shipment_consignee_line = lines.FirstOrDefault(l => l.StartsWith("SHIPPER CONSIGNEE"));

            int shipment_consignee_line_index = Array.IndexOf(lines, shipment_consignee_line);

            string consignee_line = lines[shipment_consignee_line_index + 1];

            string consignee = consignee_line.Replace(booking_confirmation, "").Trim();

            string seal_line = lines.FirstOrDefault(l => l.StartsWith("SEAL:"));

            int seal_line_index = Array.IndexOf(lines, seal_line);
        }
    }
}