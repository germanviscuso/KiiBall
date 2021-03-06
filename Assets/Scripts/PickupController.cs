﻿using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {
	
	public ParticleSystem explosion;
	public float explosionRadius = 1.0F;
	public float explosionPower = 200.0F;
	public float explosionUpwardsModifier = 0.0F;
	public bool explosionChainReaction = false;
	private bool exploded = false;
	private GameObject player;

	void Start(){
		player = GameObject.Find("player");
	}

	void OnCollisionEnter(Collision collision) {
		var obj = collision.collider.gameObject;
		if(obj.tag == "Player"){
			Explode ();
		}
	}

	void Explode(){
		if(exploded)
			return;
		exploded = true;
		player.SendMessage("IncrementScore");
		// Explosion particle animation
		explosion.Play();
		// Explosion sound
		AudioSource.PlayClipAtPoint(gameObject.audio.clip, gameObject.transform.position, 0.25f);
		// Explosion shockwave
		Vector3 explosionPos = gameObject.transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
		foreach (Collider hit in colliders) {
			if (hit && hit.rigidbody){
				hit.rigidbody.AddExplosionForce(explosionPower, explosionPos, explosionRadius, explosionUpwardsModifier, ForceMode.Force);
				if(hit.gameObject.tag == "PickUp" && explosionChainReaction){
					hit.gameObject.SendMessage("Explode");
				}
			}
		}
		// Vanish Pickup
		renderer.enabled = false;
		collider.enabled = false;
		rigidbody.isKinematic = true;
		Destroy (gameObject, 1.25F);
	}

}
