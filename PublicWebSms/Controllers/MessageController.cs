using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PublicWebSms.Controllers
{
    [PwsAuthorize]
    public class MessageController : Controller
    {
        private PwsDbContext db = new PwsDbContext();
        //
        // GET: /Message/

        public ActionResult Index()
        {
            return Redirect("~/Message/Outbox");
        }

        /*
         * Outbox: menampilkan daftar SMS yang sedang mengantri dalam proses pengiriman
         */
        public ActionResult Outbox(int success = 0)
        {
            string loggedUserName = UserSession.GetLoggedUserName();

            var dataSMS = (from smsUser in db.SMSUser where smsUser.UserName == loggedUserName && smsUser.SMS.Draft == false select smsUser.SMS).ToList();

            // AJAX Request: Tampilkan data dalam bentuk JSON
            // Web Request: Tampilkan seluruh halaman dalam bentuk tabel

            if (Request.IsAjaxRequest())
            {
                return Json(dataSMS);
            }

            ViewBag.Success = success;
            return View(dataSMS);
        }

        /*
         * Draft: menampilkan daftar SMS yang disimpan
         */
        public ActionResult Draft(int success = 0)
        {
            string loggedUserName = UserSession.GetLoggedUserName();

            var dataDraft = (from draftUser in db.DraftUser where draftUser.UserName == loggedUserName select draftUser.Draft).ToList();

            if (Request.IsAjaxRequest())
            {
                return Json(dataDraft);
            }

            ViewBag.Success = success;
            return View(dataDraft);
        }

        /*
         * Compose: menampilkan borang pembuatan pesan atau pengeditan pesan Draft
         */
        public ActionResult Compose(int smsId = -1, string destinationNumber = "", int isDraft = 0)
        {
            string loggedUserName = UserSession.GetLoggedUserName();
            ViewBag.Scheduled = false;
            if (smsId > 0 && isDraft == 0)
            {
                var smsUserData = db.SMSUser.SingleOrDefault(x => x.SMS.SmsId == smsId && x.UserName == loggedUserName);
                var smsData = smsUserData.SMS;
                ViewBag.DestinationNumber = smsData.DestinationNumber;
                ViewBag.MessageContent = smsData.Content;
                if (smsData.Scheduled)
                {
                    ViewBag.ScheduleCheck = "checked";
                }
                else
                {
                    ViewBag.ScheduleCheck = "";
                }
                ViewBag.Scheduled = smsData.Scheduled;
                ViewBag.ScheduleTime = smsData.ScheduleTime;
                return View(smsData);
            }
            else if (isDraft == 1)
            {
                var draftUserData = db.DraftUser.SingleOrDefault(x => x.Draft.DraftId == smsId && x.UserName == loggedUserName);
                var draftData = draftUserData.Draft;
                ViewBag.DestinationNumber = draftData.DestinationNumber;
                ViewBag.MessageContent = draftData.Content;
                if (draftData.Scheduled)
                {
                    ViewBag.ScheduleCheck = "checked='checked'";
                }
                else
                {
                    ViewBag.ScheduleCheck = "checked='checked'";
                }
                ViewBag.Scheduled = draftData.Scheduled;
                ViewBag.ScheduleTime = draftData.ScheduleTime;
                return View(draftData);
            }
            return View();
        }

        public ActionResult Process(SMS smsInput, int smsAction)
        {
            if (smsAction == 1)
            {
                Send(smsInput);
                return Redirect("~/Message/Outbox?success=1");
            }
            else
            {
                Draft draft = new Draft {
                    Content = smsInput.Content, DestinationNumber = smsInput.DestinationNumber,
                    Scheduled = smsInput.Scheduled
                };

                if (draft.Scheduled) draft.ScheduleTime = smsInput.ScheduleTime;
                else draft.ScheduleTime = DateTime.Now;
                SaveDraft(draft);
                return Redirect("~/Message/Draft?success=1");
            }
        }

        [HttpPost]
        public ActionResult SaveDraft(Draft draft)
        {
            bool sukses = false;
//            draft.ScheduleTime = DateTime.Parse(draft.ScheduleTime);

            // Dikomentari sementara, nanti masalah nilai bawaan harus diatur di model, bukan di kode ini (seperti tiga baris diatas itu)
            db.Drafts.Add(draft);
            db.SaveChanges();

            DraftUser draftUser = new DraftUser
            {
                UserName = UserSession.GetLoggedUserName(),
                DraftId = draft.DraftId
            };

            db.DraftUser.Add(draftUser);
            db.SaveChanges();

            sukses = true;

            if (!Request.IsAjaxRequest())
            {
                return Redirect("~/Message/Draft?success=1");
            }
            else
            {
                return Json(sukses);
            }
        }

        [HttpGet]
        public ActionResult Send(int smsId, int sendDraft = 0)
        {
            string loggedUserName = UserSession.GetLoggedUserName();
            SMS smsData = null;
            if (sendDraft == 1)
            {
                DraftUser draftUser = db.DraftUser.SingleOrDefault(x => x.DraftId == smsId && 
                    x.UserName == loggedUserName);
                smsData = new SMS
                {
                    Content = draftUser.Draft.Content, DestinationNumber = draftUser.Draft.DestinationNumber,
                    ScheduleTime = draftUser.Draft.ScheduleTime, Scheduled = draftUser.Draft.Scheduled, Draft = false,
                    Sent = false, TimeStamp = DateTime.Now
                };
                db.Drafts.Remove(draftUser.Draft);
                db.DraftUser.Remove(draftUser);
            }
            else
            {
                SMSUser smsUser = db.SMSUser.SingleOrDefault(x => x.SMSId == smsId && x.UserName == loggedUserName);
                smsData = smsUser.SMS;
                db.SMSes.Remove(smsUser.SMS);
                db.SMSUser.Remove(smsUser);
            }

            smsData.Draft = false;
            return Send(smsData);
        }

        [HttpPost]
        public ActionResult Send(SMS smsInput)
        {
            bool sukses = false;
            smsInput.TimeStamp = DateTime.Now;
            smsInput.Sent = false;
            smsInput.Draft = false;

            var context = new ValidationContext(smsInput, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(smsInput, context, results);


            // Dikomentari sementara, nanti masalah nilai bawaan harus diatur di model, bukan di kode ini (seperti tiga baris diatas itu)
            if (ModelState.IsValid && isValid)
            {
                db.SMSes.Add(smsInput);
                db.SaveChanges();

                SMSUser smsUser = new SMSUser
                {
                    UserName = UserSession.GetLoggedUserName(),
                    SMSId = smsInput.SmsId
                };

                db.SMSUser.Add(smsUser);
                db.SaveChanges();

                sukses = true;

                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/Message/Outbox?sukses=1");
                }
                else
                {
                    return Json(sukses);
                }

            }
            else
            {
                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/Message/Outbox?sukses=0");
                }
                else
                {
                    return Json(sukses);
                }
            }
        }

        public ActionResult ScheduleSMS(SMS smsInput)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SMSAPI(SMSAPIInput input)
        {
            // API Secret Code Checking
            // HARDCODED!

            if (input.APIId == "hanahbanana" && input.APISecretCode == "segogoreng")
            {
                // Ambil data SMS yang siap kirim
                // SMS yang siap kirim sementara, atau yang terjadwal saat ini selisih 5 menit (untuk jaga-jaga gituch)
                DateTime currentTime = DateTime.Now;

                var dataSMS = (from sms in db.SMSes where (sms.Sent == false && sms.Scheduled == false) || (sms.Sent == false && (sms.Scheduled == true && sms.ScheduleTime <= currentTime)) select sms );

                var jsonSMS = from sms in dataSMS.ToList() select new Dictionary<string, string> { { "Dest", sms.DestinationNumber.ToString() }, { "Msg", sms.Content.ToString() } };

                foreach (SMS sms in dataSMS)
                {
                    sms.Sent = true;
                }

                db.SaveChanges();

                // Untuk tiap SMS, tandai sms.Sent menjadi true dan kirimkan dalam bentuk JSON
                
                return Json(jsonSMS, JsonRequestBehavior.AllowGet);
            }

            return View();
        }
    }
}
