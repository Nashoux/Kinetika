using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorExploded : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "key") 
		{
			Debug.Log ("Collided");
			gameObject.GetComponent<Rigidbody> ().AddExplosionForce (150f, Vector3.up, 180);
		}
	}
}
