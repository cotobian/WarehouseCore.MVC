﻿using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
using System;
using System.Linq;
using WarehouseCore.MVC.ViewModels;
using WarehouseCore.MVC.Models;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace WarehouseCore.MVC.Helpers
{
    public class PdfParser
    {
        public ParserVm BookingParser(HttpPostedFileBase file)
        {
            ParserVm parser = new ParserVm();
            byte[] pdfbytes = null;
            BinaryReader rdr = new BinaryReader(file.InputStream);
            pdfbytes = rdr.ReadBytes((int)file.ContentLength);
            PdfReader poreader = new PdfReader(pdfbytes);
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
            PdfReader reader = new PdfReader(pdfbytes);
            //PdfReader reader = new PdfReader(filePath);
            int numPages = reader.NumberOfPages;
            string booking = "";
            for (int pageNum = 1; pageNum <= numPages; pageNum++)
            {
                string text = PdfTextExtractor.GetTextFromPage(reader, pageNum);
                booking = booking + text;
            }
            string[] lines = booking.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

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
            string routing_line = lines.FirstOrDefault(l => l.StartsWith("ROUTING INFORMATION"));
            int destination_line_index = Array.IndexOf(lines, routing_line);
            string destination_line = lines[destination_line_index + 2];
            int vnIndex = destination_line.IndexOf("VNHPH");
            destination_line = destination_line.Substring(vnIndex);
            string[] parts = destination_line.Split(' ');
            string destination = parts[1];

            //tim shipper
            string shipper_line = lines.FirstOrDefault(l => l.StartsWith("SHIPPER"));
            string shipper = booking_confirmation;
            string shipment_consignee_line = lines.FirstOrDefault(l => l.StartsWith("SHIPPER CONSIGNEE"));
            int shipment_consignee_line_index = Array.IndexOf(lines, shipment_consignee_line);
            string consignee_line = lines[shipment_consignee_line_index + 1];
            string consignee = consignee_line.Replace(booking_confirmation, "").Trim();
            string seal_line = lines.FirstOrDefault(l => l.StartsWith("SEAL:"));
            int seal_line_index = Array.IndexOf(lines, seal_line);

            string bill_of_lading_line = lines.FirstOrDefault(l => l.StartsWith("OCEAN BILL OF LADING"));
            int bill_of_lading_line_index = Array.IndexOf(lines, bill_of_lading_line);
            string dimension_line = lines[bill_of_lading_line_index + 1];
            string[] dimensions_parts = dimension_line.Split(' ');
            int dimensions_len = dimensions_parts.Length;
            string Unit = dimensions_parts[dimensions_len - 1].ToString();
            int Quantity = int.Parse(dimensions_parts[dimensions_len - 2].ToString());
            decimal CBM = decimal.Parse(dimensions_parts[dimensions_len - 4].ToString());
            decimal GWeight = decimal.Parse(dimensions_parts[dimensions_len - 8].ToString());
            string dimension = dimensions_parts[dimensions_len - 5].ToString();

            //gan du lieu Bookings
            parser.booking = new Booking();
            parser.posList = new List<POs>();
            parser.booking.Shipment = shipment;
            parser.booking.Consol = consol;
            parser.booking.Shipper = shipper;
            parser.booking.Consignee = consignee;
            parser.booking.Destination = destination;
            parser.booking.Status = 0;

            //gan du lieu POs
            foreach (string pos in po)
            {
                if (pos != "SHIPPER'S REFERENCE")
                {
                    POs poi = new POs();
                    poi.POSO = pos;
                    poi.Unit = Unit;
                    poi.Quantity = Quantity;
                    poi.Status = 0;
                    poi.CBM = CBM;
                    poi.GWeight = GWeight;
                    poi.Dimension = dimension;
                    parser.posList.Add(poi);
                }
            }

            return parser;
        }
    }
}