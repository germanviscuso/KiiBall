using UnityEngine;
using System.Collections;
using KiiCorp.Cloud.Storage;
using System;
using System.Collections.Generic;

public class Authentication : MonoBehaviour {

	private Texture2D facebookButton;
	private static List<object> friends = null;
	private static Dictionary<string, string> profile = null;
	private static bool initialized = false;
	private bool onKiiCallback = false;
	
	void Awake(){
		// Initialize FB SDK 
		if(!FB.IsLoggedIn)
			OnHideUnity (false);
		enabled = false;
		if(!initialized){
			FB.Init(SetInit);
			initialized = true;
		}
	}

	void Start(){
		facebookButton = (Texture2D)Resources.Load("login_with_facebook");
	}

	private void SetInit()                                                                       
	{                                                                                            
		Util.Log("SetInit");                                                                  
		enabled = true; // "enabled" is a property inherited from MonoBehaviour                  
		if (FB.IsLoggedIn)                                                                       
		{                                                                                        
			Util.Log("Already FB logged in");                                                    
			OnLoggedIn();                                                                        
		}                                                                                        
	}                                                                                            
	
	private void OnHideUnity(bool isGameShown)                                                   
	{                                                                                            
		Util.Log("OnHideUnity");                                                              
		if (!isGameShown)                                                                        
		{                                                                                        
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		}                                                                                        
		else                                                                                     
		{                                                                                        
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}                                                                                        
	}  

	void LoginCallback(FBResult result)
	{
		Util.Log("FB LoginCallback");
		
		if (FB.IsLoggedIn)
		{
			OnLoggedIn();
		}
	}

	void OnLoggedIn()
	{
		Util.Log("FB Logged in. ID: " + FB.UserId);
		LoginToKiiViaFacebook();
		// Reqest player info and profile picture                                                                           
		FB.API("/me?fields=id,name,username,age_range,gender,devices,first_name,last_name,locale,birthday,location,email,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);
		//LoadPicture(Util.GetPictureURL("me", 128, 128),MyPictureCallback);    
	}

	void LoginToKiiViaFacebook ()
	{
		Util.Log("FB Logged in. Access Token: " + FB.AccessToken);
		KiiUser user = null;
		onKiiCallback = true;
		KiiUser.LoginWithFacebookToken(FB.AccessToken, (KiiUser user2, Exception e) => {
			if (e == null) {
				Debug.Log ("Kii Login completed");
				user = user2;
				onKiiCallback = false;
				Util.Log("Kii Logged in. URI: " + KiiUser.CurrentUser.Uri.ToString());
				Util.Log("Kii Logged in. Username: " + KiiUser.CurrentUser.Username);
				// Now you have a logged in Kii user via Facebook ( -> KiiUser.CurrentUser)
				// And you can update the user attributes from Facebook data
				/*user.Username = FB.UserId;
				if(profile != null){
					//KiiUser.ChangeEmail(facebookEmail); 
					//KiiUser.ChangePhone(facebookPhone);
					user.Displayname = profile["first_name"];
				}
				user.Update();
				Util.Log("Current user email is: " + KiiUser.CurrentUser.Email);
				Util.Log("Current user name is: " + KiiUser.CurrentUser.Displayname);
				Util.Log("Current user phone is: " + KiiUser.CurrentUser.Phone);*/
			} else {
				user = null;
				onKiiCallback = false;
				Debug.Log ("Kii Login failed: " + e.ToString());
				Util.LogError(e.InnerException.ToString());
				return;
			}
		});
		OnHideUnity (true);
	}

	void OnGUI()
	{	
		if (!FB.IsLoggedIn)
		{
			GUI.backgroundColor = new Color(0,0,0,0);
			Rect rect = new Rect(0 , 50, 256, 64);
			if (GUI.Button(rect, facebookButton)){
				FB.Login("basic_info,email,publish_actions", LoginCallback);
			}
		}
	}

	void APICallback(FBResult result)                                                                                              
	{                                                                                                                              
		Util.Log("FB APICallback");                                                                                                
		if (result.Error != null)                                                                                                  
		{                                                                                                                          
			Util.LogError(result.Error);                                                                                           
			// Let's just try again             
			FB.API("/me?fields=id,name,username,age_range,gender,devices,first_name,last_name,locale,birthday,location,email,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);     
			return;                                                                                                                
		}                                                                                                                          
		//Util.Log(result.Text);
		profile = Util.DeserializeJSONProfile(result.Text);                                                                                                                                              
		friends = Util.DeserializeJSONFriends(result.Text);  
		Util.Log("Got FB profile and friends");
		if(profile.ContainsKey("first_name"))
			Util.Log ("Welcome " + profile["first_name"]);
		Util.Log("Profile keys:");
		foreach (string key in profile.Keys)
		{
			Util.Log(key);
		}
		Util.Log(friends.Count.ToString() + " friends retrieved");
	}                                                                                                                              

}
