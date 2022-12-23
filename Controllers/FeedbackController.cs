using Firebase.Auth;
using FirebaseAuthentication.Models;
using FirebaseAuthentication.Repository.DataConnection;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FirebaseAuthentication.Controllers
{
    public class FeedbackController : Controller
    {
        public IFirebaseClient firebaseClient;
        public IFirebaseAuthProvider authProvider;

        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig()
        {
            AuthSecret = FirebaseConstants.AuthorizationSecret,
            BasePath = FirebaseConstants.FirebaseDatabaseAddress
        };

        IFirebaseClient client;
        // GET: Feedback
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Feedbacks");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Feedback>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Feedback>(((JProperty)item).Value.ToString()));
            }
            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Create(Feedback feedback)
        {
            try
            {
                AddFeedbackToFirebase(feedback);
                ModelState.AddModelError(string.Empty, "Added Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction("Index", "Feedback");
        }

        private void AddFeedbackToFirebase(Feedback feedback)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = feedback;
            PushResponse response = client.Push("Feedbacks/", data);
            data.feedbackID = response.Result.name;
            SetResponse setResponse = client.Set("Feedbacks/" + data.feedbackID, data);
        }
    }
}
