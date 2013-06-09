using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PublicWebSms.Models;

namespace PublicWebSms.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReportAPI(ReportData reportData, SMSAPIInput apiData)
        {

            return Json(null);
        }

    }
}
