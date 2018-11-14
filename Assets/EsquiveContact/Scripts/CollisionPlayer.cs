using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPlayer : MonoBehaviour {
	
	private float life = 3;
	private float lifeSeuil1 = 1;
	private float lifeSeuil2 = 2;
	private float maxLife = 3;

	private float fade = 0f;
	private float regenAmount = 1;
	private float timeToRegen = 0;
	private float timeRoRegenValue = 5; // in seconds

	public GameObject blood;

	// Use this for initialization
	void Start () {
		InvokeRepeating("reduceTimer", 0f, 1f);
		InvokeRepeating("fadeUI", 0f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		if(life >= maxLife){
			fade = 0f;
		}

		if (timeToRegen > timeRoRegenValue){
			timeToRegen = timeRoRegenValue;
		}
		Debug.Log("fade "+fade);
		Debug.Log("life "+life);
	}

	void OnCollisionEnter(Collision collision){
		life -= 1;
		timeToRegen += timeRoRegenValue;

		if(life < maxLife && life >lifeSeuil1){
			fade = 0.5f;
			blood.GetComponent<CanvasGroup>().alpha = fade;
		}
		else if(life < lifeSeuil2 && life > 0){
			fade = 0.8f;
			blood.GetComponent<CanvasGroup>().alpha = fade;
		}
		else if(life <= 0){
			Debug.Log("you died");
		}

		Destroy(collision.rigidbody.gameObject);
	}

	private void reduceTimer(){
		if(timeToRegen > 0){
			timeToRegen -= 1;
		}
		if(timeToRegen <= 0 && life < maxLife){
			life += regenAmount;
		}
	}

	private void fadeUI(){
		if(timeToRegen > 0){
			blood.GetComponent<CanvasGroup>().alpha = fade;
			fade -= 0.05f;
		}
		if(timeToRegen <= 0){
			blood.GetComponent<CanvasGroup>().alpha = 0f;
		}
	}
}

