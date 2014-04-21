using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public int speedMultiplier;
	public float verticalThrust;
	public ParticleSystem winningHalo;
	private int count;
	private const int totalCount = 22;

	void Start(){
		count = 0;
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

	void OnCollisionEnter(Collision collision) {
		var obj = collision.collider.gameObject;
		if(obj.tag == "PickUp"){
			count++;
			if(count == totalCount)
				winningHalo.Play();
		}
	}
}
