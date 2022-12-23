
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ukhalelani_Helpdesk.Models;

namespace FirebaseAuthentication.Controllers
{
    public class LodgeComplaintController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "tS0NMWIldMwyK6mEiuOLFjgZzEh1dSlYgSJSXqAB",
            BasePath = "https://ukhalelanihelpdesk-default-rtdb.firebaseio.com"
        };
        IFirebaseClient client;
        // GET: Feedback
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Complaints");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<ComplaintModel>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<ComplaintModel>(((JProperty)item).Value.ToString()));
            }
            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Create(ComplaintModel complaint)
        {
            try
            {
                AddComplaintToFirebase(complaint);
                ModelState.AddModelError(string.Empty, "Added Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction("SuccessfullyLodge", "LodgeComplaint");
        }

        private void AddComplaintToFirebase(ComplaintModel complaint)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = complaint;
            PushResponse response = client.Push("Complaints/", data);
            data.complaintID = response.Result.name;
            SetResponse setResponse = client.Set("Complaints/" + data.complaintID, data);
        }


        [HttpGet]
        public ActionResult Detail(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Complaints/" + id);
            ComplaintModel data = JsonConvert.DeserializeObject<ComplaintModel>(response.Body);
            return View(data);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Complaints/" + id);
            ComplaintModel data = JsonConvert.DeserializeObject<ComplaintModel>(response.Body);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(ComplaintModel complaint)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Complaints/" + complaint.complaintID, complaint);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(ComplaintModel complaint)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Complaints/" + complaint.complaintID, complaint);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Complaints/" + id);

            return RedirectToAction("Index");
        }

        public ActionResult SuccessfullyLodge(string id)
        {
            return View();
        }
    }
}

