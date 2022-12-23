using Firebase.Auth;
using FireSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirebaseAuthentication.Repository.DataConnection
{
    public class FirebaseConnect : IDisposable
    {
        public IFirebaseClient firebaseClient;
        public IFirebaseAuthProvider authProvider;

        public FirebaseConnect()
        {
            IFirebaseConfig config = new FireSharp.Config.FirebaseConfig() 
            { 
                AuthSecret = FirebaseConstants.AuthorizationSecret,
                BasePath = FirebaseConstants.FirebaseDatabaseAddress
            };

            firebaseClient = new FireSharp.FirebaseClient(config);

            //exposes methods that we'll use to send emails specific to firebase and authentication
            authProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(FirebaseConstants.Web_ApiKey));

        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}