using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptObjDestructible : MonoBehaviour {

	Rigidbody myRb;

	bool starting = true;

	[SerializeField] bool myGrav;
	[SerializeField] float myMass;	

	private void Awake() {
		myRb = GetComponent<Rigidbody>();		
			//myGrav = myRb.useGravity;
			myMass = myRb.mass;		
			starting = false;			
			DestroyImmediate (this.gameObject.GetComponent<Rigidbody>());				
			this.enabled = false;			
	}

	// Use this for initialization
	void Start () {
		
			myRb = GetComponent<Rigidbody>();
			myRb.useGravity = myGrav;
			myRb.mass = myMass;
			StartCoroutine(stopingRb());
			
	}
	

	IEnumerator stopingRb(){
		yield return new WaitForSeconds(4);
		DestroyImmediate (this.gameObject.GetComponent<Rigidbody>());
		this.enabled = false;

		yield return null;

	}

}
