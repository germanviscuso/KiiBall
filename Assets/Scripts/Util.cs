using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.MiniJSON;

public class Util : ScriptableObject {

	public static void Log (string message)
	{
		Debug.Log(message);
		if (Application.isWebPlayer)
			JavascriptLog(message);
	}
	public static void LogError (string message)
	{
		Debug.LogError(message);
		if (Application.isWebPlayer)
			JavascriptLog(message);
	}

	private static void JavascriptLog(string msg)
	{
		Application.ExternalCall("console.log", msg);
	}

	public static Dictionary<string, string> DeserializeJSONProfile(string response)
	{
		var responseObject = Json.Deserialize(response) as Dictionary<string, object>;
		object nameH;
		var profile = new Dictionary<string, string>();
		if (responseObject.TryGetValue("first_name", out nameH))
		{
			profile["first_name"] = (string)nameH;
		}
		return profile;
	}

	public static List<object> DeserializeJSONFriends(string response)
	{
		var responseObject = Json.Deserialize(response) as Dictionary<string, object>;
		object friendsH;
		var friends = new List<object>();
		if (responseObject.TryGetValue("friends", out friendsH))
		{
			friends = (List<object>)(((Dictionary<string, object>)friendsH)["data"]);
		}
		return friends;
	}
}
