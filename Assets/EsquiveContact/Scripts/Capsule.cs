using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour {

	public Rigidbody projectile;

	// Use this for initialization
	void Start () {
		 InvokeRepeating("LaunchProjectile", 2.0f, 1.5f);
	}

	void LaunchProjectile() {
		Rigidbody instance = Instantiate(projectile, new Vector3(Random.Range(-1f, 1f)*0.5f, 1, 9),Quaternion.identity);
		instance.velocity = new Vector3(0,0, -5);
		Destroy(instance.gameObject, 10f);
	}
}
