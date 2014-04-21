using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject player;
	private Vector3 offset;

	private float minFov = 15f;
	private float maxFov = 90f;
	private float sensitivity = 10f;

	// Use this for initialization
	void Start () {
		offset = transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = player.transform.position + offset;
	}

	void Update(){
		float fov = Camera.main.fieldOfView;
		fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
		fov = Mathf.Clamp(fov, minFov, maxFov);
		Camera.main.fieldOfView = fov;
	}

}
