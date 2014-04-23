using UnityEngine;
using System.Collections;
using KiiCorp.Cloud.Storage;
using System;

public class PlayerController : MonoBehaviour {

	public int speedMultiplier;
	public float verticalThrust;
	public ParticleSystem winningHalo;
	private static int pickupCount;
	private static int totalPickupCount = 0;
	private static float gameTime = 0;
	public static string appScopeScoreBucket = "global_scores";
	private bool scoreSending = false;
	private bool scoreSent = false;
	private bool highscoresFetching = false;
	private KiiQueryResult<KiiObject> scores;

	void Start() {
		gameTime = 0;
		pickupCount = 0;
		var gos = GameObject.FindGameObjectsWithTag("PickUp");
		totalPickupCount = gos.Length;
		//scoreSending = false;
		//scoreSent = false;
		//highscoresFetching = false;
	}

	void Update() {
		if(transform.position.y < 0.0f)
			Application.LoadLevel("Ball");
	}

	void FixedUpdate () {
		// Process player input
		float vertical = Input.GetAxis("Vertical");
		float horizontal = Input.GetAxis("Horizontal");
		// Apply force to sphere
		Vector3 movement = new Vector3(horizontal, Input.GetButtonDown("Fire1") ? verticalThrust : 0.0f, vertical);
		rigidbody.AddForce(movement * speedMultiplier * Time.deltaTime);
	}

	/*void OnCollisionEnter(Collision collision) {
		var obj = collision.collider.gameObject;
		if(obj.tag == "PickUp")
			IncrementScore();
	}*/

	public static bool GameFinished () {
		return totalPickupCount == pickupCount;
	}

	void IncrementScore() {
		pickupCount++;
		if(GameFinished ()){
			SendScore ();
			winningHalo.Play();
		}
	}

	public static float GameTime() {
		return gameTime;
	}

	void OnGUI() {
		var style = new GUIStyle("Label");
		style.fontSize = 22;
		GUI.color = Color.white;
		Rect rect = new Rect(12 , 10, 300, 40);
		if(!GameFinished())
			gameTime = Time.timeSinceLevelLoad;
		else {
			// send score to backend
			//if(KiiUser.CurrentUser != null && !scoreSending && !scoreSent) {
				//SendScore ();
			//}
		}
		if(KiiUser.CurrentUser != null) {
			Rect rect2 = new Rect(12 , 40, 200, 40);
			GUI.Label(rect2, "User: " + KiiUser.CurrentUser.Displayname, style);
		}
		GUI.Label(rect, "Score: " + pickupCount.ToString() + "/"+ totalPickupCount.ToString() + " Time: " + gameTime.ToString("n2"), style);
		if(KiiUser.CurrentUser == null || !GameFinished())
			return;
		if(scores == null && !highscoresFetching && !scoreSending)
			GetTop10Highscores();
		else {
			// Make a group on the center of the screen
			GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 350));
			// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.
			
			// We'll make a box so you can see where the group is on-screen.
			GUI.Box (new Rect (0,0,200,350), "Your Time: " + GameTime().ToString("n2"));
			if(GUI.Button (new Rect (60,35,80,30), "Try Again"))
				Application.LoadLevel("Ball");
			if(scores != null) {
				GUI.Label(new Rect (55,80,100,30), "HIGHSCORES");
				int position = 120;
				foreach (KiiObject obj in scores) {
					double highscore = obj.GetDouble ("time");
					string username = obj.GetString ("username");
					GUI.Label(new Rect(20, position, 110, 50), Truncate(username, 15));
					GUI.Label(new Rect(150, position, 40, 50), highscore.ToString("n2"));
					position += 22;
				}
			} else {
				GUI.Label(new Rect (60,80,100,30), "Loading...");
			}
			// End the group we started above. This is very important to remember!
			GUI.EndGroup ();
		}
	}

	void SendScore() {
		scoreSending = true;
		scoreSent = false;
		KiiBucket bucket = Kii.Bucket(appScopeScoreBucket);
		KiiObject score = bucket.NewKiiObject();
		score["time"] = gameTime;
		score["username"] = KiiUser.CurrentUser.Displayname;
		// score is game completion time, the lower the better
		Util.Log ("Saving score to Kii Cloud...");
		score.Save((KiiObject obj, Exception e) => {
			if (e != null) {
				scoreSent = true;
				scoreSending = false;
				Util.LogError(e.ToString());
			}
			else {
				scoreSending = false;
				scoreSent = false;
				Util.Log("Score sent to Kii Cloud: " + gameTime.ToString("n2"));
			}
		});
	}

	void GetTop10Highscores() {
		highscoresFetching = true;
		KiiBucket bucket = Kii.Bucket (appScopeScoreBucket);
		KiiQuery query = new KiiQuery ();
		query.SortByAsc ("time");
		query.Limit = 10;
		bucket.Query(query, (KiiQueryResult<KiiObject> list, Exception e) =>{
			if (e != null) {
				highscoresFetching = false;
				scores = null;
				Util.LogError ("Failed to load high scores " + e.ToString());
			} else {
				highscoresFetching = false;
				scores = list;
			}
		});
	}

	public string Truncate(string source, int length) {
		if (source.Length > length)
			source = source.Substring(0, length);
		return source;
	}
}
