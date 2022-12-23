using FirebaseAuthentication.Models;
using FirebaseAuthentication.Repository.DataConnection;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace FirebaseAuthentication.Repository.Account
{
	public class AccountRepository : IAccount, IDisposable
	{
		private FirebaseConnect _connect;

		private Firebase.Auth.IFirebaseAuthProvider _authProvider;

		private IFirebaseClient _firebaseClient;

		public AccountRepository()
        {
			_connect = new FirebaseConnect();
			_authProvider = _connect.authProvider;
			_firebaseClient = _connect.firebaseClient;
        }

		public void Dispose()
        {
			this.Dispose();
        }

		public async Task<string> Login(Login login, string returnUrl, IOwinContext owinContext)
        {
			bool isAdmin = false;
			//connect to firebase
			//response returns token
			var fbAuthenticationResponse = await _authProvider.SignInWithEmailAndPasswordAsync(login.Email, login.Password);

			string token = fbAuthenticationResponse.FirebaseToken;
			var user = fbAuthenticationResponse.User;
			//if it's not empty then we know the user has been authenticated
			//authorize once user's been authericated
			if (!String.IsNullOrEmpty(token))
            {
				//setup claims and claims identity
				var claims = new List<Claim>();

				try
				{
					claims.Add(new Claim(ClaimTypes.Email, user.Email));
					claims.Add(new Claim(ClaimTypes.Authentication, token));

					var claimIdentities = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
					var ctx = owinContext;
					var authenticationManager = ctx.Authentication;

					//authentication process
					authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimIdentities);

					isAdmin = this.IsAdmin(login);
					//Authorization
					if (isAdmin == false)
					{
						return "User";
					}
					else
					{
						return "Admin";
					}
				}
                catch 
                {
					return "Authentication login failed.";
                }

				
            }
            else
            {
				return "Authentication token generation failed.";
            }
        }

		private bool IsAdmin(Models.Login login)
        {
			//connection to firebase through firebase client
			FirebaseResponse firebaseResponse = _firebaseClient.Get("AccessRight");
			//deserializing information from response
			dynamic accessRightData = JsonConvert.DeserializeObject<dynamic>(firebaseResponse.Body);
			bool isAdmin = false;
			if(accessRightData != null)
            {
                foreach (var accessRightEmail in accessRightData)
                {
					if(login.Email == accessRightEmail.First.Value.ToString())
                    {
						isAdmin = true;
                    }
                }
            }

			return isAdmin;
        }

		public async Task SignUp(SignUp signUp)
        {
			await _authProvider.CreateUserWithEmailAndPasswordAsync(signUp.Email, signUp.Password, signUp.Name, true);
        }

		public async Task PasswordResetLink(string EmailID)
        {
			await _authProvider.SendPasswordResetEmailAsync(EmailID);
        }
	}
}