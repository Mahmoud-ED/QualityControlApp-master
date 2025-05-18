using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class ReportsController : Controller
{




    public IActionResult Index()
    {
        return View();
    }


    public IActionResult Print()
    {
        return View();
    }










    //private readonly IWebHostEnvironment _webHostEnvironment;

    //public ReportsController(IWebHostEnvironment webHostEnvironment)
    //{
    //    _webHostEnvironment = webHostEnvironment;
    //}

    //public IActionResult ReportViewer()
    //{
    //    return View();
    //}

    //public IActionResult GenerateReport()
    //{
    //    // الحصول على المسار الجذري للمشروع
    //    var reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "Report1.rpt");

    //    // تحميل التقرير
    //    ReportDocument reportDoc = new ReportDocument();
    //    reportDoc.Load(reportPath);

    //    // إعداد الاتصال بقاعدة البيانات
    //    reportDoc.SetDatabaseLogon("AppUser", "111", "DESKTOP-8KCBVKR", "QualityDB");

    //    // تصدير التقرير إلى PDF
    //    Stream reportStream = reportDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
    //    reportStream.Seek(0, SeekOrigin.Begin);

    //    return File(reportStream, "application/pdf", "Report.pdf");
    //}
}