using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using BoldReports.Writer;

namespace bold_reports_mvc_report_writer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Export(string writerFormat)
        {
            // Here, we have loaded the sales-order-detail sample report from application the folder Resources.
            FileStream reportStream = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath(@"\App_Data\Resources\sales-order-detail.rdl"), FileMode.Open, FileAccess.Read);
            BoldReports.Writer.ReportWriter writer = new BoldReports.Writer.ReportWriter();

            string fileName = null;
            WriterFormat format;
            string type = null;

            if (writerFormat == "PDF")
            {
                fileName = "sales-order-detail.pdf";
                type = "pdf";
                format = WriterFormat.PDF;
            }
            else if (writerFormat == "Word")
            {
                fileName = "sales-order-detail.docx";
                type = "docx";
                format = WriterFormat.Word;
            }
            else if (writerFormat == "CSV")
            {
                fileName = "sales-order-detail.csv";
                type = "csv";
                format = WriterFormat.CSV;
            }
            else if (writerFormat == "HTML")
            {
                fileName = "sales-order-detail.html";
                type = "html";
                format = WriterFormat.HTML;
            }
            else if (writerFormat == "PPT")
            {
                fileName = "sales-order-detail.ppt";
                type = "ppt";
                format = WriterFormat.PPT;
            }
            else
            {
                fileName = "sales-order-detail.xlsx";
                type = "xlsx";
                format = WriterFormat.Excel;
            }

            writer.LoadReport(reportStream);
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, format);

            // Download the generated export document to the client side.
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/" + type);
            fileStreamResult.FileDownloadName = fileName;
            return fileStreamResult;
        }
    }
}