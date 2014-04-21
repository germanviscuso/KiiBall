using UnityEngine;
using System.Collections;
using KiiCorp.Cloud.Storage;
using System;

public class PlayerController : MonoBehaviour {

	public int speedMultiplier;
	public float verticalThrust;
	public ParticleSystem winningHalo;
	private int pickupCount;
	private int totalPickupCount = 0;
	private float gameTime = 0;
	public static string appScopeScoreBucket = "global_scores";
	private bool scoreSent = false;

	void Start(){
		gameTime = 0;
		pickupCount = 0;
		var gos = GameObject.FindGameObjectsWithTag("PickUp");
		totalPickupCount = gos.Length;
	}

	void Update(){
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

	void IncrementScore(){
		pickupCount++;
		if(pickupCount == totalPickupCount)
			winningHalo.Play();
	}

	void OnGUI(){
		var style = new GUIStyle("Label");
		style.fontSize = 22;
		GUI.color = Color.white;
		Rect rect = new Rect(12 , 10, 275, 40);
		if(pickupCount != totalPickupCount)
			gameTime = Time.timeSinceLevelLoad;
		else {
			// send score to backend
			if(KiiUser.CurrentUser != null && !scoreSent){
				scoreSent = true;
				SendScore ();
			}
		}
		GUI.Label(rect, "Score: " + pickupCount.ToString() + "/"+ totalPickupCount.ToString() + " Time: " + gameTime.ToString("n2"), style);
	}

	void SendScore(){
		KiiBucket bucket = Kii.Bucket(appScopeScoreBucket);
		KiiObject score = bucket.NewKiiObject();
		score["time"] = gameTime;
		score["username"] = KiiUser.CurrentUser.Username;
		// score is game completion time, the lower the better
		Util.Log ("Saving score to Kii Cloud...");
		score.Save((KiiObject obj, Exception e) => {
			if (e != null)
				Util.LogError(e.ToString());
			else
				Util.Log("Score sent to Kii Cloud: " + gameTime.ToString("n2"));
		});
	}
}
