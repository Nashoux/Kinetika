using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiaterOrange : MonoBehaviour {

	public GameObject ObjetMobile;

	void Start () 
	{
		
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnTriggerStay (Collider col)
	{
		if (col.tag == "Player") {
			if (Input.GetKeyDown (KeyCode.R)) {
				GameObject ObjetMobilePref = Instantiate (ObjetMobile, 
				 new  Vector3 (transform.position.x, 
				transform.position.y + 5, 
				transform.position.z), Quaternion.identity);
			}
		}
	}


}
