using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
using System;
using System.Linq;
using WarehouseCore.MVC.ViewModels;
using WarehouseCore.MVC.Models;

namespace WarehouseCore.MVC.Helpers
{
    public class PdfParser
    {
        public ParserVm BookingParser(string filePath)
        {
            ParserVm parser = new ParserVm();

            //tim so PO
            PdfReader poreader = new PdfReader(filePath);
            int pageNumber = 1;
            float x = 0;
            float y = 220;
            float width = 100;
            float height = 120;
            Rectangle rect = new Rectangle(x, y, x + width, y + height);
            RenderFilter[] filter = { new RegionTextRenderFilter(rect) };
            ITextExtractionStrategy strategy = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filter);
            string potext = PdfTextExtractor.GetTextFromPage(poreader, pageNumber, strategy);
            poreader.Close();
            string[] po = potext.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            //parse booking
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
            //tim so consol
            string consol_line = lines.FirstOrDefault(l => l.StartsWith("CONSOL"));
            string consol = consol_line.Replace("CONSOL", "").Trim();

            //tim so shipment
            string shipment_line = lines.FirstOrDefault(l => l.StartsWith("SHIPMENT"));
            string shipment = shipment_line.Replace("SHIPMENT", "").Trim();

            //tim destination
            string destination_line = lines.FirstOrDefault(l => l.Contains("VNHPH"));
            int vnIndex = destination_line.IndexOf("VNHPH");
            destination_line = destination_line.Substring(vnIndex);
            string[] parts = destination_line.Split(' ');
            string destination = parts[1];

            //tim shipper
            string shipper_line = lines.FirstOrDefault(l => l.StartsWith("SHIPPER"));
            string shipper = shipper_line.Replace("SHIPPER", "").Trim();
            string shipment_consignee_line = lines.FirstOrDefault(l => l.StartsWith("SHIPPER CONSIGNEE"));
            int shipment_consignee_line_index = Array.IndexOf(lines, shipment_consignee_line);
            string consignee_line = lines[shipment_consignee_line_index + 1];
            string consignee = consignee_line.Replace(booking_confirmation, "").Trim();
            string seal_line = lines.FirstOrDefault(l => l.StartsWith("SEAL:"));
            int seal_line_index = Array.IndexOf(lines, seal_line);

            //gan du lieu Bookings
            parser.booking.Shipment = shipment;
            parser.booking.Consol = "";
            parser.booking.Shipper = shipper;
            parser.booking.Consignee = consignee;
            parser.booking.Destination = destination;

            //gan du lieu POs
            foreach (string pos in po)
            {
                POs poi = new POs();
                poi.POSO = pos;
                parser.posList.Add(poi);
            }

            return parser;
        }
    }
}