using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /Contact/

        public ActionResult Index(int groupId = -1)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Contact newContact)
        {
            return View();
        }

        public ActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGroup(Group newGroup)
        {
            return View();
        }

        public ActionResult ShowContact(int idKontak)
        {
            return View();
        }

        public ActionResult ShowGroup(int idGroup)
        {
            return View();
        }




    }
}
