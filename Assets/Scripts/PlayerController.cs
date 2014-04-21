using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public int speedMultiplier;
	public float verticalThrust;
	public ParticleSystem winningHalo;
	private int count;
	private int totalCount = 0;
	private float time = 0;

	void Start(){
		count = 0;
		var gos = GameObject.FindGameObjectsWithTag("PickUp");
		totalCount = gos.Length;
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
		count++;
		if(count == totalCount)
			winningHalo.Play();
	}

	void OnGUI(){
		var style = new GUIStyle("label");
		style.fontSize = 22;
		GUI.color = Color.white;
		Rect rect = new Rect(12 , 10, 275, 40);
		if(count != totalCount)
			time = Time.timeSinceLevelLoad;
		//else {
			// send score to backend
		//}
		GUI.Label(rect, "Score: " + count.ToString() + "/"+ totalCount.ToString() + " Time: " + time.ToString("n2"), style);
	}
}
