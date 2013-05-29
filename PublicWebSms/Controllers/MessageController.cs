using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    public class MessageController : Controller
    {
        //
        // GET: /Message/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Outbox()
        {
            return View();
        }

        public ActionResult Compose()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Send(SMS smsInput)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveDraft(SMS smsInput)
        {
            return View();
        }

        public ActionResult ScheduleSMS(SMS smsInput)
        {
            return View();
        }



    }
}
